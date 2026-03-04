# WORKFLOW STATE SSOT

**Updated**: 2026-03-04
**Phase**: Content Authoring — Chapter 1 & 2 統合 + UIバグ修正
**Branch**: main
**HEAD**: `e569c9d`

## Mission

Chapter 1-2 のコンテンツ統合とUIバグ修正。ContentAuthoring シーンでの Ch1/Ch2 プレイテスト準備完了。

## Done 条件

- [x] Ch1 Yarnスクリプト（Ch1_Terminal.yarn）の作成
- [x] Ch1必要アセットの作成（CharacterProfile: pyramid/marco, TopicData: fragment_ch1_01）
- [x] Ch2 Yarnスクリプト（Ch2_LocationConfusion.yarn）の統合
- [x] Ch2必要アセットの統合（CharacterProfile: bernardo/mason/oliver, TopicData: fragment_ch1_02-04, ch2_01-03）
- [x] 矛盾ペアアセットの統合（Ch1 x4, Ch2 x3 + ContradictionDatabase）
- [x] UI演出の統合（タイプライター効果、入力中表示、選択肢レイアウト修正）
- [x] 色薄バブル問題の修正（プール再利用時の色汚染防止、デフォルトNPC色修正）
- [x] 選択肢重複表示の修正（DestroyImmediate、onClick リスナークリア）
- [x] 仕様書の更新（monetization, production_plan, open_questions）
- [x] ENGINE_FEATURE_INVENTORY.md の作成
- [ ] ContentAuthoring シーンでの Ch1/Ch2 再生確認（手動）
- [ ] 全分岐パターンの通しテスト（最低2周）

## 現在の状態

### 完了済み（2026-03-04）
- ✅ feature/task-049 ブランチから有用な変更を選択的マージ（`a1c76f9`）
  - Ch2_LocationConfusion.yarn（7ノード, 313行）
  - CharacterProfile: bernardo/mason/oliver
  - ContradictionPair: Ch1 x4 + Ch2 x3
  - TopicData: fragment_ch1_02-04, ch2_01-03
  - ChatController: タイプライター効果、入力中表示、選択肢修正
  - ChatDialogueView: システムメッセージバブルサイズ、スクロール吸着
- ✅ S2 色薄バブル修正（`6e83e78`）
  - MessageBubble.SyncOriginalColor() 追加
  - MessageBubblePool.Return() で Image.color リセット
  - CharacterDatabase デフォルトNPC色を (0.3, 0.3, 0.35) に変更
- ✅ S3 選択肢重複表示修正（`6e83e78`）
  - HideChoices: DestroyImmediate で即座削除
  - ShowChoices: onClick.RemoveAllListeners() でリスナー蓄積防止

- P1 タイプライター文字化け修正
  - DOTween.Kill(complete:true) で前回 tween を確実に完了
  - SetTarget + OnComplete でライフサイクル管理
  - MessageBubblePool.Return() で maxVisibleCharacters リセット
- P2 ClearMessages プール汚染修正
  - ラッパー（PlayerRow/NpcRow）をプールに返却していたバグを修正
  - ReturnAll() で追跡済みバブルを正しく返却、空ラッパーは Destroy
  - TypingIndicator をリセットし次回使用時に再生成
- P3 システムメッセージフォントサイズ修正（16pt、0.85x 乗算を除去）
- P4 Debug Hub ノード順序をチャプター順に変更（GetChapterOrder）
- ✅ QA フィードバック対応（`a761848`）
  - Debug Hub にチャプターヘッダー追加（視覚的な区切り）
  - システムメッセージフォント: プール再利用時も常時適用に修正
  - スクロール: LateUpdate による連続ピンニングに変更（タイプライター中の巻き戻し防止）
  - バブル状態汚染: RectTransform/CSF をプール Return + CreateMessageBubble の両方でリセット
- ✅ ChatUIConfig ScriptableObject 作成（`e569c9d`）
  - ハードコード値28箇所を SO 参照に置換（フォールバック付き）
  - Inspector から一覧・調整可能

### 次のアクション
1. **Unity 内作業**: ChatUIConfig アセットを作成（ProjectFoundPhone > Chat UI Config）し `Assets/Resources/` に配置
2. **手動確認**: ContentAuthoring シーンでの Ch1/Ch2 再生テスト（全修正の確認）
3. **次フェーズ検討**: 矛盾 Phase 2（UIフィードバック/アニメーション）
4. **UI拡張**: フラグメント一覧画面、ダッシュボード型メイン画面
5. **レイアウト**: スマホサイズ基準への移行

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
