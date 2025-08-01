// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//
// codeman.cpp - a managment class for handling multiple code managers
//

#include "common.h"
#include "jitinterface.h"
#include "corjit.h"
#include "jithost.h"
#include "eetwain.h"
#include "eeconfig.h"
#include "excep.h"
#include "appdomain.hpp"
#include "codeman.h"
#include "nibblemapmacros.h"
#include "generics.h"
#include "dynamicmethod.h"
#include "eventtrace.h"
#include "threadsuspend.h"
#include "daccess.h"

#include "exceptionhandling.h"

#include "rtlfunctions.h"

#include "debuginfostore.h"
#include "strsafe.h"

#include "configuration.h"

#include <minipal/cpufeatures.h>
#include <minipal/cpuid.h>

#ifdef HOST_64BIT
#define CHECK_DUPLICATED_STRUCT_LAYOUTS
#include "../debug/daccess/fntableaccess.h"
#endif // HOST_64BIT

#ifdef FEATURE_PERFMAP
#include "perfmap.h"
#endif

// Default number of jump stubs in a jump stub block
#define DEFAULT_JUMPSTUBS_PER_BLOCK  32

SPTR_IMPL(EECodeManager, ExecutionManager, m_pDefaultCodeMan);

SPTR_IMPL(EEJitManager, ExecutionManager, m_pEEJitManager);
#ifdef FEATURE_READYTORUN
SPTR_IMPL(ReadyToRunJitManager, ExecutionManager, m_pReadyToRunJitManager);
#endif

#ifdef FEATURE_INTERPRETER
SPTR_IMPL(InterpreterJitManager, ExecutionManager, m_pInterpreterJitManager);
SPTR_IMPL(InterpreterCodeManager, ExecutionManager, m_pInterpreterCodeMan);
#endif

SVAL_IMPL(RangeSectionMapData, ExecutionManager, g_codeRangeMap);
VOLATILE_SVAL_IMPL_INIT(LONG, ExecutionManager, m_dwReaderCount, 0);
VOLATILE_SVAL_IMPL_INIT(LONG, ExecutionManager, m_dwWriterLock, 0);

#ifndef DACCESS_COMPILE

CrstStatic ExecutionManager::m_JumpStubCrst;

unsigned   ExecutionManager::m_normal_JumpStubLookup;
unsigned   ExecutionManager::m_normal_JumpStubUnique;
unsigned   ExecutionManager::m_normal_JumpStubBlockAllocCount;
unsigned   ExecutionManager::m_normal_JumpStubBlockFullCount;

unsigned   ExecutionManager::m_LCG_JumpStubLookup;
unsigned   ExecutionManager::m_LCG_JumpStubUnique;
unsigned   ExecutionManager::m_LCG_JumpStubBlockAllocCount;
unsigned   ExecutionManager::m_LCG_JumpStubBlockFullCount;

#endif // DACCESS_COMPILE

#if defined(TARGET_AMD64) && defined(TARGET_WINDOWS) && !defined(DACCESS_COMPILE)

// Support for new style unwind information (to allow OS to stack crawl JIT compiled code).

typedef NTSTATUS (WINAPI* RtlAddGrowableFunctionTableFnPtr) (
        PVOID *DynamicTable, PRUNTIME_FUNCTION FunctionTable, ULONG EntryCount,
        ULONG MaximumEntryCount, ULONG_PTR rangeStart, ULONG_PTR rangeEnd);
typedef VOID (WINAPI* RtlGrowFunctionTableFnPtr) (PVOID DynamicTable, ULONG NewEntryCount);
typedef VOID (WINAPI* RtlDeleteGrowableFunctionTableFnPtr) (PVOID DynamicTable);

// OS entry points (only exist on Win8 and above)
static RtlAddGrowableFunctionTableFnPtr pRtlAddGrowableFunctionTable;
static RtlGrowFunctionTableFnPtr pRtlGrowFunctionTable;
static RtlDeleteGrowableFunctionTableFnPtr pRtlDeleteGrowableFunctionTable;

static bool s_publishingActive;         // Publishing to ETW is turned on
static Crst* s_pUnwindInfoTableLock;    // lock protects all public UnwindInfoTable functions

/****************************************************************************/
// initialize the entry points for new win8 unwind info publishing functions.
// return true if the initialize is successful (the functions exist)
bool InitUnwindFtns()
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
    }
    CONTRACTL_END;

    HINSTANCE hNtdll = GetModuleHandle(W("ntdll.dll"));
    if (hNtdll != NULL)
    {
        void* growFunctionTable = GetProcAddress(hNtdll, "RtlGrowFunctionTable");
        void* deleteGrowableFunctionTable = GetProcAddress(hNtdll, "RtlDeleteGrowableFunctionTable");
        void* addGrowableFunctionTable = GetProcAddress(hNtdll, "RtlAddGrowableFunctionTable");

        // All or nothing AddGroableFunctionTable is last (marker)
        if (growFunctionTable != NULL &&
            deleteGrowableFunctionTable != NULL &&
            addGrowableFunctionTable != NULL)
        {
            pRtlGrowFunctionTable = (RtlGrowFunctionTableFnPtr) growFunctionTable;
            pRtlDeleteGrowableFunctionTable = (RtlDeleteGrowableFunctionTableFnPtr) deleteGrowableFunctionTable;
            pRtlAddGrowableFunctionTable = (RtlAddGrowableFunctionTableFnPtr) addGrowableFunctionTable;
        }
        // Don't call FreeLibrary(hNtdll) because GetModuleHandle did *NOT* increment the reference count!
    }

    return (pRtlAddGrowableFunctionTable != NULL);
}

/****************************************************************************/
UnwindInfoTable::UnwindInfoTable(ULONG_PTR rangeStart, ULONG_PTR rangeEnd, ULONG size)
{
    STANDARD_VM_CONTRACT;
    _ASSERTE(s_pUnwindInfoTableLock->OwnedByCurrentThread());
    _ASSERTE((rangeEnd - rangeStart) <= 0x7FFFFFFF);

    cTableCurCount = 0;
    cTableMaxCount = size;
    cDeletedEntries = 0;
    iRangeStart = rangeStart;
    iRangeEnd = rangeEnd;
    hHandle = NULL;
    pTable = new T_RUNTIME_FUNCTION[cTableMaxCount];
}

/****************************************************************************/
UnwindInfoTable::~UnwindInfoTable()
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
    } CONTRACTL_END;
    _ASSERTE(s_publishingActive);

    // We do this lock free to because too many places still want no-trigger.   It should be OK
    // It would be cleaner if we could take the lock (we did not have to be GC_NOTRIGGER)
    UnRegister();
    delete[] pTable;
}

/*****************************************************************************/
void UnwindInfoTable::Register()
{
    _ASSERTE(s_pUnwindInfoTableLock->OwnedByCurrentThread());
    EX_TRY
    {
        hHandle = NULL;
        NTSTATUS ret = pRtlAddGrowableFunctionTable(&hHandle, pTable, cTableCurCount, cTableMaxCount, iRangeStart, iRangeEnd);
        if (ret != STATUS_SUCCESS)
        {
            _ASSERTE(!"Failed to publish UnwindInfo (ignorable)");
            hHandle = NULL;
            STRESS_LOG3(LF_JIT, LL_ERROR, "UnwindInfoTable::Register ERROR %x creating table [%p, %p]\n", ret, iRangeStart, iRangeEnd);
        }
        else
        {
            STRESS_LOG3(LF_JIT, LL_INFO100, "UnwindInfoTable::Register Handle: %p [%p, %p]\n", hHandle, iRangeStart, iRangeEnd);
        }
    }
    EX_CATCH
    {
        hHandle = NULL;
        STRESS_LOG2(LF_JIT, LL_ERROR, "UnwindInfoTable::Register Exception while creating table [%p, %p]\n",
            iRangeStart, iRangeEnd);
        _ASSERTE(!"Failed to publish UnwindInfo (ignorable)");
    }
    EX_END_CATCH
}

/*****************************************************************************/
void UnwindInfoTable::UnRegister()
{
    PVOID handle = hHandle;
    hHandle = 0;
    if (handle != 0)
    {
        STRESS_LOG3(LF_JIT, LL_INFO100, "UnwindInfoTable::UnRegister Handle: %p [%p, %p]\n", handle, iRangeStart, iRangeEnd);
        pRtlDeleteGrowableFunctionTable(handle);
    }
}

/*****************************************************************************/
// Add 'data' to the linked list whose head is pointed at by 'unwindInfoPtr'
//
/* static */
void UnwindInfoTable::AddToUnwindInfoTable(UnwindInfoTable** unwindInfoPtr, PT_RUNTIME_FUNCTION data,
                                          TADDR rangeStart, TADDR rangeEnd)
{
    CONTRACTL
    {
        THROWS;
        GC_TRIGGERS;
    }
    CONTRACTL_END;
    _ASSERTE(data->BeginAddress <= RUNTIME_FUNCTION__EndAddress(data, rangeStart));
    _ASSERTE(RUNTIME_FUNCTION__EndAddress(data, rangeStart) <=  (rangeEnd-rangeStart));
    _ASSERTE(unwindInfoPtr != NULL);

    if (!s_publishingActive)
        return;

    CrstHolder ch(s_pUnwindInfoTableLock);

    UnwindInfoTable* unwindInfo = *unwindInfoPtr;
    // was the original list null, If so lazy initialize.
    if (unwindInfo == NULL)
    {
        // We can choose the average method size estimate dynamically based on past experience
        // 128 is the estimated size of an average method, so we can accurately predict
        // how many RUNTIME_FUNCTION entries are in each chunk we allocate.

        ULONG size = (ULONG) ((rangeEnd - rangeStart) / 128) + 1;

        // To ensure the test the growing logic in debug code make the size much smaller.
        INDEBUG(size = size / 4 + 1);
        unwindInfo = (PTR_UnwindInfoTable)new UnwindInfoTable(rangeStart, rangeEnd, size);
        unwindInfo->Register();
        *unwindInfoPtr = unwindInfo;
    }
    _ASSERTE(unwindInfo != NULL);        // If new had failed, we would have thrown OOM
    _ASSERTE(unwindInfo->cTableCurCount <= unwindInfo->cTableMaxCount);
    _ASSERTE(unwindInfo->iRangeStart == rangeStart);
    _ASSERTE(unwindInfo->iRangeEnd == rangeEnd);

    // Means we had a failure publishing to the OS, in this case we give up
    if (unwindInfo->hHandle == NULL)
        return;

    // Check for the fast path: we are adding the end of an UnwindInfoTable with space
    if (unwindInfo->cTableCurCount < unwindInfo->cTableMaxCount)
    {
        if (unwindInfo->cTableCurCount == 0 ||
            unwindInfo->pTable[unwindInfo->cTableCurCount-1].BeginAddress < data->BeginAddress)
        {
            // Yeah, we can simply add to the end of table and we are done!
            unwindInfo->pTable[unwindInfo->cTableCurCount] = *data;
            unwindInfo->cTableCurCount++;

            // Add to the function table
            pRtlGrowFunctionTable(unwindInfo->hHandle, unwindInfo->cTableCurCount);

            STRESS_LOG5(LF_JIT, LL_INFO1000, "AddToUnwindTable Handle: %p [%p, %p] ADDING 0x%p TO END, now 0x%x entries\n",
                unwindInfo->hHandle, unwindInfo->iRangeStart, unwindInfo->iRangeEnd,
                data->BeginAddress, unwindInfo->cTableCurCount);
            return;
        }
    }

    // OK we need to rellocate the table and reregister.  First figure out our 'desiredSpace'
    // We could imagine being much more efficient for 'bulk' updates, but we don't try
    // because we assume that this is rare and we want to keep the code simple

    ULONG usedSpace = unwindInfo->cTableCurCount - unwindInfo->cDeletedEntries;
    ULONG desiredSpace = usedSpace * 5 / 4 + 1;        // Increase by 20%
    // Be more aggressive if we used all of our space;
    if (usedSpace == unwindInfo->cTableMaxCount)
        desiredSpace = usedSpace * 3 / 2 + 1;        // Increase by 50%

    STRESS_LOG7(LF_JIT, LL_INFO100, "AddToUnwindTable Handle: %p [%p, %p] SLOW Realloc Cnt 0x%x Max 0x%x NewMax 0x%x, Adding %x\n",
        unwindInfo->hHandle, unwindInfo->iRangeStart, unwindInfo->iRangeEnd,
        unwindInfo->cTableCurCount, unwindInfo->cTableMaxCount, desiredSpace, data->BeginAddress);

    UnwindInfoTable* newTab = new UnwindInfoTable(unwindInfo->iRangeStart, unwindInfo->iRangeEnd, desiredSpace);

    // Copy in the entries, removing deleted entries and adding the new entry wherever it belongs
    int toIdx = 0;
    bool inserted = false;    // Have we inserted 'data' into the table
    for(ULONG fromIdx = 0; fromIdx < unwindInfo->cTableCurCount; fromIdx++)
    {
        if (!inserted && data->BeginAddress < unwindInfo->pTable[fromIdx].BeginAddress)
        {
            STRESS_LOG1(LF_JIT, LL_INFO100, "AddToUnwindTable Inserted at MID position 0x%x\n", toIdx);
            newTab->pTable[toIdx++] = *data;
            inserted = true;
        }
        if (unwindInfo->pTable[fromIdx].UnwindData != 0)	// A 'non-deleted' entry
            newTab->pTable[toIdx++] = unwindInfo->pTable[fromIdx];
    }
    if (!inserted)
    {
        STRESS_LOG1(LF_JIT, LL_INFO100, "AddToUnwindTable Inserted at END position 0x%x\n", toIdx);
        newTab->pTable[toIdx++] = *data;
    }
    newTab->cTableCurCount = toIdx;
    STRESS_LOG2(LF_JIT, LL_INFO100, "AddToUnwindTable New size 0x%x max 0x%x\n",
        newTab->cTableCurCount, newTab->cTableMaxCount);
    _ASSERTE(newTab->cTableCurCount <= newTab->cTableMaxCount);

    // Unregister the old table
    *unwindInfoPtr = 0;
    unwindInfo->UnRegister();

    // Note that there is a short time when we are not publishing...

    // Register the new table
    newTab->Register();
    *unwindInfoPtr = newTab;

    delete unwindInfo;
}

/*****************************************************************************/
/* static */ void UnwindInfoTable::RemoveFromUnwindInfoTable(UnwindInfoTable** unwindInfoPtr, TADDR baseAddress, TADDR entryPoint)
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
    }
    CONTRACTL_END;
    _ASSERTE(unwindInfoPtr != NULL);

    if (!s_publishingActive)
        return;
    CrstHolder ch(s_pUnwindInfoTableLock);

    UnwindInfoTable* unwindInfo = *unwindInfoPtr;
    if (unwindInfo != NULL)
    {
        DWORD relativeEntryPoint = (DWORD)(entryPoint - baseAddress);
        STRESS_LOG3(LF_JIT, LL_INFO100, "RemoveFromUnwindInfoTable Removing %p BaseAddress %p rel %x\n",
            entryPoint, baseAddress, relativeEntryPoint);
        for(ULONG i = 0; i < unwindInfo->cTableCurCount; i++)
        {
            if (unwindInfo->pTable[i].BeginAddress <= relativeEntryPoint &&
                relativeEntryPoint < RUNTIME_FUNCTION__EndAddress(&unwindInfo->pTable[i], unwindInfo->iRangeStart))
            {
                if (unwindInfo->pTable[i].UnwindData != 0)
                    unwindInfo->cDeletedEntries++;
                unwindInfo->pTable[i].UnwindData = 0;        // Mark the entry for deletion
                STRESS_LOG1(LF_JIT, LL_INFO100, "RemoveFromUnwindInfoTable Removed entry 0x%x\n", i);
                return;
            }
        }
    }
    STRESS_LOG2(LF_JIT, LL_WARNING, "RemoveFromUnwindInfoTable COULD NOT FIND %p BaseAddress %p\n",
        entryPoint, baseAddress);
}

/****************************************************************************/
// Publish the stack unwind data 'data' which is relative 'baseAddress'
// to the operating system in a way ETW stack tracing can use.

/* static */ void UnwindInfoTable::PublishUnwindInfoForMethod(TADDR baseAddress, PT_RUNTIME_FUNCTION unwindInfo, int unwindInfoCount)
{
    STANDARD_VM_CONTRACT;
    if (!s_publishingActive)
        return;

    TADDR entry = baseAddress + unwindInfo->BeginAddress;
    RangeSection * pRS = ExecutionManager::FindCodeRange(entry, ExecutionManager::GetScanFlags());
    _ASSERTE(pRS != NULL);
    if (pRS != NULL)
    {
        for(int i = 0; i < unwindInfoCount; i++)
            AddToUnwindInfoTable(&pRS->_pUnwindInfoTable, &unwindInfo[i], pRS->_range.RangeStart(), pRS->_range.RangeEndOpen());
    }
}

/*****************************************************************************/
/* static */ void UnwindInfoTable::UnpublishUnwindInfoForMethod(TADDR entryPoint)
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
    }
    CONTRACTL_END;

    if (!s_publishingActive)
        return;

    RangeSection * pRS = ExecutionManager::FindCodeRange(entryPoint, ExecutionManager::GetScanFlags());
    _ASSERTE(pRS != NULL);
    if (pRS != NULL)
    {
        _ASSERTE(pRS->_pjit->GetCodeType() == (miManaged | miIL));
        if (pRS->_pjit->GetCodeType() == (miManaged | miIL))
        {
            // This cast is justified because only EEJitManager's have the code type above.
            EEJitManager* pJitMgr = (EEJitManager*)(pRS->_pjit);
            CodeHeader * pHeader = pJitMgr->GetCodeHeaderFromStartAddress(entryPoint);
            for(ULONG i = 0; i < pHeader->GetNumberOfUnwindInfos(); i++)
                RemoveFromUnwindInfoTable(&pRS->_pUnwindInfoTable, pRS->_range.RangeStart(), pRS->_range.RangeStart() + pHeader->GetUnwindInfo(i)->BeginAddress);
        }
    }
}

/*****************************************************************************/
// We only do this on Windows x64 (other platforms use frame-based stack crawling),
// We want good stack traces so we need to publish unwind information so ETW can
// walk the stack.
/* static */ void UnwindInfoTable::Initialize()
{
    CONTRACTL
    {
        THROWS;
        GC_NOTRIGGER;
    }
    CONTRACTL_END;

    _ASSERTE(!s_publishingActive);

    // If we don't have the APIs we need, give up
    if (!InitUnwindFtns())
        return;

    // Create the lock
    s_pUnwindInfoTableLock = new Crst(CrstUnwindInfoTableLock);
    s_publishingActive = true;
}

#else
/* static */ void UnwindInfoTable::PublishUnwindInfoForMethod(TADDR baseAddress, T_RUNTIME_FUNCTION* unwindInfo, int unwindInfoCount)
{
    LIMITED_METHOD_CONTRACT;
}

/* static */ void UnwindInfoTable::UnpublishUnwindInfoForMethod(TADDR entryPoint)
{
    LIMITED_METHOD_CONTRACT;
}

/* static */ void UnwindInfoTable::Initialize()
{
    LIMITED_METHOD_CONTRACT;
}

#endif // defined(TARGET_AMD64) && defined(TARGET_WINDOWS) && !defined(DACCESS_COMPILE)

#if !defined(DACCESS_COMPILE)
CodeHeapIterator::CodeHeapIterator(EECodeGenManager* manager, HeapList* heapList, LoaderAllocator* pLoaderAllocatorFilter)
    : m_manager(manager)
    , m_Iterator{}
    , m_Heaps{}
    , m_HeapsIndexNext{ 0 }
    , m_pLoaderAllocatorFilter{ pLoaderAllocatorFilter }
    , m_pCurrent{ NULL }
{
    CONTRACTL
    {
        THROWS;
        GC_NOTRIGGER;
        PRECONDITION(m_manager != NULL);
    }
    CONTRACTL_END;

    // Iterator through the heap list collecting the current state of the heaps.
    HeapList* current = heapList;
    while (current)
    {
        HeapListState* state = m_Heaps.AppendThrowing();
        state->Heap = current;
        state->MapBase = (void*)current->mapBase;
        state->HdrMap = current->pHdrMap;
        state->MaxCodeHeapSize = current->maxCodeHeapSize;

        current = current->GetNext();
    }

    // Move to the first method section.
    (void)NextMethodSectionIterator();

    m_manager->AddRefIterator();
}

CodeHeapIterator::~CodeHeapIterator()
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
    }
    CONTRACTL_END;

    m_manager->ReleaseIterator();
}

bool CodeHeapIterator::Next()
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
    }
    CONTRACTL_END;

    while (true)
    {
        if (!m_Iterator.Next())
        {
            if (!NextMethodSectionIterator())
                return false;
        }
        else
        {
            BYTE* code = m_Iterator.GetMethodCode();
            CodeHeader* pHdr = (CodeHeader*)(code - sizeof(CodeHeader));
            m_pCurrent = !pHdr->IsStubCodeBlock() ? pHdr->GetMethodDesc() : NULL;

            // LoaderAllocator filter
            if (m_pLoaderAllocatorFilter && m_pCurrent)
            {
                LoaderAllocator *pCurrentLoaderAllocator = m_pCurrent->GetLoaderAllocator();
                if (pCurrentLoaderAllocator != m_pLoaderAllocatorFilter)
                    continue;
            }

            return true;
        }
    }
}

bool CodeHeapIterator::NextMethodSectionIterator()
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
    }
    CONTRACTL_END;

    if (m_HeapsIndexNext >= m_Heaps.Count())
    {
        m_Iterator = {};
        return false;
    }

    HeapListState& curr = m_Heaps.Table()[m_HeapsIndexNext++];
    m_Iterator = MethodSectionIterator{
        curr.MapBase,
        (COUNT_T)curr.MaxCodeHeapSize,
        curr.HdrMap,
        (COUNT_T)HEAP2MAPSIZE(ROUND_UP_TO_PAGE(curr.MaxCodeHeapSize))};
    return true;
}
#endif // !DACCESS_COMPILE

#ifndef DACCESS_COMPILE

//---------------------------------------------------------------------------------------
//
// ReaderLockHolder::ReaderLockHolder takes the reader lock, checks for the writer lock
// and either aborts if the writer lock is held, or yields until the writer lock is released,
// keeping the reader lock.  This is normally called in the constructor for the
// ReaderLockHolder.
//
// The writer cannot be taken if there are any readers. The WriterLockHolder functions take the
// writer lock and check for any readers. If there are any, the WriterLockHolder functions
// release the writer and yield to wait for the readers to be done.

ExecutionManager::ReaderLockHolder::ReaderLockHolder()
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        CAN_TAKE_LOCK;
    } CONTRACTL_END;

    IncCantAllocCount();

    InterlockedIncrement(&m_dwReaderCount);

    EE_LOCK_TAKEN(GetPtrForLockContract());

    if (VolatileLoad(&m_dwWriterLock) != 0)
    {
        YIELD_WHILE ((VolatileLoad(&m_dwWriterLock) != 0));
    }
}

//---------------------------------------------------------------------------------------
//
// See code:ExecutionManager::ReaderLockHolder::ReaderLockHolder. This just decrements the reader count.

ExecutionManager::ReaderLockHolder::~ReaderLockHolder()
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        MODE_ANY;
    }
    CONTRACTL_END;

    InterlockedDecrement(&m_dwReaderCount);
    DecCantAllocCount();

    EE_LOCK_RELEASED(GetPtrForLockContract());
}

//---------------------------------------------------------------------------------------
//
// Returns whether the reader lock is acquired

BOOL ExecutionManager::ReaderLockHolder::Acquired()
{
    LIMITED_METHOD_CONTRACT;
    return VolatileLoad(&m_dwWriterLock) == 0;
}

ExecutionManager::WriterLockHolder::WriterLockHolder()
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        CAN_TAKE_LOCK;
    } CONTRACTL_END;

    _ASSERTE(m_dwWriterLock == 0);

    // Signal to a debugger that this thread cannot stop now
    IncCantStopCount();

    IncCantAllocCount();

    DWORD dwSwitchCount = 0;
    while (TRUE)
    {
        // While this thread holds the writer lock, we must not try to suspend it
        // or allow a profiler to walk its stack
        Thread::IncForbidSuspendThread();

        InterlockedIncrement(&m_dwWriterLock);
        if (m_dwReaderCount == 0)
            break;
        InterlockedDecrement(&m_dwWriterLock);

        // Before we loop and retry, it's safe to suspend or hijack and inspect
        // this thread
        Thread::DecForbidSuspendThread();

        __SwitchToThread(0, ++dwSwitchCount);
    }
    EE_LOCK_TAKEN(GetPtrForLockContract());
}

ExecutionManager::WriterLockHolder::~WriterLockHolder()
{
    LIMITED_METHOD_CONTRACT;

    InterlockedDecrement(&m_dwWriterLock);

    // Writer lock released, so it's safe again for this thread to be
    // suspended or have its stack walked by a profiler
    Thread::DecForbidSuspendThread();

    DecCantAllocCount();

    // Signal to a debugger that it's again safe to stop this thread
    DecCantStopCount();

    EE_LOCK_RELEASED(GetPtrForLockContract());
}

#else

// For DAC builds, we only care whether the writer lock is held.
// If it is, we will assume the locked data is in an inconsistent
// state and throw. We never actually take the lock.
// Note: Throws
ExecutionManager::ReaderLockHolder::ReaderLockHolder()
{
    SUPPORTS_DAC;

    if (m_dwWriterLock != 0)
    {
        ThrowHR(CORDBG_E_PROCESS_NOT_SYNCHRONIZED);
    }
}

ExecutionManager::ReaderLockHolder::~ReaderLockHolder()
{
}

#endif // DACCESS_COMPILE

/*-----------------------------------------------------------------------------
 This is a listing of which methods uses which synchronization mechanism
 in the ExecutionManager
//-----------------------------------------------------------------------------

==============================================================================
ExecutionManger::ReaderLockHolder and ExecutionManger::WriterLockHolder
Protects the callers of ExecutionManager::GetRangeSection from heap deletions
while walking RangeSections.  You need to take a reader lock before reading the
values: m_CodeRangeList and hold it while walking the lists

Uses ReaderLockHolder (allows multiple reeaders with no writers)
-----------------------------------------
ExecutionManager::FindCodeRange
ExecutionManager::FindReadyToRunModule
ExecutionManager::EnumMemoryRegions
AND
ExecutionManager::IsManagedCode
ExecutionManager::IsManagedCodeWithLock
The IsManagedCode checks are notable as they protect not just access to the RangeSection walking,
but the actual RangeSection while determining if a given ip IsManagedCode.

Uses WriterLockHolder (allows single writer and no readers)
-----------------------------------------
ExecutionManager::DeleteRange
*/

//-----------------------------------------------------------------------------

#if defined(TARGET_ARM) || defined(TARGET_ARM64) || defined(TARGET_LOONGARCH64) || defined(TARGET_RISCV64)
#define EXCEPTION_DATA_SUPPORTS_FUNCTION_FRAGMENTS
#endif

#if defined(EXCEPTION_DATA_SUPPORTS_FUNCTION_FRAGMENTS)
// The function fragments can be used in Hot/Cold splitting, expressing Large Functions or in 'ShrinkWrapping', which is
// delaying saving and restoring some callee-saved registers later inside the body of the method.
// (It's assumed that JIT will not emit any ShrinkWrapping-style methods)
// For these cases multiple RUNTIME_FUNCTION entries (a.k.a function fragments) are used to define
// all the regions of the function or funclet. And one of these function fragments cover the beginning of the function/funclet,
// including the prolog section and is referred as the 'Host Record'.
// This function returns TRUE if the inspected RUNTIME_FUNCTION entry is NOT a host record

BOOL IsFunctionFragment(TADDR baseAddress, PTR_RUNTIME_FUNCTION pFunctionEntry)
{
    LIMITED_METHOD_DAC_CONTRACT;

    _ASSERTE((pFunctionEntry->UnwindData & 3) == 0);   // The unwind data must be an RVA; we don't support packed unwind format
    DWORD unwindHeader = *(PTR_DWORD)(baseAddress + pFunctionEntry->UnwindData);
    _ASSERTE((0 == ((unwindHeader >> 18) & 3)) || !"unknown unwind data format, version != 0");
#if defined(TARGET_ARM)

    // On ARM, It's assumed that the prolog is always at the beginning of the function and cannot be split.
    // Given that, there are 4 possible ways to fragment a function:
    // 1. Prolog only:
    // 2. Prolog and some epilogs:
    // 3. Epilogs only:
    // 4. No Prolog or epilog
    //
    // Function fragments describing 1 & 2 are host records, 3 & 4 are not.
    // for 3 & 4, the .xdata record's F bit is set to 1, marking clearly what is NOT a host record

    _ASSERTE((pFunctionEntry->BeginAddress & THUMB_CODE) == THUMB_CODE);   // Sanity check: it's a thumb address
    DWORD Fbit = (unwindHeader >> 22) & 0x1;    // F "fragment" bit
    return (Fbit == 1);
#elif defined(TARGET_ARM64)

    // ARM64 is a little bit more flexible, in the sense that it supports partial prologs. However only one of the
    // prolog regions are allowed to alter SP and that's the Host Record. Partial prologs are used in ShrinkWrapping
    // scenarios which is not supported, hence we don't need to worry about them. discarding partial prologs
    // simplifies identifying a host record a lot.
    //
    // 1. Prolog only: The host record. Epilog Count and E bit are all 0.
    // 2. Prolog and some epilogs: The host record with accompanying epilog-only records
    // 3. Epilogs only: First unwind code is Phantom prolog (Starting with an end_c, indicating an empty prolog)
    // 4. No prologs or epilogs: First unwind code is Phantom prolog  (Starting with an end_c, indicating an empty prolog)
    //

    int EpilogCount = (int)(unwindHeader >> 22) & 0x1F;
    int CodeWords = unwindHeader >> 27;
    PTR_DWORD pUnwindCodes = (PTR_DWORD)(baseAddress + pFunctionEntry->UnwindData);
    // Skip header.
    pUnwindCodes++;

    // Skip extended header.
    if ((CodeWords == 0) && (EpilogCount == 0))
    {
        EpilogCount = (*pUnwindCodes) & 0xFFFF;
        pUnwindCodes++;
    }

    // Skip epilog scopes.
    BOOL Ebit = (unwindHeader >> 21) & 0x1;
    if (!Ebit && (EpilogCount != 0))
    {
        // EpilogCount is the number of exception scopes defined right after the unwindHeader
        pUnwindCodes += EpilogCount;
    }

    return ((*pUnwindCodes & 0xFF) == 0xE5);
#elif defined(TARGET_LOONGARCH64) || defined(TARGET_RISCV64)

    // LOONGARCH64 is a little bit more flexible, in the sense that it supports partial prologs. However only one of the
    // prolog regions are allowed to alter SP and that's the Host Record. Partial prologs are used in ShrinkWrapping
    // scenarios which is not supported, hence we don't need to worry about them. discarding partial prologs
    // simplifies identifying a host record a lot.
    //
    // 1. Prolog only: The host record. Epilog Count and E bit are all 0.
    // 2. Prolog and some epilogs: The host record with accompanying epilog-only records
    // 3. Epilogs only: First unwind code is Phantom prolog (Starting with an end_c, indicating an empty prolog)
    // 4. No prologs or epilogs: First unwind code is Phantom prolog  (Starting with an end_c, indicating an empty prolog)
    //

    int EpilogCount = (int)(unwindHeader >> 22) & 0x1F;
    int CodeWords = unwindHeader >> 27;
    PTR_DWORD pUnwindCodes = (PTR_DWORD)(baseAddress + pFunctionEntry->UnwindData);
    // Skip header.
    pUnwindCodes++;

    // Skip extended header.
    if ((CodeWords == 0) && (EpilogCount == 0))
    {
        EpilogCount = (*pUnwindCodes) & 0xFFFF;
        pUnwindCodes++;
    }

    // Skip epilog scopes.
    BOOL Ebit = (unwindHeader >> 21) & 0x1;
    if (!Ebit && (EpilogCount != 0))
    {
        // EpilogCount is the number of exception scopes defined right after the unwindHeader
        pUnwindCodes += EpilogCount;
    }

    return ((*pUnwindCodes & 0xFF) == 0xE5);
#else
    PORTABILITY_ASSERT("IsFunctionFragnent - NYI on this platform");
#endif
}

