# MVPテストガイド

> **Note (2026-03-18)**: Phase A クロージング検証は [phase-a-closing-checklist.md](acceptance/phase-a-closing-checklist.md) が最新版。
> 本ガイドの Step 1-4 (MVPTest.yarn 基本検証) は引き続き有効。Phase A セクション (Test A-F) は checklist に統合済み。

## 概要
MVPTest.yarnシナリオを使用して、UnityChatNovelGameエンジンの基本機能を検証します。

## テスト対象機能
- ✅ Yarn Spinner統合（DialoguePresenterBase）
- ✅ カスタムコマンド（SystemMessage, StartWait, Typing, set）
- ✅ 選択肢表示と分岐
- ✅ キャラクター切り替え（$speaker変数）
- ✅ メッセージバブル表示
- ✅ スクロール吸着
- ✅ タイプライター効果

## 前提条件
1. Unityエディタでプロジェクトを開く
2. Yarn Spinnerパッケージがインポート済み
3. ChatDialogueViewがシーンに配置済み
4. CharacterDatabaseに"pyramid"と"player"が登録済み

## テスト手順

### Step 1: シーンの準備
1. `Assets/Scenes/ContentAuthoring.unity`を開く
2. Hierarchyで`DialogueSystem`を選択
3. Inspectorで`DialogueRunner`の`Start Node`を`MVPTest_Start`に設定

### Step 2: 実行とテスト
1. Play modeに入る
2. 以下の動作を確認：

#### 初期表示
- [ ] システムメッセージ「本日のセッションを開始します...」が中央に表示される
- [ ] 1秒待機後、次のシステムメッセージが表示される
- [ ] TypingIndicator「...」が表示される
- [ ] Pyramidのメッセージが左側（NPC側）に表示される

#### 選択肢の表示
- [ ] 2つの選択肢が画面下部に表示される
  - 「ここはどこだ？」
  - 「誰がいるのか？」
- [ ] 選択肢がタイプライター効果完了後に表示される（食い気味でない）
- [ ] 選択肢ボタンがホバー/クリックで反応する

#### 分岐1: 「ここはどこだ？」
- [ ] プレイヤーの選択が右側（Player側）に表示される
- [ ] 0.5秒待機後、Pyramidの応答が表示される
- [ ] 「アナログチャットルーム」に関する説明が表示される
- [ ] 次の選択肢が表示される

#### 分岐2: 「誰がいるのか？」
- [ ] プレイヤーの選択が右側に表示される
- [ ] Pyramidの自己紹介が表示される
- [ ] 次の選択肢が表示される

#### 継続フロー
- [ ] 「セッションを終了する」選択で終了メッセージ表示
- [ ] 「もう少し話を聞きたい」選択で追加情報表示
- [ ] SkipWaitコマンドが機能する

### Step 3: UI動作確認
- [ ] メッセージバブルが正しく配置される（左/右）
- [ ] キャラクターアイコンが表示される（設定されている場合）
- [ ] スクロールが自動的に最下部に追従する
- [ ] タイプライター効果中もスクロール吸着が機能する
- [ ] 選択肢が二重表示されない
- [ ] TypingIndicatorが正しく表示/非表示される

### Step 4: エラーチェック
1. Consoleウィンドウを確認
2. 以下のエラーがないことを確認：
   - [ ] NullReferenceException
   - [ ] Yarn変数エラー
   - [ ] カスタムコマンドエラー
   - [ ] UI配置エラー

## 期待される結果
- すべてのメッセージが正しく表示される
- 選択肢が機能し、分岐が動作する
- UI要素が適切に配置される
- エラーが発生しない

## 問題が発生した場合

### TypingIndicatorが表示されない
- ChatController.ShowTypingIndicator()の実装を確認
- m_TypingIndicatorWrapperがnullでないか確認

### 選択肢が表示されない
- ChatController.ShowChoices()が呼ばれているか確認
- m_ChoiceContainerが正しく初期化されているか確認

### メッセージが左上に表示される
- ConfigureBubble()でラッパーが正しく作成されているか確認
- HorizontalLayoutGroupの設定を確認

### キャラクターが"unknown"になる
- CharacterDatabaseに"pyramid"と"player"が登録されているか確認
- CharacterProfile.CharacterIDが正しく設定されているか確認

---

## Phase A 検証テスト (2026-03-13)

バグ修正3件 + 矛盾Phase2 + Day Resume の動作確認。
以下を順に実施する。

### 準備

1. `Assets/Scenes/ContentAuthoring.unity` を開く
2. Unity メニュー `Project FoundPhone > Setup > Contradiction System` を実行
3. Unity メニュー `Project FoundPhone > Validate > Contradiction System Wiring` を実行
   - [ ] Console に `Contradiction system wiring OK.` が出る
4. Console をクリアする

### Test A: ダッシュボード + Ch1 再生 (Bug 4b/4d 検証)

1. Play mode に入る
2. **ダッシュボードが表示されることを確認**
   - [ ] チャンネル一覧に `ch1` が表示される
3. `ch1` をタップ → Ch1 Day1 が再生開始
4. **バブル表示の確認 (Bug 4b)**
   - [ ] Pyramid / Player のバブルが正常に角丸で表示される
   - [ ] Console に `bubbleCornerRadius` 警告が出ない（正常値の場合）
   - [ ] NullReferenceException が出ない
