# Task: Fix MCPForUnity Syntax Error

Status: OPEN
Tier: 3 (Hotfix)
Branch: hotfix/mcp-syntax
Owner: Worker
Created: 2026-01-30
Report: docs/reports/REPORT_TASK_046_FixMCPForUnitySyntax.md

## Objective
`Assets/MCPForUnity/Editor/Dependencies/Models/DependencyStatus.cs` のシンタックスエラーを修正し、コンパイルを通す。

## Context
- **Issue**: Line 60 に不正な文字（`✁`）が含まれており、C# の三項演算子が壊れている。
  `var status = IsAvailable ? "✁E : "✁E;`
- **Impact**: コンパイルエラーによりエディタ機能が動作しない。

## Focus Area
- `Assets/MCPForUnity/Editor/Dependencies/Models/DependencyStatus.cs`

## Steps
1. `DependencyStatus.cs` の60行目を修正する。
   - 修正案: `var status = IsAvailable ? "[OK]" : "[MISSING]";` (または適切な記号)
2. コンパイルエラーが解消することを確認する。

## DoD (Definition of Done)
- [ ] `DependencyStatus.cs` のコンパイルエラーが解消されている
- [ ] Unity Editor にエラーが出ない
