# Task: Build Gate Fix for Vertical Slice

Status: OPEN
Tier: 2 (Stabilization)
Branch: feature/task-049-build-gate-fix
Owner: Worker
Created: 2026-02-16
Updated: 2026-02-16
Report: docs/reports/REPORT_TASK_049_BuildGateFix_VerticalSlice.md

## Objective

縦切り導線のビルド阻害要因を解消し、PlayMode/Build 検証を再開可能な状態にする。

## Milestone

- SG-1: MVP縦切りの最終確認
- MG-1: MVP安定化と最低限の品質基盤

## Focus Area

- `Assets/Scripts/Editor/`
- `Assets/Scripts/MVP/Editor/`
- `Assets/Scripts/Debug/Editor/`
- `Assets/Scripts/*.asmdef`
- `docs/evidence/TASK_047/Build.log`

## Forbidden Area

- 仕様拡張（新機能追加）
- シナリオ本文の改稿
- パフォーマンス最適化の深掘り

## Constraints

- 目的は「ビルドを通すこと」に限定する
- ランタイム挙動（ゲームフロー）を変更しない
- EditorOnly コードの分離を優先する

## DoD

- [ ] `Build.log` で出ている Editor 名前空間系のコンパイルエラーが解消される
- [ ] Unity Editor でコンパイルエラー 0 を確認する
- [ ] Windows ビルドが成功する（`Builds/Windows/TinyChatNovel.exe` 生成）
- [ ] 変更理由を `docs/reports/REPORT_TASK_049_BuildGateFix_VerticalSlice.md` に記録する

## Test Plan

- テスト対象:
  - Editor asmdef の参照関係
  - EditorOnly スクリプトのコンパイル境界
- テスト種別:
  - EditMode（コンパイル確認）
  - ビルド検証（Windows）
- 期待結果:
  - `error CS0234 / CS0246` が再発しない
  - ビルド成果物が生成される
- テスト不要項目:
  - PlayModeのゲーム体験検証（本タスク対象外）

## Stop Conditions

- `ProjectSettings/` や `Packages/` の変更が必須になる
- 複数のランタイムasmdef再設計が必要になる
- Build環境依存で再現不能なエラーのみが残る
