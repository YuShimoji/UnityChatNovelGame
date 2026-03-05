# MVPテストガイド

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

## 次のステップ
1. このテストが成功したら、docs/evidence/TASK_027/にスクリーンショットを保存
2. Full Playthrough Testを実行（タイトル→チャット→セーブ→ロード）
3. 追加のシナリオ作成（複雑な分岐、画像表示、矛盾指摘など）

## 関連ドキュメント
- [GAME_DESIGN_DOCUMENT.md](../GAME_DESIGN_DOCUMENT.md) - 設計書
- [TASK_027](../tasks/TASK_027_Full_Playthrough_Test.md) - Full Playthrough Test
- [AI_CONTEXT.md](../AI_CONTEXT.md) - プロジェクト概要
