# 22 — サブクエスト探索コンテンツ設計（サブスレッド）

> **ステータス**: DRAFT（2026-04-08）
> **依存**: [16_subthread_ui.md](16_subthread_ui.md), [17_unlock_triggers.md](17_unlock_triggers.md), [14_interaction_mechanics.md](14_interaction_mechanics.md), [01_gdd_gameplay.md](01_gdd_gameplay.md)
> **執筆リファレンス**: [../SCENARIO_AUTHORING_GUIDE.md](../SCENARIO_AUTHORING_GUIDE.md), [../ENGINE_FEATURE_INVENTORY.md](../ENGINE_FEATURE_INVENTORY.md)

---

## 1. 目的

メインスレッド（チャプター主会話）とは別に、**サブスレッド上の「探索パート」**を積み重ね、**プレイ時間・世界観の厚み・任意の深掘り**を増やす。本文書はその**コンテンツ設計のチャーター**であり、既存エンジンで書ける範囲と、仕様・実装が足りない部分の境界を明確にする。

---

## 2. サブクエストの定義（本プロジェクトにおける）

| 用語 | 定義 |
|------|------|
| **サブクエスト** | 1 本の「メイン進行に必須とは限らない、やり込み／補足用の遊び」。**別スレッド（サブスレッドまたは分岐スレッド）**上で完結または一時離脱して体験する。 |
| **完了条件** | スレッド単位で **会話・選択・偵察フローの終端**に到達すること。Yarn では `<<CompleteThread>>` や分岐の `<<EndBranch>>`、またはメインへの合流ノードで表現する（実装コマンドは下表参照）。 |
| **メインとの関係** | **必須**: ストーリー進行のゲートに直結（例: フラグ未取得だと次 Day に進めない）。**任意**: 読了で報酬・理解が増えるがスキップ可能。本文書では章ごとに必須／任意の比率を記載する（初期は仮置きでよい）。 |

「サブクエスト」は企画用語として本書で用いる。エンジン上は [16_subthread_ui.md](16_subthread_ui.md) の **A/B/C/Branch** のいずれか（または組み合わせ）として実装する。

---

## 3. スレッド種別の優先順位（探索ボリューム用）

**プロジェクト採用（2026-04-08 起算）**: 下表の優先は **現行デフォルト**とする。変更する場合は **HUMAN_AUTHORITY** で本節を更新する。

既存の種別定義は SP-016 に従う。**探索の厚みを積む初期フェーズ**では次の優先を推奨する（**HUMAN_AUTHORITY**: レビューで変更可）。

| 優先 | 種別 | ID 接頭辞 | 探索での役割 | 初期の厚み方針 |
|------|------|-----------|--------------|----------------|
| 1 | C: 偵察 | `scout_` | 外出・調査・採取など「動いて確認する」ニュアンス | **最優先で本数を積む**。既存 UI で会話ログとして成立しやすい。 |
| 2 | A: 注釈 | `annot_` | 用語・制度・背景の短い解説 | **短い本数**で世界観の隙間を埋める。長文化しすぎない。 |
| 3 | B: 追跡 | `track_` | Wiki 型・地名／機関の追跡 | **テキストとリンクの「見た目」までは**既存で表現可能な範囲で書く。**アプリ内 Wiki 遷移**はエンジン未実装のため、初期は「疑似リンク（説明のみ）」または後続スライスで仕様化（下節ギャップ参照）。 |
| — | 分岐 | `branch_` | if 型の物語分岐 | メインに近い**重い分岐**は [21_branch_thread_spec.md](21_branch_thread_spec.md) に従い、サブクエストとしての「軽い枝」は C/A と同様に扱える。 |

---

## 4. 章あたりの目標本数（仮レンジ）

**プロジェクト採用（2026-04-08 起算）**: 下表は **執筆・見積の初期仮置き**。Ch1 パイロット実測後に数値を更新する。変更は **HUMAN_AUTHORITY**。

確定値ではない。**Ch1 パイロット**で実測し、SP-022 を更新する。

