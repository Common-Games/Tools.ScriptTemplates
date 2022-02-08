//#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using JetBrains.Annotations;

#if ODIN_INSPECTOR
using Sirenix.Utilities.Editor;
#endif

using CGTK.Utils.Extensions;

namespace CGTK.Tools.CustomScriptTemplates
{
    [Serializable]
    internal static class Preferences
    {
        private const String _EDITOR_PREFS_SCOPE = PackageConstants.PACKAGE_NAME;
        
        private static String TemplatesFolderKey => $"{_EDITOR_PREFS_SCOPE}_templates-folder-path";
        
        //TODO: Red icon or text if the path is invalid. (check on set)
        
        [PublicAPI]
        public static String TemplatesFolder
        {
            get => PlayerPrefs.GetString(key: TemplatesFolderKey, defaultValue: DefaultTemplatesFolder).AppendDirectorySeparator();

            internal
            set
            {
                if (value.IsNullOrEmpty())
                {
                    //throw new ArgumentNullException(paramName: nameof(value)); //Had to comment out because if you throw an exception the editor won't show.
                    Debug.LogError(message: $"Argument Null: {nameof(value)}");
                    return;
                }

                String __fullPath = Path.GetFullPath(value);
                
                if (__fullPath.NotValidDirectory())
                {
                    //throw new ArgumentException(message: $"Templates Folder Path ({value} -> {__fullPath}) is not a valid Directory!"); //Had to comment out because if you throw an exception the editor won't show.
                    Debug.LogError(message: $"Templates Folder Path ({value} -> {__fullPath}) is not a valid Directory!");
                    return;
                }
                
                PlayerPrefs.SetString(key: TemplatesFolderKey, value);
            }
        }
        public static String DefaultTemplatesFolder => Path.GetFullPath(path: PackageConstants.DEFAULT_SCRIPT_TEMPLATES_FOLDER);

        public static void ResetTemplatesFolder() => TemplatesFolder = DefaultTemplatesFolder;
    }
    
    public sealed class ScriptTemplatesSettingsProvider : SettingsProvider
    {
        [PublicAPI]
        public ScriptTemplatesSettingsProvider(in String path, in SettingsScope scopes, in IEnumerable<String> keywords = null) : base(path, scopes, keywords)
        { }
        
        private static readonly GUIStyle ButtonStyle = new GUIStyle(GUI.skin.button)
        {
            fixedHeight = 18,
            fixedWidth  = 18,
            padding = new RectOffset(left: 0, right: 0, top: 0, bottom: 0)
        };

        public override void OnGUI(String searchContext)
        {
            const String __LABEL = "Script Templates Folder";
            
            EditorGUILayout.BeginHorizontal();
            {
                #if ODIN_INSPECTOR
                Preferences.TemplatesFolder = SirenixEditorFields.FolderPathField(label: new GUIContent(text: __LABEL),
                    path: Preferences.TemplatesFolder, parentPath: "Assets", absolutePath: true, useBackslashes: false);
                #else
                Preferences.TemplatesFolder = EditorGUILayout.TextField(label: __LABEL, text: Preferences.TemplatesFolder);
                #endif
                
                if (GUILayout.Button(content: EditorGUIUtility.IconContent(name: "d_Refresh", text: "Reset"), ButtonStyle))
                {
                    Preferences.TemplatesFolder = Preferences.DefaultTemplatesFolder;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(text: "Regenerate Templates"))
                {
                    ScriptTemplateFactory.Reset();
                    ScriptTemplateFactory.Regenerate();
                }

                if (GUILayout.Button(text: "Reset Templates"))
                {
                    ScriptTemplateFactory.Reset();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        [SettingsProvider]
        public static SettingsProvider Create() 
            => new ScriptTemplatesSettingsProvider(path: PackageConstants.PREFERENCE_PATH, scopes: SettingsScope.User);
    }
}
//#endif