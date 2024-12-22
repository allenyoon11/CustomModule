using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace allen.utils
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
        public static bool CheckOrGetFilename(string dir, string filename, string ext, out string _filename)
        {
            var path = Path.Combine(dir, $"{filename}.{ext}");
            _filename = filename;
            try
            {
                if (File.Exists(path))
                {
                    int i = 1;
                    while (true)
                    {
                        _filename = string.Format("{0} ({1})", filename, i++);
                        path = Path.Combine(dir, $"{_filename}.{ext}");

                        if (!File.Exists(path)) break;
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

