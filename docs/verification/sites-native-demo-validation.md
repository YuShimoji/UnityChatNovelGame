# Sites-native FoundPhone Demo Validation

> 2026-07-19の静的package統合記録。後続のSites runtime変換、Owner-only access、Version 1、deployment、未完了のOwner認証後reviewは`docs/verification/2026-07-20-sites-private-runtime-validation.md`を正とする。

## Top Strip

| Field | Value |
| --- | --- |
| Thread | `integrate-sites-003` |
| Lane | `sites-native-demo-integration` |
| Epoch | `20260719-03` |
| Base | `b4e92ecb4f05a923b9177138fcf026fcfb561bba` |
| Branch | `integrate/sites-native-demo` → `main` fast-forward target |
| Worktree | `C:\Users\thank\Storage\Game Projects\UnityChatNovelGame` |
| Source | `UnityChatNovelGame-sites-native-chat-demo` / `spike/sites-native-chat-demo` / `55cb0d20` |
| Validated | 2026-07-19 JST |

## Outcome

Unity、backend、auth、package installに依存しないFoundPhone chat-novel prototypeを、source siblingの一時成果から`sites/foundphone-demo/`のtracked repository artifactへ再構成した。ローカルHTTP server上でintroから2種類の分岐endingまで完走し、restart、desktop/mobile/narrow layout、focus、local-only semantic eventsを再確認した。

actual ChatGPT Sites runtime への package compatibility は、private preview に import するまで未検証。Site 作成、private preview import、public deployment は行っていない。

## Artifact / Local URL

- artifact root: `sites/foundphone-demo/`
- local server: `tools/sites/serve-demo.ps1`
- validation URL: `http://127.0.0.1:4318/`（検証後停止済み）
- port `4317` はsource siblingの既存serverが使用中だったため、そのprocessを停止せず空いていた`4318`を使用した。

