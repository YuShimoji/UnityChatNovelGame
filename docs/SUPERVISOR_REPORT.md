# 監修役AI向け現状報告

**更新日**: 2026-07-27
**対象**: FoundPhone / UnityChatNovelGame
**同期identity**: 同期開始base `2f3753495dbb4dbdce1bf3fac763a057ac1442d2`。現在は本正本を含む`HEAD`が`origin/main` / GitHub readbackとahead/behind `0/0`
**役割**: 監修役AIが会話ログなしで、信頼できる現在地、残る判断、推奨順序、製品化までの目標を判断するための引き継ぎ正本。

## 1. 結論

このリポジトリは、**primary mainがremote parityにあり、2026-07-26のUnity / Yarn / docs / static fixture基準から開発を再開できる状態**である。さらに、remote review branch `origin/codex/sites-authoring-bridge-v1@e059e4b`は、2026-07-27にUnity compile、targeted Editor 18/18、Writer Cockpit direct window、fixture/generated local preview accessまで実証した。candidateはmainへ未統合で、人間受入、merge、hosted/public releaseを意味しない。direct Unity Web routeはblocked、基礎83件の全回帰は実セーブ隔離待ちである。

ただし、現在の受け入れ状態は次のように分ける。

| 判定対象 | 現在の判定 | 根拠 |
|---|---|---|
| Git / ソース同期 | 開発可能 | 同期開始baseは`2f37534`。本正本を含む現在の`HEAD` / `origin/main` / GitHub readbackはahead/behind `0/0`。review branchのlocal / originは`e059e4b` |
| Unity batch open / compile | 開発可能 | 2026-07-26にUnity 6000.4.9f1、39 packages、Tundra success、334 evaluated、return code 0。tracked差分なし |
| agent / terminal からの再現 | 開発可能 | `tools/run-unity.ps1` が欠落した標準 Windows 環境変数を子プロセス内だけ補完 |
| Yarn 静的検証 | active Yarnで信頼回復 | errors=0 / warnings=0 / info=3。以前の33 warningsはcommand/character registry driftとして解消 |
| Writer Cockpit navigation | lane受入 | 74 Node検索、source path/line、diagnostic一覧をaccepted laneで確認。targeted EditMode 14/14 |
| Sites authoring bridge candidate | review-ready / main未統合 | `e059e4b`でPackage v1 export action、Unity compile、Editor 18/18、direct window、fixture/generated HTTP/browser accessを確認 |
| External editor jump | 人間環境レビュー待ち | fail-closed実装は確認済み。Unity External Script Editor未設定のため実file/line移動は未証明 |
| 現行 Unity テスト全体 | 定義97件 / 全体結果未更新 | 基礎EditMode 73 + PlayMode 10、targeted Editor 14。Editor 14/14は2026-07-19、基礎83はsave data隔離待ち |
| Unity Web publication | blocked / 未証明 | Web Build Supportなし、有効なpublic gameplay sceneなし。Sites-native demoを別laneの後継とする |
| Sites-native hosted runtime | private hosted / Owner review待ち | tracked static input、Sites source `29c5245d...`、Version 1、custom user 1/group 0、deployment success。local Workerで両分岐・keyboard・responsiveを確認 |
| 最終モバイル製品 | 未到達 | Android/iOS build、署名、配布、Ch3-9、サウンド、広告、Beta は後続 |

作者基盤の次は、**User / Supervisorがexact bridge candidateを作者導線としてOK/NG判断し、assistantがmain上でsave dataを隔離して基礎83件を現行基準化する**。bridge受入後もmain統合は別判断である。Owner-only hosted本文reviewは並行可能だが、public/shared access、custom domain、Unity module導入、public scene修復は別のHuman Gateである。

## 2. Shared Focus / 北極星

### 制作基盤

ライター／デザイナーが外部エディタで Yarn を書き、Unity Editor 上で次のループを迷わず回せること。

```text
Yarn 保存
  → Refresh Nodes
  → Node検索 / source path・line確認
  → Validate / file・line診断
  → SO Sync
  → Start Node 選択
  → ContentAuthoring Apply / Play
  → 結果確認
```

AI の主担当はエンジン、ツール、パイプライン、検証導線である。Yarn 本文、キャラクターの語り口、感性調整、サブクエストの必須／任意判断はユーザー所有である。

### 製品

FoundPhone を、モバイル優先のチャット／ビジュアルノベルゲームとして iOS / Android に配布可能な状態へ持っていく。F2P + 広告、Ch3以降のサウンド統合は高位方針として存在するが、現在の実装スライスではない。

## 3. 今回の同期と復旧

