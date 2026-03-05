# UI実装仕様書

## 概要
UnityChatNovelGameのチャットUIシステムの詳細実装仕様。GAME_DESIGN_DOCUMENT.mdの設計を具体化したもの。

**最終更新**: 2025-01-XX
**実装状況**: 95%完了（MVP準備完了）

---

## 1. チャットUIシステム

### 1.1 ChatController
**責務**: メッセージバブルの生成、表示、管理

#### 主要機能
- メッセージバブル表示（左/右配置、自動伸縮）
- 選択肢表示/非表示
- スクロール自動追従（吸着機能）
- タイプライター効果
- TypingIndicator（入力中「...」表示）
- キャラクターアイコン表示
- 連続メッセージグループ化（バブルスタック）

#### 実装詳細

##### メッセージバブル構造
```
ScrollRect.content
└── PlayerRow/NpcRow (HorizontalLayoutGroup)
    ├── CharacterIcon (条件付き: 非連続メッセージのみ)
    └── MessageBubble (Image + TextMeshProUGUI)
```

##### 配置ロジック
- **Player**: 右寄せ（左に広いマージン）
- **NPC**: 左寄せ（右に広いマージン）
- **サイドマージン**:
  - 基本: `Screen.width * sideMarginPercent`
  - 上限: `min(sideMarginMaxPx, Screen.width * sideMarginMaxRatio)`
  - モバイル対応: 狭い画面でテキストがはみ出さないよう調整

##### 連続メッセージ判定
- **判定基準**: 直前のメッセージと同じ`charID`
- **スタック時の変更**:
  - 上マージン: `vPad` → `2px`（縮小）
  - キャラクターアイコン: 非表示
- **リセット条件**:
  - 異なる話者のメッセージ
  - システムメッセージ
  - ClearMessages()実行時

##### タイプライター効果
- **実装**: DOTween による`maxVisibleCharacters`のアニメーション
- **速度**: `m_TypewriterSpeed = 0.05f`（1文字あたり0.05秒）
- **スクロール連携**:
  - バブル作成時に`m_PinnedToBottom = true`を設定
  - LateUpdate()で毎フレーム最下部に固定
  - タイプライター完了後に`DelayedAutoScroll()`で最終調整

##### TypingIndicator
- **表示タイミング**: NPCメッセージ表示前（ChatDialogueViewから制御）
- **構造**: 専用GameObject（プール管理外）
  ```
  TypingIndicatorRow (HorizontalLayoutGroup)
  └── TypingIndicator (Image)
      └── Text ("...")
  ```
- **配置**: NPC側（左寄せ）
- **参照保持**: `m_TypingIndicatorWrapper`で親ラッパーを保持（動的取得を避ける）
- **ライフサイクル**: ClearMessages()時も破棄せず、非表示にして再利用

##### スクロール吸着機能
- **目的**: タイプライター効果中も最下部に追従
- **実装**:
  1. `CreateMessageBubble()`でバブル作成時に`m_PinnedToBottom = true`
  2. `LateUpdate()`で`m_ScrollRect.verticalNormalizedPosition = 0f`を毎フレーム実行
  3. タイプライター完了後に`DelayedAutoScroll()`で最終調整
- **解除条件**: ユーザーが手動スクロール（`OnScrollValueChanged()`で検出）

#### 既知の問題と対策

##### 修正済み
- ✅ スクロールのガクガク問題 → DelayedAutoScroll()で解決
- ✅ TypingIndicatorの点滅/バブル消失 → wrapper参照保持で解決
- ✅ 選択肢の二重表示 → choiceMadeガードで解決
- ✅ 選択肢の食い気味表示 → 200ms遅延で解決

##### 未解決
- ⚠️ Ch1でTypingIndicator未表示 → 原因: `<<Typing true>>`コマンド未使用 or 早送りモード有効

---

## 2. Yarn Spinner統合

### 2.1 ChatDialogueView
**責務**: DialoguePresenterBaseの実装、Yarn Spinnerとの統合

#### カスタムコマンド

