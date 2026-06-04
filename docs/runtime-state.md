# Runtime State

**Updated**: 2026-06-03（ローカル Unity 6000.4.9f1 状態 + SP-023 / SP-024 表示検収前の引き継ぎを origin/main へ反映）

## Current Position

- project: FoundPhone (UnityChatNovelGame)
- branch: main
- lane: **UI/Engine**（SP-023 / SP-024 の表示検収再開）
- slice: **ローカル追跡差分と再開文脈を main に固定し、別端末で pull 後すぐ検収へ戻れる状態**
- next_recommended_slice: **Unity 6000.4.9f1 で `SP023_NarrationMargin_Start` → `SP023_LocalExtensions_Start` → `SP023_DisplayShowcase_Start` の Unity 画面検収**
- subsequent_recommended_slice: **`SP024_Immersion_Start` による SP-024 S1/S2/S5 の Unity 見た目確認、または S4 (オンライン状態 UI) / Block 4 (フリックスレッド切替) の設計固定**
- later_recommended_slice: **SP-024 S4 (オンライン状態表示)**
- active_artifact: ChatController / ChatDialogueView / ScenarioManager / CharacterProfile / SaveData / SubthreadData / ThreadSwitcherController / BubbleStyle assets / SP023 demo yarns / SP024_ImmersionDemo
- artifact_surface: ChatController / ChatDialogueView / ChatUIConfig / ScenarioManager / CharacterDatabase / ThreadSwitcherController
- last_change_relation: sync（2026-06-03: local tracked Unity/project settings + handoff docs を remote 反映）
- plan_file: `C:\Users\thank\.claude\plans\hazy-tumbling-feigenbaum.md` （9 Block 分割の実行プラン） / `docs/plans/display-batch-showcase.md`（表示系デモ・修正版・リポジトリ正本）

## Counters

- last_user_visible_change: session 22 (2026-04-03 タイプライター同期修正)
- (blocks_since_* / consecutive_* / visual audit カウンターは廃止: 確認コスト原則 / CORE_RULESET に従い、"未実施" 指標が再実行圧力となるのを回避)

## Quantitative Metrics (0 を目指す指標のみ、件数追跡は廃止)

- tests_last_run: 2026-04-09 (EditMode pass / PlayMode pass)
- mock_files: 0
- spec_entries: 42 (`docs/spec-index.json` 配列長、検証用。SP-023/SP-024 追加)
- todo_fixme_hack: ChatController.cs:2020 に 1 件残存 (FEATURE_STATUS_AUDIT W-6 参照)
- obsolete_marks: ContradictionPair.UnlockTopic x2

## Visual Evidence

- last_visual_audit_path: docs/archive/verification-evidence/VerticalSliceSmokeGate_20260403_*.png (参考。パスのみ保持、追跡は廃止)

## Session Log

### 2026-06-03（ローカル状態の remote 反映・別端末 handoff）

- **目的**: 会話ログなしで別端末から再開できるよう、現在のローカル追跡差分・検証結果・残リスクを project-local docs に固定し、`origin/main` へ反映する。
- **Unity / packages / CI**: `ProjectVersion.txt` を 6000.4.9f1 に更新。`com.unity.nuget.newtonsoft-json` は 3.2.2。GitHub Actions の EditMode / PlayMode `unityVersion` も 6000.4.9f1 に同期。別端末は Unity 6000.4.9f1 で開く前提。
- **Assets / settings**: `CharacterProfile_NPC_Pyramid.asset` は `m_IconSide: 2`。`NotoSansJP-Regular SDF.asset` は dynamic font cache が空の状態で保存されているため、日本語表示は次回の画面検収で重点確認。`EditorBuildSettings.asset` は Unity 再保存により `m_UseUCBPForAssetBundles` 行が削除されたのみ。
- **Local-only**: `.codex/hooks.json` は絶対パスと未存在 script 参照を含むため、リモート対象外として `.gitignore` に追加。Codex/AI 向け正本は `AGENTS.md` と `docs/ai/*.md`。
- **検証**: `git diff --check` pass、Packages JSON parse pass、Unity 6000.4.9f1 batchmode open exit code 0。ログ上は Licensing handshake の一時 error、DOTween Editor asmdef no scripts warning、`UnityEngine.UI.Tests.dll` skip、MenuItem 重複 warning、MCPForUnity info が残るが、batchmode は正常終了。
- **未検証**: SP-023 3 本の画面検収、SP-024 S1/S2/S5 の画面検収、SDF cache reset 後の日本語表示、Pyramid `IconSide=2` の実表示。

