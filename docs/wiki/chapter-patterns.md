# チャプター設計パターン

## Hub & Spoke (ハブ&スポーク)

最も基本的なパターン。中央のハブノードからトピックに分岐し、消化後にハブに戻る。

```
Opening → Hub ←→ Topic1
               ←→ Topic2
               ←→ Topic3
          Hub → Winding → End
```

### 実装例 (Ch1_Day1)

```yaml
title: Ch1_Opening
---
// 変数宣言 + 導入
<<jump Ch1_Hub>>
===

title: Ch1_Hub
---
<<set $speaker to "player">>
-> トピック1 <<if not $asked_topic1>>
    <<set $asked_topic1 to true>>
    <<jump Ch1_Topic1>>
-> トピック2 <<if not $asked_topic2>>
    <<set $asked_topic2 to true>>
    <<jump Ch1_Topic2>>
-> 今日はここまで <<if $asked_topic1 and $asked_topic2>>
    <<jump Ch1_Ending>>
===
```

### ポイント

- 各トピックに `$asked_xxx` フラグを設定して消化を追跡
- 全トピック消化後に終了選択肢を表示
- 分岐スレッドも Hub から誘導 (再入防止フラグ必須)
- `<<jump Ch1_Hub>>` でトピック完了後にハブへ戻る

## Linear (線形)

一本道のストーリー。選択肢はあるが大きな分岐はない。

```
Scene1 → Scene2 → Scene3 → End
```

## Branch & Merge (分岐&合流)

大きな分岐があるが最終的に合流する。

```
Hub → BranchA → MergePoint
    → BranchB → MergePoint
```

## Day 構造

各チャプターは複数の Day で構成。Day 内は Hub & Spoke。

```
Ch1_Day1_Opening → Ch1_Day1_Hub → Ch1_Day1_End
  <<EndDay 1>>
Ch1_Day2_Opening → Ch1_Day2_Hub → Ch1_Day2_End
  <<EndDay 2>>
Ch1_Day3_Opening → Ch1_Day3_Hub → Ch1_Day3_End
  <<EndDay 3>>  # 最終Day: チャプター完了サマリー
```

## 演出パターン

### 端末不調

```yaml
<<set $speaker to "npc">>
次の議題に移りま

<<Glitch 1>>
<<StartWait 0.3>>

<<SystemMessage "【SYSTEM】接続が不安定です">>
<<StartWait 1.5>>

<<set $speaker to "npc">>
...失礼しました。次の議題に移ります。
```

### 断片発見

```yaml
<<DiscoverFragment "fragment_ch1_01" "ch1_note_facility" "施設管理規約の断片を発見">>
<<StartWait 1>>
<<set $speaker to "npc">>
これは...興味深い文書ですね。
```

### 分岐から戻った後

```yaml
// Hub に戻ったら、分岐での発見に反応
<<if $has_topic_suspicious_message>>
    <<set $speaker to "npc">>
    先ほどの分析は興味深いものでしたね。
<<endif>>
```
