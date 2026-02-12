# TASK_MVP_04_VerifyVerticalSlice

## Status
OPEN

## Tier / Branch
- Tier: 3 (Verification)
- Branch: main

## Objective
MVP縦切り（Title→Play→Choice→End）が60秒以内に完走できることを、証跡付きで確認する。

## Focus Area
- `Assets/Scenes/MVPScene.unity`
- `Assets/Scripts/MVP/MVPGameController.cs`
- `docs/AI_CONTEXT_MVP.md`

## Forbidden Area
- 既存ロジックのリファクタリング
- 外部依存の追加
- UI/演出の大幅変更

## Constraints
- MVP最短導線の維持
- Console Error/Exception 0 を維持
- 連打耐性の確認は必須

## DoD
- [ ] Title→Play→Choice→End の完走を確認
- [ ] 通し時間が60秒以内
- [ ] Choiceの両分岐でEnd到達
- [ ] 連打時に二重遷移・停止不全がない
- [ ] Console Error/Exception 0
- [ ] 証跡（スクリーンショットまたは動画）を `docs/evidence/` に保存
- [ ] `docs/AI_CONTEXT_MVP.md` のチェックリストを更新

## Test Plan
- **対象**: MVPScene / MVPGameController
- **種別**: PlayMode（手動）
- **期待結果**:
  - Title→Play→Choice→End の一連が完走する
  - 連打しても破綻しない
  - Console Error/Exception が0件
- **テスト不要項目**: EditMode/Build は本タスクではコード変更が発生しないため対象外

## Milestone
- SG-1: MVP縦切りの最終確認

## Notes
- 証跡取得は `docs/AI_CONTEXT.md` の運用ルールに従う
