using Rusleo.VoxelPathfinding.Runtime.Core;
using Rusleo.VoxelPathfinding.Runtime.StaticGrid;
using UnityEngine;

namespace Rusleo.VoxelPathfinding.Editor.GizmosDebug
{
    public class GridDrawer : BaseDrawer
    {
        [SerializeField] private GridBuilder gridBuilder;
        [SerializeField] private Color[] gizmosLODColors;

        protected override void DrawGizmos()
        {
            if (!gridBuilder) return;
            var grid = gridBuilder.GetGrid();
            if (grid?.Voxels == null) return;
            if (gizmosLODColors.Length == 0) return;

            foreach (var voxel in grid.Voxels)
            {
                DrawVoxel(voxel);
            }
        }

        private void DrawVoxel(Voxel voxel)
        {
            Gizmos.color = gizmosLODColors[voxel.LOD < gizmosLODColors.Length ? voxel.LOD : gizmosLODColors.Length - 1];
            if (voxel.IsCollided == false) Gizmos.DrawWireCube(voxel.Position, voxel.Size);
            if (voxel.Children == null) return;
            foreach (var child in voxel.Children)
            {
                DrawVoxel(child);
            }
        }
    }
}