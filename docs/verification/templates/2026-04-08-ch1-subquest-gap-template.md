# Ch1 サブクエスト通し検証 — ギャップ記録テンプレ（SUBSEQUENT）

**目的**: `docs/HANDOFF.md` の手動ハンズオン後、`docs/StorySpec/22_subquest_exploration_content.md` §6 のギャップに **P0 / P1 / P2** を付ける。  
**PlayMode**: SUBSEQUENT 発動時に 8 件スイート再実行、日付付き別ファイル (`YYYY-MM-DD-playmode-results.md`) で記録 (EN-012)。

## 検証環境

- Unity 版: （Editor 実施時に記入）期待: `ProjectSettings/ProjectVersion.txt` と一致する版
- ブランチ / コミット: `main` / `bc2bb9d`（2026-04-09 Bレーン記録時の HEAD）
- StartNode（ContentAuthoring）: `Ch1_Day1_Opening`（[ch1.asset](../../Assets/Resources/Channels/ch1.asset) と一致）

**メモ（2026-04-10）**: 手順・静的結果の正本は [2026-04-10-subsequent-completion-report.md](../2026-04-10-subsequent-completion-report.md)。下記チェックリストは **オペレーター実測後**に埋める。

**メモ（2026-04-09・Bレーン）**: [2026-04-09-blane-sp022-subsequent.md](../../archive/verification-lanes-2026-04/2026-04-09-blane-sp022-subsequent.md) を参照。リポジトリ静的整合は再確認済み。**Editor 手動チェックと PlayMode batch は未実施**（同一プロジェクトを別 Unity が開いており batch が拒否されたため）。チェックリストの `[ ]` はそのまま。

## 手動プレイ結果（Ch1 + サブ）

- [ ] Day1 必須 4 トピック通過
- [ ] 任意: `scout_ch1_network`（C）
- [ ] 任意: `annot_ch1_glossary`（A）
- [ ] Day2 モック + 任意: `scout_ch1_day2_ping`（C）
- [ ] Day3: `scout_ch1_d3_route` / `scout_ch1_d3_board` / `annot_ch1_d3_compare`（設計どおり Manifest〜Complete）
- [ ] `ch1_note_facility` / `ch1_cond_analysis` の出現タイミング

## ギャップ優先度（SP-022 §6 へ転記用）

| ID | 概要 | P0 / P1 / P2 | 種別（要仕様 / 要実装） |
|----|------|----------------|-------------------------|
| G-001 | （例）解放通知が分かりにくい | | 要仕様 |
| G-002 | | | |
| G-ENV-20260409 | PlayMode batch / Editor 通しが **未実施**（プロジェクト排他ロック）。再現手順・ログは [2026-04-09-blane-sp022-subsequent.md](../../archive/verification-lanes-2026-04/2026-04-09-blane-sp022-subsequent.md) §3 | （環境・優先度なし） | 運用メモ |
| G-OBS-20260409 | Yarn 上、`ch1_note_facility` / `ch1_cond_analysis` に `CompleteThread` が無い。意図的か・Save/Load で問題が出ないかは SUBSEQUENT 発動時に実測 | 未判定 (実測で P0〜P2 決定) | 実測事項 |

**定義の目安**

- **P0**: 進行不能、データ破綻、クラッシュに直結
- **P1**: 体験品質・オーサリング摩擦が高いが回避可能
- **P2**: 改善希望・後回しでよい

## PlayMode 8 件（好機・追記欄）

- 実行日時:
- 結果: 通過 / 失敗
- ログパス（.xml / .txt）:
