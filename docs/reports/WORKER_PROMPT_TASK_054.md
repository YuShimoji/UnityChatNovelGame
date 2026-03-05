# Worker Prompt: TASK_054_TitleSceneManagerWiringFix

## 概要
`VerticalSlice_SmokeFlow_TitleToChat_SaveLoad` の FAIL（`TitleScene: TitleScreenManager not found`）を解消し、PlayModeスモークを Green 化する。

## 事前必読（新ルール）
- `docs/02_design/ASSEMBLY_ARCHITECTURE.md`
- `docs/03_guides/UNITY_CODE_STANDARDS.md`
- `docs/03_guides/COMPILATION_GUARD_PROTOCOL.md`

## 現状
- `docs/tasks/TASK_054_TitleSceneManagerWiringFix.md` は `OPEN`。
- `TASK_047` / `TASK_052` は `DONE` だが、PlayMode は 1 PASS / 1 FAIL。
- 失敗証跡: `docs/evidence/TASK_047/PlayModeResults.xml`

## 参照
- チケット: `docs/tasks/TASK_054_TitleSceneManagerWiringFix.md`
- 連動: `docs/tasks/TASK_047_VerticalSliceSmokeGate.md`
- レポート: `docs/reports/REPORT_TASK_047_VerticalSliceSmokeGate.md`
- MISSION_LOG: `.cursor/MISSION_LOG.md`

## 境界
- Target Assemblies:
  - `ProjectFoundPhone`
  - `ProjectFoundPhone.PlayModeTests`
- Focus Area:
  - `Assets/Scenes/TitleScene.unity`
  - `Assets/Scripts/UI/TitleScreenManager.cs`
  - `Assets/Scripts/Editor/BuildSettingsHelper.cs`
  - `Assets/Scripts/Tests/PlayMode/VerticalSliceSmokeGatePlayModeTests.cs`
  - `docs/evidence/TASK_047/`
- Forbidden Area:
  - 新規機能追加
  - 監査範囲外リファクタ

## Test Plan
- テスト対象:
  - `VerticalSliceSmokeGatePlayModeTests`
  - TitleScene の TitleScreenManager 配線
- テスト種別:
  - EditMode（コンパイル確認）
  - PlayMode（CLI）
- 期待結果:
  - `VerticalSlice_SmokeFlow_TitleToChat_SaveLoad` が PASS
  - `docs/evidence/TASK_047/PlayModeResults.xml` で対象2テストが PASS

## DoD
- [ ] 失敗原因を解消し、対象PlayModeテストが PASS
- [ ] `PlayModeResults.xml` を更新し、PASS結果を確認
- [ ] Unity Editor でコンパイルエラー 0 を確認
- [ ] `docs/reports/REPORT_TASK_054_TitleSceneManagerWiringFix.md` を作成
- [ ] チケット `Status` / `DoD` を更新

## 停止条件
- 対象外アセンブリ変更が必要（=> `BLOCKED`）
- シーン破損で復旧手順が必要
- CLI再現不可

## 納品先
- `docs/inbox/REPORT_TASK_054_TitleSceneManagerWiringFix.md`
- （アーカイブ先）`docs/reports/REPORT_TASK_054_TitleSceneManagerWiringFix.md`

## 完了時に更新するファイル
- `docs/tasks/TASK_054_TitleSceneManagerWiringFix.md`
- `docs/reports/REPORT_TASK_054_TitleSceneManagerWiringFix.md`
- `docs/evidence/TASK_047/PlayModeResults.xml`
- `.cursor/MISSION_LOG.md`
