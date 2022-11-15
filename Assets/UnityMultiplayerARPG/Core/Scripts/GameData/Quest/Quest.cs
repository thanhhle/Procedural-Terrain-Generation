﻿using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public enum QuestTaskType : byte
    {
        KillMonster,
        CollectItem,
        TalkToNpc,
        Custom = 254,
    }

    [CreateAssetMenu(fileName = "Quest", menuName = "Create GameData/Quest", order = -4796)]
    public partial class Quest : BaseGameData
    {
        [Category("Quest Settings")]
        [Tooltip("Requirement to receive quest")]
        public QuestRequirement requirement = default(QuestRequirement);
        public QuestTask[] tasks = new QuestTask[0];
        [Tooltip("Quests which will be abandoned when accept this quest")]
        public Quest[] abandonQuests = new Quest[0];
        public PlayerCharacter changeCharacterClass;
        public int rewardExp = 0;
        public int rewardGold = 0;
        [ArrayElementTitle("currency")]
        public CurrencyAmount[] rewardCurrencies = new CurrencyAmount[0];
        [ArrayElementTitle("item")]
        public ItemAmount[] rewardItems = new ItemAmount[0];
        [ArrayElementTitle("item")]
        public ItemAmount[] selectableRewardItems = new ItemAmount[0];
        [ArrayElementTitle("item")]
        public ItemRandomByWeight[] randomRewardItems = new ItemRandomByWeight[0];
        [Tooltip("If this is `TRUE` character will be able to do this quest repeatedly")]
        public bool canRepeat;

        [System.NonSerialized]
        private HashSet<int> cacheKillMonsterIds;
        public HashSet<int> CacheKillMonsterIds
        {
            get
            {
                if (cacheKillMonsterIds == null)
                {
                    cacheKillMonsterIds = new HashSet<int>();
                    foreach (QuestTask task in tasks)
                    {
                        if (task.taskType == QuestTaskType.KillMonster &&
                            task.monsterCharacterAmount.monster != null &&
                            task.monsterCharacterAmount.amount > 0)
                            cacheKillMonsterIds.Add(task.monsterCharacterAmount.monster.DataId);
                    }
                }
                return cacheKillMonsterIds;
            }
        }

        [System.NonSerialized]
        private Dictionary<Currency, int> cacheRewardCurrencies;
        public Dictionary<Currency, int> CacheRewardCurrencies
        {
            get
            {
                if (cacheRewardCurrencies == null)
                    cacheRewardCurrencies = GameDataHelpers.CombineCurrencies(rewardCurrencies, new Dictionary<Currency, int>());
                return cacheRewardCurrencies;
            }
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            if (tasks != null && tasks.Length > 0)
            {
                foreach (QuestTask task in tasks)
                {
                    GameInstance.AddCharacters(task.monsterCharacterAmount.monster);
                    GameInstance.AddItems(task.itemAmount.item);
                    GameInstance.AddNpcDialogs(task.talkToNpcDialog);
                }
            }
            GameInstance.AddQuests(abandonQuests);
            GameInstance.AddCharacters(changeCharacterClass);
            GameInstance.AddCurrencies(rewardCurrencies);
            GameInstance.AddItems(rewardItems);
            GameInstance.AddItems(selectableRewardItems);
            GameInstance.AddItems(randomRewardItems);
            GameInstance.AddQuests(requirement.completedQuests);
        }

        public bool HaveToTalkToNpc(IPlayerCharacterData character, NpcEntity npcEntity, out int taskIndex, out BaseNpcDialog dialog, out bool completeAfterTalked)
        {
            taskIndex = -1;
            dialog = null;
            completeAfterTalked = false;
            if (tasks == null || tasks.Length == 0)
                return false;
            int indexOfQuest = character.IndexOfQuest(DataId);
            if (indexOfQuest < 0 || character.Quests[indexOfQuest].isComplete)
                return false;
            for (int i = 0; i < tasks.Length; ++i)
            {
                if (tasks[i].taskType != QuestTaskType.TalkToNpc ||
                    tasks[i].npcEntity == null)
                    continue;
                if (tasks[i].npcEntity.EntityId == npcEntity.EntityId)
                {
                    taskIndex = i;
                    dialog = tasks[i].talkToNpcDialog;
                    completeAfterTalked = tasks[i].completeAfterTalked;
                    return true;
                }
            }
            return false;
        }

        public bool CanReceiveQuest(IPlayerCharacterData character)
        {
            // Quest is completed, so don't show the menu which navigate to this dialog
            int indexOfQuest = character.IndexOfQuest(DataId);
            if (indexOfQuest >= 0 && character.Quests[indexOfQuest].isComplete)
                return false;
            // Character's level is lower than requirement
            if (character.Level < requirement.level)
                return false;
            // Character's has difference class
            if (requirement.character != null && requirement.character.DataId != character.DataId)
                return false;
            // Character's not complete all required quests
            if (requirement.completedQuests != null && requirement.completedQuests.Length > 0)
            {
                foreach (Quest quest in requirement.completedQuests)
                {
                    indexOfQuest = character.IndexOfQuest(quest.DataId);
                    if (indexOfQuest < 0)
                        return false;
                    if (!character.Quests[indexOfQuest].isComplete)
                        return false;
                }
            }
            return true;
        }
    }

    [System.Serializable]
    public struct QuestTask
    {
        public QuestTaskType taskType;
        [StringShowConditional(nameof(taskType), nameof(QuestTaskType.KillMonster))]
        public MonsterCharacterAmount monsterCharacterAmount;
        [StringShowConditional(nameof(taskType), nameof(QuestTaskType.CollectItem))]
        public ItemAmount itemAmount;
        [StringShowConditional(nameof(taskType), nameof(QuestTaskType.CollectItem))]
        [Tooltip("If this is `TRUE`, it will not decrease task items when quest completed")]
        public bool doNotDecreaseItemsOnQuestComplete;
        [StringShowConditional(nameof(taskType), nameof(QuestTaskType.TalkToNpc))]
        [Tooltip("Have to talk to this NPC to complete task")]
        public NpcEntity npcEntity;
        [StringShowConditional(nameof(taskType), nameof(QuestTaskType.TalkToNpc))]
        [Tooltip("This dialog will be shown immediately instead of start dialog which set to the NPC")]
        public BaseNpcDialog talkToNpcDialog;
        [StringShowConditional(nameof(taskType), nameof(QuestTaskType.TalkToNpc))]
        [Tooltip("If this is `TRUE` quest will be completed immediately after talked to NPC and all tasks done")]
        public bool completeAfterTalked;
        [StringShowConditional(nameof(taskType), nameof(QuestTaskType.Custom))]
        public BaseCustomQuestTask customQuestTask;
    }
}
