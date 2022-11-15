﻿using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Serialization;
using LiteNetLib;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    [RequireComponent(typeof(LiteNetLibIdentity))]
    [DefaultExecutionOrder(0)]
    public abstract class BaseGameEntity : LiteNetLibBehaviour, IGameEntity, IEntityMovement
    {
        public const float GROUND_DETECTION_DISTANCE = 30f;

        public int EntityId
        {
            get { return Identity.HashAssetId; }
            set { }
        }

        [Category(0, "Title Settings")]
        [Tooltip("This title will be used while `syncTitle` is empty.")]
        [FormerlySerializedAs("characterTitle")]
        [FormerlySerializedAs("itemTitle")]
        [SerializeField]
        protected string entityTitle;

        [Tooltip("Titles by language keys")]
        [FormerlySerializedAs("characterTitles")]
        [FormerlySerializedAs("itemTitles")]
        [SerializeField]
        protected LanguageData[] entityTitles;

        [Category(100, "Sync Fields", false)]
        [SerializeField]
        protected SyncFieldString syncTitle = new SyncFieldString();
        public SyncFieldString SyncTitle
        {
            get { return syncTitle; }
        }
        public string Title
        {
            get { return !string.IsNullOrEmpty(syncTitle.Value) ? syncTitle.Value : EntityTitle; }
            set { syncTitle.Value = value; }
        }

        [Category(1, "Relative GameObjects/Transforms")]
        [Tooltip("These objects will be hidden on non owner objects")]
        [SerializeField]
        private GameObject[] ownerObjects = new GameObject[0];
        public GameObject[] OwnerObjects
        {
            get { return ownerObjects; }
        }

        [Tooltip("These objects will be hidden on owner objects")]
        [SerializeField]
        private GameObject[] nonOwnerObjects = new GameObject[0];
        public GameObject[] NonOwnerObjects
        {
            get { return nonOwnerObjects; }
        }

        public virtual string EntityTitle
        {
            get { return Language.GetText(entityTitles, entityTitle); }
        }

        [Category(2, "Components")]
        [SerializeField]
        protected GameEntityModel model = null;
        public GameEntityModel Model
        {
            get { return model; }
        }

        [Category("Relative GameObjects/Transforms")]
        [Tooltip("Transform for position which camera will look at and follow while playing in TPS view mode")]
        [SerializeField]
        private Transform cameraTargetTransform = null;
        public Transform CameraTargetTransform
        {
            get
            {
                if (PassengingVehicleEntity != null)
                {
                    if (PassengingVehicleSeat.cameraTarget == VehicleSeatCameraTarget.Vehicle)
                        return PassengingVehicleEntity.Entity.CameraTargetTransform;
                }
                return cameraTargetTransform;
            }
            set { cameraTargetTransform = value; }
        }

        [Tooltip("Transform for position which camera will look at and follow while playing in FPS view mode")]
        [SerializeField]
        private Transform fpsCameraTargetTransform = null;
        public Transform FpsCameraTargetTransform
        {
            get { return fpsCameraTargetTransform; }
            set { fpsCameraTargetTransform = value; }
        }

        public Transform CacheTransform { get; private set; }

        [Category(3, "Entity Movement")]
        [SerializeField]
        private MovementSecure movementSecure = MovementSecure.NotSecure;
        public MovementSecure MovementSecure { get { return movementSecure; } set { movementSecure = value; } }

        [SerializeField]
        protected bool canSideSprint = false;
        public bool CanSideSprint { get { return canSideSprint; } }

        [SerializeField]
        protected bool canBackwardSprint = false;
        public bool CanBackwardSprint { get { return canBackwardSprint; } }

        public IEntityMovementComponent Movement { get; private set; }

        public Transform MovementTransform
        {
            get
            {
                if (PassengingVehicleEntity != null)
                {
                    // Track movement position by vehicle entity
                    return PassengingVehicleEntity.Entity.CacheTransform;
                }
                return CacheTransform;
            }
        }

        private bool foundPassengingVehicleEntity;
        private IVehicleEntity passengingVehicleEntity;
        public IVehicleEntity PassengingVehicleEntity
        {
            get
            {
                if (!foundPassengingVehicleEntity && PassengingVehicle.objectId > 0)
                {
                    passengingVehicleEntity = null;
                    LiteNetLibIdentity identity;
                    if (Manager.Assets.TryGetSpawnedObject(PassengingVehicle.objectId, out identity))
                    {
                        foundPassengingVehicleEntity = true;
                        passengingVehicleEntity = identity.GetComponent<IVehicleEntity>();
                    }
                }
                // Clear current vehicle, if not existed
                if (PassengingVehicle.objectId == 0)
                    passengingVehicleEntity = null;
                return passengingVehicleEntity;
            }
        }

        public VehicleType PassengingVehicleType
        {
            get
            {
                if (PassengingVehicleEntity != null)
                    return PassengingVehicleEntity.VehicleType;
                return null;
            }
        }

        public VehicleSeat PassengingVehicleSeat
        {
            get
            {
                if (PassengingVehicleEntity != null)
                    return PassengingVehicleEntity.Seats[PassengingVehicle.seatIndex];
                return VehicleSeat.Empty;
            }
        }

        public IEntityMovementComponent ActiveMovement
        {
            get
            {
                if (PassengingVehicleEntity != null)
                    return PassengingVehicleEntity.Entity.Movement;
                return Movement;
            }
        }

        [Category("Sync Fields")]
        [SerializeField]
        protected SyncFieldPassengingVehicle passengingVehicle = new SyncFieldPassengingVehicle();
        public PassengingVehicle PassengingVehicle
        {
            get { return passengingVehicle.Value; }
            set { passengingVehicle.Value = value; }
        }

        public float StoppingDistance
        {
            get
            {
                return ActiveMovement == null ? 0.1f : ActiveMovement.StoppingDistance;
            }
        }
        public MovementState MovementState
        {
            get
            {
                return ActiveMovement == null ? MovementState.IsGrounded : ActiveMovement.MovementState;
            }
        }
        public ExtraMovementState ExtraMovementState
        {
            get
            {
                return ActiveMovement == null ? ExtraMovementState.None : ActiveMovement.ExtraMovementState;
            }
        }
        public DirectionVector2 Direction2D
        {
            get
            {
                return ActiveMovement == null ? (DirectionVector2)Vector2.down : ActiveMovement.Direction2D;
            }
            set
            {
                if (ActiveMovement != null)
                    ActiveMovement.Direction2D = value;
            }
        }
        public virtual float MoveAnimationSpeedMultiplier { get { return 1f; } }
        public virtual bool MuteFootstepSound { get { return false; } }
        protected bool dirtyIsHide;
        protected bool isTeleporting;
        protected Vector3 teleportingPosition;
        protected Quaternion teleportingRotation;
        protected bool lastGrounded;
        protected Vector3 lastGroundedPosition;

        public GameInstance CurrentGameInstance
        {
            get { return GameInstance.Singleton; }
        }

        public BaseGameplayRule CurrentGameplayRule
        {
            get { return CurrentGameInstance.GameplayRule; }
        }

        public BaseGameNetworkManager CurrentGameManager
        {
            get { return BaseGameNetworkManager.Singleton; }
        }

        public BaseMapInfo CurrentMapInfo
        {
            get { return BaseGameNetworkManager.CurrentMapInfo; }
        }

        public BaseGameEntity Entity
        {
            get { return this; }
        }

        protected IGameEntityComponent[] EntityComponents { get; private set; }
        protected virtual bool UpdateEntityComponents { get { return true; } }

        #region Events
        public event System.Action onStart;
        public event System.Action onEnable;
        public event System.Action onDisable;
        public event System.Action onUpdate;
        public event System.Action onLateUpdate;
        public event System.Action onFixedUpdate;
        public event System.Action onSetup;
        public event System.Action onSetupNetElements;
        public event System.Action onSetOwnerClient;
        public event NetworkDestroyDelegate onNetworkDestroy;
        #endregion

        private void Awake()
        {
            InitialRequiredComponents();
            EntityComponents = GetComponents<IGameEntityComponent>();
            for (int i = 0; i < EntityComponents.Length; ++i)
            {
                EntityComponents[i].EntityAwake();
                EntityComponents[i].Enabled = true;
            }
            EntityAwake();
            this.InvokeInstanceDevExtMethods("Awake");
        }

        /// <summary>
        /// Override this function to initial required components
        /// This function will be called by this entity when awake
        /// </summary>
        public virtual void InitialRequiredComponents()
        {
            // Cache components
            CacheTransform = transform;
            if (model == null)
                model = GetComponent<GameEntityModel>();
            if (cameraTargetTransform == null)
                cameraTargetTransform = CacheTransform;
            if (fpsCameraTargetTransform == null)
                fpsCameraTargetTransform = CacheTransform;
            Movement = GetComponent<IEntityMovementComponent>();
        }

        /// <summary>
        /// Override this function to add relates game data to game instance
        /// This function will be called by GameInstance when adding the entity
        /// </summary>
        public virtual void PrepareRelatesData()
        {
            // Add pooling game effects
            GameInstance.AddPoolingObjects(GetComponentsInChildren<IPoolDescriptorCollection>(true));
        }

        /// <summary>
        /// Override this function to set instigator when attacks other entities
        /// </summary>
        /// <returns></returns>
        public virtual EntityInfo GetInfo()
        {
            return EntityInfo.Empty;
        }

        public virtual Bounds MakeLocalBounds()
        {
            return GameplayUtils.MakeLocalBoundsByCollider(CacheTransform);
        }

        protected virtual void EntityAwake() { }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
        }

        protected virtual void OnDrawGizmosSelected()
        {
        }
