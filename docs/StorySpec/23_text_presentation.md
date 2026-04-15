# SP-023: テキスト表現仕様

**ステータス**: draft
**カテゴリ**: ui
**作成日**: 2026-04-15
**改訂日**: 2026-04-15

---

## 1. 概要

チャットバブルの位置・スタイル・リッチテキスト表現を定義する。現状実装の記録に加え、新規仕様（バブルタイプ追加、地の文、アイコン向き、% 指定マージン）を含む。

---

## 2. バブル配置: % 指定マージン

### 2.1 現行パターン (実装済み)

| パターン | 水平配置 | 幅制御 | 使用条件 |
|---------|---------|--------|---------|
| NpcRow | 左寄せ | テキスト幅フィット (最大70%/600px) | NPC 発話 |
| PlayerRow | 右寄せ | テキスト幅フィット (同上) | プレイヤー発話 |
| AnnotationRow | 中央配置 | テキスト幅フィット (同上) | A型注釈スレッド |
| SystemBubble | 全幅ストレッチ | 親幅100% | SystemMessage |
| ChoiceContainer | 右寄せ | バブル幅と同一制約 | 選択肢表示 |

### 2.2 新規: % 指定マージン (BubbleMargin)

Yarn コマンドで次メッセージのバブル占有範囲を上下左右 % で指定する。

```yarn
<<BubbleMargin left right top bottom>>
Pyramid: このメッセージは指定マージンで配置される
```

| パラメータ | 型 | 単位 | 既定値 | 説明 |
|-----------|-----|------|--------|------|
| left | float | % (0-100) | 0 | 左端からのマージン (コンテナ幅に対する%) |
| right | float | % (0-100) | 0 | 右端からのマージン |
| top | float | % (0-100) | 0 | 上方向の追加マージン (バブル間スペーシングに加算) |
| bottom | float | % (0-100) | 0 | 下方向の追加マージン |

**動作**:
- `<<BubbleMargin>>` は次の 1 メッセージにのみ適用。適用後にリセット
- 省略時は現行ロジック (NpcRow/PlayerRow 等) がそのまま使われる
- left + right が 100 を超える場合は最小幅に制約

**使用例**:

```yarn
// 中央寄せ (左右均等に 15% ずつ)
<<BubbleMargin 15 15 0 0>>
Pyramid: 中央に配置されるメッセージ

// 右端に寄せて幅を狭く
<<BubbleMargin 40 5 0 0>>
Pyramid: 右側に小さく表示

// 上に大きめの余白
<<BubbleMargin 0 0 10 0>>
<<SystemMessage "--- 場面転換 ---">>
```

**実装方針**:
- ScenarioManager に `<<BubbleMargin>>` コマンドを登録
- ChatController に `m_NextBubbleMargin` (Vector4?) フィールドを追加
- ConfigureBubble() の HLG.padding 計算でコンテナ幅 * % を反映
- 適用後に null リセット

### 2.3 現行の幅計算ロジック (参考)

```
containerWidth = ScrollRect の RectTransform 幅
basePercent = ChatUIConfig.bubbleMaxWidthPercent (既定 0.7)

レスポンシブ調整:
  containerWidth < 800px → Max(basePercent, 0.85)
  containerWidth < 1000px → 0.85 → basePercent へ線形補間
  containerWidth >= 1000px → basePercent

maxBubbleWidth = Min(containerWidth * adjustedPercent, ChatUIConfig.bubbleMaxWidthPx)
fitWidth = Min(naturalTextWidth + padding + margin, maxBubbleWidth)
```

---

## 3. バブルタイプ (BubbleStyle)

### 3.1 概要

バブルの見た目 (色・背景・角丸・影・テキストスタイル) をプリセットとして定義し、Yarn コマンドで切り替える。

### 3.2 BubbleStylePreset (ScriptableObject)

`Assets/Scripts/Data/BubbleStylePreset.cs` として新設する。

