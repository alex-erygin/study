//
//  main.c
//  Celsius to fahrenheit
//
//  Created by Александр Ерыгин on 15.11.15.
//  Copyright © 2015 Alex Erygin. All rights reserved.
//

#include <stdio.h>

#define max_cells 100

#define print_header printf("Celsius\t\tFahrenheit\n"); printf("-------\t\t----------\n");
int main(int argc, const char * argv[]) {
    print_header
    
    for (int cels = 0; cels < max_cells; cels += 3) {
        float fahr = cels*9/5+32;
        printf("%d\t\t\t%.0f\n", cels, fahr);
    }
    // insert code here...
    return 0;
}
