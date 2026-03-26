# Project Context

## PROJECT CONTEXT

- プロジェクト名: FoundPhone (UnityChatNovelGame)
- 環境: Unity 6.3 LTS (6000.3.6f1) / C# / Yarn Spinner 3.1.3 / DOTween
- ブランチ戦略: trunk-based (main のみ)
- 現フェーズ: プロトタイプ → α移行中
- 直近の状態 (2026-03-26 session 10 nightshift):
  - エンジン基盤: 24 Yarnコマンド、Save/Load、Branch Phase1-4、サブスレッドUI、レスポンシブ基盤 全実装済み
  - session 10: 全コードベース監査 + レガシー根絶 (~920行+シーン2+docs25ファイル削除)
  - spec-index: 34エントリ (done 22 / partial 9 / draft 1 / todo 2) — EN-010(機能状態監査)追加
  - docs/FEATURE_STATUS_AUDIT.md: 実装26機能/未確認10件/未実装15件/懸念6件
  - Phase A手動検証 (65項目+SP-019/020) 未完了 — Unity Editor必須
  - 最終成果物像・Pipeline設計が未定義（要設計、HUMAN_AUTHORITY）

### 運用メモ

- 現在の系列: レガシー根絶完了 → Pipeline設計 + Phase A手動検証
- エンジン基盤は一通り揃っており、コードベース健全性も監査済み
- Phase A手動検証はユーザーのUnity Editor操作が必要
- session 10: 全コードベース監査 + レガシー19件削除 + FEATURE_STATUS_AUDIT.md作成

---

## CURRENT DEVELOPMENT AXIS

- 主軸: 基盤確定 + Pipeline設計
- この軸を優先する理由: エンジン機能は概ね揃ったが、最終成果物像（動画制作者のワークフロー）が未定義。ここを固めないと次のツール開発・自動化・Ch3設計の方向が定まらない
- 今ここで避けるべき脱線: 新機能実装、コンテンツ執筆、過度なリファクタ

---

## CURRENT LANE

- 主レーン: Authoring / Tooling（Pipeline設計フェーズ）
- 副レーン: Acceptance（Phase A手動検証の完了）
- 今このレーンを優先する理由: エンジン基盤の品質確定 + 最終ワークフロー定義が次の全作業の前提
- いまは深入りしないレーン: Experience Slice（Ch3設計）、Runtime Core（新機能追加）

---

## CURRENT SLICE

- スライス名: Pipeline設計 + Phase A クロージング
- ユーザー操作列: (Pipeline設計) 最終成果物像を定義 → 各工程の手動/自動を分類 → ツール要求を抽出
- 成功状態: 「動画制作者がYarnを書き、Unityで再生し、動画として出力する」までの全工程が言語化されている
- このスライスで必要な基盤能力: 既存エンジン機能で十分（新規実装不要）
- このスライスから抽出されるツール要求: Pipeline設計の結果として判明する（現時点では未確定）
- 今回はやらないこと: 新エンジン機能の実装、Ch3シナリオ執筆、マネタイズ設計

---

## FINAL DELIVERABLE IMAGE

- 最終成果物: チャット/ビジュアルノベル形式のストーリーゲーム（FoundPhone）+ そのコンテンツ制作Pipeline
- 最終的なユーザーワークフロー: **未定義 — 要設計**
  - 想定される工程（仮）: シナリオ設計 → Yarn執筆 → Unity再生確認 → 調整 → ビルド/動画出力
  - 各工程の手動/自動の境界が未確定
  - 動画制作者としてのPipelineが未言語化
- 受け入れ時の使われ方: **未定義**
- 現時点で未確定な要素:
  - 最終出力形態（ゲームアプリ / 録画動画 / 両方）
  - コンテンツ制作の自動化範囲
  - 手動介入が必要なポイントの特定
  - サウンド統合の方針
  - マネタイズモデル

---

## DECISION LOG

CLAUDE.md の DECISION LOG を参照。ここには project-context.md 作成以降の決定のみ追記する。

| 日付 | 決定事項 | 選択肢 | 決定理由 |
|------|----------|--------|----------|

---

## IDEA POOL

CLAUDE.md の IDEA POOL を参照。ここには project-context.md 作成以降のアイデアのみ追記する。

| ID | アイデア | 状態 | 関連領域 | 再訪トリガー |
|----|----------|------|----------|--------------|

---

## HANDOFF SNAPSHOT

- 現在の主レーン: Excise (完了) → Advance (Pipeline設計待ち)
- 現在のスライス: レガシー監査完了 → Pipeline設計 + Phase A クロージング
- 今回変更した対象: レガシー19件削除、docs/FEATURE_STATUS_AUDIT.md新規、docs/runtime-state.md新規、spec-index.json(EN-010追加)、VerificationMenu.cs修正、EditorBuildSettings修正
- 次回最初に確認すべきファイル: docs/FEATURE_STATUS_AUDIT.md, docs/runtime-state.md, docs/project-context.md
- 未確定の設計論点: 最終成果物の出力形態、Pipeline全体像、自動化範囲
- 今は触らない範囲: Runtime Core新機能、Ch3シナリオ、マネタイズ
