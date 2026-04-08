# PlayMode 8 件 — 実行結果（回帰ベースライン）

EN-012 / SUBSEQUENT 用の実行記録。`runtime-state.md` の session 22 実測を再掲し、次回再実行時の比較基準にする。

## メタ

- 実行日時: 2026-04-03（session 22）
- Unity（`ProjectVersion.txt`）: `6000.3.6f1`
- ブランチ / コミット: `main` / `fc90374` 以前の session 22 実行点
- 実行方法: Unity Test Runner（PlayMode）+ batch XML 出力経路整備済み

## 結果

- 合計: 8 / 8 passed
- 失敗したテスト名: なし

## ログパス

- `.xml`: batch XML 出力経路は `Assets/Scripts/Editor/TestRunnerHelper.cs` で整備済み（当該 session の添付実ファイルは未保管）
- `.txt` / コンソールログ: `runtime-state.md` session 22 記録を参照

## 備考

- 本記録は「最新取得済みの回帰ベースライン」。次回の好機再実行では本ファイルを複製し、当日の `.xml` / `.txt` 実ファイルパスを追記する。
- 実行環境差（ローカル / CI）は `docs/verification/UNITY_GITHUB_CI.md` で吸収する。

---

## 再実行（2026-04-10 セッション）

| 項目 | 内容 |
|------|------|
| 再実行 | **未実施**（エージェント環境に Unity Editor がないため） |
| オペレーター向け | `TestRunnerHelper.RunPlayModeTestsBatch` または Test Runner 全件。生成 `.txt` / `.xml` のパスを本ファイルに追記。[2026-03-31-playmode-batch-execute.md](2026-03-31-playmode-batch-execute.md) 参照 |
| 関連 | [2026-04-10-subsequent-completion-report.md](2026-04-10-subsequent-completion-report.md) 節 4 |
