# FoundPhone プロジェクト全体概要

> 最終更新: 2026-04-02 session 21

## プロジェクト概要

- 名称: FoundPhone (UnityChatNovelGame)
- 種別: チャット型ビジュアルノベルゲーム (モバイル向け)
- 環境: Unity 6.3 LTS (6000.3.6f1) / C# / Yarn Spinner 3.1.3 / DOTween
- フェーズ: プロトタイプ → Alpha 移行中
- マネタイズ: F2P + 広告 (未実装)

---

## 現在地サマリ

| 指標 | 値 |
| ---- | --- |
| 実装ファイル (.cs) | 66 |
| テストファイル | 7 (EditMode 4 + PlayMode 3) |
| EditMode テストケース | 75 (73 passed / 2 failed) |
| PlayMode テストケース | 8 (実機未確認) |
| Yarn コマンド | 24 (全実装済み) |
| Yarn ファイル (active) | 6 |
| Yarn ファイル (archive) | 5 |
| spec エントリ | 39 |
| ガジェット/SO | Topic / Character / Channel 自動生成対応 |
| Wiki ページ | 12 (Docsify) |
| TODO/FIXME/HACK | 1 |

---

## 仕様一覧 (spec-index)

### Core (ストーリー/ゲームデザイン)

| ID | タイトル | 状態 | 進捗 | 備考 |
| --- | -------- | ---- | ---- | ---- |
| SP-000 | 概要 (Overview) | done | 100% | 世界観/コアコンセプト |
| SP-001 | ゲームプレイ (GDD) | done | 100% | コアループ/サブスレッド/難易度曲線 |
| SP-002 | ストーリーバイブル | done | 100% | 世界観/テーマ/語り口 |
| SP-003 | チャプタービート | partial | 50% | 27ブロック(9章)。節詳細はCh1のみ |
| SP-003a | 第1章セクションビート | partial | 30% | Ch1 3日間の詳細ビート。DRAFT |
| SP-004 | キャラクター | done | 100% | 6名の人物設定 |
| SP-005 | AIモデル | done | 100% | AI側キャラクター群 |
| SP-007 | 断片 (Fragments) | partial | 65% | コンテンツはCh1-2のみ |
| SP-012 | 参照作品 | done | 100% | 比較軸/インスピレーション |
| SP-013 | 用語集 | done | 100% | 理論語/ゲーム内用語 |
| SP-099 | 未決定事項 | done | 100% | 解決済み41件、未決定0件 |

### System (ゲームシステム/メカニクス)

| ID | タイトル | 状態 | 進捗 | 備考 |
| --- | -------- | ---- | ---- | ---- |
| SP-006 | ハルシコイン | done | 100% | 矛盾発見→HC付与→Dashboard/Save/Load 全実装 |
| SP-009 | サウンド/ビジュアル | todo | 0% | BGM+SEのみ方針。未着手 |
| SP-010 | マネタイズ | todo | 0% | F2P + 広告。未着手 |
| SP-014 | インタラクション/メカニクス | partial | 90% | 矛盾Phase2+Branch Phase1-4。未: クロスリファレンスUI/アルケミーボード |
| SP-017 | サブスレッド解放トリガー | draft | 30% | 複合トリガー方式決定済み。残: Ch3以降の具体設定 |
| SP-019 | チャプター遷移体験 | partial | 50% | Phase 1 実装済み。Phase 2-3 未実装 |
| SP-020 | オンボーディング体験 | partial | 50% | Phase 1 実装済み。Phase 2-3 未実装 |

### UI/UX

| ID | タイトル | 状態 | 進捗 | 備考 |
| --- | -------- | ---- | ---- | ---- |
| SP-008 | UI/UX | partial | 90% | ダッシュボード/チャット/サブスレッド 全実装。未: BGM/SE統合UI |
| SP-016 | サブスレッドUI仕様 | done | 100% | Step1-4全完了。ThreadType/通知/Save/Load |
| SP-018 | 進捗可視化基盤 | partial | 70% | Phase 1 実装済み。Phase 2: チャプター間接続 |
| BL-001 | スクロール吸着フェードイン | todo | 0% | DOTween アニメーション化 |
| BL-002 | ポートレートアイコン画像 | todo | 0% | CharacterProfile SO 拡張 |
| BL-003 | スレッド柔軟メタデータ | todo | 0% | 難易度星/推奨レベル等 |

