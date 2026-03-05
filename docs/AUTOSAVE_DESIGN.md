# オートセーブ機能 設計書

## 概要
ゲームの進行状況を自動的に保存する機能。プレイヤーの操作を中断せず、重要な進行ポイントで自動保存を行う。

---

## 1. スロット構成

### 方針: 専用スロット
- オートセーブ専用の隠しスロットを使用
- 手動セーブの3スロット（slot 0-2）とは独立
- ファイル名: `AutoSave.json`（既存の `SaveData_{n}.json` と区別）

### スロット定数
```csharp
public const int AUTO_SAVE_SLOT = -1; // 手動スロット(0-2)と区別
public const string AUTO_SAVE_FILENAME = "AutoSave.json";
```

### ロード画面での表示
- 手動スロットの上部に「オートセーブ」枠を1つ表示
- ラベル: 「オートセーブ - {日時}」
- 手動セーブとは異なるスタイル（背景色やアイコンで区別）

---

## 2. トリガータイミング

### 2.1 選択肢表示前（優先度: 高）
**意図**: 選択の直前に保存することで、選び直しが可能になる

```
タイミング: RunOptionsAsync() の選択肢UI表示直前
条件: 常時（早送りモードでもセーブする）
```

**実装箇所**: `ChatDialogueView.RunOptionsAsync()`
```csharp
// 選択肢表示前にオートセーブ
await SaveManager.Instance.AutoSave();
// 既存の200ms遅延
if (!m_FastForwardEnabled)
{
    await YarnTask.Delay(200, ...);
}
m_ChatController.ShowChoices(...);
```

### 2.2 チャプター終了時（優先度: 高）
**意図**: 章の完了を記録し、次回起動時に続きから再開可能にする

```
タイミング: $chX_completed = true 設定直後
条件: チャプター完了フラグが true に変わった時
```

**実装方法**: Yarn変数の変更監視
```csharp
// ScenarioManager で $ch*_completed の変更を検知
// または <<AutoSave>> カスタムコマンドをYarnに追加
```

### 2.3 トピック解放時（優先度: 中）
**意図**: プレイ労力の報酬（トピック獲得）を確実に保存する

```
タイミング: UnlockTopic コマンド実行直後
条件: 新規トピックの場合のみ（既に解放済みのトピックでは発火しない）
```

**実装箇所**: `ChatDialogueView` の UnlockTopic ハンドラ
```csharp
// トピック解放後にオートセーブ
if (isNewTopic)
{
    await SaveManager.Instance.AutoSave();
}
```

### トリガー優先度まとめ
| トリガー | 優先度 | 頻度 | 備考 |
|---------|--------|------|------|
| 選択肢表示前 | 高 | 中 | 選び直しポイント |
| チャプター終了 | 高 | 低 | 章区切りの確定保存 |
| トピック解放 | 中 | 低 | 報酬獲得の保全 |

---

## 3. SaveManager 拡張

### 新規メソッド

```csharp
/// <summary>
/// オートセーブを実行する。
/// 連続トリガー防止のクールダウン付き。
/// </summary>
public async YarnTask AutoSave()
{
    if (Time.time - m_LastAutoSaveTime < AUTO_SAVE_COOLDOWN)
        return;

    m_LastAutoSaveTime = Time.time;

    SaveData data = CreateSaveData(AUTO_SAVE_SLOT);
    data.IsAutoSave = true;

    string json = JsonConvert.SerializeObject(data, Formatting.Indented);
    string path = Path.Combine(Application.persistentDataPath, AUTO_SAVE_FILENAME);

    // 非同期書き込み（メインスレッドをブロックしない）
    await File.WriteAllTextAsync(path, json);

    // トースト通知
    AutoSaveCompleted?.Invoke();
}
```

### 新規フィールド
```csharp
private float m_LastAutoSaveTime = -999f;
private const float AUTO_SAVE_COOLDOWN = 3f; // 3秒クールダウン

public event System.Action AutoSaveCompleted;
```

### SaveData 拡張
```csharp
public class SaveData
{
    // 既存フィールド...

    /// <summary>オートセーブかどうか</summary>
    public bool IsAutoSave;
}
```

---

## 4. クールダウン制御

### 問題
選択肢表示前とトピック解放が近接して発火する場合、連続セーブを防止する必要がある。

### 解決策
- 最小間隔: 3秒
- `Time.time` ベースのクールダウン
- クールダウン中のトリガーは無視（キューイングしない）

