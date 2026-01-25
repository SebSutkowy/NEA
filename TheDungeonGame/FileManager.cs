using System;
using System.Diagnostics;
using System.IO;

namespace TheDungeonGame
{
    internal static class FileManager
    {
        private static string BasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DungeonGame", "Data");
        private static string DatabasePath = Path.Combine(BasePath, "Database.db");

        public static string GetDatabasePath() => DatabasePath;

        public static bool FileExists(string path)
        {
            string dir = Path.Combine(BasePath, path);
            if (File.Exists(dir))
                return true;
            return false;
        }

        public static string GetDirectory(string path)
        {
            string dir = Path.Combine(BasePath, path);
            if (!File.Exists(dir))
            {
                Debug.WriteLine("Directory/File does not exist!");
                return "";
            }
            return dir;
        }

        public static void SaveData(string path, string fileName, string data)
        {
            Debug.WriteLine($"Saving to: {Path.Combine(BasePath, path, fileName)}");
            string dir = Path.Combine(BasePath, path);
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, fileName), data);
        }

        public static string ReadData(string path)
        {
            string dir = Path.Combine(BasePath, path);
            Debug.WriteLine($"Reading from {dir}");
            return File.ReadAllText(dir);
        }

    }
}
