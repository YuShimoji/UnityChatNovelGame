# Project Context

## PROJECT CONTEXT

- プロジェクト名: FoundPhone (UnityChatNovelGame)
- 環境: Unity 6.3 LTS (6000.3.6f1) / C# / Yarn Spinner 3.1.3 / DOTween
- ブランチ戦略: trunk-based (main のみ)
- 現フェーズ: プロトタイプ → α移行中
- 直近の状態 (2026-03-27 session 12):
  - Pipeline設計確定: モバイルアプリ / F2P+広告 / SO自動生成+E2E自動検証 / サウンド後回し
  - エンジン基盤: 24 Yarnコマンド、Save/Load、Branch Phase1-4、サブスレッドUI、レスポンシブ基盤 全実装済み
  - spec-index: 34エントリ (done 22 / partial 9 / draft 1 / todo 2)
  - Phase A手動検証 (65項目+SP-019/020) 未完了 — Unity Editor必須
  - 次の作業: SO自動生成ツール or Phase A手動検証 or Ch3設計

### 運用メモ

- 現在の系列: レガシー根絶完了 → Pipeline設計 + Phase A手動検証
- エンジン基盤は一通り揃っており、コードベース健全性も監査済み
- Phase A手動検証はユーザーのUnity Editor操作が必要
- session 10: 全コードベース監査 + レガシー19件削除 + FEATURE_STATUS_AUDIT.md作成

---

## CURRENT DEVELOPMENT AXIS

- 主軸: Pipeline実装 (SO自動生成 + E2E自動検証) + Phase A検証クロージング
- この軸を優先する理由: Pipeline設計が確定したため、設計から抽出されたツール要求 (SO自動生成/E2E自動検証) の実装と、Unity手動検証による品質確定が次の前提
- 今ここで避けるべき脱線: 過度なリファクタ、マネタイズ実装、サウンド統合

---

## CURRENT LANE

- 主レーン: Unlock (SO自動生成ツール / E2E自動検証)
- 副レーン: Audit (Phase A手動検証の完了)
- 今このレーンを優先する理由: Pipeline確定によりツール要求が明確化。実装すればコンテンツ量産時の摩擦が大幅に減る
- いまは深入りしないレーン: サウンド統合、マネタイズ実装、Ch3シナリオ執筆（SO自動生成ツール完成後）

---

## CURRENT SLICE

- スライス名: Pipelineツール実装 (SO自動生成)
- ユーザー操作列: Unity Editor > Tools > SO Generator → Yarnファイル選択 → Channel/Topic/Character SOが自動生成される
- 成功状態: 新チャプターのYarnを書いた後、SOを手動作成せずにEditorツール1クリックで必要なSOが揃う
- このスライスで必要な基盤能力: Yarnファイルパース (YarnContentValidatorの技術を転用)
- このスライスから抽出されるツール要求: SO自動生成Editorツール
- 今回はやらないこと: E2E自動検証 (次スライス)、サウンド統合、マネタイズ実装

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
| SO自動生成 | 自動 | 未実装 — Channel/Topic/Character SOをYarnから生成 | **todo** |
| Unity再生確認 | 手動 | ContentAuthoring シーン | done |
| E2E自動検証 | 自動 | 未実装 — ETK拡張でPlayModeテスト | **todo** |
| 調整 | 手動 | Unity Inspector + Yarn編集 | done |
| ビルド | 自動 | Unity Build Pipeline (モバイル) | 未設定 |
| 配布 | 手動 | App Store / Google Play | 未設定 |

### 未実装ツール要求（Pipeline設計から抽出）

1. **SO自動生成ツール (Editor)**: YarnファイルからChannel/Topic/Character SOを自動生成。手動SO作成の手間を削減
2. **E2E自動検証 (PlayMode)**: 全チャプターを自動再生しブロッカーを検出。ETKの拡張として実装

---

## DECISION LOG

CLAUDE.md の DECISION LOG を参照。ここには project-context.md 作成以降の決定のみ追記する。

| 日付 | 決定事項 | 選択肢 | 決定理由 |
|------|----------|--------|----------|
| 2026-03-27 | 最終出力形態: ゲームアプリ (モバイル優先) | ゲームアプリ / 録画動画 / 両方 / 未定 | チャットUIがモバイル9:16で最も自然。既存レスポンシブ基盤と整合 |
| 2026-03-27 | 自動化範囲: SO自動生成 + E2E自動検証の両方 | 最小限 / SO自動生成 / E2E自動検証 / 両方 | コンテンツ量産時の手動SO作成が最大の摩擦。E2E検証で回帰防止 |
| 2026-03-27 | サウンド統合: コンテンツ後回し (Ch3以降) | BGM+SE先行 / コンテンツ後回し / なし | ゲームプレイの核を先に固める。サウンドはコンテンツが揃ってから |
| 2026-03-27 | マネタイズ: F2P + 広告 | 後回し / F2P+広告 / 買い切り / スコープ外 | モバイルアプリのスタンダードモデル。エンジンへの影響は広告動線設計時に検討 |

---

## IDEA POOL

CLAUDE.md の IDEA POOL を参照。ここには project-context.md 作成以降のアイデアのみ追記する。

| ID | アイデア | 状態 | 関連領域 | 再訪トリガー |
|----|----------|------|----------|--------------|

---

## HANDOFF SNAPSHOT

- 現在の主レーン: Unlock (Pipelineツール実装)
- 現在のスライス: SO自動生成ツール
- 今回変更した対象 (session 12): project-context.md (Pipeline設計確定)、CLAUDE.md (DECISION LOG 4件追記)、runtime-state.md
- 次回最初に確認すべきファイル: docs/project-context.md (FINAL DELIVERABLE IMAGE)、docs/runtime-state.md
- 未確定の設計論点: 広告動線設計 (F2P)、WORKFLOW_STATE_SSOT.md廃止可否
- 今は触らない範囲: サウンド統合、マネタイズ実装、Ch3シナリオ執筆
