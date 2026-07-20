# Runtime State

**Updated**: 2026-07-21（Sites private runtime / cross-terminal sync）

## Current Position

- project: FoundPhone (UnityChatNovelGame)
- branch: main
- lane: **Authoring Tooling integrated / Sites owner-only hosted review pending**
- slice: **accepted Writer Cockpit navigation、portable static package、Sites Version 1 private runtimeを引き継いだ状態**
- next_recommended_slice: **User / Web SupervisorがOwner sign-in後のhosted app本文を両分岐・keyboard・network・responsiveで受け入れる**
- subsequent_recommended_slice: **External Script Editor line jumpの人間環境レビュー + Writer Cockpit Apply / Play再受入**
- later_recommended_slice: **save data隔離後のfull 83 tests → SP-023 / SP-024表示検収 → M1/M2/M3**
- active_artifact: WriterCockpitWindow / WriterCockpitNavigation / YarnContentValidator / YarnAuthoringRegistry / YarnSOGenerator / `sites/foundphone-demo/` / `tools/sites/` / Sites project `appgprj_6a5ddb11908081918180da7797957e63`
- artifact_surface: Editor tooling + portable static input + Owner-only Sites hosted runtime（non-canon fixture。Unity/Yarn正本とは非接続）
- last_change_relation: static sourceをSites runtimeへ等価変換し、source/version/deployment/accessを再取得してproject-local verificationへ固定
- plan_file: `docs/SUPERVISOR_REPORT.md`（現状・G0-G13 目標案） / `docs/plans/display-batch-showcase.md`（表示系デモ正本）

## Counters

- last_user_visible_change: session 22 (2026-04-03 タイプライター同期修正)
- (blocks_since_* / consecutive_* / visual audit カウンターは廃止: 確認コスト原則 / CORE_RULESET に従い、"未実施" 指標が再実行圧力となるのを回避)

## Quantitative Metrics (0 を目指す指標のみ、件数追跡は廃止)

- tests_last_run: 2026-07-19 targeted WriterCockpitNavigation EditMode 14/14 pass。full suiteは2026-04-09基準のまま
- mock_files: 0
- spec_entries: 42 (`docs/spec-index.json` 配列長、検証用。SP-023/SP-024 追加)
- todo_fixme_hack: ChatController.cs:2020 に 1 件残存 (FEATURE_STATUS_AUDIT W-6 参照)
- obsolete_marks: ContradictionPair.UnlockTopic x2

## Visual Evidence

- last_visual_audit_path: docs/archive/verification-evidence/VerticalSliceSmokeGate_20260403_*.png (参考。パスのみ保持、追跡は廃止)

## Session Log

### 2026-07-21（Sites private runtime / cross-terminal sync）

- **開始状態**: `main` / `origin/main`は`1e582639`、ahead/behind `0/0`、worktree clean。fetch後も不変。
- **Sites control plane**: project `appgprj_6a5ddb11908081918180da7797957e63`はactive、`custom` access、user 1、group 0。Version 1 source `29c5245d...`、deployment succeededを再取得。
- **runtime**: static HTML/CSS/JSをVinext/Reactへ等価変換。`content/demo.json`はbuild-time importの表示正本。local Workerで両分岐、restart、Enter/Space、focus、ARIA、320–430px、禁止機能監査をpass。
- **platform boundary**: private deployment APIを使用したが、Sitesは`type=publish` / `current_live_url`として返す。accessはOwner-onlyで、public/shared access、custom domainは未実施。
- **remaining gate**: private URLのSites標準sign-in gateまでは確認。Owner認証後のhosted app本文は未確認で、User / Web Supervisorが両分岐、network、keyboard、responsiveを最終reviewする。
- **portable handoff**: 詳細は`docs/verification/2026-07-20-sites-private-runtime-validation.md`。machine-local visualization worktree/tokenを再開前提にしない。

### 2026-07-19（Sites-native demo repository integration）

