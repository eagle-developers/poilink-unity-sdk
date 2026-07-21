using UnityEditor;
using UnityEngine;

namespace Poilink.Editor
{
    public class PoilinkSDKSettingsProvider : SettingsProvider
    {
        private const string SettingsPath = "Project/Poilink SDK";

        private PoilinkSDKSettingsProvider(string path, SettingsScope scopes = SettingsScope.Project)
            : base(path, scopes)
        {
        }

        public override void OnGUI(string searchContext)
        {
            GUILayout.Label("Poilink SDK Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "Poilink SDK configuration.\n\n" +
                "For Android: Set meta-data in AndroidManifest.xml\n" +
                "For iOS: Set values in Info.plist",
                MessageType.Info
            );

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Required Settings:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("• CLIENT_ID");
            EditorGUILayout.LabelField("• CLIENT_SECRET");
            EditorGUILayout.LabelField("• BASE_URL");

            EditorGUILayout.Space();
            if (GUILayout.Button("Open Documentation")) Application.OpenURL("https://docs.poilink.com/unity");
        }

        [SettingsProvider]
        public static SettingsProvider CreatePoilinkSDKSettingsProvider()
        {
            return new PoilinkSDKSettingsProvider(SettingsPath)
            {
                keywords = new[] { "Poilink", "SDK", "Authentication" }
            };
        }
    }
}
