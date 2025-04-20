//#if Use_OpenCVForUnity
//using OpenCVForUnity.VideoioModule;
//#endif
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using XNodeEditor;

public class VideoEditorWindow : EditorWindow
{
    private long AddFrame = 180;//快进快退的帧数
    private VideoPlayer videoPlayer;
    private float hSliderValue;
    private float lastSliderValue;
    private bool isPlaying;
    private RenderTexture renderTexture;
    private string videoFilePath;
    private VideoClip videoClip;
    private static string videoFileDir = "/BundleVideoRes";
    private static int VideoWidth = 960;
    private static int VideoHeight = 540;
    private static int RenderTextrueWidth = 1920;
    private static int RenderTextrueVideoHeight = 1080;
    private float ScaleRate = 1.0f;//分辨率缩放
    private Color bg = new Color(1, 1, 1, 0.2f);
    private Color frame = new Color(1, 1, 1, 0.2f);
    private Rect VideoPlayRect = new Rect();

    [MenuItem("Tools/Video Editor")]
    public static void ShowWindow()
    {
        GetWindow<VideoEditorWindow>("Video Editor");
    }
    private void OnEnable()
    {
        videoPlayer = new GameObject("VideoPlayer").AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        // 注册事件
        videoPlayer.prepareCompleted += OnPrepareCompleted;
        videoPlayer.loopPointReached += OnLoopPointReached;
        videoPlayer.errorReceived += OnErrorReceived;
        videoPlayer.started += OnVideoStarted;
        Application.runInBackground = true;
        EditorApplication.update += OnEditorUpdate;
    }
    
    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
        if(videoPlayer != null)
        {
            // 注册事件
            videoPlayer.prepareCompleted -= OnPrepareCompleted;
            videoPlayer.loopPointReached -= OnLoopPointReached;
            videoPlayer.errorReceived -= OnErrorReceived;
            videoPlayer.started -= OnVideoStarted;
        }
        DestroyImmediate(videoPlayer.gameObject);
        if(renderTexture != null )
        {
            renderTexture.Release();
            renderTexture = null;
        }
    }
    
    private bool isSelecting = false;
    private Vector2 startMousePosition;
    private Vector2 currentMousePosition;
    void OnEditorUpdate()
    {
        Repaint();
    }
    private void LoadVideo(string path)
    {
        videoPlayer.url = path;
        renderTexture = null;
        
        // 设置视频播放器
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = path;
        videoPlayer.isLooping = false;
        videoPlayer.skipOnDrop = true;
        
        // 准备视频播放器以获取视频信息
        videoPlayer.Prepare();
    }
    void OnPrepareCompleted(VideoPlayer source)
    {
        Debug.Log("视频准备完成，开始播放");
        // 获取视频的宽度和高度
        VideoWidth = (int)videoPlayer.width;
        VideoHeight = (int)videoPlayer.height;

        RenderTextrueWidth = (int)(VideoWidth * ScaleRate);
        RenderTextrueVideoHeight = (int)(VideoHeight * ScaleRate);
        Debug.Log($"LoadVideo: {RenderTextrueWidth}x{RenderTextrueVideoHeight}");

        renderTexture = new RenderTexture(RenderTextrueWidth, RenderTextrueVideoHeight, 0);
        VideoPlayViewRect = new Rect(20, 110, RenderTextrueWidth + 100, RenderTextrueVideoHeight + 100);
        VideoPlayRect = new Rect(VideoPlayViewRect.x + 50, VideoPlayViewRect.y + 50, RenderTextrueWidth, RenderTextrueVideoHeight);
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.Play();
        isPlaying = true;
    }

    void OnLoopPointReached(VideoPlayer source)
    {
        Debug.Log("视频播放到循环点");
    }

    void OnErrorReceived(VideoPlayer source, string message)
    {
        Debug.LogError($"视频播放出错: {message}");
    }

    void OnVideoStarted(VideoPlayer source)
    {
        Debug.Log("视频开始播放");
    }
    private Vector2 scrollPosition;
    private Rect VideoPlayViewRect = new Rect();
    private void OnGUI()
    {
        if (videoPlayer == null)
            return;
        scrollPosition = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height),
            scrollPosition, new Rect(0, 0, VideoPlayRect.width + 500, VideoPlayRect.height + 500),true,true);
        DrawSelectVideo();
        //Debug.Log(videoPlayer.isPrepared + "  width: " + videoPlayer.width + "  height   " + videoPlayer.height);
        DrawPlayControlButton();
        if (renderTexture != null)
        {
            EditorGUI.DrawRect(VideoPlayViewRect, bg);
            GUI.DrawTexture(VideoPlayRect, renderTexture);
        }
        DrawFrame();
        GUI.EndScrollView();
    }
    //视频控制按钮
    private void DrawPlayControlButton()
    {
        if (GUI.Button(new Rect(200, 20, 100, 20), "快退"))
        {
            long curframe = videoPlayer.frame - AddFrame;
            if (curframe <= 0)
            {
                curframe = 0;
            }
            videoPlayer.frame = curframe;
            hSliderValue = (float)(curframe / (float)(videoPlayer.frameCount));
            //Debug.Log("Backward hSliderValue: " + hSliderValue);
            lastSliderValue = hSliderValue;
        }
        if (GUI.Button(new Rect(300, 20, 100, 20), isPlaying ? "暂停" : "播放"))
        {
            if (isPlaying)
            {
                videoPlayer.Pause();
            }
            else
            {
                videoPlayer.Play();
            }
            isPlaying = !isPlaying;
        }
        if (GUI.Button(new Rect(400, 20, 100, 20), "快进"))
        {
            long curframe = videoPlayer.frame + AddFrame;
            videoPlayer.frame = curframe;
            hSliderValue = (float)(curframe / (float)(videoPlayer.frameCount));
            //Debug.Log("Forward hSliderValue: " + hSliderValue);
            lastSliderValue = hSliderValue;
        }
        GUI.Label(new Rect(550, 20, 100, 20), $"帧: {FormatTime(videoPlayer.frame)} / {FormatTime(videoPlayer.frameCount)}");
        GUI.Label(new Rect(700,20,100,20),$"时间: {FormatTime(videoPlayer.time)} / {FormatTime(videoPlayer.length)}");
        hSliderValue = EditorGUI.Slider(new Rect(100, 40, 800, 20), hSliderValue, 0, (float)videoPlayer.length);
        if (lastSliderValue != hSliderValue)
        {
            lastSliderValue = hSliderValue;
            videoPlayer.time = hSliderValue;
        }
    }

    //选择视频
    private void DrawSelectVideo()
    {
        GUI.Label(new Rect(0,0,80,20),"请选择：", EditorStyles.boldLabel);
        if (GUI.Button(new Rect(100, 0, 100, 20), "选择视频"))
        {
            videoFilePath = EditorUtility.OpenFilePanel("Select Video File", GetProjectPath(videoFileDir), "mp4,avi,mov");
            if (!string.IsNullOrEmpty(videoFilePath))
            {
                Debug.Log("Selected video file: " + videoFilePath);
                LoadVideo(videoFilePath);
            }
        }
        GUI.Label(new Rect(220, 0, 180, 20),$"视频分辨率: {RenderTextrueWidth}x{RenderTextrueVideoHeight}");
        GUI.Label(new Rect(400, 0, 800, 20), "当前选择的视频: " + videoFilePath);
    }
    //选择框
    private void DrawFrame()
    {
        if(isPlaying)
        {
            return;//暂停状态才能选择
        }
        // 处理鼠标按下事件
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && VideoPlayRect.Contains(Event.current.mousePosition))
        {
            isSelecting = true;
            startMousePosition = Event.current.mousePosition;
        }
        // 处理鼠标拖动事件
        if (Event.current.type == EventType.MouseDrag && isSelecting && VideoPlayRect.Contains(Event.current.mousePosition))
        {
            currentMousePosition = Event.current.mousePosition;
        }
        // 处理鼠标松开事件
        if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && VideoPlayRect.Contains(Event.current.mousePosition))
        {
            //isSelecting = false;
        }
        // 绘制选框
        if (isSelecting)
        {
            Rect selectionRect = new Rect(
                Mathf.Min(startMousePosition.x, currentMousePosition.x),
                Mathf.Min(startMousePosition.y, currentMousePosition.y),
                Mathf.Abs(startMousePosition.x - currentMousePosition.x),
                Mathf.Abs(startMousePosition.y - currentMousePosition.y)
            );
            EditorGUI.DrawRect(selectionRect, frame);
            GUI.Label(new Rect(500, 60, 200, 20), $"StartPoint({startMousePosition.x - VideoPlayRect.x},{startMousePosition.y - VideoPlayRect.y})");
            GUI.Label(new Rect(700, 60, 200, 20), $"EndPoint({currentMousePosition.x - VideoPlayRect.x},{currentMousePosition.y - VideoPlayRect.y})");
        }
        if (GUI.Button(new Rect(300, 60, 100, 20), "清除选框"))
        {
            currentMousePosition = Vector2.zero;
            startMousePosition = Vector2.zero;
            isSelecting = false;
        }
    }

    private string FormatTime(double time)
    {
        int minutes = Mathf.FloorToInt((float)time / 60F);
        int seconds = Mathf.FloorToInt((float)time - minutes * 60);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }
    /// <summary>
    /// 获取项目工程路径
    /// </summary>
    public static string GetProjectPath(string Dir)
    {
        string projectPath = Path.GetDirectoryName(Application.dataPath);
        projectPath = YooAsset.Editor.EditorTools.GetRegularPath(projectPath);
        return $"{projectPath}/{Dir}";
    }
}

