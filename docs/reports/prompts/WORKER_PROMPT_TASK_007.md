# Worker Prompt: TASK_007_Verification

## 参照
- チケット: docs/tasks/TASK_007_Verification.md
- SSOT: docs/Windsurf_AI_Collab_Rules_latest.md
- HANDOVER: docs/HANDOVER.md
- AI_CONTEXT: AI_CONTEXT.md
- MISSION_LOG: .cursor/MISSION_LOG.md
- プロジェクト仕様: 最初のプロンプト（プロジェクトルート）

## 境界

### Focus Area
- `Assets/Scenes/` 配下: DebugChatScene の作成
- `Assets/Resources/Yarn/` 配下: DebugScript の作成
- `ChatController` と `ScenarioManager` の連携確認 (実機動作)
- **証拠作成**: スクリーンショットまたは動画の撮影と `docs/evidence/` への保存

### Forbidden Area
- 既存の `MainScene` やプロダクションコードの破壊的変更
- Core System のロジック変更（バグ修正は可だが、本質的変更は不可）
- 新機能の追加（Deduction Board 等）

## Tier / Branch
- Tier: 3（検証・修正）
- Branch: feat/verify-core-system

## DoD
- [ ] `Assets/Scenes/DebugChatScene.unity` が作成され、再生可能である
- [ ] `Assets/Resources/Yarn/DebugScript.yarn` が作成され、以下の機能を含んでいる
    - [ ] `<<Message>>` (左右吹き出し)
    - [ ] `<<Image>>`
    - [ ] `<<StartWait>>` (タイピングインジケーター)
    - [ ] `<<UnlockTopic>>` (ログ出力確認)
    - [ ] `<<Glitch>>` (ログ出力確認)
- [ ] Unity Editor 上でエラーなくシナリオが最後まで進行する
- [ ] **Evidence (必須)**:
    - [ ] `docs/evidence/task007_chat_ui.png` (チャット動作中スクショ)
    - [ ] `docs/evidence/task007_console_logs.png` (コマンドログ出力スクショ)
    - [ ] (任意) `docs/evidence/task007_demo.mp4`
- [ ] docs/inbox/ にレポート（REPORT_TASK_007_Verification.md）が作成されている
- [ ] 本チケットの Report 欄にレポートパスが追記されている

## 停止条件
- Unity Editor が起動できない
- 既存のコンパイルエラーが再発し、解消不能
- 仕様の仮定が 3 つ以上必要
- 依存追加/更新、破壊的Git操作、GitHubAutoApprove不明での push が必要
- SSOT不足を `ensure-ssot.js` で解決できない

停止時は以下を実施：
1. チケットのStatusをBLOCKEDに更新
2. 事実/根拠/次手（候補）をチケット本文に追記
3. docs/inbox/REPORT_TASK_007_Verification.md を作成し、停止理由を記録
4. チケットのReport欄にレポートパスを追記

## 納品先
- docs/inbox/REPORT_TASK_007_Verification.md

## 実装ヒント
- `DebugChatScene` には `ChatController` と `ScenarioManager` のインスタンスを配置し、Inspectorで適切に参照を設定してください。
- `TopicData` や `SynthesisRecipe` はテスト用のダミーデータ (`Assets/Scripts/Data/Test/` 等) を一時的に作成しても構いません。
- Evidence 用のディレクトリ `docs/evidence/` が存在しない場合は作成してください。
