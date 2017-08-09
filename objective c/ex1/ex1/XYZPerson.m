//
//  XYZPerson.m
//  ex1
//
//  Created by Alexander Erygin on 11/07/2017.
//  Copyright © 2017 Aleksandr Erygin. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "XYZPerson.h"

@implementation XYZPerson

- (id) initWithFirstName: (NSString *)aFirstName lastName:(NSString *)aLastName {
    return [self initWithFirstName:aFirstName lastName:aLastName andBirthDate:nil];
}

- (id) initWithFirstName: (NSString *)aFirstName lastName: (NSString *)aLastName andBirthDate: (NSDate *)aBirthDate {
    self = [super init];
    
    if (self) {
        self.firstName = aFirstName;
        self.lastName = aLastName;
        self.dateOfBirth = aBirthDate;
    }
    
    return self;
}



- (void) sayHello {
    NSLog(@"Hello %@ %@!", self.firstName, self.lastName);
}

- (void) saySomething:(NSString *)greeting {
    NSLog(@"%@", greeting);
}

+ (XYZPerson*) person{
    return [[XYZPerson alloc] initWithFirstName:nil lastName:nil andBirthDate:nil];
}

@end
