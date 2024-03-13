using UnityEngine;

public class SimpleGizmo : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private Color color = Color.red;
    [SerializeField] private float radius = 1;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

#endif
}