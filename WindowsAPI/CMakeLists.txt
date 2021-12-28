cmake_minimum_required(VERSION 3.8)

project(lib_wallpaper LANGUAGES C)


# add_library(${PROJECT_NAME} window_api.h window_api.c main.c)

add_executable(${PROJECT_NAME} window_api.h window_api.c main.c)