# Task: Deduction Board Polish (Effects)

Status: DONE
Tier: 3 (Polish)
Branch: feat/deduction-polish
Owner: Worker
Created: 2026-01-29T09:00:00+09:00
Report: docs/reports/REPORT_TASK_020_DeductionBoard_Effects.md

## Objective
DeductionBoard でトピック合成が成功した際に、視覚的なフィードバック（MetaEffect）を再生し、ユーザー体験を向上させる。
`MetaEffectController` (TASK_010) を使用して、画面全体または特定の座標にエフェクトを表示する。

## Context
- **Dependencies**:
  - `TASK_010`: MetaEffectController (Implemented)
  - `TASK_018`: DeductionBoard (Implemented)
  - `TASK_019`: Synthesis Logic (In Progress)
- **Goal**: "新しい発見" の瞬間の喜びを演出する。

## Focus Area
- `Assets/Scripts/UI/DeductionBoard.cs`: 合成成功判定箇所
- `Assets/Resources/Effects/` (もし新規エフェクトが必要なら)
- `MetaEffectController` との連携

## Constraints
- **Performance**: エフェクトがゲームプレイを阻害しないこと。
- **Reusability**: 汎用的なエフェクト（"Sparkle", "Confetti" 等）を使用または作成する。

## Steps
1. `DeductionBoard.cs` の `CheckSynthesis` メソッド（合成成功時）等の適切な箇所を特定する。
2. `MetaEffectController.Instance.PlayEffect(...)` を呼び出す処理を追加する。
3. エフェクト生成位置を調整する（ドラッグ&ドロップした地点、または画面中央）。
4. 実機（Editor）で動作確認を行う。

## DoD (Definition of Done)
- [ ] 合成成功時に MetaEffect が再生される
- [ ] エフェクトが適切な位置で表示される
- [ ] エラーが出ないこと（MetaEffectController が存在しない場合のカリング）
- [ ] Report 作成
