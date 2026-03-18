# SP-018: 進捗可視化基盤 (Progress Visibility Foundation)

Status: partial
Phase: 1 (MVP)

## 概要

プレイヤーの「理解度・進捗」を直感的に可視化する基盤。
ダッシュボード上部にコンパクトなサマリーバー+ヒント誘導を表示する。

## 背景

体験逆算監査 (2026-03-18) で、以下の3つの空白が最終体験に欠けていると判明:
1. **進捗の可視化**: プレイヤーは HC 数値以外に自分の進捗を知る手段がない
2. **次の行動の誘導**: 何をすべきかのヒントが不在
3. **チャプター間の接続**: Ch1の成果がCh2でどう活きるかの可視化が不在

SP-018 は 1. と 2. をカバーする。3. は Phase 2 以降。

## データモデル: ProgressSnapshot

ProgressTracker が既存マネージャーから集約して返す構造体。

| フィールド | 型 | 取得元 |
|-----------|-----|--------|
| ChaptersTotal / Completed / InProgress | int | ChannelData SO + SaveData |
| ContradictionsTotal / Found | int | ContradictionManager |
| FragmentsTotal / Collected | int | Resources/Topics/fragment_*.asset + DeductionBoard |
| TopicsTotal / Unlocked | int | Resources/Topics/*.asset + DeductionBoard |
| HalluciCoin | int | ContradictionManager |
| OverallPercent | float | 加重平均 (Ch 35% + Cont 30% + Frag 20% + Topic 15%) |

分母はテスト/デバッグ用 (ch_etk, ch_test, debug_*, etk_*) を除外して動的取得。

## 表示: ProgressSummaryUI

ダッシュボードのタイトル+サブタイトルの下、タブバーの上に配置。

```
[=========>        ] 34%
Ch 1/2  |  Cont 3/8  |  Frag 4/7
"Long-press a message to begin contradiction detection."
```

- プログレスバー: 高さ6px、ゴールドカラー (HC表示と統一)
- ミニ数値: 分子/分母をコンパクト表示
- ヒント行: NudgeSystem の結果をイタリック・低コントラストで表示

更新タイミング: DashboardController.Show() 呼び出し時。

## 誘導: NudgeSystem

優先度順の条件テーブルからヒント文を返す pure logic。

| 優先 | 条件 | テキスト |
|------|------|---------|
| 1 | 未開始 | "Start by selecting a channel above." |
| 2 | 矛盾未発見 & プレイ中 | "Long-press a message to begin contradiction detection." |
| 3 | 矛盾発見済み & 断片未収集 | "Check your Inventory for collected fragments." |
| 4 | HCゲート不足 | "Collect {N} more HC to unlock {channel}." |
| 5 | 次チャプター利用可能 | "A new channel is available." |
| 6 | 未発見矛盾あり | "There are still undiscovered contradictions." |
| 7 | 90%以上 | "Almost there. Review what you've found." |
| 8 | フォールバック | "Continue exploring the channels." |

## ファイル構成

| ファイル | 種別 | 責務 |
|---------|------|------|
| Assets/Scripts/Core/ProgressTracker.cs | 新規 | データ集約 + ProgressSnapshot 計算 |
| Assets/Scripts/Core/NudgeSystem.cs | 新規 | ヒント文生成 (static class) |
| Assets/Scripts/UI/ProgressSummaryUI.cs | 新規 | ダッシュボード内 UI |
| Assets/Scripts/UI/DashboardController.cs | 変更 | 統合 (Build + Show) |
| Assets/Scripts/Core/ContradictionManager.cs | 変更 | TotalPairCount / DiscoveredCount プロパティ追加 |

## Phase 分割

### Phase 1 (MVP) — 実装済み
- ProgressTracker + ProgressSnapshot
- NudgeSystem (全8条件)
- ProgressSummaryUI (バー + 数値 + ヒント)
- DashboardController 統合

### Phase 2 (Ch3設計後)
- ChannelData に IsDebugChannel フラグ追加
- チャプター間接続の可視化
- サブスレッド進捗をスナップショットに追加

### Phase 3 (フルUI)
- Inventory 内に Progress サブタブ
- チャプター相関図
- 累積プレイ時間

## 検証方法

1. Unity Editor で ContentAuthoring シーン再生
2. ダッシュボード表示 → プログレスバーと数値が 0/N で表示
3. Ch1 開始 → 矛盾発見 → ダッシュボード復帰 → 数値更新確認
4. NudgeSystem のヒント文が状況に応じて変化すること
