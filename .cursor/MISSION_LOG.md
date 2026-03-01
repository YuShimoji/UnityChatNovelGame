# Mission Log

## Mission
Editor-Ready: デザイナーが Unity Editor 上で Yarn 会話を編集し、1クリックで再生確認できる制作ループを成立させる。

## Done 条件
1. [ ] ContentAuthoring シーンを開いて Play → 指定の Yarn 会話が開始する
2. [ ] Yarn の 1 行を変更→停止→再生で差分が確実に反映される
3. [ ] 画面隅に Debug Overlay（node/line/tag）が表示される
4. [ ] 失敗時は「壊れたファイル名」をログ 1 行で出す

## Pivot 記録
- 2026-03-01: Editor-Ready pivot 実施
- 旧ログ: `.cursor/MISSION_LOG_ARCHIVE_PRE_EDITOR_READY.md`
- 凍結タスク: TASK_056, TASK_057, TASK_058 (CI 関連 — Editor-Ready 後に再開)
- 理由: 品質過多の重力井戸からの脱出。デザイナーのコンテンツ制作ループ確立を最優先とする。

---

## 進捗記録

### Editor-Ready Phase (2026-03-01 pivot)
- [x] Orchestrator Driver を Mission-first 形式に刷新
- [x] WORKFLOW_STATE_SSOT を Editor-Ready 仕様にリセット
- [x] MISSION_LOG をリセット（旧ログはアーカイブ済み）
- [x] ContentAuthoring シーン生成スクリプトを追加し、`Assets/Scenes/ContentAuthoring.unity` を生成
- [x] `ChatDialogueView` に Debug Overlay（node/line/tag）自動表示を追加
- [x] `ScenarioManager` に broken yarn file 1 行ログを追加
- [x] `TitleScreenManager` に ContentAuthoring 優先遷移を追加
- [ ] Next Action: ContentAuthoring の Play 実行を自動観測する最小 validator を詰める (分類: A/C)

### 2026-03-01 Validation Pass
- [x] ContentAuthoringBatchValidator.ValidateBatch passed in Unity batchmode and confirmed autoplay/chat/debug overlay at runtime
- [x] ScenarioManagerEditor added a Yarn node popup for m_StartNode
- [x] EditorBuildSettings.asset reordered to TitleScene -> ContentAuthoring -> DebugChatScene -> MVPScene
- [ ] Next Action: begin the first real content slice in ContentAuthoring once the target node/content brief is decided
