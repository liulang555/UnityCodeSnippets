using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace AutoBuildSystem
{
    public abstract class BasePlatform : IAutoBuildPlatform
    {
        public string PlatformName { get; protected set; }
        public AutoBuildPlatform AutoBuildPlatform { get;protected set; }
        public abstract List<IBuildTask> GetPlatformTasks();
        public abstract HashSet<string> GetScriptingDefines();
        public abstract void UnitySettings(AutoBuildConfig autoBuildConfig);
        public abstract void InitializePlatformParameters(AutoBuildConfig autoBuildConfig, IChannel channel);

        #region BuildTarget
        public BuildTarget BuildTarget { get; protected set; }
        public BuildTargetGroup GetTargetGroup()
        {
            return BuildPipeline.GetBuildTargetGroup(BuildTarget);
        }
        public NamedBuildTarget GetNamedBuildTarget()
        {
            return NamedBuildTarget.FromBuildTargetGroup(GetTargetGroup());
        }
        #endregion
    }
}