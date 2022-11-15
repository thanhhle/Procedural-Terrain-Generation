﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class ActivateButtonActivator : MonoBehaviour
    {
        public GameObject[] activateObjects;

        private bool canActivate;
        private PlayerCharacterController controller;
        private ShooterPlayerCharacterController shooterController;

        private void LateUpdate()
        {
            canActivate = false;

            controller = BasePlayerCharacterController.Singleton as PlayerCharacterController;
            shooterController = BasePlayerCharacterController.Singleton as ShooterPlayerCharacterController;

            if (controller != null)
            {
                canActivate = controller.ActivatableEntityDetector.players.Count > 0 ||
                    controller.ActivatableEntityDetector.npcs.Count > 0 ||
                    controller.ActivatableEntityDetector.buildings.Count > 0;
            }


            if (shooterController != null && shooterController.SelectedEntity != null)
            {
                canActivate = shooterController.SelectedEntity is BasePlayerCharacterEntity || shooterController.SelectedEntity is NpcEntity;
                if (!canActivate)
                {
                    BuildingEntity buildingEntity = shooterController.SelectedEntity as BuildingEntity;
                    if (buildingEntity != null && !buildingEntity.IsBuildMode && buildingEntity.Activatable)
                        canActivate = true;
                }
            }

            foreach (GameObject obj in activateObjects)
            {
                obj.SetActive(canActivate);
            }
        }
    }
}
