using System;
using System.IO;
using Newtonsoft.Json;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.Core
{
    internal sealed class SaveSlotStorage
    {
        private readonly string m_Directory;
        private readonly string m_Prefix;
        private readonly string m_Extension;

        public SaveSlotStorage(string directory, string prefix, string extension)
        {
            m_Directory = directory;
            m_Prefix = prefix;
            m_Extension = extension;
        }

        public string GetPath(int slot) => Path.Combine(m_Directory, $"{m_Prefix}_{slot}{m_Extension}");

        public bool Exists(int slot) => File.Exists(GetPath(slot));

        public SaveData Read(int slot)
        {
            return JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(GetPath(slot)));
        }

        public void Write(int slot, SaveData data)
        {
            string path = GetPath(slot);
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            Directory.CreateDirectory(m_Directory);
            string temporaryPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                File.WriteAllText(temporaryPath, json);
                // Keep the last complete save until the new file is ready.
                if (File.Exists(path))
                {
                    File.Replace(temporaryPath, path, null);
                }
                else
                {
                    File.Move(temporaryPath, path);
                }
            }
            finally
            {
                File.Delete(temporaryPath);
            }
        }

        public void Delete(int slot) => File.Delete(GetPath(slot));
    }
}
