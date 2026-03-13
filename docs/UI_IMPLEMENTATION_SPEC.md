# UI実装仕様書

## 概要
FoundPhone のチャットUIシステムの実装仕様。ENGINE_FEATURE_INVENTORY.md のセクション6/9と対になる詳細リファレンス。

**最終更新**: 2026-03-12
**対象コミット**: 70fe16b (バブルリファクタ)

---

## 1. チャットUIシステム

### 1.1 ChatController
**ファイル**: `Assets/Scripts/UI/ChatController.cs`
**責務**: メッセージバブルの生成・表示・スクロール管理。`IBeginDragHandler`, `IEndDragHandler` を実装。

#### 主要機能
- メッセージバブル表示（左/右配置、テキスト幅フィット）
- NPC 名前/本文の改行分離表示
- 角丸スプライト + 影の動的生成
- 選択肢表示/非表示
- スクロール吸着（タイプライター限定ピンニング）
- タイプライター効果
- TypingIndicator（入力中「...」表示）
- キャラクターアイコン表示（DisplayMode に応じた切替）
- 連続メッセージグループ化（バブルスタック）

### 1.2 メッセージバブル構造

```
ScrollRect.content
+-- TopSpacer (LayoutElement: flexibleHeight=1, メッセージ下詰め用)
+-- NpcRow (HorizontalLayoutGroup + ContentSizeFitter)
|   +-- CharacterIcon (条件付き: 非連続 + showCharacterIcon)
|   +-- MessageBubble (Image[9-slice角丸] + Shadow + TextMeshProUGUI)
|       +-- Text: "<size=N><b>名前</b></size>\n本文"
+-- PlayerRow (HorizontalLayoutGroup + ContentSizeFitter)
|   +-- MessageBubble (Image[9-slice角丸] + Shadow + TextMeshProUGUI)
|       +-- Text: "本文のみ"
+-- SystemMessageRow
    +-- SystemBubble (Image[9-slice角丸] + TextMeshProUGUI)
```

### 1.3 NPC 名前/本文分離

NPC バブルでは名前行と本文行を改行で分離する。

- **名前行**: `messageFontSize * 0.75` のボールドフォント
- **本文行**: `messageFontSize` の通常フォント
- **リッチテキスト**: `<size={N}><b>{displayName}</b></size>\n{text}` 形式
- **幅計算**: 名前行と本文行それぞれの `GetPreferredValues` を取得し、大きい方を採用
- **Player バブル**: 名前を表示しない
- **DisplayMode.IconOnly**: 名前を表示しない

### 1.4 バブル幅フィット + 高さ計算

バブル幅はテキスト内容にフィットさせ、不必要な空白を排除する。

1. `GetPreferredValues` で折り返しなしのテキスト自然幅を取得
2. `naturalTextWidth + textPadH(20px) + safetyMargin(4px)` を算出
3. `min(上記, maxBubbleWidth)` でクランプ
   - `maxBubbleWidth = min(Screen.width * bubbleMaxWidthPercent, bubbleMaxWidthPx)`
4. `LayoutElement.preferredWidth` に設定、`flexibleWidth = 0`
5. `ConfigureBubble` でラッパー配置
6. **`FinalizeBubbleSize`** でラッパー配置後に高さを再計算

#### FinalizeBubbleSize（70fe16b 新設）

```
ConfigureBubble -> ラッパー生成 -> 利用可能幅が変化
    |
FinalizeBubbleSize:
    Canvas.ForceUpdateCanvases()
    textComponent.ForceMeshUpdate()
    preferredHeight = max(minHeight, textHeight + bubbleTextPadding)
    Canvas.ForceUpdateCanvases()
```

ラッパー配置後に幅が変動する問題（HLG + アイコンによる幅変動）を解消する。

### 1.5 角丸スプライト + 影

#### 角丸スプライト (`GetOrCreateRoundedSprite`)
- `UIConfig.bubbleSprite` が null の場合、ランタイムで白色の角丸テクスチャを生成
- **半径**: `bubbleCornerRadius` (デフォルト 16px)
- **テクスチャサイズ**: `radius * 2 + 2` (9-slice の伸縮領域含む)
- **アンチエイリアス**: 境界付近のアルファ補間あり
- **9-slice border**: `Vector4(r, r, r, r)`
- **Image.Type**: `Sliced`
- キャッシュ: `m_GeneratedRoundedSprite` に保持（1回生成）
- `bubbleSprite` を Inspector で指定すれば自動生成をスキップ

