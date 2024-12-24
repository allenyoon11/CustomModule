using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotCapturerTester : MonoBehaviour
{
    public Camera cam;
    private ScreenshotCapturer capturer;

    private void Awake()
    {
        if(cam == null)
        {
            throw new Exception("cam is null");
        }
        //capturer = new ScreenshotCapturer(cam, );
    }
}
