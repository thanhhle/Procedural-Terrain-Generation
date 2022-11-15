﻿using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct ResponseGetPartyMessage : INetSerializable
    {
        public PartyData party;

        public void Deserialize(NetDataReader reader)
        {
            bool notNull = reader.GetBool();
            if (notNull)
            {
                party = new PartyData();
                party.Deserialize(reader);
            }
        }

        public void Serialize(NetDataWriter writer)
        {
            bool notNull = party != null;
            writer.Put(notNull);
            if (notNull)
                party.Serialize(writer);
        }
    }
}
