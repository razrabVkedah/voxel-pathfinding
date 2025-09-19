using Rusleo.VoxelPathfinding.Runtime.Core.Graph;
using Rusleo.VoxelPathfinding.Runtime.Core.Grid;
using Rusleo.VoxelPathfinding.Runtime.StaticGrid;
using UnityEngine;

namespace Rusleo.VoxelPathfinding.Runtime.Graph
{
    public class GraphBuilder : MonoBehaviour
    {
        [SerializeField] private GridBuilder gridBuilder;
        private BaseGrid _grid;
        private BaseGraph _graph;

        public BaseGraph GetGraph() => _graph;
        public BaseGrid GetGrid() => _grid;

        [ContextMenu("Build Graph")]
        public void Build()
        {
            if (gridBuilder == null)
            {
                Debug.LogError("GridBuilder is null");
                return;
            }

            _grid = gridBuilder.GetGrid();
            if (_grid == null)
            {
                Debug.LogError("Grid is null");
                return;
            }

            _graph = GridToGraphConverter.ConvertGrid(_grid);
        }
    }
}