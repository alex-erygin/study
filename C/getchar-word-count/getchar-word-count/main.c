//
//  main.c
//  getchar-word-count
//
//  Created by Александр Ерыгин on 22.11.15.
//  Copyright © 2015 Alex Erygin. All rights reserved.
//

#include <stdio.h>

#define IN 1
#define OUT 0


int main(int argc, const char * argv[]) {
    int c, nl, nw, nc, state;
    
    state = OUT;
    nl = nw = nc = 0;
    
    while((c = getchar()) != EOF) {
        putchar(c);
        ++nc;
        if(c == '\n'){
            ++nl;
        }
        if(c == ' ' || c == '\n' || c == '\t'){
            state = OUT;
            putchar('\n');
        }
        else if (state == OUT){
            state = IN;
            ++nw;
        }
    }
    
    // insert code here...
    printf("строк: %d слов:%d символов:%d!\n", nl, nw, nc);
    return 0;
}
