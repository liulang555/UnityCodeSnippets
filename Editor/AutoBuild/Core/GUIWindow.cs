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
    public class GUIWindow : EditorWindow
    {
        private AppSettings appSettings;//当前配置
        private bool _isBuilding = false;//是不是正在打包

        [MenuItem("Tools/自动打包")]
        public static void ShowWindow()
        {
            GUIWindow window = GetWindow<GUIWindow>("自动打包");
            window.minSize = new Vector2 (250, 800);
            window.OnInit();
        }
        //默认值
        private void OnInit()
        {
            _isBuilding = false;
        }

        // 新增序列化对象字段
        private SerializedObject _serializedObject;
        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            // 配置选择面板
            var newSettings = (AppSettings)EditorGUILayout.ObjectField("当前配置", appSettings, typeof(AppSettings), false);
            if (newSettings != appSettings)
            {
                appSettings = newSettings;
                // 强制创建新的序列化对象
                _serializedObject = new SerializedObject(appSettings); 
            }
            
            EditorGUILayout.Space(30);
            
            if (appSettings != null)
            {
                // 确保使用最新的序列化对象
                if (_serializedObject == null)
                {
                    _serializedObject = new SerializedObject(appSettings);
                }
                _serializedObject.Update();

                EditorGUILayout.PropertyField(_serializedObject.FindProperty("_outputPath"));
                if (appSettings._platform == AutoBuildPlatform.Android_Apk 
                    || appSettings._platform == AutoBuildPlatform.Android_AAB 
                    || appSettings._platform == AutoBuildPlatform.IOS)
                {
                    EditorGUILayout.PropertyField(_serializedObject.FindProperty("_versionCode"));
                    EditorGUILayout.PropertyField(_serializedObject.FindProperty("_productVersion"));
                }
                EditorGUILayout.PropertyField(_serializedObject.FindProperty("_needBundle"));
                EditorGUILayout.PropertyField(_serializedObject.FindProperty("_platform"));
                EditorGUILayout.PropertyField(_serializedObject.FindProperty("_channelCode"));
                EditorGUILayout.PropertyField(_serializedObject.FindProperty("_httpServerType"));
                EditorGUILayout.PropertyField(_serializedObject.FindProperty("_bundleServerType"));
                EditorGUILayout.PropertyField(_serializedObject.FindProperty("_isOnlyChapter0"));
                EditorGUILayout.PropertyField(_serializedObject.FindProperty("_showTestLogView"));
                EditorGUILayout.PropertyField(_serializedObject.FindProperty("_videoDownloadMode"));
                EditorGUILayout.PropertyField(_serializedObject.FindProperty("_yooAssetPlayMode"));
                _serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.Space(30);

            // 保留原有的操作按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("修改配置", GUILayout.Height(30)))
            {
                StartBuild(false);
            }
            if (GUILayout.Button("保存", GUILayout.Height(30)))
            {
                SaveConfig();
            }
            if (GUILayout.Button("执行打包", GUILayout.Height(30)))
            {
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
            if (string.IsNullOrEmpty(appSettings._outputPath))
            {
                throw new Exception("打包路径未指定！");
            }
            if (string.IsNullOrEmpty(appSettings._productName))
            {
                throw new Exception("软件名称为空，请修复！");
            }

            Context context = new Context();
            // 使用appSettings中的配置
            context.SetParameter(BuildParameterKeys.ChannelCode, appSettings._channelCode);
            context.SetParameter(BuildParameterKeys.HttpServerType, appSettings._httpServerType);
            context.SetParameter(BuildParameterKeys.BundleServerType, appSettings._bundleServerType);
            context.SetParameter(BuildParameterKeys.IsOnlyChapter0, appSettings._isOnlyChapter0);
            context.SetParameter(BuildParameterKeys.ShowTestLogView, appSettings._showTestLogView);
            // 新增应用配置参数
            context.SetParameter(BuildParameterKeys.PackgeName, appSettings._packgeName);
            context.SetParameter(BuildParameterKeys.ProductName, appSettings._productName);
            context.SetParameter(BuildParameterKeys.ProductVersion, appSettings._productVersion);
            context.SetParameter(BuildParameterKeys.VersionCode, appSettings._versionCode);
            //m3u8
            context.SetParameter(BuildParameterKeys.M3u8SourceFileFloder, DownLoadModule.PathUtitly.GetEditorVideoDir("BundleVideoRes1080P\\M3u8"));
            context.SetParameter(BuildParameterKeys.M3u8ReplaceServerAddress, "https://tsrgame.crotnet.com/CDN/Video/1080P/CommonVideoClips/");
            //其他
            context.SetParameter(BuildParameterKeys.OutputPath, appSettings._outputPath);
            context.SetParameter(BuildParameterKeys.NeedBuildBundle, appSettings._needBundle);
            context.SetParameter(BuildParameterKeys.RemoveStartScenes, appSettings._removeStartScenes);
            context.SetParameter(BuildParameterKeys.BuildPlatform, appSettings._platform);
            context.SetParameter(BuildParameterKeys.VideoDownloadMode, appSettings._videoDownloadMode);
            context.SetParameter(BuildParameterKeys.YooAssetPlayMode, appSettings._yooAssetPlayMode);

            PreBuild();
           
            BuildManager buildManager = new BuildManager(context);

            bool successPreBuild = buildManager.ExecuteBuildTask(context, BuildTaskType.PreBuild);
            if(!successPreBuild)
                return;
            if (!isBuild)
                return;
            bool successMain = buildManager.ExecuteBuildTask(context, BuildTaskType.Build);
            if (!successMain)
                return;
            buildManager.ExecuteBuildTask(context, BuildTaskType.PostBuild);
            PostBuild(context);
        }
        private void PreBuild()
        {
            PlayerSettings.productName = appSettings._productName;
            PlayerSettings.bundleVersion = appSettings._productVersion;
        }
        private void PostBuild(Context context)
        {
            var productVersion = context.GetParameter<string>(BuildParameterKeys.ProductVersion);
            int versionCode = context.GetParameter<int>(BuildParameterKeys.VersionCode);
            appSettings._productVersion = productVersion;
            appSettings._versionCode = versionCode;
            SaveConfig();
        }
        // 新增保存配置到资产文件
        private void SaveConfig()
        {
#if UNITY_EDITOR
            if (appSettings != null)
            {
                EditorUtility.SetDirty(appSettings);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("配置保存成功！");
            }
#endif
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