| 対象 | 仮レンジ（1 章あたり） | 備考 |
|------|------------------------|------|
| C 型（偵察系サブクエスト） | 2〜5 本 | Day／節に分散させ、同時に多くを開かせすぎない |
| A 型（短い注釈） | 2〜6 本 | 1 本あたりメッセージ数に上限の目安を別表で定義予定 |
| B 型（追跡） | 0〜2 本（初期） | Wiki 実装まで本数を絞る |
| 必須 vs 任意 | 必須 1〜2 / 任意 残り | 章のテンポ次第 |

**Ch1 パイロット（成功状態の最小）**: 再現手順とセットで説明できる **サブクエスト 1〜3 本**（主に C または C+A）。

**Ch1 実装メモ（2026-04-08 以降）**: 現行 `Ch1_Day1.yarn` では **合計 6 スレッド手**（Day1: C+A 任意×2 + 潜在A、Day2: B 条件付き + C 任意、Day3: C×2+A を Hub 必須トピック内で Manifest）。いずれも **既存コマンドのみ**。本数はパイロット過剰なので、§4 の仮レンジは **実測後に Ch1 向けに圧縮**してよい。

### 4.1 Ch1 リポジトリ実測（2026-04-10）

Yarn / 03a 対応表に基づく**機械集計**（Editor 通しの可否とは別）。§4 仮レンジとの整合・圧縮方針は **HUMAN_AUTHORITY**。

| 種別 | 本数（Ch1 全 Day 合算） | ID（参照） |
|------|-------------------------|------------|
| C | 4 | `scout_ch1_network`, `scout_ch1_day2_ping`, `scout_ch1_d3_route`, `scout_ch1_d3_board` |
| A | 3 | `ch1_note_facility`, `annot_ch1_glossary`, `annot_ch1_d3_compare` |
| B | 1 | `ch1_cond_analysis` |
| 分岐 | 1 | `ch1_branch_analysis`（Day1 Winding・[21_branch_thread_spec.md](21_branch_thread_spec.md)） |

**レビュー観点**: C/A は §4 のレンジ内。B は 0〜2 の下限付近。分岐はサブクエスト定義の「軽い枝」として別枠でもよい。Day3 で C が 2 本あるため、**テンポ優先なら統合・遅延**を検討（§6.1 の必須 Hub との関係とセット）。

---

## 5. 既存 Yarn コマンドとの対応（執筆時）

詳細は SCENARIO_AUTHORING_GUIDE / ENGINE_FEATURE_INVENTORY を正とする。概要マッピング:

| やりたいこと | 代表コマンド・手段 |
|--------------|-------------------|
| スレッドを登録（即時表示） | `<<DeclareThreadTyped>>`, `<<DeclareThread>>` |
| 潜在 → 条件で顕在化 | `<<DeclareThreadLatent>>`, `<<DeclareThreadLatentCond>>`, `<<ManifestThread>>` |
| 偵察スレッド完了 | `<<CompleteThread>>`（C 型フロー） |
| 分岐の開始／終了 | `<<BeginBranch>>`, `<<EndBranch>>`, `<<SetBranchReflection>>` |
| 解放条件（HC／章クリア） | `ChannelData` SO（SP-017） |

複合条件の Yarn 記法は [17_unlock_triggers.md](17_unlock_triggers.md) のパターン D（仲介変数）等を参照。Ch1 用の具体例は SP-017 に追記予定（本プラン副次）。

---

## 6. エンジン・仕様ギャップ（要仕様 / 要実装）

spec-index（SP-016）と整合。**初期スライスでは実装しない**。ギャップを埋めるかは別スライスで優先度付けする。

### 6.1 要仕様（HUMAN_AUTHORITY が先）

