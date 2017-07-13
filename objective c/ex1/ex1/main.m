//
//  main.m
//  ex1
//
//  Created by Alexander Erygin on 11/07/2017.
//  Copyright © 2017 Aleksandr Erygin. All rights reserved.
//

#import <Foundation/Foundation.h>
#include "XYZPerson.h"

int main(int argc, const char * argv[]) {
    @autoreleasepool {
        // insert code here...
        XYZPerson *poc = [[XYZPerson alloc] init];
        [poc sayHello];
        [poc saySomething:@"!"];
    }
    return 0;
}
