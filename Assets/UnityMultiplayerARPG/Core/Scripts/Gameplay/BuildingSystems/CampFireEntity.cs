﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LiteNetLibManager;
using LiteNetLib;
using System.Text;
using Cysharp.Threading.Tasks;

namespace MultiplayerARPG
{
    public class CampFireEntity : StorageEntity
    {
        [Category(7, "Campfire Settings")]
        [SerializeField]
        protected ConvertItem[] convertItems = new ConvertItem[0];
        public ConvertItem[] ConvertItems { get { return convertItems; } }

        [Category("Events")]
        [SerializeField]
        protected UnityEvent onInitialTurnOn = new UnityEvent();
        [SerializeField]
        protected UnityEvent onInitialTurnOff = new UnityEvent();
        [SerializeField]
        protected UnityEvent onTurnOn = new UnityEvent();
        [SerializeField]
        protected UnityEvent onTurnOff = new UnityEvent();

        [Category("Sync Fields")]
        [SerializeField]
        protected SyncFieldBool isTurnOn = new SyncFieldBool();
        [SerializeField]
        protected SyncFieldFloat turnOnElapsed = new SyncFieldFloat();

        public bool IsTurnOn
        {
            get { return isTurnOn.Value; }
            set { isTurnOn.Value = value; }
        }

        public float TurnOnElapsed
        {
            get { return turnOnElapsed.Value; }
            set { turnOnElapsed.Value = value; }
        }

        public override string ExtraData
        {
            get
            {
                return new StringBuilder().Append(IsTurnOn ? byte.MaxValue : byte.MinValue).Append(':').Append(TurnOnElapsed).ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                string[] splitedTexts = value.Split(':');
                byte isTurnOn;
                if (splitedTexts.Length == 2 && byte.TryParse(splitedTexts[0], out isTurnOn))
                    IsTurnOn = isTurnOn != 0;
                float turnOnElapsed;
                if (splitedTexts.Length == 2 && float.TryParse(splitedTexts[1], out turnOnElapsed))
                    TurnOnElapsed = turnOnElapsed;
            }
        }

        private float tempDeltaTime;
        protected readonly Dictionary<int, float> convertRemainsDuration = new Dictionary<int, float>();

        protected Dictionary<int, ConvertItem> cacheFuelItems;
        public Dictionary<int, ConvertItem> CacheFuelItems
        {
            get
            {
                if (cacheFuelItems == null)
                {
                    cacheFuelItems = new Dictionary<int, ConvertItem>();
                    if (convertItems != null && convertItems.Length > 0)
                    {
                        foreach (ConvertItem convertItem in convertItems)
                        {
                            if (convertItem.item.item == null || !convertItem.isFuel) continue;
                            cacheFuelItems[convertItem.item.item.DataId] = convertItem;
                        }
                    }
                }
                return cacheFuelItems;
            }
        }

        protected Dictionary<int, ConvertItem> cacheConvertItems;
        public Dictionary<int, ConvertItem> CacheConvertItems
        {
            get
            {
                if (cacheConvertItems == null)
                {
                    cacheConvertItems = new Dictionary<int, ConvertItem>();
                    if (convertItems != null && convertItems.Length > 0)
                    {
                        foreach (ConvertItem convertItem in convertItems)
                        {
                            if (convertItem.item.item == null) continue;
                            cacheConvertItems[convertItem.item.item.DataId] = convertItem;
                        }
                    }
                }
                return cacheConvertItems;
            }
        }

        public override void OnSetup()
        {
            base.OnSetup();
            isTurnOn.onChange += OnIsTurnOnChange;
        }

        protected override void SetupNetElements()
        {
            base.SetupNetElements();
            isTurnOn.deliveryMethod = DeliveryMethod.ReliableOrdered;
            isTurnOn.syncMode = LiteNetLibSyncField.SyncMode.ServerToClients;
            turnOnElapsed.deliveryMethod = DeliveryMethod.Sequenced;
            turnOnElapsed.syncMode = LiteNetLibSyncField.SyncMode.ServerToClients;
        }

        protected override void EntityOnDestroy()
        {
            base.EntityOnDestroy();
            isTurnOn.onChange -= OnIsTurnOnChange;
        }

