﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UIDismantleItem : UIBaseOwningCharacterItem
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Return Gold Amount}")]
        public UILocaleKeySetting formatKeyReturnGold = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_GOLD);
        [Tooltip("Format => {0} = {Amount of Dismantle Item}")]
        public UILocaleKeySetting formatKeyDismantleAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);

        [Header("UI Elements for UI Dismantle Item")]
        public UIItemAmounts uiReturnItems;
        public TextWrapper uiTextReturnGold;
        public TextWrapper uiTextDismantleAmount;

        protected bool activated;
        protected string activeItemId;

        private short dismantleAmount;
        public short DismantleAmount
        {
            get { return dismantleAmount; }
            private set
            {
                dismantleAmount = value;
                if (uiTextDismantleAmount != null)
                    uiTextDismantleAmount.text = string.Format(LanguageManager.GetText(formatKeyDismantleAmount), dismantleAmount.ToString("N0"));
            }
        }

        protected override void UpdateData()
        {
            base.UpdateData();
            DismantleAmount = Amount;
        }

        public override void OnUpdateCharacterItems()
        {
            if (!IsVisible())
                return;

            // Store data to variable so it won't lookup for data from property again
            CharacterItem characterItem = CharacterItem;

            if (activated && (characterItem.IsEmptySlot() || !characterItem.id.Equals(activeItemId)))
            {
                // Item's ID is difference to active item ID, so the item may be destroyed
                // So clear data
                Data = new UIOwningCharacterItemData(InventoryType.NonEquipItems, -1);
                return;
            }

            if (uiCharacterItem != null)
            {
                if (characterItem.IsEmptySlot())
                {
                    uiCharacterItem.Hide();
                }
                else
                {
                    uiCharacterItem.Setup(new UICharacterItemData(characterItem, InventoryType), GameInstance.PlayingCharacter, IndexOfData);
                    uiCharacterItem.Show();
                }
            }

            List<ItemAmount> returningItems = BaseItem.GetDismantleReturnItems(characterItem, DismantleAmount);
            if (uiReturnItems != null)
            {
                if (characterItem.IsEmptySlot() || returningItems.Count == 0)
                {
                    uiReturnItems.Hide();
                }
                else
                {
                    uiReturnItems.displayType = UIItemAmounts.DisplayType.Simple;
                    uiReturnItems.Show();
                    uiReturnItems.Data = GameDataHelpers.CombineItems(returningItems.ToArray(), new Dictionary<BaseItem, short>()); ;
                }
            }

            if (uiTextReturnGold != null)
            {
                if (characterItem.IsEmptySlot())
                {
                    uiTextReturnGold.text = string.Format(
                            LanguageManager.GetText(formatKeyReturnGold),
                            "0");
                }
                else
                {
                    uiTextReturnGold.text = string.Format(
                            LanguageManager.GetText(formatKeyReturnGold),
                            (characterItem.GetItem().DismantleReturnGold * DismantleAmount).ToString("N0"));
                }
            }
        }

        public override void Show()
        {
            base.Show();
            activated = false;
            OnUpdateCharacterItems();
        }

        public override void Hide()
        {
            base.Hide();
            Data = new UIOwningCharacterItemData(InventoryType.NonEquipItems, -1);
        }

        public void OnClickDismantle()
        {
            if (InventoryType != InventoryType.NonEquipItems || CharacterItem.IsEmptySlot())
                return;
            activated = true;
            activeItemId = CharacterItem.id;
            GameInstance.ClientInventoryHandlers.RequestDismantleItem(new RequestDismantleItemMessage()
            {
                index = (short)IndexOfData,
                amount = DismantleAmount,
            }, ClientInventoryActions.ResponseDismantleItem);
        }

        public void OnClickSetDismantleAmount()
        {
            if (Amount > 1)
                UISceneGlobal.Singleton.ShowInputDialog(LanguageManager.GetText(UITextKeys.UI_DISMANTLE_ITEM.ToString()), LanguageManager.GetText(UITextKeys.UI_DISMANTLE_ITEM_DESCRIPTION.ToString()), OnDismantleItemAmountConfirmed, 1, CharacterItem.amount, CharacterItem.amount);
            else
                DismantleAmount = Amount;
        }

        private void OnDismantleItemAmountConfirmed(int amount)
        {
            DismantleAmount = (short)amount;
        }
    }
}