### 理由
- 3秒あれば1つの重要イベントの保存には十分
- 直後に別のトリガーが来ても、状態に大きな差はない
- キューイングは複雑化の割にメリットが少ない

---

## 5. UI通知（トースト）

### 表示仕様
- **位置**: 画面右上
- **テキスト**: 「オートセーブ完了」
- **表示時間**: 1.5秒（フェードイン0.2s + 表示1.0s + フェードアウト0.3s）
- **スタイル**: 半透明背景、小さめフォント（チャットの邪魔にならない）

### 実装方針
```csharp
public class AutoSaveToast : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private TextMeshProUGUI m_Text;

    public void Show()
    {
        // DOTween Sequence
        // FadeIn(0.2s) → Wait(1.0s) → FadeOut(0.3s)
    }
}
```

### SaveManagerとの連携
```csharp
// AutoSaveToast.cs
void OnEnable()
{
    SaveManager.Instance.AutoSaveCompleted += Show;
}
```

---

## 6. ロード画面の変更

### 現在のレイアウト
```
[Slot 0] 手動セーブ
[Slot 1] 手動セーブ
[Slot 2] 手動セーブ
```

### 変更後のレイアウト
```
[Auto]   オートセーブ - 2025/01/15 14:32  ← 新規追加
─────────────────────────────
[Slot 0] 手動セーブ
[Slot 1] 手動セーブ
[Slot 2] 手動セーブ
```

### オートセーブスロットの操作
- **ロードモード**: クリックでロード可能
- **セーブモード**: 非表示（手動上書き不可）
- **削除**: 不可（自動管理のため）

---

## 7. エラーハンドリング

### 書き込み失敗時
- ログ出力（`Debug.LogWarning`）のみ
- トースト非表示（失敗を通知しない）
- ゲーム進行は中断しない

### ファイル破損時
- ロード時に `IsValid()` チェック
- 無効な場合はスロットを「破損」と表示
- 破損スロットは削除可能

### ディスク容量不足時
- `IOException` をキャッチ
- ログ出力のみ（ゲーム進行優先）

---

## 8. Yarnカスタムコマンド（オプション）

チャプター終了時のセーブを明示的に制御するため、カスタムコマンドも用意する。

```yarn
// Ch1_DayEnd ノード末尾
<<set $ch1_completed to true>>
<<AutoSave>>
```

```csharp
// ChatDialogueView.cs
[YarnCommand("AutoSave")]
public async YarnTask AutoSaveCommand()
{
    await SaveManager.Instance.AutoSave();
}
```

---

## 9. 実装順序

### Phase 1: 基盤（必須）
1. SaveManager に AutoSave() メソッド追加
2. SaveData に IsAutoSave フィールド追加
3. AutoSave.json の読み書き

### Phase 2: トリガー接続
4. 選択肢表示前トリガー（ChatDialogueView）
5. チャプター終了トリガー（<<AutoSave>> コマンド）
6. トピック解放トリガー（UnlockTopic ハンドラ）

### Phase 3: UI
7. AutoSaveToast コンポーネント作成
8. ロード画面にオートセーブスロット追加

### Phase 4: テスト
9. EditMode テスト（AutoSave ファイル作成/読み取り）
10. PlayMode テスト（トリガー発火確認）

---

## 10. 影響範囲

| ファイル | 変更内容 |
|---------|---------|
| `SaveManager.cs` | AutoSave()、AutoSaveCompleted イベント追加 |
| `SaveData.cs` | IsAutoSave フィールド追加 |
| `ChatDialogueView.cs` | 選択肢前トリガー、AutoSave コマンド追加 |
| `SaveLoadUI.cs` | オートセーブスロット表示追加 |
| `AutoSaveToast.cs` | 新規作成 |
| `Ch1_Terminal.yarn` | <<AutoSave>> コマンド追加 |
| `Ch2_LocationConfusion.yarn` | <<AutoSave>> コマンド追加 |

---

## 付録: 仕様決定の経緯

| 項目 | 決定 | 理由 |
|------|------|------|
| スロット | 専用 | 手動セーブを圧迫しない |
| トリガー | 選択肢前/章末/トピック解放 | プレイ労力の保全 + 選び直し可能 |
| 通知 | トースト | 没入感を維持しつつ保存を確認 |
| クールダウン | 3秒 | 連続トリガーの実害防止 |
