# トラブルシューティング

## よくある問題

### Yarn ファイルがコンパイルされない

**原因**: `active/` ディレクトリ外にファイルがある

**対処**:
1. ファイルが `Assets/Resources/Yarn/active/` 内にあることを確認
2. `Project.yarnproject` の sourceFiles が `active/**/*.yarn` であることを確認

### 「DeclareThread already declared」警告

**原因**: 同じスレッドIDが2回宣言された

**対処**:
- DeclareThread は1回だけ呼ぶ (通常は Opening ノードで)
- 分岐スレッドは BeginBranch が自動宣言するため DeclareThread 不要

### 選択肢が無限に表示される

**原因**: 選択肢にフラグガードがない

**対処**:
```yaml
// 修正前: 何度でも選べる
-> トピックを聞く
    <<jump Topic>>

// 修正後: 1回のみ
-> トピックを聞く <<if not $asked_topic>>
    <<set $asked_topic to true>>
    <<jump Topic>>
```

### 分岐に何度も入れてしまう

**原因**: 分岐の再入防止フラグがない

**対処**:
```yaml
-> 分析を聞く <<if not $did_branch_analysis>>
    <<set $did_branch_analysis to true>>
    <<BeginBranch ...>>
```

### メッセージが速すぎる / 遅すぎる

**対処**:
- Inspector: `ChatDialogueView` の `Typing Indicator Duration` と `Post Message Delay` を調整
- Yarn: `<<StartWait 秒数>>` で手動調整
- Play 中: F11 で早送りモード

### バブルのフォントサイズを変更したい

`Resources/ChatUIConfig` の `messageFontSize` を変更。

### 選択肢の色を変更したい

`Resources/ChatUIConfig` の `choiceButtonColor` を変更。

### セーブデータが壊れた

Unity メニュー: `Project FoundPhone > Delete All Save Data`

### Console に DeductionBoard 警告が出る

```
DeductionBoard: Instance not found in scene
```

既知の問題。DeductionBoard はまだシーンに配置されていません。動作に影響はありません。

## デバッグ手順

1. **Console を確認**: Error (赤) > Warning (黄) の順に対処
2. **Debug Hub (F12)**: 任意のノードにジャンプして問題を再現
3. **Debug Overlay**: Inspector で `Show Debug Overlay = true` にして現在のノード・行を確認
4. **早送り (F11)**: 問題のシーンまで素早く進める
5. **YarnContentValidator**: 静的エラーを検出

## パフォーマンス

- 大量のメッセージ (100+) がある場合、スクロールが重くなることがある
- 分岐スレッドの切替時に一瞬のラグが発生することがある (履歴復元のため)
