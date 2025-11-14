# OmniSave

The easy way to save application state with C#.

## Description

OmniSave is a library for easily saving and loading application state in your C# app. This includes Godot C# projects! Here's an example of just how easy it is:
```c#
using OmniSave;
using OmniSave.SaveAdapter;

var saveManager = new SaveManager(new BinarySaveAdapter());
using var file = File.Open("save.dat", FileMode.OpenOrCreate);
saveManager.Load(file); // That's it! Your data is now loaded.

public static class Data
{
	// The [Save] attribute tells OmniSave to save this field.
	[Save] public static int Counter = 0;
}
```

Count 'em up, that's three lines to load from a file! Wanna know how to save data? It's as easy as `saveManager.Save(file)`!

All you need to do is annotate the data you want to save with the `[Save]` attribute, and OmniSave does the rest.

## Installation

### Normal C# Project (including Godot)

1. Download the latest DLL from the [releases page](https://github.com/TheArchitect4855/omnisave/releases).
2. Add it to your project folder.
3. Add it as a dependency to your `.csproj` file:

```xml
<ItemGroup>
	<Reference Include="OmniSave">
		<HintPath>path/to/dll</HintPath>
	</Reference>
</ItemGroup>
```

(this goes inside your `<Project>` tag)

## Usage

Documentation is currently in progress. Until then, please check out the examples folder and doc comments. :)

## Building from Source

Should you need to build OmniSave from source, fear not! It is simple.

1. Clone this repository.
2. Navigate to the OmniSave project directory (e.g. `omnisave/omnisave`)
3. Run `dotnet build` or `dotnet build -c Release` for a release build.

## Support

Should you find any bugs, unexpected behaviours, or would like to request a feature, please create an issue on this repository.

## Roadmap

In no particular order, I would like to:

- Add proper documentation.
- Implement better serialization/deserialization support.
- Add support for the Unity game engine. (Unity somehow is still on .NET Framework)
- Turn this into a NuGet package.
