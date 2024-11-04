using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class CsvWriter
{
    private List<string[]> lines;
    private string[] headerLine;
    private string delimiter;

    public enum FileType
    {
        CSV,
        TSV
    }

    public CsvWriter(FileType fileType, params string[] headers)
    {
        delimiter = fileType == FileType.CSV ? "," : "\t";
        this.headerLine = headers;
        lines = new List<string[]>();
    }

    public void AddLine(params string[] line)
    {
        if (line.Length != headerLine.Length)
        {
            throw new ArgumentException("Line count must be match headerLine count.");
        }
        lines.Add(line);
    }

    public void AddLines(IEnumerable<string[]> lines)
    {
        foreach (var line in lines)
        {
            AddLine(line);
        }
    }

    private bool CheckPath(string filePath, out string newFilePath)
    {
        newFilePath = filePath;
        try
        {
            if (!File.Exists(filePath))
                return true;

            string directory = Path.GetDirectoryName(newFilePath) ?? string.Empty;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(newFilePath);
            string extension = Path.GetExtension(newFilePath);

            int counter = 0;

            while (File.Exists(newFilePath))
            {
                counter++;
                newFilePath = Path.Combine(directory, $"{fileNameWithoutExtension}({counter}){extension}");
            }
            return true;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
            return false;
        }
    }

    public bool Write(string filePath, bool append = false, Encoding encoding = null)
    {
        encoding ??= Encoding.UTF8;
        bool result = CheckPath(filePath, out string path);
        try
        {
            using (StreamWriter writer = new StreamWriter(path, append, encoding))
            {
                if (!append || new FileInfo(path).Length == 0)
                {
                    writer.WriteLine(string.Join(delimiter, headerLine));
                }

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

    public async UniTask<bool> WriteAsync(string filePath, bool append = false, Encoding encoding = null)
    {
        encoding ??= Encoding.UTF8;
        bool result = CheckPath(filePath, out string path);

        try
        {
            using (StreamWriter writer = new StreamWriter(filePath, append, encoding))
            {
                if (!append || new FileInfo(filePath).Length == 0)
                {
                    await writer.WriteLineAsync(string.Join(delimiter, headerLine));
                }

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
            Debug.Log(ex.ToString());
            return false;
        }
    }
}