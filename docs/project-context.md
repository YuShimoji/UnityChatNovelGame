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

- 主軸: ゲームUI安定化 + コンテンツ制作ワークフロー実証 + 新機能 (ポートレート画像)
- この軸を優先する理由: エンジン基盤は揃ったが、実際のストーリー追加を一度も手動で実施していない。wiki + ツールで制作フローを回し、摩擦を特定する段階
- 今ここで避けるべき脱線: マネタイズ実装、サウンド統合、過度な仕様策定 (実動作の確認が先)

---

## CURRENT LANE

- 主レーン: Advance (ゲームUI安定化 + メッセージ演出) / Audit (Unity実機確認)
- 副レーン: Unlock (ポートレート画像、スレッド管理リファクタ)
- 今このレーンを優先する理由: nightshift で大量変更 (8コミット) があり、Unity実機確認が必要。その上でストーリー追加の実証テスト
- いまは深入りしないレーン: サウンド統合、マネタイズ実装、E2E自動検証

---

## CURRENT SLICE

- スライス名: ゲームUI安定化 + ストーリー追加実証
- ユーザー操作列: wiki を読む → 新 Yarn ファイルを書く → Validator → SOGenerator → Unity 再生確認 → タップスキップで操作 → 問題なく完走
- 成功状態: デザイナーが wiki だけ見て新チャプターを追加し、Unity で正常に再生できる
- このスライスで必要な基盤能力: タップスキップ (実装済み)、タイミング設定 (実装済み)、wiki (作成済み)
- このスライスから抽出されるツール要求: なし (既存ツールで十分)
- 今回はやらないこと: E2E自動検証、サウンド統合、マネタイズ実装

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

---

## IDEA POOL

CLAUDE.md の IDEA POOL を参照。ここには project-context.md 作成以降のアイデアのみ追記する。

| ID | アイデア | 状態 | 関連領域 | 再訪トリガー |
|----|----------|------|----------|--------------|
| IP-PC-001 | メッセージごとのポートレート画像挿入 | active | ui/新機能 | Unity実機確認完了後。HUMAN_AUTHORITY: インラインアバター拡大 or 独立画像バブル or カットイン？ |
| IP-PC-002 | スレッド管理のシンプル化リファクタリング | active | system/リファクタ | PLAN MODE で設計後。BeginBranch/EndBranch/SwitchToThread の責務整理 |
| IP-PC-003 | StartWait 中のタップスキップ対応 | backlog | system/演出 | 現在は RunLineAsync 内の遅延のみ。StartWait のスキップも要検討 |

---

## HANDOFF SNAPSHOT

- 現在の主レーン: Audit (UIFontConfig統合後の検証) + Advance (タップスキップ一貫性修正)
- 現在のスライス: UIFontConfig統合完了 → フォントバランス調整 + タップスキップ一貫性修正
- 今回変更した対象 (session 15):
  - [新規] Assets/Scripts/Data/UIFontConfig.cs — 7段階フォント階層 + レスポンシブスケール
  - Assets/Scripts/UI/DashboardController.cs — ハードコード→UIFontConfig参照 (10箇所)
  - Assets/Scripts/UI/InventoryTabController.cs — (7箇所)
  - Assets/Scripts/UI/TransferSelectionUI.cs — (5箇所)
  - Assets/Scripts/UI/ProgressSummaryUI.cs — (3箇所)
  - Assets/Scripts/UI/ChatController.cs — (3箇所 + GetResponsiveFontScale委譲)
  - Assets/Scripts/UI/ContradictionFeedbackController.cs — (3箇所)
  - session 14 フォントサイズ変更 (d584aaf, 8835623) を revert
- 次回最初にやること:
  1. Unity 再起動 → ChatUIConfig.asset のキャッシュリフレッシュ確認 (messageFontSize=28 がディスク上は正しい)
  2. ContentAuthoring シーン再生 → フォントサイズが全要素で統一されているか確認
  3. タップスキップの一貫性修正 (複数クリック問題 + システムメッセージ未対応)
- ユーザー報告の未解決問題:
  - フォントサイズ: revert後もメッセージが大きく見える (Unityキャッシュ疑い)
  - タップスキップ: タイミング次第で複数クリック必要
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
