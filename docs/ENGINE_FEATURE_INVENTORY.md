# Engine Feature Inventory

**最終更新**: 2026-03-22
**エンジン**: Unity 6.3 LTS (6000.3.6f1) + Yarn Spinner 3.1.3

このドキュメントは、シナリオ執筆者が「今のエンジンで何ができるか」を把握するためのリファレンスです。

---

## 1. 利用可能な Yarn コマンド

### メッセージ系

| コマンド | 構文 | 説明 |
| -------- | ---- | ---- |
| Message | `<<Message "charID" "テキスト">>` | 指定キャラのメッセージバブルを表示 |
| (矛盾タグ) | `テキスト #line:タグ名` | Yarn 標準の `#line:` タグで矛盾指摘システムの識別子を付与。`ChatDialogueView` が `TextID` として自動取得し `AddMessage` の lineTag に渡す。例: `本プログラムの対象地域は... #line:ch1_region_identity_src` |
| SystemMessage | `<<SystemMessage "テキスト">>` | 中央寄せのシステム通知を表示 |
| Image | `<<Image "charID" "imageID">>` | 画像メッセージを表示（`Resources/Images/` 内） |

### 演出系

| コマンド | 構文 | 説明 |
| -------- | ---- | ---- |
| StartWait | `<<StartWait 秒数>>` | 指定秒数の待機 + タイピングインジケーター表示 |
| SkipWait | `<<SkipWait>>` | 待機をキャンセル |
| Typing | `<<Typing true\|false>>` | タイピングインジケーターの手動表示/非表示。StartWait より細かいタイミング制御が必要な場合に使用 |
| Glitch | `<<Glitch レベル>>` | グリッチ演出（1-5段階） |

### ゲームシステム系

| コマンド | 構文 | 説明 |
| -------- | ---- | ---- |
| UnlockTopic | `<<UnlockTopic "topicID">>` | トピックカードを推理ボードに追加 |
| EndDay | `<<EndDay 日数>>` | Day 終了処理。「--- N日目 終了 ---」システムメッセージ表示 + Day進捗記録 + オートセーブ。マルチDayチャプターでは最終Day完了時のみチャンネルを完了にする |
| DeclareThread | `<<DeclareThread "threadId" "displayName">>` | サブスレッドを宣言（type=Annotation）。メインチャットに通知表示 + サイドバーにエントリ追加 |
| DeclareThreadTyped | `<<DeclareThreadTyped "threadId" "type" "displayName">>` | 型指定でサブスレッドを宣言。type: "A"(注釈)/"B"(追跡)/"C"(偵察)/"branch"(分岐)。サイドバーに型グループ分類+アイコン・色表示 |
| AddThreadMessage | `<<AddThreadMessage "threadId" "text">>` | サブスレッドにシステムメッセージを追加（メイン画面には非表示） |
| AddThreadChat | `<<AddThreadChat "threadId" "charID" "text">>` | サブスレッドにキャラクター付きメッセージを追加（バブル表示） |
| BeginBranch | `<<BeginBranch "branchId" "displayName">>` | 分岐スレッドを宣言し自動切替。以降のメッセージは分岐に流れる |
| EndBranch | `<<EndBranch true\|false>>` | 分岐を終了しメインに自動復帰。反映メッセージを投入 (優先順位: SetBranchReflection指定 > TransferFlags自動生成 > なし) |
| EndBranch (select) | `<<EndBranch true "select">>` | 知識転送選択UIを表示。プレイヤーが「どの知識を持ち帰るか」を選択後にメイン復帰 |
| SetBranchReflection | `<<SetBranchReflection "text">>` | EndBranch 時にメインへ投入する反映メッセージを設定。未設定時は分岐内UnlockTopicのトピック名から自動生成 |
| AddHalluciCoin | `<<AddHalluciCoin amount>>` | HalluciCoin を静かに加算 (通知なし、ダッシュボードバッジパルスで検知) |
| DeclareThreadLatent | `<<DeclareThreadLatent "id" "type" "name">>` | スレッドを潜在登録 (UIに出さない)。ManifestThread で顕在化 |
| DeclareThreadLatentCond | `<<DeclareThreadLatentCond "id" "type" "name" "$condition">>` | 条件付き潜在登録。Yarn変数変更時に条件を自動評価し、trueで自動顕在化。Branch型は自動BeginBranch |
| ManifestThread | `<<ManifestThread "id">>` | 潜在スレッドを顕在化。通知メッセージ+サイドバー追加 |
| CompleteThread | `<<CompleteThread "id">>` | スレッドを完了状態にする。サイドバーでグレーアウト+チェックマーク |
| DiscoverFragment | `<<DiscoverFragment "topicId" "threadId" "message">>` | 断片発見の一括実行: UnlockTopic + SystemMessage("断片「{title}」を記録") + ManifestThread + AddThreadMessage |
| AddFragmentNote | `<<AddFragmentNote "threadId" "message">>` | スレッドへの断片関連メモ追加。AddThreadMessage のセマンティックエイリアス |

