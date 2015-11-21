//
//  main.c
//  getchar-putchar
//
//  Created by Александр Ерыгин on 22.11.15.
//  Copyright © 2015 Alex Erygin. All rights reserved.
//

#include <stdio.h>

int main(int argc, const char * argv[]) {
    char c;
    
    while((c = getchar()) != EOF){
        if(c == '\t'){
            printf("\\t");
        }
        else if(c == '\b'){
            printf("\\b");
        }
        else if(c == '/'){
            printf("\\");
        }
        else{
            putchar(c);
        }
    }
    return 0;
}