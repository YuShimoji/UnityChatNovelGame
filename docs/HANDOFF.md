# Handoff

最終更新: 2026-11-26

会話ログなしで再開するための唯一のライブ現在地。履歴は追記せず Git に委ねる。詳細な監修判断は `docs/SUPERVISOR_REPORT.md`、環境条件は `docs/runtime-state.md` を参照する。

## 現在地

- FoundPhone / UnityChatNovelGame の開発は `main` から再開する。保存・分岐の分離と生成JSON試遊の動作確認版は `833cf92`。
- セーブのファイル操作を `SaveSlotStorage`、分岐中の取得情報・持ち帰り選択を `BranchThreadSession` に分離した。Yarnの呼び出し、セーブJSON、スロット番号は既存の契約を使う。
- NarrativeGen SDK `b71b3f5` をUnityへ固定参照。話者ラベルと物の属性に基づく選択肢条件に対応し、生成JSONの話者名・本文・選択肢を `DebugChatScene` の既存チャットで表示する。試遊はメモリ内の別セッションで、YarnやFoundPhoneの保存を起動しない。
- 既存の基盤工程はSP-023/024の表示確認 → M1（A/B/C・Latent・Branch）→ M2（セーブの状態同値と章遷移）→ M3（alpha判定）。今回の分岐状態と保存の分離はM1/M2の改修に使う。詳細は[開発順序](project-context.md)。

## NarrativeGenの試遊

1. Unity 6000.4.9f1でこのプロジェクトを開く。
2. `Tools > FoundPhone > Play NarrativeGen JSON...` で、NarrativeGenが保存または生成したモデルJSONを選ぶ。例はNarrativeGenの `models/generated/brass-weight.json`（六目の分銅）。
3. Gameビューのチャットに出る選択肢を押す。終了時はPlay Modeを止める。Play Mode開始シーン設定は元に戻る。

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
| Git | UnityChatNovelGameの `origin/main` に再開に必要な実装と手順を収録。NarrativeGenの `main` にあるSDKコミット `b71b3f5` を参照 | 作業開始時に `main` を fast-forward で同期 |
| Unity / C# | Unity 6000.4.9f1でPlayMode 7/7。Editorメニューから「六目の分銅」を開き、実画面でも話者名と本文を確認して4選択で結末へ到達。通常のYarn、表示、セーブ再読込の検査6件も通過 | 配布PlayerとM1/M2全体 |
| Writer Cockpit | main navigationは14/14、bridge candidateは18/18 | candidateのExport操作とApply / Play / Last Actionの人間受入 |
| 基礎回帰 | スロット書込・削除をテスト専用フォルダに隔離。既存セーブ EditMode 8件、通常チャット PlayMode 4件が通過 | 対象外の基礎テストと章遷移の状態同値 |
| Sites static package | fixture validator、両分岐、responsive / accessibility smokeを確認済み | hosted本文はOwner sign-in後review待ち |
| Docs | 開始差分で削除参照0、`git diff --check`、MkDocs strict build pass | 最新事実統合後の再実行 |

## 残る不確実性

- bridge candidateはreview-readyだがmain未統合。人間の作者UX受入とmain統合判断は別ゲートであり、このスライスでは代行しない。
- External Script Editorが設定された人間環境で、Nodeとdiagnosticの実file / line jumpは未受入。
- Writer CockpitのApply / Play / Last ActionとExport操作の人間受入は未完了。
- 既存予定の全型スレッドの実機確認、章遷移、配布ビルドでの再生は残る。
- SitesはOwner-onlyのまま。hosted本文、network、両分岐、keyboard、320–430pxはOwner sign-in後の判定が必要。public/shared accessとcustom domainは未承認。
- direct Unity WebはWeb Build Support、有効なpublic gameplay scene、TitleSceneからの公開導線が揃うまでblocked。

## 次に入れる作業

| 入口 | 解く摩擦 | 完了すると可能になること | 前提 |
|---|---|---|---|
| Advance: M1 スレッド全型 | 登録・顕在化・遷移がシーン/Yarnの管理に集中 | A/B/C・Latent・BranchをETKと通常シーンで確認し、保存・章遷移に使う状態を明確にする | SP-023/024の表示確認はユーザーと別途実施 |
| Audit: bridge作者UX | 技術証拠と作者の操作感受入が分離したまま | candidateをOK/NGで一意に判定し、main統合判断へ進める | exact `e059e4b`、人間操作 |
| Audit: Sites hosted本文 | private deploymentと実表示の間に認証ゲートが残る | Version 1の両分岐・network・keyboard・responsiveを正式受入 | Owner sign-in。access policyは変更しない |
| Explore: SP-023 / SP-024表示比較 | 実装済み表示能力の画面証跡が不足 | portrait / wide比較からUIバッチ基準を凍結 | Writer Cockpit full-loop受入後 |

## 触らない範囲

- bridge candidateのmain統合、PR、releaseを人間受入と同一視しない
- `Packages/manifest.json` / `packages-lock.json`の削除・手動再生成を復旧の第一手にしない
- ユーザー承認前のUnity module / Localization依存追加、public/shared Sites access、custom domain
- Yarn本編の代筆、未承認のB型Wiki / C型リッチカード実装
- UI問題の1件ずつの即時修正。`docs/UI_ISSUES.md`に集約してバッチ化する
