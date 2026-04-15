# Handoff

会話ログなしで再開するための最短入口。

## まず読む

1. `docs/HANDOFF.md`
2. `docs/project-context.md`
3. `docs/runtime-state.md`
4. `docs/INVARIANTS.md`
5. `docs/USER_REQUEST_LEDGER.md`

## Handoff snapshot (2026-04-15 session 2)

**本セッションの実施内容**:

### Editor 整備 + ChatUIConfig 集約 (コミット済み `570cdb3`)
- **Editor メニュー統一**: 全 29 の MenuItem を `Tools/FoundPhone/` 配下に統一。旧 `Tools/` 直下、`Tools/Verification/`、`Project FoundPhone/` を統合。Yarn Content Validator の重複登録削除
- **ChatUIConfig タイミング集約**: typewriterSpeed / typingIndicatorDuration / postMessageDelay / enableTypewriterEffect / enableTapSkip を ChatController/ChatDialogueView の SerializeField から ChatUIConfig SO に移行。画像フェードインのハードコード 0.6f を ChatUIConfig.imageFadeInDuration に修正
- **クリーンアップ**: Dev/ 空アセンブリ削除、ChatController TODO docs 重複統合、チェックボックス 21 項目リファレンス化

### 新規仕様書 (未コミット)
- **SP-023 テキスト表現仕様** (`docs/StorySpec/23_text_presentation.md`): バブル位置 (% マージン)、BubbleStylePreset (7 プリセット)、地の文 (narration)、IconSide、フリックスレッド切替、サブクエスト統合 (SP-016/017/022/BL-003)、B/C/D 保留領域 (S10-12)
- **SP-024 チャット没入仕様** (`docs/StorySpec/24_chat_immersion.md`): タイムスタンプ、既読/配信マーク、キャラ別タイピングパターン、オンライン状態、メッセージ削除痕。全機能 ChatUIConfig でオン/オフ切替 (既定オフ = 従来互換)
- **SP-023 実装 (Worker 進行中)**: BubbleStylePreset SO、IconSide、フリックスレッド切替の .cs 実装が未コミットで存在。検収後にコミット予定

### 未コミットファイル一覧
- 新規: `Assets/Scripts/Data/BubbleStylePreset.cs`, `Assets/Scripts/Data/BubbleStyleDatabase.cs`, `docs/StorySpec/24_chat_immersion.md`
- 変更: `Assets/Scripts/Core/ScenarioManager.cs`, `Assets/Scripts/Data/CharacterDatabase.cs`, `Assets/Scripts/Data/CharacterProfile.cs`, `Assets/Scripts/Data/SubthreadData.cs`, `Assets/Scripts/UI/ChatController.cs`, `Assets/Scripts/UI/ThreadSwitcherController.cs`, `docs/StorySpec/23_text_presentation.md`, `docs/spec-index.json`

- **次セッション最初に見るファイル**: `docs/HANDOFF.md` → `docs/project-context.md` → `docs/runtime-state.md`
- **未解決の設計判断**: SP-023 S10-12 (B/C/D 領域) は保留 (HUMAN_AUTHORITY)、各項目に確認時期を記載済み。SP-024 は全機能 draft

## Current Focus

- 主目的: **テキスト表現・チャット没入の仕様実装** (SP-023 / SP-024)
- 並行: エンジン能力マイルストーン (M1: サブスレッド全型実機検証、M2: セーブ/ロード + 章遷移)
- SP-023 実装スライスは Worker に委任進行中。検収後に実装困難な箇所のみ先送り
- SP-024 は仕様策定完了、実装は SP-023 完了後に着手

## Recent Doc Delta

- **C レーン（2026-04-09・完了）**: Unlock ツール整備（上記スナップショット）。`docs/OPERATOR_WORKFLOW.md` S-4 に batch 参照を追加
- **F レーン（2026-04-09・完了）**: `docs/archive/verification-lanes-2026-04/2026-04-09-f-lane-audit-evidence-index.md`（検証・CI 証跡索引）。`spec-index.json` 実測 **41** 件で `runtime-state.md` / `FEATURE_STATUS_AUDIT.md` §1 / `CLAUDE.md` を同期。レーンクローズ後は本開発（Content 軸）優先
- docs 整理を継続実施し、`docs/wiki` の重複ページを段階的に統廃合
- `docs/wiki` はポータル最小構成（`README.md` / `_sidebar.md` / `save-system.md`）へ縮約
- `docs/EVIDENCE_REUSE.md` を新設し、archive 側の Evidence Reuse 文書を統合削除
- `docs/ENGINE_FEATURE_INVENTORY.md` / `docs/SCENARIO_AUTHORING_GUIDE.md` / `docs/PROJECT_OVERVIEW.md` / `docs/HANDOFF.md` を索引・要点中心の薄型へ更新
- `docs/verification/2026-03-30-playmode-batchmode-attempt.md` に旧ローカルパス注記を追加（環境差分）

## Safe Next Steps

1. **SP-023 Worker 成果の検収**: 未コミット .cs の動作確認 (BubbleStylePreset / IconSide / フリック切替)。実装困難な箇所のみ先送り
2. 検収 OK → コミット + push
3. **SP-024 実装着手**: S3 (キャラ別タイピングパターン) を最優先。CharacterProfile 拡張 + ChatDialogueView 修正
4. SP-024 S1 (タイムスタンプ), S2 (既読マーク), S4 (オンライン状態), S5 (削除痕) を順次実装
5. **SP-023 S10-12 (B/C/D 保留領域)**: 各項目の「確認時期」に達したらユーザーに判断を求める
6. エンジン能力 M1/M2 は並行で進行可能

補足:
- SP-023 仕様: `docs/StorySpec/23_text_presentation.md`
- SP-024 仕様: `docs/StorySpec/24_chat_immersion.md`
- PlayMode 8件の回帰ベースライン: `docs/verification/2026-04-09-playmode-8-results.md`

## Source Of Truth

- 方針・スライス: `docs/project-context.md`
- 作業状態: `docs/runtime-state.md`
- 決定履歴: `docs/DECISION_LOG.md`
- 制作フロー: `docs/OPERATOR_WORKFLOW.md`
