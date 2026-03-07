# Hallucination Simulator (ハルシネーション・シミュレーター) — Docs Pack

> **更新日**: 2026-03-07
> **目的**: 企画たたき台〜合意済み設計までを、**ストーリーライター／実装者／デザイナーが同じ参照**で使える形に整流する。

## このドキュメント群の使い方
- まず `00_overview.md` を読み、全体像と用語を揃える。
- 次に `03_chapter_beats.md` を読み、3部 x 3章 x 3節（27ブロック）の物語リズムを把握する。
- 断片（Fragments）とハルシコインの扱いは `07_fragments.md` と `06_hallucicoin.md` を参照。
- 未決定事項は `99_open_questions.md` に集約。

## 推奨リポジトリ構成（例）
```
/docs                # 本パックを配置
/src                 # ゲーム実装
/assets              # 素材（権利管理は別途）
/tools               # 生成・検証・ビルド補助
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
- `99_open_questions.md` … 未決定事項（Askの残）

## ライターへの引き渡しチェック
- [ ] `03_chapter_beats.md` の各節「断片」「偵察」「層更新」が具体化されている
- [ ] `07_fragments.md` の文体カテゴリ・媒体・救出対象が確定している
- [ ] 主人公の「裏切りB/C」演出の挿入位置が合意されている