### Yarn 標準機能

| 機能 | 構文 | 説明 |
| ---- | ---- | ---- |
| 変数宣言 | `<<declare $変数名 = 初期値>>` | bool/string/float 変数を宣言 |
| 変数設定 | `<<set $変数名 to 値>>` | 変数の値を変更 |
| 条件分岐 | `<<if $条件>> ... <<endif>>` | 変数による条件分岐 |
| 選択肢 | `-> 選択肢テキスト` | プレイヤーの選択肢を表示 |
| ジャンプ | `<<jump ノード名>>` | 別ノードへ遷移 |

---

## 2. キャラクター管理

### CharacterProfile（ScriptableObject）

各キャラクターは以下のプロパティを持つ:

- **CharacterID**: Yarn スクリプト内で参照する一意のID
- **DisplayName**: UI に表示される名前
- **Icon**: アバター画像（Sprite）
- **ThemeColor**: バブルの背景色（白の9-Slice Spriteに乗算して着色）
- **IsPlayer**: `true` の場合、メッセージが右寄せ表示
- **DisplayMode**: バブル横の表示モード
  - `NameOnly` — テキスト内に名前表示、アイコンなし（デフォルト）
  - `IconOnly` — バブル横にアイコン表示、名前省略
  - `IconAndName` — アイコン+名前の両方表示

### Yarn での使い方

```yarn
<<set $speaker to "marco">>
こんにちは。 ← marco の色・名前で左寄せ表示

<<set $speaker to "player">>
やあ。 ← プレイヤーの色・名前で右寄せ表示
```

または直接 Message コマンドで:

```yarn
<<Message "marco" "こんにちは。">>
<<Message "player" "やあ。">>
```

### 現在の登録済みキャラID

`Resources/Characters/` フォルダ内の ScriptableObject で管理。
新キャラクターの追加: Unity メニュー `Create > Project FoundPhone > Character Profile`

---

## 3. 分岐と選択肢

### 基本的な分岐

選択肢のテキストはプレイヤーメッセージとして**自動的にチャットに追加**される（コード側の `RunOptionsAsync` で処理）。Yarn スクリプト側でプレイヤーのセリフをエコーする必要はない。

```yarn
title: ExampleNode
---
<<Message "npc" "どうする？">>

-> 選択肢A
    <<jump NodeA>>
-> 選択肢B
    <<jump NodeB>>
===

title: NodeA
---
<<set $speaker to "pyramid">>
応答テキスト。  ← いきなりNPCのセリフでOK（プレイヤーの「選択肢A」は自動表示済み）
===
```

### インライン選択肢

ジャンプなしの選択肢も同様に自動表示される:

```yarn
<<set $speaker to "player">>
-> この端末は不調か？
<<set $speaker to "pyramid">>
<<StartWait 0.8>>
通信環境による影響と思われます。
```

### 条件付き選択肢

```yarn
-> いつでも表示される選択肢
    <<jump Always>>
-> トピック入手後のみ表示 <<if $has_topic_signal_01>>
    <<jump Conditional>>
```

### 変数による分岐

```yarn
<<if $trust_level >= 3>>
    <<Message "marco" "君を信頼しているよ。">>
<<elseif $trust_level >= 1>>
    <<Message "marco" "まだよくわからないな。">>
<<else>>
    <<Message "marco" "...。">>
<<endif>>
```

---

## 4. セーブシステム

### 保存される情報

