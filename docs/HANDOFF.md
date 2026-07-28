# Handoff

最終更新: 2026-07-28

会話ログなしで再開するための唯一のライブ現在地。履歴は追記せず Git に委ねる。詳細な監修判断は `docs/SUPERVISOR_REPORT.md`、環境条件は `docs/runtime-state.md` を参照する。

## 現在地

- プロジェクト: FoundPhone / UnityChatNovelGame
- 作業ブランチ: `codex/reconcile-authoring-bridge-state`。`origin/codex/reconcile-authoring-bridge-state`へpush済みで、ahead / behind `0/0`
- リモート基準: `origin/main` は `197116d`。開始時の `73aef720` から3コミット先で、2026-07-26 development readiness と2026-07-27 Sites authoring bridge引継ぎを含む
- ローカル正本整理: 開始時から存在した追跡文書27件の変更は、旧 Cockpit / Dashboard / Turn Plan を廃止し、ライブ状態・環境事実・監修判断を分離する一貫した整理として独立コミットに保全。最新リモート3コミット上へ統合済み
- 現在の開発スライス: AI所有のsave-isolated現行97テストは完了。Sites authoring bridge候補とOwner-only hosted本文の人間受入を、main統合・公開ゲートと混同せず閉じる

## 取り込んだ能力

| 領域 | 最新状態 | workflow / decision への効果 |
|---|---|---|
| Writer Cockpit | 74 Node検索、source path / line、diagnostic drilldown、External Script Editorへのfail-closed jump | Yarnのエラー箇所から修正対象へ戻る導線を一画面に集約 |
| Validator | runtime command handlerとCharacterProfileをregistry化。active Yarnは`errors=0 / warnings=0 / info=3` | 既知command / characterの偽陽性を除き、作者が実診断へ集中できる |
| Sites authoring bridge | review branch `origin/codex/sites-authoring-bridge-v1@e059e4b`で、選択NodeからPackage v1をexportし、fixture/generated previewを切り替える候補を実装 | Yarn→portable static previewをmain統合前に比較できる |
| Bridge検証 | Unity compile、対象Editor 18/18、Writer Cockpit直通表示、fixture/generatedのHTTP・browser accessを確認済み | candidateの技術的到達性と人間の作者UX判断を分離 |
| Sites demo | tracked static fixtureとvalidator、Owner-only Version 1 deploymentを維持 | direct Unity Web blockerと分離して軽量UXをprivate reviewできる |
| Save-isolated regression | Editor限定save overrideとlauncher/test両側のfail-closed guardを追加。EditMode 73、PlayMode 10、targeted Editor 14の97/97 pass | 実ユーザーsaveを傷つけず、全体回帰を日常的に再実行できる |

## 開発可能性

| 経路 | 現在の判断 | 残る確認 |
|---|---|---|
| Git | `origin/main=197116d`上へ開始差分をrebaseし、正本7文書の競合を解消。作業ブランチをremoteへpushしahead / behind `0/0` | main統合は監修判断後の別操作 |
| Unity / C# | isolation付きbatch compile exit 0。テスト実行後のcode / scene / asset / package副作用なし | interactive操作は人間環境で確認 |
| Writer Cockpit | main navigationは14/14、bridge candidateは18/18 | candidateのExport操作とApply / Play / Last Actionの人間受入 |
| 現行回帰 | EditMode 73/73、PlayMode 10/10、targeted Editor 14/14。failed / skipped 0 | 次のengine変更時もassembly別に継続 |
| Sites static package | fixture validator、両分岐、responsive / accessibility smokeを確認済み | hosted本文はOwner sign-in後review待ち |
| Docs | 開始差分で削除参照0、`git diff --check`、MkDocs strict build pass | 最終更新後に再実行 |

## 残る不確実性

- bridge candidateはreview-readyだがmain未統合。人間の作者UX受入とmain統合判断は別ゲートであり、このスライスでは代行しない。
- External Script Editorが設定された人間環境で、Nodeとdiagnosticの実file / line jumpは未受入。
- Writer CockpitのApply / Play / Last ActionとExport操作の人間受入は未完了。
- full regressionのsave isolationはこの端末とUnity 6000.4.9f1で実証済み。別端末でもlauncher経由を維持し、`Logs/`を正本にしない。
- SitesはOwner-onlyのまま。hosted本文、network、両分岐、keyboard、320–430pxはOwner sign-in後の判定が必要。public/shared accessとcustom domainは未承認。
- direct Unity WebはWeb Build Support、有効なpublic gameplay scene、TitleSceneからの公開導線が揃うまでblocked。

## 次に入れる作業

| 入口 | 解く摩擦 | 完了すると可能になること | 前提 |
|---|---|---|---|
| Audit: bridge作者UX | 技術証拠と作者の操作感受入が分離したまま | candidateをOK/NGで一意に判定し、main統合判断へ進める | exact `e059e4b`、人間操作 |
| Audit: Sites hosted本文 | private deploymentと実表示の間に認証ゲートが残る | Version 1の両分岐・network・keyboard・responsiveを正式受入 | Owner sign-in。access policyは変更しない |
| Verify: Writer Cockpit full-loop | navigationとApply / Play / source jumpが別々の証拠 | 日常のYarn保存→再生導線を正式受入 | External Script Editor選択 |
| Excise: Verification menu重複 | batch compileを止めないがEditor warningとowner曖昧さが残る | 次のtooling検証ログを低ノイズ化 | 同名`MenuItem`の単一owner化 |
| Explore: SP-023 / SP-024表示比較 | 実装済み表示能力の画面証跡が不足 | portrait / wide比較からUIバッチ基準を凍結 | Writer Cockpit full-loop受入後 |

## 触らない範囲

- bridge candidateのmain統合、PR、releaseを人間受入と同一視しない
- `Packages/manifest.json` / `packages-lock.json`の削除・手動再生成を復旧の第一手にしない
- ユーザー承認前のUnity module / Localization依存追加、public/shared Sites access、custom domain
- Yarn本編の代筆、未承認のB型Wiki / C型リッチカード実装
- UI問題の1件ずつの即時修正。`docs/UI_ISSUES.md`に集約してバッチ化する
