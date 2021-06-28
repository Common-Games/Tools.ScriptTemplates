#if UNITY_EDITOR

using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using JetBrains.Annotations;

#if ODIN_INSPECTOR
using Sirenix.Utilities.Editor;
#endif

namespace CGTK.Tools.CustomScriptTemplates
{
    [Serializable]
    public static class Settings
    {
        private const String _EDITOR_PREFS_SCOPE = Constants.PACKAGE_NAME;
        private static String FolderPathKey => $"{_EDITOR_PREFS_SCOPE}_folder-path";
        [PublicAPI]
        public static String  FolderPath
        {
            //TODO: Replace DefaultPath
            get => EditorPrefs.GetString(key: FolderPathKey, defaultValue: "DefaultPath");
            
            internal
            set => EditorPrefs.SetString(key: FolderPathKey, value);
        }
        
        #if ODIN_INSPECTOR //Not needed when not using ODIN
        private static String  ProjectRelativeKey => $"{_EDITOR_PREFS_SCOPE}_project-relative";
        [PublicAPI]
        public static Boolean ProjectRelative
        {
            get => EditorPrefs.GetBool(key: ProjectRelativeKey, defaultValue: true);
            
            internal
            set => EditorPrefs.SetBool(key: ProjectRelativeKey, value);
        }

        public static Boolean UseAbsolutePath => !ProjectRelative;
        #endif
    }

    public sealed class ScriptTemplatesSettingsProvider : SettingsProvider
    {
        public ScriptTemplatesSettingsProvider(in String path, in SettingsScope scopes, in IEnumerable<String> keywords = null) : base(path, scopes, keywords)
        { }

        public override void OnGUI(String searchContext)
        {
            #if ODIN_INSPECTOR
            Settings.ProjectRelative = EditorGUILayout.Toggle(label: new GUIContent(text: "Project Relative Folder"), value: Settings.ProjectRelative);
            Settings.FolderPath = SirenixEditorFields.FolderPathField(label: new GUIContent(text: "Script Templates Folder"), path: Settings.FolderPath, parentPath: "Assets", absolutePath: Settings.UseAbsolutePath, useBackslashes: false);
            #else
            Settings.FolderPath = EditorGUILayout.TextField(label: "Script Templates Folder", text: Settings.FolderPath);
            #endif
        }

        [SettingsProvider]
        public static SettingsProvider Create() 
            => new ScriptTemplatesSettingsProvider(path: "Preferences/CGTK/Tools/Custom Script Templates", scopes: SettingsScope.User);
    }
}

#endif
