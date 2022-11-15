﻿using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = "Simple Resurrection Skill", menuName = "Create GameData/Skill/Simple Resurrection Skill", order = -4986)]
    public class SimpleResurrectionSkill : BaseSkill
    {
        [Category("Buff")]
        public IncrementalFloat buffDistance;
        public Buff buff;
        [Range(0.01f, 1f)]
        public float resurrectHpRate = 0.1f;
        [Range(0.01f, 1f)]
        public float resurrectMpRate = 0.1f;
        [Range(0.01f, 1f)]
        public float resurrectStaminaRate = 0.1f;
        [Range(0.01f, 1f)]
        public float resurrectFoodRate = 0.1f;
        [Range(0.01f, 1f)]
        public float resurrectWaterRate = 0.1f;

        protected override void ApplySkillImplement(BaseCharacterEntity skillUser, short skillLevel, bool isLeftHand, CharacterItem weapon, int hitIndex, Dictionary<DamageElement, MinMaxFloat> damageAmounts, uint targetObjectId, AimPosition aimPosition, int randomSeed, long? time)
        {
            // Resurrect target
            BasePlayerCharacterEntity targetEntity;
            if (!skillUser.CurrentGameManager.TryGetEntityByObjectId(targetObjectId, out targetEntity) || !targetEntity.IsDead())
                return;
            
            targetEntity.CurrentHp = Mathf.CeilToInt(targetEntity.GetCaches().MaxHp * resurrectHpRate);
            targetEntity.CurrentMp = Mathf.CeilToInt(targetEntity.GetCaches().MaxMp * resurrectMpRate);
            targetEntity.CurrentStamina = Mathf.CeilToInt(targetEntity.GetCaches().MaxStamina * resurrectStaminaRate);
            targetEntity.CurrentFood = Mathf.CeilToInt(targetEntity.GetCaches().MaxFood * resurrectFoodRate);
            targetEntity.CurrentWater = Mathf.CeilToInt(targetEntity.GetCaches().MaxWater * resurrectWaterRate);
            targetEntity.StopMove();
            targetEntity.CallAllOnRespawn();
            targetEntity.ApplyBuff(DataId, BuffType.SkillBuff, skillLevel, skillUser.GetInfo());
        }

        public override Dictionary<DamageElement, MinMaxFloat> GetAttackAdditionalDamageAmounts(ICharacterData skillUser, short skillLevel)
        {
            return new Dictionary<DamageElement, MinMaxFloat>();
        }

        public override float GetCastDistance(BaseCharacterEntity skillUser, short skillLevel, bool isLeftHand)
        {
            return buffDistance.GetAmount(skillLevel);
        }

        public override float GetCastFov(BaseCharacterEntity skillUser, short skillLevel, bool isLeftHand)
        {
            return 360f;
        }

        public override Dictionary<DamageElement, float> GetAttackWeaponDamageInflictions(ICharacterData skillUser, short skillLevel)
        {
            return new Dictionary<DamageElement, float>();
        }

        public override KeyValuePair<DamageElement, MinMaxFloat> GetBaseAttackDamageAmount(ICharacterData skillUser, short skillLevel, bool isLeftHand)
        {
            return new KeyValuePair<DamageElement, MinMaxFloat>();
        }

        public override SkillType SkillType
        {
            get { return SkillType.Active; }
        }

        public override bool IsBuff
        {
            get { return true; }
        }

        public override bool RequiredTarget
        {
            get { return true; }
        }

        public override Buff Buff
        {
            get { return buff; }
        }

        public override bool CanUse(BaseCharacterEntity character, short level, bool isLeftHand, uint targetObjectId, out UITextKeys gameMessage, bool isItem = false)
        {
            if (!base.CanUse(character, level, isLeftHand, targetObjectId, out gameMessage, isItem))
                return false;
            
            BasePlayerCharacterEntity targetEntity;
            if (!character.CurrentGameManager.TryGetEntityByObjectId(targetObjectId, out targetEntity) || !targetEntity.IsDead())
                return false;

            return true;
        }
    }
}
