# Project Context

## PROJECT CONTEXT

- プロジェクト名: FoundPhone (UnityChatNovelGame)
- 環境: Unity 6.3 LTS (6000.3.6f1) / C# / Yarn Spinner 3.1.3 / DOTween
- ブランチ戦略: trunk-based (main のみ)
- 現フェーズ: プロトタイプ → α移行中
- 直近の状態 (2026-04-03 session 22):
  - Session 21: 根本原因特定 + HasNode 事前チェック + テスト 4→8件拡充 + WORKFLOW_STATE_SSOT.md 廃止
  - Session 22: タイプライター同期修正 (DOTween 完了イベント待機に統一)。DebugChatScene 整備 (DQT_Start + AutoStartYarn + ChatDialogueView 追加)。SaveManager AutoSaveIndicator 安全化。PlayMode 8/8 passed + EditMode 75/75 passed → Phase 1 テストゲート通過
  - 次の作業: Content Pipeline 実機検証 (Sync → DQT/Ch1/Ch2 再生確認) → GitHub Actions CI 統合

### 運用メモ

- ユーザーはデザイナー兼ライター。AI は Yarn 執筆ではなく制作ツール/パイプライン整備に注力
- 値調整 (フォント/色/タイミング) は Inspector で行い、コード変更しない
- UI バグは docs/UI_ISSUES.md に溜めて一括修正
- PlayMode テスト: 8件 (SmokeGate 4 + ScenarioFlow 4)。共通ヘルパー分離済み
- batch 実行: `-executeMethod` 経路で NUnit XML (.xml) + plain text (.txt) 両出力対応
- Unity batchmode CLI コンパイルチェック: return code 0 で通過確認済み (2026-04-02)

---

## CURRENT DEVELOPMENT AXIS

- 主軸: E2E テスト安定化 → パイプライン実機証明 → Phase A クロージング
- この軸を優先する理由: エンジン基盤・テストコードは揃った。次は Unity 実機で動作を証明し、Alpha 宣言のゲートを通過する
- 今ここで避けるべき脱線: UI 微修正ループ、新規大型機能、サウンド統合、マネタイズ実装
- **ワークフロー原則**: 値の調整はInspector、UIバグは一括処理、セッション成果はコンテンツか機能

---

## CURRENT LANE

- 主レーン: Advance (E2E テスト安定化 + 拡充)
- 副レーン: Unlock (Content Pipeline 実機証明)
- 今このレーンを優先する理由: テスト基盤が安定すれば CI 統合 → Phase A 手動検証消化 → Alpha 宣言の道筋が開ける
- いまは深入りしないレーン: UI微調整、サウンド、マネタイズ

---

## CURRENT SLICE

- スライス名: PlayMode テスト8件実機確認 + Content Pipeline 実運用証明
- ユーザー操作列: Inspector で m_StartNode=DQT_Start → テスト8件実行 → Content Pipeline Sync → DQT/Ch1/Ch2 再生
- 成功状態: 8 passed / 0 failed + Content Pipeline の Sync が SO を正しく更新 + DQT/Ch1/Ch2 が Console エラーなしで再生
- このスライスで必要な基盤能力: HasNode 事前チェック (済)、UnityTearDown (済)、batch XML (済)、共通ヘルパー (済)
- 今回はやらないこと: UI微修正、サウンド、マネタイズ、新規テストケース追加

---

## DEVELOPMENT ROADMAP (2026-04-02 改訂 v2)

### Phase 1 (S22-23): パイプライン証明

| タスク | Actor | 前提 | 完了条件 |
|--------|-------|------|----------|
| Inspector で m_StartNode を DQT_Start に変更 | user | なし | シーン保存済み |
| Unity Test Runner で8件実行 | user | 上記完了 | 8 passed / 0 failed |
| Content Pipeline 実機検証 | shared | テスト通過 | Sync → SO 更新確認 |
| DQT / Ch1 / Ch2 通しプレイ | user | Pipeline 検証済み | Console エラーなし |
| GitHub Actions CI 統合 | assistant | batch XML 動作確認 | PR ごとに自動テスト |

### Phase 2 (S24-26): Alpha ゲート + 体験基盤

