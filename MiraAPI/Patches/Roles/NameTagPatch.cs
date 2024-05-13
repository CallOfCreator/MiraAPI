﻿using HarmonyLib;
using UnityEngine;

namespace MiraAPI.Patches.Roles;

/// <summary>
/// Set nametag color depending on option
/// </summary>
[HarmonyPatch(typeof(PlayerNameColor))]
public static class NameTagPatch
{
    [HarmonyPrefix, HarmonyPatch("Get", typeof(RoleBehaviour))]
    public static bool GetPatch([HarmonyArgument(0)] RoleBehaviour otherPlayerRole, ref Color __result)
    {
        if (PlayerControl.LocalPlayer.Data.IsDead)
        {
            __result = otherPlayerRole.NameColor;
            return false;
        }

        if (PlayerControl.LocalPlayer.Data.Role.IsImpostor && otherPlayerRole.IsImpostor)
        {
            return true;
        }

        if (PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || PlayerControl.LocalPlayer.Data.Role == null || otherPlayerRole == null)
        {
            __result = Color.white;
        }

        __result = PlayerControl.LocalPlayer.Data?.Role == otherPlayerRole ? otherPlayerRole.NameColor : Color.white;

        return false;
    }
}