### Engine/Infra (エンジン/基盤)

| ID | タイトル | 状態 | 進捗 | 備考 |
| --- | -------- | ---- | ---- | ---- |
| EN-001 | エンジン機能リファレンス | done | 100% | 24 Yarn コマンド + 暗黙仕様 |
| EN-002 | ワークフロー SSOT | done | 100% | HANDOFF.md に一本化 |
| EN-003 | セーブシステム | done | 100% | JSON/3スロット+AutoSave+スレッド対応 |
| EN-004 | Yarn 編集パイプライン | done | 100% | active/archive 分離 + Content Pipeline |
| EN-005 | オートセーブ設計 | done | 100% | slot=99, 30秒クールダウン |
| EN-006 | UI 実装仕様 | done | 100% | ChatUIConfig 40パラメータ |
| EN-007 | 仕様決定記録 | done | 100% | 設計判断の記録 |
| EN-008 | MVP テストガイド | done | 100% | 動作確認手順書 |
| EN-009 | シナリオオーサリングガイド | done | 100% | 全24コマンドリファレンス |
| EN-010 | 機能状態監査 | done | 100% | 全機能一覧/未確認/レガシー |
| EN-011 | 表示アルゴリズム仕様 | done | 100% | メッセージ表示/スキップ/スクロール |
| EN-012 | E2E PlayMode 自動検証 | partial | 60% | 8テスト。残: ETK全コマンド/Ch3/Phase A自動化 |
| SP-011 | 制作計画 | partial | 20% | ソロ開発マイルストーン叩き台 |
| SP-015 | Feature Triage | done | 100% | 優先バックログ分類 |

### 集計

| 状態 | 件数 |
| ---- | ---- |
| done | 23 |
| partial | 10 |
| draft | 1 |
| todo | 5 |
| **合計** | **39** |

---

## 実装済み主要機能

| カテゴリ | 機能 | 詳細 |
| -------- | ---- | ---- |
| チャットUI | メッセージ表示 | タイプライター/バブル/リッチテキスト/スクロール吸着 |
| チャットUI | 選択肢 | 分岐選択/自動表示/プレイヤーメッセージ化 |
| チャットUI | サブスレッド | 左サイドバー/ThreadType(A/B/C/Branch)/通知バナー |
| メカニクス | 矛盾指摘 Phase 2 | 長押しハイライト/接続線/通知パネル/HalluciCoin付与 |
| メカニクス | 分岐スレッド | BeginBranch/EndBranch (select/reflect)/知識転送 |
| メカニクス | 潜在スレッド | DeclareThreadLatentCond/リアクティブ評価/自動顕在化 |
| ダッシュボード | チャンネル選択 | Day Resume/HC ゲート/マルチ Day |
| ダッシュボード | インベントリ | 3サブタブ (断片/トピック/メモ) |
| ダッシュボード | 進捗表示 | ProgressTracker/NudgeSystem/ProgressSummaryUI |
| セーブ | 永続化 | 3スロット+AutoSave/チャット履歴/スレッド状態/Yarn変数 |
| エディタ | Content Pipeline | YarnSOGenerator (Topic/Character/Channel 同期) |
| エディタ | 静的検証 | YarnContentValidator |
| テスト | EditMode | 75件 (SaveSystem/Contradiction/Inventory/CoreLogic) |
| テスト | PlayMode | 8件 (SmokeGate 4 + ScenarioFlow 4) |
| テスト | batch 実行 | -executeMethod + NUnit XML 出力 |
| 演出 | タップスキップ | 2段階 (テキスト完了→次メッセージ) |
| 演出 | エッジホバー | Focus モード時のツールバー/章パネル表示 |
| デバッグ | DebugQuickTest | スキップ/インジケーター/選択肢の素早い確認 |
| デバッグ | EngineTestKit | Hub&Spoke 形式の全機能テスト |

---

## 未実装主要機能

