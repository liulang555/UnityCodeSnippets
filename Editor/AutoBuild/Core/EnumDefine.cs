using UnityEngine;

namespace AutoBuildSystem
{
    /// <summary>
    /// 任务类型
    /// </summary>
    public enum BuildTaskType
    {
        /// <summary>
        /// APP 构建前
        /// </summary>
        PreBuild,
        /// <summary>
        /// 构建中
        /// </summary>
        Build,
        /// <summary>
        /// APP 构建完成后
        /// </summary>
        PostBuild
    }
    public enum UseVideoSourceFile
    {
        None,
        Chapter0_4K,
        All_4k,
        Chapter0_1080P,
        All_1080P,
    }
    public enum AutoBuildPlatform
    {
        Windows,
        Windwos_SteamDeck,
        Android_Apk,
        Android_AAB,
        IOS,
        MacOs
    }
    /// <summary>
    /// 构建任务优先级枚举
    /// </summary>
    public enum BuildTaskPriority
    {
        #region 预构建任务优先级
        VersionAddTask           = 10,  // 自动增加版本号
        M3u8TsUrlReplacer        = 20,  //修改m3u8的ts文件地址为服务器地址
        ModifyScriptingDefineSymbolsTask = 30,  // 修改宏定义 Scripting Define Symbols
        AutoHybridCLRTask        = 40,  // 生成热更新代码 HybridCLR GenerateAll
        YooAssetTask             = 50,  // YooAsset 打包Bundle资源
        ScenesModifiyTask        = 60,  // 场景文件修改
        ModifyBuildConfigTask    = 70,  // 修改打包配置
        #endregion

        #region 打包任务优先级
        BuildPlayerTask   = 100, // 构建应用程序
        #endregion

        #region 后构建任务优先级
        SteamAppIDTask           = 200, // 创建 Steam AppID 文件
        OpenBuildDirectoryTask   = 1000, // 设置为较大的值，确保在其他任务之后执行
        #endregion
    }
    /// <summary>
    /// 构建系统中共享参数的键名枚举
    /// </summary>
    public enum BuildParameterKeys
    {
        OutputPath,
        NeedBuildBundle,
        RemoveStartScenes,
        BuildPlatform,
        VideoDownloadMode,
        YooAssetPlayMode,
        ShowTestLogView,
        //启动配置参数
        ChannelCode,
        HttpServerType,
        BundleServerType,
        IsOnlyChapter0,
        // 新增应用配置参数
        PackgeName,         // 对应 _packgeName
        ProductName,        // 对应 _productName
        ProductVersion,     // 对应 _productVersion
        VersionCode,        // 对应 _versionCode
        // 通用参数
        TimeStr,
        ExprotFloderPath, //通用导出文件夹
        WindwosExePath,  // windows打包exe路径
        WindwosFloderPath, // windows打包文件夹路径   带时间标签的
        BuildOutPut,  //通用打包导出路径  各个平台单独设置
        OpenDirectory, //最后打包完成之后打开的路径
        M3u8SourceFileFloder,  //源m3u8文件存放地址
        M3u8TargetFileFloder,  //m3u8拷贝到地址
        M3u8ReplaceServerAddress, //m3u8中ts文件的服务器url

        // 构建相关
        BuildSuccess,
        BuildCompletionTime,
        BuildErrorCount,

        SteamAppID,
    }
}
