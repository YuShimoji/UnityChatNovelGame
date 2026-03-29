# Yarn コマンドリファレンス

FoundPhone エンジンで使用可能な全カスタムコマンドと Yarn 標準機能の一覧。

---

## メッセージ系

### 通常メッセージ (直接セリフ)

```yaml
<<set $speaker to "pyramid">>
ここに書いたテキストがそのまま表示されます。
```

`$speaker` 変数に設定されたキャラクターのバブルとして表示。

### Message

```yaml
<<Message "charID" "テキスト">>
```

$speaker を変更せずに特定キャラクターのメッセージを表示。

### SystemMessage

```yaml
<<SystemMessage "テキスト">>
```

中央寄せのシステム通知。接続/切断/断片獲得などの演出に使用。

### Image

```yaml
<<Image "charID" "imageID">>
```

画像メッセージを表示。`Resources/Images/` 内の画像を参照。

### 矛盾タグ (#line:)

```yaml
本プログラムの対象地域は、第4管理区域に分類されています。 #line:ch1_region_identity_src
```

Yarn 標準の `#line:` タグで矛盾指摘システムの識別子を付与。

---

## 演出系

### StartWait

```yaml
<<StartWait 秒数>>
```

指定秒数の待機。待機中はタイピングインジケーター (... アニメーション) が表示される。

**テンポの目安**: 長文後 1.0-2.0s / 短文後 0.3-0.8s / 発言者交代 0.5-1.0s

### SkipWait

```yaml
<<SkipWait>>
```

実行中の StartWait をキャンセル。

### Typing

```yaml
<<Typing true>>
<<Typing false>>
```

タイピングインジケーターを手動制御。StartWait より細かいタイミング制御が必要な場合に使用。

### Glitch

```yaml
<<Glitch レベル>>
```

グリッチ演出。レベル 1-5:

| Lv | 効果 |
|----|------|
| 1 | 薄いノイズ (Ch1-3 で多用) |
| 2 | ノイズ + 軽い色収差 |
| 3 | 強い色収差 + 画面揺れ (明確な異常) |
| 4 | Lv3 の強化版 |
| 5 | 最大強度 (データモッシュ的) |

---

## ゲームシステム系

### UnlockTopic

```yaml
<<UnlockTopic "topicID">>
```

トピックカードをインベントリに追加。`$has_topic_{topicID}` 変数が自動で `true` に設定される。

### EndDay

```yaml
<<EndDay 日数>>
```

Day 終了処理:
- 通常 Day: 「--- N日目 終了 ---」システムメッセージ
- 最終 Day: チャプター完了サマリー (チャプター名 + 断片/矛盾/HC数値)
- Day 進捗記録 + 強制オートセーブ

### AddHalluciCoin

```yaml
<<AddHalluciCoin 量>>
```

HalluciCoin を静かに加算。通知なし、ダッシュボードバッジパルスで間接的に検知される。

### DiscoverFragment

```yaml
<<DiscoverFragment "topicId" "threadId" "message">>
```

断片発見の一括実行。以下を1コマンドで処理:
1. `UnlockTopic` でトピック追加
2. `SystemMessage` で「断片「{title}」を記録」表示
3. `ManifestThread` でスレッドを顕在化
4. `AddThreadMessage` でスレッドにメモ追加

---

## スレッド系

### DeclareThread / DeclareThreadTyped

```yaml
<<DeclareThread "threadId" "displayName">>
<<DeclareThreadTyped "threadId" "type" "displayName">>
```

サブスレッドを宣言。メインチャットに通知 + サイドバーにエントリ追加。

型 (type):

| 型 | 用途 | サイドバー色 |
|----|------|-------------|
| A | 注釈・覚書 | 青 |
| B | 追跡ログ | 緑 |
| C | 偵察報告 | オレンジ |
| branch | 分岐分析 | 紫 |

### DeclareThreadLatent / DeclareThreadLatentCond

```yaml
<<DeclareThreadLatent "id" "type" "name">>
<<DeclareThreadLatentCond "id" "type" "name" "$condition">>
```

スレッドを潜在登録 (サイドバーに表示しない)。`ManifestThread` で顕在化するか、条件が真になると自動顕在化。

### AddThreadMessage / AddThreadChat

```yaml
<<AddThreadMessage "threadId" "テキスト">>
<<AddThreadChat "threadId" "charID" "テキスト">>
```

指定スレッドにメッセージを追加 (メイン画面には非表示)。

### ManifestThread / CompleteThread

```yaml
<<ManifestThread "id">>
<<CompleteThread "id">>
```

潜在スレッドの顕在化 / スレッドの完了。

### BeginBranch / EndBranch

```yaml
<<BeginBranch "branchId" "displayName">>
// 分岐内のメッセージ
<<EndBranch true|false ["select"]>>
```

分岐スレッドの開始と終了。詳細は [分岐スレッド (Branch)](branch.md) を参照。

### SetBranchReflection

```yaml
<<SetBranchReflection "テキスト">>
```

EndBranch 時にメインに投入する反映メッセージを手動設定。未設定時は分岐内の UnlockTopic から自動生成。

### AddFragmentNote

```yaml
<<AddFragmentNote "threadId" "message">>
```

スレッドへの断片関連メモ追加。AddThreadMessage のセマンティックエイリアス。

---

## Yarn 標準機能

### 変数

```yaml
<<declare $変数名 = 初期値>>    // bool/string/float 宣言
<<set $変数名 to 値>>           // 値の変更
```

### 条件分岐

```yaml
<<if $条件>>
  // 真の場合
<<elseif $別条件>>
  // 別条件が真
<<else>>
  // それ以外
<<endif>>
```

### 選択肢

```yaml
-> 選択肢テキスト
    // 選択時の処理
-> 条件付き選択肢 <<if $条件>>
    // 条件が真の場合のみ表示
```

選択テキストはプレイヤーのチャットバブルとして自動表示される。

### ジャンプ

```yaml
<<jump ノード名>>
```

### コメント

```yaml
// この行は実行されない
```
