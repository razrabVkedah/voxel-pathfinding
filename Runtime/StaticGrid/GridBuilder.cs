using Rusleo.VoxelPathfinding.Runtime.Core.Grid;
using UnityEngine;

namespace Rusleo.VoxelPathfinding.Runtime.StaticGrid
{
    [ExecuteAlways]
    public class GridBuilder : MonoBehaviour
    {
        [SerializeField] private Vector3Int gridSize;
        [SerializeField] private Vector3 log0VoxelSize;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private int levelOfDetailDepth = 3;
        private BaseGrid _grid;

        public BaseGrid GetGrid() => _grid;

        [ContextMenu("Build Grid")]
        public void Build()
        {
            _grid = new BaseGrid(gridSize, transform.position, log0VoxelSize, levelOfDetailDepth, layerMask);
        }
    }
}