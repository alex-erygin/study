//
//  XYZShootingPerson.m
//  ex1
//
//  Created by Aleksandr Erygin on 13.07.17.
//  Copyright © 2017 Aleksandr Erygin. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "XYZShoutingPerson.h"

@implementation XYZShoutingPerson

- (void) saySomething:(NSString *)greetings {
    NSString *newString = [greetings uppercaseString];
    NSLog(@"%@", newString);
}

@end
