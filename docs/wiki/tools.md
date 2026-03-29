# エディタツール

## YarnContent Validator

**場所**: `Tools > FoundPhone > Yarn Content Validator`

Yarn ファイルの静的バリデーションツール。

### 検出する問題

- コマンド構文エラー
- 未定義の topicID / characterID
- 不正なノード構造

### 使い方

1. Unity メニューから起動
2. 「Validate」をクリック
3. 結果が Console に表示される
4. エラーがあれば Yarn ファイルを修正

---

## YarnSO Generator

**場所**: `Tools > FoundPhone > Yarn SO Generator`

Yarn ファイルを走査し、不足している ScriptableObject を自動検出・生成するツール。

### 検出対象

- **TopicData**: `UnlockTopic` / `DiscoverFragment` で参照される topicID
- **CharacterProfile**: `$speaker` / `Message` コマンドで参照される characterID

### 使い方

1. Unity メニューから起動
2. 「Scan Yarn Files」をクリック
3. 結果表示: 参照数、既存数、欠落数
4. 欠落がある場合は「Generate Missing SOs」で自動生成

### 出力例

```
Topic references: 14 (existing: 16, missing: 0)
Speaker references: 7 (existing: 7, missing: 0)
All SOs are present. Nothing to generate.
```

---

## Debug Hub

**キー**: F12

Play モード中に全 Yarn ノードの一覧を表示するデバッグツール。

### 機能

- 全ノード一覧 (チャプター別グループ、ストーリー順ソート)
- ノードクリック → 前ダイアログ停止 → メッセージクリア → 選択ノードから再生
- 任意のシーンから即座にジャンプ可能

### ストーリー順ソート

Yarn ファイル内の `title:` 行の出現順で自動決定。ファイル間はファイル名でソート。

---

## 早送りモード

**キー**: F11

有効時の動作:

- タイピングインジケーター表示をスキップ
- タイプライター効果をスキップ (即時全文表示)
- StartWait の待機をスキップ (30ms 最小遅延)
- 選択肢は通常通りプレイヤー操作を待つ

デバッグオーバーレイに `[FF]` タグが表示される。

---

## タップスキップ

**操作**: 画面タップ / クリック

メッセージ表示中に画面をタップすると:

1. タイピングインジケーターを即非表示
2. タイプライター効果を即完了 (全文表示)
3. 待機時間をスキップして次のメッセージへ

選択肢のボタン上のクリックはスキップ対象外。

---

## Debug Overlay

**設定**: ChatDialogueView Inspector の `Show Debug Overlay` を true に設定

右上に小さなバッジを表示:

- 折りたたみ時: `[D] ノード名`
- 展開時: ノード名、行ID、タグ
- 早送り時: `[FF]` タグ追加
- クリックで展開/折りたたみ