5. **Hub ループ確認 (Bug 4d)** — Ch1_Day1_Hub に到達するまで進める
   - 4つのトピック全てを消化する（region / terminal / marco / fragment）
   - [ ] 全トピック消化後も「もう少し考えよう」選択肢が表示される
   - [ ] 「もう少し考えよう」を選ぶとハブに戻る（無限ループしない）
   - [ ] 「先に進む」選択肢が表示される
   - [ ] Console に `RunOptionsAsync received 0 available options` 警告が出ない
6. 「先に進む」→ Ch1 Day1 を最後まで進める → `<<EndDay 1>>` が実行される
   - [ ] システムメッセージが表示される
   - [ ] Play mode を終了しない（Day Resume テストに使用）

### Test B: Day Resume (ch1 TotalDays=2)

1. Test A の続き: EndDay 1 完了後、「Back to Hub」でダッシュボードに戻る
   - [ ] ダッシュボードが再表示される
   - [ ] ch1 のステータスが InProgress になっている
2. ch1 を再度タップ
   - [ ] Console に `Resuming channel 'ch1' from day 2 node 'Ch1_Day2_Opening'` が出る
   - [ ] `--- 2日目 ---` システムメッセージが表示される
   - [ ] Day1 の冒頭ではなく Day2 から再生される
3. Day 2 を最後まで進める → `<<EndDay 2>>` 実行
   - [ ] チャンネル完了処理が走る（最終 Day のため）
4. ダッシュボードに戻る
   - [ ] ch1 のステータスが Completed になっている

### Test C: リッチテキストスコープ (Bug 4c)

1. Ch1 または DebugScript でリッチテキストを含むメッセージを確認
   - [ ] `<b>太字</b>` を含むメッセージ: 太字がそのバブル内で閉じている
   - [ ] 次のバブルに太字が漏れていない
   - [ ] キャラクター名にボディの書式が適用されていない
2. 目視で複数バブルを確認
   - [ ] 各バブルの名前部分とメッセージ部分の書式が独立している

### Test D: 矛盾指摘 Phase 2

**前提**: ContradictionManager + ContradictionFeedbackController がシーンに配置済み

1. Ch1 を再生し、矛盾タグ付きメッセージまで進める
   - 例: `#line:ch1_region_identity_src` と `#line:ch1_region_identity_tgt` の両方が画面上にある状態
2. **長押し操作**
   - ソースバブルを 0.5 秒以上長押し
   - [ ] バブルがハイライトされる（指摘モード開始）
   - [ ] ヒントバナーが表示される（EnableHints=true の場合）
3. **タップ操作**
   - ターゲットバブルをタップ
   - [ ] 成功アニメーション（緑フラッシュ + 接続線 + パルス）が再生される
   - [ ] 「矛盾を発見しました」通知が表示される
   - [ ] HalluciCoin +10 が表示される
4. **不一致の場合**
   - 関係ないバブルを長押し → 別のバブルをタップ
   - [ ] 失敗アニメーション（赤フラッシュ）
   - [ ] クールダウン（10秒）が発動する
   - [ ] クールダウン中は新たな長押しを受け付けない
5. **既発見の場合**
   - 同じ矛盾ペアを再度指摘
   - [ ] 「既に発見済み」表示（黄色フラッシュ）

### Test E: 早送り (F11) + Debug Hub (F12)

1. Ch1 再生中に F11 を押す
   - [ ] Debug Overlay に `[FF]` が表示される
   - [ ] タイピング遅延がスキップされ、高速で進行する
2. F11 を再度押して OFF にする
   - [ ] 通常速度に戻る
3. F12 を押す
   - [ ] Debug Hub オーバーレイが表示される
   - [ ] ノード一覧が表示される
4. ノードを選択して再生確認

### Test F: エラーチェック

Play mode 中の Console を通して確認:

- [ ] NullReferenceException なし
- [ ] Yarn 変数エラーなし
- [ ] `[ChatDialogueView]` 警告なし
- [ ] `[ContradictionManager]` 想定外エラーなし

---

## 判定基準

**Phase A 完了条件**: Test A〜F の全チェック項目がパス

- Test A〜B がパス → エンジン基本動作 + Day Resume 確認完了
- Test C がパス → バブル表示バグ修正確認完了
- Test D がパス → 矛盾指摘 Phase 2 動作確認完了
- Test E〜F がパス → デバッグツール + エラーフリー確認完了

全パスで WORKFLOW_STATE_SSOT.md の Phase A Done 条件を完了とする。

---

## 次のステップ

1. Phase A テスト完了後 → Phase B（第1章設計）へ移行
2. Full Playthrough Test を実行（タイトル→チャット→セーブ→ロード）

## 関連ドキュメント

- [ENGINE_FEATURE_INVENTORY.md](ENGINE_FEATURE_INVENTORY.md) - エンジン機能リファレンス
- [UI_IMPLEMENTATION_SPEC.md](UI_IMPLEMENTATION_SPEC.md) - UI実装仕様
- [WORKFLOW_STATE_SSOT.md](WORKFLOW_STATE_SSOT.md) - ワークフロー状態管理
