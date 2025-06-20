// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Runtime.Serialization;
using ResourceHashesByNameDictionary = System.Collections.Generic.Dictionary<string, string>;

namespace Microsoft.NET.Sdk.WebAssembly;

#nullable disable

/// <summary>
/// Defines the structure of a Blazor boot JSON file
/// </summary>
public class BootJsonData
{
    /// <summary>
    /// Gets the name of the assembly with the application entry point
    /// </summary>
    /// <remarks>
    /// Deprecated in .NET 8. Use <see cref="mainAssemblyName"/>
    /// </remarks>
    public string entryAssembly { get; set; }

    public string mainAssemblyName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string applicationEnvironment { get; set; }

    /// <summary>
    /// For .NET < 10, this contains <see cref="ResourcesData"/>.
    /// For .NET >= 10, this contains <see cref="AssetsData"/>.
    /// ---
    /// Gets the set of resources needed to boot the application. This includes the transitive
    /// closure of .NET assemblies (including the entrypoint assembly), the dotnet.wasm file,
    /// and any PDBs to be loaded.
    ///
    /// Within <see cref="ResourceHashesByNameDictionary"/>, dictionary keys are resource names,
    /// and values are SHA-256 hashes formatted in prefixed base-64 style (e.g., 'sha256-abcdefg...')
    /// as used for subresource integrity checking.
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    public object resources { get; set; } = new ResourcesData();

    /// <summary>
    /// Gets a value that determines whether to enable caching of the <see cref="resources"/>
    /// inside a CacheStorage instance within the browser.
    /// </summary>
    public bool? cacheBootResources { get; set; }

    /// <summary>
    /// Gets a value that determines if this is a debug build.
    /// </summary>
    public bool? debugBuild { get; set; }

    /// <summary>
    /// Gets a value that determines what level of debugging is configured.
    /// </summary>
    public int debugLevel { get; set; }

    /// <summary>
    /// Gets a value that determines if the linker is enabled.
    /// </summary>
    public bool? linkerEnabled { get; set; }

    /// <summary>
    /// Config files for the application
    /// </summary>
    /// <remarks>
    /// Deprecated in .NET 8, use <see cref="appsettings"/>
    /// </remarks>
    public List<string> config { get; set; }

