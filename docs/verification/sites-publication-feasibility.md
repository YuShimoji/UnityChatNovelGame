# Sites Publication Feasibility — Unity Web Probe

## Top Strip

| Field | Value |
| --- | --- |
| Thread | `sites-001` |
| Lane | `sites-publication-probe` |
| Epoch | `20260718-01` |
| Base | `55cb0d20c1d92a21e6e6ecb0db5d2fdbdfe562d7` |
| Branch | `spike/sites-publication-probe` |
| Worktree | `C:\Users\thank\Storage\Game Projects\UnityChatNovelGame-sites-publication-probe` |

## Outcome

Unity Web build probe は **BLOCKED**。Unity 6000.4.9f1 の Web Build Support module が未導入で、Unity Hub を使う人間の操作が必要なため、契約の Stop Condition で停止した。system install、public deployment、決済実装は行っていない。

さらに、公開用の最小 scene 列として想定した `TitleScene` → `MVPScene` は、`MVPScene.unity` が現在の base に存在せず、`TitleScene` の New Game 遷移先も公開禁止 scene に限られるため、現状の tracked assets だけでは review 可能な公開ゲーム導線を構成できない。

## Intended State Transition

`現状調査 → Web module 検出` まで実施。`公開専用 scene 列を用いた Unity Web build → ローカル配信 smoke → Sites 互換性判定パッケージ` は blocker により未到達。

## Build Module State

- Unity Editor: `C:\Program Files\Unity\Hub\Editor\6000.4.9f1\Editor\Unity.exe` は存在。
- installed playback engine: `windowsstandalonesupport`。
- expected Web support path: `C:\Program Files\Unity\Hub\Editor\6000.4.9f1\Editor\Data\PlaybackEngines\WebGLSupport`。
- observed result: expected path は存在しない。
- prerequisite / owner: user が Unity Hub の 6000.4.9f1 に Web Build Support を追加する。lane は自動 install しない。

再現確認:

```powershell
$module = 'C:\Program Files\Unity\Hub\Editor\6000.4.9f1\Editor\Data\PlaybackEngines\WebGLSupport'
Test-Path -LiteralPath $module
Get-ChildItem -LiteralPath 'C:\Program Files\Unity\Hub\Editor\6000.4.9f1\Editor\Data\PlaybackEngines'
```

観測値は `False`、列挙結果は `windowsstandalonesupport` のみ。

## Selected Public Scenes

current `EditorBuildSettings` は公開列に流用しない。公開 probe の最小候補列は次の 2 scene とした。

1. `Assets/Scenes/TitleScene.unity`
2. `Assets/Scenes/MVPScene.unity`

ただし、選択を materialize できない。

- `Assets/Scenes/MVPScene.unity` は tracked tree / filesystem の双方に存在しない。Build Settings の参照は stale。
- `TitleScene` の `m_NewGameSceneName` は `DebugChatScene`。runtime は build 内に `ContentAuthoring` があればそちらを優先する。
- `ContentAuthoring` と `DebugChatScene` は契約上、公開 build から除外必須。
- したがって `TitleScene` 単独では New Game が有効な gameplay scene に遷移しない。
- `VerificationAutomator` は `MVPScene` に `StartButton`、chat / choice state、`EndPanel` がある前提であり、公開用の最小 runtime 候補としての意図は確認できる。

公開列へ `ContentAuthoring` / `DebugChatScene` を混入させて回避することはしていない。

## Deliverable / Access Path

- blocker report: `docs/verification/sites-publication-feasibility.md`
- Web build package: 未生成
- local URL: なし

## Build Output Inventory And Sizes

`Builds/SitesProbe` は作成していない。

| Item | Result |
| --- | --- |
| File count | 0 |
| Total size | N/A |
| `.wasm` | N/A |
| `.data` | N/A |
| `.js` | N/A |

## Local Hosting / Browser Evidence

build output がないため HTTP server と browser smoke は未実施。`index.html`、主要 asset、HTTP status、MIME、runtime 起動のいずれも未検証であり、runtime success は主張しない。