| タスク | 種別 | Actor | 優先度 | 備考 |
|--------|------|-------|--------|------|
| Phase A 手動検証消化 (65項目) | Audit | shared | 高 | Alpha 宣言のゲート |
| ETK 全24コマンド網羅テスト | Infra | assistant | 高 | EN-012 60% → 90% |
| BGM/SE 基盤 (SP-009) | System | assistant | 中 | Yarn コマンド <<PlayBGM>> 等 |
| Chapter Transition Phase 2 (SP-019) | System | assistant | 中 | 次Ch解放通知 + ダッシュボード自動表示 |
| Onboarding Phase 2 (SP-020) | System | assistant | 中 | 矛盾操作チュートリアル |

### Phase 3 (S27-29): 演出深化 + ENH 実装

| タスク | 種別 | Actor | 優先度 | 備考 |
|--------|------|-------|--------|------|
| ENH 候補のうち approved を実装 | ENH | assistant | -- | FEATURE_REGISTRY.md から選定 |
| タイプライター制御拡張 (ENH-001/002) | ENH | assistant | -- | Yarn からのテンポ制御 |
| メッセージ演出バリエーション (ENH-003) | ENH | assistant | -- | 振動/フェードイン等 |
| HalluciCoin 獲得演出 (ENH-012) | ENH | assistant | -- | パーティクル/カウントアップ |
| Progress Phase 2 (SP-018) | System | assistant | 中 | チャプター間接続可視化 |
| Chapter Transition Phase 3 (SP-019) | System | assistant | 中 | SP-018 統合演出 |

### Phase 4 (S30+): コンテンツ量産 + 製品化

| タスク | 種別 | Actor | 備考 |
|--------|------|-------|------|
| Ch3 以降コンテンツ執筆 | Content | user | 人間側作業 |
| 執筆中に見つかる ENH を随時登録 | ENH | shared | Yarn オーサリング拡張等 |
| サウンド統合 | System | assistant | コンテンツが揃ってから |
| マネタイズ設計 (SP-010) | System | shared | F2P + 広告 |
| iOS / Android ビルドパイプライン | Infra | assistant | モバイル向け |
| Beta テスト → リリース | -- | shared | 最終段階 |

### ボトルネック遷移

```
Phase 1              Phase 2               Phase 3              Phase 4
パイプライン証明      Alpha ゲート          演出深化             コンテンツ量産
     |                    |                     |                    |
     v                    v                     v                    v
8件 PASS 確認 ───> Phase A 65項目 ───> ENH approved ───> Ch3+ 執筆
                   CI 自動回帰          実装              BGM/SE 統合
                   ETK 全コマンド       タイプライター     ビルド
                   BGM/SE 基盤         演出拡張           リリース
```

### ENH の受け容れ構造

Phase 3 は ENH 実装のための専用フェーズだが、ENH の候補登録は全フェーズで随時行う:
- Phase 1-2: パイプライン証明・テスト中に気づいた改善点を candidate 登録
- Phase 3: approved された ENH を集中実装
- Phase 4: 執筆中に見つかる Yarn オーサリング系 ENH を随時登録・実装

ENH の詳細は `docs/FEATURE_REGISTRY.md` を参照。

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
| E2E自動検証 | 自動 | PlayMode 8件 + batch XML 出力。共通ヘルパー分離済み | **partial (60%)** |
| 調整 | 手動 | Unity Inspector + Yarn編集 | done |
| ビルド | 自動 | Unity Build Pipeline (モバイル) | 未設定 |
| 配布 | 手動 | App Store / Google Play | 未設定 |

### 未実装ツール要求（Pipeline設計から抽出）

1. **E2E自動検証 (PlayMode)**: 全チャプターを自動再生しブロッカーを検出。ETKの拡張として実装

---

## DECISION LOG

CLAUDE.md の DECISION LOG を参照。ここには project-context.md 作成以降の決定のみ追記する。

