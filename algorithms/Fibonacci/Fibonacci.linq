<Query Kind="Program" />

void Main()
{
	int n = 40;
	
	Fib(n).Dump();
	
	var calc = new FibOptimized();
	calc.Fib(n).Dump();
	
	FibWithoutRecursion(n).Dump();
}

// Define other methods, classes and namespaces here
public int Fib(int n) {
	if ( n == 1 ) {
		return 0;
	}
	
	if ( n == 2 ) {
		return 1;
	}
	
	return Fib(n-1) + Fib(n-2);
}

public class FibOptimized {
	private readonly Dictionary<int, int> cache = new Dictionary<int, int>();
	
	public FibOptimized()
	{
		cache[1] = 0;
		cache[2] = 1;
	}
	
	public int Fib(int n)
	{
		if (cache.ContainsKey(n)){
			return cache[n];
		}
		
		cache[n] = Fib(n - 1) + Fib(n - 2);

		return cache[n];
	}
}


public int FibWithoutRecursion( int n ) {
	int prev = 1;
	int prevprev = 0;
	
	if ( n == 1 ) {
		return prevprev;
	}
	
	if ( n == 2 ) {
		return prev;
	}
	
	int i = 2;
	
	int cur = 0;
	while ( i != n ) {
		cur = prev + prevprev;
		prevprev = prev;
		prev = cur;
		i++;
	}
	
	return cur;
}