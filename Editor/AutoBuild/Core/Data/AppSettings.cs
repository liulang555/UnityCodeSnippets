using UnityEngine;
using System;
using YooAsset;

namespace AutoBuildSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AppSettings", menuName = "AutoBuildSystem/AppSettings", order = 51)]
    public class AppSettings : ScriptableObject
    {
        [OpenFolder, Header("保存路径(Root)")]
        public string _outputPath;

        [Header("包名：")]
        public string _packgeName;
        [Header("应用名：")]
        public string _productName;
        [Header("软件版本：（形如：1.0.0）")]
        public string _productVersion;
        [Header("软件版本代码：（一直递增）")]
        public int _versionCode;

        [Header("是否重新打包AssetBundle")]
        public bool _needBundle = true;

        [Header("是否移除StartGame场景（测试用）")]
        public bool _removeStartScenes = false;

        [Header("目标构建平台")]
        public AutoBuildPlatform _platform = 0;

        [Header("渠道登录方式（Steam/Epic等）")]
        public ChannelCodeType _channelCode = 0;
        
        [Header("服务器环境选择：0=内网，1=外网")]
        public int _httpServerType = 0;
        
        [Header("资源服务器类型：0=内网，1=外网S3")]
        public int _bundleServerType = 0;
        
        [Header("是否仅包含序章内容")]
        public bool _isOnlyChapter0;
        
        [Header("是否显示测试用日志面板")]
        public bool _showTestLogView;

        [Header("视频资源加载模式：Editor模拟/运行时下载")]
        public EDownloadMode _videoDownloadMode;
        
        [Header("YooAsset资源加载模式：Editor模拟/本地加载/联机模式")]
        public EPlayMode _yooAssetPlayMode;
    }
}
