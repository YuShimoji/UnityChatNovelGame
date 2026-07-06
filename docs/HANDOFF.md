# Handoff

会話ログなしで再開するための最短入口。

## まず読む

1. `docs/HANDOFF.md`
2. `docs/project-context.md`
3. `docs/runtime-state.md`
4. `docs/INVARIANTS.md`
5. `docs/USER_REQUEST_LEDGER.md`

## Handoff snapshot (2026-06-15 AI entry cleanup / local docs view)

**本セッションの実施内容**:

- `git fetch --prune origin` 後、`origin/main` 先行 1 commit (`9903ea5`) を `git pull --ff-only origin main` で取り込み。
- tracked `.claude/settings.local.json` を削除し、`.codex/config.toml` / `.codex/*.toml` / `.claude/settings.local.json` を `.gitignore` に追加。Codex / Claude の model・承認・sandbox・機械固有パスは repo-local 正本にしない。
- `AGENTS.md` / `CLAUDE.md` / `.claude/CLAUDE.md` を薄い入口ポインタへ戻し、日常ルールは `docs/REPO_LOCAL_RULES.md` に集約。
- 古いルート `CLAUDE.md` 権威参照、Ch1 固定再開プロンプト、再利用テンプレート内の旧 Unity 版固定を調整。
- 既存 Markdown 本文を移動・翻訳・要約せず、MkDocs Material 用のローカル閲覧面を追加。`mkdocs.yml` は `.mkdocs-view/` を docs_dir とし、`tools/generate-doc-nav.ps1 -PrepareView` で閲覧用コピーを作る。
- 全体概観の入口として `docs/PROJECT_STATUS_DASHBOARD.md`、ターン単位計画として `docs/DEVELOPMENT_TURN_PLAN.md`、スクリーンショット索引として `docs/VISUAL_PROGRESS_INDEX.md` を追加。`Assets/Screenshots/` は MkDocs 閲覧用コピーにも含める。

**ローカル閲覧**:

```powershell
pip install mkdocs-material
.\tools\generate-doc-nav.ps1 -PrepareView
python -m mkdocs serve -a 127.0.0.1:8000
```

`http://127.0.0.1:8000/` を Chrome / Edge / DeepL 拡張でページ翻訳し、翻訳は一時読解補助として扱う。

## Handoff snapshot (2026-07-06 Writer Cockpit MVP)

**本セッションの実施内容**:

- `Tools > FoundPhone > Writer Cockpit` を追加。Yarn 保存後の `Refresh Nodes` / `Validate All Yarn Files` / `Sync Authoring Assets` / `Validate Then Sync` / 推奨 Start Node 選択 / ContentAuthoring への Apply / Play / active Yarn フォルダ Ping/Open を一画面に集約。
- `YarnContentValidator` に件数サマリ DTO、`YarnSOGenerator` に active Yarn file/node 数・同期待ち件数サマリ DTO を追加。既存 Validator / SO Generator の解析ロジックはコピーせず再利用。
- `ContentPipelineWindow` の ContentAuthoring 適用処理を静的ヘルパー化し、既存 `Tools > FoundPhone > Content Pipeline` と Writer Cockpit が同じ Apply/Play 処理を通るようにした。
- Save / autosave 欄は読み取り専用のファイル存在確認のみ。既存セーブデータの削除・ロード・上書き・移行は行わない。
- `docs/YarnEditingPipeline.md` / `docs/OPERATOR_WORKFLOW.md` を Writer Cockpit 優先導線に更新し、`docs/PROJECT_COCKPIT.md` / `docs/PROJECT_PIPELINE.mmd` を追加。

## Handoff snapshot (2026-07-06 Package Manager recovery / validation resume)

**本セッションの実施内容**:

- `git fetch --prune origin` 後、`origin/main` 先行 1 commit (`2c6b11d`) を `git pull --ff-only origin main` で取り込み。作業ツリーは source 差分なしから開始。
- Unity 6000.4.9f1 の `path undefined` は、`Packages/packages-lock.json` 削除、`Library/PackageManager` の generated cache 削除、`Packages/manifest.json` の一時補正で再現。`manifest.json` / `packages-lock.json` / package cache の JSON 破損ではない。
- `Library/PackageManager` の generated cache を退避から復元し、`Packages/manifest.json` / `Packages/packages-lock.json` の UTC タイムスタンプを ProjectCache metadata と合わせることで、ローカル batch open は Package Manager 登録、script compile、return code 0 まで復旧。
- `Packages/manifest.json` の `com.unity.test-framework` は `2.0.1` 指定だが、lock / local cache / Unity 6000.4.9f1 built-in metadata は `1.6.0`。一時的に manifest を `1.6.0` へ合わせても fresh resolve は直らなかったため、source 変更は残していない。
- 非破壊 Yarn validator batch は `errors=0, warnings=33, info=3`、`Scanned 11 files, 74 nodes, 24 #line: tags, 42 declared variables` まで到達。警告は既存の unknown command / unknown character / undeclared variable 系。

