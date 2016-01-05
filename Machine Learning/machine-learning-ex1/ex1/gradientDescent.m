function [theta, J_history] = gradientDescent(x, y, theta, alpha, num_iters)
    %GRADIENTDESCENT Performs gradient descent to learn theta
    %   theta = GRADIENTDESENT(X, y, theta, alpha, num_iters) updates theta by 
    %   taking num_iters gradient steps with learning rate alpha
    m = length(y); % number of training examples
    J_history = zeros(num_iters, 1);

    for i = 1:num_iters
        s0 = 0.0;
        s1 = 0.0;

        for k = 1:m
            H = theta(1,1) + theta(2,1)*x(k,2);        
            s0 = s0 + (alpha/m)*(H - y(k, 1));
            s1 = s1 + (alpha/m)*(H - y(k, 1))*x(k,2);
        end

        theta(1,1) = theta(1,1) - s0;
        theta(2,1) = theta(2,1) - s1;
        fprintf('theta0: %f,  theta1:%f\n', theta(1,1), theta(2,1)); 
        J_history(i) = computeCost(x, y, theta);
    end
end
