﻿namespace MultiplayerARPG
{
    public abstract class UIDataForCharacter<T> : UISelectionEntry<T>
    {
        private ICharacterData character;
        public ICharacterData Character
        {
            get
            {
                if (character != null)
                    return character;
                return GameInstance.PlayingCharacter;
            }
            protected set
            {
                character = value;
            }
        }
        public int IndexOfData { get; protected set; }

        public virtual void Setup(T data, ICharacterData character, int indexOfData)
        {
            Character = character;
            IndexOfData = indexOfData;
            Data = data;
        }

        public bool IsOwningCharacter()
        {
            return Character == GameInstance.PlayingCharacter;
        }
    }
}