### 2026-07-27 remote parity / Sites authoring bridge access

- `git fetch --prune origin`後の同期開始baseは、primary `main`、`origin/main`、GitHub `refs/heads/main`が`2f3753495dbb4dbdce1bf3fac763a057ac1442d2`で一致し、ahead/behind `0/0`、tracked worktreeはcleanだった。pull対象はなかった。本正本を含む現在の`HEAD`もnormal push後に`origin/main` / GitHub readbackとahead/behind `0/0`を再確認した。
- `73aef720..2f37534`は2026-07-26 development-readinessの正本9件だけで、source、Yarn、scene、Packages、ProjectSettingsを変更しないdocs-only commitとしてremote反映済み。
- review branch `codex/sites-authoring-bridge-v1`、`origin/codex/sites-authoring-bridge-v1`は`e059e4b758f9e39641b5368301ede837f930724d`で一致し、親は同期開始base `2f37534`。19 files、1545 insertions / 30 deletionsのcandidateで、現在のmainへ未統合。
- exact candidateをUnity 6000.4.9f1でbatch open/compileしreturn code 0。`ProjectFoundPhone.Editor.Tests`は18/18 pass。visible Editorをexact project pathと`WriterCockpitWindow.ShowWindow`で起動し、Writer Cockpitと`Export Sites Preview Package`を実画面で確認した。
- candidateのtracked serve wrapperをport 4327で起動し、fixture / CSS / JS / demo JSON / legitimate generated Package v1 / query routeのHTTP 200、MIME、`nosniff`、`no-store`を確認。fixtureとgeneratedをブラウザで開始し、console warning/error 0。検証serverは停止した。
- 過去の`recipient_open=failed`時に開かれていたproject pathとstderrは残っていないため、過去原因は断定しない。現在はexact remote branch、commit、Unity direct window、preview commandが検証済み。
- この同期はPR、merge、tag、release、deploy、Sites source/version/access/public visibilityを変更していない。candidateの技術greenはmain統合、人間受入、hosted compatibility、public releaseの承認ではない。

### 2026-07-26 remote sync / development readiness

- `main` / `origin/main`は`73aef720`、ahead/behind `0/0`、worktree cleanで開始。`fetch --prune`後の`pull --ff-only`は`Already up to date.`で、取り込む新規commitはなかった。
- Unity 6000.4.9f1をwrapperからbatch起動し、39 packages登録、Tundra compile success、334 evaluated、return code 0、code / scene / asset差分なしを確認した。
- Yarn validatorはerrors 0 / warnings 0 / info 3、11 files / 74 nodes。static Sites fixture validatorとstrict docs buildもpassした。
- 現行テスト定義は基礎EditMode 73 + PlayMode 10 + targeted Editor 14 = 97件。実セーブを削除するtestがあるため、基礎83件と全97件の一括実行は隔離前に行っていない。
- Sites control planeはread-onlyで再取得し、active、custom access、allowed user 1、group 0、Version 1、deployment succeededを確認した。access / deployment /公開設定は変更していない。
- 詳細なコマンド、境界、warningは`docs/verification/2026-07-26-development-readiness.md`を正本とする。

### 2026-07-21 Sites private runtime / cross-terminal sync

- `main` / `origin/main`は開始時`1e582639`、ahead/behind `0/0`、clean。fetch後も同一。
- Sites projectはactive、access mode `custom`、allowlist user 1、workspace/tenant group 0。Owner個人情報とcredential/tokenは保存していない。
- Sites source `29c5245d...`はVersion 1のsource SHAと一致し、deploymentはsucceeded。static inputからReact/Vinext/Workerへの変換対応、ID、archive provenanceを日付付きverificationへ固定した。
- local Workerで`blue_signal` / `white_noise`の異なるcontinuation/outcome、restart、Enter/Space、focus、ARIA、320–430px、禁止機能監査をpass。
- private deployment APIの結果はSites上で`type=publish` / `current_live_url`と表現されるが、accessはOwner-onlyでpublic/shared accessではない。`current_preview_url`は`null`。
- 未認証URLはSites標準sign-in gateまで確認。自動sign-inは行わず、hosted app本文のOwner reviewを残した。

### 2026-07-19 Sites-native demo repository integration

