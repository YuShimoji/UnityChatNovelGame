# Handoff

会話ログを読まなくても現状を引き継げるようにするための入口ドキュメント。

## まず読む順番

1. `docs/HANDOFF.md`
2. `docs/project-context.md`
3. `docs/runtime-state.md`
4. `docs/INVARIANTS.md`
5. `docs/USER_REQUEST_LEDGER.md`
6. 実作業に応じて:
  - 制作フロー: `docs/YarnEditingPipeline.md`, `docs/SCENARIO_AUTHORING_GUIDE.md`, `docs/OPERATOR_WORKFLOW.md`
  - UI/挙動: `docs/DISPLAY_ALGORITHMS.md`, `docs/UI_ISSUES.md`
  - 実装/監査: `docs/FEATURE_STATUS_AUDIT.md`, `docs/spec-index.json`

## Current Focus

- 主目的: **Ch1 をコンテンツとして前に進める**（Yarn 執筆 + 制作パイプラインの実走）
- 次フェーズ（CURRENT の直後）: **SP-022**（`docs/StorySpec/22_subquest_exploration_content.md`）でサブクエスト（サブスレッド探索）の設計チャーターを確定し、Ch1 に **パイロット 1〜3 本**を既存 Yarn コマンドのみで追加する
- その次（SP-022 達成後）: **SUBSEQUENT** — Ch1 メイン + パイロットサブの **通し手動検証**、SP-022 §6 ギャップの **P0/P1/P2**、SP-017 に Ch1 例 1 ページ。完了後 **Ch2 執筆**へ（`docs/project-context.md` の SUBSEQUENT RECOMMENDED SLICE）
- その次（SUBSEQUENT 完了・Ch2 着手後）: **LATER** — Ch2 をメイン＋サブで前進、**P0 ギャップのみ**短い仕様／実装スライス。詳細は `docs/project-context.md` の **LATER RECOMMENDED SLICE** と **推奨プランの読み方と手動意思決定**
- 補助目的: 人間が Yarn を執筆しやすい制作システムの維持（ツール・パイプライン・検証導線）
- AI の役割: 執筆ではなく、ツール・パイプライン・検証導線の整備
- 現在のボトルネック: **Ch1 の Day／節をプレイアブルに積み上げること**。既知の Ch1 UI 挙動は **docs/UI_ISSUES.md** に蓄積済み（個別修正はバッチまで保留）

## Current State

- `YarnSOGenerator` は Topic / Character / Channel の同期まで対応済み
- `Content Pipeline` ウィンドウ追加済み
- `ScenarioManager` は StartNode から CurrentChannel を自動解決する。HasNode 事前チェック追加済み
- `Use Default Node` 固定は廃止し、推奨ノード選択へ変更済み
- PlayMode テスト: **8ケース** (SmokeGate 4件 + ScenarioFlow 4件)。共通ヘルパー `PlayModeTestHelpers.cs` に分離済み
- teardown 対策: `[UnityTearDown]` + `StopScenario()` + 待機（session 22 で SafeTeardown 強化済み）
- batch 実行時 NUnit XML（.xml）+ テキスト（.txt）出力対応済み
- 2026-04-07: Ch1 を開いて確認。Pyramid 周り・インジケータースキップ・スレッド遷移時の色の 3 件を **UI_ISSUES.md** に記録（コード変更なし）
- 2026-04-08: **SP-022** 新規（サブクエスト探索コンテンツ設計）。**spec-index.json** 登録、**project-context.md** に NEXT RECOMMENDED SLICE 追記（エンジン改修は初期スライスではしない）
- 2026-04-09: **SUBSEQUENT** スライスと **手動確認ハンズオン**を HANDOFF / project-context / runtime-state に反映（SP-022 完了後の手順）
- 2026-04-10: **LATER** スライス、**推奨プランの読み方・意思決定解説**（project-context）、**手動意思決定チェックリスト**（本ファイル）
- まだ未実施（好機に）: PlayMode 8 件の実ラン記録更新（verification/）、E2E の継続拡張（EN-012）

