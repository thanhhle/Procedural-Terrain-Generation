﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

namespace LiteNetLibManager
{
    public abstract partial class LiteNetLibSyncList : LiteNetLibElement
    {
        public partial struct Operation
        {
            public const byte Add = 0;
            public const byte Clear = 1;
            public const byte Insert = 2;
            public const byte RemoveAt = 3;
            public const byte Set = 4;
            public const byte Dirty = 5;
            public const byte RemoveFirst = 6;
            public const byte RemoveLast = 7;
            public const byte AddInitial = 8;

            public Operation(byte value)
            {
                Value = value;
            }

            public byte Value { get; private set; }

            public static implicit operator byte(Operation operation)
            {
                return operation.Value;
            }

            public static implicit operator Operation(byte value)
            {
                return new Operation(value);
            }
        }

        public delegate void OnOperationDelegate(Operation op, int itemIndex);

        [Tooltip("Sending data channel")]
        public byte dataChannel = 0;
        [Tooltip("If this is `TRUE`, this will update to owner client only, default is `FALSE`")]
        public bool forOwnerOnly = false;
        public OnOperationDelegate onOperation;

        public abstract Type FieldType { get; }
        public abstract int Count { get; }
        internal abstract void Reset();
        internal abstract void SendInitialList(long connectionId);
        internal abstract void SendOperations();
        internal abstract void ProcessOperations(NetDataReader reader);

        protected override bool CanSync()
        {
            return IsServer;
        }

        internal override sealed void Setup(LiteNetLibBehaviour behaviour, int elementId)
        {
            base.Setup(behaviour, elementId);
            if (Count > 0 && onOperation != null)
            {
                for (int i = 0; i < Count; ++i)
                {
                    onOperation.Invoke(Operation.AddInitial, i);
                }
            }
        }
    }

    public class LiteNetLibSyncList<TType> : LiteNetLibSyncList, IList<TType>
    {
        protected struct OperationEntry
        {
            public Operation operation;
            public int index;
            public TType data;
            public int count;
        }

        protected readonly List<TType> list = new List<TType>();
        protected readonly List<OperationEntry> operationEntries = new List<OperationEntry>();

        public TType this[int index]
        {
            get { return list[index]; }
            set
            {
                if (IsSetup && !IsServer)
                {
                    Logging.LogError(LogTag, "Cannot access sync list from client.");
                    return;
                }
                list[index] = value;
                PrepareOperation(Operation.Set, index);
            }
        }

        public override sealed Type FieldType
        {
            get { return typeof(TType); }
        }

