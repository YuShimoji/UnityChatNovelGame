# Workflow State SSOT

Last Updated: 2026-03-01 (ContentAuthoring validation pass)
Owner: Orchestrator

## Mission

Editor-Ready: デザイナーが Unity Editor 上で Yarn 会話を編集し、1クリックで再生確認できる制作ループを成立させる。

## Done 条件

- [ ] ContentAuthoring シーンを開いて Play → 指定の Yarn 会話が開始する
- [ ] Yarn の 1 行を変更→停止→再生で差分が確実に反映される
- [ ] 画面隅に Debug Overlay（node/line/tag）が表示される
- [ ] 失敗時は「壊れたファイル名」をログ 1 行で出す

## 選別規則

新規タスクは A/B/C に分類必須。D は Editor-Ready 達成まで採択しない。

- A: 制作ループ短縮（編集→再生→観測）
- B: コンテンツ制作の速度向上（追加が簡単）
- C: 失敗時の復旧容易性（原因が即特定）
- D: 品質/CI/テスト拡張/レポート整備 → 凍結

## Active Task Set

（Editor-Ready pivot により全 CI タスクを凍結）

- TASK_056: FROZEN (CI baseline — Editor-Ready 後に再開)
- TASK_057: FROZEN (QA EditMode coverage — Editor-Ready 後に再開)
- TASK_058: FROZEN (Remote Unity EditMode CI — Editor-Ready 後に再開)

## Current Implementation State

- `Assets/Scenes/ContentAuthoring.unity` を新規生成済み
- `VerticalSliceSceneSetup` に ContentAuthoring 生成メニューと batch entry を追加済み
- `ChatDialogueView` に runtime Debug Overlay（node/line/tag）自動生成を追加済み
- `ScenarioManager` に broken yarn file 1 行ログを追加済み
- `TitleScreenManager` は ContentAuthoring が build settings に存在すれば優先遷移する状態
- Unity batch で scene generation までは完了。Play 実行の自動観測は未確定

## Next Action

- Single Entry: ContentAuthoring シーンの Play 実行を自動観測できる最小 runtime validator を詰め、Done 条件 1/3/4 を確定させる (分類: A/C)

## 2026-03-01 Validation Pass

- ContentAuthoringBatchValidator.ValidateBatch passed in Unity batchmode.
- Observed runtime result: Yarn autoplay started, chat content populated, debug overlay showed 
ode/line/tag.
- ScenarioManagerEditor was added so authors can switch the start node from a Yarn node popup.
- ProjectSettings/EditorBuildSettings.asset now orders ContentAuthoring immediately after TitleScene.

## Next Action Override

- Start the first real content-production slice in ContentAuthoring using the node picker and overlay.
- If no narrative brief exists yet, the next blocker is user direction on what node/content to author.
