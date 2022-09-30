using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public sealed class FileHandler
{
    // Making this a singleton
    private static readonly FileHandler instance = new();
    private FileHandler() { }
    public static FileHandler Instance
    {
        get
        {
            return instance;
        }
    }

    private readonly Logger _logger = Logger.Instance;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filename">The filename of the text file to be written to.</param>
    /// <param name="line">The line to be written to the text file.</param>
    public void WriteTextFile(string filename, string line)
    {
        StreamWriter writer = new StreamWriter(Application.dataPath + "/" + filename, true);
        writer.WriteLine(line);
        writer.Close();
    }

    /// <summary>
    /// Read text file, add each line to a list, then return the output.
    /// </summary>
    /// <param name="filename">The filename of the text file to be read.</param>
    /// <returns>Returns a list of all the previously entered logs from the read text file.</returns>
    public List<string> ReadTextFile(string filename)
    {
        List<string> output = new List<string>();
        StreamReader reader = new StreamReader(Application.dataPath + "/" + filename);
        while (!reader.EndOfStream)
        {
            string inputLine = reader.ReadLine();
            output.Add(inputLine);
        }
        reader.Close();
        return output;
    }
}