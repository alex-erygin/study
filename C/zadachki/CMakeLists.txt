cmake_minimum_required(VERSION 3.6)
project(zadachki)

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11")

set(SOURCE_FILES main.cpp BitCounter.cpp BitCounter.h)
add_executable(zadachki ${SOURCE_FILES})