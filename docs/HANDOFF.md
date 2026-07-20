# Handoff

会話ログなしで再開するための最短入口。

## まず読む

1. `docs/HANDOFF.md`
2. `docs/SUPERVISOR_REPORT.md`
3. `docs/project-context.md`
4. `docs/runtime-state.md`
5. `docs/INVARIANTS.md`
6. `docs/USER_REQUEST_LEDGER.md`

## Handoff snapshot (2026-07-21 private Sites runtime / cross-terminal sync)

**本セッションの実施内容**:

- 開始時の`main` / `origin/main`は`1e582639b5730e787debd20d2131147e6515032d`、ahead/behind `0/0`、worktree clean。`git fetch --prune origin`後も同一だった。
- Sites project `appgprj_6a5ddb11908081918180da7797957e63`を再取得し、active、`custom` access、allowlist user 1、workspace/tenant group 0、Version 1、deployment succeededを確認した。Ownerの識別子、短期credential、token、期限付きURLは保存していない。
- static input `sites/foundphone-demo/`はVinext / React / Worker互換へ等価変換済み。Sites source commitは`29c5245d1f0a21802a98c0009c038f48cd746eca`で、Version 1のsource SHAと一致する。変換sourceのmachine-local worktreeはcleanだが、別端末の正本にはしない。
- local Worker runtimeで両分岐、異なるcontinuation/outcome、restart、Enter/Space、focus、ARIA live/progress、reduced-motion、320–430px横overflowなしを確認。`content/demo.json`はbuild-time importの表示正本で、外部通信、auth、storage、database、form、analytics、payment、secret、live store linkは追加していない。
- private deployment APIの結果はSites control plane上で`type=publish` / `current_live_url`、`current_preview_url=null`と表現される。これはproduction-class hosted URLだが、accessはOwner-onlyのままでpublic access、workspace共有、custom domainは行っていない。
- 未認証でprivate URLを開くとSites標準の`Continue with ChatGPT` gateが表示された。認証自動化禁止に従ってOwner sign-inは行わず、実hosted app本文の最終reviewはUser / Web Supervisorへ残した。
- 詳細、変換対応、ID、archive provenance、別端末の再取得手順は`docs/verification/2026-07-20-sites-private-runtime-validation.md`へ固定した。

## Handoff snapshot (2026-07-19 Sites-native demo repository integration)

**本セッションの実施内容**:

- 開始時のprimary `main` / `origin/main`は`b4e92ecb4f05a923b9177138fcf026fcfb561bba`、ahead/behind `0/0`、worktree clean。`git fetch --prune origin`後も同一だったため、`integrate/sites-native-demo`で統合した。
- read-only source sibling `UnityChatNovelGame-sites-native-chat-demo`の未追跡変更は、許可された`sites/foundphone-demo/**`、`tools/sites/**`、`docs/verification/sites-native-demo-validation.md`だけだった。10ファイルを実読し、統合直後のSHA-256が全件一致することを確認した。siblingは変更していない。
- `sites/foundphone-demo/`をtracked repository artifactにし、`tools/sites/serve-demo.ps1`と静的server/validatorを同梱した。内容は`non-canon verification fixture`で、Ch1/Yarn canon、backend、auth、storage、analytics、PII収集、決済、live store linkを含まない。
- port `4318`の一時HTTP serverでHTML/CSS/JS/JSONの`200`とMIME、青信号/白ノイズの異なるending、restart、desktop/390px/320px、横overflowなし、focus/live region/progress semantics、console warning/error 0、外部runtime URLなしを再確認した。serverは検証後に停止し、URLを稼働中とは扱わない。
- `SITES_IMPORT_BRIEF.md`にrepository path、entry point、local serve command、private preview checklistとpublic gateを固定した。actual ChatGPT Sites compatibilityとprivate accessは未検証で、次はHuman / Web Supervisorによるprivate preview gateである。Site作成・import・public deploymentは実施していない。
- 自動化されたEnter/Space activationは今回も状態遷移を証明できなかった。native button/link、focus移動、3px focus-visible ringは確認済みだが、実キーボード完走はprivate previewのhuman review debtとして残す。