## Sites Compatibility Verdict

**直接 Sites 互換は未判定 / blocker。推奨は「Sites-native 軽量チャット demo」**。

理由:

- Unity Web package、実サイズ、`.wasm` MIME、圧縮挙動をまだ測定できない。
- Compression Disabled なら圧縮配信用 custom header 依存を減らせる見込みだが、package 未生成のため custom-header-free 動作は未証明。
- 現在は public gameplay scene も欠けており、Web module 追加だけでは direct Unity probe が完了しない。
- Sites-native demo は Unity runtime / module / stale scene 参照から独立して、Sites 上のゲーム UX と配信適性を最小コストで検証できる。

landing-only + external hosting は、Unity Web package と外部 host 条件を測定してから選ぶべきで、現時点では決定材料が不足している。

## Changed Files

- `docs/verification/sites-publication-feasibility.md` — module / scene blocker と Sites 推奨方式を記録。

Editor helper、test、Build Profile、`.gitignore` は変更していない。

## Checks

| Check | Result |
| --- | --- |
| `git fetch --prune origin` | pass |
| base revision | exact match: `55cb0d20c1d92a21e6e6ecb0db5d2fdbdfe562d7` |
| `HEAD...origin/main` | `0 0` at branch creation |
| Web support filesystem probe | blocker reproduced |
| scene filesystem / tracked-tree inventory | `MVPScene` missing; other 3 scene files present |
| public scene exclusions | `ContentAuthoring` / `DebugChatScene` not used |
| Unity batch compile | not run after module Stop Condition |
| probe-specific Editor tests | not added / not run after module Stop Condition |
| targeted Web build | not run after module Stop Condition |
| local hosting / browser smoke | not run because no output exists |

## Write-set Adherence / Sibling Conflict

- write は許可された `docs/verification/sites-publication-feasibility.md` のみ。
- sibling lane `editor-001` の file は変更していない。
- forbidden shared surfaces、Yarn、runtime/debug surfaces、package files は変更していない。
- current Build Settings の scene 順は変更していない。

## Not Done

- Unity Web build helper / targeted test の追加
- Web build package 生成と size inventory
- Compression Disabled の build 実測
- HTTP status / MIME / browser runtime smoke
- Sites 作成・deployment・custom domain・project ID 生成
- payment / checkout / card data / advertising SDK
- public gameplay scene の復元または新規作成

## Git State

- branch: `spike/sites-publication-probe`
- base: `55cb0d20c1d92a21e6e6ecb0db5d2fdbdfe562d7`
- upstream: 未設定
- commit / push: 未実施

## Exact Blockers

1. **Human-owned module gate**: Unity 6000.4.9f1 の `WebGLSupport` playback engine が存在せず、Web build を実行できない。user の Unity Hub 操作が必要。
2. **Lane-owned scene input gate**: `EditorBuildSettings` が参照する `Assets/Scenes/MVPScene.unity` は base に存在しない。
3. **Public navigation gate**: `TitleScene` は `ContentAuthoring` または `DebugChatScene` にしか遷移できず、両方とも公開 build から除外必須。allowed write set 内では public gameplay route を修復できない。

## Successor Candidates — Data Only

| Candidate | Purpose | Effect | Requirement | State | Owner | Next move |
| --- | --- | --- | --- | --- | --- | --- |
| Sites-native lightweight chat demo | Sites 自体の UX / host 適性を先に確認 | Unity blocker と独立した reviewable demo | 新しい Sites-native lane | recommended | Web / Sites owner | public deployment なしの local artifact から開始 |
| Direct Unity Web probe retry | 既存 Unity runtime の Sites 互換性を実測 | build size / MIME / header / browser evidence を取得 | Web Build Support と有効な public gameplay scene | blocked | user + Unity lane | Hub module 追加後、scene ownership を再承認して再 dispatch |
| Landing-only + external game hosting | Sites と Unity hosting を分離 | Sites の host 制約を回避 | 外部 host 選定と Unity Web build 実測 | pending evidence | Web supervisor | direct probe 結果が不適合のときだけ評価 |
