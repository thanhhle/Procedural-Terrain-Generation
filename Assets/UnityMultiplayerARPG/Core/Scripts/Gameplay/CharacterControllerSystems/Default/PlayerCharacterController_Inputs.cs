﻿using UnityEngine;

namespace MultiplayerARPG
{
    public partial class PlayerCharacterController
    {
        public const float MIN_START_MOVE_DISTANCE = 0.01f;

        public virtual void UpdateInput()
        {
            bool isFocusInputField = GenericUtils.IsFocusInputField();
            bool isPointerOverUIObject = CacheUISceneGameplay.IsPointerOverUIObject();
            CacheGameplayCameraController.UpdateRotationX = false;
            CacheGameplayCameraController.UpdateRotationY = false;
            if (Application.isConsolePlatform)
                CacheGameplayCameraController.UpdateRotation = !isFocusInputField && !isPointerOverUIObject;
            else
                CacheGameplayCameraController.UpdateRotation = !isFocusInputField && !isPointerOverUIObject && InputManager.GetButton("CameraRotate");
            CacheGameplayCameraController.UpdateZoom = !isFocusInputField && !isPointerOverUIObject;

            if (isFocusInputField || PlayerCharacterEntity.IsDead())
            {
                PlayerCharacterEntity.KeyMovement(Vector3.zero, MovementState.None);
                return;
            }

            // If it's building something, don't allow to activate NPC/Warp/Pickup Item
            if (ConstructingBuildingEntity == null)
            {
                // Activate nearby npcs / players / activable buildings
                if (InputManager.GetButtonDown("Activate"))
                {
                    targetPlayer = null;
                    if (ActivatableEntityDetector.players.Count > 0)
                        targetPlayer = ActivatableEntityDetector.players[0];
                    targetNpc = null;
                    if (ActivatableEntityDetector.npcs.Count > 0)
                        targetNpc = ActivatableEntityDetector.npcs[0];
                    targetBuilding = null;
                    if (ActivatableEntityDetector.buildings.Count > 0)
                        targetBuilding = ActivatableEntityDetector.buildings[0];
                    targetVehicle = null;
                    if (ActivatableEntityDetector.vehicles.Count > 0)
                        targetVehicle = ActivatableEntityDetector.vehicles[0];
                    targetWarpPortal = null;
                    if (ActivatableEntityDetector.warpPortals.Count > 0)
                        targetWarpPortal = ActivatableEntityDetector.warpPortals[0];
                    targetItemsContainer = null;
                    if (ItemDropEntityDetector.itemsContainers.Count > 0)
                        targetItemsContainer = ItemDropEntityDetector.itemsContainers[0];
                    // Priority Player -> Npc -> Buildings
                    if (targetPlayer != null)
                    {
                        // Show dealing, invitation menu
                        SelectedEntity = targetPlayer;
                        CacheUISceneGameplay.SetActivePlayerCharacter(targetPlayer);
                    }
                    else if (targetNpc != null)
                    {
                        // Talk to NPC
                        SelectedEntity = targetNpc;
                        PlayerCharacterEntity.NpcAction.CallServerNpcActivate(targetNpc.ObjectId);
                    }
                    else if (targetBuilding != null)
                    {
                        // Use building
                        SelectedEntity = targetBuilding;
                        ActivateBuilding(targetBuilding);
                    }
                    else if (targetVehicle != null)
                    {
                        // Enter vehicle
                        PlayerCharacterEntity.CallServerEnterVehicle(targetVehicle.ObjectId);
                    }
                    else if (targetWarpPortal != null)
                    {
                        // Enter warp, For some warp portals that `warpImmediatelyWhenEnter` is FALSE
                        PlayerCharacterEntity.CallServerEnterWarp(targetWarpPortal.ObjectId);
                    }
                    else if (targetItemsContainer != null)
                    {
                        // Show items
                        ShowItemsContainerDialog(targetItemsContainer);
                    }
                }
                // Pick up nearby items
                if (InputManager.GetButtonDown("PickUpItem"))
                {
                    targetItemDrop = null;
                    if (ItemDropEntityDetector.itemDrops.Count > 0)
                        targetItemDrop = ItemDropEntityDetector.itemDrops[0];
                    if (targetItemDrop != null)
                        PlayerCharacterEntity.CallServerPickupItem(targetItemDrop.ObjectId);
                }
                // Reload
                if (InputManager.GetButtonDown("Reload"))
                {
                    // Reload ammo when press the button
                    ReloadAmmo();
                }
                // Find target to attack
                if (InputManager.GetButtonDown("FindEnemy"))
                {
                    ++findingEnemyIndex;
                    if (findingEnemyIndex < 0 || findingEnemyIndex >= EnemyEntityDetector.characters.Count)
                        findingEnemyIndex = 0;
                    if (EnemyEntityDetector.characters.Count > 0)
                    {
                        SetTarget(null, TargetActionType.Attack);
                        if (!EnemyEntityDetector.characters[findingEnemyIndex].IsHideOrDead())
                        {
                            SetTarget(EnemyEntityDetector.characters[findingEnemyIndex], TargetActionType.Attack);
                            if (SelectedEntity != null)
                            {
                                // Turn character to enemy but does not move or attack yet.
                                TurnCharacterToEntity(SelectedEntity);
                            }
                        }
                    }
                }
                if (InputManager.GetButtonDown("ExitVehicle"))
                {
                    // Exit vehicle
                    PlayerCharacterEntity.CallServerExitVehicle();
                }
                if (InputManager.GetButtonDown("SwitchEquipWeaponSet"))
                {
                    // Switch equip weapon set
                    GameInstance.ClientInventoryHandlers.RequestSwitchEquipWeaponSet(new RequestSwitchEquipWeaponSetMessage()
                    {
                        equipWeaponSet = (byte)(PlayerCharacterEntity.EquipWeaponSet + 1),
                    }, ClientInventoryActions.ResponseSwitchEquipWeaponSet);
                }
                if (InputManager.GetButtonDown("Sprint"))
                {
                    // Toggles sprint state
                    isSprinting = !isSprinting;
                    isWalking = false;
                }
                else if (InputManager.GetButtonDown("Walk"))
                {
                    // Toggles sprint state
                    isWalking = !isWalking;
                    isSprinting = false;
                }
                // Auto reload
                if (PlayerCharacterEntity.EquipWeapons.rightHand.IsAmmoEmpty() ||
                    PlayerCharacterEntity.EquipWeapons.leftHand.IsAmmoEmpty())
                {
                    // Reload ammo when empty and not press any keys
                    ReloadAmmo();
                }
            }
            // Update enemy detecting radius to attack distance
            EnemyEntityDetector.detectingRadius = Mathf.Max(PlayerCharacterEntity.GetAttackDistance(false), lockAttackTargetDistance);
            // Update inputs
            UpdateQueuedSkill();
            UpdatePointClickInput();
            UpdateWASDInput();
            // Set extra movement state
            if (isSprinting)
                PlayerCharacterEntity.SetExtraMovementState(ExtraMovementState.IsSprinting);
            else if (isWalking)
                PlayerCharacterEntity.SetExtraMovementState(ExtraMovementState.IsWalking);
            else
                PlayerCharacterEntity.SetExtraMovementState(ExtraMovementState.None);
        }

