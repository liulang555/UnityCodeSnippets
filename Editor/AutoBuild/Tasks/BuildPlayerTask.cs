using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace AutoBuildSystem
{
    public class BuildPlayerTask : BaseBuildTask
    {
        public BuildPlayerTask()
        {
            TaskId = "build_player";
            TaskName = "构建应用程序";
            Priority = (int)BuildTaskPriority.BuildPlayerTask;
            TaskType = BuildTaskType.Build;
            Status = AutoBuildTaskStatus.Pending;
        }

        public override bool Execute(AutoBuildConfig autoBuildConfig, IAutoBuildPlatform platform, IChannel channel)
        {
            try
            {
                Status = AutoBuildTaskStatus.Running;
                
                // 添加计时器
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                // 获取构建路径
                string outputPath = autoBuildConfig.GetParameter<string>(BuildParameterKeys.BuildOutPut);
                if (string.IsNullOrEmpty(outputPath))
                {
                    autoBuildConfig.Logger.LogError($"构建路径无效，无法继续构建");
                    Status = AutoBuildTaskStatus.Failed;
                    return false;
                }
                // 确保输出目录存在
                YooAsset.Editor.EditorTools.CreateFileDirectory(outputPath);
                bool removeStartScenes = autoBuildConfig.GetParameter<bool>(BuildParameterKeys.RemoveStartScenes);
                // 准备构建选项
                BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
                {
                    scenes = this.GetBuildScenes(removeStartScenes,autoBuildConfig.Logger),
                    locationPathName = outputPath,
                    target = platform.BuildTarget,
                    options = BuildOptions.None,
                };
                
                // 执行构建
                autoBuildConfig.Logger.Log($"开始构建 {platform.PlatformName} 应用，输出路径：{outputPath}，时间：{DateTime.Now}");
                BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
                
                // 停止计时器
                stopwatch.Stop();
                TimeSpan buildTime = stopwatch.Elapsed;
                
                // 格式化时间为易读字符串
                string formattedTime;
                if (buildTime.TotalHours >= 1)
                {
                    formattedTime = $"{buildTime.Hours}小时{buildTime.Minutes}分{buildTime.Seconds}秒";
                }
                else if (buildTime.TotalMinutes >= 1)
                {
                    formattedTime = $"{buildTime.Minutes}分{buildTime.Seconds}秒";
                }
                else
                {
                    formattedTime = $"{buildTime.Seconds}.{buildTime.Milliseconds}秒";
                }
                
                // 处理构建结果
                if (report.summary.result == BuildResult.Succeeded)
                {
                    autoBuildConfig.Logger.Log($"{platform.PlatformName} 构建成功，耗时：{formattedTime}，完成时间：{DateTime.Now}");
                    
                    // 记录构建结果信息
                    autoBuildConfig.SetParameter(BuildParameterKeys.BuildSuccess, true);
                    autoBuildConfig.SetParameter(BuildParameterKeys.BuildCompletionTime, DateTime.Now);
                    
                    Status = AutoBuildTaskStatus.Completed;
                    return true;
                }
                else
                {
                    autoBuildConfig.Logger.LogError($"{platform.PlatformName} 构建失败，耗时：{formattedTime}，错误数量：{report.summary.totalErrors}");
                    
                    // 记录构建失败信息
                    autoBuildConfig.SetParameter(BuildParameterKeys.BuildSuccess, false);
                    autoBuildConfig.SetParameter(BuildParameterKeys.BuildErrorCount, report.summary.totalErrors);
                    
                    Status = AutoBuildTaskStatus.Failed;
                    return false;
                }
            }
            catch (Exception ex)
            {
                autoBuildConfig.Logger.LogError($"构建过程发生异常：{ex.Message}\n{ex.StackTrace}");
                Status = AutoBuildTaskStatus.Failed;
                return false;
            }
        }
        
        // 删除单独的 FormatTimeSpan 方法，已合并到 Execute 方法中
        
        public string[] GetBuildScenes(bool _removeStartScenes, BuildLogger Logger)
        {
            // 获取场景列表
            List<string> levels = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (!scene.enabled) continue;

#if UNITY_EDITOR_OSX
            string path =  Path.Combine(Application.dataPath.Replace("Assets", ""), scene.path).Replace("\\", "/");
#else
                string path = Path.Combine(Application.dataPath.Replace("Assets", ""), scene.path).Replace("/", "\\");
#endif
                if (File.Exists(path))
                {
                    if (_removeStartScenes && scene.path.EndsWith("StartGame.unity"))
                    {
                        continue;
                    }
                    levels.Add(scene.path);
                    Logger.Log("AutoBuild 添加打包场景: " + scene.path);
                }
                else
                {
                    continue;
                }
            }
            return levels.ToArray();
        }
    }
}