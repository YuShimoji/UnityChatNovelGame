# Task: MetaEffectController Implementation

Status: DONE
Tier: 2
Branch: feat/meta-effect-controller
Owner: Worker
Created: 2026-01-17T02:00:00+09:00
Updated: 2026-03-01T06:10:00+09:00
Report: docs/reports/REPORT_TASK_010_MetaEffectController.md

## Objective

メタ演出（グリッチ効果等）を制御する `MetaEffectController` を実装し、`ScenarioManager.GlitchCommand` から呼び出せるようにする。

## Current Status

- `Assets/Scripts/Effects/MetaEffectController.cs` が存在する
- `ScenarioManager.GlitchCommand` から `MetaEffectController.Instance.PlayGlitchEffect(level)` が呼び出される
- `TASK_020` で `PlayEffect("Sparkle")` 系の汎用再生基盤まで拡張済み
- current automation では `Sparkle` prefab 不在 warning が residual observation として記録されているが、タスク自体の実装完了判定は妨げない

## DoD (Definition of Done)

- [x] `MetaEffectController.cs` が実装されている
- [x] `PlayGlitchEffect(int level)` が実装されている
- [x] `ScenarioManager.GlitchCommand` から呼び出せる
- [x] effect replay 基盤が `TASK_020` で拡張されている
- [x] `docs/reports/REPORT_TASK_010_MetaEffectController.md` に完了根拠が記録されている

## Evidence

- `Assets/Scripts/Effects/MetaEffectController.cs`
- `Assets/Scripts/Core/ScenarioManager.cs`
- `docs/reports/REPORT_TASK_020_DeductionBoard_Effects.md`
- `docs/reports/REPORT_TASK_027_FullPlaythroughTest.md`