#endif // EXCEPTION_DATA_SUPPORTS_FUNCTION_FRAGMENTS


#ifndef DACCESS_COMPILE

//**********************************************************************************
//  IJitManager
//**********************************************************************************
IJitManager::IJitManager()
{
    LIMITED_METHOD_CONTRACT;

    m_runtimeSupport   = ExecutionManager::GetDefaultCodeManager();
}

#endif // #ifndef DACCESS_COMPILE

// When we unload an appdomain, we need to make sure that any threads that are crawling through
// our heap or rangelist are out. For cooperative-mode threads, we know that they will have
// been stopped when we suspend the EE so they won't be touching an element that is about to be deleted.
// However for pre-emptive mode threads, they could be stalled right on top of the element we want
// to delete, so we need to apply the reader lock to them and wait for them to drain.
ExecutionManager::ScanFlag ExecutionManager::GetScanFlags()
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

#if !defined(DACCESS_COMPILE)


    Thread *pThread = GetThreadNULLOk();

    if (!pThread)
        return ScanNoReaderLock;

    // If this thread is hijacked by a profiler and crawling its own stack,
    // we do need to take the lock
    if (pThread->GetProfilerFilterContext() != NULL)
        return ScanReaderLock;

    if (pThread->PreemptiveGCDisabled() || (pThread == ThreadSuspend::GetSuspensionThread()))
        return ScanNoReaderLock;



    return ScanReaderLock;
#else
    return ScanNoReaderLock;
#endif
}

#ifdef DACCESS_COMPILE

void IJitManager::EnumMemoryRegions(CLRDataEnumMemoryFlags flags)
{
    DAC_ENUM_VTHIS();
    if (m_runtimeSupport.IsValid())
    {
        m_runtimeSupport->EnumMemoryRegions(flags);
    }
}

#endif // #ifdef DACCESS_COMPILE

#if defined(FEATURE_EH_FUNCLETS)

PTR_VOID GetUnwindDataBlob(TADDR moduleBase, PTR_RUNTIME_FUNCTION pRuntimeFunction, /* out */ SIZE_T * pSize)
{
    LIMITED_METHOD_CONTRACT;

#if defined(TARGET_AMD64)
    PTR_UNWIND_INFO pUnwindInfo(dac_cast<PTR_UNWIND_INFO>(moduleBase + RUNTIME_FUNCTION__GetUnwindInfoAddress(pRuntimeFunction)));

    *pSize = ALIGN_UP(offsetof(UNWIND_INFO, UnwindCode) +
        sizeof(UNWIND_CODE) * pUnwindInfo->CountOfUnwindCodes +
        sizeof(ULONG) /* personality routine is always present */,
            sizeof(DWORD));

    return pUnwindInfo;

#elif defined(TARGET_X86)
    PTR_UNWIND_INFO pUnwindInfo(dac_cast<PTR_UNWIND_INFO>(moduleBase + RUNTIME_FUNCTION__GetUnwindInfoAddress(pRuntimeFunction)));

    *pSize = sizeof(UNWIND_INFO);

    return pUnwindInfo;

#elif defined(TARGET_ARM) || defined(TARGET_ARM64)

    // if this function uses packed unwind data then at least one of the two least significant bits
    // will be non-zero.  if this is the case then there will be no xdata record to enumerate.
    _ASSERTE((pRuntimeFunction->UnwindData & 0x3) == 0);

    // compute the size of the unwind info
    PTR_DWORD xdata = dac_cast<PTR_DWORD>(pRuntimeFunction->UnwindData + moduleBase);
    int size = 4;

#if defined(TARGET_ARM)
    // See https://learn.microsoft.com/cpp/build/arm-exception-handling
    int unwindWords = xdata[0] >> 28;
    int epilogScopes = (xdata[0] >> 23) & 0x1f;
#else
    // See https://learn.microsoft.com/cpp/build/arm64-exception-handling
    int unwindWords = xdata[0] >> 27;
    int epilogScopes = (xdata[0] >> 22) & 0x1f;
#endif

    if (unwindWords == 0 && epilogScopes == 0)
    {
        size += 4;
        unwindWords = (xdata[1] >> 16) & 0xff;
        epilogScopes = xdata[1] & 0xffff;
    }

    if (!(xdata[0] & (1 << 21)))
        size += 4 * epilogScopes;

    size += 4 * unwindWords;

    _ASSERTE(xdata[0] & (1 << 20)); // personality routine should be always present
    size += 4;

    *pSize = size;
    return xdata;


#elif defined(TARGET_LOONGARCH64) || defined(TARGET_RISCV64)
    // TODO: maybe optimize further.
    // if this function uses packed unwind data then at least one of the two least significant bits
    // will be non-zero.  if this is the case then there will be no xdata record to enumerate.
    _ASSERTE((pRuntimeFunction->UnwindData & 0x3) == 0);

    // compute the size of the unwind info
    PTR_ULONG xdata    = dac_cast<PTR_ULONG>(pRuntimeFunction->UnwindData + moduleBase);
    ULONG epilogScopes = 0;
    ULONG unwindWords  = 0;
    ULONG size = 0;

    //If both Epilog Count and Code Word is not zero
    //Info of Epilog and Unwind scopes are given by 1 word header
    //Otherwise this info is given by a 2 word header
    if ((xdata[0] >> 27) != 0)
    {
        size = 4;
        epilogScopes = (xdata[0] >> 22) & 0x1f;
        unwindWords = (xdata[0] >> 27) & 0x1f;
    }
    else
    {
        size = 8;
        epilogScopes = xdata[1] & 0xffff;
        unwindWords = (xdata[1] >> 16) & 0xff;
    }

    if (!(xdata[0] & (1 << 21)))
        size += 4 * epilogScopes;

    size += 4 * unwindWords;

    _ASSERTE(xdata[0] & (1 << 20)); // personality routine should be always present
    size += 4;                      // exception handler RVA

    *pSize = size;
    return xdata;

#else
    PORTABILITY_ASSERT("GetUnwindDataBlob");
    return NULL;
#endif
}

// GetFuncletStartAddress returns the starting address of the function or funclet indicated by the EECodeInfo address.
TADDR IJitManager::GetFuncletStartAddress(EECodeInfo * pCodeInfo)
{
    PTR_RUNTIME_FUNCTION pFunctionEntry = pCodeInfo->GetFunctionEntry();

#ifdef TARGET_AMD64
    _ASSERTE((pFunctionEntry->UnwindData & RUNTIME_FUNCTION_INDIRECT) == 0);
#endif

    TADDR baseAddress = pCodeInfo->GetModuleBase();
    TADDR funcletStartAddress = baseAddress + RUNTIME_FUNCTION__BeginAddress(pFunctionEntry);

#if defined(EXCEPTION_DATA_SUPPORTS_FUNCTION_FRAGMENTS)
    // Is the RUNTIME_FUNCTION a fragment? If so, we need to walk backwards until we find the first
    // non-fragment RUNTIME_FUNCTION, and use that one. This happens when we have very large functions
    // and multiple RUNTIME_FUNCTION entries per function or funclet. However, all but the first will
    // have the "F" bit set in the unwind data, indicating a fragment (with phantom prolog unwind codes).

    for (;;)
    {
        if (!IsFunctionFragment(baseAddress, pFunctionEntry))
        {
            // This is not a fragment; we're done
            break;
        }

        // We found a fragment. Walk backwards in the RUNTIME_FUNCTION array until we find a non-fragment.
        // We're guaranteed to find one, because we require that a fragment live in a function or funclet
        // that has a prolog, which will have non-fragment .xdata.
        --pFunctionEntry;

        funcletStartAddress = baseAddress + RUNTIME_FUNCTION__BeginAddress(pFunctionEntry);
    }
#endif // EXCEPTION_DATA_SUPPORTS_FUNCTION_FRAGMENTS

    return funcletStartAddress;
}

BOOL IJitManager::LazyIsFunclet(EECodeInfo * pCodeInfo)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        MODE_ANY;
    }
    CONTRACTL_END;

    TADDR funcletStartAddress = GetFuncletStartAddress(pCodeInfo);
    TADDR methodStartAddress = pCodeInfo->GetStartAddress();

    return (funcletStartAddress != methodStartAddress);
}

BOOL IJitManager::IsFilterFunclet(EECodeInfo * pCodeInfo)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        MODE_ANY;
    }
    CONTRACTL_END;

    if (!pCodeInfo->IsFunclet())
        return FALSE;

    TADDR funcletStartAddress = GetFuncletStartAddress(pCodeInfo);

    // This assumes no hot/cold splitting for funclets

    _ASSERTE(FitsInU4(pCodeInfo->GetCodeAddress() - funcletStartAddress));
    DWORD relOffsetWithinFunclet = static_cast<DWORD>(pCodeInfo->GetCodeAddress() - funcletStartAddress);

    _ASSERTE(pCodeInfo->GetRelOffset() >= relOffsetWithinFunclet);
    DWORD funcletStartOffset = pCodeInfo->GetRelOffset() - relOffsetWithinFunclet;

    EH_CLAUSE_ENUMERATOR pEnumState;
    unsigned EHCount = InitializeEHEnumeration(pCodeInfo->GetMethodToken(), &pEnumState);
    _ASSERTE(EHCount > 0);

    EE_ILEXCEPTION_CLAUSE EHClause;
    for (ULONG i = 0; i < EHCount; i++)
    {
         GetNextEHClause(&pEnumState, &EHClause);

        if (IsFilterHandler(&EHClause))
        {
            if (EHClause.FilterOffset == funcletStartOffset)
            {
                return true;
            }
        }
    }

    return false;
}

#else // FEATURE_EH_FUNCLETS

PTR_VOID GetUnwindDataBlob(TADDR moduleBase, PTR_RUNTIME_FUNCTION pRuntimeFunction, /* out */ SIZE_T * pSize)
{
    *pSize = 0;
    return dac_cast<PTR_VOID>(pRuntimeFunction->UnwindData + moduleBase);
}

#endif // FEATURE_EH_FUNCLETS



#ifndef DACCESS_COMPILE

//**********************************************************************************
//  EEJitManager
//**********************************************************************************

EECodeGenManager::EECodeGenManager()
    : m_pAllCodeHeaps(NULL)
    , m_cleanupList(NULL)
    // CRST_DEBUGGER_THREAD - We take this lock on debugger thread during EnC add method, among other things
    // CRST_TAKEN_DURING_SHUTDOWN - We take this lock during shutdown if ETW is on (to do rundown)
    , m_CodeHeapLock( CrstSingleUseLock,
                        CrstFlags(CRST_UNSAFE_ANYMODE|CRST_DEBUGGER_THREAD|CRST_TAKEN_DURING_SHUTDOWN))
    , m_iteratorCount(0)
    , m_storeRichDebugInfo(false)
{
}

EEJitManager::EEJitManager()
    : m_CPUCompileFlags()
    , m_JitLoadLock( CrstSingleUseLock )
{
    CONTRACTL {
        THROWS;
        GC_NOTRIGGER;
    } CONTRACTL_END;

    m_jit = NULL;
    m_JITCompiler      = NULL;
#ifdef TARGET_AMD64
    m_pEmergencyJumpStubReserveList = NULL;
#endif
#if defined(TARGET_X86) || defined(TARGET_AMD64)
    m_JITCompilerOther = NULL;
#endif

#ifdef ALLOW_SXS_JIT
    m_alternateJit     = NULL;
    m_AltJITCompiler   = NULL;
    m_AltJITRequired   = false;
#endif

    SetCpuInfo();
}

#ifdef TARGET_ARM64
extern "C" DWORD64 __stdcall GetDataCacheZeroIDReg();
extern "C" uint64_t GetSveLengthFromOS();
#endif

void EEJitManager::SetCpuInfo()
{
    LIMITED_METHOD_CONTRACT;

    //
    // NOTE: This function needs to be kept in sync with compSetProcessor() in jit\compiler.cpp
    //

    CORJIT_FLAGS CPUCompileFlags;

    int cpuFeatures = minipal_getcpufeatures();

    // Get the maximum bitwidth of Vector<T>, rounding down to the nearest multiple of 128-bits
    uint32_t maxVectorTBitWidth = (CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_MaxVectorTBitWidth) / 128) * 128;

#if defined(TARGET_X86) || defined(TARGET_AMD64)
    CPUCompileFlags.Set(InstructionSet_VectorT128);

    if (((cpuFeatures & XArchIntrinsicConstants_Avx2) != 0) && ((maxVectorTBitWidth == 0) || (maxVectorTBitWidth >= 256)))
    {
        // We allow 256-bit Vector<T> by default
        CPUCompileFlags.Set(InstructionSet_VectorT256);
    }

    if (((cpuFeatures & XArchIntrinsicConstants_Avx512) != 0) && (maxVectorTBitWidth >= 512))
    {
        // We require 512-bit Vector<T> to be opt-in
        CPUCompileFlags.Set(InstructionSet_VectorT512);
    }

    // x86-64-v1

    if (CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableHWIntrinsic))
    {
        CPUCompileFlags.Set(InstructionSet_X86Base);
    }

    // x86-64-v2

    if (((cpuFeatures & XArchIntrinsicConstants_Sse42) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableSSE42))
    {
        CPUCompileFlags.Set(InstructionSet_SSE42);
    }

    // x86-64-v3

    if (((cpuFeatures & XArchIntrinsicConstants_Avx) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableAVX))
    {
        CPUCompileFlags.Set(InstructionSet_AVX);
    }

    if (((cpuFeatures & XArchIntrinsicConstants_Avx2) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableAVX2))
    {
        CPUCompileFlags.Set(InstructionSet_AVX2);
    }

    // x86-64-v4

    if (((cpuFeatures & XArchIntrinsicConstants_Avx512) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableAVX512))
    {
        CPUCompileFlags.Set(InstructionSet_AVX512);
    }

    // x86-64-vFuture

    if (((cpuFeatures & XArchIntrinsicConstants_Avx512v2) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableAVX512v2))
    {
        CPUCompileFlags.Set(InstructionSet_AVX512v2);
    }

    if (((cpuFeatures & XArchIntrinsicConstants_Avx512v3) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableAVX512v3))
    {
        CPUCompileFlags.Set(InstructionSet_AVX512v3);
    }

    if (((cpuFeatures & XArchIntrinsicConstants_Avx10v1) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableAVX10v1))
    {
        CPUCompileFlags.Set(InstructionSet_AVX10v1);
    }

    if (((cpuFeatures & XArchIntrinsicConstants_Avx10v2) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableAVX10v2))
    {
        CPUCompileFlags.Set(InstructionSet_AVX10v2);
        CPUCompileFlags.Set(InstructionSet_AVXVNNIINT_V512);
    }

#if defined(TARGET_AMD64)
    if (((cpuFeatures & XArchIntrinsicConstants_Apx) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableAPX))
    {
        CPUCompileFlags.Set(InstructionSet_APX);
    }
#endif  // TARGET_AMD64

    // Unversioned

    if (((cpuFeatures & XArchIntrinsicConstants_Aes) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableAES))
    {
        CPUCompileFlags.Set(InstructionSet_AES);

        if (((cpuFeatures & XArchIntrinsicConstants_Vaes) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableVAES))
        {
            CPUCompileFlags.Set(InstructionSet_AES_V256);
            CPUCompileFlags.Set(InstructionSet_AES_V512);
        }
    }

    if (((cpuFeatures & XArchIntrinsicConstants_Avx512Vp2intersect) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableAVX512VP2INTERSECT))
    {
        CPUCompileFlags.Set(InstructionSet_AVX512VP2INTERSECT);
    }

    if (((cpuFeatures & XArchIntrinsicConstants_AvxIfma) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableAVXIFMA))
    {
        CPUCompileFlags.Set(InstructionSet_AVXIFMA);
    }

    if (((cpuFeatures & XArchIntrinsicConstants_AvxVnni) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableAVXVNNI))
    {
        CPUCompileFlags.Set(InstructionSet_AVXVNNI);
    }

    if (((cpuFeatures & XArchIntrinsicConstants_Gfni) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableGFNI))
    {
        CPUCompileFlags.Set(InstructionSet_GFNI);
        CPUCompileFlags.Set(InstructionSet_GFNI_V256);
        CPUCompileFlags.Set(InstructionSet_GFNI_V512);
    }

    if (((cpuFeatures & XArchIntrinsicConstants_Sha) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableSHA))
    {
        CPUCompileFlags.Set(InstructionSet_SHA);
    }

    if (((cpuFeatures & XArchIntrinsicConstants_AvxVnniInt) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableAVXVNNIINT))
    {
        CPUCompileFlags.Set(InstructionSet_AVXVNNIINT);
    }

    if (((cpuFeatures & XArchIntrinsicConstants_WaitPkg) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableWAITPKG))
    {
        CPUCompileFlags.Set(InstructionSet_WAITPKG);
    }

    if (((cpuFeatures & XArchIntrinsicConstants_X86Serialize) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableX86Serialize))
    {
        CPUCompileFlags.Set(InstructionSet_X86Serialize);
    }
#elif defined(TARGET_ARM64)
    CPUCompileFlags.Set(InstructionSet_VectorT128);

    if (CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableHWIntrinsic))
    {
        CPUCompileFlags.Set(InstructionSet_ArmBase);
        CPUCompileFlags.Set(InstructionSet_AdvSimd);
    }

    if (((cpuFeatures & ARM64IntrinsicConstants_Aes) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableArm64Aes))
    {
        CPUCompileFlags.Set(InstructionSet_Aes);
    }

    if (((cpuFeatures & ARM64IntrinsicConstants_Atomics) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableArm64Atomics))
    {
        CPUCompileFlags.Set(InstructionSet_Atomics);
    }

    if (((cpuFeatures & ARM64IntrinsicConstants_Rcpc) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableArm64Rcpc))
    {
        CPUCompileFlags.Set(InstructionSet_Rcpc);
    }

    if (((cpuFeatures & ARM64IntrinsicConstants_Rcpc2) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableArm64Rcpc2))
    {
        CPUCompileFlags.Set(InstructionSet_Rcpc2);
    }

    if (((cpuFeatures & ARM64IntrinsicConstants_Crc32) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableArm64Crc32))
    {
        CPUCompileFlags.Set(InstructionSet_Crc32);
    }

    if (((cpuFeatures & ARM64IntrinsicConstants_Dp) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableArm64Dp))
    {
        CPUCompileFlags.Set(InstructionSet_Dp);
    }

    if (((cpuFeatures & ARM64IntrinsicConstants_Rdm) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableArm64Rdm))
    {
        CPUCompileFlags.Set(InstructionSet_Rdm);
    }

    if (((cpuFeatures & ARM64IntrinsicConstants_Sha1) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableArm64Sha1))
    {
        CPUCompileFlags.Set(InstructionSet_Sha1);
    }

    if (((cpuFeatures & ARM64IntrinsicConstants_Sha256) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableArm64Sha256))
    {
        CPUCompileFlags.Set(InstructionSet_Sha256);
    }

    if (((cpuFeatures & ARM64IntrinsicConstants_Sve) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableArm64Sve))
    {
        uint32_t maxVectorTLength = (maxVectorTBitWidth / 8);
        uint64_t sveLengthFromOS = GetSveLengthFromOS();

        // For now, enable SVE only when the system vector length is 16 bytes (128-bits)
        // TODO: https://github.com/dotnet/runtime/issues/101477
        if (sveLengthFromOS == 16)
        // if ((maxVectorTLength >= sveLengthFromOS) || (maxVectorTBitWidth == 0))
        {
            CPUCompileFlags.Set(InstructionSet_Sve);

            if (((cpuFeatures & ARM64IntrinsicConstants_Sve2) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableArm64Sve2))
            {
                CPUCompileFlags.Set(InstructionSet_Sve2);
            }
        }
    }

    // DCZID_EL0<4> (DZP) indicates whether use of DC ZVA instructions is permitted (0) or prohibited (1).
    // DCZID_EL0<3:0> (BS) specifies Log2 of the block size in words.
    //
    // We set the flag when the instruction is permitted and the block size is 64 bytes.
    if ((GetDataCacheZeroIDReg() == 4) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableArm64Dczva))
    {
        CPUCompileFlags.Set(InstructionSet_Dczva);
    }

    if ((cpuFeatures & ARM64IntrinsicConstants_Atomics) != 0)
    {
        g_arm64_atomics_present = true;
    }
#elif defined(TARGET_RISCV64)
    if (CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableHWIntrinsic))
    {
        CPUCompileFlags.Set(InstructionSet_RiscV64Base);
    }

    if (((cpuFeatures & RiscV64IntrinsicConstants_Zba) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableRiscV64Zba))
    {
        CPUCompileFlags.Set(InstructionSet_Zba);
    }

    if (((cpuFeatures & RiscV64IntrinsicConstants_Zbb) != 0) && CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_EnableRiscV64Zbb))
    {
        CPUCompileFlags.Set(InstructionSet_Zbb);
    }
#endif

    // These calls are very important as it ensures the flags are consistent with any
    // removals specified above. This includes removing corresponding 64-bit ISAs
    // and any other implications such as SSE2 depending on SSE or AdvSimd on ArmBase

    CPUCompileFlags.Set64BitInstructionSetVariants();
    CPUCompileFlags.EnsureValidInstructionSetSupport();

#if defined(TARGET_X86) || defined(TARGET_AMD64)

    bool throttleVector512 = false;
    int cpuidInfo[4];

    const int CPUID_EAX = 0;
    const int CPUID_EBX = 1;
    const int CPUID_ECX = 2;
    const int CPUID_EDX = 3;

    __cpuid(cpuidInfo, 0x00000000);

    bool isGenuineIntel = (cpuidInfo[CPUID_EBX] == 0x756E6547) && // Genu
                          (cpuidInfo[CPUID_EDX] == 0x49656E69) && // ineI
                          (cpuidInfo[CPUID_ECX] == 0x6C65746E);   // ntel

    if (isGenuineIntel)
    {
        union XarchCpuInfo
        {
            struct {
                uint32_t SteppingId       : 4;
                uint32_t Model            : 4;
                uint32_t FamilyId         : 4;
                uint32_t ProcessorType    : 2;
                uint32_t Reserved1        : 2; // Unused bits in the CPUID result
                uint32_t ExtendedModelId  : 4;
                uint32_t ExtendedFamilyId : 8;
                uint32_t Reserved         : 4; // Unused bits in the CPUID result
            };

            uint32_t Value;
        } xarchCpuInfo;

        __cpuid(cpuidInfo, 0x00000001);
        _ASSERTE((cpuidInfo[CPUID_EDX] & (1 << 15)) != 0);                                                    // CMOV

        xarchCpuInfo.Value = cpuidInfo[CPUID_EAX];

        // Some architectures can experience frequency throttling when executing
        // executing 512-bit width instructions. To account for this we set the
        // default preferred vector width to 256-bits in some scenarios. Power
        // users can override this with `DOTNET_PreferredVectorBitWidth=512` to
        // allow using such instructions where hardware support is available.

        if (xarchCpuInfo.FamilyId == 0x06)
        {
            if (xarchCpuInfo.ExtendedModelId == 0x05)
            {
                if (xarchCpuInfo.Model == 0x05)
                {
                    // * Skylake (Server)
                    // * Cascade Lake
                    // * Cooper Lake

                    throttleVector512 = true;
                }
            }
            else if (xarchCpuInfo.ExtendedModelId == 0x06)
            {
                if (xarchCpuInfo.Model == 0x06)
                {
                    // * Cannon Lake

                    throttleVector512 = true;
                }
            }
        }
    }

    // If we have a PreferredVectorBitWidth, we will pass that to JIT in the form of a virtual vector ISA of the
    // appropriate size. We will also clamp the max Vector<T> size to be no larger than PreferredVectorBitWidth,
    // because JIT maps Vector<T> to the fixed-width vector of matching size for the purposes of intrinsic
    // resolution. We want to avoid a situation where e.g. Vector.IsHardwareAccelerated returns false
    // because Vector512.IsHardwareAccelerated returns false due to config or automatic throttling.

    uint32_t preferredVectorBitWidth = (CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_PreferredVectorBitWidth) / 128) * 128;

    if ((preferredVectorBitWidth == 0) && throttleVector512)
    {
        preferredVectorBitWidth = 256;
    }

    if (preferredVectorBitWidth >= 512)
    {
        CPUCompileFlags.Set(InstructionSet_Vector512);
    }
    else if (preferredVectorBitWidth >= 256)
    {
        CPUCompileFlags.Set(InstructionSet_Vector256);
        CPUCompileFlags.Clear(InstructionSet_VectorT512);
    }
    else if (preferredVectorBitWidth != 0)
    {
        CPUCompileFlags.Set(InstructionSet_Vector128);
        CPUCompileFlags.Clear(InstructionSet_VectorT512);
        CPUCompileFlags.Clear(InstructionSet_VectorT256);
    }

    // Only one VectorT ISA can be set, and we have validated that anything left in the flags is supported
    // by both the hardware and the config. Remove everything less than the largest supported.

    if (CPUCompileFlags.IsSet(InstructionSet_VectorT512))
    {
        CPUCompileFlags.Clear(InstructionSet_VectorT256);
        CPUCompileFlags.Clear(InstructionSet_VectorT128);
    }
    else if (CPUCompileFlags.IsSet(InstructionSet_VectorT256))
    {
        CPUCompileFlags.Clear(InstructionSet_VectorT128);
    }

#endif // TARGET_X86 || TARGET_AMD64

    m_CPUCompileFlags = CPUCompileFlags;
}

// Define some data that we can use to get a better idea of what happened when we get a Watson dump that indicates the JIT failed to load.
// This will be used and updated by the JIT loading and initialization functions, and the data written will get written into a Watson dump.

enum JIT_LOAD_JIT_ID
{
    JIT_LOAD_MAIN = 500,    // The "main" JIT. Normally, this is named "clrjit.dll". Start at a number that is somewhat uncommon (i.e., not zero or 1) to help distinguish from garbage, in process dumps.
    // 501 is JIT_LOAD_LEGACY on some platforms; please do not reuse this value.
    JIT_LOAD_ALTJIT = 502,   // An "altjit". By default, named something like "clrjit_<targetos>_<target_arch>_<host_arch>.dll". Used both internally, as well as externally for JIT CTP builds.
    JIT_LOAD_INTERPRETER = 503 // The interpreter compilation phase. Named "clrinterpreter.dll".
};

enum JIT_LOAD_STATUS
{
    JIT_LOAD_STATUS_STARTING = 1001,                   // The JIT load process is starting. Start at a number that is somewhat uncommon (i.e., not zero or 1) to help distinguish from garbage, in process dumps.
    JIT_LOAD_STATUS_DONE_LOAD,                         // LoadLibrary of the JIT dll succeeded.
    JIT_LOAD_STATUS_DONE_GET_JITSTARTUP,               // GetProcAddress for "jitStartup" succeeded.
    JIT_LOAD_STATUS_DONE_CALL_JITSTARTUP,              // Calling jitStartup() succeeded.
    JIT_LOAD_STATUS_DONE_GET_GETJIT,                   // GetProcAddress for "getJit" succeeded.
    JIT_LOAD_STATUS_DONE_CALL_GETJIT,                  // Calling getJit() succeeded.
    JIT_LOAD_STATUS_DONE_CALL_GETVERSIONIDENTIFIER,    // Calling ICorJitCompiler::getVersionIdentifier() succeeded.
    JIT_LOAD_STATUS_DONE_VERSION_CHECK,                // The JIT-EE version identifier check succeeded.
    JIT_LOAD_STATUS_DONE,                              // The JIT load is complete, and successful.
};

struct JIT_LOAD_DATA
{
    JIT_LOAD_JIT_ID     jld_id;         // Which JIT are we currently loading?
    JIT_LOAD_STATUS     jld_status;     // The current load status of a JIT load attempt.
    HRESULT             jld_hr;         // If the JIT load fails, the last jld_status will be JIT_LOAD_STATUS_STARTING.
                                        //   In that case, this will contain the HRESULT returned by LoadLibrary.
                                        //   Otherwise, this will be S_OK (which is zero).
};

#ifdef FEATURE_STATICALLY_LINKED

EXTERN_C void jitStartup(ICorJitHost* host);
EXTERN_C ICorJitCompiler* getJit();

#endif // FEATURE_STATICALLY_LINKED

#if !defined(FEATURE_STATICALLY_LINKED) || defined(FEATURE_JIT)

#ifdef FEATURE_JIT
JIT_LOAD_DATA g_JitLoadData;
#endif // FEATURE_JIT

#ifdef FEATURE_INTERPRETER
JIT_LOAD_DATA g_interpreterLoadData;
#endif // FEATURE_INTERPRETER

CORINFO_OS getClrVmOs();

#define LogJITInitializationError(...) \
    LOG((LF_JIT, LL_FATALERROR, __VA_ARGS__)); \
    LogErrorToHost(__VA_ARGS__);

// LoadAndInitializeJIT: load the JIT dll into the process, and initialize it (call the UtilCode initialization function,
// check the JIT-EE interface GUID, etc.)
//
// Parameters:
//
// pwzJitName        - The filename of the JIT .dll file to load. E.g., "altjit.dll".
// pwzJitPath        - (Debug only) The full path of the JIT .dll file to load
// phJit             - On return, *phJit is the Windows module handle of the loaded JIT dll. It will be NULL if the load failed.
// ppICorJitCompiler - On return, *ppICorJitCompiler is the ICorJitCompiler* returned by the JIT's getJit() entrypoint.
//                     It is NULL if the JIT returns a NULL interface pointer, or if the JIT-EE interface GUID is mismatched.
//                     Note that if the given JIT is loaded, but the interface is mismatched, then *phJit will be legal and non-NULL
//                     even though *ppICorJitCompiler is NULL. This allows the caller to unload the JIT dll, if necessary
//                     (nobody does this today).
// pJitLoadData      - Pointer to a structure that we update as we load and initialize the JIT to indicate how far we've gotten. This
//                     is used to help understand problems we see with JIT loading that come in via Watson dumps. Since we don't throw
//                     an exception immediately upon failure, we can lose information about what the failure was if we don't store this
//                     information in a way that persists into a process dump.
// targetOs          - Target OS for JIT
//

