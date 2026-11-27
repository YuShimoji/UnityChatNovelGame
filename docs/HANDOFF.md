# Handoff

最終更新: 2026-11-27

会話ログなしで再開するための唯一のライブ現在地。履歴は追記せず Git に委ねる。詳細な監修判断は `docs/SUPERVISOR_REPORT.md`、環境条件は `docs/runtime-state.md` を参照する。

## 現在地

- 汎用アドベンチャーの作業ブランチは `adventure-reader`。Unity 6000.4.9f1で [TitleScene](../Assets/Scenes/TitleScene.unity) を開いて Play。作品一覧から WritingPage の日記・聞き込み4作とゲームブック分岐デモを読む、NarrativeGen の「藍布の耳」を遊ぶ、または FoundPhone の本編を開始できる。
- セーブのファイル操作を `SaveSlotStorage`、分岐中の取得情報・持ち帰り選択を `BranchThreadSession` に分離した。Yarnの呼び出し、セーブJSON、スロット番号は既存の契約を使う。
- NarrativeGen SDK `43dc307` をUnityへ固定参照。試遊はSDKの画面フレーム（NarrativeGenの[SP-SCREEN-001](https://github.com/YuShimoji/NarrativeGen/blob/main/docs/specs/screen-frame.md)）を `NarrativeGenChatScreen` がチャットへ描く。Studioの会話窓と同じく、話者「場所｜名前」を場所の行と話者名つき吹き出しに分け、段落ごとの吹き出し、話題と行き先（`→`）の選択肢、聞いた話題の「（済）」、旅の手帳の記録、依頼完了を出す。試遊はメモリ内の別セッションで、YarnやFoundPhoneの保存を起動しない。
- 既存の基盤工程はSP-023/024の表示確認 → M1（A/B/C・Latent・Branch）→ M2（セーブの状態同値と章遷移）→ M3（alpha判定）。今回の分岐状態と保存の分離はM1/M2の改修に使う。詳細は[開発順序](project-context.md)。

## WritingPage の作品を追加する

WritingPage の読者表示で「HTMLとして保存」し、Unity の `Tools > Adventure > Prepare WritingPage HTML...` で開く[変換画面](../tools/prepare-adventure.html)に選ぶ。保存した `.adventure` を [Assets/Resources/Adventures](../Assets/Resources/Adventures) に置くと、次のタイトル起動から選べる。画像は埋め込みのまま取り込み、章の目次・前後移動・伏せ書きが動く。

ゲームブックでは `[事務所に入る](#事務所)` で同名見出しへ移動できる。[WritingPage の分岐デモ原稿](https://github.com/YuShimoji/WritingPage/blob/3245489957e96f406d89a71a47cf29f8b1ed8ff1/samples/gamebook-styled-demo.md)と同じく会話枠と地の文を混在できる。変換時にモデルも選ぶと `{variable:枚数}` が現在値になり、`[受け取る](#choice/take)` でその手の選択を実行する。次のノードIDと一致する章・見出しへ進み、一致しなければ今の原稿を更新する。

タイトルの「リンクと変数の見本」では、本文中の「三枚受け取る」を押すと受取記録が0枚から3枚に変わり、「枚数をゼロに戻す」が選べる。

実行するゲームは [Assets/Resources/AdventureModels](../Assets/Resources/AdventureModels) に SDK 対応の JSON を置く。同梱例は[固定SDKと同じ版の「藍布の耳」](https://github.com/YuShimoji/NarrativeGen/blob/43dc3076990aff855927c82436c680b5c1d58a66/models/examples/blue-selvedge.json)。「七人目の渡し賃」は WritingPage の記録版を同梱。元ゲームの `perceptionPolicies` は固定 SDK の読込対象外。

## NarrativeGenのチャット試遊

1. Unity 6000.4.9f1でこのプロジェクトを開く。
2. `Tools > FoundPhone > Play NarrativeGen JSON...` で、NarrativeGenの [`models/examples/blue-selvedge.json`（藍布の耳）](https://github.com/YuShimoji/NarrativeGen/blob/main/models/examples/blue-selvedge.json) を選ぶ。
3. [固定の選択経路・期待フレーム](https://github.com/YuShimoji/NarrativeGen/blob/main/packages/tests/NarrativeGen.Tests/ScreenFrames/blue-selvedge.json) の順に、Gameビューのチャットで結末まで進める。Studioと場所・話者・段落・話題と行き先・旅の手帳・結末を照合する。終了時はPlay Modeを止める。Play Mode開始シーン設定は元に戻る。

Editor のチャット試遊は 6 選択で結末まで進め、Studio の場所・話者・段落・話題・手帳3件・依頼完了と照合済み（`79a6882`）。タイトルから開く新しい聞き込み画面にも同じ選択経路を使う。[3プロジェクトの申し送り](https://github.com/YuShimoji/NarrativeGen/blob/main/HANDOVER.md#unitychatnovelgamewritingpageとの接続)。

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
| Git | 既存のチャット試遊は `main`、読書・ゲームブック・モデル連携は `adventure-reader`。両方とも NarrativeGen SDK `43dc307` | 作者による表示の受入と main への取り込み |
| Unity / C# | `adventure-reader` を Unity 6000.4.9f1で EditMode 91/91、PlayMode 15/15。タイトルから日記・分岐・変数連携・FoundPhoneのチャットとセーブを確認。1200×800と390×844で実画面・場面フェードを確認 | 作者による再現度の受入、モバイル実機、FoundPhone の M1/M2 全体 |
| Windows x64 | 通常のBuild Settingsから作成した `Adventure.exe` を起動。作品一覧から「リンクと変数の見本」を開き、本文クリックで0枚→3枚→0枚を確認 | 他端末での動作と公開配布 |
| Writer Cockpit | main navigationは14/14、bridge candidateは18/18 | candidateのExport操作とApply / Play / Last Actionの人間受入 |
| 基礎回帰 | スロット書込・削除をテスト専用フォルダに隔離。既存セーブ EditMode 8件、通常チャット PlayMode 4件が通過 | 対象外の基礎テストと章遷移の状態同値 |
| Sites static package | fixture validator、両分岐、responsive / accessibility smokeを確認済み | hosted本文はOwner sign-in後review待ち |
| Docs | 開始差分で削除参照0、`git diff --check`、MkDocs strict build pass | 最新事実統合後の再実行 |

## 残る不確実性

- bridge candidateはreview-readyだがmain未統合。人間の作者UX受入とmain統合判断は別ゲートであり、このスライスでは代行しない。
- External Script Editorが設定された人間環境で、Nodeとdiagnosticの実file / line jumpは未受入。
- Writer CockpitのApply / Play / Last ActionとExport操作の人間受入は未完了。
- FoundPhone の全型スレッド・章遷移の実機確認、全作品共通の途中再開は残る。
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
