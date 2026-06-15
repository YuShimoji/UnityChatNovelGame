[CmdletBinding()]
param(
    [switch]$PrepareView,
    [string]$ViewDir = ".mkdocs-view",
    [string]$NavOut
)

$ErrorActionPreference = "Stop"
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
$OutputEncoding = [System.Text.UTF8Encoding]::new($false)

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$excludedPrefixes = @(
    ".git/",
    "node_modules/",
    "dist/",
    "build/",
    ".venv/",
    "venv/",
    "__pycache__/",
    "Library/",
    "Temp/",
    "Logs/",
    "obj/",
    "bin/",
    "docs/archive/",
    ".mkdocs-view/",
    ".mkdocs-site/",
    "site/"
)

function Convert-ToRepoPath([string]$path) {
    $fullPath = (Resolve-Path -LiteralPath $path).Path
    $relative = [System.IO.Path]::GetRelativePath($repoRoot, $fullPath)
    return $relative.Replace("\", "/")
}

function Test-IsExcluded([string]$repoPath) {
    if ($repoPath -eq "NUL" -or $repoPath -eq "docs/index.md") {
        return $true
    }

    foreach ($prefix in $excludedPrefixes) {
        if ($repoPath.StartsWith($prefix, [System.StringComparison]::OrdinalIgnoreCase)) {
            return $true
        }
    }

    return $false
}

function Get-MarkdownFiles {
    $rg = Get-Command rg -ErrorAction SilentlyContinue
    if ($rg) {
        $args = @(
            "--files",
            "--hidden",
            "-g", "*.md",
            "-g", "!.git/**",
            "-g", "!node_modules/**",
            "-g", "!dist/**",
            "-g", "!build/**",
            "-g", "!.venv/**",
            "-g", "!venv/**",
            "-g", "!__pycache__/**",
            "-g", "!Library/**",
            "-g", "!Temp/**",
            "-g", "!Logs/**",
            "-g", "!obj/**",
            "-g", "!bin/**",
            "-g", "!docs/archive/**",
            "-g", "!.mkdocs-view/**",
            "-g", "!.mkdocs-site/**",
            "-g", "!site/**",
            "-g", "!NUL"
        )
        Push-Location $repoRoot
        try {
            return (& rg @args | ForEach-Object { $_.Replace("\", "/") } | Where-Object { -not (Test-IsExcluded $_) } | Sort-Object)
        }
        finally {
            Pop-Location
        }
    }

    return Get-ChildItem -LiteralPath $repoRoot -Recurse -File -Filter "*.md" -Force -ErrorAction SilentlyContinue |
        ForEach-Object { Convert-ToRepoPath $_.FullName } |
        Where-Object { -not (Test-IsExcluded $_) } |
        Sort-Object
}

function Get-Category([string]$repoPath) {
    if ($repoPath -in @("AGENTS.md", "CLAUDE.md", "prompt-resume.md", "docs/PROJECT_OVERVIEW.md") -or
        $repoPath -in @("docs/PROJECT_STATUS_DASHBOARD.md", "docs/VISUAL_PROGRESS_INDEX.md") -or
        $repoPath -like "docs/wiki/*") {
        return "Overview"
    }

    if ($repoPath -in @(
        "docs/DEVELOPMENT_TURN_PLAN.md",
        "docs/REPO_LOCAL_RULES.md",
        "docs/HANDOFF.md",
        "docs/runtime-state.md",
        "docs/project-context.md",
        "docs/INVARIANTS.md",
        "docs/USER_REQUEST_LEDGER.md",
        "docs/OPERATOR_WORKFLOW.md",
        "docs/INTERACTION_NOTES.md",
        "docs/DECISION_LOG.md"
    )) {
        return "Runtime State"
    }

    if ($repoPath -like "docs/StorySpec/*" -or
        $repoPath -in @(
            "docs/AUTOSAVE_DESIGN.md",
            "docs/DISPLAY_ALGORITHMS.md",
            "docs/ENGINE_FEATURE_INVENTORY.md",
            "docs/SaveSystem_README.md",
            "docs/SPEC_DECISIONS.md",
            "docs/UI_IMPLEMENTATION_SPEC.md"
        )) {
        return "Specs"
    }

    if ($repoPath -like "docs/ai/*" -or
        $repoPath -like "docs/plans/*" -or
        $repoPath -in @(
            "docs/SCENARIO_AUTHORING_GUIDE.md",
            "docs/YarnEditingPipeline.md",
            "docs/FEATURE_REGISTRY.md",
            "docs/FEATURE_STATUS_AUDIT.md",
            "docs/UI_ISSUES.md"
        )) {
        return "Development Notes"
    }

    if ($repoPath -like "docs/verification/*" -or
        $repoPath -like "docs/PerformanceBaseline_RAW*" -or
        $repoPath -eq "docs/EVIDENCE_REUSE.md") {
        return "Artifacts"
    }

    return "Misc"
}

function Get-Title([string]$repoPath) {
    if ($repoPath -eq "docs/wiki/_sidebar.md") {
        return "Wiki Sidebar"
    }

    $sourcePath = Join-Path $repoRoot $repoPath
    try {
        $heading = Get-Content -LiteralPath $sourcePath -Encoding UTF8 -TotalCount 40 |
            Where-Object { $_ -match "^\s*#\s+(.+?)\s*$" } |
            Select-Object -First 1
        if ($heading) {
            return ($heading -replace "^\s*#\s+", "").Trim().Replace(":", " -")
        }
    }
    catch {
    }

    return [System.IO.Path]::GetFileNameWithoutExtension($repoPath).Replace("_", " ").Replace("-", " ")
}

function New-NavYaml([string[]]$files) {
    $order = @("Overview", "Runtime State", "Specs", "Development Notes", "Artifacts", "Misc")
    $lines = New-Object System.Collections.Generic.List[string]
    $lines.Add("nav:")
    foreach ($category in $order) {
        $items = $files | Where-Object { (Get-Category $_) -eq $category }
        if (-not $items) { continue }
        $lines.Add("  - ${category}:")
        if ($category -eq "Overview") {
            $lines.Add("      - `"Viewer Guide`": index.md")
        }
        foreach ($file in $items) {
            $title = Get-Title $file
            $lines.Add("      - `"$title`": $file")
        }
    }
    return ($lines -join [Environment]::NewLine)
}

function Copy-MarkdownView([string[]]$files) {
    $targetRoot = Join-Path $repoRoot $ViewDir
    $resolvedParent = (Resolve-Path $repoRoot).Path
    $targetFull = [System.IO.Path]::GetFullPath($targetRoot)

    if (-not $targetFull.StartsWith($resolvedParent, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to prepare view outside repository: $targetFull"
    }

    if (Test-Path -LiteralPath $targetFull) {
        Remove-Item -LiteralPath $targetFull -Recurse -Force
    }

    New-Item -ItemType Directory -Path $targetFull | Out-Null

    $indexSource = Join-Path $repoRoot "docs/index.md"
    Copy-Item -LiteralPath $indexSource -Destination (Join-Path $targetFull "index.md")

    foreach ($file in $files) {
        if ($file.StartsWith(".claude/", [System.StringComparison]::OrdinalIgnoreCase)) {
            continue
        }

        $source = Join-Path $repoRoot $file
        $destination = Join-Path $targetFull $file
        $destinationDir = Split-Path -Parent $destination
        if (-not (Test-Path -LiteralPath $destinationDir)) {
            New-Item -ItemType Directory -Path $destinationDir -Force | Out-Null
        }
        Copy-Item -LiteralPath $source -Destination $destination
    }

    $screenshotsSource = Join-Path $repoRoot "Assets/Screenshots"
    if (Test-Path -LiteralPath $screenshotsSource) {
        $screenshotsTarget = Join-Path $targetFull "Assets/Screenshots"
        New-Item -ItemType Directory -Path $screenshotsTarget -Force | Out-Null
        Get-ChildItem -LiteralPath $screenshotsSource -File |
            Where-Object { $_.Extension -in @(".png", ".jpg", ".jpeg", ".webp", ".gif") } |
            ForEach-Object {
                Copy-Item -LiteralPath $_.FullName -Destination (Join-Path $screenshotsTarget $_.Name)
            }
    }
}

$files = @(Get-MarkdownFiles | Where-Object { -not $_.StartsWith(".claude/", [System.StringComparison]::OrdinalIgnoreCase) })
$nav = New-NavYaml $files

if ($PrepareView) {
    Copy-MarkdownView $files
}

if ($NavOut) {
    $navPath = [System.IO.Path]::GetFullPath((Join-Path (Get-Location) $NavOut))
    $repoFull = (Resolve-Path $repoRoot).Path
    if (-not $navPath.StartsWith($repoFull, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to write nav outside repository: $navPath"
    }
    Set-Content -LiteralPath $navPath -Value $nav -Encoding UTF8
}
else {
    $nav
}