- primary `main` / `origin/main`は開始時`b4e92ecb`、ahead/behind `0/0`、clean。fetch後も同一で、`integrate/sites-native-demo`へ分離して統合した。
- source siblingはbase `55cb0d20`の`spike/sites-native-chat-demo`で、変更は許可surfaceの未追跡10ファイルのみ。全ファイルを実読し、統合直後のSHA-256が全件一致した。siblingはread-onlyのまま。
- `sites/foundphone-demo/`と`tools/sites/`をtracked artifact化。contentは`non-canon verification fixture`で、Ch1/Yarn canon、external dependency、backend/auth/storage/analytics、PII、payment、secret、live store linkなし。
- JSON/graph/JS/server/wrapper/prohibited-pattern、HTTP 200/MIME、青信号/白ノイズ両ending、restart、desktop/390px/320px、overflow、focus/live/progress、consoleを再検証。自動Enter/Space activationだけは未証明。
- `SITES_IMPORT_BRIEF.md`を実装と一致させ、repository path、entry point、serve command、private preview checklist、public/monetization boundaryを明示。actual Sites compatibility、private preview、public deploymentは未実施。

### 2026-07-19 parallel lane integration

- editor accepted commit `23e4b635`の親がbase `55cb0d20`であること、Yarn本文・Sites surface・package・Build Settingsを変更していないことを確認し、一時integration branchへ競合なくcherry-pickした。
- Sites siblingは`docs/verification/sites-publication-feasibility.md`だけが未追跡で、その他の変更なし。同文書だけを読み取り統合し、sibling worktreeは変更していない。
- Writer Cockpit navigationのaccepted evidenceは74 Node検索、source asset/line表示、structured diagnostics、active Yarn `errors=0 / warnings=0 / info=3`。External Script Editor実jumpは未確認のまま維持する。
- Unity Web routeはWeb Build Support、valid public gameplay scene、TitleSceneからのpublic navigationが欠ける。Sites-native lightweight chat demoをpublic deploymentなしの別lane successorとする。

### リモート同期

- 2026-07-17 の開始時は、2026-07-15 の検証記録4ファイルが未コミット。差分をレビューし、code / Yarn / scene / asset の残留変更がないことを確認して保持した。
- `git fetch --prune origin` と `git pull --ff-only origin main` を実行し、pull は `Already up to date.`。
- `main` / `origin/main` は `7eb09c0`、同期直後の tracked parity は `0/0`。
- 2026-07-13 の最新コミットは cross-terminal handoff 更新のみ。コード、Yarn、scene、asset の追加変更はない。

### Package Manager 根因

従来の記録は、`Library/PackageManager/ProjectCache*` と source timestamp を合わせた生成キャッシュ依存の復旧だった。今回のローカルにはその ProjectCache がなく、通常 batch open は次で停止した。

```text
The "path" argument must be of type string. Received undefined
Failed to resolve packages
return code 1
```

UPM の hidden `resolve` CLI を診断レベルで実行した結果、stack trace は `getDeprecatedGlobalConfigRoot()` 内の `path.join(... )` を指した。この Codex / PowerShell 子環境では標準 Windows 変数 `ALLUSERSPROFILE` が未定義だった。

`ALLUSERSPROFILE=C:\ProgramData` を**対象プロセス内だけ**補完すると、同じ manifest / lock で隔離 resolve は exit 0。実リポジトリでも ProjectCache なしの状態から fresh resolve、39 packages 登録、script compile、return code 0 まで到達した。

### 再開導線

`tools/run-unity.ps1` を追加した。

- `ProjectSettings/ProjectVersion.txt` から必要 Unity 版を読む。
- 呼び出し元で `ALLUSERSPROFILE` が欠落している場合だけ、`CommonApplicationData` を子プロセス用に設定する。
- user / system 環境変数は変更しない。
- interactive、batch open、`-executeMethod` を同じ入口で実行できる。

### Build Settings の自動差分

fresh resolve 後の初回初期化で、`BuildSettingsHelper` が `ContentAuthoring.unity` を末尾へ移動し、tracked `EditorBuildSettings.asset` を汚す現象を再現した。

原因は `[InitializeOnLoad]` の早期段階で `AssetDatabase.LoadAssetAtPath` が null を返し、ContentAuthoring を required scene から一時的に除外したこと。ファイルシステム上の存在確認へ変更し、再度 batch open して scene 順が変化しないことを確認した。

## 4. ライブ検証

### 確認済み

1. **package resolve / restore**
   - 2026-07-11: ProjectCache 欠落状態から fresh resolve 42.78秒、39 packages、return code 0
   - 2026-07-17: 現 checkout で 39 packages の cache restore を再確認
   - 2026-07-26: 現 checkoutで39 packages登録、return code 0を再確認
   - `dev.yarnspinner.unity@3.1.3`
   - `com.unity.nuget.newtonsoft-json@3.2.2`
   - log: `Logs/development-readiness-unity-open-2026-07-26.log`（ignored local evidence）