```powershell
.\tools\sites\serve-demo.ps1 -Port 4318
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
| `SITES_IMPORT_BRIEF.md` | 3,774 |
| `styles.css` | 9,229 |
| **Total** | **32,355** |

## Source Provenance / Hash Comparison

source siblingはread-onlyで扱い、下記10ファイルをprimaryへ取り込んだ直後にSHA-256が全件一致した。`SITES_IMPORT_BRIEF.md`と本検証文書だけは、その後integration contractに合わせてprimary側で更新した。

| Path | Source SHA-256 | Initial integration |
| --- | --- | --- |
| `sites/foundphone-demo/app.js` | `DF14AB07B094952FD1569E8AFFFCF662181B18DF28A6F4A5E0894BDC1F83A78A` | exact |
| `sites/foundphone-demo/index.html` | `94CC3E384EB5CC9C9A5A667F59095423E0D53A1392EC6E3EAD7AB9BCB433C70C` | exact |
| `sites/foundphone-demo/README.md` | `B5C495E84E250EEF6EA9F2C35EA625750A0DA4DB695FDC1B943A373B3A94A257` | exact |
| `sites/foundphone-demo/SITES_IMPORT_BRIEF.md` | `0A208310CCA6398E4BFBED991126A5BE432BE6F118EB3DDE90517446A9890353` | exact before integration repair |
| `sites/foundphone-demo/styles.css` | `D7C8C020D5F7ACC1F2626D7CF8D13565496C8311E07E50318B35B11704E2632B` | exact |
| `sites/foundphone-demo/content/demo.json` | `0A4230BFDF0B63D19DF0EE4F7FB556BB9F09DFD0372312214296E1D602419BFC` | exact |
| `tools/sites/serve-demo.ps1` | `33671F266FF3B0EC75CE8D87D9C188A8D4F3EA135D121E3BE7D0A11E35511D23` | exact |
| `tools/sites/static-server.mjs` | `E6B67945A70F516F04840F57F9D31EF63E48916747F0FEB75682202DC81C8DED` | exact |
| `tools/sites/validate-demo.mjs` | `A182F1EA2C5598EE90B8C79C2556FC14F12F30B69F4D47EDF1BD91892AD62891` | exact |
| `docs/verification/sites-native-demo-validation.md` | `63078F30951F0B42E9B1B81B36C9B959F6A647A6E89348424E3D46E7FD586F99` | exact before integration rewrite |

post-repairの`SITES_IMPORT_BRIEF.md`は`CCF300F207BC782BF3329A1324963F641A31C7C63EBF4C47331BA388F6C7A732`。runtime HTML/CSS/JS/JSONとserver/validatorはsource hashを維持した。

## Desktop / Mobile Evidence

| Viewport request | Observed content width | Result |
| --- | ---: | --- |
| desktop `1280 x 900` | shell `500px` | horizontal overflowなし、intro / ending表示可 |
| mobile `390 x 844` | shell `390px`、choice surface `354.8px` | horizontal overflowなし、ending/release note表示可 |
| narrow `320 x 700` | client `305px`、choice surface `277.8px` | horizontal overflowなし、choice/progress/message表示可 |

mobile choice surface は width `355px`、choice button は `48px` high。選択肢、progress、message bubble、ending CTA は portrait width 内に収まった。

## Accessibility Evidence

- `lang="ja"`、main / region / heading / list / progressbar / status landmarks を browser DOM snapshot で確認。
- 操作対象は native `button` / `a`。choice 表示時に最初の choice button へ focus が移動した。
- `:focus-visible`の`3px solid rgb(255, 217, 121)` ringをcomputed styleとdesktop/mobile screenshotで確認。
- start / choice / ending の focus target を DOM snapshot の `[active]` で確認。
- `prefers-reduced-motion: reduce` で animation / transition を実質停止。
- browser controller の `press` 注入では state transition が発火しなかったため、Enter/Space 実入力による activation は未証明。native control semantic と focus path は確認済みで、custom pointer-only control は存在しない。

## Validation Checks

| Check | Result |
| --- | --- |
| `content/demo.json` JSON parse / schema-equivalent validation | pass |
| graph reachability / 2-way choice / ending reachability | pass |
| `node --check sites/foundphone-demo/app.js` | pass |
| static server / validator JavaScript syntax | pass |
| `serve-demo.ps1` PowerShell parser | pass |
| `GET /` | `200 text/html; charset=utf-8` |
| `GET /styles.css` | `200 text/css; charset=utf-8` |
| `GET /app.js` | `200 text/javascript; charset=utf-8` |
| `GET /content/demo.json` | `200 application/json; charset=utf-8` |
| response protection | `X-Content-Type-Options: nosniff`, `Cache-Control: no-store` |
| blue route completion | pass |
| white route completion | pass |
| route-specific continuation / outcome | pass |
| restart state reset | pass |
| desktop / mobile / narrow horizontal overflow | none |
| browser console error / warning | 0 |
| local semantic events | `demo_started`, `choice_selected`, `demo_completed`, `outbound_store_intent` observed |
| declared external runtime URL / observed external resource URL | 0 / 0 |
| temporary server after validation | stopped; URL is not reported as live |

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

静的6ファイル、32,355 bytes、external runtime dependencyなし。`SITES_IMPORT_BRIEF.md`にrepository path、entry point、local serve command、private preview用input、content/privacy constraints、checklist、public gate、external monetization boundaryを記録済み。

tracked local review packageとしてはready。actual Sites runtime compatibility、private access behavior、Sites側import後のnetwork/keyboard/layoutは未検証。

## Public Deployment / Monetization Boundary

- ChatGPT Site は作成していない。
- private preview / public deployment は未実施。
- deployment URLを生成していない。
- CTA は local-only note。checkout、payment、card data、financial transaction、deceptive purchase button はない。
- 将来 monetization は承認済み external store / native appへの導線として別laneで扱う。

## Exact Remaining Blockers

1. actual Sites runtime compatibility は private preview import なしでは判定できない。actor / owner は Human + Web Supervisor。
2. browser controller 経由の Enter / Space activation は未証明。native semantic / focusは確認済みだが、private previewで実キーボード再確認が必要。