#endif

        private void Start()
        {
            for (int i = 0; i < EntityComponents.Length; ++i)
            {
                if (EntityComponents[i].Enabled)
                    EntityComponents[i].EntityStart();
            }
            EntityStart();
            if (onStart != null)
                onStart.Invoke();
        }
        protected virtual void EntityStart() { }

        public override void OnSetOwnerClient(bool isOwnerClient)
        {
            EntityOnSetOwnerClient();
            if (onSetOwnerClient != null)
                onSetOwnerClient.Invoke();
        }
        protected virtual void EntityOnSetOwnerClient()
        {
            foreach (GameObject ownerObject in ownerObjects)
            {
                if (ownerObject == null) continue;
                ownerObject.SetActive(IsOwnerClient);
            }
            foreach (GameObject nonOwnerObject in nonOwnerObjects)
            {
                if (nonOwnerObject == null) continue;
                nonOwnerObject.SetActive(!IsOwnerClient);
            }
        }

        private void OnEnable()
        {
            EntityOnEnable();
            if (onEnable != null)
                onEnable.Invoke();
        }
        protected virtual void EntityOnEnable() { }

        private void OnDisable()
        {
            EntityOnDisable();
            if (onDisable != null)
                onDisable.Invoke();
        }
        protected virtual void EntityOnDisable() { }

        private void Update()
        {
            Profiler.BeginSample("Entity Components - Update");
            if (UpdateEntityComponents)
            {
                for (int i = 0; i < EntityComponents.Length; ++i)
                {
                    if (EntityComponents[i].Enabled)
                        EntityComponents[i].EntityUpdate();
                }
            }
            Profiler.EndSample();
            EntityUpdate();
            if (onUpdate != null)
                onUpdate.Invoke();
        }

        protected virtual void EntityUpdate()
        {
            if (IsServer && CurrentGameInstance.DimensionType == DimensionType.Dimension3D)
            {
                // Ground check / ground damage will be calculated at server while dimension type is 3d only
                lastGrounded = MovementState.Has(MovementState.IsGrounded);
                if (lastGrounded)
                    lastGroundedPosition = CacheTransform.position;
            }

            if (Movement != null)
            {
                bool tempEnableMovement = PassengingVehicleEntity == null;
                // Enable movement or not
                if (Movement.Enabled != tempEnableMovement)
                {
                    if (!tempEnableMovement)
                        Movement.StopMove();
                    // Enable movement while not passenging any vehicle
                    Movement.Enabled = tempEnableMovement;
                }
            }

            if (Model != null && Model is IMoveableModel)
            {
                // Update movement animation
                (Model as IMoveableModel).SetMoveAnimationSpeedMultiplier(MoveAnimationSpeedMultiplier);
                (Model as IMoveableModel).SetMovementState(MovementState, ExtraMovementState, Direction2D, false);
            }
        }

        private void LateUpdate()
        {
            Profiler.BeginSample("Entity Components - LateUpdate");
            if (UpdateEntityComponents)
            {
                for (int i = 0; i < EntityComponents.Length; ++i)
                {
                    if (EntityComponents[i].Enabled)
                        EntityComponents[i].EntityLateUpdate();
                }
            }
            Profiler.EndSample();
            EntityLateUpdate();
            if (onLateUpdate != null)
                onLateUpdate.Invoke();
            // Update identity's hide status
            bool isHide = IsHide();
            if (dirtyIsHide != isHide)
            {
                dirtyIsHide = isHide;
                Identity.IsHide = dirtyIsHide;
            }
        }

        protected virtual void EntityLateUpdate()
        {
            if (PassengingVehicleSeat.passengingTransform != null)
            {
                // Snap character to vehicle seat
                CacheTransform.position = PassengingVehicleSeat.passengingTransform.position;
                CacheTransform.rotation = PassengingVehicleSeat.passengingTransform.rotation;
            }

            if (isTeleporting && ActiveMovement != null)
            {
                Teleport(teleportingPosition, teleportingRotation);
                isTeleporting = false;
            }
        }

        private void FixedUpdate()
        {
            Profiler.BeginSample("Entity Components - FixedUpdate");
            if (UpdateEntityComponents)
            {
                for (int i = 0; i < EntityComponents.Length; ++i)
                {
                    if (EntityComponents[i].Enabled)
                        EntityComponents[i].EntityFixedUpdate();
                }
            }
            Profiler.EndSample();
            EntityFixedUpdate();
            if (onFixedUpdate != null)
                onFixedUpdate.Invoke();
        }
        protected virtual void EntityFixedUpdate() { }

        private void OnDestroy()
        {
            for (int i = 0; i < EntityComponents.Length; ++i)
            {
                EntityComponents[i].EntityOnDestroy();
            }
            // Remove sync field events
            passengingVehicle.onChange -= OnPassengingVehicleChange;
            EntityOnDestroy();
            this.InvokeInstanceDevExtMethods("OnDestroy");
        }
        protected virtual void EntityOnDestroy()
        {
            // Exit vehicle when destroy
            if (IsServer)
                ExitVehicle();
        }

        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            SetupNetElements();
