# FoundPhone Sites Private Runtime Validation

## Top Strip

| Item | Value |
|---|---|
| Purpose | Unity Web版ではないSites-native軽量チャットのprivate review |
| Content status | `prototype fixture / not final story`。Ch1/Yarn canonではない |
| Canonical static input | `sites/foundphone-demo/` at Unity repository commit `1e582639b5730e787debd20d2131147e6515032d` |
| Sites project | `appgprj_6a5ddb11908081918180da7797957e63` / `FoundPhone Signal Preview` |
| Site slug | `foundphone-signal-preview` |
| Sites source commit | `29c5245d1f0a21802a98c0009c038f48cd746eca` |
| Saved version | Version 1 / `appgprj_6a5ddb11908081918180da7797957e63~appgver_59dabdbf36488191904886c9bae23c3c` |
| Deployment | `appgdep_6a5ddc134b508191ab81ab6dd44765aa` / succeeded |
| Private URL | `https://foundphone-signal-preview.thankyoukass.chatgpt.site` |
| Created / validated | 2026-07-20。Sites control planeとaccess policyは2026-07-21に再取得 |

## Outcome

静的fixtureをVinext / React / Cloudflare Workers互換のSites runtimeへ等価変換し、build、lint、rendered HTML test、ローカルruntimeでの両分岐完走、keyboard、focus、responsive、禁止機能監査を通した。Sites Version 1は上記source commitを参照し、private deploymentは成功している。

2026-07-21再取得時点のaccessは`custom`、allowlistはaccount user 1名、workspace group 0、tenant group 0である。Ownerの識別子、メール、短期source credential、SIWC bypass token、期限付きscreenshot URLは正本へ保存しない。

## Platform Semantics / Public Boundary

- deploymentは`deploy_private_site_version`経路で作成した。
- Sites control planeはこのdeploymentを`type=publish`、URLを`current_live_url`として返し、`current_preview_url`は`null`である。Sitesではhosted URLがproduction-classであり、ここでいうprivate previewは「Owner-only accessのhosted runtime」を指す。
- これはpublic accessを意味しない。access modeは`custom`のまま、allowlist外ユーザー、workspace group、tenant group、custom domainへの共有は行っていない。
- public accessへの変更、workspace共有、custom domain、live store link、外部配布は別のOwner Gateまで禁止する。

## Static Package To Sites Runtime Mapping

| Static source | Sites runtime equivalent | Preserved behavior / change |
|---|---|---|
| `index.html` | `app/page.tsx` + `app/FoundPhoneDemo.tsx` | semantic surfaceをserver wrapperとclient interactionへ分離 |
| `styles.css` | `app/globals.css` | mobile-first、focus-visible、320–430px、reduced-motionを維持 |
| `app.js` | React reducer/state、focus management、local-only custom events | intro → sequential chat → choice → branch → ending → restartを維持 |
| `content/demo.json` | rootの`content/demo.json`をbuild-time import | 表示内容の正本。runtime fetchを廃止し、内容の意味を変更しない |
| `README.md` / `SITES_IMPORT_BRIEF.md` | project rootの内部文書 | 画面本文にはrenderしない |
| なし | `app/layout.tsx`、Vinext/Vite/Worker設定、TypeScript、build/test設定、`public/favicon.svg` | Sites build/hostに必要なruntime plumbingのみ追加 |

静的packageとSites projectは同一Git repositoryではない。Unity repositoryの`sites/foundphone-demo/`をportableな入力正本とし、Sites projectのsource commitをhosted変換の正本として扱う。

## Interaction / Accessibility Validation

| Check | Result |
|---|---|
| JSON semantic parity | static sourceとSites側`content/demo.json`を比較してpass |
| Flow | intro → sequential chat → 2択 → 分岐応答 → ending → restartをpass |
| Branches | `blue_signal`と`white_noise`でcontinuation / outcomeが異なることをpass |
| Restart | step 1/6、未選択、初期messageへresetすることをpass |
| Keyboard | Enter / Spaceでnative buttonを起動し、両ルートを完走 |
| Focus | thread → choice → endingへ移動。明瞭なfocus-visible ringを維持 |
| A11y | `aria-live`、`progressbar`、skip link、native buttonを確認 |
| Reduced motion | `prefers-reduced-motion` pathをCSSで維持 |
| Portrait width | 320 request時のclient 305px、390 request時のclient 375px、430pxでhtml/body/shell幅が一致し、横overflowなし |
| Runtime console | application warning/errorなし |
| HTTP assets | app、build assets、faviconがlocal Worker runtimeで200 |

