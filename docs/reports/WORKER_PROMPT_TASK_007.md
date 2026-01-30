# Worker Prompt: TASK_007 Core System Verification

## Request
あなたは Unity クライアントエンジニアとして、**Core System (ChatController, ScenarioManager)** の動作検証（再試行）を行ってください。前回の検証では Evidence が提出されず、タスクが完了できませんでした。

## Context
- **Current State**: コード実装完了。Phase 1.75 通過済みだが、タスクは OPEN 状態。
- **Goal**: `DebugChatScene` を実際に動かし、**スクリーンショットを提出すること**。
- **Blocking**: これが完了しない限り、次の機能実装（Deduction Board）に着手できない（Verification First 戦略）。

## Focus Area
1. **Scene**: `Assets/Scenes/DebugChatScene.unity` (なければ作成/Setup)
2. **Script**: `Assets/Resources/Yarn/DebugScript.yarn` (全機能テスト用)
3. **Action**: Unity Editor で Play し、以下の動作を確認する:
   - メッセージ表示 (Left/Right)
   - 画像表示 (`<<Image>>`)
   - 待機 (`<<StartWait>>`)
   - ログ出力 (`<<UnlockTopic>>`, `<<Glitch>>`)

## Tasks (Step-by-Step)
1. `Assets/Scenes/DebugChatScene.unity` を開く (存在しない場合は `Tools > FoundPhone > Setup Debug Scene` を試すか、手動作成)。
2. `ScenarioManager` に `Assets/Resources/Yarn/DebugScript.yarn` をアタッチする。
3. Unity Editor で Play する。
4. シナリオを最後まで進める。
5. **Evidence 取得 (必須)**:
   - `Assets/Scripts/Utils/VerificationCapture.cs` をシーン内のGameObject（例: Camera）にアタッチする。
   - `CaptureOnStart` を true に設定。
   - PlayMode を実行し、`Docs/evidence/` に `Capture_...png` が生成されることを確認する。
   - 生成された画像を証拠として採用する。
6. `docs/tasks/TASK_007_Verification.md` の DoD チェックボックスを埋める。
7. レポート `docs/inbox/REPORT_TASK_007_Verification.md` を作成する (Evidence パスを明記)。

## Forbidden Area
- プロダクションコード (`ChatController.cs` 等) のロジック変更
- 新機能の追加

## Output
- `docs/evidence/task007_chat_ui.png`
- `docs/evidence/task007_console.png`
- `docs/inbox/REPORT_TASK_007_Verification.md`
