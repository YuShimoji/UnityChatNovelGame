# Engine Feature Inventory

実装詳細の羅列をやめ、参照先を示す索引に整理した版。

## 1. まず見る順番

1. `docs/project-context.md`（今の優先軸）
2. `docs/HANDOFF.md`（再開手順）
3. `docs/spec-index.json`（仕様状態）

## 2. 機能カテゴリ

- **Yarn コマンド仕様**: `docs/YarnEditingPipeline.md`
- **UI 実装詳細**: `docs/UI_IMPLEMENTATION_SPEC.md`
- **機能実装監査**: `docs/FEATURE_STATUS_AUDIT.md`
- **改善候補（ENH）**: `docs/FEATURE_REGISTRY.md`

## 3. 現行で重視する機能

- Content Pipeline / YarnSOGenerator
- PlayMode 8件 + EditMode 75件
- StartNode 安全化（missing node 回避）

## 4. 使い分け

- 実装の正否を確認したい: `FEATURE_STATUS_AUDIT.md`
- UI パラメータを確認したい: `UI_IMPLEMENTATION_SPEC.md`
- 執筆時のコマンドを確認したい: `YarnEditingPipeline.md`

## 5. メモ

- 本ファイルは「長い仕様書」ではなく「索引」。詳細をここへ再蓄積しない。
