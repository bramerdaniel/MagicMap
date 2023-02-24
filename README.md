# FluentSetups

[![build workflow](https://github.com/bramerdaniel/FluentSetups/actions/workflows/buildAndTest.yml/badge.svg)](https://github.com/bramerdaniel/FluentSetups/actions/workflows/buildAndTest.yml)

## What is FluentSetups

FluentSetups is a library that generates the boilerplate code for a fluent builder pattern ( I call it a fluent setup ).
It uses the roslyn source generators, to analyse your code and add the required setup classes, if you use the FluentSetups attributes in your code.

## Usage 

Lets assume that you have an object you want to create a fluent setup api for


``` csharp

 // This is the object we want to create an fluent setup/builder for
 public class Person
 {
   public int Age { get; set; }

   public string FirstName { get; set; }

   public string LastName { get; set; }

   public string FullName => $"{FirstName} {LastName}";
 }

```

Normaly you would have to write all that boilerplate code, 
with FluentSetups all you have to write, is this.

``` csharp
 [FluentSetup(typeof(Person))]
 public partial class PersonSetup
 {
 }
```
Now that you have done this, you are able to write the code below, 
without having to write any of the called methods or the static Setup class.
These are all generated for you, but you can customize them if you want, 
and the fluent setup generator will adjust the generated code to fit your needs.

``` csharp
      var person = Setup.Person()
         .WithFirstName("Robert")
         .WithLastName("Ramirez")
         .WithAge(34) 
         .Done();
```
