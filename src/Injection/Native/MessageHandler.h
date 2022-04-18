#pragma once
#include "MessageTypes.h"

extern "C"
__declspec(dllexport) LRESULT HandleHookedMessage(int code, WPARAM wParam, LPARAM lParam);