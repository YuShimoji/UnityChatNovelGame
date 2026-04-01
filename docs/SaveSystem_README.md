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
  - シナリオ進行状況（現在のノード名、全Yarn変数）
  - 獲得済みトピックIDリスト
  - 使用済みレシピIDリスト（将来の拡張用）
- Yarn変数の保存は `DialogueRunner.VariableStorage.GetAllVariables()` で全変数を直接取得（DeductionBoard非依存）

#### SaveManager (`Assets/Scripts/Core/SaveManager.cs`)
- セーブ・ロード機能を管理するシングルトンマネージャー
- 主な機能:
  - `SaveGame(slotNumber)`: ゲームを保存
  - `LoadGame(slotNumber)`: ゲームをロード
  - `DeleteSave(slotNumber)`: セーブデータを削除
  - `HasSaveInSlot(slotNumber)`: セーブデータの存在確認
  - `GetSaveInfo(slotNumber)`: セーブデータ情報の取得
  - `GetAllSaveInfo()`: 全スロットの情報取得
  - `AutoSave(forceSave)`: オートセーブ実行（slot 99、30秒クールダウン付き）→ 詳細は `AUTOSAVE_DESIGN.md`
  - `HasAutoSave()`: オートセーブデータの存在確認
  - `GetAutoSaveInfo()`: オートセーブデータ情報の取得
  - `IsValidSlot(slotNumber)`: 通常スロット (0-2) またはオートセーブ (99) の判定

### 2. UIコンポーネント

#### SaveLoadUI (`Assets/Scripts/UI/SaveLoadUI.cs`)
- セーブ・ロードUIのメインコントローラー
- セーブモード/ロードモードの切り替え
- スロット一覧の表示と管理
- ロードモード時、オートセーブスロットを先頭に表示（`HasAutoSave()` が true の場合）

#### SaveSlotUI (`Assets/Scripts/UI/SaveSlotUI.cs`)
- 個別のセーブスロットUIコンポーネント
- セーブデータの表示
- クリックイベントのハンドリング
- オートセーブスロット表示対応（ラベル「Auto Save」、削除ボタン非表示）

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

ファイル名形式: `SaveData_<SlotNumber>.json`（オートセーブは `SaveData_99.json`）

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

### チャット履歴復元時の名前重複防止

`ChatController.RestoreChatHistory()` では、保存データから NPC メッセージを復元する際に `StripNamePrefix()` を呼び出す。
これは `CreateMessageBubble` が名前行リッチテキストを再付加するため、保存データに含まれる名前プレフィックスを事前に除去して二重表示を防ぐ。

パターン: `<line-height=N%><size=N><b>名前</b></size>\n</line-height>` を先頭から1回だけ除去。

### EndDay のセーブ先

`ScenarioManager.EndDayCommand()` は Day 進捗記録後に `SaveManager.Instance.AutoSave(forceSave: true)` を呼ぶ。
以前の `SaveGame(slotNumber)` による通常スロットへの保存ではなく、オートセーブスロット (99) に強制保存する。

### 3. 実機テスト
1. トピックを複数獲得
2. セーブを実行
3. ゲームを再起動
4. ロードを実行
5. トピックが復元されていることを確認

## サブスレッドのセーブ/ロード

### 追加フィールド (SaveData)

| フィールド | 型 | 説明 |
|-----------|-----|------|
| `Subthreads` | `List<SubthreadData>` | 宣言済みサブスレッド一覧 |
| `ActiveThreadId` | `string` | セーブ時に表示中のスレッドID（null = メイン） |

### SubthreadData の構造

```csharp
public class SubthreadData
{
    public string ThreadId;
    public string DisplayName;
    public ThreadType Type;           // Annotation / Tracking / Scout / Branch
    public bool IsLatent;             // 潜在状態 (UIに非表示)
    public string ManifestCondition;  // 顕在化条件 (Yarn変数式、null=手動)
    public bool AutoBeginBranch;      // 顕在化時に自動BeginBranch (Branch型)
    public bool IsCompleted;          // 完了状態 (サイドバーでグレーアウト)
    public int AcquiredTopicCount;    // 分岐内で取得したトピック数
    public int UnreadCount;
    public List<SavedChatMessage> ChatHistory;
}
```

### セーブフロー

1. `SaveManager.CreateSaveData()` が `ScenarioManager.GetAllDeclaredThreads()` を呼出
2. 各スレッドの `ChatHistory` はスレッド別に保持済み
3. `ChatController.GetAllThreadHistories()` からメインスレッド履歴を取得
4. `ActiveThreadId` を記録（サブスレッド表示中でもメイン履歴が正しく保存される）

### ロードフロー

1. `ThreadSwitcherController.Reset()` で UI をクリア
2. `ScenarioManager.ClearDeclaredThreads()` で既存スレッドをクリア
3. `saveData.Subthreads` から `ScenarioManager.RegisterDeclaredThread()` で再登録
4. `ChatController.SetThreadHistories()` でスレッド別履歴を復元
5. `ChatController.SetActiveThreadType(thread.Type)` で種別差異レンダリングを設定
6. `ChatController.SwitchToThread(activeThreadId)` で表示スレッドを復元

