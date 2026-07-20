# Sites Import Brief — FoundPhone Static Chat Prototype

## Artifact Purpose

FoundPhone / chat-novel 方向の最小 interaction を、Unity Web Build Support や Unity gameplay scene に依存せず private review する静的 prototype。公開物や製品版ではなく、Sites runtimeへ変換するportableな入力 packageである。2026-07-20のprivate runtime変換結果は`docs/verification/2026-07-20-sites-private-runtime-validation.md`を正とする。

## Repository Location / Entry Point / Local Serve

- repository path: `sites/foundphone-demo/`
- expected entry point: `sites/foundphone-demo/index.html`
- local server wrapper: `tools/sites/serve-demo.ps1`
- repository root からの起動: `./tools/sites/serve-demo.ps1`
- port指定例: `./tools/sites/serve-demo.ps1 -Port 4318`

HTMLを`file://`で直接開かず、同一originのHTTP配信で`content/demo.json`を読み込む。Node.js以外のpackage install、backend、framework、CDNは不要。

## File Inventory

| Path | Purpose |
| --- | --- |
| `index.html` | semantic landing / chat / ending surface |
| `styles.css` | mobile-first visual language、focus、reduced motion |
| `app.js` | local state machine、choice、progress、local-only events |
| `content/demo.json` | presentation から分離した非カノン fixture content |
| `README.md` | local serve / validation / boundary |
| `SITES_IMPORT_BRIEF.md` | Sites private preview handoff |

## Intended Sites Prompt / Input

Private preview で次を入力条件として使う。

> 添付した静的 HTML/CSS/JavaScript/JSON package の情報設計、モバイル優先レイアウト、アクセシビリティ、非カノン content 境界を保って private preview を作成してください。外部通信、analytics、認証、個人情報収集、storage、決済、live store link を追加しないでください。`content/demo.json` を表示内容の正本として扱い、public publish は行わないでください。

## Content / Privacy Constraints

- `content/demo.json` は prototype fixture。承認済み canon ではない。
- Ch1 story、Yarn 本文、writer-owned dialogue は含めない。
- email、name、account、device identifier その他の個人情報を収集しない。
- backend、database、auth、persistent storage、secret、API key、external analytics を追加しない。
- semantic events は local-only。外部 endpoint へ送信しない。

## Private Preview Checklist

- [ ] package の全ファイルが import される
- [ ] `content/demo.json` が表示正本としてfetchまたはbuild-time importされる
- [ ] intro から ending まで keyboard で完走できる
- [ ] 2つの choice が異なる continuation / outcome を表示する
- [ ] restart で state と message list が初期化される
- [ ] 320–430px portrait で横 overflow がない
- [ ] focus indicator と reduced-motion が維持される
- [ ] network panel に外部 request がない
- [ ] form、personal-data input、transaction UI が追加されていない
- [ ] preview access が private のまま

2026-07-20にVinext / React / Worker互換のSites runtimeへ等価変換し、local runtime validationとOwner-only deploymentまでは完了した。残るgateはOwner認証後のhosted画面reviewである。

## Public Deployment Gate

deployment URL は production-classとして扱う。private deployment APIで作成したSites runtimeは`custom` access / Owner 1名 / group 0のまま維持する。Sites control plane上の`type=publish` / `current_live_url`をpublic accessと読み替えず、public access、workspace共有、custom domainは別のHuman Gateまで行わない。

## External Monetization Boundary

この prototype 内に checkout、payment form、purchase control、card data 処理、financial transaction を置かない。将来 monetization を検討する場合は、承認済み native app / external store の情報ページへ向かう明示的な outbound link を別 lane で設計する。現在の CTA は live destination を持たないローカル注記だけである。
