using Rusleo.VoxelPathfinding.Runtime.Graph;
using Rusleo.VoxelPathfinding.Runtime.Pathfinding;
using Rusleo.VoxelPathfinding.Runtime.StaticGrid;
using UnityEngine;

namespace Rusleo.VoxelPathfinding.Runtime
{
    public class VoxelSystemTest : MonoBehaviour
    {
        [SerializeField] private GridBuilder gridBuilder;
        [SerializeField] private GraphBuilder graphBuilder;
        [SerializeField] private PathBuilder pathBuilder;
        [SerializeField] private bool buildGrid;
        [SerializeField] private bool buildGraph;
        [SerializeField] private bool buildPath;

        private void OnValidate()
        {
            gridBuilder.Build();
            graphBuilder.Build();
            pathBuilder.Build();
        }

        private void Update()
        {
            if (buildGrid == false) return;

            gridBuilder.Build();

            if (buildGraph == false) return;

            graphBuilder.Build();

            if (buildPath == false) return;

            pathBuilder.Build();
        }
    }
}