﻿using System.Collections;
using System.Collections.Generic;
using LiteNetLibManager;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public abstract class GameSpawnArea<T> : GameArea where T : LiteNetLibBehaviour
    {
        public class SpawnPrefabData<TPrefab>
        {
            public TPrefab prefab;
            [Min(1)]
            public short level;
            [Min(1)]
            public short amount;
        }

        [Header("Spawning Data")]
        [FormerlySerializedAs("asset")]
        public T prefab;
        [FormerlySerializedAs("level")]
        [Min(1)]
        public short minLevel = 1;
        [Min(1)]
        public short maxLevel = 1;
        [Min(1)]
        public short amount = 1;
        public float respawnPendingEntitiesDelay = 5f;

        public abstract SpawnPrefabData<T>[] SpawningPrefabs { get; }

        protected float respawnPendingEntitiesTimer = 0f;
        protected readonly List<SpawnPrefabData<T>> pending = new List<SpawnPrefabData<T>>();

        protected virtual void Awake()
        {
            gameObject.layer = PhysicLayers.IgnoreRaycast;
        }

        protected virtual void LateUpdate()
        {
            if (pending.Count > 0)
            {
                respawnPendingEntitiesTimer += Time.deltaTime;
                if (respawnPendingEntitiesTimer >= respawnPendingEntitiesDelay)
                {
                    respawnPendingEntitiesTimer = 0f;
                    foreach (SpawnPrefabData<T> pendingEntry in pending)
                    {
                        Logging.LogWarning(ToString(), $"Spawning pending entities, Prefab: {pendingEntry.prefab.name}, Amount: {pendingEntry.amount}.");
                        for (int i = 0; i < pendingEntry.amount; ++i)
                        {
                            Spawn(pendingEntry.prefab, pendingEntry.level, 0);
                        }
                    }
                    pending.Clear();
                }
            }
        }

        public virtual void RegisterPrefabs()
        {
            if (prefab != null)
                BaseGameNetworkManager.Singleton.Assets.RegisterPrefab(prefab.Identity);
            foreach (SpawnPrefabData<T> spawningPrefab in SpawningPrefabs)
            {
                if (spawningPrefab.prefab != null)
                    BaseGameNetworkManager.Singleton.Assets.RegisterPrefab(spawningPrefab.prefab.Identity);
            }
        }

        public virtual void SpawnAll()
        {
            SpawnByAmount(prefab, (short)Random.Range(minLevel, maxLevel), amount);
            foreach (SpawnPrefabData<T> spawningPrefab in SpawningPrefabs)
            {
                SpawnByAmount(spawningPrefab.prefab, spawningPrefab.level, spawningPrefab.amount);
            }
        }

        public virtual void SpawnByAmount(T prefab, short level, int amount)
        {
            for (int i = 0; i < amount; ++i)
            {
                Spawn(prefab, level, 0);
            }
        }

        public virtual Coroutine Spawn(T prefab, short level, float delay)
        {
            return StartCoroutine(SpawnRoutine(prefab, level, delay));
        }

        IEnumerator SpawnRoutine(T prefab, short level, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            SpawnInternal(prefab, level);
        }

        protected abstract T SpawnInternal(T prefab, short level);
    }
}
