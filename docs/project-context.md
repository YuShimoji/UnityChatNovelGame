# Project Context

## PROJECT CONTEXT

- プロジェクト名: FoundPhone (UnityChatNovelGame)
- 環境: Unity 6.3 LTS (6000.3.6f1) / C# / Yarn Spinner 3.1.3 / DOTween
- ブランチ戦略: trunk-based (main のみ)
- 現フェーズ: プロトタイプ → α移行中
- 直近の状態 (2026-04-07):
  - session 21 までの技術系: PlayMode 失敗の根本原因は auto-start の missing_node:Start。HasNode 事前チェック + archive 除外 + TearDown StopScenario で修正。WORKFLOW_STATE_SSOT.md 廃止
  - 2026-04-07: **主軸を Ch1 コンテンツ前進に固定**。Ch1 を開いて確認済み。既知 UI 不具合は **docs/UI_ISSUES.md** に記録し、**局所コード修正は行わない**（バッチ時にまとめて対応）
  - AI の役割: Yarn 執筆ではなく制作ツール・パイプライン・検証導線の整備（USER_REQUEST_LEDGER と整合）
  - 次の作業: **Ch1 を Day 単位で Yarn 上で前進** → `Tools > FoundPhone > Content Pipeline` で **Sync Authoring Assets** → **ContentAuthoring**（本編）で通し／Day 跨ぎを確認。**ScenarioManager の StartNode は再生目的に合わせて Inspector で確認**（例: 本編 Ch1 はチャンネル／シナリオ既定、縦スライス検証は `VerticalSlice_Start` 等、デバッグ小出しは `DQT_Start`）。PlayMode 4 件は **コンテンツを止めない範囲**で好機に実ラン（EN-012）
  - 2026-04-08: **SP-022**（`docs/StorySpec/22_subquest_exploration_content.md`）新設。**次推奨スライス**は「サブクエスト探索：設計チャーター確定 + Ch1 パイロット 1〜3 本」（既存 Yarn コマンドのみ。B 型 Wiki 等のエンジン改修は初期スライスではしない）
  - 2026-04-09: **SUBSEQUENT RECOMMENDED SLICE**（SP-022 達成後）を [docs/project-context.md](docs/project-context.md) に追記。**手動確認ハンズオン**を [docs/HANDOFF.md](docs/HANDOFF.md) に追加（Ch1 + サブスレッド）
  - 2026-04-10: **LATER RECOMMENDED SLICE**（SUBSEQUENT 完了・Ch2 着手後）と **推奨プランの読み方・手動意思決定の解説**を [docs/project-context.md](docs/project-context.md) に追加。[docs/HANDOFF.md](docs/HANDOFF.md) に **手動意思決定チェックリスト**を追加

### 運用メモ

- 現在の系列: Ch1 コンテンツ前進 + 制作パイプライン実運用（既知 UI は UI_ISSUES.md へ）
- ユーザーはデザイナー兼ライター。手動でのストーリー追加がまだ未実施 — wiki で解消予定
- nightshift の変更品質が問題化。部分的・不完全な変更が検証負担を増大させるパターン。完成度優先へ
- スレッド管理 (BeginBranch/EndBranch) がユーザーに複雑と指摘された — PLAN MODE でリファクタ設計要
- task-scout 指摘の残件: verification/ 空、E2E 自動検証未整備
- 2026-03-30 session 19: `docs/verification/2026-03-30-playmode-batchmode-attempt.md` を追加。PlayMode test code は前進したが、Unity batchmode `-runTests` は XML を出さず終了
- 2026-03-31 session 20: `docs/verification/2026-03-31-playmode-batch-execute.md` を追加。`-executeMethod` で PlayMode 実行自体は通る

---

## CURRENT DEVELOPMENT AXIS