- 最後に訪れた Yarn ノード名
- 全 Yarn 変数（$has_topic_*, $speaker 等）
- 解放済みトピック一覧
- **チャット履歴**（メッセージバブルの内容: Normal/System/Image 全種別）
- セーブ日時
- 完了済みチャンネルID（ダッシュボード進行状態）

### オートセーブ（EN-005 実装済み）

- 専用スロット (slot 99) に自動保存。手動セーブ (slot 0-2) とは独立
- トリガー: ノード遷移時 / 選択肢表示前 / EndDay 時 (forceSave)
- クールダウン: 30秒（EndDay は無視して強制保存）
- UI: 画面右上に「Auto Saved」インジケーター（1秒表示、フェードアウト）
- ロード画面: ロードモード時にオートセーブスロットを先頭表示
- 詳細: `AUTOSAVE_DESIGN.md`

### 保存されない情報

- スクロール位置
- 個別メッセージの既読状態

### 注意点

- セーブ復元時は**ノードの先頭から再開**（行単位の復元は不可）
- チャット履歴は復元時にバブルとして再生成される（アニメーション付き）
- ノードを細かく分割すれば、より精密な復元ポイントを作れる
- 旧セーブデータとの後方互換性あり（ChatHistory フィールドが無い場合は空リスト扱い）

---

## 5. 演出機能

### Glitch（グリッチ効果）

| レベル | 効果 |
| ------ | ---- |
| 1 | 薄いノイズオーバーレイ（20%透過） |
| 2 | ノイズ + 軽い色収差 |
| 3 | 強い色収差 + 画面揺れ + 動的ノイズ |
| 4 | レベル3の強化版 |
| 5 | 最大強度（データモッシュ的） |

使い方:

```yarn
<<Glitch 1>>
<<StartWait 0.5>>
<<Message "system" "接続が不安定です">>
<<Glitch 3>>
```

### 待機演出

```yarn
<<StartWait 2>>       ← 2秒間タイピングインジケーター表示
<<Message "npc" "...">>  ← 待機後にメッセージ
```

### 早送りモード（F11）

- **F11キー**で早送りモードをトグル
- 有効時: タイピングインジケーター、タイプライター効果、StartWait の待機を全てスキップ（30ms最小遅延）
- デバッグオーバーレイに `[FF]` タグが表示される
- 選択肢は早送り中も通常通りプレイヤーの操作を待つ

### Debug Hub（F12）

- **F12キー**でデバッグハブをトグル
- 全 Yarn ノードの一覧表示（チャプター別グループ、**ストーリー順ソート**）
- ストーリー順は Yarn ファイル内の `title:` 行出現順で自動決定
- ノードクリック → 前ダイアログ停止 → メッセージクリア → 選択ノードから再生
- デバッグオーバーレイ（折りたたみ式）: 現在のノード名・行ID・タグ表示

---

## 6. チャットバブル表示設定

> 詳細な実装仕様は `docs/UI_IMPLEMENTATION_SPEC.md` を参照。

### ChatUIConfig（ScriptableObject）

`Resources/ChatUIConfig` で一元管理する主要パラメータ:

| パラメータ | デフォルト値 | 説明 |
| ---------- | ------------ | ---- |
| `bubbleMaxWidthPercent` | 0.7 | 画面幅に対するバブル幅上限（割合） |
| `bubbleMaxWidthPx` | 600 | バブル幅の絶対上限（px）。percentと小さい方を採用 |
| `bubbleSprite` | null | 9-slice角丸スプライト。null時はランタイム自動生成 |
| `bubbleCornerRadius` | 16 | 自動生成角丸の半径（px）。bubbleSprite指定時は無視 |
| `bubbleShadowEnabled` | true | バブルに影を付けるか |
| `showInputField` | false | テキスト入力欄の表示/非表示 |

その他 40 パラメータ（フォントサイズ、色、パディング等）の全一覧は `UI_IMPLEMENTATION_SPEC.md` セクション3.3 を参照。

### NPC 名前/本文分離（70fe16b）

NPC バブルでは名前行と本文行を改行で分離して表示する:

- **名前行**: `messageFontSize * 0.75` のボールドフォント（リッチテキスト `<size=N><b>名前</b></size>`）
- **本文行**: `messageFontSize` の通常フォント
- Player バブルには名前を表示しない
- `DisplayMode.IconOnly` のキャラクターにも名前を表示しない
- バブル幅は名前行と本文行それぞれの `GetPreferredValues` の大きい方を採用

### 角丸スプライト + 影（70fe16b）

- `bubbleSprite` が null の場合、`GetOrCreateRoundedSprite()` で白色の角丸テクスチャをランタイム生成（アンチエイリアス付き、9-slice対応）
- `bubbleShadowEnabled = true` の場合、`Shadow` コンポーネントを動的追加（既存チェック済み）
- 選択肢ボタンにも角丸スプライトを適用
- SystemMessage には影を付けない

### バブル幅フィット + FinalizeBubbleSize（70fe16b）

- `GetPreferredValues` でテキスト自然幅を取得 → `maxBubbleWidth` でクランプ → `LayoutElement.preferredWidth` に設定
- **`FinalizeBubbleSize`**: `ConfigureBubble` でラッパー配置後に `Canvas.ForceUpdateCanvases()` + `ForceMeshUpdate()` で高さを再計算。HLG+アイコンによる幅変動で高さが不整合になる問題を解消

### タイピングインジケーター

NPC メッセージの前に表示される入力中表示。3つのドットがスケールアニメーション（DOTween Yoyo）で脈動する。メッセージバブルプールとは独立した専用オブジェクト。

### 選択肢UI

選択肢はメッセージ流に溶け込む控えめなデザイン（暗めグレー青、小型、テキスト左揃え）。ScrollRect 内にインライン配置される。選択後、選択テキストがプレイヤーメッセージとして確定表示され、未選択の選択肢は CanvasGroup DOFade でフェードアウトする。色・サイズ・パディングは全て ChatUIConfig SO で調整可能。選択肢の色は `UIConfig.choiceButtonColor` を使用（プレイヤーのThemeColorとは独立）。

### スクロール制御

- **TopSpacer**: Content先頭にflexibleHeightスペーサーを配置し、メッセージが少ない時に下詰め表示
- **LateUpdate ピンニング**: タイプライター効果中（`m_IsTypewriterActive`）のみ毎フレーム最下部固定。効果終了後はワンショットAutoScrollのみ
- **IBeginDragHandler / IEndDragHandler**: ドラッグ開始で即座に吸着解除。ドラッグ終了時に最下部付近なら自動吸着を再開
- **ApplyThemeColor**: ConfigureBubbleからテーマカラー適用のみを抽出したヘルパー。CreateMessageBubble内ではラッパー生成前にこれを1回、テキスト高さ算出後にConfigureBubbleを1回だけ呼ぶ（二重ラッパー生成を防止）

### Debug Overlay（70fe16b）

- `ChatDialogueView.m_ShowDebugOverlay` のデフォルト値を `true` → `false` に変更
- Inspector で `true` に設定すれば個別有効化可能

---

## 7. ローカライズ対応状況

### 現状

- **Unity Localization パッケージ**: 未インストール
- **Yarn Spinner ローカライズ機能**: 構造的に対応可能だが未設定
- **現在の言語**: 日本語のみ（.yarn ファイルに直書き）

### Yarn Spinner でのローカライズ方法（将来対応）

Yarn Spinner 3.x は以下のローカライズをサポート:

1. **`#line:` タグ方式**: 各行にユニークIDを付与し、CSV で翻訳管理

   ```yarn
   こんにちは。 #line:greeting_001
   ```

2. **String Table 方式**: Unity Localization パッケージと連携

3. **ファイル分割方式**: 言語ごとに別 .yarn ファイルを用意

### 導入に必要な手順（概算）

1. `com.unity.localization` パッケージのインストール
2. Yarn プロジェクト設定でローカライズを有効化
3. 既存 .yarn ファイルに `#line:` タグを追加
4. CSV エクスポート → 翻訳 → インポート
5. 言語切替 UI の実装

---

## 8. 矛盾指摘システム（Phase 2 実装済み）

### 実装済み機能

