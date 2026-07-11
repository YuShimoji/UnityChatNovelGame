# Project Cockpit

最終更新: 2026-07-11

## North Star

FoundPhone を、ライター/デザイナーが Yarn を外部エディタで書き、Unity Editor 上で検証・同期・ノード再生まで短いループで回せるチャット/ビジュアルノベル制作基盤にする。

## Current Active Slice

Writer / Designer Cockpit MVP の interactive 受入。

- Unity menu: `Tools > FoundPhone > Writer Cockpit`
- Main script: `Assets/Scripts/Editor/WriterCockpitWindow.cs`
- 目的: Yarn 保存後の `Refresh Nodes` / `Validate Then Sync` / `Apply` / `Play` / Last Action / 読み取り専用 Save 状態を一画面に集める
- 現在: fresh package resolve と Editor assembly compile は通過。visible menu / window / Apply / Play は未確認
- 範囲外: Yarn本文執筆、Runtime Dashboard/DebugHub のPrefab化、既存セーブデータの書き換え・削除

## Roadmap Strip

```text
G0 development readiness      [#####] local complete
G1 Writer Cockpit acceptance  [####-] active
G1.1/1.2 validator + tests    [##---] next
G2 SP-023/SP-024 display QA   [##---] queued
M1/M2 engine confidence       [#----] later
Mobile build/release          [-----] deferred
```

## Capability Grid

```text
Yarn node discovery       [#####] active file/node count + recommended node
Static validation summary [###--] errors=0; 24 unknown-command warnings are false positives
Authoring SO sync         [#####] pending count + changed/no-change result
ContentAuthoring apply    [####-] shared helper compiled; interactive acceptance pending
Save status confidence    [###--] read-only file presence only
Runtime UI refactor       [-----] intentionally not in this slice
```

## Gate Board

| Gate | State | Requirement |
|------|-------|-------------|
| Fresh package resolve | pass locally | process-local standard Windows environment restoration + 39 packages |
| Cockpit access | batch-ready | Editor assemblies compile; interactive menu still needs a visible Editor pass |
| Existing Content Pipeline | preserved | `Tools > FoundPhone > Content Pipeline` remains available |
| Save safety | guarded | Cockpit only checks save/autosave file presence |
| Unity validation | reproducible locally | `tools/run-unity.ps1` reaches fresh resolve / cache restore / compile / return code 0 |
| Validator trust | needs repair | 33 warnings include 24 registered-command false positives |
| Visual review | deferred | SP-023 / SP-024 display review remains next lane |

## Last Worker Checkpoint

- Added Editor-only Writer Cockpit MVP.
- Added small status DTOs to Yarn validator / SO generator for reuse.
- Shared ContentAuthoring apply/play logic with the existing Content Pipeline window.
- Identified the fresh resolve root cause as missing `ALLUSERSPROFILE` in the calling shell, not malformed package JSON.
- Added `tools/run-unity.ps1` to restore that value for the child process without changing user/system environment.
- Regenerated Package Manager state from no ProjectCache, registered 39 packages, compiled scripts, and reached return code 0.
- Prevented early Editor initialization from moving ContentAuthoring to the end of Build Settings.
- Ran non-mutating Yarn validator batch: `errors=0`, `warnings=33`, `info=3`; warnings are existing content/command diagnostics, not compile or Package Manager failures.
- Detailed trust, residual work, and G0-G13 proposal: `docs/SUPERVISOR_REPORT.md`.
