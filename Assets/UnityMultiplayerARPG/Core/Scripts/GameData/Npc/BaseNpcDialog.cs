﻿using UnityEngine;
using XNode;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public abstract partial class BaseNpcDialog : Node, IGameData
    {
        [Input]
        public BaseNpcDialog input;
        [Header("NPC Dialog Configs")]
        [Tooltip("Default title")]
        public string title;
        [Tooltip("Titles by language keys")]
        public LanguageData[] titles;
        [Tooltip("Default description")]
        [TextArea]
        public string description;
        [Tooltip("Descriptions by language keys")]
        public LanguageData[] descriptions;
        public Sprite icon;

        #region Generic Data
        public string Id { get { return name; } }
        public string Title
        {
            get { return Language.GetText(titles, title); }
        }
        public string Description
        {
            get { return Language.GetText(descriptions, description); }
        }
        public int DataId { get { return MakeDataId(Id); } }

        public static int MakeDataId(string id)
        {
            return id.GenerateHashId();
        }
        #endregion

#if UNITY_EDITOR
        protected void OnValidate()
        {
            if (Validate())
                EditorUtility.SetDirty(this);
        }
#endif

        public virtual bool Validate()
        {
            return false;
        }

        public virtual void PrepareRelatesData()
        {

        }

        public override object GetValue(NodePort port)
        {
            return port.node;
        }

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            SetDialogByPort(from, to);
        }

        public override void OnRemoveConnection(NodePort port)
        {
            SetDialogByPort(port, null);
        }

        public virtual bool IsPassMenuCondition(IPlayerCharacterData character)
        {
            return true;
        }

        public static BaseNpcDialog GetValidatedDialogOrNull(BaseNpcDialog dialog, BasePlayerCharacterEntity characterEntity)
        {
            if (dialog == null || !dialog.ValidateDialog(characterEntity))
                return null;
            return dialog;
        }

        /// <summary>
        /// This will be called to render current dialog
        /// </summary>
        /// <param name="uiNpcDialog"></param>
        public abstract void RenderUI(UINpcDialog uiNpcDialog);
        /// <summary>
        /// This will be called to un-render previous dialog
        /// </summary>
        /// <param name="uiNpcDialog"></param>
        public abstract void UnrenderUI(UINpcDialog uiNpcDialog);
        /// <summary>
        /// This will be called to validate dialog to determine that it will show to player or not
        /// </summary>
        /// <param name="characterEntity"></param>
        /// <returns></returns>
        public abstract bool ValidateDialog(BasePlayerCharacterEntity characterEntity);
        /// <summary>
        /// Get next dialog by selected menu index
        /// </summary>
        /// <param name="characterEntity"></param>
        /// <param name="menuIndex"></param>
        /// <returns></returns>
        public abstract void GoToNextDialog(BasePlayerCharacterEntity characterEntity, byte menuIndex);
        protected abstract void SetDialogByPort(NodePort from, NodePort to);
        public abstract bool IsShop { get; }
    }
}
