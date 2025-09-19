using System.Collections.Generic;
using System.Linq;
using Rusleo.VoxelPathfinding.Runtime.Core.Graph;
using Rusleo.VoxelPathfinding.Runtime.Core.Grid;
using Rusleo.VoxelPathfinding.Runtime.Core.Path;
using Rusleo.VoxelPathfinding.Runtime.Utils;
using UnityEngine;
using VoxelPathfinding.Scripts.Core.Graph;

namespace Rusleo.VoxelPathfinding.Runtime.Pathfinding
{
    public static class PathfindingUtility
    {
        public static List<Node> FindPath(this BaseGraph graph, Vector3 startPos, Vector3 endPos)
        {
            var startNode = graph.FindClosestNode(startPos);
            var endNode = graph.FindClosestNode(endPos);

            return FindPath(startNode, endNode);
        }

        public static List<Node> FindPath(this BaseGraph graph, BaseGrid grid, Vector3 startPos, Vector3 endPos)
        {
            var startVoxel = VoxelUtils.GetOverlappingVoxel(grid, startPos);
            var endVoxel = VoxelUtils.GetOverlappingVoxel(grid, endPos);

            if (startVoxel == null || endVoxel == null)
                return null;

            var startNode = graph.FindNodeByVoxel(startVoxel);
            var endNode = graph.FindNodeByVoxel(endVoxel);
            return FindPath(startNode, endNode);
        }