- 主軸: コンテンツ制作フロー実証 + Ch1 完走
- この軸を優先する理由: エンジン基盤は alpha として十分。Session 13-17 が UI 微修正に費やされ、コンテンツ進行が停止。制作フローを実際に回してコンテンツを前進させる
- 今ここで避けるべき脱線: UI 微修正ループ、マネタイズ実装、サウンド統合、過度な仕様策定
- **ワークフロー原則**: 値の調整 (フォント/色/タイミング) は Inspector で行い、コード変更しない。UI バグは docs/UI_ISSUES.md に溜めて一括修正。セッション成果物は「プレイアブルなコンテンツ」か「新機能」

---

## CURRENT LANE

- 主レーン: **Content**（Ch1 を Day 単位でプレイアブルに前進）
- 副レーン: **Unlock**（制作パイプライン同期の実運用確認）+ **Audit**（DQT / Ch2–Ch3 / PlayMode は好機のみ。コンテンツを止めない範囲）
- 今このレーンを優先する理由: エンジン基盤は alpha として十分。ボトルネックは **コンテンツの前進**と **制作フローの実走**
- いまは深入りしないレーン: **UI_ISSUES.md 載せ項目の個別コード修正**、サウンド、マネタイズ、スレッド管理リファクタの本実装（IP-PC-002）

---

## CURRENT SLICE

- スライス名: **Ch1 コンテンツ前進 + 制作パイプライン実運用**（UI バッチは触らない）
- ユーザー操作列: Yarn 編集 → Content Pipeline で **Sync Authoring Assets** → ContentAuthoring（または既定の再生シーン）で **Ch1 通し／Day 跨ぎ**を確認 → 新規気づきは **UI は UI_ISSUES.md**、**進行不能のみ**ブロッカーとして別メモ
- 成功状態: Ch1 の **次の Day／節**が執筆またはノード構成として繋がり、セッションごとに **「コンテンツが増えた」**状態が残る。既知 Ch1 UI 件はバッチ対象としてリストのみ増やす
- このスライスで必要な基盤能力: タップスキップ (済)、タイミング (済)、wiki (済)、Validator (済)、SOGenerator (済)、Content Pipeline (済)
- 今回はやらないこと: UI_ISSUES 記載項目の**個別**コード修正、サウンド、マネタイズ、IP-PC-002 本実装

---

## NEXT RECOMMENDED SLICE（推奨・CURRENT の直後）

- スライス名: **サブクエスト探索：設計チャーター（SP-022）+ Ch1 パイロット 1〜3 本**
- 目的: サブスレッド（主に **C 型偵察・短い A 型注釈**）で探索パートを積み、ボリューム方針を文章で確定したうえで、Ch1 で**既存コマンドのみ**プロトタイプする
- ユーザー操作列: **SP-022 をレビュー・追記**（§3〜§4 の仮数・優先）→ `03a_ch1_section_beats.md` または Ch1 Yarn に **節↔サブクエスト ID の対応** → `DeclareThread*` / `CompleteThread` 等で実装 → Content Pipeline 同期 → 再生確認
- 成功状態: (1) SP-022 が執筆時に迷わない粒度、(2) Ch1 に再現手順付きサブクエスト **1〜3 本**、(3) **エンジンギャップ**が §6 に列挙されている
- 今回はやらないこと: **B 型 Wiki 遷移の新規エンジン実装**（仕様未確定のまま着手しない）、アルケミーボード、IP-PC-002 本実装、UI_ISSUES の個別修正
- 副次: [docs/StorySpec/17_unlock_triggers.md](docs/StorySpec/17_unlock_triggers.md) に Ch1 用具体例を **1 ページ分**追記できる状態にする

---

## SUBSEQUENT RECOMMENDED SLICE（SP-022 達成後）

