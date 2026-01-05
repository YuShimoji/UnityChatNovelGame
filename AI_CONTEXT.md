# AI Context

## 基本情報

- **最終更新**: 2026-01-06T06:45:00+09:00
- **更新者**: AI Agent (Kickstart Setup)

## レポート設定（推奨）

- **report_style**: standard
- **mode**: implementation

## 現在のミッション

- **タイトル**: プロジェクト初期セットアップ
- **Issue**: #0
- **ブランチ**: main
- **関連PR**: なし
- **進捗**: セットアップ中

## 次の中断可能点

- セットアップ完了後

## 決定事項

- `.shared-workflows` をサブモジュールとして導入
- `docs/inbox/` と `docs/tasks/` を Git 管理対象として作成

## リスク/懸念

- 既存の `Docs/` ディレクトリ（大文字）と `docs/` ディレクトリ（小文字）が混在している可能性

## Backlog（将来提案）

- [ ] プロジェクト構造の整理（Docs と docs の統合検討）

## タスク管理（短期/中期/長期）

### 短期（Next）

- [ ] セットアップ完了後の動作確認
- [ ] SSOT ファイルの確認

### 中期（Later）

- [ ] プロジェクト構造の整理

### 長期（Someday）

- [ ] 継続的な運用フローの確立

## 備考（自由記述）

- Unity プロジェクト（ChatNovelGame）の初期セットアップを実施中
- shared-workflows サブモジュールを導入し、以降の Orchestrator/Worker が自律的に動作できる環境を構築

## 履歴

- 2026-01-06 06:45: AI_CONTEXT.md を初期化
