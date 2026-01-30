# Worker Prompt for TASK_044: Structure Cleanup

## 依頼内容
リポジトリ内のディレクトリ構造を整理し、`Docs/` を `docs/` に統合してください。

## コンテキスト
- 現状: `Docs/` と `docs/` が混在。
- ゴール: `docs/` に統一。

## 手順
1. Git コマンドを使用し、`Docs/` 以下の全てのファイルを `docs/` に移動する。
   - `git mv Docs/your-file docs/your-file`
2. `Docs/` フォルダが空になったら削除する。
3. `README.md` や `MISSION_LOG.md` など、ドキュメント内のリンク（`Docs/`）を `docs/` に置換する。
   - VSCode の全置換機能などを活用。
4. 変更をステージングし、コミット準備をする。

## 注意点
- Windows 環境では大文字小文字が区別されないため、`git mv` を使用しないとGitが移動を認識しない場合があります。
- 必ず `git status` で `renamed` になっていることを確認してください。

## 提出物
- 修正されたファイル群
- `docs/reports/REPORT_TASK_044_structure_cleanup.md`
