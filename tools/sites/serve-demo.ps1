[CmdletBinding()]
param(
    [ValidateRange(1, 65535)]
    [int]$Port = 4173
)

$ErrorActionPreference = 'Stop'

$nodeCommand = Get-Command node -ErrorAction Stop
$serverScript = Join-Path $PSScriptRoot 'static-server.mjs'

if (-not (Test-Path -LiteralPath $serverScript)) {
    throw "Static server script was not found: $serverScript"
}

& $nodeCommand.Source $serverScript --port $Port
exit $LASTEXITCODE
