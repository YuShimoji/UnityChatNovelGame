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

.EXAMPLE
.\tools\run-unity.ps1 -BatchMode -IsolateTestSaveData `
    -LogFile 'Logs\editmode-tests.log' `
    -AdditionalArguments @('-runTests', '-testPlatform', 'EditMode',
        '-testResults', 'Logs\editmode-tests.xml')
#>
param(
    [string]$UnityPath,
    [switch]$BatchMode,
    [switch]$Quit,
    [switch]$IsolateTestSaveData,
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

$previousTestSaveRoot = $null
$previousTestSaveDirectory = $null
if ($IsolateTestSaveData) {
    $previousTestSaveRoot = [Environment]::GetEnvironmentVariable(
        'FOUNDPHONE_TEST_SAVE_ROOT', 'Process')
    $previousTestSaveDirectory = [Environment]::GetEnvironmentVariable(
        'FOUNDPHONE_TEST_SAVE_DIRECTORY', 'Process')
    $testSaveRoot = Join-Path ([System.IO.Path]::GetTempPath()) 'FoundPhoneTests'
    $testSaveDirectory = Join-Path $testSaveRoot (
        'unity-{0}-{1}' -f $PID, [Guid]::NewGuid().ToString('N'))
    [System.IO.Directory]::CreateDirectory($testSaveDirectory) | Out-Null
    [Environment]::SetEnvironmentVariable(
        'FOUNDPHONE_TEST_SAVE_ROOT', $testSaveRoot, 'Process')
    [Environment]::SetEnvironmentVariable(
        'FOUNDPHONE_TEST_SAVE_DIRECTORY', $testSaveDirectory, 'Process')
    Write-Output "Unity test save data isolated at: $testSaveDirectory"
}

if ($null -ne $AdditionalArguments -and
    $AdditionalArguments -contains '-runTests' -and
    -not $IsolateTestSaveData) {
    throw 'Unity test runs require -IsolateTestSaveData to protect persistent user saves.'
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

$process = $null
try {
    $process = Start-Process @startParameters
} finally {
    if ($IsolateTestSaveData) {
        [Environment]::SetEnvironmentVariable(
            'FOUNDPHONE_TEST_SAVE_ROOT', $previousTestSaveRoot, 'Process')
        [Environment]::SetEnvironmentVariable(
            'FOUNDPHONE_TEST_SAVE_DIRECTORY', $previousTestSaveDirectory, 'Process')
    }
}

if ($Wait -or $BatchMode) {
    $process.WaitForExit()
    exit $process.ExitCode
}

Write-Output "Unity started (PID $($process.Id)): $UnityPath"
