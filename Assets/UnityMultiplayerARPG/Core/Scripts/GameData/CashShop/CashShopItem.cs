﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = "Cash Shop Item", menuName = "Create CashShop/Cash Shop Item", order = -3996)]
    public class CashShopItem : BaseGameData
    {
        [Category("Cash Shop Item Settings")]
        [SerializeField]
        private string externalIconUrl = string.Empty;
        public string ExternalIconUrl { get { return externalIconUrl; } }

        [FormerlySerializedAs("sellPrice")]
        [SerializeField]
        private int sellPriceCash = 0;
        public int SellPriceCash { get { return sellPriceCash; } }

        [SerializeField]
        private int sellPriceGold = 0;
        public int SellPriceGold { get { return sellPriceGold; } }

        [Tooltip("Gold which character will receives")]
        [SerializeField]
        private int receiveGold = 0;
        public int ReceiveGold { get { return receiveGold; } }

        [ArrayElementTitle("currency")]
        [SerializeField]
        private CurrencyAmount[] receiveCurrencies = new CurrencyAmount[0];
        public CurrencyAmount[] ReceiveCurrencies { get { return receiveCurrencies; } }

        [ArrayElementTitle("item")]
        [SerializeField]
        private ItemAmount[] receiveItems = new ItemAmount[0];
        public ItemAmount[] ReceiveItems { get { return receiveItems; } }

        public CashShopItem GenerateByItem(BaseItem item, CashShopItemGeneratingData generatingData)
        {
            List<string> languageKeys = new List<string>(LanguageManager.Languages.Keys);
            List<LanguageData> titleLanguageDataList = new List<LanguageData>();
            List<LanguageData> descriptionLanguageDataList = new List<LanguageData>();
            defaultTitle = string.Format(LanguageManager.GetText(UIFormatKeys.UI_FORMAT_GENERATE_CAST_SHOP_ITEM_TITLE.ToString()), item.DefaultTitle, generatingData.amount);
            defaultDescription = string.Format(LanguageManager.GetText(UIFormatKeys.UI_FORMAT_GENERATE_CAST_SHOP_ITEM_DESCRIPTION.ToString()), item.DefaultTitle, generatingData.amount, defaultDescription);
            foreach (string languageKey in languageKeys)
            {
                titleLanguageDataList.Add(new LanguageData()
                {
                    key = languageKey,
                    value = string.Format(LanguageManager.GetTextByLanguage(languageKey, UIFormatKeys.UI_FORMAT_GENERATE_CAST_SHOP_ITEM_TITLE.ToString()), Language.GetTextByLanguageKey(item.LanguageSpecificTitles, languageKey, item.DefaultTitle), generatingData.amount),
                });
                descriptionLanguageDataList.Add(new LanguageData()
                {
                    key = languageKey,
                    value = string.Format(LanguageManager.GetTextByLanguage(languageKey, UIFormatKeys.UI_FORMAT_GENERATE_CAST_SHOP_ITEM_DESCRIPTION.ToString()), Language.GetTextByLanguageKey(item.LanguageSpecificTitles, languageKey, item.DefaultTitle), generatingData.amount, Language.GetTextByLanguageKey(item.LanguageSpecificDescriptions, languageKey, item.DefaultDescription)),
                });
            }
            languageSpecificTitles = titleLanguageDataList.ToArray();
            languageSpecificDescriptions = descriptionLanguageDataList.ToArray();
            category = item.Category;
            icon = item.Icon;
            sellPriceCash = generatingData.sellPriceCash;
            sellPriceGold = generatingData.sellPriceGold;
            receiveItems = new ItemAmount[]
            {
                    new ItemAmount()
                    {
                        item = item,
                        amount = generatingData.amount,
                    }
            };
            return this;
        }
    }
}