        protected void ReloadAmmo()
        {
            // Reload ammo at server
            if (!PlayerCharacterEntity.EquipWeapons.rightHand.IsAmmoFull())
                PlayerCharacterEntity.Reload(false);
            else if (!PlayerCharacterEntity.EquipWeapons.leftHand.IsAmmoFull())
                PlayerCharacterEntity.Reload(true);
        }

        public virtual void UpdatePointClickInput()
        {
            if (controllerMode == PlayerCharacterControllerMode.WASD)
                return;

            // If it's building something, not allow point click movement
            if (ConstructingBuildingEntity != null)
                return;

            // If it's aiming skills, not allow point click movement
            if (UICharacterHotkeys.UsingHotkey != null)
                return;

            getMouseDown = Input.GetMouseButtonDown(0);
            getMouseUp = Input.GetMouseButtonUp(0);
            getMouse = Input.GetMouseButton(0);

            if (getMouseDown)
            {
                isMouseDragOrHoldOrOverUI = false;
                mouseDownTime = Time.unscaledTime;
                mouseDownPosition = Input.mousePosition;
            }
            // Read inputs
            isPointerOverUI = CacheUISceneGameplay.IsPointerOverUIObject();
            isMouseDragDetected = (Input.mousePosition - mouseDownPosition).sqrMagnitude > DETECT_MOUSE_DRAG_DISTANCE_SQUARED;
            isMouseHoldDetected = Time.unscaledTime - mouseDownTime > DETECT_MOUSE_HOLD_DURATION;
            isMouseHoldAndNotDrag = !isMouseDragDetected && isMouseHoldDetected;
            if (!isMouseDragOrHoldOrOverUI && (isMouseDragDetected || isMouseHoldDetected || isPointerOverUI))
            {
                // Detected mouse dragging or hold on an UIs
                isMouseDragOrHoldOrOverUI = true;
            }
            // Will set move target when pointer isn't point on an UIs 
            if (!isPointerOverUI && (getMouse || getMouseUp))
            {
                // Clear target
                ClearTarget(true);
                didActionOnTarget = false;
                // Prepare temp variables
                Transform tempTransform;
                Vector3 tempVector3;
                bool tempHasMapPosition = false;
                Vector3 tempMapPosition = Vector3.zero;
                BuildingMaterial tempBuildingMaterial;
                // If mouse up while cursor point to target (character, item, npc and so on)
                bool mouseUpOnTarget = getMouseUp && !isMouseDragOrHoldOrOverUI;
                int tempCount = FindClickObjects(out tempVector3);
                for (int tempCounter = 0; tempCounter < tempCount; ++tempCounter)
                {
                    tempTransform = physicFunctions.GetRaycastTransform(tempCounter);
                    // When holding on target, or already enter edit building mode
                    if (isMouseHoldAndNotDrag)
                    {
                        targetBuilding = null;
                        tempBuildingMaterial = tempTransform.GetComponent<BuildingMaterial>();
                        if (tempBuildingMaterial != null)
                            targetBuilding = tempBuildingMaterial.BuildingEntity;
                        if (targetBuilding && !targetBuilding.IsDead())
                        {
                            SetTarget(targetBuilding, TargetActionType.ViewOptions);
                            isFollowingTarget = true;
                            tempHasMapPosition = false;
                            break;
                        }
                    }
                    else if (mouseUpOnTarget)
                    {
                        targetPlayer = tempTransform.GetComponent<BasePlayerCharacterEntity>();
                        targetMonster = tempTransform.GetComponent<BaseMonsterCharacterEntity>();
                        targetNpc = tempTransform.GetComponent<NpcEntity>();
                        targetItemDrop = tempTransform.GetComponent<ItemDropEntity>();
                        targetItemsContainer = tempTransform.GetComponent<ItemsContainerEntity>();
                        targetHarvestable = tempTransform.GetComponent<HarvestableEntity>();
                        targetBuilding = null;
                        tempBuildingMaterial = tempTransform.GetComponent<BuildingMaterial>();
                        if (tempBuildingMaterial != null)
                            targetBuilding = tempBuildingMaterial.BuildingEntity;
                        targetVehicle = tempTransform.GetComponent<VehicleEntity>();
                        if (targetPlayer)
                        {
                            // Found activating entity as player character entity
                            if (!targetPlayer.IsHideOrDead() && !targetPlayer.IsAlly(PlayerCharacterEntity.GetInfo()))
                                SetTarget(targetPlayer, TargetActionType.Attack);
                            else
                                SetTarget(targetPlayer, TargetActionType.Activate);
                            isFollowingTarget = true;
                            tempHasMapPosition = false;
                            break;
                        }
                        else if (targetMonster && !targetMonster.IsHideOrDead())
                        {
                            // Found activating entity as monster character entity
                            SetTarget(targetMonster, TargetActionType.Attack);
                            isFollowingTarget = true;
                            tempHasMapPosition = false;
                            break;
                        }
                        else if (targetNpc)
                        {
                            // Found activating entity as npc entity
                            SetTarget(targetNpc, TargetActionType.Activate);
                            isFollowingTarget = true;
                            tempHasMapPosition = false;
                            break;
                        }
                        else if (targetItemDrop)
                        {
                            // Found activating entity as item drop entity
                            SetTarget(targetItemDrop, TargetActionType.Activate);
                            isFollowingTarget = true;
                            tempHasMapPosition = false;
                            break;
                        }
                        else if (targetItemsContainer)
                        {
                            // Found activating entity as items container entity
                            SetTarget(targetItemsContainer, TargetActionType.Activate);
                            isFollowingTarget = true;
                            tempHasMapPosition = false;
                            break;
                        }
                        else if (targetHarvestable && !targetHarvestable.IsDead())
                        {
                            // Found activating entity as harvestable entity
                            SetTarget(targetHarvestable, TargetActionType.Attack);
                            isFollowingTarget = true;
                            tempHasMapPosition = false;
                            break;
                        }
                        else if (targetBuilding && !targetBuilding.IsDead() && targetBuilding.Activatable)
                        {
                            // Found activating entity as building entity
                            SetTarget(targetBuilding, TargetActionType.Activate);
                            isFollowingTarget = true;
                            tempHasMapPosition = false;
                            break;
                        }
                        else if (targetVehicle)
                        {
                            // Found activating entity as vehicle entity
                            if (targetVehicle.ShouldBeAttackTarget)
                                SetTarget(targetVehicle, TargetActionType.Attack);
                            else
                                SetTarget(targetVehicle, TargetActionType.Activate);
                            isFollowingTarget = true;
                            tempHasMapPosition = false;
                            break;
                        }
                        else if (!physicFunctions.GetRaycastIsTrigger(tempCounter))
                        {
                            // Set clicked map position, it will be used if no activating entity found
                            tempHasMapPosition = true;
                            tempMapPosition = physicFunctions.GetRaycastPoint(tempCounter);
                            break;
                        }
                    } // End mouseUpOnTarget
                }
                // When clicked on map (Not touch any game entity)
                // - Clear selected target to hide selected entity UIs
                // - Set target position to position where mouse clicked
                if (tempHasMapPosition)
                {
                    SelectedEntity = null;
                    targetPosition = tempMapPosition;
                }
                // When clicked on map (any non-collider position)
                // tempVector3 is come from FindClickObjects()
                // - Clear character target to make character stop doing actions
                // - Clear selected target to hide selected entity UIs
                // - Set target position to position where mouse clicked
                if (CurrentGameInstance.DimensionType == DimensionType.Dimension2D && mouseUpOnTarget && tempCount == 0)
                {
                    ClearTarget();
                    tempVector3.z = 0;
                    targetPosition = tempVector3;
                }

                // Found ground position
                if (targetPosition.HasValue)
                {
                    // Close NPC dialog, when target changes
                    HideNpcDialog();
                    ClearQueueUsingSkill();
                    isFollowingTarget = false;
                    if (PlayerCharacterEntity.IsPlayingActionAnimation())
                    {
                        if (pointClickInterruptCastingSkill)
                            PlayerCharacterEntity.InterruptCastingSkill();
                    }
                    else
                    {
                        OnPointClickOnGround(targetPosition.Value);
                    }
                }
            }
        }

