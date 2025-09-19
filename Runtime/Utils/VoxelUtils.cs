using Rusleo.VoxelPathfinding.Runtime.Core;
using Rusleo.VoxelPathfinding.Runtime.Core.Grid;
using UnityEngine;

namespace Rusleo.VoxelPathfinding.Runtime.Utils
{
    public static class VoxelUtils
    {
        private static readonly Collider[] Colliders = new Collider[1];

        public static bool IsVoxelCollideAnything(Voxel a, LayerMask layerMask)
        {
            var size = Physics.OverlapBoxNonAlloc(a.Position, a.Size / 2f, Colliders, Quaternion.identity, layerMask);
            return size > 0;
        }

        public static bool IsPointInsideVoxel(Vector3 point, Voxel voxel)
        {
            return Mathf.Abs(point.x - voxel.Position.x) <= voxel.Size.x / 2f &&
                   Mathf.Abs(point.y - voxel.Position.y) <= voxel.Size.y / 2f &&
                   Mathf.Abs(point.z - voxel.Position.z) <= voxel.Size.z / 2f;
        }

        public static Voxel GetOverlappingVoxel(BaseGrid grid, Vector3 point)
        {
            foreach (var voxel in grid.Voxels)
            {
                if (IsPointInsideVoxel(point, voxel) == false) continue;
                // Debug.Log(
                //    $"Voxel({voxel.Position}): isCollided - {voxel.IsCollided}, hasChildren - {voxel.Children != null}, isPointInside - {IsPointInsideVoxel(point, voxel)}");

                return RecursiveOverlapVoxelSearch(voxel, point);
            }

            Debug.Log($"Can't find voxel. {point}");
            return null;
        }

        private static Voxel RecursiveOverlapVoxelSearch(Voxel voxel, Vector3 point)
        {
            if (voxel.IsCollided == false) return voxel;
            if (voxel.Children == null)
            {
                Debug.Log("Recursive search voxel error");
                return null;
            }

            foreach (var child in voxel.Children)
            {
                if (IsPointInsideVoxel(point, child) == false) continue;

                return RecursiveOverlapVoxelSearch(child, point);
            }

            Debug.Log($"Can't find voxel. {point}");
            return null;
        }
    }
}