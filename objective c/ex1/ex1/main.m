//
//  main.m
//  ex1
//
//  Created by Alexander Erygin on 11/07/2017.
//  Copyright © 2017 Aleksandr Erygin. All rights reserved.
//

#import <Foundation/Foundation.h>
#include "XYZShoutingPerson.h"

int main(int argc, const char * argv[]) {
    @autoreleasepool {
        // insert code here...
        XYZShoutingPerson *shooter = [[XYZShoutingPerson alloc] init];
        [shooter saySomething:@"Tobi zvizda"];
    }
    return 0;
}
