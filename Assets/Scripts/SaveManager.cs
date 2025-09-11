using UnityEngine;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// A static utility class that handles saving and loading game data to/from a JSON file.
/// </summary>
public static class SaveManager
{
    // The name of the file where the game data will be saved.
    private static string saveFileName = "gamedata.json";

    /// <summary>
    /// Saves the provided game data to a JSON file.
    /// </summary>
    /// <param name="data">The data object to save.</param>
    public static void SaveGame(SaveData data)
    {
        // Convert the SaveData object to a JSON string.
        string json = JsonUtility.ToJson(data, true); // The 'true' argument formats the JSON for readability.

        // Determine the full path to the save file.
        string path = Path.Combine(Application.persistentDataPath, saveFileName);

        // Write the JSON string to the file.
        File.WriteAllText(path, json);

        Debug.Log("Game data saved to: " + path);
    }

    /// <summary>
    /// Loads game data from a JSON file.
    /// </summary>
    /// <returns>A SaveData object with the loaded data, or null if no save file exists.</returns>
    public static SaveData LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);

        // Check if the save file actually exists before trying to read it.
        if (File.Exists(path))
        {
            // Read the JSON string from the file.
            string json = File.ReadAllText(path);

            // Convert the JSON string back into a SaveData object.
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            Debug.Log("Game data loaded from: " + path);
            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found at: " + path);
            return null; // Return null if there's no save file to load.
        }
    }

    /// <summary>
    /// Checks if a save file exists in the persistent data path.
    /// </summary>
    /// <returns>True if the save file exists, false otherwise.</returns>
    public static bool DoesSaveFileExist()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        return File.Exists(path);
    }
    public static void DeleteSaveData()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save file deleted from: " + path);
        }
        else
        {
            Debug.LogWarning("Could not delete save file, as it was not found at: " + path);
        }
    }
}


[System.Serializable]
public class SaveData
{
    // Game stats
    public int score;
    public int turns;
    public int matches;

    // Grid information
    public int rows;
    public int columns;

    // Card state information
    public List<string> cardLayoutNames; // Stores the name of each card in grid order
    public List<bool> cardIsMatched;   // Stores whether each card at the same index has been matched
}