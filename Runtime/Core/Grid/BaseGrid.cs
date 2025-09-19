using Rusleo.VoxelPathfinding.Runtime.Utils;
using UnityEngine;

namespace Rusleo.VoxelPathfinding.Runtime.Core.Grid
{
    public class BaseGrid
    {
        /// <summary>
        /// Array of LOD0 voxels
        /// </summary>
        public Voxel[] Voxels;

        public Vector3Int GridSize;
        public Vector3 GridCenter;
        public int LevelOfDetail;
        public LayerMask LayerMask;
        public Vector3 VoxelSize;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridSize"></param>
        /// <param name="gridCenter"></param>
        /// <param name="lod0VoxelSize"></param>
        /// <param name="levelOfDetail">Depth of grid. 0 - one layer, 1 - two layers...</param>
        /// <param name="layerMask"></param>
        public BaseGrid(Vector3Int gridSize, Vector3 gridCenter, Vector3 lod0VoxelSize, int levelOfDetail,
            LayerMask layerMask)
        {
            if (levelOfDetail < 0)
            {
                Debug.LogError($"Wrong level of detail value: {levelOfDetail}");
                return;
            }

            GridSize = gridSize;
            GridCenter = gridCenter;
            LevelOfDetail = levelOfDetail;
            LayerMask = layerMask;
            Voxels = new Voxel[gridSize.x * gridSize.y * gridSize.z];
            VoxelSize = lod0VoxelSize;

            GenerateGrid();
        }

        private void GenerateGrid()
        {
            var counter = 0;

            for (var x = 0; x < GridSize.x; x++)
            {
                var posX = (x - (GridSize.x - 1) / 2.0f) * VoxelSize.x;
                for (var y = 0; y < GridSize.y; y++)
                {
                    var posY = (y - (GridSize.y - 1) / 2.0f) * VoxelSize.y;
                    for (var z = 0; z < GridSize.z; z++)
                    {
                        var posZ = (z - (GridSize.z - 1) / 2.0f) * VoxelSize.z;

                        var voxel = CreateVoxel(new Vector3(posX, posY, posZ) + GridCenter, VoxelSize, 0);
                        if (x > 0)
                        {
                            var leftIndex = z + y * GridSize.x + (x - 1) * GridSize.x * GridSize.y;
                            voxel.SetNeighbor(Voxels[leftIndex], 0);
                        }

                        if (y > 0)
                        {
                            var belowIndex = z + (y - 1) * GridSize.x + x * GridSize.x * GridSize.y;
                            voxel.SetNeighbor(Voxels[belowIndex], 2);
                        }

                        if (z > 0)
                        {
                            var backIndex = z - 1 + y * GridSize.x + x * GridSize.x * GridSize.y;
                            voxel.SetNeighbor(Voxels[backIndex], 4);
                        }

                        Voxels[counter] = voxel;
                        counter++;
                    }
                }
            }
        }

        private Voxel CreateVoxel(Vector3 pos, Vector3 size, int voxelLOD)
        {
            var voxel = new Voxel(pos, size, voxelLOD);
            if (VoxelUtils.IsVoxelCollideAnything(voxel, LayerMask) == false) return voxel;

            voxel.IsCollided = true;
            var nextLevelOfDetail = voxelLOD + 1;
            if (nextLevelOfDetail > LevelOfDetail) return voxel;


            var children = new Voxel[8];
            var voxelsSize = size / 2f;
            var tempOffset = voxelsSize / 2f;

            children[0] = CreateVoxel(
                new Vector3(pos.x - tempOffset.x, pos.y - tempOffset.y, pos.z - tempOffset.z),
                voxelsSize, nextLevelOfDetail);
            children[1] = CreateVoxel(
                new Vector3(pos.x + tempOffset.x, pos.y - tempOffset.y, pos.z - tempOffset.z),
                voxelsSize, nextLevelOfDetail);
            children[2] = CreateVoxel(
                new Vector3(pos.x - tempOffset.x, pos.y - tempOffset.y, pos.z + tempOffset.z),
                voxelsSize, nextLevelOfDetail);
            children[3] = CreateVoxel(
                new Vector3(pos.x + tempOffset.x, pos.y - tempOffset.y, pos.z + tempOffset.z),
                voxelsSize, nextLevelOfDetail);
            children[4] = CreateVoxel(
                new Vector3(pos.x - tempOffset.x, pos.y + tempOffset.y, pos.z - tempOffset.z),
                voxelsSize, nextLevelOfDetail);
            children[5] = CreateVoxel(
                new Vector3(pos.x + tempOffset.x, pos.y + tempOffset.y, pos.z - tempOffset.z),
                voxelsSize, nextLevelOfDetail);
            children[6] = CreateVoxel(
                new Vector3(pos.x - tempOffset.x, pos.y + tempOffset.y, pos.z + tempOffset.z),
                voxelsSize, nextLevelOfDetail);
            children[7] = CreateVoxel(
                new Vector3(pos.x + tempOffset.x, pos.y + tempOffset.y, pos.z + tempOffset.z),
                voxelsSize, nextLevelOfDetail);

            // Neighbors: -x = 0; +x = 1, -y = 2, +y = 3, -z = 4, +z = 5
            children[0].SetNeighbor(children[1], 1);
            children[0].SetNeighbor(children[4], 3);
            children[0].SetNeighbor(children[2], 5);
            children[1].SetNeighbor(children[5], 3);
            children[1].SetNeighbor(children[3], 5);
            children[2].SetNeighbor(children[3], 1);
            children[2].SetNeighbor(children[6], 3);
            children[3].SetNeighbor(children[7], 3);
            children[4].SetNeighbor(children[5], 1);
            children[4].SetNeighbor(children[6], 5);
            children[5].SetNeighbor(children[7], 5);
            children[6].SetNeighbor(children[7], 1);
            voxel.SetChildren(children);

            return voxel;
        }
    }
}