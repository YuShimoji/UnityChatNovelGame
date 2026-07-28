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
| 2026-04-15 | Ch積み上げでゲーム完成方向へのドリフト防止。エンジン能力マイルストーン中心の進行に切替 | active | docs/REPO_LOCAL_RULES.md / INVARIANTS.md ガードレール、HANDOFF.md ライブ現在地、project-context.md AXIS/ROADMAP、ドリフト検知 |
| 2026-03-30 | なるべくプロジェクト内 docs だけで引き継げる状態にせよ | done | HANDOFF.md 新設 + runtime-state / project-context / OPERATOR_WORKFLOW / prompt-resume.md |
| 2026-06-03 | handoff 時は全コンテキストをプロジェクト内に保持し、local tracked state を remote へ反映して別端末で即再開できるようにする | done | HANDOFF.md / runtime-state.md / project-context.md / git commit + push |
| 2026-06-08 | Codex Thread 開始時に repo-local モデル固定でエラーが出ないようにし、全コンテキストを project-local docs に保持して remote へ反映する | done | `.codex/config.toml` 削除 / INVARIANTS.md / HANDOFF.md / runtime-state.md / git commit + push |
| 2026-06-15 | AI 入口文書を薄いポインタに戻し、repo-local runtime pin と機械固有 local settings を再発防止する | done | AGENTS.md / CLAUDE.md / .claude/CLAUDE.md / docs/REPO_LOCAL_RULES.md / .gitignore |
| 2026-07-20 | FoundPhone静的fixtureをSites-native private reviewへ変換し、非canon表示、両分岐、mobile/a11y、禁止機能、Owner-only/public禁止を維持する | hosted runtime done / Owner review pending | `sites/foundphone-demo/` / `docs/verification/2026-07-20-sites-private-runtime-validation.md` / Sites Version 1 |
| 2026-07-21 | 全コンテキストをproject-local authorityへ保持し、localをremoteへ反映して別端末から即再開可能にする | done | HANDOFF.md / runtime-state.md / project-context.md / verification / git commit + push |
| 2026-07-10 | 監修 AI → 開発 AI の反復を、過剰停止・細切れ Prompt・更新漏れ・創造提案不足・微修正沼が起きない形へ最適化する | active | REPO_LOCAL_RULES / ai/WORKFLOWS_AND_PHASES / ai/PARALLEL_LANE_PROMPTS / ai/DECISION_GATES / INTERACTION_NOTES / HANDOFF |
| 2026-07-10 | リポジトリを開かなくても現在地を確認でき、main に追随して自動更新される閲覧面を持つ | active | MkDocs build は利用可能。GitHub Pages workflow と初回 Pages 設定は未実施 |
| 2026-07-26 | リモート最新をlocalへ取り込み、開発可能性を再検証し、監修役AI向け詳細報告と条件付き目標をproject-localに残す | done | `docs/verification/2026-07-26-development-readiness.md` / `SUPERVISOR_REPORT.md` / `HANDOFF.md` / `runtime-state.md` |
| 2026-07-27 | Sites authoring bridge候補をremoteで再取得可能にし、人間受入・main統合・hosted/public releaseを別ゲートとして引き継ぐ | review-ready / human gate pending | `origin/codex/sites-authoring-bridge-v1@e059e4b` / `docs/HANDOFF.md` / `docs/SUPERVISOR_REPORT.md` |

## 継続して効く是正要求

- UIバグは即修正せず docs/UI_ISSUES.md に溜めて一括処理 (session 17 策定)
- セッション成果物は「プレイアブルなコンテンツ」か「新機能」。UI修正だけのセッションは原則禁止
- handoff 時は `git status` / docs 同期 / commit / push / handoff summary / 次セッション開始手順までを一括で確定する
- done 済み仕様にも改善余地がある。新 Task や ENH を受け容れる構造を維持する (session 21)
- テストをパスさせるために実装を変えない。テストの前提が古いなら、テスト側を現仕様に合わせる (session 21)
- 機能一覧を提示する際、done = 完了済みで終わらせず、仮実装後に欲しい機能 (ENH) の受け入れ余地を見せる
- ガードレール（docs/REPO_LOCAL_RULES.md / INVARIANTS.md）は実行計画（project-context.md CURRENT AXIS）より上位 (2026-04-15 策定、2026-06-15 入口文書を薄型化)
- セッション成果の第一指標は「エンジン/ツール能力の前進」。コンテンツのみ 2 セッション連続はドリフト警告 (2026-04-15 策定)
- SUBSEQUENT はスキップ不可の通過ゲート。エンジン能力確認なしにフルコンテンツ執筆に進まない (2026-04-15 策定)
- 可逆な実装は関連修正・局所検証・HANDOFF 更新まで自走し、高手戻りの主観判断だけを実装前の比較ゲートで 1 回確認する (2026-07-10 策定)
- 外部 status は別 Wiki へ手動複製せず、同じ Markdown 正本から自動公開する。公開機構がない状態を「更新漏れ」で運用回避しない (2026-07-10 策定)

## Backlog Delta (spec-index 登録済み)

| ID | 内容 | 優先度 | 予定 |
|----|------|--------|------|
| BL-001 | スクロール吸着フェードイン (LateUpdate ピンニング部分) | 低 | UIバッチ修正時 |
| BL-002 | スレッド/キャラクターポートレートアイコン画像 | 中 | M7 (製品化) |
| BL-003 | スレッド柔軟メタデータ (難易度星等) | 低 | 中期以降 |

## 運用ルール

- 会話で一度出た要求のうち、次回以降も効くものをここへ残す
- 単なる感想ではなく、仕様・設計・backlog に効くものを優先する