static void LoadAndInitializeJIT(LPCWSTR pwzJitName DEBUGARG(LPCWSTR pwzJitPath), OUT HINSTANCE* phJit, OUT ICorJitCompiler** ppICorJitCompiler, IN OUT JIT_LOAD_DATA* pJitLoadData, CORINFO_OS targetOs)
{
    STANDARD_VM_CONTRACT;

    _ASSERTE(phJit != NULL);
    _ASSERTE(ppICorJitCompiler != NULL);
    _ASSERTE(pJitLoadData != NULL);

    pJitLoadData->jld_status = JIT_LOAD_STATUS_STARTING;
    pJitLoadData->jld_hr     = S_OK;

    *phJit = NULL;
    *ppICorJitCompiler = NULL;

    HRESULT hr = E_FAIL;

#ifdef _DEBUG
    if (pwzJitPath != NULL)
    {
        *phJit = CLRLoadLibrary(pwzJitPath);
        if (*phJit != NULL)
        {
            hr = S_OK;
        }
    }
#endif

    if (hr == E_FAIL)
    {
        if (pwzJitName == nullptr)
        {
            pJitLoadData->jld_hr = E_FAIL;
            LogJITInitializationError("LoadAndInitializeJIT: pwzJitName is null");
            return;
        }

        if (ValidateModuleName(pwzJitName))
        {
            // Load JIT from next to CoreCLR binary
            PathString CoreClrFolderHolder;
            if (GetClrModulePathName(CoreClrFolderHolder) && !CoreClrFolderHolder.IsEmpty())
            {
                SString::Iterator iter = CoreClrFolderHolder.End();
                BOOL findSep = CoreClrFolderHolder.FindBack(iter, DIRECTORY_SEPARATOR_CHAR_W);
                if (findSep)
                {
                    SString sJitName(pwzJitName);
                    CoreClrFolderHolder.Replace(iter + 1, CoreClrFolderHolder.End() - (iter + 1), sJitName);

                    *phJit = CLRLoadLibrary(CoreClrFolderHolder.GetUnicode());
                    if (*phJit != NULL)
                    {
                        hr = S_OK;
                    }
                }
            }
        }
        else
        {
            MAKE_UTF8PTR_FROMWIDE_NOTHROW(utf8JitName, pwzJitName);
            LogJITInitializationError("LoadAndInitializeJIT: invalid characters in %s", utf8JitName);
        }
    }

    MAKE_UTF8PTR_FROMWIDE_NOTHROW(utf8JitName, pwzJitName);

    if (SUCCEEDED(hr))
    {
        pJitLoadData->jld_status = JIT_LOAD_STATUS_DONE_LOAD;

        EX_TRY
        {
            typedef void (* pjitStartup)(ICorJitHost*);
            pjitStartup jitStartupFn = (pjitStartup) GetProcAddress(*phJit, "jitStartup");

            if (jitStartupFn)
            {
                pJitLoadData->jld_status = JIT_LOAD_STATUS_DONE_GET_JITSTARTUP;

                (*jitStartupFn)(JitHost::getJitHost());

                pJitLoadData->jld_status = JIT_LOAD_STATUS_DONE_CALL_JITSTARTUP;
            }

            typedef ICorJitCompiler* (__stdcall* pGetJitFn)();
            pGetJitFn getJitFn = (pGetJitFn) GetProcAddress(*phJit, "getJit");

            if (getJitFn)
            {
                pJitLoadData->jld_status = JIT_LOAD_STATUS_DONE_GET_GETJIT;

                ICorJitCompiler* pICorJitCompiler = (*getJitFn)();
                if (pICorJitCompiler != NULL)
                {
                    pJitLoadData->jld_status = JIT_LOAD_STATUS_DONE_CALL_GETJIT;

                    GUID versionId;
                    memset(&versionId, 0, sizeof(GUID));
                    pICorJitCompiler->getVersionIdentifier(&versionId);

                    pJitLoadData->jld_status = JIT_LOAD_STATUS_DONE_CALL_GETVERSIONIDENTIFIER;

                    if (memcmp(&versionId, &JITEEVersionIdentifier, sizeof(GUID)) == 0)
                    {
                        pJitLoadData->jld_status = JIT_LOAD_STATUS_DONE_VERSION_CHECK;

                        // Specify to the JIT that it is working with the OS that we are compiled against
                        pICorJitCompiler->setTargetOS(targetOs);

                        // The JIT has loaded and passed the version identifier test, so publish the JIT interface to the caller.
                        *ppICorJitCompiler = pICorJitCompiler;

                        // The JIT is completely loaded and initialized now.
                        pJitLoadData->jld_status = JIT_LOAD_STATUS_DONE;
                    }
                    else
                    {
                        // Mismatched version ID. Fail the load.
                        LogJITInitializationError("LoadAndInitializeJIT: mismatched JIT version identifier in %s", utf8JitName);
                    }
                }
                else
                {
                    LogJITInitializationError("LoadAndInitializeJIT: failed to get ICorJitCompiler in %s", utf8JitName);
                }
            }
            else
            {
                LogJITInitializationError("LoadAndInitializeJIT: failed to find 'getJit' entrypoint in %s", utf8JitName);
            }
        }
        EX_CATCH
        {
            LogJITInitializationError("LoadAndInitializeJIT: LoadAndInitializeJIT: caught an exception trying to initialize %s", utf8JitName);
        }
        EX_END_CATCH
    }
    else
    {
        pJitLoadData->jld_hr = hr;
        LogJITInitializationError("LoadAndInitializeJIT: failed to load %s, hr=0x%08X", utf8JitName, hr);
    }
}
#endif // !FEATURE_STATICALLY_LINKED || FEATURE_JIT

#ifdef FEATURE_STATICALLY_LINKED
static ICorJitCompiler* InitializeStaticJIT()
{
    ICorJitCompiler* newJitCompiler = NULL;
    EX_TRY
    {
        jitStartup(JitHost::getJitHost());

        newJitCompiler = getJit();

        // We don't need to call getVersionIdentifier(), since the JIT is linked together with the VM.
    }
    EX_CATCH
    {
    }
    EX_END_CATCH

    return newJitCompiler;
}
#endif // FEATURE_STATICALLY_LINKED

#ifdef FEATURE_JIT
BOOL EEJitManager::LoadJIT()
{
    STANDARD_VM_CONTRACT;

    // If the JIT is already loaded, don't take the lock.
    if (IsJitLoaded())
        return TRUE;

    // Use m_JitLoadLock to ensure that the JIT is loaded on one thread only
    CrstHolder chRead(&m_JitLoadLock);

    // Did someone load the JIT before we got the lock?
    if (IsJitLoaded())
        return TRUE;

    m_storeRichDebugInfo = CLRConfig::GetConfigValue(CLRConfig::UNSUPPORTED_RichDebugInfo) != 0;

    ICorJitCompiler* newJitCompiler = NULL;

#ifdef FEATURE_STATICALLY_LINKED
    newJitCompiler = InitializeStaticJIT();
#else // !FEATURE_STATICALLY_LINKED

    m_JITCompiler = NULL;
#if defined(TARGET_X86) || defined(TARGET_AMD64)
    m_JITCompilerOther = NULL;
#endif

    g_JitLoadData.jld_id = JIT_LOAD_MAIN;
    LPWSTR mainJitPath = NULL;
#ifdef _DEBUG
    IfFailThrow(CLRConfig::GetConfigValue(CLRConfig::INTERNAL_JitPath, &mainJitPath));
#endif
    LoadAndInitializeJIT(ExecutionManager::GetJitName() DEBUGARG(mainJitPath), &m_JITCompiler, &newJitCompiler, &g_JitLoadData, getClrVmOs());
#endif // !FEATURE_STATICALLY_LINKED

#ifdef ALLOW_SXS_JIT

    // Do not load altjit.dll unless DOTNET_AltJit is set.
    // Even if the main JIT fails to load, if the user asks for an altjit we try to load it.
    // This allows us to display load error messages for loading altjit.

    ICorJitCompiler* newAltJitCompiler = NULL;

    LPWSTR altJitConfig;
    IfFailThrow(CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_AltJit, &altJitConfig));

    m_AltJITCompiler = NULL;

    if (altJitConfig != NULL)
    {
        // Load the altjit into the system.
        // Note: altJitName must be declared as a const otherwise assigning the string
        // constructed by MAKEDLLNAME_W() to altJitName will cause a build break on Unix.
        LPCWSTR altJitName;
        IfFailThrow(CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_AltJitName, (LPWSTR*)&altJitName));

        if (altJitName == NULL)
        {
#ifdef TARGET_WINDOWS
#ifdef TARGET_X86
            altJitName = MAKEDLLNAME_W(W("clrjit_win_x86_x86"));
#elif defined(TARGET_AMD64)
            altJitName = MAKEDLLNAME_W(W("clrjit_win_x64_x64"));
#endif
#else // TARGET_WINDOWS
#ifdef TARGET_X86
            altJitName = MAKEDLLNAME_W(W("clrjit_unix_x86_x86"));
#elif defined(TARGET_AMD64)
            altJitName = MAKEDLLNAME_W(W("clrjit_unix_x64_x64"));
#elif defined(TARGET_LOONGARCH64)
            altJitName = MAKEDLLNAME_W(W("clrjit_unix_loongarch64_loongarch64"));
#elif defined(TARGET_RISCV64)
            altJitName = MAKEDLLNAME_W(W("clrjit_unix_riscv64_riscv64"));
#endif
#endif // TARGET_WINDOWS

#if defined(TARGET_ARM)
            altJitName = MAKEDLLNAME_W(W("clrjit_universal_arm_arm"));
#elif defined(TARGET_ARM64)
            altJitName = MAKEDLLNAME_W(W("clrjit_universal_arm64_arm64"));
#endif // TARGET_ARM
        }

#ifdef _DEBUG
        LPWSTR altJitPath;
        IfFailThrow(CLRConfig::GetConfigValue(CLRConfig::INTERNAL_AltJitPath, &altJitPath));
#endif

        CORINFO_OS targetOs = getClrVmOs();
        LPWSTR altJitOsConfig;
        IfFailThrow(CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_AltJitOs, &altJitOsConfig));
        if (altJitOsConfig != NULL)
        {
            // We have some inconsistency all over the place with osx vs macos, let's handle both here
            if ((_wcsicmp(altJitOsConfig, W("macos")) == 0) || (_wcsicmp(altJitOsConfig, W("osx")) == 0))
            {
                targetOs = CORINFO_APPLE;
            }
            else if ((_wcsicmp(altJitOsConfig, W("linux")) == 0) || (_wcsicmp(altJitOsConfig, W("unix")) == 0))
            {
                targetOs = CORINFO_UNIX;
            }
            else if (_wcsicmp(altJitOsConfig, W("windows")) == 0)
            {
                targetOs = CORINFO_WINNT;
            }
            else
            {
                _ASSERTE(!"Unknown AltJitOS, it has to be either Windows, Linux or macOS");
            }
        }
        g_JitLoadData.jld_id = JIT_LOAD_ALTJIT;
        LoadAndInitializeJIT(altJitName DEBUGARG(altJitPath), &m_AltJITCompiler, &newAltJitCompiler, &g_JitLoadData, targetOs);
    }

#endif // ALLOW_SXS_JIT

    // Publish the compilers.

#ifdef ALLOW_SXS_JIT
    m_AltJITRequired = (altJitConfig != NULL);
    m_alternateJit = newAltJitCompiler;
#endif // ALLOW_SXS_JIT

    m_jit = newJitCompiler;

    // Failing to load the main JIT is a failure.
    // If the user requested an altjit and we failed to load an altjit, that is also a failure.
    // In either failure case, we'll rip down the VM (so no need to clean up (unload) either JIT that did load successfully.
    return IsJitLoaded();
}
#endif // FEATURE_JIT

//**************************************************************************

CodeFragmentHeap::CodeFragmentHeap(LoaderAllocator * pAllocator, StubCodeBlockKind kind)
    : m_pAllocator(pAllocator), m_pFreeBlocks(NULL), m_kind(kind),
    // CRST_DEBUGGER_THREAD - We take this lock on debugger thread during EnC add meth
    m_Lock(CrstCodeFragmentHeap, CrstFlags(CRST_UNSAFE_ANYMODE | CRST_DEBUGGER_THREAD))
{
    WRAPPER_NO_CONTRACT;
}

void CodeFragmentHeap::AddBlock(VOID * pMem, size_t dwSize)
{
     CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        MODE_ANY;
    }
    CONTRACTL_END;

    // The new "nothrow" below failure is handled in a non-fault way, so
    // make sure that callers with FORBID_FAULT can call this method without
    // firing the contract violation assert.
    PERMANENT_CONTRACT_VIOLATION(FaultViolation, ReasonContractInfrastructure);

    FreeBlock * pBlock = new (nothrow) FreeBlock;
    // In the OOM case we don't add the block to the list of free blocks
    // as we are in a FORBID_FAULT code path.
    if (pBlock != NULL)
    {
        pBlock->m_pNext = m_pFreeBlocks;
        pBlock->m_pBlock = pMem;
        pBlock->m_dwSize = dwSize;
        m_pFreeBlocks = pBlock;
    }
}

void CodeFragmentHeap::RemoveBlock(FreeBlock ** ppBlock)
{
    LIMITED_METHOD_CONTRACT;
    FreeBlock * pBlock = *ppBlock;
    *ppBlock = pBlock->m_pNext;
    delete pBlock;
}

CodeFragmentHeap::~CodeFragmentHeap()
{
    FreeBlock* pBlock = m_pFreeBlocks;
    while (pBlock != NULL)
    {
        FreeBlock *pNextBlock = pBlock->m_pNext;
        delete pBlock;
        pBlock = pNextBlock;
    }
}

TaggedMemAllocPtr CodeFragmentHeap::RealAllocAlignedMem(size_t  dwRequestedSize
                    ,unsigned  dwAlignment
#ifdef _DEBUG
                    ,_In_ _In_z_ const char *szFile
                    ,int  lineNum
#endif
                    )
{
    CrstHolder ch(&m_Lock);

    dwRequestedSize = ALIGN_UP(dwRequestedSize, sizeof(TADDR));

    // We will try to batch up allocation of small blocks into one large allocation
#define SMALL_BLOCK_THRESHOLD 0x100
    SIZE_T nFreeSmallBlocks = 0;

    FreeBlock ** ppBestFit = NULL;
    FreeBlock ** ppFreeBlock = &m_pFreeBlocks;
    while (*ppFreeBlock != NULL)
    {
        FreeBlock * pFreeBlock = *ppFreeBlock;
        if (((BYTE *)pFreeBlock->m_pBlock + pFreeBlock->m_dwSize) - (BYTE *)ALIGN_UP(pFreeBlock->m_pBlock, dwAlignment) >= (SSIZE_T)dwRequestedSize)
        {
            if (ppBestFit == NULL || pFreeBlock->m_dwSize < (*ppBestFit)->m_dwSize)
                ppBestFit = ppFreeBlock;
        }
        else
        {
            if (pFreeBlock->m_dwSize < SMALL_BLOCK_THRESHOLD)
                nFreeSmallBlocks++;
        }
        ppFreeBlock = &(*ppFreeBlock)->m_pNext;
    }

    VOID * pMem;
    SIZE_T dwSize;
    if (ppBestFit != NULL)
    {
        pMem = (*ppBestFit)->m_pBlock;
        dwSize = (*ppBestFit)->m_dwSize;

        RemoveBlock(ppBestFit);
    }
    else
    {
        dwSize = dwRequestedSize;
        if (dwSize < SMALL_BLOCK_THRESHOLD)
            dwSize = 4 * SMALL_BLOCK_THRESHOLD;
        pMem = ExecutionManager::GetEEJitManager()->AllocCodeFragmentBlock(dwSize, dwAlignment, m_pAllocator, m_kind);
        ReportStubBlock(pMem, dwSize, m_kind);
    }

    SIZE_T dwExtra = (BYTE *)ALIGN_UP(pMem, dwAlignment) - (BYTE *)pMem;
    _ASSERTE(dwSize >= dwExtra + dwRequestedSize);
    SIZE_T dwRemaining = dwSize - (dwExtra + dwRequestedSize);

    // Avoid accumulation of too many small blocks. The more small free blocks we have, the more picky we are going to be about adding new ones.
    if ((dwRemaining >= max(sizeof(FreeBlock), sizeof(StubPrecode)) + (SMALL_BLOCK_THRESHOLD / 0x10) * nFreeSmallBlocks) || (dwRemaining >= SMALL_BLOCK_THRESHOLD))
    {
        AddBlock((BYTE *)pMem + dwExtra + dwRequestedSize, dwRemaining);
        dwSize -= dwRemaining;
    }

    TaggedMemAllocPtr tmap;
    tmap.m_pMem             = pMem;
    tmap.m_dwRequestedSize  = dwSize;
    tmap.m_pHeap            = this;
    tmap.m_dwExtra          = dwExtra;
#ifdef _DEBUG
    tmap.m_szFile           = szFile;
    tmap.m_lineNum          = lineNum;
#endif
    return tmap;
}

void CodeFragmentHeap::RealBackoutMem(void *pMem
                    , size_t dwSize
#ifdef _DEBUG
                    , _In_ _In_z_ const char *szFile
                    , int lineNum
                    , _In_ _In_z_ const char *szAllocFile
                    , int allocLineNum
#endif
                    )
{
    CrstHolder ch(&m_Lock);

    {
        ExecutableWriterHolder<BYTE> memWriterHolder((BYTE*)pMem, dwSize);
        ZeroMemory(memWriterHolder.GetRW(), dwSize);
    }

    //
    // Try to coalesce blocks if possible
    //
    FreeBlock ** ppFreeBlock = &m_pFreeBlocks;
    while (*ppFreeBlock != NULL)
    {
        FreeBlock * pFreeBlock = *ppFreeBlock;

        if ((BYTE *)pFreeBlock == (BYTE *)pMem + dwSize)
        {
            // pMem = pMem;
            dwSize += pFreeBlock->m_dwSize;
            RemoveBlock(ppFreeBlock);
            continue;
        }
        else
        if ((BYTE *)pFreeBlock + pFreeBlock->m_dwSize == (BYTE *)pMem)
        {
            pMem = pFreeBlock;
            dwSize += pFreeBlock->m_dwSize;
            RemoveBlock(ppFreeBlock);
            continue;
        }

        ppFreeBlock = &(*ppFreeBlock)->m_pNext;
    }

    AddBlock(pMem, dwSize);
}

//**************************************************************************

LoaderCodeHeap::LoaderCodeHeap(bool fMakeExecutable)
    : m_LoaderHeap(fMakeExecutable),
    m_cbMinNextPad(0)
{
    WRAPPER_NO_CONTRACT;
}

void ThrowOutOfMemoryWithinRange()
{
    CONTRACTL {
        THROWS;
        GC_NOTRIGGER;
    } CONTRACTL_END;

    // Allow breaking into debugger or terminating the process when this exception occurs
    switch (CLRConfig::GetConfigValue(CLRConfig::INTERNAL_BreakOnOutOfMemoryWithinRange))
    {
    case 1:
        DebugBreak();
        break;
    case 2:
        EEPOLICY_HANDLE_FATAL_ERROR(COR_E_OUTOFMEMORY);
        break;
    default:
        break;
    }

    EX_THROW(EEMessageException, (kOutOfMemoryException, IDS_EE_OUT_OF_MEMORY_WITHIN_RANGE));
}

#ifdef TARGET_AMD64
BYTE * EEJitManager::AllocateFromEmergencyJumpStubReserve(const BYTE * loAddr, const BYTE * hiAddr, SIZE_T * pReserveSize)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        PRECONDITION(m_CodeHeapLock.OwnedByCurrentThread());
    } CONTRACTL_END;

    for (EmergencyJumpStubReserve ** ppPrev = &m_pEmergencyJumpStubReserveList; *ppPrev != NULL; ppPrev = &(*ppPrev)->m_pNext)
    {
        EmergencyJumpStubReserve * pList = *ppPrev;

        if (loAddr <= pList->m_ptr &&
            pList->m_ptr + pList->m_size < hiAddr)
        {
            *ppPrev = pList->m_pNext;

            BYTE * pBlock = pList->m_ptr;
            *pReserveSize = pList->m_size;

            delete pList;

            return pBlock;
        }
    }

    return NULL;
}

VOID EEJitManager::EnsureJumpStubReserve(BYTE * pImageBase, SIZE_T imageSize, SIZE_T reserveSize)
{
    CONTRACTL {
        THROWS;
        GC_NOTRIGGER;
    } CONTRACTL_END;

    CrstHolder ch(&m_CodeHeapLock);

    BYTE * loAddr = pImageBase + imageSize + INT32_MIN;
    if (loAddr > pImageBase) loAddr = NULL; // overflow

    BYTE * hiAddr = pImageBase + INT32_MAX;
    if (hiAddr < pImageBase) hiAddr = (BYTE *)UINT64_MAX; // overflow

    for (EmergencyJumpStubReserve * pList = m_pEmergencyJumpStubReserveList; pList != NULL; pList = pList->m_pNext)
    {
        if (loAddr <= pList->m_ptr &&
            pList->m_ptr + pList->m_size < hiAddr)
        {
            SIZE_T used = min(reserveSize, pList->m_free);
            pList->m_free -= used;

            reserveSize -= used;
            if (reserveSize == 0)
                return;
        }
    }

    // Try several different strategies - the most efficient one first
    int allocMode = 0;

    // Try to reserve at least 16MB at a time
    SIZE_T allocChunk = max<SIZE_T>(ALIGN_UP(reserveSize, VIRTUAL_ALLOC_RESERVE_GRANULARITY), 16*1024*1024);

    while (reserveSize > 0)
    {
        NewHolder<EmergencyJumpStubReserve> pNewReserve(new EmergencyJumpStubReserve());

        while (true)
        {
            BYTE * loAddrCurrent = loAddr;
            BYTE * hiAddrCurrent = hiAddr;

            switch (allocMode)
            {
            case 0:
                // First, try to allocate towards the center of the allowed range. It is more likely to
                // satisfy subsequent reservations.
                loAddrCurrent = loAddr + (hiAddr - loAddr) / 8;
                hiAddrCurrent = hiAddr - (hiAddr - loAddr) / 8;
                break;
            case 1:
                // Try the whole allowed range
                break;
            case 2:
                // If the large allocation failed, retry with small chunk size
                allocChunk = VIRTUAL_ALLOC_RESERVE_GRANULARITY;
                break;
            default:
                return; // Unable to allocate the reserve - give up
            }

            pNewReserve->m_ptr = (BYTE*)ExecutableAllocator::Instance()->ReserveWithinRange(allocChunk, loAddrCurrent, hiAddrCurrent);

            if (pNewReserve->m_ptr != NULL)
                break;

            // Retry with the next allocation strategy
            allocMode++;
        }

        SIZE_T used = min(allocChunk, reserveSize);
        reserveSize -= used;

        pNewReserve->m_size = allocChunk;
        pNewReserve->m_free = allocChunk - used;

        // Add it to the list
        pNewReserve->m_pNext = m_pEmergencyJumpStubReserveList;
        m_pEmergencyJumpStubReserveList = pNewReserve.Extract();
    }
}
#endif // TARGET_AMD64

static size_t GetDefaultReserveForJumpStubs(size_t codeHeapSize)
{
    LIMITED_METHOD_CONTRACT;

#ifdef TARGET_64BIT
    //
    // Keep a small default reserve at the end of the codeheap for jump stubs. It should reduce
    // chance that we won't be able allocate jump stub because of lack of suitable address space.
    //
    static ConfigDWORD configCodeHeapReserveForJumpStubs;
    int percentReserveForJumpStubs = configCodeHeapReserveForJumpStubs.val(CLRConfig::INTERNAL_CodeHeapReserveForJumpStubs);

    size_t reserveForJumpStubs = percentReserveForJumpStubs * (codeHeapSize / 100);

    size_t minReserveForJumpStubs = sizeof(CodeHeader) +
        sizeof(JumpStubBlockHeader) + (size_t) DEFAULT_JUMPSTUBS_PER_BLOCK * BACK_TO_BACK_JUMP_ALLOCATE_SIZE +
        CODE_SIZE_ALIGN + BYTES_PER_BUCKET;

    return max(reserveForJumpStubs, minReserveForJumpStubs);
#else
    return 0;
#endif
}

HeapList* LoaderCodeHeap::CreateCodeHeap(CodeHeapRequestInfo *pInfo, LoaderHeap *pJitMetaHeap)
{
    CONTRACT(HeapList *) {
        THROWS;
        GC_NOTRIGGER;
        POSTCONDITION((RETVAL != NULL) || !pInfo->getThrowOnOutOfMemoryWithinRange());
    } CONTRACT_END;

    size_t   reserveSize        = pInfo->getReserveSize();
    size_t   initialRequestSize = pInfo->getRequestSize();
    const BYTE *   loAddr       = pInfo->m_loAddr;
    const BYTE *   hiAddr       = pInfo->m_hiAddr;

    // Make sure that what we are reserving will fix inside a DWORD
    if (reserveSize != (DWORD) reserveSize)
    {
        _ASSERTE(!"reserveSize does not fit in a DWORD");
        EEPOLICY_HANDLE_FATAL_ERROR(COR_E_EXECUTIONENGINE);
    }

    LOG((LF_JIT, LL_INFO100,
         "Request new LoaderCodeHeap::CreateCodeHeap(%08x, %08x, %sexecutable, for loader allocator" FMT_ADDR "in" FMT_ADDR ".." FMT_ADDR ")\n",
         (DWORD) reserveSize, (DWORD) initialRequestSize, pInfo->IsInterpreted() ? "non-" : "", DBG_ADDR(pInfo->m_pAllocator), DBG_ADDR(loAddr), DBG_ADDR(hiAddr)
                                ));

    NewHolder<LoaderCodeHeap> pCodeHeap(new LoaderCodeHeap(!pInfo->IsInterpreted() /* fMakeExecutable */));

    BYTE * pBaseAddr = NULL;
    DWORD dwSizeAcquiredFromInitialBlock = 0;
    bool fAllocatedFromEmergencyJumpStubReserve = false;

    size_t allocationSize = pCodeHeap->m_LoaderHeap.AllocMem_TotalSize(initialRequestSize);
#if defined(TARGET_64BIT)
    if (!pInfo->IsInterpreted())
    {
        allocationSize += pCodeHeap->m_LoaderHeap.AllocMem_TotalSize(JUMP_ALLOCATE_SIZE);
    }
#endif
    pBaseAddr = (BYTE *)pInfo->m_pAllocator->GetCodeHeapInitialBlock(loAddr, hiAddr, (DWORD)allocationSize, &dwSizeAcquiredFromInitialBlock);
    if (pBaseAddr != NULL)
    {
        pCodeHeap->m_LoaderHeap.SetReservedRegion(pBaseAddr, dwSizeAcquiredFromInitialBlock, FALSE);
    }
    else
    {
        // Include internal CodeHeap structures in the reserve
        allocationSize = ALIGN_UP(allocationSize, VIRTUAL_ALLOC_RESERVE_GRANULARITY);
        reserveSize = max(reserveSize, allocationSize);

        if (reserveSize != (DWORD) reserveSize)
        {
            _ASSERTE(!"reserveSize does not fit in a DWORD");
            EEPOLICY_HANDLE_FATAL_ERROR(COR_E_EXECUTIONENGINE);
        }

        if (loAddr != NULL || hiAddr != NULL)
        {
#ifdef _DEBUG
            // Always exercise the fallback path in the caller when forced relocs are turned on
            if (!pInfo->getThrowOnOutOfMemoryWithinRange() && PEDecoder::GetForceRelocs())
                RETURN NULL;
#endif
            pBaseAddr = (BYTE*)ExecutableAllocator::Instance()->ReserveWithinRange(reserveSize, loAddr, hiAddr);

            if (!pBaseAddr)
            {
                // Conserve emergency jump stub reserve until when it is really needed
                if (!pInfo->getThrowOnOutOfMemoryWithinRange())
                    RETURN NULL;
#ifdef TARGET_AMD64
                pBaseAddr = ExecutionManager::GetEEJitManager()->AllocateFromEmergencyJumpStubReserve(loAddr, hiAddr, &reserveSize);
                if (!pBaseAddr)
                    ThrowOutOfMemoryWithinRange();
                fAllocatedFromEmergencyJumpStubReserve = true;
#else
                ThrowOutOfMemoryWithinRange();
#endif // TARGET_AMD64
            }
        }
        else
        {
            pBaseAddr = (BYTE*)ExecutableAllocator::Instance()->Reserve(reserveSize);
            if (!pBaseAddr)
                ThrowOutOfMemory();
        }
        pCodeHeap->m_LoaderHeap.SetReservedRegion(pBaseAddr, reserveSize, TRUE);
    }


    // this first allocation is critical as it sets up correctly the loader heap info
    HeapList *pHp = new HeapList;

#if defined(TARGET_64BIT)
    if (pInfo->IsInterpreted())
    {
        pHp->CLRPersonalityRoutine = NULL;
        pCodeHeap->m_LoaderHeap.ReservePages(1);
    }
    else
    {
        // Set the personality routine. This is needed even outside Windows because IsIPInEpilog relies on it being set.
        pHp->CLRPersonalityRoutine = (BYTE *)pCodeHeap->m_LoaderHeap.AllocMemForCode_NoThrow(0, JUMP_ALLOCATE_SIZE, sizeof(void*), 0);
    }
#else
    // Ensure that the heap has a reserved block of memory and so the GetReservedBytesFree()
    // and GetAllocPtr() calls below return nonzero values.
    pCodeHeap->m_LoaderHeap.ReservePages(1);
#endif

    pHp->pHeap = pCodeHeap;

    size_t heapSize = pCodeHeap->m_LoaderHeap.GetReservedBytesFree();

    pHp->startAddress = (TADDR)pCodeHeap->m_LoaderHeap.GetAllocPtr();

    pHp->endAddress      = pHp->startAddress;
    pHp->maxCodeHeapSize = heapSize;
    if (pInfo->IsInterpreted())
    {
        pHp->reserveForJumpStubs = 0;
    }
    else
    {
        pHp->reserveForJumpStubs = fAllocatedFromEmergencyJumpStubReserve ? pHp->maxCodeHeapSize : GetDefaultReserveForJumpStubs(pHp->maxCodeHeapSize);
    }

    _ASSERTE(heapSize >= initialRequestSize);

    // We do not need to memset this memory, since ClrVirtualAlloc() guarantees that the memory is zero.
    // Furthermore, if we avoid writing to it, these pages don't come into our working set

    pHp->mapBase         = ROUND_DOWN_TO_PAGE(pHp->startAddress);  // round down to next lower page align
    size_t nibbleMapSize = HEAP2MAPSIZE(ROUND_UP_TO_PAGE(heapSize));
    pHp->pHdrMap         = (DWORD*)(void*)pJitMetaHeap->AllocMem(S_SIZE_T(nibbleMapSize));
#if defined(TARGET_64BIT)
    if (pHp->CLRPersonalityRoutine != NULL)
    {
        ExecutableWriterHolder<BYTE> personalityRoutineWriterHolder(pHp->CLRPersonalityRoutine, 12);
        emitJump(pHp->CLRPersonalityRoutine, personalityRoutineWriterHolder.GetRW(), (void *)ProcessCLRException);
    }
#endif // TARGET_64BIT

    pHp->pLoaderAllocator = pInfo->m_pAllocator;

    LOG((LF_JIT, LL_INFO100,
         "Created new CodeHeap(" FMT_ADDR ".." FMT_ADDR ")\n",
         DBG_ADDR(pHp->startAddress), DBG_ADDR(pHp->startAddress+pHp->maxCodeHeapSize)
         ));

    pCodeHeap.SuppressRelease();
    RETURN pHp;
}

