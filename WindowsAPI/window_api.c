#include <windows.h>
#include <stdio.h>

#include "window_api.h"

HWND FindProgmanWindow()
{
	return FindWindow("Progman", "Program Manager");
}