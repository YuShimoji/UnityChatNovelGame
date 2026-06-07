# Handoff

会話ログなしで再開するための最短入口。

## まず読む

1. `docs/HANDOFF.md`
2. `docs/project-context.md`
3. `docs/runtime-state.md`
4. `docs/INVARIANTS.md`
5. `docs/USER_REQUEST_LEDGER.md`

## Handoff snapshot (2026-06-08 remote sync / Codex config cleanup)

**本セッションの実施内容**:

- `git fetch --prune origin` 後、`origin/main` が `bdf98c4` まで 2 commit 先行していたため、ローカル差分を stash 退避 → `git pull --ff-only origin main` → stash 再適用で同期。同期直後の `HEAD...origin/main` は `0 0`。
- 2026-04-21 以降のローカル追跡差分をプロジェクト文脈ごと保存し、`main` から `origin/main` へ反映するための同期ブロック。
- Unity プロジェクトは `ProjectSettings/ProjectVersion.txt` 上で **6000.4.9f1** に更新済み。別端末は 6000.4.9f1 で開くのが最短。旧文脈の 6000.3.6f1 は今回のリモート反映後は前提にしない。
- `Packages/manifest.json` / `packages-lock.json` は `com.unity.nuget.newtonsoft-json` 3.2.2 に更新済み。新規依存追加ではなく registry package の patch 更新。
- `CharacterProfile_NPC_Pyramid.asset` の `m_IconSide` は `2`。SP-023 LocalExtensions の左右差確認で使う Pyramid 側の Inspector 状態として保持。
- `NotoSansJP-Regular SDF.asset` は dynamic font asset の glyph / character cache が空に再保存された状態。`m_ClearDynamicDataOnBuild: 1` の動的再生成前提だが、表示フォントの実画面確認はまだ必要。
- `ProjectSettings/EditorBuildSettings.asset` は Unity 6000.4.9f1 の再保存で `m_UseUCBPForAssetBundles` 行が落ちたのみ。Build Scene の順序・内容は `TitleScene` / `ContentAuthoring` / `DebugChatScene` / `MVPScene` のまま。
- `.github/workflows/*unity*-tests.yml` の `unityVersion` も 6000.4.9f1 に同期済み。CI と `ProjectVersion.txt` の版数を揃えてから引き継ぐ。
- `.codex/config.toml` は削除済み。Codex の `model` / `approval_policy` / `sandbox_mode` は repo-local に固定せず、ユーザー側・クライアント側設定を使う。再発防止は `docs/INVARIANTS.md` に固定。
- `.claude/settings.local.json` から、存在しない `.claude/hooks/*.sh` 参照を削除。権限設定のみ残す。
- `CharacterProfile.IconSide` の EditMode 2 件、`SP023_NarrationMargin_Start` と `DebugChatScene` の IconSide 配置 PlayMode 2 件を追加。画面検収の前に、実装面の回帰検出点を増やした。
- この端末には Unity 6000.4.9f1 が未導入（`C:\Program Files\Unity\Hub\Editor` には 6000.3.3f1 / 6000.3.6f1 のみ）。Unity 実行検証は 6000.4.9f1 がある別端末または CI で行う。

## Current Focus

- 主目的: **別端末で `main` を pull して SP-023 / SP-024 の表示検収を再開できる状態**
- 続き: **SP-023 の Unity 画面検収 3 本** (`SP023_NarrationMargin_Start` → `SP023_LocalExtensions_Start` → `SP023_DisplayShowcase_Start`)。その前に、追加済みの IconSide / SP-023 PlayMode 2 件を 6000.4.9f1 環境で回すと差分の足場が固い。
- その後の候補: **`SP024_Immersion_Start` による SP-024 S1/S2/S5 の Unity 見た目確認** / **SP-024 S4 オンライン状態 UI** / **Block 4 フリック切替**

## Verification / Trust

- 2026-06-08: `git diff --check` は空白エラーなし（改行変換 warning のみ）。
- 2026-06-08: `.claude/settings.local.json` は PowerShell `ConvertFrom-Json` で parse pass。
- 2026-06-08: `gpt-5-codex` / `model =` / `approval_policy` / `sandbox_mode` / 欠落 hook 名の残存検索はヒットなし。
- 2026-06-08: Unity 6000.4.9f1 がこの端末にないため、追加 EditMode / PlayMode の実行は未実施。
- `git diff --check`: pass。
- `Packages/manifest.json` / `Packages/packages-lock.json`: PowerShell `ConvertFrom-Json` で parse pass。
- Unity 6000.4.9f1 batchmode open: exit code 0。Asset import / script compile は通過し、batchmode は正常終了。
- Unity log の既知注意: Licensing handshake の一時 error、DOTween Editor asmdef の no scripts warning、`UnityEngine.UI.Tests.dll` skip、`Tools/FoundPhone/Verification/Scan DebugChatScene Missing Scripts` の MenuItem 重複 warning、MCPForUnity info。今回の同期差分では未修正。
- 画面検収は未実施。特に SDF cache reset 後の日本語表示と Pyramid `IconSide=2` は次回 Unity 目視で確認する。

## Safe Next Steps

1. 別端末では `git pull origin main` 後、Unity **6000.4.9f1** で開く。Codex 起動時は repo-local `.codex/config.toml` が存在しない前提。
2. 可能なら Unity Test Runner / batch で `CharacterProfile_IconSide_*`、`SP023_NarrationMargin_Start_EmitsExpectedBubbles`、`DebugChatScene_IconSide_ReordersCharacterIcons` を先に確認する。
3. `Assets/Scenes/DebugChatScene.unity` で Start Node を `SP023_NarrationMargin_Start` にし、6 メッセージを検収。
4. `pyramid` に `Assets/Resources/Images/debug_image_01.png` を一時割当し、DisplayMode = `IconAndName` のまま `SP023_LocalExtensions_Start` を再生して `SetThreadMeta` 即時反映と `IconSide` 左右差を確認。
5. `SP023_DisplayShowcase_Start` を再生し、6 preset が warning なしで読み込まれることを確認。
6. 3 本とも OK なら、`ChatUIConfig.asset` で `showTimestamp` / `showDeliveryStatus` を有効化し、Start Node を `SP024_Immersion_Start` に切り替えて S1/S2/S5 の見た目確認へ進む。
7. `SP024_Immersion_Start` では `SetTime` 継続、`MarkDelivered` / `MarkRead`、`DeleteLastMessage` / `DeleteMessage 2`、Narration/System の時刻非表示をまとめて確認する。
8. SP-024 の見た目確認後、次スライスは `docs/plans/display-batch-showcase.md` を参照しつつ S4 または Block 4 を選択。

補足:
- SP-023 仕様: `docs/StorySpec/23_text_presentation.md`
- SP-024 仕様: `docs/StorySpec/24_chat_immersion.md`
- PlayMode 8 件の回帰ベースライン: `docs/verification/2026-04-09-playmode-8-results.md`。2026-06-08 時点では PlayMode フォルダに追加 2 件があるため、次回結果は日付付き新ファイルで記録する。

## Source Of Truth

- 方針・スライス: `docs/project-context.md`
- 作業状態: `docs/runtime-state.md`
- 決定履歴: `docs/DECISION_LOG.md`
- 制作フロー: `docs/OPERATOR_WORKFLOW.md`
- SP-023 / SP-024 表示系デモ計画（修正版・監査済み）: `docs/plans/display-batch-showcase.md`