        /// <summary>
        /// When point click on ground, move target to the position
        /// </summary>
        /// <param name="targetPosition"></param>
        protected virtual void OnPointClickOnGround(Vector3 targetPosition)
        {
            if (Vector3.Distance(MovementTransform.position, targetPosition) > MIN_START_MOVE_DISTANCE)
            {
                destination = targetPosition;
                PlayerCharacterEntity.PointClickMovement(targetPosition);
            }
        }

        protected virtual void SetTarget(BaseGameEntity entity, TargetActionType targetActionType, bool checkControllerMode = true)
        {
            targetPosition = null;
            if (checkControllerMode && controllerMode == PlayerCharacterControllerMode.WASD)
            {
                this.targetActionType = targetActionType;
                destination = null;
                SelectedEntity = entity;
                return;
            }
            if (pointClickSetTargetImmediately ||
                (entity != null && SelectedEntity == entity) ||
                (entity != null && entity is ItemDropEntity))
            {
                this.targetActionType = targetActionType;
                destination = null;
                TargetEntity = entity;
                PlayerCharacterEntity.SetTargetEntity(entity);
            }
            SelectedEntity = entity;
        }

        protected virtual void ClearTarget(bool exceptSelectedTarget = false)
        {
            if (!exceptSelectedTarget)
                SelectedEntity = null;
            TargetEntity = null;
            PlayerCharacterEntity.SetTargetEntity(null);
            targetPosition = null;
            targetActionType = TargetActionType.Activate;
        }

