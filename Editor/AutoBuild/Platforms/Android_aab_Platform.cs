using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.Build.Reporting;
using System.IO;

namespace AutoBuildSystem
{
    public class Android_aab_Platform : BasePlatform
    {
        public Android_aab_Platform()
        {
            PlatformName = "Android_AAB";
            AutoBuildPlatform = AutoBuildPlatform.Android_AAB;
            BuildTarget = BuildTarget.Android;
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
            return new HashSet<string> { "DISABLESTEAMWORKS", "DOTWEEN", "UseHybridCLR" };
        }

        public override void UnitySettings(AutoBuildConfig autoBuildConfig)
        {
            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;
            PlayerSettings.Android.ARCoreEnabled = false;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
            EditorUserBuildSettings.buildAppBundle = true;
            PlayerSettings.Android.splitApplicationBinary = true;
        }
        public override void InitializePlatformParameters(AutoBuildConfig autoBuildConfig, IChannel channel)
        {
            string outputPath = autoBuildConfig.GetParameter<string>(BuildParameterKeys.OutputPath);
            string exprotFloderPath = Path.Combine(outputPath, AutoBuildPlatform.ToString() + "_" + channel.ChannelId.ToString());
            autoBuildConfig.SetParameter(BuildParameterKeys.ExprotFloderPath, exprotFloderPath);
            autoBuildConfig.SetParameter(BuildParameterKeys.BuildOutPut, exprotFloderPath);
            autoBuildConfig.SetParameter(BuildParameterKeys.OpenDirectory, exprotFloderPath);
        }
    }
}
