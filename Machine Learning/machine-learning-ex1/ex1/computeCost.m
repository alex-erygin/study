function J = computeCost(X, y, theta)
    m = length(y); % number of training examples
    J = 0;
    for i = 1:m
        H = theta(1,1) + theta(2,1)*X(i,2);
        J = J + (1/(2*m))*((H - y(i,1))^2);
    end
end
