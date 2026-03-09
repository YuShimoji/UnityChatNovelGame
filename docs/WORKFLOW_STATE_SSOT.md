# WORKFLOW STATE SSOT

**Updated**: 2026-03-09
**Phase**: ダッシュボード MVP 実装済み — セットアップ反映中 + 動作確認待ち
**Branch**: main

## Mission

ダッシュボード MVP の手動セットアップ・動作確認を行い、プレイヤーリソース表示域（インベントリ）へ進む。
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
- [ ] ScenarioManager の `m_AutoStartYarn=false` 反映（現状 `true`）
- [ ] ダッシュボード手動セットアップ + 動作確認（Unity手動）
- [ ] 矛盾 Phase 2 の動作確認（手動: セットアップ + 再生テスト）
- [ ] ContentAuthoring シーンでの最終再生確認（手動）
  - Ch1 後半: プレイヤーセリフ一部欠落の可能性（要再確認）
  - Ch2: タイピングインジケーター位置修正 → 要確認
  - Ch2: 選択肢タイミング修正 → 要確認

## 開発継続プラン

### ダッシュボード MVP セットアップ（反映状況）

ContentAuthoring シーンの現状:

1. [x] `Tools > FoundPhone > Create Default Channel Data` 相当: `Assets/Resources/Channels/ch1.asset`, `ch2.asset` が存在
2. [x] `Tools > FoundPhone > Add Dashboard to Scene` 相当: `DashboardManager` + `DashboardController` が存在
3. [x] DebugHubController の `m_ShowOnStart=false`
4. [ ] ScenarioManager の `m_AutoStartYarn=false`（現状: `true`）
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

| # | 作業 | 分類 | 前提 |
|---|------|------|------|
| 1 | ダッシュボード + 矛盾 Phase 2 セットアップ + 動作確認 | A | Unity 手動 |
| 2 | 発見バグの修正（あれば） | A | #1 結果 |
| 3 | プレイヤーリソース表示域（インベントリ UI） | A | 仕様策定済み (`08_ui_ux.md`) |
| 4 | スマホサイズ基準レイアウト調整 | B | #3 の UI 確定後 |
| 5 | Ch3 シナリオ設計 | A | #1-2 完了後 |

### 技術的知見

- **選択肢のプレイヤーメッセージ**: `RunOptionsAsync` でコード側が自動追加。Yarn スクリプトでのエコー行は不要
- **タイピングインジケーター**: `ConfigureBubble` が生成するラッパー(NpcRow)ごと操作する必要がある
- **DebugHub**: 前ダイアログの `Stop()` が必須（トークン汚染による早送り状態を防止）
- **共通処理**: Ch1/Ch2 のコアループは完全に共通。チャプター固有処理は矛盾システムの難易度制御のみ
- **ダッシュボード**: DebugHubController と同じプログラマティック UI パターン。ChannelData SO でチャプター管理
- **コード品質**: ScenarioManager.cs / DeductionBoard.cs に文字化けコメント約70行（D分類で凍結中）

### Claude への依頼パターン

各セッションの冒頭で以下のいずれかを伝えれば即座に作業開始可能:

1. **「セットアップ完了、動作確認した、問題なし → 次へ」** → インベントリ UI に着手
2. **「動作確認した、バグあり」+ スクショ** → バグ修正
3. **「インベントリ UI を作って」** → 設計・実装（仕様は `08_ui_ux.md` Section 2 参照）
4. **「Ch3 シナリオ設計を始めたい」** → StorySpec ベースで設計

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
