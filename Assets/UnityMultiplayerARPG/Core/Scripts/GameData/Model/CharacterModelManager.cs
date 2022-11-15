﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    [DisallowMultipleComponent]
    public partial class CharacterModelManager : BaseGameEntityComponent<BaseGameEntity>
    {
        public const byte HIDE_SETTER_ENTITY = 0;
        public const byte HIDE_SETTER_CONTROLLER = 1;

        [Header("TPS Model Settings")]
        [SerializeField]
        [FormerlySerializedAs("mainModel")]
        private BaseCharacterModel mainTpsModel = null;
        public BaseCharacterModel MainTpsModel { get { return mainTpsModel; } set { mainTpsModel = value; } }

        [Header("FPS Model Settings")]
        [SerializeField]
        private BaseCharacterModel fpsModelPrefab = null;
        [SerializeField]
        [FormerlySerializedAs("fpsModelOffsets")]
        [Tooltip("Position offsets from fps model container (Camera's transform)")]
        private Vector3 fpsModelPositionOffsets = Vector3.zero;
        [SerializeField]
        [Tooltip("Rotation offsets from fps model container (Camera's transform)")]
        private Vector3 fpsModelRotationOffsets = Vector3.zero;
        public BaseCharacterModel ActiveTpsModel { get; private set; }
        public BaseCharacterModel ActiveFpsModel { get; private set; }
        public BaseCharacterModel MainFpsModel { get; set; }

        [HideInInspector]
        [SerializeField]
        // Vehicle models setup will be moved to `BaseCharacterModel`
        private VehicleCharacterModel[] vehicleModels = new VehicleCharacterModel[0];

        public bool IsHide
        {
            get
            {
                foreach (bool hideState in hideStates.Values)
                {
                    if (hideState)
                        return true;
                }
                return false;
            }
        }
        public bool IsFps { get; private set; }

        private readonly Dictionary<byte, bool> hideStates = new Dictionary<byte, bool>();
        private int dirtyVehicleDataId;
        private byte dirtySeatIndex;

        public override void EntityAwake()
        {
            ValidateMainTpsModel();
            MigrateVehicleModels();
            MainTpsModel.MainModel = MainTpsModel;
            MainTpsModel.IsTpsModel = true;
            MainTpsModel.IsFpsModel = false;
            MainTpsModel.InitCacheData();
            SwitchTpsModel(MainTpsModel);
        }

        public override void EntityStart()
        {
            base.EntityStart();
        }

        public bool ValidateMainTpsModel()
        {
            if (MainTpsModel == null)
            {
                MainTpsModel = GetComponent<BaseCharacterModel>();
                return true;
            }
            return false;
        }

        private bool MigrateVehicleModels()
        {
            if (vehicleModels != null && vehicleModels.Length > 0)
            {
                MainTpsModel.VehicleModels = vehicleModels;
                vehicleModels = new VehicleCharacterModel[0];
                return true;
            }
            return false;
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            bool hasChanges = false;
            if (ValidateMainTpsModel())
                hasChanges = true;
            if (MigrateVehicleModels())
                hasChanges = true;
            if (hasChanges)
                EditorUtility.SetDirty(this);
#endif
        }

        public void UpdatePassengingVehicle(VehicleType vehicleType, byte seatIndex)
        {
            if (vehicleType != null)
            {
                if (dirtyVehicleDataId != vehicleType.DataId ||
                    dirtySeatIndex != seatIndex)
                {
                    dirtyVehicleDataId = vehicleType.DataId;
                    dirtySeatIndex = seatIndex;
                    VehicleCharacterModel tempData;
                    // Switch TPS model
                    if (MainTpsModel != null)
                    {
                        if (MainTpsModel.CacheVehicleModels.TryGetValue(dirtyVehicleDataId, out tempData) &&
                            seatIndex < tempData.modelsForEachSeats.Length)
                            SwitchTpsModel(tempData.modelsForEachSeats[seatIndex]);
                        else
                            SwitchTpsModel(MainTpsModel);
                    }
                    // Switch FPS Model
                    if (MainFpsModel != null)
                    {
                        if (MainFpsModel.CacheVehicleModels.TryGetValue(dirtyVehicleDataId, out tempData) &&
                            seatIndex < tempData.modelsForEachSeats.Length)
                            SwitchFpsModel(tempData.modelsForEachSeats[seatIndex]);
                        else
                            SwitchFpsModel(MainFpsModel);
                    }
                }
                return;
            }

            if (dirtyVehicleDataId != 0)
            {
                dirtyVehicleDataId = 0;
                dirtySeatIndex = 0;
                if (MainTpsModel != null)
                    SwitchTpsModel(MainTpsModel);
                if (MainFpsModel != null)
                    SwitchFpsModel(MainFpsModel);
            }
        }

        private void SwitchTpsModel(BaseCharacterModel nextModel)
        {
            if (ActiveTpsModel != null && nextModel == ActiveTpsModel) return;
            BaseCharacterModel previousModel = ActiveTpsModel;
            ActiveTpsModel = nextModel;
            ActiveTpsModel.SwitchModel(previousModel);
        }

        private void SwitchFpsModel(BaseCharacterModel nextModel)
        {
            if (ActiveFpsModel != null && nextModel == ActiveFpsModel) return;
            BaseCharacterModel previousModel = ActiveFpsModel;
            ActiveFpsModel = nextModel;
            ActiveFpsModel.SwitchModel(previousModel);
        }

        public void SetIsHide(byte setter, bool isHide)
        {
            hideStates[setter] = isHide;
            UpdateVisibleState();
        }

        public void SetIsFps(bool isFps)
        {
            if (IsFps == isFps)
                return;
            IsFps = isFps;
            UpdateVisibleState();
        }

        public void UpdateVisibleState()
        {
            GameEntityModel.EVisibleState mainModelVisibleState = GameEntityModel.EVisibleState.Visible;
            if (IsFps)
                mainModelVisibleState = GameEntityModel.EVisibleState.Fps;
            if (IsHide)
                mainModelVisibleState = GameEntityModel.EVisibleState.Invisible;
            // Set visible state to main model
            MainTpsModel.SetVisibleState(mainModelVisibleState);
            // FPS model will hide when it's not FPS mode
            if (MainFpsModel != null)
                MainFpsModel.gameObject.SetActive(IsFps);
        }

        public BaseCharacterModel InstantiateFpsModel(Transform container)
        {
            if (fpsModelPrefab == null)
                return null;
            MainFpsModel = Instantiate(fpsModelPrefab, container);
            MainFpsModel.MainModel = MainFpsModel;
            MainFpsModel.IsFpsModel = true;
            MainFpsModel.IsTpsModel = false;
            MainFpsModel.InitCacheData();
            MainFpsModel.transform.localPosition = fpsModelPositionOffsets;
            MainFpsModel.transform.localRotation = Quaternion.Euler(fpsModelRotationOffsets);
            MainFpsModel.SetEquipItems(MainTpsModel.equipItems);
            MainFpsModel.SetEquipWeapons(MainTpsModel.equipWeapons);
            ActiveFpsModel = MainFpsModel;
            return MainFpsModel;
        }
    }
}
