# Runtime State

最終更新: 2026-07-21

このファイルは共有可能な環境・検証条件だけを保持する。現在の作業判断は `docs/HANDOFF.md`、履歴は Git、監修判断は `docs/SUPERVISOR_REPORT.md` を参照する。

## 実行基準

| 項目 | 現在値 |
|---|---|
| Unity | 6000.4.9f1 |
| C# / scripting | Unity project defaults |
| Yarn Spinner | 3.1.3 |
| Git base | `73aef720` (`main`, `origin/main`) |
| Active authoring surface | `Tools > FoundPhone > Writer Cockpit` |
| Active runtime scene | `ContentAuthoring` / `DebugChatScene` |
| Sites static input | `sites/foundphone-demo/` |
| Sites access | Version 1 deployed、Owner-only / custom user 1 / group 0（2026-07-21 の remote evidence） |

## 最新の受入済み基準

- 2026-07-21 current checkout readiness refresh:
  - `main` を `51dc8bc` から `73aef720` へ fast-forward。`HEAD...origin/main = 0/0`
  - package manifest / lock JSON parse pass
  - Sites static validator pass: required files 6、demo nodes 7、branching / non-canon label / accessibility hooks / prohibited-pattern audit
  - docs nav 生成と `uvx --from mkdocs-material mkdocs build --strict` pass
  - Unity 6000.4.9f1 batch open: 39 packages、script compile、exit 0
  - Yarn validator: `errors=0, warnings=0, info=3`; 11 files、74 nodes、24 `#line:` tags、42 variables
  - `ProjectFoundPhone.Editor.Tests`: 14/14 pass、failed / skipped 0、1.46 秒
  - Unity 実行由来の tracked code / Yarn / scene / asset / package 差分なし
- Writer Cockpit navigation targeted EditMode: 14/14 pass
- Yarn validator: `errors=0, warnings=0, info=3`; 11 files、74 nodes、24 `#line:` tags、42 variables
- Unity batch open / compile: Unity 6000.4.9f1、39 packages、return code 0
- Test inventory: EditMode 73 / PlayMode 10。full suite の保存済み基準は 2026-04-09 の PlayMode 8/8 のまま
- Sites static package: JSON graph、syntax、禁止 capability、HTTP / MIME、両分岐、responsive / accessibility smoke を remote integration で pass
- Sites hosted runtime: Version 1 source と deployment success、Owner-only access を確認。Owner sign-in 後の本文 review は未実施

## Package Manager と起動条件

- `tools/run-unity.ps1` は `ProjectSettings/ProjectVersion.txt` から Unity を選び、呼び出し元に `ALLUSERSPROFILE` がない場合だけ子プロセスへ CommonApplicationData を設定する。user / system 環境変数は変更しない。
- 過去の fresh resolve の `path undefined` は package JSON ではなく、標準 Windows 環境変数の欠落が直接原因。wrapper 経由では ProjectCache なしから resolve、39 packages 登録、compile に到達済み。
- `Packages/manifest.json` / `packages-lock.json` の削除や手動再生成を通常の復旧手順にしない。依存契約を変える場合は別スライスとして扱う。

## テスト安全条件

- 現行 full 83 tests は未実行。既存テストに `Application.persistentDataPath` の save slot を削除するものがあるため、実ユーザーデータの退避または test data 隔離を先に行う。
- External Script Editor の file / line jump、Writer Cockpit の Apply / Play / Last Action は interactive 人間環境レビュー待ち。
- direct Unity Web は Web Build Support、有効な public gameplay scene、TitleScene からの navigation が揃うまで blocked。

## 生成物

- `.mkdocs-view/`, `.mkdocs-site/`, `Library/`, `Logs/`, `Temp/` は ignored local output。
- 検証ログや package cache は再現補助であり、リモート正本にしない。
- Sites の credential、token、Owner 識別子、期限付き URL は保存しない。
- 今回の targeted test では一時的な Unity Licensing access-token warning が出たが、test XML は 14/14 pass、process exit 0。開発阻害とは判定しない。