## Handoff snapshot (2026-07-19 accepted parallel results integration)

**本セッションの実施内容**:

- 同期起点の `main` / `origin/main` は `55cb0d20c1d92a21e6e6ecb0db5d2fdbdfe562d7`、ahead/behind `0/0`、worktree clean。受入済み editor commit `23e4b635` がこの base の直系であることと、Yarn本文・Sites surface・package・Build Settingsを変更していないことを確認して統合した。
- Writer Cockpitに74 Nodeの検索、Yarn asset pathと1-based `title:` 行、file/line付きValidator drilldown、設定済みExternal Script Editorへのsource jumpを追加。Refresh / Validate / Sync / Validate Then Sync / Apply / Playとread-only Save statusは維持した。
- runtime command handlerとCharacterProfile assetをValidator registryのsource of truthにし、active Yarnの結果を `errors=0 / warnings=0 / info=3` へ更新。以前のwarning 33件は登録済みcommand 24件と既存`unknown` profile 9件のregistry driftだった。
- targeted `WriterCockpitNavigation` EditModeは14/14 pass。Unity 6000.4.9f1 batch open/compileと非破壊Yarn validatorはいずれもreturn code 0。full 83 testsとpersistent save dataに触れるtestは未実行。
- `docs/verification/sites-publication-feasibility.md` を正本へ追加。Unity Web Build Supportが未導入で、`MVPScene.unity`も存在せず、`TitleScene`から公開可能なgameplay sceneへ遷移できないため、direct Unity Web routeは未証明のままblocked。
- 公開面の次候補は別laneのSites-native lightweight chat demo。Site作成・公開、Unity module導入、public scene修復はこの統合では行っていない。
- source jump実装は安全にfail closedするが、この端末のUnity External Script Editorが未設定のため、実file/line移動は人間環境レビュー負債として残る。Apply / Playの共有処理もcompile上は維持したが、本統合では対話再生していない。

## Handoff snapshot (2026-07-17 remote sync / development readiness refresh)

**本セッションの実施内容**:

- 開始時は 2026-07-15 の検証記録4ファイルが未コミットで存在。差分をレビューし、code / Yarn / scene / asset の残留変更がないことを確認して保持した。
- `git fetch --prune origin` → `git pull --ff-only origin main` を実行し、pull は `Already up to date.`。同期起点の `main` / `origin/main` は `7eb09c0`、`HEAD...origin/main` は `0/0`。
- Unity 6000.4.9f1、`Packages/manifest.json`、`Packages/packages-lock.json`、`tools/run-unity.ps1` を確認。package JSON は parse pass。
- wrapper 経由の batch open は 39 packages の cache restore、Tundra success、0 items updated / 326 evaluated、batchmode 正常終了。compile / fatal error なし、Unity 実行由来の tracked 差分なし。
- 非破壊 Yarn validator は errors=0 / warnings=33 / info=3、11 files / 74 nodes。テスト定義は静的に EditMode 73 / PlayMode 10。全83テストは save data 隔離前のため未実行。
- docs viewer は `generate-doc-nav.ps1 -PrepareView` と `uvx --from mkdocs-material mkdocs build --strict` が pass。
- Unity 起動時に Verification メニューの重複登録警告が1件再現。compile blocker ではないが、`VerificationMenu` と `MissingScriptScanner` の同一 `MenuItem` は後続で整理する。
- active slice と actor 境界は変更なし。shared owner は Writer Cockpit の interactive Apply / Play 受入、assistant owner は Validator command drift 解消と安全な83テスト基準化。

## Handoff snapshot (2026-07-13 remote parity / cross-terminal resume)

**本セッションの実施内容**:

