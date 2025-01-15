using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace neuroears.allen.utils
{
    public class FileIOHelper
    {
        public static bool CheckOrCreateDirectory(string dir)
        {
            try
            {
                if (Directory.Exists(dir))
                {
                    return true;
                }
                else
                {
                    Directory.CreateDirectory(dir);
                    return true;
                }

            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                return false;
            }
        }
        public static bool CheckOrGetFilename(string path, out string _path, out string _filename)
        {
            string dir = Path.GetDirectoryName(path);
            string filenameWithoutExt = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);

            _path = path;
            _filename = filenameWithoutExt;
            try
            {
                if (File.Exists(path))
                {
                    int i = 1;
                    while (true)
                    {
                        _filename = string.Format("{0} ({1})", filenameWithoutExt, i++);
                        _path = Path.Combine(dir, $"{_filename}{ext}");

                        if (!File.Exists(_path)) break;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                return false;
            }
        }
    }

}

