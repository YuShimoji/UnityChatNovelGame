# Scenario Authoring Guide --- FoundPhone シナリオ執筆ハンズオン

> **対象**: このプロジェクトで初めて Yarn スクリプトを書く人（自分自身を含む）
> **前提知識**: Yarn Spinner の基本構文を知らなくても OK。このガイドだけで書ける
> **関連ドキュメント**: [YarnEditingPipeline.md](YarnEditingPipeline.md)（技術的なパイプライン詳細）、[ENGINE_FEATURE_INVENTORY.md](ENGINE_FEATURE_INVENTORY.md)（エンジン機能リファレンス）
> **最終更新**: 2026-03-12

---

## 0. 5分クイックスタート

### 手順

1. `Assets/Resources/Yarn/active/` に `Test_MyScene.yarn` を新規作成
2. 以下をコピペして保存:

```yarn
title: Test_Hello
---
<<SystemMessage "テスト開始">>
<<StartWait 1>>

<<set $speaker to "pyramid">>
こんにちは。テストメッセージです。

<<StartWait 0.8>>

<<set $speaker to "player">>
-> 返事をする
    <<set $speaker to "pyramid">>
    <<StartWait 0.5>>
    返事を受け取りました。
-> 無視する
    <<set $speaker to "pyramid">>
    <<StartWait 0.5>>
    ...。
===
```

3. Unity に切り替える（Alt+Tab）。自動インポートされる
4. `ContentAuthoring` シーンを開く
5. Hierarchy の `DialogueSystem` > ScenarioManager Inspector で Start Node を `Test_Hello` に変更
6. Play ボタンを押す

画面にチャットバブルが表示されれば成功。

---

## 1. Yarn ファイルの基本構造

### ファイル = ノードの集まり

```yarn
title: ノード名
---
ここにセリフや演出を書く
===

title: 次のノード
---
別のシーンの内容
===
```

- 1つの `.yarn` ファイルに複数ノードを書ける
- `title:` と `===` で1ノードを囲む。これがないとコンパイルエラー
- ノード間は `<<jump ノード名>>` で遷移する

### ファイル配置

```
Assets/Resources/Yarn/active/
  Ch1_Day1.yarn      ← 第1章 Day 1
  Ch1_Day2.yarn      ← 第1章 Day 2
  Ch1_Day3.yarn      ← 第1章 Day 3
  DebugScript.yarn   ← デバッグ用
```

`Assets/Resources/Yarn/active/` 内に `.yarn` ファイルを置くだけでコンパイル対象になる。
旧モックは `Assets/Resources/Yarn/archive/` に移動すれば除外される。

---

## 2. コマンドリファレンス（全10コマンド）

### メッセージ系

| コマンド | 構文 | 説明 |
|----------|------|------|
| (直接セリフ) | `テキスト` | `$speaker` に設定されたキャラのバブルとして表示 |
| Message | `<<Message "charID" "テキスト">>` | 指定キャラのメッセージバブルを直接表示 |
| (矛盾タグ) | `テキスト #line:タグ名` | **推奨方式**: Yarn標準の `#line:` タグで矛盾識別子を付与。例: `対象地域は... #line:ch1_region_identity_src` |
| ~~MessageTagged~~ | `<<MessageTagged "charID" "テキスト" "lineTag">>` | 非推奨（互換のため残存）。`#line:` タグ方式を使うこと |
| SystemMessage | `<<SystemMessage "テキスト">>` | 中央寄せのシステム通知（接続/切断/断片獲得等） |
| Image | `<<Image "charID" "imageID">>` | 画像メッセージ（`Resources/Images/` 内の画像） |

### 演出系

| コマンド | 構文 | 説明 |
|----------|------|------|
| StartWait | `<<StartWait 秒数>>` | 待機 + タイピングインジケーター表示 |
| SkipWait | `<<SkipWait>>` | 待機をキャンセル |
| Typing | `<<Typing true>>` / `<<Typing false>>` | タイピングインジケーターを手動で表示/非表示 |
| Glitch | `<<Glitch レベル>>` | グリッチ演出（1-5段階。1=薄いノイズ、5=最大強度） |

### ゲームシステム系

| コマンド | 構文 | 説明 |
|----------|------|------|
| UnlockTopic | `<<UnlockTopic "topicID">>` | 断片/トピックをインベントリに追加 |
| EndDay | `<<EndDay 日数>>` | Day 終了。システムメッセージ表示 + Day進捗記録 + オートセーブ。最終Dayのみチャンネル完了 |