- `git fetch --all --prune` と `git pull --ff-only origin main` を実行。`main` / `origin/main` は `fa8eb8b` で一致し、`HEAD...@{u}` は `0/0`、開始時の作業ツリーは clean。
- コード、Yarn、scene、asset の新規変更はなし。2026-07-11 の development-readiness 状態と `docs/SUPERVISOR_REPORT.md` の G0-G13 をそのまま正本として維持。
- 別端末の最短再開は本ファイルの `Current Focus` / `Safe Next Steps`。最初の共有作業は Writer Cockpit の interactive 受入、次の assistant-owned work は Validator command drift 解消と現行83テスト基準化。
- ignored の `Library/` / `Logs/` はリモート引き継ぎ対象ではない。別端末では `tools/run-unity.ps1` を再現可能な Unity 入口として使う。

## Handoff snapshot (2026-07-11 remote sync / development readiness)

**本セッションの実施内容**:

- `git fetch --prune origin` → `git pull --ff-only origin main` で `51dc8bc` へ fast-forward。同期直後の `HEAD...origin/main` は `0/0`。
- fresh Package Manager resolve の `path undefined` は package JSON ではなく、Codex / PowerShell 子環境で標準 Windows 変数 `ALLUSERSPROFILE` が欠落していたことが直接原因。
- process-local に `C:\ProgramData` を補うと、ProjectCache 欠落状態から fresh resolve、39 packages 登録、script compile、return code 0 まで到達。
- `tools/run-unity.ps1` を追加。必要 Unity 版を `ProjectVersion.txt` から読み、環境変数は子プロセス内だけ補完する。
- `BuildSettingsHelper` の早期初期化で ContentAuthoring scene が末尾へ移動する問題を、AssetDatabase ではなくファイル存在確認へ変えて解消。wrapper 経由の再起動で Build Settings の内容差分なし。
- 非破壊 Yarn validator は errors=0 / warnings=33 / info=3、11 files / 74 nodes。warning 33件中24件は登録済みコマンドを未知扱いする偽陽性。
- 詳細な信頼評価、残作業、G0-G13 目標案は `docs/SUPERVISOR_REPORT.md`。

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

- 主目的: **受入済みWriter Cockpit navigationとOwner-only Sites runtimeを維持し、hosted app本文のOwner認証後reviewを閉じる**
- 現在: 74 Node検索、source path/line、diagnostic drilldown、active Yarn `errors=0 / warnings=0 / info=3`は受入済み。`sites/foundphone-demo/`はportableな静的入力正本、Sites Version 1はsource `29c5245d...`のprivate hosted runtimeとして存在する。
- 次の publication gate: User / Web SupervisorがOwnerとしてprivate URLを開き、hosted app本文の両分岐、restart、keyboard、network、320–430pxを確認する。Site作成、source push、Version 1、Owner-only deploymentは完了済みで、public/shared accessは未承認のまま。
- 次の shared review: 人間がUnity PreferencesでExternal Script Editorを選択し、Nodeとdiagnosticのsource jumpを各1回確認する。必要ならCockpitからApply / Playの既存interactive loopも再受入する。
- blocker: direct Unity Web routeはWeb Build Support欠落、有効なpublic gameplay scene欠落、TitleSceneの公開不可scene依存によりblocked。module導入・scene修復・公開は別判断。
- 注意: `Library/` / `Logs/` はignored local evidence。full 83 testsはsave data隔離前に実行しない。Sites packageは非canon fixtureであり、hosted runtimeが存在しても本編・最終ストーリー・public公開の受入を意味しない。

## Validation note (2026-07-20 Sites private runtime)

