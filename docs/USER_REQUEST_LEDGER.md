# User Request Ledger

ユーザーの継続要望・差分要求・backlog を保持する台帳。

## 現在有効な要求

| 日付 | 要求 | 状態 | 反映先 |
|------|------|------|--------|
| 2026-03-30 | 設計と開発ワークフローを構造化せよ | done | project-context.md ROADMAP, INVARIANTS.md |
| 2026-03-30 | 短期・中期・長期の開発想定を立て直せ | done | project-context.md DEVELOPMENT ROADMAP |
| 2026-03-30 | デバッグはデバッグ用チャプターで即確認できるようにせよ | done | DebugQuickTest.yarn (DQT_Start) |
| 2026-03-30 | 処理のアルゴリズムを明文化せよ | done | docs/DISPLAY_ALGORITHMS.md (EN-011) |
| 2026-03-29 | デザイナーが容易にストーリー追加削除できるワークフロー確立 | done | Content Pipeline window + YarnSOGenerator の ChannelData 同期 + 推奨 StartNode 導線 |
| 2026-03-30 | 最も欲しいのは人間が執筆するためのシステム周り | active | パイプライン導線は実装済み。残りは Unity 実機検証と E2E 自動検証 |
| 2026-03-30 | なるべくプロジェクト内 docs だけで引き継げる状態にせよ | done | HANDOFF.md 新設 + runtime-state / project-context / OPERATOR_WORKFLOW / prompt-resume.md |

## 未反映の是正要求

- UIバグは即修正せず docs/UI_ISSUES.md に溜めて一括処理 (session 17 策定)
- セッション成果物は「プレイアブルなコンテンツ」か「新機能」。UI修正だけのセッションは原則禁止
- handoff 時は `git status` / docs 同期 / commit / push / handoff summary / 次セッション開始手順までを一括で確定する
- done 済み仕様にも改善余地がある。新 Task や ENH を受け容れる構造を維持する (session 21)
- テストをパスさせるために実装を変えない。テストの前提が古いなら、テスト側を現仕様に合わせる (session 21)
- 機能一覧を提示する際、done = 完了済みで終わらせず、仮実装後に欲しい機能 (ENH) の受け入れ余地を見せる

## Backlog Delta (spec-index 登録済み)

| ID | 内容 | 優先度 | 予定 |
|----|------|--------|------|
| BL-001 | スクロール吸着フェードイン (LateUpdate ピンニング部分) | 低 | UIバッチ修正時 |
| BL-002 | スレッド/キャラクターポートレートアイコン画像 | 中 | 中期 S23 |
| BL-003 | スレッド柔軟メタデータ (難易度星等) | 低 | 中期以降 |

## 運用ルール

- 会話で一度出た要求のうち、次回以降も効くものをここへ残す
- 単なる感想ではなく、仕様・設計・backlog に効くものを優先する