### 2026-04-21（同期再開・ローカル差分再適用）

- **Git**: `origin/main` へ fast-forward 済み。stash 退避後に再適用し、競合を手動統合。
- **統合方針**: `BubbleStylePreset` / `BubbleStyleDatabase` はリモートの static レジストリ実装を採用し、ローカル差分は `IconSide` / `SetThreadMeta` / `SubthreadData` メタ / サイドバーメタ表示に絞って再適用。
- **追加**: `Assets/Resources/BubbleStyles/` に `thought` / `shout` / `whisper` / `announcement`、`Assets/Resources/Yarn/active/` に `SP023_LocalExtensionsDemo.yarn` / `SP023_DisplayShowcaseDemo.yarn` を追加。
- **補修**: `ChatController` の重複アイコン生成を解消し、`ThreadSwitcherController` がメタ追加後に `MetaLabel` を動的生成できるよう修正。
- **追加実装**: `CharacterProfile.defaultBubbleStylePreset`、SP-024 S3 の `TypingSpeed` / `<<SetTypingSpeed>>` / `ScenarioManager` セッション override、`SavedChatMessage` の SP-024 データ契約 (`Timestamp` / `DeliveryStatus` / `IsDeleted`) を先行反映。
- **追加**: `Assets/Resources/Yarn/active/` に `SP024_ImmersionDemo.yarn` (`SP024_Immersion_Start`) を追加し、SP-024 S1/S2/S5 の局所検証導線を分離。
- **未検証**: Unity Editor での Block 2 / LocalExtensions / DisplayShowcase の画面確認、`SP024_Immersion_Start` による S1/S2/S5 の画面確認、および S3 の待機時間差確認。

### 2026-04-20（表示系デモ計画の再監査・ドキュメント同期）

- **目的**: 「表示系一括 + SP-024 統合」旧プランと実装の齟齬を解消し、再開用にリポジトリへ正本を置く。
- **追加**: `docs/plans/display-batch-showcase.md`（SP-024 は統合デモから外す、`ChatUIConfig`/`UIFontConfig` の checked-in 実値に基づく監査、統合デモは SP-023 のみ・15–18 メッセージ目安）。
- **更新**: `docs/HANDOFF.md`（2026-04-20 スナップショット）、`docs/ai/READ_ORDER.md`（タスク別 1 行）。
- **コード変更なし**（計画・引き継ぎのみ）。

### 2026-04-16 session 3（SP-023 Block 1 検収 + Block 2 実装）

- **Block 1 (SP-023 S1 BubbleStylePreset 基盤) 完了・検収済み** コミット `ee184cf`:
  - `BubbleStylePreset.cs` SO (13 フィールド + 上書きフラグ方式)
  - `BubbleStyleDatabase.cs` 静的レジストリ (`Resources/BubbleStyles` 自動収集)
  - `<<BubbleStyle "presetId">>` Yarn コマンド登録
  - `ChatController.SetNextBubbleStyle()` + `ApplyBubbleStylePreset()` (次 1 メッセージに適用・自動リセット)
  - `default.asset` (pass-through プリセット)
  - `SP023_BubbleStyleDemo.yarn`: 3 メッセージ検証
  - **検収結果**: DebugChatScene + StartNode=SP023_BubbleStyle_Start で 3 メッセージ全表示・見た目同一・missing 警告ログ確認済み
- **Block 2 (SP-023 S2 Narration + S3 BubbleMargin) 実装完了・画面未検証** コミット `5da8f9a`:
  - `<<Narration "text">>` コマンド (narration preset + AddSystemMessage 糖衣)
  - `<<BubbleMargin l r t b>>` コマンド (次 1 メッセージのラッパー padding を % 指定で上書き)
  - `ChatController`: `m_PendingBubbleMarginPercent` + `SetNextBubbleMargin()` + `ConfigureBubble` で margin 上書き + `AddSystemMessage` で preset 消費
  - `narration.asset`: alpha=0 + italic + center + グレーテキスト + suppressWrapper
  - `SP023_NarrationMarginDemo.yarn`: 6 メッセージ (Narration×2, normal, margin×2, reset)
- **次セッション再開導線**:
  1. Block 2 画面検証: DebugChatScene → StartNode=`SP023_NarrationMargin_Start` → Play
  2. 6 メッセージの見た目確認 (narration 透明背景/細長いバブル/上下余白/自動リセット)
  3. OK なら Block 3 (SP-023 S4 IconSide) 着手