- スライス名: **Ch1 統合プレイ検証 + サブクエストギャップの優先度付け**
- 目的: メイン Ch1 とパイロット済みサブクエストを **一連の手動プレイ**で通し、[docs/StorySpec/22_subquest_exploration_content.md](docs/StorySpec/22_subquest_exploration_content.md) §6 の **エンジン／仕様ギャップを P0/P1/P2 で優先度付け**。仕様未承認のまま B 型 Wiki 実装に入らない
- 作業: (1) [docs/HANDOFF.md](docs/HANDOFF.md) の **手動確認ハンズオン（Ch1 + サブスレッド）**で通し確認 (2) ギャップを SP-022 §6 または別表に **P0/P1/P2** で追記 (3) [docs/StorySpec/17_unlock_triggers.md](docs/StorySpec/17_unlock_triggers.md) に **Ch1 用具体例 1 ページ**（NEXT の副次を完了）(4) 好機に PlayMode 4 件の実ラン結果を [docs/verification/](docs/verification/) に 1 ファイルで残す（EN-012）
- 成功状態: 「Ch1 + サブ」が **再現手順付き**で説明可能に通る。ギャップ一覧があり、次に **仕様のみ**か **実装スライス**か選べる
- 今回はやらないこと: UI_ISSUES の個別コード修正（中期 S24 まで）、B 型 Wiki のエンジン実装（仕様承認まで）、IP-PC-002 本実装

---

## LATER RECOMMENDED SLICE（SUBSEQUENT 完了・Ch2 執筆着手後）

- スライス名: **中期接続：Ch2 本編執筆 + ギャップ P0 の扱い分岐**
- 前提: SUBSEQUENT でギャップに **P0/P1/P2** が付いた状態。Ch2 Day1 の執筆を開始している（ロードマップ **S21–22** に相当）
- 推奨デフォルト: **まず Ch2 をメイン＋サブの同じパターンで前進**する（[docs/StorySpec/22_subquest_exploration_content.md](docs/StorySpec/22_subquest_exploration_content.md) の優先・本数方針を Ch2 にコピー）。**P0 が「進行不能」またはセーブ／スレッドのデータ破綻なら、その項目だけ**仕様確定＋実装（または仕様のみ）の **短いスライス**を挟む。**P1/P2 はこの段階では実装しない**
- 作業: `Ch2_LocationConfusion.yarn`（または Ch2 本体）を編集 → Content Pipeline で同期 → ContentAuthoring で再生確認。**BL-002（ポートレート）**は「Ch2 の視認性がボトルネック」と判断した時点で着手可否を決める（中期 **S23**）
- 今回はやらないこと: 中期 **S24**（UI バッチ）まで **UI_ISSUES を都度コード修正しない**。P1/P2 のエンジン丸ごと実装、IP-PC-002 本実装

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

## DEVELOPMENT ROADMAP (2026-03-30 策定)

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


| 工程        | 手動/自動 | ツール                                                             | 状態       |
| --------- | ----- | --------------------------------------------------------------- | -------- |
| シナリオ設計    | 手動    | SCENARIO_AUTHORING_GUIDE                                        | done     |
| Yarn執筆    | 手動    | VSCode + Yarn Spinner Extension                                 | done     |
| 静的バリデーション | 自動    | YarnContentValidator (Editor)                                   | done     |
| SO自動生成    | 自動    | YarnSOGenerator + Content Pipeline (Topic/Character/Channel 同期) | **done** |
| Unity再生確認 | 手動    | ContentAuthoring シーン                                            | done     |
| E2E自動検証   | 自動    | 未実装 — ETK拡張でPlayModeテスト                                         | **todo** |
| 調整        | 手動    | Unity Inspector + Yarn編集                                        | done     |
| ビルド       | 自動    | Unity Build Pipeline (モバイル)                                     | 未設定      |
| 配布        | 手動    | App Store / Google Play                                         | 未設定      |


### 未実装ツール要求（Pipeline設計から抽出）

1. **E2E自動検証 (PlayMode)**: 全チャプターを自動再生しブロッカーを検出。ETKの拡張として実装

