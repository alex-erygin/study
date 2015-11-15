//
//  main.c
//  Celsius to fahrenheit
//
//  Created by Александр Ерыгин on 15.11.15.
//  Copyright © 2015 Alex Erygin. All rights reserved.
//

#include <stdio.h>

int main(int argc, const char * argv[]) {
    
    printf("Celsius\t\tFahrenheit\n");
    printf("-------\t\t----------\n");
    for (int cels = 0; cels < 100; cels += 3) {
        float fahr = cels*9/5+32;
        printf("%d\t\t\t%.0f\n", cels, fahr);
    }
    // insert code here...
    return 0;
}
