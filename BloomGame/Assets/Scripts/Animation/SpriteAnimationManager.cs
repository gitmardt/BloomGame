using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimationManager : MonoBehaviour
{
    public Coroutine activeCoroutine;

    public void PlaySpriteAnimation(SpriteAnimation SpriteAnim, Image image)
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }

        activeCoroutine = StartCoroutine(PlaySpriteAnimRoutine(SpriteAnim, image));
    }

    private IEnumerator PlaySpriteAnimRoutine(SpriteAnimation SpriteAnim, Image image)
    {
        int length = SpriteAnim.sprites.Length;

        for (int i = 0; i < length; i++)
        {
            image.sprite = SpriteAnim.sprites[i];

            yield return new WaitForSeconds(1 / SpriteAnim.animationSpeed);
        }
    }
}
