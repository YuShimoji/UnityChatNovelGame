# TASK_029_FixAssemblyDefinitions

Status: DONE

## Objective
asmdef 競合と破損 JSON を解消し、以後のビルドと検証を安定して進められる状態を確立する。

## Context
- 当初は `ProjectFoundPhone.Tests.asmdef` の conflict marker と無効 JSON がコンパイルを阻害していた。
- `Assets/Scripts` 直下の asmdef 構成も揺れており、runtime asmdef の基点整理が必要だった。
- その後の `TASK_049` / `TASK_053` / `TASK_054` までビルドと batch verification が継続成功しているため、本タスクの意図は後続成果で実証済み。

## Final State
- `Assets/Scripts/ProjectFoundPhone.asmdef` が runtime の基点として残っている。
- `Assets/Scripts/Tests/ProjectFoundPhone.Tests.asmdef` は有効 JSON で、`ProjectFoundPhone` 参照に整流されている。
- 補助 asmdef は `Editor` / `Tests` / `Utils` などの限定用途に整理され、root 競合は再発していない。

## Evidence
- `Assets/Scripts/Tests/ProjectFoundPhone.Tests.asmdef`
- `docs/reports/REPORT_TASK_029_FixAssemblyDefinitions.md`
- `docs/reports/REPORT_TASK_049_BuildGateFix_VerticalSlice.md`
- `docs/reports/REPORT_TASK_053_MVPFinalVerificationPack.md`

## DoD
- [x] `ProjectFoundPhone.Tests.asmdef` が有効 JSON である。
- [x] runtime asmdef の基点が `Assets/Scripts/ProjectFoundPhone.asmdef` に統一されている。
- [x] asmdef 競合が後続の build / verification を阻害していない。
- [x] 本タスクの意図が後続タスクの成功で追跡可能になっている。
