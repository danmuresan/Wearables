using System;
using System.IO;
using System.Threading.Tasks;

namespace Commons.Helpers
{
    public static class FileManipulationHelper
    {
        public static string DefaultFolderPath => Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "SensorClientApp");

        public static async Task WriteToFileAsync(string filename, string content, bool append = false)
        {
            var dir = new Java.IO.File(DefaultFolderPath);
            if (!dir.Exists())
            {
                dir.Mkdirs();
            }

            var path = Path.Combine(DefaultFolderPath, filename);

            using (var streamWriter = new StreamWriter(path, append))
            {
                await streamWriter.WriteAsync(content);
            }
        }

        public static async Task<string> ReadFileContentsAsync(string filename)
        {
            string readContent = null;
            var path = Path.Combine(DefaultFolderPath, filename);

            using (var streamReader = new StreamReader(path))
            {
                readContent = await streamReader.ReadToEndAsync();
            }

            return readContent;
        }
    }
}