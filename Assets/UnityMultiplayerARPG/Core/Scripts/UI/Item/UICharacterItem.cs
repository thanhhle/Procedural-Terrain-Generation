﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class UICharacterItem : UIDataForCharacter<UICharacterItemData>
    {
        public CharacterItem CharacterItem { get { return Data.characterItem; } }
        public short Level { get { return Data.targetLevel; } }
        public InventoryType InventoryType { get { return Data.inventoryType; } }
        public BaseItem Item { get { return CharacterItem != null && CharacterItem.NotEmptySlot() ? CharacterItem.GetItem() : null; } }
        public IUsableItem UsableItem { get { return CharacterItem != null && CharacterItem.NotEmptySlot() ? CharacterItem.GetUsableItem() : null; } }
        public IEquipmentItem EquipmentItem { get { return CharacterItem != null && CharacterItem.NotEmptySlot() ? CharacterItem.GetEquipmentItem() : null; } }
        public IArmorItem ArmorItem { get { return CharacterItem != null && CharacterItem.NotEmptySlot() ? CharacterItem.GetArmorItem() : null; } }
        public IShieldItem ShieldItem { get { return CharacterItem != null && CharacterItem.NotEmptySlot() ? CharacterItem.GetShieldItem() : null; } }
        public IDefendEquipmentItem DefendItem { get { return CharacterItem != null && CharacterItem.NotEmptySlot() ? CharacterItem.GetDefendItem() : null; } }
        public IWeaponItem WeaponItem { get { return CharacterItem != null && CharacterItem.NotEmptySlot() ? CharacterItem.GetWeaponItem() : null; } }
        public IPotionItem PotionItem { get { return CharacterItem != null && CharacterItem.NotEmptySlot() ? CharacterItem.GetPotionItem() : null; } }
        public IAmmoItem AmmoItem { get { return CharacterItem != null && CharacterItem.NotEmptySlot() ? CharacterItem.GetAmmoItem() : null; } }
        public IBuildingItem BuildingItem { get { return CharacterItem != null && CharacterItem.NotEmptySlot() ? CharacterItem.GetBuildingItem() : null; } }
        public IPetItem PetItem { get { return CharacterItem != null && CharacterItem.NotEmptySlot() ? CharacterItem.GetPetItem() : null; } }
        public ISocketEnhancerItem SocketEnhancerItem { get { return CharacterItem != null && CharacterItem.NotEmptySlot() ? CharacterItem.GetSocketEnhancerItem() : null; } }
        public IMountItem MountItem { get { return CharacterItem != null && CharacterItem.NotEmptySlot() ? CharacterItem.GetMountItem() : null; } }
        public ISkillItem SkillItem { get { return CharacterItem != null && CharacterItem.NotEmptySlot() ? CharacterItem.GetSkillItem() : null; } }

        [Header("String Formats")]
        [Tooltip("Format => {0} = {Title}")]
        public UILocaleKeySetting formatKeyTitle = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Description}")]
        public UILocaleKeySetting formatKeyDescription = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Rarity Title}")]
        public UILocaleKeySetting formatKeyRarityTitle = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_RARITY);
        [Tooltip("Format => {0} = {Level}")]
        public UILocaleKeySetting formatKeyLevel = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_LEVEL);
        [Tooltip("Format => {0} = {Refine Level}")]
        public UILocaleKeySetting formatKeyRefineLevel = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_REFINE_LEVEL);
        [Tooltip("Format => {0} = {Refine Level}")]
        public UILocaleKeySetting formatKeyTitleWithRefineLevel = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_TITLE_WITH_REFINE_LEVEL);
        [Tooltip("Format => {0} = {Sell Price}")]
        public UILocaleKeySetting formatKeySellPrice = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SELL_PRICE);
        [Tooltip("Format => {0} = {Amount}, {1} = {Max Stack}")]
        public UILocaleKeySetting formatKeyStack = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_STACK);
        [Tooltip("Format => {0} = {Durability}, {1} = {Max Durability}")]
        public UILocaleKeySetting formatKeyDurability = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_DURABILITY);
        [Tooltip("Format => {0} = {Weight}")]
        public UILocaleKeySetting formatKeyWeight = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_WEIGHT);
        [Tooltip("Format => {0} = {Current Exp}, {1} = {Max Exp}")]
        public UILocaleKeySetting formatKeyExp = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CURRENT_EXP);
        [Tooltip("Format => {0} = {Lock Remains Duration}")]
        public UILocaleKeySetting formatKeyLockRemainsDuration = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Building Title}")]
        public UILocaleKeySetting formatKeyBuilding = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_BUILDING);
        [Tooltip("Format => {0} = {Pet Title}")]
        public UILocaleKeySetting formatKeyPet = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_PET);
        [Tooltip("Format => {0} = {Mount Title}")]
        public UILocaleKeySetting formatKeyMount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_MOUNT);
        [Tooltip("Format => {0} = {Skill Title}")]
        public UILocaleKeySetting formatKeySkill = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_SKILL);
        [Tooltip("Format => {0} = {Cooldown Duration}")]
        public UILocaleKeySetting formatKeyCoolDownDuration = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SKILL_COOLDOWN_DURATION);
        [Tooltip("Format => {0} = {Cooldown Remains Duration}")]
        public UILocaleKeySetting formatKeyCoolDownRemainsDuration = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Item Type Title}")]
        public UILocaleKeySetting formatKeyItemType = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_TYPE);

        [Header("UI Elements")]
        public TextWrapper uiTextTitle;
        public TextWrapper uiTextDescription;
        public TextWrapper uiTextRarity;
        public TextWrapper uiTextLevel;
        public Image imageIcon;
        public Image imageRarity;
        public TextWrapper uiTextItemType;
        public TextWrapper uiTextSellPrice;
        public TextWrapper uiTextStack;
        public TextWrapper uiTextDurability;
        public UIGageValue uiGageDurability;
        public TextWrapper uiTextWeight;
        public TextWrapper uiTextExp;
        public UIGageValue uiGageExp;

        [Header("Item Locking")]
        public TextWrapper uiTextLockRemainsDuration;
        public GameObject[] lockObjects;
        public GameObject[] noLockObjects;

        [Header("Equipment - UI Elements")]
        public UIEquipmentItemRequirement uiRequirement;
        [FormerlySerializedAs("uiStats")]
        public UICharacterStats uiIncreaseStats;
        public UICharacterStats uiIncreaseStatsRate;
        public UIAttributeAmounts uiIncreaseAttributes;
        public UIAttributeAmounts uiIncreaseAttributesRate;
        public UIResistanceAmounts uiIncreaseResistances;
        public UIArmorAmounts uiIncreaseArmors;
        [FormerlySerializedAs("uiIncreaseDamageAmounts")]
        public UIDamageElementAmounts uiIncreaseDamages;
        public UISkillLevels uiIncreaseSkillLevels;
        public UIEquipmentSet uiEquipmentSet;
        public UIEquipmentSockets uiEquipmentSockets;
        [Tooltip("Use this component to show refine info, if you are going to refine when click button, use `OnClickRefineItem` function")]
        public UIRefineItem uiRefineItem;
        [Tooltip("Use this component to show dismantle info, if you are going to dismantle when click button, use `OnClickDismantleItem` function")]
        public UIDismantleItem uiDismantleItem;
        [Tooltip("Use this component to show repair info, if you are going to repair when click button, use `OnClickRepairItem` function")]
        public UIRepairItem uiRepairItem;

        [Header("Armor/Shield - UI Elements")]
        public UIArmorAmount uiArmorAmount;

        [Header("Weapon - UI Elements")]
        [FormerlySerializedAs("uiDamageAmounts")]
        public UIDamageElementAmount uiDamageAmount;

        [Header("Weapon Ammo - UI Elements")]
        public TextWrapper uiTextCurrentAmmo;
        public TextWrapper uiTextReserveAmmo;
        public TextWrapper uiTextSumAmmo;
        public GameObject[] requireAmmoSymbols;
        public GameObject[] noRequireAmmoSymbols;
        public UIGageValue gageAmmo;

        [Header("Building - UI Elements")]
        public TextWrapper uiTextBuilding;

        [Header("Pet - UI Elements")]
        public TextWrapper uiTextPet;

        [Header("Mount - UI Elements")]
        public TextWrapper uiTextMount;

        [Header("Skill - UI Elements")]
        public TextWrapper uiTextSkill;
        public TextWrapper uiTextCoolDownDuration;
        public TextWrapper uiTextCoolDownRemainsDuration;
        public Image imageCoolDownGage;
        public GameObject[] countDownObjects;
        public GameObject[] noCountDownObjects;

        [Header("Events")]
        public UnityEvent onSetLevelZeroData = new UnityEvent();
        public UnityEvent onSetNonLevelZeroData = new UnityEvent();
        public UnityEvent onSetEquippedData = new UnityEvent();
        public UnityEvent onSetUnEquippedData = new UnityEvent();
        public UnityEvent onSetUnEquippableData = new UnityEvent();
        public UnityEvent onSetUsableData = new UnityEvent();
        public UnityEvent onSetStorageItemData = new UnityEvent();
        public UnityEvent onSetItemsContainerItemData = new UnityEvent();
        public UnityEvent onSetUnknowSourceData = new UnityEvent();
        public UnityEvent onNpcSellItemDialogAppear = new UnityEvent();
        public UnityEvent onNpcSellItemDialogDisappear = new UnityEvent();
        public UnityEvent onRefineItemDialogAppear = new UnityEvent();
        public UnityEvent onRefineItemDialogDisappear = new UnityEvent();
        public UnityEvent onDismantleItemDialogAppear = new UnityEvent();
        public UnityEvent onDismantleItemDialogDisappear = new UnityEvent();
        public UnityEvent onRepairItemDialogAppear = new UnityEvent();
        public UnityEvent onRepairItemDialogDisappear = new UnityEvent();
        public UnityEvent onEnhanceSocketItemDialogAppear = new UnityEvent();
        public UnityEvent onEnhanceSocketItemDialogDisappear = new UnityEvent();
        public UnityEvent onStorageDialogAppear = new UnityEvent();
        public UnityEvent onStorageDialogDisappear = new UnityEvent();
        public UnityEvent onEnterDealingState = new UnityEvent();
        public UnityEvent onExitDealingState = new UnityEvent();

        [Header("Options")]
        [Tooltip("UIs in this list will use cloned item data from this UI")]
        public UICharacterItem[] clones;
        public UICharacterItemDragHandler uiDragging;
        [Tooltip("UI which will be shown if this item level not reached max level")]
        public UICharacterItem uiNextLevelItem;
        [Tooltip("UIs in this list will be shown when set this item is equipment item and inventory type is `NonEquipItems` or `EquipItems`")]
        public UICharacterItem[] uiComparingEquipments;
        public bool showAmountWhenMaxIsOne;
        public bool showLevelAsDefault;
        public bool dontAppendRefineLevelToTitle;
        public bool dontShowComparingEquipments;

        protected bool isSellItemDialogAppeared;
        protected bool isRefineItemDialogAppeared;
        protected bool isDismantleItemDialogAppeared;
        protected bool isRepairItemDialogAppeared;
        protected bool isEnhanceSocketItemDialogAppeared;
        protected bool isStorageDialogAppeared;
        protected bool isDealingStateEntered;
        protected float lockRemainsDuration;
        protected bool dirtyIsLock;
        protected float coolDownRemainsDuration;
        protected bool dirtyIsCountDown;

        public bool IsSetupAsEquipSlot { get; private set; }
        public string EquipPosition { get; private set; }
        public byte EquipSlotIndex { get; private set; }

        public void SetupAsEquipSlot(string equipPosition, byte equipSlotIndex)
        {
            IsSetupAsEquipSlot = true;
            EquipPosition = equipPosition;
            EquipSlotIndex = equipSlotIndex;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            lockRemainsDuration = 0f;
            if (uiComparingEquipments != null)
            {
                foreach (UICharacterItem uiComparingEquipment in uiComparingEquipments)
                {
                    uiComparingEquipment.Hide();
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            float deltaTime = Time.deltaTime;
            UpdateLockRemainsDuration(deltaTime);

            ISkillItem skillItem = SkillItem;
            if (skillItem != null)
            {
                UpdateSkillCoolDownRemainsDuration(skillItem.UsingSkill, deltaTime);
            }
            else
            {
                if (uiTextCoolDownDuration != null)
                    uiTextCoolDownDuration.SetGameObjectActive(false);
                if (uiTextCoolDownRemainsDuration != null)
                    uiTextCoolDownRemainsDuration.SetGameObjectActive(false);
                if (imageCoolDownGage != null)
                    imageCoolDownGage.gameObject.SetActive(false);
            }
        }

        private void UpdateLockRemainsDuration(float deltaTime)
        {
            lockRemainsDuration = CharacterItem != null ? CharacterItem.lockRemainsDuration : 0f;

            if (lockRemainsDuration > 0f)
            {
                lockRemainsDuration -= deltaTime;
                if (lockRemainsDuration <= 0f)
                    lockRemainsDuration = 0f;
            }
            else
            {
                lockRemainsDuration = 0f;
            }

            if (uiTextLockRemainsDuration != null)
            {
                uiTextLockRemainsDuration.SetGameObjectActive(lockRemainsDuration > 0);
                uiTextLockRemainsDuration.text = string.Format(
                    LanguageManager.GetText(formatKeyLockRemainsDuration),
                    lockRemainsDuration.ToString("N0"));
            }

            bool isLock = lockRemainsDuration > 0f;
            if (dirtyIsLock != isLock)
            {
                dirtyIsLock = isLock;
                if (lockObjects != null)
                {
                    foreach (GameObject obj in lockObjects)
                    {
                        obj.SetActive(isLock);
                    }
                }
                if (noLockObjects != null)
                {
                    foreach (GameObject obj in noLockObjects)
                    {
                        obj.SetActive(!isLock);
                    }
                }
            }
        }

        private void UpdateSkillCoolDownRemainsDuration(BaseSkill skill, float deltaTime)
        {
            if (coolDownRemainsDuration > 0f)
            {
                coolDownRemainsDuration -= deltaTime;
                if (coolDownRemainsDuration <= 0f)
                    coolDownRemainsDuration = 0f;
            }
            else
            {
                coolDownRemainsDuration = 0f;
            }

            // Update UIs
            float coolDownDuration = skill.GetCoolDownDuration(Level);

            if (uiTextCoolDownDuration != null)
            {
                uiTextCoolDownDuration.SetGameObjectActive(skill.IsActive && coolDownDuration > 0f);
                uiTextCoolDownDuration.text = string.Format(
                    LanguageManager.GetText(formatKeyCoolDownDuration),
                    coolDownDuration.ToString("N0"));
            }

            if (uiTextCoolDownRemainsDuration != null)
            {
                uiTextCoolDownRemainsDuration.SetGameObjectActive(skill.IsActive && coolDownRemainsDuration > 0);
                uiTextCoolDownRemainsDuration.text = string.Format(
                    LanguageManager.GetText(formatKeyCoolDownRemainsDuration),
                    coolDownRemainsDuration.ToString("N0"));
            }

            if (imageCoolDownGage != null)
            {
                imageCoolDownGage.fillAmount = coolDownDuration <= 0 ? 0 : coolDownRemainsDuration / coolDownDuration;
                imageCoolDownGage.gameObject.SetActive(imageCoolDownGage.fillAmount > 0f);
            }

            bool isCountDown = coolDownRemainsDuration > 0f;
            if (dirtyIsCountDown != isCountDown)
            {
                dirtyIsCountDown = isCountDown;
                if (countDownObjects != null)
                {
                    foreach (GameObject obj in countDownObjects)
                    {
                        obj.SetActive(isCountDown);
                    }
                }
                if (noCountDownObjects != null)
                {
                    foreach (GameObject obj in noCountDownObjects)
                    {
                        obj.SetActive(!isCountDown);
                    }
                }
            }
        }

        protected override void UpdateUI()
        {
            // Update remains duration
            if (coolDownRemainsDuration <= 0f && Character != null && SkillItem != null && SkillItem.UsingSkill != null)
            {
                int indexOfSkillUsage = Character.IndexOfSkillUsage(SkillItem.UsingSkill.DataId, SkillUsageType.Skill);
                if (indexOfSkillUsage >= 0)
                    coolDownRemainsDuration = Character.SkillUsages[indexOfSkillUsage].coolDownRemainsDuration;
            }

            if (!IsOwningCharacter() || !IsVisible())
                return;

            UpdateShopUIVisibility(false);
            UpdateRefineItemUIVisibility(false);
            UpdateDismantleItemUIVisibility(false);
            UpdateRepairItemUIVisibility(false);
            UpdateEnhanceSocketUIVisibility(false);
            UpdateStorageUIVisibility(false);
            UpdateDealingState(false);
        }

        protected override void UpdateData()
        {
            // Update remains duration
            if (Character != null && SkillItem != null && SkillItem.UsingSkill != null)
            {
                int indexOfSkillUsage = Character.IndexOfSkillUsage(SkillItem.UsingSkill.DataId, SkillUsageType.Skill);
                if (indexOfSkillUsage >= 0 && Character.SkillUsages[indexOfSkillUsage].coolDownRemainsDuration - coolDownRemainsDuration > 1)
                    coolDownRemainsDuration = Character.SkillUsages[indexOfSkillUsage].coolDownRemainsDuration;
            }

            if (Level <= 0)
            {
                onSetLevelZeroData.Invoke();
            }
            else
            {
                onSetNonLevelZeroData.Invoke();
            }

            switch (InventoryType)
            {
                case InventoryType.Unknow:
                    onSetUnknowSourceData.Invoke();
                    break;
                case InventoryType.StorageItems:
                    onSetStorageItemData.Invoke();
                    break;
                case InventoryType.ItemsContainer:
                    onSetItemsContainerItemData.Invoke();
                    break;
                case InventoryType.NonEquipItems:
                case InventoryType.EquipItems:
                case InventoryType.EquipWeaponRight:
                case InventoryType.EquipWeaponLeft:
                    if (EquipmentItem != null)
                    {
                        if (InventoryType == InventoryType.NonEquipItems)
                            onSetUnEquippedData.Invoke();
                        else
                            onSetEquippedData.Invoke();
                    }
                    else
                    {
                        onSetUnEquippableData.Invoke();
                        if (UsableItem != null)
                            onSetUsableData.Invoke();
                    }
                    break;
            }

            if (uiTextTitle != null)
            {
                string str = string.Format(
                    LanguageManager.GetText(formatKeyTitle),
                    Item == null ? LanguageManager.GetUnknowTitle() : Item.Title);
                if (!dontAppendRefineLevelToTitle && EquipmentItem != null && Level > 1)
                {
                    str = string.Format(
                        LanguageManager.GetText(formatKeyTitleWithRefineLevel),
                        Item == null ? LanguageManager.GetUnknowTitle() : Item.Title,
                        (Level - 1).ToString("N0"));
                }
                uiTextTitle.text = str;
            }

            if (uiTextDescription != null)
            {
                uiTextDescription.text = string.Format(
                    LanguageManager.GetText(formatKeyDescription),
                    Item == null ? LanguageManager.GetUnknowDescription() : Item.Description);
            }

            if (uiTextRarity != null)
            {
                uiTextRarity.text = string.Format(
                    LanguageManager.GetText(formatKeyRarityTitle),
                    Item == null ? LanguageManager.GetUnknowTitle() : Item.RarityTitle);
            }

            if (uiTextLevel != null)
            {
                uiTextLevel.SetGameObjectActive(EquipmentItem != null || PetItem != null);
                if (EquipmentItem != null)
                {
                    if (showLevelAsDefault)
                    {
                        uiTextLevel.text = string.Format(
                            LanguageManager.GetText(formatKeyLevel),
                            Level.ToString("N0"));
                    }
                    else
                    {
                        uiTextLevel.text = string.Format(
                            LanguageManager.GetText(formatKeyRefineLevel),
                            (Level - 1).ToString("N0"));
                    }
                }
                else if (PetItem != null)
                {
                    uiTextLevel.text = string.Format(
                        LanguageManager.GetText(formatKeyLevel),
                        Level.ToString("N0"));
                }
            }

            if (imageIcon != null)
            {
                Sprite iconSprite = Item == null ? null : Item.Icon;
                imageIcon.gameObject.SetActive(iconSprite != null);
                imageIcon.sprite = iconSprite;
                imageIcon.preserveAspect = true;
            }

            if (imageRarity != null)
            {
                Sprite iconSprite = Item == null || Item.ItemRefine == null ? null : Item.ItemRefine.Icon;
                imageRarity.gameObject.SetActive(iconSprite != null);
                imageRarity.sprite = iconSprite;
            }

            if (uiTextItemType != null)
            {
                uiTextItemType.text = string.Format(
                    LanguageManager.GetText(formatKeyItemType),
                    Item == null ? LanguageManager.GetUnknowTitle() : Item.TypeTitle);
            }

            if (uiTextSellPrice != null)
            {
                uiTextSellPrice.text = string.Format(
                    LanguageManager.GetText(formatKeySellPrice),
                    Item == null ? "0" : Item.SellPrice.ToString("N0"));
            }

            if (uiTextStack != null)
            {
                string stackString;
                if (Item == null)
                {
                    stackString = string.Format(
                        LanguageManager.GetText(formatKeyStack),
                        "0",
                        "0");
                }
                else
                {
                    stackString = string.Format(
                        LanguageManager.GetText(formatKeyStack),
                        CharacterItem.amount.ToString("N0"),
                        Item.MaxStack);
                }
                uiTextStack.SetGameObjectActive(CharacterItem.NotEmptySlot() && (showAmountWhenMaxIsOne || (Item != null && Item.MaxStack > 1)));
                uiTextStack.text = stackString;
            }

            if (EquipmentItem != null && EquipmentItem.MaxDurability > 0)
            {
                if (uiTextDurability != null)
                {
                    uiTextDurability.SetGameObjectActive(true);
                    uiTextDurability.text = string.Format(
                        LanguageManager.GetText(formatKeyDurability),
                        CharacterItem.durability.ToString("N0"),
                        EquipmentItem.MaxDurability.ToString("N0"));
                }

                if (uiGageDurability != null)
                {
                    uiGageDurability.SetVisible(true);
                    uiGageDurability.Update(CharacterItem.durability, EquipmentItem.MaxDurability);
                }
            }
            else
            {
                if (uiTextDurability != null)
                    uiTextDurability.SetGameObjectActive(false);

                if (uiGageDurability != null)
                    uiGageDurability.SetVisible(false);
            }

            if (WeaponItem != null && WeaponItem.WeaponType.RequireAmmoType != null)
            {
                int currentAmmo = CharacterItem.ammo;
                int reserveAmmo = 0;
                if (GameInstance.PlayingCharacter != null)
                    reserveAmmo = GameInstance.PlayingCharacter.CountAmmos(WeaponItem.WeaponType.RequireAmmoType);

                if (uiTextCurrentAmmo != null)
                {
                    uiTextCurrentAmmo.SetGameObjectActive(WeaponItem.AmmoCapacity > 0);
                    uiTextCurrentAmmo.text = currentAmmo.ToString("N0");
                }

                if (uiTextReserveAmmo != null)
                {
                    uiTextReserveAmmo.SetGameObjectActive(true);
                    uiTextReserveAmmo.text = reserveAmmo.ToString("N0");
                }

                if (uiTextSumAmmo != null)
                {
                    uiTextSumAmmo.SetGameObjectActive(true);
                    uiTextSumAmmo.text = (currentAmmo + reserveAmmo).ToString("N0");
                }

                if (requireAmmoSymbols != null)
                {
                    foreach (GameObject symbol in requireAmmoSymbols)
                    {
                        symbol.SetActive(true);
                    }
                }

                if (noRequireAmmoSymbols != null)
                {
                    foreach (GameObject symbol in noRequireAmmoSymbols)
                    {
                        symbol.SetActive(false);
                    }
                }

                if (gageAmmo != null)
                {
                    gageAmmo.SetVisible(WeaponItem.AmmoCapacity > 1);
                    gageAmmo.Update(currentAmmo, WeaponItem.AmmoCapacity);
                }
            }
            else
            {
                if (uiTextCurrentAmmo != null)
                    uiTextCurrentAmmo.SetGameObjectActive(false);

                if (uiTextReserveAmmo != null)
                    uiTextReserveAmmo.SetGameObjectActive(false);

                if (uiTextSumAmmo != null)
                    uiTextSumAmmo.SetGameObjectActive(false);

                if (requireAmmoSymbols != null)
                {
                    foreach (GameObject symbol in requireAmmoSymbols)
                    {
                        symbol.SetActive(false);
                    }
                }

                if (noRequireAmmoSymbols != null)
                {
                    foreach (GameObject symbol in noRequireAmmoSymbols)
                    {
                        symbol.SetActive(true);
                    }
                }

                if (gageAmmo != null)
                    gageAmmo.SetVisible(false);
            }

            if (uiTextWeight != null)
            {
                uiTextWeight.text = string.Format(
                    LanguageManager.GetText(formatKeyWeight),
                    Item == null ? 0f.ToString("N2") : Item.Weight.ToString("N2"));
            }

            if (uiRequirement != null)
            {
                if (EquipmentItem == null ||
                    (EquipmentItem.Requirement.level <= 0 &&
                    !EquipmentItem.Requirement.HasAvailableClasses() &&
                    EquipmentItem.RequireAttributeAmounts.Count == 0))
                {
                    uiRequirement.Hide();
                }
                else
                {
                    uiRequirement.Show();
                    uiRequirement.Data = EquipmentItem;
                }
            }

            if (uiIncreaseStats != null)
            {
                CharacterStats stats = new CharacterStats();
                if (EquipmentItem != null)
                    stats += EquipmentItem.GetIncreaseStats(Level, CharacterItem.randomSeed);
                else if (SocketEnhancerItem != null)
                    stats += SocketEnhancerItem.SocketEnhanceEffect.stats;

                if (stats.IsEmpty())
                {
                    // Hide ui if stats is empty
                    uiIncreaseStats.Hide();
                }
                else
                {
                    uiIncreaseStats.displayType = UICharacterStats.DisplayType.Simple;
                    uiIncreaseStats.isBonus = true;
                    uiIncreaseStats.Show();
                    uiIncreaseStats.Data = stats;
                }
            }

            if (uiIncreaseStatsRate != null)
            {
                CharacterStats statsRate = new CharacterStats();
                if (EquipmentItem != null)
                    statsRate += EquipmentItem.GetIncreaseStatsRate(Level, CharacterItem.randomSeed);
                else if (SocketEnhancerItem != null)
                    statsRate += SocketEnhancerItem.SocketEnhanceEffect.statsRate;

                if (statsRate.IsEmpty())
                {
                    // Hide ui if stats is empty
                    uiIncreaseStatsRate.Hide();
                }
                else
                {
                    uiIncreaseStatsRate.displayType = UICharacterStats.DisplayType.Rate;
                    uiIncreaseStatsRate.isBonus = true;
                    uiIncreaseStatsRate.Show();
                    uiIncreaseStatsRate.Data = statsRate;
                }
            }

            if (uiIncreaseAttributes != null)
            {
                Dictionary<Attribute, float> attributes = null;
                if (EquipmentItem != null)
                    attributes = EquipmentItem.GetIncreaseAttributes(Level, CharacterItem.randomSeed);
                else if (SocketEnhancerItem != null)
                    attributes = GameDataHelpers.CombineAttributes(SocketEnhancerItem.SocketEnhanceEffect.attributes, attributes, 1f);

                if (attributes == null || attributes.Count == 0)
                {
                    // Hide ui if attributes is empty
                    uiIncreaseAttributes.Hide();
                }
                else
                {
                    uiIncreaseAttributes.displayType = UIAttributeAmounts.DisplayType.Simple;
                    uiIncreaseAttributes.isBonus = true;
                    uiIncreaseAttributes.Show();
                    uiIncreaseAttributes.Data = attributes;
                }
            }

            if (uiIncreaseAttributesRate != null)
            {
                Dictionary<Attribute, float> attributesRate = null;
                if (EquipmentItem != null)
                    attributesRate = EquipmentItem.GetIncreaseAttributesRate(Level, CharacterItem.randomSeed);
                else if (SocketEnhancerItem != null)
                    attributesRate = GameDataHelpers.CombineAttributes(SocketEnhancerItem.SocketEnhanceEffect.attributesRate, attributesRate, 1f);

                if (attributesRate == null || attributesRate.Count == 0)
                {
                    // Hide ui if attributes is empty
                    uiIncreaseAttributesRate.Hide();
                }
                else
                {
                    uiIncreaseAttributesRate.displayType = UIAttributeAmounts.DisplayType.Rate;
                    uiIncreaseAttributesRate.isBonus = true;
                    uiIncreaseAttributesRate.Show();
                    uiIncreaseAttributesRate.Data = attributesRate;
                }
            }

            if (uiIncreaseResistances != null)
            {
                Dictionary<DamageElement, float> resistances = null;
                if (EquipmentItem != null)
                    resistances = EquipmentItem.GetIncreaseResistances(Level, CharacterItem.randomSeed);
                else if (SocketEnhancerItem != null)
                    resistances = GameDataHelpers.CombineResistances(SocketEnhancerItem.SocketEnhanceEffect.resistances, resistances, 1f);

                if (resistances == null || resistances.Count == 0)
                {
                    // Hide ui if resistances is empty
                    uiIncreaseResistances.Hide();
                }
                else
                {
                    uiIncreaseResistances.isBonus = true;
                    uiIncreaseResistances.Show();
                    uiIncreaseResistances.Data = resistances;
                }
            }

            if (uiIncreaseArmors != null)
            {
                Dictionary<DamageElement, float> armors = null;
                if (EquipmentItem != null)
                    armors = EquipmentItem.GetIncreaseArmors(Level, CharacterItem.randomSeed);
                else if (SocketEnhancerItem != null)
                    armors = GameDataHelpers.CombineArmors(SocketEnhancerItem.SocketEnhanceEffect.armors, armors, 1f);

                if (armors == null || armors.Count == 0)
                {
                    // Hide ui if armors is empty
                    uiIncreaseArmors.Hide();
                }
                else
                {
                    uiIncreaseArmors.isBonus = true;
                    uiIncreaseArmors.Show();
                    uiIncreaseArmors.Data = armors;
                }
            }

            if (uiIncreaseDamages != null)
            {
                Dictionary<DamageElement, MinMaxFloat> damageAmounts = null;
                if (EquipmentItem != null)
                    damageAmounts = EquipmentItem.GetIncreaseDamages(Level, CharacterItem.randomSeed);
                else if (SocketEnhancerItem != null)
                    damageAmounts = GameDataHelpers.CombineDamages(SocketEnhancerItem.SocketEnhanceEffect.damages, damageAmounts, 1f);

                if (damageAmounts == null || damageAmounts.Count == 0)
                {
                    // Hide ui if damage amounts is empty
                    uiIncreaseDamages.Hide();
                }
                else
                {
                    uiIncreaseDamages.isBonus = true;
                    uiIncreaseDamages.Show();
                    uiIncreaseDamages.Data = damageAmounts;
                }
            }

            if (uiIncreaseSkillLevels != null)
            {
                Dictionary<BaseSkill, short> skillLevels = null;
                if (EquipmentItem != null)
                    skillLevels = EquipmentItem.GetIncreaseSkills(CharacterItem.randomSeed);
                else if (SocketEnhancerItem != null)
                    skillLevels = GameDataHelpers.CombineSkills(SocketEnhancerItem.SocketEnhanceEffect.skills, skillLevels);

                if (skillLevels == null || skillLevels.Count == 0)
                {
                    // Hide ui if skill levels is empty
                    uiIncreaseSkillLevels.Hide();
                }
                else
                {
                    uiIncreaseSkillLevels.displayType = UISkillLevels.DisplayType.Simple;
                    uiIncreaseSkillLevels.isBonus = true;
                    uiIncreaseSkillLevels.Show();
                    uiIncreaseSkillLevels.Data = skillLevels;
                }
            }

            if (uiEquipmentSet != null)
            {
                if (EquipmentItem == null || EquipmentItem.EquipmentSet == null || EquipmentItem.EquipmentSet.Effects.Length == 0)
                {
                    // Only equipment item has equipment set data
                    uiEquipmentSet.Hide();
                }
                else
                {
                    uiEquipmentSet.Show();
                    int equippedCount;
                    Character.GetCaches().EquipmentSets.TryGetValue(EquipmentItem.EquipmentSet, out equippedCount);
                    uiEquipmentSet.Data = new UIEquipmentSetData(EquipmentItem.EquipmentSet, equippedCount);
                }
            }

            if (uiEquipmentSockets != null)
            {
                if (EquipmentItem == null || EquipmentItem.MaxSocket <= 0)
                {
                    uiEquipmentSockets.Hide();
                }
                else
                {
                    uiEquipmentSockets.Show();
                    uiEquipmentSockets.Data = new UIEquipmentSocketsData(CharacterItem.Sockets, EquipmentItem.MaxSocket);
                }
            }

            if (uiRefineItem != null)
            {
                if (EquipmentItem == null)
                {
                    uiRefineItem.Hide();
                }
                else
                {
                    uiRefineItem.Show();
                    uiRefineItem.Data = new UIOwningCharacterItemData(InventoryType, IndexOfData);
                }
            }

            if (uiDismantleItem != null)
            {
                if (!GameInstance.Singleton.dismantleFilter.Filter(CharacterItem))
                {
                    uiDismantleItem.Hide();
                }
                else
                {
                    uiDismantleItem.Show();
                    uiDismantleItem.Data = new UIOwningCharacterItemData(InventoryType, IndexOfData);
                }
            }

            if (uiRepairItem != null)
            {
                if (EquipmentItem == null)
                {
                    uiRepairItem.Hide();
                }
                else
                {
                    uiRepairItem.Show();
                    uiRepairItem.Data = new UIOwningCharacterItemData(InventoryType, IndexOfData);
                }
            }

            if (uiArmorAmount != null)
            {
                if (DefendItem == null)
                {
                    uiArmorAmount.Hide();
                }
                else
                {
                    uiArmorAmount.Show();
                    KeyValuePair<DamageElement, float> kvPair = CharacterItem.GetArmorAmount();
                    uiArmorAmount.Data = new UIArmorAmountData(kvPair.Key, kvPair.Value);
                }
            }

            if (uiDamageAmount != null)
            {
                if (WeaponItem == null)
                {
                    uiDamageAmount.Hide();
                }
                else
                {
                    uiDamageAmount.Show();
                    KeyValuePair<DamageElement, MinMaxFloat> kvPair = CharacterItem.GetDamageAmount(null);
                    uiDamageAmount.Data = new UIDamageElementAmountData(kvPair.Key, kvPair.Value);
                }
            }

            if (PetItem != null && PetItem.PetEntity != null)
            {
                int[] expTree = GameInstance.Singleton.ExpTree;
                int currentExp = 0;
                int nextLevelExp = 0;
                if (CharacterItem.GetNextLevelExp() > 0)
                {
                    currentExp = CharacterItem.exp;
                    nextLevelExp = CharacterItem.GetNextLevelExp();
                }
                else if (Level - 2 > 0 && Level - 2 < expTree.Length)
                {
                    int maxExp = expTree[Level - 2];
                    currentExp = maxExp;
                    nextLevelExp = maxExp;
                }

                if (uiTextExp != null)
                {
                    uiTextExp.SetGameObjectActive(true);
                    uiTextExp.text = string.Format(
                        LanguageManager.GetText(formatKeyExp),
                        currentExp.ToString("N0"),
                        nextLevelExp.ToString("N0"));
                }

                if (uiGageExp != null)
                {
                    uiGageExp.SetVisible(true);
                    uiGageExp.Update(currentExp, nextLevelExp);
                }
            }
            else
            {
                if (uiTextExp != null)
                    uiTextExp.SetGameObjectActive(false);

                if (uiGageExp != null)
                    uiGageExp.SetVisible(false);
            }

            if (uiTextBuilding != null)
            {
                if (BuildingItem == null || BuildingItem.BuildingEntity == null)
                {
                    uiTextBuilding.SetGameObjectActive(false);
                }
                else
                {
                    uiTextBuilding.SetGameObjectActive(true);
                    uiTextBuilding.text = string.Format(
                        LanguageManager.GetText(formatKeyBuilding),
                        BuildingItem.BuildingEntity.Title);
                }
            }

            if (uiTextPet != null)
            {
                if (PetItem == null || PetItem.PetEntity == null)
                {
                    uiTextPet.SetGameObjectActive(false);
                }
                else
                {
                    uiTextPet.SetGameObjectActive(true);
                    uiTextPet.text = string.Format(
                        LanguageManager.GetText(formatKeyPet),
                        PetItem.PetEntity.Title);
                }
            }

            if (uiTextMount != null)
            {
                if (MountItem == null || MountItem.MountEntity == null)
                {
                    uiTextMount.SetGameObjectActive(false);
                }
                else
                {
                    uiTextMount.SetGameObjectActive(true);
                    uiTextMount.text = string.Format(
                        LanguageManager.GetText(formatKeyMount),
                        MountItem.MountEntity.Title);
                }
            }

            if (uiTextSkill != null)
            {
                if (SkillItem == null || SkillItem.UsingSkill == null)
                {
                    uiTextSkill.SetGameObjectActive(false);
                }
                else
                {
                    uiTextSkill.SetGameObjectActive(true);
                    uiTextSkill.text = string.Format(
                        LanguageManager.GetText(formatKeySkill),
                        SkillItem.UsingSkill.Title,
                        SkillItem.UsingSkillLevel);
                }
            }

            if (clones != null && clones.Length > 0)
            {
                for (int i = 0; i < clones.Length; ++i)
                {
                    if (clones[i] == null) continue;
                    clones[i].Data = Data;
                }
            }

            if (uiNextLevelItem != null)
            {
                if (Level + 1 > Item.MaxLevel)
                {
                    uiNextLevelItem.Hide();
                }
                else
                {
                    uiNextLevelItem.Setup(new UICharacterItemData(CharacterItem, (short)(Level + 1), InventoryType), Character, IndexOfData);
                    uiNextLevelItem.Show();
                }
            }

            if (uiComparingEquipments != null && !dontShowComparingEquipments)
            {
                foreach (UICharacterItem uiComparingEquipment in uiComparingEquipments)
                {
                    uiComparingEquipment.Hide();
                }
                if (IsOwningCharacter())
                {
                    int comparingEquipmentIndex = 0;
                    if (WeaponItem != null)
                    {
                        if (!GameInstance.PlayingCharacter.EquipWeapons.rightHand.IsEmptySlot() &&
                            !GameInstance.PlayingCharacter.EquipWeapons.rightHand.id.Equals(CharacterItem.id) &&
                            GameInstance.PlayingCharacter.EquipWeapons.rightHand.GetWeaponItem() != null)
                        {
                            SetupAndShowUIComparingEquipment(comparingEquipmentIndex,
                                GameInstance.PlayingCharacter.EquipWeapons.rightHand,
                                InventoryType.EquipWeaponRight, 0);
                            comparingEquipmentIndex++;
                        }
                        if (!GameInstance.PlayingCharacter.EquipWeapons.leftHand.IsEmptySlot() &&
                            !GameInstance.PlayingCharacter.EquipWeapons.leftHand.id.Equals(CharacterItem.id) &&
                            GameInstance.PlayingCharacter.EquipWeapons.leftHand.GetWeaponItem() != null)
                        {
                            SetupAndShowUIComparingEquipment(comparingEquipmentIndex,
                                GameInstance.PlayingCharacter.EquipWeapons.leftHand,
                                InventoryType.EquipWeaponLeft, 0);
                            comparingEquipmentIndex++;
                        }
                    }
                    if (ShieldItem != null)
                    {
                        if (!GameInstance.PlayingCharacter.EquipWeapons.rightHand.IsEmptySlot() &&
                            !GameInstance.PlayingCharacter.EquipWeapons.rightHand.id.Equals(CharacterItem.id) &&
                            GameInstance.PlayingCharacter.EquipWeapons.rightHand.GetShieldItem() != null)
                        {
                            SetupAndShowUIComparingEquipment(comparingEquipmentIndex,
                                GameInstance.PlayingCharacter.EquipWeapons.rightHand,
                                InventoryType.EquipWeaponRight, 0);
                            comparingEquipmentIndex++;
                        }
                        if (!GameInstance.PlayingCharacter.EquipWeapons.leftHand.IsEmptySlot() &&
                            !GameInstance.PlayingCharacter.EquipWeapons.leftHand.id.Equals(CharacterItem.id) &&
                            GameInstance.PlayingCharacter.EquipWeapons.leftHand.GetShieldItem() != null)
                        {
                            SetupAndShowUIComparingEquipment(comparingEquipmentIndex,
                                GameInstance.PlayingCharacter.EquipWeapons.leftHand,
                                InventoryType.EquipWeaponLeft, 0);
                            comparingEquipmentIndex++;
                        }
                    }
                    if (ArmorItem != null)
                    {
                        CharacterItem equipItem;
                        for (int equipItemIndex = 0; equipItemIndex < GameInstance.PlayingCharacter.EquipItems.Count; ++equipItemIndex)
                        {
                            equipItem = GameInstance.PlayingCharacter.EquipItems[equipItemIndex];
                            if (!equipItem.IsEmptySlot() &&
                                !equipItem.id.Equals(CharacterItem.id) &&
                                equipItem.GetArmorItem() != null &&
                                equipItem.GetArmorItem().ArmorType == ArmorItem.ArmorType)
                            {
                                SetupAndShowUIComparingEquipment(comparingEquipmentIndex, equipItem, InventoryType.EquipItems, equipItemIndex);
                                comparingEquipmentIndex++;
                            }
                        }
                    }
                }
            }

            UpdateShopUIVisibility(true);
            UpdateRefineItemUIVisibility(true);
            UpdateDismantleItemUIVisibility(true);
            UpdateRepairItemUIVisibility(true);
            UpdateEnhanceSocketUIVisibility(true);
            UpdateStorageUIVisibility(true);
            UpdateDealingState(true);
        }

        private void SetupAndShowUIComparingEquipment(int index, CharacterItem characterItem, InventoryType inventoryType, int indexOfData)
        {
            if (uiComparingEquipments == null || index >= uiComparingEquipments.Length)
                return;
            uiComparingEquipments[index].Setup(new UICharacterItemData(characterItem, inventoryType), GameInstance.PlayingCharacter, indexOfData);
            uiComparingEquipments[index].Show();
        }

        private void UpdateShopUIVisibility(bool initData)
        {
            if (!IsOwningCharacter())
            {
                if (initData || isSellItemDialogAppeared)
                {
                    isSellItemDialogAppeared = false;
                    if (onNpcSellItemDialogDisappear != null)
                        onNpcSellItemDialogDisappear.Invoke();
                }
                return;
            }
            // Check visible item dialog
            if (GameInstance.ItemUIVisibilityManager.IsShopDialogVisible() &&
                InventoryType == InventoryType.NonEquipItems)
            {
                if (initData || !isSellItemDialogAppeared)
                {
                    isSellItemDialogAppeared = true;
                    if (onNpcSellItemDialogAppear != null)
                        onNpcSellItemDialogAppear.Invoke();
                }
            }
            else
            {
                if (initData || isSellItemDialogAppeared)
                {
                    isSellItemDialogAppeared = false;
                    if (onNpcSellItemDialogDisappear != null)
                        onNpcSellItemDialogDisappear.Invoke();
                }
            }
        }

        private void UpdateRefineItemUIVisibility(bool initData)
        {
            if (!IsOwningCharacter())
            {
                if (initData || isRefineItemDialogAppeared)
                {
                    isRefineItemDialogAppeared = false;
                    if (onRefineItemDialogDisappear != null)
                        onRefineItemDialogDisappear.Invoke();
                }
                return;
            }
            // Check visible item dialog
            if (GameInstance.ItemUIVisibilityManager.IsRefineItemDialogVisible() &&
                EquipmentItem != null && InventoryType != InventoryType.StorageItems)
            {
                if (initData || !isRefineItemDialogAppeared)
                {
                    isRefineItemDialogAppeared = true;
                    if (onRefineItemDialogAppear != null)
                        onRefineItemDialogAppear.Invoke();
                }
            }
            else
            {
                if (initData || isRefineItemDialogAppeared)
                {
                    isRefineItemDialogAppeared = false;
                    if (onRefineItemDialogDisappear != null)
                        onRefineItemDialogDisappear.Invoke();
                }
            }
        }

        private void UpdateDismantleItemUIVisibility(bool initData)
        {
            if (!IsOwningCharacter())
            {
                if (initData || isDismantleItemDialogAppeared)
                {
                    isDismantleItemDialogAppeared = false;
                    if (onDismantleItemDialogDisappear != null)
                        onDismantleItemDialogDisappear.Invoke();
                }
                return;
            }
            // Check visible item dialog
            if (GameInstance.ItemUIVisibilityManager.IsDismantleItemDialogVisible() &&
                GameInstance.Singleton.dismantleFilter.Filter(CharacterItem) &&
                InventoryType == InventoryType.NonEquipItems)
            {
                if (initData || !isDismantleItemDialogAppeared)
                {
                    isDismantleItemDialogAppeared = true;
                    if (onDismantleItemDialogAppear != null)
                        onDismantleItemDialogAppear.Invoke();
                }
            }
            else
            {
                if (initData || isDismantleItemDialogAppeared)
                {
                    isDismantleItemDialogAppeared = false;
                    if (onDismantleItemDialogDisappear != null)
                        onDismantleItemDialogDisappear.Invoke();
                }
            }
        }

        private void UpdateRepairItemUIVisibility(bool initData)
        {
            if (!IsOwningCharacter())
            {
                if (initData || isRepairItemDialogAppeared)
                {
                    isRepairItemDialogAppeared = false;
                    if (onRepairItemDialogDisappear != null)
                        onRepairItemDialogDisappear.Invoke();
                }
                return;
            }
            // Check visible item dialog
            if (GameInstance.ItemUIVisibilityManager.IsRepairItemDialogVisible() &&
                EquipmentItem != null && InventoryType != InventoryType.StorageItems)
            {
                if (initData || !isRepairItemDialogAppeared)
                {
                    isRepairItemDialogAppeared = true;
                    if (onRepairItemDialogAppear != null)
                        onRepairItemDialogAppear.Invoke();
                }
            }
            else
            {
                if (initData || isRepairItemDialogAppeared)
                {
                    isRepairItemDialogAppeared = false;
                    if (onRepairItemDialogDisappear != null)
                        onRepairItemDialogDisappear.Invoke();
                }
            }
        }

        private void UpdateEnhanceSocketUIVisibility(bool initData)
        {
            if (!IsOwningCharacter())
            {
                if (initData || isEnhanceSocketItemDialogAppeared)
                {
                    isEnhanceSocketItemDialogAppeared = false;
                    if (onEnhanceSocketItemDialogDisappear != null)
                        onEnhanceSocketItemDialogDisappear.Invoke();
                }
                return;
            }
            // Check visible item dialog
            if (GameInstance.ItemUIVisibilityManager.IsEnhanceSocketItemDialogVisible() &&
                EquipmentItem != null && InventoryType != InventoryType.StorageItems)
            {
                if (initData || !isEnhanceSocketItemDialogAppeared)
                {
                    isEnhanceSocketItemDialogAppeared = true;
                    if (onEnhanceSocketItemDialogAppear != null)
                        onEnhanceSocketItemDialogAppear.Invoke();
                }
            }
            else
            {
                if (initData || isEnhanceSocketItemDialogAppeared)
                {
                    isEnhanceSocketItemDialogAppeared = false;
                    if (onEnhanceSocketItemDialogDisappear != null)
                        onEnhanceSocketItemDialogDisappear.Invoke();
                }
            }
        }

        private void UpdateStorageUIVisibility(bool initData)
        {
            if (!IsOwningCharacter())
            {
                if (initData || isStorageDialogAppeared)
                {
                    isStorageDialogAppeared = false;
                    if (onStorageDialogDisappear != null)
                        onStorageDialogDisappear.Invoke();
                }
                return;
            }
            // Check visible item dialog
            if (GameInstance.ItemUIVisibilityManager.IsStorageDialogVisible() &&
                InventoryType == InventoryType.NonEquipItems)
            {
                if (initData || !isStorageDialogAppeared)
                {
                    isStorageDialogAppeared = true;
                    if (onStorageDialogAppear != null)
                        onStorageDialogAppear.Invoke();
                }
            }
            else
            {
                if (initData || isStorageDialogAppeared)
                {
                    isStorageDialogAppeared = false;
                    if (onStorageDialogDisappear != null)
                        onStorageDialogDisappear.Invoke();
                }
            }
        }

        private void UpdateDealingState(bool initData)
        {
            if (!IsOwningCharacter())
            {
                if (initData || isDealingStateEntered)
                {
                    isDealingStateEntered = false;
                    if (onExitDealingState != null)
                        onExitDealingState.Invoke();
                }
                return;
            }
            // Check visible dealing dialog
            if (BaseUISceneGameplay.Singleton != null &&
                BaseUISceneGameplay.Singleton.IsDealingDialogVisibleWithDealingState() &&
                InventoryType == InventoryType.NonEquipItems)
            {
                if (initData || !isDealingStateEntered)
                {
                    isDealingStateEntered = true;
                    if (onEnterDealingState != null)
                        onEnterDealingState.Invoke();
                }
            }
            else
            {
                if (initData || isDealingStateEntered)
                {
                    isDealingStateEntered = false;
                    if (onExitDealingState != null)
                        onExitDealingState.Invoke();
                }
            }
        }

        public void OnClickEquip()
        {
            // Only unequpped equipment can be equipped
            if (!IsOwningCharacter() || InventoryType != InventoryType.NonEquipItems)
                return;

            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();

            GameInstance.ClientInventoryHandlers.RequestEquipItem(
                GameInstance.PlayingCharacter,
                (short)IndexOfData,
                ClientInventoryActions.ResponseEquipArmor,
                ClientInventoryActions.ResponseEquipWeapon);
        }

        public void OnClickUnEquip()
        {
            // Only equipped equipment can be unequipped
            if (!IsOwningCharacter() || InventoryType == InventoryType.NonEquipItems)
                return;

            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();

            GameInstance.ClientInventoryHandlers.RequestUnEquipItem(
                InventoryType,
                (short)IndexOfData,
                CharacterItem.equipSlotIndex,
                -1,
                ClientInventoryActions.ResponseUnEquipArmor,
                ClientInventoryActions.ResponseUnEquipWeapon);
        }

        public void OnClickUse()
        {
            if (!IsOwningCharacter())
                return;

            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();

            // Controlling by hotkey controller
            UICharacterHotkeys.SetupAndUseOtherHotkey(HotkeyType.Item, CharacterItem.id);
        }

        #region Drop Item Functions
        public void OnClickDrop()
        {
            // Only unequipped equipment can be dropped
            if (!IsOwningCharacter() || InventoryType != InventoryType.NonEquipItems || GameInstance.PlayingCharacterEntity == null)
                return;

            switch (GameInstance.Singleton.playerDropItemMode)
            {
                case PlayerDropItemMode.DestroyItem:
                    if (CharacterItem.amount == 1)
                    {
                        UISceneGlobal.Singleton.ShowMessageDialog(
                            LanguageManager.GetText(UITextKeys.UI_DESTROY_ITEM.ToString()),
                            LanguageManager.GetText(UITextKeys.UI_DESTROY_ITEM_DESCRIPTION.ToString()),
                            false, true, true, false, null, () =>
                            {
                                OnDropAmountConfirmed(1);
                            });
                    }
                    else
                    {
                        UISceneGlobal.Singleton.ShowInputDialog(
                            LanguageManager.GetText(UITextKeys.UI_DESTROY_ITEM.ToString()),
                            LanguageManager.GetText(UITextKeys.UI_DESTROY_ITEM_DESCRIPTION.ToString()),
                            OnDropAmountConfirmed, 1, CharacterItem.amount, CharacterItem.amount);
                    }
                    break;
                case PlayerDropItemMode.DropOnGround:
                    if (CharacterItem.amount == 1)
                    {
                        OnDropAmountConfirmed(1);
                    }
                    else
                    {
                        UISceneGlobal.Singleton.ShowInputDialog(
                            LanguageManager.GetText(UITextKeys.UI_DROP_ITEM.ToString()),
                            LanguageManager.GetText(UITextKeys.UI_DROP_ITEM_DESCRIPTION.ToString()),
                            OnDropAmountConfirmed, 1, CharacterItem.amount, CharacterItem.amount);
                    }
                    break;
            }
        }

        private void OnDropAmountConfirmed(int amount)
        {
            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();
            GameInstance.PlayingCharacterEntity.CallServerDropItem((short)IndexOfData, (short)amount);
        }
        #endregion

        #region Sell Item Functions
        public void OnClickSell()
        {
            // Only unequipped equipment can be sold
            if (!IsOwningCharacter() || InventoryType != InventoryType.NonEquipItems)
                return;

            if (CharacterItem.amount == 1)
            {
                OnSellItemAmountConfirmed(1);
            }
            else
            {
                UISceneGlobal.Singleton.ShowInputDialog(LanguageManager.GetText(UITextKeys.UI_SELL_ITEM.ToString()), LanguageManager.GetText(UITextKeys.UI_SELL_ITEM_DESCRIPTION.ToString()), OnSellItemAmountConfirmed, 1, CharacterItem.amount, CharacterItem.amount);
            }
        }

        private void OnSellItemAmountConfirmed(int amount)
        {
            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();
            GameInstance.ClientInventoryHandlers.RequestSellItem(new RequestSellItemMessage()
            {
                index = (short)IndexOfData,
                amount = (short)amount,
            }, ClientInventoryActions.ResponseSellItem);
        }
        #endregion

        #region Set Dealing Item Functions
        public void OnClickSetDealingItem()
        {
            // Only unequipped equipment can be offered
            if (!IsOwningCharacter() || InventoryType != InventoryType.NonEquipItems)
                return;

            if (CharacterItem.amount == 1)
            {
                OnSetDealingItemAmountConfirmed(1);
            }
            else
            {
                UISceneGlobal.Singleton.ShowInputDialog(LanguageManager.GetText(UITextKeys.UI_OFFER_ITEM.ToString()), LanguageManager.GetText(UITextKeys.UI_OFFER_ITEM_DESCRIPTION.ToString()), OnSetDealingItemAmountConfirmed, 1, CharacterItem.amount, CharacterItem.amount);
            }
        }

        private void OnSetDealingItemAmountConfirmed(int amount)
        {
            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();
            GameInstance.PlayingCharacterEntity.Dealing.CallServerSetDealingItem((short)IndexOfData, (short)amount);
        }
        #endregion

        #region Move To Storage Functions
        public void OnClickMoveToStorage()
        {
            OnClickMoveToStorage(-1);
        }

        public void OnClickMoveToStorage(short storageItemIndex)
        {
            // Only unequipped equipment can be moved to storage
            if (!IsOwningCharacter() || (InventoryType != InventoryType.NonEquipItems && InventoryType != InventoryType.EquipItems && InventoryType != InventoryType.EquipWeaponRight && InventoryType != InventoryType.EquipWeaponLeft))
                return;

            if (CharacterItem.amount == 1)
            {
                OnClickMoveToStorageConfirmed(storageItemIndex, 1);
            }
            else
            {
                UISceneGlobal.Singleton.ShowInputDialog(LanguageManager.GetText(UITextKeys.UI_MOVE_ITEM_TO_STORAGE.ToString()), LanguageManager.GetText(UITextKeys.UI_MOVE_ITEM_TO_STORAGE_DESCRIPTION.ToString()), (amount) =>
                {
                    OnClickMoveToStorageConfirmed(storageItemIndex, amount);
                }, 1, CharacterItem.amount, CharacterItem.amount);
            }
        }

        private void OnClickMoveToStorageConfirmed(short storageItemIndex, int amount)
        {
            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();
            StorageType storageType = GameInstance.OpenedStorageType;
            string storageOwnerId = GameInstance.OpenedStorageOwnerId;
            GameInstance.ClientStorageHandlers.RequestMoveItemToStorage(new RequestMoveItemToStorageMessage()
            {
                storageType = storageType,
                storageOwnerId = storageOwnerId,
                inventoryItemIndex = (short)IndexOfData,
                inventoryItemAmount = (short)amount,
                storageItemIndex = storageItemIndex,
                inventoryType = InventoryType,
                equipSlotIndexOrWeaponSet = EquipSlotIndex,
            }, ClientStorageActions.ResponseMoveItemToStorage);
        }
        #endregion

        #region Move From Storage Functions
        public void OnClickMoveFromStorage()
        {
            OnClickMoveFromStorage(InventoryType.NonEquipItems, Character.EquipWeaponSet, -1);
        }

        public void OnClickMoveFromStorage(InventoryType inventoryType, byte equipSlotIndex, short inventoryItemIndex)
        {
            // Only storage items can be moved from storage
            if (!IsOwningCharacter() || InventoryType != InventoryType.StorageItems)
                return;

            if (CharacterItem.amount == 1)
            {
                OnClickMoveFromStorageConfirmed(inventoryType, equipSlotIndex, inventoryItemIndex, 1);
            }
            else
            {
                UISceneGlobal.Singleton.ShowInputDialog(LanguageManager.GetText(UITextKeys.UI_MOVE_ITEM_FROM_STORAGE.ToString()), LanguageManager.GetText(UITextKeys.UI_MOVE_ITEM_FROM_STORAGE_DESCRIPTION.ToString()), (amount) =>
                {
                    OnClickMoveFromStorageConfirmed(inventoryType, equipSlotIndex, inventoryItemIndex, amount);
                }, 1, CharacterItem.amount, CharacterItem.amount);
            }
        }

        private void OnClickMoveFromStorageConfirmed(InventoryType inventoryType, byte equipSlotIndex, short inventoryItemIndex, int amount)
        {
            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();
            StorageType storageType = GameInstance.OpenedStorageType;
            string storageOwnerId = GameInstance.OpenedStorageOwnerId;
            GameInstance.ClientStorageHandlers.RequestMoveItemFromStorage(new RequestMoveItemFromStorageMessage()
            {
                storageType = storageType,
                storageOwnerId = storageOwnerId,
                storageItemIndex = (short)IndexOfData,
                storageItemAmount = (short)amount,
                inventoryItemIndex = inventoryItemIndex,
                inventoryType = inventoryType,
                equipSlotIndexOrWeaponSet = equipSlotIndex,
            }, ClientStorageActions.ResponseMoveItemFromStorage);
        }
        #endregion

        #region Move From Items Container Functions
        public void OnClickPickUpFromContainer()
        {
            // Only Items container items can be moved from ItemsContainer
            if (!IsOwningCharacter() || InventoryType != InventoryType.ItemsContainer)
                return;

            if (CharacterItem.amount == 1)
            {
                OnClickPickUpFromContainerConfirmed(1);
            }
            else
            {
                UISceneGlobal.Singleton.ShowInputDialog(LanguageManager.GetText(UITextKeys.UI_MOVE_ITEM_FROM_ITEMS_CONTAINER.ToString()), LanguageManager.GetText(UITextKeys.UI_MOVE_ITEM_FROM_ITEMS_CONTAINER_DESCRIPTION.ToString()), (amount) =>
                {
                    OnClickPickUpFromContainerConfirmed(amount);
                }, 1, CharacterItem.amount, CharacterItem.amount);
            }
        }

        private void OnClickPickUpFromContainerConfirmed(int amount)
        {
            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();
            GameInstance.PlayingCharacterEntity.CallServerPickupItemFromContainer(GameInstance.ItemsContainerUIVisibilityManager.ItemsContainerEntity.ObjectId, IndexOfData, (short)amount);
        }
        #endregion

        #region Set Refine Item Functions
        /// <summary>
        /// Use this function to set item which you want to refine to `UIRefineItem` instance
        /// </summary>
        public void OnClickSetRefineItem()
        {
            // Only owning character can refine item
            if (!IsOwningCharacter())
                return;

            if (EquipmentItem != null)
            {
                GameInstance.ItemUIVisibilityManager.ShowRefineItemDialog(InventoryType, IndexOfData);
                if (selectionManager != null)
                    selectionManager.DeselectSelectedUI();
            }
        }

        /// <summary>
        /// Use this function to refine the item
        /// </summary>
        public void OnClickRefineItem()
        {
            if (!GameInstance.Singleton.canRefineItemByPlayer)
                return;

            // Only owning character can refine item
            if (!IsOwningCharacter())
                return;

            if (EquipmentItem != null)
            {
                GameInstance.ClientInventoryHandlers.RequestRefineItem(new RequestRefineItemMessage()
                {
                    inventoryType = InventoryType,
                    index = (short)IndexOfData,
                }, ClientInventoryActions.ResponseRefineItem);
            }
        }
        #endregion

        #region Set Dismantle Item Functions
        /// <summary>
        /// Use this function to set item which you want to dismantle to `UIDismantleItem` instance
        /// </summary>
        public void OnClickSetDismantleItem()
        {
            // Only unequipped equipment can be dismantled
            if (!IsOwningCharacter() || InventoryType != InventoryType.NonEquipItems)
                return;

            if (!GameInstance.Singleton.dismantleFilter.Filter(CharacterItem))
                return;

            GameInstance.ItemUIVisibilityManager.ShowDismantleItemDialog(InventoryType, IndexOfData);
            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();
        }

        /// <summary>
        /// Use this function to dismantle the item
        /// </summary>
        public void OnClickDismantleItem()
        {
            if (!GameInstance.Singleton.canDismantleItemByPlayer)
                return;

            // Only unequipped equipment can be dismantled
            if (!IsOwningCharacter() || InventoryType != InventoryType.NonEquipItems)
                return;

            if (!GameInstance.Singleton.dismantleFilter.Filter(CharacterItem))
                return;

            if (CharacterItem.amount == 1)
            {
                OnDismantleItemAmountConfirmed(1);
            }
            else
            {
                UISceneGlobal.Singleton.ShowInputDialog(LanguageManager.GetText(UITextKeys.UI_DISMANTLE_ITEM.ToString()), LanguageManager.GetText(UITextKeys.UI_DISMANTLE_ITEM_DESCRIPTION.ToString()), OnDismantleItemAmountConfirmed, 1, CharacterItem.amount, CharacterItem.amount);
            }
        }

        private void OnDismantleItemAmountConfirmed(int amount)
        {
            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();
            GameInstance.ClientInventoryHandlers.RequestDismantleItem(new RequestDismantleItemMessage()
            {
                index = (short)IndexOfData,
                amount = (short)amount,
            }, ClientInventoryActions.ResponseDismantleItem);
        }
        #endregion

        #region Set Repair Item Functions
        /// <summary>
        /// Use this function to set item which you want to repair to `UIRepairItem` instance
        /// </summary>
        public void OnClickSetRepairItem()
        {
            // Only owning character can repair item
            if (!IsOwningCharacter())
                return;

            if (EquipmentItem != null)
            {
                GameInstance.ItemUIVisibilityManager.ShowRepairItemDialog(InventoryType, IndexOfData);
                if (selectionManager != null)
                    selectionManager.DeselectSelectedUI();
            }
        }

        /// <summary>
        /// Use this function to repair the item
        /// </summary>
        public void OnClickRepairItem()
        {
            if (!GameInstance.Singleton.canRepairItemByPlayer)
                return;

            // Only owning character can repair item
            if (!IsOwningCharacter())
                return;

            if (EquipmentItem != null)
            {
                GameInstance.ClientInventoryHandlers.RequestRepairItem(new RequestRepairItemMessage()
                {
                    inventoryType = InventoryType,
                    index = (short)IndexOfData,
                }, ClientInventoryActions.ResponseRepairItem);
            }
        }
        #endregion

        #region Set Enhance Socket Item Functions
        /// <summary>
        /// Use this function to set item which you want to enhance (by socket) to `UIEnhanceSocketItem` instance
        /// </summary>
        public void OnClickSetEnhanceSocketItem()
        {
            // Only owning character can enhance item
            if (!IsOwningCharacter())
                return;

            if (EquipmentItem != null)
            {
                GameInstance.ItemUIVisibilityManager.ShowEnhanceSocketItemDialog(InventoryType, IndexOfData);
                if (selectionManager != null)
                    selectionManager.DeselectSelectedUI();
            }
        }
        #endregion
    }
}
