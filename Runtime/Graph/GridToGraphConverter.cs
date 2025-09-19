using System;
using System.Collections.Generic;
using System.Linq;
using Rusleo.VoxelPathfinding.Runtime.Core;
using Rusleo.VoxelPathfinding.Runtime.Core.Graph;
using Rusleo.VoxelPathfinding.Runtime.Core.Grid;
using UnityEngine;
using VoxelPathfinding.Scripts.Core.Graph;

namespace Rusleo.VoxelPathfinding.Runtime.Graph
{
    public static class GridToGraphConverter
    {
        public static BaseGraph ConvertGrid(BaseGrid grid)
        {
            var nodes = new Dictionary<Voxel, Node>();

            foreach (var rootVoxel in grid.Voxels)
            {
                RecursiveConvert(rootVoxel, nodes);
            }

            return new BaseGraph(nodes.Values.ToArray());
        }

        private static void RecursiveConvert(Voxel voxel, Dictionary<Voxel, Node> nodes)
        {
            if (voxel.Children == null)
            {
                if (voxel.IsCollided) return;

                TryFindAndConnectAllNeighbors(voxel, nodes);
                return;
            }

            foreach (var voxelChild in voxel.Children)
            {
                if (voxelChild == null) continue;
                RecursiveConvert(voxelChild, nodes);
            }
        }

        /// <summary>
        /// Сюда передается воксель без дочерних объектов
        /// </summary>
        /// <param name="voxel"></param>
        /// <param name="nodes"></param>
        private static void TryFindAndConnectAllNeighbors(Voxel voxel, Dictionary<Voxel, Node> nodes)
        {
            if (Vector3.Distance(voxel.Position, new Vector3(-5.31f, 1.56f, 8.44f)) < 0.1f)
            {
                _needToDebug = true;
            }
            else
                _needToDebug = false;

            if (voxel.Neighbors == null) return;

            for (var i = 0; i < voxel.Neighbors.Length; i++)
            {
                var neighbor = voxel.Neighbors[i];
                if (neighbor != null)
                {
                    if (neighbor.Children != null) continue;
                    if (neighbor.IsCollided == false)
                        ConnectNodes(voxel, neighbor, nodes);
                }
                else
                {
                    var foundNeighbor = FindAdjoiningNeighbor(voxel, i, voxel);
                    if (foundNeighbor == null) continue;
                    if (Vector3.Distance(foundNeighbor.Position, new Vector3(-7.5f, 7.5f, 7.5f)) < 0.1f)
                    {
                        DebugLog(voxel.AsChildIndex.ToString());
                    }

                    ConnectNodes(voxel, foundNeighbor, nodes);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="childVoxel">Дочерний воксель.
        /// В этой переменной в процессе рекурсии будут оказываться родители childVoxel</param>
        /// <param name="searchDirection">Направление поиска</param>
        /// <param name="entryVoxel">Здесь хранится самый начальный воксель, из-за которого начался поиск</param>
        /// <returns></returns>
        private static Voxel FindAdjoiningNeighbor(Voxel childVoxel, int searchDirection, Voxel entryVoxel = null)
        {
            // TODO: переделать на while

            var parent = childVoxel.Parent;
            if (parent == null) return null;

            var parentNeighbor = parent.Neighbors?[searchDirection];
            if (parentNeighbor == null) return FindAdjoiningNeighbor(parent, searchDirection, entryVoxel);

            if (parentNeighbor.Children == null)
                return parentNeighbor.IsCollided ? null : parentNeighbor;

            return FindAdjoiningNeighborChild(parentNeighbor, entryVoxel, searchDirection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentVoxel">Найденный родитель, вглубь которого будем двигаться</param>
        /// <param name="childVoxel">Воксель, инициировавший поиск</param>
        /// <param name="searchDirection">Направление поиска</param>
        /// <returns></returns>
        private static Voxel FindAdjoiningNeighborChild(Voxel parentVoxel, Voxel childVoxel, int searchDirection)
        {
            if (parentVoxel == null) return null;

            var children = parentVoxel.Children;
            if (children == null)
            {
                // детей нет, углубились максимально сильно
                return parentVoxel.IsCollided ? null : parentVoxel;
            }

            if (parentVoxel.LOD >= childVoxel.LOD)
            {
                // искомый воксель оказался более детализированным
                return null;
            }

            var childrenLOD = parentVoxel.LOD + 1;
            var asChildIndex = childVoxel.LOD == childrenLOD
                ? childVoxel.AsChildIndex
                : childVoxel.GetParentByLOD(childrenLOD).AsChildIndex;

            if (asChildIndex < 0)
                Debug.LogError("Voxel is not child! " + childVoxel.Position);

            var closestChildVoxel = searchDirection switch
            {
                0 => asChildIndex switch
                {
                    0 => children[1],
                    2 => children[3],
                    4 => children[5],
                    6 => children[7],
                    _ => null
                },
                1 => asChildIndex switch
                {
                    1 => children[0],
                    3 => children[2],
                    5 => children[4],
                    7 => children[6],
                    _ => null
                },
                2 => asChildIndex switch
                {
                    0 => children[4],
                    1 => children[5],
                    2 => children[6],
                    3 => children[7],
                    _ => null
                },
                3 => asChildIndex switch
                {
                    4 => children[0],
                    5 => children[1],
                    6 => children[2],
                    7 => children[3],
                    _ => null
                },
                4 => asChildIndex switch
                {
                    0 => children[2],
                    1 => children[3],
                    4 => children[6],
                    5 => children[7],
                    _ => null
                },
                5 => asChildIndex switch
                {
                    2 => children[0],
                    3 => children[1],
                    6 => children[4],
                    7 => children[5],
                    _ => null
                },
                _ => throw new ArgumentOutOfRangeException(nameof(searchDirection), searchDirection, null)
            };

            return FindAdjoiningNeighborChild(closestChildVoxel, childVoxel, searchDirection);
        }

        private static void ConnectNodes(Voxel from, Voxel to, Dictionary<Voxel, Node> nodes)
        {
            if (from.LOD < to.LOD)
            {
                Debug.LogError(
                    "Алгоритм работает не верно, т.к. пытается присоединить воксель с малым LOD к вокселю с большим LOD." +
                    $"From: {from.LOD} to: {to.LOD}. From: {from.Position} to: {to.Position}");
                return;
            }

            if (nodes.TryGetValue(from, out var fromNode) == false)
            {
                fromNode = new Node(from, from.LOD);
                nodes[from] = fromNode;
            }

            if (nodes.TryGetValue(to, out var toNode) == false)
            {
                toNode = new Node(to, to.LOD);
                nodes[to] = toNode;
            }

            fromNode.ConnectNode(toNode);
        }

        private static bool _needToDebug = false;

        private static void DebugLog(string info)
        {
            if (_needToDebug == false) return;
            Debug.Log(info);
        }
    }
}