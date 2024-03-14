using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class Screenshot : MonoBehaviour
{
    string fileName = "filename.png";

    [Button]
    public void TakeScreenshot()
    {
        ScreenCapture.CaptureScreenshot(fileName, 2);
    }
}
