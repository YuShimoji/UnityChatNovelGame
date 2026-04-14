# Handoff

会話ログなしで再開するための最短入口。

## まず読む

1. `docs/HANDOFF.md`
2. `docs/project-context.md`
3. `docs/runtime-state.md`
4. `docs/INVARIANTS.md`
5. `docs/USER_REQUEST_LEDGER.md`

## Handoff snapshot (2026-04-09)

- **完了**: `main` と `origin/main` の突き合わせ（本コミットまで追い越しなし）。ルートの計測用 NDJSON（`debug-*.log`）を `.gitignore` に追加し、リモートへノイズを載せない状態にした。
- **並行 F レーン（Audit / Evidence）**: **完了**。証跡索引は `docs/archive/verification-lanes-2026-04/2026-04-09-f-lane-audit-evidence-index.md`。spec 件数（41）の正典同期済み。
- **並行 C レーン（Unlock / Content Pipeline）**: **完了**。`ContentPipelineBatch`（`-executeMethod`）・`YarnContentValidator` バッチログ・メニュー統一・`YarnSOGenerator` 0 件時ダイアログ修正。手順の正本は `docs/archive/verification-lanes-2026-04/2026-04-09-content-pipeline-batch.md` と `docs/YarnEditingPipeline.md` Step 4b。**本開発は主軸へ復帰** — `docs/project-context.md` の CURRENT LANE（Content 主・副は好機にパイプライン）および下記 Current Focus に従う。
- **残作業**: 下記「Current Focus」および `docs/project-context.md` の CURRENT SLICE（変更なし）。
- **意図的に触れていないもの**: アプリコード・Yarn・シーン・Prefab（本ブロックは同期と運用整備のみ）。
- **次セッション最初に見るファイル**: `docs/HANDOFF.md` → `docs/project-context.md` → `docs/runtime-state.md`
- **未解決の設計判断（新規）**: なし。SP-022 の HUMAN_AUTHORITY 領域は従来どおりユーザー判断待ち。

## Current Focus

- 主目的: Ch1 コンテンツ前進 + 制作パイプライン実走
- 次: SP-022 でサブクエスト設計を確定し、Ch1 に 1〜3 本のパイロット追加
- その次: 通し手動検証 → ギャップ P0/P1/P2 付け → Ch2 執筆

## Recent Doc Delta

- **C レーン（2026-04-09・完了）**: Unlock ツール整備（上記スナップショット）。`docs/OPERATOR_WORKFLOW.md` S-4 に batch 参照を追加
- **F レーン（2026-04-09・完了）**: `docs/archive/verification-lanes-2026-04/2026-04-09-f-lane-audit-evidence-index.md`（検証・CI 証跡索引）。`spec-index.json` 実測 **41** 件で `runtime-state.md` / `FEATURE_STATUS_AUDIT.md` §1 / `CLAUDE.md` を同期。レーンクローズ後は本開発（Content 軸）優先
- docs 整理を継続実施し、`docs/wiki` の重複ページを段階的に統廃合
- `docs/wiki` はポータル最小構成（`README.md` / `_sidebar.md` / `save-system.md`）へ縮約
- `docs/EVIDENCE_REUSE.md` を新設し、archive 側の Evidence Reuse 文書を統合削除
- `docs/ENGINE_FEATURE_INVENTORY.md` / `docs/SCENARIO_AUTHORING_GUIDE.md` / `docs/PROJECT_OVERVIEW.md` / `docs/HANDOFF.md` を索引・要点中心の薄型へ更新
- `docs/verification/2026-03-30-playmode-batchmode-attempt.md` に旧ローカルパス注記を追加（環境差分）

## Safe Next Steps

0. （任意）リポジトリ静的整合: `docs/verification/2026-04-10-ch1-day1-3-preflight.md`（Editor 通しは同ファイル B 節）。Bレーン（SP-022）記録: `docs/archive/verification-lanes-2026-04/2026-04-09-blane-sp022-subsequent.md`
1. Ch1 Yarn を前進（現行: Day1〜3 を `Ch1_Day1.yarn` / `ch1` チャンネル 3 日）
1b. **A レーン（2026-04-09）**: `Ch1_Day3_End` に第2章への牽引力（Pyramid 独白）を追加済み。**任意（SUBSEQUENT 発動時のみ）**: Validator → Sync Authoring Assets → ContentAuthoring で Day3 終端通し再生。通常スライスでは省略
2. Content Pipeline で `Sync Authoring Assets`
3. ContentAuthoring で StartNode を確認して再生
4. PlayMode 8件の再実行は **SUBSEQUENT 発動時のみ** (通常スライスでは不要、ベースラインは保持済み)
5. UI 気づきは `docs/UI_ISSUES.md`、仕様ギャップは SP-022 に記録
6. docs 拡張時は `docs/ai/READ_ORDER.md` の正典導線を崩さず、重複ページを増やさない

補足:
- PlayMode 8件の回帰ベースライン記録: `docs/verification/2026-04-09-playmode-8-results.md`
- SUBSEQUENT 判定基準（P0/P1/P2）: `docs/StorySpec/22_subquest_exploration_content.md` §6.4
- SUBSEQUENT 手順・静的整合・分岐ルールの正本: `docs/verification/2026-04-10-subsequent-completion-report.md`

### 手動確認ハンズオン（SUBSEQUENT 発動時のみ）

**通常スライス / 微修正では参照禁止**。SUBSEQUENT スライスが明示的に発動したときだけ、以下を最短手順として使う。`project-context.md` の SUBSEQUENT に該当する手順。

1. Content Pipeline で **Sync Authoring Assets** 済みであること
2. ContentAuthoring を開き、**StartNode** が Ch1 入口（例: `Ch1_Day1_Opening`）であることを確認
3. Day1 Hub を進行し、**必須 4 トピック**を消化できること
4. 任意: Hub の **電波ログ／用語メモ**で C/A パイロットがサイドバーに現れ、`CompleteThread` 後に期待どおり閉じること
5. Day2 モックに入り、**待受ポート**の C パイロットを 1 回通す（任意）
6. 断片取得済みセーブで `ch1_cond_analysis`（B・LatentCond）が期待どおり出ること
7. Day3: Hub **必須トピック**（Mason 報告・掲示板・比較）を進行し、Latent から `scout_ch1_d3_route` / `scout_ch1_d3_board` / `annot_ch1_d3_compare` が Manifest されること
8. 問題は `docs/UI_ISSUES.md`、エンジン／仕様の不足は `docs/StorySpec/22_subquest_exploration_content.md` §6 と `docs/verification/templates/2026-04-08-ch1-subquest-gap-template.md` の P0/P1/P2 表に追記

### Ch2 着手時（LATER のチェックリスト要約）

`project-context.md` の LATER に対応。SUBSEQUENT で P0/P1/P2 が付いた状態から。

1. Ch2 本編 Yarn を前進（例: `Ch2_LocationConfusion.yarn`）。サブは SP-022 と同パターンで積む
2. **P1/P2 は実装しない**。**P0** が進行不能・セーブ／スレッド破綻のときだけ短い仕様＋実装スライスを挟む
3. **BL-002（ポートレート）**は Ch2 の視認性がボトルネックと判断した時点で着手可否を決める

## Source Of Truth

- 方針・スライス: `docs/project-context.md`
- 作業状態: `docs/runtime-state.md`
- 決定履歴: `docs/DECISION_LOG.md`
- 制作フロー: `docs/OPERATOR_WORKFLOW.md`
