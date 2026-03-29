# Runtime State

**Updated**: 2026-03-29

## Current Position

- project: FoundPhone (UnityChatNovelGame)
- branch: main
- lane: Advance (メッセージ演出改善 + 分岐安定化 + 開発インフラ)
- slice: 選択肢処理安定化 + メッセージ演出 + Authoring Wiki
- active_artifact: FoundPhone エンジン基盤 (Unity 6.3 + Yarn Spinner 3.1.3)
- artifact_surface: Unity Editor > ContentAuthoring シーン + docs/wiki/
- last_change_relation: direct (メッセージ演出改善、分岐バグ修正、wiki作成)

## Counters

- blocks_since_user_visible_change: 0 (メッセージタイミング + タップスキップ + フォントサイズ修正)
- blocks_since_visual_audit: 1 (session 13 Block 1 Audit)
- blocks_since_unlock: 0 (Authoring Wiki = 開発インフラ unlock)
- consecutive_excise_blocks: 0

## Quantitative Metrics

- impl_files: 64 (テスト除く .cs)
- test_files: 5 (EditMode 4 + PlayMode 1)
- playmode_test_files: 1
- mock_files: 0
- yarn_active: 9
- yarn_archive: 1
- spec_entries: 35 (done 22 / partial 9 / draft 1 / todo 2 + 21_branch_thread_spec 新規)
- todo_fixme_hack: 1 (ChatController.cs:1233 — 行番号ズレ修正、ステータスバールーティング)
- obsolete_marks: 2 (ContradictionPair.UnlockTopic x2)
- chatcontroller_lines: 2577 (session 13: 2549 → +28行 バグ修正追加)
- wiki_pages: 12 (docs/wiki/ 内)

## Visual Evidence

- visual_evidence_status: unknown (verification/ にファイル実体なし — task-scout 指摘)
- last_visual_audit_path: (なし — ユーザー提供スクリーンショットは外部、ファイル未保存)
- blocks_since_visual_audit: 2

## Session Log

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
