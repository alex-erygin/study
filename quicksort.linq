<Query Kind="Program" />

void Main()
{
	var array = new int[] { 4, 6, 7, 3, 1, 2, 4, 77, 85, 2};
	array.Dump();
	
	QuickSort(array, 0, array.Length - 1);
	array.Dump();
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

// Define other methods and classes here