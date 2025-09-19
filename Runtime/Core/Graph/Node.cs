using System.Collections.Generic;
using Rusleo.VoxelPathfinding.Runtime.Core;
using UnityEngine;

namespace VoxelPathfinding.Scripts.Core.Graph
{
    public class Node
    {
        public readonly Voxel Voxel;
        public readonly int LOD;
        public Vector3 Position;
        public readonly List<Node> Connections;

        public float GCost; // Реальная стоимость пути (от старта до текущей ноды)
        public float HCost; // Эвристика (предполагаемая дистанция до финиша)
        public float FCost() => GCost + HCost; // Сумма реальной стоимости и эвристики
        public Node Parent; // Родительский узел (нужен для восстановления пути)

        public Node(Voxel voxel, int lod)
        {
            LOD = lod;
            Voxel = voxel;
            Position = Voxel.Position;
            Connections = new List<Node>();

            GCost = float.MaxValue;
            HCost = 0;
            Parent = null;
        }

        public void ConnectNode(Node node)
        {
            if (Connections.Contains(node))
            {
                if (node.Connections.Contains(this) == false)
                {
                    node.Connections.Add(this);
                }

                return;
            }

            Connections.Add(node);
            if (node.Connections.Contains(this) == false)
            {
                node.Connections.Add(this);
            }
        }

        public override string ToString()
        {
            return $"[Position: {Position}, LOD: {LOD}, Connections: {Connections.Count}]";
        }
    }
}