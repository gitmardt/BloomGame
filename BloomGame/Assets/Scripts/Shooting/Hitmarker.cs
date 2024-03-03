using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class Hitmarker : MonoBehaviour
{
    RawImage rawImage;
    public Transform target;
    public Canvas canvas; // Reference to the Canvas

    private void Awake()
    {
        rawImage = GetComponent<RawImage>();
    }

    private void FixedUpdate()
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(target.position);

        // Convert screen point to canvas local point
        Vector2 canvasLocalPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPoint, null, out canvasLocalPoint);

        // Set the position of the rawImage
        rawImage.rectTransform.anchoredPosition = canvasLocalPoint;
    }
}
