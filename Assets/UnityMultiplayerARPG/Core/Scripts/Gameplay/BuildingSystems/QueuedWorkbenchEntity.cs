﻿using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class QueuedWorkbenchEntity : BuildingEntity, ICraftingQueueSource
    {
        [Category(6, "Workbench Settings")]
        [SerializeField]
        protected ItemCraftFormula[] itemCraftFormulas = new ItemCraftFormula[0];
        public ItemCraftFormula[] ItemCraftFormulas { get { return itemCraftFormulas; } }
        [SerializeField]
        protected int maxQueueSize = 5;
        public int MaxQueueSize { get { return maxQueueSize; } }
        [SerializeField]
        [Tooltip("If this is > 0 it will limit distance to craft an items with this workbench entity by its value")]
        protected float craftingDistance = 5f;
        public float CraftingDistance { get { return craftingDistance; } }

        [Category("Sync Fields")]
        [SerializeField]
        protected SyncListCraftingQueueItem queueItems = new SyncListCraftingQueueItem();

        public override bool Activatable { get { return true; } }

        private Dictionary<int, ItemCraftFormula> cacheItemCraftFormulas;
        public Dictionary<int, ItemCraftFormula> CacheItemCraftFormulas
        {
            get
            {
                if (cacheItemCraftFormulas == null)
                {
                    cacheItemCraftFormulas = new Dictionary<int, ItemCraftFormula>();
                    foreach (ItemCraftFormula itemCraftFormula in itemCraftFormulas)
                    {
                        if (itemCraftFormula.ItemCraft.CraftingItem == null)
                            continue;
                        cacheItemCraftFormulas[itemCraftFormula.DataId] = itemCraftFormula;
                    }
                }
                return cacheItemCraftFormulas;
            }
        }

        public SyncListCraftingQueueItem QueueItems
        {
            get { return queueItems; }
        }

        public bool CanCraft
        {
            get { return !this.IsDead(); }
        }

        public float TimeCounter { get; set; }

        public int SourceId
        {
            get { return Identity.HashAssetId; }
        }

        public override sealed void OnSetup()
        {
            base.OnSetup();
            queueItems.forOwnerOnly = false;
        }

        protected override void EntityUpdate()
        {
            base.EntityUpdate();
            if (IsServer)
                this.UpdateQueue();
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            if (CacheItemCraftFormulas.Count > 0)
            {
                List<BaseItem> items = new List<BaseItem>();
                foreach (ItemCraftFormula itemCraftFormula in CacheItemCraftFormulas.Values)
                {
                    items.Add(itemCraftFormula.ItemCraft.CraftingItem);
                    items.AddRange(itemCraftFormula.ItemCraft.CacheCraftRequirements.Keys);
                }
                GameInstance.AddItems(items);
                GameInstance.AddItemCraftFormulas(SourceId, CacheItemCraftFormulas.Values);
            }
        }
    }
}
