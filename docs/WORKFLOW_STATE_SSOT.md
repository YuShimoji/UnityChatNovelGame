# WORKFLOW STATE SSOT

**Updated**: 2026-03-06
**Phase**: Ch1 再生バグ修正 + 動作確認待ち
**Branch**: main

## Mission

Ch1 ダイアログ再生を正常化し、矛盾 Phase 2 の動作確認を経て、フラグメント一覧 UI へ進む。

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
- [x] 技術的負債解消: namespace修正、GUID修正、deprecated API更新
- [x] 矛盾システム自動テスト（ContradictionTests.cs: 24テスト）
- [x] FragmentListUI 実装（コード生成ScrollView + DebugHub連携）
- [x] Editor セットアップスクリプト: ContradictionSystemSetup.cs, FragmentListUISetup.cs
- [x] 矛盾 Phase 2 セットアップ自動化 → Editorメニューから1クリック
- [x] Typing コマンド登録 (`AddCommandHandler<string>` + 手動パース)
- [x] DialoguePresenter 未登録バグ修正（`ScenarioManager.InitializeComponents` でランタイム登録）
- [x] レガシードキュメント整理（docs/archive/ に247件移動）
- [ ] Ch1 ダイアログ再生の確認（DialoguePresenter修正後）← **手動確認待ち**
- [ ] 矛盾 Phase 2 の動作確認（手動: 長押し → 矛盾指摘フロー）
- [ ] ContentAuthoring シーンでの最終再生確認

## 既知のバグ・修正済み

### DialoguePresenter 未登録（修正済み・未確認）

- **症状**: Ch1 再生時にコマンド（SystemMessage等）は動作するがダイアログ行が表示されない。3つ目のSystemMessage後に停止
- **原因**: `ChatDialogueView` が `DialogueRunner.dialoguePresenters` に登録されていなかった。コマンドは `AddCommandHandler` 経由で別系統のため動作していた
- **修正**: `ScenarioManager.InitializeComponents()` で `ChatDialogueView` を `DialogueRunner.DialoguePresenters` にランタイム追加
- **場所**: [ScenarioManager.cs:92-111](Assets/Scripts/Core/ScenarioManager.cs#L92-L111)
- **確認方法**: Play Mode → コンソールに `ScenarioManager: ChatDialogueView をランタイムで DialogueRunner に登録しました` が出力され、Ch1 のダイアログ行（「おはようございます。Pyramid アシスタントです。」等）が表示されること

### Typing コマンド（修正済み）

- **症状**: `<<Typing true>>` で `No Command "Typing" was found` エラー
- **修正**: `AddCommandHandler<string>("Typing", TypingCommand)` + `string.Equals("true", OrdinalIgnoreCase)` で手動パース
- **場所**: [ScenarioManager.cs:126](Assets/Scripts/Core/ScenarioManager.cs#L126), [ScenarioManager.cs:345-352](Assets/Scripts/Core/ScenarioManager.cs#L345-L352)

## 開発継続プラン

### 手動確認待ち

1. Play Mode で Ch1 再生 → ダイアログ行が正常表示されるか確認
2. 矛盾 Phase 2: Validate → 長押し → 矛盾指摘フロー確認

### 次ステップ（優先順）

| # | 作業 | 分類 | 前提 |
|---|------|------|------|
| 1 | Ch1 再生確認 + 矛盾 Phase 2 動作確認 | A | Unity 手動 |
| 2 | 発見バグの修正（あれば） | A | #1 結果 |
| 3 | フラグメント一覧 UI（収集済みフラグメントの閲覧画面） | A | — |
| 4 | ダッシュボード型メイン画面（チャンネル選択 → チャット遷移） | A | #3 並行可 |
| 5 | スマホサイズ基準レイアウト調整 | B | #3-4 の UI 確定後 |

### 技術的知見

- **DialoguePresenter登録**: Yarn Spinner 3.x では `DialogueRunner.dialoguePresenters`（シリアライズリスト）に Presenter が含まれていないと `RunLineAsync` が呼ばれない。コマンドは `AddCommandHandler` で別系統なので影響しない
- **Typing コマンドの型**: `AddCommandHandler<bool>` は Yarn の `true`/`false` 文字列を C# bool に変換できない場合がある。`AddCommandHandler<string>` + 手動パースが安全
- **選択肢のプレイヤーメッセージ**: `RunOptionsAsync` でコード側が自動追加。Yarn スクリプトでのエコー行は不要
- **タイピングインジケーター**: `ConfigureBubble` が生成するラッパー(NpcRow)ごと操作する必要がある
- **DebugHub**: 前ダイアログの `Stop()` が必須（トークン汚染による早送り状態を防止）
- **コード品質**: ScenarioManager.cs / DeductionBoard.cs に文字化けコメント約70行（D分類で凍結中）

### Claude への依頼パターン

各セッションの冒頭で以下のいずれかを伝えれば即座に作業開始可能:

1. **「Ch1 再生確認した、問題なし → 次へ」** → 矛盾確認 or フラグメント UI に着手
2. **「再生確認した、バグあり」+ ログ/スクショ** → バグ修正
3. **「フラグメント一覧 UI を作って」** → 設計・実装
4. **「ダッシュボード UI を作って」** → UI 設計・実装

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
