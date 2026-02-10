## 現在のミッション

- **タイトル**: Vertical Slice優先 — チャットノベルゲームエンジン基盤の確定
- **Issue**: なし（仕様統合後の優先度再設定フェーズ）
- **ブランチ**: main
- **関連PR**: なし
- **進捗**: SSOT整理完了。ドキュメント体系を `docs/GAME_DESIGN_DOCUMENT.md` 中心へ統合済み。

## 次の中断可能点

- Vertical Sliceのスコープ固定（MVP機能セット確定）後

## 決定事項

- **SSOTは `docs/GAME_DESIGN_DOCUMENT.md`**。`docs/CONCEPT.md` は補助メモ扱い。
- 旧仕様は `docs/specs/_ARCHIVED_*.md` にアーカイブ。
- プロジェクト方針は **チャットノベルゲームエンジンのコア機能優先**。
- 物語は手作り前提（自動生成なし）。
- 「メタホラー」「FoundPhone」はテーマとして分離可能にする。
- 探索スレッドの候補リソースは **未定（TBD）**。バッテリー管理は当面除外、後から復帰可能な設計にする。
- テスト方針を変更: 全面カバレッジ拡充より、まずは **縦切り実装 + スモーク検証** を優先。

## リスク/懸念

- 既存コード/タスク名に旧方針（推論系・網羅テスト優先）の痕跡が残っている。
- 実装済み機能の一部が、現行のVertical Slice優先度と一致しない可能性がある。
- テスト資産の一部は価値を維持するが、優先順位は再評価が必要。

## Backlog（将来提案）

- [ ] テーマ分離を前提とした演出プロファイル設計（FoundPhone / Meta Horror）
- [ ] ChatDialogueView (DialogueViewBase継承) の正式実装
- [ ] 連絡先リスト（Contact List）機能
- [ ] Addressables 移行（Resources.Load 脱却）

## タスク管理（短期/中期/長期）

> **詳細ロードマップ**: `docs/PROJECT_ROADMAP.md` を参照（次回更新で方針同期予定）

### Worker完了ステータス（履歴）

- TASK_022: DONE (Performance Baseline - Report Generated)
- TASK_023: COMPLETED (Verification Gap - Evidence Confirmed)
- TASK_024: COMPLETED (Fix Performance Compilation - Hotfix Done)
- TASK_025: IN_PROGRESS (GC Alloc Reduction - After計測待ち)
- TASK_026: COMPLETED (Project Structure Cleanup)
- TASK_027: IN_PROGRESS (Full Playthrough Test - 手動テスト待ち)
- TASK_028: COMPLETED (Save System)
- TASK_031: DONE (Compile Error Fix)
- TASK_040: DONE (Synthesis Recipes作成)
- TASK_041: DONE (Save System UI)
- TASK_043: DONE (Title Screen実装)

### 短期（Next: 1-2週間）— Vertical Slice確立

- [in_progress] Vertical Slice範囲の確定（タイトル→チャット進行→分岐→待機→セーブ/ロード）
- [pending] ChatDialogueView 実装（Yarn正式連携）
- [pending] チャットコアUXの確定（自動スクロール/入力ロック/システムメッセージ）
- [pending] サンプルストーリー最小構成の整備（機能検証用）
- [pending] スモークテスト整備（縦切りシナリオの破綻検出）
- [done] 仕様書統合（SSOT確立、アーカイブ整理）

### 中期（Later: 2-6週間）— 機能拡張と可変化

- [ ] テーマ分離可能な演出レイヤー構成
- [ ] 探索スレッドのリソース設計（TBD項目のブレスト→確定）
- [ ] SaveLoadUI ビジュアルデザイン
- [ ] Options パネル実装（音量、テキスト速度）
- [ ] Safe Area / キーボード対応の仕上げ
- [ ] 連絡先リスト + add_contact / ChangeStatus コマンド
- [ ] オートセーブ機能（OnApplicationPause + 重要ポイント）
- [ ] Yarn スクリプトテンプレート / コンテンツ制作パイプライン

### 長期（Someday: 2-6ヶ月）— プロダクション

- [ ] メインストーリー制作（3-5チャプター）
- [ ] キャラクターアート / SE / BGM
- [ ] Addressables 移行
- [ ] セーブデータ暗号化 / クラウドセーブ
- [ ] CI/CD パイプライン
- [ ] ローカライズ基盤（日本語/英語）
- [ ] QA / ストア申請準備

## ドキュメント構成

- **`docs/GAME_DESIGN_DOCUMENT.md`** — 正規仕様書（SSOT）
- **`docs/CONCEPT.md`** — 補助メモ（GDD要約）
- **`docs/specs/`** — 旧仕様メモ（アーカイブ）
- **`docs/PROJECT_ROADMAP.md`** — 実装ロードマップ
- **`docs/HANDOVER.md`** — 引き継ぎ用ステータス
- **`docs/tasks/`** — 個別タスク定義
- **`docs/reports/`** — タスク完了レポート

## 備考（自由記述）

- Unityプロジェクトのコア実装は進行済みだが、優先順位を再編中。
- 主要クラス（現状）: TopicData, SynthesisRecipe, ChatController, ScenarioManager, SaveManager, MetaEffectController, TitleScreenManager, CharacterProfile, CharacterDatabase
- これまでのテスト整備は資産として維持しつつ、当面は縦切り進行の検証を先行する。

## 運用ルール (Non-Negotiable)

- **スクリーンショット報告義務**: UI/Visual変更を含むタスク完了時は `docs/evidence/` に証跡を保存する。
  - Evidenceなし完了報告は原則「未完了」扱い。ただしブロッカー時は速度優先で進め、Issue/Taskに `Evidence Missing` を明記。

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
- 2026-02-07 14:56: PROJECT_ROADMAP.md 作成（短期/中期/長期プラン策定、課題洗い出し）
- 2026-02-07 20:35: Sprint S1/S2 実装完了（SaveData, CoreLogicTests, CharacterProfile, SystemMessage, StartWait等）
- 2026-02-08: プロジェクト総点検実施（45課題識別）
- 2026-02-09: Phase A ブロッカー解消（CQ-04/05/06/10, AS-01）
- 2026-02-10: Phase B 品質・テスト基盤（CQ-01/02/07/09, QA-07）
- 2026-02-10: 仕様統合（SSOT確立、アーカイブ化、方向性をチャットノベルエンジン優先へ更新）