- **判断ポイント (保留)**:
  - フリック閾値 15%: Block 4 実装後に実機体感で判断
- **副次観察**: SP023_BubbleStyleDemo で見えていたタイピングインジケーター差は、SP-024 S3 最小実装でキャラ別待機秒数へ切り替え済み。最終判断は Unity 実機確認待ち。

### 2026-04-15 session 2（Editor 整備 + テキスト表現仕様）

- **Editor メニュー統一**: 全 29 MenuItem を `Tools/FoundPhone/` 配下に統一 (12 ファイル変更)。サブメニュー: Scene Setup / Setup / Verification / Tests / Debug。Yarn Content Validator 重複削除
- **ChatUIConfig タイミング集約**: typewriterSpeed / typingIndicatorDuration / postMessageDelay / enableTypewriterEffect / enableTapSkip を SO に移行 (3 ファイル)。画像フェードイン 0.6f ハードコード修正
- **SP-023 テキスト表現仕様** (新設 `23_text_presentation.md`): BubbleMargin (% 指定)、BubbleStylePreset (7 プリセット)、narration (地の文)、IconSide (アイコン向き)、フリックスレッド切替、サブクエスト統合 (SP-016/017/022/BL-003)、B/C/D 保留領域 (S10-12)
- **SP-024 チャット没入仕様** (`24_chat_immersion.md`): タイムスタンプ、既読/配信マーク (DeliveryStatus)、キャラ別タイピングパターン (TypingSpeed 7 段階)、オンライン状態 (OnlineStatus)、メッセージ削除痕。2026-04-21 時点で S1/S2/S3/S5 は UI 接続と Save/Load 復元まで反映済みで、残りは S4 のオンライン状態 UI。
- **SP-023 Worker 実装**: BubbleStylePreset SO / IconSide / フリック切替の .cs が未コミットで存在。検収後にコミット予定
- **spec-index**: SP-023 + SP-024 追加 (計 42 エントリ)

### 2026-04-09（A レーン — Ch1 終端強化）

- **Yarn**: `Assets/Resources/Yarn/active/Ch1_Day1.yarn` の `Ch1_Day3_End` に、Pyramid 独白で「端末外観測が次の材料」「偵察拡大で照合解像度」の短文を追加（`<<EndDay 3>>` 前・新トピックなし）。
- **仕様**: [`docs/StorySpec/03a_ch1_section_beats.md`](StorySpec/03a_ch1_section_beats.md) Day3 Winding 節を実装と整合。
- **検証（ユーザー・当該コミットの一次確認）**: A レーン変更の確認として Validator → Sync → ContentAuthoring で Day3 終端まで再生。**セッションごとの常時義務ではない**（通常は静的整合・局所再生で足りる場合が多い。長尺通しは SUBSEQUENT 発動時または `HANDOFF.md` Safe Next Steps **1b** の任意条件に従う）。

### 2026-04-09（F レーン完了 → 本開発復帰）

- **並行 F レーン**: クローズ。以降の実行計画は `docs/project-context.md` の **CURRENT LANE / CURRENT SLICE**（Content 主、Unlock 副）を正とする
- **参照固定**: 検証・CI の索引は [2026-04-09-f-lane-audit-evidence-index.md](archive/verification-lanes-2026-04/2026-04-09-f-lane-audit-evidence-index.md)

### 2026-04-09（F レーン — Audit / Evidence）

- **verification**: [docs/archive/verification-lanes-2026-04/2026-04-09-f-lane-audit-evidence-index.md](archive/verification-lanes-2026-04/2026-04-09-f-lane-audit-evidence-index.md) を新設（PlayMode / CI / Ch1・SUBSEQUENT 証跡の索引、再開読書順）
- **整合**: `spec-index.json` は **41** エントリ（Python 実測）。`spec_entries: 42` を **41** に修正
- **コード・Yarn・シーン**: 変更なし（読み取り監査のみ）

### 2026-04-09（UI: ダッシュボード／インベントリレイアウト + ミニマル配色）

