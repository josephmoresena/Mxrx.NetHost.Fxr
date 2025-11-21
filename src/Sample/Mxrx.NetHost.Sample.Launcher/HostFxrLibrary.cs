#if STATIC_LINK
using System.Runtime.InteropServices;

using Mxrx.NetHost;
using Mxrx.NetHost.Native;

using Rxmxnx.PInvoke;

namespace Mxrxm.NetHost.Sample.Launcher;

#pragma warning disable SYSLIB1054
internal class HostFxrLibrary : IFrameworkResolverLibrary.IPInvoke
{
	private const String libraryName = "hostfxr";

	[DllImport(HostFxrLibrary.libraryName, CallingConvention = CallingConvention.Cdecl,
	           EntryPoint = IFrameworkResolverLibrary.CloseHandleSymbol)]
	public static extern RuntimeCallResult CloseContext(HostHandle handle);
	[DllImport(HostFxrLibrary.libraryName, CallingConvention = CallingConvention.Cdecl,
	           EntryPoint = IFrameworkResolverLibrary.GetDelegateSymbol)]
	public static extern RuntimeCallResult GetFunctionPointer(HostHandle handle, RuntimeDelegateType delegateType,
		out IntPtr funcPtr);
	[DllImport(HostFxrLibrary.libraryName, CallingConvention = CallingConvention.Cdecl,
	           EntryPoint = IFrameworkResolverLibrary.GetRuntimePropertiesSymbol)]
	public static extern RuntimeCallResult GetRuntimeProperties(HostHandle handle, ref UIntPtr propCount,
		ref ReadOnlyValPtr<NativeCharPointer> propKeysPtr, ref ReadOnlyValPtr<NativeCharPointer> propValuesPtr);
	[DllImport(HostFxrLibrary.libraryName, CallingConvention = CallingConvention.Cdecl,
	           EntryPoint = IFrameworkResolverLibrary.GetRuntimePropertyValueSymbol)]
	public static extern RuntimeCallResult GetRuntimePropertyValue(HostHandle handle, NativeCharPointer keyPtr,
		out NativeCharPointer valuePtr);
	[DllImport(HostFxrLibrary.libraryName, CallingConvention = CallingConvention.Cdecl,
	           EntryPoint = IFrameworkResolverLibrary.SetRuntimePropertyValueSymbol)]
	public static extern RuntimeCallResult SetRuntimePropertyValue(HostHandle handle, NativeCharPointer keyPtr,
		NativeCharPointer valuePtr);
	[DllImport(HostFxrLibrary.libraryName, CallingConvention = CallingConvention.Cdecl,
	           EntryPoint = IFrameworkResolverLibrary.InitializeForCommandSymbol)]
	public static extern RuntimeCallResult InitializeForCommandLine(Int32 argsCount,
		ReadOnlyValPtr<NativeCharPointer> argsPtr, in InitParameters initParams, out HostHandle handle);
	[DllImport(HostFxrLibrary.libraryName, CallingConvention = CallingConvention.Cdecl,
	           EntryPoint = IFrameworkResolverLibrary.InitializeForConfigSymbol)]
	public static extern RuntimeCallResult InitializeForConfigFile(NativeCharPointer configPathPtr,
		in InitParameters initParams, out HostHandle handle);
	[DllImport(HostFxrLibrary.libraryName, CallingConvention = CallingConvention.Cdecl,
	           EntryPoint = IFrameworkResolverLibrary.RunAppSymbol)]
	public static extern Int32 RunApp(HostHandle handle);
	[DllImport(HostFxrLibrary.libraryName, CallingConvention = CallingConvention.Cdecl,
	           EntryPoint = IFrameworkResolverLibrary.SetErrorWriterSymbol)]
	public static extern IntPtr SetError(HostHandle handle, IntPtr writeErrPtr);
}
#pragma warning restore SYSLIB1054
#endif