## 現在のミッション

- **タイトル**: Phase 2 準備 — テストカバレッジ拡充 & Yarn Spinner連携
- **Issue**: なし（クリーンアップ後、Phase 2着手準備）
- **ブランチ**: main
- **関連PR**: なし
- **進捗**: Core System実装完了。クリーンアップ実施中（asmdef修正、重複解消、ドキュメント同期）。

## 次の中断可能点

- クリーンアップ完了後（Phase 1テスト拡充着手前）

## 決定事項

- `.shared-workflows` をサブモジュールとして導入
- `docs/inbox/` と `docs/tasks/` を Git 管理対象として作成

## リスク/懸念

- ~~既存の `docs/` ディレクトリ（大文字）と `docs/` ディレクトリ（小文字）が混在していてる可能性~~ → 物理的には統一済み（Windows大文字小文字非区別）、パス参照も2026-02-06に統一完了

## Backlog（将来提案）

- [x] プロジェクト構造の整理（docs と docs の統合検討）→ TASK_026完了（参照パス196ファイル更新）

## タスク管理（短期/中期/長期）

### Worker完了ステータス

- TASK_022: DONE (Performance Baseline - Report Generated)
- TASK_023: COMPLETED (Verification Gap - Evidence Confirmed)
- TASK_024: COMPLETED (Fix Performance Compilation - Hotfix Done)
- TASK_025: IN_PROGRESS (GC Alloc Reduction - Code Done, After計測待ち)
- TASK_026: COMPLETED (Project Structure Cleanup)
- TASK_027: IN_PROGRESS (Full Playthrough Test - 手動テスト待ち)
- TASK_028: COMPLETED (Save System)
- TASK_031: DONE (Compile Error Fix)
- TASK_040: DONE (Synthesis Recipes作成)
- TASK_041: DONE (Save System UI)
- TASK_043: DONE (Title Screen実装)

### 短期（Next）

- [in_progress] GC Alloc Reduction After計測 (ref: docs/tasks/TASK_025_GCAllocReduction.md)
- [in_progress] Full Playthrough Test 手動実行 (ref: docs/tasks/TASK_027_FullPlaythroughTest.md)
- [pending] Phase 1: テストカバレッジ拡充（EditMode Test中心）
- [pending] Phase 2: Yarn Spinner連携準備

### 中期（Later）

- [ ] Phase 3: パフォーマンス最適化（静的レビュー・ObjectPool実装）
- [ ] Phase 4: ドキュメント整備（ADR・Doxygen・Onboarding）
- [ ] SaveLoadUIビジュアルデザイン
- [ ] オートセーブ機能

### 長期（Someday）

- [ ] コンテンツ制作
- [ ] Phase 2機能（暗号化、クラウドセーブ等）
- [ ] 継続的な運用フローの確立

## 備考（自由記述）

- Unity プロジェクト（ChatNovelGame）のコアシステム実装完了
- 主要クラス: TopicData, SynthesisRecipe, ChatController, ScenarioManager, SaveManager, DeductionBoard, MetaEffectController, TitleScreenManager
- SOLID原則に基づいた設計で拡張性を確保
- Save System（3スロット対応）実装完了
- Synthesis Recipes・Title Screen実装完了
- 2026-02-06: プロジェクトクリーンアップ実施（asmdef修正、GlitchEffect重複解消、ドキュメント同期）

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
- 2026-02-02 13:00: TASK_031完了（コンパイルエラー修正）
- 2026-02-02 18:41: TASK_026/027/028/040/041/043 ステータス更新
- 2026-02-06 13:50: プロジェクトクリーンアップ（asmdef修正、GlitchEffect重複解消、AI_CONTEXT同期）
