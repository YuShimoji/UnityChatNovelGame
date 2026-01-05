1. 【コア仕様書】プロジェクト全体構造とチャット基本機能
まず、このプロンプトでプロジェクトの土台と基本のチャットシステムを作らせます。

Markdown

# Project Specification: "Lost Phone" Style Chat Adventure
Unityバージョン: 2022.3 LTS以降
使用言語: C#
必須パッケージ: TextMeshPro, Yarn Spinner, DOTween (Pro/Free)

## 1. 概要
「拾ったスマホ」を通じてストーリーが進むチャットノベルゲームのコアシステムを構築する。
プレイヤーは架空のメッセージアプリを通じてキャラクターと対話し、物語を読み進める。

## 2. アーキテクチャ要件
以下のクラス構成で実装すること。

### A. Data Structures (ScriptableObject)
**CharacterProfile (ScriptableObject)**
各キャラクターの静的データを管理する。
- `string characterID`: Yarn Spinnerで使用するID
- `string displayName`: 表示名
- `Sprite icon`: アイコン画像
- `Color themeColor`: フキダシのベースカラー（動的に着色するため）
- `bool isPlayer`: プレイヤー自身かどうか

### B. UI Components
**ChatBubble (MonoBehaviour)**
個々のメッセージを表示するPrefab。
- 構成要素: 背景Image (Sliced), アイコンImage, メッセージText (TMP), 時刻Text (TMP)
- 要件:
    - プレイヤー（右側）と相手（左側）でレイアウトを切り替え可能にすること。
    - メッセージの長さに応じてサイズ可変（Content Size Fitter使用）。
    - 背景画像は白の9-Slice Spriteを使用し、`CharacterProfile`の色を乗算して着色すること。

**ChatWindowController (MonoBehaviour)**
チャット画面全体を管理する。
- `ScrollRect` と `VerticalLayoutGroup` を使用。
- 新しいメッセージ追加時に自動で最下部へスクロールする機能。

### C. Managers
**MessageManager (Singleton)**
チャットのログを管理し、UIへの生成命令を出す。
- `DisplayMessage(string characterID, string text)` メソッドを持つ。
- `CharacterProfile`のデータベースを持ち、IDからキャラクター情報を検索してUIに適用する。

## 3. 制約事項
- アセット削減のため、アイコンは可能な限りTextMeshProのフォントアイコン、または単純なプリミティブ画像を使用する設計にすること。
- コードは可読性を重視し、将来的なセーブ機能の実装（JsonUtility等）を考慮してデータクラスをシリアライズ可能(`[System.Serializable]`)にしておくこと。

この仕様に基づき、フォルダ構成、ScriptableObjectの定義、および基本的なチャット表示ロジックのコードを作成してください。
2. 【詳細仕様A】Yarn Spinner 連携モジュール
コアができたら、次にシナリオエンジンを接続します。

Markdown

# Implementation Spec: Yarn Spinner Integration

## 目的
Yarn Spinnerのスクリプト（.yarn）を解析し、Unity上のチャットUIに反映させるブリッジシステムを構築する。

## 実装要件

### 1. Yarn Command Handling
Yarn Spinnerの標準機能に加え、以下のカスタムコマンドハンドラを実装すること。

- `<<wait [seconds]>>`: 
    - 指定秒数だけ処理を停止し、その間「入力中...」等の演出ステートへ移行する。
    - コルーチンを使用し、完了後に次のノードへ進む。
    
- `<<send_image [image_name]>>`: 
    - テキストの代わりに画像をチャットバブルとして送信する。
    - `Resources`フォルダまたはAddressablesから画像をロードする想定。

- `<<system_message [text]>>`:
    - キャラクターの発言ではなく、システム通知（例：「グループに参加しました」）として中央揃えのグレーテキストを表示する。

### 2. Dialogue View Implementation
`DialogueViewBase` (Yarn Spinner標準クラス) を継承した `ChatDialogueView` を作成する。
- `RunLine`: テキストを受け取り、`MessageManager`経由でバブルを生成する。
- `RunOptions`: 選択肢ボタンをチャット下部（またはキーボードエリア）に生成する。
- 選択肢が選ばれるまで、進行をブロックする。

### 3. Variable Storage Bridge
Yarn Spinnerの変数をメモリ内だけでなく、将来的なセーブのために外部からアクセス・保存可能な構造にする。
3. 【詳細仕様B】演出・アニメーション（Polish）
最後に、アプリの「手触り」を良くする演出を追加します。

Markdown

# Implementation Spec: UI Animation & Polish

## 目的
DOTweenを使用し、静的なリスト表示ではなく「生きているチャットアプリ」の挙動を再現する。

## 実装要件

### 1. Message Entry Animation
新しいチャットバブルが生成される際、以下の動きをつけること。
- **Scale**: 0 -> 1 (Elastic/Back ease) で「ポンッ」と飛び出す動き。
- **Fade**: 透明 -> 不透明。
- **Slide**: 相手のメッセージは左から、自分は右から少しスライドして定位置へ。

### 2. Typing Indicator (入力中演出)
相手が返信する前の`wait`時間中に表示する専用Prefabを作成する。
- 3つのドットが波打つ（Sin波）アニメーションをDOTweenでループさせる。
- メッセージ受信と同時にフェードアウトして削除し、本メッセージを表示する。

### 3. Keyboard & Area Handling
モバイル端末のソフトウェアキーボード、および「ノッチ（Safe Area）」への対応。
- `InputSystem` または `TouchScreenKeyboard` のAPIを使用し、キーボード出現時に`ScrollRect`の`padding.bottom`または`RectTransform`の高さを動的に調整し、チャットが隠れないようにする。

### 4. Dynamic Timestamp
各メッセージに表示する時刻は、現在時刻ではなく「ゲーム内進行時間」として管理できるように設計する（初期実装は現在時刻でも可）。