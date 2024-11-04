using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ScreenshotCapturer
{
    private Camera cam;
    private int width;
    private int height;

    public enum Page
    {
        A4
    }
    public enum ImageFormat
    {
        PNG,
        JPG
    }
    public ScreenshotCapturer(Camera camera, int width, int height)
    {
        cam = camera ?? throw new ArgumentNullException(nameof(camera));
        this.width = width;
        this.height = height;
    }
    public ScreenshotCapturer(Camera camera, Page page, int dpi = 300)
    {
        cam = camera ?? throw new ArgumentNullException(nameof(camera));

        if (page == Page.A4)
        {
            width = (int)(8.27 * dpi);  // A4 ≥ ∫Ò («»ºø)
            height = (int)(11.69 * dpi); // A4 ≥Ù¿Ã («»ºø)
        }
    }

    public Texture2D Capture()
    {
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        cam.targetTexture = renderTexture;
        cam.Render();

        RenderTexture.active = renderTexture;
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        cam.targetTexture = null;
        RenderTexture.active = null;
        UnityEngine.Object.Destroy(renderTexture);

        return screenshot;
    }

    public async UniTask<Texture2D> CaptureAsync()
    {
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        cam.targetTexture = renderTexture;
        cam.Render();

        RenderTexture.active = renderTexture;
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);

        await UniTask.SwitchToMainThread();
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        cam.targetTexture = null;
        RenderTexture.active = null;
        UnityEngine.Object.Destroy(renderTexture);

        return screenshot;
    }
    public async UniTask Save(Texture2D screenshot, string filePath, ImageFormat format)
    {
        if (screenshot == null)
        {
            throw new ArgumentNullException(nameof(screenshot));
        }

        byte[] imageData;

        if (format == ImageFormat.PNG)
        {
            imageData = screenshot.EncodeToPNG();
            filePath = Path.ChangeExtension(filePath, ".png");
        }
        else if (format == ImageFormat.JPG)
        {
            imageData = screenshot.EncodeToJPG();
            filePath = Path.ChangeExtension(filePath, ".jpg");
        }
        else
        {
            throw new ArgumentException("Unsupported image format", nameof(format));
        }

        await UniTask.SwitchToThreadPool();
        await File.WriteAllBytesAsync(filePath, imageData);
    }
}