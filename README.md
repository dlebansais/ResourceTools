# ResourceTools
A set of tools to load resources, even if compressed with Costura Fody.

[![Build status](https://ci.appveyor.com/api/projects/status/9ppi48xqfttu9ey1?svg=true)](https://ci.appveyor.com/project/dlebansais/resourcetools) [![codecov](https://codecov.io/gh/dlebansais/ResourceTools/branch/master/graph/badge.svg?token=O44VPQYRE7)](undefined) [![CodeFactor](https://www.codefactor.io/repository/github/dlebansais/resourcetools/badge)](https://www.codefactor.io/repository/github/dlebansais/resourcetools)

## Use

When one needs to load resources embedded in an assembly, there are two built-in methods that make it easy:

    string[] ResourceNames = MyAssembly.GetManifestResourceNames();

And

    using Stream ResourceStream = MyAssembly.GetManifestResourceStream("My.Resource.Path");

However, if the assembly is packed with [Fody/Costura](https://github.com/Fody/Costura) these resources can be compressed and therefore not recoverable without some effort.

This tools provides some helpers to load embedded resources even if they have been compressed and packed.

## Generic load

	public static bool Load<TResource>(string resourceName, string assemblyName, out TResource value)

Loads the resource `resourceName` (for example "MyNamespace.MyFolder.myimage.png") embedded in assembly `assemblyName` (without extension). If the resource is found, the method returns `true` and a reference to the resource in `value`. Otherwise it returns `false` and `value` is undefined.

To load directly from the calling assembly, set `assemblyName` to `string.Empty`.

## Helpers

This tool can loaded specific types of resources. For example, the `LoadIcon` method will only load icons (in case the resource type is not really known):

	public static bool LoadIcon(string resourceName, string assemblyName, out Icon value)

The tool can also display log traces with the `SetLogger` method. The default is to not have traces.  