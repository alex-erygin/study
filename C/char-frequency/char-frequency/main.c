//
//  main.c
//  char-frequency
//
//  Created by Александр Ерыгин on 22.11.15.
//  Copyright © 2015 Alex Erygin. All rights reserved.
//

#include <stdio.h>

int main(int argc, const char * argv[]) {
    int c;
    int characters[256];
    
    for (int i = 0; i<255; ++i) {
        characters[i] = 0;
    }
    
    while ((c = getchar()) != EOF) {
        characters[c]++;
    }
    
    printf("chars =");
    for (int i = 0; i < 255; ++i) {
        printf("'%c':", i);
        
        for (int k=0; k<characters[i]; k++) {
            printf("#");
        }
        printf("\n");
    }
    
    return 0;
}