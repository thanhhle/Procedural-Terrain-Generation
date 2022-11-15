﻿using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = "Mount Item", menuName = "Create GameData/Item/Mount Item", order = -4883)]
    public partial class MountItem : BaseItem, IMountItem
    {
        public override string TypeTitle
        {
            get { return LanguageManager.GetText(UIItemTypeKeys.UI_ITEM_TYPE_MOUNT.ToString()); }
        }

        public override ItemType ItemType
        {
            get { return ItemType.Mount; }
        }

        [Category(3, "Mount Settings")]
        [SerializeField]
        private VehicleEntity mountEntity = null;
        public VehicleEntity MountEntity
        {
            get { return mountEntity; }
        }

        [SerializeField]
        private float useItemCooldown = 0f;
        public float UseItemCooldown
        {
            get { return useItemCooldown; }
        }

        public void UseItem(BaseCharacterEntity characterEntity, short itemIndex, CharacterItem characterItem)
        {
            if (!characterEntity.CanUseItem() || characterItem.level <= 0)
                return;

            characterEntity.Mount(MountEntity);
        }

        public bool HasCustomAimControls()
        {
            return false;
        }

        public AimPosition UpdateAimControls(Vector2 aimAxes, params object[] data)
        {
            return default;
        }

        public void FinishAimControls(bool isCancel)
        {

        }

        public bool IsChanneledAbility()
        {
            return false;
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            GameInstance.AddVehicleEntities(MountEntity);
        }
    }
}
