using UnityEngine;
using MessagePack;
using MessagePack.Resolvers;

namespace MyMessagePackExt
{
    public class MyMessagePackInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            StaticCompositeResolver.Instance.Register(
                // Resolvers.GeneratedResolver.Instance,
                StandardResolver.Instance,
                MessagePack.Unity.UnityResolver.Instance,
                MessagePack.Unity.Extension.UnityBlitWithPrimitiveArrayResolver.Instance
            );

            var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);

            MessagePackSerializer.DefaultOptions = option;
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void EditorInitialize()
        {
            Initialize();
        }
#endif
    }
}
