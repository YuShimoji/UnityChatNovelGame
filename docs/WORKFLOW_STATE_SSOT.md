# WORKFLOW STATE SSOT

**Updated**: 2026-03-04
**Phase**: Content Authoring — Chapter 2 実装完了
**Branch**: feature/task-049-build-gate-fix

## Mission

Chapter 1（端末の貧弱さ）のコンテンツ制作と動作確認。ContentAuthoring シーンでの実プレイ可能状態の達成。

## Done 条件

- [x] Ch1 Yarnスクリプト（Ch1_Terminal.yarn）の作成
- [x] Ch1必要アセットの作成（CharacterProfile: pyramid/marco, TopicData: fragment_ch1_01）
- [x] 仕様書の更新（monetization, production_plan, open_questions）
- [x] ENGINE_FEATURE_INVENTORY.md の作成
- [x] YarnEditingPipeline.md の拡充
- [x] Ch1_DRAFT_NOTES.md の作成
- [x] ContentAuthoring シーンでの Ch1 再生確認（手動） ✅ 2026-03-04
- [ ] 全分岐パターンの通しテスト（最低2周）← 残タスク

## 現在の状態

### 完了済み（2026-03-04）
- ✅ Ch1_Terminal.yarn 作成完了（10ノード, 455行, 推定50-60メッセージ）
- ✅ CharacterProfile 作成: `pyramid`, `marco`
- ✅ TopicData 作成: `fragment_ch1_01`
- ✅ **Ch1実プレイ動作確認完了**
- ✅ **F12デバッグモード実装**（DebugHubController追加）
- ✅ **MessageBubblePool実装**（バブルプーリング有効化）
- ✅ **選択肢システム修正**（DialogueException解消）
- ✅ **バブル表示最適化**（高さ・幅の動的調整）
- ✅ リモートmainブランチのマージ（矛盾指摘メカニクス統合）
- ✅ ENGINE_FEATURE_INVENTORY.md, YarnEditingPipeline.md 更新
- ✅ **選択肢2行問題の修正**（Ch1_Terminal.yarn全18箇所から<<Message>>削除）
- ✅ **入力中表示の実装**（NPC発話前に"..."表示、ランタイム自動生成）
- ✅ **テキスト逐次表示の実装**（タイプライター効果、0.05秒/文字）
- ✅ **Ch1全分岐テスト実施**（2周テスト完了、問題を特定）
- ✅ **UI問題修正**（システムメッセージバブルサイズ、スクロール吸着、タイミング）
- ✅ **Ch2設計・実装完了**（CharacterProfile×3、TopicData×3、Yarnシナリオ8ノード）
- ✅ **Ch2矛盾システム実装完了**（ContradictionPair×3、ContradictionDatabase、#line:タグ追加）

### 次のアクション
1. **矛盾システムの Scene 統合**: ContradictionDatabase を ContradictionManager に接続（Unity Editor 作業）
2. **Ch2 実プレイ確認**: Ch2_LocationConfusion.yarn の動作確認と矛盾検出テスト
3. **バブルUI微調整**: 必要に応じて spacing/padding の調整（保留中）
4. **次フェーズ**: Ch3 設計 または メカニクス拡張

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
