using System;

using JetBrains.Annotations;

namespace CGTK.Tools.CustomScriptTemplates
{
    [PublicAPI]
    internal static class Constants
    {
        public const String PACKAGE_COMPANY = "com.common-games.";
        public const String PACKAGE_GROUP   = "tools.";
        
        public const String PACKAGE_NAME    = PACKAGE_COMPANY + PACKAGE_GROUP + "custom-script-templates";
        public const String PACKAGE_PATH    = "Packages/" + PACKAGE_NAME + "/";
        
        public const String FOLDER_EDITOR    = PACKAGE_PATH + "Editor/";
        public const String FOLDER_GENERATED = PACKAGE_PATH + "_Generated/";
        
        public const String DEFAULT_SCRIPT_TEMPLATES_FOLDER = PACKAGE_PATH + "DefaultTemplates/";
        
        public const String PREFERENCE_PATH = "Preferences/CGTK/Tools/Custom Script Templates";

        public const String TEST_ASSEMBLY = "CGTK.Tools.CustomScriptTemplates.Tests";
    }
}
