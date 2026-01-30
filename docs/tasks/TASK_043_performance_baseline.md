# Task: Performance Baseline

Status: OPEN
Tier: 3
Branch: feature/perf-baseline
Owner: Worker
Created: 2026-01-30T11:15:00+09:00
Report: docs/reports/REPORT_TASK_043_performance_baseline.md

## Objective
アプリケーションの動作負荷（ロード時間、FPS、メモリ使用量）を計測し、ベースラインとして記録する。
将来的な最適化の効果測定に使用する。

## Context
- **Current State**: コア機能の実装が完了し、動作可能な状態。
- **Goal**: 主要メトリクスを定量化する。
- **Tooling**: Unity ProfilerRecorder API または System.Diagnostics を使用。

## Focus Area
- `Assets/Scripts/Debug/PerformanceBenchmark.cs` (新規作成)
- `docs/reports/PERFORMANCE_BASELINE.md` (計測結果)

## Forbidden Area
- 既存のゲームロジックの変更（計測用スクリプトの追加のみ）

## Steps
1. `Assets/Scripts/Debug/PerformanceBenchmark.cs` を作成する。
   - `Using UnityEngine.Profiling`
   - Start時に時間計測、UpdateでFPS計測、OnDestroy等でレポート出力。
2. `DebugChatScene` に `PerformanceBenchmark` コンポーネントを追加する（一時的、またはPrefabs化）。
3. シーンを実行し、自動的またはキー入力で計測を終了・ログ保存させる。
4. 生成されたログを `docs/reports/` に配置する。

## DoD (Definition of Done)
- [ ] `PerformanceBenchmark.cs` が作成されている
- [ ] 計測が実行され、以下の項目を含むレポートが作成されている:
  - [ ] Boot Time (Scene Load ~ Ready)
  - [ ] Average FPS (10s run)
  - [ ] Max Memory Warning (if any)
- [ ] `docs/reports/PERFORMANCE_BASELINE_YYYYMMDD.md` が存在する
- [ ] Report 作成 (`docs/reports/REPORT_TASK_043_performance_baseline.md`)
