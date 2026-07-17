# 監修役AI向け現状報告

**更新日**: 2026-07-17
**対象**: FoundPhone / UnityChatNovelGame
**同期起点**: `main` / `7eb09c0`（同期時点で `origin/main` と ahead/behind `0/0`）
**役割**: 監修役AIが会話ログなしで、信頼できる現在地、残る判断、推奨順序、製品化までの目標を判断するための引き継ぎ正本。

## 1. 結論

このリポジトリは、**追跡ソースの同期と Unity 6000.4.9f1 の fresh package resolve / script compile まで復旧し、コード・ツール開発を再開できる状態**になった。

ただし、現在の受け入れ状態は次のように分ける。

| 判定対象 | 現在の判定 | 根拠 |
|---|---|---|
| Git / ソース同期 | 開発可能 | 2026-07-17 に `git fetch --prune origin` → `git pull --ff-only origin main`。同期起点の `HEAD` / `origin/main` は `7eb09c0`、ahead/behind `0/0` |
| Unity batch open / compile | 開発可能 | 2026-07-17 に 39 packages の cache restore、Tundra compile success、326 items evaluated、batchmode 正常終了 |
| agent / terminal からの再現 | 開発可能 | `tools/run-unity.ps1` が欠落した標準 Windows 環境変数を子プロセス内だけ補完 |
| Yarn 静的検証 | エラーなし、警告の信頼性に課題 | errors=0 / warnings=33 / info=3。警告33件中24件は登録済みコマンドを未知扱いする偽陽性 |
| Writer Cockpit interactive 操作 | 未受入 | menu source と compile は確認済み。実ウィンドウ、Apply、Play、Last Action は visible Editor 未確認 |
| 現行 Unity テスト全体 | 未更新 | 静的には EditMode 73 / PlayMode 10。保存済み全体回帰は旧 8 PlayMode 基準 |
| 最終モバイル製品 | 未到達 | Android/iOS build、署名、配布、Ch3-9、サウンド、広告、Beta は後続 |

したがって、次の一本道は **Writer Cockpit interactive 受入 → Validator 信頼回復 → 現行回帰基準化** である。新しい大機能へ分岐する段階ではない。

## 2. Shared Focus / 北極星

### 制作基盤

ライター／デザイナーが外部エディタで Yarn を書き、Unity Editor 上で次のループを迷わず回せること。

```text
Yarn 保存
  → Refresh Nodes
  → Validate
  → SO Sync
  → Start Node 選択
  → ContentAuthoring Apply / Play
  → 結果確認
```

AI の主担当はエンジン、ツール、パイプライン、検証導線である。Yarn 本文、キャラクターの語り口、感性調整、サブクエストの必須／任意判断はユーザー所有である。

### 製品

FoundPhone を、モバイル優先のチャット／ビジュアルノベルゲームとして iOS / Android に配布可能な状態へ持っていく。F2P + 広告、Ch3以降のサウンド統合は高位方針として存在するが、現在の実装スライスではない。

## 3. 今回の同期と復旧

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
   - `dev.yarnspinner.unity@3.1.3`
   - `com.unity.nuget.newtonsoft-json@3.2.2`
   - log: `Logs/development-readiness-unity-open-2026-07-17.log`（ignored local evidence）

2. **script compile**
   - 2026-07-17: Tundra build success
   - 0 items updated / 326 evaluated
   - batchmode 正常終了、C# compile error なし

3. **Package Manager local state**
   - `ProjectCache` / `ProjectCache.md5` / `projectResolution.json` を再生成
   - すべて ignored local evidence。リモート正本にはしない

4. **Yarn validator**
   - 2026-07-17 に wrapper から再実行
   - errors=0 / warnings=33 / info=3
   - 11 files / 74 nodes / 24 `#line:` tags / 42 declared variables
   - warning 内訳: unknown command 24、unknown character 9
   - info の主な残り: undeclared variable 2
   - log: `Logs/development-readiness-yarn-validator-2026-07-17.log`（ignored local evidence）

5. **静的集計**
   - spec entries: 42
   - done 24 / partial 12 / draft 2 / todo 4
   - EditMode 73 / PlayMode 10
   - 実コード TODO: `ChatController` の将来 status routing 1件

6. **ドキュメント閲覧面**
   - 2026-07-17: `generate-doc-nav.ps1 -PrepareView` 完走
   - `uvx --from mkdocs-material mkdocs build --strict` exit 0
   - ignored PerformanceBaseline raw pagesは nav 外の INFO。欠落ファイル参照は削除