### Yarn 標準機能

| 機能 | 構文 | 説明 |
|------|------|------|
| 変数宣言 | `<<declare $変数名 = 初期値>>` | bool/string/float。ファイルの最初のノードで宣言 |
| 変数設定 | `<<set $変数名 to 値>>` | 変数の値を変更 |
| 条件分岐 | `<<if $条件>> ... <<endif>>` | 変数による条件分岐 |
| 選択肢 | `-> 選択肢テキスト` | プレイヤーの選択肢を表示 |
| ジャンプ | `<<jump ノード名>>` | 別ノードへ遷移 |
| コメント | `// コメント` | 実行されない。メモ用 |
| line tag | `テキスト #line:タグ名` | 行にタグを付与（矛盾ペア参照用） |

---

## 3. 発言者の切り替え

### 基本パターン: $speaker 変数

```yarn
<<set $speaker to "pyramid">>
最初のセリフ。
2行目も pyramid のまま。

<<set $speaker to "marco">>
ここから marco。
```

- `$speaker` を変更するまで同じキャラが話し続ける
- セリフの直前に設定すること（離れた場所で設定すると読みづらい）

### 登録済みキャラクター ID

| ID | 表示名 | 色 | 種別 | 備考 |
|----|--------|-----|------|------|
| `player` | あなた | 紺 (0.2, 0.35, 0.55) | プレイヤー | バブル右寄せ。名前非表示 |
| `pyramid` | Pyramid | 緑 (0.55, 0.82, 0.6) | AI | DisplayMode=IconOnly |
| `marco` | Marco Gross | オレンジ (0.9, 0.55, 0.3) | NPC人間 | |
| `bernardo` | Bernardo Fonseca | 紫 (0.55, 0.4, 0.75) | NPC人間 | |
| `mason` | Mason | 茶 (0.6, 0.5, 0.35) | NPC人間 | |
| `oliver` | Oliver | 青 (0.35, 0.65, 0.85) | NPC人間 | |
| `unknown` | 不明な連絡先 | グレー (0.85, 0.85, 0.85) | 不明 | |

### 選択肢はプレイヤーメッセージとして自動表示される

```yarn
<<set $speaker to "pyramid">>
どちらにしますか？

<<set $speaker to "player">>
-> Aにする
    <<jump NodeA>>
-> Bにする
    <<jump NodeB>>
```

選択後、選んだテキスト（「Aにする」等）がプレイヤーのチャットバブルとして自動で追加される。
ジャンプ先でプレイヤーのセリフをエコーする必要はない。

---

## 4. テンポと演出のパターン

### 会話のテンポ

```yarn
<<set $speaker to "pyramid">>
最初の発言。           ← 表示

<<StartWait 1>>         ← 1秒待機（タイピングインジケーター表示）

続きの発言。            ← 待機後に表示

<<StartWait 0.5>>       ← 短い間

さらに続き。
```

**テンポのガイドライン**:
- 長い発言の後: `<<StartWait 1>>` 〜 `<<StartWait 2>>`
- 短い返事の後: `<<StartWait 0.3>>` 〜 `<<StartWait 0.8>>`
- 発言者交代時: `<<StartWait 0.5>>` 〜 `<<StartWait 1>>`
- 場面転換時: `<<StartWait 1.5>>` 〜 `<<StartWait 2>>`

### 端末の不調を演出する

```yarn
<<set $speaker to "pyramid">>
次の議題に移りま

<<Glitch 1>>
<<StartWait 0.3>>

<<SystemMessage "【SYSTEM】接続が不安定です">>
<<StartWait 1.5>>

<<set $speaker to "pyramid">>
...失礼しました。次の議題に移ります。
```

Glitch レベルの目安:
- 1: 日常的なノイズ（Ch1-3で多用）
- 2-3: 明確な異常（Ch4-6）
- 4-5: 深刻な障害（Ch7-9）

### Pyramid の長考を演出する（Typing の手動制御）

```yarn
<<set $speaker to "pyramid">>
<<Typing true>>
<<StartWait 2>>        ← 2秒間「入力中...」が表示される
<<Typing false>>

...検索結果が見つかりません。
```

