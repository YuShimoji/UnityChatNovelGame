# Project Context

## PROJECT CONTEXT

- プロジェクト名: FoundPhone (UnityChatNovelGame)
- 環境: Unity 6.3 LTS (6000.3.6f1) / C# / Yarn Spinner 3.1.3 / DOTween
- ブランチ戦略: trunk-based (main のみ)
- 現フェーズ: プロトタイプ → α移行中
- 直近の状態 (2026-03-30 session 14):
  - Pipeline設計確定 + SO自動生成ツール (YarnSOGenerator) 実装済み
  - エンジン基盤: 24 Yarnコマンド + タップスキップ + タイミング設定可能
  - spec-index: 35エントリ (done 22 / partial 9 / draft 1 / todo 2 + 21_branch_thread_spec)
  - Authoring Wiki: docs/wiki/ (Docsify 12ページ、ストーリー追加 Quick Start 含む)
  - session 13-14: 選択肢バグ修正、分岐再入防止、メッセージ演出改善、フォントサイズ引き上げ
  - 次の作業: Unity実機確認 → スレッド管理リファクタ設計 → ポートレート画像 → ストーリー追加実証

### 運用メモ

- 現在の系列: メッセージ演出安定化 + コンテンツ制作インフラ整備 (wiki)
- ユーザーはデザイナー兼ライター。手動でのストーリー追加がまだ未実施 — wiki で解消予定
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

- 現在の主レーン: Advance (UI安定化) + Audit (nightshift 変更の実機確認)
- 現在のスライス: ゲームUI安定化 + ストーリー追加実証
- 今回変更した対象 (session 13-14):
  - ChatDialogueView.cs: タップスキップ + タイミング設定可能化
  - ChatController.cs: CompleteCurrentTypewriter + AnimateBubbleIn 復元時スキップ + レスポンシブスケール下限修正
  - ScenarioManager.cs: StopScenario同期化 + StartScenario安全弁 + BeginBranch再入時履歴クリア
  - ChatUIConfig.asset: フォントサイズ引き上げ (28→34等)
  - Ch1_Day1.yarn: 分岐再入防止フラグ + メタ発言削除
  - docs/wiki/ (12ページ新規)
  - docs/StorySpec/21_branch_thread_spec.md (新規)
- 次回最初に確認すべきファイル: Unity ContentAuthoring シーン再生、docs/wiki/ ブラウザ確認
- 未確定の設計論点:
  - フォントサイズ 34 の妥当性 (実機確認後に調整)
  - スレッド管理リファクタリングの方向性 (PLAN MODE)
  - ポートレート画像挿入の UI/UX 設計 (HUMAN_AUTHORITY)
  - StartWait 中のタップスキップ対応 (現在は RunLineAsync 内のみ)
- 今は触らない範囲: サウンド統合、マネタイズ実装、E2E自動検証
- task-scout 指摘の未対応:
  - FEATURE_STATUS_AUDIT.md 未更新 (YarnSOGenerator 等)
  - YarnSOGenerator の spec-index エントリ未登録
  - active/ の Yarn 整理候補 (VerticalSlice.yarn, FirstSlice.yarn, MVPTest.yarn)
  - CanvasScaler 不整合 (DebugChatScene + MetaEffectController が 1920x1080)
