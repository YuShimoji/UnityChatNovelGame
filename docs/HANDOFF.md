# Handoff

会話ログを読まなくても現状を引き継げるようにするための入口ドキュメント。

## まず読む順番

1. `docs/HANDOFF.md`
2. `docs/project-context.md`
3. `docs/runtime-state.md`
4. `docs/INVARIANTS.md`
5. `docs/USER_REQUEST_LEDGER.md`
6. 実作業に応じて:
   - 制作フロー: `docs/YarnEditingPipeline.md`, `docs/SCENARIO_AUTHORING_GUIDE.md`, `docs/OPERATOR_WORKFLOW.md`
   - UI/挙動: `docs/DISPLAY_ALGORITHMS.md`, `docs/UI_ISSUES.md`
   - 実装/監査: `docs/FEATURE_STATUS_AUDIT.md`, `docs/spec-index.json`

## Current Focus

- 主目的: 人間が Yarn を執筆しやすい制作システムを整備する
- AI の役割: 執筆ではなく、ツール・パイプライン・検証導線の整備
- 現在のボトルネック: 実装した制作フローを Unity 実機でまだ通し切れていない

## Current State

- `YarnSOGenerator` は Topic / Character / Channel の同期まで対応済み
- `Content Pipeline` ウィンドウ追加済み
- `ScenarioManager` は StartNode から CurrentChannel を自動解決する。HasNode 事前チェック追加済み
- `Use Default Node` 固定は廃止し、推奨ノード選択へ変更済み
- PlayMode テスト: 8ケース (SmokeGate 4件 + ScenarioFlow 4件)
- 共通ヘルパー `PlayModeTestHelpers.cs` に分離済み
- teardown DialogueException 対策: `[UnityTearDown]` + `StopScenario()` + 1フレーム待機
- batch 実行時 NUnit XML 出力対応済み
- まだ未実施: Unity PlayMode での通し検証、E2E テストの Unity 上での実行確認

## Recent Session Delta

### session 21 (2026-04-02)

- PlayMode テスト失敗の根本原因特定: teardown の DialogueException ではなく、auto-start の missing_node:Start が原因
- 修正: HasNode 事前チェック / ResolveLikelyBrokenYarnFile archive 除外 / TearDown StopScenario
- teardown 強化: `[TearDown]` → `[UnityTearDown]` (IEnumerator) + `StopScenario()` + 1フレーム待機
- 共通ヘルパー `PlayModeTestHelpers.cs` 分離 (シーンロード、条件待ち、エビデンス、teardown)
- batch XML 出力対応: `ITestResultAdaptor.ToXml()` → `.txt` + `.xml` 両ファイル生成
- PlayMode テストケース追加 (4件 → 8件): ScenarioFlowPlayModeTests.cs 新規 (ETK_Commands, ETK_RichText, Ch2_Opening, SaveLoad 3連サイクル)
- WORKFLOW_STATE_SSOT.md 廃止 (HANDOFF.md に一本化)
- Assets/_Recovery/ (クラッシュリカバリ残骸) 削除
- spec-index 更新: EN-012 pct 40% → 60%
- **残件**: DebugChatScene Inspector で m_StartNode を Start → DQT_Start に手動変更が必要

### session 20

- PlayMode batch 起動経路追加 (`-executeMethod`)
- SaveManager `GetCurrentNodeName()` の "Start" 固定フォールバック廃止

### session 19

- Yarn active/ クリーンアップ (4件 archive 移動)、CanvasScaler 9:16 統一
- DQT_Start PlayMode テスト追加、EN-012 登録

## Safe Next Steps

1. DebugChatScene を開き、ScenarioManager の `m_StartNode` を `Start` → `DQT_Start` に変更し、シーン保存
2. `Tools > FoundPhone > Content Pipeline` で `Sync Authoring Assets` → 制作フロー確認
3. `DQT_Start` / `Ch1_Day1_Opening` / `Ch2_Opening` を再生して通し確認
4. Unity Test Runner で PlayMode テスト8件を実行 (Window > General > Test Runner)
5. 問題が出たら UI 値調整は Inspector、構造バグはコードへ戻す

## Do Not Do Next

- UI 値調整をコード修正として進めない
- 本編 Yarn を実験台にしたエンジン検証をしない
- 「前回の反動」で別レーンへ振れない
- 会話ログだけに依存した handoff を残さない

## Current Trust Assessment

- trusted
  - DebugQuickTest 導線
  - YarnSOGenerator の Topic / Character / Channel 同期
  - StartNode 推奨導線
  - CanvasScaler 9:16 統一 (コード上。DebugChatScene.unity は未再生成)
  - HasNode 事前チェック (auto-start 安全化)
- needs re-check
  - ChannelData 自動同期の Unity 実機結果
  - CurrentChannel 自動解決が Save/Load / EndDay と競合しないこと
  - Content Pipeline window の実運用手順
  - PlayMode テスト8件の Unity 上での実行結果 (auto-start + teardown 修正済み、実行はまだ)
- dangerous / rollback candidate
  - なし

## Open Risks

- Unity 実機未確認のため、Editor 拡張の挙動はコードレビュー止まり
- `ChatController` のステータスバールーティング TODO は未解決
- `verification/` と E2E PlayMode は未整備 (テストコードはあるが実行結果なし)
- DebugChatScene.unity の CanvasScaler は 1920x1080 のまま (シーン再生成で修正要)

## Source Of Truth

- 方針・優先レーン: `docs/project-context.md`
- 直近の作業状態: `docs/runtime-state.md`
- 非交渉条件: `docs/INVARIANTS.md`
- ユーザー要求の継続事項: `docs/USER_REQUEST_LEDGER.md`
- 実際の制作フロー: `docs/OPERATOR_WORKFLOW.md`

## Canonical Gaps

- `docs/FEATURE_REGISTRY.md`: 不在
- `docs/AUTOMATION_BOUNDARY.md`: 不在
- 現時点で今回の制約・痛点は既存 canonical docs に書き戻し済み