- B 型 **アプリ内 Wiki リンク遷移**の UX（遷移先・履歴・メインへの戻り）
- C 型 **成果物カード**のリッチ表示（テキストのみで足りる範囲 vs カード UI 必須の線引き）
- サブクエスト **解放通知**の統一演出（SP-017 未決）
- 章ごとの **必須／任意**比率と、メイン Day との**推奨挿入位置**（`03a_ch1_section_beats.md` との対応表）
- **Ch1 Day3**: メイン Hub の必須トピック内に C/A を埋め込んだ場合、「任意サブクエスト」の定義を **プレイヤーがスキップ可能か**で再整理する（現状は Day 進行に必要な Hub 通過がサブ完了と結びつく）
- **静的レビュー観察（2026-04-09・Bレーン）**: `ch1_note_facility`（A）および `ch1_cond_analysis`（B）の Yarn には **`CompleteThread` が無い**。エンジン上問題ない設計か、Save/Load でスレッド状態が残り続けるかは **実測で確認**（未判定のまま [docs/verification/templates/2026-04-08-ch1-subquest-gap-template.md](../verification/templates/2026-04-08-ch1-subquest-gap-template.md) の G-OBS-20260409 を参照）。

### 6.2 要実装（仕様確定後）

- B 型 Wiki 遷移のエンジン対応（未実装）
- C 型成果物カードの UI 拡張（未実装／部分実装の整理）
- （必要なら）複合トリガーのエディタ支援・バリデーション強化

### 6.3 Ch1 パイロット実装サマリ（リポジトリ）

`Ch1_Day1.yarn` に **既存コマンドのみ**で以下を追加済み（詳細は `03a_ch1_section_beats.md` の対応表）。

| ID | 型 | 概要 |
|----|-----|------|
| scout_ch1_network | C | Day1 Hub 任意。`DeclareThreadTyped` + `CompleteThread` |
| annot_ch1_glossary | A | Day1 Hub 任意（断片閲覧後）。短い用語メモ |
| scout_ch1_day2_ping | C | Day2 モック Hub 任意 |
| scout_ch1_d3_route | C | Day3 Mason 報告。Latent → `ManifestThread` |
| scout_ch1_d3_board | C | Day3 断片 #3。Latent → `ManifestThread` |
| annot_ch1_d3_compare | A | Day3 比較。Latent → `ManifestThread` |
| （既存）ch1_note_facility | A | Latent → Manifest（断片フロー） |
| （既存）ch1_cond_analysis | B | `DeclareThreadLatentCond`（断片トピック保持時） |
| ch1_branch_analysis | 分岐 | Day1 Hub から `Ch1_Day1_BranchPyramid` 経由。`BeginBranch` / `EndBranch` |

プレイ検証で見つかった不足は本節 §6.1 / §6.2 に追記し、SUBSEQUENT で P0/P1/P2 付けする。

### 6.4 SUBSEQUENT 判定用の優先度（2026-04-09 仮固定）

`docs/verification/templates/SUBSEQUENT_playthrough_and_tests.md` と合わせて運用する。以下は「次回通し実測で更新する前提の初期優先度」。

| ID / 説明 | 優先 | 判定理由 |
|-----------|------|----------|
| Day3 Hub 必須トピック内で Manifest した C/A が進行不能を起こす | P0 | Ch1 完走不能に直結。LATER へ進む前に最優先で短い修正スライス対象 |
| Save/Load 後に `scout_*` / `annot_*` / `ch1_cond_analysis` の状態が破綻する | P0 | 再現性のあるプレイ継続障害。データ整合性の問題 |
| B 型（`ch1_cond_analysis`）の体験が「リンク遷移前提」に見え、現UIで誤読を招く | P1 | 進行不能ではないが、探索体験の品質低下が大きい |
| Day3 の C 2本（`scout_ch1_d3_route` / `scout_ch1_d3_board`）がテンポを阻害 | P1 | コンテンツ設計上の圧縮判断。仕様調整で回避可能 |
| アプリ内 Wiki 遷移 / 成果物カードの本実装不足 | P2 | 現スライスのスコープ外。仕様確定後に別スライス化 |

#### 6.4.1 実施状況（2026-04-10）

- **静的整合**（`ch1.asset` / `Ch1_Day1.yarn` / `EndDay`）: [docs/verification/2026-04-10-subsequent-completion-report.md](../verification/2026-04-10-subsequent-completion-report.md) で確認済み。テキスト上の P0 兆候はなし。
- **Editor 手動通し・Save/Load・PlayMode 8 件再実行**: 本セッションの実行環境では **未実施**。上表の P0 行は **ランタイム実測が前提**のまま残す。
- **分岐**: 実機で P0 **なし** → [LATER_CH2_PLAYBOOK.md](LATER_CH2_PLAYBOOK.md) に従い Ch2 前進。P0 **あり** → 短い P0 スライスのみ（`project-context` LATER 節）。

