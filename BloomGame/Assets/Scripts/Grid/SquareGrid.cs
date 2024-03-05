using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class SquareGrid : MonoBehaviour
{
    public int gridSize = 10;
    public int safeAreaSize = 5;
    public float cellSize = 1;
    public Vector2 randomOffset;
    public int minRandomPoints = 5;
    public int maxRandomPoints = 10;

    [Header("Gizmo options")]
    public Color mainGizmoColor = Color.red;
    public Color outerGizmoColor = Color.blue;
    public Color randomPickGizmoColor = Color.green;
    public Color safeAreaGizmoColor = Color.yellow;
    public float pointGizmoSphereRadius = 0.2f;
    public bool viewMainPoints, viewBorderPoints, viewRandomPoints, viewSafePoints;

    [HideInInspector] public List<Vector3> points = new();
    [HideInInspector] public List<Vector3> borderPoints = new();
    [HideInInspector] public List<Vector3> randomlyPickedPoints = new();
    [HideInInspector] public List<Vector3> safePoints = new();

    private bool CheckBorder(Vector3 point)
    {
        Vector3 normalizedPoint = new(point.x / cellSize, 0, point.z / cellSize);
        if (normalizedPoint.x == gridSize || normalizedPoint.z == gridSize || normalizedPoint.x == -gridSize || normalizedPoint.z == -gridSize) return true;
        else return false;
    }

    private bool CheckSafeArea(Vector3 point)
    {
        Vector3 normalizedPoint = new(point.x / cellSize, 0, point.z / cellSize);
        //if (normalizedPoint.x < safeAreaSize && normalizedPoint.z < safeAreaSize && normalizedPoint.x > -safeAreaSize && normalizedPoint.z > - safeAreaSize) return true;
        if (Vector3.Distance(Vector3.zero, normalizedPoint) < safeAreaSize) return true;
        else return false;
    }

    [Button]
    public void CreateGrid()
    {
        points.Clear();
        borderPoints.Clear();
        randomlyPickedPoints.Clear();
        safePoints.Clear();

        for (int x = -gridSize; x <= gridSize; x ++)
        {
            for (int y = -gridSize; y <= gridSize; y ++)
            {
                Vector3 point = new(x * cellSize, 0, y * cellSize);

                if(!CheckBorder(point))
                    point += new Vector3(Random.Range(-randomOffset.x, randomOffset.x), 0, Random.Range(-randomOffset.y, randomOffset.y));

                points.Add(point);
                if (CheckBorder(point))
                    borderPoints.Add(point);
                if (CheckSafeArea(point))
                    safePoints.Add(point);
            }
        }

        int randomPointAmount = Random.Range(minRandomPoints, maxRandomPoints);
        if (randomPointAmount > points.Count) Debug.LogWarning("Can't make grid because you are trying to get more random points than there are available points in the grid");

        for (int i = 0; i < randomPointAmount; i++)
        {
            int random = Random.Range(0, points.Count - 1);

            if (!CheckBorder(points[random]) && !CheckSafeArea(points[random])) 
                randomlyPickedPoints.Add(points[random]);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (viewMainPoints)
        {
            Gizmos.color = mainGizmoColor;
            for (int i = 0; i < points.Count; i++)
            {
                Gizmos.DrawSphere(points[i], pointGizmoSphereRadius);
            }
        }

        if (viewBorderPoints)
        {
            Gizmos.color = outerGizmoColor;
            for (int i = 0; i < borderPoints.Count; i++)
            {
                Gizmos.DrawSphere(borderPoints[i], pointGizmoSphereRadius + 0.1f);
            }
        }

        if (viewRandomPoints)
        {
            Gizmos.color = randomPickGizmoColor;
            for (int i = 0; i < randomlyPickedPoints.Count; i++)
            {
                Gizmos.DrawSphere(randomlyPickedPoints[i], pointGizmoSphereRadius + 0.1f);
            }
        }

        if (viewSafePoints)
        {
            Gizmos.color = safeAreaGizmoColor;
            for (int i = 0; i < safePoints.Count; i++)
            {
                Gizmos.DrawSphere(safePoints[i], pointGizmoSphereRadius + 0.1f);
            }
        }

    }
}
