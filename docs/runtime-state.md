# Runtime State

最終更新: 2026-11-26

このファイルは共有可能な環境・検証条件だけを保持する。現在の作業判断は `docs/HANDOFF.md`、履歴はGit、監修判断は`docs/SUPERVISOR_REPORT.md`を参照する。

## 実行基準

| 項目 | 現在値 |
|---|---|
| Unity | 6000.4.9f1 |
| C# / scripting | Unity project defaults |
| Yarn Spinner | 3.1.3 |
| 改修前の基準 | `main@4c09dcd` |
| NarrativeGen Unity SDK | `com.yushimoji.narrativegen@43dc307`（Git revision固定。画面フレーム `ScreenFrameBuilder` を含む） |
| Active authoring surface | `Tools > FoundPhone > Writer Cockpit` |
| Active runtime scene | `ContentAuthoring` / `DebugChatScene` |
| Bridge candidate | `origin/codex/sites-authoring-bridge-v1@e059e4b`、main未統合 |
| Sites static input | `sites/foundphone-demo/` |
| Sites access | Version 1 deployed、Owner-only / custom user 1 / group 0 |

## 最新の受入済み基準

- 2026-07-26 main development readiness:
  - Unity 6000.4.9f1 batch open: 39 packages、Tundra success、334 evaluated、return code 0
  - Yarn validator: `errors=0 / warnings=0 / info=3`; 11 files、74 nodes、24 `#line:` tags、42 variables
  - Writer Cockpit navigation targeted EditMode: 14/14 pass
  - Sites static validatorとMkDocs strict build: pass
  - tracked code / Yarn / scene / asset / package差分なし
- 2026-07-27 bridge candidate:
  - exact `e059e4b`をUnity 6000.4.9f1でcompile、return code 0
  - targeted Editor tests 18/18 pass
  - Writer Cockpitと`Export Sites Preview Package`を可視Editorで確認
  - fixture/generated Package v1のHTTP 200、MIME、`nosniff`、`no-store`、browser開始、console warning/error 0
  - candidateはmain未統合。PR、merge、deploy、Sites access変更なし
- Sites hosted runtime:
  - Version 1 sourceとdeployment success、Owner-only accessを確認
  - local runtimeで両分岐、restart、keyboard、focus、ARIA、320–430px、禁止機能監査をpass
  - Owner sign-in後のhosted本文reviewは未実施

## 保存・分岐・NarrativeGen試遊の検証

- 2026-11-26、SDK `43dc307`へ更新し、試遊を画面フレームの描画へ置き換えた。Unity 6000.4.9f1のbatchでEditMode 90/90（新規`NarrativeGenChatScreenTests` 2件を含む）、PlayMode 11/11（試遊は画面フレームの全行がチャット履歴に出ることを照合）。NarrativeGen側でC#とTSのフレーム一致（4経路の全手順）を.NET 44/44で確認。試験でTMPの動的フォント資産に字形が足された差分は戻した。Unity実画面の目視は未実施。
- 2026-11-26、SDKの.NET試験38/38。Studioの現行3作の各1経路が終端に到達し、「藍布の耳」の属性条件と「潮待ちの石段」の時刻条件を通過。Unity PlayModeは7/7。「六目の分銅」の話者名と本文を照合し、`look_baker` → `tell_baker_clerk` → `look_clerk` → `verify_baker` で結末に到達。試遊はセーブファイルを作らず、同じ実行で既存6件も通過。

- 2026-11-25、Unity 6000.4.9f1でセーブ EditMode 8/8、既存チャットとセーブ PlayMode 4/4。セーブファイルの新規作成・上書き・読込、タイトルから通常チャットに入り保存と再読込を確認。
- NarrativeGen C# SDK .NET 35/35。チーズバーガーの生成モデル2本で知覚イベントの有無、次の選択肢、0.1と0.12を展開した本文を照合。
- C# SDKとTypeScript版で、生成5モデルの初手ごとに次の先頭選択肢を選ぶ12経路を比較。場面ID、選択肢ID、イベントID、展開本文が一致した。
- SDK固定参照後、Unity PlayMode 5/5。実際の `cheeseburger-weight.json` を既存チャットに読み込み、探偵Aの `look_detective_a` → `verify_detective_a` で終端へ到達。同じ実行で既存回帰4件を確認。
- 分岐スレッドの同じIDへのロード後再入場、取得情報の保持、持ち帰り選択、終了時のクリアをC#回帰試験で確認。

## Package Managerと起動条件

- `tools/run-unity.ps1`は`ProjectSettings/ProjectVersion.txt`からUnityを選び、呼び出し元に`ALLUSERSPROFILE`がない場合だけ子プロセスへCommonApplicationDataを設定する。user / system環境変数は変更しない。
- 過去のfresh resolveの`path undefined`はpackage JSONではなく、標準Windows環境変数の欠落が直接原因。wrapper経由ではProjectCacheなしからresolve、39 packages登録、compileに到達済み。
- `Packages/manifest.json` / `packages-lock.json`の削除や手動再生成を通常の復旧手順にしない。依存契約を変える場合は別スライスとして扱う。

## テスト安全条件

- 基礎テスト定義はEditMode 73 / PlayMode 10。targeted Editorはmain 14、bridge candidate 18であり、assembly / SHA別に記録する。
- 保存操作を含むEditMode / PlayModeはテストごとに一意の一時フォルダを使う。通常の `Application.persistentDataPath` と分離する。
- External Script Editorのfile / line jump、Writer CockpitのApply / Play / Last Action、bridge Exportの操作感はinteractive人間環境レビュー待ち。
- direct Unity WebはWeb Build Support、有効なpublic gameplay scene、TitleSceneからのnavigationが揃うまでblocked。

## 生成物

- `.mkdocs-view/`, `.mkdocs-site/`, `Library/`, `Logs/`, `Temp/`はignored local output。
- 検証ログ、test XML、generated preview JSON、package cacheは再現補助であり、秘密情報を含めず、必要な結果だけを正本へ転記する。
- Sitesのcredential、token、Owner識別子、期限付きURLは保存しない。
