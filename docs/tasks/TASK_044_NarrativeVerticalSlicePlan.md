# Task: Narrative Vertical Slice Plan (Mock Story Playable)

Status: READY
Tier: 1 (Planning/Execution Guide)
Branch: main
Owner: Worker
Created: 2026-02-09
Updated: 2026-02-09
Report: docs/reports/REPORT_TASK_044_NarrativeVerticalSlice.md (to be created on completion)

## Objective

次回以降の実装を、以下の一区切りまで到達するように段階化する。

- 区切り条件: テキストを編集し、Unity上で簡単なモックストーリーを実際にプレイできる。

## Milestone Definition

- M1 (Vertical Slice):
  - `Assets/Resources/Yarn/` のシナリオテキストを編集して、開始ノードから終了まで進行できる。
  - 分岐（2択以上）を1回含む。
  - `ChatDialogueView` と `ChatController` 経由で行表示・選択肢表示が機能する。
  - PlayMode で `DebugChatScene` から完走できる。

## Phase Breakdown

### Phase 0: Baseline Lock (0.5 day)
- 目的: 直近の修正を固定し、次工程での差分混入を防ぐ。
- 作業:
  1. 現在の compile error 解消状態をコミット。
  2. `DebugChatScene` の実行確認ログを残す。
  3. シーン起動手順を `docs/reports/` に短く記録。
- DoD:
  - `main` で再現可能な起動手順が1つに確定している。

### Phase 1: Narrative Engine Validation (0.5 day)
- 目的: Yarn Spinner と Ink のどちらを SSOT にするか判断を固定する。
- 比較対象:
  - Yarn Spinner for Unity
  - ink-unity-integration
- 判定軸:
  1. Unity UI 連携の容易さ
  2. コマンド拡張（Message/Image/Typing/Status）適合
  3. チーム運用（非エンジニアがテキスト編集可能か）
  4. 現行資産との互換性（既存 `ScenarioManager`/`ChatDialogueView`）
- DoD:
  - 本タスク末尾の「Decision」が確定し、以降の実装でブレない。

### Phase 2: Story Authoring Pipeline (1 day)
- 目的: モックストーリー制作を短サイクルで回せるようにする。
- 作業:
  1. Yarnファイル命名規則を定義（例: `CH01_*.yarn`）。
  2. 開始ノード規約（例: `Start`）を定義。
  3. テキスト差し替え手順（編集→Import→Play）を `docs/` に明記。
- DoD:
  - 新規メンバーが手順書のみで1本の短編を差し替え可能。

### Phase 3: Mock Story Implementation (1-2 days)
- 目的: 実際にプレイ可能な最小ストーリーを作る。
- 作業:
  1. 10-20行程度の会話を Yarn で実装。
  2. 選択肢1回、分岐2ルート、どちらも終端ノードへ到達させる。
  3. 最低1つの演出コマンド（Typing or SystemMessage）を利用。
- DoD:
  - M1 の区切り条件を満たす。

### Phase 4: Verification + Hand-off (0.5 day)
- 目的: 以降拡張できる品質で一区切りを確定する。
- 作業:
  1. PlayMode で通し確認。
  2. 既知制約（未実装コマンド等）をレポート化。
  3. 次スプリントの入口タスクを3件起票。
- DoD:
  - `docs/reports/REPORT_TASK_044_NarrativeVerticalSlice.md` が作成済み。

## Decision: Yarn Spinner vs Ink (as of 2026-02-09)

### Project Fit Conclusion
- 採用継続: Yarn Spinner（現行プロジェクトの目的により適合）

### Why
1. 既存実装が Yarn 前提で構築済み（`ScenarioManager`, `ChatDialogueView`, Yarn custom command 設計）。
2. チャットゲーム向けの「会話行 + コマンド」の分離が現設計と一致。
3. Ink へ移行すると、既存コマンド群と実行パイプラインの再設計コストが高い。
4. まずは M1（モックプレイ）達成が最優先で、エンジン移行はクリティカルパス外。

### Version Notes
- 現在の `Packages/manifest.json` は `dev.yarnspinner.unity` を `v2.5.1` に固定。
- Yarn Spinner Unity latest release: `v3.1.1` (published 2025-12-04).
- ink-unity-integration latest release: `1.2.1` (published 2024-07-31).
- 方針:
  - M1 達成までは現行 Yarn を維持。
  - M1 後に Yarn 3.1.x へのアップグレード検証を別タスク化。

## Source Links (engine validation)
- https://github.com/YarnSpinnerTool/YarnSpinner-Unity/releases/latest
- https://github.com/inkle/ink-unity-integration/releases/latest
- https://github.com/inkle/ink

## Risks and Controls
- リスク: Yarn バージョン差異による API 変化。
  - 対策: M1 後に専用アップグレードタスクを分離。
- リスク: シナリオ方式の二重化（Yarn + ScriptableObject）。
  - 対策: M1範囲では Yarn を SSOT とし、SO は補助データ用途に限定。

## Immediate Next Tasks (ordered)
1. TASK_044-A: Mock Story Yarn 原稿作成（10-20行、2分岐）
2. TASK_044-B: DebugChatScene で開始ノードを固定し、通しプレイ確認
3. TASK_044-C: 実行レポート作成（スクリーンショット + ログ）