2. **script compile**
   - 2026-07-26: Tundra build success
   - 1 item updated / 334 evaluated
   - batchmode 正常終了、C# compile error なし

3. **Package Manager local state**
   - `ProjectCache` / `ProjectCache.md5` / `projectResolution.json` を再生成
   - すべて ignored local evidence。リモート正本にはしない

4. **Yarn validator**
   - 2026-07-26 に現checkoutをwrapperから再実行
   - errors=0 / warnings=0 / info=3
   - 11 files / 74 nodes / 24 `#line:` tags / 42 declared variables
   - 変更前warning 33件: registered command 24、既存`unknown` CharacterProfile 9。変更後のunknown command / character偽陽性は0
   - info はproject settings由来の可能性があるundeclared variable通知。errors / warningsとして扱わない
   - log: `Logs/development-readiness-yarn-validator-2026-07-26.log`（ignored local evidence）

5. **静的集計**
   - spec entries: 42
   - done 24 / partial 12 / draft 2 / todo 4
   - 基礎EditMode 73 / PlayMode 10 / targeted Editor 14、合計97
   - 2026-07-19のtargeted Editorは14/14 pass。基礎83件は今回未実行
   - 実コード TODO: `ChatController` の将来 status routing 1件

6. **ドキュメント閲覧面**
   - 2026-07-26: `generate-doc-nav.ps1 -PrepareView` 完走
   - `uvx --from mkdocs-material mkdocs build --strict` exit 0
   - ignored PerformanceBaseline raw pagesは nav 外の INFO。欠落ファイル参照は削除

7. **Writer Cockpit targeted integration**
   - 2026-07-19: `ProjectFoundPhone.Editor.Tests` 14/14 pass
   - Node index/source line/filter、known/unknown registry、active Yarn false-positive 0、安全なmissing-file処理を確認
   - 2026-07-26: Unity 6000.4.9f1 batch open/compile return code 0、実行後tracked code / scene / asset差分なし

8. **Publication feasibility**
   - `docs/verification/sites-publication-feasibility.md`をtracked authorityへ統合
   - Unity Web build/package/HTTP/MIME/browser runtime/Sites compatibilityは未検証
   - Site作成・deployment、Unity module導入、Build Settings変更は未実施

9. **Sites-native package / hosted runtime**
   - `sites/foundphone-demo/`、`tools/sites/`、`docs/verification/sites-native-demo-validation.md`をtracked artifactとして統合
   - local static packageのHTTP/MIME、両分岐、restart、desktop/mobile/narrow、accessibility smoke、禁止機能監査をpass
   - Vinext/React/Workerへ等価変換し、`npm run lint`、build + Node tests 3/3、local Workerの両分岐・keyboard・responsiveをpass
   - 2026-07-26にSites projectをread-only再取得。Version 1、source SHA、custom allowed user 1 / group 0、deployment successを維持。hosted本文だけOwner sign-in後review待ち

10. **Sites authoring bridge review candidate**
   - remote branch / local branch: `codex/sites-authoring-bridge-v1`
   - exact SHA: `e059e4b758f9e39641b5368301ede837f930724d`、parent `2f37534`
   - Unity 6000.4.9f1 compile return code 0、targeted Editor 18/18
   - direct windowでWriter CockpitとPackage export actionを可視確認
   - Package schema `foundphone.sites-preview-package` v1、verified node `SP023_NarrationMargin_Start`
   - fixture / generated local previewはHTTP・browser smokeをpassし、server停止済み
   - main未統合。candidateの人間受入、PR/merge、hosted Sites更新は未実施

11. **非阻害 warning**
   - Unity: `VerificationMenu` と `MissingScriptScanner` が同じ `Tools/FoundPhone/Verification/Scan DebugChatScene Missing Scripts` を登録
   - UnityConnect: 終了時の public CDN request timeout
   - docs: `uvx` の provider 推奨と MkDocs 2.0 将来互換性告知
   - いずれも今回の compile、validator、strict docs build の終了コードを失敗にはしていない

### 未実行 / 人間環境レビュー

- 設定済みExternal Script EditorでのNode/diagnostic file-line jump。
- Writer CockpitのApply / Play / Last Actionを含む既存loopの本統合後再実行。
- review branch `e059e4b`での実Export操作と作者UXのOK/NG、およびmain統合判断。
- 基礎EditMode 73 / PlayMode 10、およびtargeted Editor 14を合わせた全97件の同日実行。
- SP-023 / SP-024 の画面検収。
- Save / Load state equality と章遷移の横断検証。
- Unity Web build、Sites hosted本文のOwner sign-in後review、Sites public/shared access。

