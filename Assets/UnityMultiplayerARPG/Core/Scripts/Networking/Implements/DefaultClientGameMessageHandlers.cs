﻿using LiteNetLib.Utils;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class DefaultClientGameMessageHandlers : MonoBehaviour, IClientGameMessageHandlers
    {
        public void HandleGameMessage(MessageHandlerData messageHandler)
        {
            GameMessage gameMessage = messageHandler.ReadMessage<GameMessage>();
            ClientGenericActions.ClientReceiveGameMessage(gameMessage.message);
        }

        public void HandleNotifyRewardExp(MessageHandlerData messageHandler)
        {
            ClientGenericActions.NotifyRewardExp(messageHandler.Reader.GetPackedInt());
        }

        public void HandleNotifyRewardGold(MessageHandlerData messageHandler)
        {
            ClientGenericActions.NotifyRewardGold(messageHandler.Reader.GetPackedInt());
        }

        public void HandleNotifyRewardItem(MessageHandlerData messageHandler)
        {
            ClientGenericActions.NotifyRewardItem(
                messageHandler.Reader.GetPackedInt(),
                messageHandler.Reader.GetPackedShort());
        }

        public void HandleNotifyRewardCurrency(MessageHandlerData messageHandler)
        {
            ClientGenericActions.NotifyRewardCurrency(
                messageHandler.Reader.GetPackedInt(),
                messageHandler.Reader.GetPackedInt());
        }

        public void HandleNotifyStorageOpened(MessageHandlerData messageHandler)
        {
            ClientStorageActions.NotifyStorageOpened(
                (StorageType)messageHandler.Reader.GetByte(),
                messageHandler.Reader.GetString(),
                messageHandler.Reader.GetPackedUInt(),
                messageHandler.Reader.GetPackedShort(),
                messageHandler.Reader.GetPackedShort());
        }

        public void HandleNotifyStorageClosed(MessageHandlerData messageHandler)
        {
            ClientStorageActions.NotifyStorageClosed();
        }

        public void HandleNotifyStorageItems(MessageHandlerData messageHandler)
        {
            ClientStorageActions.NotifyStorageItemsUpdated(messageHandler.Reader.GetList<CharacterItem>());
        }

        public void HandleNotifyPartyInvitation(MessageHandlerData messageHandler)
        {
            ClientPartyActions.NotifyPartyInvitation(messageHandler.ReadMessage<PartyInvitationData>());
        }

        public void HandleNotifyGuildInvitation(MessageHandlerData messageHandler)
        {
            ClientGuildActions.NotifyGuildInvitation(messageHandler.ReadMessage<GuildInvitationData>());
        }

        public void HandleUpdatePartyMember(MessageHandlerData messageHandler)
        {
            if (GameInstance.JoinedParty != null)
                GameInstance.JoinedParty.UpdateSocialGroupMember(messageHandler.ReadMessage<UpdateSocialMemberMessage>());
            ClientPartyActions.NotifyPartyUpdated(GameInstance.JoinedParty);
        }

        public void HandleUpdateParty(MessageHandlerData messageHandler)
        {
            UpdatePartyMessage message = messageHandler.ReadMessage<UpdatePartyMessage>();
            if (message.type == UpdatePartyMessage.UpdateType.Create)
            {
                GameInstance.JoinedParty = new PartyData(message.id, message.shareExp, message.shareItem, message.characterId);
            }
            else if (GameInstance.JoinedParty != null && GameInstance.JoinedParty.id == message.id)
            {
                switch (message.type)
                {
                    case UpdatePartyMessage.UpdateType.ChangeLeader:
                        GameInstance.JoinedParty.SetLeader(message.characterId);
                        break;
                    case UpdatePartyMessage.UpdateType.Setting:
                        GameInstance.JoinedParty.Setting(message.shareExp, message.shareItem);
                        break;
                    case UpdatePartyMessage.UpdateType.Terminate:
                        GameInstance.JoinedParty = null;
                        break;
                }
            }
            ClientPartyActions.NotifyPartyUpdated(GameInstance.JoinedParty);
        }

        public void HandleUpdateGuildMember(MessageHandlerData messageHandler)
        {
            if (GameInstance.JoinedGuild != null)
                GameInstance.JoinedGuild.UpdateSocialGroupMember(messageHandler.ReadMessage<UpdateSocialMemberMessage>());
            ClientGuildActions.NotifyGuildUpdated(GameInstance.JoinedGuild);
        }

        public void HandleUpdateGuild(MessageHandlerData messageHandler)
        {
            UpdateGuildMessage message = messageHandler.ReadMessage<UpdateGuildMessage>();
            if (message.type == UpdateGuildMessage.UpdateType.Create)
            {
                GameInstance.JoinedGuild = new GuildData(message.id, message.guildName, message.characterId);
            }
            else if (GameInstance.JoinedGuild != null && GameInstance.JoinedGuild.id == message.id)
            {
                switch (message.type)
                {
                    case UpdateGuildMessage.UpdateType.ChangeLeader:
                        GameInstance.JoinedGuild.SetLeader(message.characterId);
                        break;
                    case UpdateGuildMessage.UpdateType.SetGuildMessage:
                        GameInstance.JoinedGuild.guildMessage = message.guildMessage;
                        break;
                    case UpdateGuildMessage.UpdateType.SetGuildMessage2:
                        GameInstance.JoinedGuild.guildMessage2 = message.guildMessage;
                        break;
                    case UpdateGuildMessage.UpdateType.SetGuildRole:
                        GameInstance.JoinedGuild.SetRole(message.guildRole, message.roleName, message.canInvite, message.canKick, message.shareExpPercentage);
                        break;
                    case UpdateGuildMessage.UpdateType.SetGuildMemberRole:
                        GameInstance.JoinedGuild.SetMemberRole(message.characterId, message.guildRole);
                        break;
                    case UpdateGuildMessage.UpdateType.SetSkillLevel:
                        GameInstance.JoinedGuild.SetSkillLevel(message.dataId, message.level);
                        if (GameInstance.PlayingCharacterEntity != null)
                            GameInstance.PlayingCharacterEntity.ForceMakeCaches();
                        break;
                    case UpdateGuildMessage.UpdateType.SetGold:
                        GameInstance.JoinedGuild.gold = message.gold;
                        break;
                    case UpdateGuildMessage.UpdateType.SetScore:
                        GameInstance.JoinedGuild.score = message.score;
                        break;
                    case UpdateGuildMessage.UpdateType.SetOptions:
                        GameInstance.JoinedGuild.options = message.options;
                        break;
                    case UpdateGuildMessage.UpdateType.SetAutoAcceptRequests:
                        GameInstance.JoinedGuild.autoAcceptRequests = message.autoAcceptRequests;
                        break;
                    case UpdateGuildMessage.UpdateType.SetRank:
                        GameInstance.JoinedGuild.rank = message.rank;
                        break;
                    case UpdateGuildMessage.UpdateType.LevelExpSkillPoint:
                        GameInstance.JoinedGuild.level = message.level;
                        GameInstance.JoinedGuild.exp = message.exp;
                        GameInstance.JoinedGuild.skillPoint = message.skillPoint;
                        break;
                    case UpdateGuildMessage.UpdateType.Terminate:
                        GameInstance.JoinedGuild = null;
                        if (GameInstance.PlayingCharacterEntity != null)
                            GameInstance.PlayingCharacterEntity.ForceMakeCaches();
                        break;
                }
            }
            ClientGuildActions.NotifyGuildUpdated(GameInstance.JoinedGuild);
        }

        public void HandleUpdateFriends(MessageHandlerData messageHandler)
        {
            ClientFriendActions.NotifyFriendsUpdated(messageHandler.Reader.GetList<SocialCharacterData>());
        }
    }
}
