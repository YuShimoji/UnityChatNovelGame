using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace ProjectFoundPhone.Tests
{
    /// <summary>
    /// Unity tests must never read, overwrite, or delete the real player save directory.
    /// The launcher provides an isolated directory under the OS temp root; tests fail
    /// closed when that boundary is absent or points anywhere else.
    /// </summary>
    public static class TestSaveDataIsolation
    {
        private const string EnvironmentVariableName = "FOUNDPHONE_TEST_SAVE_DIRECTORY";
        private const string RootEnvironmentVariableName = "FOUNDPHONE_TEST_SAVE_ROOT";
        private const string TestSaveRootDirectoryName = "FoundPhoneTests";

        public static string RequireDirectory()
        {
            string configuredDirectory =
                Environment.GetEnvironmentVariable(EnvironmentVariableName);
            Assert.That(
                configuredDirectory,
                Is.Not.Null.And.Not.Empty,
                $"Run Unity tests through tools/run-unity.ps1 -IsolateTestSaveData. "
                + $"{EnvironmentVariableName} is not configured.");

            string fullDirectory = Path.GetFullPath(configuredDirectory);
            string configuredRoot =
                Environment.GetEnvironmentVariable(RootEnvironmentVariableName);
            Assert.That(
                configuredRoot,
                Is.Not.Null.And.Not.Empty,
                $"{RootEnvironmentVariableName} is not configured.");

            string allowedRoot = Path.GetFullPath(configuredRoot);
            Assert.That(
                Path.GetFileName(allowedRoot),
                Is.EqualTo(TestSaveRootDirectoryName),
                $"{RootEnvironmentVariableName} must end with {TestSaveRootDirectoryName}.");
            string allowedPrefix =
                allowedRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;

            Assert.That(
                fullDirectory.StartsWith(allowedPrefix, StringComparison.OrdinalIgnoreCase),
                Is.True,
                $"{EnvironmentVariableName} must be under {allowedRoot}.");
            Assert.That(
                string.Equals(
                    fullDirectory,
                    Path.GetFullPath(Application.persistentDataPath),
                    StringComparison.OrdinalIgnoreCase),
                Is.False,
                "The isolated test directory must not equal Application.persistentDataPath.");

            Directory.CreateDirectory(fullDirectory);
            return fullDirectory;
        }

        public static string GetSaveFilePath(int slotNumber)
        {
            return Path.Combine(RequireDirectory(), $"SaveData_{slotNumber}.json");
        }

        public static void CleanupSaveSlots(params int[] slotNumbers)
        {
            foreach (int slotNumber in slotNumbers)
            {
                string filePath = GetSaveFilePath(slotNumber);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }
    }
}
