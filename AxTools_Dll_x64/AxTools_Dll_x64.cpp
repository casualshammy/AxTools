// AxTools_Dll_x64.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"


extern "C" __declspec(dllexport) char* __cdecl GetMailUsername();

extern "C" __declspec(dllexport) char* __cdecl GetMailUsername()
{
	char* b = new char[5];
	b[0] = 'a';
	b[1] = 'x';
	b[2] = 'i';
	b[3] = 'o';
	b[4] = '\0';
	return b;
}