- **開始状態**: primary `main` / `origin/main`は`b4e92ecb4f05a923b9177138fcf026fcfb561bba`、ahead/behind `0/0`、worktree clean。`git fetch --prune origin`後も不変。
- **source audit**: read-only sibling `spike/sites-native-chat-demo`はbase `55cb0d20`で、未追跡変更は`sites/foundphone-demo/**`、`tools/sites/**`、`docs/verification/sites-native-demo-validation.md`のみ。全10ファイルを実読し、統合直後のSHA-256が全件一致。siblingは変更なし。
- **tracked artifact**: `sites/foundphone-demo/`、`tools/sites/`、検証正本をrepository artifact化。`SITES_IMPORT_BRIEF.md`にrepository path、entry point、serve command、private preview/public gateを補完。
- **targeted validation**: JSON parse、7-node graph到達性、JS/static-server/PowerShell wrapper syntax、禁止pattern audit、HTTP 200/MIME/nosniff/no-storeをpass。port `4318`の一時serverは検証後停止。
- **browser**: 青信号/白ノイズ両ending、route差、restart初期化、`1280x900` / `390x844` / `320x700`、横overflowなし、48px choice、3px focus ring、ARIA live/progress、console warning/error 0を確認。外部runtime URL、form/input、auth/storage/analytics/payment/secret/live destinationなし。
- **quality debt**: 自動化されたEnter/Space activationは状態遷移を証明できず、native control semantics/focus pathだけを確認。actual Sites runtime compatibility、private access、実キーボードはprivate previewのHuman Gate。
- **boundary**: ChatGPT Site作成、private preview import、public deployment、Unity Web module、scene/asset/Yarn/package変更、full 83 testsは未実施。direct Unity Web routeはblocked継続。

### 2026-07-19（accepted parallel results integration）

- **開始状態**: primary `main` / `origin/main` は `55cb0d20`、ahead/behind `0/0`、worktree clean。editor accepted commit `23e4b635` はbase `55cb0d20`の直系で、Sites/Yarn本文/package/BuildSettingsへの変更なし。
- **editor integration**: 一時integration branchへ競合なくcherry-pick。Writer Cockpitに74 Node検索、source asset/1-based title行、diagnostic drilldown、file/line source jumpを追加。Refresh / Validate / Sync / Validate Then Sync / Apply / Playは維持し、Save statusは`File.Exists`だけのread-only。
- **validator**: runtime literal `AddCommandHandler`とCharacterProfile assetをsource of truthに変更。active Yarnは `errors=0 / warnings=0 / info=3`、11 files / 74 nodes。故意の未知command/characterを検出する回帰testは維持。
- **targeted validation**: detached integration validation worktreeでEditMode 14/14 pass、Unity 6000.4.9f1 batch compile return code 0、非破壊Yarn validator return code 0。Unity実行由来のtracked差分なし。full 83 tests / PlayMode全体は未実行。
- **publication evidence**: `docs/verification/sites-publication-feasibility.md` を統合。Web Build Supportなし、`MVPScene.unity`欠落、TitleSceneの公開不可scene依存によりdirect Unity Web routeはblocked。互換性・build・公開成功は未証明。
- **human review debt**: External Script Editor未設定のため実file/line jumpは未確認。アプリ選択を自動化せず、安全な未設定statusまでをaccepted evidenceとする。
- **next move**: publication successorは別laneのSites-native lightweight chat demo。public operationなしのlocal artifactから開始し、Unity module導入・public scene修復・公開は別gateとする。

### 2026-07-17（remote sync / development readiness refresh）

- **開始状態**: 2026-07-15 の検証記録4ファイルが未コミットで存在。差分をレビューし、code / Yarn / scene / asset の残留変更がないことを確認して保持した。
- **同期**: `git fetch --prune origin` → `git pull --ff-only origin main` を実行し、pull は `Already up to date.`。同期起点の `main` / `origin/main` は `7eb09c0`、`HEAD...origin/main` は `0/0`。
- **toolchain**: Unity 6000.4.9f1 executable と ProjectVersion が一致。`Packages/manifest.json` / `Packages/packages-lock.json` は parse pass。wrapper batch open は 39 packages の cache restore、Tundra success、0 items updated / 326 evaluated、batchmode 正常終了。
- **validator**: errors=0 / warnings=33 / info=3、11 files / 74 nodes / 24 `#line:` tags / 42 declared variables。既知の command registry drift は未解消。
- **tests**: 静的な属性数は EditMode 73 / PlayMode 10。save data 隔離前のため全83テストは未実行し、結果として扱わない。
- **docs viewer**: `generate-doc-nav.ps1 -PrepareView` と `uvx --from mkdocs-material mkdocs build --strict` は pass。PerformanceBaseline raw pages の nav 外 INFO のみ。
- **warning**: Unity 起動時に `VerificationMenu` と `MissingScriptScanner` の同一 `MenuItem` 重複警告が再現。compile blocker ではなく、後続の小さな Editor tooling 整理対象。UnityConnect CDN timeout と MkDocs 将来互換性告知も非阻害。
- **state**: Unity 実行由来の tracked 差分なし。active slice は Writer Cockpit interactive 受入のまま。`Library/` / `Logs/` は ignored local evidence であり、remote authority ではない。
- **next move**: shared owner が Writer Cockpit の Refresh → Validate Then Sync → Apply / Play → Last Action を1回受け入れる。次に assistant owner が Validator command drift と安全な83テスト基準化を閉じる。

### 2026-07-13（remote parity / cross-terminal handoff）