        public override void DeselectBuilding()
        {
            base.DeselectBuilding();
            ClearTarget();
        }

        public virtual void UpdateWASDInput()
        {
            if (controllerMode == PlayerCharacterControllerMode.PointClick)
                return;

            // If mobile platforms, don't receive input raw to make it smooth
            bool raw = !InputManager.useMobileInputOnNonMobile && !Application.isMobilePlatform;
            Vector3 moveDirection = GetMoveDirection(InputManager.GetAxis("Horizontal", raw), InputManager.GetAxis("Vertical", raw));
            moveDirection.Normalize();

            // Move
            if (moveDirection.sqrMagnitude > 0f)
            {
                HideNpcDialog();
                ClearQueueUsingSkill();
                destination = null;
                isFollowingTarget = false;
                if (TargetEntity != null && Vector3.Distance(CacheTransform.position, TargetEntity.CacheTransform.position) >= wasdClearTargetDistance)
                {
                    // Clear target when character moved far from target
                    ClearTarget();
                }
                if (!PlayerCharacterEntity.IsPlayingActionAnimation())
                    PlayerCharacterEntity.SetLookRotation(Quaternion.LookRotation(moveDirection));
            }

            // Attack when player pressed attack button
            if (InputManager.GetButton("Attack"))
                UpdateWASDAttack();

            // Always forward
            MovementState movementState = MovementState.Forward;
            if (InputManager.GetButtonDown("Jump"))
                movementState |= MovementState.IsJump;
            PlayerCharacterEntity.KeyMovement(moveDirection, movementState);
        }

