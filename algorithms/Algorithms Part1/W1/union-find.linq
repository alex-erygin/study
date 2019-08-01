<Query Kind="Program" />

void Main()
{
	
}

// Define other methods and classes here

public class QuickFind {
	private int[] id;
	
	public QuickFind(int N){
		id = new int[N];
		for (int i=0; i < N; i++){
			id[i] = i;
		}
	}
	
	public bool Connected(int p, int q){
		return id[p] == id[q];
	}
	
	public void Union(int p, int q) {
		int pid = id[p];
		int qid = id[q];
		
		for (int i=0; i < id.Length; i++){
			if(id[i] == pid) {
				id[i] = qid;
			}
		}
	}
}


public class WeightedQuickUnionWithPathCompression {
	private int[] id;
	private int[] sz;
	
	public WeightedQuickUnionWithPathCompression(int N) {
		id = new int[N];
		sz = new int[N];
		for(int i=0; i < N; i++) {
			id[i] = i;
			sz[i] = 1;
		}
	}
	
	private int Root(int i) {
		while (i != id[i]) {
			id[i] = id[id[i]]; // path compression
			i = id[i];
		}
		
		return i;
	}
	
	public bool Connected(int p, int q) {
		return Root(p) == Root(q);
	}
	
	public void Union(int p, int q) {
		int i = Root(p);
		int j = Root(q);
		if (i == j) return;
		
		if (sz[i] < sz[j]) {
			id[i] = j;
			sz[j] += sz[i];
		} else {
			id[j] = i;
			sz[i] += sz[j];
		}
		
		
		id[i] = j;		
	}
}