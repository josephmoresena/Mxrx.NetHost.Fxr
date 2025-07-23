namespace Mxrx.NetHost;

/// <summary>
/// CLR function result.
/// </summary>
public enum RuntimeCallResult : UInt32
{
	/// <summary>
	/// The operation completed successfully.
	/// </summary>
	Success = 0x0,
	/// <summary>
	/// The host has already been initialized and cannot be initialized again.
	/// </summary>
	HostAlreadyInitialized = 0x1,
	/// <summary>
	/// The runtime properties provided differ from those already initialized.
	/// </summary>
	DifferentRuntimeProperties = 0x2,
	/// <summary>
	/// One or more invalid arguments were provided.
	/// </summary>
	InvalidArguments = 0x80008081,
	/// <summary>
	/// Failed to load the host library (hostfxr or hostpolicy).
	/// </summary>
	HostLibraryLoadFailure = 0x80008082,
	/// <summary>
	/// The host library required for initialization is missing.
	/// </summary>
	HostLibraryMissingFailure = 0x80008083,
	/// <summary>
	/// Failed to find or load the required host entry point.
	/// </summary>
	HostEntryPointFailure = 0x80008084,
	/// <summary>
	/// Failed to find the currently executing host.
	/// </summary>
	CurrentHostFindFailure = 0x80008085,
	/// <summary>
	/// Failed to resolve the CoreCLR runtime.
	/// </summary>
	CoreClrResolveFailure = 0x80008087,
	/// <summary>
	/// Failed to bind to the CoreCLR runtime.
	/// </summary>
	CoreClrBindFailure = 0x80008088,
	/// <summary>
	/// Failed to initialize the CoreCLR runtime.
	/// </summary>
	CoreClrInitFailure = 0x80008089,
	/// <summary>
	/// Failed to execute the CoreCLR runtime.
	/// </summary>
	CoreClrExeFailure = 0x8000808a,
	/// <summary>
	/// Failed to initialize the application resolver.
	/// </summary>
	ResolverInitFailure = 0x8000808b,
	/// <summary>
	/// Failed to resolve the application using the resolver.
	/// </summary>
	ResolverResolveFailure = 0x8000808c,
	/// <summary>
	/// Failed to initialize the host during startup.
	/// </summary>
	HostInitFailure = 0x8000808e,
	/// <summary>
	/// Failed to locate the installed .NET SDK.
	/// </summary>
	HostSdkFindFailure = 0x80008091,
	/// <summary>
	/// Invalid arguments were provided to the host API.
	/// </summary>
	HostInvalidArgs = 0x80008092,
	/// <summary>
	/// The runtime configuration file is invalid.
	/// </summary>
	InvalidConfigFile = 0x80008093,
	/// <summary>
	/// The specified application is not runnable.
	/// </summary>
	AppArgNotRunnable = 0x80008094,
	/// <summary>
	/// The application host executable is not bound to a runtime.
	/// </summary>
	AppHostExeNotBoundFailure = 0x80008095,
	/// <summary>
	/// A required framework is missing.
	/// </summary>
	FrameworkMissingFailure = 0x80008096,
	/// <summary>
	/// The requested host API call failed.
	/// </summary>
	HostApiFailed = 0x80008097,
	/// <summary>
	/// The buffer provided to the host API was too small.
	/// </summary>
	HostApiBufferTooSmall = 0x80008098,
	/// <summary>
	/// Failed to locate the application path.
	/// </summary>
	AppPathFindFailure = 0x8000809a,
	/// <summary>
	/// Failed to resolve the .NET SDK.
	/// </summary>
	SdkResolveFailure = 0x8000809b,
	/// <summary>
	/// The targeted framework is incompatible.
	/// </summary>
	FrameworkCompatFailure = 0x8000809c,
	/// <summary>
	/// A framework compatibility issue occurred, but a retry is possible.
	/// </summary>
	FrameworkCompatRetry = 0x8000809d,
	/// <summary>
	/// Failed to extract bundled files from the application.
	/// </summary>
	BundleExtractionFailure = 0x8000809f,
	/// <summary>
	/// An I/O error occurred during bundle extraction.
	/// </summary>
	BundleExtractionIoError = 0x800080a0,
	/// <summary>
	/// A duplicate runtime property was specified.
	/// </summary>
	HostDuplicateProperty = 0x800080a1,
	/// <summary>
	/// The requested host API version is not supported.
	/// </summary>
	HostApiUnsupportedVersion = 0x800080a2,
	/// <summary>
	/// The host is in an invalid state for the requested operation.
	/// </summary>
	HostInvalidState = 0x800080a3,
	/// <summary>
	/// The requested runtime property was not found.
	/// </summary>
	HostPropertyNotFound = 0x800080a4,
	/// <summary>
	/// The configuration provided to the host is incompatible.
	/// </summary>
	HostIncompatibleConfig = 0x800080a5,
	/// <summary>
	/// The requested host API scenario is not supported.
	/// </summary>
	HostApiUnsupportedScenario = 0x800080a6,
	/// <summary>
	/// A required host feature is disabled.
	/// </summary>
	HostFeatureDisabled = 0x800080a7,
	/// <summary>
	/// Missing method exception. A method was not found.
	/// </summary>
	MissingMethodException = 0x80131512,
	/// <summary>
	/// Missing field exception. A field was not found.
	/// </summary>
	MissingFieldException = 0x80131511,
	/// <summary>
	/// Member access exception. Invalid access to a class member.
	/// </summary>
	MemberAccessException = 0x8013151A,
	/// <summary>
	/// File not found exception. A file could not be found.
	/// </summary>
	FileNotFoundException = 0x80070002,
	/// <summary>
	/// File load exception. A file could not be loaded.
	/// </summary>
	FileLoadException = 0x80131621,
	/// <summary>
	/// Bad image format exception. A file was in an invalid format.
	/// </summary>
	BadImageFormatException = 0x8007000B,
	/// <summary>
	/// Type load exception. A type could not be loaded.
	/// </summary>
	TypeLoadException = 0x80131522,
	/// <summary>
	/// Invalid cast exception. An invalid cast occurred.
	/// </summary>
	InvalidCastException = 0x80004002,
	/// <summary>
	/// Not implemented exception. The method or operation is not implemented.
	/// </summary>
	NotImplementedException = 0x80004001,
	/// <summary>
	/// Argument exception. An invalid argument was supplied.
	/// </summary>
	ArgumentException = 0x80070057,
	/// <summary>
	/// Argument null exception. A null argument was supplied where not allowed.
	/// </summary>
	ArgumentNullException = 0x80004003,
	/// <summary>
	/// Invalid operation exception. Operation is not valid in the current state.
	/// </summary>
	InvalidOperationException = 0x80131509,
	/// <summary>
	/// Object disposed exception. An operation was performed on a disposed object.
	/// </summary>
	ObjectDisposedException = 0x80131622,
	/// <summary>
	/// Unauthorized access exception. Access is denied.
	/// </summary>
	UnauthorizedAccessException = 0x80070005,
	/// <summary>
	/// Format exception. Invalid format was detected.
	/// </summary>
	FormatException = 0x80131537,
	/// <summary>
	/// Index out of range exception. Index was outside the bounds of the array.
	/// </summary>
	IndexOutOfRangeException = 0x80131508,
	/// <summary>
	/// Out of memory exception. Not enough memory is available.
	/// </summary>
	OutOfMemoryException = 0x8007000E,
	/// <summary>
	/// Overflow exception. An arithmetic overflow occurred.
	/// </summary>
	OverflowException = 0x80131516,
	/// <summary>
	/// Stack overflow exception. The evaluation stack overflowed.
	/// </summary>
	StackOverflowException = 0x800703E9,
	/// <summary>
	/// Unknown error occurred.
	/// </summary>
	Unknown = 0x80131500,
}