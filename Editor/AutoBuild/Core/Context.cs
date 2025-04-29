using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace AutoBuildSystem
{
    public class Context
    {
        #region 共享参数
        /// <summary>
        /// 任务间共享的参数字典，用于在不同任务、平台和渠道之间传递数据
        /// </summary>
        public Dictionary<BuildParameterKeys, object> SharedParameters { get; private set; } = new Dictionary<BuildParameterKeys, object>();

        /// <summary>
        /// 添加或更新共享参数
        /// </summary>
        public void SetParameter(BuildParameterKeys key, object value)
        {
            if (SharedParameters.ContainsKey(key))
            {
                SharedParameters[key] = value;
            }
            else
            {
                SharedParameters.Add(key, value);
            }
        }

        /// <summary>
        /// 获取共享参数，如果不存在则返回默认值
        /// </summary>
        public T GetParameter<T>(BuildParameterKeys key, T defaultValue = default)
        {
            if (SharedParameters.TryGetValue(key, out object value))
            {
                if (value is T typedValue)
                {
                    return typedValue;
                }
                else
                {
                    // 类型不匹配
                    string errorMsg = $"参数类型不匹配: 键 {key} 的值类型为 {value.GetType().Name}，但请求的类型为 {typeof(T).Name}";
                    if (Logger != null)
                        Logger.LogError(errorMsg);
                    else
                        Debug.LogError(errorMsg);
                    return defaultValue;
                }
            }
            
            // 参数不存在
            string warningMsg = $"参数不存在: {key}，使用默认值 {defaultValue}";
            if (Logger != null)
                Logger.LogWarning(warningMsg);
            else
                Debug.LogWarning(warningMsg);
            return defaultValue;
        }

        /// <summary>
        /// 检查参数是否存在
        /// </summary>
        public bool HasParameter(BuildParameterKeys key)
        {
            return SharedParameters.ContainsKey(key);
        }
        #endregion

        #region 日志记录器
        /// <summary>
        /// 日志记录器
        /// </summary>
        public BuildLogger Logger { get; private set; }
        /// <summary>
        /// 初始化日志记录器
        /// </summary>
        /// <param name="logFilePath">日志文件路径，如果为null则自动生成</param>
        public void InitLogger(string logFilePath = null)
        {
            Logger = new BuildLogger(logFilePath);
            Logger.Log($"初始化构建配置:\n" +
                     $"平台: {GetParameter<AutoBuildPlatform>(BuildParameterKeys.BuildPlatform)}\n" +
                     $"渠道: {GetParameter<ChannelCodeType>(BuildParameterKeys.ChannelCode)}\n" +
                     $"输出路径: {GetParameter<string>(BuildParameterKeys.OutputPath)}\n" +
                     $"下载模式: {GetParameter<EDownloadMode>(BuildParameterKeys.VideoDownloadMode)}\n" +
                     $"运行模式: {GetParameter<YooAsset.EPlayMode>(BuildParameterKeys.YooAssetPlayMode)}\n" +
                     $"是否需要打包资源: {GetParameter<bool>(BuildParameterKeys.NeedBuildBundle)}\n" +
                     $"是否移除StartGame场景: {GetParameter<bool>(BuildParameterKeys.RemoveStartScenes)}\n" +
                     $"是不是只有序章: {GetParameter<bool>(BuildParameterKeys.IsOnlyChapter0)}\n" +
                     $"服务器类型: {GetParameter<int>(BuildParameterKeys.HttpServerType)}\n" +
                     $"资源服务器: {GetParameter<int>(BuildParameterKeys.BundleServerType)}");
        }
        #endregion
    }
}