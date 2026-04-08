# SUBSEQUENT: 手動通しプレイ + PlayMode 記録（チェックリスト）

`docs/project-context.md` の **SUBSEQUENT RECOMMENDED SLICE** 用。Unity Editor で実施し、結果をこのファイルまたは日付付き別名で `docs/verification/` に残す。

## 1. Ch1 メイン + サブ（手動）

前提: `ContentAuthoring.unity`、ScenarioManager の **Start Node** が `Ch1_Day1_Opening`（または検証したい Day の開始ノード）。

- [ ] `Tools > FoundPhone > Content Pipeline` → **Sync Authoring Assets**（エラーなし）
- [ ] Play で Ch1 Day1 → Day2 → Day3 まで通し（`EndDay` まで）
- [ ] サイドバーで SP-022 系スレッド（`scout_*` / `annot_*` / `ch1_cond_analysis` 等）を開き、メッセージが期待どおりか目視
- [ ] Save → Load（任意）でスレッド状態が破綻しないか
- [ ] 新規 UI 気づき → `docs/UI_ISSUES.md`（コード修正はバッチまで）

## 2. ギャップの P0 / P1 / P2 付け

`docs/StorySpec/22_subquest_exploration_content.md` §6 または別表に転記:

| ID / 説明 | 優先 | メモ |
|-----------|------|------|
| （例） | P0 / P1 / P2 | プレイヤーに見える挙動を 1 段落 |

## 3. PlayMode 8 件（好機）

Unity Test Runner → PlayMode 全実行、または batch（`TestRunnerHelper` / `-executeMethod`）。

- [ ] 8 passed / 0 failed
- [ ] 生成物: `TestResults_*.xml` / `.txt` 等をこのフォルダにコピーし、本ファイル末尾に **実行日・Unity 版・ブランチ**を記載

### 実行記録（追記用）

- 日付:
- Unity:
- ブランチ / コミット:
- 結果サマリ:
