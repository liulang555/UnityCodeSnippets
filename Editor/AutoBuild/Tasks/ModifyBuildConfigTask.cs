using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AutoBuildSystem
{
    public class ModifyBuildConfigTask : BaseBuildTask
    {
        public const string resourcePath = "Resources/BuildConfig.json";

        public ModifyBuildConfigTask()
        {
            TaskId = "modify_build_config";
            TaskName = "修改打包配置";
            Priority = (int)BuildTaskPriority.ModifyBuildConfigTask;
            TaskType = BuildTaskType.PreBuild;
            Status = AutoBuildTaskStatus.Pending;
        }
        public override bool Execute(AutoBuildConfig config, IAutoBuildPlatform platform, IChannel channel)
        {
            try
            {
                Status = AutoBuildTaskStatus.Running;
                
                string fullPath = Application.dataPath + "/" + resourcePath;
                if (File.Exists(fullPath))
                {
                    string originalText = File.ReadAllText(fullPath);
                    BuildConfig buildConfig = JsonUtility.FromJson<BuildConfig>(originalText);
                    
                    // 使用SharedParameters中的参数
                    buildConfig.ChannelCode = config.GetParameter<ChannelCodeType>(BuildParameterKeys.ChannelCode);
                    buildConfig.HttpServerType = config.GetParameter<int>(BuildParameterKeys.HttpServerType);
                    buildConfig.OnlyChapter0 = config.GetParameter<bool>(BuildParameterKeys.IsOnlyChapter0);
                    
                    int bundleServerType = config.GetParameter<int>(BuildParameterKeys.BundleServerType);
                    if (bundleServerType == 0)
                    {
                        buildConfig.BundleServerUrl = "http://192.168.0.185:8080";
                    }
                    if (bundleServerType == 1)
                    {
                        buildConfig.BundleServerUrl = "https://s3.ap-southeast-1.amazonaws.com/static.innovationdreamtech.com";
                    }

                    string modifiedText = JsonUtility.ToJson(buildConfig, true);

                    //config.Logger.Log($"配置更新 ChannelCode: {buildConfig.ChannelCode}");
                    //config.Logger.Log($"配置更新 HttpServerType: {buildConfig.HttpServerType}");
                    //config.Logger.Log($"配置更新 BundleServerUrl: {buildConfig.BundleServerUrl}");
                    //config.Logger.Log($"配置更新 ShowTestLogView: {buildConfig.ShowTestLogView}");

                    File.WriteAllText(fullPath, modifiedText);
                    AssetDatabase.Refresh();
                    //config.Logger.Log("配置更新完成");

                    Status = AutoBuildTaskStatus.Completed;
                    return true;
                }
                else
                {
                    config.Logger.LogError($"配置更新错误 File not found: {fullPath}");
                    Status = AutoBuildTaskStatus.Failed;
                    return false;
                }
            }
            catch (Exception e)
            {
                config.Logger.LogError($"配置更新错误: {e.Message}");
                Status = AutoBuildTaskStatus.Failed;
                return false;
            }
        }
    }
}

