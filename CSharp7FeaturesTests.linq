<Query Kind="Statements" />

// 1. Numeric literals
double distance = 10_000;
distance.Dump();

//2. Out variabls
var length = Int32.TryParse("1000",out int result);
result.Dump();

//3. Pattern Maching
object x = "test";

if(x is string test)
{
	test.Dump();
}


//4. Tuples... like Python
var user = ("Bob", 23, 32);
user.Item1.Dump();
user.Item2.Dump();
user.Item3.Dump();

//5. Named Tuple
var vehicle = (Name:"Honda", Year:"1992");
vehicle.Name.Dump();
vehicle.Year.Dump();

//6. Elvis operator
