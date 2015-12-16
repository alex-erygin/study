    ServiceThrottlingBehavior stb = new ServiceThrottlingBehavior {
        MaxConcurrentSessions = 100,
        MaxConcurrentCalls = 100,
        MaxConcurrentInstances = 100
    };
    ServiceHost.Description.Behaviors.Add(stb);

    //http://stackoverflow.com/questions/794338/wcf-how-do-i-add-a-servicethrottlingbehavior-to-a-wcf-service