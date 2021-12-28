#include <windows.h>
#include <stdio.h>

static HWND _child;
static HWND _parent;

BOOL CALLBACK EnumWindowsProc(_In_ HWND hwnd, _In_ LPARAM Lparam)
{
	//HWND hDefView = FindWindowEx(hwnd, 0, L"SHELLDLL_DefView", 0);
	//if (hDefView != 0) {
	//	// 找它的下一个窗口，类名为WorkerW，隐藏它
	//	HWND hWorkerw = FindWindowEx(0, hwnd, L"WorkerW", 0);
	//	ShowWindow(hWorkerw, SW_HIDE);

	//	return FALSE;
	//}
	//return TRUE;
	char szTitle[100];
	GetClassName(hwnd, szTitle, 100);
	printf(szTitle);
		printf("\n");
	if (strcmp(szTitle, "WorkerW") == 0)
	{
		EnumChildWindows(hwnd, EnumWindowsProc, 0);
	}
	else if (strcmp(szTitle, "SHELLDLL_DefView") == 0)
	{
		printf(szTitle);
		printf("\n");
		ShowWindow(hwnd, SW_SHOW);
		//HWND parentHwnd = GetParent(hwnd);
		//_parent = hwnd;
		//EnumChildWindows(hwnd, EnumWindowsProc, 0);
	}

	if (strcmp(szTitle, "UnityWndClass") == 0)
	{
		printf(szTitle);
		printf("\n");
		
		char szWindowText[100];
		GetWindowText(hwnd, szWindowText, 100);
		if (strcmp(szWindowText, "PSD2UGUI") == 0)
		{
			printf(szWindowText);
			printf("\n");
			_child = hwnd;
		}
	}


	//if (_parent!=0 && _child!=0)
	//{
	//	SetParent(_child, _parent);							// 将视频窗口设置为PM的子窗口
	//	_parent = 0;
	//	_child = 0;
	//}
}

int main(int argc, char* args[])
{
	_parent = 0;
	_child = 0;

	HWND hProgman = FindWindow("Progman", "Program Manager");				// 找到PM窗口
	//SendMessageTimeout(hProgman, 0x52C, 0, 0, 0, 100, 0);	// 给它发特殊消息
	//HWND hFfplay = FindWindow(L"SDL_app", 0);				// 找到视频窗口
	//SetParent(hFfplay, hProgman);							// 将视频窗口设置为PM的子窗口
	EnumWindows(EnumWindowsProc, 0);						// 找到第二个WorkerW窗口并隐藏它
	//EnumChildWindows(hProgman, EnumWindowsProc, 0);
	if ( _child!=0)
	{
		HWND oldParent = GetParent(_child);
		SetParent(_child, hProgman);							// 将视频窗口设置为PM的子窗口
		ShowWindow(0x0001077A, SW_HIDE);
		printf("Set bg\n");
	/*	_parent = 0;
		_child = 0;*/
		Sleep(10000);
		SetParent(_child, oldParent);
		ShowWindow(0x0001077A, SW_SHOW);
		printf("Set back");
	}

	while (1)
	{

	}
	//// 视频路径、1920和1080，要根据实际情况改。建议使用GetSystemMetrics函数获取分辨率属性
	//LPCWSTR lpParameter = L" F:\\桌面\\动态壁纸Demo\\视频.mp4  -noborder -x 1920 -y 1080  -loop 0";
	//STARTUPINFO si{ 0 };
	//PROCESS_INFORMATION pi{ 0 };

	//// 下面是我电脑上ffplay的路径，要根据实际情况改
	//if (CreateProcess(L"F:\\免安装程序\\ffmpeg-20200523\\bin\\ffplay.exe", (LPWSTR)lpParameter, 0, 0, 0, 0, 0, 0, &si, &pi))
	//{
	//	Sleep(200);												// 等待视频播放器启动完成。可用循环获取窗口尺寸来代替Sleep()

	//	HWND hProgman = FindWindow(L"Progman", 0);				// 找到PM窗口
	//	SendMessageTimeout(hProgman, 0x52C, 0, 0, 0, 100, 0);	// 给它发特殊消息
	//	HWND hFfplay = FindWindow(L"SDL_app", 0);				// 找到视频窗口
	//	SetParent(hFfplay, hProgman);							// 将视频窗口设置为PM的子窗口
	//	EnumWindows(EnumWindowsProc, 0);						// 找到第二个WorkerW窗口并隐藏它
	//}

	return 0;

}