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

        public override void UnitySettings(Context context)
        {
           
        }
        public override void InitializePlatformParameters(Context context, IChannel channel)
        {
            string timestr = context.GetParameter<string>(BuildParameterKeys.TimeStr);
            string outputPath = context.GetParameter<string>(BuildParameterKeys.OutputPath);
            string exprotFloderPath = Path.Combine(outputPath, AutoBuildPlatform.ToString() + "_" + channel.ChannelId.ToString());
            string windwosFloderPath = Path.Combine(exprotFloderPath, $"{channel.ProductName}_{timestr}");
            string exePath = Path.Combine(windwosFloderPath, $"{channel.ProductName}.exe");

            context.SetParameter(BuildParameterKeys.ExprotFloderPath, exprotFloderPath);
            context.SetParameter(BuildParameterKeys.WindwosFloderPath, windwosFloderPath);
            context.SetParameter(BuildParameterKeys.WindwosExePath, exePath);
            context.SetParameter(BuildParameterKeys.BuildOutPut, exePath);
            context.SetParameter(BuildParameterKeys.OpenDirectory, Path.GetDirectoryName(exePath));
        }
    }
} 