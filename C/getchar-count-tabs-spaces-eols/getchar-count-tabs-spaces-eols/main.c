//
//  main.c
//  getchar-count-tabs-spaces-eols
//
//  Created by Александр Ерыгин on 22.11.15.
//  Copyright © 2015 Alex Erygin. All rights reserved.
//

#include <stdio.h>

int main(int argc, const char * argv[]) {
    // insert code here...
    char c;
    int spaceCount, tabCount, eolCount;
    
    spaceCount = 0;
    tabCount = 0;
    eolCount = 0;
    
    while((c = getchar()) != EOF){
        if(c == '\n'){
            ++eolCount;
        }
        if(c == ' '){
            ++spaceCount;
        }
        if(c == '\t'){
            ++tabCount;
        }
    }
    
    printf("Всего пробелов: %d\nВсего табов: %d\nВсего строк:%d\n", spaceCount, tabCount, eolCount);
    return 0;
}