        protected void UpdateWASDAttack()
        {
            destination = null;
            BaseCharacterEntity targetEntity;

            if (TryGetSelectedTargetAsAttackingEntity(out targetEntity))
                SetTarget(targetEntity, TargetActionType.Attack, false);

            if (wasdLockAttackTarget)
            {
                if (!TryGetAttackingEntity(out targetEntity) || targetEntity.IsHideOrDead())
                {
                    // Find nearest target and move to the target
                    targetEntity = PlayerCharacterEntity
                        .FindNearestAliveCharacter<BaseCharacterEntity>(
                        Mathf.Max(PlayerCharacterEntity.GetAttackDistance(isLeftHandAttacking), lockAttackTargetDistance),
                        false,
                        true,
                        false);
                }
                if (targetEntity != null && !targetEntity.IsHideOrDead())
                {
                    // Set target, then attack later when moved nearby target
                    SelectedEntity = targetEntity;
                    SetTarget(targetEntity, TargetActionType.Attack, false);
                    isFollowingTarget = true;
                }
                else
                {
                    // No nearby target, so attack immediately
                    RequestAttack();
                    isFollowingTarget = false;
                }
            }
            else if (!wasdLockAttackTarget)
            {
                // Find nearest target and set selected target to show character hp/mp UIs
                SelectedEntity = PlayerCharacterEntity
                    .FindNearestAliveCharacter<BaseCharacterEntity>(
                    PlayerCharacterEntity.GetAttackDistance(isLeftHandAttacking),
                    false,
                    true,
                    false);
                if (SelectedEntity != null)
                {
                    // Look at target and attack
                    TurnCharacterToEntity(SelectedEntity);
                }
                // Not lock target, so not finding target and attack immediately
                RequestAttack();
                isFollowingTarget = false;
            }
        }

        protected void UpdateQueuedSkill()
        {
            if (PlayerCharacterEntity.IsDead())
            {
                ClearQueueUsingSkill();
                return;
            }
            if (queueUsingSkill.skill == null || queueUsingSkill.level <= 0)
                return;
            if (PlayerCharacterEntity.IsPlayingActionAnimation())
                return;
            destination = null;
            BaseSkill skill = queueUsingSkill.skill;
            short skillLevel = queueUsingSkill.level;
            BaseCharacterEntity targetEntity;
            // Point click mode always lock on target
            bool wasdLockAttackTarget = this.wasdLockAttackTarget || controllerMode == PlayerCharacterControllerMode.PointClick;

            if (skill.HasCustomAimControls() && queueUsingSkill.aimPosition.type == AimPositionType.Position)
            {
                // Target not required, use skill immediately
                TurnCharacterToPosition(queueUsingSkill.aimPosition.position);
                RequestUsePendingSkill();
                isFollowingTarget = false;
                return;
            }

            if (skill.IsAttack)
            {
                if (wasdLockAttackTarget)
                {
                    if (!TryGetSelectedTargetAsAttackingEntity(out targetEntity) || targetEntity.IsHideOrDead())
                    {
                        // Try find nearby enemy if no selected target or selected taget is not enemy or target is hide or dead
                        targetEntity = PlayerCharacterEntity
                            .FindNearestAliveCharacter<BaseCharacterEntity>(
                            Mathf.Max(skill.GetCastDistance(PlayerCharacterEntity, skillLevel, isLeftHandAttacking), lockAttackTargetDistance),
                            false,
                            true,
                            false);
                    }
                    if (targetEntity != null && !targetEntity.IsHideOrDead())
                    {
                        // Set target, then use skill later when moved nearby target
                        SelectedEntity = targetEntity;
                        SetTarget(targetEntity, TargetActionType.UseSkill, false);
                        isFollowingTarget = true;
                    }
                    else
                    {
                        // No target, so use skill immediately
                        RequestUsePendingSkill();
                        isFollowingTarget = false;
                    }
                }
                else
                {
                    // Find nearest target and set selected target to show character hp/mp UIs
                    SelectedEntity = PlayerCharacterEntity
                        .FindNearestAliveCharacter<BaseCharacterEntity>(
                        skill.GetCastDistance(PlayerCharacterEntity, skillLevel, isLeftHandAttacking),
                        false,
                        true,
                        false);
                    if (SelectedEntity != null)
                    {
                        // Look at target and attack
                        TurnCharacterToEntity(SelectedEntity);
                    }
                    // Not lock target, so not finding target and use skill immediately
                    RequestUsePendingSkill();
                    isFollowingTarget = false;
                }
            }
            else
            {
                // Not attack skill, so use skill immediately
                if (skill.RequiredTarget)
                {
                    if (SelectedEntity == null)
                    {
                        PlayerCharacterEntity.QueueGameMessage(UITextKeys.UI_ERROR_NO_SKILL_TARGET);
                        ClearQueueUsingSkill();
                        isFollowingTarget = false;
                        return;
                    }
                    if (wasdLockAttackTarget)
                    {
                        // Set target, then use skill later when moved nearby target
                        if (SelectedEntity is BaseCharacterEntity)
                        {
                            SetTarget(SelectedEntity, TargetActionType.UseSkill, false);
                            isFollowingTarget = true;
                        }
                        else
                        {
                            ClearQueueUsingSkill();
                            isFollowingTarget = false;
                        }
                    }
                    else
                    {
                        // Try apply skill to selected entity immediately, it will fail if selected entity is far from the character
                        if (SelectedEntity is BaseCharacterEntity)
                        {
                            if (SelectedEntity != PlayerCharacterEntity)
                            {
                                // Look at target and use skill
                                TurnCharacterToEntity(SelectedEntity);
                            }
                            RequestUsePendingSkill();
                            isFollowingTarget = false;
                        }
                        else
                        {
                            ClearQueueUsingSkill();
                            isFollowingTarget = false;
                        }
                    }
                }
                else
                {
                    // Target not required, use skill immediately
                    RequestUsePendingSkill();
                    isFollowingTarget = false;
                }
            }
        }

