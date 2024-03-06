#if UNITY_EDITOR
using UnityEngine;

public class SimpleGizmo : MonoBehaviour
{
    [SerializeField] private Color color = Color.red;
    [SerializeField] private float radius = 1;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
#endif