| フィールド | 型 | 説明 |
|-----------|-----|------|
| presetId | string | Yarn から参照する ID (例: `"narration"`, `"whisper"`, `"shout"`) |
| displayName | string | Editor 表示名 |
| backgroundColor | Color | バブル背景色 (null = キャラ ThemeColor を使用) |
| backgroundAlpha | float | 背景の不透明度 (0 = 背景なし = 地の文) |
| bubbleSprite | Sprite | 9-slice スプライト (null = ChatUIConfig 既定) |
| cornerRadius | float | 自動生成角丸の半径 (-1 = ChatUIConfig 既定) |
| shadowEnabled | bool | 影の有無 |
| shadowColor | Color | 影の色 |
| shadowDistance | Vector2 | 影のオフセット |
| textColor | Color | テキスト色 (null = ChatUIConfig 既定) |
| fontSize | float | フォントサイズ (-1 = ChatUIConfig 既定) |
| fontStyle | FontStyles | Normal / Bold / Italic / BoldItalic |
| textAlignment | TextAlignmentOptions | TopLeft / Center 等 |
| padding | RectOffset | バブル内パディング |

### 3.3 組み込みプリセット

以下を初期プリセットとして提供する。

| presetId | 用途 | 背景 | テキスト | 備考 |
|----------|------|------|---------|------|
| `default` | 通常メッセージ | キャラ ThemeColor | NPC/Player色 | 現行動作と同一 |
| `narration` | 地の文・ナレーション | alpha=0 (透明) | 薄いグレー、イタリック、中央揃え | 背景なしの全幅テキスト |
| `thought` | 内心・独白 | 半透明ダーク | イタリック | バブルあり、思考表現 |
| `shout` | 叫び・強調 | 明るい赤系 | 白、太字 | 警告や衝撃の表現 |
| `whisper` | ささやき・小声 | 半透明 | 小サイズ、薄色 | 控えめな表現 |
| `system` | システムメッセージ | 現行 SystemMessage 相当 | 中央、イタリック | 現行動作と同一 |
| `announcement` | 通知・アナウンス | 型色帯 | 白、太字 | Day 遷移等のアナウンス |

### 3.4 Yarn コマンド

```yarn
// 次の1メッセージにプリセットを適用
<<BubbleStyle "narration">>
地の文として表示されるテキスト。背景なし、イタリック。

// 明示的にデフォルトに戻す
<<BubbleStyle "default">>
Pyramid: 通常のバブルに戻る

// 地の文の短縮形
<<Narration "シーンが暗転する。静寂が訪れた。">>
```

**`<<Narration>>`** は `<<BubbleStyle "narration">>` + `<<SystemMessage>>` の短縮形。

**動作**:
- `<<BubbleStyle>>` は次の 1 メッセージにのみ適用、適用後にリセット
- `<<BubbleMargin>>` と併用可能 (BubbleStyle が見た目、BubbleMargin が位置)
- プリセットに定義がないフィールドは ChatUIConfig / CharacterProfile のフォールバック

### 3.5 地の文パターン (`narration`)

背景なしの全幅テキスト。ナレーション・場面描写・心理描写に使用。

**見た目**:
- 背景: 完全透明 (`backgroundAlpha = 0`)
- 影: なし
- テキスト: 薄グレー、イタリック、中央揃え
- 幅: 全幅ストレッチ (SystemMessage と同じ anchorMin/Max パターン)
- ラッパー: 生成しない

**実装方針**: SystemMessage のロジックをベースに、backgroundAlpha=0 + BubbleStylePreset のテキストスタイルを適用する分岐を追加。

---

## 4. アイコン配置

### 4.1 現行ロジック (実装済み)

- NPC: アイコンを `SetAsFirstSibling()` → バブルの左
- Player: アイコンを `SetAsLastSibling()` → バブルの右
- 判定: `CharacterProfile.m_IsPlayer` フラグで固定

### 4.2 新規: アイコン向きの明示指定

CharacterProfile に `iconSide` フィールドを追加し、キャラクター単位でアイコンの左右を決められるようにする。

| フィールド | 型 | 説明 |
|-----------|-----|------|
| iconSide | IconSide enum | `Left` / `Right` / `Auto` |

```csharp
public enum IconSide
{
    Auto,   // 既定: IsPlayer なら Right、それ以外は Left
    Left,   // 常に左
    Right   // 常に右
}
```

