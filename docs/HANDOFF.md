# Handoff

最終更新: 2026-07-28

会話ログなしで再開するための唯一のライブ現在地。履歴は追記せず Git に委ねる。詳細な監修判断は `docs/SUPERVISOR_REPORT.md`、環境条件は `docs/runtime-state.md` を参照する。

## 現在地

- プロジェクト: FoundPhone / UnityChatNovelGame
- 作業ブランチ: `codex/reconcile-authoring-bridge-state`
- リモート基準: `origin/main` は `197116d`。開始時の `73aef720` から3コミット先で、2026-07-26 development readiness と2026-07-27 Sites authoring bridge引継ぎを含む
- ローカル正本整理: 開始時から存在した追跡文書27件の変更は、旧 Cockpit / Dashboard / Turn Plan を廃止し、ライブ状態・環境事実・監修判断を分離する一貫した整理として独立コミットに保全した。現在は最新リモート事実と統合中
- 現在の開発スライス: Sites authoring bridge候補の人間受入、save-isolated基礎83テスト、Owner-only hosted本文reviewを、統合・公開ゲートを混同せず閉じる

## 取り込んだ能力

| 領域 | 最新状態 | workflow / decision への効果 |
|---|---|---|
| Writer Cockpit | 74 Node検索、source path / line、diagnostic drilldown、External Script Editorへのfail-closed jump | Yarnのエラー箇所から修正対象へ戻る導線を一画面に集約 |
| Validator | runtime command handlerとCharacterProfileをregistry化。active Yarnは`errors=0 / warnings=0 / info=3` | 既知command / characterの偽陽性を除き、作者が実診断へ集中できる |
| Sites authoring bridge | review branch `origin/codex/sites-authoring-bridge-v1@e059e4b`で、選択NodeからPackage v1をexportし、fixture/generated previewを切り替える候補を実装 | Yarn→portable static previewをmain統合前に比較できる |
| Bridge検証 | Unity compile、対象Editor 18/18、Writer Cockpit直通表示、fixture/generatedのHTTP・browser accessを確認済み | candidateの技術的到達性と人間の作者UX判断を分離 |
| Sites demo | tracked static fixtureとvalidator、Owner-only Version 1 deploymentを維持 | direct Unity Web blockerと分離して軽量UXをprivate reviewできる |

## 開発可能性

| 経路 | 現在の判断 | 残る確認 |
|---|---|---|
| Git | `origin/main=197116d`を取得済み。開始差分は独立コミットへ保全し、競合範囲を正本7文書に限定 | 統合後の最終SHA、ahead/behind、push結果を確定する |
| Unity / C# | mainの2026-07-26基準は39 packages、script compile、batch exit 0 | 今回branchでの狭い再検証 |
| Writer Cockpit | main navigationは14/14、bridge candidateは18/18 | candidateのExport操作とApply / Play / Last Actionの人間受入 |
| 基礎回帰 | EditMode 73 / PlayMode 10を静的集計済み | 実セーブを隔離して83件を現行Unityで実行 |
| Sites static package | fixture validator、両分岐、responsive / accessibility smokeを確認済み | hosted本文はOwner sign-in後review待ち |
| Docs | 開始差分で削除参照0、`git diff --check`、MkDocs strict build pass | 最新事実統合後の再実行 |

## 残る不確実性

- bridge candidateはreview-readyだがmain未統合。人間の作者UX受入とmain統合判断は別ゲートであり、このスライスでは代行しない。
- External Script Editorが設定された人間環境で、Nodeとdiagnosticの実file / line jumpは未受入。
- Writer CockpitのApply / Play / Last ActionとExport操作の人間受入は未完了。
- 基礎83テストは実ユーザーの`Application.persistentDataPath`を傷つけない隔離を確立してから実行する。
- SitesはOwner-onlyのまま。hosted本文、network、両分岐、keyboard、320–430pxはOwner sign-in後の判定が必要。public/shared accessとcustom domainは未承認。
- direct Unity WebはWeb Build Support、有効なpublic gameplay scene、TitleSceneからの公開導線が揃うまでblocked。

## 次に入れる作業

| 入口 | 解く摩擦 | 完了すると可能になること | 前提 |
|---|---|---|---|
| Advance: save-isolated基礎83テスト | 全体回帰が旧8 PlayMode基準のまま | EditMode 73 / PlayMode 10を安全な現行基準として固定 | test data隔離 |
| Audit: bridge作者UX | 技術証拠と作者の操作感受入が分離したまま | candidateをOK/NGで一意に判定し、main統合判断へ進める | exact `e059e4b`、人間操作 |
| Audit: Sites hosted本文 | private deploymentと実表示の間に認証ゲートが残る | Version 1の両分岐・network・keyboard・responsiveを正式受入 | Owner sign-in。access policyは変更しない |
| Explore: SP-023 / SP-024表示比較 | 実装済み表示能力の画面証跡が不足 | portrait / wide比較からUIバッチ基準を凍結 | Writer Cockpit full-loop受入後 |

## 触らない範囲

- bridge candidateのmain統合、PR、releaseを人間受入と同一視しない
- `Packages/manifest.json` / `packages-lock.json`の削除・手動再生成を復旧の第一手にしない
- ユーザー承認前のUnity module / Localization依存追加、public/shared Sites access、custom domain
- Yarn本編の代筆、未承認のB型Wiki / C型リッチカード実装
- UI問題の1件ずつの即時修正。`docs/UI_ISSUES.md`に集約してバッチ化する
