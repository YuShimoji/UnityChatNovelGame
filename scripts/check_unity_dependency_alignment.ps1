param(
    [string]$ManifestPath = "Packages/manifest.json",
    [string]$LockPath = "Packages/packages-lock.json"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if (-not (Test-Path $ManifestPath)) {
    throw "manifest.json not found: $ManifestPath"
}
if (-not (Test-Path $LockPath)) {
    throw "packages-lock.json not found: $LockPath"
}

$manifest = Get-Content -Path $ManifestPath -Raw | ConvertFrom-Json
$lock = Get-Content -Path $LockPath -Raw | ConvertFrom-Json

$directDeps = $manifest.dependencies.PSObject.Properties
$lockDeps = $lock.dependencies

$mismatches = New-Object System.Collections.Generic.List[object]
foreach ($dep in $directDeps) {
    $name = $dep.Name
    $manifestVersion = [string]$dep.Value

    if (-not $lockDeps.PSObject.Properties.Name.Contains($name)) {
        $mismatches.Add([pscustomobject]@{
            Package = $name
            ManifestVersion = $manifestVersion
            LockVersion = "<missing>"
            Kind = "MissingInLock"
        })
        continue
    }

    $lockVersion = [string]$lockDeps.$name.version
    if ($lockVersion -ne $manifestVersion) {
        $mismatches.Add([pscustomobject]@{
            Package = $name
            ManifestVersion = $manifestVersion
            LockVersion = $lockVersion
            Kind = "VersionMismatch"
        })
    }
}

$riskFlags = New-Object System.Collections.Generic.List[string]
if ($lockDeps.PSObject.Properties.Name.Contains("com.unity.ai.assistant") -and
    $lockDeps.PSObject.Properties.Name.Contains("dev.yarnspinner.unity")) {
    $riskFlags.Add("AI Assistant + YarnSpinner coexistence: Roslyn/Shared assembly duplicate risk")
}

if ($lockDeps.PSObject.Properties.Name.Contains("com.unity.2d.enhancers")) {
    $enhancers = $lockDeps."com.unity.2d.enhancers".dependencies
    if ($enhancers.PSObject.Properties.Name.Contains("com.unity.ai.generators")) {
        $enhancersPinned = [string]$enhancers."com.unity.ai.generators"
        $resolved = ""
        if ($lockDeps.PSObject.Properties.Name.Contains("com.unity.ai.generators")) {
            $resolved = [string]$lockDeps."com.unity.ai.generators".version
        }
        if ($resolved -and $enhancersPinned -ne $resolved) {
            $riskFlags.Add("com.unity.2d.enhancers expects ai.generators=$enhancersPinned but lock resolves $resolved")
        }
    }
}

Write-Output "=== Dependency Alignment Check ==="
Write-Output "Manifest: $ManifestPath"
Write-Output "Lock: $LockPath"
Write-Output ""

if ($mismatches.Count -eq 0) {
    Write-Output "[OK] No direct manifest/lock mismatches."
} else {
    Write-Output "[WARN] Found $($mismatches.Count) manifest/lock mismatch(es):"
    $mismatches | Sort-Object Package | Format-Table -AutoSize
}

Write-Output ""
if ($riskFlags.Count -eq 0) {
    Write-Output "[OK] No known high-risk package combination flags."
} else {
    Write-Output "[WARN] Risk flags:"
    foreach ($flag in $riskFlags) {
        Write-Output " - $flag"
    }
}
