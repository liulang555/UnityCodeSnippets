using UnityEngine;
using UnityEditor;
using ConfigTool;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AutoBuildSystem
{
    public class AutoBuildWindow : EditorWindow
    {
        #region 参数
        private const string _cachedPathKey = "AutoBuildWindowCachedPath";
        private bool _isBuilding = false; // 添加一个标志位表示是否正在构建
        private string _outputPath;//根路径

        private ChannelCodeType _channelCode = 0;//登录方式选择
        private AutoBuildPlatform _platform = 0;//登录方式选择
        private int _httpServerType = 0;//服务器环境
        private int _bundleServerType = 0;//资源服务器地址
        private bool _isOnlyChapter0;// 是不是只有序章的内容
        private bool _showTestLogView;//是不是打开测试的log界面

        private EDownloadMode _videoDownloadMode;
        private YooAsset.EPlayMode _yooAssetPlayMode;

        private bool _needBundle = true;//重新打包资源
        private bool _removeStartScenes = false;//移除StartGame场景，测试的时候更方便
        private UseVideoSourceFile _useVideoSourceFile;
        #endregion

        [MenuItem("Tools/自动打包")]
        public static void ShowWindow()
        {
            AutoBuildWindow window = GetWindow<AutoBuildWindow>("自动打包");
            window.minSize = new Vector2 (250, 600);
            window.OnInit();
        }
        //默认值
        private void OnInit()
        {
            _yooAssetPlayMode = YooAsset.EPlayMode.OfflinePlayMode;
            _videoDownloadMode = EDownloadMode.OfflinePlayMode;
            _outputPath = EditorPrefs.GetString(_cachedPathKey, "");// 从 EditorPrefs 中加载缓存的路径
            _needBundle = false;
            _isOnlyChapter0 = false;
        }
        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space(30);

            GUILayout.BeginHorizontal();// 水平布局组，将输入框和按钮放在同一行
            EditorGUILayout.LabelField("保存路径:", GUILayout.Width(60));
            _outputPath = EditorGUILayout.TextField(_outputPath, GUILayout.Width(170));
            if (GUILayout.Button("Select Folder"))
            {
                string path = EditorUtility.OpenFolderPanel("Select a Folder", "", "");
                if (!string.IsNullOrEmpty(path))
                {
                    _outputPath = path;
                    EditorPrefs.SetString(_cachedPathKey, _outputPath); // 将路径保存到 EditorPrefs 中
                }
            }
            GUILayout.EndHorizontal();

            // 资源打包开关
            _needBundle = EditorGUILayout.Toggle("是否重新打包资源", _needBundle);
            EditorGUILayout.Space(20);
            _removeStartScenes = EditorGUILayout.Toggle("移除StartGame场景", _removeStartScenes);
            EditorGUILayout.Space(20);

            // 网络类型单选
            EditorGUILayout.LabelField("服务器环境:");
            EditorGUILayout.BeginHorizontal();
            _httpServerType = GUILayout.Toolbar(_httpServerType, new[] { "内网", "外网" });
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);

            // 资源服务器地址
            EditorGUILayout.LabelField("资源服务器地址:");
            EditorGUILayout.BeginHorizontal();
            _bundleServerType = GUILayout.Toolbar(_bundleServerType, new[] { "内网", "外网s3" });
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(20);
            _isOnlyChapter0 = EditorGUILayout.Toggle("只有序章", _isOnlyChapter0);
            EditorGUILayout.Space(20);
            _showTestLogView = EditorGUILayout.Toggle("打开Log", _showTestLogView);
            EditorGUILayout.Space(20);
            // 登录方式选择
            _platform = (AutoBuildPlatform)EditorGUILayout.EnumPopup("平台", _platform);
            EditorGUILayout.Space(20);
            _channelCode = (ChannelCodeType)EditorGUILayout.EnumPopup("登录方式选择", _channelCode);

            EditorGUILayout.Space(40);
            _videoDownloadMode = (EDownloadMode)EditorGUILayout.EnumPopup("视频加载模式", _videoDownloadMode);
            _yooAssetPlayMode = (YooAsset.EPlayMode)EditorGUILayout.EnumPopup("bundle加载模式", _yooAssetPlayMode);
            // 选择打包完成之后 拷贝到streamasset中的视频文件
            EditorGUILayout.Space(20);
            _useVideoSourceFile = (UseVideoSourceFile)EditorGUILayout.EnumPopup("视频文件选择", _useVideoSourceFile);
            EditorGUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("修改配置", GUILayout.Height(30)))
            {
                StartBuild(false);
            }
            // 打包按钮
            if (GUILayout.Button("执行打包", GUILayout.Height(30)))
            {
                StartBuild(true);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Ios测试", GUILayout.Height(30)))
            {
                _platform = AutoBuildPlatform.Ios;
                _channelCode = ChannelCodeType.AppleStore;
                _removeStartScenes = true;
                _httpServerType = 0;
                _yooAssetPlayMode = YooAsset.EPlayMode.OfflinePlayMode;
                _videoDownloadMode = EDownloadMode.OfflinePlayMode;
                _isOnlyChapter0 = true;
                _showTestLogView = true;
                StartBuild(true);
            }
            if (GUILayout.Button("Win测试", GUILayout.Height(30)))
            {
                _platform = AutoBuildPlatform.Windows;
                _channelCode = ChannelCodeType.LocalTest;
                _removeStartScenes = true;
                _httpServerType = 0;
                _yooAssetPlayMode = YooAsset.EPlayMode.OfflinePlayMode;
                _videoDownloadMode = EDownloadMode.OfflinePlayMode;
                _isOnlyChapter0 = true;
                _showTestLogView = true;
                StartBuild(true);
            }
            if (GUILayout.Button("Android测试", GUILayout.Height(30)))
            {
                _platform = AutoBuildPlatform.Android_Apk;
                _channelCode = ChannelCodeType.LocalTest;
                _removeStartScenes = true;
                _httpServerType = 0;
                _yooAssetPlayMode = YooAsset.EPlayMode.OfflinePlayMode;
                _videoDownloadMode = EDownloadMode.OfflinePlayMode;
                _isOnlyChapter0 = true;
                _showTestLogView = true;
                StartBuild(true);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        
        private void StartBuild(bool isBuild)
        {
            if (_isBuilding){ 
                return; 
            }
            _isBuilding = true;
            //确保构建操作在下一帧执行,避免 OnGUI 执行时间过长导致的布局错误
            EditorApplication.delayCall += () =>
            {
                try
                {
                    ExecuteBuild(isBuild);
                    _isBuilding = false;
                }
                finally
                {
                    _isBuilding = false;
                    Repaint(); // 刷新窗口
                }
            };
        }
        private void ExecuteBuild(bool isBuild)
        {
            AutoBuildConfig autoBuildConfig = new AutoBuildConfig();
            autoBuildConfig.SetParameter(BuildParameterKeys.OutputPath, _outputPath);
            autoBuildConfig.SetParameter(BuildParameterKeys.NeedBuildBundle, _needBundle);
            autoBuildConfig.SetParameter(BuildParameterKeys.RemoveStartScenes, _removeStartScenes);
            autoBuildConfig.SetParameter(BuildParameterKeys.BuildPlatform, _platform);
            autoBuildConfig.SetParameter(BuildParameterKeys.VideoDownloadMode, _videoDownloadMode);
            autoBuildConfig.SetParameter(BuildParameterKeys.YooAssetPlayMode, _yooAssetPlayMode);

            autoBuildConfig.SetParameter(BuildParameterKeys.ChannelCode, _channelCode);
            autoBuildConfig.SetParameter(BuildParameterKeys.HttpServerType, _httpServerType);
            autoBuildConfig.SetParameter(BuildParameterKeys.BundleServerType,_bundleServerType);
            autoBuildConfig.SetParameter(BuildParameterKeys.IsOnlyChapter0, _isOnlyChapter0);
            autoBuildConfig.SetParameter(BuildParameterKeys.ShowTestLogView, _showTestLogView);

            BuildManager buildManager = new BuildManager(autoBuildConfig);

            bool successPreBuild = buildManager.ExecuteBuildTask(autoBuildConfig, BuildTaskType.PreBuild);
            if(!successPreBuild)
                return;
            if (!isBuild)
                return;
            bool successMain = buildManager.ExecuteBuildTask(autoBuildConfig, BuildTaskType.Build);
            if (!successMain)
                return;
            buildManager.ExecuteBuildTask(autoBuildConfig, BuildTaskType.PostBuild);
        }
        private void LogGraphics()
        {
            UnityEngine.Rendering.GraphicsDeviceType[] graphicsDeviceTypes = PlayerSettings.GetGraphicsAPIs( BuildTarget.Android);
            for (int i = 0; i < graphicsDeviceTypes.Length; i++)
            {
                Debug.Log("BuildTarget.Android,LogGraphics: " + graphicsDeviceTypes[i]);
            }
        }
    }
}