---

## DECISION LOG

CLAUDE.md の DECISION LOG を参照。ここには project-context.md 作成以降の決定のみ追記する。


| 日付         | 決定事項                                              | 選択肢                            | 決定理由                                                                                  |
| ---------- | ------------------------------------------------- | ------------------------------ | ------------------------------------------------------------------------------------- |
| 2026-03-27 | 最終出力形態: ゲームアプリ (モバイル優先)                           | ゲームアプリ / 録画動画 / 両方 / 未定        | チャットUIがモバイル9:16で最も自然。既存レスポンシブ基盤と整合                                                    |
| 2026-03-27 | 自動化範囲: SO自動生成 + E2E自動検証の両方                        | 最小限 / SO自動生成 / E2E自動検証 / 両方    | コンテンツ量産時の手動SO作成が最大の摩擦。E2E検証で回帰防止                                                      |
| 2026-03-27 | サウンド統合: コンテンツ後回し (Ch3以降)                          | BGM+SE先行 / コンテンツ後回し / なし       | ゲームプレイの核を先に固める。サウンドはコンテンツが揃ってから                                                       |
| 2026-03-27 | マネタイズ: F2P + 広告                                   | 後回し / F2P+広告 / 買い切り / スコープ外    | モバイルアプリのスタンダードモデル。エンジンへの影響は広告動線設計時に検討                                                 |
| 2026-03-29 | タップスキップ + タイミング設定可能化                              | タップスキップ / F11のみ / 自動送り         | VN標準のテキスト送り操作。Inspector で TypingIndicatorDuration(0.8s), PostMessageDelay(0.4s) を調整可能 |
| 2026-03-29 | Branch Thread: Yarn 再入防止フラグ必須 + コード安全策            | フラグ必須 / コードのみ / 両方             | フラグで1回限り + BeginBranch再入時に古い履歴クリア。仕様書 21_branch_thread_spec.md 作成                     |
| 2026-03-29 | フォントサイズ: messageFontSize 28→34 + スケール下限 0.78→0.85 | 28維持 / 32 / 34 / 36            | CanvasScaler MatchHeight=1.0 で狭Canvas時のレスポンシブ縮小に耐える。34*0.85=28.9px                    |
| 2026-03-29 | Authoring Wiki: Docsify ベースで docs/wiki/ に作成       | Docsify / MkDocs / 単一HTML / なし | CDNのみでビルド不要。既存 .md を活かせる。npx docsify serve で即起動                                       |
| 2026-03-30 | フォントサイズバランス: messageFontSize 28→22 + body 18→20   | 22 / 24 / UIFontConfig全体引き上げ   | .asset nightshift膨張値が未revertだった根本原因を修正。Inspector微調整可能                                 |
| 2026-03-30 | 開発ワークフロー再構造化                                      | 現状維持 / コンテンツ優先 / UI先行          | 5セッションのUI微修正ループを脱却。値調整はInspector、UIバグは一括処理、セッション成果はコンテンツか機能                           |
| 2026-03-30 | AI の役割: 制作システム整備。Yarn 執筆はユーザー                     | AI執筆 / AI支援+ユーザー執筆 / ユーザー単独    | ユーザーフィードバック。最も欲しいのは「人間が執筆するためのシステム周り」。機能検証はDebugQuickTestで行い本編を実験台にしない                |
| 2026-04-07 | Ch1 で発見した UI 不具合はバッチまで個別コード修正しない                    | 都度修正 / 記録のみ→バッチ                      | 全体開発（コンテンツ前進）を優先。UI_ISSUES.md に集約。中期ロードマップ S24 の UI バッチ修正でまとめる                                |
| 2026-04-08 | ボリュームはサブスレッド探索（サブクエスト）の積み上げで取る。ゲーム設計の空白は **SP-022** で先行策定 | メインのみ厚く / サブ探索で厚く / 後回し | ユーザー方針。資料以上の探索パートはチャーター→Ch1 パイロット→ギャップ列挙の順。初期はエンジン改修しない（B 型 Wiki 等は要仕様） |
| 2026-04-09 | SP-022 達成後の正式順序: **通し手動検証 → ギャップ P0/P1/P2 →（副次）SP-017 Ch1 例 → Ch2 執筆（S21–22）** | パイロット直ちに Ch2 / 検証を挟む | サブクエスト追加後の回帰と仕様負債を可視化してから次章へ。エンジン実装はギャップ承認後の別スライス |
| 2026-04-10 | SUBSEQUENT 後のデフォルトは **Ch2 をメイン＋サブで前進**。エンジンは **P0 のみ**短いスライスで対応。P1/P2 は繰り上げない | Ch2 前に P1 まで実装 / P0 も後回し | コンテンツの前進を主レーンに保ちつつ、プレイ不能だけを止める。S24 まで UI 都度修正はしない方針と整合 |


