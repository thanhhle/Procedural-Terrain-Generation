﻿using System.Collections.Generic;
using System.Text;
using LiteNetLib.Utils;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Flags]
    internal enum CharacterItemSyncState : byte
    {
        None = 0,
        IsEquipment = 1 << 1,
        IsWeapon = 1 << 2,
        IsPet = 1 << 3,
        IsEmpty = 1 << 4,
    }

    internal static class CharacterItemSyncStateExtensions
    {
        internal static bool Has(this CharacterItemSyncState self, CharacterItemSyncState flag)
        {
            return (self & flag) == flag;
        }
    }

    [System.Serializable]
    public class CharacterItem : INetSerializable
    {
        public static readonly CharacterItem Empty = new CharacterItem();
        public string id;
        public int dataId;
        public short level;
        public short amount;
        public byte equipSlotIndex;
        public float durability;
        public int exp;
        public float lockRemainsDuration;
        public long expireTime;
        public short randomSeed;
        public short ammo;
        public List<int> sockets = new List<int>();

        [System.NonSerialized]
        private int dirtyDataId;

        [System.NonSerialized]
        private BaseItem cacheItem;
        [System.NonSerialized]
        private IUsableItem cacheUsableItem;
        [System.NonSerialized]
        private IEquipmentItem cacheEquipmentItem;
        [System.NonSerialized]
        private IDefendEquipmentItem cacheDefendItem;
        [System.NonSerialized]
        private IArmorItem cacheArmorItem;
        [System.NonSerialized]
        private IWeaponItem cacheWeaponItem;
        [System.NonSerialized]
        private IShieldItem cacheShieldItem;
        [System.NonSerialized]
        private IPotionItem cachePotionItem;
        [System.NonSerialized]
        private IAmmoItem cacheAmmoItem;
        [System.NonSerialized]
        private IBuildingItem cacheBuildingItem;
        [System.NonSerialized]
        private IPetItem cachePetItem;
        [System.NonSerialized]
        private ISocketEnhancerItem cacheSocketEnhancerItem;
        [System.NonSerialized]
        private IMountItem cacheMountItem;
        [System.NonSerialized]
        private ISkillItem cacheSkillItem;

        [System.NonSerialized]
        private readonly StringBuilder stringBuilder = new StringBuilder();

        public List<int> Sockets
        {
            get
            {
                if (sockets == null)
                    sockets = new List<int>();
                return sockets;
            }
        }

        private void MakeCache()
        {
            if (dirtyDataId != dataId)
            {
                dirtyDataId = dataId;
                cacheItem = null;
                cacheUsableItem = null;
                cacheEquipmentItem = null;
                cacheDefendItem = null;
                cacheArmorItem = null;
                cacheWeaponItem = null;
                cacheShieldItem = null;
                cachePotionItem = null;
                cacheAmmoItem = null;
                cacheBuildingItem = null;
                cachePetItem = null;
                cacheSocketEnhancerItem = null;
                cacheMountItem = null;
                cacheSkillItem = null;

                if (GameInstance.Items.TryGetValue(dataId, out cacheItem) && cacheItem != null)
                {
                    if (cacheItem.IsUsable())
                        cacheUsableItem = cacheItem as IUsableItem;
                    if (cacheItem.IsEquipment())
                        cacheEquipmentItem = cacheItem as IEquipmentItem;
                    if (cacheItem.IsDefendEquipment())
                        cacheDefendItem = cacheItem as IDefendEquipmentItem;
                    if (cacheItem.IsArmor())
                        cacheArmorItem = cacheItem as IArmorItem;
                    if (cacheItem.IsWeapon())
                        cacheWeaponItem = cacheItem as IWeaponItem;
                    if (cacheItem.IsShield())
                        cacheShieldItem = cacheItem as IShieldItem;
                    if (cacheItem.IsPotion())
                        cachePotionItem = cacheItem as IPotionItem;
                    if (cacheItem.IsAmmo())
                        cacheAmmoItem = cacheItem as IAmmoItem;
                    if (cacheItem.IsBuilding())
                        cacheBuildingItem = cacheItem as IBuildingItem;
                    if (cacheItem.IsPet())
                        cachePetItem = cacheItem as IPetItem;
                    if (cacheItem.IsSocketEnhancer())
                        cacheSocketEnhancerItem = cacheItem as ISocketEnhancerItem;
                    if (cacheItem.IsMount())
                        cacheMountItem = cacheItem as IMountItem;
                    if (cacheItem.IsSkill())
                        cacheSkillItem = cacheItem as ISkillItem;
                }
            }
        }

        public BaseItem GetItem()
        {
            MakeCache();
            return cacheItem;
        }

        public IUsableItem GetUsableItem()
        {
            MakeCache();
            return cacheUsableItem;
        }

        public IEquipmentItem GetEquipmentItem()
        {
            MakeCache();
            return cacheEquipmentItem;
        }

        public IDefendEquipmentItem GetDefendItem()
        {
            MakeCache();
            return cacheDefendItem;
        }

        public IArmorItem GetArmorItem()
        {
            MakeCache();
            return cacheArmorItem;
        }

        public IWeaponItem GetWeaponItem()
        {
            MakeCache();
            return cacheWeaponItem;
        }

        public IShieldItem GetShieldItem()
        {
            MakeCache();
            return cacheShieldItem;
        }

        public IPotionItem GetPotionItem()
        {
            MakeCache();
            return cachePotionItem;
        }

        public IAmmoItem GetAmmoItem()
        {
            MakeCache();
            return cacheAmmoItem;
        }

        public IBuildingItem GetBuildingItem()
        {
            MakeCache();
            return cacheBuildingItem;
        }

        public IPetItem GetPetItem()
        {
            MakeCache();
            return cachePetItem;
        }

        public ISocketEnhancerItem GetSocketEnhancerItem()
        {
            MakeCache();
            return cacheSocketEnhancerItem;
        }

        public IMountItem GetMountItem()
        {
            MakeCache();
            return cacheMountItem;
        }

        public ISkillItem GetSkillItem()
        {
            MakeCache();
            return cacheSkillItem;
        }

        public short GetMaxStack()
        {
            return GetItem() == null ? (short)0 : GetItem().MaxStack;
        }

        public float GetMaxDurability()
        {
            return GetEquipmentItem() == null ? 0f : GetEquipmentItem().MaxDurability;
        }

        public bool IsFull()
        {
            return amount == GetMaxStack();
        }

        public bool IsBroken()
        {
            return GetMaxDurability() > 0 && durability <= 0;
        }

        public bool IsLock()
        {
            return lockRemainsDuration > 0;
        }

        public bool IsAmmoEmpty()
        {
            IWeaponItem item = GetWeaponItem();
            if (item != null)
            {
                if (item.AmmoCapacity > 0)
                    return ammo == 0;
            }
            return false;
        }

        public bool IsAmmoFull()
        {
            IWeaponItem item = GetWeaponItem();
            if (item != null)
            {
                if (item.AmmoCapacity > 0)
                    return ammo >= item.AmmoCapacity;
            }
            return true;
        }

        public void Lock(float duration)
        {
            lockRemainsDuration = duration;
        }

        public bool ShouldRemove(long currentTime)
        {
            return expireTime > 0 && expireTime < currentTime;
        }

        public void Update(float deltaTime)
        {
            lockRemainsDuration -= deltaTime;
        }

        public float GetEquipmentStatsRate()
        {
            return GameInstance.Singleton.GameplayRule.GetEquipmentStatsRate(this);
        }

        public int GetNextLevelExp()
        {
            if (GetPetItem() == null || level <= 0)
                return 0;
            int[] expTree = GameInstance.Singleton.ExpTree;
            if (level > expTree.Length)
                return 0;
            return expTree[level - 1];
        }

        public KeyValuePair<DamageElement, float> GetArmorAmount()
        {
            if (GetDefendItem() == null)
                return new KeyValuePair<DamageElement, float>();
            return GetDefendItem().GetArmorAmount(level, GetEquipmentStatsRate());
        }

        public KeyValuePair<DamageElement, MinMaxFloat> GetDamageAmount(ICharacterData characterData)
        {
            if (GetWeaponItem() == null)
                return new KeyValuePair<DamageElement, MinMaxFloat>();
            return GetWeaponItem().GetDamageAmount(level, GetEquipmentStatsRate(), characterData);
        }

        public CharacterStats GetIncreaseStats()
        {
            if (GetEquipmentItem() == null)
                return CharacterStats.Empty;
            return GetEquipmentItem().GetIncreaseStats(level, randomSeed);
        }

        public CharacterStats GetIncreaseStatsRate()
        {
            if (GetEquipmentItem() == null)
                return CharacterStats.Empty;
            return GetEquipmentItem().GetIncreaseStatsRate(level, randomSeed);
        }

        public Dictionary<Attribute, float> GetIncreaseAttributes()
        {
            if (GetEquipmentItem() == null)
                return null;
            return GetEquipmentItem().GetIncreaseAttributes(level, randomSeed);
        }

        public Dictionary<Attribute, float> GetIncreaseAttributesRate()
        {
            if (GetEquipmentItem() == null)
                return null;
            return GetEquipmentItem().GetIncreaseAttributesRate(level, randomSeed);
        }

        public Dictionary<DamageElement, float> GetIncreaseResistances()
        {
            if (GetEquipmentItem() == null)
                return null;
            return GetEquipmentItem().GetIncreaseResistances(level, randomSeed);
        }

        public Dictionary<DamageElement, float> GetIncreaseArmors()
        {
            if (GetEquipmentItem() == null)
                return null;
            return GetEquipmentItem().GetIncreaseArmors(level, randomSeed);
        }

        public Dictionary<DamageElement, MinMaxFloat> GetIncreaseDamages()
        {
            if (GetEquipmentItem() == null)
                return null;
            return GetEquipmentItem().GetIncreaseDamages(level, randomSeed);
        }

        public Dictionary<BaseSkill, short> GetIncreaseSkills()
        {
            if (GetEquipmentItem() == null)
                return null;
            return GetEquipmentItem().GetIncreaseSkills(randomSeed);
        }

        public CharacterStats GetSocketsIncreaseStats()
        {
            if (GetEquipmentItem() == null || Sockets.Count == 0)
                return CharacterStats.Empty;
            CharacterStats result = new CharacterStats();
            BaseItem tempEnhancer;
            foreach (int socketId in Sockets)
            {
                if (GameInstance.Items.TryGetValue(socketId, out tempEnhancer))
                    result += (tempEnhancer as ISocketEnhancerItem).SocketEnhanceEffect.stats;
            }
            return result;
        }

        public CharacterStats GetSocketsIncreaseStatsRate()
        {
            if (GetEquipmentItem() == null || Sockets.Count == 0)
                return CharacterStats.Empty;
            CharacterStats result = new CharacterStats();
            BaseItem tempEnhancer;
            foreach (int socketId in Sockets)
            {
                if (GameInstance.Items.TryGetValue(socketId, out tempEnhancer))
                    result += (tempEnhancer as ISocketEnhancerItem).SocketEnhanceEffect.statsRate;
            }
            return result;
        }

        public Dictionary<Attribute, float> GetSocketsIncreaseAttributes()
        {
            if (GetEquipmentItem() == null || Sockets.Count == 0)
                return null;
            Dictionary<Attribute, float> result = new Dictionary<Attribute, float>();
            BaseItem tempEnhancer;
            foreach (int socketId in Sockets)
            {
                if (GameInstance.Items.TryGetValue(socketId, out tempEnhancer))
                    result = GameDataHelpers.CombineAttributes((tempEnhancer as ISocketEnhancerItem).SocketEnhanceEffect.attributes, result, 1f);
            }
            return result;
        }

        public Dictionary<Attribute, float> GetSocketsIncreaseAttributesRate()
        {
            if (GetEquipmentItem() == null || Sockets.Count == 0)
                return null;
            Dictionary<Attribute, float> result = new Dictionary<Attribute, float>();
            BaseItem tempEnhancer;
            foreach (int socketId in Sockets)
            {
                if (GameInstance.Items.TryGetValue(socketId, out tempEnhancer))
                    result = GameDataHelpers.CombineAttributes((tempEnhancer as ISocketEnhancerItem).SocketEnhanceEffect.attributesRate, result, 1f);
            }
            return result;
        }

        public Dictionary<DamageElement, float> GetSocketsIncreaseResistances()
        {
            if (GetEquipmentItem() == null || Sockets.Count == 0)
                return null;
            Dictionary<DamageElement, float> result = new Dictionary<DamageElement, float>();
            BaseItem tempEnhancer;
            foreach (int socketId in Sockets)
            {
                if (GameInstance.Items.TryGetValue(socketId, out tempEnhancer))
                    result = GameDataHelpers.CombineResistances((tempEnhancer as ISocketEnhancerItem).SocketEnhanceEffect.resistances, result, 1f);
            }
            return result;
        }

        public Dictionary<DamageElement, float> GetSocketsIncreaseArmors()
        {
            if (GetEquipmentItem() == null || Sockets.Count == 0)
                return null;
            Dictionary<DamageElement, float> result = new Dictionary<DamageElement, float>();
            BaseItem tempEnhancer;
            foreach (int socketId in Sockets)
            {
                if (GameInstance.Items.TryGetValue(socketId, out tempEnhancer))
                    result = GameDataHelpers.CombineArmors((tempEnhancer as ISocketEnhancerItem).SocketEnhanceEffect.armors, result, 1f);
            }
            return result;
        }

        public Dictionary<DamageElement, MinMaxFloat> GetSocketsIncreaseDamages()
        {
            if (GetEquipmentItem() == null || Sockets.Count == 0)
                return null;
            Dictionary<DamageElement, MinMaxFloat> result = new Dictionary<DamageElement, MinMaxFloat>();
            BaseItem tempEnhancer;
            foreach (int socketId in Sockets)
            {
                if (GameInstance.Items.TryGetValue(socketId, out tempEnhancer))
                    result = GameDataHelpers.CombineDamages((tempEnhancer as ISocketEnhancerItem).SocketEnhanceEffect.damages, result, 1f);
            }
            return result;
        }

        public Dictionary<BaseSkill, short> GetSocketsIncreaseSkills()
        {
            if (GetEquipmentItem() == null || Sockets.Count == 0)
                return null;
            Dictionary<BaseSkill, short> result = new Dictionary<BaseSkill, short>();
            BaseItem tempEnhancer;
            foreach (int socketId in Sockets)
            {
                if (GameInstance.Items.TryGetValue(socketId, out tempEnhancer))
                    result = GameDataHelpers.CombineSkills((tempEnhancer as ISocketEnhancerItem).SocketEnhanceEffect.skills, result);
            }
            return result;
        }

        public CharacterItem Clone(bool generateNewId = false)
        {
            return new CharacterItem()
            {
                id = generateNewId ? GenericUtils.GetUniqueId() : id,
                dataId = dataId,
                level = level,
                amount = amount,
                equipSlotIndex = equipSlotIndex,
                durability = durability,
                exp = exp,
                lockRemainsDuration = lockRemainsDuration,
                expireTime = expireTime,
                randomSeed = randomSeed,
                ammo = ammo,
                sockets = new List<int>(sockets),
            };
        }

        public static CharacterItem Create(BaseItem item, short level = 1, short amount = 1, short? randomSeed = null)
        {
            return Create(item.DataId, level, amount, randomSeed);
        }

        public static CharacterItem Create(int dataId, short level = 1, short amount = 1, short? randomSeed = null)
        {
            CharacterItem newItem = new CharacterItem();
            newItem.id = GenericUtils.GetUniqueId();
            newItem.dataId = dataId;
            if (level <= 0)
                level = 1;
            newItem.level = level;
            newItem.amount = amount;
            newItem.durability = 0f;
            newItem.exp = 0;
            newItem.lockRemainsDuration = 0f;
            newItem.ammo = 0;
            BaseItem tempItem;
            if (GameInstance.Items.TryGetValue(dataId, out tempItem) && tempItem is IEquipmentItem)
            {
                newItem.durability = (tempItem as IEquipmentItem).MaxDurability;
                newItem.lockRemainsDuration = tempItem.LockDuration;
                if (randomSeed.HasValue)
                    newItem.randomSeed = randomSeed.Value;
                else
                    newItem.randomSeed = (short)Random.Range(short.MinValue, short.MaxValue);
            }
            return newItem;
        }

        public static CharacterItem CreateEmptySlot()
        {
            return Create(0, 1, 0);
        }

        public List<int> ReadSockets(string sockets, char separator = ';')
        {
            Sockets.Clear();
            string[] splitTexts = sockets.Split(separator);
            foreach (string text in splitTexts)
            {
                if (string.IsNullOrEmpty(text))
                    continue;
                Sockets.Add(int.Parse(text));
            }
            return Sockets;
        }

        public string WriteSockets(char separator = ';')
        {
            stringBuilder.Clear();
            foreach (int socket in Sockets)
            {
                stringBuilder
                    .Append(socket)
                    .Append(separator);
            }
            return stringBuilder.ToString();
        }

        public void Serialize(NetDataWriter writer)
        {
            if (this.IsEmptySlot())
            {
                writer.Put((byte)CharacterItemSyncState.IsEmpty);
                return;
            }
            bool isEquipment = GetEquipmentItem() != null;
            bool isWeapon = isEquipment && GetWeaponItem() != null;
            bool isPet = GetPetItem() != null;
            CharacterItemSyncState syncState = CharacterItemSyncState.None;
            if (isEquipment)
            {
                syncState |= CharacterItemSyncState.IsEquipment;
            }
            if (isWeapon)
            {
                syncState |= CharacterItemSyncState.IsWeapon;
            }
            if (isPet)
            {
                syncState |= CharacterItemSyncState.IsPet;
            }
            writer.Put((byte)syncState);

            writer.Put(id);
            writer.PutPackedLong(expireTime);
            writer.PutPackedInt(dataId);
            writer.PutPackedShort(level);
            writer.PutPackedShort(amount);
            writer.Put(equipSlotIndex);
            writer.Put(lockRemainsDuration);

            if (isEquipment)
            {
                writer.Put(durability);
                writer.PutPackedInt(exp);

                byte socketCount = (byte)Sockets.Count;
                writer.Put(socketCount);
                if (socketCount > 0)
                {
                    foreach (int socketDataId in Sockets)
                    {
                        writer.PutPackedInt(socketDataId);
                    }
                }

                writer.PutPackedShort(randomSeed);
            }

            if (isWeapon)
            {
                writer.PutPackedShort(ammo);
            }

            if (isPet)
            {
                writer.PutPackedInt(exp);
            }
        }

        public void Deserialize(NetDataReader reader)
        {
            CharacterItemSyncState syncState = (CharacterItemSyncState)reader.GetByte();
            if (syncState == CharacterItemSyncState.IsEmpty)
            {
                id = string.Empty;
                dataId = 0;
                level = 0;
                amount = 0;
                equipSlotIndex = 0;
                durability = 0;
                exp = 0;
                lockRemainsDuration = 0;
                expireTime = 0;
                randomSeed = 0;
                ammo = 0;
                Sockets.Clear();
                return;
            }

            id = reader.GetString();
            expireTime = reader.GetPackedLong();
            dataId = reader.GetPackedInt();
            level = reader.GetPackedShort();
            amount = reader.GetPackedShort();
            equipSlotIndex = reader.GetByte();
            lockRemainsDuration = reader.GetFloat();

            if (syncState.Has(CharacterItemSyncState.IsEquipment))
            {
                durability = reader.GetFloat();
                exp = reader.GetPackedInt();

                byte socketCount = reader.GetByte();
                Sockets.Clear();
                for (byte i = 0; i < socketCount; ++i)
                {
                    Sockets.Add(reader.GetPackedInt());
                }

                randomSeed = reader.GetPackedShort();
            }

            if (syncState.Has(CharacterItemSyncState.IsWeapon))
            {
                ammo = reader.GetPackedShort();
            }

            if (syncState.Has(CharacterItemSyncState.IsPet))
            {
                exp = reader.GetPackedInt();
            }
        }
    }

    [System.Serializable]
    public class SyncListCharacterItem : LiteNetLibSyncList<CharacterItem>
    {
    }
}
