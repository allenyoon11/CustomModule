using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace neuroears.allen.utils
{
    public class OpenDir
    {
        public enum PathType
        {
            PROJECT, DATA
        }
        public static void Open(bool showLog = false) => OpenPath(Application.persistentDataPath, showLog);
        public static void Open(string path, bool showLog = false) => OpenPath(path, showLog);
        public static void Open(params string[] paths) => OpenPath(Path.Combine(paths), false);
        public static void Open(bool showLog, params string[] paths) => OpenPath(Path.Combine(paths), showLog);
        public static void Open(PathType type, bool showLog = false) => OpenPath(GetSavedPath(type), showLog);

        #region private
        private static string GetSavedPath(PathType type)
        {
            return type switch
            {
                PathType.PROJECT => Application.persistentDataPath,
                PathType.DATA => Path.Combine(Application.persistentDataPath, "data"),
                _ => ""
            };
        }
        private static void OpenPath(string path, bool log)
        {
            path = path.Replace("/", "\\");
            Process.Start("explorer.exe", path);
            if(log)
            {
                Debug.Log($"open: {path}");
            }
        }
        #endregion
    }

}
