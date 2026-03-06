# Task: ChatDialogueView 正式実装

Status: CLOSED
Tier: 1 (Feature)
Branch: feature/task-056-chat-dialogue-view
Owner: Worker
Created: 2026-02-24
Updated: 2026-02-25
Report: docs/reports/REPORT_TASK_056_ChatDialogueView.md
Milestone: SG-1
Test Phase: Slice

## Objective
- Yarn Spinner の DialogueViewBase 系（現行実装では DialoguePresenterBase）として ChatDialogueView を正式運用可能にする。
- ChatController との連携を強化し、line/options/dialogue lifecycle を一貫処理する。

## Focus Area
- Assets/Scripts/UI/ChatDialogueView.cs
- Assets/Scripts/UI/ChatController.cs
- Assets/Scripts/Core/ScenarioManager.cs

## DoD
- [x] Line 表示で話者解決が一貫（CharacterName / $speaker / fallback）
- [x] Options 表示・キャンセル・確定時のUI状態遷移が安定
- [x] Dialogue開始/終了で入力状態・選択肢表示が破綻しない
- [x] コンパイルエラー 0

## Validation (3-level)
- Current Score: 3/3 (High)
- Reason: 既存実装があり、統合強化の作業境界が明確

## Layer Split
- Layer A: 実装・静的確認
- Layer B: DebugChatScene で手動遷移検証
