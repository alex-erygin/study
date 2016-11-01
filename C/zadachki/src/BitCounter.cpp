//
// Created by alex on 01/11/16.
//

#include "BitCounter.h"

int BitCounter::CountBits(int number) {
    auto size = sizeof(int);
    int result = 0;
    int value = 1;
    for (int i = 0; i < size; i++) {
        if((value & number) == value){
            result ++;
        }
        value = value << 1;
    }

    return result;
}