- **目的**: 現在の全コンテキストを project-local docs に保持し、別端末が会話ログなしで `main` から再開できる状態をリモートへ固定する。
- **同期**: `git fetch --all --prune` と `git pull --ff-only origin main` を実行。同期前の作業ツリーは clean、`main` / `origin/main` は `fa8eb8b`、`HEAD...@{u}` は `0/0`。
- **状態**: コード、Yarn、scene、asset の追加変更なし。active slice は Writer / Designer Cockpit MVP の interactive 受入のまま。
- **残作業**: 目的は作者導線の実在確認と検証信頼性の回復。効果は Writer Cockpit を日常制作入口として受け入れ、現行回帰基準を固定すること。要件は Unity 6000.4.9f1、`tools/run-unity.ps1`、既存 save data の隔離。状態は interactive Cockpit、EditMode 73 / PlayMode 10、SP-023/024 visual acceptance が未確認。shared owner は Cockpit Apply / Play の受入、assistant owner は Validator command drift 解消と83テスト実行。次の move は `docs/HANDOFF.md` の Safe Next Steps 1 から順に実行する。
- **境界**: ignored `Library/` / `Logs/` は別端末へ移送しない。再現可能な tracked source / docs / wrapper のみを remote authority とする。

### 2026-07-11（remote sync / fresh Package Manager resolve / supervisor handoff）

- **同期**: `2c6b11d` → `51dc8bc` を fast-forward。同期直後の `HEAD...origin/main` は `0/0`。
- **ライブ差分**: tracked docs が記録していた `ProjectCache` と 2026-07-07 logs は現ワークスペースに存在せず、通常 batch open は fresh resolve で `path undefined` / return code 1。
- **根因**: UPM stack trace は `getDeprecatedGlobalConfigRoot()` の `path.join`。Codex / PowerShell 子環境で標準 `ALLUSERSPROFILE` が欠落していた。
- **復旧**: process-local に `C:\ProgramData` を補完し、隔離 resolve exit 0。実リポジトリでも 39 packages、script compile、return code 0。
- **tooling**: `tools/run-unity.ps1` を追加。user/system environment は変更せず、ProjectVersion から Unity を選ぶ。
- **auto-dirty fix**: `BuildSettingsHelper` の早期 AssetDatabase null による ContentAuthoring scene reorder を filesystem check へ変更。wrapper 再起動で Build Settings 内容差分なし。
- **validator**: errors=0 / warnings=33 / info=3、11 files / 74 nodes。warning 33件中24件は登録済み command の偽陽性。
- **docs viewer**: `generate-doc-nav.ps1 -PrepareView` + `mkdocs build --strict` pass。ignored PerformanceBaseline raw pagesは nav 外の INFO のみ。
- **未確認**: interactive Cockpit、EditMode 73 / PlayMode 10、SP-023/024 visual acceptance。全テストは save data 隔離後に実行する。
- **handoff**: `docs/SUPERVISOR_REPORT.md` に trust、残作業、Human Authority、G0-G13 を固定。

### 2026-07-07（Writer Cockpit validation recheck）

- **目的**: リモート同期後のローカル `main` が、添付Promptの Package Manager recovery 完了条件をまだ満たすか再確認し、次の作業判断を固める。
- **同期**: `git pull --ff-only` は `Already up to date.`。開始時と再確認時の作業ツリーは source 差分なし。
- **Package diagnostics**: Unity 6000.4.9f1 executable と `ProjectVersion.txt` は一致。`Packages/manifest.json` / `Packages/packages-lock.json` / `Assets/MCPForUnity/package.json` は JSON parse pass。`Packages/` 直下に package.json はなく、`file:` / `../` / 絶対 Windows path / package `path` field は検出なし。既知差分として `com.unity.test-framework` は manifest `2.0.1`、lock/cache `1.6.0` のまま。
- **Batch validation**: `Logs/writer-cockpit-unity-open-2026-07-07.log` で Package Manager cache restore、39 packages 登録、script compile、return code 0 を確認。
- **Yarn validator**: `Logs/writer-cockpit-yarn-validator-2026-07-07.log` で `errors=0, warnings=33, info=3` / `Scanned 11 files, 74 nodes, 24 #line: tags, 42 declared variables` を確認。警告は既存 Yarn 診断で、Package Manager / compile failure ではない。
- **静的到達性**: `Tools > FoundPhone > Writer Cockpit` と `Tools > FoundPhone > Content Pipeline` の `MenuItem` source を再確認。Writer Cockpit の save status は `File.Exists` の read-only 確認のみで、SaveGame / LoadGame / DeleteSave 呼び出しはない。
- **残り**: interactive Unity Editor 上で Cockpit 実メニュー、画面表示、Apply / Play は未確認。次は generated Package Manager cache や package source を触らず、復旧済みローカル状態のまま visible Editor pass を行う。

