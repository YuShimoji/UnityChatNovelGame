# FoundPhone Sites-native Chat Demo

Unity、backend、package install を使わない静的チャットノベル prototype。内容はすべて **非カノンの検証用 fixture** で、Ch1 本編は含まない。

## Local review

repository root から実行する。

```powershell
.\tools\sites\serve-demo.ps1
```

ブラウザで `http://127.0.0.1:4173/` を開く。停止は server terminal で `Ctrl+C`。

別 port を使う場合:

```powershell
.\tools\sites\serve-demo.ps1 -Port 4317
```

## Validation

```powershell
node --check .\sites\foundphone-demo\app.js
node .\tools\sites\validate-demo.mjs
git diff --check
```

## Interaction

`intro → start → sequential messages → choice → branch response → ending` の一本道。choice の選択肢によって後続メッセージと ending result が変わる。すべて native button / link で操作でき、明示的な focus ring と reduced-motion path を持つ。

semantic event は browser 内の `foundphone:event` とローカル console にだけ記録する。外部送信はない。

- `demo_started`
- `choice_selected`
- `demo_completed`
- `outbound_store_intent`

## Boundaries

- account、login、email/name capture、database、storage、analytics、secret はない。
- checkout、payment form、card data、transaction、live store link はない。
- actual ChatGPT Sites runtime への package compatibility は、private preview に import するまで未検証。
- public deployment は human gate。この artifact は Site を作成・公開しない。
