using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextReferences : MonoBehaviour
{
    public TypewriterByCharacter[] typewriters;

    private void OnEnable()
    {
        for (int i = 0; i < typewriters.Length; i++)
        {
            typewriters[i].StartShowingText();
        }
    }

    public void DisappearText()
    {
        for (int i = 0; i < typewriters.Length; i++)
        {
            typewriters[i].StartDisappearingText();
        }
    }
}
