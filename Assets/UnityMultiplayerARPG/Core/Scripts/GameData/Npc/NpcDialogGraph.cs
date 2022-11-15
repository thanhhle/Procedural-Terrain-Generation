﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = "Npc Dialog Graph", menuName = "Create GameData/Npc Dialog Graph", order = -4797)]
    public class NpcDialogGraph : NodeGraph
    {
        public List<BaseNpcDialog> GetDialogs()
        {
            List<BaseNpcDialog> dialogs = new List<BaseNpcDialog>();
            if (nodes != null && nodes.Count > 0)
            {
                for (int i = 0; i < nodes.Count; ++i)
                {
                    nodes[i].name = name + " " + i;
                    dialogs.Add(nodes[i] as BaseNpcDialog);
                }
            }
            return dialogs;
        }
    }
}
