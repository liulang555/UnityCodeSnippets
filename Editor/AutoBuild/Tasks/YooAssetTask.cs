using log4net.Repository.Hierarchy;
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
        }

        public override bool ExecuteInternal(Context context, IAutoBuildPlatform platform, IChannel channel)
        {
            try
            {
                base.LogParameter(context, BuildParameterKeys.NeedBuildBundle);
                bool needBuildBundle = context.GetParameter<bool>(BuildParameterKeys.NeedBuildBundle);
                if (!needBuildBundle)
                {
                    return true;
                }

                LogString(context,$"开始构建资源: {DateTime.Now}");

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
                    LogString(context, $"构建资源成功: {DateTime.Now}");
                    return true;
                }
                else
                {
                    context.Logger.LogError($"构建资源失败: {DateTime.Now}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"AutoBuild YooAssetTask 错误: {ex.Message}");
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
