# Task: Fix ReImport Syntax & Encoding Errors

Status: OPEN
Tier: 1 (Emergency)
Branch: hotfix/reimport-errors
Owner: Worker
Created: 2026-01-30T17:55:00+09:00
Report: docs/reports/REPORT_TASK_047_FixReimportErrors.md

## Objective
ReImport 後に大量発生したコンパイルエラー（CS8641, CS1003, CS1525 等）を解消する。
主な原因と思われる文字コード（Encoding）の不整合と、それに伴う構文破壊を修正する。

## Context
- **Issue**: `ScenarioManager.cs` 等で文字化け（mojibake）が確認されており、日本語コメントがコードとして誤認識されている、または中括弧 `}` の欠落が発生している可能性が高い。
- **Affected Files**:
  - `Assets/Scripts/Core/ScenarioManager.cs`
  - `Assets/Scripts/Data/TopicData.cs`
  - `Assets/Scripts/Effects/GlitchEffect.cs`
  - `Assets/Scripts/UI/ChatController.cs`
  - `Assets/Scripts/Effects/MetaEffectController.cs`
  - 他 MCP 関連ファイル（Warning）

## Steps
1. **Encoding Fix**:
   - 対象ファイルのエンコーディングを確認し、`UTF-8 with BOM` に統一する。
   - 文字化けしているコメントがあれば修正（または削除）する。
   - 不正な文字（制御文字等）が混入していないか確認する。
2. **Syntax Repair**:
   - エラーログにある `else cannot start a statement` や `} expected` の箇所を確認し、欠落している中括弧 `}` や丸括弧 `)` を補完する。
   - `TopicData.cs` の `public` 識別子エラー（CS0106）など、構造的な破壊を修正する。
3. **Deprecation Fix**:
   - `ManageGameObjectCommon.cs` や `ManageScene.cs` の `FindObjectsOfType` 警告を `FindObjectsByType` に修正する（可能であれば）。優先度は低。

## DoD (Definition of Done)
- [ ] 全てのコンパイルエラー（Error）が解消されている。
- [ ] 文字化けが解消され、日本語コメントが正常に読める状態である。
- [ ] Report 作成 (`docs/reports/REPORT_TASK_047_FixReimportErrors.md`)
