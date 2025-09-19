using Rusleo.VoxelPathfinding.Runtime.Graph;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using VoxelPathfinding.Scripts.Core.Graph;

namespace Rusleo.VoxelPathfinding.Editor.GizmosDebug
{
    public class GraphDrawer : BaseDrawer
    {
        [SerializeField] private GraphBuilder graphBuilder;
        [SerializeField] private bool drawText;
        [SerializeField] private Color textColor;
        [SerializeField] private float[] nodesRadius;
        [SerializeField] private Color[] nodesColors;

        protected override void DrawGizmos()
        {
            if (!graphBuilder) return;
            var graph = graphBuilder.GetGraph();
            if (graph == null) return;

            if (nodesRadius.Length == 0) return;
            if (nodesColors.Length == 0) return;

            foreach (var node in graph.Nodes)
            {
                DrawNode(node);
            }
        }

        private void DrawNode(Node node)
        {
            var c1 = nodesColors[node.LOD < nodesColors.Length ? node.LOD : nodesColors.Length - 1];

            foreach (var connection in node.Connections)
            {
                var c2 = nodesColors[connection.LOD < nodesColors.Length ? connection.LOD : nodesColors.Length - 1];
                Gizmos.color = (c1 + c2) / 2f;
                Gizmos.DrawLine(node.Position, connection.Position);
            }

            Gizmos.color = nodesColors[node.LOD < nodesColors.Length ? node.LOD : nodesColors.Length - 1];

            Gizmos.DrawSphere(node.Position,
                nodesRadius[node.LOD < nodesColors.Length ? node.LOD : nodesColors.Length - 1]);

            if (drawText)
            {
#if UNITY_EDITOR
                Handles.color = textColor;

                var p = node.Position;
                Handles.Label(p, p.ToString());
#endif
            }
        }
    }
}