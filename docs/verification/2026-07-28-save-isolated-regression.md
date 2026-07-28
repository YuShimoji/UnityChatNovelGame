# 2026-07-28 Save-Isolated Regression

## 目的

Unity 6000.4.9f1で基礎EditMode 73件、PlayMode 10件、targeted Editor 14件を同日実行し、実ユーザーの`Application.persistentDataPath`を変更せず現行97件の基準を固定する。

## 安全境界

- `tools/run-unity.ps1 -IsolateTestSaveData`は、OS一時領域の`FoundPhoneTests`配下へ実行ごとの専用directoryを作り、子Unity processだけに隔離rootとsave directoryを渡す。
- `-runTests`を指定して`-IsolateTestSaveData`を省略すると、Unityを起動する前にlauncherがfail-closedで停止する。
- `SaveManager`が環境overrideを読むのは`UNITY_EDITOR`だけ。隔離directoryがlauncher指定rootの子でない、root名が異なる、または実`persistentDataPath`と一致する場合は保存せず例外で停止する。
- EditMode / PlayMode helperも同じ境界を必須化し、cleanupは隔離directory内の指定slotだけを削除する。
- 製品buildの通常保存先とslot契約は変更していない。

## 実装確認

| 対象 | 確認内容 | 結果 |
|---|---|---|
| launcher guard | isolationなしの`-runTests`を起動前に拒否 | pass |
| caller環境復元 | Unity起動後に`FOUNDPHONE_TEST_SAVE_*`を呼び出し元の元値へ復元 | pass |
| compile | isolation付きbatch open / compile | exit 0 |
| EditMode save | slot 0 / 2 / 99のwrite / load / deleteが一時directoryだけを使用 | pass |
| PlayMode save | slot 0 / 99のwrite / load / cleanupが一時directoryだけを使用 | pass |
| 実ユーザーsave | `SaveData_99.json`の実行前後SHA-256 | `8FB0F337313517E93ABDBE0372ED4B2C5E5C11AF54FBC035B78F0988E5197537`で不変 |
| tracked副作用 | Unity由来のcode / scene / asset / package差分 | なし |

## テスト結果

| Assembly / platform | 件数 | pass | failed | skipped | 実行時間 |
|---|---:|---:|---:|---:|---:|
| `ProjectFoundPhone.Tests` / EditMode | 73 | 73 | 0 | 0 | 0.462秒 |
| `ProjectFoundPhone.PlayModeTests` / PlayMode | 10 | 10 | 0 | 0 | 19.322秒 |
| `ProjectFoundPhone.Editor.Tests` / EditMode | 14 | 14 | 0 | 0 | 1.817秒 |
| 合計 | 97 | 97 | 0 | 0 | 21.601秒 |

各実行は`-assemblyNames`でassemblyを分離し、XMLとUnity logをignored `Logs/`へ出力した。XMLは全件でinconclusive 0、process exit 0、compile error 0を示した。

launcherの環境復元追加後には`SaveSystemTests` 8件を再実行し、8/8 passと呼び出し元environmentの元値復元を確認した。

## Fail-closed調整

最初のcompileでは、EditMode helperをPlayMode assemblyから参照できず停止した。PlayMode側の既存共通helperへ同じ境界を置いて解消した。

最初のEditMode実行では、UnityがOS tempを8.3短縮名で返し、launcherの長いpathとの比較が不一致になった。実保存先へfallbackせずexit 2で停止した。launcherが隔離rootと子directoryを同時に渡し、Unity側がその親子関係と実save非一致を検証する方式へ変えた後、97件がgreenになった。

## 判定

G1.2「現行回帰基準化」は完了。今後のCLI testは`tools/run-unity.ps1 -IsolateTestSaveData`を必須入口とし、基礎83件とtargeted Editor件数をassembly別に記録する。

この結果はSites authoring bridge候補の人間受入、main統合、Owner-only hosted本文review、public/shared公開を承認するものではない。
