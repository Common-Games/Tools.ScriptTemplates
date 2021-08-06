#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using JetBrains.Annotations;

#if ODIN_INSPECTOR
using Sirenix.Utilities.Editor;
#endif

using CGTK.Utilities.Extensions;

namespace CGTK.Tools.CustomScriptTemplates
{
    [Serializable]
    public static class Preferences
    {
        private const String _EDITOR_PREFS_SCOPE = Constants.PACKAGE_NAME;
        
        private static String TemplatesFolderKey => $"{_EDITOR_PREFS_SCOPE}_templates-folder-path";
        
        [PublicAPI]
        public static String TemplatesFolder
        {
            get => EditorPrefs.GetString(key: TemplatesFolderKey, defaultValue: DefaultTemplatesFolder).AppendDirectorySeparator();

            internal
            set
            {
                if (value.IsNullOrEmpty())
                {
                    throw new ArgumentNullException(paramName: nameof(value));
                }

                String __fullPath = Path.GetFullPath(value);
                
                if (__fullPath.NotValidDirectory())
                {
                    throw new ArgumentException(message: $"Templates Folder Path ({value} -> {__fullPath}) is not a valid Directory!");
                }
                
                EditorPrefs.SetString(key: TemplatesFolderKey, value);
            }
        }
        public static String DefaultTemplatesFolder => Path.GetFullPath(path: Constants.DEFAULT_SCRIPT_TEMPLATES_FOLDER);

        public static void ResetTemplatesFolder() => TemplatesFolder = DefaultTemplatesFolder;
    }
    
    public sealed class ScriptTemplatesSettingsProvider : SettingsProvider
    {
        [PublicAPI]
        public ScriptTemplatesSettingsProvider(in String path, in SettingsScope scopes, in IEnumerable<String> keywords = null) : base(path, scopes, keywords)
        { }

        public override void OnGUI(String searchContext)
        {
            const String __LABEL = "Script Templates Folder";
            
            EditorGUILayout.BeginHorizontal();
            #if ODIN_INSPECTOR
            Preferences.TemplatesFolder = SirenixEditorFields.FolderPathField(label: new GUIContent(text: __LABEL), path: Preferences.TemplatesFolder, parentPath: "Assets", absolutePath: true, useBackslashes: false);
            #else
            Preferences.TemplatesFolder = EditorGUILayout.TextField(label: __LABEL, text: Preferences.TemplatesFolder);
            #endif
            if (GUILayout.Button(text: "Reset", options: GUILayout.Width(80)))
            {
                Preferences.TemplatesFolder = Preferences.DefaultTemplatesFolder;
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(text: "Regenerate Templates"))
            {
                ScriptTemplateFactory.RemoveAll();
                ScriptTemplateFactory.CreateAll();    
            }
            
            if (GUILayout.Button(text: "Reset Templates"))
            {
                ScriptTemplateFactory.RemoveAll();
            }
            EditorGUILayout.EndHorizontal();
        }

        [SettingsProvider]
        public static SettingsProvider Create() 
            => new ScriptTemplatesSettingsProvider(path: Constants.PREFERENCE_PATH, scopes: SettingsScope.User);
    }
}

#endif