## Current Focus

- 主目的: **復旧済みの local Package Manager cache 状態を維持し、Writer Cockpit の interactive menu / Apply / Play を確認**
- 続き: Unity **6000.4.9f1** の batch open / compile と非破壊 Yarn validator batch は到達済み。`Tools > FoundPhone > Writer Cockpit` と `Tools > FoundPhone > Content Pipeline` は MenuItem source を確認済みだが、interactive Editor 上のメニュー表示と Cockpit 操作は未確認。
- 注意: `Library/PackageManager` の generated cache 削除、`Packages/packages-lock.json` 再生成、`Packages/manifest.json` 変更は `path undefined` を再発させる。次は fresh resolve 復旧ではなく、現在の復旧済みローカル状態で Cockpit UI を確認するのが最短。
- その後の候補: Cockpit から `DQT_Start` または推奨ノードを Apply / Play し、問題なければ SP-023 / SP-024 表示検収へ戻る。

## Validation note (2026-07-06 Writer Cockpit)

- Unity executable: `C:\Program Files\Unity\Hub\Editor\6000.4.9f1\Editor\Unity.exe`
- Batch logs:
  - `Logs/writer-cockpit-unity-open-2026-07-06.log`
  - `Logs/writer-cockpit-unity-open-2026-07-06-rerun.log`
  - `Logs/writer-cockpit-yarn-validator-2026-07-06.log`
- Recovery logs:
  - `Logs/writer-cockpit-cache-utc-timestamp-restored-2026-07-06.log`（batch open / compile / return code 0）
  - `Logs/writer-cockpit-final-yarn-validator-2026-07-06.log`（Yarn validator batch: errors=0, warnings=33, info=3）
- Result: original attempts stopped before package load with `Failed to resolve packages: The "path" argument must be of type string. Received undefined. No packages loaded.` The local validation loop is now recovered through PackageManager cache/timestamp restoration, but fresh Package Manager resolution remains fragile.
- Verification note: `docs/verification/2026-07-06-writer-cockpit-unity-validation.md`

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
- `.codex/config.toml` は削除済み。Codex の実行環境は repo-local に固定せず、ユーザー側・クライアント側設定を使う。再発防止は `docs/INVARIANTS.md` に固定。
- tracked `.claude/settings.local.json` も削除し、機械固有の権限・絶対パスは repo に持たせない方針へ寄せた。
- `CharacterProfile.IconSide` の EditMode 2 件、`SP023_NarrationMargin_Start` と `DebugChatScene` の IconSide 配置 PlayMode 2 件を追加。画面検収の前に、実装面の回帰検出点を増やした。
- この端末には Unity 6000.4.9f1 が未導入（`C:\Program Files\Unity\Hub\Editor` には 6000.3.3f1 / 6000.3.6f1 のみ）。Unity 実行検証は 6000.4.9f1 がある別端末または CI で行う。

## Previous Focus (2026-06-08)

- 主目的: **別端末で `main` を pull して SP-023 / SP-024 の表示検収を再開できる状態**
- 続き: **SP-023 の Unity 画面検収 3 本** (`SP023_NarrationMargin_Start` → `SP023_LocalExtensions_Start` → `SP023_DisplayShowcase_Start`)。その前に、追加済みの IconSide / SP-023 PlayMode 2 件を 6000.4.9f1 環境で回すと差分の足場が固い。
- その後の候補: **`SP024_Immersion_Start` による SP-024 S1/S2/S5 の Unity 見た目確認** / **SP-024 S4 オンライン状態 UI** / **Block 4 フリック切替**

## Verification / Trust

- 2026-06-08: `git diff --check` は空白エラーなし（改行変換 warning のみ）。
- 2026-06-15: repo-local Codex 実行環境固定と tracked `.claude/settings.local.json` の残存検索は、ignore / invariants / rules 上の禁止記述を除きヒットなし。
- 2026-06-15: `python -m mkdocs build` pass（Material for MkDocs の MkDocs 2.0 告知 warning は表示されるが build は正常終了）。
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
- 全体概観: `docs/PROJECT_STATUS_DASHBOARD.md`
- ターン単位プラン: `docs/DEVELOPMENT_TURN_PLAN.md`
- 画面証跡索引: `docs/VISUAL_PROGRESS_INDEX.md`
- 作業状態: `docs/runtime-state.md`
- 決定履歴: `docs/DECISION_LOG.md`
- 制作フロー: `docs/OPERATOR_WORKFLOW.md`
- SP-023 / SP-024 表示系デモ計画（修正版・監査済み）: `docs/plans/display-batch-showcase.md`
