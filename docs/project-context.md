# Project Context

## PROJECT CONTEXT

**優先関係:** `docs/REPO_LOCAL_RULES.md` と `docs/INVARIANTS.md` の責務境界は本ファイルの実行計画より**上位**の制約である。実行計画がガードレールと矛盾する場合、ガードレールが優先される。衝突する場合はガードレール遵守方向に修正する。

- 表形式の決定履歴: [docs/DECISION_LOG.md](DECISION_LOG.md)（必要時のみ）

- プロジェクト名: FoundPhone (UnityChatNovelGame)
- 環境: Unity 6000.4.9f1 / C# / Yarn Spinner 3.1.3 / DOTween
- ブランチ戦略: trunk-based (main のみ)
- 現フェーズ: プロトタイプ → α移行中
- 直近の状態 (2026-04-10 リモート同期マージ、2026-04-09 handoff 追記):
  - **運用 (2026-06-03)**: ローカル追跡差分と handoff 文脈を `origin/main` へ反映する同期ブロック。別端末の最短再開は Unity 6000.4.9f1 + `docs/HANDOFF.md`。`NotoSansJP-Regular SDF.asset` の dynamic cache reset 後の日本語表示は次回画面検収で重点確認
  - **運用 (2026-06-08)**: Codex の repo-local 実行環境固定を削除し、欠落 `.claude/hooks/*.sh` 参照も除去。`origin/main` 先行分を fast-forward で取り込み、追加の IconSide / SP-023 テストと handoff 文脈を project-local docs に固定
  - **運用 (2026-07-06)**: Prompt により T+1 Writer / Designer Cockpit MVP を active slice に昇格。`Tools > FoundPhone > Writer Cockpit` で Yarn 保存後の Refresh / Validate / Sync / Apply / Play / Last Action / read-only Save status を一画面化。Unity 6000.4.9f1 は存在するが Package Manager `path undefined` で停止するため、次はその解消後に compile/menu/Apply/Play 確認
  - **運用 (2026-07-11)**: fresh resolve の `path undefined` は package JSON ではなく、Codex / PowerShell 子環境の標準 Windows 変数 `ALLUSERSPROFILE` 欠落が直接原因と判明。`tools/run-unity.ps1` で process-local に補完し、ProjectCache 欠落状態から 39 packages 登録、compile、return code 0 を確認。次は Writer Cockpit interactive 受入
  - **運用 (2026-07-13)**: `main` / `origin/main` を `fa8eb8b`、`HEAD...@{u}` を `0/0` で再確認。コード・Yarn・scene・asset の追加変更はなく、別端末は `docs/HANDOFF.md` の Current Focus / Safe Next Steps から Writer Cockpit interactive 受入を再開する
  - **運用 (2026-07-17)**: 2026-07-15 の未コミット docs 4件をレビューして保持し、`main` / `origin/main` を `7eb09c0`、ahead/behind `0/0` で再同期。現 checkout で Unity 6000.4.9f1 batch open / Tundra compile / Yarn validator errors=0 / strict docs build を再確認。全83テストと Writer Cockpit interactive 受入は未実行のまま、active slice と owner 境界を維持
  - **運用 (2026-07-21)**: tracked static fixtureをSites-native Vinext/React runtimeへ等価変換し、Owner-only accessのVersion 1をhost。source/version/deployment/accessのIDと変換対応を`docs/verification/2026-07-20-sites-private-runtime-validation.md`へ固定。次はOwner sign-in後のhosted本文reviewで、public/shared accessは未承認
  - **運用 (2026-07-26)**: `73aef720`で`main` / `origin/main` parity 0/0とfast-forward pull済みを確認。Unity 6000.4.9f1 compile、Yarn errors 0 / warnings 0、strict docs、static Sites fixtureを再検証。現行test定義は基礎83 + targeted Editor 14 = 97で、次はpersistent saveを保護するtest isolationと基礎83件の現行基準化
  - **運用 (2026-07-27)**: `main` / `origin/main` / GitHub readbackを`2f37534`、ahead/behind 0/0で確認。remote review branch `origin/codex/sites-authoring-bridge-v1=e059e4b`は現mainの直系1 commitで、Unity compile、targeted Editor 18/18、Writer Cockpit direct window、fixture/generated local preview accessまで検証済み。main統合・人間受入・deployは未実施
  - **運用 (2026-04-09)**: セッション引き継ぎで `main`≒`origin/main` を確認。計測用 `debug-*.log` は `.gitignore` でリポジトリ外に固定。再開の最短導線は `docs/HANDOFF.md` の Handoff snapshot
  - **技術 (session 21–22)**: PlayMode 失敗の根本原因は auto-start の missing_node:Start。HasNode 事前チェック + archive 除外 + TearDown（`UnityTearDown` + `StopScenario` + 待機）で修正。WORKFLOW_STATE_SSOT.md 廃止。Session 22: タイプライター同期（DOTween 完了待機）、DebugChatScene 整備、SaveManager AutoSaveIndicator 安全化、PlayMode **8**/8・EditMode 75/75 をローカルで通過（batch XML・共通ヘルパー分離済み）
  - **方向性修正 (2026-04-15)**: Ch 積み上げ構造の構造的ドリフトを修正。主軸を**エンジン能力マイルストーン**に切替。ガードレール（`docs/REPO_LOCAL_RULES.md` / `docs/INVARIANTS.md`）を実行計画より上位に再配置。SUBSEQUENT を通過ゲート（スキップ不可）に変更
  - AI の役割: Yarn 執筆ではなく制作ツール・パイプライン・検証導線の整備（USER_REQUEST_LEDGER と整合）
  - 次の作業: assistantはsave data isolationと基礎83件の現行回帰を閉じる。人間はWriter Cockpit interactive loopとOwner-only hosted本文を並行受入し、その後SP-023 / SP-024表示検収とM1へ戻る