7. **非阻害 warning**
   - Unity: `VerificationMenu` と `MissingScriptScanner` が同じ `Tools/FoundPhone/Verification/Scan DebugChatScene Missing Scripts` を登録
   - UnityConnect: 終了時の public CDN request timeout
   - docs: `uvx` の provider 推奨と MkDocs 2.0 将来互換性告知
   - いずれも今回の compile、validator、strict docs build の終了コードを失敗にはしていない

### 未実行

- interactive Unity Editor での Writer Cockpit 実操作。
- EditMode 73 / PlayMode 10 の全件実行。
- SP-023 / SP-024 の画面検収。
- Save / Load state equality と章遷移の横断検証。

全テストを直ちに実行しなかった理由は、既存テストに `Application.persistentDataPath` の save slot を削除するものがあるため。実行前に実ユーザーデータ退避または test data 隔離が必要である。

## 5. Current Trust Assessment

### trusted

- 同期起点で `main` と `origin/main` が一致していたこと。
- 2026-07-17 の現 checkout で package restore、Tundra compile、batchmode 正常終了を再確認したこと。
- 2026-07-17 の Yarn validator が errors=0 / warnings=33 / info=3 で完走したこと。
- `ALLUSERSPROFILE` 欠落が fresh resolve の直接原因だったこと。
- process-local 補完で隔離 resolve と実プロジェクト fresh resolve が成功したこと。
- Unity 6000.4.9f1 の package registration と script compile。
- wrapper 経由の再実行が cache restore と compile success に到達したこと。
- Writer Cockpit / Content Pipeline の menu source と Editor assembly compile。
- Yarn validator が errors=0 で完走したこと。

### needs re-check

- Writer Cockpit の実 menu、window layout、Refresh、Validate Then Sync、Apply、Play、Last Action。
- Validator warning 33件の意味。unknown command 24件は登録表ドリフトによる偽陽性。
- 現行 83 テストの Unity 6000.4.9f1 ベースライン。
- SP-023 / SP-024 の実表示、日本語 SDF、IconSide。
- Save / Load、Unread、Branch、削除痕、EndDay、章遷移の状態同値。
- fresh clone / 別端末。今回証明したのはこの端末と wrapper 経路。
- Verification メニューの同名 `MenuItem` 重複。Writer Cockpit の compile blocker ではないが、Editor menu の診断ノイズになる。

### dangerous / rollback candidate

- 現時点で tracked source に rollback 必須の変更はない。
- `Library/`、`Logs/`、`Temp/` は生成物で、成功証跡でも commit しない。
- `Packages/manifest.json` / `packages-lock.json` の削除・手動再生成を復旧の第一手にしない。今回の根因は dependency JSON ではなかった。

## 6. 完成度の読み方

以下はスケジュール用の概算で、品質保証値ではない。

```text
Writer Cockpit 実装       [####-] 80-90%  compile済み、interactive受入待ち
エンジン alpha 能力       [###--] 60-70%  主要能力あり、M1/M2横断実証待ち
自動検証・CI              [##---] 35-45%  資産あり、現行全体基準とE2E不足
Ch1 製品縦断              [#----] 20-30%  full authoring解放ゲート前
最終モバイル製品          [#----] 20-30%  build / distribution /後半content未着手
```

「spec done 24件」や「コードが存在する」ことを製品完成と読み替えない。最大ギャップは、作者導線、全スレッド型、状態完全性、モバイル成果物への一気通貫証明である。

## 7. 残作業