## Recent Session Delta

### 2026-04-10 (LATER + 意思決定解説)

- **project-context.md**: `LATER RECOMMENDED SLICE`、`推奨プランの読み方と手動意思決定（解説）`、中期ロードマップ補足、DECISION LOG、HANDOFF SNAPSHOT
- **本ファイル**: Current Focus に LATER、**手動意思決定チェックリスト**、Safe Next Steps 11
- **runtime-state.md**: `later_recommended_slice`、Session Log

### 2026-04-09 (SUBSEQUENT スライス + 手動ハンズオン)

- **project-context.md**: `SUBSEQUENT RECOMMENDED SLICE`（SP-022 達成後）新設。中期ロードマップに「SUBSEQUENT 完了 → S21–22 Ch2 執筆」接続行を追加。DECISION LOG（2026-04-09）追記
- **本ファイル**: `## 手動確認ハンズオン（Ch1 + サブスレッド）` 追加。Safe Next Steps に SUBSEQUENT 用ステップ追加

### 2026-04-08 (SP-022 / 次期サブクエスト軸)

- **SP-022**: `docs/StorySpec/22_subquest_exploration_content.md` 作成（DRAFT）。StorySpec README・spec-index 更新
- **project-context.md**: NEXT RECOMMENDED SLICE、ロードマップ補足、DECISION LOG、HANDOFF SNAPSHOT 更新
- 方針: ボリュームはサブスレッド探索の積み上げ。B 型 Wiki 等は **ギャップ列挙**に留め、仕様確定前にエンジン実装しない

### 2026-04-07 (plan sync)

- **主軸確定**: Content レーン — Ch1 コンテンツ前進 + 制作パイプライン実運用
- Ch1 プレイ確認のみ。発見した UI 件は **UI_ISSUES.md** へ追記。局所コード修正は行わない方針を **project-context.md** の DECISION LOG に明文化
- `docs/project-context.md` / `docs/runtime-state.md` / 本ファイルを同期

### session 21 (2026-04-02)

- PlayMode テスト失敗の根本原因特定: teardown の DialogueException ではなく、auto-start の missing_node:Start が原因
- 修正: HasNode 事前チェック / ResolveLikelyBrokenYarnFile archive 除外 / TearDown StopScenario
- teardown 強化: `[TearDown]` → `[UnityTearDown]` (IEnumerator) + `StopScenario()` + 1フレーム待機
- 共通ヘルパー `PlayModeTestHelpers.cs` 分離 (シーンロード、条件待ち、エビデンス、teardown)
- batch XML 出力対応: `ITestResultAdaptor.ToXml()` → `.txt` + `.xml` 両ファイル生成
- PlayMode テストケース追加 (4件 → 8件): ScenarioFlowPlayModeTests.cs 新規 (ETK_Commands, ETK_RichText, Ch2_Opening, SaveLoad 3連サイクル)
- WORKFLOW_STATE_SSOT.md 廃止 (HANDOFF.md に一本化)
- Assets/_Recovery/ (クラッシュリカバリ残骸) 削除
- spec-index 更新: EN-012 pct 40% → 60%
- **補足**: `DebugChatScene` の `m_StartNode` は用途により異なる（本編・VerticalSlice・DQT など）。**再生目的に合わせて** Inspector で確認し、**欠落ノード（旧 `Start` 等）を指さない**こと。エンジン小出し検証では `DQT_Start` 等が一般的

### session 20

- PlayMode batch 起動経路追加 (`-executeMethod`)
- SaveManager `GetCurrentNodeName()` の "Start" 固定フォールバック廃止

### session 19

- Yarn active/ クリーンアップ (4件 archive 移動)、CanvasScaler 9:16 統一
- DQT_Start PlayMode テスト追加、EN-012 登録

## Safe Next Steps

