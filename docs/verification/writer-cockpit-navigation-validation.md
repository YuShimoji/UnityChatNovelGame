# Writer Cockpit Source Navigation Validation

## Top Strip

- thread: `editor-001`
- lane: `writer-cockpit-navigation`
- epoch: `20260718-01`
- base: `55cb0d20c1d92a21e6e6ecb0db5d2fdbdfe562d7`
- branch: `feat/writer-cockpit-navigation`
- worktree: `C:\Users\thank\Storage\Game Projects\UnityChatNovelGame-writer-cockpit-navigation`
- Unity: `6000.4.9f1`

## Outcome

Writer Cockpit に Node 名検索、Yarn asset path / `title:` 行表示、設定済み External Script Editor への file/line jump、structured Validator drilldown を追加した。既存 Refresh / Validate / Sync / Validate Then Sync / Apply / Play と read-only Save status は維持した。

状態遷移は `summary-only` から、`Node検索 -> source location -> Validator file/line診断 -> source jump` まで到達した。External Script Editor 未設定時は Windows のアプリ選択を出さず、Unity Preferences での設定が必要だと表示する。

## Node Index Design

- `YarnSOGenerator.YarnNodeSourceLocation` が `NodeName / AssetPath / TitleLine` を保持する。
- active Yarn の各行を1-basedで走査し、`title:` の位置を取得する。
- `AuthoringScanSummary` が従来の `NodeNames` と併せて `NodeLocations` を返す。
- Cockpit のfilterはNode名を大小文字無視で検索し、scroll一覧を安定順で表示する。
- 現active Yarnは `11 files / 74 unique nodes / 74 source locations`。

## Validator And Registry

- `ValidationReport` は `ValidationSummary` と `ValidationResult[]` を返す。
- 各結果は `ValidationLevel / File / Line / Message` を持つ。
- command registryはEditor側の固定リストを廃止し、runtime `.AddCommandHandler("name", ...)` 宣言をsource of truthとして索引する。
- character registryは `CharacterProfile` assetをsource of truthとし、profileを持たない `system / narrator` だけを明示的に補う。
- before: `errors=0 / warnings=33 / info=3`。内訳は登録済みcommand由来24件と、存在する `CharacterProfile_NPC_Unknown` を見落としていた9件。
- after: `errors=0 / warnings=0 / info=3`。故意の未登録commandは引き続きWarningになる。

## Interactive Evidence

- 変更前: main checkoutのUnityで `Tools > FoundPhone > Writer Cockpit` を開き、11 files / 74 nodes、既存Action群、単一Node popup、read-only Save statusを確認した。
- 変更後: writer worktreeのUnityで `SP024` が `1 / 74 nodes` に絞られ、`SP024_Immersion_Start` が `Assets/Resources/Yarn/active/SP024_ImmersionDemo.yarn:13` を表示することを確認した。
- CockpitからValidateを実行し、`Error 0 / Warning 0 / Info 3` と `file / line / message / Open` を持つ診断一覧を確認した。
- source jump実行時、この端末ではExternal Script Editorが未設定だった。最終実装はアプリ選択へ進まず、`External Script Editor is not configured. Set it in Unity Preferences > External Tools.` を表示した。
- 設定済みExternal Script Editorでの実ファイル/行移動は未検証であり、人間環境の設定ゲートとして残る。

## Validation

- targeted EditMode: `14 / 14 passed`
  - XML: `Logs/writer-cockpit-navigation-editmode-post-interactive-2026-07-19.xml`
  - log: `Logs/writer-cockpit-navigation-editmode-post-interactive-2026-07-19.log`
- active Yarn validator: `errors=0 / warnings=0 / info=3`
  - `11 files / 74 nodes / 24 #line tags / 42 declared variables`
  - log: `Logs/writer-cockpit-navigation-yarn-validator-final-2026-07-19.log`
- final batch compile: return code `0`
  - log: `Logs/writer-cockpit-navigation-compile-final-2026-07-19.log`
- `git diff --check`: pass
- PlayMode全体、full 83 tests、persistent save dataを触るtestは実行していない。

`Logs/` はignored local evidenceであり、別端末の正本ではない。この文書とtracked source/testsが再現可能な引き継ぎ面である。

## Write Set And Safety

- Writer Cockpit / Validator / YarnSOGenerator、Editor-only helper、targeted Editor tests、本検証文書だけを変更した。
- `SitesPublicationProbe.cs`、`docs/sites/`、sites feasibility文書、HANDOFF、runtime-state、PROJECT_COCKPIT、EditorBuildSettings、Runtime UI、Yarn本文、SaveManagerは変更していない。
- sibling `spike/sites-publication-probe` worktreeとの差分重複は0件。
- Save欄は `File.Exists` によるread-only表示のまま。Save / Load / Delete / AutoSave API呼び出しはない。

## Quality Debt And Not Done

- command registry抽出はliteralな `.AddCommandHandler("name", ...)` を対象にする。将来、変数名・属性・生成コードへ登録方式を変える場合はregistry extractorの拡張が必要。
- External Script EditorをUnity Preferencesで設定後、Nodeと診断の両方からfile/line jumpを1回受け入れる必要がある。設定するアプリの選択は人間所有。
- Apply / Playの共有処理はcompile上維持したが、このlaneではContentAuthoringの再生受入を実行していない。
- Web content package、Sites build、publishing、Runtime UI、Yarn本文変更は実施していない。

## Successor Candidates (data only)

```yaml
- id: HUMAN-EDITOR-CONFIG-001
  owner: human
  purpose: Unity External Script Editorを選択しfile/line jumpを実受入する
  state: blocked_by_environment_configuration
  next_move: Preferences > External ToolsでEditorを設定し、NodeとdiagnosticのOpenを各1回実行する
- id: H1-CONTENT-PACKAGE-001
  owner: successor_lane
  purpose: 選択NodeからWeb公開用content packageを安全に生成する
  state: unlocked_by_node_source_index
  requirement: writer-owned content judgmentとsites lane境界を維持する
  next_move: 別laneでpackage contractを定義する
```
