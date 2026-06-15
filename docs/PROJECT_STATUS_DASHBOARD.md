# Project Status Dashboard

このページは、プロジェクト全体を短時間で概観するための案内板です。実装・仕様・証跡の正本をここへ移し替えるものではなく、「どこを見れば判断できるか」をまとめます。

## まず見る場所

| 知りたいこと | 最初に見るファイル | そこで分かること | 補助で見る場所 |
|---|---|---|---|
| いま何をしているか | `docs/HANDOFF.md` | 直近の焦点、Safe Next Steps、検証境界 | `docs/runtime-state.md` |
| 現在の開発軸 | `docs/project-context.md` | CURRENT / NEXT / SUBSEQUENT / LATER の流れ | `docs/DEVELOPMENT_TURN_PLAN.md` |
| 実装済み機能 | `docs/FEATURE_STATUS_AUDIT.md` | 実装済み・未確認・未実装の表 | `docs/ENGINE_FEATURE_INVENTORY.md` |
| 今後の新機能 | `docs/FEATURE_REGISTRY.md` | ENH 候補、状態、次フェーズ | `docs/FEATURE_STATUS_AUDIT.md` §4 |
| 項目別の実装内容 | `docs/ENGINE_FEATURE_INVENTORY.md` | Yarn / UI / Save / pipeline などの参照先 | 各仕様ファイル |
| UI / 表示の既知問題 | `docs/UI_ISSUES.md` | バッチ修正対象と fixed 状態 | `docs/INTERACTION_NOTES.md` |
| すぐ見られる画面証跡 | `docs/VISUAL_PROGRESS_INDEX.md` | スクリーンショット配置、鮮度、次に撮る対象 | `Assets/Screenshots/` |
| 翻訳しながら読む | `docs/index.md` | MkDocs 起動手順と注意 | `mkdocs.yml` |

## 概観性の現状

| 観点 | 現在の見え方 | 今回の調整 |
|---|---|---|
| 機能実装の履歴 | `FEATURE_STATUS_AUDIT.md` にまとまっているが入口が分散 | このダッシュボードから直接辿れるようにした |
| 新機能と進捗 | `FEATURE_REGISTRY.md` と `project-context.md` に分かれている | 「今後の新機能」と「開発軸」を分けて案内 |
| 項目別実装 | `ENGINE_FEATURE_INVENTORY.md` が索引、詳細は各仕様へ委譲 | 「項目別の実装内容」の入口として明示 |
| スクリーンショット | `Assets/Screenshots/` にあるが docs 上の索引が弱い | `VISUAL_PROGRESS_INDEX.md` を追加 |
| ターン単位プラン | `CURRENT / NEXT / SUBSEQUENT / LATER` はあるが日付・マイルストーン混在 | `DEVELOPMENT_TURN_PLAN.md` に Turn 表として整理 |

## 機能とドキュメントの対応

| 領域 | 実装・状態を見る | 仕様・使い方を見る | 次の改善を見る |
|---|---|---|---|
| チャット表示 / Yarn コマンド | `docs/FEATURE_STATUS_AUDIT.md` | `docs/YarnEditingPipeline.md` | `docs/FEATURE_REGISTRY.md` |
| サブスレッド / 分岐 | `docs/FEATURE_STATUS_AUDIT.md` | `docs/StorySpec/16_subthread_ui.md`, `docs/StorySpec/21_branch_thread_spec.md` | `docs/project-context.md` |
| Save / Load | `docs/FEATURE_STATUS_AUDIT.md` | `docs/SaveSystem_README.md` | `docs/DEVELOPMENT_TURN_PLAN.md` |
| UI / レイアウト | `docs/UI_IMPLEMENTATION_SPEC.md` | `docs/DISPLAY_ALGORITHMS.md` | `docs/UI_ISSUES.md` |
| 制作パイプライン | `docs/ENGINE_FEATURE_INVENTORY.md` | `docs/SCENARIO_AUTHORING_GUIDE.md`, `docs/YarnEditingPipeline.md` | `docs/OPERATOR_WORKFLOW.md` |
| 表示系デモ | `docs/plans/display-batch-showcase.md` | `docs/StorySpec/23_text_presentation.md`, `docs/StorySpec/24_chat_immersion.md` | `docs/HANDOFF.md` |

## まだ不確実なところ

- `Assets/Screenshots/` の画像は MVP 期の見た目確認として有用だが、SP-023 / SP-024 の最新検収スクリーンショットではない。
- `FEATURE_STATUS_AUDIT.md` の件数は 2026-04-21 基準のため、追加テスト後の全体再スキャンは別ターンで行う。
- `project-context.md` の長期ログは履歴として厚い。日常の進行判断は `HANDOFF.md` と `DEVELOPMENT_TURN_PLAN.md` を優先する。
