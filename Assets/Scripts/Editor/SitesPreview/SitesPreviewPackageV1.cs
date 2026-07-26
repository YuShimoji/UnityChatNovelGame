#if UNITY_EDITOR
using System;

namespace ProjectFoundPhone.Editor
{
    [Serializable]
    public sealed class SitesPreviewPackageV1
    {
        public string schema;
        public int version;
        public string packageIdentitySha256;
        public string contentLabel;
        public string canonStatus;
        public string nodeName;
        public SitesPreviewSourceV1 source;
        public string[] supportedConstructs;
        public SitesPreviewDisplayLineV1[] displayLines;
        public SitesPreviewDiagnosticV1[] diagnostics;
    }

    [Serializable]
    public sealed class SitesPreviewSourceV1
    {
        public string assetPath;
        public int titleLine;
        public string contentSha256;
    }

    [Serializable]
    public sealed class SitesPreviewDisplayLineV1
    {
        public int ordinal;
        public int sourceLine;
        public string kind;
        public string speakerId;
        public string speakerLabel;
        public string text;
    }

    [Serializable]
    public sealed class SitesPreviewDiagnosticV1
    {
        public int sourceLine;
        public string severity;
        public string code;
        public string command;
        public string message;
    }

    public readonly struct SitesPreviewExportResult
    {
        public SitesPreviewExportResult(
            bool success,
            SitesPreviewPackageV1 package,
            string outputPath,
            string message)
        {
            Success = success;
            Package = package;
            OutputPath = outputPath ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public bool Success { get; }
        public SitesPreviewPackageV1 Package { get; }
        public string OutputPath { get; }
        public string Message { get; }
        public int DisplayLineCount => Package?.displayLines?.Length ?? 0;
        public int UnsupportedConstructCount => Package?.diagnostics?.Length ?? 0;
        public string PackageIdentitySha256 => Package?.packageIdentitySha256 ?? string.Empty;
    }
}
#endif
