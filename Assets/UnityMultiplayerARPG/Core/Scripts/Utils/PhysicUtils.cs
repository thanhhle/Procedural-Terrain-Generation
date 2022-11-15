﻿using System.Collections.Generic;
using UnityEngine;

public static class PhysicUtils
{
    /// <summary>
    /// Physics2D.OverlapCircleNonAlloc then sort ASC
    /// </summary>
    /// <param name="position"></param>
    /// <param name="distance"></param>
    /// <param name="colliders"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    public static int SortedOverlapCircleNonAlloc(Vector3 position, float distance, Collider2D[] colliders, int layerMask)
    {
        int count = Physics2D.OverlapCircleNonAlloc(position, distance, colliders, layerMask);
        System.Array.Sort(colliders, 0, count, new ColliderComparer(position));
        return count;
    }

    /// <summary>
    /// Physics.SortedOverlapSphereNonAlloc then sort ASC
    /// </summary>
    /// <param name="position"></param>
    /// <param name="distance"></param>
    /// <param name="colliders"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    public static int SortedOverlapSphereNonAlloc(Vector3 position, float distance, Collider[] colliders, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        int count = Physics.OverlapSphereNonAlloc(position, distance, colliders, layerMask, queryTriggerInteraction);
        System.Array.Sort(colliders, 0, count, new ColliderComparer(position));
        return count;
    }

    /// <summary>
    /// Physics2D.RaycastNonAlloc then sort ASC
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="hits"></param>
    /// <param name="distance"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    public static int SortedRaycastNonAlloc2D(Ray2D ray, RaycastHit2D[] hits, float distance, int layerMask)
    {
        return SortedRaycastNonAlloc2D(ray.origin, ray.direction, hits, distance, layerMask);
    }

    /// <summary>
    /// Physics2D.RaycastNonAlloc then sort ASC
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="hits"></param>
    /// <param name="distance"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    public static int SortedRaycastNonAlloc2D(Vector3 origin, Vector3 direction, RaycastHit2D[] hits, float distance, int layerMask)
    {
        int count = Physics2D.RaycastNonAlloc(origin, direction, hits, distance, layerMask);
        System.Array.Sort(hits, 0, count, new RaycastHitComparer());
        return count;
    }

    /// <summary>
    /// Physics.RaycastNonAlloc then sort ASC
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="hits"></param>
    /// <param name="distance"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    public static int SortedRaycastNonAlloc3D(Ray ray, RaycastHit[] hits, float distance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        return SortedRaycastNonAlloc3D(ray.origin, ray.direction, hits, distance, layerMask, queryTriggerInteraction);
    }

    /// <summary>
    /// Physics.RaycastNonAlloc then sort ASC
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="hits"></param>
    /// <param name="distance"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    public static int SortedRaycastNonAlloc3D(Vector3 origin, Vector3 direction, RaycastHit[] hits, float distance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        int count = Physics.RaycastNonAlloc(origin, direction, hits, distance, layerMask, queryTriggerInteraction);
        System.Array.Sort(hits, 0, count, new RaycastHitComparer());
        return count;
    }

    /// <summary>
    /// Physics2D.LinecastNonAlloc then sort ASC
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="hits"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    public static int SortedLinecastNonAlloc2D(Vector2 start, Vector2 end, RaycastHit2D[] hits, int layerMask)
    {
        int count = Physics2D.LinecastNonAlloc(start, end, hits, layerMask);
        System.Array.Sort(hits, 0, count, new RaycastHitComparer());
        return count;
    }

    public static Vector3 FindGroundedPosition(Vector3 origin, int raycastLength, float distance, int layerMask, Transform exceptionObject = null)
    {
        return FindGroundedPosition(origin, new RaycastHit[raycastLength], distance, layerMask, exceptionObject);
    }

    public static Vector3 FindGroundedPosition(Vector3 origin, RaycastHit[] allocHits, float distance, int layerMask, Transform exceptionObject = null)
    {
        Vector3 result;
        FindGroundedPosition(origin, allocHits, distance, layerMask, out result, exceptionObject);
        return result;
    }

    public static bool FindGroundedPosition(Vector3 origin, RaycastHit[] allocHits, float distance, int layerMask, out Vector3 result, Transform exceptionObject = null)
    {
        result = origin;
        float nearestDistance = float.MaxValue;
        bool foundGround = false;
        float tempDistance;
        // Raycast to find hit floor
        int hitCount = Physics.RaycastNonAlloc(origin + (Vector3.up * distance * 0.5f), Vector3.down, allocHits, distance, layerMask, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < hitCount; ++i)
        {
            if (exceptionObject != null && exceptionObject.root == allocHits[i].transform.root)
                continue;
            tempDistance = Vector3.Distance(origin, allocHits[i].point);
            if (tempDistance < nearestDistance)
            {
                result = allocHits[i].point;
                nearestDistance = tempDistance;
                foundGround = true;
            }
        }
        return foundGround;
    }

    /// <summary>
    /// Sort ASC by distance from position to collider's position
    /// </summary>
    public struct ColliderComparer : IComparer<Collider>, IComparer<Collider2D>
    {
        private Vector3 position;
        public ColliderComparer(Vector3 position)
        {
            this.position = position;
        }

        public int Compare(Collider x, Collider y)
        {
            return Vector3.Distance(position, x.transform.position)
                .CompareTo(Vector3.Distance(position, y.transform.position));
        }

        public int Compare(Collider2D x, Collider2D y)
        {
            return Vector3.Distance(position, x.transform.position)
                .CompareTo(Vector3.Distance(position, y.transform.position));
        }
    }


    /// <summary>
    /// Sort ASC by distance from origin to impact point
    /// </summary>
    public struct RaycastHitComparer : IComparer<RaycastHit>, IComparer<RaycastHit2D>
    {
        public int Compare(RaycastHit x, RaycastHit y)
        {
            return x.distance.CompareTo(y.distance);
        }

        public int Compare(RaycastHit2D x, RaycastHit2D y)
        {
            return x.distance.CompareTo(y.distance);
        }
    }


    /// <summary>
    /// Sort ASC by distance from origin to impact point
    /// </summary>
    public struct RaycastHitComparerCustomOrigin : IComparer<RaycastHit>, IComparer<RaycastHit2D>
    {
        private Vector3 customOrigin;
        public RaycastHitComparerCustomOrigin(Vector3 customOrigin)
        {
            this.customOrigin = customOrigin;
        }

        public int Compare(RaycastHit x, RaycastHit y)
        {
            return Vector3.Distance(customOrigin, x.point)
                .CompareTo(Vector3.Distance(customOrigin, y.point));
        }

        public int Compare(RaycastHit2D x, RaycastHit2D y)
        {
            return Vector3.Distance(customOrigin, x.point)
                .CompareTo(Vector3.Distance(customOrigin, y.point));
        }
    }
}