### 2026-07-06（Writer / Designer Cockpit MVP）

- **目的**: ライター/デザイナーが Yarn 保存後に Unity Editor 上で node/status 確認、Validate、SO Sync、Start Node 適用、Play、Last Action、read-only Save / autosave 状態確認まで一画面で回せるようにする。
- **Editor tooling**: `Assets/Scripts/Editor/WriterCockpitWindow.cs` を追加し、`Tools > FoundPhone > Writer Cockpit` を新設。既存 `ContentPipelineWindow` はメニュー維持のまま、ContentAuthoring Apply/Play 処理を共有ヘルパー化。
- **Reuse boundary**: `YarnContentValidator` は件数サマリ、`YarnSOGenerator` は active Yarn file/node 数と同期待ち件数を返す DTO を追加。大きな解析ロジックのコピーはしていない。
- **Save safety**: Cockpit の Save / autosave 欄は `Application.persistentDataPath` 配下の既定ファイル名存在確認のみ。既存セーブデータの削除・ロード・上書き・移行はしない。
- **Docs**: `docs/YarnEditingPipeline.md` / `docs/OPERATOR_WORKFLOW.md` を Writer Cockpit 優先導線に更新。`docs/PROJECT_COCKPIT.md` / `docs/PROJECT_PIPELINE.mmd` を追加。
- **検証境界**: active Yarn は 11 files / 74 node titles を PowerShell + `rg` で確認。`git diff --check` は空白エラーなし。当該チェックポイント時点では Unity compile/menu確認は未実施。

### 2026-07-06（Writer Cockpit Unity integration validation attempt）

- **目的**: Unity 6000.4.9f1 で Writer Cockpit の compile/menu 到達性と、非破壊 Yarn validator batch の実行可否を確認する。
- **Unity availability**: `C:\Program Files\Unity\Hub\Editor\6000.4.9f1\Editor\Unity.exe` が存在。ProjectVersion も `6000.4.9f1`。
- **実行**: batch open を2回、`ContentPipelineBatch.RunYarnValidatorBatch` を1回実行。ログは ignored の `Logs/` 配下に保存。
- **結果**: すべて Package Manager 解決で停止し、`Failed to resolve packages: The "path" argument must be of type string. Received undefined. No packages loaded.` を出して return code 1 終了。Writer Cockpit / Content Pipeline のUnity上のmenu到達性は未証明。
- **静的監査**: `manifest.json` / `packages-lock.json` は PowerShell `ConvertFrom-Json` parse pass。`WriterCockpitWindow.cs` は Editor-only `#if UNITY_EDITOR`、`ProjectFoundPhone.Editor` namespace、`MenuItem("Tools/FoundPhone/Writer Cockpit")` を確認。`SaveGame` / `LoadGame` / `DeleteSave` / `AutoSave` 呼び出しはなく、save status は `File.Exists` のみ。
- **検証ノート**: `docs/verification/2026-07-06-writer-cockpit-unity-validation.md`。

### 2026-07-06（Package Manager recovery / Writer Cockpit batch validation）

- **目的**: 添付Promptの次スライスに従い、Package Manager `path undefined` を復旧または精密化し、Writer Cockpit の compile / validator 到達性を確認する。
- **同期**: `git fetch --prune origin` 後、`origin/main` 先行 1 commit (`2c6b11d`) を fast-forward で取り込み。source は clean。
- **診断**: `Packages/manifest.json` / `Packages/packages-lock.json` / `ProjectSettings/PackageManagerSettings.asset` を確認。package JSON 破損や local path dependency は見つからず。`com.unity.test-framework` は manifest `2.0.1`、lock / cache / Unity built-in metadata `1.6.0` の差があるが、manifest を一時的に `1.6.0` へ合わせても fresh resolve は直らなかったため source 変更は残していない。
- **再現条件**: `Packages/packages-lock.json` 削除、`Library/PackageManager` generated cache 削除、または manifest 変更で `Failed to resolve packages: The "path" argument must be of type string. Received undefined. No packages loaded.` が再発。registry access 前の local Package Manager 解決で落ちる。
- **復旧**: 退避した `Library/PackageManager/ProjectCache` / `ProjectCache.md5` / `projectResolution.json` を戻し、`Packages/manifest.json` / `Packages/packages-lock.json` の `LastWriteTimeUtc` を ProjectCache metadata に合わせることで batch open は復旧。`Logs/writer-cockpit-cache-utc-timestamp-restored-2026-07-06.log` で Package Manager 39 packages 登録、batch quit、return code 0。
- **追加検証**: `Logs/writer-cockpit-final-yarn-validator-2026-07-06.log` で非破壊 Yarn validator batch 到達。`errors=0, warnings=33, info=3` / `Scanned 11 files, 74 nodes, 24 #line: tags, 42 declared variables`。警告は既存の unknown command / unknown character / undeclared variable 系。
- **残り**: Interactive Unity Editor で `Tools > FoundPhone > Writer Cockpit` の実メニュー表示、Cockpit 表示、Apply / Play は未確認。次は `Library/PackageManager` や `packages-lock.json` を触らずに interactive 確認する。