        public override sealed int Count
        {
            get { return list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public TType Get(int index)
        {
            return this[index];
        }

        public void Set(int index, TType value)
        {
            this[index] = value;
        }

        public void Add(TType item)
        {
            if (IsSetup && !CanSync())
            {
                Logging.LogError(LogTag, "Cannot access sync list from client.");
                return;
            }
            int index = list.Count;
            list.Add(item);
            PrepareOperation(Operation.Add, index);
        }

        public void AddRange(IEnumerable<TType> collection)
        {
            if (IsSetup && !CanSync())
            {
                Logging.LogError(LogTag, "Cannot access sync list from client.");
                return;
            }
            foreach (TType item in collection)
            {
                Add(item);
            }
        }

        public void Insert(int index, TType item)
        {
            if (IsSetup && !CanSync())
            {
                Logging.LogError(LogTag, "Cannot access sync list from client.");
                return;
            }
            list.Insert(index, item);
            PrepareOperation(Operation.Insert, index);
        }

        public bool Contains(TType item)
        {
            return list.Contains(item);
        }

        public int IndexOf(TType item)
        {
            return list.IndexOf(item);
        }

        public bool Remove(TType value)
        {
            if (IsSetup && !CanSync())
            {
                Logging.LogError(LogTag, "Cannot access sync list from client.");
                return false;
            }
            int index = IndexOf(value);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            if (IsSetup && !CanSync())
            {
                Logging.LogError(LogTag, "Cannot access sync list from client.");
                return;
            }
            if (index == 0)
            {
                list.RemoveAt(index);
                PrepareOperation(Operation.RemoveFirst, 0);
            }
            else if (index == list.Count - 1)
            {
                list.RemoveAt(index);
                PrepareOperation(Operation.RemoveLast, index);
            }
            else
            {
                list.RemoveAt(index);
                PrepareOperation(Operation.RemoveAt, index);
            }
        }

        public void Clear()
        {
            if (IsSetup && !CanSync())
            {
                Logging.LogError(LogTag, "Cannot access sync list from client.");
                return;
            }
            list.Clear();
            PrepareOperation(Operation.Clear, -1);
        }

        public void CopyTo(TType[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TType> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public void Dirty(int index)
        {
            if (IsSetup && !CanSync())
            {
                Logging.LogError(LogTag, "Cannot access sync list from client.");
                return;
            }
            PrepareOperation(Operation.Dirty, index);
        }

        internal override sealed void Reset()
        {
            list.Clear();
        }

        protected void PrepareOperation(Operation operation, int index)
        {
            OnOperation(operation, index);
            PrepareOperation(operationEntries, operation, index);
        }

        protected void PrepareOperation(List<OperationEntry> operationEntries, Operation operation, int index)
        {
            switch (operation)
            {
                case Operation.Clear:
                    operationEntries.Add(new OperationEntry()
                    {
                        operation = operation,
                        index = index,
                        count = 0,
                    });
                    break;
                case Operation.RemoveAt:
                case Operation.RemoveFirst:
                case Operation.RemoveLast:
                    operationEntries.Add(new OperationEntry()
                    {
                        operation = operation,
                        index = index,
                        count = 1,
                    });
                    break;
                default:
                    operationEntries.Add(new OperationEntry()
                    {
                        operation = operation,
                        index = index,
                        data = list[index],
                        count = 1,
                    });
                    break;
            }
        }

        internal override sealed void SendInitialList(long connectionId)
        {
            if (!CanSync())
                return;
            List<OperationEntry> addInitialOperationEntries = new List<OperationEntry>();
            for (int i = 0; i < Count; ++i)
            {
                if (!ContainsAddOperation(i))
                    PrepareOperation(addInitialOperationEntries, Operation.AddInitial, i);
            }
            LiteNetLibServer server = Manager.Server;
            TransportHandler.WritePacket(server.Writer, GameMsgTypes.OperateSyncList);
            SerializeForSendOperations(server.Writer, addInitialOperationEntries);
            server.SendMessage(connectionId, dataChannel, DeliveryMethod.ReliableOrdered, server.Writer);
        }

        private bool ContainsAddOperation(int index)
        {
            for (int i = 0; i < operationEntries.Count; ++i)
            {
                if (operationEntries[i].operation == Operation.Add && operationEntries[i].index == index)
                    return true;
            }
            return false;
        }

        internal override sealed void SendOperations()
        {
            if (operationEntries.Count <= 0 || !CanSync())
                return;
            LiteNetLibGameManager manager = Manager;
            LiteNetLibServer server = manager.Server;
            TransportHandler.WritePacket(manager.Server.Writer, GameMsgTypes.OperateSyncList);
            SerializeForSendOperations(manager.Server.Writer, operationEntries);
            operationEntries.Clear();
            if (forOwnerOnly)
            {
                if (manager.ContainsConnectionId(ConnectionId))
                    server.SendMessage(ConnectionId, dataChannel, DeliveryMethod.ReliableOrdered, server.Writer);
            }
            else
            {
                foreach (long connectionId in manager.GetConnectionIds())
                {
                    if (Identity.HasSubscriberOrIsOwning(connectionId))
                        server.SendMessage(connectionId, dataChannel, DeliveryMethod.ReliableOrdered, server.Writer);
                }
            }
        }

        internal override sealed void ProcessOperations(NetDataReader reader)
        {
            int operationCount = reader.GetPackedInt();
            for (int i = 0; i < operationCount; ++i)
            {
                DeserializeOperation(reader);
            }
        }

        protected void SerializeForSendOperations(NetDataWriter writer, List<OperationEntry> operationEntries)
        {
            LiteNetLibElementInfo.SerializeInfo(GetInfo(), writer);
            writer.PutPackedInt(operationEntries.Count);
            for (int i = 0; i < operationEntries.Count; ++i)
            {
                SerializeOperation(writer, operationEntries[i]);
            }
        }

        protected void DeserializeOperation(NetDataReader reader)
        {
            Operation operation = reader.GetByte();
            int index = -1;
            TType item;
            switch (operation)
            {
                case Operation.Add:
                case Operation.AddInitial:
                    item = DeserializeValueForAddOrInsert(index, reader);
                    index = list.Count;
                    list.Add(item);
                    break;
                case Operation.Insert:
                    index = reader.GetInt();
                    item = DeserializeValueForAddOrInsert(index, reader);
                    list.Insert(index, item);
                    break;
                case Operation.Set:
                case Operation.Dirty:
                    index = reader.GetInt();
                    item = DeserializeValueForSetOrDirty(index, reader);
                    list[index] = item;
                    break;
                case Operation.RemoveAt:
                    index = reader.GetInt();
                    list.RemoveAt(index);
                    break;
                case Operation.RemoveFirst:
                    index = 0;
                    list.RemoveAt(index);
                    break;
                case Operation.RemoveLast:
                    index = list.Count - 1;
                    list.RemoveAt(index);
                    break;
                case Operation.Clear:
                    list.Clear();
                    break;
                default:
                    index = reader.GetInt();
                    item = DeserializeValueForCustomDirty(index, operation, reader);
                    list[index] = item;
                    break;
            }
            OnOperation(operation, index);
        }

        protected void SerializeOperation(NetDataWriter writer, OperationEntry entry)
        {
            writer.Put((byte)entry.operation);
            switch (entry.operation)
            {
                case Operation.Add:
                case Operation.AddInitial:
                    SerializeValueForAddOrInsert(entry.index, writer, entry.data);
                    break;
                case Operation.Insert:
                    writer.Put(entry.index);
                    SerializeValueForAddOrInsert(entry.index, writer, entry.data);
                    break;
                case Operation.Set:
                case Operation.Dirty:
                    writer.Put(entry.index);
                    SerializeValueForSetOrDirty(entry.index, writer, entry.data);
                    break;
                case Operation.RemoveAt:
                    writer.Put(entry.index);
                    break;
                case Operation.RemoveFirst:
                case Operation.RemoveLast:
                case Operation.Clear:
                    break;
                default:
                    writer.Put(entry.index);
                    SerializeValueForCustomDirty(entry.index, entry.operation, writer, entry.data);
                    break;
            }
        }

        protected virtual TType DeserializeValue(NetDataReader reader)
        {
            return reader.GetValue<TType>();
        }

        protected virtual void SerializeValue(NetDataWriter writer, TType value)
        {
            writer.PutValue(value);
        }

        protected virtual TType DeserializeValueForAddOrInsert(int index, NetDataReader reader)
        {
            return DeserializeValue(reader);
        }

        protected virtual void SerializeValueForAddOrInsert(int index, NetDataWriter writer, TType value)
        {
            SerializeValue(writer, value);
        }

        protected virtual TType DeserializeValueForSetOrDirty(int index, NetDataReader reader)
        {
            return DeserializeValue(reader);
        }

        protected virtual void SerializeValueForSetOrDirty(int index, NetDataWriter writer, TType value)
        {
            SerializeValue(writer, value);
        }

        protected virtual void SerializeValueForCustomDirty(int index, byte customOperation, NetDataWriter writer, TType value)
        {
            SerializeValue(writer, value);
        }

        protected virtual TType DeserializeValueForCustomDirty(int index, byte customOperation, NetDataReader reader)
        {
            return DeserializeValue(reader);
        }

        protected void OnOperation(Operation operation, int index)
        {
            if (onOperation != null)
                onOperation.Invoke(operation, index);
        }
    }

    #region Implement for general usages and serializable
    // Generics

    [Serializable]
    public class SyncListBool : LiteNetLibSyncList<bool>
    {
    }

    [Serializable]
    public class SyncListByte : LiteNetLibSyncList<byte>
    {
    }

    [Serializable]
    public class SyncListChar : LiteNetLibSyncList<char>
    {
    }

    [Serializable]
    public class SyncListDouble : LiteNetLibSyncList<double>
    {
    }

    [Serializable]
    public class SyncListFloat : LiteNetLibSyncList<float>
    {
    }

    [Serializable]
    public class SyncListInt : LiteNetLibSyncList<int>
    {
    }

    [Serializable]
    public class SyncListLong : LiteNetLibSyncList<long>
    {
    }

    [Serializable]
    public class SyncListSByte : LiteNetLibSyncList<sbyte>
    {
    }

    [Serializable]
    public class SyncListShort : LiteNetLibSyncList<short>
    {
    }

    [Serializable]
    public class SyncListString : LiteNetLibSyncList<string>
    {
    }

    [Serializable]
    public class SyncListUInt : LiteNetLibSyncList<uint>
    {
    }

    [Serializable]
    public class SyncListULong : LiteNetLibSyncList<ulong>
    {
    }

    [Serializable]
    public class SyncListUShort : LiteNetLibSyncList<ushort>
    {
    }

    // Unity

    [Serializable]
    public class SyncListColor : LiteNetLibSyncList<Color>
    {
    }

    [Serializable]
    public class SyncListQuaternion : LiteNetLibSyncList<Quaternion>
    {
    }

    [Serializable]
    public class SyncListVector2 : LiteNetLibSyncList<Vector2>
    {
    }

    [Serializable]
    public class SyncListVector2Int : LiteNetLibSyncList<Vector2Int>
    {
    }

    [Serializable]
    public class SyncListVector3 : LiteNetLibSyncList<Vector3>
    {
    }

    [Serializable]
    public class SyncListVector3Int : LiteNetLibSyncList<Vector3Int>
    {
    }

    [Serializable]
    public class SyncListVector4 : LiteNetLibSyncList<Vector4>
    {
    }

    [Serializable]
    public class SyncListDirectionVector2 : LiteNetLibSyncList<DirectionVector2>
    {
    }

    [Serializable]
    public class SyncListDirectionVector3 : LiteNetLibSyncList<DirectionVector3>
    {
    }
    #endregion
}
