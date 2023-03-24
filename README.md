# MagicMap

SourceGenerator based package for generating boilerplate code like object mappers

This is still under construction

```C#
public class Person
{
   public string Name { get; set; } 
   public int Age { get; set; } 
}

public record PersonModel
{
   public string Name { get; set; }
   public long Age { get; set; }
}

[TypeMapper(typeof(Person), typeof(PersonModel))]
internal partial class PersonMapper
{

}


```
