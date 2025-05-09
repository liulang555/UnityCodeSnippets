using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System;
using System.IO;

namespace AutoBuildSystem
{
    public class IosPlatform : BasePlatform
    {
        public IosPlatform()
        {
            PlatformName = "IOS";
            AutoBuildPlatform = AutoBuildPlatform.IOS;
            BuildTarget = BuildTarget.iOS;
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
            //EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;
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
