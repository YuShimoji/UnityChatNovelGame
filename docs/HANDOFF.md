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
- `ScenarioManager` は StartNode から CurrentChannel を自動解決する
- `Use Default Node` 固定は廃止し、推奨ノード選択へ変更済み
- PlayMode テスト: 4ケース (SmokeGate / Ch1SaveLoad / DQTStart / ChoiceImageFallback)
- まだ未実施: Unity PlayMode での通し検証、E2E テストの Unity 上での実行確認

## Recent Session Delta

### session 19

- 堆積物整理
  - Yarn active/ から参照なし4件を archive/ へ移動 (FirstSlice, MVPTest, DebugScript, SubthreadTest)
  - CanvasScaler を MetaEffectController + DebugSceneBuilder で 1080x1920 / matchHeight=1.0 に統一
  - 未コミット Topic .asset 6件をコミット
- E2E 検証スコープ
  - DQT_Start 起動テスト追加
  - EN-012 として spec-index に E2E PlayMode 自動検証を登録 (partial 40%)
- メトリクス修正: impl 66, yarn_active 6, yarn_archive 5, playmode_test_cases 4

### session 18

- 制作フローの手動穴埋め
  - ChannelData 手動作成依存を解消
  - Content Pipeline window を追加
  - 推奨 StartNode 導線を統一
  - StartScenario 時の CurrentChannel 自動同期を追加

### session 17

- 自動スキップバグ修正
- DialogueException 修正
- DebugQuickTest 追加
- フォントバランス調整
- canonical docs 初期化

## Safe Next Steps

1. Unity で `Tools > FoundPhone > Content Pipeline` を開く
2. `Sync Authoring Assets` を実行
3. `DQT_Start` を起点に再生して制作フロー導線を確認
4. `Ch1_Day1_Opening` / `Ch2_Opening` / `Ch3_Day1_Opening` を順に再生
5. Unity Test Runner で PlayMode テスト4件を実行 (Window > General > Test Runner)
6. 問題が出たら UI 値調整は Inspector、構造バグはコードへ戻す
7. 実機確認後に E2E 自動検証の拡張へ進む

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
- needs re-check
  - ChannelData 自動同期の Unity 実機結果
  - CurrentChannel 自動解決が Save/Load / EndDay と競合しないこと
  - Content Pipeline window の実運用手順
  - PlayMode テスト4件の Unity 上での実行結果
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
