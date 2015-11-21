//
//  main.c
//  getchar-count-lines
//
//  Created by Александр Ерыгин on 22.11.15.
//  Copyright © 2015 Alex Erygin. All rights reserved.
//

#include <stdio.h>

int main(int argc, const char * argv[]) {
    // insert code here...
    int c, lineCount;
    lineCount = 0;
    
    while((c = getchar()) != EOF){
        if(c == '\n'){
            ++lineCount;
        }
    }
    
    printf("Всего строк: %d\n", lineCount);
    return 0;
}
