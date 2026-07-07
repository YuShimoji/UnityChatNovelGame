# Project Cockpit

最終更新: 2026-07-07

## North Star

FoundPhone を、ライター/デザイナーが Yarn を外部エディタで書き、Unity Editor 上で検証・同期・ノード再生まで短いループで回せるチャット/ビジュアルノベル制作基盤にする。

## Current Active Slice

Writer / Designer Cockpit MVP。

- Unity menu: `Tools > FoundPhone > Writer Cockpit`
- Main script: `Assets/Scripts/Editor/WriterCockpitWindow.cs`
- 目的: Yarn 保存後の `Refresh Nodes` / `Validate Then Sync` / `Apply` / `Play` / Last Action / 読み取り専用 Save 状態を一画面に集める
- 範囲外: Yarn本文執筆、Runtime Dashboard/DebugHub のPrefab化、既存セーブデータの書き換え・削除

## Roadmap Strip

```text
T+1 Writer Cockpit MVP        [#####] active
T+2 SP-023/SP-024 display QA  [##---] next
T+3 Save/Load confidence      [#----] later
M6 UI batch                   [-----] deferred
Mobile build/release          [-----] deferred
```

## Capability Grid

```text
Yarn node discovery       [#####] active file/node count + recommended node
Static validation summary [####-] counts surfaced; details remain in Validator/Console
Authoring SO sync         [#####] pending count + changed/no-change result
ContentAuthoring apply    [#####] selected node apply/play through shared helper
Save status confidence    [###--] read-only file presence only
Runtime UI refactor       [-----] intentionally not in this slice
```

## Gate Board

| Gate | State | Requirement |
|------|-------|-------------|
| Cockpit access | batch-ready | 2026-07-07 recheck reaches Editor assemblies; interactive menu open still needs a visible Editor pass |
| Existing Content Pipeline | preserved | `Tools > FoundPhone > Content Pipeline` remains available |
| Save safety | guarded | Cockpit only checks save/autosave file presence |
| Unity validation | recovered locally | Package Manager cache/timestamp state restored; batch open reaches return code 0 |
| Visual review | deferred | SP-023 / SP-024 display review remains next lane |

## Last Worker Checkpoint

- Added Editor-only Writer Cockpit MVP.
- Added small status DTOs to Yarn validator / SO generator for reuse.
- Shared ContentAuthoring apply/play logic with the existing Content Pipeline window.
- Recovered the local Package Manager validation path by restoring coherent `Library/PackageManager` cache metadata and matching `Packages/manifest.json` / `Packages/packages-lock.json` timestamps.
- Ran Unity 6000.4.9f1 batch open to Package Manager registration, script compile, and return code 0.
- Ran non-mutating Yarn validator batch: `errors=0`, `warnings=33`, `info=3`; warnings are existing content/command diagnostics, not compile or Package Manager failures.
- Rechecked the same path on 2026-07-07 after remote sync: `Logs/writer-cockpit-unity-open-2026-07-07.log` and `Logs/writer-cockpit-yarn-validator-2026-07-07.log` still pass the local validation loop.
- Fresh Package Manager resolution remains fragile: deleting `packages-lock.json`, clearing `Library/PackageManager`, or changing the manifest re-enters `path undefined`.
- Verification note: `docs/verification/2026-07-06-writer-cockpit-unity-validation.md`.
