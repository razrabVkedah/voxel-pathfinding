using UnityEngine;

namespace Rusleo.VoxelPathfinding.Runtime.Core
{
    public class Voxel
    {
        public Vector3 Position;
        public Vector3 Size;

        /// <summary>
        /// Level of detail.
        /// LOD0 - Start.
        /// LOD1 - Smaller than LOD0.
        /// LOD2 - Smaller than LOD1.
        /// And so on...
        /// </summary>
        public readonly int LOD;

        public bool IsStatic;
        public Voxel Parent;
        public Voxel[] Children;
        public int AsChildIndex = -1;
        public bool IsCollided;

        //////////
        public readonly Voxel[] Neighbors;
        ///////////

        /// <summary>
        /// Return total count of voxels inside current voxel
        /// </summary>
        /// <param name="includeThis"></param>
        /// <returns></returns>
        public int GetVoxelsCount(bool includeThis)
        {
            var count = includeThis ? 1 : 0;
            if (Children == null) return count;

            foreach (var child in Children)
            {
                count += child.GetVoxelsCount(true);
            }

            return count;
        }

        public Voxel GetParentByLOD(int lod, int fallCount = 10)
        {
            if (lod >= LOD)
            {
                Debug.LogError(
                    $"You trying to get parent by LOD. But input LOD is greater or equal current LOD. {lod} {LOD}");
                return null;
            }

            var tempParent = Parent;

            var fallbackCounter = 0;
            while (fallbackCounter < fallCount)
            {
                if (tempParent == null) return null;
                if (tempParent.LOD == lod) return tempParent;
                tempParent = tempParent.Parent;
                fallbackCounter++;
            }

            Debug.LogWarning($"Got fallback amount... {fallCount}");
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="lod"></param>
        /// <param name="neighborsCount">6 or 18 or 26</param>
        public Voxel(Vector3 position, Vector3 size, int lod, int neighborsCount = 6)
        {
            Position = position;
            Size = size;
            LOD = lod;
            Neighbors = new Voxel[neighborsCount];
        }

        public void SetChildren(Voxel[] children)
        {
            Children = children;
            for (var i = 0; i < Children.Length; i++)
            {
                var child = Children[i];
                child.Parent = this;
                child.AsChildIndex = i;
            }
        }

        public void SetNeighbor(Voxel voxel, int neighborIndex)
        {
            if (LOD != voxel.LOD)
            {
                Debug.LogError("Neighbors error");
                return;
            }

            Neighbors[neighborIndex] = voxel;
            voxel.Neighbors[neighborIndex % 2 == 0 ? neighborIndex + 1 : neighborIndex - 1] = this;
        }

        public override string ToString()
        {
            return $"[Position: {Position.x}, {Position.y}, {Position.z}, LOD: {LOD}, IsCollided: {IsCollided}]";
        }
    }
}