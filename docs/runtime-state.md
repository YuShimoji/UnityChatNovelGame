# Runtime State

**Updated**: 2026-04-02 session 21

## Current Position

- project: FoundPhone (UnityChatNovelGame)
- branch: main
- lane: Audit + Fix (PlayMode テスト失敗原因特定 + 修正)
- slice: auto-start 安全化 + archive 除外 + TearDown StopScenario
- active_artifact: FoundPhone エンジン基盤 (Unity 6.3 + Yarn Spinner 3.1.3)
- artifact_surface: Unity Editor > DebugChatScene
- last_change_relation: direct (PlayMode テスト失敗の根本原因修正)

## Counters

- blocks_since_user_visible_change: 2 (session 18 Content Pipeline)
- blocks_since_visual_audit: 8 (session 13 Audit → 14 x2 → 15 → 16 → 17 → 18 → 19 → 20 → 21)
- blocks_since_unlock: 2
- consecutive_excise_blocks: 0

## Quantitative Metrics

- impl_files: 66 (テスト除く .cs、実測)
- test_files: 5 (EditMode 4 + PlayMode 1)
- playmode_test_files: 1
- playmode_test_cases: 4 (SmokeGate + Ch1SaveLoad + DQTStart + ChoiceImageFallback)
- mock_files: 0
- yarn_active: 6 (Ch1_Day1, Ch2_LocationConfusion, Ch3_InstitutionalFragments, DebugQuickTest, EngineTestKit, VerticalSlice)
- yarn_archive: 5 (Ch1_Terminal, FirstSlice, MVPTest, DebugScript, SubthreadTest)
- spec_entries: 40 (done 23 / partial 10 / draft 1 / todo 5 + EN-012 + BL-001/002/003)
- todo_fixme_hack: 1 (ChatController.cs — ステータスバールーティング)
- obsolete_marks: 2 (ContradictionPair.UnlockTopic x2)
- chatcontroller_lines: 2325
- wiki_pages: 12 (docs/wiki/ 内)

## Visual Evidence

- visual_evidence_status: stale (Unity PlayMode 未実施。blocks_since_visual_audit: 8)
- last_visual_audit_path: (なし)
- blocks_since_visual_audit: 8

## Session Log

### 2026-04-02 session 21
- Block 1 (Audit + Fix): PlayMode テスト失敗の根本原因特定 + 修正
  - **根本原因**: テスト失敗は teardown DialogueException ではなく、
    DebugChatScene の auto-start が `m_StartNode="Start"` (archive 移動済み DebugScript.yarn のノード) を
    参照し、`Debug.LogError` が NUnit に拾われていたのが原因
  - **修正 1**: `ScenarioManager.Start()` で `HasNode()` による事前チェック追加。
    ノード不在時は LogWarning でスキップ (テスト失敗にならない)
  - **修正 2**: `ResolveLikelyBrokenYarnFile()` が archive/ ディレクトリを検索対象から除外
  - **修正 3**: PlayMode TearDown で `StopScenario()` を明示呼出し、
    OnDestroy 時の async 継続 → DialogueException を防止
  - **残件**: DebugChatScene の Inspector で `m_StartNode` を `Start` → `DQT_Start` に手動変更が必要
  - コミット: `a54752b`

### 2026-03-31 session 20
- Block 1 (Advance): PlayMode batch 実行経路の確立 + Save/Load 復帰バグの切り分け
  - **優先度整理**:
    1. PlayMode 自動検証の起動経路を持つ
    2. `CurrentChannel` Save/Load テストを実ランで確認
    3. 残る teardown 例外を解消
  - **batch entry 追加**: `TestRunnerHelper.RunPlayModeTestsBatch()` を追加し、
    `-executeMethod` + custom args で PlayMode テストを起動可能にした。
  - **`Start` 復帰バグ修正**: `SaveManager.GetCurrentNodeName()` の `"Start"` 固定フォールバックを廃止。
    `$current_node` 不在時は `CurrentChannel -> ChannelData.StartNodeName -> ScenarioManager.DefaultStartNode`
    へフォールバックするよう修正。
  - **実ラン結果**: `DebugChatScene_Ch1Start_PreservesCurrentChannelAcrossSaveLoad` は batch 実行で起動。
    `missing_node:Start` は解消し、`SaveManager: Game loaded from slot 0` まで到達。
    残失敗は teardown 周辺の
    `DialogueException: Cannot continue running dialogue. No node has been selected.`
  - **証跡**:
    - `docs/verification/2026-03-31-playmode-batch-execute.md`
    - `docs/verification/VerticalSliceSmokeGate_20260331_044945_DebugChatScene.txt`
    - `Temp/playmode-batch-execute.log`

### 2026-03-30 session 19
- Block 1 (Excise + Audit): 堆積物整理 + CanvasScaler統一
  - **Yarn active/ クリーンアップ**: 参照なし4件を archive/ へ移動 (FirstSlice, MVPTest, DebugScript, SubthreadTest)
  - **CanvasScaler 9:16統一**: MetaEffectController + DebugSceneBuilder を 1920x1080 → 1080x1920, matchHeight=1.0 に修正
  - **未コミット Topic .asset 6件**: YarnSOGenerator 自動生成分をコミット
  - **PlayMode テスト改善**: WaitForChatMessages/WaitForCondition ヘルパー + Ch1 Save/Load テスト (session 18 未コミット分)
  - **runtime-state.md メトリクス修正**: impl 65→66, yarn_active 11→6, yarn_archive 1→5 (実測値に修正)
