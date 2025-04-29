using UnityEditor;
using UnityEngine;

namespace AutoBuildSystem
{
    public class VersionAddTask : BaseBuildTask
    {
        public VersionAddTask()
        {
            TaskId = "version_add";
            TaskName = "自动增加 版本号";
            Priority = (int)BuildTaskPriority.VersionAddTask;
            TaskType = BuildTaskType.PreBuild;
        }
        public override bool ExecuteInternal(Context context, IAutoBuildPlatform platform, IChannel channel)
        {
            // 获取当前版本信息
            var productVersion = context.GetParameter<string>(BuildParameterKeys.ProductVersion);
            int versionCode = context.GetParameter<int>(BuildParameterKeys.VersionCode);
            // 版本号自增
            versionCode++;
            context.SetParameter(BuildParameterKeys.VersionCode, versionCode);
            // 自动增加版本号逻辑
            if (!string.IsNullOrEmpty(productVersion))
            {
                var versionParts = productVersion.Split('.');
                if (versionParts.Length == 3)
                {
                    productVersion = $"{versionParts[0]}.{versionParts[1]}.{versionCode}";
                    context.SetParameter(BuildParameterKeys.ProductVersion, productVersion);
                }
            }
            // 应用平台设置
            switch (platform.AutoBuildPlatform)
            {
                case AutoBuildPlatform.Android_Apk:
                case AutoBuildPlatform.Android_AAB:
                    PlayerSettings.Android.bundleVersionCode = versionCode;
                    break;
                case AutoBuildPlatform.IOS:
                    PlayerSettings.iOS.buildNumber = versionCode.ToString();
                    break;
            }
            PlayerSettings.productName = context.GetParameter<string>(BuildParameterKeys.ProductName);
            PlayerSettings.bundleVersion = productVersion;
            context.Logger.Log($"版本号更新成功！当前版本: {productVersion} ({versionCode})");
            return true;
        }
    }
}

