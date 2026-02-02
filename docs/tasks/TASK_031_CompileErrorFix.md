# Task: Compile Error Fix

Status: IN_PROGRESS
Tier: 1 (Critical)
Branch: main
Owner: Cascade
Created: 2026-02-02T12:30:00+09:00
Report: N/A

## Objective
Unityプロジェクトのコンパイルエラーを修正し、Play Modeを実行可能にする。

## Context
ユーザーがPlay Modeを実行しようとしたところ、以下のコンパイルエラーが発生：

### エラー一覧

**警告（非推奨API）:**
1. `ManageGameObjectCommon.cs(133,46)`: `Object.FindObjectsOfType(Type, bool)` → `Object.FindObjectsByType`に置き換え
2. `ManageScene.cs(400,36)`: `Object.FindObjectsOfType<T>()` → `Object.FindObjectsByType`に置き換え
3. `PerformanceBenchmark.cs(36,17)`: `Object.FindObjectOfType<T>()` → `Object.FindFirstObjectByType`に置き換え

**エラー（実装不足）:**
4. `ScenarioManager.cs(53,38)`: `DialogueRunner.NodeExists`が存在しない
5. `ChatScenarioTester.cs(16,46)`: 引数型の不一致（`ChatScenarioData` → `string`）
6. `DeductionBoardVerification.cs(52,29)`: `ScenarioManager.StartScenario`が見つからない（実装済みのはず）
7. `DeductionBoard.cs(274,45)`: `ScenarioManager.SetVariable`が見つからない（実装済みのはず）
8. `DeductionBoard.cs(282,81)`: `MetaEffectController.PlayEffect`が見つからない（実装済みのはず）

## Scope
- 非推奨API警告の修正（MCPForUnity、PerformanceBenchmark）
- Yarn Spinner API互換性の確認と修正
- 実装済みメソッドが認識されない原因の特定と修正

## Constraints
- 既存の機能を破壊しない
- TASK_029/030（ワーカー依頼済み）とは独立して対処

## Steps
1. 非推奨API警告を修正（Tier 1: 安全な置き換え）
2. `DialogueRunner.NodeExists`の使用箇所を特定し、代替実装を検討
3. `ChatScenarioTester`の引数型を確認
4. 実装済みメソッドが認識されない原因を調査（名前空間、アクセス修飾子など）
5. コンパイルエラーをすべて解消
6. Unity Editorでコンパイル成功を確認

## DoD (Definition of Done)
- [ ] すべてのコンパイルエラーが解消されている
- [ ] Unity EditorでPlay Modeが実行可能
- [ ] 既存機能が正常に動作する
