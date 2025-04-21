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
                    base.LogParameter(config, BuildParameterKeys.ChannelCode);
                    buildConfig.ChannelCode = config.GetParameter<ChannelCodeType>(BuildParameterKeys.ChannelCode);
                    
                    base.LogParameter(config, BuildParameterKeys.HttpServerType);
                    buildConfig.HttpServerType = config.GetParameter<int>(BuildParameterKeys.HttpServerType);
                    
                    base.LogParameter(config, BuildParameterKeys.IsOnlyChapter0);
                    buildConfig.OnlyChapter0 = config.GetParameter<bool>(BuildParameterKeys.IsOnlyChapter0);
                    
                    base.LogParameter(config, BuildParameterKeys.ShowTestLogView);
                    buildConfig.ShowTestLogView = config.GetParameter<bool>(BuildParameterKeys.ShowTestLogView);
                    
                    base.LogParameter(config, BuildParameterKeys.BundleServerType);
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

                    File.WriteAllText(fullPath, modifiedText);
                    AssetDatabase.Refresh();

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

