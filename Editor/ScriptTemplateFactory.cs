//#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using UnityEditor;

using CGTK.Utilities.Extensions;

namespace CGTK.Tools.CustomScriptTemplates
{
    internal static class ScriptTemplateFactory
    {
        #region Fields
        
        private const String _SCRIPT = 
            "#if UNITY_EDITOR \n" +
            "using UnityEditor; \n" +
            "namespace CGTK.Tools.CustomScriptTemplates  \n" +
            "{  \n" +
                "internal static partial class Templates  \n" +
                "{  \n" +
                    "[MenuItem(itemName: \"Assets/Create/Script/#FOLDERS##NAME#\", priority= 30)]  \n" +
                    "internal static void #NAME#()  \n" +
                    "{  \n" +
                        "ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath: @\"#TEMPLATE_PATH#\", defaultNewFileName: \"New#NAME#.cs\");  \n" +        
                    "} \n" +
                "} \n" +
            "} \n" +
            "#endif \n";

        private const String _META =
            "fileFormatVersion: 2\n" +
            "guid: #GUID#\n" +
            "MonoImporter:\n" +
            "externalObjects: {}\n" +
            "serializedVersion: 2\n" +
            "defaultReferences: []\n" +
            "executionOrder: 0\n" +
            "icon: {instanceID: 0}\n" +
            "userData:\n" +
            "assetBundleName:\n" +
            "assetBundleVariant:";
        
        #endregion

        #region Methods
        
        [MenuItem(itemName: "Tools/CGTK/Script Templates/Regenerate")]
        public static void Regenerate()
        {
            IEnumerable<(String folders, String name, String path)> __templates = Templates.Gather();

            foreach ((String __folders, String __name, String __path) in __templates)
            {
                CreateScript(name: __name, folders: __folders.ToUnityFormatting(), path: __path);
            }
        }
        
        [MenuItem(itemName: "Tools/CGTK/Script Templates/Reset")]
        public static void Reset()
        {
            DirectoryInfo __directory = new DirectoryInfo(path: Constants.FOLDER_GENERATED);
            __directory.RemoveFiles(fileExtensionToRemove: ".cs");
            __directory.RemoveFiles(fileExtensionToRemove: ".cs.meta");
        }

        private static void CreateScript(String name, in String folders, in String path)
        {
            name = name.MakeValidScriptName();
            
            String __script = _SCRIPT;

            __script = __script.Replace(oldValue: "#NAME#", newValue: name);
            __script = __script.Replace(oldValue: "#FOLDERS#", newValue: folders);
            __script = __script.Replace(oldValue: "#TEMPLATE_PATH#", newValue: path);

            String __filePath = Path.Combine(Constants.FOLDER_GENERATED, path2: $"{name}.cs");
      
            File.WriteAllText(path: __filePath, contents: __script, encoding: new UTF8Encoding(true));
            
            CreateMeta(name);
            
            AssetDatabase.ImportAsset(__filePath);
        }
        
        /// <summary>
        /// We need to generate them manually because we're creating the scripts in the Packages folders.
        /// Unity does not generate .meta files for that folder, but it needs .meta's to actually recognize the scripts as being valid.
        /// </summary>
        private static void CreateMeta(in String name)
        {
            String __meta = _META;
            
            __meta = __meta.Replace(oldValue: "#GUID#", newValue: Guid.NewGuid().ToString());

            String __filePath = Path.Combine(Constants.FOLDER_GENERATED, path2: $"{name}.cs.meta");
      
            File.WriteAllText(path: __filePath, contents: __meta, encoding: new UTF8Encoding(true));
            AssetDatabase.ImportAsset(__filePath);
        }

        #endregion
    }
}
//#endif