using neuroears.allen.utils;
using System;
using System.IO;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class FramePlayerTester : MonoBehaviour
{
    public RawImage rawImage;
    public string filename = "test";
    public string extension = "mp4";

    private string dir;
    private VideoPlayer videoPlayer;
    private bool isPrepared = false;
    private string loadedFileName;

    private void Awake()
    {
        dir = Application.persistentDataPath;
    }
    void Start()
    {
        SetSubscribe();
        Debug.LogWarning("DEV::press 'O' to open directory");
    }

    private void SetSubscribe()
    {
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.O))
            .Subscribe(_ =>
            {
                OpenDir.Open(dir);
            }).AddTo(this);

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Space))
            .Subscribe(_ =>
            {
                if (!isPrepared || !videoPlayer.isPrepared || filename != loadedFileName)
                {
                    ReleaseVideo();
                    LoadVideo();
                    return;
                }
                long nextFrame = videoPlayer.frame + 1;

                if (nextFrame < (long)videoPlayer.frameCount)
                { 
                    videoPlayer.frame = nextFrame;
                    videoPlayer.Play();
                    videoPlayer.Pause();

                    if (videoPlayer.texture != null)
                    {
                        SetTexture(videoPlayer.texture);
                        Debug.Log($"{nextFrame}/{videoPlayer.frameCount}");
                    }
                }
                else
                {
                    Debug.Log("🔚 마지막 프레임입니다.");
                }
            }).AddTo(this);
    }
    private void LoadVideo()
    {
        // 1. VideoPlayer 컴포넌트 동적 추가
        videoPlayer = gameObject.AddComponent<VideoPlayer>();

        // 2. 설정
        string fullPath = Path.Combine(dir, $"{filename}.{extension}");
        Debug.Log($"Load Video {fullPath}");

        string videoUrl = "file://" + fullPath;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = videoUrl;
        videoPlayer.renderMode = VideoRenderMode.APIOnly;
        videoPlayer.skipOnDrop = false;
        videoPlayer.playOnAwake = false;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.isLooping = false;

        // 3. 준비 완료 시 이벤트 등록
        videoPlayer.prepareCompleted += OnVideoPrepared;

        // 4. 로드 시작
        videoPlayer.Prepare();
        loadedFileName = filename;
    }
    private void ReleaseVideo()
    {
        isPrepared = false;
        loadedFileName = null;

        if (videoPlayer != null)
        {
            // 1. 이벤트 제거
            videoPlayer.prepareCompleted -= OnVideoPrepared;

            // 2. 재생 중지
            if (videoPlayer.isPlaying)
                videoPlayer.Stop();

            // 3. RenderTexture 해제 (선택적)
            if (videoPlayer.targetTexture != null)
            {
                videoPlayer.targetTexture.Release();
                videoPlayer.targetTexture = null;
            }

            // 4. VideoPlayer 제거
            Destroy(videoPlayer);
            videoPlayer = null;
        }
    }
    private void OnVideoPrepared(VideoPlayer vp)
    {
        isPrepared = true;

        vp.frame = 0;
        vp.Play();
        vp.Pause();

        if (vp.texture != null)
        {
            SetTexture(vp.texture);
        }
    }
    private void SetTexture(Texture tex)
    {
        float ratio = (float)tex.width / tex.height;
        Debug.Log($"{tex.width}x{tex.height} | {ratio}");
        rawImage.GetComponent<AspectRatioFitter>().aspectRatio = ratio;
        rawImage.texture = tex;

    }
}
