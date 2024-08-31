﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using MiraAPI.Modifiers.Types;
using MiraAPI.Utilities;
using Reactor.Utilities;
using Reactor.Utilities.Attributes;
using TMPro;
using UnityEngine;

namespace MiraAPI.Modifiers;

/// <summary>
/// The component for handling modifiers.
/// </summary>
[RegisterInIl2Cpp]
public class ModifierComponent(IntPtr ptr) : MonoBehaviour(ptr)
{
    /// <summary>
    /// Gets the active modifiers on the player.
    /// </summary>
    public ImmutableList<BaseModifier> ActiveModifiers => Modifiers.ToImmutableList();

    private List<BaseModifier> Modifiers { get; set; } = [];

    private PlayerControl _player = null!;

    private TextMeshPro _modifierText = null!;

    internal void ClearModifiers()
    {
        foreach (var modifier in Modifiers)
        {
            modifier.OnDeactivate();
        }

        Modifiers.Clear();
    }

    private void Start()
    {
        _player = GetComponent<PlayerControl>();
        Modifiers = [];

        if (!_player.AmOwner)
        {
            return;
        }

        _modifierText = Helpers.CreateTextLabel("ModifierText", HudManager.Instance.transform, AspectPosition.EdgeAlignments.RightTop, new Vector3(10.1f, 3.5f, 0), textAlignment: TextAlignmentOptions.Right);
        _modifierText.verticalAlignment = VerticalAlignmentOptions.Top;
    }

    private void FixedUpdate()
    {
        foreach (var modifier in Modifiers)
        {
            modifier.FixedUpdate();
        }
    }

    private void Update()
    {
        foreach (var modifier in Modifiers)
        {
            modifier.Update();
        }

        if (!_modifierText || !_player.AmOwner)
        {
            return;
        }

        var filteredModifiers = Modifiers.Where(mod => !mod.HideOnUi);

        var baseModifiers = filteredModifiers as BaseModifier[] ?? filteredModifiers.ToArray();

        if (baseModifiers.Length != 0)
        {
            var stringBuild = new StringBuilder();
            foreach (var mod in baseModifiers)
            {
                stringBuild.Append(CultureInfo.InvariantCulture, $"\n{mod.ModifierName}");
                if (mod is TimedModifier timer)
                {
                    stringBuild.Append(CultureInfo.InvariantCulture, $" <size=70%>({Math.Round(timer.Duration - timer.TimeRemaining, 0)}s/{timer.Duration}s)</size>");
                }
            }
            _modifierText.text = $"<b><size=130%>Modifiers:</b></size>{stringBuild}";
        }
        else if (_modifierText.text != string.Empty)
        {
            _modifierText.text = string.Empty;
        }
    }

    /// <summary>
    /// Removes a modifier from the player.
    /// </summary>
    /// <param name="type">The modifier type.</param>
    public void RemoveModifier(Type type)
    {
        var modifier = Modifiers.Find(x => x.GetType() == type);

        if (modifier is null)
        {
            Logger<MiraApiPlugin>.Error($"Cannot remove modifier {type.Name} because it is not active.");
            return;
        }

        RemoveModifier(modifier);
    }

    /// <summary>
    /// Removes a modifier from the player.
    /// </summary>
    /// <typeparam name="T">The modifier type.</typeparam>
    public void RemoveModifier<T>() where T : BaseModifier
    {
        RemoveModifier(typeof(T));
    }

    /// <summary>
    /// Removes a modifier from the player.
    /// </summary>
    /// <param name="modifierId">The modifier ID.</param>
    public void RemoveModifier(uint modifierId)
    {
        var modifier = Modifiers.Find(x => x.ModifierId == modifierId);

        if (modifier is null)
        {
            Logger<MiraApiPlugin>.Error($"Cannot remove modifier with id {modifierId} because it is not active.");
            return;
        }

        RemoveModifier(modifier);
    }

    /// <summary>
    /// Removes a modifier from the player.
    /// </summary>
    /// <param name="modifier">The modifier object.</param>
    public void RemoveModifier(BaseModifier modifier)
    {
        if (!Modifiers.Contains(modifier))
        {
            Logger<MiraApiPlugin>.Error($"Cannot remove modifier {modifier.ModifierName} because it is not active on this player.");
            return;
        }

        modifier.OnDeactivate();
        Modifiers.Remove(modifier);

        if (_player.AmOwner)
        {
            HudManager.Instance.SetHudActive(true);
        }
    }

    /// <summary>
    /// Adds a modifier to the player.
    /// </summary>
    /// <param name="modifier">The modifier to add.</param>
    /// <returns>The modifier that was added.</returns>
    public BaseModifier? AddModifier(BaseModifier modifier)
    {
        if (Modifiers.Contains(modifier))
        {
            Logger<MiraApiPlugin>.Error($"Player already has modifier with id {modifier.ModifierId}!");
            return null;
        }

        var modifierId = ModifierManager.GetModifierId(modifier.GetType());

        if (modifierId == null)
        {
            Logger<MiraApiPlugin>.Error($"Cannot add modifier {modifier.GetType().Name} because it has no ID!");
            return null;
        }

        Modifiers.Add(modifier);
        modifier.Player = _player;
        modifier.ModifierId = modifierId.Value;
        modifier.OnActivate();

        if (!_player.AmOwner)
        {
            return modifier;
        }

        if (modifier is TimedModifier { AutoStart: true } timer)
        {
            timer.StartTimer();
        }

        HudManager.Instance.SetHudActive(true);

        return modifier;
    }

    /// <summary>
    /// Adds a modifier to the player.
    /// </summary>
    /// <param name="type">The modifier type.</param>
    /// <returns>The modifier that was added.</returns>
    public BaseModifier? AddModifier(Type type)
    {
        var modifierId = ModifierManager.GetModifierId(type);
        if (modifierId == null)
        {
            Logger<MiraApiPlugin>.Error($"Cannot add modifier {type.Name} because it is not registered.");
            return null;
        }

        if (Modifiers.Find(x => x.ModifierId == modifierId) != null)
        {
            Logger<MiraApiPlugin>.Error($"Player already has modifier with id {modifierId}!");
            return null;
        }

        if (Activator.CreateInstance(type) is not BaseModifier modifier)
        {
            Logger<MiraApiPlugin>.Error($"Cannot add modifier {type.Name} because it is null.");
            return null;
        }

        AddModifier(modifier);

        return modifier;
    }

    /// <summary>
    /// Adds a modifier to the player.
    /// </summary>
    /// <typeparam name="T">The Type of the modifier.</typeparam>
    /// <returns>The new modifier.</returns>
    public T? AddModifier<T>() where T : BaseModifier
    {
        return AddModifier(typeof(T)) as T;
    }
}
