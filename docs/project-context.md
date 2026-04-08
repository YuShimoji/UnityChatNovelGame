# Project Context

## PROJECT CONTEXT

**優先関係:** 実行計画は本ファイルの `CURRENT DEVELOPMENT AXIS` / `CURRENT LANE` / `CURRENT SLICE` に従う。ルート `CLAUDE.md` の DEVELOPMENT PURPOSE は全体ガードレール。衝突する提案はユーザー確認のうえで。

- 表形式の決定履歴: [docs/DECISION_LOG.md](DECISION_LOG.md)（必要時のみ）

- プロジェクト名: FoundPhone (UnityChatNovelGame)
- 環境: Unity 6.3 LTS (6000.3.6f1) / C# / Yarn Spinner 3.1.3 / DOTween
- ブランチ戦略: trunk-based (main のみ)
- 現フェーズ: プロトタイプ → α移行中
- 直近の状態 (2026-04-10 リモート同期マージ、2026-04-09 handoff 追記):
  - **運用 (2026-04-09)**: セッション引き継ぎで `main`≒`origin/main` を確認。計測用 `debug-*.log` は `.gitignore` でリポジトリ外に固定。再開の最短導線は `docs/HANDOFF.md` の Handoff snapshot
  - **技術 (session 21–22)**: PlayMode 失敗の根本原因は auto-start の missing_node:Start。HasNode 事前チェック + archive 除外 + TearDown（`UnityTearDown` + `StopScenario` + 待機）で修正。WORKFLOW_STATE_SSOT.md 廃止。Session 22: タイプライター同期（DOTween 完了待機）、DebugChatScene 整備、SaveManager AutoSaveIndicator 安全化、PlayMode **8**/8・EditMode 75/75 をローカルで通過（batch XML・共通ヘルパー分離済み）
  - **コンテンツ軸 (2026-04-07〜10)**: 主軸を **Ch1 コンテンツ前進**に固定。既知 UI は **docs/UI_ISSUES.md**、局所コード修正はバッチまで保留。**SP-022** / SUBSEQUENT / LATER のスライスと意思決定ドキュメントを反映
  - AI の役割: Yarn 執筆ではなく制作ツール・パイプライン・検証導線の整備（USER_REQUEST_LEDGER と整合）
  - 次の作業: Ch1 を Day 単位で Yarn 上で前進 → Content Pipeline **Sync Authoring Assets** → ContentAuthoring で通し確認。**StartNode** は再生目的に合わせて Inspector で確認。PlayMode 8 件・Pipeline 実機・**GitHub Actions CI** は好機に（EN-012 / インフラ）

### 運用メモ

- 現在の系列: Ch1 コンテンツ前進 + 制作パイプライン実運用（既知 UI は UI_ISSUES.md へ）
- ユーザーはデザイナー兼ライター。AI は Yarn 執筆ではなく制作ツール／パイプライン整備に注力
- 値調整（フォント／色／タイミング）は Inspector。UI バグは UI_ISSUES.md に溜めて一括修正
- PlayMode テスト: **8 件**。batch: `-executeMethod` で NUnit `.xml` + `.txt` 両出力
- nightshift の変更品質が問題化しうる — 完成度優先。スレッド管理リファクタは IP-PC-002（PLAN MODE）
- task-scout 指摘の残件: verification/ の実ラン記録拡充、E2E（EN-012）継続
- 2026-03-30 session 19: `docs/verification/2026-03-30-playmode-batchmode-attempt.md`。`-runTests` は XML 未生成で終了する事例あり
- 2026-03-31 session 20: `docs/verification/2026-03-31-playmode-batch-execute.md`。`-executeMethod` で PlayMode 実行は通る

---

## CURRENT DEVELOPMENT AXIS

- 主軸: **コンテンツ制作フロー実証 + Ch1 完走**（並行してテスト・パイプラインは好機に実ラン）
- この軸を優先する理由: エンジン基盤は alpha として十分。ボトルネックは **コンテンツの前進**と **制作フローの実走**。Session 13–17 型の UI 微修正ループを避ける
- 今ここで避けるべき脱線: UI_ISSUES 載せ項目の個別コード修正ループ、過度な仕様策定のみ、サウンド／マネタイズの先取り
- **ワークフロー原則**: 値の調整は Inspector、UI バグは UI_ISSUES.md に一括、セッション成果は「プレイアブルなコンテンツ」か「新機能」

