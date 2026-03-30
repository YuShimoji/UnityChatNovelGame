# Runtime State

**Updated**: 2026-03-30 session 16

## Current Position

- project: FoundPhone (UnityChatNovelGame)
- branch: main
- lane: Advance (サブスレッド安定化 + アルゴリズム明文化)
- slice: スレッド切替フラッシュ防止 + スムーズスクロール + アルゴリズム仕様文書化
- active_artifact: FoundPhone エンジン基盤 (Unity 6.3 + Yarn Spinner 3.1.3)
- artifact_surface: Unity Editor > ContentAuthoring シーン
- last_change_relation: direct (SwitchToThread フェードイン + PerformAutoScroll スムーズ化)

## Counters

- blocks_since_user_visible_change: 0 (スレッド切替フェードイン + スムーズスクロール)
- blocks_since_visual_audit: 4 (session 13 Block 1 Audit → session 14 x2 → session 15 → session 16)
- blocks_since_unlock: 1
- consecutive_excise_blocks: 0

## Quantitative Metrics

- impl_files: 65 (テスト除く .cs)
- test_files: 5 (EditMode 4 + PlayMode 1)
- playmode_test_files: 1
- mock_files: 0
- yarn_active: 9
- yarn_archive: 1
- spec_entries: 39 (done 23 / partial 9 / draft 1 / todo 5 — EN-011追加, BL-001/002/003追加)
- todo_fixme_hack: 1 (ChatController.cs — ステータスバールーティング)
- obsolete_marks: 2 (ContradictionPair.UnlockTopic x2)
- chatcontroller_lines: ~2640 (SwitchToThread改修 + スムーズスクロール追加)
- wiki_pages: 12 (docs/wiki/ 内)

## Visual Evidence

- visual_evidence_status: stale (Unity PlayMode 未実施。コード変更のみ)
- last_visual_audit_path: (なし)
- blocks_since_visual_audit: 4

## Session Log

### 2026-03-30 session 16
- Block 1 (Advance): アルゴリズム明文化 + サブスレッド安定化実装
  - docs/DISPLAY_ALGORITHMS.md 新規作成: メッセージ表示/スキップ/スクロール/スレッド切替の全フロー明文化
  - Backlog 3件登録: BL-001(スクロールフェード), BL-002(ポートレートアイコン), BL-003(スレッドメタデータ)
  - **SwitchToThread フラッシュ防止**: CanvasGroup alpha=0 → RestoreChatHistory → 2フレーム待機 → スクロール復元 → フェードイン(0.15秒)
  - **PerformAutoScroll スムーズ化**: verticalNormalizedPosition 直接代入 → DOTween 0.2秒アニメーション (BL-001 部分対応)
  - **2段階タップスキップ**: 第1タップ=テキスト全文表示(PostMessageDelay継続)、第2タップ=次メッセージへ進む
    - ChatDialogueView: m_PostSkipCts 新設、DelayWithPostSkip 追加、RunLineAsync を Phase 1/Phase 2 に分離
  - **OnDisable クリーンアップ強化**: ScrollTween/FadeInCoroutine/PostSkipCts の解放追加
  - ユーザーフィードバック吸収: デザイン制御不足 / 明文化なき実装の禁止 / サブスレッド不具合4件の原因特定

### 2026-03-30 session 15
- Block 1 (Unlock): UIFontConfig 新設 + ハードコードフォントサイズ統合
  - nightshift (session 14) のフォントサイズ変更 (d584aaf, 8835623) を revert — 部分的でバランス崩れ
  - UIFontConfig.cs 新設: 7段階フォント階層 (title/heading/subheading/body/caption/small/tiny) + レスポンシブスケール
  - ハードコード値を UIFontConfig 参照に置換: DashboardController(10), InventoryTabController(7), TransferSelectionUI(5), ProgressSummaryUI(3), ChatController(3+GetResponsiveFontScale委譲), ContradictionFeedbackController(3)
  - 未変更: ThreadSwitcherController (サイドバー密レイアウト、別途設計要), ChatDialogueView (debug), DebugHubController (debug)
  - **ユーザー報告 (HANDOFF 時)**:
    1. revert後もメッセージが大きく見える (ディスク上は28に戻っているがUnityキャッシュ未反映の可能性)
    2. タップスキップが複数クリック必要な場合がある (タイミング依存)
    3. システムメッセージにタップスキップが効かない (一貫性の欠如)
    → 次セッションで Audit + 修正が必要

### 2026-03-29 session 14 (nightshift)
- Block 1 (Advance): session 13 修正コミット + メッセージ演出改善
  - RestoreChatHistory アニメ・タイプライタースキップ (session 13 確認済み)
  - StopScenario 同期化 + 二重クリック防止 (session 13)
  - **タップスキップ実装**: ChatDialogueView に画面タップでアニメスキップ + DelayWithSkip
  - **タイミング設定可能化**: TypingIndicatorDuration (0.8s), PostMessageDelay (0.4s)
  - ChatController: CompleteCurrentTypewriter() + IsTypewriterActive 公開
- Block 2 (Advance + Unlock): 分岐安定化 + wiki
  - Ch1_Day1.yarn: $d1_did_branch_pyramid 再入防止フラグ追加
  - ScenarioManager: BeginBranch 再入時に古い履歴クリア
  - Pyramid のメタ発言「別スレッドです」削除
  - docs/StorySpec/21_branch_thread_spec.md 新規作成
  - **docs/wiki/ 作成**: Docsify ベースの統合リファレンス (12ページ)
    - quick-start / commands / characters / branch / workflow / chapter-patterns / tools / ui-config / save-system / troubleshooting
  - ChatUIConfig フォントサイズ引き上げ (messageFontSize 28→34 等)

### 2026-03-28/29 session 13
- Block 1 (Audit): Unity Audit — git pull (session 12 取込) + コンパイルエラー修正 + obsolete警告修正
  - VerificationMenu.cs: using Assets.Scripts.Dev 追加 (CS0246 解消)
  - enableWordWrapping → textWrappingMode 5箇所 (CS0618 x5 解消)
  - YarnSOGenerator 動作確認: Topic 14参照(既存16, 欠落6) / Speaker 7参照(既存7, 欠落0)
  - ContentAuthoring シーン再生: 通常フロー正常、選択肢後のバグを確認 (次ブロック対応)
  - runtime-state.md メトリクス精査: impl 67→64, test 6→5
- Block 2 (Advance): 選択肢共通処理バグ修正 — StopScenario/StartScenario 競合 + UI堅牢化
  - **根本原因修正**: StopScenario() を同期実行に変更 (StopDialogueDeferred 廃止)
  - StartScenario() に安全弁追加: 実行中ダイアログを強制停止してから開始
  - ChoiceButtonHandler: クリック時に全ボタン即座に interactable=false (二重クリック防止)
  - FadeAndHideChoices: m_IsFadingChoices 再入ガード追加

### 2026-03-27 session 12
- Block 1 (Advance): Pipeline設計確定 — 出力形態/プラットフォーム/自動化/サウンド/マネタイズの5決定
- Block 2 (Unlock): YarnSOGenerator.cs 新規作成 (Editor ツール)

### 2026-03-26 session 11 nightshift
- Block 1-5: 各種 Excise/Advance/Unlock (ChatController 2753→2549行)

### 2026-03-26 session 10 nightshift
- 全コードベース監査 + レガシー19件削除 + FEATURE_STATUS_AUDIT.md作成
