# Quick Start: ストーリーを追加する

## Step 1: Yarn ファイルを作成する

`Assets/Resources/Yarn/active/` に新しい `.yarn` ファイルを作成します。

例: `MyNewScene.yarn`

```yaml
title: MyNewScene_Start
---
<<SystemMessage "シーン開始">>
<<StartWait 1.5>>

<<set $speaker to "pyramid">>
こんにちは。新しいシーンへようこそ。

<<StartWait 0.8>>

少し説明しましょう。

<<set $speaker to "player">>
-> 詳しく聞かせて
    <<set $speaker to "pyramid">>
    <<StartWait 0.5>>
    了解しました。
    <<jump MyNewScene_Detail>>
-> 大丈夫、わかっている
    <<set $speaker to "pyramid">>
    <<StartWait 0.3>>
    それは頼もしい。
===

title: MyNewScene_Detail
---
<<set $speaker to "pyramid">>
<<StartWait 0.8>>

ここからは詳細な説明です。

<<StartWait 0.5>>

何か質問はありますか？

<<set $speaker to "player">>
-> いや、十分だ
    <<set $speaker to "pyramid">>
    了解しました。

<<EndDay 1>>
===
```

## Step 2: Unity で確認する

1. Unity Editor にフォーカスを戻す (Alt+Tab)
2. Yarn ファイルは自動的にインポートされる
3. `ContentAuthoring` シーンを開く
4. Hierarchy で `DialogueSystem` > `ScenarioManager` を選択
5. Inspector の **Start Node** を `MyNewScene_Start` に変更
6. **Play** ボタンを押す

## Step 3: テンポを調整する

### StartWait の目安

| 状況 | 秒数 | 説明 |
|------|------|------|
| 長い発言の後 | 1.0 - 2.0 | 読む時間を確保 |
| 短い返事の後 | 0.3 - 0.8 | テンポよく |
| 発言者交代 | 0.5 - 1.0 | 切り替えの間 |
| 場面転換 | 1.5 - 2.0 | ゆったり |
| 緊迫した場面 | 0.2 - 0.5 | 速いテンポ |

### タップスキップ

プレイヤーは画面をタップすることでメッセージのアニメーションをスキップし、即座に全文を表示できます。F11 キーで早送りモードにすることもできます。

## Step 4: キャラクターを使う

### 既存キャラクター

```yaml
<<set $speaker to "player">>    # プレイヤー (右寄せ、紺色)
<<set $speaker to "pyramid">>   # Pyramid AI (緑色、アイコン表示)
<<set $speaker to "marco">>     # Marco Gross (オレンジ色)
<<set $speaker to "bernardo">>  # Bernardo Fonseca (紫色)
<<set $speaker to "mason">>     # Mason (茶色)
<<set $speaker to "oliver">>    # Oliver (青色)
<<set $speaker to "unknown">>   # 不明な連絡先 (グレー)
```

### 新しいキャラクターを追加する

1. Unity メニュー: `Create > Project FoundPhone > Character Profile`
2. `Resources/Characters/` フォルダに保存
3. Inspector で CharacterID, DisplayName, ThemeColor, Icon を設定
4. Yarn で `<<set $speaker to "新しいID">>` で使用

## Step 5: 新しいチャプターを追加する

### ファイル構成

```
Assets/Resources/Yarn/active/
  Ch4_NewChapter.yarn    # メインストーリー
```

### チャプターの基本構造 (Hub & Spoke)

```yaml
title: Ch4_Opening
---
<<declare $ch4_asked_topic1 = false>>
<<declare $ch4_asked_topic2 = false>>

<<SystemMessage "Chapter 4 開始">>
<<StartWait 2>>

<<set $speaker to "npc_name">>
// 導入の会話...

<<jump Ch4_Hub>>
===

title: Ch4_Hub
---
<<set $speaker to "player">>
-> トピック1について <<if not $ch4_asked_topic1>>
    <<set $ch4_asked_topic1 to true>>
    <<jump Ch4_Topic1>>
-> トピック2について <<if not $ch4_asked_topic2>>
    <<set $ch4_asked_topic2 to true>>
    <<jump Ch4_Topic2>>
-> 今日はここまで <<if $ch4_asked_topic1 and $ch4_asked_topic2>>
    <<jump Ch4_Ending>>
===
```

### 必要な ScriptableObject

新チャプターを追加する際に必要なSO:

1. **ChannelData** -- ダッシュボードのチャンネル定義
2. **TopicData** -- 断片/トピックカード (UnlockTopic で参照)
3. **CharacterProfile** -- 新キャラクター (既存キャラのみなら不要)

> **YarnSOGenerator** (Tools > FoundPhone > Yarn SO Generator) で不足SOを自動検出・生成できます。

## Step 6: 検証する

### YarnContentValidator

Unity メニュー: `Tools > FoundPhone > Yarn Content Validator`

- Yarn ファイル内のコマンド構文エラーを検出
- 未定義の topicID / characterID を報告
- 使用前に必ず実行すること

### Debug Hub (F12)

Play モード中に **F12** を押すとデバッグハブが開きます:

- 全ノード一覧 (チャプター別、ストーリー順)
- ノードクリックで即座にジャンプ
- 任意のシーンから確認可能

### チェックリスト

- [ ] Yarn ファイルが `active/` に配置されている
- [ ] 全ノードに `title:` と `===` がある
- [ ] 変数宣言が最初のノードにある
- [ ] 選択肢にフラグガード (`<<if not $flag>>`) がある
- [ ] EndDay でチャプター終了処理がある
- [ ] YarnContentValidator でエラーなし
- [ ] YarnSOGenerator で不足SOなし
- [ ] ContentAuthoring シーンで再生確認済み