全テストを直ちに実行しなかった理由は、`SaveSystemTests`とPlayMode helperが`Application.persistentDataPath`の`SaveData_*.json`を削除するため。実行前に実ユーザーデータ退避またはtest data隔離が必要である。

## 5. Current Trust Assessment

### trusted

- 2026-07-27にprimary `main`、`origin/main`、GitHub readbackが`2f37534`で一致し、ahead/behind 0/0、tracked cleanだったこと。
- review branchのlocal / originが`e059e4b`で一致し、親が現main `2f37534`であること。
- exact candidateのUnity compile return code 0、targeted Editor 18/18、direct Writer Cockpit window、fixture/generated HTTP/browser access。
- accepted editor commitがbaseの直系で、許可されたEditor/test/verification filesだけを変更していたこと。
- 2026-07-26の現checkoutで39 packages、Tundra compile、batchmode正常終了を再確認したこと。
- 2026-07-19 のtargeted EditMode 14/14、batch compile、Yarn validator `errors=0 / warnings=0 / info=3`。
- `ALLUSERSPROFILE` 欠落が fresh resolve の直接原因だったこと。
- process-local 補完で隔離 resolve と実プロジェクト fresh resolve が成功したこと。
- Unity 6000.4.9f1 の package registration と script compile。
- wrapper 経由の再実行が cache restore と compile success に到達したこと。
- Writer CockpitのNode検索、source location、diagnostic drilldownのaccepted interactive evidence。
- runtime handler / CharacterProfile registryから既存false-positiveが0になることと、故意のunknownを回帰testが検出すること。
- Sites feasibility文書が記録するmodule/scene/navigation blocker。これはUnity Web compatibility成功の証明ではない。
- Sites Version 1がsource `29c5245d...`を参照し、deployment statusがsucceededであること。
- Sites access policyがcustom、allowlist user 1、workspace/tenant group 0であること。
- local Worker runtimeで両分岐、restart、Enter/Space、focus、ARIA、320–430px、禁止機能監査がpassしたこと。

### needs re-check

- 設定済みExternal Script Editorでの実file/line jump。
- Writer CockpitのApply、Play、Last Actionを含む既存loopの本統合後interactive再確認。
- `e059e4b`の実Export操作と作者UX human acceptance。技術greenだけでmain統合へ進めない。
- 基礎83件とtargeted Editor 14を合わせた現行97件のUnity 6000.4.9f1ベースライン。
- SP-023 / SP-024 の実表示、日本語 SDF、IconSide。
- Save / Load、Unread、Branch、削除痕、EndDay、章遷移の状態同値。
- fresh clone / 別端末。今回証明したのはこの端末と wrapper 経路。
- Verification メニューの同名 `MenuItem` 重複。Writer Cockpit の compile blocker ではないが、Editor menu の診断ノイズになる。
- Direct Unity Web build / package size / MIME / browser runtime。Web moduleとpublic gameplay routeが揃うまでblocked。
- Sites標準sign-in後に表示されるhosted app本文。未認証gateまでは確認したが、Owner sign-in、両分岐、network panel、keyboard、responsiveの最終reviewは未実施。
- Sites toolchain dependencyの11 vulnerabilities。強制audit fixは行わず、次のsource更新laneでbuild互換性と分離評価する。

### dangerous / rollback candidate

- 現時点で tracked source に rollback 必須の変更はない。
- `Library/`、`Logs/`、`Temp/` は生成物で、成功証跡でも commit しない。
- `Packages/manifest.json` / `packages-lock.json` の削除・手動再生成を復旧の第一手にしない。今回の根因は dependency JSON ではなかった。

## 6. 完成度の読み方

以下はスケジュール用の概算で、品質保証値ではない。

```text
Writer Cockpit navigation [#####] accepted  source検索・診断drilldown・targeted test済み
Writer Cockpit full loop  [####-] 80-90%  Apply/Play再受入とexternal editor実jump待ち
エンジン alpha 能力       [###--] 60-70%  主要能力あり、M1/M2横断実証待ち
自動検証・CI              [##---] 35-45%  資産あり、現行全体基準とE2E不足
Sites-native publication  [####-] Owner-only Version 1 hosted、hosted本文review待ち
Ch1 製品縦断              [#----] 20-30%  full authoring解放ゲート前
最終モバイル製品          [#----] 20-30%  build / distribution /後半content未着手
```

「spec done 24件」や「コードが存在する」ことを製品完成と読み替えない。最大ギャップは、作者導線、全スレッド型、状態完全性、モバイル成果物への一気通貫証明である。

## 7. 残作業

