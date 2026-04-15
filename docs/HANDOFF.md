# Handoff

会話ログなしで再開するための最短入口。

## まず読む

1. `docs/HANDOFF.md`
2. `docs/project-context.md`
3. `docs/runtime-state.md`
4. `docs/INVARIANTS.md`
5. `docs/USER_REQUEST_LEDGER.md`

## Handoff snapshot (2026-04-15)

**本セッションで 5 コミット + handoff 整備コミットをリモートへ反映 (docs のみ、コード/Yarn/シーン変更なし)。**

構造的クリーンアップ 5 コミット (新しい順):
- `7f17b83` SUBSEQUENT 条件付き運用への最終整合 + spec-index 件数 40 統一
- `8038b1d` Ch1 通し最優先化の構造的誘因と永続未実施マーカー除去 (「好機に」等曖昧語撤去、SUBSEQUENT 発動条件明文化、上から順ルール廃止)
- `1a03bd9` 周辺クリーンアップ A+B (未参照バイナリ削除、VerticalSlice スクショ archive、runtime-state カウンター簡素化、完了レーン証跡 archive 移動)
- `2f0d19a` テスト/手動確認過多の構造的誘因を除去 (CORE_RULESET に Verification cost principle、INTERACTION_NOTES に手動確認発動条件、phase-a/ch1-playthrough checklist archive 化)
- `c7f06e2` テスト過剰整理 (過剰防御テスト 8 件削除、MVP_TEST_GUIDE 削除、件数カウンター廃止)

**2026-04-15 追加クリーンアップ (実施済み)**:
- Dev/ 空アセンブリ定義を削除 + Editor.asmdef の参照除去
- ChatController TODO の docs 重複を W-6 クロスリファレンスに統合 (6 箇所 → DECISION_LOG + W-6 の 2 正本 + 3 箇所クロスリファレンス)
- runtime-state.md の todo_fixme_hack カウンター数値を廃止方針に整合
- SaveSystem_README / 03b_ch3 / 16_subthread_ui のチェックボックス 21 項目をリファレンス表記に変換
- TestRunnerHelper.cs のレガシー MenuItem にコメント追加

**次回任意のクリーンアップ候補 (忘却防止)**:
- 2026-04-08-spec-gap-radar.md「検証待ち」表現 (手続き的記述のため低優先)
- project-context.md:45「好機のみ」は 8038b1d で除去済み (確認完了)

前セッション分 (維持):
- **並行 F レーン（Audit / Evidence）**: **完了**。証跡索引は `docs/archive/verification-lanes-2026-04/2026-04-09-f-lane-audit-evidence-index.md`。spec 件数（41）の正典同期済み。
- **並行 C レーン（Unlock / Content Pipeline）**: **完了**。`ContentPipelineBatch`（`-executeMethod`）・`YarnContentValidator` バッチログ・メニュー統一・`YarnSOGenerator` 0 件時ダイアログ修正。手順の正本は `docs/archive/verification-lanes-2026-04/2026-04-09-content-pipeline-batch.md` と `docs/YarnEditingPipeline.md` Step 4b。**本開発は主軸へ復帰** — `docs/project-context.md` の CURRENT LANE（Content 主・副は好機にパイプライン）および下記 Current Focus に従う。
- **残作業**: 下記「Current Focus」および `docs/project-context.md` の CURRENT SLICE（2026-04-15 にエンジン能力マイルストーン M1 に切替）。
- **意図的に触れていないもの**: アプリコード・Yarn・シーン・Prefab（本ブロックは同期と運用整備のみ）。
- **次セッション最初に見るファイル**: `docs/HANDOFF.md` → `docs/project-context.md` → `docs/runtime-state.md`
- **未解決の設計判断（新規）**: なし。SP-022 の HUMAN_AUTHORITY 領域は従来どおりユーザー判断待ち。

## Current Focus

