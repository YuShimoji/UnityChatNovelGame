# オートセーブ機能 設計書

**最終更新**: 2026-03-13
**実装コミット**: 6cc1a63 (EN-005)
**ステータス**: 実装済み

## 概要
ゲームの進行状況を自動的に保存する機能。プレイヤーの操作を中断せず、重要な進行ポイントで自動保存を行う。

---

## 1. スロット構成

### 方針: 専用スロット (slot 99)
- 手動セーブの3スロット（slot 0-2）とは独立
- ファイル名: `SaveData_99.json`（通常スロットと同じ命名規則）
- `IsValidSlot()` で通常スロット (0-2) とオートセーブスロット (99) を統一判定

### 定数

```csharp
public const int AutoSaveSlot = 99;
private const float AutoSaveCooldownSeconds = 30f;
private const float AutoSaveIndicatorDuration = 1f;
```

### ロード画面での表示
- **ロードモード**: 手動スロットの上部に「Auto Save」枠を表示
- **セーブモード**: 非表示（手動上書き不可）
- **削除ボタン**: 非表示（自動管理のため）

---

## 2. トリガータイミング

### 2.1 ノード遷移時（OnNodeEnter）
**意図**: Yarnノード切り替え = シーン進行の区切りで自動保存

```
タイミング: ChatDialogueView.OnNodeEnter()
条件: クールダウン (30秒) が経過している場合のみ
```

**実装箇所**: `ChatDialogueView.OnNodeEnter()`
```csharp
if (SaveManager.Instance != null)
{
    SaveManager.Instance.AutoSave();
}
```

### 2.2 選択肢表示前（RunOptionsAsync）
**意図**: 選択の直前に保存することで、選び直しが可能になる

```
タイミング: ChatDialogueView.RunOptionsAsync() の選択肢UI表示直前
条件: クールダウン (30秒) が経過している場合のみ
```

**実装箇所**: `ChatDialogueView.RunOptionsAsync()`
```csharp
if (SaveManager.Instance != null)
{
    SaveManager.Instance.AutoSave();
}
```

### 2.3 Day終了時（EndDay）
**意図**: 章の進行を確実に記録する。クールダウンを無視して強制保存

```
タイミング: ScenarioManager.EndDayCommand() 末尾
条件: 常時（forceSave: true でクールダウンを無視）
```

**実装箇所**: `ScenarioManager.EndDayCommand()`
```csharp
SaveManager.Instance.AutoSave(forceSave: true);
```

### トリガーまとめ

| トリガー | 場所 | forceSave | 頻度 | 備考 |
|---------|------|-----------|------|------|
| ノード遷移 | ChatDialogueView.OnNodeEnter | false | 高 | シーン区切りの自動保存 |
| 選択肢表示前 | ChatDialogueView.RunOptionsAsync | false | 中 | 選び直しポイント |
| Day終了 | ScenarioManager.EndDayCommand | true | 低 | 重要イベント。クールダウン無視 |

---

## 3. SaveManager 拡張

### API

```csharp
/// <summary>
/// オートセーブを実行する。クールダウン (30秒) 付き。
/// forceSave=true でクールダウンを無視。
/// 内部的には SaveGame(AutoSaveSlot) を呼ぶ。
/// </summary>
public bool AutoSave(bool forceSave = false)

/// <summary>オートセーブデータが存在するか</summary>
public bool HasAutoSave()

/// <summary>オートセーブデータの情報を取得</summary>
public SaveData GetAutoSaveInfo()

/// <summary>通常スロット (0-2) またはオートセーブ (99) か判定</summary>
private bool IsValidSlot(int slotNumber)
```

### 内部フィールド

```csharp
private float m_LastAutoSaveTime = -AutoSaveCooldownSeconds; // 初回即発火
private GameObject m_AutoSaveIndicator;
private Coroutine m_AutoSaveIndicatorCoroutine;
```

---

## 4. クールダウン制御

### 仕様
- 最小間隔: **30秒**
- `Time.time` ベースのクールダウン
- クールダウン中のトリガーは無視（キューイングしない）
- `forceSave: true` でクールダウンを無視（EndDay用）

### 理由
- 30秒あればノード遷移→選択肢表示のような連続イベントを間引ける
- EndDay は重要イベントのため、クールダウン中でも必ず保存する

---

## 5. UI通知（インジケーター）

### 実装方式
SaveManager 内で Canvas + CanvasGroup を動的生成する自己完結型。

### 表示仕様
- **位置**: 画面右上
- **テキスト**: 「Auto Saved」（白、18pt）
- **アニメーション**: CanvasGroup.alpha によるフェード
  - フェードイン 0.2秒 → 表示 1.0秒 → フェードアウト 0.3秒
- **Sorting Order**: 100（最前面）

### 実装構造

```
Canvas (ScreenSpaceOverlay, sortOrder=100)
└── AutoSaveIndicator (RectTransform + CanvasGroup)
    └── TextMeshProUGUI "Auto Saved"
```

- `EnsureAutoSaveIndicator()` で初回アクセス時に動的生成
- `ShowAutoSaveIndicator()` → `AutoSaveIndicatorRoutine()` コルーチンでフェード制御

---

## 6. ロード画面の変更

### 変更後のレイアウト（ロードモード時）

```
[Auto Save]  Auto Save - 2026/03/13 14:32  ← HasAutoSave() が true の時のみ表示
─────────────────────────────
[Slot 1] 手動セーブ
[Slot 2] 手動セーブ
[Slot 3] 手動セーブ
```

### 実装

- `SaveLoadUI.RefreshSlots()`: ロードモード時、`HasAutoSave()` が true なら先頭に AutoSave スロットを生成
- `SaveSlotUI.Setup()`: `isAutoSave` パラメータで表示を分岐
  - ラベル: 「Auto Save」（通常は「Slot N」）
  - 削除ボタン: 非表示

---

## 7. エラーハンドリング

### 書き込み失敗時
- `Debug.LogError` でログ出力
- トースト非表示（失敗を通知しない）
- ゲーム進行は中断しない（`SaveGame` の戻り値 false）

---

## 8. 影響ファイル

| ファイル | 変更内容 |
|---------|---------|
| `SaveManager.cs` | AutoSave(), HasAutoSave(), GetAutoSaveInfo(), IsValidSlot(), インジケーターUI |
| `ChatDialogueView.cs` | OnNodeEnter + RunOptionsAsync トリガー |
| `ScenarioManager.cs` | EndDayCommand 内で AutoSave(forceSave: true) |
| `SaveLoadUI.cs` | RefreshSlots でオートセーブスロット表示 |
| `SaveSlotUI.cs` | Setup/UpdateUI で isAutoSave 分岐 |

---

## 付録: 設計決定の経緯

| 項目 | 決定 | 理由 |
|------|------|------|
| スロット番号 | 99 | 通常スロット (0-2) と十分離れた値。SaveData_{N}.json の命名規則を統一 |
| クールダウン | 30秒 | 3秒では頻繁すぎ。30秒でノード遷移→選択肢の連続を間引く |
| forceSave | EndDay のみ | Day終了は確実に記録すべき重要イベント |
| 同期API | bool戻り値 | 非同期にする複雑さに見合わない。SaveGame の同期パスを再利用 |
| インジケーター | SaveManager内蔵 | 独立コンポーネント不要。Canvas動的生成で自己完結 |
