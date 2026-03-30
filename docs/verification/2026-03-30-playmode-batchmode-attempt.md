# 2026-03-30 PlayMode Batchmode Attempt

## Scope

- 目的: `VerticalSliceSmokeGatePlayModeTests.DebugChatScene_Ch1Start_PreservesCurrentChannelAcrossSaveLoad`
  の batchmode 実行確認
- Unity: `6000.3.6f1`
- 対象プロジェクト: `UnityChatNovelGame`

## Added Coverage

- `Assets/Scripts/Tests/PlayMode/VerticalSliceSmokeGatePlayModeTests.cs`
  に `Ch1_Day1_Opening` 開始時の `CurrentChannelID == "ch1"` 自動割り当てと、
  Save/Load 後の `CurrentChannel` 復元を確認する PlayMode テストを追加。

## Command

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.3.6f1\Editor\Unity.exe' `
  -batchmode -nographics `
  -projectPath 'C:\Users\thank\Storage\Game Projects\UnityChatNovelGame' `
  -runTests -testPlatform PlayMode `
  -testResults 'C:\Users\thank\Storage\Game Projects\UnityChatNovelGame\Temp\playmode-test-results.xml' `
  -logFile 'C:\Users\thank\Storage\Game Projects\UnityChatNovelGame\Temp\playmode-test.log' `
  -testFilter 'ProjectFoundPhone.Tests.VerticalSliceSmokeGatePlayModeTests.DebugChatScene_Ch1Start_PreservesCurrentChannelAcrossSaveLoad' `
  -quit
```

## Result

- 2 回試行。
- どちらも batchmode 自体は return code `0` で終了。
- ただし `Temp/playmode-test-results.xml` は未生成。
- `Temp/playmode-test.log` 上は asset import / script compile の後に
  `Batchmode quit successfully invoked` で終了しており、テスト開始ログや NUnit 結果は確認できなかった。

## Current Assessment

- 追加した PlayMode テストコード自体は repo に反映済み。
- ただし現環境では Unity batchmode の `-runTests` 経路が成立していないため、
  自動検証の実ラン結果は未取得。
- 次は `runTests` が XML を出さない理由の切り分けが必要。