| 機能 | 説明 | ファイル |
| ---- | ---- | -------- |
| 長押し選択 | バブル0.5秒長押しで1つ目選択、タップで2つ目選択。選択時にスケールパルス演出 | `MessageBubble.cs` |
| 矛盾判定 | ContradictionDatabase の順不同マッチング、クールダウン10秒 | `ContradictionManager.cs` |
| 成功演出 | 緑フラッシュ + スケールパルス + 接続線 + 通知パネル | `ContradictionFeedbackController.cs` |
| 失敗演出(不一致) | 赤フラッシュ + 回転シェイク + エラーバナー + クールダウン | 同上 |
| 失敗演出(既発見) | 黄フラッシュ + 「既に発見済み」バナー | 同上 |
| ヒントバナー | 1つ目選択時に「2つ目をタップ」表示 | 同上 |
| 接続線 | 2バブル間の直線（Image回転方式、成功/失敗で色変化→フェードアウト） | 同上 |
| HalluciCoin | 矛盾発見時に報酬加算、セーブ/ロード対応済み | `ContradictionManager.cs` |
| ~~トピック自動解放~~ | ~~矛盾発見時に ContradictionPair.UnlockTopic を DeductionBoard に追加~~ **廃止済み**: 矛盾報酬は HalluciCoin のみに変更（2026-03-07決定） | `DeductionBoard.cs` |
| データ | 7ペア（Ch1x4, Ch2x3）、全報酬10コイン、難易度1 | `Resources/Contradictions/` |
| タグ方式 | Yarn 標準 `#line:` タグを使用（推奨）。`ChatDialogueView.TextID` → `AddMessage` lineTag → `MessageBubble.LineTag` に自動伝播。`<<MessageTagged>>` は非推奨（互換のため残存） | `ChatDialogueView.cs` |

### セットアップ要件

ContentAuthoring シーンの Canvas 直下に `ContradictionFeedbackController` を配置し、
`ChatController` への参照を Inspector でアサインすること。

---

## 9. ダッシュボード（MVP 実装済み）

### 実装済み機能

| 機能 | 説明 | ファイル |
| ---- | ---- | -------- |
| チャンネル一覧 | ChannelData SO ベースのカード表示（Locked/Available/InProgress/Completed） | `DashboardController.cs` |
| チャンネル遷移 | カードクリック → チャット開始、Back/ESC → ダッシュボード復帰 | 同上 |
| HalluciCoin 表示 | 右上に "HC: N" 表示（ContradictionManager.HalluciCoin 参照） | 同上 |
| ChannelData | ScriptableObject: ID, DisplayName, Description, StartNodeName, ChapterNumber, RequiredCompletedChannelID, TotalDays, DayStartNodeNames, EnableHints, MaxHintDifficulty | `ChannelData.cs` |
| 進行状態管理 | SaveData.CompletedChannelIDs でアンロック条件判定。ChannelDayProgress でマルチDay進捗管理 | `SaveData.cs` |
| Day Resume | マルチDayチャプターの途中再開。ChannelDayProgress から次のDay開始ノードを決定 | `DashboardController.cs` |
| チャプター別ヒント制御 | チャンネル選択時に ContradictionManager へ chapter/hint policy を反映（EnableHints, MaxHintDifficulty） | `DashboardController.cs`, `ContradictionManager.cs` |
| Editor ツール | チャンネルデータ自動生成 + シーンセットアップ | `ChannelDataCreator.cs`, `DashboardSceneSetup.cs` |

### セットアップ要件

1. `Tools > FoundPhone > Create Default Channel Data` でアセット生成
2. `Tools > FoundPhone > Add Dashboard to Scene` で DashboardController 追加
3. ScenarioManager の `m_AutoStartYarn` を false に変更
4. DebugHubController の `m_ShowOnStart` を false に変更

### 既定アセット値（Repository 現在値）

#### `Assets/Resources/Channels/ch1.asset`

- ChannelID: `ch1`
- DisplayName: `Ch.1 -- Terminal`
- StartNodeName: `Ch1_Day1_Opening`
- ChapterNumber: `1`
- TotalDays: `2`
- DayStartNodeNames: `[Ch1_Day1_Opening, Ch1_Day2_Opening]`
- RequiredCompletedChannelID: `(empty)`
- EnableHints: `false`
- MaxHintDifficulty: `1`

#### `Assets/Resources/Channels/ch2.asset`

