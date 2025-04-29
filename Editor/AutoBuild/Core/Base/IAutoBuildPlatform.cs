using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace AutoBuildSystem
{
    public interface IAutoBuildPlatform
    {
        public string PlatformName { get; }
        AutoBuildPlatform AutoBuildPlatform { get; }
        /// <summary>
        /// 当前平台需要的任务
        /// </summary>
        /// <returns></returns>
        List<IBuildTask> GetPlatformTasks();
        /// <summary>
        /// 当前平台的设置
        /// </summary>
        /// <param name="config"></param>
        void UnitySettings(Context context);
        HashSet<string> GetScriptingDefines();
        /// <summary>
        /// 设置平台特定的参数到共享参数字典
        /// </summary>
        /// <param name="context">构建配置</param>
        void InitializePlatformParameters(Context context, IChannel channel);

        #region BuildTarget
        /// <summary>
        /// 当前的硬件平台类型
        /// </summary>
        BuildTarget BuildTarget { get; }
        /// <summary>
        /// 获取对应的 BuildTargetGroup
        /// </summary>
        /// <returns></returns>
        BuildTargetGroup GetTargetGroup();
        /// <summary>
        /// 获取对应的 NamedBuildTarget
        /// </summary>
        /// <returns></returns>
        NamedBuildTarget GetNamedBuildTarget();
        #endregion
    }
}