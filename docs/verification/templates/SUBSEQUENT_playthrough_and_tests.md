# SUBSEQUENT: 手動通しプレイ + PlayMode 記録（チェックリスト）

`docs/project-context.md` の **SUBSEQUENT RECOMMENDED SLICE** 用。Unity Editor で実施し、結果をこのファイルまたは日付付き別名で `docs/verification/` に残す。

**正本（再現手順 + 静的結果 + 分岐ルール）**: [2026-04-10-subsequent-completion-report.md](2026-04-10-subsequent-completion-report.md)（2026-04-10: エージェントは Editor を起動できないため、下記チェックは **手順確定まで**。実プレイはオペレーター実施後に本ファイルまたは同レポートへ追記）

## 1. Ch1 メイン + サブ（手動）

前提: `ContentAuthoring.unity`、ScenarioManager の **Start Node** が `Ch1_Day1_Opening`（または検証したい Day の開始ノード）。

- [ ] `Tools > FoundPhone > Content Pipeline` → **Sync Authoring Assets**（エラーなし）
- [ ] Play で Ch1 Day1 → Day2 → Day3 まで通し（`EndDay` まで）
- [ ] サイドバーで SP-022 系スレッド（`scout_*` / `annot_*` / `ch1_cond_analysis` 等）を開き、メッセージが期待どおりか目視
- [ ] Day3: `scout_ch1_d3_route` / `scout_ch1_d3_board` / `annot_ch1_d3_compare` が Hub 必須トピック経由で現れ、`CompleteThread` 後の状態が破綻しないか
- [ ] Save → Load（任意）でスレッド状態が破綻しないか
- [ ] 新規 UI 気づき → `docs/UI_ISSUES.md`（コード修正はバッチまで）

## 2. ギャップの P0 / P1 / P2 付け

`docs/StorySpec/22_subquest_exploration_content.md` §6 または別表に転記:

| ID / 説明 | 優先 | メモ |
|-----------|------|------|
| （例） | P0 / P1 / P2 | プレイヤーに見える挙動を 1 段落 |

## 3. PlayMode 8 件（好機）

Unity Test Runner → PlayMode 全実行、または batch（`TestRunnerHelper` / `-executeMethod`）。

- [x] 回帰ベースライン（8 passed / 0 failed）を `runtime-state.md` session 22 から転記
- [x] 記録ファイル作成: `2026-04-09-playmode-8-results.md`
- [ ] 次回再実行時: `TestResults_*.xml` / `.txt` 実ファイルの保存先を追記

### 実行記録（追記用）

- 日付: 2026-04-09（記録日。実行ベースは 2026-04-03 session 22）
- Unity: `6000.3.6f1`
- ブランチ / コミット: `main` / `dadc0ba`（記録時HEAD）
- 結果サマリ: 8 / 8 passed（回帰ベースライン）

詳細テンプレ: [`2026-04-10-playmode-8-results-TEMPLATE.md`](2026-04-10-playmode-8-results-TEMPLATE.md)  
今回の実記録: [`../2026-04-09-playmode-8-results.md`](../2026-04-09-playmode-8-results.md)

## 4. ギャップ転記の例（コピー用）

| ID / 説明 | 優先 | メモ |
|-----------|------|------|
| （例）Day3 Hub 通過後にサブが再 Manifest され進行不能 | P0 | 再現手順 3 行 |
| （例）B 型がクリック不能で誤解を招く | P1 | UI_ISSUES へリンク可 |
| （例）Wiki 本格遷移が欲しい | P2 | SP-022 §6.2・IDEA POOL |
