﻿using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    [DisallowMultipleComponent]
    public partial class PlayerCharacterBuildingComponent : BaseNetworkedGameEntityComponent<BasePlayerCharacterEntity>
    {
        public bool CallServerConstructBuilding(short itemIndex, Vector3 position, Quaternion rotation, uint parentObjectId)
        {
            if (!Entity.CanDoActions())
                return false;
            RPC(ServerConstructBuilding, itemIndex, position, rotation, parentObjectId);
            return true;
        }

        [ServerRpc]
        protected void ServerConstructBuilding(short itemIndex, Vector3 position, Quaternion rotation, uint parentObjectId)
        {
#if !CLIENT_BUILD
            if (!Entity.CanDoActions() ||
                itemIndex >= Entity.NonEquipItems.Count)
                return;

            BuildingEntity buildingEntity;
            CharacterItem nonEquipItem = Entity.NonEquipItems[itemIndex];
            if (nonEquipItem.IsEmptySlot() ||
                nonEquipItem.GetBuildingItem() == null ||
                nonEquipItem.GetBuildingItem().BuildingEntity == null ||
                !GameInstance.BuildingEntities.TryGetValue(nonEquipItem.GetBuildingItem().BuildingEntity.EntityId, out buildingEntity) ||
                !Entity.DecreaseItemsByIndex(itemIndex, 1))
                return;

            Entity.FillEmptySlots();
            BuildingSaveData buildingSaveData = new BuildingSaveData();
            buildingSaveData.Id = GenericUtils.GetUniqueId();
            buildingSaveData.ParentId = string.Empty;
            BuildingEntity parentBuildingEntity;
            if (Manager.TryGetEntityByObjectId(parentObjectId, out parentBuildingEntity))
                buildingSaveData.ParentId = parentBuildingEntity.Id;
            buildingSaveData.EntityId = buildingEntity.EntityId;
            buildingSaveData.CurrentHp = buildingEntity.MaxHp;
            buildingSaveData.RemainsLifeTime = buildingEntity.LifeTime;
            buildingSaveData.Position = position;
            buildingSaveData.Rotation = rotation;
            buildingSaveData.CreatorId = Entity.Id;
            buildingSaveData.CreatorName = Entity.CharacterName;
            CurrentGameManager.CreateBuildingEntity(buildingSaveData, false);
#endif
        }

        public bool CallServerDestroyBuilding(uint objectId)
        {
            if (!CurrentGameplayRule.CanInteractEntity(Entity, objectId))
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return false;
            }
            RPC(ServerDestroyBuilding, objectId);
            return true;
        }

        [ServerRpc]
        protected void ServerDestroyBuilding(uint objectId)
        {
#if !CLIENT_BUILD
            if (!Entity.CanDoActions())
                return;

            BuildingEntity buildingEntity;
            if (!Manager.TryGetEntityByObjectId(objectId, out buildingEntity))
            {
                // Can't find the entity
                return;
            }

            if (!Entity.IsGameEntityInDistance(buildingEntity, CurrentGameInstance.conversationDistance))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            if (!buildingEntity.IsCreator(Entity))
            {
                // Character is not the creator
                return;
            }

            buildingEntity.Destroy();
#endif
        }

        public bool CallServerOpenDoor(uint objectId, string password)
        {
            if (!CurrentGameplayRule.CanInteractEntity(Entity, objectId))
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return false;
            }
            RPC(ServerOpenDoor, objectId, password);
            return true;
        }

        [ServerRpc]
        protected void ServerOpenDoor(uint objectId, string password)
        {
#if !CLIENT_BUILD
            if (!Entity.CanDoActions())
                return;

            DoorEntity doorEntity;
            if (!Manager.TryGetEntityByObjectId(objectId, out doorEntity))
            {
                // Can't find the entity
                return;
            }

            if (!Entity.IsGameEntityInDistance(doorEntity, CurrentGameInstance.conversationDistance))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            if (doorEntity.Lockable && doorEntity.IsLocked && !doorEntity.LockPassword.Equals(password))
            {
                // Wrong password
                return;
            }

            doorEntity.IsOpen = true;
#endif
        }

        public bool CallServerCloseDoor(uint objectId)
        {
            if (!CurrentGameplayRule.CanInteractEntity(Entity, objectId))
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return false;
            }
            RPC(ServerCloseDoor, objectId);
            return true;
        }

        [ServerRpc]
        protected void ServerCloseDoor(uint objectId)
        {
#if !CLIENT_BUILD
            if (!Entity.CanDoActions())
                return;

            DoorEntity doorEntity;
            if (!Manager.TryGetEntityByObjectId(objectId, out doorEntity))
            {
                // Can't find the entity
                return;
            }

            if (!Entity.IsGameEntityInDistance(doorEntity, CurrentGameInstance.conversationDistance))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            doorEntity.IsOpen = false;
#endif
        }

        public bool CallServerTurnOnCampFire(uint objectId)
        {
            if (!CurrentGameplayRule.CanInteractEntity(Entity, objectId))
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return false;
            }
            RPC(ServerTurnOnCampFire, objectId);
            return true;
        }

        [ServerRpc]
        protected void ServerTurnOnCampFire(uint objectId)
        {
#if !CLIENT_BUILD
            if (!Entity.CanDoActions())
                return;

            CampFireEntity campfireEntity;
            if (!Manager.TryGetEntityByObjectId(objectId, out campfireEntity))
            {
                // Can't find the entity
                return;
            }

            if (!Entity.IsGameEntityInDistance(campfireEntity, CurrentGameInstance.conversationDistance))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            campfireEntity.TurnOn();
#endif
        }

        public bool CallServerTurnOffCampFire(uint objectId)
        {
            if (!CurrentGameplayRule.CanInteractEntity(Entity, objectId))
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return false;
            }
            RPC(ServerTurnOffCampFire, objectId);
            return true;
        }

        [ServerRpc]
        protected void ServerTurnOffCampFire(uint objectId)
        {
#if !CLIENT_BUILD
            if (!Entity.CanDoActions())
                return;

            CampFireEntity campfireEntity;
            if (!Manager.TryGetEntityByObjectId(objectId, out campfireEntity))
            {
                // Can't find the entity
                return;
            }

            if (!Entity.IsGameEntityInDistance(campfireEntity, CurrentGameInstance.conversationDistance))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            campfireEntity.TurnOff();
#endif
        }

        public bool CallServerCraftItemByWorkbench(uint objectId, int dataId)
        {
            RPC(ServerCraftItemByWorkbench, objectId, dataId);
            return true;
        }

        [ServerRpc]
        protected void ServerCraftItemByWorkbench(uint objectId, int dataId)
        {
#if !CLIENT_BUILD
            if (!Entity.CanDoActions())
                return;

            WorkbenchEntity workbenchEntity;
            if (!Manager.TryGetEntityByObjectId(objectId, out workbenchEntity))
            {
                // Can't find the entity
                return;
            }

            if (!Entity.IsGameEntityInDistance(workbenchEntity, CurrentGameInstance.conversationDistance))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            workbenchEntity.CraftItem(Entity, dataId);
#endif
        }

        public bool CallServerSetBuildingPassword(uint objectId, string password)
        {
            if (!CurrentGameplayRule.CanInteractEntity(Entity, objectId))
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return false;
            }
            RPC(ServerSetBuildingPassword, objectId, password);
            return true;
        }

        [ServerRpc]
        protected void ServerSetBuildingPassword(uint objectId, string password)
        {
#if !CLIENT_BUILD
            if (!Entity.CanDoActions())
                return;

            BuildingEntity buildingEntity;
            if (!Manager.TryGetEntityByObjectId(objectId, out buildingEntity))
            {
                // Can't find the entity
                return;
            }

            if (!Entity.IsGameEntityInDistance(buildingEntity, CurrentGameInstance.conversationDistance))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            if (!buildingEntity.IsCreator(Entity))
            {
                // Character is not the creator
                return;
            }

            if (!buildingEntity.Lockable)
            {
                // It's not lockable building
                return;
            }

            buildingEntity.LockPassword = password;
            buildingEntity.IsLocked = true;
#endif
        }

        public bool CallServerLockBuilding(uint objectId)
        {
            if (!CurrentGameplayRule.CanInteractEntity(Entity, objectId))
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return false;
            }
            RPC(ServerLockBuilding, objectId);
            return true;
        }

        [ServerRpc]
        protected void ServerLockBuilding(uint objectId)
        {
#if !CLIENT_BUILD
            if (!Entity.CanDoActions())
                return;

            BuildingEntity buildingEntity;
            if (!Manager.TryGetEntityByObjectId(objectId, out buildingEntity))
            {
                // Can't find the entity
                return;
            }

            if (!Entity.IsGameEntityInDistance(buildingEntity, CurrentGameInstance.conversationDistance))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            if (!buildingEntity.IsCreator(Entity))
            {
                // Character is not the creator
                return;
            }

            if (!buildingEntity.Lockable)
            {
                // It's not lockable building
                return;
            }

            buildingEntity.IsLocked = true;
#endif
        }

        public bool CallServerUnlockBuilding(uint objectId)
        {
            if (!CurrentGameplayRule.CanInteractEntity(Entity, objectId))
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return false;
            }
            RPC(ServerUnlockBuilding, objectId);
            return true;
        }

        [ServerRpc]
        protected void ServerUnlockBuilding(uint objectId)
        {
#if !CLIENT_BUILD
            if (!Entity.CanDoActions())
                return;

            BuildingEntity buildingEntity;
            if (!Manager.TryGetEntityByObjectId(objectId, out buildingEntity))
            {
                // Can't find the entity
                return;
            }

            if (!Entity.IsGameEntityInDistance(buildingEntity, CurrentGameInstance.conversationDistance))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            if (!buildingEntity.IsCreator(Entity))
            {
                // Character is not the creator
                return;
            }

            if (!buildingEntity.Lockable)
            {
                // It's not lockable building
                return;
            }

            buildingEntity.IsLocked = false;
#endif
        }
    }
}
