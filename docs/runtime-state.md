# Runtime State

最終更新: 2026-07-28

このファイルは共有可能な環境・検証条件だけを保持する。現在の作業判断は `docs/HANDOFF.md`、履歴はGit、監修判断は`docs/SUPERVISOR_REPORT.md`を参照する。

## 実行基準

| 項目 | 現在値 |
|---|---|
| Unity | 6000.4.9f1 |
| C# / scripting | Unity project defaults |
| Yarn Spinner | 3.1.3 |
| Remote main | `197116d` |
| 作業ブランチ | `codex/reconcile-authoring-bridge-state`。同名のorigin branchへpush済み、ahead / behind `0/0` |
| Active authoring surface | `Tools > FoundPhone > Writer Cockpit` |
| Active runtime scene | `ContentAuthoring` / `DebugChatScene` |
| Bridge candidate | `origin/codex/sites-authoring-bridge-v1@e059e4b`、main未統合 |
| Sites static input | `sites/foundphone-demo/` |
| Sites access | Version 1 deployed、Owner-only / custom user 1 / group 0 |

## 最新の受入済み基準

- 2026-07-26 main development readiness:
  - Unity 6000.4.9f1 batch open: 39 packages、Tundra success、334 evaluated、return code 0
  - Yarn validator: `errors=0 / warnings=0 / info=3`; 11 files、74 nodes、24 `#line:` tags、42 variables
  - Writer Cockpit navigation targeted EditMode: 14/14 pass
  - Sites static validatorとMkDocs strict build: pass
  - tracked code / Yarn / scene / asset / package差分なし
- 2026-07-27 bridge candidate:
  - exact `e059e4b`をUnity 6000.4.9f1でcompile、return code 0
  - targeted Editor tests 18/18 pass
  - Writer Cockpitと`Export Sites Preview Package`を可視Editorで確認
  - fixture/generated Package v1のHTTP 200、MIME、`nosniff`、`no-store`、browser開始、console warning/error 0
  - candidateはmain未統合。PR、merge、deploy、Sites access変更なし
- 2026-07-28 save-isolated regression:
  - `tools/run-unity.ps1 -IsolateTestSaveData`とEditor限定save overrideを追加。未指定の`-runTests`はUnity起動前に拒否
  - `ProjectFoundPhone.Tests` EditMode 73/73 pass、failed / skipped / inconclusive 0、0.462秒
  - `ProjectFoundPhone.PlayModeTests` PlayMode 10/10 pass、failed / skipped / inconclusive 0、19.322秒
  - `ProjectFoundPhone.Editor.Tests` EditMode 14/14 pass、failed / skipped / inconclusive 0、1.817秒
  - 実`SaveData_99.json`の実行前後SHA-256は`8FB0F337313517E93ABDBE0372ED4B2C5E5C11AF54FBC035B78F0988E5197537`で不変
  - 詳細は`docs/verification/2026-07-28-save-isolated-regression.md`
- Sites hosted runtime:
  - Version 1 sourceとdeployment success、Owner-only accessを確認
  - local runtimeで両分岐、restart、keyboard、focus、ARIA、320–430px、禁止機能監査をpass
  - Owner sign-in後のhosted本文reviewは未実施

## 今回の開始状態

- 開始HEADは`73aef720`のdetached状態で、追跡文書27件に未コミット変更、未追跡ファイルなし。
- fetch前の`origin/main`は`2f37534`、fetch後は`197116d`へ更新され、開始HEADより3コミット先。
- ローカル変更とリモート変更の重複は`HANDOFF`、`PROJECT_COCKPIT`、`PROJECT_STATUS_DASHBOARD`、`SUPERVISOR_REPORT`、`USER_REQUEST_LEDGER`、`project-context`、`runtime-state`の7件。
- ローカル差分は旧重複正本3件の削除とauthority責務分離として独立コミットへ保全。リモート3コミットは最新実測事実として統合する。

## Package Managerと起動条件

- `tools/run-unity.ps1`は`ProjectSettings/ProjectVersion.txt`からUnityを選び、呼び出し元に`ALLUSERSPROFILE`がない場合だけ子プロセスへCommonApplicationDataを設定する。user / system環境変数は変更しない。
- 過去のfresh resolveの`path undefined`はpackage JSONではなく、標準Windows環境変数の欠落が直接原因。wrapper経由ではProjectCacheなしからresolve、39 packages登録、compileに到達済み。
- `Packages/manifest.json` / `packages-lock.json`の削除や手動再生成を通常の復旧手順にしない。依存契約を変える場合は別スライスとして扱う。

## テスト安全条件

- 基礎テストはEditMode 73 / PlayMode 10、targeted Editorはmain 14、bridge candidate 18。assembly / SHA別に記録する。
- CLI testは`tools/run-unity.ps1 -IsolateTestSaveData`を必須入口にする。launcherとtest helperは未設定・root逸脱・実`persistentDataPath`一致をfail-closedで拒否する。
- External Script Editorのfile / line jump、Writer CockpitのApply / Play / Last Action、bridge Exportの操作感はinteractive人間環境レビュー待ち。
- direct Unity WebはWeb Build Support、有効なpublic gameplay scene、TitleSceneからのnavigationが揃うまでblocked。

## 生成物

- `.mkdocs-view/`, `.mkdocs-site/`, `Library/`, `Logs/`, `Temp/`はignored local output。
- 検証ログ、test XML、generated preview JSON、package cacheは再現補助であり、秘密情報を含めず、必要な結果だけを正本へ転記する。
- Sitesのcredential、token、Owner識別子、期限付きURLは保存しない。
