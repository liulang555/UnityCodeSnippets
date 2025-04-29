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
        }

        public override bool ExecuteInternal(Context context, IAutoBuildPlatform platform, IChannel channel)
        {
            try
            {
                EditorSceneManager.OpenScene("Assets/Scenes/InitPC.unity");
                GameObject targetObject = GameObject.Find("Global");

                if (targetObject == null)
                {
                    context.Logger.LogError("未找到名为 Global 的游戏对象");
                    return false;
                }

                MainEntry mainEntry = targetObject.GetComponent<MainEntry>();
                if (mainEntry == null)
                {
                    context.Logger.LogError("未在目标游戏对象上找到 MainEntry 脚本");
                    return false;
                }
                
                base.LogParameter(context, BuildParameterKeys.VideoDownloadMode);
                EDownloadMode videoDownloadMode = context.GetParameter<EDownloadMode>(BuildParameterKeys.VideoDownloadMode);
                
                base.LogParameter(context, BuildParameterKeys.YooAssetPlayMode);
                YooAsset.EPlayMode yooAssetPlayMode = context.GetParameter<YooAsset.EPlayMode>(BuildParameterKeys.YooAssetPlayMode);
                
                mainEntry.YooAssetMode = yooAssetPlayMode;
                mainEntry.DownloadVideoMode = videoDownloadMode;

                EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
                return true;
            }
            catch (System.Exception ex)
            {
                context.Logger.LogError($"场景修改任务错误: {ex.Message}");
                return false;
            }
        }
    }
}

