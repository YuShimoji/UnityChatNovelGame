# Ch3 設計根拠 — なぜ今 Ch3 なのか

Status: DRAFT
作成日: 2026-03-18

## 前提

Ch1/Ch2 はエンジン検証用モックとして機能している (`docs/REPO_LOCAL_RULES.md` / `docs/INVARIANTS.md` の開発境界)。
Ch3 も同様に「コンテンツ制作」ではなく「エンジン基盤の未検証領域を埋める検証媒体」として位置づける。

## Ch1/Ch2 を深化させる案 vs Ch3 に進む案

### Ch1/Ch2 深化のメリットと限界

- メリット: 既存コンテンツの品質向上、手動検証の完了度向上
- 限界: **新しいエンジン能力の検証にならない**。カバー済みの機能パターンを反復するだけ

### Ch3 に進む意義 (= Ch3 でしか検証できないもの)

| 検証対象 | なぜ Ch3 が必要 |
|----------|----------------|
| チャプター間接続体験 | Ch1→Ch2 はHCゲートで接続済み。しかし「Ch1/Ch2 の知識が Ch3 でどう活きるか」は3つ目がないと確認不可。ProgressTracker の OverallPercent が意味を持つには最低3チャプターが必要 |
| DiscoverFragment 実運用 | ETK モックでは不十分。実ストーリー文脈で使って初めてオーサリング摩擦の削減度が測れる |
| SP-099 目安の実践検証 | 「断片3/ch + スレッド2-3/ch」がソロ開発で持続可能かは Ch3 を1本作ってみないとわからない |
| 複合トリガー | HC + 進行 + 断片のOR条件を実チャプターで検証。DeclareThreadLatentCond の実用パターンを確認 |
| ProgressTracker の分母拡大 | Ch2 までは OverallPercent が 0〜50% 付近で推移。Ch3 追加で 0〜100% の全域を検証可能 |

## Ch3 のストーリー上の役割 (SP-003 より)

- テーマ: 地名混在が「仕様」である可能性に触れる (層更新B)
- キャラ: ベルナルドが断片の文体分析を披露。制度の形跡を指摘
- 断片: 制度文書風だが参照先が消えている (不可索引物の予感)
- 演出: ハルシコインが「いつの間にか増えている」初回
- 位置づけ: 第1部の末章。情報密度のピーク。第2部への引き

## Ch3 で検証するエンジン能力 (リファレンス。アクティブ TODO ではない)

- DiscoverFragment コマンドの実運用 (最低2回使用)
- 複合トリガー (DeclareThreadLatentCond で $has_topic 条件)
- B型追跡スレッドの充実 (3件以上のメッセージ蓄積)
- ProgressTracker の数値変化 (Ch1+Ch2+Ch3 で OverallPercent が意味ある変化を示す)
- チャプター間接続: Ch1/Ch2 の断片が Ch3 の Yarn 分岐条件 ($has_topic_fragment_*) に影響
- CompleteThread の自然な使用 (Ch3 Day3 終了時)
- NudgeSystem のヒント文遷移 (Ch2完了→Ch3開始→Ch3途中→Ch3完了)

## Ch3 の制約

- 断片: 3個 (SP-099 決定)
- サブスレッド: 2-3個 (SP-099 決定)
- 矛盾ペア: 3-4個 (Ch1: 4, Ch2: 3 の中間)
- Day数: 3日 (SP-003 「3節=3日」)
- 登場キャラ: 既存6名 (新キャラは第2部以降)

## Unity Editor セットアップ手順

### 1. ChannelData (ch3.asset)
- `Tools > FoundPhone > Create Default Channel Data` を実行
- ch3.asset が自動生成される (ChannelDataCreator.cs に定義済み)
- Inspector で RequiredHalluciCoin を設定 (推奨: 5)

### 2. TopicData (3件)
以下を `Assets/Resources/Topics/` に手動作成 (Create > ScriptableObject > TopicData):

| TopicID | Title | Description |
|---------|-------|-------------|
| fragment_ch3_01 | 文書分析報告 | 施設管理規約と行政区画変遷記録の参照先が体系的に欠損。意図的削除の可能性 |
| fragment_ch3_02 | 管理区域移行計画書（案） | 承認印黒塗り。施設管理規約と同一書式 — 同一発行元の証拠 |
| fragment_ch3_03 | 管理区域統合運用規則（暫定版） | 第1条で公開索引からの除外を明記。不可索引物の制度的根拠 |

### 3. ContradictionPair (3件)
ContradictionDatabase SO の Inspector で m_Pairs に以下を追加:

| ContradictionID | SourceLineTag | TargetLineTag | Chapter | RewardCoin | Difficulty |
|----------------|---------------|---------------|---------|------------|------------|
| ch3_doc_style | ch3_doc_style_src | ch3_doc_style_tgt | 3 | 1 | 2 |
| ch3_reference_gap | ch3_reference_gap_src | ch3_reference_gap_tgt | 3 | 1 | 2 |
| ch3_closure_date | ch3_closure_date_src | ch3_closure_date_tgt | 3 | 1 | 2 |

## やらないこと

- 本番クオリティのストーリー執筆 (モック品質で十分)
- サウンド/ビジュアル (SP-009 未着手)
- 主人公裏切りUI (Ch6 以降)
- アルケミーボード (凍結中)