### 2026-06-15（AI 入口薄型化・ローカル docs viewer 整備）

- **目的**: repo-local Codex 実行環境固定や機械固有 local settings を再発させず、既存 Markdown 正本を移動・要約・翻訳せずにブラウザで横断確認できる閲覧面を足す。
- **同期**: `git fetch --prune origin` 後、`origin/main` 先行 1 commit を `git pull --ff-only origin main` で取り込み済み。
- **AI / client 設定**: tracked `.claude/settings.local.json` を削除し、`.codex/config.toml` / `.codex/*.toml` / `.claude/settings.local.json` を `.gitignore` に追加。`AGENTS.md` / `CLAUDE.md` / `.claude/CLAUDE.md` は薄い入口ポインタに戻し、実行環境設定はユーザー側 client configuration へ委ねる。
- **正本ルール**: `docs/REPO_LOCAL_RULES.md` を追加し、日常ルール、開発境界、Codex / client runtime config 境界、docs-only 検証方針を集約。古いルート入口文書への権威参照は `docs/REPO_LOCAL_RULES.md` / `docs/INVARIANTS.md` 参照へ付け替え。
- **閲覧面**: MkDocs Material を第一候補として `mkdocs.yml` / `docs/index.md` / `tools/generate-doc-nav.ps1` を追加。既存 Markdown 本文は分類・閲覧対象として扱い、翻訳版や要約版の恒久ファイルは作らない。
- **概観調整**: `docs/PROJECT_STATUS_DASHBOARD.md` で実装済み機能・今後の新機能・項目別実装・画面証跡・ターン単位計画の行き先を一枚化。`docs/DEVELOPMENT_TURN_PLAN.md` で日付ではなく Turn 0-6 の区切りに整理。`docs/VISUAL_PROGRESS_INDEX.md` で `Assets/Screenshots/` の既存 14 枚と次に撮るべき SP-023 / SP-024 画像名を明示。
- **検証**: `python -m mkdocs build` pass。Material for MkDocs の MkDocs 2.0 告知 warning は表示されるが、viewer build 自体は正常終了。

### 2026-06-08（Codex 起動設定整理・追加テスト・remote 反映準備）

- **目的**: Codex Thread 開始時の repo-local モデル固定エラーを解消し、同じ文脈を project-local docs に残したうえで、別端末が `main` pull 直後に再開できる状態へ戻す。
- **Git**: `git fetch --prune origin` で `origin/main` が `bdf98c4` まで先行。ローカル差分を stash 退避し、`git pull --ff-only origin main` で fast-forward 後に stash 再適用。同期直後の `HEAD...origin/main` は `0 0`。
- **Codex / assistant 設定**: tracked `.codex/config.toml` を削除し、Codex 実行環境の repo-local 固定を廃止。`docs/INVARIANTS.md` に、Codex 実行環境設定はユーザー側設定へ委ねるルールを追加。
- **Claude local 設定**: この時点では `.claude/settings.local.json` の欠落 hook 参照のみを除去。2026-06-15 に tracked local settings 自体を削除。
- **テスト差分**: `CoreLogicTests` に `CharacterProfile.IconSide` の既定値・設定値テストを追加。`ScenarioFlowPlayModeTests` に `SP023_NarrationMargin_Start` のバブル生成確認と、`DebugChatScene` 上の IconSide 左右配置確認を追加。
- **検証境界**: `git diff --check` は空白エラーなし、Codex 固定設定と欠落 hook 名の残存検索はヒットなし。この端末には Unity 6000.4.9f1 がないため、追加 EditMode / PlayMode 実行は未実施。
- **次の所有者**: assistant / CI は追加テストの Unity 6000.4.9f1 実行確認を担当。ユーザーは SP-023 / SP-024 の画面検収判断を担当。

### 2026-06-03（ローカル状態の remote 反映・別端末 handoff）

