;; Licensed to the .NET Foundation under one or more agreements.
;; The .NET Foundation licenses this file to you under the MIT license.

include AsmMacros.inc

;; extern "C" DWORD __stdcall SseAndAvxStateSupport();
LEAF_ENTRY SseAndAvxStateSupport, _TEXT
        mov     ecx, 0                  ; Specify xcr0
        xgetbv                          ; result in EDX:EAX
        and eax, 06H
        cmp eax, 06H                    ; check OS has enabled SSE and AVX state support
        jne     not_supported
        mov     eax, 1
        jmp     done
    not_supported:
        mov     eax, 0
    done:
        ret
LEAF_END SseAndAvxStateSupport, _TEXT

        end

;; extern "C" DWORD __stdcall Avx512StateSupport();
LEAF_ENTRY Avx512StateSupport, _TEXT
        mov     ecx, 0                  ; Specify xcr0
        xgetbv                          ; result in EDX:EAX
        and eax, E0H
        cmp eax, E0H                    ; check OS has enabled AVX-512 state support
        jne     not_supported
        mov     eax, 1
        jmp     done
    not_supported:
        mov     eax, 0
    done:
        ret
LEAF_END Avx512StateSupport, _TEXT

        end
