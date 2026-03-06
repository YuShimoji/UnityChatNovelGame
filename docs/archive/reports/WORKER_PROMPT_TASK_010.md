# Worker Prompt: TASK_010_MetaEffectController

あなたは Worker Agent です。以下の指示に従い、タスクを遂行してください。
Orchestrator によって定義された境界(Focus/Forbidden)を遵守することが求められます。

## 参照情報
- **チケット**: `docs/tasks/TASK_010_MetaEffectController.md` (必読)
- **SSOT**: `docs/Windsurf_AI_Collab_Rules_latest.md`
- **HANDOVER**: `docs/HANDOVER.md`
- **Context**: `AI_CONTEXT.md`
- **関連実装**: `Assets/Scripts/Core/ScenarioManager.cs` (GlitchCommand)

## ミッション
**MetaEffectController (演出エフェクト) 実装**

- `MetaEffectController.cs` を新規作成し、画面全体のエフェクト管理を実装する。
- `GlitchEffect.cs` を新規作成し、グリッチ演出(ノイズ、画面揺れ等)を実装する。
- `MetaEffectOverlay.prefab` を作成し、画面オーバーレイとして配置可能にする。
- `ScenarioManager.GlitchCommand` から呼び出せるようにする。

## 境界 (Boundaries)

### Focus Area(変更許可)
- `Assets/Scripts/UI/MetaEffectController.cs` (新規)
- `Assets/Scripts/UI/Effects/GlitchEffect.cs` (新規)
- `Assets/Prefabs/UI/MetaEffectOverlay.prefab` (新規)
- `Assets/Scripts/Core/ScenarioManager.cs` (GlitchCommand連携のみ)

### Forbidden Area(変更禁止)
- `Assets/Scripts/UI/ChatController.cs`
- `Assets/Scripts/UI/DeductionBoard.cs`
- 既存Camera設定の破壊的変更

## Definition of Done (DoD)
- [ ] `MetaEffectController.cs` が実装され、エフェクトの開始/停止ができる
- [ ] `GlitchEffect.cs` が実装され、レベルに応じたグリッチ演出ができる
- [ ] `MetaEffectOverlay.prefab` が作成されている
- [ ] `ScenarioManager.GlitchCommand` からエフェクト呼び出しが機能する
- [ ] Unity Editorで動作確認が完了している
- [ ] `docs/reports/REPORT_TASK_010_MetaEffectController.md` にレポートが作成されている

## 停止条件 (Stop & Report)
- Post Processingが必須になった場合
- パフォーマンス問題が発生した場合

## 技術ヒント
- DOTweenを使用してアニメーション(画面揺れ、アルファ変更等)
- UI Imageのcolor/materialを変更してノイズ表現
- Canvasの最前面にオーバーレイを配置

## 納品物
- 新規作成されたコード(MetaEffectController.cs, GlitchEffect.cs)
- 新規Prefab
- `docs/reports/REPORT_TASK_010_MetaEffectController.md`
