# Yarn Editing Pipeline

Windsurf (VS Code) で Yarn ファイルを編集し、Unity で即座に動作確認するための手順書。

---

## 前提条件

- **Unity 6** + YarnSpinner 3.1.3 (`dev.yarnspinner.unity`)
- **Windsurf** (VS Code 系エディタ) + [Yarn Spinner 拡張](https://marketplace.visualstudio.com/items?itemName=SecretLab.yarn-spinner)
- 本プロジェクト: `UnityChatNovelGame`

---

## 1. ファイル配置

Yarn ファイルはすべて `Assets/Resources/Yarn/` に配置:

| ファイル | ノード数 | 内容 |
|---------|---------|------|
| `FirstSlice.yarn` | 4 | Act 1 導入シナリオ |
| `VerticalSlice.yarn` | 4 | 垂直スライスデモ |
| `DebugScript.yarn` | 1 | カスタムコマンドテスト用 |

`Project.yarnproject` が `"sourceFiles": ["**/*.yarn"]` で全 `.yarn` ファイルを自動検出する。
新しい `.yarn` ファイルを追加する場合、同ディレクトリに置くだけで自動認識される。

---

## 2. Windsurf での編集

### 開き方
1. Windsurf でプロジェクトルートを開く
2. `Assets/Resources/Yarn/` 内の `.yarn` ファイルを開く
3. Yarn Spinner 拡張による構文ハイライトが有効になる

### ノードグラフ表示
- `Ctrl+Shift+P` → `Yarn Spinner: Show Graph` でノード接続を可視化

### 基本構文
```yarn
title: NodeName
---
<<set $speaker to "unknown">>
ここにセリフを書く。

-> 選択肢A
    <<jump NextNodeA>>
-> 選択肢B
    <<jump NextNodeB>>
===
```

---

## 3. カスタムコマンド一覧

ScenarioManager に登録済みのコマンド:

| コマンド | 用途 | 例 |
|---------|------|-----|
| `<<set $speaker to "ID">>` | 発言者の切替 | `<<set $speaker to "unknown">>` |
| `<<SystemMessage "text">>` | 画面中央にシステム通知 | `<<SystemMessage "【SYSTEM】通信切断">>` |
| `<<StartWait seconds>>` | 待機 + タイピング演出 | `<<StartWait 1.5>>` |
| `<<SkipWait>>` | 待機をスキップ | `<<SkipWait>>` |
| `<<Message "charID" "text">>` | 直接メッセージ追加 | `<<Message "player" "Hello">>` |
| `<<Image "charID" "imageID">>` | 画像メッセージ | `<<Image "player" "photo_01">>` |
| `<<UnlockTopic "topicID">>` | トピック解放 | `<<UnlockTopic "signal_01">>` |
| `<<Glitch level>>` | グリッチ演出 (1-3) | `<<Glitch 2>>` |

---

## 4. Unity への反映

`.yarn` ファイルを保存後:
1. Unity Editor のウィンドウに切り替える（Alt+Tab）
2. Unity が自動的に変更をインポートする
3. Console にエラーがなければ反映完了

**変更が反映されない場合:**
- `Project.yarnproject` を右クリック → `Reimport`
- または Unity メニュー `Assets > Refresh`

---

## 5. テスト方法

### 方法A: Inspector「Play from Node」（Editor 用）

1. ContentAuthoring シーンを開く (`Assets/Scenes/ContentAuthoring.unity`)
2. Hierarchy で `DialogueSystem` を選択
3. ScenarioManager Inspector の **Start Node** ドロップダウンからノードを選択
4. **「Play from Node」** ボタンをクリック
5. 自動で Play Mode に入り、選択ノードから再生開始

### 方法B: Debug Hub（ランタイム）

1. ContentAuthoring シーンで Play Mode に入る
2. **F12** キーを押す → Debug Hub オーバーレイが表示
3. ノードボタンをタップ → そのノードから再生開始
4. 再生中に左上の **「Back to Hub」** → 停止してハブに戻る
5. 別のノードを選択して続けてテスト可能

---

## 6. トラブルシューティング

### ノードがドロップダウンに表示されない
- `.yarn` ファイルが保存されているか確認
- Unity Editor に切り替えて再インポートを待つ
- Console で Yarn コンパイルエラーを確認
- ノードに `title:` ヘッダーと `===` フッターがあるか確認

### ダイアログが開始しない
- DialogueRunner に YarnProject がアサインされているか確認
- `m_AutoStartYarn` が有効になっているか確認（Play from Node は自動で有効化する）
- Console の "ScenarioManager" エラーを確認

### カスタムコマンドが動作しない
- `ScenarioManager.RegisterCustomCommands()` でコマンドが登録される
- ScenarioManager に DialogueRunner と ChatController の参照があるか確認
- `#if YARN_SPINNER` が有効か確認（asmdef の versionDefines で自動定義）

### Debug Hub が表示されない
- ContentAuthoring シーンの DialogueSystem に `DebugHubController` コンポーネントが追加されているか確認
- `DialogueRunner` フィールドがアサインされているか確認
- F12 キーを押しているか確認
