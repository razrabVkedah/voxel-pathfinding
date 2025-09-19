using System;
using System.Collections.Generic;
using Rusleo.VoxelPathfinding.Runtime.Core.Path;
using Rusleo.VoxelPathfinding.Runtime.Pathfinding;
using UnityEngine;
using Gizmos = UnityEngine.Gizmos;

namespace Rusleo.VoxelPathfinding.Editor.GizmosDebug
{
    public class PathDrawer : BaseDrawer
    {
        [SerializeField] private PathBuilder pathBuilder;
        [SerializeField] private Color pathColor;
        [SerializeField] private PathTypeToDraw pathTypeToDraw = PathTypeToDraw.Default;

        protected override void DrawGizmos()
        {
            if (!pathBuilder) return;
            var path = pathBuilder.GetPath();
            var warpedPath = pathBuilder.GetWarpedPath();
            Gizmos.color = pathColor;

            switch (pathTypeToDraw)
            {
                case PathTypeToDraw.None:
                    break;
                case PathTypeToDraw.Default:
                    if (path?.PathNodes == null) return;
                    DrawDefaultPath(path);
                    break;
                case PathTypeToDraw.Warped:
                    if (warpedPath == null) return;
                    DrawWarpedPath(warpedPath);
                    break;
                case PathTypeToDraw.Spline:
                    if (path?.PathNodes == null) return;
                    DrawBSpline(path);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void DrawDefaultPath(Path path)
        {
            Gizmos.DrawLine(path.StartPosition, path.PathNodes[0].Position);

            for (var i = 0; i < path.PathNodes.Count - 1; i++)
            {
                Gizmos.DrawLine(path.PathNodes[i].Position, path.PathNodes[i + 1].Position);
            }

            Gizmos.DrawLine(path.EndPosition, path.PathNodes[^1].Position);
        }

        private static void DrawWarpedPath(List<Vector3> path)
        {
            for (var i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]);
            }
        }

        private static void DrawBSpline(Path path)
        {
            // Инициализация B-spline с контрольными точками и узлами
            List<Vector3> points = new List<Vector3> { path.StartPosition };
            foreach (var controlPoint in path.PathNodes)
            {
                points.Add(controlPoint.Position);
            }

            points.Add(path.EndPosition);

            // Узлы для B-spline (например, равномерно распределенные)
            List<float> knots = new List<float>();
            for (int i = 0; i < points.Count + 3; i++)
            {
                knots.Add(i);
            }

            var bSpline = new BSpline(points, knots, 2); // 2 - степень кривой (квадратичная)

            if (path.PathNodes.Count < 1)
                return;

            // Количество точек для отображения кривой
            int numberOfPoints = 100;

            // Плавно рисуем кривую
            Vector3 previousPoint = bSpline.GetPoint(0f);
            Gizmos.color = Color.red;

            for (int i = 1; i <= numberOfPoints; i++)
            {
                float t = i / (float)numberOfPoints;
                Vector3 currentPoint = bSpline.GetPoint(t);
                Gizmos.DrawLine(previousPoint, currentPoint);
                previousPoint = currentPoint;
            }

            // Отображаем контрольные точки для визуализации
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(path.StartPosition, 0.1f);
            foreach (var controlPoint in path.PathNodes)
            {
                Gizmos.DrawSphere(controlPoint.Position, 0.1f);
            }

            Gizmos.DrawSphere(path.EndPosition, 0.1f);
        }
    }

    public enum PathTypeToDraw
    {
        None,
        Default,
        Warped,
        Spline
    }
}