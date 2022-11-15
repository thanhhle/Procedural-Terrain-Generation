﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [RequireComponent(typeof(PlayerCharacterBuildingComponent))]
    [RequireComponent(typeof(PlayerCharacterCraftingComponent))]
    [RequireComponent(typeof(PlayerCharacterDealingComponent))]
    [RequireComponent(typeof(PlayerCharacterNpcActionComponent))]
    public abstract partial class BasePlayerCharacterEntity : BaseCharacterEntity, IPlayerCharacterData
    {
        [Category("Character Settings")]
        [Tooltip("This is list which used as choice of character classes when create character")]
        [SerializeField]
        [FormerlySerializedAs("playerCharacters")]
        protected PlayerCharacter[] characterDatabases;
        [Tooltip("Leave this empty to use GameInstance's controller prefab")]
        [SerializeField]
        protected BasePlayerCharacterController controllerPrefab;

        public PlayerCharacter[] CharacterDatabases
        {
            get { return characterDatabases; }
            set { characterDatabases = value; }
        }

        public BasePlayerCharacterController ControllerPrefab
        {
            get { return controllerPrefab; }
        }

        public PlayerCharacterBuildingComponent Building
        {
            get; private set;
        }

        public PlayerCharacterCraftingComponent Crafting
        {
            get; private set;
        }

        public PlayerCharacterDealingComponent Dealing
        {
            get; private set;
        }

        public PlayerCharacterNpcActionComponent NpcAction
        {
            get; private set;
        }

        public int IndexOfCharacterDatabase(int dataId)
        {
            for (int i = 0; i < CharacterDatabases.Length; ++i)
            {
                if (CharacterDatabases[i] != null && CharacterDatabases[i].DataId == dataId)
                    return i;
            }
            return -1;
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            GameInstance.AddCharacters(CharacterDatabases);
        }

        public override EntityInfo GetInfo()
        {
            return new EntityInfo(
                EntityTypes.Player,
                ObjectId,
                Id,
                DataId,
                FactionId,
                PartyId,
                GuildId,
                IsInSafeArea);
        }

        protected override void EntityAwake()
        {
            base.EntityAwake();
            gameObject.tag = CurrentGameInstance.playerTag;
            gameObject.layer = CurrentGameInstance.playerLayer;
        }

        public override void InitialRequiredComponents()
        {
            base.InitialRequiredComponents();
            Building = gameObject.GetOrAddComponent<PlayerCharacterBuildingComponent>();
            Crafting = gameObject.GetOrAddComponent<PlayerCharacterCraftingComponent>();
            Dealing = gameObject.GetOrAddComponent<PlayerCharacterDealingComponent>();
            NpcAction = gameObject.GetOrAddComponent<PlayerCharacterNpcActionComponent>();
            gameObject.GetOrAddComponent<PlayerCharacterItemLockAndExpireComponent>();
        }

        protected override void EntityUpdate()
        {
            Profiler.BeginSample("BasePlayerCharacterEntity - Update");
            base.EntityUpdate();
            if (this.IsDead())
            {
                StopMove();
                SetTargetEntity(null);
                return;
            }
            Profiler.EndSample();
        }

        public override bool CanDoActions()
        {
            return base.CanDoActions() && Dealing.DealingState == DealingState.None;
        }

        public override void NotifyEnemySpotted(BaseCharacterEntity ally, BaseCharacterEntity attacker)
        {
            // TODO: May send data to client
        }
    }
}