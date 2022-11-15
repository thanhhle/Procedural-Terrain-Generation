﻿using UnityEngine;

namespace MultiplayerARPG
{
    public enum GameAreaType
    {
        Radius,
        Square,
    }

    public class GameArea : MonoBehaviour
    {
        public const float GROUND_DETECTION_DISTANCE = 100f;
        protected static readonly RaycastHit[] findGroundRaycastHits = new RaycastHit[10];
        public Color gizmosColor = Color.magenta;
        public GameAreaType type;
        [Header("Radius Area")]
        public float randomRadius = 5f;
        [Header("Square Area")]
        public float squareSizeX;
        public float squareSizeZ;
        public float squareGizmosHeight = 5f;

        protected GameInstance CurrentGameInstance { get { return GameInstance.Singleton; } }

        protected IPhysicFunctions physicFunctions;

        public bool GetRandomPosition(out Vector3 randomedPosition)
        {
            randomedPosition = transform.position;

            switch (GameInstance.Singleton.DimensionType)
            {
                case DimensionType.Dimension3D:
                    switch (type)
                    {
                        case GameAreaType.Radius:
                            randomedPosition += new Vector3(Random.Range(-1f, 1f) * randomRadius, 0f, Random.Range(-1f, 1f) * randomRadius);
                            break;
                        case GameAreaType.Square:
                            randomedPosition += new Vector3(Random.Range(-0.5f, 0.5f) * squareSizeX, 0f, Random.Range(-0.5f, 0.5f) * squareSizeZ);
                            break;
                    }
                    return FindGroundedPosition(randomedPosition, GROUND_DETECTION_DISTANCE, out randomedPosition);
                case DimensionType.Dimension2D:
                    switch (type)
                    {
                        case GameAreaType.Radius:
                            randomedPosition += new Vector3(Random.Range(-1f, 1f) * randomRadius, Random.Range(-1f, 1f) * randomRadius);
                            break;
                        case GameAreaType.Square:
                            randomedPosition += new Vector3(Random.Range(-0.5f, 0.5f) * squareSizeX, Random.Range(-0.5f, 0.5f) * squareSizeZ);
                            break;
                    }
                    return true;
            }
            return false;
        }

        public Quaternion GetRandomRotation()
        {
            if (GameInstance.Singleton.DimensionType == DimensionType.Dimension3D)
                return Quaternion.Euler(Vector3.up * Random.Range(0, 360));
            return Quaternion.identity;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = gizmosColor;
            switch (type)
            {
                case GameAreaType.Radius:
                    Gizmos.DrawWireSphere(transform.position, randomRadius);
                    break;
                case GameAreaType.Square:
                    Gizmos.DrawWireCube(transform.position + Vector3.up * squareGizmosHeight * 0.5f * 0.5f, new Vector3(squareSizeX, squareGizmosHeight * 0.5f, squareSizeZ));
                    Gizmos.DrawWireCube(transform.position + Vector3.down * squareGizmosHeight * 0.5f * 0.5f, new Vector3(squareSizeX, squareGizmosHeight * 0.5f, squareSizeZ));
                    break;
            }
        }
#endif

        public bool FindGroundedPosition(Vector3 fromPosition, float findDistance, out Vector3 result)
        {
            return PhysicUtils.FindGroundedPosition(fromPosition, findGroundRaycastHits, findDistance, GroundLayerMask, out result);
        }

        public virtual int GroundLayerMask { get { return -1; } }
    }
}
