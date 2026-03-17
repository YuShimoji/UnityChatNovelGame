# WORKFLOW STATE SSOT

**Updated**: 2026-03-17
**Phase**: エンジン検証 Phase A — 全主要機能実装完了、手動検証待ち
**Branch**: main

## Mission

ダッシュボード MVP + インベントリ UI + チャットバブル修正 + サブスレッドUI (5a-5e+5d) の動作確認を行い、安定ベースラインを確立する。
残: Phase Aクロージング手動検証(56項目) → Ch3シナリオ設計。
Ch1/Ch2 Yarn スクリプトはエンジン検証用モックであり、本番コンテンツではない。

## Done 条件（現フェーズ）

- [x] Ch1/Ch2 コンテンツ統合
- [x] UI バグ修正（縦書き、色薄、選択肢重複、フォントサイズ、テキスト色上書き）
- [x] ChatUIConfig SO + null ガード除去リファクタリング
- [x] レイアウト崩れ修正（マージンキャップ追加）
- [x] 選択肢メッセージ埋没化（色・サイズ・配置調整）
- [x] 早送り機能（F11トグル、デバッグオーバーレイ表示）
- [x] Debug Hub ストーリー順ソート（Yarn title: 行出現順）
- [x] 矛盾 Phase 2: UI フィードバック（成功/失敗演出、接続線、通知、トピック解放連携）
- [x] Ch2 バグ修正: 早送り状態 / アニメーション重複 / プレイヤー選択肢非表示
  - RunOptionsAsync: 選択肢テキストをプレイヤーメッセージとして自動表示（Yarn側エコー不要に）
  - DebugHub: 前ダイアログ停止 + IsDialogueRunning ガード
  - StartWait: FastForwardEnabled 対応
  - ShowTypingIndicator: ラッパーごと移動（上部固定バグ修正）
  - RunOptionsAsync: 選択肢表示前 400ms 遅延
- [x] レガシードキュメント整理（docs/reports, docs/tasks, docs/evidence, docs/inbox, docs/logs 等 318ファイル削除）
- [x] ダッシュボード MVP 実装（DashboardController + ChannelData + SaveData拡張 + Editorツール）
- [x] ContentAuthoring シーンに `DashboardManager` 追加（DashboardController / `m_ShowOnStart=true`）
- [x] `Assets/Resources/Channels/` に `ch1.asset`, `ch2.asset` を配置
- [x] DebugHubController の `m_ShowOnStart=false` 反映
- [x] ScenarioManager の `m_AutoStartYarn=false` 反映（2026-03-11 シーンファイル直接編集）
- [x] インベントリ UI（3サブタブ + オーバーレイ導線 + DebugHub統合）
- [x] チャットバブル表示修正（二重ラッパー・選択肢色・下詰め・スクロール吸着）
- [x] レガシー docs 整理（_ARCHIVED_ specs 10件 + retros + tasks 削除）
- [x] ダッシュボード動作確認（2026-03-11）— タブ切替・Ch1開始・ESC復帰・インベントリ表示 PASS
- [ ] 矛盾 Phase 2 の動作確認（手動: セットアップ + 再生テスト）— タグ検証済み、手動テスト待ち
- [ ] ContentAuthoring シーンでの最終再生確認（手動）
  - Ch1 後半: プレイヤーセリフ一部欠落の可能性（要再確認）
  - Ch2: タイピングインジケーター位置修正 → 要確認
  - Ch2: 選択肢タイミング修正 → 要確認
- [x] サブスレッドUI最小スライス実装（2026-03-15）— DeclareThread/AddThreadMessage + トグルボタン切替 + Save/Load対応
- [x] サブスレッドUI ThreadType 導入（2026-03-16）— ThreadType enum (Annotation/Tracking/Scout/Branch) + DeclareThreadTyped コマンド + 型別アイコン/色 + スレッド切替時ヘッダーバー
- [x] 発見バグ修正（2026-03-11 テスト検出 → d2d2d78 で修正済み）
  - Bug-1: ESC 停止時 `DialogueException` → StopScenario deferred Stop() via coroutine
  - Bug-2: 選択肢 `DialogueException` → CancelActiveWait first, Stop next frame
  - Bug-3: `DeductionBoard: Instance not found` → s_WarnedNotFound flag で warn-once 化

## 開発継続プラン

### ダッシュボード MVP セットアップ（反映状況）

ContentAuthoring シーンの現状:

1. [x] `Tools > FoundPhone > Create Default Channel Data` 相当: `Assets/Resources/Channels/ch1.asset`, `ch2.asset` が存在
2. [x] `Tools > FoundPhone > Add Dashboard to Scene` 相当: `DashboardManager` + `DashboardController` が存在
3. [x] DebugHubController の `m_ShowOnStart=false`
4. [x] ScenarioManager の `m_AutoStartYarn=false`（2026-03-11 反映済み）
5. [ ] 上記反映後の手動再生確認

### 矛盾 Phase 2 セットアップ（未完了）

ContentAuthoring シーンで以下を行う必要あり:

1. Canvas 直下に空 GameObject「ContradictionFeedback」を作成
2. `ContradictionFeedbackController` コンポーネントを追加
3. Inspector で `Chat Controller` フィールドにシーン内の ChatController をアサイン

### 動作確認手順

**ダッシュボード:**

1. シーン再生 → ダッシュボードがデフォルト表示
2. Ch1 カード [AVAILABLE] → クリック → チャット開始
3. Ch2 カード [LOCKED]（CompletedChannelIDs 空のため）
4. Back ボタン or ESC → シナリオ停止 → ダッシュボード復帰
5. F12 → DebugHub 独立表示（競合なし）
6. HC 表示 = ContradictionManager.HalluciCoin

**矛盾 Phase 2:**

1. Ch1 でプレイ開始
2. `#line:` タグ付きメッセージまで進行（L169, L173 付近の region_identity ペア等）
3. 矛盾タグ付きバブルを 0.5秒長押し → 青ハイライト + ヒントバナー確認
4. 対応するもう一方のバブルをタップ → 緑フラッシュ + 通知パネル + 接続線確認
5. 不一致バブルをタップ → 赤フラッシュ + エラーバナー + クールダウン確認

### 次ステップ（優先順）