void * LoaderCodeHeap::AllocMemForCode_NoThrow(size_t header, size_t size, DWORD alignment, size_t reserveForJumpStubs)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
    } CONTRACTL_END;

    if (m_cbMinNextPad > (SSIZE_T)header) header = m_cbMinNextPad;

    void * p = m_LoaderHeap.AllocMemForCode_NoThrow(header, size, alignment, reserveForJumpStubs);
    if (p == NULL)
        return NULL;

    // If the next allocation would have started in the same nibble map entry, allocate extra space to prevent it from happening
    // Note that m_cbMinNextPad can be negative
    m_cbMinNextPad = ALIGN_UP((SIZE_T)p + 1, BYTES_PER_BUCKET) - ((SIZE_T)p + size);

    return p;
}

void CodeHeapRequestInfo::Init()
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        PRECONDITION((m_hiAddr == 0) ||
                     ((m_loAddr < m_hiAddr) &&
                      ((m_loAddr + m_requestSize) < m_hiAddr)));
    } CONTRACTL_END;

    if (m_pAllocator == NULL)
        m_pAllocator = m_pMD->GetLoaderAllocator();
    m_isDynamicDomain = (m_pMD != NULL) && m_pMD->IsLCGMethod();
    m_isCollectible = m_pAllocator->IsCollectible();
    m_throwOnOutOfMemoryWithinRange = true;
}

#ifdef FEATURE_EH_FUNCLETS

#ifdef HOST_64BIT
extern "C" PT_RUNTIME_FUNCTION GetRuntimeFunctionCallback(IN ULONG64   ControlPc,
                                                        IN PVOID     Context)
#else
extern "C" PT_RUNTIME_FUNCTION GetRuntimeFunctionCallback(IN ULONG     ControlPc,
                                                        IN PVOID     Context)
#endif
{
    WRAPPER_NO_CONTRACT;

    PT_RUNTIME_FUNCTION prf = NULL;

    // We must preserve this so that GCStress=4 eh processing doesnt kill last error.
    PreserveLastErrorHolder preserveLastError;

#ifdef ENABLE_CONTRACTS
    // Some 64-bit OOM tests use the hosting interface to re-enter the CLR via
    // RtlVirtualUnwind to track unique stacks at each failure point. RtlVirtualUnwind can
    // result in the EEJitManager taking a reader lock. This, in turn, results in a
    // CANNOT_TAKE_LOCK contract violation if a CANNOT_TAKE_LOCK function were on the stack
    // at the time. While it's theoretically possible for "real" hosts also to re-enter the
    // CLR via RtlVirtualUnwind, generally they don't, and we'd actually like to catch a real
    // host causing such a contract violation. Therefore, we'd like to suppress such contract
    // asserts when these OOM tests are running, but continue to enforce the contracts by
    // default. This function returns whether to suppress locking violations.
    CONDITIONAL_CONTRACT_VIOLATION(
        TakesLockViolation,
        g_pConfig->SuppressLockViolationsOnReentryFromOS());
#endif // ENABLE_CONTRACTS

    EECodeInfo codeInfo((PCODE)ControlPc);
    if (codeInfo.IsValid())
        prf = codeInfo.GetFunctionEntry();

    LOG((LF_EH, LL_INFO1000000, "GetRuntimeFunctionCallback(%p) returned %p\n", ControlPc, prf));

    return  prf;
}
#endif // FEATURE_EH_FUNCLETS

HeapList* EECodeGenManager::NewCodeHeap(CodeHeapRequestInfo *pInfo, DomainCodeHeapList *pADHeapList)
{
    CONTRACT(HeapList *) {
        THROWS;
        GC_NOTRIGGER;
        PRECONDITION(m_CodeHeapLock.OwnedByCurrentThread());
        POSTCONDITION((RETVAL != NULL) || !pInfo->getThrowOnOutOfMemoryWithinRange());
    } CONTRACT_END;

    size_t initialRequestSize = pInfo->getRequestSize();
    size_t minReserveSize = VIRTUAL_ALLOC_RESERVE_GRANULARITY; //     ( 64 KB)

#ifdef HOST_64BIT
    if (pInfo->m_hiAddr == 0)
    {
        if (pADHeapList->m_CodeHeapList.Count() > CODE_HEAP_SIZE_INCREASE_THRESHOLD)
        {
            minReserveSize *= 4; // Increase the code heap size to 256 KB for workloads with a lot of code.
        }

        // For non-DynamicDomains that don't have a loAddr/hiAddr range
        // we bump up the reserve size for the 64-bit platforms
        if (!pInfo->IsDynamicDomain())
        {
            minReserveSize *= 8; // CodeHeaps are larger on AMD64 (256 KB to 2048 KB)
        }
    }
#endif

    size_t reserveSize = initialRequestSize;

#if defined(TARGET_64BIT)
    if (!pInfo->IsInterpreted())
    {
        reserveSize += JUMP_ALLOCATE_SIZE;
    }
#endif

    if (reserveSize < minReserveSize)
        reserveSize = minReserveSize;
    reserveSize = ALIGN_UP(reserveSize, VIRTUAL_ALLOC_RESERVE_GRANULARITY);

    pInfo->setReserveSize(reserveSize);

    HeapList *pHp = NULL;

    DWORD flags = RangeSection::RANGE_SECTION_CODEHEAP;

    if (pInfo->IsInterpreted())
    {
        flags |= RangeSection::RANGE_SECTION_INTERPRETER;
    }

    if (pInfo->IsDynamicDomain())
    {
        flags |= RangeSection::RANGE_SECTION_COLLECTIBLE;
        pHp = HostCodeHeap::CreateCodeHeap(pInfo, this);
    }
    else
    {
        LoaderHeap *pJitMetaHeap = pADHeapList->m_pAllocator->GetLowFrequencyHeap();

        if (pInfo->IsCollectible())
            flags |= RangeSection::RANGE_SECTION_COLLECTIBLE;

        pHp = LoaderCodeHeap::CreateCodeHeap(pInfo, pJitMetaHeap);
    }
    if (pHp == NULL)
    {
        _ASSERTE(!pInfo->getThrowOnOutOfMemoryWithinRange());
        RETURN(NULL);
    }

    _ASSERTE (pHp != NULL);
    _ASSERTE (pHp->maxCodeHeapSize >= initialRequestSize);

    // Append the current code heap to the new code heap element.
    pHp->SetNext(m_pAllCodeHeaps);

    EX_TRY
    {
        TADDR pStartRange = pHp->GetModuleBase();
        TADDR pEndRange = (TADDR) &((BYTE*)pHp->startAddress)[pHp->maxCodeHeapSize];

        ExecutionManager::AddCodeRange(pStartRange,
                                       pEndRange,
                                       this,
                                       (RangeSection::RangeSectionFlags)flags,
                                       pHp);

        if (!pInfo->IsInterpreted())
        {
            //
            // add a table to cover each range in the range list
            //
            InstallEEFunctionTable(
                    (PVOID)pStartRange,   // this is just an ID that gets passed to RtlDeleteFunctionTable;
                    (PVOID)pStartRange,
                    (ULONG)((ULONG64)pEndRange - (ULONG64)pStartRange),
                    GetRuntimeFunctionCallback,
                    this);
        }
    }
    EX_CATCH
    {
        // If we failed to alloc memory in ExecutionManager::AddCodeRange()
        // then we will delete the LoaderHeap that we allocated

        delete pHp->pHeap;
        delete pHp;

        pHp = NULL;
    }
    EX_END_CATCH

    if (pHp == NULL)
    {
        ThrowOutOfMemory();
    }

    m_pAllCodeHeaps = pHp;

    HeapList **ppHeapList = pADHeapList->m_CodeHeapList.AppendThrowing();
    *ppHeapList = pHp;

    RETURN(pHp);
}

void* EECodeGenManager::AllocCodeWorker(CodeHeapRequestInfo *pInfo,
                                     size_t header, size_t blockSize, unsigned align,
                                     HeapList ** ppCodeHeap)
{
    CONTRACT(void *) {
        THROWS;
        GC_NOTRIGGER;
        PRECONDITION(m_CodeHeapLock.OwnedByCurrentThread());
        POSTCONDITION((RETVAL != NULL) || !pInfo->getThrowOnOutOfMemoryWithinRange());
    } CONTRACT_END;

    pInfo->setRequestSize(header+blockSize+(align-1)+pInfo->getReserveForJumpStubs());

    void *      mem         = NULL;
    HeapList * pCodeHeap    = NULL;
    DomainCodeHeapList *pList = NULL;

    // Avoid going through the full list in the common case - try to use the most recently used codeheap
    if (pInfo->IsDynamicDomain())
    {
#ifdef FEATURE_INTERPRETER
        if (pInfo->IsInterpreted())
        {
            pCodeHeap = (HeapList *)pInfo->m_pAllocator->m_pLastUsedInterpreterDynamicCodeHeap;
            pInfo->m_pAllocator->m_pLastUsedInterpreterDynamicCodeHeap = NULL;
        }
        else
#endif // FEATURE_INTERPRETER
        {
            pCodeHeap = (HeapList *)pInfo->m_pAllocator->m_pLastUsedDynamicCodeHeap;
            pInfo->m_pAllocator->m_pLastUsedDynamicCodeHeap = NULL;
        }
    }
    else
    {
#ifdef FEATURE_INTERPRETER
        if (pInfo->IsInterpreted())
        {
            pCodeHeap = (HeapList *)pInfo->m_pAllocator->m_pLastUsedInterpreterCodeHeap;
            pInfo->m_pAllocator->m_pLastUsedInterpreterCodeHeap = NULL;
        }
        else
#endif // FEATURE_INTERPRETER
        {
            pCodeHeap = (HeapList *)pInfo->m_pAllocator->m_pLastUsedCodeHeap;
            pInfo->m_pAllocator->m_pLastUsedCodeHeap = NULL;
        }
    }

    // If we will use a cached code heap, ensure that the code heap meets the constraints
    if (pCodeHeap && CanUseCodeHeap(pInfo, pCodeHeap))
    {
        mem = (pCodeHeap->pHeap)->AllocMemForCode_NoThrow(header, blockSize, align, pInfo->getReserveForJumpStubs());
    }

    if (mem == NULL)
    {
        pList = GetCodeHeapList(pInfo, pInfo->m_pAllocator);
        if (pList != NULL)
        {
            for (int i = 0; i < pList->m_CodeHeapList.Count(); i++)
            {
                pCodeHeap = pList->m_CodeHeapList[i];

                // Validate that the code heap can be used for the current request
                if (CanUseCodeHeap(pInfo, pCodeHeap))
                {
                    mem = (pCodeHeap->pHeap)->AllocMemForCode_NoThrow(header, blockSize, align, pInfo->getReserveForJumpStubs());
                    if (mem != NULL)
                        break;
                }
            }
        }

        if (mem == NULL)
        {
            // Let us create a new heap.
            if (pList == NULL)
            {
                // not found so need to create the first one
                pList = CreateCodeHeapList(pInfo);
                _ASSERTE(pList == GetCodeHeapList(pInfo, pInfo->m_pAllocator));
            }
            _ASSERTE(pList);

            pCodeHeap = NewCodeHeap(pInfo, pList);
            if (pCodeHeap == NULL)
            {
                _ASSERTE(!pInfo->getThrowOnOutOfMemoryWithinRange());
                RETURN(NULL);
            }

            mem = (pCodeHeap->pHeap)->AllocMemForCode_NoThrow(header, blockSize, align, pInfo->getReserveForJumpStubs());
            if (mem == NULL)
                ThrowOutOfMemory();
            _ASSERTE(mem);
        }
    }

    if (pInfo->IsDynamicDomain())
    {
#ifdef FEATURE_INTERPRETER
        if (pInfo->IsInterpreted())
        {
            pInfo->m_pAllocator->m_pLastUsedInterpreterDynamicCodeHeap = pCodeHeap;
        }
        else
#endif // FEATURE_INTERPRETER
        {
            pInfo->m_pAllocator->m_pLastUsedDynamicCodeHeap = pCodeHeap;
        }
    }
    else
    {
#ifdef FEATURE_INTERPRETER
        if (pInfo->IsInterpreted())
        {
            pInfo->m_pAllocator->m_pLastUsedInterpreterCodeHeap = pCodeHeap;
        }
        else
#endif // FEATURE_INTERPRETER
        {
            pInfo->m_pAllocator->m_pLastUsedCodeHeap = pCodeHeap;
        }
    }

    // Record the pCodeHeap value into ppCodeHeap
    *ppCodeHeap = pCodeHeap;

    _ASSERTE((TADDR)mem >= pCodeHeap->startAddress);

    if (((TADDR) mem)+blockSize > (TADDR)pCodeHeap->endAddress)
    {
        // Update the CodeHeap endAddress
        pCodeHeap->endAddress = (TADDR)mem+blockSize;
    }

    RETURN(mem);
}

template<typename TCodeHeader>
void EECodeGenManager::AllocCode(MethodDesc* pMD, size_t blockSize, size_t reserveForJumpStubs, CorJitAllocMemFlag flag, void** ppCodeHeader, void** ppCodeHeaderRW,
                                 size_t* pAllocatedSize, HeapList** ppCodeHeap
                               , BYTE** ppRealHeader
#ifdef FEATURE_EH_FUNCLETS
                               , UINT nUnwindInfos
#endif
                                )
{
    CONTRACTL {
        THROWS;
        GC_NOTRIGGER;
    } CONTRACTL_END;

    //
    // Alignment
    //

    unsigned alignment = CODE_SIZE_ALIGN;

    if ((flag & CORJIT_ALLOCMEM_FLG_32BYTE_ALIGN) != 0)
    {
        alignment = max(alignment, 32u);
    }
    else if ((flag & CORJIT_ALLOCMEM_FLG_16BYTE_ALIGN) != 0)
    {
        alignment = max(alignment, 16u);
    }

#if defined(TARGET_X86)
    // when not optimizing for code size, 8-byte align the method entry point, so that
    // the JIT can in turn 8-byte align the loop entry headers.
    else if ((g_pConfig->GenOptimizeType() != OPT_SIZE))
    {
        alignment = max(alignment, 8u);
    }
#endif

    //
    // Compute header layout
    //

    SIZE_T totalSize = blockSize;

    TCodeHeader * pCodeHdr = NULL;
    TCodeHeader * pCodeHdrRW = NULL;

    CodeHeapRequestInfo requestInfo(pMD);
    SIZE_T realHeaderSize;
#ifdef FEATURE_INTERPRETER
    if (std::is_same<TCodeHeader, InterpreterCodeHeader>::value)
    {
#ifdef FEATURE_EH_FUNCLETS
        _ASSERTE(nUnwindInfos == 0);
#endif
        _ASSERTE(reserveForJumpStubs == 0);
        requestInfo.SetInterpreted();
        realHeaderSize = sizeof(InterpreterRealCodeHeader);
    }
    else
#endif // FEATURE_INTERPRETER
    {
        requestInfo.setReserveForJumpStubs(reserveForJumpStubs);

#ifdef FEATURE_EH_FUNCLETS
        realHeaderSize = offsetof(RealCodeHeader, unwindInfos[0]) + (sizeof(T_RUNTIME_FUNCTION) * nUnwindInfos);
#else
        realHeaderSize = sizeof(RealCodeHeader);
#endif
    }

    // if this is a LCG method then we will be allocating the RealCodeHeader
    // following the code so that the code block can be removed easily by
    // the LCG code heap.
    if (requestInfo.IsDynamicDomain())
    {
        totalSize = ALIGN_UP(totalSize, sizeof(void*)) + realHeaderSize;
        static_assert_no_msg(CODE_SIZE_ALIGN >= sizeof(void*));
    }

    // Scope the lock
    {
        CrstHolder ch(&m_CodeHeapLock);

        // Allocate the record code pointer early to avoid allocation failures.
        void* dummyRecordedCodePtr;
        void** recordedCodePtr = pMD->IsLCGMethod()
            ? pMD->AsDynamicMethodDesc()->GetLCGMethodResolver()->AllocateRecordCodePointer()
            : &dummyRecordedCodePtr;

        *ppCodeHeap = NULL;
        TADDR pCode = (TADDR) AllocCodeWorker(&requestInfo, sizeof(TCodeHeader), totalSize, alignment, ppCodeHeap);
        _ASSERTE(*ppCodeHeap);
        _ASSERTE(IS_ALIGNED(pCode, alignment));

        // Record the code pointer
        *recordedCodePtr = (void*)pCode;

        pCodeHdr = ((TCodeHeader *)pCode) - 1;

        *pAllocatedSize = sizeof(TCodeHeader) + totalSize;

        if (ExecutableAllocator::IsWXORXEnabled()
#ifdef FEATURE_INTERPRETER
            && !std::is_same<TCodeHeader, InterpreterCodeHeader>::value
#endif // FEATURE_INTERPRETER
        )
        {
            pCodeHdrRW = (TCodeHeader *)new BYTE[*pAllocatedSize];
        }
        else
        {
            pCodeHdrRW = pCodeHdr;
        }

        if (requestInfo.IsDynamicDomain())
        {
            // Set the real code header to the writeable mapping so that we can set its members via the CodeHeader methods below
            pCodeHdrRW->SetRealCodeHeader((BYTE *)(pCodeHdrRW + 1) + ALIGN_UP(blockSize, sizeof(void*)));
        }
        else
        {
            // TODO: think about the CodeHeap carrying around a RealCodeHeader chunking mechanism
            //
            // allocate the real header in the low frequency heap
            BYTE* pRealHeader = (BYTE*)(void*)pMD->GetLoaderAllocator()->GetLowFrequencyHeap()->AllocMem(S_SIZE_T(realHeaderSize));
            pCodeHdrRW->SetRealCodeHeader(pRealHeader);
        }

        pCodeHdrRW->SetDebugInfo(NULL);
        pCodeHdrRW->SetEHInfo(NULL);
        pCodeHdrRW->SetGCInfo(NULL);
        pCodeHdrRW->SetMethodDesc(pMD);
#ifdef FEATURE_EH_FUNCLETS
        if (std::is_same<TCodeHeader, CodeHeader>::value)
        {
            ((CodeHeader*)pCodeHdrRW)->SetNumberOfUnwindInfos(nUnwindInfos);
        }
#endif

        if (requestInfo.IsDynamicDomain())
        {
            *ppRealHeader = (BYTE*)pCode + ALIGN_UP(blockSize, sizeof(void*));
        }
        else
        {
            *ppRealHeader = NULL;
        }
    }

    *ppCodeHeader = pCodeHdr;
    *ppCodeHeaderRW = pCodeHdrRW;
}

template void EECodeGenManager::AllocCode<CodeHeader>(MethodDesc* pMD, size_t blockSize, size_t reserveForJumpStubs, CorJitAllocMemFlag flag, void** ppCodeHeader, void** ppCodeHeaderRW,
                                                      size_t* pAllocatedSize, HeapList** ppCodeHeap
                                                    , BYTE** ppRealHeader
#ifdef FEATURE_EH_FUNCLETS
                                                    , UINT nUnwindInfos
#endif
                                                     );

#ifdef FEATURE_INTERPRETER
template void EECodeGenManager::AllocCode<InterpreterCodeHeader>(MethodDesc* pMD, size_t blockSize, size_t reserveForJumpStubs, CorJitAllocMemFlag flag, void** ppCodeHeader, void** ppCodeHeaderRW,
                                                                 size_t* pAllocatedSize, HeapList** ppCodeHeap
                                                               , BYTE** ppRealHeader
#ifdef FEATURE_EH_FUNCLETS
                                                               , UINT nUnwindInfos
#endif
                                                                );
#endif // FEATURE_INTERPRETER

EEJitManager::DomainCodeHeapList *EECodeGenManager::GetCodeHeapList(CodeHeapRequestInfo *pInfo, LoaderAllocator *pAllocator, BOOL fDynamicOnly)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        PRECONDITION(m_CodeHeapLock.OwnedByCurrentThread());
    } CONTRACTL_END;

    DomainCodeHeapList *pList = NULL;
    DomainCodeHeapList **ppList = NULL;
    int count = 0;

    // get the appropriate list of heaps
    // pMD is NULL for NGen modules during Module::LoadTokenTables
    if (fDynamicOnly || (pInfo != NULL && pInfo->IsDynamicDomain()))
    {
        ppList = m_DynamicDomainCodeHeaps.Table();
        count = m_DynamicDomainCodeHeaps.Count();
    }
    else
    {
        ppList = m_DomainCodeHeaps.Table();
        count = m_DomainCodeHeaps.Count();
    }

    // this is a virtual call - pull it out of the loop
    BOOL fCanUnload = pAllocator->CanUnload();

    // look for a DomainCodeHeapList
    for (int i=0; i < count; i++)
    {
        if (ppList[i]->m_pAllocator == pAllocator ||
            (!fCanUnload && !ppList[i]->m_pAllocator->CanUnload()))
        {
            pList = ppList[i];
            break;
        }
    }
    return pList;
}

bool EECodeGenManager::CanUseCodeHeap(CodeHeapRequestInfo *pInfo, HeapList *pCodeHeap)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        PRECONDITION(m_CodeHeapLock.OwnedByCurrentThread());
    } CONTRACTL_END;

    if ((pInfo->m_loAddr == 0) && (pInfo->m_hiAddr == 0))
    {
        // We have no constraint so this non empty heap will be able to satisfy our request
        if (pInfo->IsDynamicDomain())
        {
            _ASSERTE(pCodeHeap->reserveForJumpStubs == 0);
            return true;
        }
        else
        {
            BYTE * lastAddr = (BYTE *) pCodeHeap->startAddress + pCodeHeap->maxCodeHeapSize;

            BYTE * loRequestAddr  = (BYTE *) pCodeHeap->endAddress;
            BYTE * hiRequestAddr = loRequestAddr + pInfo->getRequestSize() + BYTES_PER_BUCKET;
            if (hiRequestAddr <= lastAddr - pCodeHeap->reserveForJumpStubs)
            {
                return true;
            }
        }
    }
    else
    {
        // We also check to see if an allocation in this heap would satisfy
        // the [loAddr..hiAddr] requirement

        // Calculate the byte range that can ever be returned by
        // an allocation in this HeapList element
        //
        BYTE * firstAddr      = (BYTE *) pCodeHeap->startAddress;
        BYTE * lastAddr       = (BYTE *) pCodeHeap->startAddress + pCodeHeap->maxCodeHeapSize;

        _ASSERTE(pCodeHeap->startAddress <= pCodeHeap->endAddress);
        _ASSERTE(firstAddr <= lastAddr);

        if (pInfo->IsDynamicDomain())
        {
            _ASSERTE(pCodeHeap->reserveForJumpStubs == 0);

            // We check to see if every allocation in this heap
            // will satisfy the [loAddr..hiAddr] requirement.
            //
            // Dynamic domains use a free list allocator,
            // thus we can receive any address in the range
            // when calling AllocMemory with a DynamicDomain

            // [firstaddr .. lastAddr] must be entirely within
            // [pInfo->m_loAddr .. pInfo->m_hiAddr]
            //
            if ((pInfo->m_loAddr <= firstAddr)   &&
                (lastAddr        <= pInfo->m_hiAddr))
            {
                // This heap will always satisfy our constraint
                return true;
            }
        }
        else // non-DynamicDomain
        {
            // Calculate the byte range that would be allocated for the
            // next allocation request into [loRequestAddr..hiRequestAddr]
            //
            BYTE * loRequestAddr  = (BYTE *) pCodeHeap->endAddress;
            BYTE * hiRequestAddr  = loRequestAddr + pInfo->getRequestSize() + BYTES_PER_BUCKET;
            _ASSERTE(loRequestAddr <= hiRequestAddr);

            // loRequestAddr and hiRequestAddr must be entirely within
            // [pInfo->m_loAddr .. pInfo->m_hiAddr]
            //
            if ((pInfo->m_loAddr <= loRequestAddr)   &&
                (hiRequestAddr   <= pInfo->m_hiAddr))
            {
                // Additionally hiRequestAddr must also be less than or equal to lastAddr.
                // If throwOnOutOfMemoryWithinRange is not set, conserve reserveForJumpStubs until when it is really needed.
                if (hiRequestAddr <= lastAddr - (pInfo->getThrowOnOutOfMemoryWithinRange() ? 0 : pCodeHeap->reserveForJumpStubs))
                {
                    // This heap will be able to satisfy our constraint
                    return true;
                }
            }
       }
    }

    return false;
}

EEJitManager::DomainCodeHeapList* EECodeGenManager::CreateCodeHeapList(CodeHeapRequestInfo *pInfo)
{
    CONTRACTL
    {
        THROWS;
        GC_NOTRIGGER;
        PRECONDITION(m_CodeHeapLock.OwnedByCurrentThread());
    } CONTRACTL_END;

    NewHolder<DomainCodeHeapList> pNewList(new DomainCodeHeapList(pInfo->m_pAllocator));

    DomainCodeHeapList** ppList = pInfo->IsDynamicDomain()
        ? m_DynamicDomainCodeHeaps.AppendThrowing()
        : m_DomainCodeHeaps.AppendThrowing();
    *ppList = pNewList;

    return pNewList.Extract();
}

LoaderHeap *EECodeGenManager::GetJitMetaHeap(MethodDesc *pMD)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
    } CONTRACTL_END;

    LoaderAllocator *pAllocator = pMD->GetLoaderAllocator();
    _ASSERTE(pAllocator);

    return pAllocator->GetLowFrequencyHeap();
}

JumpStubBlockHeader *  EEJitManager::AllocJumpStubBlock(MethodDesc* pMD, DWORD numJumps,
                                                        BYTE * loAddr, BYTE * hiAddr,
                                                        LoaderAllocator *pLoaderAllocator,
                                                        bool throwOnOutOfMemoryWithinRange)
{
    CONTRACT(JumpStubBlockHeader *) {
        THROWS;
        GC_NOTRIGGER;
        PRECONDITION(loAddr < hiAddr);
        PRECONDITION(pLoaderAllocator != NULL);
        POSTCONDITION((RETVAL != NULL) || !throwOnOutOfMemoryWithinRange);
    } CONTRACT_END;

    _ASSERTE((sizeof(JumpStubBlockHeader) % CODE_SIZE_ALIGN) == 0);

    size_t blockSize = sizeof(JumpStubBlockHeader) + (size_t) numJumps * BACK_TO_BACK_JUMP_ALLOCATE_SIZE;

    HeapList *pCodeHeap = NULL;
    CodeHeapRequestInfo    requestInfo(pMD, pLoaderAllocator, loAddr, hiAddr);
    requestInfo.setThrowOnOutOfMemoryWithinRange(throwOnOutOfMemoryWithinRange);

    TADDR                  mem;
    ExecutableWriterHolderNoLog<JumpStubBlockHeader> blockWriterHolder;

    // Scope the lock
    {
        CrstHolder ch(&m_CodeHeapLock);

        mem = (TADDR) AllocCodeWorker(&requestInfo, sizeof(CodeHeader), blockSize, CODE_SIZE_ALIGN, &pCodeHeap);
        if (mem == (TADDR)0)
        {
            _ASSERTE(!throwOnOutOfMemoryWithinRange);
            RETURN(NULL);
        }

        // CodeHeader comes immediately before the block
        CodeHeader * pCodeHdr = (CodeHeader *) (mem - sizeof(CodeHeader));
        ExecutableWriterHolder<CodeHeader> codeHdrWriterHolder(pCodeHdr, sizeof(CodeHeader));
        codeHdrWriterHolder.GetRW()->SetStubCodeBlockKind(STUB_CODE_BLOCK_JUMPSTUB);

        NibbleMapSetUnlocked(pCodeHeap, mem, blockSize);

        blockWriterHolder.AssignExecutableWriterHolder((JumpStubBlockHeader *)mem, sizeof(JumpStubBlockHeader));

        _ASSERTE(IS_ALIGNED(blockWriterHolder.GetRW(), CODE_SIZE_ALIGN));
    }

    ReportStubBlock((void*)mem, blockSize, STUB_CODE_BLOCK_JUMPSTUB);

    blockWriterHolder.GetRW()->m_next            = NULL;
    blockWriterHolder.GetRW()->m_used            = 0;
    blockWriterHolder.GetRW()->m_allocated       = numJumps;
    if (pMD && pMD->IsLCGMethod())
        blockWriterHolder.GetRW()->SetHostCodeHeap(static_cast<HostCodeHeap*>(pCodeHeap->pHeap));
    else
        blockWriterHolder.GetRW()->SetLoaderAllocator(pLoaderAllocator);

    LOG((LF_JIT, LL_INFO1000, "Allocated new JumpStubBlockHeader for %d stubs at" FMT_ADDR " in loader allocator " FMT_ADDR "\n",
         numJumps, DBG_ADDR(mem) , DBG_ADDR(pLoaderAllocator) ));

    RETURN((JumpStubBlockHeader*)mem);
}

void * EEJitManager::AllocCodeFragmentBlock(size_t blockSize, unsigned alignment, LoaderAllocator *pLoaderAllocator, StubCodeBlockKind kind)
{
    CONTRACT(void *) {
        THROWS;
        GC_NOTRIGGER;
        PRECONDITION(pLoaderAllocator != NULL);
        POSTCONDITION(CheckPointer(RETVAL));
    } CONTRACT_END;

    HeapList *pCodeHeap = NULL;
    CodeHeapRequestInfo    requestInfo(NULL, pLoaderAllocator, NULL, NULL);

#ifdef TARGET_AMD64
    // CodeFragments are pretty much always Precodes that may need to be patched with jump stubs at some point in future
    // We will assume the worst case that every FixupPrecode will need to be patched and reserve the jump stubs accordingly
    requestInfo.setReserveForJumpStubs((blockSize / 8) * JUMP_ALLOCATE_SIZE);
#endif

    TADDR                  mem;

    // Scope the lock
    {
        CrstHolder ch(&m_CodeHeapLock);

        mem = (TADDR) AllocCodeWorker(&requestInfo, sizeof(CodeHeader), blockSize, alignment, &pCodeHeap);

        // CodeHeader comes immediately before the block
        CodeHeader * pCodeHdr = (CodeHeader *) (mem - sizeof(CodeHeader));
        ExecutableWriterHolder<CodeHeader> codeHdrWriterHolder(pCodeHdr, sizeof(CodeHeader));
        codeHdrWriterHolder.GetRW()->SetStubCodeBlockKind(kind);

        NibbleMapSetUnlocked(pCodeHeap, mem, blockSize);

        // Record the jump stub reservation
        pCodeHeap->reserveForJumpStubs += requestInfo.getReserveForJumpStubs();
    }

    RETURN((void *)mem);
}

BYTE* EECodeGenManager::AllocFromJitMetaHeap(MethodDesc* pMD, size_t blockSize)
{
    CONTRACTL {
        THROWS;
        GC_NOTRIGGER;
    } CONTRACTL_END;

    BYTE *pMem = NULL;
    if (pMD->IsLCGMethod())
    {
        CrstHolder ch(&m_CodeHeapLock);
        pMem = (BYTE*)(void*)pMD->AsDynamicMethodDesc()->GetResolver()->GetJitMetaHeap()->New(blockSize);
    }
    else
    {
        pMem = (BYTE*) (void*)GetJitMetaHeap(pMD)->AllocMem(S_SIZE_T(blockSize));
    }

    return pMem;
}
#endif // !DACCESS_COMPILE