#endif
        }

        public override void OnSetup()
        {
            base.OnSetup();

            if (onSetup != null)
                onSetup.Invoke();

            SetupNetElements();

            passengingVehicle.onChange += OnPassengingVehicleChange;
        }

        protected virtual void SetupNetElements()
        {
            if (onSetupNetElements != null)
                onSetupNetElements.Invoke();
            syncTitle.deliveryMethod = DeliveryMethod.ReliableOrdered;
            syncTitle.syncMode = LiteNetLibSyncField.SyncMode.ServerToClients;
            passengingVehicle.deliveryMethod = DeliveryMethod.ReliableOrdered;
            passengingVehicle.syncMode = LiteNetLibSyncField.SyncMode.ServerToClients;
            passengingVehicle.doNotSyncInitialDataImmediately = true;
        }

        protected void OnPassengingVehicleChange(bool isInitial, PassengingVehicle passengingVehicle)
        {
            // Set `foundPassengingVehicleEntity` to `True` if `objectId` is `0` so it will not find `PassengingVehicleEntity`
            foundPassengingVehicleEntity = passengingVehicle.objectId == 0;
        }

        #region Net Functions
        [ServerRpc]
        protected void ServerEnterVehicle(uint objectId)
        {
#if !CLIENT_BUILD
            // Call this function at server
            LiteNetLibIdentity identity;
            if (Manager.Assets.TryGetSpawnedObject(objectId, out identity))
            {
                IVehicleEntity vehicleEntity = identity.GetComponent<IVehicleEntity>();
                byte seatIndex;
                if (vehicleEntity != null &&
                    vehicleEntity.GetAvailableSeat(out seatIndex))
                    EnterVehicle(vehicleEntity, seatIndex);
            }
#endif
        }

        [ServerRpc]
        protected void ServerEnterVehicleToSeat(uint objectId, byte seatIndex)
        {
#if !CLIENT_BUILD
            // Call this function at server
            LiteNetLibIdentity identity;
            if (Manager.Assets.TryGetSpawnedObject(objectId, out identity))
            {
                IVehicleEntity vehicleEntity = identity.GetComponent<IVehicleEntity>();
                if (vehicleEntity != null)
                    EnterVehicle(vehicleEntity, seatIndex);
            }
#endif
        }

        [ServerRpc]
        protected void ServerExitVehicle()
        {
#if !CLIENT_BUILD
            // Call this function at server
            ExitVehicle();
#endif
        }

        [AllRpc]
        protected void AllOnExitVehicle()
        {
            // Call this function at client
            foundPassengingVehicleEntity = true;
            passengingVehicleEntity = null;
        }

        [AllRpc]
        protected void AllPlayJumpAnimation()
        {
            this.PlayJumpAnimation();
        }

        [AllRpc]
        protected void AllPlayPickupAnimation()
        {
            this.PlayPickupAnimation();
        }
        #endregion

        #region RPC Calls
        public void CallServerEnterVehicle(uint objectId)
        {
            RPC(ServerEnterVehicle, objectId);
        }

        public void CallServerEnterVehicleToSeat(uint objectId, byte seatIndex)
        {
            RPC(ServerEnterVehicleToSeat, objectId, seatIndex);
        }

        public void CallServerExitVehicle()
        {
            RPC(ServerExitVehicle);
        }
        #endregion

        public override void OnNetworkDestroy(byte reasons)
        {
            base.OnNetworkDestroy(reasons);
            if (onNetworkDestroy != null)
                onNetworkDestroy.Invoke(reasons);
        }

        public virtual float GetMoveSpeed()
        {
            return 0;
        }

        public virtual bool CanMove()
        {
            return false;
        }

        public virtual bool CanSprint()
        {
            return false;
        }

        public virtual bool CanCrouch()
        {
            return false;
        }

        public virtual bool CanCrawl()
        {
            return false;
        }

        public virtual bool IsHide()
        {
            return false;
        }

        public void StopMove()
        {
            if (ActiveMovement != null)
                ActiveMovement.StopMove();
        }

        public void KeyMovement(Vector3 moveDirection, MovementState moveState)
        {
            if (ActiveMovement != null)
                ActiveMovement.KeyMovement(moveDirection, moveState);
        }

        public void PointClickMovement(Vector3 position)
        {
            if (ActiveMovement != null)
                ActiveMovement.PointClickMovement(position);
        }

        public void SetExtraMovementState(ExtraMovementState extraMovementState)
        {
            if (ActiveMovement != null)
                ActiveMovement.SetExtraMovementState(extraMovementState);
        }

        public void SetLookRotation(Quaternion rotation)
        {
            if (ActiveMovement != null)
                ActiveMovement.SetLookRotation(rotation);
        }

        public Quaternion GetLookRotation()
        {
            if (ActiveMovement != null)
                return ActiveMovement.GetLookRotation();
            return Quaternion.identity;
        }

        public void Teleport(Vector3 position, Quaternion rotation)
        {
            if (ActiveMovement == null)
            {
                teleportingPosition = position;
                teleportingRotation = rotation;
                isTeleporting = true;
                return;
            }
            Vector3 groundedPosition;
            if (FindGroundedPosition(position, GROUND_DETECTION_DISTANCE, out groundedPosition))
                position = groundedPosition;
            OnTeleport(position, rotation);
            if (IsServer)
                ActiveMovement.Teleport(position, rotation);
            if (IsServer && CurrentGameInstance.DimensionType == DimensionType.Dimension3D)
            {
                // Ground check / ground damage will be calculated at server while dimension type is 3d only
                lastGrounded = true;
                lastGroundedPosition = position;
            }
        }

        protected virtual void OnTeleport(Vector3 position, Quaternion rotation)
        {

        }

        public bool FindGroundedPosition(Vector3 fromPosition, float findDistance, out Vector3 result)
        {
            result = CacheTransform.position;
            if (ActiveMovement != null)
                return ActiveMovement.FindGroundedPosition(fromPosition, findDistance, out result);
            return true;
        }

        public void CallAllPlayJumpAnimation()
        {
            RPC(AllPlayJumpAnimation);
        }

        public void CallAllPlayPickupAnimation()
        {
            RPC(AllPlayPickupAnimation);
        }

        protected bool EnterVehicle(IVehicleEntity vehicle, byte seatIndex)
        {
            if (!IsServer || vehicle == null || PassengingVehicle.objectId > 0 || !vehicle.IsSeatAvailable(seatIndex))
                return false;

            // Change object owner to driver
            if (vehicle.IsDriver(seatIndex))
                Manager.Assets.SetObjectOwner(vehicle.Entity.ObjectId, ConnectionId);

            // Set passenger to vehicle
            vehicle.SetPassenger(seatIndex, this);

            // Set mount info
            PassengingVehicle = new PassengingVehicle()
            {
                objectId = vehicle.Entity.ObjectId,
                seatIndex = seatIndex,
            };

            return true;
        }

        protected void ExitVehicle()
        {
            if (!IsServer || PassengingVehicleEntity == null)
                return;

            bool isDriver = PassengingVehicleEntity.IsDriver(PassengingVehicle.seatIndex);
            bool isDestroying = PassengingVehicleEntity.IsDestroyWhenExit(PassengingVehicle.seatIndex);

            // Clear object owner from driver
            if (PassengingVehicleEntity.IsDriver(PassengingVehicle.seatIndex))
                Manager.Assets.SetObjectOwner(PassengingVehicleEntity.Entity.ObjectId, -1);

            BaseGameEntity vehicleEntity = PassengingVehicleEntity.Entity;
            if (isDestroying)
            {
                // Remove all entity from vehicle
                PassengingVehicleEntity.RemoveAllPassengers();
                // Destroy vehicle entity
                vehicleEntity.NetworkDestroy();
            }
            else
            {
                // Remove this from vehicle
                PassengingVehicleEntity.RemovePassenger(PassengingVehicle.seatIndex);
                // Stop move if driver exit (if not driver continue move by driver controls)
                if (isDriver)
                    vehicleEntity.StopMove();
            }
        }

        /// <summary>
        /// This function will be called by Vehicle Entity to inform that this entity exited vehicle
        /// </summary>
        public void ExitedVehicle(Vector3 exitPosition, Quaternion exitRotation)
        {
            // Clear passenging vehicle data
            PassengingVehicle = default(PassengingVehicle);

            // Clear vehicle entity at clients
            RPC(AllOnExitVehicle);

            // Teleport to exit transform
            Teleport(exitPosition, exitRotation);
        }
    }
}
