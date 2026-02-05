# Task: Chat UI Data Integration
Status: DONE
Tier: 2
Branch: main
Owner: Worker
Created: 2026-01-16T13:50:00Z
Report: docs/reports/REPORT_TASK_008_ChatUI_Integration.md

## Objective
`ChatController` と `ScenarioManager` (またはデータローダー) を連携させ、外部ファイル（JSON/CSV/ScriptableObject）からチャットデータを読み込み、UIに表示する機能を実装する。
また、ユーザーの選択（ボタン押下など）に応じて次のメッセージを表示するフローを構築する。

## Context
- **前工程**: TASK_007でChat UIの見た目と基本動作は完成済み。
- **ゴール**: 定義されたシナリオデータに基づいて、チャットが自動・対話的に進行するようにする。
- **参照**: `ChatController.cs`, `ScenarioManager.cs` (既存ロジックを確認)

## Focus Area
- `Assets/Scripts/Core/` (データ読み込み、進行管理)
- `Assets/Scripts/UI/ChatController.cs` (外部からのデータ注入IF)
- `Assets/Resources/Data/` (テスト用シナリオデータ作成)

## Forbidden Area
- `Assets/Scripts/UI/` の見た目に関する大幅な変更（機能追加はOK）
- 既存の他のシステム（探索パートなど）への影響

## Constraints
- データ形式はプロジェクトの標準（JSONまたはScriptableObject）に従う。
- 非同期読み込みが必要な場合は `UniTask` または `Coroutine` を使用する。

## Steps
1. **データ定義**: チャットシナリオ用のデータ構造（Message, Sender, Delay, Choices等）を定義する（既に存在すれば再利用）。
2. **ローダー実装**: `ChatScenarioLoader` または `ScenarioManager` にチャットデータ読み込み機能を追加。
3. **コントローラー拡張**: `ChatController` に `PlayScenario(ScenarioData data)` のようなメソッドを追加。
4. **分岐ロジック**: 選択肢（ボタン）が表示され、選択によって展開が変わる仕組みを実装（または既存利用）。
5. **結合テスト**: `ChatDevScene` でシナリオデータを流し込み、一連の会話が成立することを確認。

## DoD (Definition of Done)
- [x] チャットシナリオデータ（テスト用含む）が定義されている
- [x] シナリオデータを読み込み、`ChatController` で順次表示できる
- [x] 相手側のメッセージには適切なディレイ（Typing演出）が入る
- [x] ユーザー入力（選択肢または自由入力）待ちの状態を作れる
- [x] **Evidence**: Implement automated verification using `VerificationCapture` (TASK_016).
  - Screenshots of Chat UI in action saved to `docs/evidence/`.
- [x] `docs/reports/REPORT_TASK_008_ChatUI_Integration.md` に動作確認のログ/スクショがある
