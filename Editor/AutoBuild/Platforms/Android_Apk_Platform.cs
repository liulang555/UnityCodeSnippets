using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System;
using System.IO;

namespace AutoBuildSystem
{
    public class Android_Apk_Platform : BasePlatform
    {
        public Android_Apk_Platform()
        {
            PlatformName = "Android_Apk";
            AutoBuildPlatform = AutoBuildPlatform.Android_Apk;
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
                new VersionAddTask(),
            };
        }

        public override HashSet<string> GetScriptingDefines()
        {
            return new HashSet<string> { "DISABLESTEAMWORKS", "DOTWEEN", "UseHybridCLR" };
        }

        public override void UnitySettings(Context context)
        {
            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;
            PlayerSettings.Android.ARCoreEnabled = false;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
            EditorUserBuildSettings.buildAppBundle = false;
            PlayerSettings.Android.splitApplicationBinary = false;
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