1. **Ch1**: `Assets/...` の Ch1 Yarn を編集し、次の Day／節を前に進める
2. `Tools > FoundPhone > Content Pipeline` → **Sync Authoring Assets** → エラーがないか確認
3. **本編確認**: ContentAuthoring（または本編用シーン）を開き、**ScenarioManager の StartNode** がそのシナリオの入口ノードになっているか確認してから Ch1 を通し再生
4. **デバッグ小出し**: エンジン挙動だけ試すときは `DebugChatScene` を開き、**目的に応じて** `m_StartNode` を `DQT_Start` や `VerticalSlice_Start` 等に設定（欠落ノードを指さないこと）。変更したらシーン保存
5. **好機に**: Unity Test Runner で PlayMode テスト **8 件**を実行し、結果を `docs/verification/` 等へ残す（EN-012）
6. 新規の UI 気づきは **docs/UI_ISSUES.md** へ追記。値の微調整は Inspector。**進行不能**だけは構造バグとしてコードを検討
7. **（CURRENT 完了後）** `docs/StorySpec/22_subquest_exploration_content.md`（SP-022）をレビューし、§3・§4 の仮数・優先を Ch1 に合わせて更新する
8. **（同上）** `03a_ch1_section_beats.md` または Ch1 Yarn に **節 ↔ サブクエスト ID** の対応を書き、**C/A 型中心**でパイロット **1〜3 本**を追加 → Content Pipeline → 再生確認。不足は SP-022 §6 にギャップとして追記
9. **（SP-022 達成後・SUBSEQUENT）** 下記 **手動確認ハンズオン**で Ch1 メイン + サブを通し、問題は UI_ISSUES、ギャップは SP-022 §6 に **P0/P1/P2** で追記
10. **（同上）** `docs/StorySpec/17_unlock_triggers.md` に Ch1 用具体例を 1 ページ追記。好機に PlayMode 8 件の結果を `docs/verification/` に 1 ファイル保存。その後 **Ch2 執筆**へ移行
11. **（SUBSEQUENT 完了後）** `docs/project-context.md` の **LATER RECOMMENDED SLICE** と **推奨プランの読み方と手動意思決定**を読み、下記 **手動意思決定チェックリスト**を踏んでから Ch2 本編＋サブを進める（手順の詳細は project-context に集約）

## 手動意思決定チェックリスト（短）

SUBSEQUENT 完了後〜LATER に入る前後で、**人間が一度は口頭または文書で確定する**とよい項目。

1. **ギャップを実装に回す前に**、SP-022 §6（または別表）の該当行に **プレイヤーに見える挙動を 1 段落**書いたか（未記述のままコードに入らない）。
2. 付けた **P0** は本当に **進行不能またはデータ破綻**か。単なる見た目の不満は **UI_ISSUES** または **P1/P2** に落とせないか。
3. **Ch2** でサブクエストの本数・必須／任意の比率を **Ch1 と同じにするか、減らすか**、方針を決めたか（SP-022 §4 の仮数を Ch2 用に更新するか）。
4. **P1/P2** を「ついでに」と実装に混ぜないと決めたか（LATER のデフォルトは **P0 のみ**例外スライス）。
5. **BL-002（ポートレート）**を今やるかは、**Ch2 の視認性がボトルネックか**で判断する（詰まっていなければ S23 は後ろにずらしてよい）。
6. **B 型 Wiki・解放通知**をエンジンで触る前に、SP-017 / SP-022 で **HUMAN_AUTHORITY** の承認があるか。
7. **AI・エージェント**に任せるのはツール・パイプライン・検証系に留め、**シナリオのトーンと必須サブの是非**は自分（ライター）で決めたか。

## 手動確認ハンズオン（Ch1 + サブスレッド）

想定読者: デザイナー兼ライター。Unity Editor での **目視確認**用（自動テストの代替ではない）。

