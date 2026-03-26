# Yarn Editing Pipeline

Windsurf (VS Code) で Yarn ファイルを編集し、Unity で即座に動作確認するための手順書。

**最終更新**: 2026-03-13

---

## 前提条件

- **Unity 6** + YarnSpinner 3.1.3 (`dev.yarnspinner.unity`)
- **Windsurf** (VS Code 系エディタ) + [Yarn Spinner 拡張](https://marketplace.visualstudio.com/items?itemName=SecretLab.yarn-spinner)
- 本プロジェクト: `UnityChatNovelGame`

---

## 1. ファイル配置

Yarn ファイルはディレクトリで管理:

```
Assets/Resources/Yarn/
  Project.yarnproject        ← sourceFiles: ["active/**/*.yarn"]
  active/                    ← コンパイル対象（ランタイムで使用）
    Ch1_Day1.yarn            Chapter 1 Day 1 (ハブ&スポーク型)
    Ch2_LocationConfusion.yarn  Chapter 2
    MVPTest.yarn             MVPテスト用
    FirstSlice.yarn          Act 1 導入シナリオ
    VerticalSlice.yarn       垂直スライスデモ
    DebugScript.yarn         カスタムコマンドテスト用
  archive/                   ← コンパイル対象外（旧モック保管）
    Ch1_Terminal.yarn         旧 Chapter 1 モック
```

`Project.yarnproject` の `sourceFiles` が `active/` 配下のみを対象とする。
新しい `.yarn` ファイルは `active/` に配置すればコンパイル対象になる。
旧モックや不使用ファイルは `archive/` に移動するだけで除外できる。

---

## 2. Windsurf での編集

### 開き方

1. Windsurf でプロジェクトルートを開く
2. `Assets/Resources/Yarn/active/` 内の `.yarn` ファイルを開く
3. Yarn Spinner 拡張による構文ハイライトが有効になる

### ノードグラフ表示

- `Ctrl+Shift+P` → `Yarn Spinner: Show Graph` でノード接続を可視化

### 基本構文

```yarn
title: NodeName
---
<<set $speaker to "unknown">>
ここにセリフを書く。

-> 選択肢A
    <<jump NextNodeA>>
-> 選択肢B
    <<jump NextNodeB>>
===
```

---

## 3. カスタムコマンド一覧

ScenarioManager に登録済みのコマンド:

| コマンド | 用途 | 例 |
| --- | --- | --- |
| `<<set $speaker to "ID">>` | 発言者の切替 | `<<set $speaker to "unknown">>` |
| `<<SystemMessage "text">>` | 画面中央にシステム通知 | `<<SystemMessage "【SYSTEM】通信切断">>` |
| `<<StartWait seconds>>` | 待機 + タイピング演出 | `<<StartWait 1.5>>` |
| `<<SkipWait>>` | 待機をスキップ | `<<SkipWait>>` |
| `<<Message "charID" "text">>` | 直接メッセージ追加 | `<<Message "player" "Hello">>` |
| `<<Image "charID" "imageID">>` | 画像メッセージ | `<<Image "player" "photo_01">>` |
| `<<UnlockTopic "topicID">>` | トピック解放 | `<<UnlockTopic "signal_01">>` |
| `<<Glitch level>>` | グリッチ演出 (1-5) | `<<Glitch 2>>` |
| `<<Typing true\|false>>` | タイピングインジケーター手動制御 | `<<Typing true>>` |
| `テキスト #line:タグ名` | 矛盾タグ付きメッセージ（推奨） | `対象地域は... #line:ch1_region_identity_src` |
| ~~`<<MessageTagged "charID" "text" "tag">>`~~ | 削除済み。上記 `#line:` タグ方式を使うこと | ~~`<<MessageTagged "pyramid" "..." "ch2_src">>`~~ |
| `<<EndDay N>>` | Day 終了 + Day進捗記録 + オートセーブ。最終Dayのみチャンネル完了 | `<<EndDay 1>>` |

---

## 4. シナリオ執筆ワークフロー（推奨手順）

### Step 1: 構成を決める

1. チャプターの**ビート表**を確認（`docs/StorySpec/03_chapter_beats.md`）
2. 登場キャラクター・AIモデルを決定
3. シーンをノード単位に分割（1シーン = 1ノード、10-30メッセージ目安）

### Step 2: ノード設計図を書く

ノード間の遷移をテキストで整理してから .yarn に着手する:

```text
Ch1_Opening → Ch1_PyramidIntro → Ch1_FirstChat
                                       ↓
                               選択肢A → Ch1_AskAboutTerminal
                               選択肢B → Ch1_StayQuiet
                                       ↓
                               Ch1_Contradiction → Ch1_DayEnd
```

### Step 3: .yarn ファイルを作成

```text
Assets/Resources/Yarn/active/Ch1_Day1.yarn
```

ファイル内に複数ノードを記述可能。命名規則:

- `Ch{N}_{シーン名}` （例: `Ch1_Opening`, `Ch1_MarcoIntro`）

### Step 4: キャラクター ScriptableObject を作成

新キャラが必要な場合:

1. Unity メニュー `Create > Project FoundPhone > Character Profile`
2. CharacterID / DisplayName / ThemeColor を設定
3. `Assets/Resources/Characters/` に保存

### Step 5: 動作確認

1. Windsurf で .yarn ファイルを保存
2. Unity に Alt+Tab で切り替え（自動インポート）
3. **方法A**: Inspector「Play from Node」で特定ノードをテスト
4. **方法B**: F12 で Debug Hub を開き、ノードを選択

### Step 6: イテレーション

- テキスト修正 → 保存 → Unity 自動反映 → Play Mode で確認
- Console エラーが出たら構文を確認（`title:` / `===` の漏れが多い）

---

## 5. 執筆テンプレート

### 新チャプター開始テンプレート

```yarn
title: Ch1_Opening
---
<<declare $ch1_completed = false>>

<<SystemMessage "--- 1日目 ---">>
<<StartWait 1>>

<<set $speaker to "pyramid">>
おはようございます。本日のセッションを開始します。

<<StartWait 0.8>>

<<set $speaker to "player">>
...おはよう。
===
```

### 会話パターン: 複数キャラの掛け合い

```yarn
title: Ch1_GroupChat
---
<<set $speaker to "marco">>
この端末、また調子が悪い。昨日送ったメッセージが届いてないみたいだ。

<<StartWait 1>>

<<set $speaker to "pyramid">>
通信ログを確認しました。昨日 14:32 にメッセージは正常に送信されています。

<<set $speaker to "bernardo">>
<<StartWait 0.5>>
14:32？ 昨日の14時台は端末がフリーズしていたはずだが。

<<set $speaker to "player">>
-> 確かに、自分の端末もその時間は動かなかった
    <<jump Ch1_ConfirmFreeze>>
-> Pyramidの記録を信じる
    <<jump Ch1_TrustAI>>
===
```

### 演出パターン: 端末不調

```yarn
title: Ch1_TerminalGlitch
---
<<set $speaker to "pyramid">>
次の議題に移りま

<<Glitch 1>>
<<StartWait 0.3>>

<<SystemMessage "【SYSTEM】接続が不安定です">>

<<StartWait 1.5>>

<<set $speaker to "pyramid">>
...失礼しました。次の議題に移ります。

<<set $speaker to "marco">>
<<StartWait 0.5>>
今のは何だった？ 一瞬画面が乱れたような...
===
```

### 断片発見パターン

```yarn
title: Ch1_FindFragment
---
<<set $speaker to "mason">>
...古い掲示板を見つけた。紙が一枚貼ってある。

<<StartWait 1>>

<<SystemMessage "断片を発見しました">>

<<set $speaker to "mason">>
読めるか？ 半分かすれてるが...

<<set $speaker to "player">>
-> 読んでみる
    <<UnlockTopic "fragment_ch1_01">>
    <<SystemMessage "断片「施設利用規約（部分）」を獲得しました">>
    <<jump Ch1_ReadFragment>>
-> 後にする
    <<jump Ch1_SkipFragment>>
===
```

---

## 6. Unity への反映

`.yarn` ファイルを保存後:

1. Unity Editor のウィンドウに切り替える（Alt+Tab）
2. Unity が自動的に変更をインポートする
3. Console にエラーがなければ反映完了

**変更が反映されない場合:**

- `Project.yarnproject` を右クリック → `Reimport`
- または Unity メニュー `Assets > Refresh`

---

## 7. テスト方法

### 方法A: Inspector「Play from Node」（Editor 用）

1. ContentAuthoring シーンを開く (`Assets/Scenes/ContentAuthoring.unity`)
2. Hierarchy で `DialogueSystem` を選択
3. ScenarioManager Inspector の **Start Node** ドロップダウンからノードを選択
4. **「Play from Node」** ボタンをクリック
5. 自動で Play Mode に入り、選択ノードから再生開始

### 方法B: Debug Hub（ランタイム）

1. ContentAuthoring シーンで Play Mode に入る
2. **F12** キーを押す → Debug Hub オーバーレイが表示
3. ノードボタンをタップ → そのノードから再生開始
4. 再生中に左上の **「Back to Hub」** → 停止してハブに戻る
5. 別のノードを選択して続けてテスト可能

---

## 8. トラブルシューティング

### ノードがドロップダウンに表示されない

- `.yarn` ファイルが保存されているか確認
- Unity Editor に切り替えて再インポートを待つ
- Console で Yarn コンパイルエラーを確認
- ノードに `title:` ヘッダーと `===` フッターがあるか確認

### ダイアログが開始しない

- DialogueRunner に YarnProject がアサインされているか確認
- `m_AutoStartYarn` が有効になっているか確認（Play from Node は自動で有効化する）
- Console の "ScenarioManager" エラーを確認

### カスタムコマンドが動作しない

- `ScenarioManager.RegisterCustomCommands()` でコマンドが登録される
- ScenarioManager に DialogueRunner と ChatController の参照があるか確認
- `#if YARN_SPINNER` が有効か確認（asmdef の versionDefines で自動定義）

### Debug Hub が表示されない

- ContentAuthoring シーンの DialogueSystem に `DebugHubController` コンポーネントが追加されているか確認
- `DialogueRunner` フィールドがアサインされているか確認
- F12 キーを押しているか確認

---

## 9. チェックリスト: 新チャプター追加時

- [ ] `.yarn` ファイルを `Assets/Resources/Yarn/active/` に作成
- [ ] `Assets/Resources/Channels/` に対応する `ChannelData` を追加/更新（ID, StartNodeName, RequiredCompletedChannelID）
- [ ] チャプターのヒント方針を `ChannelData` に反映（EnableHints, MaxHintDifficulty）
- [ ] 全ノードに `title:` と `===` がある
- [ ] 新キャラの CharacterProfile ScriptableObject を作成済み
- [ ] 変数宣言 `<<declare>>` がプロジェクト内で重複していない
- [ ] Unity Console にコンパイルエラーがない
- [ ] Play from Node / Debug Hub で全ノードの動作確認済み
- [ ] 選択肢の全分岐を通してテスト済み