| ID | 目的 | 効果 | 必要条件 | 現在地 | 主担当 / 所有物 | 次の動き |
|---|---|---|---|---|---|---|
| R0 | Writer Cockpit interactive 受入 | 作者導線を実使用可能へ上げる | Unity 6000.4.9f1、復旧済み package state | compile 済み、visible UI 未確認 | shared。assistant=技術導線、user=操作感判断 | wrapper で Editor 起動、`DQT_Start` を Apply / Play |
| R1 | Validator 信頼回復 | warning を作者が判断可能な信号へ戻す | ScenarioManager 登録コマンドとの同期方法 | unknown command 24件が偽陽性 | assistant / Validator と回帰テスト | registry drift を防ぐ小スライス |
| R2 | 現行回帰基準化 | 73 EditMode / 10 PlayMode を現行基準にする | save data 退避または test data 隔離 | 最終全体記録は旧8 PlayMode | assistant/CI / XML・txt結果 | データ安全策の後に batch 実行 |
| R3 | SP-023 / 024 表示契約 | 実装済み表示能力を受理し M1へ戻る | R0、ユーザーの画面判断 | demo Yarn とコードあり、証跡なし | shared / visual evidence | SP-023 3ノード → SP-024 S1/S2/S5 |
| R4 | M1 全スレッド型 | A/B/C、Latent、Branch を本編外で信頼可能にする | R2、DebugQuickTest / ETK | 個別実装あり、横断証明なし | assistant / harness + PlayMode | 最小モックと状態観測を追加 |
| R5 | M2 状態完全性 | Save→Load→続行と章遷移の破綻を防ぐ | R4、test data 隔離 | 実装あり、同値検証不足 | assistant / round-trip tests | Unread、Branch、subthread、EndDay を固定 |
| R6 | M3 alpha gate | P0=0 を確認し full Ch1 authoring を解放 | R4+R5 | gate 未発動 | shared / audit判定 | 未確認・未実装を P0/P1/P2 化 |
| R7 | 人間判断の固定 | AI の先回り実装を防ぐ | 体験意図の短い決定 | B/C表現、通知、タップ仕様等が保留 | user / specs・decision log | 実装直前に必要な判断だけ確定 |

### 許容して後送する負債

| ID | 目的 / 影響 | 必要条件・状態 | owner / next |
|---|---|---|---|
| D1 | `ScenarioManager` は command 33件を登録するが解除は31件で、`DiscoverFragment` / `AddFragmentNote` が欠落。再 enable 時の handler 重複リスク | 現在の compile blocker ではない。R1 と同じ command registry 監査で扱える | assistant。R1 の回帰テストと一緒に対称性を固定 |
| D2 | `FEATURE_STATUS_AUDIT.md` のファイル数・テスト数・TODO行、`spec-index.json` の旧8 PlayMode / SP-024進捗が現物より古い | M3 再監査前に更新。今ここで status を推測昇格しない | supervisor/shared。R2結果後に正本更新 |
| D3 | Build Settings に ContentAuthoring / DebugChatScene / MVPScene が enabled。production scene列ではない | Android production build前に Build Profile と scene責務を確定 | assistant + user。G10 smoke と G11 product buildで分離 |
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
| G1 | Writer Cockpit 受入 | visible menu、Refresh、Validate Then Sync、Apply、Play、Last Action が1ループ成功 | ContentAuthoring scene を保存する点に注意 | shared / user判断 + tool |
| G1.1 | Validator 信頼回復 | 登録済み command の偽陽性 0、実 warning を分類可能 | runtime登録表との二重管理を避ける | assistant / validator + tests |
| G1.2 | 現行回帰 | EditMode 73 / PlayMode 10 の日付付き結果 | save data 隔離必須 | assistant/CI / test artifacts |
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

M3 通過前は G1 → G2 → M1 → M2 → M3 が一本道。M3 通過後に初めて次の分岐が意味を持つ。

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
- Android / iOS の投入順、identifier、署名、広告位置、同意、privacy。
- Beta 合格指標、最終コンテンツ量、サウンド制作範囲。

## 11. 次にやらないこと

- 新規 Yarn 本文をAIが主成果として書く。
- UI_ISSUES を1件ずつコード修正して手動確認ループを回す。
- `Library/PackageManager` や lock を理由なく削除する。
- Validatorの errors=0 だけで warning の信頼性問題を無視する。
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

最初の判断は新機能選択ではない。G1 の interactive 受入を閉じ、その直後に G1.1 Validator 信頼回復と G1.2 現行回帰を小さく完了させる。その後に SP-023/024 表示受入へ戻り、M1 → M2 → M3 を通過する。

M3 通過後の最遠推奨線は、G6 Ch1製品縦断 → G7 E2E/CI量産耐性 → G8 Ch2制作スケール → G9製品UX統合 → G10 Android技術smoke → G11 Android製品ビルド → G12 Ch3-9・音・Beta・収益化・配布 → G13 1.x運用である。これは依存順を示す監修用の目標提案であり、候補機能、コンテンツ量、署名、広告、配布判断を先行承認するものではない。

報告時は、コード存在、batch compile、interactive受入、テスト通過、human visual approval を混同しない。各成果を別の信頼レベルとして記録する。
