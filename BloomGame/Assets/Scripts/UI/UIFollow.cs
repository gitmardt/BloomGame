using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class UIFollow : MonoBehaviour
{
    public Canvas canvas;
    public Camera cam;
    public Vector3 offset;
    public GameObject player;

    void Update()
    {
        if (cam == null || canvas == null) return;

        UpdatePosition();
    }

    [Button]
    void UpdatePosition()
    {
        Vector3 playerScreenPos = cam.WorldToScreenPoint(player.transform.position);
        Vector3 playerViewportPos = cam.ScreenToViewportPoint(playerScreenPos);
        Vector3 worldPos = cam.ViewportToWorldPoint(new Vector3(playerViewportPos.x, playerViewportPos.y, canvas.planeDistance));
        transform.position = worldPos + offset;
    }
}