GCInfoToken EEJitManager::GetGCInfoToken(const METHODTOKEN& MethodToken)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

    // The JIT-ed code always has the current version of GCInfo
    return{ GetCodeHeader(MethodToken)->GetGCInfo(), GCINFO_VERSION };
}

#ifdef FEATURE_INTERPRETER
GCInfoToken InterpreterJitManager::GetGCInfoToken(const METHODTOKEN& MethodToken)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

    // The Interpreter IR always has the current version of GCInfo
    return{ GetCodeHeader(MethodToken)->GetGCInfo(), GCINFO_VERSION };
}
#endif // FEATURE_INTERPRETER

// creates an enumeration and returns the number of EH clauses
unsigned EEJitManager::InitializeEHEnumeration(const METHODTOKEN& MethodToken, EH_CLAUSE_ENUMERATOR* pEnumState)
{
    LIMITED_METHOD_CONTRACT;
    EE_ILEXCEPTION * EHInfo = GetCodeHeader(MethodToken)->GetEHInfo();

    pEnumState->iCurrentPos = 0;     // since the EH info is not compressed, the clause number is used to do the enumeration
    pEnumState->pExceptionClauseArray = 0;

    if (!EHInfo)
        return 0;

    pEnumState->pExceptionClauseArray = dac_cast<TADDR>(EHInfo->EHClause(0));
    return *(dac_cast<PTR_unsigned>(dac_cast<TADDR>(EHInfo) - sizeof(size_t)));
}

#ifdef FEATURE_INTERPRETER
// creates an enumeration and returns the number of EH clauses
unsigned InterpreterJitManager::InitializeEHEnumeration(const METHODTOKEN& MethodToken, EH_CLAUSE_ENUMERATOR* pEnumState)
{
    LIMITED_METHOD_CONTRACT;
    EE_ILEXCEPTION * EHInfo = GetCodeHeader(MethodToken)->GetEHInfo();

    pEnumState->iCurrentPos = 0;     // since the EH info is not compressed, the clause number is used to do the enumeration
    pEnumState->pExceptionClauseArray = 0;

    if (!EHInfo)
        return 0;

    pEnumState->pExceptionClauseArray = dac_cast<TADDR>(EHInfo->EHClause(0));
    return *(dac_cast<PTR_unsigned>(dac_cast<TADDR>(EHInfo) - sizeof(size_t)));
}
#endif // FEATURE_INTERPRETER

PTR_EXCEPTION_CLAUSE_TOKEN EECodeGenManager::GetNextEHClause(EH_CLAUSE_ENUMERATOR* pEnumState,
                              EE_ILEXCEPTION_CLAUSE* pEHClauseOut)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
    } CONTRACTL_END;

    unsigned iCurrentPos = pEnumState->iCurrentPos;
    pEnumState->iCurrentPos++;

    EE_ILEXCEPTION_CLAUSE* pClause = &(dac_cast<PTR_EE_ILEXCEPTION_CLAUSE>(pEnumState->pExceptionClauseArray)[iCurrentPos]);
    *pEHClauseOut = *pClause;
    return dac_cast<PTR_EXCEPTION_CLAUSE_TOKEN>(pClause);
}

#ifndef DACCESS_COMPILE
TypeHandle EECodeGenManager::ResolveEHClause(EE_ILEXCEPTION_CLAUSE* pEHClause,
                                         CrawlFrame *pCf)
{
    // We don't want to use a runtime contract here since this codepath is used during
    // the processing of a hard SO. Contracts use a significant amount of stack
    // which we can't afford for those cases.
    STATIC_CONTRACT_THROWS;
    STATIC_CONTRACT_GC_TRIGGERS;

    _ASSERTE(NULL != pCf);
    _ASSERTE(NULL != pEHClause);
    _ASSERTE(IsTypedHandler(pEHClause));


    TypeHandle typeHnd = TypeHandle();
    mdToken typeTok = mdTokenNil;

    // CachedTypeHandle's are filled in at JIT time, and not cached when accessed multiple times
    if (HasCachedTypeHandle(pEHClause))
    {
        return TypeHandle::FromPtr(pEHClause->TypeHandle);
    }
    else
    {
        typeTok = pEHClause->ClassToken;
    }

    MethodDesc* pMD = pCf->GetFunction();
    Module* pModule = pMD->GetModule();
    _ASSERTE(pModule != NULL);

    SigTypeContext typeContext(pMD);
    return ClassLoader::LoadTypeDefOrRefOrSpecThrowing(pModule, typeTok, &typeContext,
                                                       ClassLoader::ReturnNullIfNotFound);
}

void EEJitManager::DeleteFunctionTable(PVOID pvTableID)
{
    DeleteEEFunctionTable(pvTableID);
}

// appdomain is being unloaded, so delete any data associated with it. We have to do this in two stages.
// On the first stage, we remove the elements from the list. On the second stage, which occurs after a GC
// we know that only threads who were in preemptive mode prior to the GC could possibly still be looking
// at an element that is about to be deleted. All such threads are guarded with a reader count, so if the
// count is 0, we can safely delete, otherwise we must add to the cleanup list to be deleted later. We know
// there can only be one unload at a time, so we can use a single var to hold the unlinked, but not deleted,
// elements.
void EECodeGenManager::Unload(LoaderAllocator* pAllocator)
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        PRECONDITION(pAllocator != NULL);
    }
    CONTRACTL_END;

    CrstHolder ch(&m_CodeHeapLock);
    // If we are in the middle of an enumeration, we cannot unload any code heaps.
    if (m_iteratorCount != 0)
    {
        // Record the LoaderAllocator and return.
        LoaderAllocator** toUnload = m_delayUnload.Append();
        if (toUnload == NULL)
        {
            EEPOLICY_HANDLE_FATAL_ERROR_WITH_MESSAGE(COR_E_FAILFAST,
                W("Failed to delay unload code heap for LoaderAllocator"));
            return;
        }

        *toUnload = pAllocator;
        return;
    }

    // Unload any code heaps that were delayed for unloading.
    UINT32 unloadCount = m_delayUnload.Count();
    if (unloadCount != 0)
    {
        for (UINT32 i = 0; i < unloadCount; ++i)
        {
            UnloadWorker(m_delayUnload.Table()[i]);
        }
        m_delayUnload.Clear();
    }

    UnloadWorker(pAllocator);

    ExecutableAllocator::ResetLazyPreferredRangeHint();
}

void EECodeGenManager::UnloadWorker(LoaderAllocator* pAllocator)
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        PRECONDITION(m_CodeHeapLock.OwnedByCurrentThread());
        PRECONDITION(pAllocator != NULL);
    }
    CONTRACTL_END;

    DomainCodeHeapList **ppList = m_DomainCodeHeaps.Table();
    int count = m_DomainCodeHeaps.Count();

    for (int i=0; i < count; i++) {
        if (ppList[i]->m_pAllocator== pAllocator) {
            DomainCodeHeapList *pList = ppList[i];
            m_DomainCodeHeaps.DeleteByIndex(i);

            // pHeapList is allocated in pHeap, so only need to delete the LoaderHeap itself
            count = pList->m_CodeHeapList.Count();
            for (i=0; i < count; i++) {
                HeapList *pHeapList = pList->m_CodeHeapList[i];
                DeleteCodeHeap(pHeapList);
            }

            // this is ok to do delete as anyone accessing the DomainCodeHeapList structure holds the critical section.
            delete pList;

            break;
        }
    }
    ppList = m_DynamicDomainCodeHeaps.Table();
    count = m_DynamicDomainCodeHeaps.Count();
    for (int i=0; i < count; i++) {
        if (ppList[i]->m_pAllocator== pAllocator) {
            DomainCodeHeapList *pList = ppList[i];
            m_DynamicDomainCodeHeaps.DeleteByIndex(i);

            // pHeapList is allocated in pHeap, so only need to delete the CodeHeap itself
            count = pList->m_CodeHeapList.Count();
            for (i=0; i < count; i++) {
                HeapList *pHeapList = pList->m_CodeHeapList[i];
                // m_DynamicDomainCodeHeaps should only contain HostCodeHeap.
                RemoveFromCleanupList(static_cast<HostCodeHeap*>(pHeapList->pHeap));
                DeleteCodeHeap(pHeapList);
            }

            // this is ok to do delete as anyone accessing the DomainCodeHeapList structure holds the critical section.
            delete pList;

            break;
        }
    }
}

EEJitManager::DomainCodeHeapList::DomainCodeHeapList(LoaderAllocator* allocator)
    : m_pAllocator{ allocator }
{
    LIMITED_METHOD_CONTRACT;
}

void EECodeGenManager::RemoveCodeHeapFromDomainList(CodeHeap *pHeap, LoaderAllocator *pAllocator)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        PRECONDITION(m_CodeHeapLock.OwnedByCurrentThread());
    } CONTRACTL_END;

    // get the AppDomain heap list for pAllocator in m_DynamicDomainCodeHeaps
    DomainCodeHeapList *pList = GetCodeHeapList(NULL, pAllocator, TRUE);

    // go through the heaps and find and remove pHeap
    int count = pList->m_CodeHeapList.Count();
    for (int i = 0; i < count; i++) {
        HeapList *pHeapList = pList->m_CodeHeapList[i];
        if (pHeapList->pHeap == pHeap) {
            // found the heap to remove. If this is the only heap we remove the whole DomainCodeHeapList
            // otherwise we just remove this heap
            if (count == 1) {
                m_DynamicDomainCodeHeaps.Delete(pList);
                delete pList;
            }
            else
                pList->m_CodeHeapList.Delete(i);

            // if this heaplist is cached in the loader allocator, we must clear it
            if (pAllocator->m_pLastUsedDynamicCodeHeap == ((void *) pHeapList))
            {
                pAllocator->m_pLastUsedDynamicCodeHeap = NULL;
            }

            break;
        }
    }
}

#ifdef FEATURE_INTERPRETER

InterpreterJitManager::InterpreterJitManager()
    : m_interpreter(NULL)
    , m_interpreterHandle(NULL)
    , m_interpreterLoadLock(CrstSingleUseLock)
{
    LIMITED_METHOD_CONTRACT;
    SetCodeManager(ExecutionManager::GetInterpreterCodeManager());
}

BOOL InterpreterJitManager::LoadInterpreter()
{
    STANDARD_VM_CONTRACT;

    // If the interpreter is already loaded, don't take the lock.
    if (IsInterpreterLoaded())
        return TRUE;

    // Use m_interpreterLoadLock to ensure that the interpreter is loaded on one thread only
    CrstHolder chRead(&m_interpreterLoadLock);

    // Did someone load the interpreter before we got the lock?
    if (IsInterpreterLoaded())
        return TRUE;

    m_storeRichDebugInfo = CLRConfig::GetConfigValue(CLRConfig::UNSUPPORTED_RichDebugInfo) != 0;

    ICorJitCompiler* newInterpreter = NULL;
    m_interpreter = NULL;

// If both JIT and interpret are available, statically link the JIT. Interpreter can be loaded dynamically
// via config switch for testing purposes.
#if defined(FEATURE_STATICALLY_LINKED) && !defined(FEATURE_JIT)
    newInterpreter = InitializeStaticJIT();
#else // FEATURE_STATICALLY_LINKED && !FEATURE_JIT
    g_interpreterLoadData.jld_id = JIT_LOAD_INTERPRETER;

    LPWSTR interpreterPath = NULL;
#ifdef _DEBUG
    IfFailThrow(CLRConfig::GetConfigValue(CLRConfig::INTERNAL_InterpreterPath, &interpreterPath));
#endif
    LoadAndInitializeJIT(ExecutionManager::GetInterpreterName() DEBUGARG(interpreterPath), &m_interpreterHandle, &newInterpreter, &g_interpreterLoadData, getClrVmOs());
#endif // FEATURE_STATICALLY_LINKED && !FEATURE_JIT

    // Publish the interpreter.
    m_interpreter = newInterpreter;

    return IsInterpreterLoaded();
}
#endif // FEATURE_INTERPRETER

void EECodeGenManager::FreeHostCodeHeapMemoryWorker(HostCodeHeap* pCodeHeap, void* codeStart)
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        PRECONDITION(m_CodeHeapLock.OwnedByCurrentThread());
        PRECONDITION(pCodeHeap != NULL);
        PRECONDITION(codeStart != NULL);
    }
    CONTRACTL_END;

    // clean up the NibbleMap
    NibbleMapDeleteUnlocked(pCodeHeap->m_pHeapList, (TADDR)codeStart);

    // The caller of this method doesn't call HostCodeHeap->FreeMemForCode
    // directly because the operation should be protected by m_CodeHeapLock.
    pCodeHeap->FreeMemForCode(codeStart);
}

void ExecutionManager::CleanupCodeHeaps()
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
    }
    CONTRACTL_END;

    _ASSERTE (IsAtProcessExit() || (GCHeapUtilities::IsGCInProgress()  && ::IsGCThread()));

    GetEEJitManager()->CleanupCodeHeaps();
#ifdef FEATURE_INTERPRETER
    GetInterpreterJitManager()->CleanupCodeHeaps();
#endif // FEATURE_INTERPRETER
}

void EECodeGenManager::CleanupCodeHeaps()
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
    }
    CONTRACTL_END;

    _ASSERTE (IsAtProcessExit() || (GCHeapUtilities::IsGCInProgress() && ::IsGCThread()));

    // Check outside the lock if there are any heaps to clean up
    if (m_cleanupList == NULL)
        return;

    CrstHolder ch(&m_CodeHeapLock);

    // If there are any iterators, we cannot clean up the heaps yet.
    // We will try on the next GC to do the cleanup.
    if (m_iteratorCount != 0)
        return;

    if (m_cleanupList == NULL)
        return;

    HostCodeHeap *pHeap = m_cleanupList;
    m_cleanupList = NULL;

    while (pHeap)
    {
        HostCodeHeap *pNextHeap = pHeap->m_pNextHeapToRelease;

        DWORD allocCount = pHeap->m_AllocationCount;
        if (allocCount == 0)
        {
            LOG((LF_BCL, LL_INFO100, "Level2 - Destryoing CodeHeap [%p, vt(0x%zx)] - ref count 0\n", pHeap, *(size_t*)pHeap));
            RemoveCodeHeapFromDomainList(pHeap, pHeap->m_pAllocator);
            DeleteCodeHeap(pHeap->m_pHeapList);
        }
        else
        {
            LOG((LF_BCL, LL_INFO100, "Level2 - Restoring CodeHeap [%p, vt(0x%zx)] - ref count %d\n", pHeap, *(size_t*)pHeap, allocCount));
        }
        pHeap = pNextHeap;
    }
}

void EECodeGenManager::RemoveFromCleanupList(HostCodeHeap *pCodeHeap)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        PRECONDITION(m_CodeHeapLock.OwnedByCurrentThread());
    } CONTRACTL_END;

    HostCodeHeap *pHeap = m_cleanupList;
    HostCodeHeap *pPrevHeap = NULL;
    while (pHeap)
    {
        if (pHeap == pCodeHeap)
        {
            if (pPrevHeap)
            {
                // remove current heap from list
                pPrevHeap->m_pNextHeapToRelease = pHeap->m_pNextHeapToRelease;
            }
            else
            {
                m_cleanupList = pHeap->m_pNextHeapToRelease;
            }
            break;
        }
        pPrevHeap = pHeap;
        pHeap = pHeap->m_pNextHeapToRelease;
    }
}

CodeHeapIterator EECodeGenManager::GetCodeHeapIterator(LoaderAllocator* pLoaderAllocatorFilter)
{
    CONTRACTL
    {
        THROWS;
        GC_NOTRIGGER;
    }
    CONTRACTL_END;

    CrstHolder ch(&m_CodeHeapLock);
    return CodeHeapIterator
    {
        this,
        m_pAllCodeHeaps,
        pLoaderAllocatorFilter
    };
}

void EECodeGenManager::AddRefIterator()
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        PRECONDITION(m_CodeHeapLock.OwnedByCurrentThread());
    }
    CONTRACTL_END;

    m_iteratorCount++;
}

void EECodeGenManager::ReleaseIterator()
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
    }
    CONTRACTL_END;

    CrstHolder ch(&m_CodeHeapLock);
    _ASSERTE(m_iteratorCount > 0);
    m_iteratorCount--;
}

void EECodeGenManager::AddToCleanupList(HostCodeHeap *pCodeHeap)
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        PRECONDITION(m_CodeHeapLock.OwnedByCurrentThread());
    }
    CONTRACTL_END;

    // it may happen that the current heap count goes to 0 and later on, before it is destroyed, it gets reused
    // for another dynamic method.
    // It's then possible that the ref count reaches 0 multiple times. If so we simply don't add it again
    // Also on cleanup we check the ref count is actually 0.
    HostCodeHeap *pHeap = m_cleanupList;
    while (pHeap)
    {
        if (pHeap == pCodeHeap)
        {
            LOG((LF_BCL, LL_INFO100, "Level2 - CodeHeap [%p, vt(0x%zx)] - Already in list\n", pCodeHeap, *(size_t*)pCodeHeap));
            break;
        }
        pHeap = pHeap->m_pNextHeapToRelease;
    }
    if (pHeap == NULL)
    {
        pCodeHeap->m_pNextHeapToRelease = m_cleanupList;
        m_cleanupList = pCodeHeap;
        LOG((LF_BCL, LL_INFO100, "Level2 - CodeHeap [%p, vt(0x%zx)] - ref count %d - Adding to cleanup list\n", pCodeHeap, *(size_t*)pCodeHeap, pCodeHeap->m_AllocationCount));
    }
}

bool EECodeGenManager::TryFreeHostCodeHeapMemory(HostCodeHeap* pCodeHeap, void* codeStart)
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        PRECONDITION(pCodeHeap != NULL);
        PRECONDITION(codeStart != NULL);
    }
    CONTRACTL_END;

    CrstHolder ch(&m_CodeHeapLock);
    if (m_iteratorCount != 0)
    {
        // If we are in the middle of an enumeration, we cannot destroy code heap memory.
        return false;
    }

    FreeHostCodeHeapMemoryWorker(pCodeHeap, codeStart);
    return true;
}

void EECodeGenManager::DeleteCodeHeap(HeapList *pHeapList)
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        PRECONDITION(m_CodeHeapLock.OwnedByCurrentThread());
    }
    CONTRACTL_END;

    // Remove from the list of heaps
    HeapList *pHp = m_pAllCodeHeaps;
    if (pHp == pHeapList)
    {
        m_pAllCodeHeaps = pHp->GetNext();
    }
    else
    {
        HeapList *pHpNext = pHp->GetNext();

        while (pHpNext != pHeapList)
        {
            pHp = pHpNext;
            _ASSERTE(pHp != NULL);  // should always find the HeapList
            pHpNext = pHp->GetNext();
        }
        pHp->SetNext(pHeapList->GetNext());
    }

    DeleteFunctionTable((PVOID)pHeapList->GetModuleBase());

    ExecutionManager::DeleteRange((TADDR)pHeapList->GetModuleBase());

    LOG((LF_JIT, LL_INFO100, "DeleteCodeHeap start %p end %p\n",
                              (const BYTE*)pHeapList->startAddress,
                              (const BYTE*)pHeapList->endAddress     ));

    CodeHeap* pHeap = pHeapList->pHeap;
    delete pHeap;
    delete pHeapList;
}

#endif // #ifndef DACCESS_COMPILE

template<typename TCodeHeader>
static TCodeHeader * GetCodeHeaderFromDebugInfoRequest(const DebugInfoRequest & request)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

    TADDR address = (TADDR) request.GetStartAddress();
    _ASSERTE(address != (TADDR)0);

    TCodeHeader * pHeader = dac_cast<DPTR(TCodeHeader)>(address & ~3) - 1;
    _ASSERTE(pHeader != NULL);

    return pHeader;
}

//-----------------------------------------------------------------------------
// Get vars from Jit Store
//-----------------------------------------------------------------------------
BOOL EECodeGenManager::GetBoundariesAndVarsWorker(
        PTR_BYTE pDebugInfo,
        IN FP_IDS_NEW fpNew,
        IN void * pNewData,
        BoundsType boundsType,
        OUT ULONG32 * pcMap,
        OUT ICorDebugInfo::OffsetMapping **ppMap,
        OUT ULONG32 * pcVars,
        OUT ICorDebugInfo::NativeVarInfo **ppVars)
{
    CONTRACTL {
        THROWS;       // on OOM.
        GC_NOTRIGGER; // getting vars shouldn't trigger
        SUPPORTS_DAC;
    } CONTRACTL_END;

    // No header created, which means no jit information is available.
    if (pDebugInfo == NULL)
        return FALSE;

#ifdef FEATURE_ON_STACK_REPLACEMENT
    BOOL hasFlagByte = TRUE;
#else
    BOOL hasFlagByte = FALSE;
#endif

    if (m_storeRichDebugInfo)
    {
        hasFlagByte = TRUE;
    }

    // Uncompress. This allocates memory and may throw.
    CompressDebugInfo::RestoreBoundariesAndVars(
        fpNew,
        pNewData, // allocators
        boundsType,
        pDebugInfo,      // input
        pcMap, ppMap,    // output
        pcVars, ppVars,  // output
        hasFlagByte
    );

    return TRUE;
}

BOOL EEJitManager::GetBoundariesAndVars(
        const DebugInfoRequest & request,
        IN FP_IDS_NEW fpNew,
        IN void * pNewData,
        BoundsType boundsType,
        OUT ULONG32 * pcMap,
        OUT ICorDebugInfo::OffsetMapping **ppMap,
        OUT ULONG32 * pcVars,
        OUT ICorDebugInfo::NativeVarInfo **ppVars)
{
    CONTRACTL {
        THROWS;       // on OOM.
        GC_NOTRIGGER; // getting vars shouldn't trigger
        SUPPORTS_DAC;
    } CONTRACTL_END;

    CodeHeader * pHdr = GetCodeHeaderFromDebugInfoRequest<CodeHeader>(request);
    _ASSERTE(pHdr != NULL);

    PTR_BYTE pDebugInfo = pHdr->GetDebugInfo();

    return GetBoundariesAndVarsWorker(pDebugInfo, fpNew, pNewData, boundsType, pcMap, ppMap, pcVars, ppVars);
}

size_t EEJitManager::WalkILOffsets(
    const DebugInfoRequest & request,
    BoundsType boundsType,
    void* pContext,
    size_t (* pfnWalkILOffsets)(ICorDebugInfo::OffsetMapping *pOffsetMapping, void *pContext))
{
    CONTRACTL {
        THROWS;       // on OOM.
        GC_NOTRIGGER; // getting vars shouldn't trigger
        SUPPORTS_DAC;
    } CONTRACTL_END;

    CodeHeader * pHdr = GetCodeHeaderFromDebugInfoRequest<CodeHeader>(request);
    _ASSERTE(pHdr != NULL);

    PTR_BYTE pDebugInfo = pHdr->GetDebugInfo();

    return WalkILOffsetsWorker(pDebugInfo, boundsType, pContext, pfnWalkILOffsets);
}

size_t EECodeGenManager::WalkILOffsetsWorker(PTR_BYTE pDebugInfo,
        BoundsType boundsType,
        void* pContext,
        size_t (* pfnWalkILOffsets)(ICorDebugInfo::OffsetMapping *pOffsetMapping, void *pContext))
{
    CONTRACTL {
        THROWS;       // on OOM.
        GC_NOTRIGGER; // getting vars shouldn't trigger
        SUPPORTS_DAC;
    } CONTRACTL_END;

    // No header created, which means no jit information is available.
    if (pDebugInfo == NULL)
        return 0;

#ifdef FEATURE_ON_STACK_REPLACEMENT
    BOOL hasFlagByte = TRUE;
#else
    BOOL hasFlagByte = FALSE;
#endif

    if (m_storeRichDebugInfo)
    {
        hasFlagByte = TRUE;
    }

    // Uncompress. This allocates memory and may throw.
    return CompressDebugInfo::WalkILOffsets(
        pDebugInfo,      // input
        boundsType,
        hasFlagByte,
        pContext,
        pfnWalkILOffsets
    );
}


BOOL EECodeGenManager::GetRichDebugInfoWorker(
    PTR_BYTE pDebugInfo,
    IN FP_IDS_NEW fpNew, IN void* pNewData,
    OUT ICorDebugInfo::InlineTreeNode** ppInlineTree,
    OUT ULONG32* pNumInlineTree,
    OUT ICorDebugInfo::RichOffsetMapping** ppRichMappings,
    OUT ULONG32* pNumRichMappings)
{
    CONTRACTL {
        THROWS;       // on OOM.
        GC_NOTRIGGER; // getting debug info shouldn't trigger
        SUPPORTS_DAC;
    } CONTRACTL_END;

    if (!m_storeRichDebugInfo)
    {
        return FALSE;
    }

    // No header created, which means no debug information is available.
    if (pDebugInfo == NULL)
        return FALSE;

    CompressDebugInfo::RestoreRichDebugInfo(
        fpNew, pNewData,
        pDebugInfo,
        ppInlineTree, pNumInlineTree,
        ppRichMappings, pNumRichMappings);

    return TRUE;
}

BOOL EEJitManager::GetRichDebugInfo(
    const DebugInfoRequest& request,
    IN FP_IDS_NEW fpNew, IN void* pNewData,
    OUT ICorDebugInfo::InlineTreeNode** ppInlineTree,
    OUT ULONG32* pNumInlineTree,
    OUT ICorDebugInfo::RichOffsetMapping** ppRichMappings,
    OUT ULONG32* pNumRichMappings)
{
    CONTRACTL {
        THROWS;       // on OOM.
        GC_NOTRIGGER; // getting debug info shouldn't trigger
        SUPPORTS_DAC;
    } CONTRACTL_END;

    CodeHeader * pHdr = GetCodeHeaderFromDebugInfoRequest<CodeHeader>(request);
    _ASSERTE(pHdr != NULL);

    PTR_BYTE pDebugInfo = pHdr->GetDebugInfo();

    return GetRichDebugInfoWorker(pDebugInfo, fpNew, pNewData, ppInlineTree, pNumInlineTree, ppRichMappings, pNumRichMappings);
}

#ifdef FEATURE_INTERPRETER
BOOL InterpreterJitManager::GetBoundariesAndVars(
    const DebugInfoRequest & request,
    IN FP_IDS_NEW fpNew,
    IN void * pNewData,
    BoundsType boundsType,
    OUT ULONG32 * pcMap,
    OUT ICorDebugInfo::OffsetMapping **ppMap,
    OUT ULONG32 * pcVars,
    OUT ICorDebugInfo::NativeVarInfo **ppVars)
{
    CONTRACTL {
        THROWS;       // on OOM.
        GC_NOTRIGGER; // getting vars shouldn't trigger
        SUPPORTS_DAC;
    } CONTRACTL_END;

    InterpreterCodeHeader * pHdr = GetCodeHeaderFromDebugInfoRequest<InterpreterCodeHeader>(request);
    _ASSERTE(pHdr != NULL);

    PTR_BYTE pDebugInfo = pHdr->GetDebugInfo();

    // Interpreter-TODO: This is a temporary workaround until the interpreter produces the debug info
    if (pDebugInfo == NULL)
    {
        if (pcMap) *pcMap = 0;
        if (ppMap) *ppMap = NULL;
        if (pcVars) *pcVars = 0;
        if (ppVars) *ppVars = NULL;
        return TRUE;
    }

    return GetBoundariesAndVarsWorker(pDebugInfo, fpNew, pNewData, boundsType, pcMap, ppMap, pcVars, ppVars);
}

size_t InterpreterJitManager::WalkILOffsets(
    const DebugInfoRequest & request,
    BoundsType boundsType,
    void* pContext,
    size_t (* pfnWalkILOffsets)(ICorDebugInfo::OffsetMapping *pOffsetMapping, void *pContext))
{
    CONTRACTL {
        THROWS;       // on OOM.
        GC_NOTRIGGER; // getting vars shouldn't trigger
        SUPPORTS_DAC;
    } CONTRACTL_END;

    InterpreterCodeHeader * pHdr = GetCodeHeaderFromDebugInfoRequest<InterpreterCodeHeader>(request);
    _ASSERTE(pHdr != NULL);

    PTR_BYTE pDebugInfo = pHdr->GetDebugInfo();

    // Interpreter-TODO: This is a temporary workaround until the interpreter produces the debug info
    if (pDebugInfo == NULL)
    {
        return 0;
    }

    return WalkILOffsetsWorker(pDebugInfo, boundsType, pContext, pfnWalkILOffsets);
}


BOOL InterpreterJitManager::GetRichDebugInfo(
    const DebugInfoRequest& request,
    IN FP_IDS_NEW fpNew, IN void* pNewData,
    OUT ICorDebugInfo::InlineTreeNode** ppInlineTree,
    OUT ULONG32* pNumInlineTree,
    OUT ICorDebugInfo::RichOffsetMapping** ppRichMappings,
    OUT ULONG32* pNumRichMappings)
{
    CONTRACTL {
        THROWS;       // on OOM.
        GC_NOTRIGGER; // getting debug info shouldn't trigger
        SUPPORTS_DAC;
    } CONTRACTL_END;

    InterpreterCodeHeader * pHdr = GetCodeHeaderFromDebugInfoRequest<InterpreterCodeHeader>(request);
    _ASSERTE(pHdr != NULL);

    PTR_BYTE pDebugInfo = pHdr->GetDebugInfo();

    return GetRichDebugInfoWorker(pDebugInfo, fpNew, pNewData, ppInlineTree, pNumInlineTree, ppRichMappings, pNumRichMappings);
}
#endif // FEATURE_INTERPRETER

#ifdef DACCESS_COMPILE
void CodeHeader::EnumMemoryRegions(CLRDataEnumMemoryFlags flags, IJitManager* pJitMan)
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    }
    CONTRACTL_END;

    DAC_ENUM_DTHIS();

    this->pRealCodeHeader.EnumMem();

#ifdef FEATURE_EH_FUNCLETS
    // UnwindInfos are stored in an array immediately following the RealCodeHeader structure in memory.
    if (this->pRealCodeHeader->nUnwindInfos)
    {
        DacEnumMemoryRegion(PTR_TO_MEMBER_TADDR(RealCodeHeader, pRealCodeHeader, unwindInfos), this->pRealCodeHeader->nUnwindInfos * sizeof(T_RUNTIME_FUNCTION));
    }
#endif // FEATURE_EH_FUNCLETS

#ifdef FEATURE_ON_STACK_REPLACEMENT
    BOOL hasFlagByte = TRUE;
#else
    BOOL hasFlagByte = FALSE;
#endif

    if (this->GetDebugInfo() != NULL)
    {
        CompressDebugInfo::EnumMemoryRegions(flags, this->GetDebugInfo(), hasFlagByte);
    }
}

//-----------------------------------------------------------------------------
// Enumerate for minidumps.
//-----------------------------------------------------------------------------
template<typename TCodeHeader>
void EECodeGenManager::EnumMemoryRegionsForMethodDebugInfoWorker(CLRDataEnumMemoryFlags flags, EECodeInfo * pCodeInfo)
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    }
    CONTRACTL_END;

    DebugInfoRequest request;
    request.InitFromStartingAddr(pCodeInfo->GetMethodDesc(), pCodeInfo->GetStartAddress());

    TCodeHeader * pHeader = GetCodeHeaderFromDebugInfoRequest<TCodeHeader>(request);

    pHeader->EnumMemoryRegions(flags, NULL);
}

