# WORKFLOW STATE SSOT

Updated: 2026-02-25

## Current Phase
- P2.5 Diverge (Phase 7 Development Acceleration context)

## In-Progress
- TASK_056: ChatDialogueView の正式実装（Yarn連携強化）
- TASK_057: MessageBubble のオブジェクトプーリング導入
- TASK_058: CharacterProfile ベースの自動カラーリング

## Project Position
- Branch: feature/task-049-build-gate-fix
- Remote Sync: origin/feature に behind 0（ahead 5）
- Main Sync: origin/main に behind 0（ahead 6）
- Submodule: .shared-workflows @ caa90c5（同期済み）

## Verification Policy (3-Level)
- 3: High (実装＋静的確認が揃っている)
- 2: Medium (実装は可能だが検証/依存確認が残る)
- 1: Low (前提不足や不確実性が高い)

## Task Validation Snapshot
- TASK_056: 3/3 (High)
  - 根拠: 既存 `ChatDialogueView` と `ChatController` が存在し、統合拡張の着手条件が整っている。
- TASK_057: 2/3 (Medium)
  - 根拠: 実装方針は明確だが、Layer B の実測（Profiler/PlayMode）で確認が必要。
- TASK_058: 2/3 (Medium)
  - 根拠: `CharacterProfile`/`CharacterDatabase` は存在するが、UI反映点の統一が必要。

## Blockers
- なし（開発優先モード）
- 注記: TASK_055 の検証は開発優先のため一時スキップ（ユーザー指示）

## Next Action
- TASK_056 を Layer A/B 分割で先行着手し、完了後に TASK_057 → TASK_058 の順で実行する。

## Layer Split
- Layer A (AI実装):
  - TASK_056: DialogueView 側の話者解決・選択肢/入力制御の統合
  - TASK_057: Bubble生成/破棄経路をプール経路へ統一
  - TASK_058: CharacterProfile の表示名/色を Bubble 描画へ一元適用
- Layer B (手動検証):
  - TASK_056: DebugChatScene で Yarn line/options の遷移確認
  - TASK_057: 長文会話時の GC/フレーム落ち観測（Profiler）
  - TASK_058: player/NPC/system の配色・可読性確認
