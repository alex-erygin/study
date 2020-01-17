package com.hulio.springbootexample;

import org.junit.jupiter.api.Test;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.test.context.ContextConfiguration;
import org.springframework.util.Assert;

@SpringBootTest
@ContextConfiguration(classes = StudentConfig.class)
class SpringBootExampleApplicationTests {
    @Autowired
    private Subject subject;

    @Test
    void contextLoads() {
        Assert.notNull(subject);
    }

}
