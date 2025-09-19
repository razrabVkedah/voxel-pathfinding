using System.Collections.Generic;
using Rusleo.VoxelPathfinding.Runtime.Core.Path;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rusleo.VoxelPathfinding.Runtime.Pathfinding
{
    public class BeeMover : MonoBehaviour
    {
        [Header("Pathfinding")] [SerializeField]
        private PathBuilder pathBuilder;

        [SerializeField] private Transform target;

        [Header("Movement")] [SerializeField] private Rigidbody rb;
        [SerializeField] private float speed;
        [Range(0f, 1f)] [SerializeField] private float bezierLerp;
        [SerializeField] private float maxVelocityDelta = 1f;

        [Header("Shake")] [SerializeField] private float shakeFrequency = 1;
        [SerializeField] private float shakeAmplitude = 1;
        private float _shakeOffsetX;
        private float _shakeOffsetY;
        private float _shakeOffsetZ;

        [Header("Debug")] [SerializeField] private bool drawPath = true;

        private Path _path;
        private List<Vector3> _warpedPath;

        public Path GetPath() => _path;
        public List<Vector3> GetWarpedPath() => _warpedPath;

        private void Start()
        {
            _shakeOffsetX = Random.Range(0f, 100f);
            _shakeOffsetY = Random.Range(0f, 100f);
            _shakeOffsetZ = Random.Range(0f, 100f);
        }

        private void FixedUpdate()
        {
            _path = pathBuilder.BuildPath(transform.position, target.position);
            if (_path == null) return;
            PathfindingUtility.WarpPath(_path, out _warpedPath);
            //PathfindingUtility.DistanceMergePath(_warpedPath);

            Move2();
        }

        private void Move2()
        {
            if (_warpedPath.Count < 3)
            {
                Debug.LogWarning("Необходимо задать 3 точки кривой Безье в массиве warpedPath.");
                return;
            }

            var mainPosition = Vector3.Lerp(
                Vector3.Lerp(_warpedPath[0], _warpedPath[1], bezierLerp),
                Vector3.Lerp(_warpedPath[1], _warpedPath[2], bezierLerp),
                bezierLerp);

            // 2. Определение базового направления движения к главной точке
            var baseDir = (mainPosition - transform.position).normalized;

            var shakeOffset = GetShakeOffset();

            // 4. Расчёт желаемой скорости: движение по траектории + независимое шатание
            var desiredVelocity = (baseDir * speed) + shakeOffset;

            // 5. Корректировка текущей скорости Rigidbody для достижения желаемой скорости
            var velocityDelta = desiredVelocity - rb.linearVelocity;

            velocityDelta = Vector3.ClampMagnitude(velocityDelta, maxVelocityDelta);

            // Применяем силу для коррекции скорости
            rb.AddForce(velocityDelta, ForceMode.VelocityChange);
        }

        private void Move1()
        {
            if (_warpedPath.Count < 3)
            {
                var dir = _warpedPath[1] - transform.position;
                if (dir.magnitude <= speed * Time.fixedDeltaTime)
                    rb.linearVelocity = Vector3.zero;
                else
                {
                    rb.linearVelocity = dir.normalized * speed;
                    transform.forward = dir;
                }
            }
            else
            {
                var p = Vector3.Lerp(
                    Vector3.Lerp(_warpedPath[0], _warpedPath[1], bezierLerp),
                    Vector3.Lerp(_warpedPath[1], _warpedPath[2], bezierLerp),
                    bezierLerp);
                var dir = p - transform.position;
                rb.linearVelocity = dir.normalized * speed;
                transform.forward = dir;
            }
        }

        private Vector3 GetShakeOffset()
        {
            var shakeX = (Mathf.PerlinNoise(Time.fixedTime * shakeFrequency, _shakeOffsetX) - 0.5f) * shakeAmplitude;
            var shakeY = (Mathf.PerlinNoise(Time.fixedTime * shakeFrequency, _shakeOffsetY) - 0.5f) * shakeAmplitude;
            var shakeZ = (Mathf.PerlinNoise(Time.fixedTime * shakeFrequency, _shakeOffsetZ) - 0.5f) * shakeAmplitude;
            return new Vector3(shakeX, shakeY, 0f);
        }

        private void OnDrawGizmos()
        {
            if (drawPath == false) return;
            if (_warpedPath == null) return;

            for (var i = 0; i < _warpedPath.Count - 1; i++)
            {
                Gizmos.DrawLine(_warpedPath[i], _warpedPath[i + 1]);
            }
        }
    }
}