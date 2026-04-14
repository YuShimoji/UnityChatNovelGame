# Feature Status Audit

**監査日**: 2026-03-30
**監査対象**: FoundPhone (UnityChatNovelGame) 全コードベース
**ブランチ**: main

---

## 1. 定量サマリー

| 項目 | 数値 | 最終更新 |
|------|------|----------|
| 実装ファイル (.cs) | 66 (テスト除く) / 71 (テスト含む) | session 19 実測（要再スキャン時は `runtime-state` 参照） |
| テストファイル | **7**（EditMode **4** + PlayMode テスト **2** + `PlayModeTestHelpers` **1**） | **2026-04-09 Dレーン再突合** |
| PlayMode テストケース | **8**（`VerticalSliceSmokeGatePlayModeTests` 4 + `ScenarioFlowPlayModeTests` 4） | **2026-04-09 Dレーン再突合** |
| EditMode テストケース | **75**（`CoreLogic` 33 + `Contradiction` 30 + `SaveSystem` 8 + `InventoryTab` 4） | **2026-04-09 Dレーン再突合** |
| Yarnファイル (active) | 6 | session 19 (4件 archive 移動後) |
| Yarnファイル (archive) | 5 | session 19 |
| Yarnコマンド | 24 | |
| 仕様エントリ (spec-index) | **41**（done 24 / partial 10 / draft 2 / todo 5、`docs/spec-index.json` 配列長） | **2026-04-09 F レーン実測** |
| TODO/FIXME/HACK | 1件 (ChatController.cs:1241) | session 19 |
| [Obsolete] マーク | 2件 (ContradictionPair.UnlockTopic x2) | |
| docs ファイル数 | ~30 (archive除く) | |

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

## 3. 未確認機能

Unity Editor 再生を要する個別確認項目 (旧 U-1〜U-10 / C-1〜C-3 計13件) は、**SUBSEQUENT 発動時にまとめて確認**する。個別 U-/C- 表は「永続未実施リスト」となり再実行圧力源になるため廃止 (2026-04-15)。

主要な未確認領域:
- Ch1 後半〜Ch2 の視覚系 (タイピングインジケーター / 選択肢タイミング / プレイヤーセリフ)
- SP-019/SP-020 Phase 1 のオンボーディング/チャプター完了演出
- C型偵察スレッド / Save/Load 後の UnreadCount / EndBranch 知識転送選択
- ContradictionFeedbackController のシーンセットアップ

これらは Ch1 主要変更時 or Ch2 着手前の検収 (SUBSEQUENT 発動条件) でまとめて確認する。

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
| N-12 | Ch3シナリオ | SP-003 | 次 | Ch2 前進後 (SUBSEQUENT 発動判定を経て) |
| N-13 | E2E自動検証 (PlayMode) | EN-004/project-context | 高 | ETK 拡張方針の確定 |
| N-14 | SP-017 解放通知演出 | SP-017 | 低 | Ch3設計後 |
| N-15 | 複合条件記法集 | SP-017 | 低 | Ch3設計後 |

---

## 5. 現存する懸念点

| # | 懸念 | 重大度 | 対処方針 |
|---|------|--------|----------|
| W-1 | ~~Phase A手動検証65項目が未実施~~ | — | **廃止** (2026-04-15): チェックリストを `docs/archive/acceptance/` へ archive 済み。SUBSEQUENT 発動時に必要な範囲を都度選定 |
| W-2 | Content Pipeline は実装済みだが実機検証は SUBSEQUENT 待ち | INFO | HIGH から INFO に緩和。SUBSEQUENT 発動時に確認 |
| W-3 | ContradictionFeedbackController のシーンセットアップ | INFO | SUBSEQUENT 発動時にセットアップ |
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
| D-17 | （旧）docs/archive/inbox/ | 整理済み（履歴参照） | - |
| D-18 | （旧）docs/archive/reports/ | 整理済み（履歴参照） | - |
| D-19 | （旧）docs/archive/tasks/ | 整理済み（履歴参照） | - |
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
| docs/project-context.md | 長期計画の参照資料（archive から移設） |
| docs/EVIDENCE_REUSE.md | 検証方法論の現行要約 |
| （旧 BUBBLE_REFACTOR_TEST_PLAN） | 整理済み。必要時は git 履歴参照 |
