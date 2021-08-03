#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using CGTK.Utilities.Extensions;
using UnityEditor;
using UnityEngine;

using JetBrains.Annotations;
using Sirenix.OdinInspector;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.Utilities.Editor;
#endif

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
                String __input = value;
                
                #if ODIN_INSPECTOR
                if (UseRelativePath)
                {
                    //__input = Path.GetFullPath(path: value.IsNullOrEmpty() ? "Assets/" + value : value);
                    __input = value.IsNullOrEmpty() ? $"Assets/{value}" : value;
                }
                else
                {
                    __input = Path.GetFullPath(__input);
                }
                #endif

                if (__input.IsNullOrEmpty())
                {
                    throw new ArgumentNullException(paramName: nameof(value));
                }

                if (Path.GetFullPath(__input).NotValidPath())
                {
                    //throw new ArgumentException(message: "Templates Folder Path is not a valid Folder!");
                }
                
                EditorPrefs.SetString(key: TemplatesFolderKey, __input);
            }
        }
        public static String DefaultTemplatesFolder =>
            #if ODIN_INSPECTOR
            UseRelativePath ? Constants.DEFAULT_SCRIPT_TEMPLATES_FOLDER :
            #endif
            Path.GetFullPath(path: Constants.DEFAULT_SCRIPT_TEMPLATES_FOLDER);

        public static void ResetTemplatesFolder() => TemplatesFolder = DefaultTemplatesFolder;

        
        #if ODIN_INSPECTOR //Not needed when not using ODIN
        private static String RelativePathKey => $"{_EDITOR_PREFS_SCOPE}_use-relative-path";
        //private static String ProjectRelativeDefaultValue = ""
        [PublicAPI]
        public static Boolean UseRelativePath
        {
            get => EditorPrefs.GetBool(key: RelativePathKey, defaultValue: false);

            internal
            set //TODO: Make Path Project Relative when switching to that, and vice versa.
            {
                EditorPrefs.SetBool(key: RelativePathKey, value);

                TemplatesFolder = Path.GetFullPath(TemplatesFolder);
            }
        }
        
        public static Boolean UseAbsolutePath => !UseRelativePath;
        #endif
    }
    
    public sealed class ScriptTemplatesSettingsProvider : SettingsProvider
    {
        [PublicAPI]
        public ScriptTemplatesSettingsProvider(in String path, in SettingsScope scopes, in IEnumerable<String> keywords = null) : base(path, scopes, keywords)
        { }

        public override void OnGUI(String searchContext)
        {
            #if ODIN_INSPECTOR
            //Preferences.UseRelativePath = EditorGUILayout.Toggle(label: new GUIContent(text: "Project Relative Folder"), value: Preferences.UseRelativePath);
            
            EditorGUILayout.BeginHorizontal();
            Preferences.TemplatesFolder = SirenixEditorFields.FolderPathField(label: new GUIContent(text: "Script Templates Folder"), path: Preferences.TemplatesFolder, parentPath: "Assets", absolutePath: Preferences.UseAbsolutePath, useBackslashes: false);
            if (GUILayout.Button(text: "Reset", options: GUILayout.Width(80)))
            {
                Preferences.TemplatesFolder = Preferences.DefaultTemplatesFolder;
            }
            EditorGUILayout.EndHorizontal();
            #else
            Preferences.TemplatesFolder = EditorGUILayout.TextField(label: "Script Templates Folder", text: Preferences.TemplatesFolder);
            #endif
            
            if (GUILayout.Button(text: "Regenerate Templates"))
            {
                ScriptTemplateFactory.RemoveAll();
                ScriptTemplateFactory.CreateAll();    
            }
            
            if (GUILayout.Button(text: "Reset Templates"))
            {
                ScriptTemplateFactory.RemoveAll();
            }
        }

        [SettingsProvider]
        public static SettingsProvider Create() 
            => new ScriptTemplatesSettingsProvider(path: Constants.PREFERENCE_PATH, scopes: SettingsScope.User);
    }
}

#endif
