[CmdletBinding()]
<#
.SYNOPSIS
Launches the Unity version required by this repository.

.DESCRIPTION
Restores the standard Windows ALLUSERSPROFILE value for the child process when
the calling shell omitted it. Unity Package Manager needs that value during a
fresh resolve. The setting is process-local and does not modify user or system
environment variables.

.EXAMPLE
.\tools\run-unity.ps1

.EXAMPLE
.\tools\run-unity.ps1 -BatchMode -Quit -LogFile 'Logs\unity-open.log'

.EXAMPLE
.\tools\run-unity.ps1 -BatchMode -Quit `
    -LogFile 'Logs\yarn-validator.log' `
    -ExecuteMethod 'ProjectFoundPhone.Editor.ContentPipelineBatch.RunYarnValidatorBatch'
#>
param(
    [string]$UnityPath,
    [switch]$BatchMode,
    [switch]$Quit,
    [string]$LogFile,
    [string]$ExecuteMethod,
    [string[]]$AdditionalArguments,
    [switch]$Wait
)

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$projectVersionFile = Join-Path $repoRoot 'ProjectSettings\ProjectVersion.txt'

if (-not (Test-Path -LiteralPath $projectVersionFile)) {
    throw "Unity project version file was not found: $projectVersionFile"
}

if ([string]::IsNullOrWhiteSpace($UnityPath)) {
    $versionLine = Get-Content -LiteralPath $projectVersionFile -Encoding utf8 |
        Where-Object { $_ -match '^m_EditorVersion:\s*(.+)$' } |
        Select-Object -First 1

    if ($null -eq $versionLine) {
        throw "Unity editor version could not be read from: $projectVersionFile"
    }

    $unityVersion = $Matches[1].Trim()
    $UnityPath = Join-Path ${env:ProgramFiles} "Unity\Hub\Editor\$unityVersion\Editor\Unity.exe"
}

if (-not (Test-Path -LiteralPath $UnityPath)) {
    throw "Unity executable was not found: $UnityPath"
}

if ([string]::IsNullOrWhiteSpace($env:ALLUSERSPROFILE)) {
    $env:ALLUSERSPROFILE = [Environment]::GetFolderPath(
        [Environment+SpecialFolder]::CommonApplicationData)
}

$unityArguments = @('-projectPath', $repoRoot)

if ($BatchMode) {
    $unityArguments += @('-batchmode', '-nographics')
}

if ($Quit) {
    $unityArguments += '-quit'
}

if (-not [string]::IsNullOrWhiteSpace($LogFile)) {
    $resolvedLogFile = if ([System.IO.Path]::IsPathRooted($LogFile)) {
        $LogFile
    } else {
        Join-Path $repoRoot $LogFile
    }
    $unityArguments += @('-logFile', $resolvedLogFile)
}

if (-not [string]::IsNullOrWhiteSpace($ExecuteMethod)) {
    $unityArguments += @('-executeMethod', $ExecuteMethod)
}

if ($null -ne $AdditionalArguments) {
    $unityArguments += $AdditionalArguments
}

$quotedArguments = foreach ($argument in $unityArguments) {
    if ($argument -match '[\s"]') {
        '"' + $argument.Replace('"', '\"') + '"'
    } else {
        $argument
    }
}

$startParameters = @{
    FilePath = $UnityPath
    ArgumentList = $quotedArguments
    PassThru = $true
}

if ($BatchMode) {
    $startParameters.WindowStyle = 'Hidden'
}

$process = Start-Process @startParameters

if ($Wait -or $BatchMode) {
    $process.WaitForExit()
    exit $process.ExitCode
}

Write-Output "Unity started (PID $($process.Id)): $UnityPath"
