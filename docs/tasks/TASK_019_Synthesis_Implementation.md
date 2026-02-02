# Task: Synthesis System Implementation

Status: DONE
Tier: 2
Branch: feat/synthesis-system
Owner: Worker
Created: 2026-01-28T23:10:00+09:00
Report: docs/reports/REPORT_TASK_019_Synthesis_Implementation.md

## Objective
DeductionBoard の核となる「推論（合成）」システムを稼働させる。
`SynthesisRecipe` ScriptableObject のアセットを作成し、`Resources/Recipes` に配置して、DeductionBoard 上での合成動作を確認する。

## Context
- **Pre-requisites**:
  - `DeductionBoard.cs`: Implemented (TASK_018)
  - `SynthesisRecipe.cs`: Implemented (Data)
  - `TopicData`: Implemented.
- **Missing**:
  - `Assets/Resources/Recipes/` ディレクトリ
  - 実際の Recipe アセット
- **Goal**: 2つの証拠（Topic）を組み合わせて新しい証拠を得るゲームループを成立させる。

## Focus Area
- `Assets/Resources/Recipes/` (New Directory & Assets)
- `Assets/Scripts/UI/DeductionBoard.cs` (Logic verification/fixes)
- `Assets/Data/Topics/` (If new test topics are needed)

## Forbidden Area
- `ChatController.cs`
- `UnityEditor` namespace in Runtime scripts

## Constraints
- **Path**: Recipes must be in `Resources/Recipes` so `DeductionBoard.LoadRecipes()` can find them.
- **Data**: Create at least 1 valid recipe pair for testing (e.g., Topic A + Topic B = Topic C).

## Steps
1. Create directory `Assets/Resources/Recipes`.
2. Create dummy `TopicData` if needed (Topic A, B, C) or use existing ones.
3. Create a `SynthesisRecipe` asset (A + B -> C).
4. Run `DebugChatScene` (or similar).
5. Open DeductionBoard, add Topic A and B.
6. Drag A onto B.
7. Verify Topic C appears.
8. Capture Evidence.

## DoD (Definition of Done)
- [ ] `Assets/Resources/Recipes` ディレクトリが作成されている
- [ ] 少なくとも1つの動作する `SynthesisRecipe` アセットが作成されている
- [ ] **Verification**:
  - [ ] ドラッグ＆ドロップで合成が成功し、新しいトピックが解放される
  - [ ] 合成結果がログに表示される
- [ ] Evidence (`docs/evidence/`) 取得
- [ ] Report 作成
