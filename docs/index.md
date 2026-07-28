# UnityChatNovelGame Local Document View

このページは、既存 Markdown 正本を改変せずにブラウザで横断閲覧・監査・ページ翻訳確認するための入口です。

## 目的

- リポジトリ内の Markdown を MkDocs Material のツリーペインで確認する
- Chrome / Edge / DeepL 拡張のページ翻訳を、一時的な読解補助として使う
- 仕様書本文の意味内容、制約、用語、判断基準を要約・翻訳・再構成しない
- 翻訳版の恒久ファイルを作らない

## 概観の入口

- ライブ現在地と次の入口: `docs/HANDOFF.md`
- 安定した全体像: `docs/PROJECT_OVERVIEW.md`
- 長期の開発軸: `docs/project-context.md`
- スクリーンショット配置: `docs/VISUAL_PROGRESS_INDEX.md`

Dashboard、Turn Plan、Project Cockpit に状態を複製する旧方式は 2026-07-10 に廃止した。現在地は `HANDOFF.md` だけを更新する。

## 分類方針

- `Overview`: 入口、プロジェクト概要、読み始めの案内
- `Runtime State`: 再開状態、運用ルール、handoff、現在位置
- `Specs`: StorySpec、UI / Save / Engine など仕様寄りの正本
- `Development Notes`: AI ルール、制作フロー、計画、改善候補
- `Artifacts`: 検証記録、テンプレート、性能計測、証跡
- `Misc`: 外部パッケージ README など、分類の確信が低い補助資料

分類は閲覧用の仮置きです。文書の正本性や優先順位を新しく決めるものではありません。

## 起動手順

Windows PowerShell:

```powershell
pip install mkdocs-material
.\tools\generate-doc-nav.ps1 -PrepareView
python -m mkdocs serve -a 127.0.0.1:8000
```

ブラウザで `http://127.0.0.1:8000/` を開きます。Chrome / Edge のページ翻訳、または DeepL 拡張で表示中ページを一時翻訳して確認してください。

## 注意

- `.mkdocs-view/` と `.mkdocs-site/` はローカル生成物で、正本ではありません
- 既存 Markdown の配置と本文は、閲覧面のために移動・要約・翻訳しません
- ナビ候補を確認したい場合は `.\tools\generate-doc-nav.ps1` を実行してください