**動作**:
- `Auto`: 現行ロジックと同一 (IsPlayer で判定)
- `Left` / `Right`: IsPlayer に関係なくアイコン位置を固定
- バブルの左右寄せ自体は変更しない (寄せはキャラ/スレッド型で決まる)
- バブルの角や反転は行わない (工数・バグ削減)

**実装箇所**: ConfigureBubble() 行 534-542 の `isPlayer` 判定を `iconSide` で分岐に変更。

---

## 5. リッチテキスト表現

### 5.1 使用可能なタグ (TMP ネイティブ、実装済み)

| タグ | 記法例 | 状態 |
|------|--------|------|
| 太字 | `<b>テキスト</b>` | 使用可 |
| イタリック | `<i>テキスト</i>` | 使用可 |
| 下線 | `<u>テキスト</u>` | 使用可 |
| 打消し線 | `<s>テキスト</s>` | 使用可 |
| 色変更 | `<color=#FF0000>テキスト</color>` | 使用可 |
| サイズ変更 | `<size=30>テキスト</size>` | 使用可 |
| 行高さ | `<line-height=80%>テキスト</line-height>` | 内部使用 (名前行) |
| リンク | `<link="id">テキスト</link>` | 内部使用 (スレッドリンク) |

### 5.2 カスタムマークアップ (ChatTextParser、実装済み)

サブスレッド表示中のみ有効。

| マークアップ | 変換後 | 用途 |
|------------|--------|------|
| `[link:threadId:ラベル]` | `<link="thread:threadId"><color=#4A90D9><u>ラベル</u></color></link>` | スレッドリンク |
| `[artifact:recording:説明]` | `<color=#FF9800>▶</color> <i>説明</i>` | 録音成果物 |
| `[artifact:photo:説明]` | `<color=#4CAF50>▢</color> <i>説明</i>` | 写真成果物 |
| `[artifact:sample:説明]` | `<color=#9C27B0>◆</color> <i>説明</i>` | サンプル成果物 |

### 5.3 エフェクト系コマンド (実装済み)

| コマンド | 記法 | 効果 |
|---------|------|------|
| Glitch | `<<Glitch level>>` (1-5) | 画面全体にノイズ・色ずれ |
| StartWait | `<<StartWait 秒>>` | タイピングインジケーター + 待機 |
| SkipWait | `<<SkipWait>>` | 待機キャンセル |

### 5.4 スタイルの決定要因 (実装済み)

| 要因 | 制御対象 | 設定箇所 |
|------|---------|---------|
| キャラクター ID | バブル背景色 | CharacterProfile.m_ThemeColor |
| ChatUIConfig | テキスト色 (NPC/Player/System) | ChatUIConfig SO |
| スレッド型 | 背景色ティント (A=青/B=緑/C=橙/分岐=紫) | ChatController.GetThreadTypeColor() |
| Annotation カード | テキスト明度向上 + 中央配置 | ChatController.ConfigureBubble() |
| Yarn 本文 | TMP リッチテキストタグ | Yarn スクリプト直書き |

---

## 6. スレッドのフリック切替

### 6.1 現行 (実装済み)

- スレッド切替: サイドバー (左端スワイプで開閉) → 一覧からタップ
- サイドバー開閉: ハンバーガーボタン / 左端スワイプ / オーバーレイタップ
- フリック検出: 未実装

### 6.2 新規: 左右フリックでスレッド切替

チャット画面上で左右フリックすることで、隣接するスレッドに切り替える。

**動作**:
- 左フリック: 次のスレッドへ (サイドバーの下方向)
- 右フリック: 前のスレッドへ (サイドバーの上方向)
- メインスレッドも順序に含む (先頭がメイン)
- 完了スレッドはスキップ可 (設定で変更可能)
- スワイプ距離の閾値: 画面幅の 15% 以上
- 閾値未満はキャンセル (元に戻る)

**スレッド順序**:
1. メイン (null)
2. 宣言順 (DeclareThread の呼び出し順、SubthreadData のリスト順)
3. 潜在 (IsLatent=true) はスキップ

