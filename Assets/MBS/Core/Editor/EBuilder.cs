using UnityEditor;

namespace MBS
{
    [CustomEditor(typeof(MBSBuilder))]
    internal class EBuilder : Editor
    {
        private const string ALL_COMPONENTS_DISABLED = "All MBS Components disabled. To enable do Tools -> MBS -> Enable All Components";


        private MBSBuilder builder;
        private EBuilder_Inspector inspector;
        private EBuilder_SceneView sceneView;

        private void OnEnable()
        {
            Utilities.AddTagIfNotExist(DefaultConfig.TAG_WALL);

            builder = (MBSBuilder)target;
            builder.SoftInitialization();
            builder.AFTER_DESERIALIZATION = false;

            inspector = new EBuilder_Inspector();
            sceneView = new EBuilder_SceneView();
        }

        public override void OnInspectorGUI()
        {
            if (!builder.enabled) return;

            InitBuilderDeserialization();

            if (MBSConfig.Singleton.pluginDisabled)
            {
                EditorGUILayout.HelpBox(ALL_COMPONENTS_DISABLED, MessageType.Warning);
                return;
            }

            if (inspector == null)
                inspector = new EBuilder_Inspector();

            inspector.InspectorBootstrap(builder);
        }

        private void OnSceneGUI()
        {
            if (!builder.enabled) return;

            InitBuilderDeserialization();

            if (MBSConfig.Singleton.pluginDisabled)
                return;

            if (sceneView == null)
                sceneView = new EBuilder_SceneView();

            sceneView.SceneViewBootstrap(builder);
        }

        private void InitBuilderDeserialization()
        {
            if (!builder.AFTER_DESERIALIZATION) return;

            builder.HardInitialization();
            builder.AFTER_DESERIALIZATION = false;
        }
    }
}



