# Delegation Prompts — UnityChatNovelGame

別の Claude セッションに委譲可能なタスクのプロンプト集。
各プロンプトを新しいセッション（同じリポジトリ）に渡して使用する。
CLAUDE.md が共有されるため、コーディング規約・アーキテクチャは自動的に適用される。

作業完了後は main ブランチにコミットし、コア開発セッションで統合確認する。

---

## Task A: MessageTagged 参照を #line: タグ方式に統一

```
UnityChatNovelGame (Unity 6.3 / Yarn Spinner 3.1.3) のドキュメントで
MessageTagged コマンドの参照を #line: タグ方式に統一してください。

### 背景
矛盾指摘システムでメッセージにタグを付ける方法が2つある:
- <<MessageTagged "id">> (旧方式、非推奨)
- #line:tag_id (新方式、推奨。Yarn Spinner 標準)

実際の Yarn スクリプト (Assets/Resources/Yarn/active/) では全て #line: を使用。
コード上も ScenarioManager.cs line 155 で MessageTagged ハンドラは登録されているが
active な Yarn ファイルでは使われていない。

### 対象ファイル (7件)
1. docs/UI_IMPLEMENTATION_SPEC.md
2. docs/StorySpec/14_interaction_mechanics.md
3. docs/ROADMAP_TO_PRODUCTION.md
4. docs/ENGINE_FEATURE_INVENTORY.md (既に部分修正済み、残存確認)
5. docs/SCENARIO_AUTHORING_GUIDE.md (既に部分修正済み、残存確認)
6. docs/YarnEditingPipeline.md (既に部分修正済み、残存確認)
7. docs/spec-index.json (summary 内の記述)

### 作業内容
- 各ファイルで `MessageTagged` を grep し、文脈に応じて:
  - 推奨方式として `#line:` タグを記載
  - `<<MessageTagged>>` は「非推奨。互換のため残存」と注記
  - コード例は `#line:` 方式に書き換え
- 4-6 は 655d6a4 コミットで部分修正済み。残存箇所のみ対応。
- 実コード変更は不要（MessageTagged ハンドラは互換のため残す）

### 検証
修正後、全対象ファイルで `MessageTagged` を grep し、
未修正箇所がないこと（または明示的に非推奨注記があること）を確認。
```

---

## Task B: 文字化けコメント修正

```
UnityChatNovelGame の C# ソースファイルに含まれる Shift-JIS 文字化け
コメントを正しい UTF-8 日本語に書き換えてください。

### 対象ファイル (4件, 計約92行)
1. Assets/Scripts/Core/ScenarioManager.cs — 62行
2. Assets/Scripts/Tests/CoreLogicTests.cs — 6行
3. Assets/Scripts/UI/SaveLoadUI.cs — 14行
4. Assets/Scripts/UI/SaveSlotUI.cs — 10行

### 文字化けの特徴
「繧」「縺」「蜻」「繝」「逕」「莉」等の文字列が含まれる XML doc コメント。
例: `/// Yarn Spinner縺ｮDialogueRunner繧偵Λ繝・・縺励...`
→ 正: `/// Yarn SpinnerのDialogueRunnerをラップし、カスタムコマンドを処理するシナリオ管理クラス`

### 作業方針
1. 各ファイルを読み、文字化け行を特定
2. 周辺コード（メソッド名、引数、実装）から元の意味を推測
3. 正しい日本語 UTF-8 の XML doc コメントに書き換え
4. コメント以外のコード変更は禁止

### 推測のヒント
- ScenarioManager: Yarn Spinner のカスタムコマンド (StartWait, Typing,
  SystemMessage, Glitch, UnlockTopic, EndDay) のハンドラ登録・解除
- SaveLoadUI / SaveSlotUI: セーブ・ロードUIの表示・操作
- CoreLogicTests: 基本ロジックのユニットテスト

### 検証
修正後、以下の grep でヒット数が 0 になること:
grep -c "繧\|縺\|蜻\|繝\|逕\|莉" <対象ファイル>
```

---

## Task C: オートセーブ実装 (EN-005)

```
UnityChatNovelGame (Unity 6.3 / Yarn Spinner 3.1.3) にオートセーブ機能を
実装してください。設計は docs/specs/EN-005_autosave_design.md を参照。