1. **準備**: プロジェクトを開き、`Assets/Scenes/ContentAuthoring.unity`（本編）を開く。Hierarchy で **ScenarioManager** が付いた GameObject を選択し、Inspector の **Start Node** が Ch1 Day1 の入口 **`Ch1_Day1_Opening`** になっているか確認する（`ContentAuthoring.unity` の既定はこれ。変更している場合はそのノードが `Assets/Resources/Yarn/active/Ch1_Day1.yarn` に存在すること）。
2. **Yarn 同期**: `Tools > FoundPhone > Content Pipeline` → **Sync Authoring Assets** を実行し、エラーがないことを確認する。
3. **再生開始**: 画面上部の **Play** を押して再生モードに入る。ダッシュボードから第 1 章へ入る通常導線、またはシーンの自動開始に従う。
4. **メイン進行**: **メインスレッド**のまま Day1 の会話を進める。選択肢・`EndDay` などはシナリオどおり操作する。
5. **サブスレッド**: 画面左の **スレッド一覧（サイドバー）**を開く。SP-022 パイロットで追加した **C 型（`scout_`）や A 型（`annot_`）**が表示されるタイミングまでメインを進める。該当スレッドをタップして切り替え、会話を **終端**（例: `<<CompleteThread>>` 相当）まで読む。
6. **メインへ復帰**: メインスレッドに戻し、フラグ・進行が期待どおりか目視する（メインが先に進めなくなっていないか等）。
7. **セーブ確認（任意）**: サブ完了の前後で **Save** → **Load** を試し、アクティブスレッドや履歴が破綻していないか見る。進行不能なら **メモのみ**（このスライスでは UI_ISSUES へ追記し、個別コード修正はバッチまで保留）。
8. **記録**: 見た目・操作の問題は `docs/UI_ISSUES.md`。エンジンや仕様で足りないことは `docs/StorySpec/22_subquest_exploration_content.md` §6 に **P0（ブロッカー）/ P1（次スプリント）/ P2（あとで）** を付けて追記する。

## Do Not Do Next

- **UI_ISSUES.md に載った項目を、このスライスで個別にコード修正しない**（バッチ時まで保留）
- **B 型 Wiki リンク遷移など、SP-022 §6 のギャップを、仕様未確定のままエンジン実装で埋めない**（別スライスで優先度付け）
- UI 値調整をコード修正として進めない
- 本編 Yarn を実験台にしたエンジン検証をしない
- 「前回の反動」で別レーンへ振れない
- 会話ログだけに依存した handoff を残さない

## Current Trust Assessment

- trusted
  - DebugQuickTest 導線
  - YarnSOGenerator の Topic / Character / Channel 同期
  - StartNode 推奨導線
  - CanvasScaler 9:16 統一 (コード上。DebugChatScene.unity は未再生成)
  - HasNode 事前チェック (auto-start 安全化)
- needs re-check
  - ChannelData 自動同期の Unity 実機結果
  - CurrentChannel 自動解決が Save/Load / EndDay と競合しないこと
  - Content Pipeline window の実運用手順
  - PlayMode テスト 8 件の **再実行・verification 記録**（session 22 でローカル通過済み。環境差分の再確認は好機に）
- dangerous / rollback candidate
  - なし

## Open Risks

- Unity 実機未確認のため、Editor 拡張の挙動はコードレビュー止まり
- `ChatController` のステータスバールーティング TODO は未解決
- `verification/` と E2E PlayMode は未整備 (テストコードはあるが実行結果なし)
- DebugChatScene.unity の CanvasScaler は 1920x1080 のまま (シーン再生成で修正要)

## Source Of Truth

- 方針・優先レーン: `docs/project-context.md`
- 直近の作業状態: `docs/runtime-state.md`
- 非交渉条件: `docs/INVARIANTS.md`
- ユーザー要求の継続事項: `docs/USER_REQUEST_LEDGER.md`
- 実際の制作フロー: `docs/OPERATOR_WORKFLOW.md`

## Canonical Gaps

- `docs/AUTOMATION_BOUNDARY.md`: 不在
- 現時点で今回の制約・痛点は既存 canonical docs に書き戻し済み