通常の `<<StartWait>>` でもタイピングインジケーターは自動表示されるが、
`<<Typing>>` を手動で使うと StartWait より前にインジケーターを出したり、
表示タイミングを細かく制御できる。

---

## 5. コンテンツパターン集

### パターン A: 断片の発見

```yarn
// --- 断片発見シーン ---
<<set $speaker to "marco">>
昨日、端末に変なテキストが表示されてた。

<<StartWait 1>>

<<set $speaker to "marco">>
「...第3次改定に基づき、本施設は管理局の管轄下にある...」

<<StartWait 0.5>>

途中で切れてる。参照先も欠けてる。

<<set $speaker to "player">>
-> そのテキスト、まだ残ってるか？
-> 何の文書だ？

<<set $speaker to "marco">>
<<StartWait 0.8>>
残ってる。こういうのは取っておく癖がついた。

<<UnlockTopic "fragment_ch1_01">>
<<SystemMessage "断片「施設管理規約（部分）」を記録しました">>
```

**ポイント**:
- `<<UnlockTopic>>` の ID は `fragment_ch{章番号}_{連番}` 形式
- SystemMessage で獲得を通知（プレイヤーへのフィードバック）
- 断片の内容（本文テキスト）は会話中にキャラが読み上げる形で提示

### パターン B: キャラクターの合流

```yarn
<<StartWait 1.5>>

<<SystemMessage "新しい参加者が接続しました">>

<<set $speaker to "bernardo">>
<<Typing true>>
<<StartWait 1.5>>
<<Typing false>>
失礼。Bernardo Fonsecaです。接続に手間取りました。

<<set $speaker to "marco">>
<<StartWait 0.5>>
来たか。Barnaby、紹介するよ。Bernardoは文書を読むのが得意な人だ。
```

**ポイント**:
- SystemMessage で接続通知を出してから発言
- 初登場時は `<<Typing true>>` で長めの「入力中」を演出（新しい人が慎重に書いている感覚）
- 既存キャラによる紹介を挟む

### パターン C: Day の終わり（Pyramid の「補足」パターン）

```yarn
// --- 全員退出後のPyramid補足 ---
<<set $speaker to "pyramid">>
本日のセッションを終了します。

<<StartWait 0.5>>

<<SystemMessage "Marco Gross が切断しました">>
<<StartWait 1>>
<<SystemMessage "Bernardo Fonseca が切断しました">>

<<StartWait 2>>

<<set $speaker to "pyramid">>
Barnabyさん、1点補足があります。

<<StartWait 0.5>>

// ここに矛盾を深める情報を追加する
先ほどの断片について再検索したところ、該当する文書が確認できませんでした。

<<StartWait 1.5>>

<<SystemMessage "...">>
<<StartWait 2>>

<<EndDay 1>>
```

**ポイント**:
- 他のキャラが切断した後、Pyramid が 1対1 でプレイヤーに話しかける
- 「補足」の体裁で矛盾情報や不穏な事実を伝える
- 最後に `<<EndDay N>>` で Day を終了

### パターン D: 矛盾の種まき（Ch1 --- メカニクスなし）

```yarn
<<set $speaker to "pyramid">>
本プログラムの対象地域は、第4管理区域です。 #line:ch1_region_identity_src

<<set $speaker to "marco">>
<<StartWait 0.5>>
第4管理区域。聞いたことあるか？ #line:ch1_region_identity_tgt
```

**ポイント**:
- `#line:タグ名` を行末に付与する（Pyramidのソース側に `_src`、人間のターゲット側に `_tgt`）
- Ch1 では矛盾指摘メカニクスは発火しないが、タグは付けておく
- プレイヤーは「おかしいな」と感じるだけ（指摘する手段がない）

### パターン E: 矛盾指摘（Ch2以降 --- メカニクスあり）

```yarn
// #line: タグを使う方法（推奨）
<<set $speaker to "pyramid">>
この地域の正式名称は「東部統合区」です。 #line:ch2_location_east_src

<<StartWait 0.8>>

<<set $speaker to "marco">>
統合区？ 俺の住所には「西区」と書いてあるが。 #line:ch2_location_east_tgt
```

`#line:` タグを付与したメッセージは矛盾指摘の対象になる。
プレイヤーが長押し+タップで矛盾を指摘できる。
`ChatDialogueView` が `TextID` として自動取得し、`MessageBubble.LineTag` に伝播する。

---

## 6. 命名規則

