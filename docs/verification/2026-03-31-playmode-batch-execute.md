# 2026-03-31 PlayMode Batch Execute

## Goal

- `CurrentChannel` Save/Load PlayMode テストを batch 実行で再現し、
  手動確認なしで失敗点を特定する。

## What Changed

- `Assets/Scripts/Editor/TestRunnerHelper.cs`
  に `RunPlayModeTestsBatch()` を追加。
- custom args:
  - `-ProjectFoundPhoneTestFilter=...`
  - `-ProjectFoundPhoneResultFile=...`
- `Assets/Scripts/Core/SaveManager.cs`
  の `GetCurrentNodeName()` を修正し、
  `$current_node` 不在時は `"Start"` 固定ではなく
  `CurrentChannel -> ChannelData.StartNodeName -> ScenarioManager.DefaultStartNode`
  の順でフォールバックするようにした。
- `Assets/Scripts/Core/ScenarioManager.cs`
  に `DefaultStartNode` を追加。

## Command

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.3.6f1\Editor\Unity.exe' `
  -batchmode -nographics `
  -projectPath 'C:\Users\thank\Storage\Game Projects\UnityChatNovelGame' `
  -executeMethod ProjectFoundPhone.Editor.TestRunnerHelper.RunPlayModeTestsBatch `
  '-ProjectFoundPhoneTestFilter=ProjectFoundPhone.Tests.VerticalSliceSmokeGatePlayModeTests.DebugChatScene_Ch1Start_PreservesCurrentChannelAcrossSaveLoad' `
  '-ProjectFoundPhoneResultFile=C:\Users\thank\Storage\Game Projects\UnityChatNovelGame\docs\verification\playmode-batch-result.txt' `
  -logFile 'C:\Users\thank\Storage\Game Projects\UnityChatNovelGame\Temp\playmode-batch-execute.log'
```

## Findings

- `-executeMethod` 経路で PlayMode テスト本体の起動には成功。
- 初回失敗要因:
  - `SaveManager` が `$current_node` 不在時に `"Start"` を保存し、
    Load 時に存在しない `Start` ノードを要求していた。
- 修正後:
  - `SaveManager: Game loaded from slot 0` まで進行し、
    `"Start"` 起因の `missing_node:Start` は解消。
- 現在の残失敗:
  - teardown 周辺で `DialogueException: Cannot continue running dialogue. No node has been selected.`
  - evidence:
    - `docs/verification/VerticalSliceSmokeGate_20260331_044945_DebugChatScene.txt`
    - `Temp/playmode-batch-execute.log`

## Current Interpretation

- 手動確認の前段ブロッカーだった「batch で PlayMode テストを起動できない」は大きく緩和。
- いまの主問題は `StartWait` / `OnDestroy` / `DialogueRunner` 停止境界での
  teardown 例外で、実ランの最後だけが不安定。
