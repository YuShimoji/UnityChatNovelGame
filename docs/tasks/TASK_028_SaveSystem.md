# Task: Save System Implementation

Status: COMPLETED
Tier: 2 (Feature)
Branch: feature/save-system
Owner: Worker
Created: 2026-02-02T04:00:00+09:00
Report: N/A

## Objective
ゲーム進行状況を保存・読み込みできるセーブシステムを実装する。

## Context
- 現状: セーブ機能なし（毎回最初から開始）。
- ユーザー体験向上のため、中断・再開機能が必要。
- 中期目標として計画されていた。

## Scope
保存対象:
- シナリオ進行状況（現在のノード/ダイアログ位置）
- 獲得済み Topic 一覧
- Deduction Board の状態
- Synthesis で作成済みのアイテム

## Constraints
- JSON形式でローカル保存（`Application.persistentDataPath`）。
- 暗号化は任意（Phase 1 では平文可）。
- 複数スロット対応は任意（Phase 1 では 1 スロットで可）。

## Steps
1. SaveData クラスの設計（保存対象の構造定義）。
2. SaveManager の実装（Save/Load/Delete）。
3. 各システム（ScenarioManager, TopicData, DeductionBoard）との連携。
4. UI（セーブ/ロードボタン）の追加。
5. 動作検証。

## DoD (Definition of Done)
- [x] SaveManager が実装されている。
- [x] Save/Load が正常に動作する（Evidence 添付）。
- [x] ゲーム再起動後もデータが保持される。
- [x] レポートが `docs/reports/REPORT_TASK_028_SaveSystem.md` に作成されている。
