# Save System - 実装ガイド

## 概要
ゲーム進行状況を保存・読み込みできるセーブシステムの実装。

## 実装内容

### 1. コアコンポーネント

#### SaveData (`Assets/Scripts/Data/SaveData.cs`)
- ゲームの保存データを表すシリアライズ可能なクラス
- JSON形式で保存
- 保存内容:
  - セーブメタデータ（バージョン、日時、スロット番号）
  - シナリオ進行状況（現在のノード名、Yarn変数）
  - 獲得済みトピックIDリスト
  - 使用済みレシピIDリスト（将来の拡張用）

#### SaveManager (`Assets/Scripts/Core/SaveManager.cs`)
- セーブ・ロード機能を管理するシングルトンマネージャー
- 主な機能:
  - `SaveGame(slotNumber)`: ゲームを保存
  - `LoadGame(slotNumber)`: ゲームをロード
  - `DeleteSave(slotNumber)`: セーブデータを削除
  - `HasSaveInSlot(slotNumber)`: セーブデータの存在確認
  - `GetSaveInfo(slotNumber)`: セーブデータ情報の取得
  - `GetAllSaveInfo()`: 全スロットの情報取得

### 2. UIコンポーネント

#### SaveLoadUI (`Assets/Scripts/UI/SaveLoadUI.cs`)
- セーブ・ロードUIのメインコントローラー
- セーブモード/ロードモードの切り替え
- スロット一覧の表示と管理

#### SaveSlotUI (`Assets/Scripts/UI/SaveSlotUI.cs`)
- 個別のセーブスロットUIコンポーネント
- セーブデータの表示
- クリックイベントのハンドリング

### 3. デバッグツール

#### SaveSystemDebugger (`Assets/Scripts/Editor/SaveSystemDebugger.cs`)
- エディタウィンドウでセーブシステムをデバッグ
- メニュー: `Project FoundPhone/Debug/Save System Debugger`
- 機能:
  - スロット選択
  - セーブ/ロード/削除の実行
  - セーブ情報の表示
  - 全スロット概要の表示
  - Persistent Data Pathを開く

#### SaveSystemTests (`Assets/Scripts/Tests/SaveSystemTests.cs`)
- 単体テスト
- テスト項目:
  - SaveDataの生成と検証
  - セーブ/ロード/削除の動作確認
  - スロット情報の取得

## セットアップ手順

### 1. SaveManagerの配置
1. 空のGameObjectを作成（名前: `SaveManager`）
2. `SaveManager`コンポーネントをアタッチ
3. 設定:
   - Save File Prefix: `SaveData`（デフォルト）
   - Save File Extension: `.json`（デフォルト）
   - Max Save Slots: `3`（デフォルト）

### 2. SaveLoadUIの作成
1. Canvas内に空のGameObjectを作成（名前: `SaveLoadUI`）
2. `SaveLoadUI`コンポーネントをアタッチ
3. UI構造を作成:
   ```
   SaveLoadUI
   ├─ Panel (背景パネル)
   │  ├─ TitleText (TextMeshProUGUI)
   │  ├─ SlotContainer (Vertical Layout Group)
   │  └─ CloseButton (Button)
   ```
4. `SaveLoadUI`の設定:
   - Panel: 背景パネルを設定
   - Slot Container: スロット一覧の親オブジェクト
   - Slot Prefab: `SaveSlotUI`プレハブを設定
   - Close Button: 閉じるボタンを設定
   - Title Text: タイトルテキストを設定

### 3. SaveSlotUIプレハブの作成
1. 新しいGameObjectを作成（名前: `SaveSlotUI`）
2. `SaveSlotUI`コンポーネントをアタッチ
3. UI構造を作成:
   ```
   SaveSlotUI
   ├─ MainButton (Button)
   │  ├─ SlotNumberText (TextMeshProUGUI)
   │  ├─ SaveDataPanel
   │  │  └─ SaveInfoText (TextMeshProUGUI)
   │  └─ EmptySlotPanel
   │     └─ EmptySlotText (TextMeshProUGUI)
   └─ DeleteButton (Button)
   ```
4. `SaveSlotUI`の設定:
   - Main Button: メインボタン
   - Delete Button: 削除ボタン
   - Slot Number Text: スロット番号表示
   - Save Info Text: セーブ情報表示
   - Empty Slot Text: 空スロット表示
   - Save Data Panel: セーブデータがある時に表示
   - Empty Slot Panel: 空スロットの時に表示
5. Prefabとして保存

## 使用方法

### コードからの使用

```csharp
// セーブ
SaveManager.Instance.SaveGame(0); // スロット0に保存

// ロード
SaveManager.Instance.LoadGame(0); // スロット0からロード

// 削除
SaveManager.Instance.DeleteSave(0); // スロット0を削除

// セーブデータの存在確認
bool hasSave = SaveManager.Instance.HasSaveInSlot(0);

// セーブ情報の取得
SaveData info = SaveManager.Instance.GetSaveInfo(0);
if (info != null)
{
    Debug.Log(info.GetSummary());
}
```

### UIからの使用

```csharp
// SaveLoadUIの参照を取得
SaveLoadUI saveLoadUI = FindFirstObjectByType<SaveLoadUI>();

// セーブモードで表示
saveLoadUI.ShowSaveMode();

// ロードモードで表示
saveLoadUI.ShowLoadMode();

// 非表示
saveLoadUI.Hide();
```

## 保存場所

セーブデータは`Application.persistentDataPath`に保存されます:
- **Windows**: `%USERPROFILE%\AppData\LocalLow\<CompanyName>\<ProductName>\`
- **Mac**: `~/Library/Application Support/<CompanyName>/<ProductName>/`
- **Linux**: `~/.config/unity3d/<CompanyName>/<ProductName>/`

ファイル名形式: `SaveData_<SlotNumber>.json`

## テスト手順

### 1. エディタでのテスト
1. Play Modeに入る
2. `Project FoundPhone/Debug/Save System Debugger`を開く
3. スロットを選択して「Save Game」をクリック
4. 「Load Game」をクリックして復元を確認
5. 「Delete Save」で削除を確認

### 2. ユニットテストの実行
1. Test Runnerウィンドウを開く（`Window > General > Test Runner`）
2. PlayModeタブを選択
3. `SaveSystemTests`を実行
4. 全テストが通過することを確認

### 3. 実機テスト
1. トピックを複数獲得
2. セーブを実行
3. ゲームを再起動
4. ロードを実行
5. トピックが復元されていることを確認

## 今後の拡張予定

### Phase 2
- [ ] セーブデータの暗号化
- [ ] 複数スロット対応（3スロット以上）
- [ ] オートセーブ機能
- [ ] クラウドセーブ対応

### Phase 3
- [ ] セーブデータのバージョン管理と互換性処理
- [ ] セーブデータの圧縮
- [ ] セーブデータの整合性チェック（チェックサム）

## トラブルシューティング

### セーブが失敗する
- `Application.persistentDataPath`への書き込み権限を確認
- ディスク容量を確認
- Consoleでエラーメッセージを確認

### ロードが失敗する
- セーブファイルが存在するか確認
- JSONフォーマットが正しいか確認
- セーブデータのバージョンが一致するか確認

### トピックが復元されない
- `Resources/Topics/`にTopicDataが存在するか確認
- TopicIDが正しく保存されているか確認
- DeductionBoardがシーンに存在するか確認

## 関連ドキュメント
- Core Specification: ゲーム全体の仕様
- HANDOVER.md: プロジェクト引き継ぎドキュメント
- TASK_028_SaveSystem.md: タスク定義
