package com.hulio.springbootexample;

import org.springframework.stereotype.Component;

@Component
public class History implements Subject {
    private String content = "history chapter";

    @Override
    public void attend() {
        System.out.print("Reading :" + content);
    }
}