        private void OnIsTurnOnChange(bool isInitial, bool isTurnOn)
        {
            if (isInitial)
            {
                if (isTurnOn)
                    onInitialTurnOn.Invoke();
                else
                    onInitialTurnOff.Invoke();
            }
            else
            {
                if (isTurnOn)
                    onTurnOn.Invoke();
                else
                    onTurnOff.Invoke();
            }
        }

        protected override void EntityUpdate()
        {
            base.EntityUpdate();
            if (!IsServer)
                return;

            if (!IsTurnOn)
            {
                if (convertRemainsDuration.Count > 0)
                    convertRemainsDuration.Clear();
                return;
            }

            if (!CanTurnOn())
            {
                IsTurnOn = false;
                TurnOnElapsed = 0f;
                return;
            }

            // Consume fuel and convert item
            tempDeltaTime = Time.unscaledDeltaTime;
            TurnOnElapsed += tempDeltaTime;

            ConvertItem convertData;
            List<CharacterItem> items = new List<CharacterItem>(GameInstance.ServerStorageHandlers.GetStorageEntityItems(this));
            CharacterItem tempItem;
            for (int i = 0; i < items.Count; ++i)
            {
                tempItem = items[i];
                if (!CacheConvertItems.ContainsKey(tempItem.dataId))
                    continue;

                convertData = CacheConvertItems[tempItem.dataId];

                if (!convertRemainsDuration.ContainsKey(tempItem.dataId))
                    convertRemainsDuration.Add(tempItem.dataId, convertData.convertInterval);

                convertRemainsDuration[tempItem.dataId] -= tempDeltaTime;

                if (convertRemainsDuration[tempItem.dataId] <= 0f)
                {
                    convertRemainsDuration[tempItem.dataId] = convertData.convertInterval;
                    ConvertItem(convertData).Forget();
                }
            }
        }

        protected async UniTaskVoid ConvertItem(ConvertItem convertData)
        {
            StorageId storageId = new StorageId(StorageType.Building, Id);
            ItemAmount tempItemAmount = convertData.item;
            await GameInstance.ServerStorageHandlers.DecreaseStorageItems(storageId, tempItemAmount.item.DataId, tempItemAmount.amount);
            if (convertData.convertedItem.item != null)
            {
                tempItemAmount = convertData.convertedItem;
                CharacterItem convertedItem = CharacterItem.Create(tempItemAmount.item.DataId, 1, tempItemAmount.amount);
                if (!await GameInstance.ServerStorageHandlers.IncreaseStorageItems(storageId, convertedItem))
                {
                    // Cannot add item to storage, so drop to ground
                    ItemDropEntity.DropItem(this, convertedItem, new string[0]);
                }
            }
        }

        public void TurnOn()
        {
            if (!CanTurnOn())
                return;
            IsTurnOn = true;
            TurnOnElapsed = 0f;
        }

        public void TurnOff()
        {
            IsTurnOn = false;
            TurnOnElapsed = 0f;
        }

        public bool CanTurnOn()
        {
            if (CacheFuelItems.Count == 0)
            {
                // Not require fuel
                return true;
            }
            List<CharacterItem> items = GameInstance.ServerStorageHandlers.GetStorageEntityItems(this);
            Dictionary<int, short> countItems = new Dictionary<int, short>();
            foreach (CharacterItem item in items)
            {
                if (CacheFuelItems.ContainsKey(item.dataId))
                {
                    if (!countItems.ContainsKey(item.dataId))
                        countItems.Add(item.dataId, 0);
                    countItems[item.dataId] += item.amount;
                    if (countItems[item.dataId] >= CacheFuelItems[item.dataId].item.amount)
                        return true;
                }
            }
            return false;
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            List<BaseItem> items = new List<BaseItem>();
            if (convertItems != null && convertItems.Length > 0)
            {
                foreach (ConvertItem convertItem in convertItems)
                {
                    items.Add(convertItem.item.item);
                    items.Add(convertItem.convertedItem.item);
                }
            }
            GameInstance.AddItems(items);
        }
    }
}
