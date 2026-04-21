# SP-024: チャット没入仕様

**ステータス**: partial
**カテゴリ**: ui
**作成日**: 2026-04-15

2026-04-21 時点で、S3 の最小実装 (CharacterProfile の typing preset + `<<SetTypingSpeed>>` + `ChatDialogueView` の待機秒数解決) に加え、S1/S2/S5 は `ChatController` / `ScenarioManager` / `ChatUIConfig` まで含めて実装済み。具体的には `<<SetTime>>`、`<<MarkDelivered>>`、`<<MarkRead>>`、`<<DeleteLastMessage>>`、`<<DeleteMessage>>` が登録され、タイムスタンプ・既読/配信マーク・削除痕の Save/Load 復元まで接続済み。検証用 Yarn は `Assets/Resources/Yarn/active/SP024_ImmersionDemo.yarn` (`SP024_Immersion_Start`) を使う。未接続なのは S4 のオンライン状態 UI のみ。

---

## 1. 概要

FoundPhone は「拾ったスマホの中を覗く」チャット型 VN。チャットアプリとしてのリアリティを補強する没入要素を定義する。全機能は ChatUIConfig のフラグでオン/オフ切替可能。キャラ別パラメータは CharacterProfile に追加。既定は全て無効 (= 従来動作と完全互換)。

---

## 2. タイムスタンプ

補足:
- `showTimestamp` と `showDeliveryStatus` をオンにした状態で `SP024_Immersion_Start` を再生すると、S1/S2/S5 をまとめて見た目確認できる
- `SP024_Immersion_Start` は scene を変更せず StartNode 差し替えだけで回せる局所検証ノード

### 2.1 データ

SavedChatMessage に以下を追加:

| フィールド | 型 | 説明 |
|-----------|-----|------|
| timestamp | string | ゲーム内時刻 ("HH:mm" 形式)。null = 非表示 |

### 2.2 Yarn コマンド

```yarn
<<SetTime "14:32">>
Pyramid: この発言は 14:32 と表示される
Pyramid: これも 14:32（SetTime は次の SetTime まで継続）

<<SetTime "14:35">>
Marco: 時刻が変わった
```

- `<<SetTime>>` は以降のメッセージに継続適用 (メッセージ単位ではなく状態変更)
- `<<SetTime "">>` で時刻表示をクリア

### 2.3 表示

- バブル右下 (PlayerRow) または左下 (NpcRow) に小さく表示
- フォント: ChatUIConfig.messageFontSize の 60%
- 色: テキスト色の 50% alpha
- SystemMessage / Narration バブルには非表示

### 2.4 Day 区切りバー

- `<<EndDay N>>` コマンド実行時、チャット領域に日付区切りバーを自動挿入
- 表示例: `── Day 2 ──`
- ChatUIConfig.showDayDivider で切替

### 2.5 ChatUIConfig

| フィールド | 型 | 既定 | 説明 |
|-----------|-----|------|------|
| showTimestamp | bool | false | タイムスタンプ表示のグローバル切替 |
| showDayDivider | bool | true | Day 区切りバーの自動挿入 |

---

## 3. 既読/配信マーク

### 3.1 データ

SavedChatMessage に以下を追加:

| フィールド | 型 | 説明 |
|-----------|-----|------|
| deliveryStatus | DeliveryStatus | Sent / Delivered / Read |

```csharp
public enum DeliveryStatus
{
    None,       // 非表示 (NPC メッセージ、または機能無効時)
    Sent,       // 送信済み (チェック 1 個)
    Delivered,  // 配信済み (チェック 2 個、グレー)
    Read        // 既読 (チェック 2 個、青)
}
```

### 3.2 適用対象

- プレイヤーメッセージのみ。NPC / SystemMessage には `None`
- 新規メッセージは `Sent` で開始

### 3.3 状態遷移

```
Sent → Delivered → Read
```

- 自動遷移: `ChatUIConfig.autoReadDelay` 秒後に Sent → Delivered → Read (各ステップで delay 適用)
- autoReadDelay = 0 の場合は自動遷移なし (Yarn コマンドで手動制御)

### 3.4 Yarn コマンド

```yarn
Player: これを送信する
<<MarkDelivered>>
<<StartWait 1.5>>
<<MarkRead>>
```

- `<<MarkDelivered>>`: 直前のプレイヤーメッセージを Delivered に変更
- `<<MarkRead>>`: 直前のプレイヤーメッセージを Read に変更
- 直前メッセージが NPC の場合は無視

### 3.5 表示

- バブル右下、タイムスタンプの右隣
- Sent: 灰チェック 1 個
- Delivered: 灰チェック 2 個
- Read: 青チェック 2 個 (色は ChatUIConfig で設定可能)

### 3.6 ChatUIConfig