- URL: `https://foundphone-signal-preview.thankyoukass.chatgpt.site`。Sites標準sign-in gateの先はOwner manual review待ち。
- Access: `custom` / allowed user 1 / workspace group 0 / tenant group 0。2026-07-21再取得。
- Source/version: Sites source `29c5245d...` = Version 1 source SHA。deployment `appgdep_6a5ddc134b508191ab81ab6dd44765aa`はsucceeded。
- Runtime mapping: static HTML/CSS/JSをReact/Vinextへ変換し、`content/demo.json`をbuild-time import。internal README/briefは非表示。
- Validation: `npm run lint`、`npm test`（build + 3/3）、local Worker両分岐、restart、keyboard/focus、responsive、prohibited-capability auditをpass。
- Remaining: Owner sign-in後の実hosted本文、network panel、両分岐の最終OK/NG。public/shared access、custom domainは禁止継続。
- Detail: `docs/verification/2026-07-20-sites-private-runtime-validation.md`。

## Validation note (2026-07-19 Sites-native demo integration)

- Repository package: `sites/foundphone-demo/`、entry point `index.html`、local wrapper `tools/sites/serve-demo.ps1`。
- Static validation: JSON parse、7-node graph、choice targets、全node到達性、JS/server/wrapper syntax、禁止pattern auditはpass。
- HTTP: port `4318`で`/`、`/styles.css`、`/app.js`、`/content/demo.json`が`200`。MIME、`nosniff`、`no-store`を確認。
- Browser: 青信号/白ノイズの異なるending、restartで`進行 1 / 6`・未選択・message 1件へ初期化、desktop `1280x900`、mobile `390x844`、narrow `320x700`で横overflowなし。
- Accessibility: skip link、native controls、choice focus移動、3px focus-visible、aria-live、progressbar semantics、reduced-motion CSSを確認。Enter/Space自動化は未証明でhuman review debt。
- Boundaries: console warning/error 0、外部runtime URL/resource 0、form/input/auth/storage/analytics/payment/secret/live destinationなし。actual Sites private preview/public deploymentは未実施。

## Validation note (2026-07-19 parallel integration)

- Targeted EditMode: `ProjectFoundPhone.Editor.Tests` / `WriterCockpitNavigation` 14/14 pass。
- Unity: 6000.4.9f1 batch open/compile return code 0。実行後のvalidation worktreeにtracked差分なし。
- Yarn: `errors=0 / warnings=0 / info=3`、11 files / 74 nodes / 24 `#line:` tags / 42 declared variables。unknown command / unknown character偽陽性は0。
- Source navigation: `SP024_Immersion_Start` → `Assets/Resources/Yarn/active/SP024_ImmersionDemo.yarn:13`の表示はaccepted laneで確認済み。設定済みExternal Script Editorで開く実動作は未確認。
- Publication: `docs/verification/sites-publication-feasibility.md` がWeb module / public scene / navigation blockerを記録。Unity Web compatibilityやSites公開成功は主張しない。

## Validation note (2026-07-17 current checkout)

- Git: `main` / `origin/main` は `7eb09c0`、ahead/behind `0/0`。Unity 実行由来の code / scene / asset 差分なし。
- Unity: `Logs/development-readiness-unity-open-2026-07-17.log` で 39 packages、Tundra success、326 evaluated、batchmode 正常終了。
- Yarn: `Logs/development-readiness-yarn-validator-2026-07-17.log` で errors=0 / warnings=33 / info=3、11 files / 74 nodes / 24 `#line:` tags / 42 declared variables。
- 静的テスト定義: EditMode 73 / PlayMode 10。実行結果ではない。
- 非阻害 warning: Verification メニューの同名 `MenuItem` 重複、UnityConnect の CDN timeout、MkDocs の将来互換性告知。いずれも今回の終了コードを失敗にはしていない。
- 未確認: Writer Cockpit interactive menu / window / Apply / Play、現行83テスト、SP-023/024 visual acceptance。

## Validation note (2026-07-11 development readiness)

