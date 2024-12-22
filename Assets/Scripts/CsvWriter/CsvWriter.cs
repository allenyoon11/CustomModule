using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace allen.utils
{
    public class CsvWriter
    {
        private List<string[]> lines;
        private string path = null;

        public CsvWriter(params string[] headers) : this(null, headers) { }
        public CsvWriter(string path, params string[] headers)
        {
            this.path = path;
            lines = new List<string[]>();
            if(headers.Length > 0) AddHeader(headers);
        }

        public void AddHeader(string[] line)
        {
            lines.Insert(0, line);
        }
        public void AddLine(params string[] line)
        {
            if(this.lines.Count > 0)
            {
                var columnCnt = this.lines[0].Length;
                if(columnCnt == line.Length)
                {
                    lines.Add(line);
                }
                else
                {
                    throw new Exception($"comlumn cnt is not match {line.Length}/{columnCnt}");
                }

            }
        }

        public void AddLines(IEnumerable<string[]> lines)
        {
            foreach (var line in lines)
            {
                AddLine(line);
            }
        }

        private bool CheckPath(string path, out string _path)
        {
            _path = path;
            string dir = Path.GetDirectoryName(path);
            string filenameWithoutExt = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path).Substring(1);
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                int counter = 0;

                while (File.Exists(_path))
                {
                    _path = Path.Combine(dir, $"{filenameWithoutExt} ({++counter}).{ext}");
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
                return false;
            }
        }
        public bool Write(string path, bool append = false, Encoding encoding = null)
        {
            this.path = path;
            return Write(append, encoding);
        }
        public bool Write(bool append = false, Encoding encoding = null)
        {
            if(string.IsNullOrEmpty(this.path)) throw new ArgumentNullException($"path is null");
            var ext = Path.GetExtension(this.path).Substring(1);
            string delimiter = ",";
            switch (ext)
            {
                case "csv": delimiter = ","; break;
                case "tsv": delimiter = "\t"; break;
            }
            encoding ??= Encoding.UTF8;
            bool result = CheckPath(this.path, out string _path);
            if (!result)
            {
                throw new Exception($"failed check path {this.path}");
            }
            try
            {
                using (StreamWriter writer = new StreamWriter(_path, append, encoding))
                {

                    foreach (var line in lines)
                    {
                        writer.WriteLine(string.Join(delimiter, line));
                    }
                    lines.Clear();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
                return false;
            }
        }
        public async UniTask<bool> WriteAsync(string path, bool append = false, Encoding encoding = null)
        {
            this.path = path;
            return await WriteAsync(append, encoding);
        }
        public async UniTask<bool> WriteAsync(bool append = false, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(this.path)) throw new ArgumentNullException($"path is null");
            var ext = Path.GetExtension(this.path).Substring(1);
            string delimiter = ",";
            switch (ext)
            {
                case "csv": delimiter = ","; break;
                case "tsv": delimiter = "\t"; break;
            }
            encoding ??= Encoding.UTF8;

            bool result = CheckPath(this.path, out string path);
            if (!result)
            {
                throw new Exception($"failed check path {this.path}");
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(path, append, encoding))
                {
                    foreach (var line in lines)
                    {
                        await writer.WriteLineAsync(string.Join(delimiter, line));
                    }
                    await writer.FlushAsync();
                    lines.Clear();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                return false;
            }
        }
    }

}