##### 実装済み
| コマンド | 機能 | 実装状況 |
|---------|------|---------|
| `<<Message charID text>>` | 指定キャラクターのメッセージ表示 | ✅ 完全実装 |
| `<<SystemMessage text>>` | システムメッセージ（中央揃え） | ✅ 完全実装 |
| `<<StartWait seconds>>` | 指定秒数待機 | ✅ 完全実装 |
| `<<SkipWait>>` | 待機をスキップ | ✅ 完全実装 |
| `<<Typing bool>>` | TypingIndicator表示制御 | ✅ 完全実装（手動） |
| `<<UnlockTopic topicID>>` | トピック解放 | ✅ 完全実装 |
| `<<Glitch level>>` | グリッチエフェクト | ⚠️ スケルトンのみ |
| `<<Image resourcePath>>` | 画像メッセージ表示 | ✅ 完全実装 |

##### 未実装
| コマンド | 機能 | 優先度 |
|---------|------|--------|
| `<<AddContact charID>>` | 連絡先追加 | 低（Phase 3+） |
| `<<ChangeStatus charID status>>` | 連絡先ステータス変更 | 低（Phase 3+） |

#### TypingIndicator自動表示ロジック
```csharp
// ChatDialogueView.RunLineAsync() line 73-77
bool isPlayer = charID == "player";
if (!isPlayer && !m_FastForwardEnabled)
{
    m_ChatController.ShowTypingIndicator(true);
    await YarnTask.Delay((int)(m_LineDisplayDelay * 0.6f * 1000), ...);
    m_ChatController.ShowTypingIndicator(false);
}
```
- **表示時間**: `m_LineDisplayDelay * 0.6` = 0.3秒（デフォルト）
- **スキップ条件**: プレイヤーメッセージ、早送りモード有効時

#### 早送りモード
- **トグル**: F11キー
- **効果**:
  - TypingIndicatorスキップ
  - タイプライター待機時間スキップ
  - 選択肢表示前の遅延スキップ
- **UI表示**: DebugOverlay左上に`[FF]`タグ表示

#### 選択肢表示タイミング制御
```csharp
// ChatDialogueView.RunOptionsAsync() line 141-143
if (!m_FastForwardEnabled)
{
    await YarnTask.Delay(200, cancellationToken.NextContentToken).SuppressCancellationThrow();
}
```
- **遅延時間**: 200ms
- **目的**: タイプライター効果完了を確実に待機

---

## 3. UIコンポーネント詳細

### 3.1 MessageBubble
**ScriptableObject**: なし（動的生成）
**プーリング**: MessageBubblePoolで管理

#### 構成要素
- **Image**: 背景（テーマカラー適用）
- **TextMeshProUGUI**: メッセージテキスト
- **LayoutElement**: 高さ自動調整
- **MessageBubble**: 矛盾指摘用コンポーネント（LineTag設定時のみ）

#### サイズ制御
- **minHeight**: `UIConfig.bubbleMinHeight`
- **preferredHeight**: `textComponent.preferredHeight + UIConfig.bubbleTextPadding`
- **flexibleWidth**: 1f（HorizontalLayoutGroupで制御）

### 3.2 CharacterIcon
**実装**: 実行時生成（Discord風円形アイコン）

#### 構成
```
CharacterIconContainer (Mask + Image)
└── Icon (Image: CharacterProfile.Icon)
```

#### サイズ
- **iconSize**: `UIConfig.characterIconSize = 40px`
- **spacing**: `UIConfig.iconBubbleSpacing = 8px`

#### マスク
- **Sprite**: CreateCircleSprite()でランタイム生成
- **Material**: なし（Maskコンポーネントで円形クリッピング）

### 3.3 ChatUIConfig (ScriptableObject)
**場所**: `Assets/Resources/Config/ChatUIConfig.asset`

#### 主要パラメータ
| パラメータ | デフォルト値 | 説明 |
|-----------|-------------|------|
| `sideMarginPercent` | 0.25f | サイドマージンの画面幅比率 |
| `sideMarginMaxPx` | 200 | サイドマージンの最大ピクセル数 |
| `sideMarginMaxRatio` | 0.18f | サイドマージンの画面幅最大比率（モバイル対応） |
| `characterIconSize` | 40f | キャラクターアイコンサイズ |
| `iconBubbleSpacing` | 8f | アイコンとバブルの間隔 |
| `showCharacterIcon` | true | アイコン表示フラグ |
| `typingIndicatorColor` | (0.3, 0.3, 0.35) | TypingIndicator背景色 |
| `typingIndicatorFontSize` | 18f | TypingIndicatorフォントサイズ |