        public void UpdateFollowTarget()
        {
            if (!isFollowingTarget)
                return;

            if (TryGetAttackingEntity(out targetDamageable))
            {
                if (targetDamageable.IsHideOrDead())
                {
                    ClearQueueUsingSkill();
                    PlayerCharacterEntity.StopMove();
                    ClearTarget();
                    return;
                }
                float attackDistance = 0f;
                float attackFov = 0f;
                GetAttackDistanceAndFov(isLeftHandAttacking, out attackDistance, out attackFov);
                AttackOrMoveToEntity(targetDamageable, attackDistance, CurrentGameInstance.playerLayer.Mask | CurrentGameInstance.monsterLayer.Mask);
            }
            else if (TryGetUsingSkillEntity(out targetDamageable))
            {
                if (queueUsingSkill.skill.IsAttack && targetDamageable.IsHideOrDead())
                {
                    ClearQueueUsingSkill();
                    PlayerCharacterEntity.StopMove();
                    ClearTarget();
                    return;
                }
                float castDistance = 0f;
                float castFov = 0f;
                GetUseSkillDistanceAndFov(isLeftHandAttacking, out castDistance, out castFov);
                UseSkillOrMoveToEntity(targetDamageable, castDistance);
            }
            else if (TryGetDoActionEntity(out targetPlayer))
            {
                DoActionOrMoveToEntity(targetPlayer, CurrentGameInstance.conversationDistance, () =>
                {
                    // TODO: Do something
                });
            }
            else if (TryGetDoActionEntity(out targetNpc))
            {
                DoActionOrMoveToEntity(targetNpc, CurrentGameInstance.conversationDistance, () =>
                {
                    if (!didActionOnTarget)
                    {
                        didActionOnTarget = true;
                        PlayerCharacterEntity.NpcAction.CallServerNpcActivate(targetNpc.ObjectId);
                    }
                });
            }
            else if (TryGetDoActionEntity(out targetItemDrop))
            {
                DoActionOrMoveToEntity(targetItemDrop, CurrentGameInstance.pickUpItemDistance, () =>
                {
                    PlayerCharacterEntity.CallServerPickupItem(targetItemDrop.ObjectId);
                    ClearTarget();
                });
            }
            else if (TryGetDoActionEntity(out targetItemsContainer))
            {
                DoActionOrMoveToEntity(targetItemsContainer, CurrentGameInstance.pickUpItemDistance, () =>
                {
                    ShowItemsContainerDialog(targetItemsContainer);
                });
            }
            else if (TryGetDoActionEntity(out targetBuilding, TargetActionType.Activate))
            {
                DoActionOrMoveToEntity(targetBuilding, CurrentGameInstance.conversationDistance, () =>
                {
                    if (!didActionOnTarget)
                    {
                        didActionOnTarget = true;
                        ActivateBuilding(targetBuilding);
                    }
                });
            }
            else if (TryGetDoActionEntity(out targetBuilding, TargetActionType.ViewOptions))
            {
                DoActionOrMoveToEntity(targetBuilding, CurrentGameInstance.conversationDistance, () =>
                {
                    if (!didActionOnTarget)
                    {
                        didActionOnTarget = true;
                        ShowCurrentBuildingDialog();
                    }
                });
            }
            else if (TryGetDoActionEntity(out targetVehicle))
            {
                DoActionOrMoveToEntity(targetVehicle, CurrentGameInstance.conversationDistance, () =>
                {
                    PlayerCharacterEntity.CallServerEnterVehicle(targetVehicle.ObjectId);
                    ClearTarget();
                });
            }
        }

        protected virtual bool OverlappedEntity<T>(T entity, Vector3 sourcePosition, Vector3 targetPosition, float distance)
            where T : BaseGameEntity
        {
            if (Vector3.Distance(sourcePosition, targetPosition) <= distance)
                return true;
            // Target is far from controlling entity, try overlap the entity
            if (entity == null)
                return false;
            return physicFunctions.IsGameEntityInDistance(entity, sourcePosition, distance, false);
        }

