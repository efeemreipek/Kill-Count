using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Screenshotter
{
    public static int index = 0;
    public static string name = "screenshot_";

    public static void Screenshot()
    {
        string fileName = name + index + ".png";
        ScreenCapture.CaptureScreenshot(fileName);
        index++;
    }
}
