# Project Cockpit

最終更新: 2026-07-06

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
| Cockpit access | ready | `Tools > FoundPhone > Writer Cockpit` appears after Unity compile |
| Existing Content Pipeline | preserved | `Tools > FoundPhone > Content Pipeline` remains available |
| Save safety | guarded | Cockpit only checks save/autosave file presence |
| Unity validation | blocked | Unity 6000.4.9f1 exists, but Package Manager stops with `path undefined` before menu/compile proof |
| Visual review | deferred | SP-023 / SP-024 display review remains next lane |

## Last Worker Checkpoint

- Added Editor-only Writer Cockpit MVP.
- Added small status DTOs to Yarn validator / SO generator for reuse.
- Shared ContentAuthoring apply/play logic with the existing Content Pipeline window.
- Ran Unity 6000.4.9f1 validation attempts; all stopped in Package Manager before Writer Cockpit menu/compile reachability could be proven.
- Verification note: `docs/verification/2026-07-06-writer-cockpit-unity-validation.md`.