| カテゴリ | 機能 | 優先度 | 備考 |
| -------- | ---- | ------ | ---- |
| サウンド | BGM/SE | 中 | SP-009。コンテンツが揃ってから |
| マネタイズ | F2P + 広告 | 低 | SP-010。製品化フェーズ |
| コンテンツ | Ch3 以降 | -- | 人間側作業 |
| UI | チャプター遷移 Phase 2-3 | 中 | SP-019。次Ch解放通知+演出 |
| UI | オンボーディング Phase 2-3 | 中 | SP-020。矛盾チュートリアル+インタラクティブガイド |
| UI | 進捗可視化 Phase 2 | 中 | SP-018。チャプター間接続 |
| メカニクス | アルケミーボード | 低 | SP-014。DeductionBoard 凍結中 |
| メカニクス | ブランチ間クロスリファレンスUI | 低 | SP-014 |
| UI | ポートレートアイコン | 低 | BL-002 |
| UI | スクロールフェードイン | 低 | BL-001 |
| テスト | ETK 全コマンド網羅 | 高 | EN-012 → 90% |
| テスト | Phase A 手動検証自動化 | 高 | 65 項目 |
| ビルド | iOS/Android パイプライン | 低 | 製品化フェーズ |

---

## 既知のテスト問題 (2件)

| テスト | エラー | 診断 |
| ------ | ------ | ---- |
| `Manager_GetDiscoveredList_ReturnsCorrectList` | `DontDestroyOnLoad can only be used in play mode` | EditMode テストから SaveManager.Instance (PlayMode 専用) を呼んでいる。テスト設計の問題 |
| `Manager_ShouldShowHint_ReturnsFalseForChapter4Plus` | `Expected: False, But was: True` | テストは「Ch4+ でヒント非表示」を期待するが、実装は ChannelData ベースのポリシーに移行済み。チャプター番号によるヒント抑制は削除されている。テストの前提が古い |

- 1件目: SaveManager の DontDestroyOnLoad が EditMode で呼べない制約。モック化ではなく、テストを PlayMode に移すか、SaveManager 経由を迂回する設計変更が選択肢
- 2件目: 実装が正しくテストの期待値が古い。ChannelData で EnableHints=false に設定すればヒント非表示にできるが、チャプター番号での一律抑制は現仕様にない

---

## 開発ロードマップ

### 短期 (S22-23): パイプライン証明

| タスク | Actor | 完了条件 |
| ------ | ----- | -------- |
| Inspector で m_StartNode=DQT_Start に変更 | user | シーン保存済み |
| PlayMode テスト8件実行 | user | 8 passed / 0 failed |
| Content Pipeline 実機検証 | shared | Sync → SO 更新確認 |
| DQT / Ch1 / Ch2 通しプレイ | user | Console エラーなし |
| GitHub Actions CI 統合 | assistant | PR ごとに自動テスト |

### 中期 (S24-28): 体験完成 + Phase A クロージング

| タスク | Actor | 優先度 |
| ------ | ----- | ------ |
| Phase A 手動検証 (65項目) | shared | 高 |
| ETK 全24コマンド網羅テスト | assistant | 高 |
| BGM/SE 基盤 (SP-009) | assistant | 中 |
| Progress Phase 2 (SP-018) | assistant | 中 |
| Chapter Transition Phase 2 (SP-019) | assistant | 中 |
| Onboarding Phase 2 (SP-020) | assistant | 中 |

### 長期 (S29+): 製品化

| タスク | Actor |
| ------ | ----- |
| Ch3 以降コンテンツ | user |
| マネタイズ (SP-010) | shared |
| iOS/Android ビルド | assistant |
| サウンド統合 | assistant |
| Beta テスト → リリース | shared |

---

## コンテンツ制作パイプライン

```
シナリオ設計 → Yarn執筆 → 静的検証 → SO自動生成 → Unity再生確認 → E2E自動検証 → 調整 → ビルド → 配布
  [手動]       [手動]     [自動]      [自動]        [手動]         [自動/60%]   [手動]  [未設定] [未設定]
```

---

## ドキュメント構成

| ドキュメント | 役割 |
| ------------ | ---- |
| HANDOFF.md | 開発状態の入口。セッション間引き継ぎ |
| project-context.md | 方針/ロードマップ/決定ログ/IDEA POOL |
| runtime-state.md | 現在位置/カウンター/セッションログ |
| INVARIANTS.md | 非交渉条件/UX不変量/禁止解釈 |
| USER_REQUEST_LEDGER.md | 継続要望/backlog/未反映要求 |
| OPERATOR_WORKFLOW.md | 人間の実ワークフロー/痛点 |
| INTERACTION_NOTES.md | 報告UI/手動確認/質問形式 |
| spec-index.json | 仕様エントリ39件のインデックス |
| FEATURE_STATUS_AUDIT.md | 全機能の実装状態監査 |
