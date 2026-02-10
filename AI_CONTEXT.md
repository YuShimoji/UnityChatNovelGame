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
- ~~**SaveData.YarnVariables の Dictionary<string, object> が JsonUtility でシリアライズ不可**~~ → Newtonsoft.Json に切替済み
- ~~**Yarn Spinner がGitHub直参照（バージョン固定なし）**~~ → コミットハッシュ `#a94063e96004` (v3.1.3) で固定済み
- **シナリオシステムの二重構造**（Yarn Spinner方式 + ScriptableObject方式）— 方針明確化が必要

## Backlog（将来提案）

- [x] プロジェクト構造の整理（docs と docs の統合検討）→ TASK_026完了（参照パス196ファイル更新）
- [x] CharacterProfile ScriptableObject 導入 → Sprint S1/S2で実装済み、CharacterProfileCreator Editor ツール追加済み
- [ ] ChatDialogueView (DialogueViewBase継承) の正式実装
- [ ] 連絡先リスト（Contact List）機能
- [ ] Addressables 移行（Resources.Load 脱却）

## タスク管理（短期/中期/長期）

> **詳細ロードマップ**: `docs/PROJECT_ROADMAP.md` を参照（2026-02-07作成）

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

### 短期（Next: 1-2週間）— 品質基盤の確立

- [in_progress] GC Alloc Reduction After計測 (TASK_025)
- [in_progress] Full Playthrough Test 手動実行 (TASK_027)
- [done] **SaveData シリアライズ修正** — Newtonsoft.Json に切替済み
- [done] **CharacterProfile SO 基盤** — CharacterProfileCreator.cs で Editor メニューから生成可能
- [pending] テストカバレッジ拡充 — ChatController / ScenarioManager / DeductionBoard (各3ケース+)
- [pending] ImageCommand 実装完了 — 画像バブルPrefab + 実Sprite表示
- [pending] ChatDialogueView 実装 — DialogueViewBase 継承、Yarn Spinner正式連携
- [pending] system_message コマンド / StartWait進行制御修正

### 中期（Later: 2-6週間）— 機能拡充 & コンテンツ制作準備

- [ ] MessageBubble オブジェクトプーリング
- [ ] CharacterProfile ベースのバブルカラーリング
- [ ] メッセージアニメーション強化（Scale + Slide + Fade）
- [ ] SaveLoadUI ビジュアルデザイン
- [ ] Options パネル実装（音量、テキスト速度）
- [ ] Safe Area / キーボード対応
- [ ] 連絡先リスト（Contact List）+ add_contact / ChangeStatus コマンド
- [ ] オートセーブ機能（OnApplicationPause + 重要ポイント）
- [ ] Yarn スクリプトテンプレート / コンテンツ制作パイプライン
- [ ] CharacterProfile / Topic / Recipe の本番マスターデータ設計

### 長期（Someday: 2-6ヶ月）— プロダクション & リリース

- [ ] メインストーリー制作（3-5チャプター）
- [ ] キャラクターアート / SE / BGM
- [ ] Addressables 移行
- [ ] セーブデータ暗号化 / クラウドセーブ
- [ ] CI/CD パイプライン
- [ ] ローカライズ基盤（日本語/英語）
- [ ] QA / ストア申請準備

## 備考（自由記述）

- Unity プロジェクト（ChatNovelGame）のコアシステム実装完了
- 主要クラス: TopicData, SynthesisRecipe, ChatController, ScenarioManager, SaveManager, DeductionBoard, MetaEffectController, TitleScreenManager, **CharacterProfile**, **CharacterDatabase**
- SOLID原則に基づいた設計で拡張性を確保
- Save System（3スロット対応）実装完了
- Synthesis Recipes・Title Screen実装完了
- 2026-02-06: プロジェクトクリーンアップ実施（asmdef修正、GlitchEffect重複解消、ドキュメント同期）
- 2026-02-07: Sprint S1/S2 実施—下記完了:
  - SaveDataシリアライズをNewtonsoft.Jsonに切替（Dictionary<string,object>対応）
  - CoreLogicTests.cs 作成（18テストケース: TopicData/SynthesisRecipe/DeductionBoard/SaveData）
  - CharacterProfile SO + CharacterDatabase 作成、ChatController統合（テーマカラー適用）
  - ImageCommand実装完了（AddImageMessage + Sprite実表示）
  - SystemMessageコマンド実装（中央揃えグレーテキスト）
  - StartWaitをCoroutineベースブロッキングに修正
  - TopicData/SynthesisRecipeのOnValidate実装

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
- 2026-02-07 14:56: PROJECT_ROADMAP.md 作成（短期/中期/長期プラン策定、課題・技術的負債の洗い出し、AI_CONTEXT同期）
- 2026-02-07 20:35: Sprint S1/S2 実装完了（S1-3,S1-4,S1-5/6,S2-1,S2-3,S2-4,S2-5）— 新規: CharacterProfile.cs, CharacterDatabase.cs, CoreLogicTests.cs 修正: SaveData.cs, SaveManager.cs, ChatController.cs, ScenarioManager.cs, TopicData.cs, SynthesisRecipe.cs, Tests.asmdef
- 2026-02-08: プロジェクト総点検実施 — 45課題を5カテゴリで識別、docs/tasks/AUDIT_*.md に記録
- 2026-02-09: Phase A ブロッカー解消 — CQ-04/05(TODO解消), CQ-06(Show/Hide実装), CQ-10(YarnSpinnerバージョン固定), AS-01(Characters/フォルダ+CharacterProfileCreator作成)
- 2026-02-10: Phase B コード品質・テスト基盤 — CQ-09(空ファイル削除), CQ-07(IsInputLocked実機能化+SetInputEnabled), CQ-02(ConfigureBubble抽出), CQ-01(Update→onValueChanged), QA-07(CharacterProfile/Database テスト8件追加), テスト失敗修正(TopicCardPrefab設定, SafeDestroy)
