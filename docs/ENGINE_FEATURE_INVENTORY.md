# Engine Feature Inventory

**最終更新**: 2026-03-06
**エンジン**: Unity 6.3 LTS (6000.3.6f1) + Yarn Spinner 3.1.3

このドキュメントは、シナリオ執筆者が「今のエンジンで何ができるか」を把握するためのリファレンスです。

---

## 1. 利用可能な Yarn コマンド

### メッセージ系

| コマンド | 構文 | 説明 |
| -------- | ---- | ---- |
| Message | `<<Message "charID" "テキスト">>` | 指定キャラのメッセージバブルを表示 |
| MessageTagged | `<<MessageTagged "charID" "テキスト" "lineTag">>` | 矛盾タグ付きメッセージ。矛盾指摘システムの識別子として使用 |
| SystemMessage | `<<SystemMessage "テキスト">>` | 中央寄せのシステム通知を表示 |
| Image | `<<Image "charID" "imageID">>` | 画像メッセージを表示（`Resources/Images/` 内） |

### 演出系

| コマンド | 構文 | 説明 |
| -------- | ---- | ---- |
| StartWait | `<<StartWait 秒数>>` | 指定秒数の待機 + タイピングインジケーター表示 |
| SkipWait | `<<SkipWait>>` | 待機をキャンセル |
| Glitch | `<<Glitch レベル>>` | グリッチ演出（1-5段階） |

### ゲームシステム系

| コマンド | 構文 | 説明 |
| -------- | ---- | ---- |
| UnlockTopic | `<<UnlockTopic "topicID">>` | トピックカードを推理ボードに追加 |

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
- **Icon**: アバター画像（Sprite）※現在バブルには未表示
- **ThemeColor**: バブルの背景色
- **IsPlayer**: `true` の場合、メッセージが右寄せ表示

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

---

## 6. ローカライズ対応状況

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

## 7. 矛盾指摘システム（Phase 2 実装済み）

### 実装済み機能

| 機能 | 説明 | ファイル |
| ---- | ---- | -------- |
| 長押し選択 | バブル0.5秒長押しで1つ目選択、タップで2つ目選択 | `MessageBubble.cs` |
| 矛盾判定 | ContradictionDatabase の順不同マッチング、クールダウン10秒 | `ContradictionManager.cs` |
| 成功演出 | 緑フラッシュ + スケールパルス + 接続線 + 通知パネル | `ContradictionFeedbackController.cs` |
| 失敗演出(不一致) | 赤フラッシュ + 回転シェイク + エラーバナー + クールダウン | 同上 |
| 失敗演出(既発見) | 黄フラッシュ + 「既に発見済み」バナー | 同上 |
| ヒントバナー | 1つ目選択時に「2つ目をタップ」表示 | 同上 |
| 接続線 | 2バブル間の直線（Image回転方式、成功/失敗で色変化→フェードアウト） | 同上 |
| HalluciCoin | 矛盾発見時に報酬加算、セーブ/ロード対応済み | `ContradictionManager.cs` |
| トピック自動解放 | 矛盾発見時に ContradictionPair.UnlockTopic を DeductionBoard に追加 | `DeductionBoard.cs` |
| データ | 7ペア（Ch1x4, Ch2x3）、全報酬10コイン、難易度1 | `Resources/Contradictions/` |

### セットアップ要件

ContentAuthoring シーンの Canvas 直下に `ContradictionFeedbackController` を配置し、
`ChatController` への参照を Inspector でアサインすること。

---

## 8. 未実装機能（StorySpec で必要だが現在ない機能）

### 優先度: 高（メインループに必要）

| 機能 | 説明 | 実装難度 |
| ---- | ---- | -------- |
| 断片インベントリ | 収集した断片テキストの閲覧UI | 中 |
| ダッシュボード画面 | チャンネル選択→チャット遷移のメイン画面 | 高 |

### 優先度: 中（サブコンテンツに必要）

| 機能 | 説明 | 実装難度 |
| ---- | ---- | -------- |
| サブスレッドUI | Discord 的なマルチスレッド表示 | 高 |
| 偵察システム | ロケーション探索・アイテム収集 | 高 |
| 断片クロスリファレンス | 断片同士の照合・矛盾検出 | 中 |

### 優先度: 低（後から追加可能）

| 機能 | 説明 | 実装難度 |
| ---- | ---- | -------- |
| ローカライズ | 日英切替 | 中 |
| 広告/スタミナ | F2P マネタイズ基盤 | 中 |
| コンタクトリスト | キャラクター管理UI | 低 |
| チャット検索 | 過去ログのキーワード検索 | 低 |

---

## 9. ノード設計のベストプラクティス

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