| フィールド | 型 | 既定 | 説明 |
|-----------|-----|------|------|
| showDeliveryStatus | bool | false | 既読/配信マーク表示 |
| autoReadDelay | float | 2.0 | 自動遷移の待機秒数 (0=無効) |
| deliveryReadColor | Color | #4A90D9 | 既読チェックマークの色 |

---

## 4. タイピングパターン (キャラ別カスタマイズ)

### 4.1 現状の問題

`ChatDialogueView.m_TypingIndicatorDuration` (現在は ChatUIConfig.typingIndicatorDuration) が全キャラクター統一 (0.8s)。キャラクターの性格や状況に応じた速度差がない。

### 4.2 CharacterProfile 拡張

| フィールド | 型 | 既定 | 説明 |
|-----------|-----|------|------|
| typingSpeed | TypingSpeed | Default | タイピング速度プリセット |
| customTypingDelay | float | 0.0 | typingSpeed=Custom のときの秒数 |

```csharp
public enum TypingSpeed
{
    Default,    // ChatUIConfig.typingIndicatorDuration を使用
    Instant,    // 0s (インジケーター非表示)
    Fast,       // 0.3s
    Normal,     // 0.8s (ChatUIConfig 既定と同一)
    Slow,       // 1.5s
    VerySlow,   // 3.0s
    Custom      // customTypingDelay の値を使用
}
```

### 4.3 解決ロジック

ChatDialogueView.RunLineAsync で:

```
1. `ScenarioManager` のセッション override を確認
2. override がなければ `CharacterDatabase.Instance.GetProfile(charID)` を取得
3. profile.typingSpeed が Default → ChatUIConfig.typingIndicatorDuration を使用
4. profile.typingSpeed が Custom → profile.customTypingDelay を使用
5. それ以外 → enum に対応する固定値を使用

2026-04-21 の最小実装では、この解決ロジックは **NPC 発話前の typing indicator 待機秒数** のみに適用する。タイプライターの 1 文字あたり速度はまだグローバルのまま。
```

### 4.4 Yarn コマンドによる動的変更

```yarn
// Pyramid の速度を一時的に遅くする (AI 劣化演出)
<<SetTypingSpeed "pyramid" "veryslow">>
Pyramid: ...........
<<SetTypingSpeed "pyramid" "normal">>
```

- `<<SetTypingSpeed charID speed>>`: 指定キャラの typingSpeed を動的変更
- speed: "default" / "instant" / "fast" / "normal" / "slow" / "veryslow" / "custom:1.2"
- セッション内のみ有効 (Save/Load では CharacterProfile の既定に戻る)

### 4.5 タイプライター速度 (補足)

`ChatUIConfig.typewriterSpeed` (1文字あたり秒) もキャラ別にしたい場合は同じパターンで CharacterProfile に `typewriterSpeedOverride` (float, -1 = ChatUIConfig 既定) を追加可能。ただし現時点では保留 (HUMAN_AUTHORITY)。

---

## 5. オンライン状態

### 5.1 CharacterProfile 拡張

| フィールド | 型 | 既定 | 説明 |
|-----------|-----|------|------|
| defaultOnlineStatus | OnlineStatus | Online | 起動時のオンライン状態 |

```csharp
public enum OnlineStatus
{
    Online,     // 緑ドット
    Away,       // 黄ドット
    Offline,    // 灰ドット
    Hidden      // ドット非表示 (存在を隠す)
}
```

オンライン状態や派生アイコンは「感情の固定表現」ではなく、通信状態・異常状態・更新状態などの state key を前提に扱う。感情差分は必要になった時だけ別レイヤーで検討する。

### 5.2 Yarn コマンド

```yarn
<<SetOnlineStatus "pyramid" "offline">>
<<StartWait 3>>
<<SetOnlineStatus "pyramid" "online">>
Pyramid: ただいま
```

- セッション内のみ有効 (Save/Load では defaultOnlineStatus に戻る)

### 5.3 表示

- アイコン右下に小ドット (直径 10px)
- アイコン非表示の場合、名前行の右に小テキスト ("Online" / "Away") で代替
- showCharacterIcon=false かつ showOnlineStatus=true の場合はテキスト表示
- メインスレッドのヘッダーバーにも対話相手の状態を表示

### 5.4 ChatUIConfig

| フィールド | 型 | 既定 | 説明 |
|-----------|-----|------|------|
| showOnlineStatus | bool | false | オンライン状態表示 |
| onlineColor | Color | #4CAF50 | Online ドットの色 |
| awayColor | Color | #FFC107 | Away ドットの色 |
| offlineColor | Color | #9E9E9E | Offline ドットの色 |

---

## 6. メッセージ削除痕

### 6.1 データ

SavedChatMessage に以下を追加:

| フィールド | 型 | 説明 |
|-----------|-----|------|
| isDeleted | bool | 削除済みフラグ (true = 削除痕表示) |