| ID | 目的 | 効果 | 必要条件 | 現在地 | 主担当 / 所有物 | 次の動き |
|---|---|---|---|---|---|---|
| R0 | Writer Cockpit full-loop受入 | accepted navigationと既存Apply/Playを日常導線として閉じる | External Script Editorの人間選択、Unity 6000.4.9f1 | search/source/diagnostics accepted。実jumpとApply/Play再確認待ち | shared。assistant=技術導線、user=editor/操作感判断 | NodeとdiagnosticのOpen各1回、必要なら`DQT_Start`をApply / Play |
| R0B | Sites authoring bridge受入 | author→export→local previewをmain統合前に評価できる | exact `e059e4b`、Unity 6000.4.9f1、Owner / Supervisorの操作判断 | remote branch、compile、18/18、direct window、fixture/generated accessはgreen。実Exportと人間受入、main統合は未実施 | shared。assistant=技術証拠、user/supervisor=作者UXと統合判断 | exact candidateでExportを1回実行し、fixture/generatedをOK/NG記録。OK後も別指示までmergeしない |
| R1 | Validator信頼維持 | 実warningだけを作者が判断可能にする | runtime registration方式をliteral handlerから変える際のtest更新 | 2026-07-26 active Yarn errors 0 / warnings 0 / info 3、targeted 14/14は2026-07-19 | assistant / registry helper + tests | 新registration方式を導入する時だけextractorを拡張 |
| R2 | 現行回帰基準化 | 基礎83件を安全に実行し、全97件の現在地を固定する | save data退避またはtest専用path隔離 | 定義はEditMode 73 / PlayMode 10 / Editor 14。基礎全体結果は旧8 PlayMode基準 | assistant/CI / isolation + XML・txt結果 | 削除対象pathを隔離して基礎83件をbatch実行 |
| P1 | Sites-native lightweight demo | Unity blockerと独立してprivate UXをreview可能にする | Owner sign-in、access変更禁止 | static input、Sites Version 1、Owner-only access、deployment、local runtime keyboard/両分岐まで完了。hosted本文未確認 | User + Web Supervisor / hosted review | private URLへsign-inし、両分岐・network・keyboard・responsiveをOK/NG記録 |
| P2 | Direct Unity Web再調査 | Unity runtimeのhost互換性を実測する | Web Build Support、有効なpublic gameplay sceneとnavigation | blocked、build outputなし | user + Unity lane | module/scene gateが両方解消した時だけ再dispatch |
| R3 | SP-023 / 024 表示契約 | 実装済み表示能力を受理し M1へ戻る | R0、ユーザーの画面判断 | demo Yarn とコードあり、証跡なし | shared / visual evidence | SP-023 3ノード → SP-024 S1/S2/S5 |
| R4 | M1 全スレッド型 | A/B/C、Latent、Branch を本編外で信頼可能にする | R2、DebugQuickTest / ETK | 個別実装あり、横断証明なし | assistant / harness + PlayMode | 最小モックと状態観測を追加 |
| R5 | M2 状態完全性 | Save→Load→続行と章遷移の破綻を防ぐ | R4、test data 隔離 | 実装あり、同値検証不足 | assistant / round-trip tests | Unread、Branch、subthread、EndDay を固定 |
| R6 | M3 alpha gate | P0=0 を確認し full Ch1 authoring を解放 | R4+R5 | gate 未発動 | shared / audit判定 | 未確認・未実装を P0/P1/P2 化 |
| R7 | 人間判断の固定 | AI の先回り実装を防ぐ | 体験意図の短い決定 | B/C表現、通知、タップ仕様等が保留 | user / specs・decision log | 実装直前に必要な判断だけ確定 |

### 許容して後送する負債

| ID | 目的 / 影響 | 必要条件・状態 | owner / next |
|---|---|---|---|
| D1 | `ScenarioManager` は command 33件を登録するが解除は31件で、`DiscoverFragment` / `AddFragmentNote` が欠落。再 enable 時の handler 重複リスク | 現在の compile blocker ではない。R1 と同じ command registry 監査で扱える | assistant。R1 の回帰テストと一緒に対称性を固定 |
| D2 | `FEATURE_STATUS_AUDIT.md`の機能判定本文と`spec-index.json`の旧8 PlayMode / SP-024進捗が現物より古い | テスト件数とTODO位置は2026-07-26再集計済み。機能statusはM3再監査前に更新し、推測昇格しない | supervisor/shared。R2結果後に結果列を更新 |
| D3 | Build Settingsはproduction/public scene列ではなく、参照する`MVPScene.unity`も現treeに存在しない | Android product buildまたはdirect Unity Web再調査前にscene責務を確定。現在は変更禁止 | assistant + user。G10/G11またはP2の明示laneで分離 |
| D4 | SP-023フリック、SP-024 S4、B/C rich UI、候補ENH | candidate / hold のまま。Human Authority と value path 未通過 | user approval後のみ実装 |
| D5 | UI_ISSUES 3件 | 個別修正禁止。3-5件単位またはM6 UI batch | shared。再現情報だけ保持 |
| D6 | Verification メニューの同名 `MenuItem` 重複 | compile blocker ではないが、起動時 warning と menu owner の曖昧さを残す | assistant。R1 または次の Editor tooling 小スライスで単一 owner に整理し、batch open で warning 消失を確認 |

