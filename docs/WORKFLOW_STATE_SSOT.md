# WORKFLOW STATE SSOT

**Updated**: 2026-03-03  
**Phase**: Content Authoring — Chapter 1 実装  
**Branch**: main

## Mission

Chapter 1（端末の貧弱さ）のコンテンツ制作と動作確認。ContentAuthoring シーンでの実プレイ可能状態の達成。

## Done 条件

- [x] Ch1 Yarnスクリプト（Ch1_Terminal.yarn）の作成
- [x] Ch1必要アセットの作成（CharacterProfile: pyramid/marco, TopicData: fragment_ch1_01）
- [x] 仕様書の更新（monetization, production_plan, open_questions）
- [x] ENGINE_FEATURE_INVENTORY.md の作成
- [x] YarnEditingPipeline.md の拡充
- [x] Ch1_DRAFT_NOTES.md の作成
- [ ] ContentAuthoring シーンでの Ch1 再生確認（手動）
- [ ] 全分岐パターンの通しテスト（最低2周）

## 現在の状態

### 完了済み（2026-03-03）
- ✅ Ch1_Terminal.yarn 作成完了（10ノード, 455行, 推定50-60メッセージ）
- ✅ CharacterProfile 作成: `pyramid`（薄い緑系 r:0.55/g:0.82/b:0.6）
- ✅ CharacterProfile 作成: `marco`（暖色系 r:0.9/g:0.55/b:0.3）
- ✅ TopicData 作成: `fragment_ch1_01`（施設管理規約（部分））
- ✅ CharacterProfileCreator.cs 更新（pyramid/marco追加）
- ✅ Yarn構文の一貫性修正（`=` → `to` 統一）
- ✅ Ch1_DRAFT_NOTES.md チェックリスト更新
- ✅ StorySpec 仕様書群の更新（10_monetization, 11_production_plan, 99_open_questions）
- ✅ ENGINE_FEATURE_INVENTORY.md 新規作成
- ✅ YarnEditingPipeline.md 大幅拡充
- ✅ 変数名衝突チェック（$ch1_プレフィックスで安全）
- ✅ ノード名衝突チェック（Ch1_プレフィックスで安全）
- ✅ Yarnプロジェクト設定確認（**/*.yarn globで自動取り込み）

### 次のアクション
1. **手動確認**: Unity ContentAuthoring シーンでの Ch1 再生テスト
2. **改善検討**: Ch1_DRAFT_NOTES.md の「今後の課題」セクション対応
3. **次フェーズ**: Ch2 設計・指摘メカニクス実装の検討

## 選別規則

当面は以下の作業分類に従い、D（将来のための品質や汎化）は凍結とします。

- A. コア機能・目的の達成
- B. 制作/開発速度の向上・互換設定
- C. 失敗からの復旧しやすさ
- D. テスト拡充、過度なレポート、当面に直結しないリファクタリング → **凍結**

## 禁止事項

- Editor-Ready 状態（1クリックでの再生確認やデバッグ表示）を損なう変更を行わないこと。
- MVP の最小導線を破壊しないこと。
- Console Error / Exception を発生させないこと。
- 過度なテスト要求、過剰なレポート生成、今の目的に直結しない汎化リファクタリングを行わないこと。
