# 分岐スレッド (Branch Thread)

## 概要

Branch Thread は、メイン会話から一時的に分岐して別の分析/視点を提供し、その後メインに復帰する仕組みです。

## 基本パターン

```yaml
// Hub ノードから分岐へ誘導
-> 分析を聞く <<if not $did_branch_analysis>>
    <<set $did_branch_analysis to true>>
    <<jump AnalysisBranch>>
```

```yaml
title: AnalysisBranch
---
<<set $speaker to "npc">>
<<StartWait 0.5>>

分析の導入...

<<set $speaker to "player">>
-> 詳しく聞かせて
    <<BeginBranch "branch_analysis" "分析">>
    <<set $speaker to "npc">>
    <<StartWait 0.8>>
    ここからは分岐スレッドです。
    // 分析の内容...
    <<UnlockTopic "topic_finding">>
    <<EndBranch true "select">>
-> 今はいい
    // 分岐に入らない

<<jump Hub>>
===
```

## ライフサイクル

```
1. BeginBranch → 分岐スレッドに自動切替 (画面クリア + 空状態)
2. メッセージが分岐スレッドに蓄積
3. UnlockTopic で TransferFlags に知識を登録
4. EndBranch → メイン復帰
   - "select" モード: 知識転送選択UI表示 → プレイヤーが選択
   - 通常モード: 全知識を自動反映
5. 反映メッセージがメインに投入
6. メイン会話が続行
```

## 設計ルール

### 1. 再入防止フラグは必須

```yaml
// 必須: フラグで再入を防止
<<set $did_branch_xxx to true>>
<<BeginBranch ...>>
```

フラグがないと、Hub に戻った後に何度でも分岐に入れてしまいます。

### 2. メタ発言を避ける

「これは別のスレッドです」のようなメタ発言は不要。画面の切り替えで分岐は視覚的に示されます。

### 3. select モードは2件以上の知識がある場合に使う

```yaml
// 2件以上: select で選択させる
<<UnlockTopic "topic_a">>
<<UnlockTopic "topic_b">>
<<EndBranch true "select">>

// 1件のみ: select なしで自動反映
<<UnlockTopic "topic_a">>
<<EndBranch true>>
```

### 4. 反映メッセージ

EndBranch 後にメインに投入されるメッセージ:

1. `SetBranchReflection` で手動設定した場合 → そのテキスト
2. 未設定で UnlockTopic がある場合 → トピック名から自動生成
3. いずれもない場合 → なし

```yaml
// 手動設定
<<SetBranchReflection "重要な手がかりを得た">>
<<EndBranch true>>
```

## 分岐 ID の命名規則

```
ch{章番号}_branch_{内容}

例:
ch1_branch_analysis
ch2_branch_location_check
ch3_branch_document_review
```

## 既知の制限

- **分岐のネスト不可**: BeginBranch の中で別の BeginBranch は呼べない
- **セーブ復元**: 分岐内でセーブ → ロードすると分岐状態は失われる (Yarn 変数は保持)
- **サイドバー**: 分岐スレッドは紫色で表示。完了後もエントリは残る
