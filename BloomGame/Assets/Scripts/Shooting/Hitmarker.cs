using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Hitmarker : MonoBehaviour
{
    [HideInInspector] public Image image;
    public Player player;
    public Transform target;
    public Canvas canvas; // Reference to the Canvas
    public float smoothAimDelay = 5f;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void FixedUpdate()
    {
        //Vector2 screenPoint = Camera.main.WorldToScreenPoint(target.position);
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPoint, null, out Vector2 aimTargetPosition)

        Vector3 mousePosition = Mouse.current.position.ReadValue();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), mousePosition, null, out Vector2 screenPosition);
        image.rectTransform.anchoredPosition = Vector3.Lerp(image.rectTransform.anchoredPosition, screenPosition, Time.deltaTime * smoothAimDelay);
    }
}
