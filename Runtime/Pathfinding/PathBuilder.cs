using System.Collections.Generic;
using Rusleo.VoxelPathfinding.Runtime.Core.Path;
using Rusleo.VoxelPathfinding.Runtime.Graph;
using UnityEngine;

namespace Rusleo.VoxelPathfinding.Runtime.Pathfinding
{
    [ExecuteAlways]
    public class PathBuilder : MonoBehaviour
    {
        [SerializeField] private GraphBuilder graphBuilder;
        [SerializeField] private bool executeAlways;
        [SerializeField] private Transform bee;
        [Header("Go to:")] [SerializeField] private Transform target;
        private Path _path;
        private List<Vector3> _warpedPath = new();

        public Path GetPath() => _path;
        public List<Vector3> GetWarpedPath() => _warpedPath;

        private void Update()
        {
            if (executeAlways == false) return;

            Build();
        }

        [ContextMenu("Build Path")]
        public void Build()
        {
            if (target == null)
            {
                Debug.LogError("Target transform in null!");
                return;
            }

            if (graphBuilder == null)
            {
                Debug.LogError("GraphBuilder in null");
                return;
            }

            var graph = graphBuilder.GetGraph();
            if (graph == null)
            {
                Debug.LogError("Graph is null");
                return;
            }

            var path = graph.FindPath(bee.position, target.position);
            _path = new Path(bee.position, target.position, path);
            PathfindingUtility.WarpPath(_path, out _warpedPath);
        }

        public Path BuildPath(Vector3 startPoint, Vector3 endPoint)
        {
            var graph = graphBuilder.GetGraph();
            var grid = graphBuilder.GetGrid();

            var pathNodes = graph.FindPath(grid, startPoint, endPoint);
            if (pathNodes != null) return new Path(startPoint, endPoint, pathNodes);

            Debug.Log("Path building fail");
            return null;
        }
    }
}