- Launcher: `tools/run-unity.ps1`
- Fresh resolve / compile: `Logs/dev-readiness-fixed-env-unity-open-2026-07-11.log`
- Wrapper compile recheck: `Logs/dev-readiness-wrapper-open-2026-07-11.log`
- Yarn validator: `Logs/dev-readiness-wrapper-yarn-validator-2026-07-11.log`
- Result: fresh resolve 42.78秒、39 packages、Tundra success、return code 0。
- Validator: errors=0 / warnings=33 / info=3。unknown command 24件は現 runtime 登録と Validator の known list のドリフト。
- 未確認: interactive menu / window / Apply / Play、現行83テスト、SP-023/024 visual acceptance。

## Validation note (2026-07-06 Writer Cockpit)

- Unity executable: `C:\Program Files\Unity\Hub\Editor\6000.4.9f1\Editor\Unity.exe`
- Batch logs:
  - `Logs/writer-cockpit-unity-open-2026-07-06.log`
  - `Logs/writer-cockpit-unity-open-2026-07-06-rerun.log`
  - `Logs/writer-cockpit-yarn-validator-2026-07-06.log`
- Recovery logs:
  - `Logs/writer-cockpit-cache-utc-timestamp-restored-2026-07-06.log`（batch open / compile / return code 0）
  - `Logs/writer-cockpit-final-yarn-validator-2026-07-06.log`（Yarn validator batch: errors=0, warnings=33, info=3）
- 2026-07-07 recheck logs:
  - `Logs/writer-cockpit-unity-open-2026-07-07.log`（Package Manager cache restore / 39 packages / script compile / return code 0）
  - `Logs/writer-cockpit-yarn-validator-2026-07-07.log`（Yarn validator batch: errors=0, warnings=33, info=3 / 11 files / 74 nodes）
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

1. User / Web SupervisorがOwnerとして`https://foundphone-signal-preview.thankyoukass.chatgpt.site`を開き、Sites標準sign-in後のhosted app本文を確認する。
2. Owner-only accessを変えずに、両分岐、restart、desktop/mobile、実キーボード、networkを再確認し、actual hosted runtimeをOK/NGで記録する。
3. public access、workspace共有、custom domain、live store link、PII/auth/payment追加は別のHuman Gateまで行わない。
4. Unity laneでは`.\tools\run-unity.ps1`でUnity **6000.4.9f1**を開き、External Script Editor jumpと必要なWriter Cockpit Apply / Playを再受入する。
5. save dataを退避または隔離した後、EditMode 73 / PlayMode 10の現行結果を記録する。
6. ここまで通ったらSP-023 3ノード → `SP024_Immersion_Start`の表示検収へ進み、M1 → M2 → M3のgate前にfull Ch1 authoringへ進まない。

補足:
- SP-023 仕様: `docs/StorySpec/23_text_presentation.md`
- SP-024 仕様: `docs/StorySpec/24_chat_immersion.md`
- PlayMode 8 件の回帰ベースライン: `docs/verification/2026-04-09-playmode-8-results.md`。2026-06-08 時点では PlayMode フォルダに追加 2 件があるため、次回結果は日付付き新ファイルで記録する。

## Source Of Truth

- 監修役AI向け現状・目標案: `docs/SUPERVISOR_REPORT.md`
- Sites private runtime検証・再取得: `docs/verification/2026-07-20-sites-private-runtime-validation.md`
- 方針・スライス: `docs/project-context.md`
- 全体概観: `docs/PROJECT_STATUS_DASHBOARD.md`
- ターン単位プラン: `docs/DEVELOPMENT_TURN_PLAN.md`
- 画面証跡索引: `docs/VISUAL_PROGRESS_INDEX.md`
- 作業状態: `docs/runtime-state.md`
- 決定履歴: `docs/DECISION_LOG.md`
- 制作フロー: `docs/OPERATOR_WORKFLOW.md`
- SP-023 / SP-024 表示系デモ計画（修正版・監査済み）: `docs/plans/display-batch-showcase.md`
