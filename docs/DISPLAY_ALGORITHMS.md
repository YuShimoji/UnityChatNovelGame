# 表示アルゴリズム仕様

メッセージ表示・スキップ・スレッド切替の処理フローを明文化する。
新機能の実装前にこのドキュメントを参照し、既存フローとの整合性を確認すること。

## 値調整ポリシー (session 17 策定)

フォントサイズ・色・タイミング・レイアウト値の調整は **コード変更ではなく Inspector 操作** で行う。

- **ChatUIConfig** (Resources/ChatUIConfig.asset): メッセージフォント、バブル、選択肢、インジケーター
- **UIFontConfig** (Resources/UIFontConfig.asset): UI 全体のフォント階層 (7段階)

Inspector で変更 → Play → 確認 → 調整。コード変更・コミット・セッション消費は不要。
セッションで「値を変えたので確認してください」のループを回さない。

UI バグ (振る舞いの問題) は `docs/UI_ISSUES.md` に記録し、3-5件溜まったら一括修正する。

---

## 1. メッセージ表示シーケンス

Yarn の1行が画面に表示されるまでの全ステップ。

```
Yarn行 → RunLineAsync → タイピングインジケータ → バブル生成 → タイプライター → 完了待機 → 次の行へ
```

### 1.1 RunLineAsync (ChatDialogueView)

| ステップ | 処理 | 所要時間 | スキップ可能 |
|---------|------|---------|------------|
| 1. 話者解決 | CharacterName → $speaker変数 → "npc" の優先順位 | 即時 | - |
| 2. タイピングインジケータ | NPC発話のみ。吹き出し「...」を表示 | 0.8秒 (TypingIndicatorDuration) | Yes |
| 3. バブル生成 | ChatController.AddMessage() でバブルを生成 | 即時 | - |
| 4. タイプライター効果 | DOTween で1文字ずつ表示 | 文字数 x 0.05秒 | Yes |
| 5. ポストメッセージ遅延 | テキスト全表示後の間 | 0.4秒 (PostMessageDelay) | Yes |
| 6. 完了 | m_IsShowingLine = false → 次の行へ | 即時 | - |

**早送りモード (F11)**: ステップ2-5を全スキップし、30ms の最小遅延のみ。

### 1.2 バブル生成フロー (ChatController.CreateMessageBubble)

```
AcquireBubble (プール or 新規生成)
  → RectTransform リセット
  → TextMeshProUGUI にテキスト設定
    - NPC: 名前行(太字, 本文の75%サイズ) + 改行 + 本文
    - Player: 本文のみ
    - A型注釈: 本文のみ、中央配置
    - System: 本文のみ、専用色
  → バブル幅フィット計算
    - naturalTextWidth = max(本文幅, 名前幅)
    - fitWidth = min(naturalTextWidth + padding, maxBubbleWidth)
  → ConfigureBubble (ラッパー生成 + アイコン配置 + 左右寄せ)
  → FinalizeBubbleSize (Canvas強制更新 → 高さ確定)
  → AnimateBubbleIn (scale 0→1)
  → ApplyTypewriterEffect (DOTween)
  → スクロール予約
```

### 1.3 フォントサイズ

UIFontConfig (ScriptableObject) で一元管理。ハードコード禁止。

| 階層 | 用途例 | デフォルト値 (UIFontConfig) |
|------|--------|--------------------------|
| title | 画面タイトル (HALLUCINATION SIMULATOR) | 28 |
| heading | セクション見出し、カードタイトル | 22 |
| subheading | 強調本文、アクションボタン | 20 |
| body | 本文、ボタンテキスト、ラベル | 20 |
| caption | 説明文、サブタイトル | 18 |
| small | ステータス、バッジ | 14 |
| tiny | ミニ統計、ヒント | 11 |

レスポンシブスケール: Canvas幅 < 900 で縮小 (下限 0.78)。

**ChatUIConfig (チャット固有)**:
- messageFontSize: 22 — UIFontConfig.heading と同格。メッセージは画面の主要素だが title ではない。
- systemMessageFontSize: 18 — body レベル。
- typingIndicatorFontSize: 18
- choiceFontSize: 18-24 (autoSize)

ChatController の messageFontSize は ChatUIConfig で管理。UIFontConfig とは別系統だが、
フォント階層全体のバランスを考慮して値を設定すること。

**session 17 修正**: .asset ファイルに session 14 nightshift の膨張値 (choiceFontSizeMax=36 等) が
残っていたのが「フォントが戻っていない」原因だった。.cs の revert だけでは不十分で、
.asset (Unity が実際に使う値) も修正が必要。

---

## 2. スキップ処理

### 2.1 タップスキップ (2段階スキップ)

**トリガー**: 画面左クリック/タップ (m_EnableTapSkip = true, メッセージ表示中のみ)
**除外**: UI ボタン上のクリックはスキップしない (EventSystem.IsPointerOverGameObject + Button判定)