**視覚フィードバック**:
- フリック中: チャット領域が水平にスライド (指に追従)
- 閾値超え: 次スレッドのヘッダー色がプレビュー表示
- 確定時: SwitchToThread() を呼び出し、フェードイン (現行ロジック)

**競合回避**:
- 縦スクロール中は水平フリックを無視 (角度判定: 水平 +-30 度以内で発動)
- 選択肢表示中はフリック無効
- サイドバー開閉中はフリック無効

**実装方針**:
- ChatDialogueView または専用コンポーネントに `IBeginDragHandler`, `IDragHandler`, `IEndDragHandler` を実装
- ThreadSwitcherController.GetOrderedThreadIds() でフリック先を解決
- 閾値判定後に OnSelectThread() を呼び出し

---

## 7. サブクエストスレッド仕様 (統合)

SP-016, SP-017, SP-022, BL-003 に散在するスレッド関連仕様をここに統合する。

### 7.1 スレッド種別

| 種別 | enum | 接頭辞 | 色 | アイコン | 用途 |
|------|------|--------|-----|---------|------|
| メイン | (null) | - | キャラ色 | - | チャプター主会話 |
| A: 注釈 | Annotation | `annot_` | #4A90D9 青 | [A] | 用語・背景・制度の短い解説 |
| B: 追跡 | Tracking | `track_` | #4CAF50 緑 | [B] | リンク追跡 (Wiki型) |
| C: 偵察 | Scout | `scout_` | #FF9800 橙 | [C] | 外出・探索・成果物 |
| 分岐 | Branch | `branch_` | #9C27B0 紫 | [>] | ストーリー分岐 |

### 7.2 章あたり目標本数

| 種別 | 仮レンジ | 実測圧縮後 | 優先順位 |
|------|---------|-----------|---------|
| C型 (偵察) | 2-5 | 3-4 | 最優先 |
| A型 (注釈) | 2-6 | 2-3 | 第2 |
| B型 (追跡) | 0-2 | 0-1 | 第3 |
| 必須 vs 任意 | - | 必須 1-2、残り任意 | - |

### 7.3 SubthreadData フィールド (現行)

| フィールド | 型 | 説明 |
|-----------|-----|------|
| ThreadId | string | 識別子 (例: `scout_ch1_d3_route`) |
| DisplayName | string | UI 表示名 |
| Type | ThreadType | 種別 |
| IsLatent | bool | 潜在状態 (UI 非表示) |
| ManifestCondition | string | 顕在化条件 (Yarn 変数式) |
| AutoBeginBranch | bool | 顕在化時に自動 BeginBranch |
| IsCompleted | bool | 完了状態 |
| AcquiredTopicCount | int | 取得トピック数 |
| UnreadCount | int | 未読数 |
| ChatHistory | List | 会話履歴 |

### 7.4 新規: メタデータ拡張 (BL-003 統合)

SubthreadData に以下を追加する。

| フィールド | 型 | 説明 | Yarn 指定 |
|-----------|-----|------|----------|
| difficulty | int | 難易度 (星 1-5, 0=未設定) | `<<DeclareThreadMeta>>` のオプション |
| estimatedLength | string | 目安プレイ時間 ("短" / "中" / "長") | 同上 |
| description | string | サイドバー表示用の 1 行説明 | 同上 |
| requiredLevel | string | 推奨条件の表示テキスト (例: "Ch1 Day2 以降") | 同上 |
| rewardHint | string | 報酬のヒント (例: "断片を入手可能") | 同上 |

**Yarn コマンド例**:
```yarn
<<DeclareThreadTyped "scout_ch1_d3_route" "C" "電波ルート調査">>
<<SetThreadMeta "scout_ch1_d3_route" difficulty=3 length="中" desc="Pyramid が検出した電波源の位置を特定する">>
```

### 7.5 サイドバー表示

現行のサイドバーエントリに加え、メタデータがある場合は以下を表示:

```
[C] 電波ルート調査           ★★★☆☆
    Pyramid が検出した電波源の位置を特定する
    目安: 中 | 断片を入手可能
```

### 7.6 サブクエストサンプル (Ch1 実装済み)

