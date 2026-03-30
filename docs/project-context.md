# Project Context

## PROJECT CONTEXT

- プロジェクト名: FoundPhone (UnityChatNovelGame)
- 環境: Unity 6.3 LTS (6000.3.6f1) / C# / Yarn Spinner 3.1.3 / DOTween
- ブランチ戦略: trunk-based (main のみ)
- 現フェーズ: プロトタイプ → α移行中
- 直近の状態 (2026-03-30 session 18):
  - Session 17: 自動スキップ修正 + DialogueException修正 + フォントバランス調整 + ロードマップ策定 + canonical docs 初期化
  - Session 18: 制作パイプライン改善。YarnSOGenerator が ChannelData 同期まで対応、Content Pipeline window 追加、handoff docs 整備
  - 方向転換: AI は Yarn 執筆ではなく制作ツール/パイプライン整備に注力すべき
  - 次の作業: 人間が執筆するためのシステム周りの実機検証 (DebugQuickTest / Ch1-Ch3 / E2E)

### 運用メモ

- 現在の系列: UI基盤統合 (UIFontConfig) + メッセージ演出一貫性修正
- ユーザーはデザイナー兼ライター。手動でのストーリー追加がまだ未実施 — wiki で解消予定
- nightshift の変更品質が問題化。部分的・不完全な変更が検証負担を増大させるパターン。完成度優先へ
- スレッド管理 (BeginBranch/EndBranch) がユーザーに複雑と指摘された — PLAN MODE でリファクタ設計要
- task-scout 指摘の残件: verification/ 空、E2E 自動検証未整備

---

## CURRENT DEVELOPMENT AXIS

- 主軸: コンテンツ制作フロー実証 + Ch1 完走
- この軸を優先する理由: エンジン基盤は alpha として十分。Session 13-17 が UI 微修正に費やされ、コンテンツ進行が停止。制作フローを実際に回してコンテンツを前進させる
- 今ここで避けるべき脱線: UI 微修正ループ、マネタイズ実装、サウンド統合、過度な仕様策定
- **ワークフロー原則**: 値の調整 (フォント/色/タイミング) は Inspector で行い、コード変更しない。UI バグは docs/UI_ISSUES.md に溜めて一括修正。セッション成果物は「プレイアブルなコンテンツ」か「新機能」

---

## CURRENT LANE

- 主レーン: Unlock (制作パイプラインの実運用確認)
- 副レーン: Audit (DebugQuickTest / Ch1-Ch3 再生確認)
- 今このレーンを優先する理由: 制作ツール側の欠落は埋まったため、次は実際にその導線が摩擦なく回るか確認する段階
- いまは深入りしないレーン: UI微調整 (Inspector で自律調整)、サウンド、マネタイズ

---

## CURRENT SLICE

- スライス名: 制作パイプライン運用実証 + docs handoff 完全化
- ユーザー操作列: Yarn 編集 → Content Pipeline で同期 → DQT_Start 確認 → Ch1/Ch2/Ch3 再生 → 問題記録
- 成功状態: 会話ログなしでも docs だけで現状把握でき、制作フローを Unity 上で再現できる
- このスライスで必要な基盤能力: タップスキップ (済)、タイミング (済)、wiki (済)、Validator (済)、SOGenerator (済)
- 今回はやらないこと: UI微修正 (UI_ISSUES.md に記録のみ)、サウンド、マネタイズ

---

## DEVELOPMENT ROADMAP (2026-03-30 策定)

### 短期 (Session 18-20): Ch1 完走 + 制作フロー実証
- S18: Ch1 Day1 通しプレイ + Day2 執筆開始 + ツール実証
- S19: Ch1 Day2 完成 + Day1→Day2 遷移テスト
- S20: Ch1 Day3 完成 + Ch1 通しプレイ + SP-019/020 Phase 1 検証

### 中期 (Session 21-28): Alpha ビルド (Ch1-2 完走可能)
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

| 工程 | 手動/自動 | ツール | 状態 |
|------|-----------|--------|------|
| シナリオ設計 | 手動 | SCENARIO_AUTHORING_GUIDE | done |
| Yarn執筆 | 手動 | VSCode + Yarn Spinner Extension | done |
| 静的バリデーション | 自動 | YarnContentValidator (Editor) | done |
| SO自動生成 | 自動 | YarnSOGenerator + Content Pipeline (Topic/Character/Channel 同期) | **done** |
| Unity再生確認 | 手動 | ContentAuthoring シーン | done |
| E2E自動検証 | 自動 | 未実装 — ETK拡張でPlayModeテスト | **todo** |
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

## HANDOFF SNAPSHOT (session 18)

- 現在の主レーン: Unlock (制作パイプラインの実運用確認)
- 現在のスライス: Content Pipeline / ChannelData 自動同期 / handoff docs 整備
- session 17-18 で完了した作業:
  - 自動スキップバグ根絶 (NextContentToken リーク修正)
  - DialogueException 修正 (m_IsDestroying ガード)
  - DebugQuickTest.yarn 新規 (DQT_Start)
  - フォントサイズバランス修正 (.asset nightshift膨張値 + 全体底上げ)
  - 開発ロードマップ策定 (短期/中期/長期)
  - canonical docs 4本初期化 (INVARIANTS/LEDGER/WORKFLOW/INTERACTION)
  - YarnSOGenerator 拡張: ChannelData 同期まで対応
  - Content Pipeline window 追加
  - StartNode 推奨導線への統一
  - handoff docs 整備: HANDOFF.md 追加
- session 17-18 で判明した重要な認識修正:
  - Yarn 執筆はユーザーの仕事。AI はシステム/ツール/パイプラインを整備する
  - 「前回の反動」で方向を決めない。本当に必要なものを特定する
- 次回最初にやること:
  1. Content Pipeline の Unity 実機確認
  2. DQT_Start / Ch1 / Ch2 / Ch3 再生確認
  3. E2E PlayMode 検証のスコープ切り出し
- フォント/色/タイミングの微調整: Inspector の ChatUIConfig / UIFontConfig で自律的に行う (コード変更不要)
  - タップスキップ: システムメッセージに効かない (一貫性の欠如)
  - 全般: nightshift の雑な変更パターン。完成度優先へ方針転換
- 未確定の設計論点:
  - UIFontConfig の値調整 (Inspector で全UI一括。現在はデフォルト=旧ハードコード値)
  - ThreadSwitcherController のフォント統合 (サイドバー密レイアウト、別途設計要)
  - スレッド管理リファクタリングの方向性 (PLAN MODE)
  - ポートレート画像挿入の UI/UX 設計 (HUMAN_AUTHORITY)
- 今は触らない範囲: サウンド統合、マネタイズ実装、E2E自動検証
- task-scout 指摘の未対応:
  - active/ の Yarn 整理候補 (VerticalSlice.yarn, FirstSlice.yarn, MVPTest.yarn)
  - CanvasScaler 不整合 (DebugChatScene + MetaEffectController が 1920x1080)
