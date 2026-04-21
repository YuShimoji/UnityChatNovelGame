# Handoff

会話ログなしで再開するための最短入口。

## まず読む

1. `docs/HANDOFF.md`
2. `docs/project-context.md`
3. `docs/runtime-state.md`
4. `docs/INVARIANTS.md`
5. `docs/USER_REQUEST_LEDGER.md`

## Handoff snapshot (2026-04-21 sync resume)

**本セッションの実施内容**:

- `origin/main` を 2026-04-20 時点まで fast-forward 済み。表示系デモ計画の正本は `docs/plans/display-batch-showcase.md`。
- ローカル未コミット差分として、`IconSide`、`SetThreadMeta`、`SubthreadData` メタ拡張、`ThreadSwitcherController` のメタ表示行を再適用済み。
- `BubbleStylePreset` / `BubbleStyleDatabase` は **リモート側の static レジストリ実装**に統一。`BubbleStyleDatabase` のシーンアタッチは不要。
- `Assets/Resources/BubbleStyles/` に `thought` / `shout` / `whisper` / `announcement` を追加し、`SP023_LocalExtensions_Start` / `SP023_DisplayShowcase_Start` の検証 Yarn を新設。
- `ChatController` の重複アイコン生成と、`ThreadSwitcherController` のメタ行未生成ケースを補修済み。
- `CharacterProfile.defaultBubbleStylePreset`、SP-024 S3 の `TypingSpeed` / `<<SetTypingSpeed>>`、および SP-024 S1/S2/S5 (`SetTime` / `MarkDelivered` / `MarkRead` / `DeleteLastMessage` / `DeleteMessage`) の UI 接続まで実装済み。
- Unity Editor での再検収は未実施。まず Block 2、その後に LocalExtensions、最後に DisplayShowcase を回し、SP-023 が閉じたら `SP024_Immersion_Start` で S1/S2/S5 を確認するのが最短。

## Current Focus

- 主目的: **SP-023 の Unity 画面検収 3 本** (`SP023_NarrationMargin_Start` → `SP023_LocalExtensions_Start` → `SP023_DisplayShowcase_Start`)
- 続き: **Block 2 / LocalExtensions / Block 6 の見た目確認結果を確定**
- その後の候補: **`SP024_Immersion_Start` による SP-024 S1/S2/S5 の Unity 見た目確認** / **SP-024 S4 オンライン状態 UI** / **Block 4 フリック切替**

## Safe Next Steps

1. `Assets/Scenes/DebugChatScene.unity` で Start Node を `SP023_NarrationMargin_Start` にし、6 メッセージを検収
2. `pyramid` に `Assets/Resources/Images/debug_image_01.png` を一時割当し、DisplayMode = `IconAndName` のまま `SP023_LocalExtensions_Start` を再生して `SetThreadMeta` 即時反映と `IconSide` 左右差を確認
3. `SP023_DisplayShowcase_Start` を再生し、6 preset が warning なしで読み込まれることを確認
4. 3 本とも OK なら、`ChatUIConfig.asset` で `showTimestamp` / `showDeliveryStatus` を有効化し、Start Node を `SP024_Immersion_Start` に切り替えて S1/S2/S5 の見た目確認へ進む
5. `SP024_Immersion_Start` では `SetTime` 継続、`MarkDelivered` / `MarkRead`、`DeleteLastMessage` / `DeleteMessage 2`、Narration/System の時刻非表示をまとめて確認する
6. SP-024 の見た目確認後、次スライスは `docs/plans/display-batch-showcase.md` を参照しつつ S4 または Block 4 を選択

補足:
- SP-023 仕様: `docs/StorySpec/23_text_presentation.md`
- SP-024 仕様: `docs/StorySpec/24_chat_immersion.md`
- PlayMode 8 件の回帰ベースライン: `docs/verification/2026-04-09-playmode-8-results.md`

## Source Of Truth

- 方針・スライス: `docs/project-context.md`
- 作業状態: `docs/runtime-state.md`
- 決定履歴: `docs/DECISION_LOG.md`
- 制作フロー: `docs/OPERATOR_WORKFLOW.md`
- SP-023 / SP-024 表示系デモ計画（修正版・監査済み）: `docs/plans/display-batch-showcase.md`