**2段階スキップ**:

| タップ | 状態 | 動作 | 結果 |
| ------ | ---- | ---- | ---- |
| 第1タップ | インジケータ or タイプライター中 | CompleteCurrentTypewriter + m_LineSkipCts.Cancel | テキスト全文表示。PostMessageDelay は継続 |
| 第2タップ | PostMessageDelay 中 | m_PostSkipCts.Cancel | 次のメッセージへ進む |

**設計意図**:
- 第1タップ: テキストが全文表示される。ユーザーが読む時間がある (PostMessageDelay = 0.4秒)
- 第2タップ: 読み終わったら次へ進む
- 高速読者: 2回素早くタップすれば即時進行 (従来と同等の速度)
- テキストは必ず全文表示されてから次へ進む (complete: true が保証)

### 2.2 早送りモード (FastForwardEnabled)

**トリガー**: F11キーでトグル
**効果**: 全メッセージの表示時間を 30ms に短縮。タイピングインジケータを非表示。

### 2.3 DelayWithSkip の仕組み

```
DelayWithSkip(milliseconds)
  → m_LineSkipCts.Token のみで待機
  → タップスキップ (第1タップ) のみで中断
  → Yarn の NextContentToken は混入させない (自動スキップバグの防止)

DelayWithPostSkip(milliseconds)
  → m_PostSkipCts.Token のみで待機
  → 第2タップのみで中断
```

**Yarn NextContentToken の扱い (session 17 修正)**:
- NextContentToken を遅延の LinkedTokenSource に混入させると、前の行のキャンセル状態がリークし、
  後続の行がインジケーター/タイプライターを全てスキップする自動スキップバグを引き起こす。
- 修正: 遅延は自前の CTS のみで制御し、NextContentToken は各遅延後にポーリングチェック。
- `token.IsNextContentRequested` が true なら即時完了して return。

```
RunLineAsync の制御フロー:
  1. token.IsNextContentRequested? → Yes: 即時表示して return
  2. インジケーター遅延 (m_LineSkipCts のみ)
  3. token.IsNextContentRequested チェック
  4. タイプライター遅延 (m_LineSkipCts のみ)
  5. token.IsNextContentRequested チェック
  6. ポストメッセージ遅延 (m_PostSkipCts のみ)
```

### 2.4 SystemMessage のスキップ

SystemMessage は Yarn の同期コマンドとして実行される (ScenarioManager.SystemMessageCommand)。
ChatController.AddSystemMessage() で直接追加され、タイプライター効果なし、遅延なし。
→ m_IsShowingLine が true にならないため、タップスキップの対象外。

**これは仕様**: システムメッセージは即時表示・即時完了。スキップすべき遅延がない。
次の通常メッセージが RunLineAsync に入ればタップスキップが有効になる。
複数の SystemMessage が連続する場合は全て同一フレームで表示される。

---

## 3. スクロール制御

### 3.1 状態フラグ

| フラグ | 意味 | セット条件 |
|--------|------|-----------|
| m_IsUserScrolling | ユーザーが過去ログを閲覧中 | verticalPos > 0.1 (最下部から離れた) |
| m_IsUserDragging | ドラッグ操作中 | OnBeginDrag / OnEndDrag |
| m_PinnedToBottom | タイプライター中に最下部吸着 | 新メッセージ追加時 (ユーザースクロール中でなければ) |
| m_IsAutoScrolling | 自動スクロール実行中 | AutoScroll 実行中のみ (誤検知防止) |
| m_IsTypewriterActive | タイプライター進行中 | ApplyTypewriterEffect / DOTween完了 |

### 3.2 スクロール動作

```
[通常時 — 新メッセージ追加]
  ユーザーが最下部にいる → m_PinnedToBottom = true
  → LateUpdate で毎フレーム最下部に固定 (タイプライター中、即時)
  → タイプライター完了 + 0.1秒後に DelayedAutoScroll
  → PerformAutoScroll: DOTween で 0.2秒かけてスムーズスクロール (BL-001)
    ※ 既にほぼ最下部 (< 0.02) なら即時移動

[ユーザーが過去ログを見ている時]
  m_IsUserScrolling = true → 自動スクロール抑止
  ユーザーが最下部に戻る → m_IsUserScrolling = false, m_PinnedToBottom = true

[スレッド切替時]
  CanvasGroup alpha=0 でコンテンツ非表示
  → RestoreChatHistory で一括復元 (非表示のまま)
  → 2フレーム待機 (レイアウト確定)
  → スクロール位置を即時復元
  → CanvasGroup alpha を 0.15秒かけてフェードイン
```

---

## 4. スレッド切替

### 4.1 切替フロー

