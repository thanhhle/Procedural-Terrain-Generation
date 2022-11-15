﻿using UnityEngine;

namespace MultiplayerARPG
{
    public static class CraftingQueueSourceExtension
    {
        public static bool IsInCraftDistance(this ICraftingQueueSource source, Vector3 crafterPosition)
        {
            return source.CraftingDistance <= 0f || Vector3.Distance(crafterPosition, source.transform.position) <= source.CraftingDistance;
        }

        public static void UpdateQueue(this ICraftingQueueSource source)
        {
            if (!source.CanCraft)
            {
                if (source.QueueItems.Count > 0)
                    source.QueueItems.Clear();
                return;
            }

            if (source.QueueItems.Count == 0)
            {
                source.TimeCounter = 0f;
                return;
            }

            CraftingQueueItem craftingItem = source.QueueItems[0];
            ItemCraftFormula formula = GameInstance.ItemCraftFormulas[craftingItem.dataId];
            BasePlayerCharacterEntity crafter;
            if (!BaseGameNetworkManager.Singleton.TryGetEntityByObjectId(craftingItem.crafterId, out crafter))
            {
                // Crafter may left the game
                source.QueueItems.RemoveAt(0);
                return;
            }

            if (!source.IsInCraftDistance(crafter.CacheTransform.position))
            {
                // Crafter too far from crafting source
                source.QueueItems.RemoveAt(0);
                return;
            }

            UITextKeys errorMessage;
            if (!formula.ItemCraft.CanCraft(crafter, out errorMessage))
            {
                source.TimeCounter = 0f;
                source.QueueItems.RemoveAt(0);
                GameInstance.ServerGameMessageHandlers.SendGameMessage(crafter.ConnectionId, errorMessage);
                return;
            }

            source.TimeCounter += Time.unscaledDeltaTime;
            if (source.TimeCounter >= 1f)
            {
                craftingItem.craftRemainsDuration -= source.TimeCounter;
                source.TimeCounter = 0f;
                if (craftingItem.craftRemainsDuration <= 0f)
                {
                    // Reduce items and add crafting item
                    formula.ItemCraft.CraftItem(crafter);
                    // Reduce amount
                    if (craftingItem.amount > 1)
                    {
                        --craftingItem.amount;
                        craftingItem.craftRemainsDuration = formula.CraftDuration;
                        source.QueueItems[0] = craftingItem;
                    }
                    else
                    {
                        source.QueueItems.RemoveAt(0);
                    }
                }
                else
                {
                    // Update remains duration
                    source.QueueItems[0] = craftingItem;
                }
            }
        }

        public static void AppendCraftingQueueItem(this ICraftingQueueSource source, uint crafterId, int dataId, short amount)
        {
            if (!source.CanCraft)
                return;
            ItemCraftFormula itemCraftFormula;
            if (!GameInstance.ItemCraftFormulas.TryGetValue(dataId, out itemCraftFormula))
                return;
            if (source.QueueItems.Count >= source.MaxQueueSize)
                return;
            source.QueueItems.Add(new CraftingQueueItem()
            {
                crafterId = crafterId,
                dataId = dataId,
                amount = amount,
                craftRemainsDuration = itemCraftFormula.CraftDuration,
            });
        }

        public static void ChangeCraftingQueueItem(this ICraftingQueueSource source, uint crafterId, int index, short amount)
        {
            if (!source.CanCraft)
                return;
            if (index < 0 || index >= source.QueueItems.Count)
                return;
            if (source.QueueItems[index].crafterId != crafterId)
                return;
            if (amount <= 0)
            {
                source.QueueItems.RemoveAt(index);
                return;
            }
            CraftingQueueItem craftingItem = source.QueueItems[index];
            craftingItem.amount = amount;
            source.QueueItems[index] = craftingItem;
        }

        public static void CancelCraftingQueueItem(this ICraftingQueueSource source, uint crafterId, int index)
        {
            if (!source.CanCraft)
                return;
            if (index < 0 || index >= source.QueueItems.Count)
                return;
            if (source.QueueItems[index].crafterId != crafterId)
                return;
            source.QueueItems.RemoveAt(index);
        }
    }
}