        protected virtual bool OverlappedEntityHitBox<T>(T entity, Vector3 sourcePosition, Vector3 targetPosition, float distance)
            where T : BaseGameEntity
        {
            if (Vector3.Distance(sourcePosition, targetPosition) <= distance)
                return true;
            // Target is far from controlling entity, try overlap the entity
            if (entity == null)
                return false;
            return physicFunctions.IsGameEntityHitBoxInDistance(entity, sourcePosition, distance, false);
        }

        protected virtual void DoActionOrMoveToEntity(BaseGameEntity entity, float distance, System.Action action)
        {
            Vector3 sourcePosition = CacheTransform.position;
            Vector3 targetPosition = entity.CacheTransform.position;
            if (OverlappedEntity(entity, sourcePosition, targetPosition, distance))
            {
                // Stop movement to do action
                PlayerCharacterEntity.StopMove();
                // Do action
                action.Invoke();
                // This function may be used by extending classes
                OnDoActionOnEntity();
            }
            else
            {
                // Move to target entity
                UpdateTargetEntityPosition(sourcePosition, targetPosition, distance);
            }
        }

        protected virtual void OnDoActionOnEntity()
        {

        }

        protected virtual void AttackOrMoveToEntity(IDamageableEntity entity, float distance, int layerMask)
        {
            Transform damageTransform = PlayerCharacterEntity.GetWeaponDamageInfo(ref isLeftHandAttacking).GetDamageTransform(PlayerCharacterEntity, isLeftHandAttacking);
            Vector3 sourcePosition = damageTransform.position;
            Vector3 targetPosition = entity.OpponentAimTransform.position;
            if (OverlappedEntityHitBox(entity.Entity, sourcePosition, targetPosition, distance))
            {
                // Stop movement to attack
                PlayerCharacterEntity.StopMove();
                // Turn character to attacking target
                TurnCharacterToEntity(entity.Entity);
                // Do action
                RequestAttack();
                // This function may be used by extending classes
                OnAttackOnEntity();
            }
            else
            {
                // Move to target entity
                UpdateTargetEntityPosition(sourcePosition, targetPosition, distance);
            }
        }

        protected virtual void OnAttackOnEntity()
        {

        }

        protected virtual void UseSkillOrMoveToEntity(IDamageableEntity entity, float distance)
        {
            if (queueUsingSkill.skill != null)
            {
                Transform applyTransform = queueUsingSkill.skill.GetApplyTransform(PlayerCharacterEntity, isLeftHandAttacking);
                Vector3 sourcePosition = applyTransform.position;
                Vector3 targetPosition = entity.OpponentAimTransform.position;
                if (entity.GetObjectId() == PlayerCharacterEntity.GetObjectId() /* Applying skill to user? */ ||
                    OverlappedEntityHitBox(entity.Entity, sourcePosition, targetPosition, distance))
                {
                    // Set next frame target action type
                    targetActionType = queueUsingSkill.skill.IsAttack ? TargetActionType.Attack : TargetActionType.Activate;
                    // Stop movement to use skill
                    PlayerCharacterEntity.StopMove();
                    // Turn character to attacking target
                    TurnCharacterToEntity(entity.Entity);
                    // Use the skill
                    RequestUsePendingSkill();
                    // This function may be used by extending classes
                    OnUseSkillOnEntity();
                }
                else
                {
                    // Move to target entity
                    UpdateTargetEntityPosition(sourcePosition, targetPosition, distance);
                }
            }
            else
            {
                // Can't use skill
                targetActionType = TargetActionType.Activate;
                ClearQueueUsingSkill();
                return;
            }
        }

        protected virtual void OnUseSkillOnEntity()
        {

        }

        protected virtual void UpdateTargetEntityPosition(Vector3 sourcePosition, Vector3 targetPosition, float distance)
        {
            if (PlayerCharacterEntity.IsPlayingActionAnimation())
                return;

            Vector3 direction = (targetPosition - sourcePosition).normalized;
            Vector3 position = targetPosition - (direction * (distance - StoppingDistance));
            if (Vector3.Distance(MovementTransform.position, position) > MIN_START_MOVE_DISTANCE &&
                Vector3.Distance(previousPointClickPosition, position) > MIN_START_MOVE_DISTANCE)
            {
                PlayerCharacterEntity.PointClickMovement(position);
                previousPointClickPosition = position;
            }
        }

        protected void TurnCharacterToEntity(BaseGameEntity entity)
        {
            if (entity == null)
                return;
            TurnCharacterToPosition(entity.CacheTransform.position);
        }

        protected void TurnCharacterToPosition(Vector3 position)
        {
            Vector3 lookAtDirection = (position - CacheTransform.position).normalized;
            if (lookAtDirection.sqrMagnitude > 0)
                PlayerCharacterEntity.SetLookRotation(Quaternion.LookRotation(lookAtDirection));
        }

