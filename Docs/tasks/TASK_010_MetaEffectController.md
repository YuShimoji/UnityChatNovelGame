# Task: MetaEffectController (演出エフェクト) 実装
Status: OPEN
Tier: 2
Branch: main
Owner: Worker
Created: 2026-01-16T13:55:00Z
Report: docs/reports/REPORT_TASK_010_MetaEffectController.md

## Objective
画面全体に影響を与える演出エフェクト(グリッチ、フェード、ノイズ等)を管理する「MetaEffectController」を実装する。
ScenarioManagerのGlitchCommandから呼び出され、レベルに応じたエフェクトを表示する。

## Context
- **前工程**: TASK_002でGlitchCommandのスケルトンが実装済み(Debug.Logで代替中)
- **ゴール**: チャットノベルゲームの演出強化(恐怖演出、システム異常演出など)

## Focus Area
- `Assets/Scripts/UI/MetaEffectController.cs` (新規)
- `Assets/Scripts/UI/Effects/GlitchEffect.cs` (新規)
- `Assets/Prefabs/UI/MetaEffectOverlay.prefab` (新規、画面オーバーレイ)
- `Assets/Scripts/Core/ScenarioManager.cs` (GlitchCommand連携部分のみ)

## Forbidden Area
- `Assets/Scripts/UI/ChatController.cs` への変更
- `Assets/Scripts/UI/DeductionBoard.cs` への変更
- 既存のCamera設定の破壊的変更

## Constraints
- エフェクトはUI Canvas上のオーバーレイとして実装(Post Processing不使用)
- DOTweenを使用したアニメーション
- エフェクトレベル(0-3程度)に応じた強度調整

## Steps
1. MetaEffectController.cs の基本構造を実装(エフェクト開始/停止)
2. GlitchEffect.cs を実装(ノイズ/画面揺れ/色収差風)
3. MetaEffectOverlay.prefab を作成
4. ScenarioManager.GlitchCommandと連携
5. テストシーンで動作確認

## DoD (Definition of Done)
- [ ] `MetaEffectController.cs` が実装され、エフェクトの開始/停止ができる
- [ ] `GlitchEffect.cs` が実装され、レベルに応じたグリッチ演出ができる
- [ ] `MetaEffectOverlay.prefab` が作成されている
- [ ] `ScenarioManager.GlitchCommand` からエフェクト呼び出しが機能する
- [ ] Unity Editorで動作確認が完了している
- [ ] `docs/reports/REPORT_TASK_010_MetaEffectController.md` にレポートが作成されている

## 停止条件
- Post Processingが必須になった場合(要別途判断)
- パフォーマンス問題が発生した場合

## Notes
- TASK_008 (ChatUI Integration), TASK_009 (DeductionBoard) と並行実行可能
- 将来的にはフェード、ノイズ、画面揺れなど複数エフェクトを追加予定
