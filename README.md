# LINQ Expression based validation library
```C#
var i = 0;
try {
  i.ThrowIf(v => v == 0);
} catch (PassageException<int> exc) {
  Console.WriteLine(exc.Message);
  Assert.IsTrue(exc.Message.Contains("v == 0"));
  throw;
}
```
