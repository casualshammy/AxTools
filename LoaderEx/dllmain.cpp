#include "stdafx.h"
#include <Windows.h>
#include "MSCorEE.h"
#include <metahost.h>
#include <string.h>

void __cdecl StartPoint(void);
ICLRRuntimeHost* __cdecl GetRuntimeHost(LPCWSTR dotNetVersion);
int __cdecl ExecuteClrCode(ICLRRuntimeHost* host, LPCWSTR assemblyPath, LPCWSTR typeName, LPCWSTR function, LPCWSTR param);
TCHAR myPath[MAX_PATH+1];

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		GetModuleFileName(hModule, myPath, _countof(myPath));
		CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)&StartPoint, 0, 0, NULL);
			break;
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

void StartPoint()
{
	ICLRRuntimeHost* host = GetRuntimeHost(L"v4.0.30319");
	size_t slen = wcslen(myPath);
	for (int n = slen - 1; n >= slen - 11; --n)
	{
		myPath[n] = '\0';
	}
	slen = wcslen(myPath);
	myPath[slen] = '\\';
	myPath[slen + 1] = 'A';
	myPath[slen + 2] = 'T';
	myPath[slen + 3] = '.';
	myPath[slen + 4] = 'd';
	myPath[slen + 5] = 'l';
	myPath[slen + 6] = 'l';
	ExecuteClrCode(host , myPath, L"ATProject.Program", L"DllMain", myPath);
	// At some point you will need to call Release(). You can do it now or during cleanup code
	//host->Release();
}

/// <summary>
/// Returns a pointer to a running CLR host of the specified version
/// </summary>
/// <param name="dotNetVersion">The exact version number of the CLR you want to
/// run. This can be obtained by looking in the C:\Windows\Microsoft.NET\Framework
/// directory and copy the name of the last directory.</param>
/// <returns>A running CLR host or NULL. You are responsible for calling Release() on it.</returns>
ICLRRuntimeHost* GetRuntimeHost(LPCWSTR dotNetVersion)
{
	ICLRMetaHost* metaHost = NULL;
	ICLRRuntimeInfo* info = NULL;
	ICLRRuntimeHost* runtimeHost = NULL;

	// Get the CLRMetaHost that tells us about .NET on this machine
	if (S_OK == CLRCreateInstance(CLSID_CLRMetaHost, IID_ICLRMetaHost, (LPVOID*)&metaHost))
	{
		// Get the runtime information for the particular version of .NET
		if (S_OK == metaHost->GetRuntime(dotNetVersion, IID_ICLRRuntimeInfo, (LPVOID*)&info))
		{
			// Get the actual host
			if (S_OK == info->GetInterface(CLSID_CLRRuntimeHost, IID_ICLRRuntimeHost, (LPVOID*)&runtimeHost))
			{
				// Start it. This is okay to call even if the CLR is already running
				runtimeHost->Start();
			}
		}
	}
	if (NULL != info)
		info->Release();
	if (NULL != metaHost)
		metaHost->Release();

	return runtimeHost;
}

/// <summary>
/// Executes some code in the CLR. The function must have the signature: public static int Function(string param)
/// </summary>
/// <param name="host">A started instance of the CLR</param>
/// <param name="assemblyPath">The full path to your compiled code file.
/// i.e. "C:\MyProject\MyCode.dll"</param>
/// <param name="typeName">The full type name of the class to be called, including the
/// namespace. i.e. "MyCode.MyClass"</param>
/// <param name="function">The name of the function to be called. i.e. "MyFunction"</param>
/// <param name="param">A string parameter to pass to the function.</param>
/// <returns>The integer return code from the function or -1 if the function did not run</returns>
int ExecuteClrCode(ICLRRuntimeHost* host, LPCWSTR assemblyPath, LPCWSTR typeName, LPCWSTR function, LPCWSTR param)
{
	if (NULL == host)
		return -1;

	DWORD result = -1;
	if (S_OK != host->ExecuteInDefaultAppDomain(assemblyPath, typeName, function, param, &result))
		return -1;

	return result;
}