void EEJitManager::EnumMemoryRegionsForMethodDebugInfo(CLRDataEnumMemoryFlags flags, EECodeInfo * pCodeInfo)
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    }
    CONTRACTL_END;

    EnumMemoryRegionsForMethodDebugInfoWorker<CodeHeader>(flags, pCodeInfo);
}

#ifdef FEATURE_INTERPRETER
void InterpreterCodeHeader::EnumMemoryRegions(CLRDataEnumMemoryFlags flags, IJitManager* pJitMan)
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    }
    CONTRACTL_END;

    DAC_ENUM_DTHIS();

    this->pRealCodeHeader.EnumMem();

    if (this->GetDebugInfo() != NULL)
    {
        CompressDebugInfo::EnumMemoryRegions(flags, this->GetDebugInfo(), FALSE /* hasFlagByte */);
    }
}

void InterpreterJitManager::EnumMemoryRegionsForMethodDebugInfo(CLRDataEnumMemoryFlags flags, EECodeInfo * pCodeInfo)
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    }
    CONTRACTL_END;

    EnumMemoryRegionsForMethodDebugInfoWorker<InterpreterCodeHeader>(flags, pCodeInfo);
}
#endif // FEATURE_INTERPRETER

#endif // DACCESS_COMPILE

PCODE EEJitManager::GetCodeAddressForRelOffset(const METHODTOKEN& MethodToken, DWORD relOffset)
{
    WRAPPER_NO_CONTRACT;

    CodeHeader * pHeader = GetCodeHeader(MethodToken);
    return pHeader->GetCodeStartAddress() + relOffset;
}

#ifdef FEATURE_INTERPRETER
PCODE InterpreterJitManager::GetCodeAddressForRelOffset(const METHODTOKEN& MethodToken, DWORD relOffset)
{
    WRAPPER_NO_CONTRACT;

    InterpreterCodeHeader * pHeader = GetCodeHeader(MethodToken);
    return pHeader->GetCodeStartAddress() + relOffset;
}
#endif // FEATURE_INTERPRETER

template<typename TCodeHeader>
BOOL EECodeGenManager::JitCodeToMethodInfoWorker(
    RangeSection * pRangeSection,
    PCODE currentPC,
    MethodDesc ** ppMethodDesc,
    EECodeInfo * pCodeInfo)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

    _ASSERTE(pRangeSection != NULL);

    if (pRangeSection->_flags & RangeSection::RANGE_SECTION_RANGELIST)
        return FALSE;

    TADDR start = dac_cast<PTR_EECodeGenManager>(pRangeSection->_pjit)->FindMethodCode(pRangeSection, currentPC);
    if (start == (TADDR)0)
        return FALSE;

    TCodeHeader * pCHdr = (DPTR(TCodeHeader))(start - sizeof(TCodeHeader));
    if (pCHdr->IsStubCodeBlock())
        return FALSE;

    _ASSERTE(pCHdr->GetMethodDesc()->SanityCheck());

    if (pCodeInfo)
    {
        pCodeInfo->m_methodToken = METHODTOKEN(pRangeSection, dac_cast<TADDR>(pCHdr));

        // This can be counted on for Jitted code. For NGEN code in the case
        // where we have hot/cold splitting this isn't valid and we need to
        // take into account cold code.
        pCodeInfo->m_relOffset = (DWORD)(PCODEToPINSTR(currentPC) - start);

#ifdef FEATURE_EH_FUNCLETS
        // Computed lazily by code:EEJitManager::LazyGetFunctionEntry
        if (pCHdr->MayHaveFunclets())
        {
            // Computed lazily by code:EEJitManager::LazyGetFunctionEntry
            pCodeInfo->m_pFunctionEntry = NULL;
            pCodeInfo->m_isFuncletCache = EECodeInfo::IsFuncletCache::NotSet;
        }
        else
        {
            pCodeInfo->m_pFunctionEntry = pCHdr->GetUnwindInfo(0);
            pCodeInfo->m_isFuncletCache = EECodeInfo::IsFuncletCache::IsNotFunclet;
        }
#endif
    }

    if (ppMethodDesc)
    {
        *ppMethodDesc = pCHdr->GetMethodDesc();
    }
    return TRUE;
}

BOOL EEJitManager::JitCodeToMethodInfo(
        RangeSection * pRangeSection,
        PCODE currentPC,
        MethodDesc ** ppMethodDesc,
        EECodeInfo * pCodeInfo)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

    return JitCodeToMethodInfoWorker<CodeHeader>(pRangeSection, currentPC, ppMethodDesc, pCodeInfo);
}

#ifdef FEATURE_INTERPRETER
BOOL InterpreterJitManager::JitCodeToMethodInfo(
        RangeSection * pRangeSection,
        PCODE currentPC,
        MethodDesc ** ppMethodDesc,
        EECodeInfo * pCodeInfo)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

    return JitCodeToMethodInfoWorker<InterpreterCodeHeader>(pRangeSection, currentPC, ppMethodDesc, pCodeInfo);
}

TADDR InterpreterJitManager::GetFuncletStartAddress(EECodeInfo * pCodeInfo)
{
    EH_CLAUSE_ENUMERATOR enumState;
    unsigned ehCount;

    IJitManager *pJitMan = pCodeInfo->GetJitManager();
    ehCount = pJitMan->InitializeEHEnumeration(pCodeInfo->GetMethodToken(), &enumState);
    DWORD relOffset = pCodeInfo->GetRelOffset();
    TADDR methodBaseAddress = pCodeInfo->GetCodeAddress() - relOffset;

    for (unsigned i = 0; i < ehCount; i++)
    {
        EE_ILEXCEPTION_CLAUSE ehClause;
        pJitMan->GetNextEHClause(&enumState, &ehClause);

        if ((ehClause.HandlerStartPC <= relOffset) && (relOffset < ehClause.HandlerEndPC))
        {
            return methodBaseAddress + ehClause.HandlerStartPC;
        }

        // For filters, we also need to check the filter funclet range. The filter funclet is always stored right
        // before its handler funclet (according to ECMA-355). So the filter end offset is equal to the start offset of the handler funclet.
        if (IsFilterHandler(&ehClause) && (ehClause.FilterOffset <= relOffset) && (relOffset < ehClause.HandlerStartPC))
        {
            return methodBaseAddress + ehClause.FilterOffset;
        }
    }

    return methodBaseAddress;
}

DWORD InterpreterJitManager::GetFuncletStartOffsets(const METHODTOKEN& MethodToken, DWORD* pStartFuncletOffsets, DWORD dwLength)
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
    }
    CONTRACTL_END;

    EH_CLAUSE_ENUMERATOR enumState;
    unsigned ehCount;

    ehCount = InitializeEHEnumeration(MethodToken, &enumState);

    DWORD nFunclets = 0;
    for (unsigned i = 0; i < ehCount; i++)
    {
        EE_ILEXCEPTION_CLAUSE ehClause;
        GetNextEHClause(&enumState, &ehClause);
        if (nFunclets < dwLength)
        {
            pStartFuncletOffsets[nFunclets] = ehClause.HandlerStartPC;
        }
        nFunclets++;
        if (IsFilterHandler(&ehClause))
        {
            if (nFunclets < dwLength)
            {
                pStartFuncletOffsets[nFunclets] = ehClause.FilterOffset;
            }

            nFunclets++;
        }
    }

    return nFunclets;
}

void InterpreterJitManager::JitTokenToMethodRegionInfo(const METHODTOKEN& MethodToken, MethodRegionInfo * methodRegionInfo)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
        PRECONDITION(methodRegionInfo != NULL);
    } CONTRACTL_END;

    methodRegionInfo->hotStartAddress  = JitTokenToStartAddress(MethodToken);
    methodRegionInfo->hotSize          = GetCodeManager()->GetFunctionSize(GetGCInfoToken(MethodToken));
    methodRegionInfo->coldStartAddress = 0;
    methodRegionInfo->coldSize         = 0;
}

#endif // FEATURE_INTERPRETER

StubCodeBlockKind EEJitManager::GetStubCodeBlockKind(RangeSection * pRangeSection, PCODE currentPC)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

    if (pRangeSection->_flags & RangeSection::RANGE_SECTION_RANGELIST)
    {
        return pRangeSection->_pRangeList->GetCodeBlockKind();
    }

    TADDR start = dac_cast<PTR_EEJitManager>(pRangeSection->_pjit)->FindMethodCode(pRangeSection, currentPC);
    if (start == (TADDR)0)
        return STUB_CODE_BLOCK_NOCODE;
    CodeHeader * pCHdr = PTR_CodeHeader(start - sizeof(CodeHeader));
    return pCHdr->IsStubCodeBlock() ? pCHdr->GetStubCodeBlockKind() : STUB_CODE_BLOCK_MANAGED;
}


TADDR EECodeGenManager::FindMethodCode(PCODE currentPC)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

    RangeSection * pRS = ExecutionManager::FindCodeRange(currentPC, ExecutionManager::GetScanFlags());
    if (pRS == NULL || (pRS->_flags & RangeSection::RANGE_SECTION_CODEHEAP) == 0)
        return STUB_CODE_BLOCK_NOCODE;
    return dac_cast<PTR_EECodeGenManager>(pRS->_pjit)->FindMethodCode(pRS, currentPC);
}

// Finds the header corresponding to the code at offset "delta".
// Returns NULL if there is no header for the given "delta"
// For implementation details, see comment in nibblemapmacros.h
TADDR EECodeGenManager::FindMethodCode(RangeSection * pRangeSection, PCODE currentPC)
{
    using namespace NibbleMap;

    LIMITED_METHOD_DAC_CONTRACT;

    _ASSERTE(pRangeSection != NULL);
    _ASSERTE(pRangeSection->_flags & RangeSection::RANGE_SECTION_CODEHEAP);

    HeapList *pHp = pRangeSection->_pHeapList;

    if ((currentPC < pHp->startAddress) ||
        (currentPC > pHp->endAddress))
    {
        return 0;
    }

    TADDR base = pHp->mapBase;
    TADDR delta = currentPC - base;
    PTR_DWORD pMap = pHp->pHdrMap;
    PTR_DWORD pMapStart = pMap;

    DWORD dword;
    DWORD tmp;

    size_t startPos = ADDR2POS(delta);  // align to 32byte buckets
                                        // ( == index into the array of nibbles)
    DWORD  offset   = ADDR2OFFS(delta); // this is the offset inside the bucket + 1

    _ASSERTE(offset == (offset & NIBBLE_MASK));

    pMap += (startPos >> LOG2_NIBBLES_PER_DWORD); // points to the proper DWORD of the map

    // #1 look up DWORD represnting current PC
    _ASSERTE(pMap != NULL);
    dword = VolatileLoadWithoutBarrier<DWORD>(pMap);

    // #2 if DWORD is a pointer, then we can return
    if (IsPointer(dword))
    {
        TADDR newAddr = base + DecodePointer(dword);
        return newAddr;
    }

    // #3 check if corresponding nibble is intialized and points to an equal or earlier address
    tmp = dword >> POS2SHIFTCOUNT(startPos);
    if ((tmp & NIBBLE_MASK) && ((tmp & NIBBLE_MASK) <= offset))
    {
        return base + POSOFF2ADDR(startPos, tmp & NIBBLE_MASK);
    }

    // #4 try to find preceeding nibble in the DWORD
    tmp >>= NIBBLE_SIZE;
    if (tmp)
    {
        startPos--;
        while(!(tmp & NIBBLE_MASK))
        {
            tmp >>= NIBBLE_SIZE;
            startPos--;
        }
        return base + POSOFF2ADDR(startPos, tmp & NIBBLE_MASK);
    }

    // #5.1 read previous DWORD
    // We skipped the remainder of the DWORD,
    // so we must set startPos to the highest position of
    // previous DWORD, unless we are already on the first DWORD
    if (startPos < NIBBLES_PER_DWORD)
    {
        return 0;
    }

    startPos = ((startPos >> LOG2_NIBBLES_PER_DWORD) << LOG2_NIBBLES_PER_DWORD) - 1;
    dword = VolatileLoadWithoutBarrier<DWORD>(--pMap);

    // We should not have read a value before the start of the map.
    _ASSERTE(pMapStart <= pMap);

    // If the second dword is not empty, it either has a nibble or a pointer
    if (dword)
    {
        // #5.2 either DWORD is a pointer
        if (IsPointer(dword))
        {
            return base + DecodePointer(dword);
        }

        // #5.4 or contains a nibble
        tmp = dword;
        while(!(tmp & NIBBLE_MASK))
        {
            tmp >>= NIBBLE_SIZE;
            startPos--;
        }
        return base + POSOFF2ADDR(startPos, tmp & NIBBLE_MASK);
    }

    // If none of the above was found, return 0
    return 0;
}

#if !defined(DACCESS_COMPILE)

void EECodeGenManager::NibbleMapSet(HeapList * pHp, TADDR pCode, size_t codeSize)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
    } CONTRACTL_END;

    CrstHolder ch(&m_CodeHeapLock);
    NibbleMapSetUnlocked(pHp, pCode, codeSize);
}

// For implementation details, see comment in nibblemapmacros.h
void EECodeGenManager::NibbleMapSetUnlocked(HeapList * pHp, TADDR pCode, size_t codeSize)
{
    using namespace NibbleMap;

    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        PRECONDITION(m_CodeHeapLock.OwnedByCurrentThread());
    } CONTRACTL_END;

    _ASSERTE(pCode >= pHp->mapBase);
    _ASSERTE(pCode + codeSize <= pHp->startAddress + pHp->maxCodeHeapSize);

    // remove bottom two bits to ensure alignment math
    // on ARM32 Thumb, the low bits indicate the thumb instruction set
    pCode = ALIGN_DOWN(pCode, CODE_ALIGN);

    size_t delta = pCode - pHp->mapBase;

    size_t pos = ADDR2POS(delta);
    DWORD value = ADDR2OFFS(delta);

    DWORD index = (DWORD) (pos >> LOG2_NIBBLES_PER_DWORD);
    DWORD mask  = POS2MASK(pos);

    value <<= POS2SHIFTCOUNT(pos);

    PTR_DWORD pMap = pHp->pHdrMap;

    // assert that we don't overwrite an existing offset
    // the nibble is empty and the DWORD is not a pointer
    _ASSERTE(!((*(pMap+index)) & ~mask) && !IsPointer(*(pMap+index)));

    VolatileStore<DWORD>(pMap+index, ((*(pMap+index))&mask)|value);

    size_t firstByteAfterMethod = delta + codeSize;
    DWORD encodedPointer = EncodePointer(delta);
    index++;
    while ((index + 1) * BYTES_PER_DWORD <= firstByteAfterMethod)
    {
        // All of these DWORDs should be empty
        _ASSERTE(!(*(pMap+index)));
        VolatileStore<DWORD>(pMap+index, encodedPointer);

        index++;
    }
}

void EECodeGenManager::NibbleMapDeleteUnlocked(HeapList* pHp, TADDR pCode)
{
    using namespace NibbleMap;

    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        PRECONDITION(m_CodeHeapLock.OwnedByCurrentThread());
    } CONTRACTL_END;

    _ASSERTE(pCode >= pHp->mapBase);

    // remove bottom two bits to ensure alignment math
    // on ARM32 Thumb, the low bits indicate the thumb instruction set
    pCode = ALIGN_DOWN(pCode, CODE_ALIGN);

    size_t delta = pCode - pHp->mapBase;

    size_t pos = ADDR2POS(delta);
    DWORD value = ADDR2OFFS(delta);

    DWORD index = (DWORD) (pos >> LOG2_NIBBLES_PER_DWORD);
    DWORD mask  = POS2MASK(pos);

    PTR_DWORD pMap = pHp->pHdrMap;

    // Assert that the DWORD is not a pointer. Deleting a portion of a pointer
    // would cause the state of the map to be invalid. Deleting empty nibbles,
    // a no-op, is allowed and can occur when removing JIT data.
    pMap += index;
    _ASSERTE(!IsPointer(*pMap));

    // delete the relevant nibble
    VolatileStore<DWORD>(pMap, (*pMap) & mask);

    // the last DWORD of the nibble map is reserved to be empty for bounds checking
    pMap++;
    while (IsPointer(*pMap) && DecodePointer(*pMap) == delta){
        // The next DWORD is a pointer to the nibble being deleted, so we can delete it
        VolatileStore<DWORD>(pMap, 0);
        pMap++;
    }
}

#endif // !DACCESS_COMPILE

#if defined(FEATURE_EH_FUNCLETS)
PTR_RUNTIME_FUNCTION EEJitManager::LazyGetFunctionEntry(EECodeInfo * pCodeInfo)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

    if (!pCodeInfo->IsValid())
    {
        return NULL;
    }

    CodeHeader * pHeader = dac_cast<PTR_CodeHeader>(GetCodeHeader(pCodeInfo->GetMethodToken()));

    DWORD address = RUNTIME_FUNCTION__BeginAddress(pHeader->GetUnwindInfo(0)) + pCodeInfo->GetRelOffset();

    // We need the module base address to calculate the end address of a function from the functionEntry.
    // Thus, save it off right now.
    TADDR baseAddress = pCodeInfo->GetModuleBase();

    // NOTE: We could binary search here, if it would be helpful (e.g., large number of funclets)
    for (UINT iUnwindInfo = 0; iUnwindInfo < pHeader->GetNumberOfUnwindInfos(); iUnwindInfo++)
    {
        PTR_RUNTIME_FUNCTION pFunctionEntry = pHeader->GetUnwindInfo(iUnwindInfo);

        if (RUNTIME_FUNCTION__BeginAddress(pFunctionEntry) <= address && address < RUNTIME_FUNCTION__EndAddress(pFunctionEntry, baseAddress))
        {
            return pFunctionEntry;
        }
    }

    return NULL;
}

DWORD EEJitManager::GetFuncletStartOffsets(const METHODTOKEN& MethodToken, DWORD* pStartFuncletOffsets, DWORD dwLength)
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
    }
    CONTRACTL_END;

    CodeHeader * pCH = dac_cast<PTR_CodeHeader>(GetCodeHeader(MethodToken));
    TADDR moduleBase = JitTokenToModuleBase(MethodToken);

    _ASSERTE(pCH->GetNumberOfUnwindInfos() >= 1);

    DWORD parentBeginRva = RUNTIME_FUNCTION__BeginAddress(pCH->GetUnwindInfo(0));

    DWORD nFunclets = 0;
    for (COUNT_T iUnwindInfo = 1; iUnwindInfo < pCH->GetNumberOfUnwindInfos(); iUnwindInfo++)
    {
        PTR_RUNTIME_FUNCTION pFunctionEntry = pCH->GetUnwindInfo(iUnwindInfo);

#if defined(EXCEPTION_DATA_SUPPORTS_FUNCTION_FRAGMENTS)
        if (IsFunctionFragment(moduleBase, pFunctionEntry))
        {
            // This is a fragment (not the funclet beginning); skip it
            continue;
        }
#endif // EXCEPTION_DATA_SUPPORTS_FUNCTION_FRAGMENTS

        DWORD funcletBeginRva = RUNTIME_FUNCTION__BeginAddress(pFunctionEntry);
        DWORD relParentOffsetToFunclet = funcletBeginRva - parentBeginRva;

        if (nFunclets < dwLength)
            pStartFuncletOffsets[nFunclets] = relParentOffsetToFunclet;
        nFunclets++;
    }

    return nFunclets;
}

#if defined(DACCESS_COMPILE)
// This function is basically like RtlLookupFunctionEntry(), except that it works with DAC
// to read the function entries out of process.  Also, it can only look up function entries
// inside mscorwks.dll, since DAC doesn't know anything about other unmanaged dll's.
void GetUnmanagedStackWalkInfo(IN  ULONG64   ControlPc,
                               OUT UINT_PTR* pModuleBase,
                               OUT UINT_PTR* pFuncEntry)
{
    WRAPPER_NO_CONTRACT;

    if (pModuleBase)
    {
        *pModuleBase = 0;
    }

    if (pFuncEntry)
    {
        *pFuncEntry = 0;
    }

    PEDecoder peDecoder(DacGlobalBase());

    SIZE_T baseAddr = dac_cast<TADDR>(peDecoder.GetBase());
    SIZE_T cbSize   = (SIZE_T)peDecoder.GetVirtualSize();

    // Check if the control PC is inside mscorwks.
    if ( (baseAddr <= ControlPc) &&
         (ControlPc < (baseAddr + cbSize))
       )
    {
        if (pModuleBase)
        {
            *pModuleBase = baseAddr;
        }

        if (pFuncEntry)
        {
            // Check if there is a static function table.
            COUNT_T cbSize = 0;
            TADDR   pExceptionDir = peDecoder.GetDirectoryEntryData(IMAGE_DIRECTORY_ENTRY_EXCEPTION, &cbSize);

            if (pExceptionDir != 0)
            {
                // Do a binary search on the static function table of mscorwks.dll.
                HRESULT hr = E_FAIL;
                TADDR   taFuncEntry;
                T_RUNTIME_FUNCTION functionEntry;

                DWORD dwLow  = 0;
                DWORD dwHigh = cbSize / sizeof(T_RUNTIME_FUNCTION);
                DWORD dwMid  = 0;

                while (dwLow <= dwHigh)
                {
                    dwMid = (dwLow + dwHigh) >> 1;
                    taFuncEntry = pExceptionDir + dwMid * sizeof(T_RUNTIME_FUNCTION);
                    hr = DacReadAll(taFuncEntry, &functionEntry, sizeof(functionEntry), false);
                    if (FAILED(hr))
                    {
                        return;
                    }

                    if (ControlPc < baseAddr + functionEntry.BeginAddress)
                    {
                        dwHigh = dwMid - 1;
                    }
                    else if (ControlPc >= baseAddr + RUNTIME_FUNCTION__EndAddress(&functionEntry, baseAddr))
                    {
                        dwLow = dwMid + 1;
                    }
                    else
                    {
                        _ASSERTE(pFuncEntry);
#ifdef _TARGET_AMD64_
                        // On amd64, match RtlLookupFunctionEntry behavior by resolving indirect function entries
                        // back to the associated owning function entry.
                        if ((functionEntry.UnwindData & RUNTIME_FUNCTION_INDIRECT) != 0)
                        {
                            DWORD dwRvaOfOwningFunctionEntry = (functionEntry.UnwindData & ~RUNTIME_FUNCTION_INDIRECT);
                            taFuncEntry = peDecoder.GetRvaData(dwRvaOfOwningFunctionEntry);
                            hr = DacReadAll(taFuncEntry, &functionEntry, sizeof(functionEntry), false);
                            if (FAILED(hr))
                            {
                                return;
                            }

                            _ASSERTE((functionEntry.UnwindData & RUNTIME_FUNCTION_INDIRECT) == 0);
                        }
#endif // _TARGET_AMD64_

                        *pFuncEntry = (UINT_PTR)(T_RUNTIME_FUNCTION*)PTR_RUNTIME_FUNCTION(taFuncEntry);
                        break;
                    }
                }

                if (dwLow > dwHigh)
                {
                    _ASSERTE(*pFuncEntry == 0);
                }
            }
        }
    }
}
#endif // DACCESS_COMPILE

extern "C" void GetRuntimeStackWalkInfo(IN  ULONG64   ControlPc,
                                        OUT UINT_PTR* pModuleBase,
                                        OUT UINT_PTR* pFuncEntry)
{

    WRAPPER_NO_CONTRACT;

    PreserveLastErrorHolder preserveLastError;

    if (pModuleBase)
        *pModuleBase = 0;
    if (pFuncEntry)
        *pFuncEntry = 0;

    EECodeInfo codeInfo((PCODE)ControlPc);
    if (!codeInfo.IsValid())
    {
#if defined(DACCESS_COMPILE)
        GetUnmanagedStackWalkInfo(ControlPc, pModuleBase, pFuncEntry);
#endif // DACCESS_COMPILE
        return;
    }

    if (pModuleBase)
    {
        *pModuleBase = (UINT_PTR)codeInfo.GetModuleBase();
    }

    if (pFuncEntry)
    {
        *pFuncEntry = (UINT_PTR)(PT_RUNTIME_FUNCTION)codeInfo.GetFunctionEntry();
    }
}
#endif // FEATURE_EH_FUNCLETS

#ifdef DACCESS_COMPILE

void EECodeGenManager::EnumMemoryRegions(CLRDataEnumMemoryFlags flags)
{
    IJitManager::EnumMemoryRegions(flags);

    //
    // Save all of the code heaps.
    //

    HeapList* heap;
    for (heap = m_pAllCodeHeaps; heap; heap = heap->GetNext())
    {
        DacEnumHostDPtrMem(heap);

        if (heap->pHeap.IsValid())
        {
            heap->pHeap->EnumMemoryRegions(flags);
        }

        DacEnumMemoryRegion(heap->startAddress, (ULONG32)
                            (heap->endAddress - heap->startAddress));

        if (heap->pHdrMap.IsValid())
        {
            ULONG32 nibbleMapSize = (ULONG32)
                HEAP2MAPSIZE(ROUND_UP_TO_PAGE(heap->maxCodeHeapSize));
            DacEnumMemoryRegion(dac_cast<TADDR>(heap->pHdrMap), nibbleMapSize);
        }
    }
}
#endif // #ifdef DACCESS_COMPILE



#ifndef DACCESS_COMPILE

//*******************************************************
// Execution Manager
//*******************************************************

// Init statics
void ExecutionManager::Init()
{
    CONTRACTL {
        THROWS;
        GC_NOTRIGGER;
    } CONTRACTL_END;

    m_JumpStubCrst.Init(CrstJumpStubCache, CrstFlags(CRST_UNSAFE_ANYMODE|CRST_DEBUGGER_THREAD));

    new(&g_codeRangeMap) RangeSectionMap();

    m_pDefaultCodeMan = new EECodeManager();

    m_pEEJitManager = new EEJitManager();


#ifdef FEATURE_READYTORUN
    m_pReadyToRunJitManager = new ReadyToRunJitManager();
#endif

#ifdef FEATURE_INTERPRETER
    m_pInterpreterCodeMan = new InterpreterCodeManager();
    m_pInterpreterJitManager = new InterpreterJitManager();
#endif
}

#endif // #ifndef DACCESS_COMPILE

//**************************************************************************
RangeSection *
ExecutionManager::FindCodeRange(PCODE currentPC, ScanFlag scanFlag)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

    if (currentPC == (PCODE)NULL)
        return NULL;

    if (scanFlag == ScanReaderLock)
        return FindCodeRangeWithLock(currentPC);

    // Since ScanReaderLock is not set, then we should behave AS IF the ReaderLock is held
    RangeSectionLockState lockState = RangeSectionLockState::ReaderLocked;
    return GetRangeSection(currentPC, &lockState);
}

//**************************************************************************
NOINLINE // Make sure that the slow path with lock won't affect the fast path
RangeSection *
ExecutionManager::FindCodeRangeWithLock(PCODE currentPC)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

    RangeSectionLockState lockState = RangeSectionLockState::None;
    RangeSection *result = GetRangeSection(currentPC, &lockState);
    if (lockState == RangeSectionLockState::NeedsLock)
    {
        ReaderLockHolder rlh;
        lockState = RangeSectionLockState::ReaderLocked;
        result = GetRangeSection(currentPC, &lockState);
    }
    return result;
}


//**************************************************************************
PCODE ExecutionManager::GetCodeStartAddress(PCODE currentPC)
{
    WRAPPER_NO_CONTRACT;
    _ASSERTE(currentPC != (PCODE)NULL);

    EECodeInfo codeInfo(currentPC);
    if (!codeInfo.IsValid())
        return (PCODE)NULL;
    return PINSTRToPCODE(codeInfo.GetStartAddress());
}

//**************************************************************************
NativeCodeVersion ExecutionManager::GetNativeCodeVersion(PCODE currentPC)
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        FORBID_FAULT;
    }
    CONTRACTL_END;

    EECodeInfo codeInfo(currentPC);
    return codeInfo.IsValid() ? codeInfo.GetNativeCodeVersion() : NativeCodeVersion();
}

//**************************************************************************
MethodDesc * ExecutionManager::GetCodeMethodDesc(PCODE currentPC)
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        FORBID_FAULT;
    }
    CONTRACTL_END

    EECodeInfo codeInfo(currentPC);
    if (!codeInfo.IsValid())
        return NULL;
    return codeInfo.GetMethodDesc();
}

//**************************************************************************
BOOL ExecutionManager::IsManagedCode(PCODE currentPC)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
    } CONTRACTL_END;

    if (currentPC == (PCODE)NULL)
        return FALSE;

    if (GetScanFlags() == ScanReaderLock)
        return IsManagedCodeWithLock(currentPC);

    // Since ScanReaderLock is not set, then we must assume that the ReaderLock is effectively taken.
    RangeSectionLockState lockState = RangeSectionLockState::ReaderLocked;
    return IsManagedCodeWorker(currentPC, &lockState);
}

//**************************************************************************
NOINLINE // Make sure that the slow path with lock won't affect the fast path
BOOL ExecutionManager::IsManagedCodeWithLock(PCODE currentPC)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
    } CONTRACTL_END;

    RangeSectionLockState lockState = RangeSectionLockState::None;
    BOOL result = IsManagedCodeWorker(currentPC, &lockState);

    if (lockState == RangeSectionLockState::NeedsLock)
    {
        ReaderLockHolder rlh;
        lockState = RangeSectionLockState::ReaderLocked;
        result = IsManagedCodeWorker(currentPC, &lockState);
    }

    return result;
}

//**************************************************************************
// Assumes that the ExecutionManager reader/writer lock is taken or that
// it is safe not to take it.
BOOL ExecutionManager::IsManagedCodeWorker(PCODE currentPC, RangeSectionLockState *pLockState)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
    } CONTRACTL_END;

    // This may get called for arbitrary code addresses. Note that the lock is
    // taken over the call to JitCodeToMethodInfo too so that nobody pulls out
    // the range section from underneath us.

    RangeSection * pRS = GetRangeSection(currentPC, pLockState);
    if (pRS == NULL)
        return FALSE;

#ifdef FEATURE_INTERPRETER
    if (pRS->_flags & RangeSection::RANGE_SECTION_INTERPRETER)
    {
        if (dac_cast<PTR_InterpreterJitManager>(pRS->_pjit)->JitCodeToMethodInfo(pRS, currentPC, NULL, NULL))
            return TRUE;
    }
    else
#endif
    if (pRS->_flags & RangeSection::RANGE_SECTION_CODEHEAP)
    {
        // Typically if we find a Jit Manager we are inside a managed method
        // but on we could also be in a stub, so we check for that
        // as well and we don't consider stub to be real managed code.
        TADDR start = dac_cast<PTR_EECodeGenManager>(pRS->_pjit)->FindMethodCode(pRS, currentPC);
        if (start == (TADDR)0)
            return FALSE;
        CodeHeader * pCHdr = PTR_CodeHeader(start - sizeof(CodeHeader));
        if (!pCHdr->IsStubCodeBlock())
            return TRUE;
    }
#ifdef FEATURE_READYTORUN
    else
    if (pRS->_pR2RModule != NULL)
    {
        if (dac_cast<PTR_ReadyToRunJitManager>(pRS->_pjit)->JitCodeToMethodInfo(pRS, currentPC, NULL, NULL))
            return TRUE;
    }
#endif

    return FALSE;
}