#### 6.4.2 実施状況（2026-04-09・Bレーン）

- **静的整合の再確認**: [docs/archive/verification-lanes-2026-04/2026-04-09-blane-sp022-subsequent.md](../archive/verification-lanes-2026-04/2026-04-09-blane-sp022-subsequent.md) §2。C 型パイロットの `Declare*` / `CompleteThread` 対は取れる。
- **PlayMode batch / Editor 通し**: **未実施** — 同一プロジェクトを別 Unity インスタンスが開いており batch が拒否（同ファイル §3）。ギャップテンプレに **G-ENV-20260409** を記録。
- **P0 の断定**: 変更なし（ランタイム実測まで保留）。

---

## 7. 次のアクション（執筆者／プロジェクト）

1. 本書の **§4 仮レンジ**と **§3 優先**をレビューし、Ch1 に合う数値に更新する（当面の運用値は「C: 3〜4, A: 2〜3, B: 0〜1」）。
2. `03a_ch1_section_beats.md` または Ch1 Yarn 冒頭コメントに、**どの Day／節にどのサブクエスト ID を置くか**の対応表を追加する。
3. ~~Ch1 に **パイロット 1〜3 本**を Yarn のみで追加し、Content Pipeline 同期後に再生確認する。~~ → **実施済み**（§4.1・§6.3）。残りは Editor 通しと数値の HUMAN_AUTHORITY 確定。
4. プレイ中に見つかった **ギャップ**を §6 に追記し、別スライス化する。

### 7.1 Bレーンからの提案ドラフト（2026-04-09・**要 HUMAN_AUTHORITY**・未承認）

以下は [docs/ai/PARALLEL_LANE_PROMPTS.md](../ai/PARALLEL_LANE_PROMPTS.md) レーン B の範囲で整理した**提案**であり、確定ではない。

- **§3 種別優先**: Ch1 の機械集計（§4.1）は既存の「C 最優先 → A → B 控えめ」と整合しており、**文言変更は不要**とする判断が妥当（変更する場合はレビュー根拠を §3 に 1 段落で追記）。
- **§4 本数・テンポ**: Day3 で C が 2 本（`scout_ch1_d3_route` / `scout_ch1_d3_board`）ある。テンポ優先なら **1 本に統合**、または **1 本を Day2 以降へ遅延**する案を検討余地として列挙するのみ（実装は Content レーン）。
- **Day3「任意サブ」と Hub 必須**: 現状、Manifest が Hub 必須トピックにぶら下がるため、プレイヤーがメインを進めると **機械的にサブも触る**構造になる。「任意」との両立を取るには、(a) Hub から切り離して Latent のままダッシュボード等から開く、(b) 用語上「任意」を「読了報酬」に寄せて定義を変える、(c) 現状維持を明示する、のいずれかの **明示的選択**が必要。

---

## 8. 更新履歴

- **2026-04-10**: §6.4.1 に静的整合完了と Editor 未実施の区別、分岐ルールへの参照を追記。
- **2026-04-09**: §6.4 に SUBSEQUENT 判定用の P0/P1/P2 初期優先度を追加。LATER 接続の判定基準を明文化。
- **2026-04-09**: Bレーン成果 — §6.1 静的レビュー観察、§6.4.2（実施状況）、§7.1（要 HUMAN_AUTHORITY の提案ドラフト）。
- **2026-04-10**: §4.1 Ch1 実測表（機械集計）を追加。プラン v2 Phase B 向けに §3・§4 のレビュー材料を明示。
- **2026-04-10**: Day3 パイロット（C×2+A）と `ch1` 3 日目。§6.1 に必須 Hub とサブ定義の整理メモ。
- **2026-04-08**: Ch1 パイロット（Yarn）追加、§3/§4 プロジェクト採用注記、§6.3 実装サマリ
- **2026-04-08**: 初版（DRAFT）。プロジェクト次期プランに合わせて新設。
