#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine;

namespace ProjectFoundPhone.Editor
{
    public static class SitesPreviewBatch
    {
        public const string VerificationNodeName = "SP023_NarrationMargin_Start";

        public static void ExportVerificationNode()
        {
            YarnSOGenerator.YarnNodeSourceLocation location =
                YarnSOGenerator.GetAuthoringScanSummary()
                    .NodeLocations
                    .FirstOrDefault(item =>
                        string.Equals(item.NodeName, VerificationNodeName, StringComparison.Ordinal));

            if (string.IsNullOrWhiteSpace(location.NodeName))
            {
                throw new InvalidOperationException(
                    $"Verification node was not found: {VerificationNodeName}");
            }

            SitesPreviewExportResult result = SitesPreviewExporter.Export(location);
            if (!result.Success)
            {
                throw new InvalidOperationException(result.Message);
            }

            Debug.Log(
                $"Sites Preview Package v1 exported: node={location.NodeName}, " +
                $"lines={result.DisplayLineCount}, unsupported={result.UnsupportedConstructCount}, " +
                $"identity={result.PackageIdentitySha256}, output={result.OutputPath}");
        }
    }
}
#endif
