﻿using System.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UIEquipmentSet : UIBaseEquipmentBonus<UIEquipmentSetData>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Set Title}, {1} = {List Of Effect}")]
        public UILocaleKeySetting formatKeySet = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_EQUIPMENT_SET);
        [Tooltip("Format => {0} = {Equip Amount}, {1} = {List Of Bonus}")]
        public UILocaleKeySetting formatKeyAppliedEffect = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_EQUIPMENT_SET_APPLIED_EFFECT);
        [Tooltip("Format => {0} = {Equip Amount}, {1} = {List Of Bonus}")]
        public UILocaleKeySetting formatKeyUnappliedEffect = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_EQUIPMENT_SET_UNAPPLIED_EFFECT);
        
        protected override void UpdateData()
        {
            StringBuilder allBonusText = new StringBuilder();
            int effectCount = 1;
            string tempText;
            foreach (EquipmentBonus effect in Data.equipmentSet.Effects)
            {
                tempText = GetEquipmentBonusText(effect);
                if (!string.IsNullOrEmpty(tempText))
                {
                    if (allBonusText.Length > 0)
                        allBonusText.Append('\n');
                    allBonusText.AppendFormat(
                        effectCount <= Data.equippedCount ?
                            LanguageManager.GetText(formatKeyAppliedEffect) :
                            LanguageManager.GetText(formatKeyUnappliedEffect),
                        effectCount.ToString("N0"),
                        tempText);
                }
                ++effectCount;
            }

            if (uiTextAllBonus != null)
            {
                uiTextAllBonus.SetGameObjectActive(allBonusText.Length > 0);
                uiTextAllBonus.text = string.Format(
                    LanguageManager.GetText(formatKeySet),
                    Data.equipmentSet.Title,
                    allBonusText.ToString());
            }
        }
    }
}