---

## IDEA POOL

CLAUDE.md の IDEA POOL を参照。ここには project-context.md 作成以降のアイデアのみ追記する。


| ID        | アイデア                  | 状態      | 関連領域         | 再訪トリガー                                                         |
| --------- | --------------------- | ------- | ------------ | -------------------------------------------------------------- |
| IP-PC-001 | メッセージごとのポートレート画像挿入    | active  | ui/新機能       | Unity実機確認完了後。HUMAN_AUTHORITY: インラインアバター拡大 or 独立画像バブル or カットイン？ |
| IP-PC-002 | スレッド管理のシンプル化リファクタリング  | active  | system/リファクタ | PLAN MODE で設計後。BeginBranch/EndBranch/SwitchToThread の責務整理      |
| IP-PC-003 | StartWait 中のタップスキップ対応 | backlog | system/演出    | 現在は RunLineAsync 内の遅延のみ。StartWait のスキップも要検討                    |


---

## HANDOFF SNAPSHOT (2026-04-10)

- 主レーン: **Content**（Ch1 前進） / 副: **Unlock**（Pipeline 実運用）
- **CURRENT スライス**: Ch1 コンテンツ前進 + パイプライン実運用。Ch1 既知 UI は **UI_ISSUES.md** に記録済み（本スライスではコード変更なし）
- **次に推奨（NEXT）**: **SP-022**（`docs/StorySpec/22_subquest_exploration_content.md`）を埋め、Ch1 にサブクエスト（サブスレッド探索）パイロット **1〜3 本**。spec-index **SP-022** 参照
- **その次（SUBSEQUENT）**: Ch1 統合プレイ検証 + ギャップ **P0/P1/P2** 付け。手順は **HANDOFF.md の手動確認ハンズオン** → SP-017 Ch1 例 1 ページ → 好機に EN-012 ログ。完了後 **Ch2 執筆（ロードマップ S21–22）**へ
- **その次（LATER）**: Ch2 本編＋サブを **SP-022 と同パターン**で前進。**P0 のみ**仕様／実装の短いスライス可。詳細は **推奨プランの読み方と手動意思決定**、判断は **HANDOFF の手動意思決定チェックリスト**
- 次にやること（CURRENT）:
  1. Ch1 を Yarn 上で前に進める（Day／節）
  2. Content Pipeline → Sync Authoring Assets → 本編シーンで再生確認
  3. **StartNode は目的に合わせて** ScenarioManager の Inspector で確認（本編 Ch1 / VerticalSlice / DQT など）
- 長期メモ（設計論点の置き場）: UIFontConfig・ThreadSwitcher・IP-PC-002・ポートレート・EN-012 拡張は **HANDOFF.md / runtime-state.md** および IDEA POOL を参照
- 今は触らない: UI_ISSUES の個別修正、サウンド、マネタイズ、**B 型 Wiki のエンジン新規実装**（SP-022 のギャップ扱い）