- **目的**: 会話ログなしで別端末から再開できるよう、現在のローカル追跡差分・検証結果・残リスクを project-local docs に固定し、`origin/main` へ反映する。
- **Unity / packages / CI**: `ProjectVersion.txt` を 6000.4.9f1 に更新。`com.unity.nuget.newtonsoft-json` は 3.2.2。GitHub Actions の EditMode / PlayMode `unityVersion` も 6000.4.9f1 に同期。別端末は Unity 6000.4.9f1 で開く前提。
- **Assets / settings**: `CharacterProfile_NPC_Pyramid.asset` は `m_IconSide: 2`。`NotoSansJP-Regular SDF.asset` は dynamic font cache が空の状態で保存されているため、日本語表示は次回の画面検収で重点確認。`EditorBuildSettings.asset` は Unity 再保存により `m_UseUCBPForAssetBundles` 行が削除されたのみ。
- **Local-only**: `.codex/hooks.json` は絶対パスと未存在 script 参照を含むため、リモート対象外として `.gitignore` に追加。Codex/AI 向け正本は `AGENTS.md` と `docs/ai/*.md`。
- **検証**: `git diff --check` pass、Packages JSON parse pass、Unity 6000.4.9f1 batchmode open exit code 0。ログ上は Licensing handshake の一時 error、DOTween Editor asmdef no scripts warning、`UnityEngine.UI.Tests.dll` skip、MenuItem 重複 warning、MCPForUnity info が残るが、batchmode は正常終了。
- **未検証**: SP-023 3 本の画面検収、SP-024 S1/S2/S5 の画面検収、SDF cache reset 後の日本語表示、Pyramid `IconSide=2` の実表示。

### 2026-04-21（同期再開・ローカル差分再適用）

- **Git**: `origin/main` へ fast-forward 済み。stash 退避後に再適用し、競合を手動統合。
- **統合方針**: `BubbleStylePreset` / `BubbleStyleDatabase` はリモートの static レジストリ実装を採用し、ローカル差分は `IconSide` / `SetThreadMeta` / `SubthreadData` メタ / サイドバーメタ表示に絞って再適用。
- **追加**: `Assets/Resources/BubbleStyles/` に `thought` / `shout` / `whisper` / `announcement`、`Assets/Resources/Yarn/active/` に `SP023_LocalExtensionsDemo.yarn` / `SP023_DisplayShowcaseDemo.yarn` を追加。
- **補修**: `ChatController` の重複アイコン生成を解消し、`ThreadSwitcherController` がメタ追加後に `MetaLabel` を動的生成できるよう修正。
- **追加実装**: `CharacterProfile.defaultBubbleStylePreset`、SP-024 S3 の `TypingSpeed` / `<<SetTypingSpeed>>` / `ScenarioManager` セッション override、`SavedChatMessage` の SP-024 データ契約 (`Timestamp` / `DeliveryStatus` / `IsDeleted`) を先行反映。
- **追加**: `Assets/Resources/Yarn/active/` に `SP024_ImmersionDemo.yarn` (`SP024_Immersion_Start`) を追加し、SP-024 S1/S2/S5 の局所検証導線を分離。
- **未検証**: Unity Editor での Block 2 / LocalExtensions / DisplayShowcase の画面確認、`SP024_Immersion_Start` による S1/S2/S5 の画面確認、および S3 の待機時間差確認。

### 2026-04-20（表示系デモ計画の再監査・ドキュメント同期）

- **目的**: 「表示系一括 + SP-024 統合」旧プランと実装の齟齬を解消し、再開用にリポジトリへ正本を置く。
- **追加**: `docs/plans/display-batch-showcase.md`（SP-024 は統合デモから外す、`ChatUIConfig`/`UIFontConfig` の checked-in 実値に基づく監査、統合デモは SP-023 のみ・15–18 メッセージ目安）。
- **更新**: `docs/HANDOFF.md`（2026-04-20 スナップショット）、`docs/ai/READ_ORDER.md`（タスク別 1 行）。
- **コード変更なし**（計画・引き継ぎのみ）。

### 2026-04-16 session 3（SP-023 Block 1 検収 + Block 2 実装）

- **Block 1 (SP-023 S1 BubbleStylePreset 基盤) 完了・検収済み** コミット `ee184cf`:
  - `BubbleStylePreset.cs` SO (13 フィールド + 上書きフラグ方式)
  - `BubbleStyleDatabase.cs` 静的レジストリ (`Resources/BubbleStyles` 自動収集)
  - `<<BubbleStyle "presetId">>` Yarn コマンド登録
  - `ChatController.SetNextBubbleStyle()` + `ApplyBubbleStylePreset()` (次 1 メッセージに適用・自動リセット)
  - `default.asset` (pass-through プリセット)
  - `SP023_BubbleStyleDemo.yarn`: 3 メッセージ検証
  - **検収結果**: DebugChatScene + StartNode=SP023_BubbleStyle_Start で 3 メッセージ全表示・見た目同一・missing 警告ログ確認済み
