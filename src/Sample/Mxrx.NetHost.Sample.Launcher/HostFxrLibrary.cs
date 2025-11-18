#if STATIC_LINK
using System.Runtime.InteropServices;

using Mxrx.NetHost;

using Rxmxnx.PInvoke;

namespace Mxrxm.NetHost.Sample.Launcher;

#pragma warning disable SYSLIB1054
internal class HostFxrLibrary : IFrameworkResolverLibrary
{
	private const String libraryName = "hostfxr";

	[DllImport(HostFxrLibrary.libraryName, CallingConvention = CallingConvention.Cdecl,
	           EntryPoint = IFrameworkResolverLibrary.CloseHandleSymbol)]
	public static extern RuntimeCallResult CloseContext(IntPtr handle);
	[DllImport(HostFxrLibrary.libraryName, CallingConvention = CallingConvention.Cdecl,
	           EntryPoint = IFrameworkResolverLibrary.GetDelegateSymbol)]
	public static extern RuntimeCallResult GetFunctionPointer(IntPtr handle, RuntimeDelegateType delegateType,
		out IntPtr funcPtr);
	[DllImport(HostFxrLibrary.libraryName, CallingConvention = CallingConvention.Cdecl,
	           EntryPoint = IFrameworkResolverLibrary.GetRuntimePropertiesSymbol)]
	public static extern RuntimeCallResult GetRuntimeProperties(IntPtr handle, ref UIntPtr count,
		ValPtr<IntPtr> keysPtr, ValPtr<IntPtr> valuesPtr);
	[DllImport(HostFxrLibrary.libraryName, CallingConvention = CallingConvention.Cdecl,
	           EntryPoint = IFrameworkResolverLibrary.GetRuntimePropertyValueSymbol)]
	public static extern RuntimeCallResult GetRuntimePropertyValue(IntPtr handle, IntPtr keyPtr, out IntPtr valuePtr);
	[DllImport(HostFxrLibrary.libraryName, CallingConvention = CallingConvention.Cdecl,
	           EntryPoint = IFrameworkResolverLibrary.SetRuntimePropertyValueSymbol)]
	public static extern RuntimeCallResult SetRuntimePropertyValue(IntPtr handle, IntPtr keyPtr, IntPtr valuePtr);
	[DllImport(HostFxrLibrary.libraryName, CallingConvention = CallingConvention.Cdecl,
	           EntryPoint = IFrameworkResolverLibrary.InitializeForCommandSymbol)]
	public static extern RuntimeCallResult Initialize(Int32 argCount, IntPtr argPtr, IntPtr paramsPtr,
		out IntPtr handle);
	[DllImport(HostFxrLibrary.libraryName, CallingConvention = CallingConvention.Cdecl,
	           EntryPoint = IFrameworkResolverLibrary.InitializeForConfigSymbol)]
	public static extern RuntimeCallResult Initialize(IntPtr configPathPtr, IntPtr paramsPtr, out IntPtr handle);
	[DllImport(HostFxrLibrary.libraryName, CallingConvention = CallingConvention.Cdecl,
	           EntryPoint = IFrameworkResolverLibrary.RunAppSymbol)]
	public static extern Int32 RunApp(IntPtr handle);
	[DllImport(HostFxrLibrary.libraryName, CallingConvention = CallingConvention.Cdecl,
	           EntryPoint = IFrameworkResolverLibrary.SetErrorWriterSymbol)]
	public static extern void SetError(IntPtr handle, IntPtr writeErrPtr);
}
#pragma warning restore SYSLIB1054
#endif