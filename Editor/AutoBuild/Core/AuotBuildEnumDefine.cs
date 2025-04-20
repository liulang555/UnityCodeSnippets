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
        Ios,
        MacOs
    }
    public enum AutoBuildTaskStatus
    {
        Pending,
        Running,
        Completed,
        Failed,
        Cancelled
    }
    /// <summary>
    /// 构建任务优先级枚举
    /// </summary>
    public enum BuildTaskPriority
    {
        #region 预构建任务优先级
        ModifyScriptingDefineSymbolsTask = 10,  // 修改宏定义 Scripting Define Symbols
        AutoHybridCLRTask        = 20,  // 生成热更新代码 HybridCLR GenerateAll
        YooAssetTask             = 30,  // YooAsset 打包Bundle资源
        ScenesModifiyTask        = 40,  // 场景文件修改
        ModifyBuildConfigTask    = 50,  // 修改打包配置
        VersionAddTask           = 60,  // 自动增加版本号
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
        //启动配置参数
        ChannelCode,
        HttpServerType,
        BundleServerType,
        IsOnlyChapter0,
        // 通用参数
        TimeStr,
        ExprotFloderPath, //通用导出文件夹
        WindwosExePath,  // windows打包exe路径
        WindwosFloderPath, // windows打包文件夹路径   带时间标签的
        BuildOutPut,  //通用打包导出路径  各个平台单独设置
        OpenDirectory, //最后打包完成之后打开的路径

        // 构建相关
        BuildSuccess,
        BuildCompletionTime,
        BuildErrorCount,

        SteamAppID,
    }
}
