# SUBSEQUENT 完了レポート（手順確定 + 静的整合 + 分岐）

**目的**: [SUBSEQUENT_playthrough_and_tests.md](SUBSEQUENT_playthrough_and_tests.md) の DoD を、**再現手順の正本化**と**リポジトリ静的検証**まで完了させ、**Editor 実測後の分岐**（Ch2 前進 vs 短い P0 スライス）を一文で選べる状態にする。

**実施日**: 2026-04-10（エージェントセッション）  
**ブランチ / コミット**: `main` / `dadc0ba`  
**Unity（期待）**: `6000.3.6f1`（[ProjectSettings/ProjectVersion.txt](../../ProjectSettings/ProjectVersion.txt) と一致）

---

## 1. 前提固定（再開ゲート）

読了・整合対象:

1. [docs/HANDOFF.md](../HANDOFF.md)
2. [docs/project-context.md](../project-context.md)
3. [docs/runtime-state.md](../runtime-state.md)
4. [docs/verification/2026-04-10-ch1-day1-3-preflight.md](2026-04-10-ch1-day1-3-preflight.md) 節 A（静的）

---

## 2. 静的整合（リポジトリ・本セッションで実施）

[2026-04-10-ch1-day1-3-preflight.md](2026-04-10-ch1-day1-3-preflight.md) 節 A と一致することを再確認した。


| チェック                                                                       | 結果                                                         |
| -------------------------------------------------------------------------- | ---------------------------------------------------------- |
| [ch1.asset](../../Assets/Resources/Channels/ch1.asset) `m_TotalDays`       | **3**                                                      |
| `m_DayStartNodeNames`                                                      | `Ch1_Day1_Opening`, `Ch1_Day2_Opening`, `Ch1_Day3_Opening` |
| [Ch1_Day1.yarn](../../Assets/Resources/Yarn/active/Ch1_Day1.yarn) `title:` | Day1〜3 の Hub / スポーク / Winding / End を含む                    |
| `<<EndDay 1>>` / `2` / `3`                                                 | 各 Day 終端に存在                                                |


**解釈**: Yarn 上の到達可能グラフに明らかな欠落は見えない。**進行不能・Save/Load 破綻はランタイム検証が必要**（Editor 実測）。

---

## 3. Ch1 手動通し（再現手順・正本）

以下は [SUBSEQUENT_playthrough_and_tests.md](SUBSEQUENT_playthrough_and_tests.md) / [HANDOFF.md](../HANDOFF.md) と同じ流れを、1 本の手順書にまとめたもの。

1. Unity でプロジェクトを開く（バージョンは `ProjectVersion.txt` 参照）。
2. `Tools > FoundPhone > Content Pipeline` → **Sync Authoring Assets**（エラーが出たら解消まで）。
3. `ContentAuthoring` シーンを開く。`ScenarioManager` の **Start Node** = `Ch1_Day1_Opening`（[ch1.asset](../../Assets/Resources/Channels/ch1.asset) の `m_StartNodeName` と一致）。
4. Play で **Day1 → EndDay 1 → Day2 → EndDay 2 → Day3 → EndDay 3** まで通す。
5. サイドバーで SP-022 系（`scout_`* / `annot_*` / `ch1_cond_analysis` 等）を開き、メッセージが期待どおりか目視。
6. Day3: `scout_ch1_d3_route` / `scout_ch1_d3_board` / `annot_ch1_d3_compare` が Hub 必須トピック経由で現れ、`CompleteThread` 後に破綻しないか。
7. （推奨）Save → Load でスレッド状態が破綻しないか。
8. UI 気づき → [docs/UI_ISSUES.md](../UI_ISSUES.md)。進行不能・仕様ギャップ → [22_subquest_exploration_content.md](../StorySpec/22_subquest_exploration_content.md) §6。

### 手動通し結果（Editor）


| 項目       | 結果                                                   |
| -------- | ---------------------------------------------------- |
| 実施       | **本セッションでは未実施**（エージェント実行環境に Unity Editor が存在しない）     |
| オペレーター向け | 上記手順を実施後、本表の下に **実施日・Console 重大エラー有無・P0 有無** を追記すること |


---

## 4. PlayMode 8 件（再実行）


| 項目                 | 結果                                                                                                                                                     |
| ------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 本セッションでの再実行        | **未実施**（同上）                                                                                                                                            |
| 既定の batch 出力先（コード） | `TestRunnerHelper.RunPlayModeTestsBatch` → `-ProjectFoundPhoneResultFile=` で指定。未指定時は `docs/verification/playmode-batch-result.txt`（同一ベース名に `.xml` も生成） |
| 実行コマンド例            | [2026-03-31-playmode-batch-execute.md](2026-03-31-playmode-batch-execute.md) の **Command**（フィルタを外せば全 PlayMode を対象にできる）                                 |
| 回帰ベースライン           | [2026-04-09-playmode-8-results.md](2026-04-09-playmode-8-results.md)（session 22 由来 8/8）                                                                |


**オペレーター向け**: 再実行後、`playmode-batch-result.txt` / `.xml` の**実パス**を [2026-04-09-playmode-8-results.md](2026-04-09-playmode-8-results.md) に追記するか、日付付きファイルを新設する。

---

## 5. ギャップ（SP-022 / テンプレ）

転記先:

- [22_subquest_exploration_content.md](../StorySpec/22_subquest_exploration_content.md) §6.4（更新済みの優先度表）
- [2026-04-08-ch1-subquest-gap-template.md](2026-04-08-ch1-subquest-gap-template.md)

本セッションでは **Editor 未実施のため、新規 G-ID の実測追記はなし**（静的整合のみ）。

---

## 6. 分岐決定（Ch2 前進 vs 短い P0）

[03a_ch1_section_beats.md](../StorySpec/03a_ch1_section_beats.md) の「LATER 接続 OK 条件」および [LATER_CH2_PLAYBOOK.md](../StorySpec/LATER_CH2_PLAYBOOK.md) に照らした**確定ルール**。


| 条件                                          | 次スライス                                                                 |
| ------------------------------------------- | --------------------------------------------------------------------- |
| Editor 手動通しで **P0 なし**（進行不能・Save/Load 破綻なし） | **LATER**: `Ch2_LocationConfusion.yarn` 等で Ch2 本編＋サブ前進（P1/P2 は繰り上げない） |
| **P0 あり**                                   | **短いスライス**: 仕様確定＋必要なら最小実装のみ。終了後に再度 SUBSEQUENT 判定                      |


**本セッション時点の判定**: Editor 未実施のため **「未確定（実機で上表を適用）」**。静的検証のみでは **P0 の有無は断定しない**。

---

## 7. 副次（SP-017）

[17_unlock_triggers.md](../StorySpec/17_unlock_triggers.md) §6 に Ch1 具体例が既にある。追加執筆は不要。実機での挙動差分が出たら §6 表の脚注で追記する。