using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AutoBuildSystem
{
    public class BuildManager
    {
        private Dictionary<AutoBuildPlatform, IAutoBuildPlatform> platforms;
        private Dictionary<ChannelCodeType, IChannel> channels;
        private IAutoBuildPlatform currentPlatform;
        private IChannel currentChannel;

        #region 初始化
        public BuildManager(Context context)
        {
            string timestr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
            context.SetParameter(BuildParameterKeys.TimeStr, timestr);
            
            this.platforms = new Dictionary<AutoBuildPlatform, IAutoBuildPlatform>();
            this.channels = new Dictionary<ChannelCodeType, IChannel>();
            AutoRegisterPlatforms();// 自动注册所有平台
            AutoRegisterChannels();// 自动注册所有渠道

            ChannelCodeType channelCode = context.GetParameter<ChannelCodeType>(BuildParameterKeys.ChannelCode);
            if (channels.ContainsKey(channelCode))
            {
                currentChannel = channels[channelCode];
                currentChannel.InitializeChannelParameters(context);
            }
            else
            {
                Debug.LogError($"Channel {channelCode} not found");
            }

            AutoBuildPlatform platform = context.GetParameter<AutoBuildPlatform>(BuildParameterKeys.BuildPlatform);
            if (platforms.ContainsKey(platform))
            {
                 currentPlatform = platforms[platform];
                 currentPlatform.InitializePlatformParameters(context, currentChannel);
                 currentPlatform.UnitySettings(context);
            }
            else
            {
                Debug.LogError($"Platform {platform} not found");
            }

            // 初始化日志记录器
            string ExprotFloderPath = context.GetParameter<string>(BuildParameterKeys.ExprotFloderPath);
            string logFileName = platform.ToString() + "_" + channelCode.ToString() + "_" + timestr + ".txt";
            string logFilePath = Path.Combine(ExprotFloderPath, logFileName);
            context.InitLogger(logFilePath);
        }
        // 自动注册所有实现了IAutoBuildPlatform接口的平台
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
        // 自动注册所有实现了IChannel接口的渠道
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
        #endregion

        /// <summary>
        /// 执行任务
        /// </summary>
        public bool ExecuteBuildTask(Context context, BuildTaskType buildTaskType)
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

                context.Logger.Log($"--------------------------------------------->{buildTaskType} 阶段<---------------------------------------------");
                int totalSeconds = 0;

                foreach (var task in selectBuildTasks)
                {
                    bool success = task.Execute(context, currentPlatform, currentChannel);
                    totalSeconds += task.BuildTimes;
                    if (!success)
                    {
                        context.Logger.LogError($"{buildTaskType} 阶段任务 {task.TaskName} 失败");
                        return false;
                    }
                }

                // 记录总耗时
                context.Logger.Log($"--------------------------------------------->{buildTaskType} 阶段总耗时: {totalSeconds} 秒<---------------------------------------------");
                return true;
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"{buildTaskType} 阶段执行失败: {ex.Message}");
                throw;
            }
        }
    }
}