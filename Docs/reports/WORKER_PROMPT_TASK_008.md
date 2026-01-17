# Worker Prompt: TASK_008_ChatUI_Integration

あなたは Worker Agent です。以下の指示に従い、タスクを遂行してください。
Orchestrator によって定義された境界（Focus/Forbidden）を遵守することが求められます。

## 参照情報
- **チケット**: `docs/tasks/TASK_008_ChatUI_Integration.md` (必読)
- **SSOT**: `docs/Windsurf_AI_Collab_Rules_latest.md`
- **HANDOVER**: `docs/HANDOVER.md`
- **Context**: `AI_CONTEXT.md`
- **Previous Implementation**: `Assets/Scripts/UI/ChatController.cs`

## ミッション
**Chat UI Data Integration (Scenario Loading & Playback)**

- `ScriptableObject` ベースのチャットシナリオデータ構造 (`ChatScenarioData`) を定義する。
- シナリオの進行管理クラス (`ChatScenarioManager`) を実装し、`ChatController` を制御する。
- 適切なディレイ（タイピング演出時間含む）を実装し、会話のテンポを作る。
- `ChatDevScene` を使用し、実際にデータが読み込まれ会話が成立することを確認する。

## 境界 (Boundaries)

### Focus Area（変更許可）
- `Assets/Scripts/Core/Data/` (新規 ScriptableObject)
- `Assets/Scripts/Core/ChatScenarioManager.cs` (新規)
- `Assets/Scripts/UI/ChatController.cs` (外部連携用メソッド追加)
- `Assets/Resources/Data/` (テストデータ)

### Forbidden Area（変更禁止）
- `Assets/Scripts/UI/` のVisual部分（Prefabの見た目、色など）
- `Assets/Scripts/Core/` の既存ロジック（`ScenarioManager` 等）に深い依存を作ること（疎結合を維持）

## Definition of Done (DoD)
- [ ] チャットシナリオデータ（テスト用含む）が定義されている
- [ ] シナリオデータを読み込み、`ChatController` で順次表示できる
- [ ] 相手側のメッセージには適切なディレイ（Typing演出）が入る
- [ ] ユーザー入力（選択肢または自由入力）待ちの状態を作れる
- [ ] `docs/reports/REPORT_TASK_008_ChatUI_Integration.md` に動作確認のログ/スクショがある

## 停止条件 (Stop & Report)
- `ChatController` の既存機能（手動送信）を破壊してしまう場合
- Unity Editor がクラッシュする場合
- 要件定義にない複雑な分岐（フラグ管理など）が必要になった場合（シンプルに保つ）

## 納品物
- 新規/変更されたコード
- テストデータ
- `docs/reports/REPORT_TASK_008_ChatUI_Integration.md`
