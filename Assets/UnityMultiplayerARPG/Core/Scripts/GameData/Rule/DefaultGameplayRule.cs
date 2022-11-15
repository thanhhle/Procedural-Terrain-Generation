﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = "Default Gameplay Rule", menuName = "Create GameplayRule/Default Gameplay Rule", order = -2999)]
    public partial class DefaultGameplayRule : BaseGameplayRule
    {
        [Header("Levelling/Stat/Skill")]
        public float increaseStatPointEachLevel = 5;
        public float increaseSkillPointEachLevel = 1;
        [Tooltip("If this > `0`, it will increase stat point until character reached max level. If it's `20`, it will increase stat point until character reached level `20`")]
        public short increaseStatPointsUntilReachedLevel = 0;
        [Tooltip("If this > `0`, it will increase skill point until character reached max level. If it's `20`, it will increase skill point until character reached level `20`")]
        public short increaseSkillPointsUntilReachedLevel = 0;
        [Range(0f, 100f)]
        public float expLostPercentageWhenDeath = 2f;
        [Header("Stamina/Sprint")]
        public float staminaRecoveryPerSeconds = 5;
        public float staminaDecreasePerSeconds = 5;
        public float moveSpeedRateWhileOverweight = 0.05f;
        [FormerlySerializedAs("moveSpeedRateWhileSprint")]
        public float moveSpeedRateWhileSprinting = 1.5f;
        [Header("Walk")]
        public float moveSpeedRateWhileWalking = 0.5f;
        [Header("Crouch")]
        public float moveSpeedRateWhileCrouching = 0.35f;
        [Header("Crawl")]
        public float moveSpeedRateWhileCrawling = 0.15f;
        [Header("Swim")]
        public float moveSpeedRateWhileSwimming = 0.5f;
        [Header("Hp/Mp/Food/Water")]
        public int hungryWhenFoodLowerThan = 40;
        public int thirstyWhenWaterLowerThan = 40;
        public float foodDecreasePerSeconds = 4;
        public float waterDecreasePerSeconds = 2;
        [Range(0f, 1f)]
        public float hpRecoveryRatePerSeconds = 0.05f;
        [Range(0f, 1f)]
        public float mpRecoveryRatePerSeconds = 0.05f;
        [Range(0f, 1f)]
        public float hpDecreaseRatePerSecondsWhenHungry = 0.05f;
        [Range(0f, 1f)]
        public float mpDecreaseRatePerSecondsWhenHungry = 0.05f;
        [Range(0f, 1f)]
        public float hpDecreaseRatePerSecondsWhenThirsty = 0.05f;
        [Range(0f, 1f)]
        public float mpDecreaseRatePerSecondsWhenThirsty = 0.05f;
        [Header("Durability")]
        public float normalDecreaseWeaponDurability = 0.5f;
        public float normalDecreaseShieldDurability = 0.5f;
        public float normalDecreaseArmorDurability = 0.1f;
        public float blockedDecreaseWeaponDurability = 0.5f;
        public float blockedDecreaseShieldDurability = 0.75f;
        public float blockedDecreaseArmorDurability = 0.15f;
        public float criticalDecreaseWeaponDurability = 0.75f;
        public float criticalDecreaseShieldDurability = 0.5f;
        public float criticalDecreaseArmorDurability = 0.15f;
        public float missDecreaseWeaponDurability = 0f;
        public float missDecreaseShieldDurability = 0;
        public float missDecreaseArmorDurability = 0f;
        [Header("Damage Occurs")]
        public bool alwaysHitWhenCriticalOccurs = true;
        [Header("Fall Damage")]
        [Tooltip("Character will receive damage 1% of Max Hp, when fall distance = min distance")]
        public float fallDamageMinDistance = 5;
        [Tooltip("Character will receive damage 100% of Max Hp, when fall distance >= max distance")]
        public float fallDamageMaxDistance = 20;
        [Header("Level Up")]
        public bool recoverHpWhenLevelUp;
        public bool recoverMpWhenLevelUp;
        public bool recoverFoodWhenLevelUp;
        public bool recoverWaterWhenLevelUp;
        public bool recoverStaminaWhenLevelUp;

        public override bool RandomAttackHitOccurs(Vector3 fromPosition, BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver, Dictionary<DamageElement, MinMaxFloat> damageAmounts, CharacterItem weapon, BaseSkill skill, short skillLevel, int randomSeed, out bool isCritical, out bool isBlocked)
        {
            isCritical = false;
            isBlocked = false;
            if (attacker == null)
                return true;
            isCritical = Random.value <= GetCriticalChance(attacker, damageReceiver);
            bool isHit = Random.value <= GetHitChance(attacker, damageReceiver);
            if (!isHit && isCritical && alwaysHitWhenCriticalOccurs)
                isHit = true;
            isBlocked = Random.value <= GetBlockChance(attacker, damageReceiver);
            return isHit;
        }

        public override float RandomAttackDamage(Vector3 fromPosition, BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver, DamageElement damageElement, MinMaxFloat damageAmount, CharacterItem weapon, BaseSkill skill, short skillLevel, int randomSeed)
        {
            return damageAmount.Random(randomSeed);
        }

        public override float GetHitChance(BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver)
        {
            // Attacker stats
            CharacterStats attackerStats = attacker.GetCaches().Stats;
            // Damage receiver stats
            CharacterStats dmgReceiverStats = damageReceiver.GetCaches().Stats;
            // Calculate chance to hit
            float attackerAcc = attackerStats.accuracy;
            float dmgReceiverEva = dmgReceiverStats.evasion;
            short attackerLvl = attacker.Level;
            short dmgReceiverLvl = damageReceiver.Level;
            float hitChance = 2f;

            if (attackerAcc != 0 && dmgReceiverEva != 0)
                hitChance *= (attackerAcc / (attackerAcc + dmgReceiverEva));

            if (attackerLvl != 0 && dmgReceiverLvl != 0)
                hitChance *= ((float)attackerLvl / (float)(attackerLvl + dmgReceiverLvl));

            // Minimum hit chance is 5%
            if (hitChance < 0.05f)
                hitChance = 0.05f;
            // Maximum hit chance is 95%
            if (hitChance > 0.95f)
                hitChance = 0.95f;
            return hitChance;
        }

        public override float GetCriticalChance(BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver)
        {
            float criRate = attacker.GetCaches().Stats.criRate;
            // Minimum critical chance is 5%
            if (criRate < 0.05f)
                criRate = 0.05f;
            // Maximum critical chance is 95%
            if (criRate > 0.95f)
                criRate = 0.95f;
            return criRate;
        }

        public override float GetCriticalDamage(BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver, float damage)
        {
            return damage * attacker.GetCaches().Stats.criDmgRate;
        }

        public override float GetBlockChance(BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver)
        {
            float blockRate = damageReceiver.GetCaches().Stats.blockRate;
            // Minimum block chance is 5%
            if (blockRate < 0.05f)
                blockRate = 0.05f;
            // Maximum block chance is 95%
            if (blockRate > 0.95f)
                blockRate = 0.95f;
            return blockRate;
        }

        public override float GetBlockDamage(BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver, float damage)
        {
            float blockDmgRate = damageReceiver.GetCaches().Stats.blockDmgRate;
            // Minimum block damage is 5%
            if (blockDmgRate < 0.05f)
                blockDmgRate = 0.05f;
            // Maximum block damage is 95%
            if (blockDmgRate > 0.95f)
                blockDmgRate = 0.95f;
            return damage - (damage * blockDmgRate);
        }

        public override float GetDamageReducedByResistance(Dictionary<DamageElement, float> damageReceiverResistances, Dictionary<DamageElement, float> damageReceiverArmors, float damageAmount, DamageElement damageElement)
        {
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            // Reduce damage by resistance
            float resistanceAmount;
            if (damageReceiverResistances.TryGetValue(damageElement, out resistanceAmount))
            {
                if (resistanceAmount > damageElement.MaxResistanceAmount)
                    resistanceAmount = damageElement.MaxResistanceAmount;
                damageAmount -= damageAmount * resistanceAmount; // If resistance is minus damage will be increased
            }
            // Reduce damage by armor
            float armorAmount;
            if (damageReceiverArmors.TryGetValue(damageElement, out armorAmount))
            {
                // Formula: Attack * 100 / (100 + Defend)
                damageAmount *= 100f / (100f + armorAmount);
            }
            return damageAmount;
        }

        public override int GetTotalDamage(Vector3 fromPosition, EntityInfo instigator, DamageableEntity damageReceiver, float totalDamage, CharacterItem weapon, BaseSkill skill, short skillLevel)
        {
            return (int)totalDamage;
        }

        public override float GetRecoveryHpPerSeconds(BaseCharacterEntity character)
        {
            if (IsHungry(character))
                return 0;
            return (character.GetCaches().MaxHp * hpRecoveryRatePerSeconds) + character.GetCaches().Stats.hpRecovery;
        }

        public override float GetRecoveryMpPerSeconds(BaseCharacterEntity character)
        {
            if (IsThirsty(character))
                return 0;
            return (character.GetCaches().MaxMp * mpRecoveryRatePerSeconds) + character.GetCaches().Stats.mpRecovery;
        }

        public override float GetRecoveryStaminaPerSeconds(BaseCharacterEntity character)
        {
            return staminaRecoveryPerSeconds + character.GetCaches().Stats.staminaRecovery;
        }

        public override float GetDecreasingHpPerSeconds(BaseCharacterEntity character)
        {
            if (character is BaseMonsterCharacterEntity)
                return 0f;
            float result = 0f;
            if (IsHungry(character))
                result += character.GetCaches().MaxHp * hpDecreaseRatePerSecondsWhenHungry;
            if (IsThirsty(character))
                result += character.GetCaches().MaxHp * hpDecreaseRatePerSecondsWhenThirsty;
            return result;
        }

        public override float GetDecreasingMpPerSeconds(BaseCharacterEntity character)
        {
            if (character is BaseMonsterCharacterEntity)
                return 0f;
            float result = 0f;
            if (IsHungry(character))
                result += character.GetCaches().MaxMp * mpDecreaseRatePerSecondsWhenHungry;
            if (IsThirsty(character))
                result += character.GetCaches().MaxMp * mpDecreaseRatePerSecondsWhenThirsty;
            return result;
        }

        public override float GetDecreasingStaminaPerSeconds(BaseCharacterEntity character)
        {
            if (character.ExtraMovementState != ExtraMovementState.IsSprinting ||
                !character.MovementState.Has(MovementState.IsGrounded) ||
                (!character.MovementState.Has(MovementState.Forward) &&
                !character.MovementState.Has(MovementState.Backward) &&
                !character.MovementState.Has(MovementState.Left) &&
                !character.MovementState.Has(MovementState.Right)))
                return 0f;
            return staminaDecreasePerSeconds;
        }

        public override float GetDecreasingFoodPerSeconds(BaseCharacterEntity character)
        {
            if (character is BaseMonsterCharacterEntity)
                return 0f;
            return foodDecreasePerSeconds;
        }

        public override float GetDecreasingWaterPerSeconds(BaseCharacterEntity character)
        {
            if (character is BaseMonsterCharacterEntity)
                return 0f;
            return waterDecreasePerSeconds;
        }

        public override float GetExpLostPercentageWhenDeath(BaseCharacterEntity character)
        {
            if (character is BaseMonsterCharacterEntity)
                return 0f;
            return expLostPercentageWhenDeath;
        }

        public override float GetOverweightMoveSpeedRate(BaseGameEntity gameEntity)
        {
            // For some gameplay rule, move speed rate may difference for specific entiy type.
            return moveSpeedRateWhileOverweight;
        }

        public override float GetSprintMoveSpeedRate(BaseGameEntity gameEntity)
        {
            // For some gameplay rule, move speed rate may difference for specific entiy type.
            return moveSpeedRateWhileSprinting;
        }

        public override float GetWalkMoveSpeedRate(BaseGameEntity gameEntity)
        {
            // For some gameplay rule, move speed rate may difference for specific entiy type.
            return moveSpeedRateWhileWalking;
        }

        public override float GetCrouchMoveSpeedRate(BaseGameEntity gameEntity)
        {
            // For some gameplay rule, move speed rate may difference for specific entiy type.
            return moveSpeedRateWhileCrouching;
        }

        public override float GetCrawlMoveSpeedRate(BaseGameEntity gameEntity)
        {
            // For some gameplay rule, move speed rate may difference for specific entiy type.
            return moveSpeedRateWhileCrawling;
        }

        public override float GetSwimMoveSpeedRate(BaseGameEntity gameEntity)
        {
            // For some gameplay rule, move speed rate may difference for specific entiy type.
            return moveSpeedRateWhileSwimming;
        }

        public override float GetLimitWeight(ICharacterData character, CharacterStats stats)
        {
            return stats.weightLimit;
        }

        public override float GetTotalWeight(ICharacterData character, CharacterStats stats)
        {
            float result = character.EquipItems.GetTotalItemWeight() + character.NonEquipItems.GetTotalItemWeight();
            // Weight from right hand equipment
            if (character.EquipWeapons.rightHand.NotEmptySlot())
                result += character.EquipWeapons.rightHand.GetItem().Weight;
            // Weight from left hand equipment
            if (character.EquipWeapons.leftHand.NotEmptySlot())
                result += character.EquipWeapons.leftHand.GetItem().Weight;
            return result;
        }

        public override short GetLimitSlot(ICharacterData character, CharacterStats stats)
        {
            return (short)(stats.slotLimit + GameInstance.Singleton.baseSlotLimit);
        }

        public override short GetTotalSlot(ICharacterData character, CharacterStats stats)
        {
            return character.NonEquipItems.GetTotalItemSlot();
        }

        public override bool IsHungry(BaseCharacterEntity character)
        {
            return foodDecreasePerSeconds > 0 && character.CurrentFood < hungryWhenFoodLowerThan;
        }

        public override bool IsThirsty(BaseCharacterEntity character)
        {
            return waterDecreasePerSeconds > 0 && character.CurrentWater < thirstyWhenWaterLowerThan;
        }

        public override bool RewardExp(BaseCharacterEntity character, Reward reward, float multiplier, RewardGivenType rewardGivenType, out int rewardedExp)
        {
            rewardedExp = 0;
            if ((character is BaseMonsterCharacterEntity) &&
                (character as BaseMonsterCharacterEntity).SummonType != SummonType.PetItem)
            {
                // If it's monster and not pet, do not increase exp
                return false;
            }

            bool isLevelUp = false;
            int exp = reward.exp;
            BasePlayerCharacterEntity playerCharacter = character as BasePlayerCharacterEntity;
            if (playerCharacter != null)
            {
                GuildData guildData;
                switch (rewardGivenType)
                {
                    case RewardGivenType.KillMonster:
                        exp = Mathf.CeilToInt(exp * multiplier * (ExpRate + playerCharacter.GetCaches().Stats.expRate));
                        if (GameInstance.ServerGuildHandlers.TryGetGuild(playerCharacter.GuildId, out guildData))
                            exp += Mathf.CeilToInt(exp * guildData.IncreaseExpGainPercentage * 0.01f);
                        break;
                    case RewardGivenType.PartyShare:
                        exp = Mathf.CeilToInt(exp * multiplier * (ExpRate + playerCharacter.GetCaches().Stats.expRate));
                        if (GameInstance.ServerGuildHandlers.TryGetGuild(playerCharacter.GuildId, out guildData))
                            exp += Mathf.CeilToInt(exp * guildData.IncreaseShareExpGainPercentage * 0.01f);
                        break;
                }
            }

            int nextLevelExp = character.GetNextLevelExp();
            if (nextLevelExp > 0)
            {
                // Increasing level if character not reached max level yet
                character.Exp = character.Exp.Increase(exp);
                while (nextLevelExp > 0 && character.Exp >= nextLevelExp)
                {
                    character.Exp = character.Exp - nextLevelExp;
                    ++character.Level;
                    nextLevelExp = character.GetNextLevelExp();
                    if (playerCharacter != null)
                    {
                        try
                        {
                            if (increaseStatPointsUntilReachedLevel == 0 ||
                                character.Level + 1 < increaseStatPointsUntilReachedLevel)
                            {
                                checked
                                {
                                    playerCharacter.StatPoint += increaseStatPointEachLevel;
                                }
                            }
                        }
                        catch (System.OverflowException)
                        {
                            playerCharacter.StatPoint = float.MaxValue;
                        }

                        try
                        {
                            if (increaseSkillPointsUntilReachedLevel == 0 ||
                                character.Level + 1 < increaseSkillPointsUntilReachedLevel)
                            {
                                checked
                                {
                                    playerCharacter.SkillPoint += increaseSkillPointEachLevel;
                                }
                            }
                        }
                        catch (System.OverflowException)
                        {
                            playerCharacter.SkillPoint = float.MaxValue;
                        }
                    }
                    isLevelUp = true;
                }

            }

            if (nextLevelExp <= 0)
            {
                // Don't collect exp if character reached max level
                character.Exp = 0;
            }

            if (isLevelUp && !character.IsDead())
            {
                if (recoverHpWhenLevelUp)
                    character.CurrentHp = character.MaxHp;
                if (recoverMpWhenLevelUp)
                    character.CurrentMp = character.MaxMp;
                if (recoverFoodWhenLevelUp)
                    character.CurrentFood = character.MaxFood;
                if (recoverWaterWhenLevelUp)
                    character.CurrentWater = character.MaxWater;
                if (recoverStaminaWhenLevelUp)
                    character.CurrentStamina = character.MaxStamina;
            }
            rewardedExp = exp;
            return isLevelUp;
        }

        public override void RewardCurrencies(BaseCharacterEntity character, Reward reward, float multiplier, RewardGivenType rewardGivenType, out int rewardedGold)
        {
            rewardedGold = 0;
            if (character is BaseMonsterCharacterEntity)
            {
                // Don't give reward currencies to monsters
                return;
            }

            int gold = reward.gold;
            BasePlayerCharacterEntity playerCharacter = character as BasePlayerCharacterEntity;
            if (playerCharacter != null)
            {
                GuildData guildData;
                switch (rewardGivenType)
                {
                    case RewardGivenType.KillMonster:
                        gold = Mathf.CeilToInt(gold * multiplier * (GoldRate + playerCharacter.GetCaches().Stats.goldRate));
                        if (GameInstance.ServerGuildHandlers.TryGetGuild(playerCharacter.GuildId, out guildData))
                            gold += Mathf.CeilToInt(gold * guildData.IncreaseGoldGainPercentage * 0.01f);
                        break;
                    case RewardGivenType.PartyShare:
                        gold = Mathf.CeilToInt(gold * multiplier * (GoldRate + playerCharacter.GetCaches().Stats.goldRate));
                        if (GameInstance.ServerGuildHandlers.TryGetGuild(playerCharacter.GuildId, out guildData))
                            gold += Mathf.CeilToInt(gold * guildData.IncreaseShareGoldGainPercentage * 0.01f);
                        break;
                }

                playerCharacter.Gold = playerCharacter.Gold.Increase(gold);
                playerCharacter.IncreaseCurrencies(reward.currencies, multiplier);
                rewardedGold = gold;
            }
        }

        public override float GetEquipmentStatsRate(CharacterItem characterItem)
        {
            if (characterItem.GetMaxDurability() <= 0)
                return 1;
            float durabilityRate = (float)characterItem.durability / (float)characterItem.GetMaxDurability();
            if (durabilityRate > 0.5f)
                return 1f;
            else if (durabilityRate > 0.3f)
                return 0.75f;
            else if (durabilityRate > 0.15f)
                return 0.5f;
            else if (durabilityRate > 0.05f)
                return 0.25f;
            else
                return 0f;
        }

        public override void OnCharacterRespawn(ICharacterData character)
        {
            character.CurrentHp = character.GetCaches().MaxHp;
            character.CurrentMp = character.GetCaches().MaxMp;
            character.CurrentStamina = character.GetCaches().MaxStamina;
            character.CurrentFood = character.GetCaches().MaxFood;
            character.CurrentWater = character.GetCaches().MaxWater;
        }

        public override void OnCharacterReceivedDamage(BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver, CombatAmountType combatAmountType, int damage, CharacterItem weapon, BaseSkill skill, short skillLevel)
        {
            float decreaseWeaponDurability;
            float decreaseShieldDurability;
            float decreaseArmorDurability;
            GetDecreaseDurabilityAmount(combatAmountType, out decreaseWeaponDurability, out decreaseShieldDurability, out decreaseArmorDurability);
            if (attacker != null)
            {
                // Decrease Weapon Durability
                DecreaseEquipWeaponsDurability(attacker, decreaseWeaponDurability);
                CharacterStats stats = attacker.GetCaches().Stats;
                // Hp Leeching, don't decrease damage receiver's Hp again
                int leechAmount = Mathf.CeilToInt(damage * stats.hpLeechRate);
                if (leechAmount != 0)
                {
                    attacker.CurrentHp += leechAmount;
                    attacker.CurrentHp = Mathf.Clamp(attacker.CurrentHp, 0, attacker.MaxHp);
                }
                // Mp Leeching
                leechAmount = Mathf.CeilToInt(damage * stats.mpLeechRate);
                if (leechAmount != 0)
                {
                    attacker.CurrentMp += leechAmount;
                    attacker.CurrentMp = Mathf.Clamp(attacker.CurrentMp, 0, attacker.MaxMp);
                    damageReceiver.CurrentMp -= leechAmount;
                    damageReceiver.CurrentMp = Mathf.Clamp(damageReceiver.CurrentMp, 0, damageReceiver.MaxMp);
                }
                // Stamina Leeching
                leechAmount = Mathf.CeilToInt(damage * stats.staminaLeechRate);
                if (leechAmount != 0)
                {
                    attacker.CurrentStamina += leechAmount;
                    attacker.CurrentStamina = Mathf.Clamp(attacker.CurrentStamina, 0, attacker.MaxStamina);
                    damageReceiver.CurrentStamina -= leechAmount;
                    damageReceiver.CurrentStamina = Mathf.Clamp(damageReceiver.CurrentStamina, 0, damageReceiver.MaxStamina);
                }
                // Applies status effects
                IEquipmentItem tempEquipmentItem;
                Buff tempBuff;
                BaseSkill tempSkill;
                EntityInfo attackerInfo = attacker.GetInfo();
                EntityInfo damageReceiverInfo = damageReceiver.GetInfo();
                // Attacker
                foreach (CharacterItem armorItem in attacker.EquipItems)
                {
                    tempEquipmentItem = armorItem.GetEquipmentItem();
                    ApplyStatusEffectsWhenAttacking(armorItem, tempEquipmentItem, attackerInfo, attacker, damageReceiver);
                }
                tempEquipmentItem = attacker.EquipWeapons.GetRightHandEquipmentItem();
                if (tempEquipmentItem != null)
                {
                    ApplyStatusEffectsWhenAttacking(attacker.EquipWeapons.rightHand, tempEquipmentItem, attackerInfo, attacker, damageReceiver);
                }
                tempEquipmentItem = attacker.EquipWeapons.GetLeftHandEquipmentItem();
                if (tempEquipmentItem != null)
                {
                    ApplyStatusEffectsWhenAttacking(attacker.EquipWeapons.leftHand, tempEquipmentItem, attackerInfo, attacker, damageReceiver);
                }
                foreach (CharacterBuff characterBuff in attacker.Buffs)
                {
                    tempBuff = characterBuff.GetBuff();
                    tempBuff.ApplySelfStatusEffectsWhenAttacking(characterBuff.level, attackerInfo, attacker);
                    tempBuff.ApplyEnemyStatusEffectsWhenAttacking(characterBuff.level, attackerInfo, damageReceiver);
                }
                foreach (KeyValuePair<BaseSkill, short> characterSkill in attacker.GetCaches().Skills)
                {
                    tempSkill = characterSkill.Key;
                    if (!tempSkill.IsPassive)
                        continue;
                    tempSkill.Buff.ApplySelfStatusEffectsWhenAttacking(characterSkill.Value, attackerInfo, attacker);
                    tempSkill.Buff.ApplyEnemyStatusEffectsWhenAttacking(characterSkill.Value, attackerInfo, damageReceiver);
                }
                // Damage Receiver
                foreach (CharacterItem armorItem in damageReceiver.EquipItems)
                {
                    tempEquipmentItem = armorItem.GetEquipmentItem();
                    ApplyStatusEffectsWhenAttacked(armorItem, tempEquipmentItem, damageReceiverInfo, attacker, damageReceiver);
                }
                tempEquipmentItem = damageReceiver.EquipWeapons.GetRightHandEquipmentItem();
                if (tempEquipmentItem != null)
                {
                    ApplyStatusEffectsWhenAttacked(damageReceiver.EquipWeapons.rightHand, tempEquipmentItem, damageReceiverInfo, attacker, damageReceiver);
                }
                tempEquipmentItem = damageReceiver.EquipWeapons.GetLeftHandEquipmentItem();
                if (tempEquipmentItem != null)
                {
                    ApplyStatusEffectsWhenAttacked(damageReceiver.EquipWeapons.leftHand, tempEquipmentItem, damageReceiverInfo, attacker, damageReceiver);
                }
                foreach (CharacterBuff characterBuff in damageReceiver.Buffs)
                {
                    tempBuff = characterBuff.GetBuff();
                    tempBuff.ApplySelfStatusEffectsWhenAttacked(characterBuff.level, damageReceiverInfo, damageReceiver);
                    tempBuff.ApplyEnemyStatusEffectsWhenAttacked(characterBuff.level, damageReceiverInfo, attacker);
                }
                foreach (KeyValuePair<BaseSkill, short> characterSkill in damageReceiver.GetCaches().Skills)
                {
                    tempSkill = characterSkill.Key;
                    if (!tempSkill.IsPassive)
                        continue;
                    tempSkill.Buff.ApplySelfStatusEffectsWhenAttacked(characterSkill.Value, damageReceiverInfo, damageReceiver);
                    tempSkill.Buff.ApplyEnemyStatusEffectsWhenAttacked(characterSkill.Value, damageReceiverInfo, attacker);
                }
            }
            // Decrease Shield Durability
            DecreaseEquipShieldsDurability(damageReceiver, decreaseShieldDurability);
            // Decrease Armor Durability
            DecreaseEquipItemsDurability(damageReceiver, decreaseArmorDurability);
        }

        private void ApplyStatusEffectsWhenAttacking(CharacterItem characterItem, IEquipmentItem equipmentItem, EntityInfo attackerInfo, BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver)
        {
            equipmentItem.ApplySelfStatusEffectsWhenAttacking(characterItem.level, attackerInfo, attacker);
            equipmentItem.ApplyEnemyStatusEffectsWhenAttacking(characterItem.level, attackerInfo, damageReceiver);
            if (characterItem.Sockets.Count > 0)
            {
                foreach (int socketItemDataId in characterItem.Sockets)
                {
                    ApplyStatusEffectsWhenAttacking(socketItemDataId, attackerInfo, attacker, damageReceiver);
                }
            }
        }

        private void ApplyStatusEffectsWhenAttacking(int socketItemDataId, EntityInfo attackerInfo, BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver)
        {
            if (!GameInstance.Items.ContainsKey(socketItemDataId))
                return;
            ISocketEnhancerItem tempSocketEnhancerItem = GameInstance.Items[socketItemDataId] as ISocketEnhancerItem;
            tempSocketEnhancerItem.ApplySelfStatusEffectsWhenAttacking(attackerInfo, attacker);
            tempSocketEnhancerItem.ApplyEnemyStatusEffectsWhenAttacking(attackerInfo, damageReceiver);
        }

        private void ApplyStatusEffectsWhenAttacked(CharacterItem characterItem, IEquipmentItem equipmentItem, EntityInfo damageReceiverInfo, BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver)
        {
            equipmentItem.ApplySelfStatusEffectsWhenAttacked(characterItem.level, damageReceiverInfo, damageReceiver);
            equipmentItem.ApplyEnemyStatusEffectsWhenAttacked(characterItem.level, damageReceiverInfo, attacker);
            if (characterItem.Sockets.Count > 0)
            {
                foreach (int socketItemDataId in characterItem.Sockets)
                {
                    ApplyStatusEffectsWhenAttacked(socketItemDataId, damageReceiverInfo, attacker, damageReceiver);
                }
            }
        }

        private void ApplyStatusEffectsWhenAttacked(int socketItemDataId, EntityInfo damageReceiverInfo, BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver)
        {
            if (!GameInstance.Items.ContainsKey(socketItemDataId))
                return;
            ISocketEnhancerItem tempSocketEnhancerItem = GameInstance.Items[socketItemDataId] as ISocketEnhancerItem;
            tempSocketEnhancerItem.ApplySelfStatusEffectsWhenAttacked(damageReceiverInfo, damageReceiver);
            tempSocketEnhancerItem.ApplyEnemyStatusEffectsWhenAttacked(damageReceiverInfo, attacker);
        }

        public override void OnHarvestableReceivedDamage(BaseCharacterEntity attacker, HarvestableEntity damageReceiver, CombatAmountType combatAmountType, int damage, CharacterItem weapon, BaseSkill skill, short skillLevel)
        {
            float decreaseWeaponDurability;
            float decreaseShieldDurability;
            float decreaseArmorDurability;
            GetDecreaseDurabilityAmount(combatAmountType, out decreaseWeaponDurability, out decreaseShieldDurability, out decreaseArmorDurability);
            if (attacker != null)
            {
                // Decrease Weapon Durability
                DecreaseEquipWeaponsDurability(attacker, decreaseWeaponDurability);
            }
        }

        private void GetDecreaseDurabilityAmount(CombatAmountType combatAmountType, out float decreaseWeaponDurability, out float decreaseShieldDurability, out float decreaseArmorDurability)
        {
            decreaseWeaponDurability = normalDecreaseWeaponDurability;
            decreaseShieldDurability = normalDecreaseShieldDurability;
            decreaseArmorDurability = normalDecreaseArmorDurability;
            switch (combatAmountType)
            {
                case CombatAmountType.BlockedDamage:
                    decreaseWeaponDurability = blockedDecreaseWeaponDurability;
                    decreaseShieldDurability = blockedDecreaseShieldDurability;
                    decreaseArmorDurability = blockedDecreaseArmorDurability;
                    break;
                case CombatAmountType.CriticalDamage:
                    decreaseWeaponDurability = criticalDecreaseWeaponDurability;
                    decreaseShieldDurability = criticalDecreaseShieldDurability;
                    decreaseArmorDurability = criticalDecreaseArmorDurability;
                    break;
                case CombatAmountType.Miss:
                    decreaseWeaponDurability = missDecreaseWeaponDurability;
                    decreaseShieldDurability = missDecreaseShieldDurability;
                    decreaseArmorDurability = missDecreaseArmorDurability;
                    break;
            }
        }

        private void DecreaseEquipWeaponsDurability(BaseCharacterEntity entity, float decreaseDurability)
        {
            bool tempDestroy;
            EquipWeapons equipWeapons = entity.EquipWeapons;
            CharacterItem rightHand = equipWeapons.rightHand;
            CharacterItem leftHand = equipWeapons.leftHand;
            if (rightHand.GetWeaponItem() != null && rightHand.GetMaxDurability() > 0)
            {
                rightHand = DecreaseDurability(rightHand, decreaseDurability, out tempDestroy);
                if (tempDestroy)
                    equipWeapons.rightHand = CharacterItem.Empty;
                else
                    equipWeapons.rightHand = rightHand;
            }
            if (leftHand.GetWeaponItem() != null && leftHand.GetMaxDurability() > 0)
            {
                leftHand = DecreaseDurability(leftHand, decreaseDurability, out tempDestroy);
                if (tempDestroy)
                    equipWeapons.leftHand = CharacterItem.Empty;
                else
                    equipWeapons.leftHand = leftHand;
            }
            entity.EquipWeapons = equipWeapons;
        }

        private void DecreaseEquipShieldsDurability(BaseCharacterEntity entity, float decreaseDurability)
        {
            bool tempDestroy;
            EquipWeapons equipWeapons = entity.EquipWeapons;
            CharacterItem rightHand = equipWeapons.rightHand;
            CharacterItem leftHand = equipWeapons.leftHand;
            if (rightHand.GetShieldItem() != null && rightHand.GetMaxDurability() > 0)
            {
                rightHand = DecreaseDurability(rightHand, decreaseDurability, out tempDestroy);
                if (tempDestroy)
                    equipWeapons.rightHand = CharacterItem.Empty;
                else
                    equipWeapons.rightHand = rightHand;
            }
            if (leftHand.GetShieldItem() != null && leftHand.GetMaxDurability() > 0)
            {
                leftHand = DecreaseDurability(leftHand, decreaseDurability, out tempDestroy);
                if (tempDestroy)
                    equipWeapons.leftHand = CharacterItem.Empty;
                else
                    equipWeapons.leftHand = leftHand;
            }
            entity.EquipWeapons = equipWeapons;
        }

        private void DecreaseEquipItemsDurability(BaseCharacterEntity entity, float decreaseDurability)
        {
            bool tempDestroy;
            int count = entity.EquipItems.Count;
            for (int i = count - 1; i >= 0; --i)
            {
                CharacterItem equipItem = entity.EquipItems[i];
                if (equipItem.GetMaxDurability() <= 0)
                    continue;
                equipItem = DecreaseDurability(equipItem, decreaseDurability, out tempDestroy);
                if (tempDestroy)
                    entity.EquipItems.RemoveAt(i);
                else
                    entity.EquipItems[i] = equipItem;
            }
        }

        private CharacterItem DecreaseDurability(CharacterItem characterItem, float decreaseDurability, out bool destroy)
        {
            destroy = false;
            IEquipmentItem item = characterItem.GetEquipmentItem();
            if (item != null)
            {
                if (characterItem.durability - decreaseDurability <= 0 && item.DestroyIfBroken)
                    destroy = true;
                characterItem.durability -= decreaseDurability;
                if (characterItem.durability < 0)
                    characterItem.durability = 0;
            }
            return characterItem;
        }

        public override bool CurrenciesEnoughToBuyItem(IPlayerCharacterData character, NpcSellItem sellItem, short amount)
        {
            if (character.Gold < sellItem.sellPrice * amount)
                return false;
            if (sellItem.sellPrices == null || sellItem.sellPrices.Length == 0)
                return true;
            return character.HasEnoughCurrencies(sellItem.sellPrices, amount);
        }

        public override void DecreaseCurrenciesWhenBuyItem(IPlayerCharacterData character, NpcSellItem sellItem, short amount)
        {
            character.Gold -= sellItem.sellPrice * amount;
            if (sellItem.sellPrices == null || sellItem.sellPrices.Length == 0)
                return;
            character.DecreaseCurrencies(sellItem.sellPrices, amount);
        }

        public override void IncreaseCurrenciesWhenSellItem(IPlayerCharacterData character, BaseItem item, short amount)
        {
            character.Gold = character.Gold.Increase(item.SellPrice * amount);
        }

        public override bool CurrenciesEnoughToRefineItem(IPlayerCharacterData character, ItemRefineLevel refineLevel)
        {
            return character.Gold >= refineLevel.RequireGold;
        }

        public override void DecreaseCurrenciesWhenRefineItem(IPlayerCharacterData character, ItemRefineLevel refineLevel)
        {
            character.Gold -= refineLevel.RequireGold;
        }

        public override bool CurrenciesEnoughToRepairItem(IPlayerCharacterData character, ItemRepairPrice repairPrice)
        {
            return character.Gold >= repairPrice.RequireGold;
        }

        public override void DecreaseCurrenciesWhenRepairItem(IPlayerCharacterData character, ItemRepairPrice repairPrice)
        {
            character.Gold -= repairPrice.RequireGold;
        }

        public override bool CurrenciesEnoughToCraftItem(IPlayerCharacterData character, ItemCraft itemCraft)
        {
            return character.Gold >= itemCraft.RequireGold;
        }

        public override void DecreaseCurrenciesWhenCraftItem(IPlayerCharacterData character, ItemCraft itemCraft)
        {
            character.Gold -= itemCraft.RequireGold;
        }

        public override bool CurrenciesEnoughToRemoveEnhancer(IPlayerCharacterData character)
        {
            return character.Gold >= GameInstance.Singleton.enhancerRemoval.RequireGold;
        }

        public override void DecreaseCurrenciesWhenRemoveEnhancer(IPlayerCharacterData character)
        {
            character.Gold -= GameInstance.Singleton.enhancerRemoval.RequireGold;
        }

        public override bool CurrenciesEnoughToCreateGuild(IPlayerCharacterData character, SocialSystemSetting setting)
        {
            return character.Gold >= setting.CreateGuildRequiredGold;
        }

        public override void DecreaseCurrenciesWhenCreateGuild(IPlayerCharacterData character, SocialSystemSetting setting)
        {
            character.Gold -= setting.CreateGuildRequiredGold;
        }

        public override Reward MakeMonsterReward(MonsterCharacter monster, short level)
        {
            Reward result = new Reward();
            result.exp = monster.RandomExp(level);
            result.gold = monster.RandomGold(level);
            return result;
        }

        public override Reward MakeQuestReward(Quest quest)
        {
            Reward result = new Reward();
            result.exp = quest.rewardExp;
            result.gold = quest.rewardGold;
            result.currencies = quest.rewardCurrencies;
            return result;
        }

        public override float GetRecoveryUpdateDuration()
        {
            return 1f;
        }

        public override void ApplyFallDamage(BaseCharacterEntity character, Vector3 lastGroundedPosition)
        {
            if (character.CacheTransform.position.y >= lastGroundedPosition.y)
                return;
            float dist = lastGroundedPosition.y - character.CacheTransform.position.y;
            if (dist < fallDamageMinDistance)
                return;
            int damage = Mathf.CeilToInt(character.MaxHp * (float)(dist - fallDamageMinDistance) / (float)(fallDamageMaxDistance - fallDamageMinDistance));
            if (damage < 0)
                damage = 0;
            character.CurrentHp -= damage;
            character.ReceivedDamage(character.CacheTransform.position, EntityInfo.Empty, null, CombatAmountType.NormalDamage, damage, null, null, 0);
            if (character.IsDead())
            {
                // Dead by itself, so instigator is itself
                character.ValidateRecovery(character.GetInfo());
            }
        }

        public override bool CanInteractEntity(BaseCharacterEntity character, uint objectId)
        {
            if (!character.CanDoActions())
                return false;
            BaseGameEntity interactingEntity;
            if (!character.Manager.Assets.TryGetSpawnedObject(objectId, out interactingEntity))
                return false;
            // This function will sort: near to far, so loop from 0
            float dist = Vector3.Distance(character.CacheTransform.position, interactingEntity.CacheTransform.position);
            Vector3 dir = (interactingEntity.CacheTransform.position - character.CacheTransform.position).normalized;
            if (interactingEntity is ICraftingQueueSource)
            {
                if (!(interactingEntity as ICraftingQueueSource).IsInCraftDistance(character.CacheTransform.position))
                    return false;
            }
            // Find that the entity is behind the wall or not
            int count = character.FindPhysicFunctions.Raycast(character.MeleeDamageTransform.position, dir, dist, GameInstance.Singleton.buildingLayer.Mask, QueryTriggerInteraction.Ignore);
            IGameEntity gameEntity;
            for (int i = 0; i < count; ++i)
            {
                gameEntity = character.FindPhysicFunctions.GetRaycastObject(i).GetComponent<IGameEntity>();
                if (gameEntity == null) continue;
                if (gameEntity.GetObjectId() == objectId)
                {
                    // It's target entity, so interact it
                    return true;
                }
                if (gameEntity.Entity is BuildingEntity)
                {
                    // Cannot interact object behind the wall
                    return false;
                }
            }
            // Not hit anything, assume that it can interact
            return true;
        }

        public override Vector3 GetSummonPosition(BaseCharacterEntity character)
        {
            float halfDist = (GameInstance.Singleton.minSummonDistance + GameInstance.Singleton.maxSummonDistance) * 0.5f;
            Vector2 randomCircle = Random.insideUnitCircle * halfDist;
            // X
            if (randomCircle.x < 0 && Mathf.Abs(randomCircle.x) < halfDist)
                randomCircle.x = -halfDist;
            else if (randomCircle.x > 0 && randomCircle.x < halfDist)
                randomCircle.x = halfDist;
            // Y
            if (randomCircle.y < 0 && Mathf.Abs(randomCircle.y) < halfDist)
                randomCircle.y = -halfDist;
            else if (randomCircle.y > 0 && randomCircle.y < halfDist)
                randomCircle.y = halfDist;
            if (GameInstance.Singleton.DimensionType == DimensionType.Dimension2D)
                return character.CacheTransform.position + new Vector3(randomCircle.x, randomCircle.y, 0f);
            return character.CacheTransform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
        }

        public override Quaternion GetSummonRotation(BaseCharacterEntity character)
        {
            if (GameInstance.Singleton.DimensionType == DimensionType.Dimension2D)
                return Quaternion.identity;
            return character.CacheTransform.rotation;
        }
    }
}