### ファイル名

```
Ch{章番号}_Day{日番号}.yarn      ← 本編（推奨）
Ch{章番号}_{シーン名}.yarn       ← 旧形式（既存ファイルに残存）
Test_{名前}.yarn                 ← テスト用
```

### ノード名

```
Ch1_Day1_Opening          ← 章_日_シーン（推奨形式）
Ch1_Day1_MarcoArrival
Ch1_Day1_FragmentDiscovery
Ch1_Day1_DayEnd
```

1ノード = 1シーン（10-30メッセージ目安）。
分岐点では必ずノードを分割する。

### 変数名

```yarn
<<declare $met_marco = false>>        ← キャラ遭遇フラグ
<<declare $fragment_found = false>>   ← 断片発見フラグ
<<declare $trust_level = 0>>          ← 数値パラメータ
```

- 変数宣言は各 Yarn ファイルの最初のノードにまとめる
- プロジェクト内で変数名が重複するとエラーになる
- `$speaker` は予約変数（発言者の切り替えに使用）

### トピック/断片 ID

```
fragment_ch1_01    ← 断片（インベントリの Fragments タブに表示）
fragment_ch1_02
topic_found_phone  ← トピック（Topics タブに表示）
record_ch1_log     ← レコード（Records タブ。将来拡張）
```

プレフィックスでインベントリの表示タブが自動分類される。

---

## 7. ノード設計のコツ

### セーブ復元を意識する

ノードの先頭がセーブ復元ポイントになる。復帰時に文脈がわかるようにする:

```yarn
title: Ch1_Day2_BernardoAnalysis
---
// 復帰時にも文脈がわかる
<<SystemMessage "--- 2日目・ベルナルドの分析 ---">>
<<set $speaker to "bernardo">>
昨日の断片を見せてもらった。
===
```

### 選択肢の設計

選択肢は「世界を変える」選択ではなく「何を聞くか」「どう反応するか」が基本:

```yarn
// 良い例: プレイヤーの態度を表現
-> 自分も外に出てみたい
-> 断片をもっと集めよう
-> Pyramid は本当に信用できるのか

// 避けるべき例: ゲームの大局を決定してしまう
-> AI を全員シャットダウンする
-> この場を去る
```

### インライン展開 vs ノード分割

短い反応はインラインで書く（ノードを増やしすぎない）:

```yarn
<<set $speaker to "player">>
-> 聞いたことがない
    <<set $speaker to "marco">>
    <<StartWait 0.5>>
    だよな。誰も知らない。
-> 詳しく教えてくれ
    <<set $speaker to "marco">>
    <<StartWait 0.5>>
    俺もよくわからないんだが...

// インライン展開の後に合流して続行
<<set $speaker to "pyramid">>
<<StartWait 1>>
補足いたします。
```

長い分岐（5メッセージ以上）はノード分割:

```yarn
-> 詳しく聞く
    <<jump Ch1_Day1_DetailedExplanation>>
-> 後にする
    <<jump Ch1_Day1_SkipExplanation>>
```

---

## 8. テストワークフロー

### 方法 A: Play from Node（推奨）

1. `ContentAuthoring` シーンを開く
2. Hierarchy > `DialogueSystem` を選択
3. ScenarioManager Inspector の **Start Node** ドロップダウンでノード選択
4. **Play from Node** ボタン → 自動で Play Mode に入り、選択ノードから再生

### 方法 B: Debug Hub（F12）

1. Play Mode 中に **F12** キーを押す
2. ノード一覧が表示される（チャプター別、ストーリー順ソート）
3. ノードをクリック → 即座に再生

### 方法 C: 早送りモード（F11）

- Play Mode 中に **F11** で早送りトグル
- 全ての StartWait / タイプライター効果がスキップされる
- 選択肢は通常通り操作する
- 長いシーンを素早く確認したい時に使う

### テストチェックリスト

- [ ] 全ノードが Play from Node で再生できる
- [ ] 全選択肢の分岐を通した
- [ ] バブルの色・名前が正しいキャラに対応している
- [ ] UnlockTopic で断片が記録される
- [ ] EndDay で Day が終了する
- [ ] Console にコンパイルエラーがない

---

## 9. よくある間違いと対処法

### `title:` や `===` を忘れた

```
// エラー: ノードの区切りがない
Error: Unexpected token
```

