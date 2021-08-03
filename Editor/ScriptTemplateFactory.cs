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
        private const String _BASELINE = 
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
            String __script = _BASELINE;
            
            __script = __script.Replace(oldValue: "#NAME#", newValue: name);
            __script = __script.Replace(oldValue: "#FOLDERS#", newValue: folders);
            __script = __script.Replace(oldValue: "#TEMPLATE_PATH#", newValue: path);

            String __finalPath = Path.Combine(Constants.FOLDER_GENERATED, path2: $"{name}.cs");
      
            File.WriteAllText(path: __finalPath, contents: __script, encoding: new UTF8Encoding(true));
            AssetDatabase.ImportAsset(__finalPath);
        }
    }
}

#endif