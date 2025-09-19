using UnityEngine;

namespace Rusleo.VoxelPathfinding.Editor.GizmosDebug
{
    public abstract class BaseDrawer : MonoBehaviour
    {
        [SerializeField] private bool draw;
        [SerializeField] private bool drawIfNotSelected;

        private void OnDrawGizmos()
        {
            if (draw == false) return;
            if (drawIfNotSelected == false) return;
        
            DrawGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            if (draw == false) return;
            if (drawIfNotSelected) return;
        
            DrawGizmos();
        }

        protected abstract void DrawGizmos();
    }
}