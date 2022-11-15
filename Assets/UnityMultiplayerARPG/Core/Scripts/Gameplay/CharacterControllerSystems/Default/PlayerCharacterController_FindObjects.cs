﻿using UnityEngine;

namespace MultiplayerARPG
{
    public partial class PlayerCharacterController
    {
        public const float BUILDING_CONSTRUCTING_GROUND_FINDING_DISTANCE = 100f;

        public int FindClickObjects(out Vector3 worldPosition2D)
        {
            return physicFunctions.RaycastPickObjects(CacheGameplayCameraController.Camera, InputManager.MousePosition(), CurrentGameInstance.GetTargetLayerMask(), 100f, out worldPosition2D);
        }

        public void FindAndSetBuildingAreaByAxes(Vector2 aimAxes)
        {
            Vector3 raycastPosition = CacheTransform.position + (GameplayUtils.GetDirectionByAxes(CacheGameplayCameraController.CameraTransform, aimAxes.x, aimAxes.y) * ConstructingBuildingEntity.BuildDistance);
            if (CurrentGameInstance.DimensionType == DimensionType.Dimension3D)
                raycastPosition += Vector3.up;
            LoopSetBuildingArea(physicFunctions.RaycastDown(raycastPosition, CurrentGameInstance.GetBuildLayerMask(), 100f, QueryTriggerInteraction.Collide));
        }

        public void FindAndSetBuildingAreaByMousePosition()
        {
            LoopSetBuildingArea(physicFunctions.RaycastPickObjects(CacheGameplayCameraController.Camera, InputManager.MousePosition(), CurrentGameInstance.GetBuildLayerMask(), Vector3.Distance(CacheGameplayCameraController.CameraTransform.position, MovementTransform.position) + ConstructingBuildingEntity.BuildDistance, out _, QueryTriggerInteraction.Collide));
        }

        /// <summary>
        /// Return true if found building area
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private bool LoopSetBuildingArea(int count)
        {
            ConstructingBuildingEntity.BuildingArea = null;
            ConstructingBuildingEntity.HitSurface = false;
            BuildingEntity buildingEntity;
            BuildingArea buildingArea;
            Transform tempTransform;
            Vector3 tempRaycastPoint;
            Vector3 snappedPosition = GetBuildingPlacePosition(ConstructingBuildingEntity.Position);
            for (int tempCounter = 0; tempCounter < count; ++tempCounter)
            {
                tempTransform = physicFunctions.GetRaycastTransform(tempCounter);
                if (ConstructingBuildingEntity.CacheTransform.root == tempTransform.root)
                {
                    // Hit collider which is part of constructing building entity, skip it
                    continue;
                }

                tempRaycastPoint = physicFunctions.GetRaycastPoint(tempCounter);
                snappedPosition = GetBuildingPlacePosition(tempRaycastPoint);

                if (CurrentGameInstance.DimensionType == DimensionType.Dimension3D)
                {
                    // Find ground position from upper position
                    bool hitAimmingObject = false;
                    Vector3 raycastOrigin = tempRaycastPoint + Vector3.up * BUILDING_CONSTRUCTING_GROUND_FINDING_DISTANCE * 0.5f;
                    RaycastHit[] groundHits = Physics.RaycastAll(raycastOrigin, Vector3.down, BUILDING_CONSTRUCTING_GROUND_FINDING_DISTANCE, CurrentGameInstance.GetBuildLayerMask());
                    for (int j = 0; j < groundHits.Length; ++j)
                    {
                        if (groundHits[j].transform == tempTransform)
                        {
                            tempRaycastPoint = groundHits[j].point;
                            snappedPosition = GetBuildingPlacePosition(tempRaycastPoint);
                            ConstructingBuildingEntity.Position = GetBuildingPlacePosition(snappedPosition);
                            hitAimmingObject = true;
                            break;
                        }
                    }
                    if (!hitAimmingObject)
                        continue;
                }
                else
                {
                    ConstructingBuildingEntity.Position = GetBuildingPlacePosition(snappedPosition);
                }

                buildingEntity = tempTransform.root.GetComponent<BuildingEntity>();
                buildingArea = tempTransform.GetComponent<BuildingArea>();
                if ((buildingArea == null || !ConstructingBuildingEntity.BuildingTypes.Contains(buildingArea.buildingType))
                    && buildingEntity == null)
                {
                    // Hit surface which is not building area or building entity
                    ConstructingBuildingEntity.BuildingArea = null;
                    ConstructingBuildingEntity.HitSurface = true;
                    break;
                }

                if (buildingArea == null || !ConstructingBuildingEntity.BuildingTypes.Contains(buildingArea.buildingType))
                {
                    // Skip because this area is not allowed to build the building that you are going to build
                    continue;
                }

                ConstructingBuildingEntity.BuildingArea = buildingArea;
                ConstructingBuildingEntity.HitSurface = true;
                return true;
            }
            ConstructingBuildingEntity.Position = GetBuildingPlacePosition(snappedPosition);
            return false;
        }
    }
}