#### 影 (`ApplyBubbleVisuals`)
- `bubbleShadowEnabled = true` の場合、`Shadow` コンポーネントを動的追加
- 既存 Shadow の存在チェック済み（プール再利用時の二重追加防止）
- **色**: `bubbleShadowColor` (デフォルト rgba(0,0,0,0.3))
- **距離**: `bubbleShadowDistance` (デフォルト (2, -2))
- SystemMessage には影を付けない設計

### 1.6 テーマカラー適用フロー

1回のバブル生成で2段階に分けて適用し、二重ラッパー生成を防止する:

1. **`ApplyThemeColor`**: ラッパー生成なし。バブル背景色 + テキスト色 + `ApplyBubbleVisuals`(角丸+影) のみ適用
2. **`ConfigureBubble`**: ラッパー (PlayerRow/NpcRow) を生成し、バブルを配置。再度 `ApplyBubbleVisuals` を呼ぶが、角丸スプライトはキャッシュ済み、Shadow は存在チェック済み

### 1.7 配置ロジック
- **Player**: 右寄せ（ラッパーの左パディングでマージン確保）
- **NPC**: 左寄せ（ラッパーの右パディングでマージン確保）
- **パディング**: `wrapperEdgePadding` (デフォルト 12px)

### 1.8 連続メッセージ判定
- **判定基準**: 直前のメッセージと同じ `charID`
- **スタック時の変更**: 上マージン縮小、キャラクターアイコン非表示
- **リセット条件**: 異なる話者、システムメッセージ、`ClearMessages()` 実行時

### 1.9 スクロール吸着

- **TopSpacer**: Content 先頭に `flexibleHeight` スペーサーを配置し、メッセージが少ない時に下詰め表示
- **LateUpdate ピンニング**: タイプライター効果中 (`m_IsTypewriterActive`) のみ毎フレーム最下部固定
- **ドラッグ制御**: `IBeginDragHandler` でドラッグ開始時に即座に吸着解除。`IEndDragHandler` で最下部付近なら自動吸着を再開
- **AutoScroll**: タイプライター完了後のワンショット調整

### 1.10 タイプライター効果
- **実装**: DOTween による `maxVisibleCharacters` のアニメーション
- **速度**: `m_TypewriterSpeed` (1文字あたりの秒数)
- **スクロール連携**: タイプライター開始で `m_IsTypewriterActive = true` → LateUpdate ピンニング開始

### 1.11 TypingIndicator
- **表示タイミング**: NPC メッセージ表示前 (ChatDialogueView から制御)
- **構造**: 専用 GameObject（プール管理外）。NPC 側（左寄せ）
- **ラッパー**: `m_TypingIndicatorWrapper` で親ラッパーを保持
- **ライフサイクル**: `ClearMessages()` 時も破棄せず、非表示にして再利用

---

## 2. Yarn Spinner 統合

### 2.1 ChatDialogueView
**ファイル**: `Assets/Scripts/UI/ChatDialogueView.cs`
**責務**: `DialoguePresenterBase` の実装。Yarn Spinner との統合。

#### Debug Overlay
- **デフォルト**: `m_ShowDebugOverlay = false` (70fe16b で true -> false に変更)
- **有効化**: Inspector で `m_ShowDebugOverlay = true` に変更
- **表示内容**: 現在のノード名・行ID・タグ

#### 早送りモード
- **トグル**: F11キー
- **効果**: TypingIndicator スキップ / タイプライター待機スキップ / 選択肢前遅延スキップ
- **最小遅延**: 30ms (完全スキップではない)
- **UI表示**: Debug Overlay に `[FF]` タグ

#### TypingIndicator 自動表示ロジック
- **条件**: NPC メッセージ かつ 早送り無効時
- **表示時間**: `m_LineDisplayDelay * 0.6` (デフォルト 0.3秒)

