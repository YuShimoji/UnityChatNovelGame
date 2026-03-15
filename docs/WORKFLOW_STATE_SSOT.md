# WORKFLOW STATE SSOT

**Updated**: 2026-03-15
**Phase**: エンジン検証 Phase A — サブスレッドUI最小スライス実装完了、手動テスト確認済み
**Branch**: main

## Mission

ダッシュボード MVP + インベントリ UI + チャットバブル修正の動作確認を行い、安定ベースラインを確立する。
その後 Ch3 シナリオ設計・サブスレッド UI へ進む。
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
| 6 | スマホサイズ基準レイアウト調整 | B | #4b,4c 完了後 | 未着手 |
| 7 | Ch3 シナリオ設計 | A | #2,5 完了後 | 未着手 |

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
- **セーブ復元名前重複**: 修正済み (147b36d)。StripNamePrefix で後方互換
- **バブル幅リサイズ対応**: 修正済み (8b22b71)。Screen.width → RectTransform.rect.width
- **EngineTestKit**: 追加済み (6da0aba)。F12 Debug Hub + ch_test Dashboard テスト用8ノード

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
