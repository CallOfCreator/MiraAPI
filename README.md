[![](https://dcbadge.limes.pink/api/server/all-of-us-launchpad-794950428756410429)](https://discord.gg/all-of-us-launchpad-794950428756410429)

# Mira API

A thorough, but simple, Among Us modding API that covers:
- Roles
- Options
- Gamemodes
- Assets
- HUD Elements
- Compatibility
  
Mira API strives to be simple and easy to use, while also using as many base game elements as possible. The result is a less intrusive, better modding API that covers general use cases.

# Usage

To start using Mira API, you need to:
1. Add a reference to Mira API either through a DLL, project reference, or NuGet package.
2. Add a BepInDependency on your plugin class like this: `[BepInDependency(MiraApiPlugin.Id)]`
3. Implement the IMiraPlugin interface on your plugin class.

For a full example, see [this file](https://github.com/All-Of-Us-Mods/MiraAPI/blob/master/MiraAPI.Example/ExamplePlugin.cs).

## Roles
Roles are very simple in Mira API. There are 3 things you need to do to create a custom role:
1. Create a class that inherits from a base game role (like `RoleBehaviour`, `CrewmateRole`, `ImpostorRole`, etc).
2. Implement the `ICustomRole` interface from Mira API.
3. Add the `[RegisterCustomRole]` attribute to the class.

See [this file](https://github.com/All-Of-Us-Mods/MiraAPI/blob/master/MiraAPI.Example/CustomRole.cs) for a code example.

## Options
Options are also very simple in Mira API. Options are split up into Groups and Options. Every Option needs to be in a Group.

To create a group, you need to create a class that implements the `IModdedOptionGroup` interface. Groups contain 4 properties, `GroupName`, `GroupColor`, `GroupVisible`, and `AdvancedRole`. Only the `GroupName` is required.

Here is an example of a group class:
```csharp
public class MyOptionsGroup : IModdedOptionGroup
{
    public string GroupName => "My Options"; // this is required
    
    [ModdedNumberOption("My Number Option", min: 0, max: 10)]
    public float MyNumberOption { get; set; } = 5f;
}
```

You can access any group class using the `ModdedGroupSingleton` class like this:
```
// MyOptionsGroup is a class that implements IModdedOptionGroup
var myGroup = ModdedGroupSingleton<MyOptionsGroup>.Instance; // gets the instance of the group
System.Out.Console.WriteLine(myGroup.MyNumberOption); // prints the value of the option to the console
```

Once you have an options group, there are two ways to make the actual options:
- Use an Option Attribute with a property.  
- Create a ModdedOption property.

This is an example of using an Option Attribute on a property:
```csharp
// The first parameter is always the name of the option. The rest are dependent on the type of option.
[ModdedNumberOption("Sussy level", min: 0, max: 10)]
public float SussyLevel { get; set; } = 4f; // You can set a default value here.
```

And this is an example of a ModdedOption property:
```csharp
public ModdedToggleOption YeezusAbility { get; } = new ModdedToggleOption("Yeezus Ability", false);
```

Here is a full list of ModdedOption classes you can use: 
- `ModdedEnumOption`
- `ModdedNumberOption`
- `ModdedStringOption`
- `ModdedToggleOption`

To see a full example of an options class, see [this file](https://github.com/All-Of-Us-Mods/MiraAPI/blob/master/MiraAPI.Example/ExampleOptions.cs).

### Role Options

You can also specify a role type for an option or option group.

To set the role type for an entire group, set the `AdvancedRole` property on that group like this: 
```csharp
public class MyOptionsGroup : IModdedOptionGroup
{
    public string GroupName => "My Options";
    public Type AdvancedRole => typeof(MyRole); // this is the role that will have these options
    
    [ModdedNumberOption("Ability Uses", min: 0, max: 10)]
    public float AbilityUses { get; set; } = 5f;
}
```

To set the role type for individual options, specify the `roleType` parameter in the option like this:
```csharp
// this group doesnt specify a role, so it will show up in the global settings
public class MyOptionsGroup : IModdedOptionGroup
{
    public string GroupName => "My Options";
    
    // this option will only show up in the settings for MyRole
    [ModdedNumberOption("Ability Uses", min: 0, max: 10, roleType: typeof(MyRole))]
    public float AbilityUses { get; set; } = 5f;
}
```

An example can be found [here](https://github.com/All-Of-Us-Mods/MiraAPI/blob/master/MiraAPI.Example/Options/Roles/CustomRoleSettings.cs).

## Buttons

Mira API provides a simple interface for adding ability buttons to the game. There is only 2 steps:
1. Create a class that inherits from the `CustomActionButton` class and implement the properties and methods.
2. Add the `[RegisterCustomButton]` attribute to the class.

The button API is simple, but provides a lot of flexibility. There are various methods you can override to customize the behaviour of your button. See [this file](https://github.com/All-Of-Us-Mods/MiraAPI/blob/master/MiraAPI/Hud/CustomActionButton.cs) for a full list of methods you can override.

An example button can be found [here](https://github.com/All-Of-Us-Mods/MiraAPI/blob/master/MiraAPI.Example/ExampleButton.cs).

## Assets

Mira API provides a simple, but expandable asset system. The core of the system is the `LoadableAsset<T>` class. This is a generic abstract class that provides a pattern for loading assets. 

Mira API comes with two asset loaders:
1. `LoadableBundleAsset<T>`: This is used for loading assets from AssetBundles.
2. `LoadableResourceAsset`: This is used for loading **only sprites** from the Embedded Resources within a mod.

The code below shows how to use these asset loaders:
```csharp
// Load a sprite from an AssetBundle
AssetBundle bundle = AssetBundleManager.Load("MyBundle"); // AssetBundleManager is a utility provided by Reactor
LoadableAsset<Sprite> mySpriteAsset = new LoadableBundleAsset<Sprite>("MySprite", bundle);
Sprite sprite = mySpriteAsset.LoadAsset();

// Load a sprite from an Embedded Resource
// Make sure to set the Build Action of your image to Embedded Resource!
LoadableAsset<Sprite> buttonAsset = new LoadableResourceAsset("ExampleMod.Resources.MyButton.png");
Sprite button = buttonSpriteAsset.LoadAsset();
```

You can create your own asset loaders by inheriting from `LoadableAsset<T>` and implementing the `LoadAsset` method.

# Disclaimer

> This mod is not affiliated with Among Us or Innersloth LLC, and the content contained therein is not endorsed or otherwise sponsored by Innersloth LLC. Portions of the materials contained herein are property of Innersloth LLC. © Innersloth LLC.
