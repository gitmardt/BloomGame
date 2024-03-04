using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class Hitmarker : MonoBehaviour
{
    RawImage rawImage;
    public ThirdPersonMovement player;
    public Transform target;
    public Canvas canvas; // Reference to the Canvas
    public float smoothAimDelay = 5f;

    private void Awake()
    {
        rawImage = GetComponent<RawImage>();
    }

    private void FixedUpdate()
    {
        //Vector2 screenPoint = Camera.main.WorldToScreenPoint(target.position);
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPoint, null, out Vector2 aimTargetPosition);

        Vector3 mousePosition = Mouse.current.position.ReadValue();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), mousePosition, null, out Vector2 screenPosition);
        rawImage.rectTransform.anchoredPosition = Vector3.Lerp(rawImage.rectTransform.anchoredPosition, screenPosition, Time.deltaTime * smoothAimDelay);
    }
}
