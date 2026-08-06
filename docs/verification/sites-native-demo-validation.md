# Sites-native FoundPhone Demo Validation

## Top Strip

| Field | Value |
| --- | --- |
| Thread | `sites-native-002` |
| Lane | `sites-native-chat-demo` |
| Epoch | `20260718-02` |
| Base | `55cb0d20c1d92a21e6e6ecb0db5d2fdbdfe562d7` |
| Branch | `spike/sites-native-chat-demo` |
| Worktree | `C:\Users\thank\Storage\Game Projects\UnityChatNovelGame-sites-native-chat-demo` |
| Validated | 2026-07-19 JST |

## Outcome

Unity、backend、auth、package install に依存しない FoundPhone chat-novel prototype を `sites/foundphone-demo/` に作成した。ローカル HTTP server 上で intro から2種類の分岐 endingまで完走し、restart、desktop/mobile layout、focus、local-only semantic events を確認した。

actual ChatGPT Sites runtime への package compatibility は、private preview に import するまで未検証。Site 作成、private preview import、public deployment は行っていない。

## Artifact / Local URL

- artifact root: `sites/foundphone-demo/`
- local server: `tools/sites/serve-demo.ps1`
- validation URL: `http://127.0.0.1:4317/`
- default port `4173` は開始時に既存プロセスが使用中だったため、そのプロセスを停止せず `4317` を使用した。

```powershell
.\tools\sites\serve-demo.ps1 -Port 4317
```

## Content Source / Non-canon Status

- presentation と content は `content/demo.json` で分離。
- content id: `foundphone-signal-fixture-v1`。
- canon status: `non-canon verification fixture`。
- 画面上に `Prototype content / not final story` を常時表示。
- Ch1、Yarn 本文、承認済み story dialogue は使用していない。

## Interaction Flow

`landing / intro → start → system fixture notice → relay message → choice → branch response → common closing → ending CTA`

- `blue_signal`: 双方向の短い通信が残る検証用 ending。
- `white_noise`: 端末内の記録再生だけが残る検証用 ending。
- ending の `デモをもう一度` で message list、progress、choice state が `1 / 6` へ初期化されることを確認。
- `今後のリリース注記` は local text を展開するだけで、link、購入、予約、入力を持たない。

## Artifact Inventory

| File | Bytes |
| --- | ---: |
| `app.js` | 9,819 |
| `content/demo.json` | 3,279 |
| `index.html` | 4,694 |
| `README.md` | 1,560 |
| `SITES_IMPORT_BRIEF.md` | 3,265 |
| `styles.css` | 9,229 |
| **Total** | **31,846** |

## Desktop / Mobile Evidence

| Viewport request | Observed content width | Result |
| --- | ---: | --- |
| desktop `1280 x 900` | shell `500px` | horizontal overflowなし、intro / ending表示可 |
| mobile `390 x 844` | client / shell `375px` | horizontal overflowなし、start button `52px` high |
| narrow `320 x 700` | client `305px` | horizontal overflowなし、heading `278px` wide |

mobile choice surface は width `355px`、choice button は `48px` high。選択肢、progress、message bubble、ending CTA は portrait width 内に収まった。

## Accessibility Evidence

- `lang="ja"`、main / region / heading / list / progressbar / status landmarks を browser DOM snapshot で確認。
- 操作対象は native `button` / `a`。choice 表示時に最初の choice button へ focus が移動した。
- `:focus-visible` の明瞭な yellow ring を desktop/mobile screenshot で確認。
- start / choice / ending の focus target を DOM snapshot の `[active]` で確認。
- `prefers-reduced-motion: reduce` で animation / transition を実質停止。
- browser controller の `press` 注入では state transition が発火しなかったため、Enter/Space 実入力による activation は未証明。native control semantic と focus path は確認済みで、custom pointer-only control は存在しない。

## Validation Checks

| Check | Result |
| --- | --- |
| `content/demo.json` JSON parse / schema-equivalent validation | pass |
| graph reachability / 2-way choice / ending reachability | pass |
| `node --check sites/foundphone-demo/app.js` | pass |
| static server / JS syntax | pass |
| `GET /` | `200 text/html; charset=utf-8` |
| `GET /styles.css` | `200 text/css; charset=utf-8` |
| `GET /app.js` | `200 text/javascript; charset=utf-8` |
| `GET /content/demo.json` | `200 application/json; charset=utf-8` |
| response protection | `X-Content-Type-Options: nosniff`, `Cache-Control: no-store` |
| blue route completion | pass |
| white route completion | pass |
| route-specific continuation / outcome | pass |
| restart state reset | pass |
| browser console error / warning | 0 |
| local semantic events | `demo_started`, `choice_selected`, `demo_completed`, `outbound_store_intent` observed |

## Prohibited Capability Audit

`tools/sites/validate-demo.mjs` で runtime surface を対象に確認。

- personal-data input / form: none
- card number / checkout / payment handler: none
- external analytics endpoint: none
- external runtime URL: none
- secret-like value / API key: none
- browser persistent storage: none
- auth / login / database / backend: none
- live store destination / transaction: none
- public publish / custom domain: not performed

semantic events は browser 内 console と `foundphone:event` にだけ出し、外部送信しない。

## Sites Import / Readiness Assessment

静的6ファイル、31,846 bytes、external runtime dependencyなし。`SITES_IMPORT_BRIEF.md` に private preview 用 input、content/privacy constraints、checklist、public gate、external monetization boundaryを記録済み。

local review package としては ready。actual Sites runtime compatibility、private access behavior、Sites側 import 後のnetwork/keyboard/layoutは未検証。

## Public Deployment / Monetization Boundary

- ChatGPT Site は作成していない。
- private preview / public deployment は未実施。
- deployment URLを生成していない。
- CTA は local-only note。checkout、payment、card data、financial transaction、deceptive purchase button はない。
- 将来 monetization は承認済み external store / native appへの導線として別laneで扱う。

## Exact Remaining Blockers

1. actual Sites runtime compatibility は private preview import なしでは判定できない。actor / owner は Human + Web Supervisor。
2. browser controller 経由の Enter / Space activation は未証明。native semantic / focusは確認済みだが、private previewで実キーボード再確認が必要。
