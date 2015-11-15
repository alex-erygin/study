//
//  main.c
//  FahrenheitToCelsius
//
//  Created by Александр Ерыгин on 15.11.15.
//  Copyright © 2015 Alex Erygin. All rights reserved.
//

#include <stdio.h>

int main(int argc, const char * argv[]) {
    float fahr, celsius;
    int lower, upper, step;
    
    lower = 0;
    upper = 300;
    step = 20;
    
    fahr = lower;
    
    printf("Fahr\tCelsium\n");
    printf("----\t-------\n");
    while(fahr <= upper){
        celsius = 5 * (fahr-32) / 9;
        printf("%.0f\t\t%3.2f\n", fahr, celsius);
        fahr = fahr + step;
    }

    return 0;
}
