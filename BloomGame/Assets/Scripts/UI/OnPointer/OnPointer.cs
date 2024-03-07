using SmoothShakePro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnPointer : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler
{
    public SmoothShakeStarter shake;

    public UnityEvent playEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        playEvent.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(shake) shake.StartShake();

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (shake) shake.StopShake();
    }
}