//**************************************************************************
// Assumes that it is safe not to take it the ExecutionManager reader/writer lock
BOOL ExecutionManager::IsReadyToRunCode(PCODE currentPC)
{
    CONTRACTL{
        NOTHROW;
        GC_NOTRIGGER;
    } CONTRACTL_END;

#ifdef FEATURE_READYTORUN
    RangeSectionLockState lockState = RangeSectionLockState::None;
    RangeSection * pRS = GetRangeSection(currentPC, &lockState);

    // Since R2R images are not collectible, and we always can find
    // non-collectible RangeSections without taking a lock we don't need
    // to take the actual ReaderLock here if GetRangeSection returns NULL

#ifdef _DEBUG
    if (pRS == NULL)
    {
        // This logic checks to ensure that the behavior of the fully locked
        // lookup matches that of the unlocked lookup.
        // Note that if the locked lookup finds something, we need to check
        // the unlocked lookup, in case a new module was loaded in the meantime.
        ReaderLockHolder rlh;
        lockState = RangeSectionLockState::ReaderLocked;
        if (GetRangeSection(currentPC, &lockState) != NULL)
        {
            lockState = RangeSectionLockState::None;
            assert(GetRangeSection(currentPC, &lockState) == NULL);
        }
    }
#endif // _DEBUG

    if (pRS != NULL && (pRS->_pR2RModule != NULL))
    {
        if (dac_cast<PTR_ReadyToRunJitManager>(pRS->_pjit)->JitCodeToMethodInfo(pRS, currentPC, NULL, NULL))
            return TRUE;
    }
#endif

    return FALSE;
}

#ifndef FEATURE_STATICALLY_LINKED
/*********************************************************************/
// This static method returns the name of the jit dll
//
LPCWSTR ExecutionManager::GetJitName()
{
    STANDARD_VM_CONTRACT;

    // Try to obtain a name for the jit library from the env. variable
    LPWSTR pwzJitNameMaybe = NULL;
    IfFailThrow(CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_JitName, &pwzJitNameMaybe));

    LPCWSTR  pwzJitName = pwzJitNameMaybe;
    if (NULL == pwzJitName)
    {
        pwzJitName = MAKEDLLNAME_W(W("clrjit"));
    }

    return pwzJitName;
}

#endif // !FEATURE_STATICALLY_LINKED

#ifdef FEATURE_INTERPRETER

// This static method returns the name of the interpreter dll
//
LPCWSTR ExecutionManager::GetInterpreterName()
{
    STANDARD_VM_CONTRACT;

    // Try to obtain a name for the jit library from the env. variable
    LPWSTR pwzInterpreterNameMaybe = NULL;
    IfFailThrow(CLRConfig::GetConfigValue(CLRConfig::EXTERNAL_InterpreterName, &pwzInterpreterNameMaybe));

    LPCWSTR pwzInterpreterName = pwzInterpreterNameMaybe;
    if (NULL == pwzInterpreterName)
    {
        pwzInterpreterName = MAKEDLLNAME_W(W("clrinterpreter"));
    }

    return pwzInterpreterName;
}
#endif // FEATURE_INTERPRETER

RangeSection* ExecutionManager::GetRangeSection(TADDR addr, RangeSectionLockState *pLockState)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

    return GetCodeRangeMap()->LookupRangeSection(addr, pLockState);
}

/* static */
PTR_Module ExecutionManager::FindReadyToRunModule(TADDR currentData)
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        MODE_ANY;
        SUPPORTS_DAC;
    }
    CONTRACTL_END;

#ifdef FEATURE_READYTORUN
    RangeSectionLockState lockState = RangeSectionLockState::None;
    RangeSection * pRS = GetRangeSection(currentData, &lockState);

    // Since R2R images are not collectible, and we always can find
    // non-collectible RangeSections without taking a lock we don't need
    // to take the actual ReaderLock here if GetRangeSection returns NULL
    if (pRS == NULL)
    {
#ifdef _DEBUG
        {
            // This logic checks to ensure that the behavior of the fully locked
            // lookup matches that of the unlocked lookup.
            // Note that if the locked lookup finds something, we need to check
            // the unlocked lookup, in case a new module was loaded in the meantime.
            ReaderLockHolder rlh;
            lockState = RangeSectionLockState::ReaderLocked;
            if (GetRangeSection(currentData, &lockState) != NULL)
            {
                lockState = RangeSectionLockState::None;
                assert(GetRangeSection(currentData, &lockState) == NULL);
            }
        }
#endif // _DEBUG

        return NULL;
    }

    return pRS->_pR2RModule;
#else
    return NULL;
#endif
}


/* static */
PTR_Module ExecutionManager::FindModuleForGCRefMap(TADDR currentData)
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    }
    CONTRACTL_END;

#ifndef FEATURE_READYTORUN
    return NULL;
#else
    RangeSection * pRS = FindCodeRange(currentData, ExecutionManager::GetScanFlags());
    if (pRS == NULL)
        return NULL;

    return pRS->_pR2RModule;
#endif // FEATURE_READYTORUN
}

#ifndef DACCESS_COMPILE

NOINLINE
void ExecutionManager::AddCodeRange(TADDR          pStartRange,
                                    TADDR          pEndRange,
                                    IJitManager *  pJit,
                                    RangeSection::RangeSectionFlags flags,
                                    PTR_Module     pModule)
{
    CONTRACTL {
        THROWS;
        GC_NOTRIGGER;
        PRECONDITION(pStartRange < pEndRange);
        PRECONDITION(CheckPointer(pJit));
        PRECONDITION(CheckPointer(pModule));
    } CONTRACTL_END;

    ReaderLockHolder rlh;
    RangeSectionLockState lockState = RangeSectionLockState::ReaderLocked; //

    PTR_RangeSection pRange = GetCodeRangeMap()->AllocateRange(Range(pStartRange, pEndRange), pJit, flags, pModule, &lockState);
    if (pRange == NULL)
        ThrowOutOfMemory();
}

NOINLINE
void ExecutionManager::AddCodeRange(TADDR          pStartRange,
                                    TADDR          pEndRange,
                                    IJitManager *  pJit,
                                    RangeSection::RangeSectionFlags flags,
                                    PTR_HeapList   pHp)
{
    CONTRACTL {
        THROWS;
        GC_NOTRIGGER;
        PRECONDITION(pStartRange < pEndRange);
        PRECONDITION(CheckPointer(pJit));
        PRECONDITION(CheckPointer(pHp));
    } CONTRACTL_END;

    ReaderLockHolder rlh;
    RangeSectionLockState lockState = RangeSectionLockState::ReaderLocked; //

    PTR_RangeSection pRange = GetCodeRangeMap()->AllocateRange(Range(pStartRange, pEndRange), pJit, flags, pHp, &lockState);

    if (pRange == NULL)
        ThrowOutOfMemory();
}

NOINLINE
void ExecutionManager::AddCodeRange(TADDR          pStartRange,
                                    TADDR          pEndRange,
                                    IJitManager *  pJit,
                                    RangeSection::RangeSectionFlags flags,
                                    PTR_CodeRangeMapRangeList   pRangeList)
{
    CONTRACTL {
        THROWS;
        GC_NOTRIGGER;
        PRECONDITION(pStartRange < pEndRange);
        PRECONDITION(CheckPointer(pJit));
        PRECONDITION(CheckPointer(pRangeList));
    } CONTRACTL_END;

    ReaderLockHolder rlh;
    RangeSectionLockState lockState = RangeSectionLockState::ReaderLocked; //

    PTR_RangeSection pRange = GetCodeRangeMap()->AllocateRange(Range(pStartRange, pEndRange), pJit, flags, pRangeList, &lockState);

    if (pRange == NULL)
        ThrowOutOfMemory();
}

// Deletes a single range starting at pStartRange
void ExecutionManager::DeleteRange(TADDR pStartRange)
{
    CONTRACTL {
        NOTHROW; // If this becomes throwing, then revisit the queuing of deletes below.
        GC_NOTRIGGER;
    } CONTRACTL_END;

    RangeSection *pCurr = FindCodeRangeWithLock(pStartRange);
    GetCodeRangeMap()->RemoveRangeSection(pCurr);


#if defined(TARGET_AMD64)
    PTR_UnwindInfoTable unwindTable = pCurr->_pUnwindInfoTable;
#endif

    {
        // Acquire the WriterLock and prevent any readers from walking the RangeList.
        // This also forces us to enter a forbid suspend thread region, to prevent
        // hijacking profilers from grabbing this thread and walking it (the walk may
        // require the reader lock, which would cause a deadlock).
        WriterLockHolder wlh;

        RangeSectionLockState lockState = RangeSectionLockState::WriteLocked;

        GetCodeRangeMap()->CleanupRangeSections(&lockState);
        // Unlike the previous implementation, we no longer attempt to avoid freeing
        // the memory behind the RangeSection here, as we do not support the hosting
        // api taking over memory allocation.
    }

    //
    // Now delete the unwind info table
    //
#if defined(TARGET_AMD64)
    if (unwindTable != 0)
        delete unwindTable;
#endif // defined(TARGET_AMD64)
}

#endif // #ifndef DACCESS_COMPILE

#ifdef DACCESS_COMPILE

void RangeSection::EnumMemoryRegions(CLRDataEnumMemoryFlags flags)
{
    if (!DacEnumMemoryRegion(dac_cast<TADDR>(this), sizeof(*this)))
        return;

    if (_pjit.IsValid())
    {
        _pjit->EnumMemoryRegions(flags);
    }

#ifdef FEATURE_READYTORUN
    if (_pR2RModule != NULL)
    {
        if (_pR2RModule.IsValid())
        {
            _pR2RModule->EnumMemoryRegions(flags, true);
        }
    }
#endif // FEATURE_READYTORUN
}


void ExecutionManager::EnumMemoryRegions(CLRDataEnumMemoryFlags flags)
{
    ReaderLockHolder rlh;

    //
    // Report the global data portions.
    //

    GetCodeRangeMap().EnumMem();
    m_pDefaultCodeMan.EnumMem();

#ifdef FEATURE_INTERPRETER
    m_pInterpreterCodeMan.EnumMem();
#endif // FEATURE_INTERPRETER

    //
    // Walk structures and report.
    //
    GetCodeRangeMap()->EnumMemoryRegions(flags);
}
#endif // #ifdef DACCESS_COMPILE

#if !defined(DACCESS_COMPILE)

void ExecutionManager::Unload(LoaderAllocator *pLoaderAllocator)
{
    CONTRACTL {
        THROWS;
        GC_NOTRIGGER;
    } CONTRACTL_END;

    // a size of 0 is a signal to Nirvana to flush the entire cache
    FlushInstructionCache(GetCurrentProcess(),0,0);

    JumpStubCache * pJumpStubCache = (JumpStubCache *) pLoaderAllocator->m_pJumpStubCache;
    if (pJumpStubCache != NULL)
    {
        delete pJumpStubCache;
        pLoaderAllocator->m_pJumpStubCache = NULL;
    }

    GetEEJitManager()->Unload(pLoaderAllocator);
#ifdef FEATURE_INTERPRETER
    GetInterpreterJitManager()->Unload(pLoaderAllocator);
#endif // FEATURE_INTERPRETER
}

// This method is used by the JIT and the runtime for PreStubs. It will return
// the address of a short jump thunk that will jump to the 'target' address.
// It is only needed when the target architecture has a perferred call instruction
// that doesn't actually span the full address space.  This is true for x64 where
// the preferred call instruction is a 32-bit pc-rel call instruction.
// (This is also true on ARM64, but it not true for x86)
//
// For these architectures, in JITed code and in the prestub, we encode direct calls
// using the preferred call instruction and we also try to ensure that the Jitted
// code is within the 32-bit pc-rel range of clr.dll to allow direct JIT helper calls.
//
// When the call target is too far away to encode using the preferred call instruction.
// We will create a short code thunk that uncoditionally jumps to the target address.
// We call this jump thunk a "jumpStub" in the CLR code.
// We have the requirement that the "jumpStub" that we create on demand be usable by
// the preferred call instruction, this requires that on x64 the location in memory
// where we create the "jumpStub" be within the 32-bit pc-rel range of the call that
// needs it.
//
// The arguments to this method:
//  pMD    - the MethodDesc for the currenty managed method in Jitted code
//           or for the target method for a PreStub
//           It is required if calling from or to a dynamic method (LCG method)
//  target - The call target address (this is the address that was too far to encode)
//  loAddr
//  hiAddr - The range of the address that we must place the jumpStub in, so that it
//           can be used to encode the preferred call instruction.
//  pLoaderAllocator
//         - The Loader allocator to use for allocations, this can be null.
//           When it is null, then the pMD must be valid and is used to obtain
//           the allocator.
//
// This method will either locate and return an existing jumpStub thunk that can be
// reused for this request, because it meets all of the requirements necessary.
// Or it will allocate memory in the required region and create a new jumpStub that
// meets all of the requirements necessary.
//
// Note that for dynamic methods (LCG methods) we cannot share the jumpStubs between
// different methods. This is because we allow for the unloading (reclaiming) of
// individual dynamic methods. And we associate the jumpStub memory allocated with
// the dynamic method that requested the jumpStub.
//

PCODE ExecutionManager::jumpStub(MethodDesc* pMD, PCODE target,
                                 BYTE * loAddr,   BYTE * hiAddr,
                                 LoaderAllocator *pLoaderAllocator,
                                 bool throwOnOutOfMemoryWithinRange)
{
    CONTRACT(PCODE) {
        THROWS;
        GC_NOTRIGGER;
        MODE_ANY;
        PRECONDITION(pLoaderAllocator != NULL || pMD != NULL);
        PRECONDITION(loAddr < hiAddr);
        POSTCONDITION((RETVAL != NULL) || !throwOnOutOfMemoryWithinRange);
    } CONTRACT_END;

    PCODE jumpStub = (PCODE)NULL;

    if (pLoaderAllocator == NULL)
    {
        pLoaderAllocator = pMD->GetLoaderAllocator();
    }
    _ASSERTE(pLoaderAllocator != NULL);

    bool                 isLCG          = pMD && pMD->IsLCGMethod();
    LCGMethodResolver *  pResolver      = nullptr;
    JumpStubCache *      pJumpStubCache = (JumpStubCache *) pLoaderAllocator->m_pJumpStubCache;

    if (isLCG)
    {
        pResolver      = pMD->AsDynamicMethodDesc()->GetLCGMethodResolver();
        pJumpStubCache = pResolver->m_pJumpStubCache;
    }

    CrstHolder ch(&m_JumpStubCrst);
    if (pJumpStubCache == NULL)
    {
        pJumpStubCache = new JumpStubCache();
        if (isLCG)
        {
            pResolver->m_pJumpStubCache = pJumpStubCache;
        }
        else
        {
            pLoaderAllocator->m_pJumpStubCache = pJumpStubCache;
        }
    }

    if (isLCG)
    {
        // Increment counter of LCG jump stub lookup attempts
        m_LCG_JumpStubLookup++;
    }
    else
    {
        // Increment counter of normal jump stub lookup attempts
        m_normal_JumpStubLookup++;
    }

    // search for a matching jumpstub in the jumpStubCache
    //
    for (JumpStubTable::KeyIterator i = pJumpStubCache->m_Table.Begin(target),
        end = pJumpStubCache->m_Table.End(target); i != end; i++)
    {
        jumpStub = i->m_jumpStub;

        _ASSERTE(jumpStub != (PCODE)NULL);

        // Is the matching entry with the requested range?
        if (((TADDR)loAddr <= jumpStub) && (jumpStub <= (TADDR)hiAddr))
        {
            RETURN(jumpStub);
        }
    }

    // If we get here we need to create a new jump stub
    // add or change the jump stub table to point at the new one
    jumpStub = getNextJumpStub(pMD, target, loAddr, hiAddr, pLoaderAllocator, throwOnOutOfMemoryWithinRange); // this statement can throw
    if (jumpStub == (PCODE)NULL)
    {
        _ASSERTE(!throwOnOutOfMemoryWithinRange);
        RETURN((PCODE)NULL);
    }

    _ASSERTE(((TADDR)loAddr <= jumpStub) && (jumpStub <= (TADDR)hiAddr));

    LOG((LF_JIT, LL_INFO10000, "Add JumpStub to" FMT_ADDR "at" FMT_ADDR "\n",
            DBG_ADDR(target), DBG_ADDR(jumpStub) ));

    RETURN(jumpStub);
}

PCODE ExecutionManager::getNextJumpStub(MethodDesc* pMD, PCODE target,
                                        BYTE * loAddr, BYTE * hiAddr,
                                        LoaderAllocator *pLoaderAllocator,
                                        bool throwOnOutOfMemoryWithinRange)
{
    CONTRACT(PCODE) {
        THROWS;
        GC_NOTRIGGER;
        PRECONDITION(pLoaderAllocator != NULL);
        PRECONDITION(m_JumpStubCrst.OwnedByCurrentThread());
        POSTCONDITION((RETVAL != NULL) || !throwOnOutOfMemoryWithinRange);
    } CONTRACT_END;

    BYTE *           jumpStub       = NULL;
    BYTE *           jumpStubRW     = NULL;
    bool             isLCG          = pMD && pMD->IsLCGMethod();
    // For LCG we request a small block of 4 jumpstubs, because we can not share them
    // with any other methods and very frequently our method only needs one jump stub.
    // Using 4 gives a request size of (32 + 4*12) or 80 bytes.
    // Also note that request sizes are rounded up to a multiples of 16.
    // The request size is calculated into 'blockSize' in AllocJumpStubBlock.
    // For x64 the value of BACK_TO_BACK_JUMP_ALLOCATE_SIZE is 12 bytes
    // and the sizeof(JumpStubBlockHeader) is 32.
    //
    DWORD            numJumpStubs   = isLCG ? 4 : DEFAULT_JUMPSTUBS_PER_BLOCK;
    JumpStubCache *  pJumpStubCache = (JumpStubCache *) pLoaderAllocator->m_pJumpStubCache;

    if (isLCG)
    {
        LCGMethodResolver *  pResolver;
        pResolver      = pMD->AsDynamicMethodDesc()->GetLCGMethodResolver();
        pJumpStubCache = pResolver->m_pJumpStubCache;
    }

    JumpStubBlockHeader ** ppHead   = &(pJumpStubCache->m_pBlocks);
    JumpStubBlockHeader *  curBlock = *ppHead;
    ExecutableWriterHolderNoLog<JumpStubBlockHeader> curBlockWriterHolder;

    // allocate a new jumpstub from 'curBlock' if it is not fully allocated
    //
    while (curBlock)
    {
        _ASSERTE(pLoaderAllocator == (isLCG ? curBlock->GetHostCodeHeap()->GetAllocator() : curBlock->GetLoaderAllocator()));

        if (curBlock->m_used < curBlock->m_allocated)
        {
            jumpStub = (BYTE *) curBlock + sizeof(JumpStubBlockHeader) + ((size_t) curBlock->m_used * BACK_TO_BACK_JUMP_ALLOCATE_SIZE);

            if ((loAddr <= jumpStub) && (jumpStub <= hiAddr))
            {
                // We will update curBlock->m_used at "DONE"
                size_t blockSize = sizeof(JumpStubBlockHeader) + (size_t) numJumpStubs * BACK_TO_BACK_JUMP_ALLOCATE_SIZE;
                curBlockWriterHolder.AssignExecutableWriterHolder(curBlock, blockSize);
                jumpStubRW = (BYTE *)((TADDR)jumpStub + (TADDR)curBlockWriterHolder.GetRW() - (TADDR)curBlock);
                goto DONE;
            }
        }
        curBlock = curBlock->m_next;
    }

    // If we get here then we need to allocate a new JumpStubBlock

    if (isLCG)
    {
#ifdef TARGET_AMD64
        // Note this these values are not requirements, instead we are
        // just confirming the values that are mentioned in the comments.
        _ASSERTE(BACK_TO_BACK_JUMP_ALLOCATE_SIZE == 12);
        _ASSERTE(sizeof(JumpStubBlockHeader) == 32);
#endif

        // Increment counter of LCG jump stub block allocations
        m_LCG_JumpStubBlockAllocCount++;
    }
    else
    {
        // Increment counter of normal jump stub block allocations
        m_normal_JumpStubBlockAllocCount++;
    }

    // AllocJumpStubBlock will allocate from the LoaderCodeHeap for normal methods
    // and will allocate from a HostCodeHeap for LCG methods.
    //
    // note that this can throw an OOM exception

    curBlock = ExecutionManager::GetEEJitManager()->AllocJumpStubBlock(pMD, numJumpStubs, loAddr, hiAddr, pLoaderAllocator, throwOnOutOfMemoryWithinRange);
    if (curBlock == NULL)
    {
        _ASSERTE(!throwOnOutOfMemoryWithinRange);
        RETURN((PCODE)NULL);
    }

    curBlockWriterHolder.AssignExecutableWriterHolder(curBlock, sizeof(JumpStubBlockHeader) + ((size_t) (curBlock->m_used + 1) * BACK_TO_BACK_JUMP_ALLOCATE_SIZE));

    jumpStubRW = (BYTE *) curBlockWriterHolder.GetRW() + sizeof(JumpStubBlockHeader) + ((size_t) curBlock->m_used * BACK_TO_BACK_JUMP_ALLOCATE_SIZE);
    jumpStub = (BYTE *) curBlock + sizeof(JumpStubBlockHeader) + ((size_t) curBlock->m_used * BACK_TO_BACK_JUMP_ALLOCATE_SIZE);

    _ASSERTE((loAddr <= jumpStub) && (jumpStub <= hiAddr));

    curBlockWriterHolder.GetRW()->m_next = *ppHead;
    *ppHead = curBlock;

DONE:

    _ASSERTE((curBlock->m_used < curBlock->m_allocated));

#ifdef TARGET_ARM64
    // 8-byte alignment is required on ARM64
    _ASSERTE(((UINT_PTR)jumpStub & 7) == 0);
#endif

    emitBackToBackJump(jumpStub, jumpStubRW, (void*) target);

#ifdef FEATURE_PERFMAP
    PerfMap::LogStubs(__FUNCTION__, "emitBackToBackJump", (PCODE)jumpStub, BACK_TO_BACK_JUMP_ALLOCATE_SIZE, PerfMapStubType::IndividualWithinBlock);
#endif

    // We always add the new jumpstub to the jumpStubCache
    //
    _ASSERTE(pJumpStubCache != NULL);

    JumpStubEntry entry;

    entry.m_target = target;
    entry.m_jumpStub = (PCODE)jumpStub;

    pJumpStubCache->m_Table.Add(entry);

    curBlockWriterHolder.GetRW()->m_used++;    // record that we have used up one more jumpStub in the block

    // Every time we create a new jumpStub thunk one of these counters is incremented
    if (isLCG)
    {
        // Increment counter of LCG unique jump stubs
        m_LCG_JumpStubUnique++;
    }
    else
    {
        // Increment counter of normal unique jump stubs
        m_normal_JumpStubUnique++;
    }

    // Is the 'curBlock' now completely full?
    if (curBlock->m_used == curBlock->m_allocated)
    {
        if (isLCG)
        {
            // Increment counter of LCG jump stub blocks that are full
            m_LCG_JumpStubBlockFullCount++;

            // Log this "LCG JumpStubBlock filled" along with the four counter values
            STRESS_LOG4(LF_JIT, LL_INFO1000, "LCG JumpStubBlock filled - (%u, %u, %u, %u)\n",
                        m_LCG_JumpStubLookup, m_LCG_JumpStubUnique,
                        m_LCG_JumpStubBlockAllocCount, m_LCG_JumpStubBlockFullCount);
        }
        else
        {
            // Increment counter of normal jump stub blocks that are full
            m_normal_JumpStubBlockFullCount++;

            // Log this "normal JumpStubBlock filled" along with the four counter values
            STRESS_LOG4(LF_JIT, LL_INFO1000, "Normal JumpStubBlock filled - (%u, %u, %u, %u)\n",
                        m_normal_JumpStubLookup, m_normal_JumpStubUnique,
                        m_normal_JumpStubBlockAllocCount, m_normal_JumpStubBlockFullCount);

            if ((m_LCG_JumpStubLookup > 0) && ((m_normal_JumpStubBlockFullCount % 5) == 1))
            {
                // Every 5 occurrence of the above we also
                // Log "LCG JumpStubBlock status" along with the four counter values
                STRESS_LOG4(LF_JIT, LL_INFO1000, "LCG JumpStubBlock status - (%u, %u, %u, %u)\n",
                            m_LCG_JumpStubLookup, m_LCG_JumpStubUnique,
                            m_LCG_JumpStubBlockAllocCount, m_LCG_JumpStubBlockFullCount);
            }
        }
    }

    RETURN((PCODE)jumpStub);
}
#endif // !DACCESS_COMPILE

static void GetFuncletStartOffsetsHelper(PCODE pCodeStart, SIZE_T size, SIZE_T ofsAdj,
    PTR_RUNTIME_FUNCTION pFunctionEntry, TADDR moduleBase,
    DWORD * pnFunclets, DWORD* pStartFuncletOffsets, DWORD dwLength)
{
    _ASSERTE(FitsInU4((pCodeStart + size) - moduleBase));
    DWORD endAddress = (DWORD)((pCodeStart + size) - moduleBase);

    // Entries are sorted and terminated by sentinel value (DWORD)-1
    for (; RUNTIME_FUNCTION__BeginAddress(pFunctionEntry) < endAddress; pFunctionEntry++)
    {
#ifdef TARGET_AMD64
        _ASSERTE((pFunctionEntry->UnwindData & RUNTIME_FUNCTION_INDIRECT) == 0);
#endif

#if defined(EXCEPTION_DATA_SUPPORTS_FUNCTION_FRAGMENTS)
        if (IsFunctionFragment(moduleBase, pFunctionEntry))
        {
            // This is a fragment (not the funclet beginning); skip it
            continue;
        }
#endif // EXCEPTION_DATA_SUPPORTS_FUNCTION_FRAGMENTS

        if (*pnFunclets < dwLength)
        {
            TADDR funcletStartAddress = (moduleBase + RUNTIME_FUNCTION__BeginAddress(pFunctionEntry)) + ofsAdj;
            _ASSERTE(FitsInU4(funcletStartAddress - pCodeStart));
            pStartFuncletOffsets[*pnFunclets] = (DWORD)(funcletStartAddress - pCodeStart);
        }
        (*pnFunclets)++;
    }
}

#if defined(FEATURE_EH_FUNCLETS) && defined(DACCESS_COMPILE)

//
// To locate an entry in the function entry table (the program exceptions data directory), the debugger
// performs a binary search over the table.  This function reports the entries that are encountered in the
// binary search.
//
// Parameters:
//   pRtf: The target function table entry to be located
//   pNativeLayout: A pointer to the loaded native layout for the module containing pRtf
//
static void EnumRuntimeFunctionEntriesToFindEntry(PTR_RUNTIME_FUNCTION pRtf, PTR_PEImageLayout pNativeLayout)
{
    pRtf.EnumMem();

    if (pNativeLayout == NULL)
    {
        return;
    }

    IMAGE_DATA_DIRECTORY * pProgramExceptionsDirectory = pNativeLayout->GetDirectoryEntry(IMAGE_DIRECTORY_ENTRY_EXCEPTION);
    if (!pProgramExceptionsDirectory ||
        (pProgramExceptionsDirectory->Size == 0) ||
        (pProgramExceptionsDirectory->Size % sizeof(T_RUNTIME_FUNCTION) != 0))
    {
        // Program exceptions directory malformatted
        return;
    }

    PTR_BYTE moduleBase(pNativeLayout->GetBase());
    PTR_RUNTIME_FUNCTION firstFunctionEntry(moduleBase + pProgramExceptionsDirectory->VirtualAddress);

    if (pRtf < firstFunctionEntry ||
        ((dac_cast<TADDR>(pRtf) - dac_cast<TADDR>(firstFunctionEntry)) % sizeof(T_RUNTIME_FUNCTION) != 0))
    {
        // Program exceptions directory malformatted
        return;
    }

    UINT_PTR indexToLocate = pRtf - firstFunctionEntry;

    UINT_PTR low = 0; // index in the function entry table of low end of search range
    UINT_PTR high = (pProgramExceptionsDirectory->Size) / sizeof(T_RUNTIME_FUNCTION) - 1; // index of high end of search range
    UINT_PTR mid = (low + high) / 2; // index of entry to be compared

    if (indexToLocate > high)
    {
        return;
    }

    while (indexToLocate != mid)
    {
        PTR_RUNTIME_FUNCTION functionEntry = firstFunctionEntry + mid;
        functionEntry.EnumMem();
        if (indexToLocate > mid)
        {
            low = mid + 1;
        }
        else
        {
            high = mid - 1;
        }
        mid = (low + high) / 2;
        _ASSERTE(low <= mid && mid <= high);
    }
}
#endif // FEATURE_EH_FUNCLETS

#if defined(FEATURE_READYTORUN)

// Return start of exception info for a method, or 0 if the method has no EH info
DWORD NativeExceptionInfoLookupTable::LookupExceptionInfoRVAForMethod(PTR_CORCOMPILE_EXCEPTION_LOOKUP_TABLE pExceptionLookupTable,
                                                                              COUNT_T numLookupEntries,
                                                                              DWORD methodStartRVA,
                                                                              COUNT_T* pSize)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

    _ASSERTE(pExceptionLookupTable != NULL);

    COUNT_T start = 0;
    COUNT_T end = numLookupEntries - 2;

    // The last entry in the lookup table (end-1) points to a sentinel entry.
    // The sentinel entry helps to determine the number of EH clauses for the last table entry.
    _ASSERTE(pExceptionLookupTable->ExceptionLookupEntry(numLookupEntries-1)->MethodStartRVA == (DWORD)-1);

    // Binary search the lookup table
    // Using linear search is faster once we get down to small number of entries.
    while (end - start > 10)
    {
        COUNT_T middle = start + (end - start) / 2;

        _ASSERTE(start < middle && middle < end);

        DWORD rva = pExceptionLookupTable->ExceptionLookupEntry(middle)->MethodStartRVA;

        if (methodStartRVA < rva)
        {
            end = middle - 1;
        }
        else
        {
            start = middle;
        }
    }

    for (COUNT_T i = start; i <= end; ++i)
    {
        DWORD rva = pExceptionLookupTable->ExceptionLookupEntry(i)->MethodStartRVA;
        if (methodStartRVA  == rva)
        {
            CORCOMPILE_EXCEPTION_LOOKUP_TABLE_ENTRY *pEntry = pExceptionLookupTable->ExceptionLookupEntry(i);

            //Get the count of EH Clause entries
            CORCOMPILE_EXCEPTION_LOOKUP_TABLE_ENTRY * pNextEntry = pExceptionLookupTable->ExceptionLookupEntry(i + 1);
            *pSize = pNextEntry->ExceptionInfoRVA - pEntry->ExceptionInfoRVA;

            return pEntry->ExceptionInfoRVA;
        }
    }

    // Not found
    return 0;
}

int NativeUnwindInfoLookupTable::LookupUnwindInfoForMethod(DWORD RelativePc,
                                                           PTR_RUNTIME_FUNCTION pRuntimeFunctionTable,
                                                           int Low,
                                                           int High)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;


#ifdef TARGET_ARM
    RelativePc |= THUMB_CODE;
