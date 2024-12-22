using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace allen.utils
{
    public class CsvWriterTest : MonoBehaviour
    {
        private CsvWriter writer;
        private string dir;
        private string filename;
        private string[] header = { "A", "B", "C" };
        private List<string[]> lines = new List<string[]>();
        private void Awake()
        {
            dir = Path.Combine(Application.persistentDataPath, "data");
            filename = "test";
            string path = Path.Combine(dir, $"{filename}.csv");
            writer = new CsvWriter(path, header);
        }
        private void Start()
        {
            for (int i = 0; i < 10; i++)
            {
                string[] line = { $"a{i}", $"b{i}", $"c{i}" };
                lines.Add(line);
            }

            Export();
        }

        private async void Export()
        {
            foreach(var line in lines)
            {
                writer.AddLine(line);
            }
            await writer.WriteAsync();
        }
    }

}