### 6.2 Yarn コマンド

```yarn
Pyramid: 秘密の情報だ
<<DeleteLastMessage>>
// 上のメッセージが「このメッセージは削除されました」に置換される

// 特定のメッセージを削除 (チャット履歴の末尾から N 番目)
<<DeleteMessage 3>>
```

- `<<DeleteLastMessage>>`: 直前のメッセージを削除済みに変更
- `<<DeleteMessage N>>`: 末尾から N 番目のメッセージを削除済みに変更 (1 = 最後)
- 削除済みバブルは元のテキストを保持せず、固定テキストに置換

### 6.3 表示

- バブル背景: 薄グレー (alpha 0.3)
- テキスト: イタリック、薄色、「このメッセージは削除されました」
- バブル形状: 既定バブルと同一 (スプライト維持)
- アイコン: 表示しない
- タイムスタンプ: 元の時刻を維持

### 6.4 Save/Load

- `isDeleted = true` のメッセージは復元時も削除痕として表示
- 元のテキストは保存しない (プライバシー演出)

---

## 7. グローバル切替まとめ

ChatUIConfig に追加するフィールド一覧:

| フィールド | 型 | 既定 | セクション |
|-----------|-----|------|-----------|
| showTimestamp | bool | false | S1 |
| showDayDivider | bool | true | S1 |
| showDeliveryStatus | bool | false | S2 |
| autoReadDelay | float | 2.0 | S2 |
| deliveryReadColor | Color | #4A90D9 | S2 |
| showOnlineStatus | bool | false | S4 |
| onlineColor | Color | #4CAF50 | S4 |
| awayColor | Color | #FFC107 | S4 |
| offlineColor | Color | #9E9E9E | S4 |

CharacterProfile に追加するフィールド一覧:

| フィールド | 型 | 既定 | セクション |
|-----------|-----|------|-----------|
| typingSpeed | TypingSpeed | Default | S3 |
| customTypingDelay | float | 0.0 | S3 |
| defaultOnlineStatus | OnlineStatus | Online | S4 |

SavedChatMessage に追加するフィールド一覧:

| フィールド | 型 | 既定 | セクション |
|-----------|-----|------|-----------|
| timestamp | string | null | S1 |
| deliveryStatus | DeliveryStatus | None | S2 |
| isDeleted | bool | false | S5 |

---

## 8. 新規 Yarn コマンドまとめ

| コマンド | パラメータ | 説明 | セクション |
|---------|-----------|------|-----------|
| `<<SetTime>>` | "HH:mm" | 以降のメッセージに時刻を付与 | S1 |
| `<<MarkDelivered>>` | なし | 直前プレイヤーメッセージを配信済みに | S2 |
| `<<MarkRead>>` | なし | 直前プレイヤーメッセージを既読に | S2 |
| `<<SetTypingSpeed>>` | charID, speed | キャラのタイピング速度を動的変更 | S3 |
| `<<SetOnlineStatus>>` | charID, status | キャラのオンライン状態を動的変更 | S4 |
| `<<DeleteLastMessage>>` | なし | 直前メッセージを削除痕に | S5 |
| `<<DeleteMessage>>` | N (int) | 末尾から N 番目を削除痕に | S5 |

---

## 9. 実装ファイルマップ

| ファイル | 変更内容 |
|---------|---------|
| `Assets/Scripts/Data/ChatUIConfig.cs` | S7 のフィールド追加 |
| `Assets/Scripts/Data/CharacterProfile.cs` | typingSpeed, defaultOnlineStatus 追加 |
| `Assets/Scripts/Data/SaveData.cs` | SavedChatMessage に timestamp/deliveryStatus/isDeleted 追加 |
| `Assets/Scripts/UI/ChatController.cs` | タイムスタンプ表示、既読マーク表示、削除痕バブル、オンラインドット |
| `Assets/Scripts/UI/ChatDialogueView.cs` | キャラ別 typingIndicatorDuration 解決ロジック |
| `Assets/Scripts/Core/ScenarioManager.cs` | `SetTypingSpeed` 実装、残りコマンド群の受け皿 |

---

## 10. 実装順序 (推奨)

| 順序 | スライス | 依存 |
|------|---------|------|
| 1 | S3: タイピングパターン (CharacterProfile 拡張 + ChatDialogueView 修正) | なし | 実装済み |
| 2 | S1: タイムスタンプ (SavedChatMessage 拡張 + バブル表示) | なし | 実装済み |
| 3 | S2: 既読/配信マーク | S1 (タイムスタンプ横に表示) | 実装済み |
| 4 | S4: オンライン状態 | なし | 未実装 |
| 5 | S5: メッセージ削除痕 | なし | 実装済み |

次の推奨順は、SP-023 の画面検収を閉じた後に S1/S2/S5 の Unity 見た目確認、その次に S4 のオンライン状態 UI です。
