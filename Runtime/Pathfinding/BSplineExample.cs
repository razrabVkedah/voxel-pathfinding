using UnityEngine;
using System.Collections.Generic;
using Rusleo.VoxelPathfinding.Runtime.Core.Path;

namespace Rusleo.VoxelPathfinding.Runtime.Pathfinding
{
    public class BSplineExample : MonoBehaviour
    {
        public List<Transform> controlPoints;
        public float speed = 1f;
        private BSpline bSpline;
        private float t = 0f;

        void Start()
        {
            // Инициализация B-spline с контрольными точками и узлами
            List<Vector3> points = new List<Vector3>();
            foreach (var controlPoint in controlPoints)
            {
                points.Add(controlPoint.position);
            }

            // Узлы для B-spline (например, равномерно распределенные)
            List<float> knots = new List<float>();
            for (int i = 0; i < points.Count + 2; i++)
            {
                knots.Add(i);
            }

            bSpline = new BSpline(points, knots, 2); // 2 - степень кривой (квадратичная)
        }

        void Update()
        {
            // Вычисление нового положения объекта на кривой
            t += Time.deltaTime * speed;

            // Если t больше 1, перезапускаем движение
            if (t > 1f)
            {
                t = 0f;
            }

            // Получаем точку на кривой
            Vector3 position = bSpline.GetPoint(t);
            transform.position = position;
        }

        // Отображение кривой через Gizmos
        private void OnDrawGizmos()
        {
            if (bSpline == null || controlPoints.Count < 2)
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
            foreach (var controlPoint in controlPoints)
            {
                Gizmos.DrawSphere(controlPoint.position, 0.1f);
            }
        }
    }
}