        public static List<Node> FindPath(Node startNode, Node targetNode)
        {
            if (startNode == null || targetNode == null)
                return null;

            // Открытый список: узлы, которые нужно проверить
            var openSet = new List<Node> { startNode };
            // Закрытый список: узлы, которые уже проверены
            var closedSet = new HashSet<Node>();

            // Инициализация начального узла
            startNode.GCost = 0;
            startNode.HCost = Heuristic(startNode, targetNode);
            startNode.Parent = null;

            while (openSet.Count > 0)
            {
                // Сортируем по FCost и выбираем узел с минимальной стоимостью
                openSet.Sort((a, b) => a.FCost().CompareTo(b.FCost()));
                var currentNode = openSet[0];

                // Если нашли путь к цели – восстанавливаем путь
                if (currentNode == targetNode)
                    return RetracePath(startNode, targetNode);

                // Переносим текущий узел в закрытый список
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                // Обрабатываем соседей
                foreach (var neighbor in currentNode.Connections)
                {
                    if (closedSet.Contains(neighbor))
                        continue;

                    // Вычисляем стоимость пути через текущий узел
                    var tentativeGCost =
                        currentNode.GCost + Vector3.Distance(currentNode.Position, neighbor.Position);

                    if (!openSet.Contains(neighbor) || tentativeGCost < neighbor.GCost)
                    {
                        neighbor.GCost = tentativeGCost;
                        neighbor.HCost = Heuristic(neighbor, targetNode);
                        neighbor.Parent = currentNode;

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            Debug.LogWarning("Can't find path. " + startNode + ", " + targetNode);
            return null;
        }

        /// <summary>
        /// Функция эвристики: используем Евклидово расстояние
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static float Heuristic(Node a, Node b)
        {
            return (a.Position - b.Position).sqrMagnitude;
        }

        /// <summary>
        /// Восстановление пути от конечного узла к начальному
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="endNode"></param>
        /// <returns></returns>
        private static List<Node> RetracePath(Node startNode, Node endNode)
        {
            var path = new List<Node>();
            var currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            path.Add(startNode);
            path.Reverse(); // Разворачиваем путь от старта к цели
            return path;
        }

        public static void WarpPath(Path path, out List<Vector3> result)
        {
            if (path.PathNodes.Count == 0)
                result = new List<Vector3> { path.StartPosition, path.EndPosition };

            result = new List<Vector3>(path.PathNodes.Count) { path.StartPosition };
            if (TryGetLineBoxIntersection2(path.StartPosition, path.PathNodes[0].Position, path.PathNodes[0].Position,
                    path.PathNodes[0].Voxel.Size, out var intersection))
            {
                result.Add(intersection);
            }
            // else Debug.Log("Failed Start");

            var isIntersectionFailed = false;
            for (var i = 0; i < path.PathNodes.Count - 1; i++)
            {
                var current = path.PathNodes[i];
                var next = path.PathNodes[i + 1];
                if (TryGetLineBoxIntersection2(current.Position, next.Position, current.Position, current.Voxel.Size,
                        out intersection))
                {
                    result.Add(intersection);
                    isIntersectionFailed = false;
                }
                else
                {
                    // Debug.Log("Failed " + i);
                    if (isIntersectionFailed == false)
                    {
                        result.Add(current.Position);
                    }

                    result.Add(next.Position);
                    isIntersectionFailed = true;
                }
            }

            if (TryGetLineBoxIntersection2(path.PathNodes[^1].Position, path.EndPosition, path.PathNodes[^1].Position,
                    path.PathNodes[^1].Voxel.Size, out intersection))
            {
                result.Add(intersection);
            }
            // else Debug.Log("Failed End");

            result.Add(path.EndPosition);
        }

        public static void DistanceMergePath(List<Vector3> path, float mergeDistance = 0.05f)
        {
            var start = path.Aggregate("", (current, vector3) => current + (vector3 + " "));
            Debug.Log(start);
            for (var i = 0; i < path.Count - 1; i++)
            {
                if (Vector3.Distance(path[i], path[i + 1]) > mergeDistance) continue;
                path.RemoveAt(i + 1);
                i--;
                Debug.Log("Remove");
            }

            start = path.Aggregate("", (current, vector3) => current + (vector3 + " "));
            Debug.Log(start);
        }

        /// <summary>
        /// Пытается найти точку пересечения отрезка с осесимметричным прямоугольным параллелепипедом.
        /// Если пересечение найдено, возвращает true, а точка пересечения (ближайшая к lineStart) передается через out-параметр.
        /// </summary>
        /// <param name="lineStart">Начало отрезка</param>
        /// <param name="lineEnd">Конец отрезка</param>
        /// <param name="boxCenter">Центр параллелепипеда</param>
        /// <param name="boxSize">Размеры параллелепипеда по осям x, y, z</param>
        /// <param name="intersection">Найденная точка пересечения</param>
        /// <returns>true, если пересечение найдено, иначе false</returns>
        public static bool TryGetLineBoxIntersection(Vector3 lineStart, Vector3 lineEnd, Vector3 boxCenter,
            Vector3 boxSize, out Vector3 intersection)
        {
            intersection = Vector3.zero;
            var direction = lineEnd - lineStart;
            var tMin = 0f;
            var tMax = 1f;

            // Вычисляем половинные размеры параллелепипеда.
            var halfSize = boxSize * 0.5f;
            var boxMin = boxCenter - halfSize;
            var boxMax = boxCenter + halfSize;

            // Используем метод "slab" по осям x, y, z.
            for (var i = 0; i < 3; i++)
            {
                // Если направление вдоль оси почти нулевое, отрезок параллелен соответствующим плоскостям.
                if (Mathf.Abs(direction[i]) < 1e-6f)
                {
                    // Если начальная точка отрезка вне диапазона по данной оси, пересечения нет.
                    if (lineStart[i] < boxMin[i] || lineStart[i] > boxMax[i])
                        return false;
                }
                else
                {
                    var invD = 1f / direction[i];
                    var t1 = (boxMin[i] - lineStart[i]) * invD;
                    var t2 = (boxMax[i] - lineStart[i]) * invD;

                    // Гарантируем, что t1 <= t2.
                    if (t1 > t2)
                    {
                        (t1, t2) = (t2, t1);
                    }

                    tMin = Mathf.Max(tMin, t1);
                    tMax = Mathf.Min(tMax, t2);

                    // Если интервал пуст, пересечения нет.
                    if (tMin > tMax)
                        return false;
                }
            }

            // Если найдено пересечение, выбираем точку с минимальным t (ближайшую к lineStart)
            var tIntersection = -1f;
            if (tMin is >= 0f and <= 1f)
            {
                tIntersection = tMin;
            }
            else if (tMax is >= 0f and <= 1f)
            {
                tIntersection = tMax;
            }
            else
            {
                return false;
            }

            intersection = lineStart + tIntersection * direction;
            return true;
        }

        public static bool TryGetLineBoxIntersection2(Vector3 lineStart, Vector3 lineEnd, Vector3 boxCenter,
            Vector3 boxSize, out Vector3 intersection)
        {
            intersection = Vector3.zero;
            Vector3 d = lineEnd - lineStart;
            Vector3 halfSize = boxSize * 0.5f;
            Vector3 min = boxCenter - halfSize;
            Vector3 max = boxCenter + halfSize;
            float closestT = float.PositiveInfinity;
            bool hit = false;
            const float epsilon = 1e-6f;

            // Проходим по осям: 0 - x, 1 - y, 2 - z
            for (int axis = 0; axis < 3; axis++)
            {
                // Получаем значения начала, направления и границ для текущей оси
                float start = (axis == 0 ? lineStart.x : (axis == 1 ? lineStart.y : lineStart.z));
                float dir = (axis == 0 ? d.x : (axis == 1 ? d.y : d.z));
                float minPlane = (axis == 0 ? min.x : (axis == 1 ? min.y : min.z));
                float maxPlane = (axis == 0 ? max.x : (axis == 1 ? max.y : max.z));

                // Если направление вдоль оси не нулевое
                if (Mathf.Abs(dir) > epsilon)
                {
                    // Проверяем пересечение с плоскостью minPlane
                    float t = (minPlane - start) / dir;
                    if (t > epsilon && t <= 1f)
                    {
                        Vector3 pt = lineStart + t * d;
                        if (IsWithinFace(pt, axis, min, max, epsilon) && t < closestT)
                        {
                            closestT = t;
                            intersection = pt;
                            hit = true;
                        }
                    }

                    // Проверяем пересечение с плоскостью maxPlane
                    t = (maxPlane - start) / dir;
                    if (t > epsilon && t <= 1f)
                    {
                        Vector3 pt = lineStart + t * d;
                        if (IsWithinFace(pt, axis, min, max, epsilon) && t < closestT)
                        {
                            closestT = t;
                            intersection = pt;
                            hit = true;
                        }
                    }
                }
            }

            return hit;
        }

        // Метод-помощник для проверки, что точка pt попадает в границы грани куба (то есть по двум оставшимся осям)
        private static bool IsWithinFace(Vector3 pt, int fixedAxis, Vector3 min, Vector3 max, float epsilon)
        {
            return fixedAxis switch
            {
                0 => // x фиксирован, проверяем y и z
                    pt.y >= min.y - epsilon && pt.y <= max.y + epsilon && pt.z >= min.z - epsilon &&
                    pt.z <= max.z + epsilon,
                1 => // y фиксирован, проверяем x и z
                    pt.x >= min.x - epsilon && pt.x <= max.x + epsilon && pt.z >= min.z - epsilon &&
                    pt.z <= max.z + epsilon,
                2 => // z фиксирован, проверяем x и y
                    pt.x >= min.x - epsilon && pt.x <= max.x + epsilon && pt.y >= min.y - epsilon &&
                    pt.y <= max.y + epsilon,
                _ => false
            };
        }
    }
}