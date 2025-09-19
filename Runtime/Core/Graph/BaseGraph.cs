using System.Linq;
using UnityEngine;
using VoxelPathfinding.Scripts.Core.Graph;

namespace Rusleo.VoxelPathfinding.Runtime.Core.Graph
{
    public class BaseGraph
    {
        public readonly Node[] Nodes;

        public BaseGraph(Node[] nodes)
        {
            Nodes = nodes;
        }

        public Node FindClosestNode(Vector3 position)
        {
            if (Nodes == null || Nodes.Length == 0)
                return null;

            var closest = (position - Nodes[0].Position).sqrMagnitude;
            var closestNodeIndex = 0;

            for (var i = 1; i < Nodes.Length; i++)
            {
                var distance = (Nodes[i].Position - position).sqrMagnitude;
                if (distance >= closest) continue;

                closest = distance;
                closestNodeIndex = i;
            }

            return Nodes[closestNodeIndex];
        }

        public Node FindNodeByVoxel(Voxel voxel)
        {
            return Nodes.FirstOrDefault(node => node.Voxel == voxel);
        }
    }
}