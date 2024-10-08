﻿using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace MiraAPI.Example.Roles;

[RegisterCustomRole]
public class FreezerRole : ImpostorRole, ICustomRole
{
    public string RoleName => "Freezer";
    public string RoleLongDescription => "Freeze another player for a duration of time.";
    public string RoleDescription => RoleLongDescription;
    public Color RoleColor => Palette.Blue;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    [HideFromIl2Cpp]
    public LoadableAsset<Sprite> OptionsScreenshot => ExampleAssets.Banner;
    public int MaxPlayers => 2;
}
