﻿using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public delegate void NetworkDestroyDelegate(
        byte reasons);
    public delegate void ReceiveDamageDelegate(
        Vector3 fromPosition,
        IGameEntity attacker,
        Dictionary<DamageElement, MinMaxFloat> damageAmounts,
        CharacterItem weapon,
        BaseSkill skill,
        short skillLevel);
    public delegate void ReceivedDamageDelegate(
        Vector3 fromPosition,
        IGameEntity attacker,
        CombatAmountType combatAmountType,
        int totalDamage,
        CharacterItem weapon,
        BaseSkill skill,
        short skillLevel);
    public delegate void AppliedRecoveryAmountDelegate(
        EntityInfo causer,
        int amount);
    public delegate void AttackRoutineDelegate(
        bool isLeftHand,
        CharacterItem weapon,
        int hitIndex,
        DamageInfo damageInfo,
        Dictionary<DamageElement, MinMaxFloat> damageAmounts,
        AimPosition aimPosition);
    public delegate void UseSkillRoutineDelegate(
        BaseSkill skill,
        short level,
        bool isLeftHand,
        CharacterItem weapon,
        int hitIndex,
        Dictionary<DamageElement, MinMaxFloat> damageAmounts,
        uint targetObjectId,
        AimPosition aimPosition);
    public delegate void LaunchDamageEntityDelegate(
        bool isLeftHand,
        CharacterItem weapon,
        Dictionary<DamageElement, MinMaxFloat> damageAmounts,
        BaseSkill skill,
        short skillLevel,
        int randomSeed,
        AimPosition aimPosition,
        Vector3 stagger,
        HashSet<DamageHitObjectInfo> hitObjectIds);
    public delegate void ApplyBuffDelegate(
        int dataId,
        BuffType type,
        short level);
}
