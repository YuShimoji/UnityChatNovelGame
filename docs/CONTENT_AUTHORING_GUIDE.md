# Story Content Authoring Guide

このプロジェクトでは、ストーリー制作に **Yarn Spinner** を使用しています。このドキュメントでは、シナリオテキストの編集方法と、それをゲームに反映させる手順を解説します。

## 1. 推奨編集環境

Yarnファイルの編集には、以下の環境を推奨します。

- **エディタ:** [Visual Studio Code (VS Code)](https://code.visualstudio.com/)
- **拡張機能:** [Yarn Spinner for VS Code](https://marketplace.visualstudio.com/items?itemName=YarnSpinner.yarn-spinner)
  - 構文ハイライト、エラーチェック、ノードの可視化グラフ機能が利用可能です。

## 2. シナリオファイルの構成

シナリオファイルは `Assets/Resources/Yarn/` ディレクトリに `.yarn` 拡張子で保存されています。

- **`FirstSlice.yarn`**: 現在のテスト用メインシナリオ。
- **`Project.yarnproject`**: 全てのYarnファイルを束ねてコンパイルするための設定ファイル。新しい `.yarn` ファイルを追加した際は、ここに含まれている必要があります（現在は `**/*.yarn` で自動包含設定されています）。

## 3. 基本的な書き方

```yarn
title: Node_Name
---
<<set $speaker to "character_id">>
セリフの内容をここに書きます。

-> 選択肢A
    <<jump Node_A>>
-> 選択肢B
    <<jump Node_B>>
===
```

### 使用可能なカスタムコマンド

- `<<SystemMessage "テキスト">>`: 中央にシステム通知を表示。
- `<<StartWait 秒数>>`: 指定秒数待機（相手が入力中 ... のアニメーションが表示されます）。
- `<<UnlockTopic "TopicID">>`: 推論用のトピックを解放。
- `<<Glitch 強度>>`: 画面にノイズ演出を入れる。

## 4. Unityへの反映手順

1. VS Codeで `.yarn` ファイルを保存します。
2. Unity Editorに戻ります。自動的にインポートとコンパイルが走ります。
3. `ContentAuthoring` シーンで `ScenarioManager` の `Start Node` を、書き換えたノード名に合わせます。
4. Playボタンを押して確認します。

## 5. 今後の拡張予定（演出・機能）

ユーザー様からの要望に基づき、以下の機能を Phase 2 以降で実装予定です。

- **選択肢のインライン化:** 画面下部の固定ボタンではなく、チャットの流れの中に選択肢ボタンが表示される形式への変更。
- **マルチスレッドチャット:** 複数の相手と並行して会話が進むシステム。
- **リッチな演出:** 画像表示コマンドの強化、キャラクターごとのタイピング速度変更、メッセージ受信アニメーションの改善。
