using System.Collections.Generic;
using UnityEngine;
using VoxelPathfinding.Scripts.Core.Graph;

namespace Rusleo.VoxelPathfinding.Runtime.Core.Path
{
    public class Path
    {
        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public readonly List<Node> PathNodes;
        public List<Vector3> WarpedPath;

        public Path(Vector3 startPosition, Vector3 endPosition, List<Node> pathNodes)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
            PathNodes = pathNodes;
        }
    }
}