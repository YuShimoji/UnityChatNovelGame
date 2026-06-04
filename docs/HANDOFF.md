# Handoff

会話ログなしで再開するための最短入口。

## まず読む

1. `docs/HANDOFF.md`
2. `docs/project-context.md`
3. `docs/runtime-state.md`
4. `docs/INVARIANTS.md`
5. `docs/USER_REQUEST_LEDGER.md`

## Handoff snapshot (2026-06-03 remote sync)

**本セッションの実施内容**:

- 2026-04-21 以降のローカル追跡差分をプロジェクト文脈ごと保存し、`main` から `origin/main` へ反映するための同期ブロック。
- Unity プロジェクトは `ProjectSettings/ProjectVersion.txt` 上で **6000.4.9f1** に更新済み。別端末は 6000.4.9f1 で開くのが最短。旧文脈の 6000.3.6f1 は今回のリモート反映後は前提にしない。
- `Packages/manifest.json` / `packages-lock.json` は `com.unity.nuget.newtonsoft-json` 3.2.2 に更新済み。新規依存追加ではなく registry package の patch 更新。
- `CharacterProfile_NPC_Pyramid.asset` の `m_IconSide` は `2`。SP-023 LocalExtensions の左右差確認で使う Pyramid 側の Inspector 状態として保持。
- `NotoSansJP-Regular SDF.asset` は dynamic font asset の glyph / character cache が空に再保存された状態。`m_ClearDynamicDataOnBuild: 1` の動的再生成前提だが、表示フォントの実画面確認はまだ必要。
- `ProjectSettings/EditorBuildSettings.asset` は Unity 6000.4.9f1 の再保存で `m_UseUCBPForAssetBundles` 行が落ちたのみ。Build Scene の順序・内容は `TitleScene` / `ContentAuthoring` / `DebugChatScene` / `MVPScene` のまま。
- `.github/workflows/*unity*-tests.yml` の `unityVersion` も 6000.4.9f1 に同期済み。CI と `ProjectVersion.txt` の版数を揃えてから引き継ぐ。
- `.codex/hooks.json` はこの端末の絶対パスを含み、参照先 hook script が存在しないためリモート対象外にした。project-local 正本は `AGENTS.md` と `docs/ai/*.md`。

## Current Focus

- 主目的: **別端末で `main` を pull して SP-023 / SP-024 の表示検収を再開できる状態**
- 続き: **SP-023 の Unity 画面検収 3 本** (`SP023_NarrationMargin_Start` → `SP023_LocalExtensions_Start` → `SP023_DisplayShowcase_Start`)
- その後の候補: **`SP024_Immersion_Start` による SP-024 S1/S2/S5 の Unity 見た目確認** / **SP-024 S4 オンライン状態 UI** / **Block 4 フリック切替**

## Verification / Trust

- `git diff --check`: pass。
- `Packages/manifest.json` / `Packages/packages-lock.json`: PowerShell `ConvertFrom-Json` で parse pass。
- Unity 6000.4.9f1 batchmode open: exit code 0。Asset import / script compile は通過し、batchmode は正常終了。
- Unity log の既知注意: Licensing handshake の一時 error、DOTween Editor asmdef の no scripts warning、`UnityEngine.UI.Tests.dll` skip、`Tools/FoundPhone/Verification/Scan DebugChatScene Missing Scripts` の MenuItem 重複 warning、MCPForUnity info。今回の同期差分では未修正。
- 画面検収は未実施。特に SDF cache reset 後の日本語表示と Pyramid `IconSide=2` は次回 Unity 目視で確認する。

## Safe Next Steps

1. 別端末では `git pull origin main` 後、Unity **6000.4.9f1** で開く。
2. `Assets/Scenes/DebugChatScene.unity` で Start Node を `SP023_NarrationMargin_Start` にし、6 メッセージを検収。
3. `pyramid` に `Assets/Resources/Images/debug_image_01.png` を一時割当し、DisplayMode = `IconAndName` のまま `SP023_LocalExtensions_Start` を再生して `SetThreadMeta` 即時反映と `IconSide` 左右差を確認。
4. `SP023_DisplayShowcase_Start` を再生し、6 preset が warning なしで読み込まれることを確認。
5. 3 本とも OK なら、`ChatUIConfig.asset` で `showTimestamp` / `showDeliveryStatus` を有効化し、Start Node を `SP024_Immersion_Start` に切り替えて S1/S2/S5 の見た目確認へ進む。
6. `SP024_Immersion_Start` では `SetTime` 継続、`MarkDelivered` / `MarkRead`、`DeleteLastMessage` / `DeleteMessage 2`、Narration/System の時刻非表示をまとめて確認する。
7. SP-024 の見た目確認後、次スライスは `docs/plans/display-batch-showcase.md` を参照しつつ S4 または Block 4 を選択。

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
