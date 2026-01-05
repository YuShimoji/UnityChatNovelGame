1. プロジェクト全体仕様書 (Core Specification)
このセクションは、プロジェクトの「憲法」となる部分です。AIとの会話の最初にこのテキストを貼り付けて、プロジェクトの前提知識を与えてください。

Markdown

# Project Context: Unity Chat Novel Game

## 1. Project Overview
Unityで開発する、Yarn Spinnerをベースとした「チャットノベル（Lost Phone系）」ゲーム。
プレイヤーは架空のスマートフォンのインターフェースを通じて、キャラクターとメッセージアプリ形式で対話し、ミステリーを解き明かす。

## 2. Tech Stack & Tools
- **Engine:** Unity 2022.3 LTS or later
- **Language:** C#
- **Narrative Engine:** Yarn Spinner (for Unity)
- **UI Animation:** DOTween Pro
- **Data Format:** ScriptableObject (Static Data), JSON (Save Data)
- **Text Assets:** TextMeshPro (support for Icon Fonts)

## 3. Core Architecture (MVC Pattern)
- **Model:**
    - `ChatProfile` (ScriptableObject): キャラクターの名前、アイコン、ID。
    - `MessageData`: 個々のメッセージ構造体（送信者ID, 本文, 画像パス, タイムスタンプ）。
- **View:**
    - `ChatTimelineView`: メッセージバブルの生成、スクロール制御、アニメーション担当。
    - `MessageBubble`: 個々のフキダシPrefab制御（9-Slice, テキストサイズ調整）。
- **Controller:**
    - `ChatManager`: Yarn Spinnerからのコマンドを受け取り、ModelをViewに渡す仲介役。
    - `YarnCommandHandler`: Yarnスクリプト内の独自コマンド（画像送信、待機など）を処理。

## 4. Key Constraints
- **No Hardcoding:** 会話内容は全てYarnファイルで管理する。
- **Responsive:** 縦横比が異なるモバイル端末（iOS/Android）に対応するUI設計（SafeArea対応）。
- **Performance:** チャットログが長くなっても重くならないよう、オブジェクトプーリングまたは最適化されたインスタンス化を行う。
2. 機能別詳細仕様 (Module Specifications)
以下は、実装フェーズごとにAIに渡す具体的な指示書です。

A. チャットUI & 演出仕様 (UI/UX Implementation)
「Visual」部分の実装指示です。

Markdown

# Feature Spec: Chat UI System

## Requirements
Unity uGUIを使用して、LINEやWhatsAppのようなモダンなチャット画面を構築する。

## 1. Hierarchy Structure
- **ScrollView:** 慣性スクロール付き。
    - **Viewport**
        - **Content (Vertical Layout Group):**
            - Padding: Left/Right 20, Top/Bottom 20
            - Spacing: 15
            - Content Size Fitter: Vertical fit set to Preferred Size.

## 2. Prefab Design (`MessageBubble`)
- **Components:**
    - Image (Sliced Sprite): 角丸四角形（Bubble_Base.png）。
    - TextMeshProUGUI: メッセージ本文。
    - Layout Element: Flexible Width = 0 (横幅自動拡張を防ぐ)。
- **Behavior:**
    - テキスト量に応じて縦に伸びる。
    - 画面幅の70%を最大幅とする。
    - **Right_Bubble (Player):** 右寄せ、色はアクセントカラー。
    - **Left_Bubble (NPC):** 左寄せ、色はグレー、左端にアイコン画像を表示。

## 3. Animation (using DOTween)
- **Spawn Animation:**
    - `OnEnable` または生成時に実行。
    - Scale: 0 -> 1 (Elastic/BackOut Ease) で「ポンッ」と飛び出す演出。
    - Fade: Alpha 0 -> 1。
- **Auto Scroll:**
    - 新しいメッセージ追加時、自動的に最下部へスクロール。
    - `Canvas.ForceUpdateCanvases()` を使用し、ズレを防ぐこと。

## 4. Assets
- Use `Bubble_Base.png` (9-sliced) for background.
- Use `Avatar_Default.png` for missing icons.
B. Yarn Spinner 連携仕様 (Logic Implementation)
「Logic」部分、シナリオエンジンとの接続指示です。

Markdown

# Feature Spec: Yarn Spinner Integration

## Requirements
Yarn Spinnerの標準機能を使用しつつ、チャット特有の挙動をカスタムコマンドで実装する。

## 1. Yarn Scripting Rules
- 通常のセリフはそのまま表示。
- 話者名（Character Name）によって、左右のバブル（Player vs NPC）を自動判定。

## 2. Custom Commands Implementation (`YarnCommandHandler.cs`)
以下のコマンドをC#側で属性 `[YarnCommand("command_name")]` を用いて実装する。

- `<<send_image "image_id">>`:
    - テキストの代わりに画像バブルを表示する。
- `<<typing_status "char_id" duration>>`:
    - 指定した秒数、相手の「入力中...」アニメーションを表示し、進行を一時停止する。
- `<<wait_realtime seconds>>`:
    - ゲーム内進行を実際の時間（秒）だけ停止する。
- `<<add_contact "char_id">>`:
    - 連絡先リストに新しいキャラクターを追加する。

## 3. Dialogue Runner Configuration
- `Dialogue Views` には標準の `Line View` ではなく、独自の `ChatView` をアタッチする。
- `UserRequestedViewAdvancement` は無効化し、自動送りまたはタップ送りを制御できるようにする。
C. データ管理仕様 (Data & Persistence)
「Data」部分、キャラクター定義とセーブ周りの指示です。

Markdown

# Feature Spec: Data Management

## 1. Character Profiles (`ChatProfile` ScriptableObject)
- **Fields:**
    - `string ID` (Yarnスクリプト内の名前と一致させる)
    - `string DisplayName` (表示名)
    - `Sprite Icon`
    - `Color ThemeColor` (バブルの色など)

## 2. Save System (Concept)
- Yarn Spinnerの標準セーブシステム（Variable Storage）に加え、以下のゲーム状態をJSON化して保存するManagerを作成する。
    - **Unlock Status:** 解放された画像、追加された連絡先。
    - **Chat History:** 過去のログ（再起動時に復元するため）。
    - **Current Node:** 現在どの会話ノードにいるか。