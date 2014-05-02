#include <Windows.h>
#include "MSCorEE.h"
#include <metahost.h>
//#include "main.h"
#pragma unmanaged

void __cdecl StartDotNet(void);

BOOL APIENTRY DllMain(HANDLE hModule, DWORD ul_reason_for_call, LPVOID lpReeserved)
{
	switch (ul_reason_for_call)
	{
		case DLL_PROCESS_ATTACH:
			CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)&StartDotNet, 0, 0, NULL);
			break;
		case DLL_THREAD_ATTACH:
		case DLL_THREAD_DETACH:
		case DLL_PROCESS_DETACH:
			break;
	}
	return TRUE;
}

void StartDotNet()
{
	HRESULT hr;
	ICLRRuntimeHost *pClrHost = NULL;
	ICLRMetaHost *pMetaHost = NULL;
	hr = CLRCreateInstance(CLSID_CLRMetaHost, IID_ICLRMetaHost, (LPVOID*)&pMetaHost);
	//MessageBox(NULL, L"CLRCreateInstance Done.", NULL, NULL);

	ICLRRuntimeInfo * lpRuntimeInfo = NULL;

	hr = pMetaHost->GetRuntime(L"v4.0.30319", IID_ICLRRuntimeInfo, (LPVOID*)&lpRuntimeInfo); 
	//MessageBox(NULL, L"pMetaHost->GetRuntime Done.", NULL, NULL);

	ICLRRuntimeHost * lpRuntimeHost = NULL;
    
    hr = lpRuntimeInfo->GetInterface(CLSID_CLRRuntimeHost, IID_ICLRRuntimeHost, (LPVOID *)&lpRuntimeHost);
    //MessageBox(NULL, L"lpRuntimeInfo->GetInterface Done.", NULL, NULL);

    hr = lpRuntimeHost->Start();
    //MessageBox(NULL, L"lpRuntimeHost->Start() Done.", NULL, NULL);

    DWORD dwRet = 0;

    hr = lpRuntimeHost->ExecuteInDefaultAppDomain(L"C:\\ATProject.dll", L"ATProject.Program", L"DllMain", L"Injection Worked", &dwRet);

    lpRuntimeHost->Release();

}