//#if UNITY_EDITOR

using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

using CGTK.Utils.Extensions;

[assembly: InternalsVisibleTo(assemblyName: CGTK.Tools.CustomScriptTemplates.PackageConstants.TEST_ASSEMBLY)]
namespace CGTK.Tools.CustomScriptTemplates
{
	internal static partial class Templates
	{
		public static IEnumerable<(String folders, String name, String path)> Gather()
		{
			String __templatesFolder = Preferences.TemplatesFolder.ToPathFormatting();
			
			if (__templatesFolder.IsNullOrEmpty()) throw new NullReferenceException(message: nameof(__templatesFolder));

			if (!Directory.Exists(__templatesFolder))
			{
				throw new DirectoryNotFoundException(message: $"{nameof(__templatesFolder)}: {__templatesFolder}");
			}
			
			String[] __filePaths = Directory.GetFiles(path: __templatesFolder, searchPattern: "*.txt", SearchOption.AllDirectories);

			List<(String folders, String name, String path)> __result = new List<(String folders, String name, String path)>(__filePaths.Length);
			
			foreach (String __filePath in __filePaths)
			{
				String __name = Path.GetFileNameWithoutExtension(__filePath);

				String __fullPath = __filePath.AppendDirectorySeparator().Substring(startIndex: 0 , length: __filePath.LastIndexOf(value: "\\", StringComparison.Ordinal)).AppendDirectorySeparator();

				String __folders = String.Empty;
				
				if (__fullPath != __templatesFolder)
				{
					//Disgusting but all other things I tried didn't work. TODO: Replace with better method!
					__folders = __fullPath;
					//__folders = __folders.Remove(text: __templatesFolder);
					__folders = __folders.Remove(text: __templatesFolder.AppendDirectorySeparator());
					__folders = __folders.Remove(text: Path.GetFileName(__filePath));
					__folders = __folders.AppendDirectorySeparator();
				}
				
				//Only visible if you disable "Clear on Recompile" in the Console Window.
				Debug.Log(message: "\n" + 
								   "<b>File Path: </b>\n" +
								   $"<i>{__filePath} </i> \n" +
								   "<b>Full Path: </b>\n" +
								   $"<i>{__fullPath} </i> \n" +
								   "<b>Templates Folder: </b>\n" +
								   $"<i>{__templatesFolder} </i> \n" + 
								   $"<color=cyan><b>Result ({__folders})</b></color>");

				__result.Add(item: (folders: __folders, name: __name, path: __filePath));
			}

			return __result;
		}
	}
    
}
//#endif