- **Block 2 (SP-023 S2 Narration + S3 BubbleMargin) 実装完了・画面未検証** コミット `5da8f9a`:
  - `<<Narration "text">>` コマンド (narration preset + AddSystemMessage 糖衣)
  - `<<BubbleMargin l r t b>>` コマンド (次 1 メッセージのラッパー padding を % 指定で上書き)
  - `ChatController`: `m_PendingBubbleMarginPercent` + `SetNextBubbleMargin()` + `ConfigureBubble` で margin 上書き + `AddSystemMessage` で preset 消費
  - `narration.asset`: alpha=0 + italic + center + グレーテキスト + suppressWrapper
  - `SP023_NarrationMarginDemo.yarn`: 6 メッセージ (Narration×2, normal, margin×2, reset)
- **次セッション再開導線**:
  1. Block 2 画面検証: DebugChatScene → StartNode=`SP023_NarrationMargin_Start` → Play
  2. 6 メッセージの見た目確認 (narration 透明背景/細長いバブル/上下余白/自動リセット)
  3. OK なら Block 3 (SP-023 S4 IconSide) 着手
- **判断ポイント (保留)**:
  - フリック閾値 15%: Block 4 実装後に実機体感で判断
- **副次観察**: SP023_BubbleStyleDemo で見えていたタイピングインジケーター差は、SP-024 S3 最小実装でキャラ別待機秒数へ切り替え済み。最終判断は Unity 実機確認待ち。

### 2026-04-15 session 2（Editor 整備 + テキスト表現仕様）

- **Editor メニュー統一**: 全 29 MenuItem を `Tools/FoundPhone/` 配下に統一 (12 ファイル変更)。サブメニュー: Scene Setup / Setup / Verification / Tests / Debug。Yarn Content Validator 重複削除
- **ChatUIConfig タイミング集約**: typewriterSpeed / typingIndicatorDuration / postMessageDelay / enableTypewriterEffect / enableTapSkip を SO に移行 (3 ファイル)。画像フェードイン 0.6f ハードコード修正
- **SP-023 テキスト表現仕様** (新設 `23_text_presentation.md`): BubbleMargin (% 指定)、BubbleStylePreset (7 プリセット)、narration (地の文)、IconSide (アイコン向き)、フリックスレッド切替、サブクエスト統合 (SP-016/017/022/BL-003)、B/C/D 保留領域 (S10-12)
- **SP-024 チャット没入仕様** (`24_chat_immersion.md`): タイムスタンプ、既読/配信マーク (DeliveryStatus)、キャラ別タイピングパターン (TypingSpeed 7 段階)、オンライン状態 (OnlineStatus)、メッセージ削除痕。2026-04-21 時点で S1/S2/S3/S5 は UI 接続と Save/Load 復元まで反映済みで、残りは S4 のオンライン状態 UI。
- **SP-023 Worker 実装**: BubbleStylePreset SO / IconSide / フリック切替の .cs が未コミットで存在。検収後にコミット予定
- **spec-index**: SP-023 + SP-024 追加 (計 42 エントリ)

### 2026-04-09（A レーン — Ch1 終端強化）

- **Yarn**: `Assets/Resources/Yarn/active/Ch1_Day1.yarn` の `Ch1_Day3_End` に、Pyramid 独白で「端末外観測が次の材料」「偵察拡大で照合解像度」の短文を追加（`<<EndDay 3>>` 前・新トピックなし）。
- **仕様**: [`docs/StorySpec/03a_ch1_section_beats.md`](StorySpec/03a_ch1_section_beats.md) Day3 Winding 節を実装と整合。
- **検証（ユーザー・当該コミットの一次確認）**: A レーン変更の確認として Validator → Sync → ContentAuthoring で Day3 終端まで再生。**セッションごとの常時義務ではない**（通常は静的整合・局所再生で足りる場合が多い。長尺通しは SUBSEQUENT 発動時または `HANDOFF.md` Safe Next Steps **1b** の任意条件に従う）。

### 2026-04-09（F レーン完了 → 本開発復帰）

- **並行 F レーン**: クローズ。以降の実行計画は `docs/project-context.md` の **CURRENT LANE / CURRENT SLICE**（Content 主、Unlock 副）を正とする
- **参照固定**: 検証・CI の索引は [2026-04-09-f-lane-audit-evidence-index.md](archive/verification-lanes-2026-04/2026-04-09-f-lane-audit-evidence-index.md)

### 2026-04-09（F レーン — Audit / Evidence）

- **verification**: [docs/archive/verification-lanes-2026-04/2026-04-09-f-lane-audit-evidence-index.md](archive/verification-lanes-2026-04/2026-04-09-f-lane-audit-evidence-index.md) を新設（PlayMode / CI / Ch1・SUBSEQUENT 証跡の索引、再開読書順）
- **整合**: `spec-index.json` は **41** エントリ（Python 実測）。`spec_entries: 42` を **41** に修正
- **コード・Yarn・シーン**: 変更なし（読み取り監査のみ）

