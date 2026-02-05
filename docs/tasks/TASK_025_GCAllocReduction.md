# Task: GC Alloc Reduction (Baseline Follow-up)

Status: OPEN
Tier: 2 (Performance)
Branch: feature/gc-alloc-reduction
Owner: Worker
Created: 2026-01-31T01:00:00+09:00
Report: docs/reports/REPORT_TASK_025_GCAllocReduction.md

## Objective
TASK_022のベースライン計測で観測されたGC Alloc（平均22KB/frame）を低減し、将来の最適化効果を定量化できる状態にする。

## Context
- Baseline (TASK_022):
  - FPS: Avg 184.8
  - GC Alloc: Avg 22 KB/frame (Max 23 KB/frame)
  - Memory Used: Avg 336 MB
- 計測環境: DebugChatScene / WindowsEditor

## Focus Area
- 主要対象:
  - Chat表示更新ループ（文字送り/メッセージ生成）
  - エフェクト再生（DOTween、UIアニメ）
  - ログ出力・文字列結合
- 関連確認:
  - `Assets/Scripts/Utils/PerformanceMonitor.cs`（計測継続用）

## Forbidden Area
- 仕様変更（ユーザー体験を変えるUI/演出の変更）。
- 無関係な大規模リファクタリング（構造整理は別タスク）。

## Constraints
- まず「alloc発生源の特定（Profilerでの確認）」はWorkerが行う。
- ただし、本タスク内での過度な推測変更は禁止。変更は計測結果に根拠を持つこと。
- 計測はTASK_022と同条件（DebugChatScene、10秒、1秒サンプル）で再実行し、比較を残す。

## Steps
1. Unity Profiler（Timeline / Memory）でGC Allocの発生箇所を特定。
2. 上位1〜3箇所を対象に、alloc削減の小さな変更を実施。
3. DebugChatSceneで再計測し、Before/Afterをレポートに記載。

## DoD (Definition of Done)
- [ ] 主なGC Alloc発生箇所（上位1〜3）が特定され、レポートに根拠として添付されている。
- [ ] 変更後の計測で、GC Allocがベースライン（平均22KB/frame）から低減している。
- [ ] `docs/reports/REPORT_TASK_025_GCAllocReduction.md` が作成され、Before/Afterが明記されている。
