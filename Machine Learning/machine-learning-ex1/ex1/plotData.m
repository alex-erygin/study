function plotData(x, y)
figure; % open a new figure window
plot(x,y, 'rx', 'MarkerSize', 10);
xlabel('population size, in 10,000s');
ylabel('profit in $10,000s');
end
