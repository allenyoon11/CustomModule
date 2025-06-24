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
    public string filename = "";

    private string dir;
    private VideoPlayer videoPlayer;
    private bool isPrepared = false;

    private void Awake()
    {
        dir = Application.persistentDataPath;
    }
    void Start()
    {
        

        SetSubscribe();
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
                if (!isPrepared || !videoPlayer.isPrepared)
                {
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
                        rawImage.texture = videoPlayer.texture;
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
        string fullPath = Path.Combine(dir, filename);
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
    }
    private void OnVideoPrepared(VideoPlayer vp)
    {
        isPrepared = true;

        vp.frame = 0;
        vp.Play();
        vp.Pause();

        if (vp.texture != null)
        {
            rawImage.texture = vp.texture;
        }
    }
}
