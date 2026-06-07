using UnityEditor;
using UnityEngine;

namespace Poilink.Editor
{
    public static class PoilinkSettingsEditor
    {
        private const string SettingsPath = "Assets/Resources/PoilinkSettings.asset";
        private const string ResourcesFolderPath = "Assets/Resources";

        public static void CreateSettingsAsset()
        {
            if (!AssetDatabase.IsValidFolder(ResourcesFolderPath)) AssetDatabase.CreateFolder("Assets", "Resources");

            var existingSettings = AssetDatabase.LoadAssetAtPath<PoilinkSettings>(SettingsPath);
            if (existingSettings != null)
            {
                Selection.activeObject = existingSettings;
                EditorGUIUtility.PingObject(existingSettings);
                Debug.Log("[PoilinkSDK] Settings asset already exists at: " + SettingsPath);
                return;
            }

            var settings = ScriptableObject.CreateInstance<PoilinkSettings>();
            AssetDatabase.CreateAsset(settings, SettingsPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = settings;
            EditorGUIUtility.PingObject(settings);

            Debug.Log("[PoilinkSDK] Settings asset created at: " + SettingsPath);
        }

        [MenuItem("Window/Poilink Settings", priority = 2000)]
        public static void OpenSettings()
        {
            var settings = Resources.Load<PoilinkSettings>("PoilinkSettings");

            if (settings == null)
            {
                if (EditorUtility.DisplayDialog(
                        "Poilink Settings Not Found",
                        "PoilinkSettings asset does not exist. Do you want to create it now?",
                        "Create",
                        "Cancel"))
                    CreateSettingsAsset();
                return;
            }

            Selection.activeObject = settings;
            EditorGUIUtility.PingObject(settings);
        }
    }
}