### 運用メモ

- 現在の系列: Writer / Designer Cockpit による制作パイプライン実運用 + その後のエンジン能力マイルストーン復帰（既知 UI は UI_ISSUES.md へ）
- ユーザーはデザイナー兼ライター。AI は Yarn 執筆ではなく制作ツール／パイプライン整備に注力
- 値調整（フォント／色／タイミング）は Inspector。UI バグは UI_ISSUES.md に溜めて一括修正
- PlayMode テスト: tracked PlayMode フォルダは **10 件**（2026-06-08 に SP-023 / IconSide 2 件追加）。最後に記録済みの全体回帰ベースラインは 2026-04-09 の **8/8**。batch: `-executeMethod` で NUnit `.xml` + `.txt` 両出力
- nightshift の変更品質が問題化しうる — 完成度優先。スレッド管理リファクタは IP-PC-002（PLAN MODE）
- task-scout 指摘の残件: verification/ の実ラン記録拡充、E2E（EN-012）継続
- 2026-03-30 session 19: `docs/verification/2026-03-30-playmode-batchmode-attempt.md`。`-runTests` は XML 未生成で終了する事例あり
- 2026-03-31 session 20: `docs/verification/2026-03-31-playmode-batch-execute.md`。`-executeMethod` で PlayMode 実行は通る

---

## CURRENT DEVELOPMENT AXIS

- 主軸: **エンジン能力の前進 + 検証コンテンツによる実証**
- この軸の意味: セッション成果は「エンジン/ツールとして何ができるようになったか」で測る。コンテンツ（Yarn 執筆）はエンジン能力を検証する手段として使い、コンテンツ量の増加自体を進捗指標にしない
- エンジン基盤の状態: alpha として基本コマンド群の動作は確認済みだが、FEATURE_STATUS_AUDIT の未実装 15 件・未確認 13 件が残存。サブスレッド全型の実機検証、セーブ/ロードの完全性、章遷移の堅牢化は未達
- 今ここで避けるべき脱線: UI_ISSUES 載せ項目の個別コード修正ループ、過度な仕様策定のみ、サウンド／マネタイズの先取り、**エンジン能力の検証を伴わないコンテンツ積み上げ**
- **ワークフロー原則**: 値の調整は Inspector、UI バグは UI_ISSUES.md に一括、セッション成果は「**エンジン/ツール能力の前進**」を第一に。コンテンツのみのセッションが 2 回続いたらドリフト警告

---

## CURRENT LANE

- 主レーン: **Authoring Tooling**（Writer / Designer Cockpit MVP + Sites Authoring Bridge review）
- 副レーン: **Engine / UI Review**（Cockpit導線確認後に SP-023 / SP-024 表示検収へ戻る）
- 優先理由: 一画面の作者UX、source navigation、Validator warning信頼性に加え、remote review branchではYarn→Package v1→local preview bridgeとexact accessが実装・compile・targeted test済み。残るbottleneckはcandidateの人間受入とmain統合判断、interactive Apply / Play、人間設定のsource jump、実セーブを保護した基礎83件の現行回帰
- いまは深入りしないレーン: Yarn本文執筆、Runtime Dashboard/DebugHub の広範なPrefab化、既存セーブデータ操作、UI_ISSUES.md 載せ項目の個別コード修正、サウンド、マネタイズ

---

## CURRENT SLICE

