﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MultiplayerARPG
{
    public class ThrowableDamageEntity : BaseDamageEntity
    {
        public float destroyDelay;
        public UnityEvent onExploded;
        public UnityEvent onDestroy;
        public float explodeDistance;

        protected float throwForce;
        protected float lifetime;
        protected bool isExploded;

        public Rigidbody CacheRigidbody { get; private set; }
        public Rigidbody2D CacheRigidbody2D { get; private set; }

        protected float throwedTime;
        protected bool destroying;

        protected override void Awake()
        {
            base.Awake();
            gameObject.layer = PhysicLayers.IgnoreRaycast;
            CacheRigidbody = GetComponent<Rigidbody>();
            CacheRigidbody2D = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// Setup this component data
        /// </summary>
        /// <param name="instigator">Weapon's or skill's instigator who to spawn this to attack enemy</param>
        /// <param name="weapon">Weapon which was used to attack enemy</param>
        /// <param name="damageAmounts">Calculated damage amounts</param>
        /// <param name="skill">Skill which was used to attack enemy</param>
        /// <param name="skillLevel">Level of the skill</param>
        /// <param name="throwForce">Calculated throw force</param>
        /// <param name="lifetime">Calculated life time</param>
        public virtual void Setup(
            EntityInfo instigator,
            CharacterItem weapon,
            Dictionary<DamageElement, MinMaxFloat> damageAmounts,
            BaseSkill skill,
            short skillLevel,
            float throwForce,
            float lifetime)
        {
            Setup(instigator, weapon, damageAmounts, skill, skillLevel);
            this.throwForce = throwForce;
            this.lifetime = lifetime;

            if (lifetime <= 0)
            {
                // Explode immediately when lifetime is 0
                Explode();
                PushBack(destroyDelay);
                destroying = true;
                return;
            }
            isExploded = false;
            destroying = false;
            throwedTime = Time.unscaledTime;
            if (CurrentGameInstance.DimensionType == DimensionType.Dimension2D)
            {
                CacheRigidbody2D.velocity = Vector2.zero;
                CacheRigidbody2D.angularVelocity = 0f;
                CacheRigidbody2D.AddForce(CacheTransform.forward * throwForce, ForceMode2D.Impulse);
            }
            else
            {
                CacheRigidbody.velocity = Vector3.zero;
                CacheRigidbody.angularVelocity = Vector3.zero;
                CacheRigidbody.AddForce(CacheTransform.forward * throwForce, ForceMode.Impulse);
            }
        }

        protected virtual void Update()
        {
            if (destroying)
                return;

            if (Time.unscaledTime - throwedTime >= lifetime)
            {
                Explode();
                PushBack(destroyDelay);
                destroying = true;
            }
        }

        protected override void OnPushBack()
        {
            if (onDestroy != null)
                onDestroy.Invoke();
            base.OnPushBack();
        }

        protected virtual bool FindTargetHitBox(GameObject other, out DamageableHitBox target)
        {
            target = null;

            if (other.GetComponent<IUnHittable>() != null)
                return false;

            target = other.GetComponent<DamageableHitBox>();

            if (target == null || target.IsDead() || !target.CanReceiveDamageFrom(instigator))
                return false;

            return true;
        }

        protected virtual bool FindAndApplyDamage(GameObject other)
        {
            DamageableHitBox target;
            if (FindTargetHitBox(other, out target))
            {
                ApplyDamageTo(target);
                return true;
            }
            return false;
        }

        protected virtual void Explode()
        {
            if (isExploded)
                return;

            isExploded = true;

            if (onExploded != null)
                onExploded.Invoke();

            if (!IsServer)
                return;

            ExplodeApplyDamage();
        }

        protected virtual void ExplodeApplyDamage()
        {
            if (CurrentGameInstance.DimensionType == DimensionType.Dimension2D)
            {
                Collider2D[] colliders2D = Physics2D.OverlapCircleAll(CacheTransform.position, explodeDistance);
                foreach (Collider2D collider in colliders2D)
                {
                    FindAndApplyDamage(collider.gameObject);
                }
            }
            else
            {
                Collider[] colliders = Physics.OverlapSphere(CacheTransform.position, explodeDistance);
                foreach (Collider collider in colliders)
                {
                    FindAndApplyDamage(collider.gameObject);
                }
            }
        }
    }
}
