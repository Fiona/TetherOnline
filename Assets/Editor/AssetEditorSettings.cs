using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace TetherOnline
{
    class AssetEditorSettings : ScriptableObject
    {
        public const string settingsAssetPath = "Assets/Editor/AssetEditorSettings.asset";

        [SerializeField]
        private string sharedAssetsLocalPath;

        internal static AssetEditorSettings GetSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<AssetEditorSettings>(settingsAssetPath);
            if(settings != null)
                return settings;

            settings = CreateInstance<AssetEditorSettings>();
            settings.sharedAssetsLocalPath = "";
            AssetDatabase.CreateAsset(settings, settingsAssetPath);
            AssetDatabase.SaveAssets();
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetSettings());
        }
    }

    static class AssetEditorSettingsRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateAssetEditorSettingsRegister()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider("Project/ProjectAssets", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = "Tether Online - Project Assets",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    var settings = AssetEditorSettings.GetSerializedSettings();
                    EditorGUILayout.PropertyField(settings.FindProperty("sharedAssetsLocalPath"), new GUIContent("Local path of shared assets"));
                    settings.ApplyModifiedPropertiesWithoutUndo();
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Shared Assets", "Local Path" })
            };

            return provider;
        }
    }

}