# Handoff

会話ログなしで再開するための最短入口。

## まず読む

1. `docs/HANDOFF.md`
2. `docs/project-context.md`
3. `docs/runtime-state.md`
4. `docs/INVARIANTS.md`
5. `docs/USER_REQUEST_LEDGER.md`

## Handoff snapshot (2026-04-20)

**本セッションの実施内容**:

- SP-023 / SP-024 の表示系デモ計画を **checked-in 根拠で再監査**し、旧案（SP-024 を統合デモへ一括込め、UIFontConfig を 0.78 起点で調整する前提等）を **撤回**。修正版を `docs/plans/display-batch-showcase.md` に置き、リモートと共有できる形にした。
- Block 2 の `SP023_NarrationMarginDemo.yarn`（6 メッセージ）は **局所検収として継続で妥当**。SP-024 系 Yarn コマンドは **未登録**のため統合デモから外す方針を正とする。

**Safe Next Steps（次セッション最初）**:

1. 未実施なら **Block 2 画面検収**: `Assets/Scenes/DebugChatScene.unity` → ScenarioManager の Start Node を `SP023_NarrationMargin_Start` → Play → 6 メッセージ確認 → 再生後 Start Node を元（例: `DQT_Start`）に戻してシーン保存。
2. **OK なら Block 3（IconSide）** → Block 6（BubbleStyle プリセット 4 種）→ 統合 Yarn `SP023_DisplayShowcaseDemo.yarn`（**BubbleStyle / Narration / BubbleMargin のみ**。計画細部は `docs/plans/display-batch-showcase.md`）。
3. SP-024（S1/S2/S4/S5）は **実装・コマンド登録後**に別検収。現時点では統合デモに含めない。

---

## Handoff snapshot (2026-04-16 session 3)

**本セッションの実施内容**:

### SP-023 Block 1 完了・検収済み (コミット `ee184cf`)
- `BubbleStylePreset` SO + `BubbleStyleDatabase` + `<<BubbleStyle "presetId">>` コマンド
- `ChatController.SetNextBubbleStyle()` + `ApplyBubbleStylePreset()` (次 1 メッセージ適用・自動リセット)
- `default.asset` (pass-through) + `SP023_BubbleStyleDemo.yarn`
- **ユーザー検収済み**: DebugChatScene / StartNode=`SP023_BubbleStyle_Start` で 3 メッセージ全表示・見た目同一・missing 警告ログ確認

### SP-023 Block 2 実装完了・画面未検証 (コミット `5da8f9a`)
- `<<Narration "text">>` コマンド (narration preset + AddSystemMessage 糖衣)
- `<<BubbleMargin l r t b>>` コマンド (次 1 メッセージに % 指定 padding を上書き)
- `narration.asset` (alpha=0 + italic + center + グレー + suppressWrapper)
- `SP023_NarrationMarginDemo.yarn` (6 メッセージ: Narration×2 / normal / margin×2 / reset)

### 実行プラン
- `C:\Users\thank\.claude\plans\hazy-tumbling-feigenbaum.md` に 9 Block 分割の完全計画あり
- Cadence: **Block 毎に user 実機検収 → OK でコミット → 次 Block**
- SP-024 既定フラグ方針: 全機能既定オフ (仕様書通り)

## Safe Next Steps (次セッション最初のアクション)

1. **Block 2 画面検収**
   - DebugChatScene を開き、ScenarioManager の `Start Node` を `SP023_NarrationMargin_Start` に変更 → Play
   - 6 メッセージの確認:
     - 1/6, 2/6 (Narration): 背景透明・グレーイタリック中央揃え
     - 3/6: 通常バブル (基準)
     - 4/6: 左右 20% ずつ内側 (細長いバブル)
     - 5/6: 上下に余白
     - 6/6: 自動リセット確認 (通常バブルに戻る)
   - 再生後は Start Node を元の値 (`DQT_Start`) に戻してシーン保存
2. **OK なら Block 3 着手** (SP-023 S4 IconSide)
   - `CharacterProfile` に `IconSide enum` フィールド追加 (Auto/Left/Right)
   - `ChatController.ConfigureBubble` の isPlayer 単独判定を IconSide 優先に変更
   - 既存データは Auto = 従来挙動
3. **NG なら原因調査 + 修正** (Block 2 コミット差分を参照)

## 未解決の判断ポイント (HUMAN_AUTHORITY)

- **SP-023 S10** (CharacterProfile.defaultBubbleStylePreset): Block 2 検収後に採否判断
- **フリック閾値 15%** (Block 4): 実機体感で調整要否判断
- **SP-023 S11 (表情アイコン切替)** / **S12 (AI 劣化ビジュアル)** / **SP-024 typewriterSpeedOverride**: 当該 Block 到達時に再評価

## 副次観察

- SP023_BubbleStyleDemo でタイピングインジケーターが全メッセージで非表示。SP-023 変更とは無関係 (ChatDialogueView pre-message delay 挙動)。Block 7 (SP-024 S3 タイピング速度) 実装時に挙動を見直す

## Current Focus

- 主目的: **テキスト表現・チャット没入の仕様実装** (SP-023 / SP-024 を 9 Block で実行中)
- SP-023 Block 2/9 実装完了 (9 Block 中 2 Block)
- SP-024 は仕様策定完了、実装は Block 7 (S3 タイピング速度) から開始予定
- 並行: エンジン能力マイルストーン (M1/M2) は現在スタック外

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
- SP-023 / SP-024 表示系デモ計画（修正版・監査済み）: `docs/plans/display-batch-showcase.md`
