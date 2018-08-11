# CompilerAttributes

[![NuGet version (CompilerAttributes)](https://img.shields.io/nuget/v/CompilerAttributes.svg?style=flat-square)](https://www.nuget.org/packages/CompilerAttributes/)
[![Build status](https://ci.appveyor.com/api/projects/status/xcfe8editvax7l3m/branch/master?svg=true)](https://ci.appveyor.com/project/gregsdennis/compilerattributes/branch/master)
[![MyGet Build Status](https://www.myget.org/BuildSource/Badge/littlecrabsolutions?identifier=d13bf0e6-34b1-4011-9242-5b134eb9f7c3)](https://www.myget.org/)
[![Percentage of issues still open](http://isitmaintained.com/badge/open/gregsdennis/CompilerAttributes.svg)](http://isitmaintained.com/project/gregsdennis/CompilerAttributes "Percentage of issues still open")
[![Average time to resolve an issue](http://isitmaintained.com/badge/resolution/gregsdennis/CompilerAttributes.svg)](http://isitmaintained.com/project/gregsdennis/CompilerAttributes "Average time to resolve an issue")

[![Discuss on Slack](/Resources/Slack_RGB.png)](https://join.slack.com/t/manateeopensource/shared_invite/enQtMzU4MjgzMjgyNzU3LWQ0ODM5ZTVhMTVhODY1Mjk5MTIxMjgxZjI2NWRiZWZkYmExMDM0MDRjNGE4OWRkMjYxMTc1M2ViMTZiYzM0OTI)

<a href="http://www.jetbrains.com/resharper"><img src="http://i61.tinypic.com/15qvwj7.jpg" alt="ReSharper" title="ReSharper"></a>

We've all been in the situation where we wanted the compiler to interpret custom attributes like it does for `Obsolete`.  Sadly, the compiler is hardwired to recognize only that one.

Until now!  This project is a 2-in-1 library that contains a Roslyn analyzer and a couple of attributes that you can use to create compiler-recognized attributes to your heart's content!

It's published via a Nuget package that gets installed both as an analyzer and as a library reference.  Here's how it works:

## Install the Nuget packages

```powershell
Install-Package CompilerAttributes
```

## Create an attribute

This attribute is what you'll use to decorate identifiers that should generate a warning or error.

```csharp
public class MyAttributeAttribute : Attribute
{
}
```

***NOTE** By convention, attribute class names are suffixed with "Attribute".  When they're used, the suffix can be omitted.  Thus using the above attribute will look like `[MyAttribute]`.*

## Decorate your attribute

CompilerAttributes provides two attributes to mark your attribute as one that generates compiler output: `GeneratesWarningAttribute` and `GeneratesErrorAttribute`.  Pick one and decorate your attribute with it.

```csharp
[GeneratesWarning("{0} generated a warning")]
public class MyAttributeAttribute : Attribute
{
}
```

The single argument that is passed into the attribute is the message that will appear in the compiler output.  It supports up to a single string format argument (i.e. `{0}`) that will be replaced with the identifier name.

## Enjoy the fruits of your labor

Now whenever you use the `MyAttribute` attribute on something that thing will generate a compiler warning.

```csharp
[MyAttribute]
public class Class1
{
}

// later

var instance = new Class1();        // this line generates a warning that says "Class1 generated a warning"
```

## Backstory

I originally created this because I needed a way to indicate that some features I was working on for [Manatee.Json](https://github.com/gregsdennis/Manatee.Json) were going to be experimental, kind of an [opposite of the `Obsolete` attribute](https://stackoverflow.com/q/17487930/878701).

## Contributing

If you have questions, experience problems, or feature ideas, please create an issue.

If you'd like to help out with the code, please feel free to fork and create a pull request.

### The Project

This code uses C# 7 features, so a compiler/IDE that supports these features is required.

There are three solutions:

- *ComplierAttributes* - This is the main solution that builds the Nuget package.  It contains three projects the main project, a test project, and a VISX generating project.  Though I don't publish from the VISX project, it's immensely useful for debugging.  It will start a new instance of VS under a different profile and install the VISX.  From there, you just need to open a solution.  That's where the other solution comes in.
- *ClassLibrary1* - This has some boilerplate code that gives as many examples of usage of a symbol that I cared to think of.  Happily, all of these usages are underlined with little green squigglies, and the error output shows a bunch of warnings.
- *AutoBuild* - This one just contains the main project.  I use it to isolate this project as part of my CI process.  Just ignore it.

### Building

During development, building within Visual Studio should be fine.

I don't use the test project right now.  It was added as part of the template.  It could use some test cases... eventually.

### Code style and maintenance

I use [Jetbrains Resharper](https://www.jetbrains.com/resharper/) in Visual Studio to maintain the code style (and for many of the other things that it does).  The solution is set up with team style settings, so if you're using Resharper the settings should automatically load.  Please follow the suggestions.
