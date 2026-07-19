# Project Cockpit

最終更新: 2026-07-19

## North Star

FoundPhone を、ライター/デザイナーが Yarn を外部エディタで書き、Unity Editor 上で検証・同期・ノード再生まで短いループで回せるチャット/ビジュアルノベル制作基盤にする。

## Current Active Slice

Writer Cockpit source navigationの受入済み状態を維持し、公開面はSites-native lightweight chat demoの別laneへ進める。

- Unity menu: `Tools > FoundPhone > Writer Cockpit`
- Main script: `Assets/Scripts/Editor/WriterCockpitWindow.cs`
- 作者導線: 74 Node検索 → source path/line → structured diagnostics → source jump。既存`Refresh` / `Validate Then Sync` / `Apply` / `Play`を維持
- 現在: targeted EditMode 14/14、Unity compile、active Yarn `errors=0 / warnings=0 / info=3`がpass。External Script Editorでの実jumpは未設定環境のためhuman review debt
- 公開面: direct Unity Web routeはmodule/public scene/navigation不足でblocked。Sites-native lightweight chat demoがactive successor
- 範囲外: Yarn本文執筆、Runtime Dashboard/DebugHub、既存save data変更、Unity module導入、Site公開

## Roadmap Strip

```text
G0 development readiness      [#####] local complete
G1 source navigation lane     [#####] accepted
G1.1 validator trust          [#####] active Yarn warnings 0
G1.2 full regression          [##---] save isolation pending
P1 Sites-native demo          [#----] successor; separate lane
P2 direct Unity Web route     [-----] blocked
G2 SP-023/SP-024 display QA   [##---] queued
M1/M2 engine confidence       [#----] later
Mobile build/release          [-----] deferred
```

## Capability Grid

```text
Yarn node discovery       [#####] 74 nodes + search + source path/title line
Structured diagnostics    [#####] severity/file/line/message + source action
Validator registry trust  [#####] errors=0 / warnings=0 / info=3 on active Yarn
Authoring SO sync         [#####] pending count + changed/no-change result
ContentAuthoring apply    [####-] shared helper compiled; interactive acceptance pending
Save status confidence    [###--] read-only file presence only
Sites-native demo         [#----] successor selected; artifact not built
Direct Unity Web          [-----] module and public gameplay route blocked
Runtime UI refactor       [-----] intentionally not in this slice
```

## Gate Board

| Gate | State | Requirement |
|------|-------|-------------|
| Fresh package resolve | pass locally | process-local standard Windows environment restoration + 39 packages |
| Cockpit source navigation | accepted | Node search/source line/diagnostic drilldown checked; targeted tests 14/14 |
| External editor jump | human review debt | implementation fails closed; Unity External Script Editor must be selected by human |
| Existing Content Pipeline | preserved | `Tools > FoundPhone > Content Pipeline` remains available |
| Save safety | guarded | Cockpit only checks save/autosave file presence |
| Unity validation | reproducible locally | `tools/run-unity.ps1` reaches fresh resolve / cache restore / compile / return code 0 |
| Validator trust | pass for active Yarn | runtime handlers / CharacterProfile registry; errors 0 / warnings 0 / info 3 |
| Direct Unity Web | blocked | Web Build Support absent; valid public gameplay scene/navigation absent |
| Sites-native publication | successor only | separate lane and local review artifact required; no Site has been published |
| Visual review | deferred | SP-023 / SP-024 display review remains next lane |

## Last Worker Checkpoint

- Integrated accepted editor commit from base `55cb0d20`: Node search, source path/title line, external-editor action, structured Validator drilldown.
- Replaced duplicated Validator command/character lists with runtime handler and CharacterProfile asset discovery. Active Yarn is `errors=0 / warnings=0 / info=3`.
- Revalidated the integrated tree with targeted EditMode 14/14, Unity 6000.4.9f1 batch compile, and non-mutating Yarn validator.
- Preserved the existing Content Pipeline Apply/Play helpers and read-only Save status; full 83 tests remain gated by save-data isolation.
- Integrated `docs/verification/sites-publication-feasibility.md`. Direct Unity Web remains unproven and blocked; Sites-native lightweight demo is the bounded successor.
- Detailed trust, residual work, and successor boundaries: `docs/SUPERVISOR_REPORT.md`.
