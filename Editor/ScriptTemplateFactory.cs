#if UNITY_EDITOR

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using UnityEditor;

using CGTK.Utilities.Extensions;

namespace CGTK.Tools.CustomScriptTemplates
{
    public static class ScriptTemplateFactory
    {
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
            "guid: 20570d736d80b9f43aa778aa2ec0dbff\n" +
            "MonoImporter:\n" +
            "externalObjects: {}\n" +
            "serializedVersion: 2\n" +
            "defaultReferences: []\n" +
            "executionOrder: 0\n" +
            "icon: {instanceID: 0}\n" +
            "userData:\n" +
            "assetBundleName:\n" +
            "assetBundleVariant:";

        public static void CreateAll()
        {
            IEnumerable<(String folders, String name, String path)> __templates = Templates.Gather();

            foreach ((String __folders, String __name, String __path) in __templates)
            {
                CreateScript(name: __name, folders: __folders.ToUnityFormatting(), path: __path);
            }
        }

        public static void RemoveAll()
        {
            DirectoryInfo __directory = new DirectoryInfo(path: Constants.FOLDER_GENERATED);
            __directory.RemoveFiles(fileExtensionToRemove: ".cs");
        }
        
        public static void CreateScript(in String name, in String folders, in String path)
        {
            String __script = _SCRIPT;
            
            __script = __script.Replace(oldValue: "#NAME#", newValue: name);
            __script = __script.Replace(oldValue: "#FOLDERS#", newValue: folders);
            __script = __script.Replace(oldValue: "#TEMPLATE_PATH#", newValue: path);

            String __finalPath = Path.Combine(Constants.FOLDER_GENERATED, path2: $"{name}.cs");
      
            File.WriteAllText(path: __finalPath, contents: __script, encoding: new UTF8Encoding(true));
            
            CreateMeta(name);
            
            AssetDatabase.ImportAsset(__finalPath);
        }
        
        public static void CreateMeta(in String name)
        {
            String __meta = _META;
            
            __meta = __meta.Replace(oldValue: "#GUID#", newValue: Guid.NewGuid().ToString());

            String __finalPath = Path.Combine(Constants.FOLDER_GENERATED, path2: $"{name}.cs.meta");
      
            File.WriteAllText(path: __finalPath, contents: __meta, encoding: new UTF8Encoding(true));
            AssetDatabase.ImportAsset(__finalPath);
        }
    }
}

#endif