# Hallucination Simulator (ハルシネーション・シミュレーター) — Docs Pack

> **更新日**: 2026-03-02  
> **目的**: 企画たたき台〜合意済み設計までを、**ストーリーライター／実装者／デザイナーが同じ参照**で使える形に整流する。

## このドキュメント群の使い方
- まず `docs/00_overview.md` を読み、全体像と用語を揃える。
- 次に `docs/03_chapter_beats.md` を読み、3部×3節（9ブロック）の物語リズムを把握する。
- 断片（Fragments）とハルシコインの扱いは `docs/05_fragments.md` と `docs/06_hallucicoin.md` を参照。
- 未決定事項は `docs/99_open_questions.md` に集約。

## 推奨リポジトリ構成（例）
```
/docs                # 本パックを配置
/src                 # ゲーム実装
/assets              # 素材（権利管理は別途）
/tools               # 生成・検証・ビルド補助
```

## Docs Index
- `docs/00_overview.md` … 企画の要点（ワンページ）
- `docs/01_gdd_gameplay.md` … ゲーム要件（システム）
- `docs/02_story_bible.md` … 世界観・人物・語り口
- `docs/03_chapter_beats.md` … 9ブロック・ビート表（脚本の骨）
- `docs/04_characters.md` … 人物キャラ詳細（人間）
- `docs/05_ai_models.md` … AIモデル群（キャラ差＝失調の差）
- `docs/06_hallucicoin.md` … ハルシコイン仕様
- `docs/07_fragments.md` … 断片（不可索引物）仕様
- `docs/08_ui_ux.md` … UI/UX（検索・チャットUI）
- `docs/09_audio_visual.md` … サウンド／ビジュアル方針
- `docs/10_monetization.md` … マネタイズ案
- `docs/11_production_plan.md` … 役割・制作計画（叩き台）
- `docs/12_reference_works.md` … 参照作品・比較軸
- `docs/13_glossary.md` … 用語集（理論語）
- `docs/99_open_questions.md` … 未決定事項（Askの残）

## ライターへの引き渡しチェック
- [ ] `docs/03_chapter_beats.md` の各節「断片」「偵察」「層更新」が具体化されている  
- [ ] `docs/07_fragments.md` の文体カテゴリ・媒体・救出対象が確定している  
- [ ] 主人公の「裏切りB/C」演出の挿入位置が合意されている
