# Feature Status Audit

**監査日**: 2026-03-30
**監査対象**: FoundPhone (UnityChatNovelGame) 全コードベース
**ブランチ**: main

---

## 1. 定量サマリー

| 項目 | 数値 |
|------|------|
| 実装ファイル (.cs) | 78 (テスト含む) |
| テストファイル | 9 (EditMode 7 + PlayMode 2) |
| Yarnファイル (active) | 9 |
| Yarnファイル (archive) | 1 |
| Yarnコマンド | 24 |
| 仕様エントリ (spec-index) | 34 (done 22 / partial 9 / draft 1 / todo 2) |
| TODO/FIXME/HACK | 1件 (ChatController.cs:1296) |
| [Obsolete] マーク | 2件 (ContradictionPair.UnlockTopic x2) |
| docs ファイル数 | ~25 (archive除く) |

---

## 2. 実装済み機能一覧

| # | 機能 | 仕様ID | 検証状態 | 検証手段 |
|---|------|--------|----------|----------|
| 1 | メッセージ表示 (Message/SystemMessage/Image) | EN-001 | コードレビュー済 | Unity再生 |
| 2 | 演出 (StartWait/SkipWait/Typing/Glitch) | EN-001 | コードレビュー済 | Unity再生 |
| 3 | トピック解放 (UnlockTopic) | EN-001 | コードレビュー済 | Unity再生 |
| 4 | Day終了処理 (EndDay) | EN-001 | コードレビュー済 | Unity再生 |
| 5 | サブスレッド宣言・表示 (DeclareThread/Typed/AddThreadMessage/Chat) | SP-016 | コードレビュー済 | Unity再生 |
| 6 | 潜在スレッド (DeclareThreadLatent/LatentCond/Manifest/Complete) | SP-016 | コードレビュー済 | Unity再生 |
| 7 | 分岐スレッド (BeginBranch/EndBranch/SetBranchReflection) | SP-014 | コードレビュー済 | Unity再生 |
| 8 | 知識転送選択UI (EndBranch "select") | SP-014 | コードレビュー済 | Unity再生 |
| 9 | 断片発見 (DiscoverFragment/AddFragmentNote) | SP-007 | コードレビュー済 | Unity再生 |
| 10 | HalluciCoin加算 (AddHalluciCoin) | SP-006 | コードレビュー済 | Unity再生 |
| 11 | 矛盾発見システム (#line:タグ + 長押し指摘) | SP-014 | タグ突合済 | Unity再生 (手動操作) |
| 12 | HCゲート (ChannelData.RequiredHalluciCoin) | SP-006 | コードレビュー済 | Unity再生 |
| 13 | ダッシュボード (チャプター選択/HC表示/Day Resume) | SP-008 | 動作確認済 (2026-03-11) | Unity再生 |
| 14 | インベントリUI (3サブタブ/オーバーレイ) | SP-008 | 動作確認済 (2026-03-11) | Unity再生 |
| 15 | チャットバブル表示 (角丸/名前分離/色/スクロール吸着) | EN-006 | 動作確認済 (2026-03-11) | Unity再生 |
| 16 | サイドバースレッド一覧 (左スライドイン/ThreadType分類) | SP-016 | コードレビュー済 | Unity再生 |
| 17 | 通知バナー (スレッドメッセージ/DeclareThread) | SP-016 | コードレビュー済 | Unity再生 |
| 18 | 種別差異レンダリング (A型カード/B,C,分岐ティント) | SP-016 | コードレビュー済 | Unity再生 |
| 19 | スマホレスポンシブ基盤 (バブル幅/フォント/サイドバー) | SP-008 | コードレビュー済 | Unity再生 (複数解像度) |
| 20 | Save/Load (3スロット+AutoSave+スレッド+変数) | EN-003/EN-005 | コードレビュー済 | Unity再生 |
| 21 | 早送り (F11) | EN-001 | コードレビュー済 | Unity再生 |
| 22 | Debug Hub (F12) | EN-001 | 動作確認済 | Unity再生 |
| 23 | 選択肢表示 (自動プレイヤーメッセージ化) | EN-001 | コードレビュー済 | Unity再生 |
| 24 | チャプター完了サマリー (SP-019 Phase 1) | SP-019 | コードレビュー済 | Unity再生 |
| 25 | オンボーディングヒント (SP-020 Phase 1) | SP-020 | コードレビュー済 | Unity再生 |
| 26 | 進捗可視化 (ProgressTracker/NudgeSystem/ProgressSummaryUI) | SP-018 | コードレビュー済 | Unity再生 |
| 27 | 制作パイプライン統合ツール (Content Pipeline + ChannelData同期) | EN-004 | コードレビュー済 | Unity Editor |

---

## 3. 未確認機能 (検証手段別)

### Unity Editor再生が必要 (Phase A手動検証: 65項目+SP-019/020)

| # | 確認対象 | 確認内容 | リスク |
|---|---------|---------|-------|
| U-1 | 矛盾Phase 2 | 長押しハイライト+接続線+通知パネル | タグ突合済だが手動操作が必要 |
| U-2 | Ch1後半 | プレイヤーセリフ一部欠落の可能性 | 未確認 |
| U-3 | Ch2 | タイピングインジケーター位置修正の確認 | 修正済だが目視未確認 |
| U-4 | Ch2 | 選択肢タイミング修正の確認 | 修正済だが目視未確認 |
| U-5 | ContentAuthoringシーン | 最終再生確認 | ダッシュボード→Ch1→Ch2フルフロー |
| U-6 | SP-019 | チャプター完了サマリー表示 | 最終Dayまで到達が必要 |
| U-7 | SP-020 | オンボーディングヒント表示 | 初回断片/矛盾発見が必要 |
| U-8 | C型偵察スレッド | CompleteThread/成果物カード | ETK_ThreadParallelでテスト可能 |
| U-9 | UnreadCount復元 | Save/Load後のバッジ表示 | コードレビュー正常、Unity検証待ち |
| U-10 | 知識転送選択UI | EndBranch "select"の操作確認 | ETK_BranchTransferSelectでテスト可能 |

### コードレビューのみ (自動テストなし)

| # | 確認対象 | 状態 | 備考 |
|---|---------|------|------|
| C-1 | ContradictionFeedbackController | セットアップ未完了 | シーンへのアサインが必要 |
| C-2 | TransferSelectionUI | コード実装済 | Phase Aで操作確認必要 |
| C-3 | ProgressSummaryUI | コード実装済 | ダッシュボード内バー表示確認必要 |

---

## 4. 未実装機能

| # | 機能 | 仕様ID | 優先度 | 前提条件 |
|---|------|--------|--------|----------|
| N-1 | BGM/SE統合 | SP-009 | 低 | サウンド方針未決定 |
| N-2 | マネタイズ | SP-010 | 低 | ビジネス判断 (HUMAN_AUTHORITY) |
| N-3 | SP-019 Phase 2: 次Ch解放通知+ダッシュボード自動表示 | SP-019 | 中 | Phase 1検証後 |
| N-4 | SP-019 Phase 3: チャプター間接続演出 | SP-019 | 低 | SP-018 Phase 2 |
| N-5 | SP-020 Phase 2: 矛盾操作チュートリアル | SP-020 | 中 | Phase 1検証後 |
| N-6 | SP-020 Phase 3: インタラクティブガイド | SP-020 | 低 | Phase 2後 |
| N-7 | SP-018 Phase 2: チャプター間接続可視化 | SP-018 | 低 | Ch3設計後 |
| N-8 | ブランチ間クロスリファレンスUI | SP-014 | 低 | Ch3以降で必要性評価 |
| N-9 | B型Wikiリンク遷移 | SP-016/IP-002 | hold | B型コンテンツ3章分 |
| N-10 | C型成果物カードリッチ表示 | SP-016/IP-003 | hold | C型Unity検証後 |
| N-11 | アルケミーボード | SP-014/IP-001 | hold | 矛盾3章安定運用後 |
| N-12 | Ch3シナリオ | SP-003 | 次 | Phase A + 実機検証後 |
| N-13 | E2E自動検証 (PlayMode) | EN-004/project-context | 高 | ETK 拡張方針の確定 |
| N-14 | SP-017 解放通知演出 | SP-017 | 低 | Ch3設計後 |
| N-15 | 複合条件記法集 | SP-017 | 低 | Ch3設計後 |

---

## 5. 現存する懸念点

| # | 懸念 | 重大度 | 対処方針 |
|---|------|--------|----------|
| W-1 | Phase A手動検証65項目が未実施 | HIGH | ユーザーのUnity Editor操作が必須 |
| W-2 | Content Pipeline は実装済みだが Unity 実機検証が未完 | HIGH | ContentAuthoring / DebugQuickTest / Ch1-Ch3 で再生確認 |
| W-3 | ContradictionFeedbackControllerのシーンセットアップ未完了 | MEDIUM | Phase A実施時にセットアップ |
| W-4 | MessageTaggedコマンド | — | 削除済み (session 11) |
| W-5 | ContradictionPairの[Obsolete]フィールド (UnlockTopic) | LOW | SerializeFieldのため削除するとSO壊れる。放置 |
| W-6 | ChatController.cs:1296 TODO (ステータスバールーティング) | LOW | 将来のUI拡張時に対応 |

---

## 6. 本セッションで削除したレガシーコード

| # | 対象 | 理由 | 行数 |
|---|------|------|------|
| D-1 | FragmentListUI.cs | [Obsolete] InventoryTabControllerで置換済 | ~140 |
| D-2 | FragmentListUISetup.cs (Editor) | D-1のセットアップ | ~50 |
| D-3 | FragmentListUITests.cs | D-1のテスト | ~140 |
| D-4 | DeductionBoardSynthesisTest.cs | MonoBehaviour手動テスト。DeductionBoard凍結 | ~98 |
| D-5 | DeductionBoardVerification.cs | TASK検証スクリプト。DeductionBoard凍結 | ~80 |
| D-6 | DeductionBoardTestSetup.cs (Editor) | D-4/D-5のセットアップ | ~90 |
| D-7 | VerificationCapture.cs (Utils) | D-5のみが使用。他に参照なし | ~80 |
| D-8 | ChatScenarioTester.cs (Dev) | TASK_013時代の検証スクリプト | 32 |
| D-9 | TopicUnlockVerifier.cs (Dev) | TASK_013時代の検証スクリプト | 33 |
| D-10 | MVPGameController.cs | MVPフェーズ残骸。本番シーンで未使用 | ~30 |
| D-11 | MVPSceneSetup.cs (Editor) | D-10のセットアップ | ~30 |
| D-12 | MVPTestHelper.cs (Editor) | DeductionBoardリフレクション。凍結機能 | ~120 |
| D-13 | MVPScene.unity | MVP検証シーン。ビルドから除外 | - |
| D-14 | VerificationScene.unity | DeductionBoard検証シーン。凍結機能 | - |
| D-15 | docs/evidence/TASK_047/ | 旧ビルドログ (5ファイル) | - |
| D-16 | docs/evidence/TASK_049/ | 旧エラー証跡 (3ファイル) | - |
| D-17 | docs/archive/inbox/ | 旧セッション管理 (3ファイル) | - |
| D-18 | docs/archive/reports/ | 旧レポート (9ファイル) | - |
| D-19 | docs/archive/tasks/ | 旧タスク管理 (5ファイル) | - |
| **合計** | | | **~920行 + シーン2つ + ドキュメント25ファイル** |

---

## 7. 保持判断 (削除しない理由)

| 対象 | 理由 |
|------|------|
| VerificationAutomator.cs (887行) | Phase A手動検証の自動化基盤として活用可能 |
| VerificationMenu.cs | Editor検証メニュー。MVPScene/VerificationScene参照は除去 |
| ~~MessageTagged (ScenarioManager内)~~ | 削除済み (session 11) |
| ContradictionPair.UnlockTopic | [Obsolete]だがSerializeFieldで既存SOに影響。放置が安全 |
| ChatScenarioData.cs (Data/) | ScenarioManager.PlayScenarioが使用。デバッグ機能として有効 |
| MissingScriptScanner.cs | Editor検証ユーティリティ。VerificationMenuから参照 |
| ContentAuthoringBatchValidator.cs | Editor検証ツール。ScenarioManagerEditorから参照 |
| docs/archive/ROADMAP_TO_PRODUCTION.md | 長期計画の参照資料として残す |
| docs/archive/EVIDENCE_REUSE_*.md | 検証方法論の参照資料 |
| docs/archive/BUBBLE_REFACTOR_TEST_PLAN.md | 完了済みだが設計判断の記録として有用 |
