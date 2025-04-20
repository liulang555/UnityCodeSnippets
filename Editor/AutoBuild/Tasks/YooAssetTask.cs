using System;
using UnityEditor;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;

namespace AutoBuildSystem
{
    public class YooAssetTask : BaseBuildTask
    {
        public YooAssetTask()
        {
            TaskId = "yoo_asset";
            TaskName = "YooAsset 打包Bundle资源";
            Priority = (int)BuildTaskPriority.YooAssetTask;
            TaskType = BuildTaskType.PreBuild;
            Status = AutoBuildTaskStatus.Pending;
        }

        public override bool Execute(AutoBuildConfig config, IAutoBuildPlatform platform, IChannel channel)
        {
            try
            {
                Status = AutoBuildTaskStatus.Running;

                bool needBuildBundle = config.GetParameter<bool>(BuildParameterKeys.NeedBuildBundle);
                if (!needBuildBundle)
                {
                    Status = AutoBuildTaskStatus.Completed;
                    return true;
                }

                config.Logger.Log($"AutoBuild YooAssetTask 开始构建资源: {DateTime.Now}");

                var buildoutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
                var streamingAssetsRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();

                // 构建参数
                BuiltinBuildParameters buildParameters = new BuiltinBuildParameters
                {
                    BuildOutputRoot = buildoutputRoot,
                    BuildinFileRoot = streamingAssetsRoot,
                    BuildPipeline = EBuildPipeline.BuiltinBuildPipeline.ToString(),
                    BuildTarget = platform.BuildTarget,
                    BuildMode = EBuildMode.ForceRebuild,
                    PackageName = "DefaultPackage",
                    PackageVersion = "1.0",
                    EnableSharePackRule = true,
                    VerifyBuildingResult = true,
                    FileNameStyle = EFileNameStyle.BundleName_HashName,
                    BuildinFileCopyOption = EBuildinFileCopyOption.ClearAndCopyAll,
                    BuildinFileCopyParams = string.Empty,
                    EncryptionServices = CreateEncryptionInstance(),
                    CompressOption = ECompressOption.LZ4
                };

                // 执行构建
                BuiltinBuildPipeline pipeline = new BuiltinBuildPipeline();
                var buildResult = pipeline.Run(buildParameters, true);

                if (buildResult.Success)
                {
                    config.Logger.Log($"AutoBuild YooAssetTask 构建资源成功: {DateTime.Now}");
                    Status = AutoBuildTaskStatus.Completed;
                    return true;
                }
                else
                {
                    config.Logger.LogError($"AutoBuild YooAssetTask 构建资源失败: {DateTime.Now}");
                    Status = AutoBuildTaskStatus.Failed;
                    return false;
                }
            }
            catch (Exception ex)
            {
                config.Logger.LogError($"AutoBuild YooAssetTask 错误: {ex.Message}");
                Status = AutoBuildTaskStatus.Failed;
                return false;
            }
        }

        /// <summary>
        /// 创建加密类实例
        /// </summary>
        private static IEncryptionServices CreateEncryptionInstance()
        {
            var encyptionClassName = AssetBundleBuilderSetting.GetPackageEncyptionClassName("DefaultPackage", EBuildPipeline.BuiltinBuildPipeline);
            var encryptionClassTypes = EditorTools.GetAssignableTypes(typeof(IEncryptionServices));
            var classType = encryptionClassTypes.Find(x => x.FullName.Equals(encyptionClassName));
            if (classType != null)
                return (IEncryptionServices)Activator.CreateInstance(classType);
            else
                return null;
        }
    }
}
