using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AutoBuildSystem
{
    public class BuildManager
    {
        private Dictionary<AutoBuildPlatform, IAutoBuildPlatform> platforms;
        private Dictionary<ChannelCodeType, IChannel> channels;
        private IAutoBuildPlatform currentPlatform;
        private IChannel currentChannel;
        public BuildManager(AutoBuildConfig autoBuildConfig)
        {
            string timestr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
            autoBuildConfig.SetParameter(BuildParameterKeys.TimeStr, timestr);
            
            this.platforms = new Dictionary<AutoBuildPlatform, IAutoBuildPlatform>();
            this.channels = new Dictionary<ChannelCodeType, IChannel>();
            AutoRegisterPlatforms();// 自动注册所有平台
            AutoRegisterChannels();// 自动注册所有渠道

            ChannelCodeType channelCode = autoBuildConfig.GetParameter<ChannelCodeType>(BuildParameterKeys.ChannelCode);
            if (channels.ContainsKey(channelCode))
            {
                currentChannel = channels[channelCode];
                currentChannel.InitializeChannelParameters(autoBuildConfig);
            }
            else
            {
                Debug.LogError($"Channel {channelCode} not found");
            }

            AutoBuildPlatform platform = autoBuildConfig.GetParameter<AutoBuildPlatform>(BuildParameterKeys.BuildPlatform);
            if (platforms.ContainsKey(platform))
            {
                 currentPlatform = platforms[platform];
                 currentPlatform.InitializePlatformParameters(autoBuildConfig, currentChannel);
                 currentPlatform.UnitySettings(autoBuildConfig);
            }
            else
            {
                Debug.LogError($"Platform {platform} not found");
            }

            // 初始化日志记录器
            string ExprotFloderPath = autoBuildConfig.GetParameter<string>(BuildParameterKeys.ExprotFloderPath);
            string logFileName = platform.ToString() + "_" + channelCode.ToString() + "_" + timestr + ".txt";
            string logFilePath = Path.Combine(ExprotFloderPath, logFileName);
            autoBuildConfig.InitLogger(logFilePath);
        }
        
        /// <summary>
        /// 自动注册所有实现了IAutoBuildPlatform接口的平台
        /// </summary>
        private void AutoRegisterPlatforms()
        {
            // 获取当前程序集中所有实现了IAutoBuildPlatform接口的非抽象类
            var platformTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IAutoBuildPlatform).IsAssignableFrom(t) && 
                       !t.IsInterface && !t.IsAbstract);
                       
            foreach (var type in platformTypes)
            {
                try
                {
                    var platform = (IAutoBuildPlatform)Activator.CreateInstance(type);
                    if (!platforms.ContainsKey(platform.AutoBuildPlatform))
                    {
                        platforms[platform.AutoBuildPlatform] = platform;
                        Debug.Log($"自动注册平台: {platform.AutoBuildPlatform}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"注册平台 {type.Name} 失败: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// 自动注册所有实现了IChannel接口的渠道
        /// </summary>
        private void AutoRegisterChannels()
        {
            // 获取当前程序集中所有实现了IChannel接口的非抽象类
            var channelTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IChannel).IsAssignableFrom(t) && 
                       !t.IsInterface && !t.IsAbstract);
                       
            foreach (var type in channelTypes)
            {
                try
                {
                    var channel = (IChannel)Activator.CreateInstance(type);
                    if (!channels.ContainsKey(channel.ChannelId))
                    {
                        channels[channel.ChannelId] = channel;
                        Debug.Log($"自动注册渠道: {channel.ChannelId}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"注册渠道 {type.Name} 失败: {ex.Message}");
                }
            }
        }
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="autoBuildConfig"></param>
        /// <param name="buildTaskType"></param>
        /// <returns></returns>
        public bool ExecuteBuildTask(AutoBuildConfig autoBuildConfig, BuildTaskType buildTaskType)
        {
            try
            {
                // 获取当前类型的任务
                var allTasks = new List<IBuildTask>();
                allTasks.AddRange(currentPlatform.GetPlatformTasks());
                allTasks.AddRange(currentChannel.GetChannelTasks());
                var selectBuildTasks = allTasks.Where(t => t.TaskType == buildTaskType)
                                         .OrderBy(t => t.Priority)
                                         .ToList();

                autoBuildConfig.Logger.Log($"-------------------->{buildTaskType} 阶段<-----------------");
                int totalSeconds = 0; // 添加总计时
                foreach (var task in selectBuildTasks)
                {
                    try
                    {
                        // 使用分隔线使日志更清晰
                        autoBuildConfig.Logger.Log($"执行  -- {task.TaskName} --");
                        if (task.Status == AutoBuildTaskStatus.Running)
                        {
                            autoBuildConfig.Logger.LogWarning($"任务 {task.TaskName} 已在运行中");
                            return false;
                        }
                        // 使用 Stopwatch 进行精确计时
                        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                        bool success = task.Execute(autoBuildConfig, currentPlatform, currentChannel);
                        stopwatch.Stop();
                        int seconds = (int)(stopwatch.ElapsedMilliseconds / 1000f);   // 计算耗时（秒）
                        totalSeconds += seconds;
                        if (success)
                        {
                            autoBuildConfig.Logger.Log($"成功 {task.TaskName}  (耗时: {seconds} 秒)");
                        }
                        else
                        {
                            autoBuildConfig.Logger.LogError($"失败 {task.TaskName}  (耗时: {seconds} 秒)");
                            autoBuildConfig.Logger.LogError($"{buildTaskType} 阶段任务 {task.TaskName} 失败");
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        autoBuildConfig.Logger.LogError($"任务 {task.TaskName} 执行出错: {ex.Message}\n{ex.StackTrace}");
                        return false;
                    }
                }
                // 记录总耗时
                autoBuildConfig.Logger.Log($"--------------------------------------------->{buildTaskType} 阶段总耗时: {totalSeconds} 秒");
                return true;
            }
            catch (Exception ex)
            {
                autoBuildConfig.Logger.LogError($"{buildTaskType} 阶段执行失败: {ex.Message}");
                throw;
            }
        }
    }
}