### 現状
- SaveManager.cs: SaveGame(int slot) / LoadGame(int slot) は実装済み
- EndDay コマンド (ScenarioManager.cs:414-454): EndDay 実行時に
  ChannelDayProgress を記録し SaveGame() を呼び出す（これが唯一の自動保存箇所）
- SaveData.cs: 全保存項目定義済み (ChatHistory, YarnVariables,
  CompletedChannelIDs, ChannelDayProgress, DiscoveredContradictionIDs 等)
- 保存形式: JSON (Newtonsoft.Json)、PlayerPrefs ベース

### 実装すべき機能
1. AutoSave() メソッドを SaveManager に追加
   - 専用スロット (slot=99 等) を使用
   - クールダウン (30秒等) で連続保存を防止
   - SaveGame() のラッパーとして実装
2. トリガーフック
   - ノード遷移時 (DialogueRunner.onNodeStart)
   - 選択肢選択時 (RunOptionsAsync 完了後)
   - EndDay 時 (既存の SaveGame 呼び出しを AutoSave に統合)
3. UI通知 (最小限)
   - 保存中に画面端に小さいインジケーター表示 (1秒程度)
   - ChatController に AddSystemIndicator() 等は不要。Canvas 直下に軽量表示。

### 制約
- AutoSave スロットは通常セーブスロットと別管理
- AutoSave は LoadGame 画面で「オートセーブ」として表示
- パフォーマンス: JSON シリアライズは軽量だが、毎フレーム実行しない
- ChatHistory が大きい場合の保存コストに注意 (非同期化は任意)

### 関連ファイル
- Assets/Scripts/Core/SaveManager.cs (メインの保存ロジック)
- Assets/Scripts/Core/ScenarioManager.cs (EndDay コマンド 414-454行)
- Assets/Scripts/UI/ChatDialogueView.cs (RunOptionsAsync, onNodeStart)
- Assets/Scripts/Data/SaveData.cs (保存データ構造)

### 検証
1. ノード遷移でオートセーブが走ることを Console ログで確認
2. クールダウン内の連続トリガーが無視されること
3. オートセーブスロットからのロードが正常動作すること
4. 通常セーブスロットに影響がないこと
```

---

## Task D: セーブ復元時の名前重複防止

```
UnityChatNovelGame のセーブロード機能で、チャット復元時にキャラクター名が
重複表示されるバグを修正してください。

### 問題
- CreateMessageBubble() (ChatController.cs:595-632) で、NPC メッセージの
  finalText に名前行を埋め込んでいる:
  `finalText = $"<line-height=80%><size={nameSize}><b>{displayName}</b></size>\n</line-height>{body}"`
- SavedChatMessage.Text にはこの finalText (名前込み) が保存される
- ロード時に RestoreChatHistory() が SavedChatMessage.Text を
  AddMessage() に渡すと、AddMessage → CreateMessageBubble で
  再度名前が付加され、名前が2重に表示される

### 修正方針
以下のいずれかのアプローチを選択:
A) SavedChatMessage に OriginalText (名前なし本文) を追加し、
   保存時は本文のみ、復元時は本文から再構築
B) RestoreChatHistory で isRestore フラグを渡し、
   CreateMessageBubble 内で名前付加をスキップ
C) SavedChatMessage.Text を保存する時点で名前部分を除去

### 関連ファイル
- Assets/Scripts/UI/ChatController.cs
  - CreateMessageBubble() 595-632行
  - RestoreChatHistory() (grep "RestoreChatHistory" で特定)
- Assets/Scripts/Data/SaveData.cs (SavedChatMessage 構造)
- Assets/Scripts/Core/SaveManager.cs (ApplySaveData 271行~)

### 検証
1. メッセージを数件表示 → セーブ → ロード
2. ロード後、NPC名が1回だけ表示されること
3. Player メッセージ (名前なし) に影響がないこと
4. SystemMessage に影響がないこと
```

---

## Task E: 画面リサイズ対応

```
UnityChatNovelGame のチャットバブル幅計算で Screen.width を使用している箇所を、
リサイズに追従するよう修正してください。

### 問題
ChatController.cs の3箇所で `Screen.width * UIConfig.bubbleMaxWidthPercent`
を使用してバブル最大幅を計算している (line 339, 642, 1659)。
Screen.width は起動時のウィンドウサイズで固定されるため、
プレイ中にウィンドウをリサイズしてもバブル幅が追従しない。