    /// <summary>
    /// Config files for the application
    /// </summary>
    public List<string> appsettings { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="ICUDataMode"/> that determines how icu files are loaded.
    /// </summary>
    /// <remarks>
    /// Deprecated since .NET 8. Use <see cref="globalizationMode"/> instead.
    /// </remarks>
    public GlobalizationMode? icuDataMode { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="GlobalizationMode"/> that determines how icu files are loaded.
    /// </summary>
    public string globalizationMode { get; set; }

    /// <summary>
    /// Gets a value for mono runtime options.
    /// </summary>
    public string[] runtimeOptions { get; set; }

    /// <summary>
    /// Gets or sets configuration extensions.
    /// </summary>
    public Dictionary<string, Dictionary<string, object>> extensions { get; set; }

    /// <summary>
    /// Gets or sets environment variables.
    /// </summary>
    public System.Collections.Generic.Dictionary<string, string> environmentVariables { get; set; }
    /// <summary>
    /// Subset of runtimeconfig.json
    /// </summary>
    public RuntimeConfigData runtimeConfig { get; set; }

    /// <summary>
    /// Gets or sets diagnostic tracing.
    /// </summary>
    public object diagnosticTracing { get; set; }

    /// <summary>
    /// Gets or sets pthread pool initial size.
    /// </summary>
    public int? pthreadPoolInitialSize { get; set; }

    /// <summary>
    /// Gets or sets pthread pool unused size.
    /// </summary>
    public int? pthreadPoolUnusedSize { get; set; }
}

/// <summary>
/// Subset of runtimeconfig.json
/// </summary>
public class RuntimeConfigData
{
    /// <summary>
    /// Runtime options
    /// </summary>
    public RuntimeOptionsData runtimeOptions { get; set; }
}

public class RuntimeOptionsData
{
    /// <summary>
    /// Config properties for the runtime
    /// </summary>
    public Dictionary<string, object> configProperties { get; set; }
}

public class ResourcesData
{
    /// <summary>
    /// Gets a hash of all resources
    /// </summary>
    public string hash { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, string> fingerprinting { get; set; }

    /// <summary>
    /// .NET Wasm runtime resources (dotnet.wasm, dotnet.js) etc.
    /// </summary>
    /// <remarks>
    /// Deprecated in .NET 8, use <see cref="jsModuleWorker"/>, <see cref="jsModuleNative"/>, <see cref="jsModuleRuntime"/>, <see cref="wasmNative"/>, <see cref="wasmSymbols"/>, <see cref="icu"/>.
    /// </remarks>
    [DataMember(EmitDefaultValue = false)]
    public ResourceHashesByNameDictionary runtime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ResourceHashesByNameDictionary jsModuleWorker { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ResourceHashesByNameDictionary jsModuleDiagnostics { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ResourceHashesByNameDictionary jsModuleNative { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ResourceHashesByNameDictionary jsModuleRuntime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ResourceHashesByNameDictionary wasmNative { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ResourceHashesByNameDictionary wasmSymbols { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ResourceHashesByNameDictionary icu { get; set; }

    /// <summary>
    /// "assembly" (.dll) resources needed to start MonoVM
    /// </summary>
    public ResourceHashesByNameDictionary coreAssembly { get; set; } = new ResourceHashesByNameDictionary();

    /// <summary>
    /// "assembly" (.dll) resources
    /// </summary>
    public ResourceHashesByNameDictionary assembly { get; set; } = new ResourceHashesByNameDictionary();

    /// <summary>
    /// "debug" (.pdb) resources needed to start MonoVM
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    public ResourceHashesByNameDictionary corePdb { get; set; }

    /// <summary>
    /// "debug" (.pdb) resources
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    public ResourceHashesByNameDictionary pdb { get; set; }

    /// <summary>
    /// localization (.satellite resx) resources
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, ResourceHashesByNameDictionary> satelliteResources { get; set; }

    /// <summary>
    /// Assembly (.dll) resources that are loaded lazily during runtime
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    public ResourceHashesByNameDictionary lazyAssembly { get; set; }

    /// <summary>
    /// JavaScript module initializers that Blazor will be in charge of loading.
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    public ResourceHashesByNameDictionary libraryInitializers { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ResourceHashesByNameDictionary modulesAfterConfigLoaded { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ResourceHashesByNameDictionary modulesAfterRuntimeReady { get; set; }

    /// <summary>
    /// Extensions created by users customizing the initialization process. The format of the file(s)
    /// is up to the user.
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, ResourceHashesByNameDictionary> extensions { get; set; }

    /// <summary>
    /// Additional assets that the runtime consumes as part of the boot process.
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, AdditionalAsset> runtimeAssets { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, ResourceHashesByNameDictionary> coreVfs { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, ResourceHashesByNameDictionary> vfs { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<string> remoteSources { get; set; }
}

public class AssetsData
{
    /// <summary>
    /// Gets a hash of all resources
    /// </summary>
    public string hash { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<JsAsset> jsModuleWorker { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<JsAsset> jsModuleDiagnostics { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<JsAsset> jsModuleNative { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<JsAsset> jsModuleRuntime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<WasmAsset> wasmNative { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<SymbolsAsset> wasmSymbols { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<GeneralAsset> icu { get; set; }

    /// <summary>
    /// "assembly" (.dll) resources needed to start MonoVM
    /// </summary>
    public List<GeneralAsset> coreAssembly { get; set; } = new();

    /// <summary>
    /// "assembly" (.dll) resources
    /// </summary>
    public List<GeneralAsset> assembly { get; set; } = new();

    /// <summary>
    /// "debug" (.pdb) resources needed to start MonoVM
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    public List<GeneralAsset> corePdb { get; set; }

    /// <summary>
    /// "debug" (.pdb) resources
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    public List<GeneralAsset> pdb { get; set; }

    /// <summary>
    /// localization (.satellite resx) resources
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, List<GeneralAsset>> satelliteResources { get; set; }

    /// <summary>
    /// Assembly (.dll) resources that are loaded lazily during runtime
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    public List<GeneralAsset> lazyAssembly { get; set; }

    /// <summary>
    /// JavaScript module initializers that Blazor will be in charge of loading.
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    public List<JsAsset> libraryInitializers { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<JsAsset> modulesAfterConfigLoaded { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<JsAsset> modulesAfterRuntimeReady { get; set; }

    /// <summary>
    /// Extensions created by users customizing the initialization process. The format of the file(s)
    /// is up to the user.
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, ResourceHashesByNameDictionary> extensions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<VfsAsset> coreVfs { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<VfsAsset> vfs { get; set; }
}

[DataContract]
public class JsAsset
{
    public string name { get; set; }
    public string moduleExports { get; set; }
}

[DataContract]
public class SymbolsAsset
{
    public string name { get; set; }
}

[DataContract]
public class WasmAsset
{
    public string name { get; set; }
    public string integrity { get; set; }
    public string resolvedUrl { get; set; }
}

[DataContract]
public class GeneralAsset
{
    public string virtualPath { get; set; }
    public string name { get; set; }
    public string integrity { get; set; }
    public string resolvedUrl { get; set; }
}

[DataContract]
public class VfsAsset
{
    public string virtualPath { get; set; }
    public string name { get; set; }
    public string integrity { get; set; }
    public string resolvedUrl { get; set; }
}

public enum GlobalizationMode : int
{
    // Note that the numeric values are serialized and used in JS code, so don't change them without also updating the JS code
    // Note that names are serialized as string and used in JS code

    /// <summary>
    /// Load optimized icu data file based on the user's locale
    /// </summary>
    Sharded = 0,

    /// <summary>
    /// Use the combined icudt.dat file
    /// </summary>
    All = 1,

    /// <summary>
    /// Do not load any icu data files.
    /// </summary>
    Invariant = 2,

    /// <summary>
    /// Load custom icu file provided by the developer.
    /// </summary>
    Custom = 3,
}

[DataContract]
public class AdditionalAsset
{
    [DataMember(Name = "hash")]
    public string hash { get; set; }

    [DataMember(Name = "behavior")]
    public string behavior { get; set; }
}
