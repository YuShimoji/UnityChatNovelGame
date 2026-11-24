using System;
using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ProjectFoundPhone.Tests")]
[assembly: InternalsVisibleTo("ProjectFoundPhone.PlayModeTests")]

namespace ProjectFoundPhone.Core
{
    // Scene transitions can create new SaveManagers. Keep all of them in the same test storage scope.
    internal sealed class SaveStorageIsolation : IDisposable
    {
        private readonly string m_Directory;
        private bool m_Disposed;

        internal static string DirectoryOverride { get; private set; }

        internal SaveStorageIsolation()
        {
            if (DirectoryOverride != null)
            {
                throw new InvalidOperationException("A save storage scope is already active.");
            }

            m_Directory = Path.Combine(Path.GetTempPath(), "FoundPhone-SaveTests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(m_Directory);
            DirectoryOverride = m_Directory;
        }

        public void Dispose()
        {
            if (m_Disposed)
            {
                return;
            }

            m_Disposed = true;
            DirectoryOverride = null;
            foreach (string file in Directory.EnumerateFiles(m_Directory))
            {
                File.Delete(file);
            }
            Directory.Delete(m_Directory);
        }
    }
}
