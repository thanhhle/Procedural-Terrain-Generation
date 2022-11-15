﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = "Skill", menuName = "Create GameData/Skill/Skill", order = -4989)]
    public partial class Skill : BaseSkill
    {
        public enum SkillAttackType : byte
        {
            None,
            Normal,
            BasedOnWeapon,
        }

        public enum SkillBuffType : byte
        {
            None,
            BuffToUser,
            BuffToNearbyAllies,
            BuffToNearbyCharacters,
            BuffToTarget,
            Toggle,
        }

        [Category("Skill Settings")]
        public SkillType skillType;

        [Category(3, "Attacking")]
        public SkillAttackType skillAttackType;
        public DamageInfo damageInfo;
        public DamageIncremental damageAmount;
        public DamageEffectivenessAttribute[] effectivenessAttributes;
        public DamageInflictionIncremental[] weaponDamageInflictions;
        public DamageIncremental[] additionalDamageAmounts;
        [FormerlySerializedAs("increaseDamageWithBuffs")]
        public bool increaseDamageAmountsWithBuffs;
        public bool isDebuff;
        public Buff debuff;
        public HarvestType harvestType;
        public IncrementalMinMaxFloat harvestDamageAmount;

        [Category(4, "Buff")]
        public SkillBuffType skillBuffType;
        public IncrementalFloat buffDistance;
        public Buff buff;

        [Category(5, "Summon/Mount/Item Craft")]
        public SkillSummon summon;
        public SkillMount mount;
        public ItemCraft itemCraft;

        [System.NonSerialized]
        private Dictionary<Attribute, float> cacheEffectivenessAttributes;
        public Dictionary<Attribute, float> CacheEffectivenessAttributes
        {
            get
            {
                if (cacheEffectivenessAttributes == null)
                    cacheEffectivenessAttributes = GameDataHelpers.CombineDamageEffectivenessAttributes(effectivenessAttributes, new Dictionary<Attribute, float>());
                return cacheEffectivenessAttributes;
            }
        }

        protected override void ApplySkillImplement(BaseCharacterEntity skillUser, short skillLevel, bool isLeftHand, CharacterItem weapon, int hitIndex, Dictionary<DamageElement, MinMaxFloat> damageAmounts, uint targetObjectId, AimPosition aimPosition, int randomSeed, long? time)
        {
            // Craft item
            if (skillType == SkillType.CraftItem &&
                skillUser is BasePlayerCharacterEntity)
            {
                // Apply craft skill at server only
                if (!skillUser.IsServer)
                    return;
                BasePlayerCharacterEntity castedCharacter = skillUser as BasePlayerCharacterEntity;
                UITextKeys gameMessage;
                if (!itemCraft.CanCraft(castedCharacter, out gameMessage))
                {
                    GameInstance.ServerGameMessageHandlers.SendGameMessage(skillUser.ConnectionId, gameMessage);
                    return;
                }
                itemCraft.CraftItem(castedCharacter);
                return;
            }

            // Apply skills only when it's active skill
            if (skillType != SkillType.Active)
                return;

            // Apply buff, summons at server only
            if (skillUser.IsServer)
            {
                ApplySkillBuff(skillUser, skillLevel, targetObjectId);
                ApplySkillSummon(skillUser, skillLevel);
                ApplySkillMount(skillUser, skillLevel);
            }

            // Apply attack skill
            if (IsAttack)
            {
                // Launch damage entity to apply damage to other characters
                GetDamageInfo(skillUser, isLeftHand).LaunchDamageEntity(
                    skillUser,
                    isLeftHand,
                    weapon,
                    damageAmounts,
                    this,
                    skillLevel,
                    randomSeed,
                    aimPosition,
                    Vector3.zero,
                    out _,
                    time);
            }
        }

        protected void ApplySkillBuff(BaseCharacterEntity skillUser, short skillLevel, uint targetObjectId)
        {
            if (skillUser.IsDead() || !skillUser.IsServer || skillLevel <= 0)
                return;

            EntityInfo instigator = skillUser.GetInfo();
            List<BaseCharacterEntity> tempCharacters;
            switch (skillBuffType)
            {
                case SkillBuffType.BuffToUser:
                    skillUser.ApplyBuff(DataId, BuffType.SkillBuff, skillLevel, instigator);
                    break;
                case SkillBuffType.BuffToNearbyAllies:
                    tempCharacters = skillUser.FindAliveCharacters<BaseCharacterEntity>(buffDistance.GetAmount(skillLevel), true, false, false);
                    foreach (BaseCharacterEntity applyBuffCharacter in tempCharacters)
                    {
                        applyBuffCharacter.ApplyBuff(DataId, BuffType.SkillBuff, skillLevel, instigator);
                    }
                    skillUser.ApplyBuff(DataId, BuffType.SkillBuff, skillLevel, instigator);
                    break;
                case SkillBuffType.BuffToNearbyCharacters:
                    tempCharacters = skillUser.FindAliveCharacters<BaseCharacterEntity>(buffDistance.GetAmount(skillLevel), true, false, true);
                    foreach (BaseCharacterEntity applyBuffCharacter in tempCharacters)
                    {
                        applyBuffCharacter.ApplyBuff(DataId, BuffType.SkillBuff, skillLevel, instigator);
                    }
                    skillUser.ApplyBuff(DataId, BuffType.SkillBuff, skillLevel, instigator);
                    break;
                case SkillBuffType.BuffToTarget:
                    BaseCharacterEntity targetEntity;
                    if (!skillUser.CurrentGameManager.TryGetEntityByObjectId(targetObjectId, out targetEntity))
                        targetEntity = skillUser;
                    if (!targetEntity.IsDead())
                        targetEntity.ApplyBuff(DataId, BuffType.SkillBuff, skillLevel, instigator);
                    break;
                case SkillBuffType.Toggle:
                    int indexOfBuff = skillUser.IndexOfBuff(DataId, BuffType.SkillBuff);
                    if (indexOfBuff >= 0)
                        skillUser.Buffs.RemoveAt(indexOfBuff);
                    else
                        skillUser.ApplyBuff(DataId, BuffType.SkillBuff, skillLevel, instigator);
                    break;
            }
        }

        protected void ApplySkillSummon(BaseCharacterEntity skillUser, short skillLevel)
        {
            if (skillUser.IsDead() || !skillUser.IsServer || skillLevel <= 0)
                return;
            int i = 0;
            int amountEachTime = summon.AmountEachTime.GetAmount(skillLevel);
            for (i = 0; i < amountEachTime; ++i)
            {
                CharacterSummon newSummon = CharacterSummon.Create(SummonType.Skill, DataId);
                newSummon.Summon(skillUser, summon.Level.GetAmount(skillLevel), summon.Duration.GetAmount(skillLevel));
                skillUser.Summons.Add(newSummon);
            }
            int count = 0;
            for (i = 0; i < skillUser.Summons.Count; ++i)
            {
                if (skillUser.Summons[i].dataId == DataId)
                    ++count;
            }
            int maxStack = summon.MaxStack.GetAmount(skillLevel);
            int unSummonAmount = count > maxStack ? count - maxStack : 0;
            CharacterSummon tempSummon;
            for (i = unSummonAmount; i > 0; --i)
            {
                int summonIndex = skillUser.IndexOfSummon(DataId, SummonType.Skill);
                tempSummon = skillUser.Summons[summonIndex];
                if (summonIndex >= 0)
                {
                    skillUser.Summons.RemoveAt(summonIndex);
                    tempSummon.UnSummon(skillUser);
                }
            }
        }

        protected void ApplySkillMount(BaseCharacterEntity skillUser, short skillLevel)
        {
            if (skillUser.IsDead() || !skillUser.IsServer || skillLevel <= 0)
                return;

            skillUser.Mount(mount.MountEntity);
        }

        protected DamageInfo GetDamageInfo(BaseCharacterEntity skillUser, bool isLeftHand)
        {
            switch (skillAttackType)
            {
                case SkillAttackType.Normal:
                    return damageInfo;
                case SkillAttackType.BasedOnWeapon:
                    return skillUser.GetWeaponDamageInfo(ref isLeftHand);
            }
            return default(DamageInfo);
        }

        public override SkillType SkillType
        {
            get { return skillType; }
        }

        public override bool IsAttack
        {
            get { return skillAttackType != SkillAttackType.None; }
        }

        public override bool IsBuff
        {
            get { return skillType == SkillType.Passive || skillBuffType != SkillBuffType.None; }
        }

        public override bool IsDebuff
        {
            get { return IsAttack && isDebuff; }
        }

        public override float GetCastDistance(BaseCharacterEntity skillUser, short skillLevel, bool isLeftHand)
        {
            if (!IsAttack)
                return buffDistance.GetAmount(skillLevel);
            if (skillAttackType == SkillAttackType.Normal)
                return GetDamageInfo(skillUser, isLeftHand).GetDistance();
            return skillUser.GetAttackDistance(isLeftHand);
        }

        public override float GetCastFov(BaseCharacterEntity skillUser, short skillLevel, bool isLeftHand)
        {
            if (!IsAttack)
                return 360f;
            if (skillAttackType == SkillAttackType.Normal)
                return GetDamageInfo(skillUser, isLeftHand).GetFov();
            return skillUser.GetAttackFov(isLeftHand);
        }

        public override KeyValuePair<DamageElement, MinMaxFloat> GetBaseAttackDamageAmount(ICharacterData skillUser, short skillLevel, bool isLeftHand)
        {
            switch (skillAttackType)
            {
                case SkillAttackType.Normal:
                    return damageAmount.ToKeyValuePair(skillLevel, 1f, GetEffectivenessDamage(skillUser));
                case SkillAttackType.BasedOnWeapon:
                    return skillUser.GetWeaponDamages(ref isLeftHand);
            }
            return new KeyValuePair<DamageElement, MinMaxFloat>();
        }

        public override Dictionary<DamageElement, float> GetAttackWeaponDamageInflictions(ICharacterData skillUser, short skillLevel)
        {
            if (!IsAttack)
                return new Dictionary<DamageElement, float>();
            return GameDataHelpers.CombineDamageInflictions(weaponDamageInflictions, new Dictionary<DamageElement, float>(), skillLevel);
        }

        public override Dictionary<DamageElement, MinMaxFloat> GetAttackAdditionalDamageAmounts(ICharacterData skillUser, short skillLevel)
        {
            if (!IsAttack)
                return new Dictionary<DamageElement, MinMaxFloat>();
            return GameDataHelpers.CombineDamages(additionalDamageAmounts, new Dictionary<DamageElement, MinMaxFloat>(), skillLevel, 1f);
        }

        public override bool IsIncreaseAttackDamageAmountsWithBuffs(ICharacterData skillUser, short skillLevel)
        {
            return increaseDamageAmountsWithBuffs;
        }

        public override HarvestType GetHarvestType()
        {
            return harvestType;
        }

        public override IncrementalMinMaxFloat GetHarvestDamageAmount()
        {
            return harvestDamageAmount;
        }

        protected float GetEffectivenessDamage(ICharacterData skillUser)
        {
            return GameDataHelpers.GetEffectivenessDamage(CacheEffectivenessAttributes, skillUser);
        }

        public override Buff Buff
        {
            get
            {
                if (!IsBuff)
                    return Buff.Empty;
                return buff;
            }
        }

        public override Buff Debuff
        {
            get
            {
                if (!IsDebuff)
                    return Buff.Empty;
                return debuff;
            }
        }

        public override SkillSummon Summon
        {
            get { return summon; }
        }

        public override SkillMount Mount
        {
            get { return mount; }
        }

        public override ItemCraft ItemCraft
        {
            get { return itemCraft; }
        }

        public override bool RequiredTarget
        {
            get { return skillBuffType == SkillBuffType.BuffToTarget; }
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            GameInstance.AddCharacterEntities(summon.MonsterEntity);
            GameInstance.AddVehicleEntities(mount.MountEntity);
            GameInstance.AddItems(itemCraft.CraftingItem);
            GameInstance.AddItems(itemCraft.CacheCraftRequirements.Keys);
            damageInfo.PrepareRelatesData();
        }

        public override Transform GetApplyTransform(BaseCharacterEntity skillUser, bool isLeftHand)
        {
            if (IsAttack)
                return GetDamageInfo(skillUser, isLeftHand).GetDamageTransform(skillUser, isLeftHand);
            return base.GetApplyTransform(skillUser, isLeftHand);
        }

        public override bool CanUse(BaseCharacterEntity character, short level, bool isLeftHand, uint targetObjectId, out UITextKeys gameMessage, bool isItem = false)
        {
            bool canUse = base.CanUse(character, level, isLeftHand, targetObjectId, out gameMessage, isItem);
            if (!canUse && gameMessage == UITextKeys.UI_ERROR_NO_SKILL_TARGET)
            {
                // Still allow to use skill but it's going to set applies target to skill user
                gameMessage = UITextKeys.NONE;
                return true;
            }
            return canUse;
        }
    }
}
