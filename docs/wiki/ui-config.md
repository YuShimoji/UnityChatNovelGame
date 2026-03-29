# ChatUIConfig パラメータ

`Resources/ChatUIConfig` で一元管理するUI設定。Inspector で調整可能。

## バブル

| パラメータ | デフォルト | 説明 |
|-----------|----------|------|
| bubbleMaxWidthPercent | 0.7 | 画面幅に対するバブル幅上限 (割合) |
| bubbleMaxWidthPx | 600 | バブル幅の絶対上限 (px) |
| bubbleSprite | null | 9-slice角丸スプライト (null: 自動生成) |
| bubbleCornerRadius | 16 | 自動生成角丸の半径 (px) |
| bubbleShadowEnabled | true | バブルに影を付けるか |
| bubbleAnimationDuration | 0.2 | バブル出現アニメーション時間 (秒) |

## フォント

| パラメータ | デフォルト | 説明 |
|-----------|----------|------|
| messageFontSize | (要確認) | メッセージのフォントサイズ |
| systemFontSize | (要確認) | システムメッセージのフォントサイズ |

## 選択肢

| パラメータ | デフォルト | 説明 |
|-----------|----------|------|
| choiceButtonColor | (暗めグレー青) | 選択肢ボタンの背景色 |

## メッセージタイミング (ChatDialogueView)

| パラメータ | デフォルト | 説明 |
|-----------|----------|------|
| TypingIndicatorDuration | 0.8 | NPC発話前のインジケーター表示時間 (秒) |
| PostMessageDelay | 0.4 | メッセージ表示後の余韻時間 (秒) |
| EnableTapSkip | true | 画面タップでスキップ可能にするか |

## タイプライター

| パラメータ | デフォルト | 説明 |
|-----------|----------|------|
| m_EnableTypewriterEffect | true | タイプライター効果を有効にするか |
| m_TypewriterSpeed | 0.05 | 1文字あたりの表示時間 (秒) |

## その他

| パラメータ | デフォルト | 説明 |
|-----------|----------|------|
| showInputField | false | テキスト入力欄の表示/非表示 |

> 全パラメータの詳細は `docs/UI_IMPLEMENTATION_SPEC.md` セクション3.3 を参照。
