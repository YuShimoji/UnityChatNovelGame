# F レーン（Audit / Evidence）— 検証・CI 証跡索引

**作成日**: 2026-04-09  
**レーン状態**: **完了**（2026-04-09）。本開発の優先順位は `docs/project-context.md` の CURRENT LANE に従う。  
**目的**: 次セッションが **同じ前提**（PlayMode ベースライン、手動ハンズオン、静的整合、CI 導線）で再開できるよう、正典へのポインタを 1 ファイルに集約する。

## 本セッションで実施した監査（エージェント環境）

| 項目 | 結果 |
|------|------|
| `docs/spec-index.json` | 配列要素数 **41**（`python -c "len(json.load(...))"` で実測） |
| `status` 内訳 | done 24 / partial 10 / todo 5 / draft 2（合計 41） |
| Unity Editor / PlayMode 再実行 | **未実施**（本環境に Editor なし。ベースラインは下記既存記録を SSOT とする） |

## PlayMode 回帰（8 件）

| 文書 | 内容 |
|------|------|
| [2026-04-09-playmode-8-results.md](2026-04-09-playmode-8-results.md) | session 22 時点 **8/8 passed** の回帰ベースライン、再実行手順・未実施メモ |
| [2026-03-31-playmode-batch-execute.md](2026-03-31-playmode-batch-execute.md) | `-executeMethod` 経路での batch 実行メモ |
| [2026-03-30-playmode-batchmode-attempt.md](2026-03-30-playmode-batchmode-attempt.md) | `-runTests` で XML 未生成となった事例（環境差の参照用） |

実装側の入口: `Assets/Scripts/Editor/TestRunnerHelper.cs`（batch XML / `.txt` 出力）。

## GitHub Actions（EditMode / PlayMode）

| 文書 | 内容 |
|------|------|
| [UNITY_GITHUB_CI.md](UNITY_GITHUB_CI.md) | ワークフロー路径、シークレット前提、`unityVersion` と `ProjectVersion.txt` の整合 |

## Ch1 / SUBSEQUENT / ギャップ

| 文書 | 内容 |
|------|------|
| [2026-04-10-ch1-day1-3-preflight.md](2026-04-10-ch1-day1-3-preflight.md) | Day1〜3 静的整合・Editor 通しメモ |
| [SUBSEQUENT_playthrough_and_tests.md](SUBSEQUENT_playthrough_and_tests.md) | 通し手動確認とテストのチェックリスト |
| [2026-04-10-subsequent-completion-report.md](2026-04-10-subsequent-completion-report.md) | SUBSEQUENT 正本（再現手順・分岐表） |
| [2026-04-08-ch1-subquest-gap-template.md](2026-04-08-ch1-subquest-gap-template.md) | サブクエストギャップ P0/P1/P2 用 |

## 再開時の読書順（F レーン推奨）

1. `docs/HANDOFF.md`（Handoff snapshot / Current Focus）  
2. `docs/project-context.md`（CURRENT LANE / SLICE）  
3. 本ファイル → 上表から必要な verification のみ開く  

## 正典との整合（2026-04-09）

- `docs/runtime-state.md` の **spec_entries** は **`docs/spec-index.json` の要素数（41）** と一致させる（従来の 42 は誤記と判断し同期済み）。
- `docs/FEATURE_STATUS_AUDIT.md` §1 の仕様エントリ行、ルート `CLAUDE.md` 技術サマリの spec-index 件数を **41** に揃えた。