## 分岐スレッドのセーブ/ロード

### 追加フィールド (SaveData)

| フィールド | 型 | 説明 |
|-----------|-----|------|
| `BranchThread` | `BranchThreadState` | 分岐スレッドの実行状態 |

### BranchThreadState の構造

- `ActiveBranchId`: 実行中の分岐ID
- `IsActive`: 分岐実行中か
- `WasCompleted`: 分岐が完了したか
- `TransferFlags`: 分岐内で取得したフラグ一覧
- `TransferredFlags`: プレイヤーが持ち帰ると選択したフラグ (EndBranch "select" 時)
- `HiddenFlags`: プレイヤーが持ち帰らないと選択したフラグ
- `SelectionApplied`: 知識転送選択UIを通過したか
- `ReflectionMessage`: EndBranch 時の反映メッセージ (SetBranchReflection で設定)

### セーブ/ロードフロー

- セーブ: `ScenarioManager.GetBranchThreadStateSnapshot()` → `SaveData.BranchThread`
- ロード: `SaveData.BranchThread` → `ScenarioManager.ApplyBranchThreadState()`
- 分岐中にセーブした場合、ロード後に分岐状態が正しく復元される

### 後方互換性

旧セーブデータに `Subthreads` フィールドがない場合、デシリアライズ時に空リスト (`null` → `new List<SubthreadData>()`) として扱われる。`ActiveThreadId` が `null` の場合はメインスレッドとして処理される。

### JSON サンプル（サブスレッド部分）

```json
{
  "Subthreads": [
    {
      "ThreadId": "thread_notes",
      "DisplayName": "調査メモ",
      "Type": 0,
      "ChatHistory": [
        { "CharacterID": "", "Text": "初期メモ", "IsPlayerMessage": false }
      ],
      "UnreadCount": 0
    }
  ],
  "ActiveThreadId": null
}
```

---

## 既知の問題・制限事項 (2026-03-18 監査)

### ~~CRITICAL: YarnVariables の保存範囲~~ — **修正済み** (6d87d3e)

`SaveManager.GetYarnVariables()` は `$has_topic_*` プレフィックスの変数のみを保存する。`$halluci_coin` は `ApplySaveData` 内で `SetVariable<float>` を呼び出してYarn変数に同期するよう修正済み。

残存制限: Yarnスクリプト内で `<<set>>` された `$has_topic_*` 以外のカスタム変数は保存されない。`$speaker`, `$auto_speaker_after_choice` はランタイム専用で保存不要。

### ~~HIGH: 分岐中セーブ→ロード時の TransferFlags クリア~~ — **修正済み** (6d87d3e)

`BeginBranchThread()` で同一分岐が既にアクティブな場合は `TransferFlags.Clear()` をスキップするよう修正済み。

### ~~HIGH: EndBranch SelectionUI 中の StopScenario 割り込み~~ — **修正済み** (6d87d3e)

`StopScenario` で `TransferSelectionUI` が表示中なら強制非表示にするよう修正済み。Yarn側の非同期は `DialogueRunner.Stop()` で中断される。

### ~~MEDIUM: ContradictionManager.m_CurrentChannel のロード後未設定~~ — **修正済み**

`SaveData.CurrentChannelID` フィールドを追加。`ApplySaveData` で `ScenarioManager.SetCurrentChannel` と `ContradictionManager.SetCurrentChannel` の両方を復元するよう修正。EndDay のチャンネル進捗記録とヒントポリシーが Load 後も正しく動作する。

### ~~MEDIUM: 矛盾発見後の AutoSave 欠如~~ — **修正済み** (6d87d3e)

矛盾発見成功時 (`SelectSecond`) に `SaveManager.Instance.AutoSave()` を呼び出すよう修正済み。

### LOW: UnreadCount のロード後復元 — **コードレビュー正常** (2026-03-22)

`SubthreadData.UnreadCount` は Newtonsoft.Json で正しくシリアライズ/デシリアライズされる (public int field)。ロード時の流れ: `RegisterDeclaredThread` → `OnThreadDeclared` → `AddThreadEntry` → `UpdateBadge` (L616) で `GetDeclaredThread(threadId).UnreadCount` を参照してバッジ表示を更新する。コード上の問題なし。Unity Editor での手動確認で最終検証すること。

---

## 今後の拡張予定

### Phase 2
- [ ] セーブデータの暗号化
- [ ] 複数スロット対応（3スロット以上）
- [x] オートセーブ機能（EN-005, 6cc1a63）→ 詳細は `AUTOSAVE_DESIGN.md`
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
- ENGINE_FEATURE_INVENTORY.md: エンジン機能リファレンス
- HANDOFF.md: 開発状態の入口