```
[手動切替 — サイドバーからの選択]
  OnSelectThread(threadId)
    → ChatController.SetActiveThreadType(type)
    → ChatController.SwitchToThread(threadId, history)
    → thread.UnreadCount = 0
    → UI更新 (ヘッダー, バッジ, サイドバー閉じ)

[自動切替 — BeginBranch]
  Yarn: <<BeginBranch branchId ...>>
    → DeclareThreadInternal (未宣言なら)
    → 再入時: 古い履歴クリア
    → ChatController.SwitchToThread(branchId, history)
    → ThreadSwitcher.ForceUpdateHeaderBar(branchId)

[自動復帰 — EndBranch]
  Yarn: <<EndBranch>>
    → 知識転送UI (selectモードの場合)
    → 反映メッセージ決定
    → ChatController.SwitchToThread(null)  ← メインに復帰
    → AddSystemMessage(反映メッセージ)
    → ThreadSwitcher.ForceUpdateHeaderBar(null)
```

### 4.2 ChatController.SwitchToThread の内部処理

```
SwitchToThread(threadId, history)
  1. 現在のスレッド履歴を保存
     m_ThreadHistories[currentKey] = m_ChatHistory.ToList()
     m_ThreadScrollPositions[currentKey] = scrollPosition

  2. m_ActiveThreadId = threadId

  3. 対象スレッドの履歴を取得
     優先順位: 保存済み履歴 → 引数のhistory → 空リスト

  4. RestoreChatHistory(targetHistory)
     → ClearMessages() で全バブル破棄
     → 各メッセージを AddMessage で再生成 (アニメなし、タイプライターなし)

  5. 保存済みスクロール位置を1フレーム後に復元
```

### 4.3 LatentThread (条件付き自動顕在化)

```
DeclareThreadLatentCond(threadId, type, name, condition)
  → ManifestCondition を登録
  → EvaluateLatentCondition で即時評価

条件成立時:
  → ManifestThread(threadId)
  → Branch型なら AutoBeginBranch で自動分岐開始
  → ChatController.SwitchToThread + ThreadSwitcher更新
```

### 4.4 既知の問題と対策

1. **メッセージ一括更新 (対策済み)**: SwitchToThread で CanvasGroup alpha=0 にしてから
   ClearMessages → RestoreChatHistory を実行し、レイアウト確定後にフェードインで表示。
   ユーザーにはフラッシュが見えない。

2. **スクロール位置復元 (対策済み)**: 2フレーム待機してレイアウトを確定させてから
   スクロール位置を即時復元。CanvasGroup で隠しているためちらつきなし。

3. **EndBranch 時の順序**: SwitchToThread(null) → AddSystemMessage の順序で実行される。
   CanvasGroup alpha=0 の間に AddSystemMessage が呼ばれるため、
   反映メッセージも含めてフェードインで一括表示される (意図通り)。

---

## 5. 選択肢表示

```
RunOptionsAsync(dialogueOptions)
  1. タイプライター完了待機 (早送りOFF時: 200ms)
  2. 選択肢テキスト抽出
  3. ShowChoices() でボタンUI生成
  4. while ループで選択確定を待機 (毎フレーム YarnTask.Yield)
  5. 選択時:
     - 全ボタン interactable=false (二重クリック防止)
     - 選択テキストを player メッセージとして AddMessage
     - $auto_speaker_after_choice = true なら $speaker = "player"
  6. AutoSave
  7. 選択結果を Yarn に返却
```

---

## 6. 設計ポリシー (今後の実装に適用)

### 表示挙動に関わる実装の事前明文化ルール

1. **表示タイミングに影響する変更は、このドキュメントを先に更新してから実装する。**
2. **パラメータのハードコード禁止。** ChatUIConfig / UIFontConfig の SO に定義し、Inspector で調整可能にする。
3. **スキップ処理の一貫性。** 新しいメッセージ種別を追加する場合、タップスキップ・早送りとの整合性を明記する。
4. **スクロール制御の干渉テスト。** 新しいスクロール操作を追加する場合、既存の6つの状態フラグとの相互作用を確認する。
5. **スレッド切替の副作用確認。** SwitchToThread を呼ぶ新しい経路を追加する場合、履歴保存・復元・スクロール位置の3点を確認する。

---

## 参照ファイル

| ファイル | 責務 |
|---------|------|
| ChatDialogueView.cs | メッセージ表示シーケンス、スキップ、選択肢 |
| ChatController.cs | バブル生成、スクロール制御、スレッド履歴管理 |
| ThreadSwitcherController.cs | サイドバーUI、手動スレッド切替 |
| ScenarioManager.cs | Yarnコマンド実行、BeginBranch/EndBranch |
| ChatUIConfig (SO) | バブル表示パラメータ (fontSize, 色, 幅, 間隔) |
| UIFontConfig (SO) | 全UIフォント階層 (7段階 + レスポンシブスケール) |