- Block 2 (Advance + docs): E2E スコープ + docs 同期 + UI_ISSUES 初記録
  - **DQT_Start PlayMode テスト追加**: DebugQuickTest の起動→メッセージ表示を自動検証
  - **EN-012 登録**: spec-index に E2E PlayMode 自動検証を partial 40% で追加
  - **FEATURE_STATUS_AUDIT 定量サマリー更新**: 実測値に修正
  - **CLAUDE.md 状態更新**: session 19 に同期
  - **HANDOFF.md session 19 差分追記**
  - **UI_ISSUES.md**: DebugChatScene.unity CanvasScaler 不整合を初記録
- Block 2-prior (session 18 末尾): task-scout + `CurrentChannel` Save/Load 検証の固定
  - **task-scout 再確認**: 主ボトルネックは `Content Pipeline -> ContentAuthoring/DebugQuickTest/Ch1-Ch3 -> E2E PlayMode`
    の証跡不足。`visual_evidence_status: stale` と `docs/verification/` 空を確認。
  - **PlayMode テスト追加**: `VerticalSliceSmokeGatePlayModeTests` に
    `DebugChatScene_Ch1Start_PreservesCurrentChannelAcrossSaveLoad` を追加。
    `Ch1_Day1_Opening` で `CurrentChannelID == "ch1"` 自動割り当てと、
    Save/Load 後の `CurrentChannel` 復元を確認する狙い。
  - **batchmode 実行結果**: Unity 6000.3.6f1 で `-runTests -testPlatform PlayMode` を 2 回試行したが、
    いずれも asset import / script compile 後に終了し、`Temp/playmode-test-results.xml` は未生成。
    詳細は `docs/verification/2026-03-30-playmode-batchmode-attempt.md` に記録。
  - 次タスク:
    1. Unity Test Runner batchmode が XML 未生成で終了する原因切り分け
    2. 追加した `CurrentChannel` PlayMode テストの実ラン確認
    3. day progress / thread state まで PlayMode 検証を拡張

### 2026-03-30 session 18
- Block 1 (Unlock): 制作パイプラインの欠落補完 + docs handoff 強化
  - **YarnSOGenerator 拡張**: TopicData / CharacterProfile に加えて ChannelData を自動同期。
    `Ch{N}` / `Ch{N}_Day{M}` ノード規約から StartNodeName / TotalDays / DayStartNodeNames を推定。
  - **ContentPipelineWindow 新規**: Validator 起動 / SO同期 / 推奨 StartNode 選択 /
    ContentAuthoring シーンへの適用 / 即再生を 1 ウィンドウに集約。
  - **StartNode 導線修正**: ScenarioManagerEditor と VerticalSliceSceneSetup の
    `VerticalSlice_Start` 固定を廃止し、`DQT_Start` 優先の推奨ノード方式に統一。
  - **CurrentChannel 自動解決**: ScenarioManager.StartScenario がノード名から ChannelData を解決し、
    EndDay / ヒントポリシー / ContentAuthoring 再生時の文脈ずれを防ぐ。
  - **handoff docs 整備**: HANDOFF.md 新規。project-context / runtime-state /
    OPERATOR_WORKFLOW / INVARIANTS を会話非依存で引ける状態へ更新。
  - 残タスク:
    1. Unity で Content Pipeline 実運用確認
    2. DQT_Start / Ch1 / Ch2 / Ch3 の再生確認
    3. E2E PlayMode 検証導線の追加

### 2026-03-30 session 17
- Block 1 (Advance): 自動スキップバグ修正 + DialogueException修正 + DebugQuickTest
  - **自動スキップ根本修正**: DelayWithSkip/DelayWithPostSkip から token.NextContentToken を除去。
    Yarn の NextContentToken がリークして後続行の全遅延が 0ms になっていた。
    修正後は m_LineSkipCts/m_PostSkipCts のみで遅延制御。NextContentToken はポーリングチェックに変更。
  - **DialogueException 修正**: m_IsDestroying フラグ追加。OnDestroy 中の CancelActiveWait →
    StartWaitCommand 継続 → DialogueRunner.Continue() で node 未選択エラーを防止。
  - **DebugQuickTest.yarn 新規**: DQT_Start ノード。インジケーター/タイプライター/スキップ/選択肢を
    7テストで網羅。Inspector で StartNode を DQT_Start に設定して即実行。
  - **DISPLAY_ALGORITHMS.md 更新**: スキップ制御フロー図 + フォント階層表を実コードに合わせて修正。
  - ユーザーフィードバック吸収:
    1. 自動スキップ (途中から何も押さずにスキップ) → NextContentToken リーク修正
    2. インジケーターのスキップ一貫性 → 自前CTS制御で統一
    3. フォントサイズ (メッセージ本文のみ浮く) → HUMAN_AUTHORITY: messageFontSize=28 vs UIFontConfig.body=18
    4. デバッグ環境整備 → DebugQuickTest.yarn
    5. DialogueException (StartWait + OnDestroy) → m_IsDestroying ガード
  - 未コミット: 全4ファイル
  - 未解決: フォントサイズバランス調整 (HUMAN_AUTHORITY)

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