- ChannelID: `ch2`
- DisplayName: `Ch.2 -- Location Confusion`
- StartNodeName: `Ch2_Opening`
- ChapterNumber: `2`
- RequiredCompletedChannelID: `ch1`
- EnableHints: `true`
- MaxHintDifficulty: `1`

### インベントリUI（実装済み）

| 機能 | 説明 | ファイル |
| ---- | ---- | -------- |
| タブ切替 | ダッシュボードに Channels / Inventory の2タブ | `DashboardController.cs` |
| 3サブタブ | Fragments / Records / Topics の切替 | `InventoryTabController.cs` |
| 断片表示 | `fragment_` プレフィックスで DeductionBoard.UnlockedTopics をフィルタ、チャプターバッジ付き | 同上 |
| トピック表示 | `topic_` プレフィックス + `T_*` / `debug_*` をトピック扱いで表示 | 同上 |
| Records | プレースホルダー（将来 `record_` プレフィックスで拡張予定） | 同上 |
| オーバーレイ表示 | チャット画面からINVボタンでダッシュボードをオーバーレイ表示 | `ChatController.cs`, `DashboardController.cs` |
| DebugHub連携 | Fragment Archive → ShowInventory() に統合 | `DebugHubController.cs` |

### TopicData プレフィックス分類規約

TopicData の `TopicID` プレフィックスでインベントリの表示カテゴリを決定する。
分類ロジックは `InventoryTabController.GetCategory()` に集約。

| プレフィックス | カテゴリ | 用途 | 命名例 |
| -------------- | -------- | ---- | ------ |
| `fragment_` | Fragment | 不可索引物の断片テキスト。章番号+連番 | `fragment_ch1_01`, `fragment_ch2_03` |
| `record_` | Record | 将来拡張用（ログ・記録系コンテンツ） | `record_ch1_call_log` |
| 上記以外 | Topic | 一般トピック。調査の手がかり・情報 | `topic_found_phone`, `T_MissingPerson` |

**補足:**

- 判定は大文字小文字を区別しない（`OrdinalIgnoreCase`）
- `T_*` や `debug_*` は明示的プレフィックスを持たないため Topic に分類される
- `debug_*` はデバッグ用アセット。本番ビルドでは除外を推奨
- 新カテゴリ追加時は `InventoryItemCategory` enum と `GetCategory()` を拡張する

### インベントリ セットアップ要件

- Fragments / Topics の表示には `DeductionBoard` コンポーネントがシーン内に存在する必要がある
- 不在時は `DeductionBoard: Instance not found in scene.` Warning が出るが、空リスト表示で動作は継続する
- ContentAuthoring シーンには現在 DeductionBoard が未配置（2026-03-11 確認）

### 既知の制限

| 制限 | 説明 | 影響 |
| ---- | ---- | ---- |
| ~~チャンネルレジューム~~ | 実装済み。マルチDayチャプターは ChannelDayProgress + DayStartNodeNames で途中再開可能 | — |
| ESC 停止時の非同期競合 | StartWait 等の Yarn コマンド実行中に ESC → `StopScenario()` で `DialogueException` 発生 | `Cannot continue running dialogue. No node has been selected.` エラー。動作は継続するがログ汚染 |
| 選択肢クリック競合 | Dialogue が選択待ち状態を離れた後に選択 UI コールバックが発火すると `DialogueException` | `SetSelectedOption was called, but Dialogue wasn't waiting for a selection.` エラー |
| DeductionBoard 依存 | InventoryTab は `DeductionBoard.Instance` を参照。シーンに不在だと Warning スパム | 動作は継続するがログノイズが多い |

### 未実装（将来拡張）

- 検索バー装飾

---

## 10a. サブスレッドUI（Step 1-4 全実装済み）

メイン⇔複数サブスレッドの切替機能。種別差異レンダリング + 出現通知対応。

### アーキテクチャ