| 日付 | 決定事項 | 選択肢 | 決定理由 |
|------|----------|--------|----------|
| 2026-03-27 | 最終出力形態: ゲームアプリ (モバイル優先) | ゲームアプリ / 録画動画 / 両方 / 未定 | チャットUIがモバイル9:16で最も自然。既存レスポンシブ基盤と整合 |
| 2026-03-27 | 自動化範囲: SO自動生成 + E2E自動検証の両方 | 最小限 / SO自動生成 / E2E自動検証 / 両方 | コンテンツ量産時の手動SO作成が最大の摩擦。E2E検証で回帰防止 |
| 2026-03-27 | サウンド統合: コンテンツ後回し (Ch3以降) | BGM+SE先行 / コンテンツ後回し / なし | ゲームプレイの核を先に固める。サウンドはコンテンツが揃ってから |
| 2026-03-27 | マネタイズ: F2P + 広告 | 後回し / F2P+広告 / 買い切り / スコープ外 | モバイルアプリのスタンダードモデル。エンジンへの影響は広告動線設計時に検討 |
| 2026-03-29 | タップスキップ + タイミング設定可能化 | タップスキップ / F11のみ / 自動送り | VN標準のテキスト送り操作。Inspector で TypingIndicatorDuration(0.8s), PostMessageDelay(0.4s) を調整可能 |
| 2026-03-29 | Branch Thread: Yarn 再入防止フラグ必須 + コード安全策 | フラグ必須 / コードのみ / 両方 | フラグで1回限り + BeginBranch再入時に古い履歴クリア。仕様書 21_branch_thread_spec.md 作成 |
| 2026-03-29 | フォントサイズ: messageFontSize 28→34 + スケール下限 0.78→0.85 | 28維持 / 32 / 34 / 36 | CanvasScaler MatchHeight=1.0 で狭Canvas時のレスポンシブ縮小に耐える。34*0.85=28.9px |
| 2026-03-29 | Authoring Wiki: Docsify ベースで docs/wiki/ に作成 | Docsify / MkDocs / 単一HTML / なし | CDNのみでビルド不要。既存 .md を活かせる。npx docsify serve で即起動 |
| 2026-03-30 | フォントサイズバランス: messageFontSize 28→22 + body 18→20 | 22 / 24 / UIFontConfig全体引き上げ | .asset nightshift膨張値が未revertだった根本原因を修正。Inspector微調整可能 |
| 2026-03-30 | 開発ワークフロー再構造化 | 現状維持 / コンテンツ優先 / UI先行 | 5セッションのUI微修正ループを脱却。値調整はInspector、UIバグは一括処理、セッション成果はコンテンツか機能 |
| 2026-03-30 | AI の役割: 制作システム整備。Yarn 執筆はユーザー | AI執筆 / AI支援+ユーザー執筆 / ユーザー単独 | ユーザーフィードバック。最も欲しいのは「人間が執筆するためのシステム周り」。機能検証はDebugQuickTestで行い本編を実験台にしない |

---

## IDEA POOL

CLAUDE.md の IDEA POOL を参照。ここには project-context.md 作成以降のアイデアのみ追記する。

| ID | アイデア | 状態 | 関連領域 | 再訪トリガー |
|----|----------|------|----------|--------------|
| IP-PC-001 | メッセージごとのポートレート画像挿入 | active | ui/新機能 | Unity実機確認完了後。HUMAN_AUTHORITY: インラインアバター拡大 or 独立画像バブル or カットイン？ |
| IP-PC-002 | スレッド管理のシンプル化リファクタリング | active | system/リファクタ | PLAN MODE で設計後。BeginBranch/EndBranch/SwitchToThread の責務整理 |
| IP-PC-003 | StartWait 中のタップスキップ対応 | backlog | system/演出 | 現在は RunLineAsync 内の遅延のみ。StartWait のスキップも要検討 |

---

## HANDOFF SNAPSHOT (session 21)

- 現在の主レーン: Advance (E2E テスト安定化 + 拡充)
- 現在のスライス: Content Pipeline 実機検証 (Phase 1 テストゲート通過済み)
- session 22 で完了した作業:
  - タイプライター同期修正: ChatDialogueView の独自計時を DOTween 完了イベント待機に統一
  - DebugChatScene 整備: m_StartNode=DQT_Start, AutoStartYarn=true, ChatDialogueView 追加
  - SaveManager AutoSaveIndicatorRoutine の null チェック追加 (シーン遷移安全化)
  - PlayMode テスト 0/8 → 8/8 passed (SafeTeardown 強化 + シーン遷移先動的解決 + テスト簡略化)
  - EditMode テスト 74/75 → 75/75 passed (DontDestroyOnLoad 回避)
- 次回最初にやること:
  1. Content Pipeline 実機検証 (Sync Authoring Assets → DQT/Ch1/Ch2 再生確認)
  2. GitHub Actions CI 統合
- 既知の軽微な問題:
  - DQT_Start の選択肢後スキップ不具合 (急務ではない)
  - 連打時のメッセージ遷移が不安定 (インジケーター + タイプライターの独立遅延が原因。ENH 候補)
- 未確定の設計論点:
  - スレッド管理リファクタリングの方向性 (IP-PC-002、PLAN MODE)
  - ポートレート画像挿入の UI/UX 設計 (IP-PC-001、HUMAN_AUTHORITY)
- 今は触らない範囲: サウンド統合、マネタイズ実装、新規大型機能
