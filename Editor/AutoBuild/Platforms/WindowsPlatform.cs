using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using UnityEditor.Build.Reporting;

namespace AutoBuildSystem
{
    public class WindowsPlatform : BasePlatform
    {
        public WindowsPlatform()
        {
            PlatformName = "Windows";
            AutoBuildPlatform = AutoBuildPlatform.Windows;
            BuildTarget = BuildTarget.StandaloneWindows;
        }

        public override List<IBuildTask> GetPlatformTasks()
        {
            return new List<IBuildTask>
            {
                new ModifyScriptingDefineSymbolsTask(),
                new AutoHybridCLRTask(),
                new YooAssetTask(),
                new ScenesModifiyTask(),
                new ModifyBuildConfigTask(),
                new BuildPlayerTask(),
                new OpenBuildDirectoryTask(),
            };
        }
        public override HashSet<string> GetScriptingDefines()
        {
            return new HashSet<string> { "DOTWEEN", "UseHybridCLR" };
        }

        public override void UnitySettings(AutoBuildConfig autoBuildConfig)
        {
           
        }
        public override void InitializePlatformParameters(AutoBuildConfig autoBuildConfig, IChannel channel)
        {
            string timestr = autoBuildConfig.GetParameter<string>(BuildParameterKeys.TimeStr);
            string outputPath = autoBuildConfig.GetParameter<string>(BuildParameterKeys.OutputPath);
            string exprotFloderPath = Path.Combine(outputPath, AutoBuildPlatform.ToString() + "_" + channel.ChannelId.ToString());
            string windwosFloderPath = Path.Combine(exprotFloderPath, $"{channel.ProductName}_{timestr}");
            string exePath = Path.Combine(windwosFloderPath, $"{channel.ProductName}.exe");

            autoBuildConfig.SetParameter(BuildParameterKeys.ExprotFloderPath, exprotFloderPath);
            autoBuildConfig.SetParameter(BuildParameterKeys.WindwosFloderPath, windwosFloderPath);
            autoBuildConfig.SetParameter(BuildParameterKeys.WindwosExePath, exePath);
            autoBuildConfig.SetParameter(BuildParameterKeys.BuildOutPut, exePath);
            autoBuildConfig.SetParameter(BuildParameterKeys.OpenDirectory, Path.GetDirectoryName(exePath));
        }
    }
} 