---

## CURRENT LANE

- 主レーン: **Content**（Ch1 を Day 単位でプレイアブルに前進）
- 副レーン: **Unlock**（制作パイプライン同期の実運用確認）+ **Audit**（DQT / Ch2–Ch3 / PlayMode 8 件は好機のみ。コンテンツを止めない範囲）
- 優先理由: 上記 **CURRENT DEVELOPMENT AXIS** の「この軸を優先する理由」と同じ
- いまは深入りしないレーン: **UI_ISSUES.md 載せ項目の個別コード修正**、サウンド、マネタイズ、スレッド管理リファクタの本実装（IP-PC-002）

---

## CURRENT SLICE

- スライス名: **Ch1 コンテンツ前進 + 制作パイプライン実運用**（UI バッチは触らない）
- ユーザー操作列: Yarn 編集 → Content Pipeline で **Sync Authoring Assets** → ContentAuthoring（または既定の再生シーン）で **Ch1 通し／Day 跨ぎ**を確認 → 新規気づきは **UI は UI_ISSUES.md**、**進行不能のみ**ブロッカーとして別メモ。静的整合の記録は [docs/verification/2026-04-10-ch1-day1-3-preflight.md](docs/verification/2026-04-10-ch1-day1-3-preflight.md)
- 成功状態: Ch1 の **次の Day／節**が執筆またはノード構成として繋がり、セッションごとに **「コンテンツが増えた」**状態が残る。既知 Ch1 UI 件はバッチ対象としてリストのみ増やす
- このスライスで必要な基盤能力: タップスキップ (済)、タイミング (済)、wiki (済)、Validator (済)、SOGenerator (済)、Content Pipeline (済)
- 今回はやらないこと: [横断保留](#横断保留) を参照

---

## NEXT RECOMMENDED SLICE（推奨・CURRENT の直後）

- スライス名: **サブクエスト探索：設計チャーター（SP-022）+ Ch1 パイロット 1〜3 本**
- 目的: サブスレッド（主に **C 型偵察・短い A 型注釈**）で探索パートを積み、ボリューム方針を文章で確定したうえで、Ch1 で**既存コマンドのみ**プロトタイプする
- ユーザー操作列: **SP-022 をレビュー・追記**（§3〜§4 の仮数・優先）→ `03a_ch1_section_beats.md` または Ch1 Yarn に **節↔サブクエスト ID の対応** → `DeclareThread*` / `CompleteThread` 等で実装 → Content Pipeline 同期 → 再生確認
- 成功状態: (1) SP-022 が執筆時に迷わない粒度、(2) Ch1 に再現手順付きサブクエスト **1〜3 本**、(3) **エンジンギャップ**が §6 に列挙されている
- 今回はやらないこと: [横断保留](#横断保留) を参照（特に B 型 Wiki 未承認実装・アルケミーボード）
- 副次: [docs/StorySpec/17_unlock_triggers.md](docs/StorySpec/17_unlock_triggers.md) に Ch1 用具体例を **1 ページ分**追記できる状態にする

---

## SUBSEQUENT RECOMMENDED SLICE（SP-022 達成後）

- スライス名: **Ch1 統合プレイ検証 + サブクエストギャップの優先度付け**
- 目的: メイン Ch1 とパイロット済みサブクエストを **一連の手動プレイ**で通し、[docs/StorySpec/22_subquest_exploration_content.md](docs/StorySpec/22_subquest_exploration_content.md) §6 の **エンジン／仕様ギャップを P0/P1/P2 で優先度付け**。仕様未承認のまま B 型 Wiki 実装に入らない
- 作業: (1) [docs/HANDOFF.md](docs/HANDOFF.md) の **手動確認ハンズオン（Ch1 + サブスレッド）**で通し確認 (2) ギャップを SP-022 §6 または別表に **P0/P1/P2** で追記 (3) [docs/StorySpec/17_unlock_triggers.md](docs/StorySpec/17_unlock_triggers.md) に **Ch1 用具体例 1 ページ**（NEXT の副次を完了）(4) 好機に PlayMode **8** 件の実ラン結果を [docs/verification/](docs/verification/) に 1 ファイルで残す（EN-012）
- 成功状態: 「Ch1 + サブ」が **再現手順付き**で説明可能に通る。ギャップ一覧があり、次に **仕様のみ**か **実装スライス**か選べる
- 今回はやらないこと: [横断保留](#横断保留) を参照

---

## LATER RECOMMENDED SLICE（SUBSEQUENT 完了・Ch2 執筆着手後）

- スライス名: **中期接続：Ch2 本編執筆 + ギャップ P0 の扱い分岐**
- 前提: SUBSEQUENT でギャップに **P0/P1/P2** が付いた状態。Ch2 Day1 の執筆を開始している（ロードマップ **S21–22** に相当）
- 推奨デフォルト: **まず Ch2 をメイン＋サブの同じパターンで前進**する（[docs/StorySpec/22_subquest_exploration_content.md](docs/StorySpec/22_subquest_exploration_content.md) の優先・本数方針を Ch2 にコピー）。**P0 が「進行不能」またはセーブ／スレッドのデータ破綻なら、その項目だけ**仕様確定＋実装（または仕様のみ）の **短いスライス**を挟む。**P1/P2 はこの段階では実装しない**
- 作業: `Ch2_LocationConfusion.yarn`（または Ch2 本体）を編集 → Content Pipeline で同期 → ContentAuthoring で再生確認。**BL-002（ポートレート）**は「Ch2 の視認性がボトルネック」と判断した時点で着手可否を決める（中期 **S23**）
- 今回はやらないこと: [横断保留](#横断保留) を参照（LATER では P1/P2 丸ごと実装しない）

---

## 横断保留

スライス別の「今回はやらないこと」をここに集約する。

| 項目 | いつまで / 条件 |
|------|-----------------|
| UI_ISSUES の個別コード修正 | 中期 **S24**（UI バッチ）まで |
| サウンド・マネタイズの先取り | ロードマップどおり後回し |
| IP-PC-002 本実装 | PLAN MODE 設計完了まで |
| B 型 Wiki のエンジン新規実装 | SP-022 §6 ギャップの仕様承認まで |
| アルケミーボードの再開 | NEXT スライスでは着手しない |
| P1/P2 のエンジンを丸ごと実装 | LATER ではしない（**P0 のみ**短いスライス可） |

---

## 推奨プランの読み方と手動意思決定（解説）

- **四段スライスの意味**
  - **CURRENT**: いま優先している塊（Ch1 メイン前進 + パイプライン）。
  - **NEXT**: その直後。**SP-022** でサブクエスト方針を確定し Ch1 にパイロット 1〜3 本。
  - **SUBSEQUENT**: Ch1 メイン＋サブの **通し手動検証**と、ギャップの **P0/P1/P2** 付け。副次で SP-017 Ch1 例・EN-012 ログ。
  - **LATER**: SUBSEQUENT 完了後、**Ch2 執筆**へ進みつつ **P0 だけ**例外スライスしうる段階。  
  上から順に「完了」を積むのが安全。**検証なしで次章だけ厚くする**と、ギャップと仕様負債が見えにくくなる。

- **HUMAN_AUTHORITY（人間が先に決める領域）**
  - SP-022 の **§3・§4**（スレッド種別の優先、章あたり本数の仮レンジ）。
  - SP-022 **§6** に出たギャップのうち、「プレイ体験としてどう見せるか」の **1 段落の仕様**（特に B 型 Wiki・C 型成果物カード・解放通知）。
  - [docs/StorySpec/17_unlock_triggers.md](docs/StorySpec/17_unlock_triggers.md) の **解放記法・通知演出**の具体値。
  - **どのサブクエストを必須／任意にするか**、トーン・シナリオ内容。  
  これらは **エンジン実装や大きなコード変更の前に**承認・記述しておく。

- **P0 / P1 / P2 の目安**
  - **P0**: 進行不能、セーブロード後の破綻、誤表示でプレイ継続が困難。
  - **P1**: 次の「仕様／エンジン」スプリントで取る価値が高いが、回避策で執筆は進められる。
  - **P2**: 中期 S24 の UI バッチや別マイルストーンまで延期してよい。

- **AI・エージェントとの役割分担**
  - **向いている**: ツール・パイプライン・検証導線・ドキュメント同期・PlayMode 補助。
  - **ユーザー（ライター／デザイナー）向き**: ストーリー内容、キャラの語り口、サブクエストの必須度、上記 HUMAN_AUTHORITY の判断。

---

## DEVELOPMENT ROADMAP (2026-03-30 策定、2026-04-10 技術注記)

- **技術メモ（session 22 時点）**: PlayMode **8** 件・EditMode 75 件・batch XML はコード上整備済み。CI 統合・全シナリオ実機はこの表の「好機」欄と HANDOFF で追う。

### 短期 (Session 18-20): Ch1 完走 + 制作フロー実証

- **補足 (2026-04-08)**: 「Ch1 完走」と並行して、**短期〜中期の境界**で **サブクエスト探索（SP-022）**を明示的に挟む。Session 番号は固定せず、**メイン Day 執筆が一段落したタイミング**で NEXT RECOMMENDED SLICE に移行する
- S18: Ch1 Day1 通しプレイ + Day2 執筆開始 + ツール実証
- S19: Ch1 Day2 完成 + Day1→Day2 遷移テスト
- S20: Ch1 Day3 完成 + Ch1 通しプレイ + SP-019/020 Phase 1 検証

### 中期 (Session 21-28): Alpha ビルド (Ch1-2 完走可能)

- **補足 (2026-04-09)**: **SUBSEQUENT RECOMMENDED SLICE**（Ch1 統合検証 + ギャップ優先度付け）を完了したら、主軸をここに接続し **Ch2 執筆（S21–22）**へ移る
- **補足 (2026-04-10)**: Ch2 執筆中の分岐・優先は **LATER RECOMMENDED SLICE** と本文書「推奨プランの読み方と手動意思決定」を参照（P0 のみ例外スライス、S23 は視認性が詰まったら）
- S21-22: Ch2 Day1-3 執筆
- S23: BL-002 ポートレートアイコン
- S24: Ch1-2 通しプレイ + UI バッチ修正
- S25-26: Ch3 Day1-3 執筆
- S27: SP-019 Phase 2 + SP-020 Phase 2
- S28: Android 初回ビルド

### 長期 (Session 29+): Beta → リリース

- Ch4-6 (第2部) → Ch7-9 (第3部) → サウンド → Beta テスト → リリース
- 旧 `docs/archive/ROADMAP_TO_PRODUCTION.md` の要旨は本節へ統合済み

### ENH・長期ロードマップ（2026-04-02 改訂 v2 要約）

- **Phase 2–4**（Alpha ゲート、ENH 集中、製品化）は `docs/FEATURE_REGISTRY.md` と整合。詳細テーブルが必要なら履歴コミット参照。
- ENH の候補登録は全フェーズで随時。Phase 3 で approved ENH を集中実装する方針は変更なし。

---

## FINAL DELIVERABLE IMAGE

- 最終成果物: モバイル向けチャット/ビジュアルノベルゲームアプリ（FoundPhone）
- プラットフォーム: モバイル優先 (iOS/Android)
- マネタイズ: F2P + 広告
- サウンド: コンテンツ充実後に統合（Ch3以降）

### コンテンツ制作Pipeline（確定）

```
シナリオ設計 → Yarn執筆 → YarnContentValidator → SO自動生成 → Unity再生確認 → E2E自動検証 → 調整 → ビルド → 配布
  [手動]        [手動]      [自動/Editor]          [自動/Editor]   [手動]           [自動/PlayMode]   [手動]   [自動]   [手動]
```

| 工程 | 手動/自動 | ツール | 状態 |
|------|-----------|--------|------|
| シナリオ設計 | 手動 | SCENARIO_AUTHORING_GUIDE | done |
| Yarn執筆 | 手動 | VSCode + Yarn Spinner Extension | done |
| 静的バリデーション | 自動 | YarnContentValidator (Editor) | done |
| SO自動生成 | 自動 | YarnSOGenerator + Content Pipeline (Topic/Character/Channel 同期) | **done** |
| Unity再生確認 | 手動 | ContentAuthoring シーン | done |
| E2E自動検証 | 自動 | PlayMode **8** 件 + batch XML（.xml/.txt）。共通ヘルパー分離済み。全チャプター網羅は ETK 拡張で継続 | **partial（EN-012 目安 60%）** |
| 調整 | 手動 | Unity Inspector + Yarn編集 | done |
| ビルド | 自動 | Unity Build Pipeline (モバイル) | 未設定 |
| 配布 | 手動 | App Store / Google Play | 未設定 |

### 未実装ツール要求（Pipeline設計から抽出）

1. **E2E自動検証 (PlayMode)**: 全チャプターを自動再生しブロッカーを検出。ETKの拡張として実装
