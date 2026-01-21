<<<<<<< HEAD
# Task: MetaEffectController Implementation

Status: DONE
Tier: 2
Branch: feat/meta-effect-controller
Owner: Worker
Created: 2026-01-17T02:00:00+09:00
Report: docs/reports/REPORT_TASK_010_MetaEffectController.md 

## Objective
メタ演出（グリッチ効果等）を制御する `MetaEffectController` を実装する。
`ScenarioManager` の `GlitchCommand` から呼び出されるグリッチ効果システムを構築する。

## Context
- `ScenarioManager.cs` の `GlitchCommand(int level)` が実装済みだが、現在は Debug.Log のみ
- `GlitchCommand` は `MetaEffectController.Instance.PlayGlitchEffect(level)` を呼び出す想定
- グリッチ効果は画面全体にノイズを表示する演出（レベル1-5程度を想定）
- 将来的に他のメタ演出（画面揺れ、色調変更等）も追加可能な設計が必要

## Focus Area
- `Assets/Scripts/Effects/MetaEffectController.cs`: シングルトンまたは静的アクセサを持つコントローラー
- `Assets/Scripts/Effects/GlitchEffect.cs`: グリッチ効果の実装（ShaderまたはPost-Processing）
- `Assets/Materials/Effects/GlitchMaterial.mat`: グリッチ用マテリアル（必要に応じて）
- Unity Post-Processing Stack または Shader Graph を使用した実装
- レベルに応じた強度調整（1-5程度）

## Forbidden Area
- 既存の `ChatController` や `ScenarioManager` の破壊的変更
- 過度な視覚効果（まずは基本的なグリッチ効果のみ）
- 外部アセットの追加（Unity標準機能または既存パッケージのみ使用）

## Constraints
- テスト: 主要パスのみ（GlitchCommandからの呼び出し確認）
- フォールバック: 新規追加禁止（既存のコマンドハンドラとの連携を維持）
- Unity 2022.3 LTS 以降に対応
- パフォーマンス: 60fps維持を目標（重い処理は避ける）

## DoD (Definition of Done)
- [x] `MetaEffectController.cs` が実装されている（シングルトンまたは静的アクセサ）
- [x] `PlayGlitchEffect(int level)` メソッドが実装されている
- [x] `ScenarioManager.GlitchCommand` から `MetaEffectController.Instance.PlayGlitchEffect(level)` を呼び出せる
- [x] レベル1-5に応じたグリッチ強度が反映される
- [x] Unity Editor 上で動作確認ができる（DebugScript.yarn の `<<Glitch>>` コマンドで確認）
- [ ] **Evidence**: グリッチ効果のスクリーンショットまたは動画（ユーザー確認が必要）
- [x] `docs/inbox/` にレポート (`REPORT_TASK_010_MetaEffectController.md`) が作成されている
- [x] 本チケットの Report 欄にレポートパスが追記されている

## Notes
- Status は OPEN / IN_PROGRESS / BLOCKED / DONE を想定
- BLOCKED の場合は、事実/根拠/次手（候補）を本文に追記し、Report に docs/inbox/REPORT_...md を必ず設定
- グリッチ効果の実装方法は Shader、Post-Processing、または UI Image のマテリアル変更など、Unity標準機能で実現可能な方法を選択すること
- 将来的な拡張性（他のメタ演出追加）を考慮した設計を推奨
=======
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
>>>>>>> origin/main