ローカルQAはbuild済みWorkerをWranglerで起動して行った。Wrangler CLIは匿名telemetry noticeを表示したが、これは開発toolchainの通知であり、アプリruntimeにexternal analyticsを追加したものではない。

## Prohibited Capability Audit

app source、rendered HTML、runtime behaviorを確認し、次を追加していない。

- external application request / external analytics
- authentication implementation / login form / personal-data input
- database / D1 / R2 / persistent storage
- API key / secret / environment variable dependency
- form / payment / checkout / card data / transaction
- live store link / public sharing / custom domain

Sites標準のprivate-access sign-in gateはhost platformの境界であり、FoundPhone app内のauth実装ではない。未認証状態でprivate URLを開くと`Continue with ChatGPT` gateが表示された。認証・ログインを自動化しない制約に従い、Ownerとしてのsign-inは実施していない。

## Validation Commands / Results

- `npm run lint`: pass
- `npm test`: build pass + Node tests 3/3 pass
- `npm ls --depth=0`: exit 0。machine-local `node_modules`は`@emnapi/wasi-threads@1.2.1`と`@tybys/wasm-util@0.10.2`をextraneousと表示
- local Worker browser QA: both branches、restart、keyboard/focus、320–430px、consoleをpass
- source static audit: prohibited network/storage/form/secret patternなし
- `git status --short --branch`: Sites source worktree clean at `29c5245d...`

`npm install`はtoolchain dependencyに11 vulnerabilities（low 2 / moderate 3 / high 6）を報告した。sourceを変える`npm audit fix --force`は実行していない。extraneous 2 packageはuntrackedなlocal dependency stateで、validated source / lockfileを変える`npm prune`も実行していない。次にSites sourceを更新するlaneで、build互換性を保ったdependency更新として分離評価する。

## Version / Archive Provenance

- Version 1 source: `29c5245d1f0a21802a98c0009c038f48cd746eca`
- Sites archive content hash: `sha256:0533759035a45b0d9fea9b232bcfd2ae05a360a9bbab167939cc8c2856101ea8`
- Sites archive inventory: 36 files / 1,720,320 bytes
- machine-local recovery archive: `foundphone-sites-private-preview-v1.tar.gz`
- machine-local archive SHA-256: `D87A0FF4CC5C46FB1A8FBFAEE058F0B4CF37539933AFA77699F470C748C3245C`

machine-local archiveとvisualization worktreeは補助証跡であり、別端末の正本にはしない。

## Cross-Terminal Resume

1. Unity repositoryの`main`をpullし、`docs/HANDOFF.md`と本書を読む。
2. portableな入力は`sites/foundphone-demo/`、hosted runtimeの識別子はTop Stripを使う。
3. Sites connectorでprojectを取得し、access mode、allowlist件数、Version 1のsource SHA、deployment statusを再確認する。
4. Sites sourceを修正する場合だけ短期source repository credentialを取得し、credentialをGit remote、文書、shell historyへ保存しない。local visualization pathは別端末に存在する前提にしない。
5. Ownerがprivate URLを開き、Sites標準sign-in後にhosted runtimeの両分岐、restart、keyboard、320–430px、network panelを確認する。public/shared accessへ変更しない。

## Remaining Gate

Hosted source、private deployment、access policyは確認済み。未完了なのは、Owner認証後の実hosted画面で行う最終reviewだけである。

- 目的: local Workerと実Sites dispatchの差を閉じる。
- 効果: actual hosted runtimeをprivate reviewとして受け入れ可能にする。
- 要件: Ownerの手動sign-in、access policyを変更しないこと。
- 状態: 未認証gateまでは確認済み。app本文のhosted実画面は未確認。
- owner: User / Web Supervisor。
- next move: private URLで両分岐、restart、keyboard、responsive、networkをOK/NG記録する。
