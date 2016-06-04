<Query Kind="Program">
  <NuGetReference>Benchmark.It</NuGetReference>
  <Namespace>BenchmarkIt</Namespace>
</Query>

void Main()
{
	var array = new int[] { 4, 6, 7, 3, 1, 2, 4, 77, 85, 2};
	var array2 = new int[] { 4, 6, 7, 3, 1, 2, 4, 77, 85, 2};

	BenchmarkIt.Benchmark.This("Array.Sort", ()=> Array.Sort(array2))
		.Against.This($"Quick sort array of {array.Length} items", ()=> QuickSort(array, 0, array.Length - 1))
		.For(10).Iterations()
		.PrintComparison();
}

static void QuickSort(int[] a, int l, int r) {
	int temp;
	int x = a[l + (r-l)/2];
	int i = l;
	int j = r;

	while (i <= j) {
		while (a[i] < x) i++;
		while (a[j] > x) j--;
		if (i <= j) {
			temp = a[i];
			a[i] = a[j];
			a[j] = temp;
			i++;
			j--;
		}
	}

	if (i < r) {
		QuickSort(a, i, r);
	}

	if (l < j) {
		QuickSort(a, l, j);
	}

}