| ID | 種別 | 表示名 | 起点 |
|----|------|--------|------|
| scout_ch1_network | C | ネットワーク偵察 | Day1 Hub |
| scout_ch1_day2_ping | C | Ping 応答調査 | Day2 |
| scout_ch1_d3_route | C | 電波ルート調査 | Day3 Hub |
| scout_ch1_d3_board | C | 掲示板スキャン | Day3 Hub |
| ch1_note_facility | A | 施設メモ | Day1 |
| annot_ch1_glossary | A | 用語集 | Day1 |
| annot_ch1_d3_compare | A | 比較分析 | Day3 |
| ch1_cond_analysis | B | 条件分析 | 断片取得後 |
| ch1_branch_analysis | 分岐 | 分析分岐 | 断片取得後 |

### 7.7 解放トリガーパターン (SP-017 統合)

| パターン | 方法 | 例 |
|---------|------|-----|
| A: Yarn 変数条件 | `<<DeclareThreadLatentCond "id" "type" "name" "$condition">>` | 断片取得後に自動顕在化 |
| B: ストーリー進行 | ChannelData SO の RequiredCompletedChannelID | 章クリア後に解放 |
| C: HC 閾値 | ChannelData SO の RequiredHalluciCoin | HC 蓄積で解放 |
| D: 複合 AND | Yarn 変数で AND 式を組む | `$has_x and $ch2_completed` |
| E: 手動 Manifest | `<<ManifestThread "id">>` | 特定ノード通過時に即顕在化 |

### 7.8 エンジンギャップ (未実装)

| 項目 | 優先度 | 備考 |
|------|--------|------|
| B型 Wiki 遷移 UX | P2 | テキストのみは既存対応、リンク遷移は未実装 |
| C型 成果物カード UI | P2 | マークアップ `[artifact:]` は実装済み、リッチカードは未実装 |
| 解放通知演出 | P2 | 通知バナー (3.5秒) は実装済み、演出拡張は未実装 |
| 必須/任意の UI 区別 | P1 | SubthreadData にフラグなし |
| 難易度星 UI | P1 | BL-003、本仕様 §7.4 で定義 |

---

## 8. 実装ファイルマップ

| ファイル | 役割 |
|---------|------|
| `Assets/Scripts/UI/ChatController.cs` | バブル生成・配置・スタイル適用・タイプライター |
| `Assets/Scripts/UI/ChatDialogueView.cs` | Yarn → ChatController の橋渡し・タイミング制御 |
| `Assets/Scripts/UI/ChatTextParser.cs` | マークアップ変換・未閉じタグ補完・名前除去 |
| `Assets/Scripts/UI/ThreadSwitcherController.cs` | サイドバー・スレッド切替・通知バナー |
| `Assets/Scripts/Data/ChatUIConfig.cs` | 色・サイズ・タイミング・レイアウトの SO 一元管理 |
| `Assets/Scripts/Data/CharacterProfile.cs` | キャラクター別テーマ色・表示モード・アイコン向き |
| `Assets/Scripts/Data/SubthreadData.cs` | スレッドデータ・メタデータ |
| `Assets/Scripts/Data/BubbleStylePreset.cs` | バブルスタイルプリセット (新設) |
| `Assets/Scripts/Effects/GlitchEffect.cs` | 画面グリッチエフェクト |
| `Assets/Scripts/Core/ScenarioManager.cs` | Yarn コマンド登録 (BubbleStyle/BubbleMargin/Narration 追加) |

---

## 9. 実装順序 (推奨)

| 順序 | スライス | 依存 |
|------|---------|------|
| 1 | BubbleStylePreset SO + `<<BubbleStyle>>` コマンド | なし |
| 2 | narration プリセット + `<<Narration>>` 短縮形 | 1 |
| 3 | `<<BubbleMargin>>` コマンド (% 指定) | なし |
| 4 | CharacterProfile.iconSide (アイコン左右) | なし |
| 5 | フリックスレッド切替 | なし |
| 6 | SubthreadData メタデータ拡張 + サイドバー表示 | なし |
| 7 | 追加プリセット (thought/shout/whisper/announcement) | 1 |