- 主目的: エンジン能力マイルストーン 1 (サブスレッド全型の実機検証)
- 次: エンジン能力マイルストーン 2 (セーブ/ロード完全性 + 章遷移堅牢化)
- **SUBSEQUENT（通過ゲート・スキップ不可）**: M1 と M2 の完了が発動条件。エンジン能力レビュー + Ch1 フルコンテンツ執筆の解放判定
- **LATER**: SUBSEQUENT 通過後に Ch1 フルコンテンツ前進 + P1 段階実装

## Recent Doc Delta

- **C レーン（2026-04-09・完了）**: Unlock ツール整備（上記スナップショット）。`docs/OPERATOR_WORKFLOW.md` S-4 に batch 参照を追加
- **F レーン（2026-04-09・完了）**: `docs/archive/verification-lanes-2026-04/2026-04-09-f-lane-audit-evidence-index.md`（検証・CI 証跡索引）。`spec-index.json` 実測 **41** 件で `runtime-state.md` / `FEATURE_STATUS_AUDIT.md` §1 / `CLAUDE.md` を同期。レーンクローズ後は本開発（Content 軸）優先
- docs 整理を継続実施し、`docs/wiki` の重複ページを段階的に統廃合
- `docs/wiki` はポータル最小構成（`README.md` / `_sidebar.md` / `save-system.md`）へ縮約
- `docs/EVIDENCE_REUSE.md` を新設し、archive 側の Evidence Reuse 文書を統合削除
- `docs/ENGINE_FEATURE_INVENTORY.md` / `docs/SCENARIO_AUTHORING_GUIDE.md` / `docs/PROJECT_OVERVIEW.md` / `docs/HANDOFF.md` を索引・要点中心の薄型へ更新
- `docs/verification/2026-03-30-playmode-batchmode-attempt.md` に旧ローカルパス注記を追加（環境差分）

## Safe Next Steps

1. DebugQuickTest で各サブスレッド型 (A/B/C/Latent/Branch) を個別に再生確認
2. 不具合があればエンジンコード修正 → 修正確認再生
3. 修正に対応する PlayMode テストを追加
4. 全型の確認完了後、M1 達成を runtime-state に記録し M2 に移行
5. UI 気づきは `docs/UI_ISSUES.md`、仕様ギャップは SP-022 に記録
6. docs 拡張時は `docs/ai/READ_ORDER.md` の正典導線を崩さず、重複ページを増やさない

補足:
- PlayMode 8件の回帰ベースライン記録: `docs/verification/2026-04-09-playmode-8-results.md`
- SUBSEQUENT 判定基準（P0/P1/P2）: `docs/StorySpec/22_subquest_exploration_content.md` §6.4
- SUBSEQUENT 手順・静的整合・分岐ルールの正本: `docs/verification/2026-04-10-subsequent-completion-report.md`

### SUBSEQUENT 発動時の手順（M1+M2 完了後）

SUBSEQUENT ゲート通過のための手順。M1（サブスレッド全型実機検証）と M2（セーブ/ロード + 章遷移）が完了していること。

1. FEATURE_STATUS_AUDIT の未実装/未確認を P0/P1/P2 で再評価
2. PlayMode テスト再実行、結果を日付付きファイルで記録
3. P0 が 0 件であることを確認 → Ch1 フルコンテンツ執筆を LATER で解放
4. P0 が残存する場合 → P0 修正スライスを挟む
5. 判定結果を project-context.md に記録

### LATER（SUBSEQUENT 通過後）

1. Ch1 の Day 単位でのコンテンツ前進（SP-022 サブクエスト含む）
2. P1 項目の仕様確定と実装を並行
3. Ch2 への移行は Ch1 で検証した全エンジン能力の PlayMode テストカバレッジ確認後

## Source Of Truth

- 方針・スライス: `docs/project-context.md`
- 作業状態: `docs/runtime-state.md`
- 決定履歴: `docs/DECISION_LOG.md`
- 制作フロー: `docs/OPERATOR_WORKFLOW.md`
