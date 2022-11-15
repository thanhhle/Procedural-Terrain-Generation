﻿using UnityEditor;

namespace MultiplayerARPG
{
    [CustomEditor(typeof(Item))]
    [CanEditMultipleObjects]
    public class ItemEditor : BaseGameDataEditor
    {
        protected override void SetFieldCondition()
        {
            Item item = target as Item;
            // Armor
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Armor), nameof(item.maxSocket));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Armor), nameof(item.equipmentModels));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Armor), nameof(item.requirement));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Armor), nameof(item.increaseStats));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Armor), nameof(item.increaseStatsRate));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Armor), nameof(item.increaseAttributes));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Armor), nameof(item.increaseAttributesRate));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Armor), nameof(item.increaseResistances));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Armor), nameof(item.increaseArmors));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Armor), nameof(item.increaseDamages));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Armor), nameof(item.increaseSkillLevels));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Armor), nameof(item.selfStatusEffectsWhenAttacking));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Armor), nameof(item.enemyStatusEffectsWhenAttacking));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Armor), nameof(item.selfStatusEffectsWhenAttacked));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Armor), nameof(item.enemyStatusEffectsWhenAttacked));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Armor), nameof(item.randomBonus));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Armor), nameof(item.equipmentSet));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Armor), nameof(item.armorType));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Armor), nameof(item.armorAmount));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Armor), nameof(item.maxDurability));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Armor), nameof(item.destroyIfBroken));
            // Weapon
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.maxSocket));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.equipmentModels));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.subEquipmentModels));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.requirement));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.launchClip));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.reloadClip));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.emptyClip));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.increaseStats));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.increaseStatsRate));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.increaseAttributes));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.increaseAttributesRate));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.increaseResistances));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.increaseArmors));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.increaseDamages));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.increaseSkillLevels));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.selfStatusEffectsWhenAttacking));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.enemyStatusEffectsWhenAttacking));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.selfStatusEffectsWhenAttacked));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.enemyStatusEffectsWhenAttacked));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.randomBonus));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.equipmentSet));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.weaponType));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.damageAmount));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.harvestDamageAmount));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.moveSpeedRateWhileAttacking));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.moveSpeedRateWhileCharging));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.moveSpeedRateWhileReloading));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.ammoCapacity));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.weaponAbility));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.crosshairSetting));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.fireType));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.fireStagger));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.fireSpread));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.destroyImmediatelyAfterFired));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.maxDurability));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Weapon), nameof(item.destroyIfBroken));
            // Shield
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Shield), nameof(item.maxSocket));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Shield), nameof(item.equipmentModels));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Shield), nameof(item.requirement));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Shield), nameof(item.increaseStats));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Shield), nameof(item.increaseStatsRate));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Shield), nameof(item.increaseAttributes));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Shield), nameof(item.increaseAttributesRate));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Shield), nameof(item.increaseResistances));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Shield), nameof(item.increaseArmors));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Shield), nameof(item.increaseDamages));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Shield), nameof(item.increaseSkillLevels));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Shield), nameof(item.selfStatusEffectsWhenAttacking));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Shield), nameof(item.enemyStatusEffectsWhenAttacking));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Shield), nameof(item.selfStatusEffectsWhenAttacked));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Shield), nameof(item.enemyStatusEffectsWhenAttacked));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Shield), nameof(item.randomBonus));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Shield), nameof(item.equipmentSet));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Shield), nameof(item.armorAmount));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Shield), nameof(item.maxDurability));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Shield), nameof(item.destroyIfBroken));
            // Potion
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Potion), nameof(item.buff));
            // Ammo
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Ammo), nameof(item.increaseDamages));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Ammo), nameof(item.ammoType));
            // Building
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Building), nameof(item.buildingEntity));
            // Pet
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Pet), nameof(item.petEntity));
            // Socket Enhancer
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.SocketEnhancer), nameof(item.socketEnhanceEffect));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.SocketEnhancer), nameof(item.selfStatusEffectsWhenAttacking));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.SocketEnhancer), nameof(item.enemyStatusEffectsWhenAttacking));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.SocketEnhancer), nameof(item.selfStatusEffectsWhenAttacked));
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.SocketEnhancer), nameof(item.enemyStatusEffectsWhenAttacked));
            // Mount
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Mount), nameof(item.mountEntity));
            // Attribute Increase
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.AttributeIncrease), nameof(item.attributeAmount));
            // Skill Use
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.Skill), nameof(item.skillLevel));
            // Skill Learn
            ShowOnEnum(nameof(item.itemType), nameof(Item.LegacyItemType.SkillLearn), nameof(item.skillLevel));
        }
    }
}
