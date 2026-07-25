# 2026-07-26 Development Readiness Verification

## Scope

- リモートの最新状態を `main` へ fast-forward 可能な方法で取り込む。
- 現 checkout が Unity / Yarn / docs / Sites fixture の次作業へ進めるかを非破壊で確認する。
- 人間受入、実セーブに触れる全体テスト、公開設定変更は今回の検証に含めない。

## Git Synchronization

| 項目 | 結果 |
|---|---|
| branch | `main` |
| sync base | `73aef720933a630024b2e6ee460a02e74f70bf94` |
| fetch | `git fetch --prune origin` 完了 |
| pull | `git pull --ff-only origin main` → `Already up to date.` |
| parity | 検証開始時 `HEAD...origin/main` = ahead 0 / behind 0 |
| worktree | Unity / Yarn / docs検証前は clean。検証由来のcode / scene / asset差分なし |

リモートに新規commitはなく、取り込み後のソース起点は既存handoff commit `73aef720` のままだった。

## Toolchain

| 項目 | 確認値 |
|---|---|
| Unity | `6000.4.9f1` |
| Unity executable | `C:\Program Files\Unity\Hub\Editor\6000.4.9f1\Editor\Unity.exe` |
| launcher | `tools/run-unity.ps1` |
| Node.js | `v24.13.0` |
| PowerShell | `7.6.3` |
| package metadata | `Packages/manifest.json` / `Packages/packages-lock.json` parse pass |

## Validation Results

| Surface | Command / evidence | Result |
|---|---|---|
| Unity batch open / compile | `tools/run-unity.ps1 -BatchMode -Quit` | return code 0、39 packages登録、Tundra success、334 evaluated、tracked差分なし |
| Yarn validator | `RunYarnValidatorBatch` | return code 0、errors 0 / warnings 0 / info 3、11 files / 74 nodes / 24 `#line:` tags / 42 variables |
| Static Sites fixture | `node tools/sites/validate-demo.mjs` | PASS、required files 6、graph nodes 7、両分岐・非canon表示・a11y hooks・禁止pattern監査 pass |
| Documentation | `generate-doc-nav.ps1 -PrepareView` + `mkdocs build --strict` | exit 0 |
| Direct Unity Web prerequisites | module / scene / navigation read-only check | blocked継続。WebGLSupportなし、`Assets/Scenes/MVPScene.unity`なし、有効なpublic gameplay routeなし |
| Sites control plane | read-only project / access / version / deployment取得 | active、custom access、allowed user 1、group 0、Version 1、deployment succeeded |

Ignored local logs:

- `Logs/development-readiness-unity-open-2026-07-26.log`
- `Logs/development-readiness-yarn-validator-2026-07-26.log`

これらは端末内の補助証跡であり、リモート正本にはしない。

## Current Test Inventory and Safety Gate

現行ソースの属性ベース集計は次のとおり。

| Assembly / surface | Cases | Current evidence |
|---|---:|---|
| `ProjectFoundPhone.Tests` EditMode | 73 | 定義を静的集計。今回未実行 |
| `ProjectFoundPhone.Tests` PlayMode | 10 | 定義を静的集計。今回未実行 |
| `ProjectFoundPhone.Editor.Tests` targeted Editor | 14 | 2026-07-19 に14/14 pass。今回のcompileで再コンパイル成功 |
| total tracked cases | 97 | 基礎全体83 + targeted Editor 14 |

`SaveSystemTests` と PlayMode helper は `Application.persistentDataPath` の `SaveData_*.json` を削除する。実ユーザーデータ退避またはtest専用pathへの隔離を行うまで、基礎83件と全97件を一括実行しない。今回の「開発可能」はcompile / validator / docs / fixtureの再現性を指し、全回帰passを意味しない。

## Non-blocking Diagnostics

- obsolete API `Object.FindFirstObjectByType<T>()` による `CS0618` warning。
- `VerificationMenu` と `MissingScriptScanner` の同名 `MenuItem` 登録。
- Unity licensing / UnityConnect / public CDNの終了時通信warning。
- MkDocs 2.0将来互換性とprovider推奨のnotice。

いずれも今回のcompile、validator、strict docs buildの終了コードを失敗にはしていない。機能開発と混ぜず、Editor toolingまたはdependency maintenanceの独立スライスで扱う。

## Sites Boundary

- Sites設定、access policy、allowlist、version、deploymentは変更していない。
- private hosted runtimeはOwner-onlyを維持している。
- hosted本文のOwner sign-in後reviewは未実施。
- public/shared access、custom domain、外部通信、analytics、auth追加、storage、payment、live store linkは未承認・未実施。
- fixtureは常にnon-canon verification用であり、本編または最終ストーリーの受入証拠ではない。

## Development-Ready Interpretation

次のassistant-owned作業は、実セーブを保護するtest isolation設計と基礎83件の新基準化である。並行するhuman-owned gateは次の2件。

1. Ownerとしてprivate Sites本文を開き、両分岐、restart、keyboard、network、320–430pxをOK/NG判定する。
2. UnityでExternal Script Editor jumpとWriter CockpitのApply / Play / Last Actionを1ループ受け入れる。

この2件の人間確認を待つ間も、save isolationの設計・テスト整備は独立して進められる。public publish、Unity Web module導入、scene責務変更、Yarn本文追加は別の明示ゲートとする。
