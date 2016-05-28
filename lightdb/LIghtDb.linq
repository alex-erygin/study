<Query Kind="Program">
  <NuGetReference>LiteDB</NuGetReference>
  <Namespace>LiteDB</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	using (var db = new LiteDatabase(@"d:\\tmp\\mydata.db"))
	{
		var collection = db.GetCollection<User>("Users");

		int n = 1000;
		using (var bench = new Benchmark($"Insert {n} records:"))
		{
			for (int i = 0; i < n; i++)
			{
				var id = Guid.NewGuid();
				collection.Insert(new User { Id = id, Name = "Petrovich" });
			}
		}

		using (var bench = new Benchmark("Get collection count:"))
		{
			collection.Count().Dump();
		}

		using (var bench = new Benchmark("Skip take:"))
		{
			foreach (var user in collection.FindAll().Skip(10000).Take(10))
			{
				user.Id.Dump();
			}
		}
	}
}

public class User
{
	public Guid Id { get; set; }
	public string Name { get; set; }
}

public class Benchmark : IDisposable
{
	private readonly Stopwatch timer = new Stopwatch();
	private readonly string benchmarkName;

	public Benchmark(string benchmarkName)
	{
		this.benchmarkName = benchmarkName;
		timer.Start();
	}
	
	public void Dispose() {
		timer.Stop();
		benchmarkName.Dump();
		timer.Elapsed.Dump();
	}
}
// Define other methods and classes here
