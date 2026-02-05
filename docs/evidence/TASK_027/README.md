# TASK_027 Evidence Collection Guide

このディレクトリは、Full Playthrough Test（TASK_027）のエビデンス（スクリーンショット）を保存する場所です。

## 必要なスクリーンショット

以下のタイミングでスクリーンショットを撮影してください:

### 1. 01_initial_scene.png

- **タイミング**: PlayMode開始直後
- **内容**: シーン全体の初期状態
- **確認ポイント**: UI要素が正しく配置されているか

### 2. 02_chat_messages.png

- **タイミング**: チャットメッセージが複数表示された後
- **内容**: Chat UIに複数のメッセージが表示されている状態
- **確認ポイント**: メッセージバブルのスタイル、送信者の区別、スクロール

### 3. 03_topic_unlocked.png

- **タイミング**: UnlockTopicコマンド実行後
- **内容**: DeductionBoardに新しいトピックカードが表示されている状態
- **確認ポイント**: トピックカードの内容、レイアウト

### 4. 04_synthesis_test.png

- **タイミング**: 合成システムのテスト中または後
- **内容**: トピックをドラッグ&ドロップしている様子、または合成結果
- **確認ポイント**: ドラッグ可能性、レシピマッチング、新トピック生成

### 5. 05_glitch_effect.png

- **タイミング**: Glitchエフェクトが最大強度（intensity 5）の時
- **内容**: 画面にGlitchエフェクトが適用されている状態
- **確認ポイント**: エフェクトの視認性、UI機能への影響

### 6. 06_console_log.png

- **タイミング**: PlayMode終了後
- **内容**: Unity Consoleの全ログ
- **確認ポイント**: エラーや警告の有無

## スクリーンショット撮影方法

### Unity Game Viewのスクリーンショット

1. Unity Editor → Game View にフォーカス
2. Windows: `Win + Shift + S` でスニッピングツール
3. または Unity Menu → Assets → Take Screenshot

### Consoleのスクリーンショット

1. Unity Editor → Console Window を開く
2. ログを全て表示
3. `Win + Shift + S` でスニッピングツール

## ファイル保存先

すべてのスクリーンショットを **このディレクトリ** (docs/evidence/TASK_027/) に保存してください。

ファイル名は上記の番号付き名前（01_initial_scene.png など）を使用してください。