### 2026-04-09（UI: ダッシュボード／インベントリレイアウト + ミニマル配色）

- **レイアウト**: `InventoryTabController` の親パネル上端を **200px** インセットに変更（`DashboardController` チャンネル ScrollView の `-200` と整合）。TabBar（-150〜-190）と InventoryRoot の縦重なりを解消。
- **見た目**: `DashboardController` / `InventoryTabController` / `ThreadSwitcherController` / `ProgressSummaryUI` の背景・カード色を低コントラスト寄りに統一（実行時負荷の増加なし）。
- **検証**: Unity 目視確認は SUBSEQUENT 発動時 (`docs/UI_ISSUES.md` に記録)。

### 2026-04-09（セッション引き継ぎ・リモート同期）

- **Git**: `main` と `origin/main` を fetch で突き合わせ。追跡ファイルの未プッシュ差分はなし。ルートの計測 NDJSON を `debug-*.log` として `.gitignore`
- **ドキュメント**: `HANDOFF.md`（Handoff snapshot）、`project-context.md`（直近の状態 1 行）、本ファイルの Updated / Session Log
- **コード・Yarn**: 変更なし

### 2026-04-09（次回推奨プラン実行）

- **再開ゲート**: `HANDOFF` / `project-context` / `runtime-state` を起点に次回実行順を固定
- **verification**:
  - `SUBSEQUENT_playthrough_and_tests.md` に PlayMode 回帰ベースライン参照を追記
  - `2026-04-09-playmode-8-results.md` を新規作成（8/8 pass の基準記録）
- **SP-022/03a**:
  - SP-022 §6.4 に P0/P1/P2 の初期優先度を追加
  - 03a に SUBSEQUENT→LATER の移行判定メモを追加

### 2026-04-10（SUBSEQUENT 完了 → Ch2 分岐プラン）

- **正本**: `docs/verification/2026-04-10-subsequent-completion-report.md` (Ch1 再現手順、静的整合、分岐表)
- **更新**: `SUBSEQUENT_playthrough_and_tests.md` / `2026-04-10-ch1-day1-3-preflight.md` 節 C / `2026-04-09-playmode-8-results.md`（再実行欄）/ SP-022 §6.4.1 / `03a` / `2026-04-08-ch1-subquest-gap-template.md` / `HANDOFF.md` / `17_unlock_triggers.md` §6 先頭
- **判定**: Editor 実測まで P0 有無は未確定。実測 P0 なし → LATER（Ch2）。P0 あり → 短い P0 のみ

### 2026-04-10 (Content レーン — Ch1 Day3 + 検証・CI 導線)

- **Ch1**: `Ch1_Day1.yarn` に Day3（`Ch1_Day3_*`）を追加。`ch1.asset` の `m_TotalDays: 3` と Day 開始ノードを更新
- **Day2 Winding**: `fragment_ch1_02` の `UnlockTopic`（03a の断片 #2 導線）
- **SP-022**: Day3 パイロット（`scout_ch1_d3_route` / `scout_ch1_d3_board` / `annot_ch1_d3_compare`）。`03a` / `22` / `17` を同期
- **SUBSEQUENT**: `docs/verification/templates/SUBSEQUENT_playthrough_and_tests.md` チェックリスト新設
- **LATER**: `docs/StorySpec/LATER_CH2_PLAYBOOK.md` オペレーション短冊
- **CI**: `.github/workflows/unity-playmode-tests.yml` 新設。EditMode ワークフローの Unity 版を `6000.3.6f1` に合わせる

### 2026-04-10 (docs cleanup phase 2-4)

- レガシー文書整理を継続し、重複 wiki ページを正典へ移植後に削除
- `docs/EVIDENCE_REUSE.md` を新設し、archive 側 Evidence Reuse 文書を統合
- `docs/archive/ROADMAP_TO_PRODUCTION.md` を要約移植後に削除
- docs/wiki 第4弾で `characters` / `branch` / `chapter-patterns` / `ui-config` / `troubleshooting` を削除し、wiki をポータル最小構成へ縮約
- `FEATURE_STATUS_AUDIT.md` の旧 archive 参照を「整理済み（履歴参照）」へ更新

### 2026-04-10 (LATER + 意思決定解説)

- **project-context.md** / **HANDOFF.md** / **runtime-state.md**: LATER スライス、意思決定解説、チェックリスト
- コード・Yarn 変更なし（当該コミット）

### 2026-04-09 (SUBSEQUENT + 手動ハンズオン)

- SUBSEQUENT スライス、HANDOFF 手動ハンズオン追記 等（ドキュメントのみ）

---

2026-04-08 以前の Session Log は [docs/archive/runtime-state-session-log-2026-03_04.md](archive/runtime-state-session-log-2026-03_04.md) に切出済み (session 10〜22)。