#endif

    // Entries are sorted and terminated by sentinel value (DWORD)-1

    // Binary search the RUNTIME_FUNCTION table
    // Use linear search once we get down to a small number of elements
    // to avoid Binary search overhead.
    while (High - Low > 10)
    {
       int Middle = Low + (High - Low) / 2;

       PTR_RUNTIME_FUNCTION pFunctionEntry = pRuntimeFunctionTable + Middle;
       if (RelativePc < pFunctionEntry->BeginAddress)
       {
           High = Middle - 1;
       }
       else
       {
           Low = Middle;
       }
    }

    for (int i = Low; i <= High; ++i)
    {
        // This is safe because of entries are terminated by sentinel value (DWORD)-1
        PTR_RUNTIME_FUNCTION pNextFunctionEntry = pRuntimeFunctionTable + (i + 1);

        if (RelativePc < pNextFunctionEntry->BeginAddress)
        {
            PTR_RUNTIME_FUNCTION pFunctionEntry = pRuntimeFunctionTable + i;
            if (RelativePc >= pFunctionEntry->BeginAddress)
            {
                return i;
            }
            break;
        }
    }

    return -1;
}

int HotColdMappingLookupTable::LookupMappingForMethod(ReadyToRunInfo* pInfo, ULONG MethodIndex)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

    if (pInfo->m_nHotColdMap == 0)
    {
        return -1;
    }

    // Casting the lookup table size to an int is safe:
    // We index the RUNTIME_FUNCTION table with ints, and the lookup table
    // contains a subset of the indices in the RUNTIME_FUNCTION table.
    // Thus, |lookup table| <= |RUNTIME_FUNCTION table|.
    _ASSERTE(pInfo->m_nHotColdMap <= pInfo->m_nRuntimeFunctions);
    const int nLookupTable = (int)(pInfo->m_nHotColdMap);

    // The lookup table contains pairs of hot/cold indices, and thus should have an even size.
    _ASSERTE((nLookupTable % 2) == 0);
    int high = ((nLookupTable - 1) / 2);
    int low  = 0;

    const int indexCorrection = (int)(MethodIndex < pInfo->m_pHotColdMap[0]);

    // Binary search the lookup table.
    // Use linear search once we get down to a small number of elements
    // to avoid binary search overhead.
    while (high - low > 10)
    {
        const int middle = low + (high - low) / 2;
        const int index = (middle * 2) + indexCorrection;

        if (MethodIndex < pInfo->m_pHotColdMap[index])
        {
            high = middle - 1;
        }
        else
        {
            low = middle;
        }
    }

    // In each pair of indices in lookup table, the first index is of the cold fragment.
    const bool isColdCode = (indexCorrection == 0);

    for (int i = low; i <= high; ++i)
    {
        const int index = (i * 2);

        if (pInfo->m_pHotColdMap[index + indexCorrection] == MethodIndex)
        {
            if (isColdCode)
            {
                return index + 1;
            }

            return index;
        }
        else if (isColdCode && (MethodIndex > pInfo->m_pHotColdMap[index]))
        {
            // If MethodIndex is a cold funclet from a cold block, the above search will fail.
            // To get its corresponding hot block, find the cold block containing the funclet,
            // then use the lookup table.
            // The cold funclet's MethodIndex will be greater than its cold block's MethodIndex,
            // but less than the next cold block's MethodIndex in the lookup table.
            const bool isFuncletIndex = ((index + 2) == nLookupTable) || (MethodIndex < pInfo->m_pHotColdMap[index + 2]);

            if (isFuncletIndex)
            {
                return index + 1;
            }
        }
    }

    return -1;
}


//***************************************************************************************
//***************************************************************************************

#ifndef DACCESS_COMPILE

ReadyToRunJitManager::ReadyToRunJitManager()
{
    WRAPPER_NO_CONTRACT;
}

#endif // #ifndef DACCESS_COMPILE

ReadyToRunInfo * ReadyToRunJitManager::JitTokenToReadyToRunInfo(const METHODTOKEN& MethodToken)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

    return MethodToken.m_pRangeSection->_pR2RModule->GetReadyToRunInfo();
}

UINT32 ReadyToRunJitManager::JitTokenToGCInfoVersion(const METHODTOKEN& MethodToken)
{
    CONTRACTL{
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

    READYTORUN_HEADER * header = JitTokenToReadyToRunInfo(MethodToken)->GetReadyToRunHeader();

    return GCInfoToken::ReadyToRunVersionToGcInfoVersion(header->MajorVersion, header->MinorVersion);
}

PTR_RUNTIME_FUNCTION ReadyToRunJitManager::JitTokenToRuntimeFunction(const METHODTOKEN& MethodToken)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

    return dac_cast<PTR_RUNTIME_FUNCTION>(MethodToken.m_pCodeHeader);
}

TADDR ReadyToRunJitManager::JitTokenToStartAddress(const METHODTOKEN& MethodToken)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

    return JitTokenToModuleBase(MethodToken) +
        RUNTIME_FUNCTION__BeginAddress(dac_cast<PTR_RUNTIME_FUNCTION>(MethodToken.m_pCodeHeader));
}

GCInfoToken ReadyToRunJitManager::GetGCInfoToken(const METHODTOKEN& MethodToken)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

    PTR_RUNTIME_FUNCTION pRuntimeFunction = JitTokenToRuntimeFunction(MethodToken);
    TADDR baseAddress = JitTokenToModuleBase(MethodToken);

    SIZE_T nUnwindDataSize;
    PTR_VOID pUnwindData = GetUnwindDataBlob(baseAddress, pRuntimeFunction, &nUnwindDataSize);

    // GCInfo immediately follows unwind data
    PTR_BYTE gcInfo = dac_cast<PTR_BYTE>(pUnwindData) + nUnwindDataSize;
    UINT32 gcInfoVersion = JitTokenToGCInfoVersion(MethodToken);

    return{ gcInfo, gcInfoVersion };
}

unsigned ReadyToRunJitManager::InitializeEHEnumeration(const METHODTOKEN& MethodToken, EH_CLAUSE_ENUMERATOR* pEnumState)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
    } CONTRACTL_END;

    ReadyToRunInfo * pReadyToRunInfo = JitTokenToReadyToRunInfo(MethodToken);

    IMAGE_DATA_DIRECTORY * pExceptionInfoDir = pReadyToRunInfo->FindSection(ReadyToRunSectionType::ExceptionInfo);
    if (pExceptionInfoDir == NULL)
        return 0;

    PEImageLayout * pLayout = pReadyToRunInfo->GetImage();

    PTR_CORCOMPILE_EXCEPTION_LOOKUP_TABLE pExceptionLookupTable = dac_cast<PTR_CORCOMPILE_EXCEPTION_LOOKUP_TABLE>(pLayout->GetRvaData(pExceptionInfoDir->VirtualAddress));

    COUNT_T numLookupTableEntries = (COUNT_T)(pExceptionInfoDir->Size / sizeof(CORCOMPILE_EXCEPTION_LOOKUP_TABLE_ENTRY));
    // at least 2 entries (1 valid entry + 1 sentinel entry)
    _ASSERTE(numLookupTableEntries >= 2);

    DWORD methodStartRVA = (DWORD)(JitTokenToStartAddress(MethodToken) - JitTokenToModuleBase(MethodToken));

    COUNT_T ehInfoSize = 0;
    DWORD exceptionInfoRVA = NativeExceptionInfoLookupTable::LookupExceptionInfoRVAForMethod(pExceptionLookupTable,
                                                                  numLookupTableEntries,
                                                                  methodStartRVA,
                                                                  &ehInfoSize);
    if (exceptionInfoRVA == 0)
        return 0;

    pEnumState->iCurrentPos = 0;
    pEnumState->pExceptionClauseArray = JitTokenToModuleBase(MethodToken) + exceptionInfoRVA;

    return ehInfoSize / sizeof(CORCOMPILE_EXCEPTION_CLAUSE);
}

PTR_EXCEPTION_CLAUSE_TOKEN ReadyToRunJitManager::GetNextEHClause(EH_CLAUSE_ENUMERATOR* pEnumState,
                              EE_ILEXCEPTION_CLAUSE* pEHClauseOut)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
    } CONTRACTL_END;

    unsigned iCurrentPos = pEnumState->iCurrentPos;
    pEnumState->iCurrentPos++;

    CORCOMPILE_EXCEPTION_CLAUSE* pClause = &(dac_cast<PTR_CORCOMPILE_EXCEPTION_CLAUSE>(pEnumState->pExceptionClauseArray)[iCurrentPos]);

    // copy to the input parameter, this is a nice abstraction for the future
    // if we want to compress the Clause encoding, we can do without affecting the call sites
    pEHClauseOut->TryStartPC = pClause->TryStartPC;
    pEHClauseOut->TryEndPC = pClause->TryEndPC;
    pEHClauseOut->HandlerStartPC = pClause->HandlerStartPC;
    pEHClauseOut->HandlerEndPC = pClause->HandlerEndPC;
    pEHClauseOut->Flags = pClause->Flags;
    pEHClauseOut->FilterOffset = pClause->FilterOffset;

    return dac_cast<PTR_EXCEPTION_CLAUSE_TOKEN>(pClause);
}

StubCodeBlockKind ReadyToRunJitManager::GetStubCodeBlockKind(RangeSection * pRangeSection, PCODE currentPC)
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        MODE_ANY;
        SUPPORTS_DAC;
    }
    CONTRACTL_END;

    DWORD rva = (DWORD)(currentPC - pRangeSection->_range.RangeStart());

    PTR_ReadyToRunInfo pReadyToRunInfo = pRangeSection->_pR2RModule->GetReadyToRunInfo();

    PTR_IMAGE_DATA_DIRECTORY pDelayLoadMethodCallThunksDir = pReadyToRunInfo->GetDelayMethodCallThunksSection();
    if (pDelayLoadMethodCallThunksDir != NULL)
    {
        if (pDelayLoadMethodCallThunksDir->VirtualAddress <= rva
                && rva < pDelayLoadMethodCallThunksDir->VirtualAddress + pDelayLoadMethodCallThunksDir->Size)
            return STUB_CODE_BLOCK_METHOD_CALL_THUNK;
    }

    return STUB_CODE_BLOCK_UNKNOWN;
}

#ifndef DACCESS_COMPILE

TypeHandle ReadyToRunJitManager::ResolveEHClause(EE_ILEXCEPTION_CLAUSE* pEHClause,
                                              CrawlFrame* pCf)
{
    CONTRACTL {
        THROWS;
        GC_TRIGGERS;
    } CONTRACTL_END;

    _ASSERTE(NULL != pCf);
    _ASSERTE(NULL != pEHClause);
    _ASSERTE(IsTypedHandler(pEHClause));

    MethodDesc *pMD = PTR_MethodDesc(pCf->GetFunction());

    _ASSERTE(pMD != NULL);

    Module* pModule = pMD->GetModule();
    _ASSERTE(pModule != NULL);

    SigTypeContext typeContext(pMD);
    mdToken typeTok = pEHClause->ClassToken;
    return ClassLoader::LoadTypeDefOrRefOrSpecThrowing(pModule, typeTok, &typeContext,
                                                          ClassLoader::ReturnNullIfNotFound);
}

#endif // #ifndef DACCESS_COMPILE

//-----------------------------------------------------------------------------
// Ngen info manager
//-----------------------------------------------------------------------------
BOOL ReadyToRunJitManager::GetBoundariesAndVars(
        const DebugInfoRequest & request,
        IN FP_IDS_NEW fpNew,
        IN void * pNewData,
        BoundsType boundsType,
        OUT ULONG32 * pcMap,
        OUT ICorDebugInfo::OffsetMapping **ppMap,
        OUT ULONG32 * pcVars,
        OUT ICorDebugInfo::NativeVarInfo **ppVars)
{
    CONTRACTL {
        THROWS;       // on OOM.
        GC_NOTRIGGER; // getting vars shouldn't trigger
        SUPPORTS_DAC;
    } CONTRACTL_END;

    EECodeInfo codeInfo(request.GetStartAddress());
    if (!codeInfo.IsValid())
        return FALSE;

    ReadyToRunInfo * pReadyToRunInfo = JitTokenToReadyToRunInfo(codeInfo.GetMethodToken());
    PTR_RUNTIME_FUNCTION pRuntimeFunction = JitTokenToRuntimeFunction(codeInfo.GetMethodToken());

    PTR_BYTE pDebugInfo = pReadyToRunInfo->GetDebugInfo(pRuntimeFunction);
    if (pDebugInfo == NULL)
        return FALSE;

    // Uncompress. This allocates memory and may throw.
    CompressDebugInfo::RestoreBoundariesAndVars(
        fpNew,
        pNewData, // allocators
        boundsType,
        pDebugInfo,      // input
        pcMap, ppMap,    // output
        pcVars, ppVars,  // output
        FALSE);          // no patchpoint info

    return TRUE;
}

size_t ReadyToRunJitManager::WalkILOffsets(
    const DebugInfoRequest & request,
    BoundsType boundsType,
    void* pContext,
    size_t (* pfnWalkILOffsets)(ICorDebugInfo::OffsetMapping *pOffsetMapping, void *pContext))
{   
    CONTRACTL {
        THROWS;       // on OOM.
        GC_NOTRIGGER; // getting vars shouldn't trigger
        SUPPORTS_DAC;
    } CONTRACTL_END;

    EECodeInfo codeInfo(request.GetStartAddress());
    if (!codeInfo.IsValid())
        return FALSE;

    ReadyToRunInfo * pReadyToRunInfo = JitTokenToReadyToRunInfo(codeInfo.GetMethodToken());
    PTR_RUNTIME_FUNCTION pRuntimeFunction = JitTokenToRuntimeFunction(codeInfo.GetMethodToken());

    PTR_BYTE pDebugInfo = pReadyToRunInfo->GetDebugInfo(pRuntimeFunction);
    if (pDebugInfo == NULL)
        return FALSE;

    // Uncompress. This allocates memory and may throw.
    return CompressDebugInfo::WalkILOffsets(
        pDebugInfo,      // input
        boundsType,
        FALSE, // no patchpoint info
        pContext, pfnWalkILOffsets);
}

BOOL ReadyToRunJitManager::GetRichDebugInfo(
    const DebugInfoRequest& request,
    IN FP_IDS_NEW fpNew, IN void* pNewData,
    OUT ICorDebugInfo::InlineTreeNode** ppInlineTree,
    OUT ULONG32* pNumInlineTree,
    OUT ICorDebugInfo::RichOffsetMapping** ppRichMappings,
    OUT ULONG32* pNumRichMappings)
{
    return FALSE;
}

#ifdef DACCESS_COMPILE
//
// Need to write out debug info
//
void ReadyToRunJitManager::EnumMemoryRegionsForMethodDebugInfo(CLRDataEnumMemoryFlags flags, EECodeInfo * pCodeInfo)
{
    SUPPORTS_DAC;

    if (!pCodeInfo->IsValid())
        return;

    ReadyToRunInfo * pReadyToRunInfo = JitTokenToReadyToRunInfo(pCodeInfo->GetMethodToken());
    PTR_RUNTIME_FUNCTION pRuntimeFunction = JitTokenToRuntimeFunction(pCodeInfo->GetMethodToken());

    PTR_BYTE pDebugInfo = pReadyToRunInfo->GetDebugInfo(pRuntimeFunction);
    if (pDebugInfo == NULL)
        return;

    CompressDebugInfo::EnumMemoryRegions(flags, pDebugInfo, FALSE);
}
#endif

PCODE ReadyToRunJitManager::GetCodeAddressForRelOffset(const METHODTOKEN& MethodToken, DWORD relOffset)
{
    WRAPPER_NO_CONTRACT;

    MethodRegionInfo methodRegionInfo;
    JitTokenToMethodRegionInfo(MethodToken, &methodRegionInfo);

    if (relOffset < methodRegionInfo.hotSize)
        return methodRegionInfo.hotStartAddress + relOffset;

    SIZE_T coldOffset = relOffset - methodRegionInfo.hotSize;
    _ASSERTE(coldOffset < methodRegionInfo.coldSize);
    return methodRegionInfo.coldStartAddress + coldOffset;
}

BOOL ReadyToRunJitManager::JitCodeToMethodInfo(RangeSection * pRangeSection,
                                            PCODE currentPC,
                                            MethodDesc** ppMethodDesc,
                                            OUT EECodeInfo * pCodeInfo)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

    // If the address is in a thunk, return NULL.
    if (GetStubCodeBlockKind(pRangeSection, currentPC) != STUB_CODE_BLOCK_UNKNOWN)
    {
        return FALSE;
    }

    TADDR currentInstr = PCODEToPINSTR(currentPC);

    TADDR ImageBase = pRangeSection->_range.RangeStart();

    DWORD RelativePc = (DWORD)(currentInstr - ImageBase);

    Module * pModule = pRangeSection->_pR2RModule;
    ReadyToRunInfo * pInfo = pModule->GetReadyToRunInfo();

    COUNT_T nRuntimeFunctions = pInfo->m_nRuntimeFunctions;
    PTR_RUNTIME_FUNCTION pRuntimeFunctions = pInfo->m_pRuntimeFunctions;

    int MethodIndex = NativeUnwindInfoLookupTable::LookupUnwindInfoForMethod(RelativePc,
                                                                             pRuntimeFunctions,
                                                                             0,
                                                                             nRuntimeFunctions - 1);

    if (MethodIndex < 0)
        return FALSE;

    if (ppMethodDesc == NULL && pCodeInfo == NULL)
    {
        // Bail early if caller doesn't care about the MethodDesc or EECodeInfo.
        // Avoiding the method desc lookups below also prevents deadlocks when this
        // is called from IsManagedCode.
        return TRUE;
    }

#ifdef FEATURE_EH_FUNCLETS
    // Save the raw entry
    PTR_RUNTIME_FUNCTION RawFunctionEntry = pRuntimeFunctions + MethodIndex;

    const int lookupIndex = HotColdMappingLookupTable::LookupMappingForMethod(pInfo, (ULONG)MethodIndex);
    if ((lookupIndex != -1) && ((lookupIndex & 1) == 1))
    {
        // If the MethodIndex happens to be the cold code block, turn it into the associated hot code block
        MethodIndex = pInfo->m_pHotColdMap[lookupIndex];
    }

    MethodDesc *pMethodDesc;
    while ((pMethodDesc = pInfo->GetMethodDescForEntryPoint(ImageBase + RUNTIME_FUNCTION__BeginAddress(pRuntimeFunctions + MethodIndex))) == NULL)
        MethodIndex--;
#endif

    PTR_RUNTIME_FUNCTION FunctionEntry = pRuntimeFunctions + MethodIndex;

    if (ppMethodDesc)
    {
#ifdef FEATURE_EH_FUNCLETS
        *ppMethodDesc = pMethodDesc;
#else
        *ppMethodDesc = pInfo->GetMethodDescForEntryPoint(ImageBase + RUNTIME_FUNCTION__BeginAddress(FunctionEntry));
#endif
        _ASSERTE(*ppMethodDesc != NULL);
    }

    if (pCodeInfo)
    {
        // We are using RUNTIME_FUNCTION as METHODTOKEN
        pCodeInfo->m_methodToken = METHODTOKEN(pRangeSection, dac_cast<TADDR>(FunctionEntry));

#ifdef FEATURE_EH_FUNCLETS
        AMD64_ONLY(_ASSERTE((RawFunctionEntry->UnwindData & RUNTIME_FUNCTION_INDIRECT) == 0));
        pCodeInfo->m_pFunctionEntry = RawFunctionEntry;
#endif
        MethodRegionInfo methodRegionInfo;
        JitTokenToMethodRegionInfo(pCodeInfo->m_methodToken, &methodRegionInfo);
        if ((methodRegionInfo.coldSize > 0) && (currentInstr >= methodRegionInfo.coldStartAddress))
        {
            pCodeInfo->m_relOffset = (DWORD)
                (methodRegionInfo.hotSize + (currentInstr - methodRegionInfo.coldStartAddress));
        }
        else
        {
            pCodeInfo->m_relOffset = (DWORD)(currentInstr - methodRegionInfo.hotStartAddress);
        }
    }

    return TRUE;
}

#if defined(FEATURE_EH_FUNCLETS)
PTR_RUNTIME_FUNCTION ReadyToRunJitManager::LazyGetFunctionEntry(EECodeInfo * pCodeInfo)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
    } CONTRACTL_END;

    if (!pCodeInfo->IsValid())
    {
        return NULL;
    }

    // code:ReadyToRunJitManager::JitCodeToMethodInfo computes PTR_RUNTIME_FUNCTION eagerly. This path is only
    // reachable via EECodeInfo::GetMainFunctionInfo, and so we can just return the main entry.
    _ASSERTE(pCodeInfo->GetRelOffset() == 0);

    return dac_cast<PTR_RUNTIME_FUNCTION>(pCodeInfo->GetMethodToken().m_pCodeHeader);
}

TADDR ReadyToRunJitManager::GetFuncletStartAddress(EECodeInfo * pCodeInfo)
{
    LIMITED_METHOD_DAC_CONTRACT;

    return IJitManager::GetFuncletStartAddress(pCodeInfo);
}

DWORD ReadyToRunJitManager::GetFuncletStartOffsets(const METHODTOKEN& MethodToken, DWORD* pStartFuncletOffsets, DWORD dwLength)
{
    PTR_RUNTIME_FUNCTION pFirstFuncletFunctionEntry = dac_cast<PTR_RUNTIME_FUNCTION>(MethodToken.m_pCodeHeader) + 1;

    TADDR moduleBase = JitTokenToModuleBase(MethodToken);
    DWORD nFunclets = 0;
    MethodRegionInfo regionInfo;
    JitTokenToMethodRegionInfo(MethodToken, &regionInfo);

    // pFirstFuncletFunctionEntry will work for ARM when passed to GetFuncletStartOffsetsHelper()
    // even if it is a fragment of the main body and not a RUNTIME_FUNCTION for the beginning
    // of the first hot funclet, because GetFuncletStartOffsetsHelper() will skip all the function
    // fragments until the first funclet, if any, is found.

    GetFuncletStartOffsetsHelper(regionInfo.hotStartAddress, regionInfo.hotSize, 0,
        pFirstFuncletFunctionEntry, moduleBase,
        &nFunclets, pStartFuncletOffsets, dwLength);

    // Technically, the cold code is not a funclet, but it looks like the debugger wants it
    if (regionInfo.coldSize > 0)
    {
        ReadyToRunInfo * pInfo = JitTokenToReadyToRunInfo(MethodToken);
        PTR_RUNTIME_FUNCTION pRuntimeFunctions = pInfo->m_pRuntimeFunctions;
        int i = 0;
        while (true)
        {
            pFirstFuncletFunctionEntry = pRuntimeFunctions + i;
            if (regionInfo.coldStartAddress == moduleBase + RUNTIME_FUNCTION__BeginAddress(pFirstFuncletFunctionEntry))
            {
                break;
            }
            i++;
        }

        GetFuncletStartOffsetsHelper(regionInfo.coldStartAddress, regionInfo.coldSize, 0,
            pFirstFuncletFunctionEntry, moduleBase,
            &nFunclets, pStartFuncletOffsets, dwLength);
    }

    return nFunclets;
}

BOOL ReadyToRunJitManager::LazyIsFunclet(EECodeInfo* pCodeInfo)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
    } CONTRACTL_END;

    ReadyToRunInfo * pInfo = JitTokenToReadyToRunInfo(pCodeInfo->GetMethodToken());

    COUNT_T nRuntimeFunctions = pInfo->m_nRuntimeFunctions;
    PTR_RUNTIME_FUNCTION pRuntimeFunctions = pInfo->m_pRuntimeFunctions;

    ULONG methodIndex = (ULONG)(pCodeInfo->GetFunctionEntry() - pRuntimeFunctions);

    const int lookupIndex = HotColdMappingLookupTable::LookupMappingForMethod(pInfo, methodIndex);

    if ((lookupIndex != -1) && ((lookupIndex & 1) == 1))
    {
        // This maps to a hot entry in the lookup table, so check its unwind info
        SIZE_T unwindSize;
        PTR_VOID pUnwindData = GetUnwindDataBlob(pCodeInfo->GetModuleBase(), pCodeInfo->GetFunctionEntry(), &unwindSize);
        _ASSERTE(pUnwindData != NULL);

#ifdef TARGET_AMD64
        // Chained unwind info is used only for cold part of the main code
        const UCHAR chainedUnwindFlag = (((PTR_UNWIND_INFO)pUnwindData)->Flags & UNW_FLAG_CHAININFO);
        return (chainedUnwindFlag == 0);
#else
        // TODO: We need a solution for arm64 here
        return false;
#endif
    }

    // Fall back to existing logic if it is not cold

    TADDR funcletStartAddress = GetFuncletStartAddress(pCodeInfo);
    TADDR methodStartAddress = pCodeInfo->GetStartAddress();

    return (funcletStartAddress != methodStartAddress);
}

BOOL ReadyToRunJitManager::IsFilterFunclet(EECodeInfo * pCodeInfo)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        MODE_ANY;
    }
    CONTRACTL_END;

    if (!pCodeInfo->IsFunclet())
        return FALSE;

#ifdef TARGET_X86
    // x86 doesn't use personality routines in unwind data, so we have to fallback to
    // the slow implementation
    return IJitManager::IsFilterFunclet(pCodeInfo);
#else
    // Get address of the personality routine for the function being queried.
    SIZE_T size;
    PTR_VOID pUnwindData = GetUnwindDataBlob(pCodeInfo->GetModuleBase(), pCodeInfo->GetFunctionEntry(), &size);
    _ASSERTE(pUnwindData != NULL);

    // Personality routine is always the last element of the unwind data
    DWORD rvaPersonalityRoutine = *(dac_cast<PTR_DWORD>(dac_cast<TADDR>(pUnwindData) + size) - 1);

    // Get the personality routine for the first function in the module, which is guaranteed to be not a funclet.
    ReadyToRunInfo * pInfo = JitTokenToReadyToRunInfo(pCodeInfo->GetMethodToken());
    if (pInfo->m_nRuntimeFunctions == 0)
        return FALSE;

    PTR_VOID pFirstUnwindData = GetUnwindDataBlob(pCodeInfo->GetModuleBase(), pInfo->m_pRuntimeFunctions, &size);
    _ASSERTE(pFirstUnwindData != NULL);
    DWORD rvaFirstPersonalityRoutine = *(dac_cast<PTR_DWORD>(dac_cast<TADDR>(pFirstUnwindData) + size) - 1);

    // Compare the two personality routines. If they are different, then the current function is a filter funclet.
    BOOL fRet = (rvaPersonalityRoutine != rvaFirstPersonalityRoutine);

    // Verify that the optimized implementation is in sync with the slow implementation
    _ASSERTE(fRet == IJitManager::IsFilterFunclet(pCodeInfo));

    return fRet;
#endif
}

#endif  // FEATURE_EH_FUNCLETS

void ReadyToRunJitManager::JitTokenToMethodRegionInfo(const METHODTOKEN& MethodToken,
                                                   MethodRegionInfo * methodRegionInfo)
{
    CONTRACTL {
        NOTHROW;
        GC_NOTRIGGER;
        SUPPORTS_DAC;
        PRECONDITION(methodRegionInfo != NULL);
    } CONTRACTL_END;

    methodRegionInfo->hotStartAddress  = JitTokenToStartAddress(MethodToken);
    methodRegionInfo->hotSize          = GetCodeManager()->GetFunctionSize(GetGCInfoToken(MethodToken));
    methodRegionInfo->coldStartAddress = 0;
    methodRegionInfo->coldSize         = 0;

    ReadyToRunInfo * pInfo = JitTokenToReadyToRunInfo(MethodToken);
    COUNT_T nRuntimeFunctions = pInfo->m_nRuntimeFunctions;
    PTR_RUNTIME_FUNCTION pRuntimeFunctions = pInfo->m_pRuntimeFunctions;

    PTR_RUNTIME_FUNCTION pRuntimeFunction = dac_cast<PTR_RUNTIME_FUNCTION>(MethodToken.m_pCodeHeader);

    ULONG methodIndex = (ULONG)(pRuntimeFunction - pRuntimeFunctions);

    const int lookupIndex = HotColdMappingLookupTable::LookupMappingForMethod(pInfo, methodIndex);

    // If true, this method has no cold code
    if (lookupIndex == -1)
    {
        return;
    }
#ifdef TARGET_X86
    _ASSERTE(!"hot cold splitting is not supported for x86");
#else
    _ASSERTE((lookupIndex & 1) == 0);
    ULONG coldMethodIndex = pInfo->m_pHotColdMap[lookupIndex];
    PTR_RUNTIME_FUNCTION pColdRuntimeFunction = pRuntimeFunctions + coldMethodIndex;
    methodRegionInfo->coldStartAddress = JitTokenToModuleBase(MethodToken)
        + RUNTIME_FUNCTION__BeginAddress(pColdRuntimeFunction);

    ULONG coldMethodIndexNext;
    if ((ULONG)(lookupIndex) == (pInfo->m_nHotColdMap - 2))
    {
        coldMethodIndexNext = nRuntimeFunctions - 1;
    }
    else
    {
        coldMethodIndexNext = pInfo->m_pHotColdMap[lookupIndex + 2] - 1;
    }

    PTR_RUNTIME_FUNCTION pLastRuntimeFunction = pRuntimeFunctions + coldMethodIndexNext;
    methodRegionInfo->coldSize = RUNTIME_FUNCTION__EndAddress(pLastRuntimeFunction, 0)
        - RUNTIME_FUNCTION__BeginAddress(pColdRuntimeFunction);
    methodRegionInfo->hotSize -= methodRegionInfo->coldSize;
#endif //TARGET_X86
}

#ifdef DACCESS_COMPILE

void ReadyToRunJitManager::EnumMemoryRegions(CLRDataEnumMemoryFlags flags)
{
    IJitManager::EnumMemoryRegions(flags);
}

#if defined(FEATURE_EH_FUNCLETS)

//
// EnumMemoryRegionsForMethodUnwindInfo - enumerate the memory necessary to read the unwind info for the
// specified method.
//
// Note that in theory, a dump generation library could save the unwind information itself without help
// from us, since it's stored in the image in the standard function table layout for Win64.  However,
// dump-generation libraries assume that the image will be available at debug time, and if the image
// isn't available then it is acceptable for stackwalking to break.  For ngen images (which are created
// on the client), it usually isn't possible to have the image available at debug time, and so for minidumps
// we must explicitly ensure the unwind information is saved into the dump.
//
// Arguments:
//     flags - EnumMem flags
//     pMD   - MethodDesc for the method in question
//
void ReadyToRunJitManager::EnumMemoryRegionsForMethodUnwindInfo(CLRDataEnumMemoryFlags flags, EECodeInfo * pCodeInfo)
{
    // Get the RUNTIME_FUNCTION entry for this method
    PTR_RUNTIME_FUNCTION pRtf = pCodeInfo->GetFunctionEntry();

    if (pRtf==NULL)
    {
        return;
    }

    // Enumerate the function entry and other entries needed to locate it in the program exceptions directory
    ReadyToRunInfo * pReadyToRunInfo = JitTokenToReadyToRunInfo(pCodeInfo->GetMethodToken());
    EnumRuntimeFunctionEntriesToFindEntry(pRtf, pReadyToRunInfo->GetImage());

    SIZE_T size;
    PTR_VOID pUnwindData = GetUnwindDataBlob(pCodeInfo->GetModuleBase(), pRtf, &size);
    if (pUnwindData != NULL)
        DacEnumMemoryRegion(PTR_TO_TADDR(pUnwindData), size);
}

#endif //FEATURE_EH_FUNCLETS
#endif // #ifdef DACCESS_COMPILE

#endif
