using UnityEditor.SceneManagement;
using UnityEngine;

namespace AutoBuildSystem
{
    public class ScenesModifiyTask : BaseBuildTask
    {
        public ScenesModifiyTask()
        {
            TaskId = "scenes_modify";
            TaskName = "场景文件修改";
            Priority = (int)BuildTaskPriority.ScenesModifiyTask;
            TaskType = BuildTaskType.PreBuild;
            Status = AutoBuildTaskStatus.Pending;
        }

        public override bool Execute(AutoBuildConfig config, IAutoBuildPlatform platform, IChannel channel)
        {
            try
            {
                Status = AutoBuildTaskStatus.Running;

                EditorSceneManager.OpenScene("Assets/Scenes/InitPC.unity");
                GameObject targetObject = GameObject.Find("Global");

                if (targetObject == null)
                {
                    config.Logger.LogError("未找到名为 Global 的游戏对象");
                    Status = AutoBuildTaskStatus.Failed;
                    return false;
                }

                MainEntry mainEntry = targetObject.GetComponent<MainEntry>();
                if (mainEntry == null)
                {
                    config.Logger.LogError("未在目标游戏对象上找到 MainEntry 脚本");
                    Status = AutoBuildTaskStatus.Failed;
                    return false;
                }
                
                base.LogParameter(config, BuildParameterKeys.VideoDownloadMode);
                EDownloadMode videoDownloadMode = config.GetParameter<EDownloadMode>(BuildParameterKeys.VideoDownloadMode);
                
                base.LogParameter(config, BuildParameterKeys.YooAssetPlayMode);
                YooAsset.EPlayMode yooAssetPlayMode = config.GetParameter<YooAsset.EPlayMode>(BuildParameterKeys.YooAssetPlayMode);
                
                mainEntry.YooAssetMode = yooAssetPlayMode;
                mainEntry.DownloadVideoMode = videoDownloadMode;
                //config.Logger.Log($"场景配置已修改 - YooAssetMode: {yooAssetPlayMode}, DownloadVideoMode: {videoDownloadMode}");

                EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

                Status = AutoBuildTaskStatus.Completed;
                return true;
            }
            catch (System.Exception ex)
            {
                config.Logger.LogError($"场景修改任务错误: {ex.Message}");
                Status = AutoBuildTaskStatus.Failed;
                return false;
            }
        }
    }
}

