# Worker Prompt: TASK_007_ChatUI_Implementation

あなたは Worker Agent です。以下の指示に従い、タスクを遂行してください。
Orchestrator によって定義された境界（Focus/Forbidden）を遵守することが求められます。

## 参照情報
- **チケット**: `docs/tasks/TASK_007_ChatUI_Implementation.md` (必読)
- **SSOT**: `docs/Windsurf_AI_Collab_Rules_latest.md`
- **HANDOVER**: `docs/HANDOVER.md`
- **Context**: `AI_CONTEXT.md`

## ミッション
**Chat UI の実装 (Asset/Scene/Script)**

- `ChatDevScene` を作成し、チャット画面（スクロールビュー、入力欄、送信ボタン）を構築する。
- 既存の `ChatController` を活用し、UIイベントと連携させる。
- PlayMode でチャット送信・表示が機能することを確認する。

## 境界 (Boundaries)

### Focus Area（変更許可）
- `Assets/Scenes/ChatDevScene.unity` (新規)
- `Assets/Scripts/Core/ChatController.cs` (UI連携追加のみ)
- `Assets/Prefabs/UI/` (調整が必要な場合)

### Forbidden Area（変更禁止）
- `Assets/Scripts/Core/` のその他ロジック（`ScenarioManager` 等）
- 既存のPrefabを理由なく破壊すること
- プロジェクト設定（ProjectSettings）

## Definition of Done (DoD)
- [ ] `ChatDevScene` が作成され、エラーなく再生できる
- [ ] 画面下部にテキスト入力欄と送信ボタンがある
- [ ] 送信ボタン押下で、入力されたテキストが `ChatController` に渡される
- [ ] `ChatController` が `MessageBubble` を ScrollView 内に生成する
- [ ] タイピング演出（ `TypingIndicator` ）が表示・非表示される
- [ ] 自動スクロール（簡易実装）が機能する
- [ ] レポート `docs/reports/REPORT_TASK_007_ChatUI_Implementation.md` が作成されている

## 停止条件 (Stop & Report)
- `ChatController` のロジックに重大な欠陥があり、UI連携が不可能な場合
- Unity Editor がクラッシュする場合
- 依存する Prefab (`MessageBubble` 等) が破損している場合

## 納品物
- 変更されたコード/シーン
- `docs/reports/REPORT_TASK_007_ChatUI_Implementation.md` (実行結果のスクショあるいはログを含むこと)
