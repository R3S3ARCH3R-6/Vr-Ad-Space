using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Outputs data documenting where a head is looking to a JSON file.
/// Has methods to create the file, get the filename, write the data to a JSON, and end the file, all with proper formatting.
/// </summary>
public class OutputRawGazeData
{
    /// <summary>
    /// The path of the output file. Completed in FindMostRecentFile.
    /// </summary>
    public string path = Application.dataPath + "/Data/testData";

    /// <summary>
    /// Creates and returns a new filename. 
    /// The new filename will be incremented by one from the most recent filename.
    /// (Filename format: path + fileNumber + fileType)
    /// </summary>
    /// <returns>The filename of the new file.</returns>
    string CreateNewFilename()
    {
        int fileNumber = 0; //The number at the end of the filename in the path (before the suffix). Will increment from 0.
        string fileType = ".json"; //The end portion of the path. Contains the file type.

        //Iterate through filenames until we find one that doesn't exist. When the loop ends, we return the filename with filenumber incremented by 1.
        while (File.Exists(path + fileNumber + fileType))
        {
            fileNumber++; //Increment fileNumber = X by one if file  "testDataX.json" exists.
        }
        return (path + fileNumber + fileType);
    }

    /// <summary>
    /// Creates a new file, and returns its filename. 
    /// The new filename will be incremented by one from the most recent filename.
    /// (Filename format: path + fileNumber + fileType)
    /// </summary>
    /// <returns>The filename of the new file.</returns>
    public string CreateFile()
    {
        string filename = CreateNewFilename();
        StreamWriter w = File.AppendText(filename); //Open a new StreamWriter at file filename.
        w.WriteLine("{"); //First line of JSON input. We create the array.
        w.WriteLine("\t\t\"data\": ["); //Second line of JSON input. The array's name.
        w.WriteLine();  //Blank line
        w.Close(); //Close our streamwriter
        return filename;
    }

    /// <summary>
    /// Writes a JSON entry representing where the player is looking to a json file.
    /// The resulting JSON entry will be formatted like it is an entry in a JSON array.
    /// </summary>
    /// <param name="g">What the player is looking at, as an object of class GazeData</param>
    /// <param name="filename">The filename to write to.</param>
    public void WriteGazeData(GazeData g, string filename)
    {
        string data = JsonUtility.ToJson(g, true); //Create a JSON representation of our gaze data.
        data = FormatJSON(data);
        StreamWriter w = new StreamWriter(filename, true); //Open a new StreamWriter at file filename.
        w.WriteLine(data); //Write the line to the file.
        w.Close(); //We're done adding to the file, so we close the streamwriter.
    }

    /// <summary>
    /// Terminates the file using the proper JSON formatting.
    /// </summary>
    /// <param name="filename">The file being closed.</param>
    public void EndFile(string filename)
    {
        //We get all of the lines from the file using LINQ. We also use the features of the IEnumerable interface to skip the last line. At the end of it all, we turn it into a list.
        //We need to skip the last line because the most recent entry will be the last element in the array. The last line needs to be "}", not "},".
        //We also use append to add the last bit of formatting to our JSON.
        List<string> lines = (from string line in File.ReadLines(filename).Reverse().Skip(1).Reverse().Append("\t\t}").Append("\t\t]").Append("}")
                              select line).ToList();
        //Now, we replace all of the entries in the file with this updated data.
        File.WriteAllLines(filename, lines);
    }

    /// <summary>
    /// Given JSON data, adds two tabs before every endline, and a comma at the very end of the string.
    /// This allows us to store all of our entries in a JSON array, which we'll close at the end of the program.
    /// </summary>
    /// <param name="data">The JSON data string.</param>
    /// <returns>The formatted JSON string.</returns>
    string FormatJSON(string data)
    {
        data = data.Insert(0, "\t\t"); //Inserts two tabs before the first curly bracket.
        data = data.Replace("\n", "\n\t\t"); //Inserts two tabs after every endline.
        return data + ","; //Add a comma to the end so we can have everything properly formatted in the JSON array.
    }

}

