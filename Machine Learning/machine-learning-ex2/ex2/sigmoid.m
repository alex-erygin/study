function g = sigmoid(z)
%SIGMOID Compute sigmoid functoon
%   J = SIGMOID(z) computes the sigmoid of z.

% You need to return the following variables correctly 
g = zeros(size(z));

% ====================== YOUR CODE HERE ======================
% Instructions: Compute the sigmoid of each value of z (z can be a matrix,
%               vector or scalar).
rows = size(z,1);
cols = size(z, 2);
scalar = isscalar(z);

if scalar == 1 
    g(1,1) = 1/(1 + exp(-z));
elseif scalar == 0
    for i=1:rows
        for k = 1:cols
            g(i,k) = 1/(1 + exp(-z(i,k)));
        end
    end
end





% =============================================================

end