### 修正方針
1. ChatController に `float GetMaxBubbleWidth()` ヘルパーを追加
   - 現在の Canvas / RectTransform の幅を基準に計算
   - `Mathf.Min(canvasWidth * UIConfig.bubbleMaxWidthPercent, UIConfig.bubbleMaxWidthPx)`
2. 3箇所の `Screen.width` 使用を `GetMaxBubbleWidth()` に置換
3. リサイズ時の再計算は不要（新バブルから自動適用される）

### 関連ファイル
- Assets/Scripts/UI/ChatController.cs (line 339, 642, 1659)
- Assets/Scripts/Data/ChatUIConfig.cs (bubbleMaxWidthPercent, bubbleMaxWidthPx)

### 制約
- 既存バブルのリサイズ（全バブル再計算）は不要
- Canvas の参照は m_ScrollRect.GetComponentInParent<Canvas>() 等で取得
- パフォーマンスに注意（GetMaxBubbleWidth は頻繁に呼ばれる）

### 検証
1. Unity Editor で GameView のサイズを変更
2. 新しいバブルが変更後のサイズに追従すること
3. 既存バブルは変更されなくてよい
```

---

## Task F: generatedSprite=null 調査

```
UnityChatNovelGame の ChatController.ApplyBubbleVisuals で、
GetOrCreateRoundedSprite() が null を返す問題を調査してください。

### 症状
- ApplyBubbleVisuals (ChatController.cs:2283) で
  `Sprite sprite = UIConfig.bubbleSprite ?? GetOrCreateRoundedSprite();`
  の結果が null
- ログ: `[ApplyBubbleVisuals] sprite is null. cornerRadius=16, bubbleSprite=null`
- 全バブルで発生（キャッシュ m_GeneratedRoundedSprite が効いていない）
- しかし NullReferenceException は出ない
  (GetOrCreateRoundedSprite 内の m_GeneratedRoundedSprite.name = "..." で NRE が出るはず)
- バブルの視覚的表示には問題がない（角丸が正しく表示されているように見える）

### 矛盾点
1. Sprite.Create (line 2240-2248) は null を返さないはず
2. line 2249 `m_GeneratedRoundedSprite.name = "GeneratedRoundedBubble"` で
   NRE が出ない → sprite は有効なオブジェクト
3. しかし `sprite != null` (Unity の overloaded operator) が false を返す

### 調査方針
1. Unity 6.3 LTS (6000.3.6f1) での Sprite.Create の挙動確認
   - Runtime 生成 Sprite が Unity の `== null` チェックで false になるケースがあるか
   - `ReferenceEquals(sprite, null)` vs `sprite == null` の違いを検証
2. GetOrCreateRoundedSprite に追加ログを入れて、Create 直後と
   ApplyBubbleVisuals での評価結果を比較
3. 必要なら `System.Object.ReferenceEquals` で C# 参照としての null チェックに変更

### 関連コード
- Assets/Scripts/UI/ChatController.cs
  - m_GeneratedRoundedSprite (field, line 85)
  - GetOrCreateRoundedSprite() (line 2181-2251)
  - ApplyBubbleVisuals() (line 2283-2311)

### 期待する成果物
1. 原因の特定 (Unity 6 固有の問題 / コードバグ / 想定通りの動作)
2. 修正パッチ（原因に応じて）
3. ログが出なくなることの確認
```

---

## 使い方

各プロンプトを新しい Claude セッション（同じリポジトリ）に渡して使用。
CLAUDE.md が共有されるため、コーディング規約・アーキテクチャは自動的に適用される。

作業完了後は main ブランチにコミットし、コア開発セッションで統合確認する。

### 実行結果 (2026-03-13 完了)

| タスク | コミット | 状態 |
|--------|----------|------|
| Task A: MessageTagged 統一 | `2a34836` | done |
| Task B: 文字化けコメント | `2a34836` | done |
| Task D: 名前重複防止 | `147b36d` | done |
| Task F: generatedSprite=null | `147b36d` | done (原因: Unity fake null + ?? 演算子) |
| Task E: 画面リサイズ | `8b22b71` | done |
| Task C: オートセーブ | `6cc1a63` | done |

全6タスク完了。Unity手動確認は未実施。