全てのノードは `title: 名前` で始まり `===` で終わる。

### 変数宣言が重複した

```
// エラー: 同じ変数を2箇所で declare している
Error: Variable $met_marco has already been declared
```

`<<declare>>` はプロジェクト全体で1回だけ。別ファイルで同じ変数名を宣言しない。

### 選択肢の後にセリフを直接書いた

```yarn
// 間違い: 選択肢の後にインデントなしでセリフ
-> 選択肢A
-> 選択肢B
続きのセリフ     ← これが選択肢Bの一部として解釈されてしまう

// 正しい: インデント合わせ
-> 選択肢A
    <<set $speaker to "pyramid">>
    応答A
-> 選択肢B
    <<set $speaker to "pyramid">>
    応答B
```

### `$speaker` を設定し忘れた

バブルの色がおかしい場合、直前の `$speaker` 設定を確認する。
ノード冒頭で必ず `$speaker` を再設定すること（前ノードの状態は引き継がれるが、
セーブ復元でどのノードから再開するかわからないため）。

### SystemMessage の括弧

```yarn
// 間違い: 引用符なし
<<SystemMessage 接続しました>>

// 正しい: 引用符で囲む
<<SystemMessage "接続しました">>
```

全てのコマンドの文字列引数は `""` で囲む。

---

## 10. キャラクターの声（執筆参考）

### Pyramid（AI アシスタント）

- **文体**: 敬体。正確に聞こえるが根拠が曖昧
- **特徴**: 長い。丁寧だが的外れ。自信ありげに矛盾することを言う
- **口調例**:
  - 「本プログラムの対象地域は、第4管理区域に分類されています」
  - 「失礼しました。一時的に利用できないようです」
  - 「なお、先ほどの件について補足がございます」
- **注意**: Pyramid は「嘘をつく」のではなく「正確な情報を持っていない状態で正確に話す」

### Marco Gross（実地の知恵）

- **文体**: 口語。ツッコミ役。皮肉を混ぜる
- **特徴**: Pyramid の形式主義を嫌う。実体験ベースの判断
- **口調例**:
  - 「3回目の接続で、やっとだ」
  - 「聞いたことあるか？」
  - 「まあ、いい。Pyramid に正確な情報を期待するのは無理か」
- **注意**: Marco は不満を言うが、状況を受け入れてもいる。絶望ではなく諦観

### Bernardo Fonseca（断片分析師）

- **文体**: 文語寄り。正確。テキストを読む時だけ異様に饒舌
- **特徴**: 元新聞社の校正者/編集者。文体の違いを見抜く能力
- **口調例**:
  - 「この文体は行政文書だが、書式が半端だ」
  - 「改定があるということは、第1次と第2次が存在するはず」
  - 「まだわからない。だがパターンが見えてきた」
- **注意**: 断定を避ける。常に「材料が足りない」と留保する

### Mason（偵察者）

- **文体**: 簡潔。事実だけ伝える。感情的な解釈を避ける
- **特徴**: 外に出て物理的な情報を集める行動派
- **口調例**:
  - 「掲示板に紙が一枚貼ってあった」
  - 「看板の地名が違っていた」
  - 「明日、もう少し遠くまで行ってみる」
- **注意**: 短い文で話す。「なぜ」は聞かない。「何があったか」だけ報告する

### Oliver（安定の錨）--- Ch2 から登場

- **文体**: 穏やか。調停的
- **特徴**: 感情の安定装置。対立を緩和する
- **口調例**: （Ch2 執筆時に具体化）
- **注意**: 存在感は控えめだが、いないと空気が違う

---

## 付録: 新チャプター追加チェックリスト

1. `.yarn` ファイルを `Assets/Resources/Yarn/active/` に作成
2. `Assets/Resources/Channels/` に ChannelData アセットを追加/更新
   - Unity メニュー: `Create > Project FoundPhone > Channel Data`
   - ChannelID, DisplayName, StartNodeName, ChapterNumber を設定
   - RequiredCompletedChannelID で前提チャンネルを指定
3. 新キャラがいる場合: `Create > Project FoundPhone > Character Profile`
4. 新断片がある場合: `Create > Project FoundPhone > Topic Data`
   - TopicID のプレフィックスで分類が決まる（`fragment_` / `topic_` / `record_`）
5. Unity Console にエラーがないことを確認
6. Play from Node + F11早送り で全分岐を通す