---

## 4. パフォーマンス最適化

### 4.1 オブジェクトプーリング
- **MessageBubblePool**: メッセージバブルの再利用
- **TypingIndicator**: プール外（専用インスタンス）
- **ChoiceContainer**: シーン常駐（子要素のみ破棄/再生成）

### 4.2 Profilerマーカー
- `ChatController.CreateMessageBubble`
- `ChatController.AddMessage`
- `ChatController.ShowChoices`
- `ChatController.AutoScroll`

### 4.3 GC Alloc削減
- `m_ChatHistory`: List再利用
- DOTween: `SetUpdate(true)`でタイムスケール独立
- Layout更新: `Canvas.ForceUpdateCanvases()`で即座に反映

---

## 5. テスト戦略

### 5.1 EditModeテスト（実装済み: 18ケース）
- TopicData検証
- SynthesisRecipe検証
- SaveData検証

### 5.2 PlayModeテスト（拡充予定）
- **ChatController**:
  - AddMessage（通常、連続、システム）
  - ShowChoices / HideChoices
  - AutoScroll / ユーザースクロール検出
- **ScenarioManager**:
  - カスタムコマンド処理
  - Yarn変数管理

### 5.3 手動テスト（MVP_TEST_GUIDE.md）
- MVPTest.yarn実行
- Full Playthrough Test（TASK_027）

---

## 6. 既知の制限事項

### 6.1 現在の制限
1. **探索スレッド**: 未実装（Phase 4+）
2. **ミニゲーム**: 未実装（Phase 4+）
3. **連絡先リスト**: 未実装（Phase 3+）
4. **グリッチエフェクト詳細**: Lv1-2のみスケルトン実装
5. **Addressables**: Resources.Load継続（移行はPhase 4+）

### 6.2 技術的負債
- なし（現時点で重大な負債なし）

---

## 7. 今後の実装予定

### Week 2（品質基盤）
1. PlayModeテストケース拡充
2. ContradictionManager ↔ DeductionBoard 連携
3. GC Alloc最適化

### Week 3（Phase 3基盤）
4. Safe Area / キーボード対応
5. テーマ分離フレームワーク
6. グリッチエフェクト詳細実装

---

## 付録A: トラブルシューティング

### A.1 TypingIndicatorが表示されない
**原因**:
- Yarnシナリオで`<<Typing true>>`未使用
- 早送りモード（F11）が有効

**解決策**:
1. F11を押して早送りモード無効化
2. シナリオに`<<Typing true>>`追加（自動表示に依存する場合は不要）

### A.2 スクロールが吸着しない
**原因**:
- `m_PinnedToBottom`がfalseのまま
- ユーザーが手動スクロール中

**解決策**:
- 最下部付近（verticalNormalizedPosition >= 0.99）までスクロール

### A.3 選択肢が二重表示される
**原因**:
- コールバックが2回実行

**解決策**:
- ChatDialogueView.cs line 149-150でガード実装済み

### A.4 バブルが消失する
**原因**:
- TypingIndicatorWrapperの誤参照（修正済み）

**解決策**:
- `m_TypingIndicatorWrapper`フィールドで参照保持（実装済み）

---

## 付録B: 設定値推奨

### B.1 モバイル対応
- `sideMarginMaxRatio`: 0.15 - 0.20（9:16画面）
- `characterIconSize`: 35 - 45px
- `messageFontSize`: 24 - 28

### B.2 デスクトップ
- `sideMarginMaxPx`: 200 - 300
- `characterIconSize`: 40 - 50px
- `messageFontSize`: 28 - 32

### B.3 パフォーマンス
- `m_TypewriterSpeed`: 0.03 - 0.07（読みやすさとのバランス）
- `autoScrollDelay`: 0.05 - 0.1（レイアウト更新待ち）
