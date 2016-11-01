cmake_minimum_required(VERSION 3.6)
project(zadachki)

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11")
add_subdirectory(lib/googletest)
include_directories(lib/googletest/googletest/include)
set(SOURCE_FILES main.cpp src/BitCounter.cpp src/BitCounter.h)
add_executable(zadachki ${SOURCE_FILES})
target_link_libraries(zadachki gtest gtest_main)