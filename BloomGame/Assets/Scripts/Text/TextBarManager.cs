using SmoothShakePro;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class TextInfo
{
    public string text;
    public float waitDuration = 5;
}

[System.Serializable]
public class DialogueInfo
{
    public string name;
    public TextInfo[] texts;
}

public class TextBarManager : MonoBehaviour
{
    public static TextBarManager instance;

    public AudioSource walkieAudio;
    public DialogueInfo[] dialogues;
    public TextMeshPro textSpot;
    public SmoothShakeStarter textShake;

    private void Awake()
    {
        instance = this;
    }

    public void StartDialogue(string name) => StartCoroutine(PlayDialogue(name)); 

    public IEnumerator PlayDialogue(string name)
    {
        for (int i = 0; i < dialogues.Length; i++)
        {
            if(name == dialogues[i].name)
            {
                textShake.StartShake();
                walkieAudio.Play();
                for (int u = 0; u < dialogues[i].texts.Length; u++)
                {
                    textSpot.text = dialogues[i].texts[u].text;
                    yield return new WaitForSeconds(dialogues[i].texts[u].waitDuration);
                }
                walkieAudio.Play();
                textShake.StopShake();
                textSpot.text = "";
            }
        }
    }
}
