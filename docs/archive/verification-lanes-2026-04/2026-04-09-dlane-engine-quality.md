# Dレーン（Engine / Quality）— 2026-04-09 実施記録

[docs/ai/PARALLEL_LANE_PROMPTS.md](../ai/PARALLEL_LANE_PROMPTS.md) レーン D の計画に沿った実施内容と回帰結果の要約。

## 実施サマリー

| 項目 | 内容 |
|------|------|
| Editor コンパイル | `YarnContentValidator` のローカル関数が `out` パラメータを捕捉して **CS1628** となっていたため、集計用ローカル変数へ変更 |
| EditMode 回帰 | `TestRunnerHelper.RunEditModeTestsBatch` 新設後、batch で **75 / 75 passed** |
| ランタイム修正 | `SaveManager.Instance` が EditMode で `DontDestroyOnLoad` を呼び `ContradictionTests.Manager_SelectSecond_AlreadyDiscovered` が先頭実行時に落ちていたため、**`Application.isPlaying` 時のみ** `DontDestroyOnLoad` を実行（[INVARIANTS.md](../INVARIANTS.md) の EditMode / PlayMode 境界と整合） |
| PlayMode 回帰 | **本セッション未実施** — 別 Unity インスタンスが同一プロジェクトを開いており batch 起動が拒否された。直前のベースラインは [2026-04-09-playmode-8-results.md](../../verification/2026-04-09-playmode-8-results.md)（8/8）を参照 |

## batch コマンド例（EditMode）

Unity 6000.3.6f1 / プロジェクトパスは環境に合わせて置換。

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.3.6f1\Editor\Unity.exe' `
  -batchmode -nographics `
  -projectPath 'C:\Users\PLANNER007\UnityChatNovelGame' `
  -executeMethod ProjectFoundPhone.Editor.TestRunnerHelper.RunEditModeTestsBatch `
  '-ProjectFoundPhoneResultFile=C:\Users\PLANNER007\UnityChatNovelGame\docs\verification\editmode-batch-result.txt' `
  -logFile 'C:\Users\PLANNER007\UnityChatNovelGame\docs\verification\editmode-batch-editor.log'
```

- 既定の結果パス（`-ProjectFoundPhoneResultFile` 省略時）: `docs/verification/editmode-batch-result.txt`（併せて `.xml` を出力）
- PlayMode は既存どおり `RunPlayModeTestsBatch`（[2026-03-31-playmode-batch-execute.md](../../verification/2026-03-31-playmode-batch-execute.md)）

## 変更ファイル（コード）

- [Assets/Scripts/Editor/TestRunnerHelper.cs](../../Assets/Scripts/Editor/TestRunnerHelper.cs) — `RunEditModeTestsBatch`、メニュー `Tools/Run EditMode Tests Manual`、batch 共通 `EnqueueBatchTestRun`
- [Assets/Scripts/Editor/YarnContentValidator.cs](../../Assets/Scripts/Editor/YarnContentValidator.cs) — CS1628 回避
- [Assets/Scripts/Core/SaveManager.cs](../../Assets/Scripts/Core/SaveManager.cs) — `DontDestroyOnLoad` の PlayMode ガード

## ドキュメント

- [docs/FEATURE_STATUS_AUDIT.md](../FEATURE_STATUS_AUDIT.md) §1 — テストファイル数・EditMode/PlayMode ケース数を実測で更新

## オペレーター向け（PlayMode の再実行）

1. 他の Unity で本プロジェクトを閉じる
2. `RunPlayModeTestsBatch` を実行し、生成された `.txt` / `.xml` のパスを [2026-04-09-playmode-8-results.md](../../verification/2026-04-09-playmode-8-results.md) に追記するか、本ファイルに「再実行」節を追加
