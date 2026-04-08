# SP-017: サブスレッド解放トリガー仕様

**Status**: DRAFT
**Category**: core / authoring
**Related**: SP-014 (インタラクション・メカニクス), SP-016 (サブスレッドUI), SP-099 sec G (決定済み)

---

## 1. 概要

サブスレッド (A型/B型/C型/Branch型) の出現タイミングを制御するトリガー条件の仕様。
SP-099 sec G で「複合トリガー方式」が決定済み。本文書はその実装仕様を定義する。

## 2. 決定済み事項 (SP-099 sec G, 2026-03-18)

3条件ORの複合トリガー方式を採用:

| トリガー条件 | 実装手段 | 状態 |
|-------------|---------|------|
| HC閾値 | `ChannelData.RequiredHalluciCoin` | 既存 (ChannelData SO) |
| ストーリー進行 | `ChannelData.RequiredCompletedChannelID` / `CompletedChannelIDs` | 既存 (SaveData) |
| 断片収集 | Yarn変数 `$has_topic_fragment_*` → `DeclareThreadLatentCond` | 既存 (Yarn変数条件) |

- 新規エンジン機能は不要。既存の `DeclareThreadLatentCond` + `ChannelData` SO の組み合わせでカバー
- オーサリング側で3条件を組み合わせて使う

## 3. 既存実装

### DeclareThreadLatentCond (Yarn コマンド)
```yarn
<<DeclareThreadLatentCond "thread_id" "type" "display_name" "$yarn_variable_condition">>
```
- Yarn変数が `true` になったときに自動顕在化
- Branch型の場合、`AutoBeginBranch` で自動分岐開始

### ChannelData (ScriptableObject)
- `RequiredHalluciCoin`: HC閾値によるチャンネル解放
- `RequiredCompletedChannelID`: 前章クリアによるチャンネル解放

## 4. 未実装・未設計 (HUMAN_AUTHORITY)

以下は仕様策定にユーザー判断が必要:

- [ ] 複合条件 (AND/OR) の Yarn 記法パターン集
- [ ] トピック/断片/HC閾値による複合トリガーのオーサリングガイド追記
- [ ] Ch3 以降の具体的なトリガー設定 (各章のスレッド解放条件)
- [ ] 解放通知 UI の演出仕様 (新スレッド出現時のフィードバック)
- [ ] `ManifestThread` 呼び出しタイミングの設計指針

## 5. オーサリングパターン (案)

### パターン A: Yarn変数条件 (断片収集)
```yarn
<<DeclareThreadLatentCond "annot_theory" "A" "理論体系" "$has_topic_fragment_theory">>
// → $has_topic_fragment_theory が true になったときに出現
```

### パターン B: ストーリー進行 (章クリア)
```
// ChannelData SO の RequiredCompletedChannelID で制御
// Yarn 側の記述は不要
```

### パターン C: HC閾値
```
// ChannelData SO の RequiredHalluciCoin で制御
// Yarn 側の記述は不要
```

### パターン D: 複合条件 (AND)
```yarn
// Yarn変数を仲介変数として使う
<<set $trigger_combo to ($has_topic_fragment_x and $ch2_completed)>>
<<DeclareThreadLatentCond "complex_thread" "B" "複合条件スレッド" "$trigger_combo">>
```

---

## 6. Ch1 具体例（SP-022 副次）

`Ch1_Day1.yarn` で実際に使っているパターン。複合条件の本格運用前の参照用。実機での解放タイミング差分は [docs/verification/2026-04-10-subsequent-completion-report.md](../verification/2026-04-10-subsequent-completion-report.md) 実行後に追記する。

| 目的 | 実装 |
|------|------|
| 断片トピック取得後に B 型スレッドを自動顕在化 | `<<DeclareThreadLatentCond "ch1_cond_analysis" "B" "断片分析レポート" "$has_topic_fragment_ch1_01">>`（Day2 Opening） |
| 断片共有のタイミングで A 型を Manifest + メッセージ注入 | `<<ManifestThread "ch1_note_facility">>` + `<<AddThreadMessage ...>>`（Day1 Fragment） |
| 任意サブクエスト（C/A）の開始〜完了 | `<<DeclareThreadTyped "…" "C" "表示名">>` または型 A 同様 → `<<AddThreadMessage …>>` → `<<CompleteThread "…">>`（SP-022 パイロット） |
| Day3 で本線トピックに埋め込む C/A（Latent→Manifest） | Day3 Opening で `DeclareThreadLatent`、各スポークで `ManifestThread` + `AddThreadChat` / `AddThreadMessage`、Winding で `CompleteThread`（`scout_ch1_d3_*` / `annot_ch1_d3_compare`） |

HC 閾値・前章クリアのみの解放は ChannelData 側（パターン B/C）で足りる場合、Yarn に条件行を書かない。

---

*本文書は SP-099 sec G の決定を独立仕様として分離したもの。詳細設計はユーザー承認後に拡充する。*
