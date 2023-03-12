using System;

namespace ACv3.Presentation
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
            }

            throw new Exception("実装漏れ");
        }
    }
}