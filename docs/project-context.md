# Project Context

## PROJECT CONTEXT

- プロジェクト名: FoundPhone (UnityChatNovelGame)
- 環境: Unity 6.3 LTS (6000.3.6f1) / C# / Yarn Spinner 3.1.3 / DOTween
- ブランチ戦略: trunk-based (main のみ)
- 現フェーズ: プロトタイプ → α移行中
- 直近の状態 (2026-03-30 session 15):
  - UIFontConfig 新設: 7段階フォント階層でUI全体のフォントサイズを一元管理 (31箇所統合)
  - session 14 のフォントサイズ変更は revert 済み (部分的でバランス崩れのため)
  - 未解決: タップスキップの一貫性 (複数クリック必要な場合あり、システムメッセージ未対応)
  - 未解決: revert 後も Unity が旧フォントサイズをキャッシュしている可能性 (再起動で確認要)
  - 次の作業: Unity 再起動して Audit → タップスキップ一貫性修正 → UIFontConfig でフォントバランス調整

### 運用メモ

- 現在の系列: UI基盤統合 (UIFontConfig) + メッセージ演出一貫性修正
- ユーザーはデザイナー兼ライター。手動でのストーリー追加がまだ未実施 — wiki で解消予定
- nightshift の変更品質が問題化。部分的・不完全な変更が検証負担を増大させるパターン。完成度優先へ
- スレッド管理 (BeginBranch/EndBranch) がユーザーに複雑と指摘された — PLAN MODE でリファクタ設計要
- task-scout 指摘: FEATURE_STATUS_AUDIT.md 未更新、YarnSOGenerator spec 未登録、verification/ 空

---

## CURRENT DEVELOPMENT AXIS

- 主軸: コンテンツ制作フロー実証 + Ch1 完走
- この軸を優先する理由: エンジン基盤は alpha として十分。Session 13-17 が UI 微修正に費やされ、コンテンツ進行が停止。制作フローを実際に回してコンテンツを前進させる
- 今ここで避けるべき脱線: UI 微修正ループ、マネタイズ実装、サウンド統合、過度な仕様策定
- **ワークフロー原則**: 値の調整 (フォント/色/タイミング) は Inspector で行い、コード変更しない。UI バグは docs/UI_ISSUES.md に溜めて一括修正。セッション成果物は「プレイアブルなコンテンツ」か「新機能」

---

## CURRENT LANE

- 主レーン: Advance (コンテンツ制作 — Ch1 Day2/Day3 執筆)
- 副レーン: Audit (Ch1 通しプレイ体験確認)
- 今このレーンを優先する理由: 5セッション分の UI 修正が完了。次はコンテンツを実際に作り、制作フローの摩擦を特定する段階
- いまは深入りしないレーン: UI微調整 (Inspector で自律調整)、サウンド、マネタイズ

---

## CURRENT SLICE

- スライス名: Ch1 完走 + 制作フロー実証
- ユーザー操作列: Ch1 Day1 通しプレイ → Day2 ビート確認 → Yarn 執筆 → Validator → SOGenerator → Unity 再生 → Day3 同様 → Ch1 通しプレイ
- 成功状態: Ch1 (Day1-3) を新規プレイヤーとして通しプレイできる。制作フローの摩擦が特定されリスト化されている
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
| SO自動生成 | 自動 | YarnSOGenerator (Editor: Tools > FoundPhone > Yarn SO Generator) | **done** (session 12) |
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
| 2026-03-29 | タップスキップ + タイミング設定可能化 | タップスキップ / F11のみ / 自動送り | VN標準のテキスト送り操作。Inspector で TypingIndicatorDuration(0.8s), PostMessageDelay(0.4s) を調整可能 |
| 2026-03-29 | Branch Thread: Yarn 再入防止フラグ必須 + コード安全策 | フラグ必須 / コードのみ / 両方 | フラグで1回限り + BeginBranch再入時に古い履歴クリア。仕様書 21_branch_thread_spec.md 作成 |
| 2026-03-29 | フォントサイズ: messageFontSize 28→34 + スケール下限 0.78→0.85 | 28維持 / 32 / 34 / 36 | CanvasScaler MatchHeight=1.0 で狭Canvas時のレスポンシブ縮小に耐える。34*0.85=28.9px |
| 2026-03-29 | Authoring Wiki: Docsify ベースで docs/wiki/ に作成 | Docsify / MkDocs / 単一HTML / なし | CDNのみでビルド不要。既存 .md を活かせる。npx docsify serve で即起動 |
| 2026-03-30 | フォントサイズバランス: messageFontSize 28→22 + body 18→20 | 22 / 24 / UIFontConfig全体引き上げ | .asset nightshift膨張値が未revertだった根本原因を修正。Inspector微調整可能 |
| 2026-03-30 | 開発ワークフロー再構造化 | 現状維持 / コンテンツ優先 / UI先行 | 5セッションのUI微修正ループを脱却。値調整はInspector、UIバグは一括処理、セッション成果はコンテンツか機能 |

---

## IDEA POOL

CLAUDE.md の IDEA POOL を参照。ここには project-context.md 作成以降のアイデアのみ追記する。

| ID | アイデア | 状態 | 関連領域 | 再訪トリガー |
|----|----------|------|----------|--------------|
| IP-PC-001 | メッセージごとのポートレート画像挿入 | active | ui/新機能 | Unity実機確認完了後。HUMAN_AUTHORITY: インラインアバター拡大 or 独立画像バブル or カットイン？ |
| IP-PC-002 | スレッド管理のシンプル化リファクタリング | active | system/リファクタ | PLAN MODE で設計後。BeginBranch/EndBranch/SwitchToThread の責務整理 |
| IP-PC-003 | StartWait 中のタップスキップ対応 | backlog | system/演出 | 現在は RunLineAsync 内の遅延のみ。StartWait のスキップも要検討 |

---

## HANDOFF SNAPSHOT (session 17)

- 現在の主レーン: Advance (コンテンツ制作 — Ch1 完走)
- 現在のスライス: Ch1 通しプレイ + 制作フロー実証
- session 17 で完了した作業:
  - 自動スキップバグ根絶 (NextContentToken リーク修正)
  - DialogueException 修正 (m_IsDestroying ガード)
  - DebugQuickTest.yarn 新規 (DQT_Start)
  - フォントサイズバランス修正 (.asset nightshift膨張値 + 全体底上げ)
  - 開発ロードマップ策定 (短期/中期/長期)
  - docs/UI_ISSUES.md 新設 (UIバグ一括処理運用)
- 次回最初にやること:
  1. Ch1 Day1 通しプレイ → 体験として成立するか確認
  2. 発見した UI 問題は UI_ISSUES.md に記録 (即修正しない)
  3. Ch1 Day2 の Yarn 執筆開始 (03a_ch1_section_beats.md の Day2 ビートに基づく)
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
  - FEATURE_STATUS_AUDIT.md 未更新 (YarnSOGenerator 等)
  - YarnSOGenerator の spec-index エントリ未登録
  - active/ の Yarn 整理候補 (VerticalSlice.yarn, FirstSlice.yarn, MVPTest.yarn)
  - CanvasScaler 不整合 (DebugChatScene + MetaEffectController が 1920x1080)