#### 選択肢表示タイミング
- 早送り無効時、200ms の遅延を挿入
- タイプライター効果完了を確実に待機するため

#### 実装済み Yarn コマンド

| コマンド | 機能 |
|---------|------|
| `<<Message charID text>>` | 指定キャラのメッセージ表示 |
| `テキスト #line:タグ名` | 矛盾タグ付きメッセージ（推奨。Yarn標準 `#line:` タグ方式） |
| `<<MessageTagged charID text lineTag>>` | 矛盾タグ付きメッセージ（非推奨。互換のため残存） |
| `<<SystemMessage text>>` | システムメッセージ（中央揃え） |
| `<<StartWait seconds>>` | 指定秒数待機 + タイピングインジケーター |
| `<<SkipWait>>` | 待機をスキップ |
| `<<UnlockTopic topicID>>` | トピック解放 |
| `<<Glitch level>>` | グリッチエフェクト (1-5) |
| `<<Image charID imageID>>` | 画像メッセージ表示 |

---

## 3. UIコンポーネント詳細

### 3.1 MessageBubble
**ファイル**: `Assets/Scripts/UI/MessageBubble.cs`
**インターフェース**: `IPointerDownHandler`, `IPointerUpHandler`, `IPointerClickHandler`
**プーリング**: `MessageBubblePool` で管理

#### 構成要素
- **Image**: 背景 (9-slice 角丸スプライト + テーマカラー乗算)
- **Shadow**: 影 (動的追加)
- **TextMeshProUGUI**: メッセージテキスト (リッチテキスト対応)
- **LayoutElement**: preferredWidth/preferredHeight で制御
- **MessageBubble**: 矛盾指摘用コンポーネント (LineTag 設定時のみ有効化)

#### 矛盾指摘連携
- `LineTag` 設定時: 長押し (0.5秒) で1つ目選択、タップで2つ目選択
- 選択時にスケールパルス演出
- `SyncOriginalColor()`: プール再利用時の色汚染防止

### 3.2 CharacterIcon
- **実装**: 実行時生成 (Discord 風円形アイコン)
- **マスク**: `CreateCircleSprite()` でランタイム生成
- **サイズ**: `UIConfig.characterIconSize` (デフォルト 40px)
- **間隔**: `UIConfig.iconBubbleSpacing` (デフォルト 8px)

### 3.3 ChatUIConfig (ScriptableObject)
**ファイル**: `Assets/Scripts/Data/ChatUIConfig.cs`
**アクセス**: `ChatUIConfig.Instance` (Resources.Load、未配置時はデフォルトインスタンス生成)

#### 全パラメータ一覧

