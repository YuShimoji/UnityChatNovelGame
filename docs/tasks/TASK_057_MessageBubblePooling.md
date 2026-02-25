# Task: MessageBubble オブジェクトプーリング導入

Status: CLOSED
Tier: 2 (Performance)
Branch: feature/task-057-message-bubble-pooling
Owner: Worker
Created: 2026-02-24
Updated: 2026-02-25
Report: docs/reports/REPORT_TASK_057_MessageBubblePooling.md
Milestone: SG-1
Test Phase: Slice

## Objective
- メッセージ増加時の Instantiate/Destroy 負荷を低減するため、MessageBubble にプーリングを導入する。

## Focus Area
- Assets/Scripts/UI/ChatController.cs
- Assets/Prefabs/UI/MessageBubble.prefab

## DoD
- [x] AddMessage/ClearMessages がプール経由で動作
- [x] Destroy 常用を回避し、再利用経路がある
- [x] コンパイルエラー 0

## Validation (3-level)
- Current Score: 2/3 (Medium)
- Reason: 設計は明確だが Layer B の実測が未実施

## Layer Split
- Layer A: プール実装・既存機能互換の維持
- Layer B: Profiler で GC/CPU の改善確認
