using System;

namespace Presentation
{
    enum SceneName
    {
        Root,
        World,
    }

    static class SceneNameExt
    {
        internal static string ToStringName(this SceneName sceneName)
        {
            switch (sceneName)
            {
                case SceneName.Root:
                    return "RootScene";
                case SceneName.World:
                    return "WorldScene";
                default:
                    throw new ArgumentOutOfRangeException(nameof(sceneName), sceneName, null);
            }
        }
    }
}