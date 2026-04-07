# Hallucination Simulator (ハルシネーション・シミュレーター) — Docs Pack

> **更新日**: 2026-04-08
> **目的**: 企画たたき台〜合意済み設計までを、**ストーリーライター／実装者／デザイナーが同じ参照**で使える形に整流する。

## このドキュメント群の使い方
- まず `00_overview.md` を読み、全体像と用語を揃える。
- 次に `03_chapter_beats.md` を読み、3部 x 3章 x 3節（27ブロック）の物語リズムを把握する。
- 断片（Fragments）とハルシコインの扱いは `07_fragments.md` と `06_hallucicoin.md` を参照。
- 未決定事項は `99_open_questions.md` に集約。

## リポジトリ構成

```
Assets/              # Unity ソース・リソース
Packages/            # Unity パッケージ
ProjectSettings/     # Unity プロジェクト設定
docs/                # 本パック（仕様・設計ドキュメント）
scripts/             # ビルド・検証補助スクリプト
```

## Docs Index
- `00_overview.md` … 企画の要点（ワンページ）
- `01_gdd_gameplay.md` … ゲーム要件（システム）
- `02_story_bible.md` … 世界観・人物・語り口
- `03_chapter_beats.md` … 27ブロック（9章）ビート表（脚本の骨。章レベルの概要、節の詳細は未設計）
- `04_characters.md` … 人物キャラ詳細（人間）
- `05_ai_models.md` … AIモデル群（キャラ差＝失調の差）
- `06_hallucicoin.md` … ハルシコイン仕様
- `07_fragments.md` … 断片（不可索引物）仕様
- `08_ui_ux.md` … UI/UX（検索・チャットUI）
- `09_audio_visual.md` … サウンド／ビジュアル方針
- `10_monetization.md` … マネタイズ案
- `11_production_plan.md` … 役割・制作計画（叩き台）
- `12_reference_works.md` … 参照作品・比較軸
- `13_glossary.md` … 用語集（理論語）
- `14_interaction_mechanics.md` … インタラクション・メカニクス（3システム候補 + 設計原則）
- `15_feature_triage_2026-03-10.md` … 機能トリアージ（4項目の現状分類と優先バックログ）
- `16_subthread_ui.md` … サブスレッドUI仕様（統合型スレッドモデル + 2段階トリガー）
- `22_subquest_exploration_content.md` … サブクエスト（サブスレッド探索）コンテンツ設計チャーター（DRAFT）
- `99_open_questions.md` … 未決定事項（Askの残）

### 関連ドキュメント（docs/ 直下）

- `../SCENARIO_AUTHORING_GUIDE.md` … Yarn シナリオ執筆ハンズオン（コマンドリファレンス + コンテンツパターン集）
- `../ENGINE_FEATURE_INVENTORY.md` … エンジン機能リファレンス
- `../YarnEditingPipeline.md` … Yarn 編集パイプライン（技術手順）

## ライターへの引き渡しチェック
- [ ] `03_chapter_beats.md` の各節「断片」「偵察」「層更新」が具体化されている
- [ ] `07_fragments.md` の文体カテゴリ・媒体・救出対象が確定している
- [ ] 主人公の「裏切りB/C」演出の挿入位置が合意されている

---

## 更新履歴

- **2026-04-08**: `22_subquest_exploration_content.md` 追加（サブクエスト探索のボリューム方針・SP-022）。
- **2026-03-12**: リポジトリ構成を現在の Unity プロジェクト構造に修正。旧 Addendum を統合。
- **2026-03-10**: `15_feature_triage_2026-03-10.md` 追加（顔アイコン/スレッド管理/テキスト演出/デザイナー環境のトリアージ）。
- **2026-03-09**: Sync Addendum — Unity プロジェクト構成に準拠。
- **2026-03-07**: 初版。
