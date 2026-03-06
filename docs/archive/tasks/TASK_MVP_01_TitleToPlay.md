# TASK_MVP_01_TitleToPlay

## Objective
タイトル画面から1操作で会話開始状態へ遷移し、MVP導線の入口を確定する。

## Scope
- 触る範囲
  - タイトルUIのPlay導線（ボタン/イベント）
  - Play開始時の二重入力防止（連打ガード）
  - 必要最小限のシーン遷移または同一シーン内状態切替
- 触らない範囲
  - Save/LoadやOptionsの機能拡張
  - 演出強化、アニメーション追加
  - シーン構成の大幅見直し

## Non-Goals
- タイトル画面の見た目改善
- 複数解像度最適化
- タイトル機能の網羅実装

## Implementation Plan
1. ✅ MVPGameController.cs を新規作成。Title→Chat→Choice→Endを1クラスで完結。
2. ✅ m_IsTransitioning フラグで連打ガードを実装。
3. ✅ MVPSceneSetup.cs (Editor) でシーン自動構築メニューを追加。

## 実装対象ファイル（実績）
- `Assets/Scripts/MVP/MVPGameController.cs` ← 新規作成
- `Assets/Scripts/MVP/Editor/MVPSceneSetup.cs` ← 新規作成
- `Assets/Scenes/MVPScene.unity` ← Editorメニューで自動生成

## DoD
- Play開始操作から会話開始まで1回で遷移する。
- 連打しても多重遷移が起きない。
- Play Mode実行中にConsole Error/Exceptionが0。
- 次タスク（ChatFlow）開始条件を満たす。

## Manual Test Steps
1. Unity Editor メニュー `Tools > FoundPhone > Setup MVP Scene` を実行。
2. MVPScene.unity が生成されたことを確認。
3. Play Modeを開始する。
4. タイトル画面で「はじめる」ボタンを1回クリックし、チャット画面に遷移することを確認。
5. 再度Playし、「はじめる」を高速連打し、多重遷移が起きないことを確認。
6. Consoleを確認し、Error/Exceptionが0件であることを確認。

## Output Format
- `Summary`: 実装内容を3-5行で要約
- `Changed Files`: 変更ファイル一覧
- `Key Decisions`: 連打ガードや開始経路固定の判断理由
- `Manual Test Result`: 手順ごとの結果（Pass/Fail）
- `Known Limitations`: 既知の未対応事項（MVP範囲外）