| Header | パラメータ | デフォルト値 | 説明 |
|--------|-----------|-------------|------|
| **Message Bubble** | `messageFontSize` | 28 | 通常メッセージのフォントサイズ |
| | `bubbleMinHeight` | 60 | バブルの最小高さ |
| | `bubbleTextPadding` | 20 | バブル内テキストの上下パディング |
| | `bubbleInitialHeight` | 72 | バブルの初期高さ (RectTransform) |
| | `playerTextColor` | white | プレイヤーのテキスト色 |
| | `npcTextColor` | (0.9, 0.9, 0.9) | NPC のテキスト色 |
| | `bubbleAnimationDuration` | 0.4 | バブル出現アニメーション時間 (秒) |
| | `bubbleMaxWidthPercent` | 0.7 | バブル幅上限 (画面幅割合) |
| | `bubbleMaxWidthPx` | 600 | バブル幅上限 (px 絶対値) |
| | `bubbleSprite` | null | 9-slice 角丸スプライト (null=自動生成) |
| | `bubbleCornerRadius` | 16 | 自動生成角丸の半径 (px) |
| **Bubble Shadow** | `bubbleShadowEnabled` | true | バブルに影を付けるか |
| | `bubbleShadowColor` | rgba(0,0,0,0.3) | 影の色 |
| | `bubbleShadowDistance` | (2, -2) | 影のオフセット (px) |
| **Layout** | `wrapperEdgePadding` | 12 | ラッパーの端パディング (px) |
| | `wrapperVerticalPadding` | 4 | ラッパーの上下パディング (px) |
| | `minLayoutSpacing` | 10 | VLG 最小スペーシング |
| **Character Icon** | `showCharacterIcon` | true | アイコン表示フラグ |
| | `characterIconSize` | 40 | アイコンサイズ (px) |
| | `iconBubbleSpacing` | 8 | アイコンとバブルの間隔 (px) |
| **System Message** | `systemMessageFontSize` | 16 | フォントサイズ |
| | `systemMessageTextColor` | (0.75, 0.75, 0.8) | テキスト色 |
| | `systemMessageBgColor` | rgba(0.5, 0.5, 0.5, 0.3) | 背景色 |
| | `systemMessageMinHeight` | 40 | 最小高さ |
| **Choice** | `choiceButtonColor` | rgba(0.22, 0.25, 0.35, 0.85) | 選択肢ボタン背景色 |
| | `choiceButtonHighlightColor` | rgba(0.3, 0.35, 0.5, 0.95) | ハイライト色 |
| | `choiceButtonPressedColor` | rgba(0.2, 0.28, 0.45, 1.0) | 押下色 |
| | `choiceTextColor` | (0.82, 0.85, 0.95) | 選択肢テキスト色 |
| | `choiceFontSizeMin` | 18 | 最小フォントサイズ |
| | `choiceFontSizeMax` | 26 | 最大フォントサイズ |
| | `choiceButtonMinHeight` | 42 | ボタン最小高さ |
| | `choiceButtonPreferredHeight` | 50 | ボタン推奨高さ |
| | `choiceSpacing` | 6 | コンテナスペーシング |
| | `choicePaddingHorizontal` | 20 | 左右パディング |
| **Typing Indicator** | `typingIndicatorColor` | (0.3, 0.3, 0.35, 0.9) | 背景色 |
| | `typingIndicatorTextColor` | (0.7, 0.7, 0.7) | テキスト色 |
| | `typingIndicatorFontSize` | 18 | フォントサイズ |
| **Scroll** | `autoScrollDelay` | 0.1 | 自動スクロール起動遅延 (秒) |
| **Image** | `imageFadeInDuration` | 0.6 | 画像フェードイン時間 (秒) |
| **Input Field** | `showInputField` | false | 入力欄表示 (false=選択肢のみ) |

---

## 4. パフォーマンス

### 4.1 オブジェクトプーリング
- **MessageBubblePool**: メッセージバブルの再利用
- **TypingIndicator**: プール外（専用インスタンス）
- **ChoiceContainer**: シーン常駐（子要素のみ破棄/再生成）

### 4.2 GC Alloc 削減
- `m_ChatHistory`: List 再利用
- DOTween: `SetUpdate(true)` でタイムスケール独立
- 角丸スプライト: `m_GeneratedRoundedSprite` で1回生成キャッシュ
- Layout 更新: `Canvas.ForceUpdateCanvases()` で即座に反映

---

## 5. 既知の制限事項

| 制限 | 説明 |
|------|------|
| SystemMessage ルーティング | 現在は全てチャットバブルとして表示。ステータスバー/トースト分岐は TODO (ChatController.cs:1056) |
| 画面リサイズ | `Screen.width` は初回計算時の値。ウィンドウリサイズ後の再計算が走らない |
| セーブ復元時の名前表示 | 復元パスでも `finalText` に名前を埋め込むため、保存データに名前が含まれていると重複の可能性あり |
| チャンネルレジューム | チャンネル選択は常に `StartNodeName` から再開始。途中復帰は未対応 |
| 文字化けコメント | ScenarioManager.cs / DeductionBoard.cs に約70行 (D分類で凍結中) |

---

## 付録: トラブルシューティング

### TypingIndicator が表示されない
- **原因**: 早送りモード (F11) が有効 / プレイヤーメッセージ
- **解決策**: F11 を押して早送り無効化

### スクロールが吸着しない
- **原因**: ユーザーがドラッグ中 / タイプライター非活性
- **解決策**: 最下部付近までスクロール

### バブル高さ不足
- **原因**: `FinalizeBubbleSize` の `Canvas.ForceUpdateCanvases()` タイミング
- **対策**: 70fe16b で `FinalizeBubbleSize` 新設により構造的に解消
