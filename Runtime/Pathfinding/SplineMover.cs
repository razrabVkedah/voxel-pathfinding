using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rusleo.VoxelPathfinding.Runtime.Pathfinding
{
    public class SplineMover : MonoBehaviour
    {
        [SerializeField] private PathBuilder pathBuilder;
        [SerializeField] private List<Vector3> controlPoints;
        [SerializeField] private float speed = 5f;

        private float _totalDistance; // Общее пройденное расстояние

        private void Start()
        {
            var path = pathBuilder.GetPath();
            controlPoints = path.PathNodes.Select(n => n.Position).ToList();
            controlPoints.Insert(0, path.StartPosition);
            controlPoints.Add(path.EndPosition);

            if (controlPoints.Count < 4)
            {
                Debug.LogError("Для Catmull-Rom сплайна нужно минимум 4 точки!");
            }
        }

        private void Update()
        {
            _totalDistance += speed * Time.deltaTime; // Увеличиваем пройденное расстояние
            transform.position = GetSmoothPosition(_totalDistance); // Получаем позицию на сплайне
            var dir = GetSmoothPosition(_totalDistance + 0.1f) - transform.position;
            if (dir.magnitude > 0.05f) transform.forward = dir.normalized;
        }

        private Vector3 GetSmoothPosition(float distance)
        {
            float accumulatedDistance = 0f;

            for (int i = 0; i < controlPoints.Count - 3; i++)
            {
                float segmentLength = EstimateSegmentLength(controlPoints[i], controlPoints[i + 1],
                    controlPoints[i + 2], controlPoints[i + 3]);

                if (distance <= accumulatedDistance + segmentLength)
                {
                    float t = (distance - accumulatedDistance) / segmentLength; // Нормализуем t

                    // Вычисляем тангенсы (направление движения) на основе локальной длины отрезков
                    Vector3 tangentA = (controlPoints[i + 2] - controlPoints[i]) / 2f;
                    Vector3 tangentB = (controlPoints[i + 3] - controlPoints[i + 1]) / 2f;

                    // Используем сглаженные тангенсы для плавного движения
                    return SplinesUtility.GetPoint(controlPoints[i + 1], controlPoints[i + 2], tangentA, tangentB, t);
                }

                accumulatedDistance += segmentLength;
            }

            return controlPoints[^2]; // Конечная точка
        }

        private float EstimateSegmentLength(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int resolution = 10)
        {
            float length = 0f;
            Vector3 previousPoint = p1;

            for (int i = 1; i <= resolution; i++)
            {
                float t = i / (float)resolution;
                Vector3 currentPoint = SplinesUtility.GetPoint(p1, p2, (p2 - p0) / 2f, (p3 - p1) / 2f, t);
                length += Vector3.Distance(previousPoint, currentPoint);
                previousPoint = currentPoint;
            }

            return length;
        }
    }
}