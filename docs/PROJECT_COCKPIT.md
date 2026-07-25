# Project Cockpit

最終更新: 2026-07-26

## North Star

FoundPhone を、ライター/デザイナーが Yarn を外部エディタで書き、Unity Editor 上で検証・同期・ノード再生まで短いループで回せるチャット/ビジュアルノベル制作基盤にする。

## Current Active Slice

2026-07-26のremote parityと開発環境再検証を起点に、実セーブを保護した基礎83件の現行回帰を作る。Writer Cockpit full loopとOwner-only Sites本文は人間が並行受入する。

- Unity menu: `Tools > FoundPhone > Writer Cockpit`
- Main script: `Assets/Scripts/Editor/WriterCockpitWindow.cs`
- 作者導線: 74 Node検索 → source path/line → structured diagnostics → source jump。既存`Refresh` / `Validate Then Sync` / `Apply` / `Play`を維持
- 現在: targeted EditMode 14/14、Unity compile、active Yarn `errors=0 / warnings=0 / info=3`がpass。External Script Editorでの実jumpは未設定環境のためhuman review debt
- 公開面: `sites/foundphone-demo/`はportableなtracked input。Sites Version 1 / source `29c5245d...`はOwner-onlyでhost済み。次はOwner sign-in後の実hosted本文review
- 範囲外: Yarn本文執筆、Runtime Dashboard/DebugHub、既存save data変更、Unity module導入、public/shared access、custom domain

## Roadmap Strip

```text
G0 development readiness      [#####] local complete
G1 source navigation lane     [#####] accepted
G1.1 validator trust          [#####] active Yarn warnings 0
G1.2 current regression       [##---] 97 defined; base 83 save isolation pending
P1 Sites-native demo          [####-] private hosted runtime; Owner review pending
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
Sites-native demo         [####-] Owner-only Version 1; local runtime QA pass, hosted body review pending
Direct Unity Web          [-----] module and public gameplay route blocked
Runtime UI refactor       [-----] intentionally not in this slice
```

## Gate Board

| Gate | State | Requirement |
|------|-------|-------------|
| Remote parity / compile | pass locally 2026-07-26 | `73aef720` parity 0/0、39 packages、Tundra success |
| Cockpit source navigation | accepted | Node search/source line/diagnostic drilldown checked; targeted tests 14/14 |
| External editor jump | human review debt | implementation fails closed; Unity External Script Editor must be selected by human |
| Existing Content Pipeline | preserved | `Tools > FoundPhone > Content Pipeline` remains available |
| Save safety | guarded | Cockpit only checks save/autosave file presence |
| Unity validation | reproducible locally | `tools/run-unity.ps1` reaches fresh resolve / cache restore / compile / return code 0 |
| Validator trust | pass for active Yarn | runtime handlers / CharacterProfile registry; errors 0 / warnings 0 / info 3 |
| Direct Unity Web | blocked | Web Build Support absent; valid public gameplay scene/navigation absent |
| Sites-native publication | private hosted / Owner review debt | Version 1、Owner-only access、deployment success。hosted本文はsign-in後review待ち |
| Visual review | deferred | SP-023 / SP-024 display review remains next lane |

## Last Worker Checkpoint

- Integrated accepted editor commit from base `55cb0d20`: Node search, source path/title line, external-editor action, structured Validator drilldown.
- Replaced duplicated Validator command/character lists with runtime handler and CharacterProfile asset discovery. Active Yarn is `errors=0 / warnings=0 / info=3`.
- Revalidated the integrated tree with targeted EditMode 14/14, Unity 6000.4.9f1 batch compile, and non-mutating Yarn validator.
- Preserved the existing Content Pipeline Apply/Play helpers and read-only Save status; current definitions are base 83 + targeted Editor 14 = 97, with base 83 gated by save-data isolation.
- Integrated `docs/verification/sites-publication-feasibility.md`. Direct Unity Web remains unproven and blocked; Sites-native lightweight demo is the bounded successor.
- Integrated `sites/foundphone-demo/` and `tools/sites/` from a read-only sibling with exact pre-repair SHA-256 parity. Local HTTP, both routes, restart, desktop/mobile/narrow, accessibility smoke, prohibited-pattern audit passed.
- Sites Version 1をOwner-onlyでhostし、source/version/access/deploymentを再取得。local runtimeではactual React/Worker build、keyboard、両分岐、responsiveを確認済み。
- Next publication unlockはOwner sign-in後のhosted本文review。public/shared accessとcustom domainは未承認・未実施。
- Detailed trust, residual work, and successor boundaries: `docs/SUPERVISOR_REPORT.md`.