| # | 作業 | 分類 | 前提 | 状態 |
| --- | ------ | ---- | ------ | ------ |
| 1 | ~~ダッシュボード動作確認~~ | A | — | **済** (2026-03-11) |
| 2 | ~~発見バグ修正 (Bug-1/2/3)~~ | A | — | **済** (d2d2d78) |
| 3 | ~~インベントリ UI~~ | A | — | **済** (2026-03-11) |
| 4 | ~~チャットバブル表示 + スクロール修正~~ | A | — | **済** (2026-03-11、目視PASS) |
| 4a | ~~Yarnディレクトリ分離 (active/archive)~~ | B | — | **済** (87d9193) |
| 4b | ~~角丸スプライト生成失敗修正~~ | A | — | **済** (d472c6e: radius<=0 時デフォルト16fフォールバック) |
| 4c | ~~名前行リッチテキストスコープ修正~~ | A | — | **済** (d472c6e: CloseUnclosedRichTextTags で未閉タグ補完) |
| 4d | ~~Hub選択肢ループ修正~~ | A | — | **済** (d472c6e: C#側0選択肢セーフティ + Yarnフォールバック選択肢) |
| 5 | 矛盾 Phase 2 動作確認 | A | ContradictionFeedback セットアップ | **タグ検証済** (7ペア全一致、手動テスト待ち) |
| 5a | ~~サブスレッドUI最小スライス~~ | A | — | **済** (2026-03-15: DeclareThread/AddThreadMessage + トグルUI + Save/Load) |
| 5b | ~~サブスレッドUI ThreadType + 型別レンダリング~~ | A | #5a | **済** (2026-03-16: ThreadType enum + DeclareThreadTyped + ヘッダーバー) |
| 5c | ~~通知バナー（新着スレッドメッセージ表示）~~ | A | #5b | **済** (2026-03-16: DOTweenフェードイン/アウト + クリックでスレッド切替 + 型色/アイコン表示) |
| 5d | ~~サイドバー型スレッド一覧UI~~ | B | #5b | **済** (2026-03-16: ドロップダウン→左スライドインサイドバー、ThreadTypeグループ、ハンバーガーボタン+未読合計バッジ、DOTweenアニメ、オーバーレイ) |
| 5e | ~~複数サブスレッド同時並走 / B・C型の実用検証~~ | B | #5b | **済** (2026-03-16: ETK_ThreadParallel追加。ノード遷移維持+交互追加+Save/Load復元確認項目) |
| 5f | ~~種別差異レンダリング (Step 3 Phase 3a)~~ | A | #5b | **済** (2026-03-17: A型注釈カード+B/C/分岐ティント+SystemMessageティント) |
| 5g | ~~出現通知 (Step 3 Phase 3b)~~ | A | #5b | **済** (2026-03-17: 型色リッチテキスト通知+ハンバーガーパルス) |
| 6 | ~~スマホレスポンシブ基盤 (CRITICAL+HIGH)~~ | B | #4b,4c 完了後 | **済** (2026-03-17: バブル幅/フォント/サイドバー幅レスポンシブ) |
| 7 | ~~SP-014 Phase 1: 分岐内トピック自動追跡~~ | A | #5a | **済** (2026-03-17: UnlockTopic→TransferFlags自動登録, ResolveReflectionMessage自動生成) |
| 8 | ~~SP-014 Phase 2: 条件付き分岐トリガー~~ | A | #7 | **済** (2026-03-17: DeclareThreadLatentCond+リアクティブ評価+AutoBeginBranch+安全弁+サイドバーバッジ) |
| 9 | ~~SP-006 ゲートメカニクス~~ | A | — | **済** (2026-03-17: HCゲートUI+ChannelData.RequiredHalluciCoin+ダッシュボードHC N/M表示) |
| 10 | Ch3 シナリオ設計 | A | #2,5 完了後 | 未着手 |

### 技術的知見

- **選択肢のプレイヤーメッセージ**: `RunOptionsAsync` でコード側が自動追加。Yarn スクリプトでのエコー行は不要
- **タイピングインジケーター**: `ConfigureBubble` が生成するラッパー(NpcRow)ごと操作する必要がある
- **DebugHub**: 前ダイアログの `Stop()` が必須（トークン汚染による早送り状態を防止）
- **共通処理**: Ch1/Ch2 のコアループは完全に共通。チャプター固有処理は矛盾システムの難易度制御のみ
- **ダッシュボード**: DebugHubController と同じプログラマティック UI パターン。ChannelData SO でチャプター管理
- **コード品質**: 文字化けコメント修正済み (Task B: 2a34836)。ScenarioManager/CoreLogicTests/SaveLoadUI/SaveSlotUI の全92行を UTF-8 日本語に復元
- **オートセーブ**: EN-005 実装済み (6cc1a63)。slot=99, 30秒CD, ノード遷移/選択肢/EndDay トリガー
- **bubbleSprite fake null**: 修正済み (147b36d)。?? → != null で Unity overloaded == 対応。角丸9-sliceが初めて有効化
- **サブスレッドUI**: データスワップ方式 (ClearMessages + RestoreChatHistory)。スレッド別にスクロール位置保存。DeclareThread/AddThreadMessageでYarnから宣言
- **DeclareThreadTyped**: `DeclareThread` (2引数) と別コマンド名で登録。Yarn Spinner は同名の引数違いオーバーロードを解決できないため分離が必要。既存の `DeclareThread` は Annotation フォールバックとして維持
- **ThreadType 取得方法**: `OnThreadDeclared` イベントシグネチャは変更なし (`Action<string, string>`)。型情報は `ScenarioManager.GetDeclaredThread(threadId).Type` 経由で取得
- **ヘッダーバー表示制御**: スレッド切替時にのみ表示・非表示を切替。Main スレッド選択時は非表示。型別色は 15% alpha の背景帯 + 90% alpha のラベルで構成
- **型別アイコン/色**: Annotation=[A], Tracking=[B], Scout=[C], Branch=[>]。色は ThreadSwitcherController 定数 (TypeColorAnnotation 等) で一元管理
- **Yarn コマンド数**: 全22コマンド (DeclareThreadLatentCond 追加後)
- **SubthreadTest.yarn**: DeclareThreadTyped 対応に更新済み。型指定の検証モックとして機能
- **通知バナー**: OnThreadMessageAdded で非アクティブスレッドへのメッセージ検出 → DOTween Sequence (fadeIn 0.25s → 3.5s表示 → fadeOut 0.4s)。クリックで OnSelectThread 呼出。Reset/OnSelectThread で HideNotificationBanner
- **スレッドのノード遷移維持**: m_DeclaredThreads はノード遷移(jump)で消えない。ClearDeclaredThreads は SaveManager.LoadGame のみで呼ばれる
- **Save/Load修正 (3d0a0f6)**: GetAllThreadHistories()からメイン履歴取得 + SetActiveThreadId削除 + ロード前にThreadSwitcherController.Reset()呼出
- **セーブ復元名前重複**: 修正済み (147b36d)。StripNamePrefix で後方互換
- **バブル幅リサイズ対応**: 修正済み (8b22b71)。Screen.width → RectTransform.rect.width
- **EngineTestKit**: 追加済み (6da0aba)。F12 Debug Hub + ch_test Dashboard テスト用8ノード
- **サイドバーUI (5d)**: ドロップダウン→左スライドインサイドバーに全面置換。ハンバーガーボタン(≡)+未読合計バッジ、半透明オーバーレイ(タップで閉じ)、ThreadTypeグループヘッダー、DOTween スライドアニメ(0.25s OutCubic/InCubic)、ScrollRect内蔵でスレッド多数でもスクロール可。Main エントリは常に先頭
- **種別差異レンダリング (5f)**: ChatController.m_ActiveThreadType で表示中スレッドの型を追跡。A型(注釈): 中央配置カード(キャラアイコン/名前省略、型色ベース暗背景、型色明テキスト)。B/C/分岐: キャラ色に型色を12%混合したティント。SystemMessage: サブスレッド内で型色10%ティント。ThreadSwitcherController.OnSelectThread と SaveManager.ApplySaveData の両方で SetActiveThreadType を呼ぶ
- **出現通知 (5g)**: DeclareThread時に型アイコン+型色リッチテキスト付きSystemMessage。ハンバーガーボタンが型色で2回パルス (DOTween)
- **Branch Step 2+ (SP-014)**: BeginBranch(自動切替) / EndBranch(自動復帰+反映メッセージ) / SetBranchReflection(Yarn指定型)。分岐中はサイドバーで自由切替可
- **Branch TransferFlags自動追跡 (SP-014 Phase 1)**: 分岐内でUnlockTopicを呼ぶとTransferFlagsに自動登録。EndBranch時にSetBranchReflection未設定ならTransferFlagsのトピック名から反映メッセージを自動生成 (ResolveReflectionMessage)。優先順位: Yarn指定 > TransferFlags自動 > なし
- **スマホレスポンシブ (SP-008)**: GetResponsiveBubblePercent (canvas幅<800→0.85) / GetResponsiveFontScale (canvas幅<900→縮小) / GetSidebarWidth (canvas幅40%上限)
- **HalluciCoin通知バッジ (SP-006)**: RefreshCoinDisplay で前回値と比較、増加時にDOTweenパルス (scale 1.3x + 色ハイライト)
- **HCゲート (SP-006)**: ChannelData.RequiredHalluciCoin + ダッシュボードLocked表示 (HC N/M形式)。ch2.asset に RequiredHalluciCoin=2 設定済み
- **DeclareThreadLatentCond (SP-016 Step4)**: 条件付き潜在スレッド。Yarn変数変更時にリアクティブ評価し、trueで自動顕在化。Branch型はAutoBeginBranch。安全弁付き
- **Ch1 BranchPyramid**: BeginBranch/EndBranch/UnlockTopicのエンジン検証モック。Day2にDeclareThreadLatentCondも組込済み

### Claude への依頼パターン

各セッションの冒頭で以下のいずれかを伝えれば即座に作業開始可能:

1. **「動作確認した、問題なし → 次へ」** → Ch3 シナリオ設計 or サブスレッド UI に着手
2. **「動作確認した、バグあり」+ スクショ** → バグ修正
3. **「Ch3 シナリオ設計を始めたい」** → StorySpec ベースで設計
4. **「スマホレイアウト調整」** → 9:16 基準でUI調整

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

---

## 2026-03-09 Clarification

- `ScenarioManager.m_AutoStartYarn` runtime default is `false`.
- Scene/setup tools may set `m_AutoStartYarn=true` only for debug or content-authoring preview workflows.
- For production-like validation, treat `false` as the baseline expectation.

## 2026-03-09 C-Branch Spike Step1

- Implemented: branch bridge model + runtime/save-load wiring.
- Added data model: `BranchThreadState`.
- Added ScenarioManager APIs: begin/add-flag/end + snapshot/apply.
- SaveData/SaveManager now persist branch bridge state (`BranchThread`).
- Remaining for next step: option entry, branch return hook, deterministic reflected message.

## 2026-03-10 Feature Triage (Handover)

- Added spec note: `docs/StorySpec/15_feature_triage_2026-03-10.md`.
- Classified current state for 4 items as existing/partial/unimplemented at feature level.
- Priority order fixed for next phase:
  1) Thread-management spec split (UI subthread vs branch-state bridge)
  2) Designer-facing unified authoring guide
  3) External wiki-ready documentation package
  4) Face-icon behavior matrix and text-animation responsibility split
- C-branch implementation scope remains Step2+ for execution work:
  - option entry
  - return hook
  - deterministic reflected message
