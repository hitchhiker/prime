using System;
using System.IO;

namespace Prime.KeysManager.Utils
{
    public static class ElectronUtils
    {
        public static string FindElectronUiDirectory(string electronFolderName, string rootPath)
        {
            if(string.IsNullOrWhiteSpace(electronFolderName))
                return null;

            var currentPath = rootPath;
            var testPath = Path.Combine(currentPath, electronFolderName);

            while (!Directory.Exists(testPath))
            {
                var separatorIndex = currentPath.LastIndexOf(Path.DirectorySeparatorChar);

                if(separatorIndex >= 0) {
                    currentPath = currentPath.Substring(0, separatorIndex);
                    testPath = Path.Combine(currentPath, electronFolderName);
                }
                else 
                    break;
            }
            
            return string.IsNullOrWhiteSpace(currentPath) == false ? Path.Combine(currentPath, electronFolderName) : null;
        }
    }
}