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
        
        NSMutableString* firstName = [[NSMutableString alloc] initWithString:@"Дядька"];
        XYZPerson* man = [[XYZPerson alloc] initWithFirstName:firstName lastName:@"Смит"];
        
        [firstName appendString:@" а может быть и тетька"];
        
        [man sayHello];
    }
    return 0;
}
