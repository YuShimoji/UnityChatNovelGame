# Bレーン（SP-022 / SUBSEQUENT）検証記録 — 2026-04-09

**目的**: [docs/ai/PARALLEL_LANE_PROMPTS.md](../ai/PARALLEL_LANE_PROMPTS.md) レーン B に沿い、Ch1 サブクエストの到達・整合を確認し、再現手順の正本に接続する。

**コミット（記録時）**: `main` / `bc2bb9d`  
**Unity（期待）**: `6000.3.6f1`（[ProjectSettings/ProjectVersion.txt](../../ProjectSettings/ProjectVersion.txt)）

---

## 1. 実施内容サマリ

| 区分 | 結果 |
|------|------|
| リポジトリ静的整合（ch1 / Yarn / EndDay） | **実施・問題なし**（下記 §2） |
| PlayMode 8 件（batch） | **未実施** — 同一プロジェクトを別 Unity インスタンスが開いており batch 起動が拒否された |
| Editor 手動通し（HANDOFF ハンズオン） | **未実施** — 上記と同じくオペレーター環境で実施要。手順の正本は [2026-04-10-subsequent-completion-report.md](2026-04-10-subsequent-completion-report.md) §3 |

**解釈**: [22_subquest_exploration_content.md](../StorySpec/22_subquest_exploration_content.md) §6.4.1 の区分どおり、**P0 の有無はランタイム実測まで断定しない**。本記録は静的側の追認と、自動検証がブロックされた**環境理由**の証跡。

---

## 2. 静的整合（再確認）

| チェック | 結果 |
|----------|------|
| [ch1.asset](../../Assets/Resources/Channels/ch1.asset) `m_TotalDays` | **3** |
| `m_DayStartNodeNames` | `Ch1_Day1_Opening`, `Ch1_Day2_Opening`, `Ch1_Day3_Opening` |
| `m_StartNodeName` | `Ch1_Day1_Opening` |
| [Ch1_Day1.yarn](../../Assets/Resources/Yarn/active/Ch1_Day1.yarn) `title:` | Day1〜3 の Hub / スポーク / Winding / End を含む |
| `<<EndDay 1>>` / `2` / `3` | 各 Day 終端に存在 |

### SP-022 パイロット ID（Yarn 上の対応）

- **C（CompleteThread あり）**: `scout_ch1_network`, `annot_ch1_glossary`, `scout_ch1_day2_ping`, `scout_ch1_d3_route`, `scout_ch1_d3_board`, `annot_ch1_d3_compare` — いずれも `Declare*` / `Manifest*` と `CompleteThread` の対が取れる。
- **A（断片系）**: `ch1_note_facility` — Latent → Manifest、**Yarn 上に `CompleteThread` なし**（意図またはセーブ影響は実測で確認）。
- **B**: `ch1_cond_analysis` — LatentCond、`AddThreadMessage` のみ。**`CompleteThread` なし**（同上）。
- **分岐**: `ch1_branch_analysis` — `BeginBranch` / `EndBranch`（別表参照）。

---

## 3. PlayMode batch 実行ログ（試行）

1. **初回**: `-quit` 併用によりコンパイル途中で終了し結果ファイル未生成。
2. **再試行**: `Another Unity instance is running with this project open` により **fatal error で中止**。

**オペレーター向け**: 他 Editor を閉じたうえで [2026-03-31-playmode-batch-execute.md](2026-03-31-playmode-batch-execute.md) の形式で `RunPlayModeTestsBatch` を再実行し、`.txt` / `.xml` のパスを [2026-04-09-playmode-8-results.md](2026-04-09-playmode-8-results.md) または日付付きファイルに追記する。

---

## 4. 転記先

- ギャップ表テンプレ: [2026-04-08-ch1-subquest-gap-template.md](2026-04-08-ch1-subquest-gap-template.md)
- SUBSEQUENT 正本: [2026-04-10-subsequent-completion-report.md](2026-04-10-subsequent-completion-report.md)
- SP-022: [22_subquest_exploration_content.md](../StorySpec/22_subquest_exploration_content.md) §6.1 / §6.4.1
