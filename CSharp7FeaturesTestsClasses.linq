<Query Kind="Program" />

void Main()
{
	var foo = new Foo("Test");
	
	foo.DumpTrace();

}

public class Foo
{
	public Foo(string name) => Name = name;

	public string Name { get; set; }
	
	public string GetEnd() => throw new NotImplementedException();

	~Foo() => Console.WriteLine("Finalize123");
}