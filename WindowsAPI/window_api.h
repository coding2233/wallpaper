#include <stdio.h>
#include <stdint.h>
#if defined _WIN32 || defined __CYGWIN__
#ifdef KITNET_NO_EXPORT
#define API
#else
#define API __declspec(dllexport)
#endif
#else
#ifdef __GNUC__
#define API  __attribute__((__visibility__("default")))
#else
#define API
#endif
#endif

#if defined __cplusplus
#define EXTERN extern "C"
#else
#include <stdarg.h>
#include <stdbool.h>
#define EXTERN extern
#endif

#define WALLPAPER_API EXTERN API

WALLPAPER_API HWND FindProgmanWindow();