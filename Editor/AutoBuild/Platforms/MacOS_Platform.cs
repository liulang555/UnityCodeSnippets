using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System;
using System.IO;

namespace AutoBuildSystem
{
    public class MacOS_Platform : BasePlatform
    {
        public MacOS_Platform()
        {
            PlatformName = "MacOS";
            AutoBuildPlatform = AutoBuildPlatform.MacOs;
            BuildTarget = BuildTarget.StandaloneOSX;
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
            return new HashSet<string> {  "DOTWEEN", "UseHybridCLR" };
        }

        public override void UnitySettings(Context context)
        {
#if UNITY_EDITOR_OSX
             UnityEditor.OSXStandalone.UserBuildSettings.createXcodeProject = true;
#endif
        }
        public override void InitializePlatformParameters(Context context, IChannel channel)
        {
            string outputPath = context.GetParameter<string>(BuildParameterKeys.OutputPath);
            string exprotFloderPath = Path.Combine(outputPath, AutoBuildPlatform.ToString() + "_" + channel.ChannelId.ToString());
            context.SetParameter(BuildParameterKeys.ExprotFloderPath, exprotFloderPath);
            context.SetParameter(BuildParameterKeys.BuildOutPut, exprotFloderPath);
            context.SetParameter(BuildParameterKeys.OpenDirectory, exprotFloderPath);
        }
    }
}