- **データモデル**: `SubthreadData.cs` — ThreadId, DisplayName, Type (ThreadType enum), IsLatent, ManifestCondition, AutoBeginBranch, IsCompleted, AcquiredTopicCount, UnreadCount, ChatHistory
- **ThreadType**: Annotation(A)/Tracking(B)/Scout(C)/Branch — `DeclareThreadTyped` で指定、`DeclareThread` はAnnotationにフォールバック
- **Yarnコマンド**: `DeclareThread` / `DeclareThreadTyped` / `AddThreadMessage` / `AddThreadChat` (ScenarioManager登録)
- **切替方式**: ChatControllerのデータスワップ (ClearMessages + RestoreChatHistory)
- **UI**: `ThreadSwitcherController.cs` — 左スライドインサイドバー (ハンバーガーボタン≡ + 未読合計バッジ、半透明オーバーレイ、ThreadTypeグループヘッダー、Main常時先頭、DOTweenスライドアニメ0.25s、ScrollRect内蔵)
- **Save/Load**: SaveData.Subthreads + ActiveThreadId, SaveManager対応済み（ThreadType含む、ロード時 `SetActiveThreadType` 呼出）
- **スクロール位置**: スレッド別に保存・復元
- **未読管理**: AddThreadMessage時にUnreadCountインクリメント、スレッド切替時にリセット
- **通知バナー**: 非アクティブスレッドへのメッセージ追加時に画面上部にトースト通知。型色/アイコン付き、DOTweenフェード (0.25s in → 3.5s表示 → 0.4s out)、クリックでスレッド切替

### 種別差異レンダリング（Step 3 Phase 3a）

`ChatController.m_ActiveThreadType` に基づきバブル外観を差別化:

- **A型 (注釈)**: 情報カード表示。中央配置 (`AnnotationRow`)、キャラアイコン/名前省略、型色ベースの暗い背景、型色テキスト
- **B/C/分岐**: 通常バブル + 型色ティント (キャラ色に12%混合)
- **SystemMessage**: サブスレッド内で型色10%ティント

### 出現通知（Step 3 Phase 3b）

- **システムメッセージ**: `DeclareThread` 時にメインチャットへ型アイコン+型色リッチテキスト付き通知 (例: `<color=#4A90D9>[A]</color> 新しいスレッド「覚書」が利用可能です`)
- **ハンバーガーパルス**: 新スレッド宣言時にボタン背景が型色で2回パルス (DOTween)

### 制限事項

- サブスレッド内でのYarnノード実行は未対応
- サブスレッド内矛盾指摘は未対応

### 使用例

```yarn
<<DeclareThreadTyped "note_1" "A" "Pyramidの覚書">>
<<AddThreadMessage "note_1" "覚書1: タイムスタンプが不一致">>
<<AddThreadMessage "note_1" "覚書2: 要照合">>

<<DeclareThreadTyped "log_1" "B" "Marco調査ログ">>
<<AddThreadChat "log_1" "marco" "第4管理区域の名称に矛盾がある。">>
```

### 検証モック

- `Assets/Resources/Yarn/active/SubthreadTest.yarn` — F12 Debug Hub から起動
- `ETK_ThreadType` / `ETK_ThreadParallel` — EngineTestKit 内テストノード
- `ch_etk.asset` — ダッシュボードから ETK_Menu を起動可能

---

## 10b. 分岐システム（Phase 1-4 全実装済み）

ストーリー分岐スレッドの宣言・自動切替・知識転送の仕組み。

### アーキテクチャ

- **状態管理**: `BranchThreadState.cs` — IsInBranch, BranchThreadId, TransferFlags (分岐内UnlockTopic自動追跡), TransferredFlags, HiddenFlags, SelectionApplied, ReflectionMessage
- **UI**: `TransferSelectionUI.cs` — EndBranch "select" 時の知識転送選択パネル。プレイヤーが「どの知識を持ち帰るか」を選択
- **Yarnコマンド**: BeginBranch / EndBranch / EndBranch "select" / SetBranchReflection

### 実装済み機能

