## 現在のミッション

- **タイトル**: プロジェクトセットアップ改善 & 次期機能開発（Chat UI）準備
- **Issue**: なし（準備フェーズ）
- **ブランチ**: main
- **関連PR**: なし
- **進捗**: Phase 1.5 Audit (Post-Integration). Tasks 001-015 completed and integrated.


## 次の中断可能点

- 次のタスク起票後

## 決定事項

- `.shared-workflows` をサブモジュールとして導入
- `docs/inbox/` と `docs/tasks/` を Git 管理対象として作成

## リスク/懸念

- 既存の `Docs/` ディレクトリ（大文字）と `docs/` ディレクトリ（小文字）が混在していてる可能性

## Backlog（将来提案）

- [ ] プロジェクト構造の整理（Docs と docs の統合検討）

## タスク管理（短期/中期/長期）

### Worker完了ステータス

- TASK_022: DONE (Performance Baseline - Report Generated)
- TASK_023: OPEN (Verification Gap - Evidence Pending)
- TASK_024: COMPLETED (Fix Performance Compilation - Hotfix Done)
- TASK_025: OPEN (GC Alloc Reduction - Dispatched Ready)

### 短期（Next）

- [pending] Unity プロジェクト構造の整理 (ref: docs/tasks/TASK_001_UnityProjectStructure.md, Status: OPEN)
- [pending] Chat UI Implementation (ref: docs/tasks/TASK_007_ChatUI_Implementation.md, Status: CLOSED)
- [in_progress] Topic ScriptableObjects Creation (ref: docs/tasks/TASK_011_TopicScriptableObjects.md, Status: IN_PROGRESS)
- [in_progress] TopicData Verification and Evidence Collection (ref: docs/tasks/TASK_013_TopicDataVerification.md, Status: IN_PROGRESS)
- [pending] Deduction Board Implementation (ref: docs/tasks/TASK_016_DeductionBoard_Conflict.md, Status: CLOSED (Merged to TASK_018))
- [completed] Performance Baseline Measurement (ref: docs/tasks/TASK_022_PerformanceBaseline.md, Status: DONE)
- [pending] Verification Gap Closure (ref: docs/tasks/TASK_023_VerificationGap.md, Status: OPEN)
- [completed] Fix Performance Compilation Error (ref: docs/tasks/TASK_024_FixPerformanceCompilation.md, Status: COMPLETED)
- [pending] GC Alloc Reduction (ref: docs/tasks/TASK_025_GCAllocReduction.md, Status: OPEN)

### 中期（Later）

- [ ] Yarn Spinner連携の詳細実装
- [ ] テストコード作成
- [ ] 統合テスト

### 長期（Someday）

- [ ] プロジェクト構造の整理
- [ ] 継続的な運用フローの確立

## 備考（自由記述）

- Unity プロジェクト（ChatNovelGame）のコアシステム実装完了
- 4つの主要クラス（TopicData, SynthesisRecipe, ChatController, ScenarioManager）を作成・実装完了
- SOLID原則に基づいた設計で拡張性を確保
- TASK_002（ロジック実装）完了
- 次のステップ: Prefab作成、DeductionBoard実装、MetaEffectController実装

## 運用ルール (Non-Negotiable)

- **スクリーンショット報告義務**: UI/Visual な変更を含むタスク完了時は、必ず `docs/evidence/` にスクリーンショット（または動画）を保存し、レポートに添付すること。
  - Evidence なき完了報告は原則「未完了」とみなすが、ブロッカーになる場合は「速度優先」で進行してよい（その場合は Issue/Task に Evidence Missing と明記すること）。

## Worker Status

- Active Workers: None

## 履歴

- 2026-01-06 06:45: AI_CONTEXT.md を初期化
- 2026-01-06 08:10: TASK_001完了（Unity Core System Skeleton実装）
- 2026-01-06 08:20: TASK_002起票完了（ロジック実装タスク）
- 2026-01-06 09:00: TASK_002完了（ロジック実装完了）