- **レイアウト**: `InventoryTabController` の親パネル上端を **200px** インセットに変更（`DashboardController` チャンネル ScrollView の `-200` と整合）。TabBar（-150〜-190）と InventoryRoot の縦重なりを解消。
- **見た目**: `DashboardController` / `InventoryTabController` / `ThreadSwitcherController` / `ProgressSummaryUI` の背景・カード色を低コントラスト寄りに統一（実行時負荷の増加なし）。
- **検証**: Unity 目視確認は SUBSEQUENT 発動時 (`docs/UI_ISSUES.md` に記録)。

### 2026-04-09（セッション引き継ぎ・リモート同期）

- **Git**: `main` と `origin/main` を fetch で突き合わせ。追跡ファイルの未プッシュ差分はなし。ルートの計測 NDJSON を `debug-*.log` として `.gitignore`
- **ドキュメント**: `HANDOFF.md`（Handoff snapshot）、`project-context.md`（直近の状態 1 行）、本ファイルの Updated / Session Log
- **コード・Yarn**: 変更なし

### 2026-04-09（次回推奨プラン実行）

- **再開ゲート**: `HANDOFF` / `project-context` / `runtime-state` を起点に次回実行順を固定
- **verification**:
  - `SUBSEQUENT_playthrough_and_tests.md` に PlayMode 回帰ベースライン参照を追記
  - `2026-04-09-playmode-8-results.md` を新規作成（8/8 pass の基準記録）
- **SP-022/03a**:
  - SP-022 §6.4 に P0/P1/P2 の初期優先度を追加
  - 03a に SUBSEQUENT→LATER の移行判定メモを追加

### 2026-04-10（SUBSEQUENT 完了 → Ch2 分岐プラン）

- **正本**: `docs/verification/2026-04-10-subsequent-completion-report.md` (Ch1 再現手順、静的整合、分岐表)
- **更新**: `SUBSEQUENT_playthrough_and_tests.md` / `2026-04-10-ch1-day1-3-preflight.md` 節 C / `2026-04-09-playmode-8-results.md`（再実行欄）/ SP-022 §6.4.1 / `03a` / `2026-04-08-ch1-subquest-gap-template.md` / `HANDOFF.md` / `17_unlock_triggers.md` §6 先頭
- **判定**: Editor 実測まで P0 有無は未確定。実測 P0 なし → LATER（Ch2）。P0 あり → 短い P0 のみ

### 2026-04-10 (Content レーン — Ch1 Day3 + 検証・CI 導線)

- **Ch1**: `Ch1_Day1.yarn` に Day3（`Ch1_Day3_*`）を追加。`ch1.asset` の `m_TotalDays: 3` と Day 開始ノードを更新
- **Day2 Winding**: `fragment_ch1_02` の `UnlockTopic`（03a の断片 #2 導線）
- **SP-022**: Day3 パイロット（`scout_ch1_d3_route` / `scout_ch1_d3_board` / `annot_ch1_d3_compare`）。`03a` / `22` / `17` を同期
- **SUBSEQUENT**: `docs/verification/templates/SUBSEQUENT_playthrough_and_tests.md` チェックリスト新設
- **LATER**: `docs/StorySpec/LATER_CH2_PLAYBOOK.md` オペレーション短冊
- **CI**: `.github/workflows/unity-playmode-tests.yml` 新設。EditMode ワークフローの Unity 版を `6000.3.6f1` に合わせる

### 2026-04-10 (docs cleanup phase 2-4)

- レガシー文書整理を継続し、重複 wiki ページを正典へ移植後に削除
- `docs/EVIDENCE_REUSE.md` を新設し、archive 側 Evidence Reuse 文書を統合
- `docs/archive/ROADMAP_TO_PRODUCTION.md` を要約移植後に削除
- docs/wiki 第4弾で `characters` / `branch` / `chapter-patterns` / `ui-config` / `troubleshooting` を削除し、wiki をポータル最小構成へ縮約
- `FEATURE_STATUS_AUDIT.md` の旧 archive 参照を「整理済み（履歴参照）」へ更新

### 2026-04-10 (LATER + 意思決定解説)

- **project-context.md** / **HANDOFF.md** / **runtime-state.md**: LATER スライス、意思決定解説、チェックリスト
- コード・Yarn 変更なし（当該コミット）

### 2026-04-09 (SUBSEQUENT + 手動ハンズオン)

- SUBSEQUENT スライス、HANDOFF 手動ハンズオン追記 等（ドキュメントのみ）

---

2026-04-08 以前の Session Log は [docs/archive/runtime-state-session-log-2026-03_04.md](archive/runtime-state-session-log-2026-03_04.md) に切出済み (session 10〜22)。