        public override void UseHotkey(HotkeyType type, string relateId, AimPosition aimPosition)
        {
            ClearQueueUsingSkill();
            switch (type)
            {
                case HotkeyType.Skill:
                    UseSkill(relateId, aimPosition);
                    break;
                case HotkeyType.Item:
                    UseItem(relateId, aimPosition);
                    break;
            }
        }

        protected void UseSkill(string id, AimPosition aimPosition)
        {
            BaseSkill skill;
            short skillLevel;
            if (!GameInstance.Skills.TryGetValue(BaseGameData.MakeDataId(id), out skill) || skill == null ||
                !PlayerCharacterEntity.GetCaches().Skills.TryGetValue(skill, out skillLevel))
                return;
            SetQueueUsingSkill(aimPosition, skill, skillLevel);
        }

        protected void UseItem(string id, AimPosition aimPosition)
        {
            int itemIndex;
            BaseItem item;
            int dataId = BaseGameData.MakeDataId(id);
            if (GameInstance.Items.ContainsKey(dataId))
            {
                item = GameInstance.Items[dataId];
                itemIndex = OwningCharacter.IndexOfNonEquipItem(dataId);
            }
            else
            {
                InventoryType inventoryType;
                byte equipWeaponSet;
                CharacterItem characterItem;
                if (PlayerCharacterEntity.IsEquipped(
                    id,
                    out inventoryType,
                    out itemIndex,
                    out equipWeaponSet,
                    out characterItem))
                {
                    GameInstance.ClientInventoryHandlers.RequestUnEquipItem(
                        inventoryType,
                        (short)itemIndex,
                        equipWeaponSet,
                        -1,
                        ClientInventoryActions.ResponseUnEquipArmor,
                        ClientInventoryActions.ResponseUnEquipWeapon);
                    return;
                }
                item = characterItem.GetItem();
            }

            if (itemIndex < 0)
                return;

            if (item == null)
                return;

            if (item.IsEquipment())
            {
                GameInstance.ClientInventoryHandlers.RequestEquipItem(
                        PlayerCharacterEntity,
                        (short)itemIndex,
                        ClientInventoryActions.ResponseEquipArmor,
                        ClientInventoryActions.ResponseEquipWeapon);
            }
            else if (item.IsSkill())
            {
                SetQueueUsingSkill(aimPosition, (item as ISkillItem).UsingSkill, (item as ISkillItem).UsingSkillLevel, (short)itemIndex);
            }
            else if (item.IsBuilding())
            {
                destination = null;
                PlayerCharacterEntity.StopMove();
                buildingItemIndex = itemIndex;
                ShowConstructBuildingDialog();
            }
            else if (item.IsUsable())
            {
                PlayerCharacterEntity.CallServerUseItem((short)itemIndex);
            }
        }

        public override AimPosition UpdateBuildAimControls(Vector2 aimAxes, BuildingEntity prefab)
        {
            // Instantiate constructing building
            if (ConstructingBuildingEntity == null)
            {
                InstantiateConstructingBuilding(prefab);
                buildYRotate = 0;
            }
            // Rotate by keys
            Vector3 buildingAngles = Vector3.zero;
            if (CurrentGameInstance.DimensionType == DimensionType.Dimension3D)
            {
                if (buildRotationSnap)
                {
                    if (InputManager.GetButtonDown("RotateLeft"))
                        buildYRotate -= buildRotateAngle;
                    if (InputManager.GetButtonDown("RotateRight"))
                        buildYRotate += buildRotateAngle;
                    // Make Y rotation set to 0, 90, 180
                    buildingAngles.y = buildYRotate = Mathf.Round(buildYRotate / buildRotateAngle) * buildRotateAngle;
                }
                else
                {
                    float deltaTime = Time.deltaTime;
                    if (InputManager.GetButton("RotateLeft"))
                        buildYRotate -= buildRotateSpeed * deltaTime;
                    if (InputManager.GetButton("RotateRight"))
                        buildYRotate += buildRotateSpeed * deltaTime;
                    // Rotate by set angles
                    buildingAngles.y = buildYRotate;
                }
                ConstructingBuildingEntity.BuildYRotation = buildYRotate;
            }
            ConstructingBuildingEntity.Rotation = Quaternion.Euler(buildingAngles);
            // Find position to place building
            if (InputManager.useMobileInputOnNonMobile || Application.isMobilePlatform)
                FindAndSetBuildingAreaByAxes(aimAxes);
            else
                FindAndSetBuildingAreaByMousePosition();
            return AimPosition.CreatePosition(ConstructingBuildingEntity.Position);
        }

        public override void FinishBuildAimControls(bool isCancel)
        {
            if (isCancel)
                CancelBuild();
        }
    }
}
