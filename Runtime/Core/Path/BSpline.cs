using System.Collections.Generic;
using UnityEngine;

namespace Rusleo.VoxelPathfinding.Runtime.Core.Path
{
    public class BSpline
    {
        private List<Vector3> controlPoints;
        private List<float> knots;
        private int degree;

        // Конструктор B-spline
        public BSpline(List<Vector3> controlPoints, List<float> knots, int degree)
        {
            this.controlPoints = controlPoints;
            this.knots = knots;
            this.degree = degree;

            // Убедимся, что узлов достаточно для степени кривой и количества контрольных точек
            if (knots.Count != controlPoints.Count + degree + 1)
            {
                Debug.LogError("Неверное количество узлов для кривой B-spline.");
            }
        }

        // Вычисление B-spline на основе контрольных точек и параметра t
        public Vector3 GetPoint(float t)
        {
            int n = controlPoints.Count;
            Vector3 result = Vector3.zero;

            // Считаем базисные функции
            for (int i = 0; i < n; i++)
            {
                float N = BasisFunction(i, degree, t);
                result += N * controlPoints[i];
            }

            return result;
        }

        // Базисная функция для B-spline
        private float BasisFunction(int i, int p, float t)
        {
            if (p == 0)
            {
                // Базисная функция 0-го порядка (линейная)
                return (t >= knots[i] && t < knots[i + 1]) ? 1.0f : 0.0f;
            }
            else
            {
                // Убедимся, что индексы не выходят за границы массива
                float denom1 = knots[i + p] - knots[i];
                float denom2 = knots[i + p + 1] - knots[i + 1];

                // Проверим, что деноминаторы не равны нулю
                if (denom1 == 0 || denom2 == 0) return 0f;

                // Рекурсивное вычисление базисной функции для большего порядка
                float coeff1 = (t - knots[i]) / denom1 * BasisFunction(i, p - 1, t);
                float coeff2 = (knots[i + p + 1] - t) / denom2 * BasisFunction(i + 1, p - 1, t);

                return coeff1 + coeff2;
            }
        }
    }
}