## 8. 推奨目標列

- G0-G9 は既存 `project-context.md` の CURRENT / NEXT / M1-M8 を、依存と出口条件が見える形へ展開したもの。
- G10-G13 は最終成果物から逆算した**提案**であり、個別実装の承認ではない。
- FEATURE_REGISTRY の candidate を、この表だけで approved に昇格させない。

| 順 | 目標 | 主成果物 / 完了条件 | 依存・リスク | actor / owner |
|---|---|---|---|---|
| G0 | 開発環境再現性 | fresh resolve、compile、再利用可能 launcher。**この端末では達成** | 別端末は未証明。ignored cacheを正本化しない | assistant/tool / launcher・検証記録 |
| G1 | Writer Cockpit 受入 | navigation laneはaccepted。残りは設定済みEditor jumpとApply/Play/Last Actionの1ループ | ContentAuthoring scene保存とhuman editor選択に注意 | shared / user判断 + tool |
| G1.1 | Validator 信頼回復 | **active Yarnで達成**: 登録済みcommand/character偽陽性0、targeted test 14/14 | literal handler以外へ登録方式を変える時はextractor更新 | assistant / validator + tests |
| G1.2 | 現行回帰 | 基礎EditMode 73 / PlayMode 10とtargeted Editor 14、合計97件のassembly別・日付付き結果 | save data隔離必須。失敗を仕様差と不具合へ分類 | assistant/CI / isolation + test artifacts |
| G2 | SP-023/024 表示受入 | SP-023 3ノード、SP-024 S1/S2/S5、日本語SDF、IconSide の OK/NG と証跡 | 値調整は Inspector。個別修正ループ禁止 | shared / visual evidence |
| G3 / M1 | サブスレッド全型 | A/B/C、Latent、Branch、知識転送、Complete を ETK + PlayMode で実証 | B/C rich UI を判断前に作り込まない | assistant / engine harness |
| G4 / M2 | 状態完全性と章遷移 | Save→Load state equality、EndDay、章完了、再開で重複・欠落なし | schema versioning は Beta 前に必要 | assistant / tests |
| G5 / M3 | alpha 通過ゲート | 再監査、P0/P1/P2、P0=0 なら Ch1 full authoring 解放 | gate はスキップ不可 | shared / gate decision |
| G6 | Ch1 製品縦断 | author→validate→sync→play→save→chapter complete を人間執筆で完走 | 本文・演出判断は user。量だけを進捗にしない | user + assistant / Ch1 |
| G7 / M4 | E2E / CI 量産耐性 | 主要 command、chapter smoke、failure diagnostics、CI artifacts | package resolve と credentials の再現性 | assistant/CI / automation |
| G8 / M5 | Ch2 制作スケール | 同じ導線で第2章、Ch1→Ch2状態持越し、選択P1を検証 | P1を一度に広げない | shared / Ch2 |
| G9 / M6 | 製品UX統合 | SP-018/019/020後続、UI batch、承認済みENH | Ch1/2の実使用観測後 | shared / product UX |
| G10 / M7a | Android技術 smoke | development APK、実機起動、safe area、tap、save path、性能の早期確認 | 本番署名やStore投入ではない | assistant + user device / probe |
| G11 / M7b | Android製品ビルド | production profile、scene列、identifier、version、署名、CI AAB/APK | keystoreとbusiness IDは human authority | shared / Android artifact |
| G12 / M8 | Ch3-9・音・Beta・収益化・配布 | 章単位 lock、音、closed beta、広告/同意/privacy、store assets、RC | 一括制作せず章ゲート。iOSはmacOS/Xcode前提 | shared / release candidate |
| G13 | 1.x 運用 | crash/性能監視、save migration、hotfix、OS/Unity更新方針、承認済みENH | release後の実データで優先度を決める | shared / live operations |

## 9. 将来の意味ある分岐