- スライス名: **Writer / Designer Cockpit MVP + Sites Authoring Bridge review**
- 目的: 既存の作者loopに加え、非canon検証nodeを決定的Package v1としてlocal static previewへ渡すcandidateが、実際に作者から到達可能かを受け入れる
- ユーザー操作列: 外部エディタで `.yarn` 保存 → `Tools > FoundPhone > Writer Cockpit` → `Refresh Nodes` → `Validate Then Sync` → 推奨または任意 Start Node を選択 → `Apply Node To ContentAuthoring Scene` または `Play ContentAuthoring From Selected Node`
- 成功状態: CockpitがUnityから開け、既存Apply/Playを維持し、選択した非canon検証nodeのPackage exportとfixture/generated previewを人間が迷わず実行できる。candidate受入後もmain統合は別判断とする
- 現在の残り: review branch `e059e4b`はdirect window、Export操作の存在、compile、18/18、fixture/generated HTTP/browser accessまで実証。実Export操作、Apply / Play / Last Action、External Script Editor jumpの人間受入とmain統合判断が未完了
- コンテンツの扱い: 既存 active Yarn のみ使用。新規ストーリー本文は書かない
- 今回はやらないこと: [横断保留](#横断保留) を参照

---

## NEXT RECOMMENDED SLICE（推奨・CURRENT の直後）

- スライス名: **Save-isolated current regression baseline**
- 目的: Unity 6000.4.9f1上の基礎EditMode 73 / PlayMode 10を、実ユーザーsaveへ触れない現行基準として記録する
- 作業列: testが触れるpersistent pathを列挙 → test専用pathまたは安全な退避/復元を設計 → 基礎83件batch → targeted Editor 14の再実行判断 → 日付付きXML / txt記録
- 成功状態: 実ユーザーsaveに変更なし、基礎83件の合否と残失敗を説明可能、全97件のassembly別結果が追跡可能
- コンテンツの扱い: 既存 demo Yarn のみ使用。新規執筆はしない
- 今回はやらないこと: [横断保留](#横断保留) を参照

---

## SUBSEQUENT RECOMMENDED SLICE (通過ゲート・スキップ不可)

- スライス名: **エンジン能力レビュー + Ch1 フルコンテンツ執筆の解放判定**
- **発動条件**: エンジン能力マイルストーン 1 と 2 の成功状態を**両方**達成していること
- 目的: FEATURE_STATUS_AUDIT の未実装/未確認から次に取り組むべき項目を P0/P1/P2 で優先度付けし、Ch1 フルコンテンツ執筆を開始してよいかを判定する
- 判定基準:
  - P0（進行不能・データ破綻）が 0 件 → Ch1 フルコンテンツ執筆を LATER で解放
  - P0 が残存 → P0 修正のスライスを挟む
- 作業: (1) FEATURE_STATUS_AUDIT 未実装/未確認の再評価 (2) P0/P1/P2 振り分け (3) PlayMode テスト再実行 (4) 判定結果を本ファイルに記録
- **このゲートはスキップ不可**。LATER に進むには SUBSEQUENT を通過する必要がある
- 今回はやらないこと: [横断保留](#横断保留) を参照

---

## LATER RECOMMENDED SLICE（SUBSEQUENT 通過後）

- スライス名: **Ch1 フルコンテンツ前進 + エンジン能力 P1 の段階的実装**
- 前提: SUBSEQUENT のエンジン能力ゲートを通過し、P0 が 0 件であること
- 作業: (1) Ch1 の Day 単位でのコンテンツ前進（SP-022 サブクエスト含む）(2) P1 項目の仕様確定と実装を並行
- 進捗指標: チャプター数ではなく「新しいエンジン能力の検証を伴うコンテンツ」の割合
- Ch2 への移行条件: Ch1 で検証した全エンジン能力が PlayMode テストでカバーされていること
- 今回はやらないこと: [横断保留](#横断保留) を参照

---

## 横断保留

スライス別の「今回はやらないこと」をここに集約する。

| 項目 | いつまで / 条件 |
|------|-----------------|
| UI_ISSUES の個別コード修正 | **M6** 以降（UI バッチ）まで |
| サウンド・マネタイズの先取り | ロードマップどおり後回し |
| IP-PC-002 本実装 | PLAN MODE 設計完了まで |
| B 型 Wiki のエンジン新規実装 | SP-022 §6 ギャップの仕様承認まで |
| アルケミーボードの再開 | NEXT スライスでは着手しない |
| P1/P2 のエンジンを丸ごと実装 | LATER ではしない（**P0 のみ**短いスライス可） |

---

## 推奨プランの読み方と手動意思決定（解説）

- **四段スライスの意味**
  - **CURRENT**: Writer / Designer Cockpit MVP の interactive 受入
  - **NEXT**: save-isolated現行83テスト基準化。targeted Editor 14と合わせて97件の現在地を固定し、その後SP-023 / SP-024表示検収またはM1へ復帰
  - **SUBSEQUENT**: **通過ゲート（スキップ不可）**。エンジン能力レビュー + Ch1 フルコンテンツ執筆の解放判定。M1+M2 完了が発動条件
  - **LATER**: Ch1 フルコンテンツ前進 + P1 段階実装 (SUBSEQUENT 通過後)
- **進行順序**: CURRENT → NEXT → SUBSEQUENT → LATER の順。SUBSEQUENT は通過ゲートであり、スキップ不可。エンジン能力の確認なしにフルコンテンツ執筆に進むことを防ぐ

- **HUMAN_AUTHORITY（人間が先に決める領域）**
  - SP-022 の **§3・§4**（スレッド種別の優先、章あたり本数の仮レンジ）。
  - SP-022 **§6** に出たギャップのうち、「プレイ体験としてどう見せるか」の **1 段落の仕様**（特に B 型 Wiki・C 型成果物カード・解放通知）。
  - [docs/StorySpec/17_unlock_triggers.md](docs/StorySpec/17_unlock_triggers.md) の **解放記法・通知演出**の具体値。
  - **どのサブクエストを必須／任意にするか**、トーン・シナリオ内容。  
  これらは **エンジン実装や大きなコード変更の前に**承認・記述しておく。

- **P0 / P1 / P2 の目安**
  - **P0**: 進行不能、セーブロード後の破綻、誤表示でプレイ継続が困難。
  - **P1**: 次の「仕様／エンジン」スプリントで取る価値が高いが、回避策で執筆は進められる。
  - **P2**: M6 以降の UI バッチや別マイルストーンまで延期してよい。

- **AI・エージェントとの役割分担**
  - **向いている**: ツール・パイプライン・検証導線・ドキュメント同期・PlayMode 補助。
  - **ユーザー（ライター／デザイナー）向き**: ストーリー内容、キャラの語り口、サブクエストの必須度、上記 HUMAN_AUTHORITY の判断。

---

## ENGINE CAPABILITY MILESTONES (2026-04-15 改訂: エンジン能力ベース)

旧ロードマップ（2026-03-30 策定、Ch 番号ベース S18-S28）は Ch 積み上げ構造のため廃止。エンジン能力マイルストーンで再構成する。

### 短期: エンジン能力マイルストーン 1-2

- **M1**: サブスレッド全型の実機検証 (A/B/C 型、Latent、Branch) — DebugQuickTest + 最小モック + PlayMode テスト追加
- **M2**: セーブ/ロード完全性 + 章遷移堅牢化 — Save/Load ラウンドトリップ + EndDay/章遷移の検証

### 中期: エンジン能力レビュー + コンテンツ解放

- **M3**: FEATURE_STATUS_AUDIT 再評価、未実装/未確認の P0/P1/P2 振り分け (SUBSEQUENT ゲート)
- **M3 通過後**: Ch1 フルコンテンツ前進 (P1 実装と並行)。SP-022 サブクエスト探索もこの段階
- **M4**: E2E 自動検証の拡充 (EN-012/EN-013)
- **M5**: Ch2 コンテンツ前進 (Ch1 で検証したエンジン能力の PlayMode テストカバレッジ確認後)

### 長期: 製品化

- **M6**: SP-019 Phase 2-3 / SP-020 Phase 2-3 / SP-018 Phase 2
- **M7**: Android 初回ビルド + BL-002 ポートレートアイコン
- **M8**: Ch3-9 + サウンド + Beta テスト + リリース

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
| E2E自動検証 | 自動 | PlayMode フォルダ **10** 件 + batch XML（.xml/.txt）。共通ヘルパー分離済み。最後の記録済み全体回帰は 2026-04-09 の 8/8、2026-06-08 追加 2 件は Unity 6000.4.9f1 実行待ち。全チャプター網羅は ETK 拡張で継続 | **partial（EN-012 目安 60%）** |
| 調整 | 手動 | Unity Inspector + Yarn編集 | done |
| ビルド | 自動 | Unity Build Pipeline (モバイル) | 未設定 |
| 配布 | 手動 | App Store / Google Play | 未設定 |

### 未実装ツール要求（Pipeline設計から抽出）

1. **E2E自動検証 (PlayMode)**: 全チャプターを自動再生しブロッカーを検出。ETKの拡張として実装