| 機能 | 説明 | ファイル |
| ---- | ---- | -------- |
| 分岐開始 | `BeginBranch` でBranch型スレッド宣言+自動切替。以降のメッセージは分岐に流れる | `ScenarioManager.cs` |
| 分岐終了 | `EndBranch true/false` でメイン復帰。反映メッセージ投入 | 同上 |
| 知識転送選択 | `EndBranch true "select"` で選択UI表示。プレイヤーが持ち帰る知識を選択 | `TransferSelectionUI.cs` |
| TransferFlags自動追跡 | 分岐内の `UnlockTopic` を自動記録、EndBranch時に反映メッセージ生成 | `BranchThreadState.cs` |
| 反映メッセージ | 優先順位: SetBranchReflection指定 > TransferFlags自動生成 > なし | `ScenarioManager.cs` |
| 条件付き自動分岐 | `DeclareThreadLatentCond` + AutoBeginBranch で Yarn変数変化時に自動分岐開始 | 同上 |
| Save/Load | BranchThreadState 全フィールド保存・復元対応 | `SaveManager.cs` |
| 安全弁 | EndBranch 待機ループは CancellationToken でキャンセル可能 (StopScenario対応) | `ScenarioManager.cs` |
| ForceClose | TransferSelectionUI の強制閉じ (StopScenario時) | `TransferSelectionUI.cs` |

### 使用例

```yarn
<<BeginBranch "ch2_recon" "偵察分岐">>
<<Message "mason" "この区域を調べよう。">>
<<UnlockTopic "topic_area_4">>
<<SetBranchReflection "区域4の情報を入手した。">>
<<EndBranch true>>
```

### 知識転送選択モード

```yarn
<<BeginBranch "ch2_deep" "深層調査">>
<<UnlockTopic "topic_a">>
<<UnlockTopic "topic_b">>
<<UnlockTopic "topic_c">>
<<EndBranch true "select">>
```

プレイヤーに選択UIが表示され、持ち帰るトピックを選択。`$has_topic_*` 変数はtrue維持（知っているが見せないだけ）、HiddenFlags で非表示管理。

---

## 10c. 進捗可視化基盤（Phase 1 実装済み）

チャプター進捗の集約表示 + 次アクション誘導。

### アーキテクチャ

- **データ集約**: `ProgressTracker.cs` — チャプター進捗・矛盾・断片・トピックの加重平均進捗率算出
- **ヒント生成**: `NudgeSystem.cs` — 進捗状況に基づく次アクション誘導テキスト生成
- **UI表示**: `ProgressSummaryUI.cs` — ダッシュボード内の進捗バー + 数値 + 誘導テキスト

### 進捗率計算

加重平均: チャプター進行 35% + 矛盾発見 30% + 断片収集 20% + トピック解放 15%

### 制限事項

- Phase 1 (MVP) のみ実装。Phase 2 (チャプター間接続可視化) は未着手
- 詳細: `docs/StorySpec/18_progress_visibility.md`

---

## 11. 未実装機能（StorySpec で必要だが現在ない機能）

### 優先度: 中（サブコンテンツに必要）

| 機能 | 説明 | 実装難度 |
| ---- | ---- | -------- |
| 偵察システム | ロケーション探索・アイテム収集 | 高 |
| 断片クロスリファレンス | 断片同士の照合・矛盾検出 | 中 |
| ブランチ間クロスリファレンスUI | 分岐間の情報比較UI | 中 |

### 優先度: 低（後から追加可能）

| 機能 | 説明 | 実装難度 |
| ---- | ---- | -------- |
| ローカライズ | 日英切替 | 中 |
| 広告/スタミナ | F2P マネタイズ基盤 | 中 |
| コンタクトリスト | キャラクター管理UI | 低 |
| チャット検索 | 過去ログのキーワード検索 | 低 |
| サウンド (SP-009) | BGM + SE | 中 |

---

## 12. ノード設計のベストプラクティス

### ノード命名規則（推奨）

```
Ch1_Opening          ← チャプター1の冒頭
Ch1_MarcoIntro       ← Marco 登場シーン
Ch1_FirstContradiction ← 最初の矛盾発生
Ch1_DayEnd           ← チャプター1の終了
```

### 1ノードの推奨サイズ

- **10-30メッセージ**程度（セーブ復元の粒度を考慮）
- 分岐点で必ずノードを分割する
- 「1シーン = 1ノード」を基本とする

### セーブポイントとしてのノード

ノードの先頭がセーブ復元ポイントになるため:

- ノード冒頭で `$speaker` を必ず設定し直す
- ノード冒頭に文脈がわかる SystemMessage を入れると復帰時に親切

```yarn
title: Ch1_AfterLunch
---
<<SystemMessage "--- 1日目・午後 ---">>
<<set $speaker to "pyramid">>
午後のセッションを始めましょう。
===
```