製品本体は G1 → G2 → M1 → M2 → M3 が一本道。これと独立したpublication probeとして、Sites-native lightweight chat demoだけは別laneで先行可能である。direct Unity Webはblocker解消前に再開しない。

| 分岐 | 目的 | 強くなる点 | 主な代償 | 最適な状況 |
|---|---|---|---|---|
| **縦断スライス優先（推奨）** | Ch1を製品ループとして完成 | 最終成果物の実在感と作者フローの学習が最短 | E2EをCh1と並行維持する必要 | 外部デモ期限がなく、製品本体を進めたい |
| 信頼性優先 | M4 E2E/CIを先に閉じる | 章追加時の回帰・診断コストが下がる | 人間が触れる本編成果が遅れる | Save/CI/resolveが再び不安定化した場合 |
| ショーケース優先 | S4、フリック、承認済み演出を先行 | 見せられる画面が早く増える | 状態完全性と製品縦断を遅らせる | 明確な展示・審査期限がある場合だけ |

## 10. Human Authority

- Yarn 本文、キャラクターの声、ストーリー品質。
- サブクエストの必須／任意比率。
- B型 Wiki、C型成果物カード、解放通知の体験仕様。
- タイピングインジケーター中のタップをスキップに含めるか。
- 遷移時の色変化の正しい見た目。
- Unity Preferencesで使うExternal Script Editorの選択。
- Web Build Support導入、public gameplay scene責務、Sites public/shared access・custom domain判断。
- Android / iOS の投入順、identifier、署名、広告位置、同意、privacy。
- Beta 合格指標、最終コンテンツ量、サウンド制作範囲。

## 11. 次にやらないこと

- 新規 Yarn 本文をAIが主成果として書く。
- UI_ISSUES を1件ずつコード修正して手動確認ループを回す。
- `Library/PackageManager` や lock を理由なく削除する。
- Validatorの errors=0 だけで warning の信頼性問題を無視する。
- Unity Web blockerを回避するためにmoduleを自動導入、公開不可sceneを混入、またはSites accessをpublic/sharedへ変更する。
- candidate ENH を承認なしで実装する。
- M1/M2/M3 を飛ばして full Ch1、Ch2、サウンド、広告へ進む。
- ignored `Library/` や `Logs/` をリモート再現性の根拠にする。

## 12. 再開コマンド

### interactive Editor

```powershell
.\tools\run-unity.ps1
```

起動後:

1. `Tools > FoundPhone > Writer Cockpit`
2. `Refresh Nodes`
3. `DQT_Start` または推奨ノード
4. `Validate Then Sync`
5. `Apply Node To ContentAuthoring Scene`
6. `Play ContentAuthoring From Selected Node`
7. Last Action / ContentAuthoring status を確認

### batch open / compile

```powershell
.\tools\run-unity.ps1 -BatchMode -Quit `
  -LogFile 'Logs\unity-open.log'
```

### 非破壊 Yarn validator

```powershell
.\tools\run-unity.ps1 -BatchMode -Quit `
  -LogFile 'Logs\yarn-validator.log' `
  -ExecuteMethod 'ProjectFoundPhone.Editor.ContentPipelineBatch.RunYarnValidatorBatch'
```

## 13. 監修役AIへの最終指示

primary `main=origin/main=2f37534`は開発再開可能。review branch `origin/codex/sites-authoring-bridge-v1=e059e4b`は技術的にreview-readyだがmain未統合である。最初の判断は、User / Supervisorがexact candidateのWriter Cockpit exportとfixture/generated previewをOK/NGで受け入れること。OKでもPR/mergeは別指示まで行わない。

G1.1 Validator信頼回復とWriter Cockpit navigation laneは受入済み。製品本体は、人間環境でExternal Script Editor jumpと必要なApply/Play loopを閉じ、main上のsave data隔離後のG1.2、SP-023/024、M1 → M2 → M3へ進む。公開面はOwner-only Sites Version 1まで到達しており、次はOwner sign-in後のhosted本文reviewだけを行う。direct Unity Webはmodule/public scene/navigation gateが揃うまでblocked、public/shared accessは別Human Gateとする。

M3 通過後の最遠推奨線は、G6 Ch1製品縦断 → G7 E2E/CI量産耐性 → G8 Ch2制作スケール → G9製品UX統合 → G10 Android技術smoke → G11 Android製品ビルド → G12 Ch3-9・音・Beta・収益化・配布 → G13 1.x運用である。これは依存順を示す監修用の目標提案であり、候補機能、コンテンツ量、署名、広告、配布判断を先行承認するものではない。

報告時は、コード存在、batch compile、interactive受入、テスト通過、human visual approval を混同しない。各成果を別の信頼レベルとして記録する。
