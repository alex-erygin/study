//
//  XYZPerson.m
//  ex1
//
//  Created by Alexander Erygin on 11/07/2017.
//  Copyright © 2017 Aleksandr Erygin. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface XYZPerson : NSObject

@property NSString *firstName;

@property NSString *lastName;

@property NSDate *dateOfBirth;

- (void) sayHello;

+ (id) person;

@end
