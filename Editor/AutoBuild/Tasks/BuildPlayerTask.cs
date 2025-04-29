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
        }

        public override bool ExecuteInternal(Context context, IAutoBuildPlatform platform, IChannel channel)
        {
            try
            {
                // 添加计时器
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                base.LogParameter(context, BuildParameterKeys.BuildOutPut);// 获取构建路径前先打印参数
                string outputPath = context.GetParameter<string>(BuildParameterKeys.BuildOutPut);
                if (string.IsNullOrEmpty(outputPath))
                {
                    context.Logger.LogError($"构建路径无效，无法继续构建");
                    return false;
                }
                // 确保输出目录存在
                YooAsset.Editor.EditorTools.CreateFileDirectory(outputPath);
                
                base.LogParameter(context, BuildParameterKeys.RemoveStartScenes);// 获取是否移除开始场景参数前先打印参数
                bool removeStartScenes = context.GetParameter<bool>(BuildParameterKeys.RemoveStartScenes);
                
                // 准备构建选项
                BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
                {
                    scenes = this.GetBuildScenes(context,removeStartScenes),
                    locationPathName = outputPath,
                    target = platform.BuildTarget,
                    options = BuildOptions.None,
                };
                // 执行构建
                LogString(context, $"开始构建 {platform.PlatformName} 应用，时间：{DateTime.Now}");
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
                    LogString(context, $"{platform.PlatformName} 构建成功，耗时：{formattedTime}，完成时间：{DateTime.Now}");
                    
                    // 记录构建结果信息
                    context.SetParameter(BuildParameterKeys.BuildSuccess, true);
                    context.SetParameter(BuildParameterKeys.BuildCompletionTime, DateTime.Now);
                    
                    return true;
                }
                else
                {
                    context.Logger.LogError($"{platform.PlatformName} 构建失败，耗时：{formattedTime}，错误数量：{report.summary.totalErrors}");
                    
                    // 记录构建失败信息
                    context.SetParameter(BuildParameterKeys.BuildSuccess, false);
                    context.SetParameter(BuildParameterKeys.BuildErrorCount, report.summary.totalErrors);
                    
                    return false;
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"构建过程发生异常：{ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }
        
        public string[] GetBuildScenes(Context context,bool _removeStartScenes)
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
                    LogString(context, "添加场景: " + scene.path);
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