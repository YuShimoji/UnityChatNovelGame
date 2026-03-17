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
3. ~~docs/ROADMAP_TO_PRODUCTION.md~~ (archived 2026-03-16)
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

---

# Batch 2 (2026-03-16)

以下のタスクはサブスレッド拡張と並行して実行可能。
各タスクは独立しており、互いに依存しない。

---

## Task G: ETK サブスレッドテストノード追加

```
UnityChatNovelGame の EngineTestKit (Assets/Resources/Yarn/active/EngineTestKit.yarn) に
サブスレッドのテスト項目を追加してください。

### 背景
サブスレッドUI最小スライスが実装済み (b53cbac):
- <<DeclareThread "threadId" "displayName">> でサブスレッドを宣言
- <<AddThreadMessage "threadId" "text">> でサブスレッドにメッセージ追加
- 右上のトグルボタンでメイン⇔サブスレッド切替

EngineTestKit はF12 Debug Hubから各機能をテストできるシナリオ集。
ETK_Menu (Hub&Spoke) に新しいテスト項目を追加する。

### 作業内容
1. ETK_Menu に「サブスレッド テスト」選択肢を追加
   - `<<declare $etk_done_subthread = false>>` を追加
   - 他のテスト項目と同じパターン (<<if not $etk_done_subthread>>)
   - 「テスト完了」条件に $etk_done_subthread を追加

2. ETK_Subthread ノードを新規作成
   - DeclareThread でテスト用スレッドを宣言
   - AddThreadMessage でスレッドに3件程度のメッセージ追加
   - SystemMessage で「右上のボタンでスレッドを切り替えてください」の案内
   - 60秒の StartWait (テスト時間確保、F11で早送り可能)
   - 「メニューに戻る」選択肢で $etk_done_subthread = true に設定

3. 既存の SubthreadTest.yarn (Assets/Resources/Yarn/active/SubthreadTest.yarn) は
   独立テスト用として残す（削除しない）

### 検証
- ETK_Menu で「サブスレッド テスト」が表示される
- テスト実行中にスレッド切替ボタンが出現する
- 全テスト完了後に「テスト完了」選択肢が表示される

### 注意
- EngineTestKit.yarn の既存構造 (ETK_ プレフィックス、Hub&Spoke パターン) を踏襲
- テスト内容は最小限に。目的は「DeclareThread/AddThreadMessage/切替が動く」の確認
```

---

## Task H: UI_IMPLEMENTATION_SPEC サブスレッド追記

```
UnityChatNovelGame の docs/UI_IMPLEMENTATION_SPEC.md にサブスレッドUI関連の
実装仕様を追記してください。

### 背景
サブスレッドUI最小スライスが実装済み (b53cbac)。
ENGINE_FEATURE_INVENTORY.md にはセクション10aとして概要を記載済みだが、
UI_IMPLEMENTATION_SPEC.md には未反映。

### 追記内容

1. ChatController のサブスレッド対応セクションを追加:
   - m_ActiveThreadId / m_ThreadHistories / m_ThreadScrollPositions の役割
   - SwitchToThread() のデータスワップ方式 (ClearMessages + RestoreChatHistory)
   - GetAllThreadHistories() / SetThreadHistories() / SetActiveThreadId() の用途

2. ThreadSwitcherController セクションを追加:
   - プログラマティックUI (Canvas右上にトグルボタン)
   - ScenarioManager.OnThreadDeclared イベント購読
   - メイン⇔サブスレッド切替のフロー

3. SaveData のサブスレッド拡張:
   - Subthreads (List<SubthreadData>)
   - ActiveThreadId

### 関連ファイル (読み取り対象)
- Assets/Scripts/UI/ChatController.cs — SwitchToThread 等のメソッド
- Assets/Scripts/UI/ThreadSwitcherController.cs — 全体
- Assets/Scripts/Data/SubthreadData.cs — データモデル
- Assets/Scripts/Data/SaveData.cs — Subthreads / ActiveThreadId
- Assets/Scripts/Core/SaveManager.cs — CreateSaveData / ApplySaveData のサブスレッド部分
- docs/ENGINE_FEATURE_INVENTORY.md — セクション10a (整合性確認)

### 制約
- 既存セクションの構造を維持
- コード変更は不要 (ドキュメントのみ)
- 「最小スライス」であることを明記し、将来拡張との境界を示す
```

---

## Task I: EngineTestKit のダッシュボードチャンネル登録確認

```
UnityChatNovelGame の ETK (EngineTestKit) テストシナリオが
Dashboard から正しく起動できるか確認し、不足があれば修正してください。

### 背景
- ETK は F12 Debug Hub から起動する設計
- Dashboard には ch_test チャンネル (Assets/Resources/Channels/ch_test.asset) が
  存在し、ETK_DayResume_Day1 を参照している
- しかし ETK_Menu (メインテスト) の Dashboard 経由起動は未確認

### 調査・作業内容
1. Assets/Resources/Channels/ にある全 .asset を確認
2. ch_test.asset の設定を読み取り:
   - ChannelID, DisplayName, StartNode, TotalDays
   - 依存関係 (RequiredChannelIDs)
3. ETK_Menu を Dashboard から起動するには:
   - ch_test.asset の StartNode が ETK_DayResume_Day1 → これは Day Resume テスト用
   - ETK_Menu 用のチャンネル (ch_etk.asset 等) が必要か検討
4. 不足があれば:
   - ch_etk.asset を作成 (StartNode=ETK_Menu, TotalDays=1, RequiredChannelIDs=[])
   - または既存の ch_test.asset を ETK_Menu に変更
5. 対応方針を判断して実行

### 関連ファイル
- Assets/Resources/Channels/*.asset
- Assets/Resources/Yarn/active/EngineTestKit.yarn
- Assets/Scripts/Data/ChannelData.cs (SO定義)
- Assets/Scripts/UI/DashboardController.cs (チャンネル読み込みロジック)

### 制約
- Ch1/Ch2 のチャンネルに影響を与えない
- ChannelData SO は ScriptableObject のため、直接 .asset ファイルの
  テキスト編集は難しい場合がある。その場合は手順書を残す
```

---

## Task J: SaveSystem_README サブスレッド対応更新

```
UnityChatNovelGame の docs/SaveSystem_README.md を更新し、
サブスレッドのセーブ/ロード対応を反映してください。

### 背景
サブスレッドUI最小スライスが実装済み (b53cbac)。
SaveData に以下が追加された:
- Subthreads: List<SubthreadData> (宣言済みサブスレッド)
- ActiveThreadId: string (現在表示中のスレッドID、null=メイン)

SubthreadData の構造:
- ThreadId: string
- DisplayName: string
- ChatHistory: List<SavedChatMessage>

SaveManager の変更:
- CreateSaveData: ScenarioManager.GetAllDeclaredThreads() → saveData.Subthreads
- ApplySaveData: Subthreads → ScenarioManager.RegisterDeclaredThread() で再登録
  + ActiveThreadId → ChatController.SwitchToThread() で復元

### 作業内容
1. SaveData のフィールド一覧にサブスレッド関連を追加
2. セーブ/ロードフローの説明にサブスレッド復元手順を追記
3. 後方互換性の説明 (旧セーブデータにSubthreads がない場合→空リスト)
4. JSON サンプルの更新 (Subthreads の例を追加)

### 関連ファイル (読み取り対象)
- docs/SaveSystem_README.md (更新対象)
- Assets/Scripts/Data/SaveData.cs
- Assets/Scripts/Data/SubthreadData.cs
- Assets/Scripts/Core/SaveManager.cs (CreateSaveData / ApplySaveData)

### 制約
- コード変更は不要 (ドキュメントのみ)
- 既存のセクション構造を維持
```

---

### Batch 2 実行結果

| タスク | コミット | 状態 |
|--------|----------|------|
| Task G: ETK サブスレッドテスト | — | Task K に統合（ThreadType 対応版で実装済み） |
| Task H: UI_IMPL_SPEC 追記 | `8ad85a8` | done (セクション5追加: サブスレッドUI + 種別差異レンダリング) |
| Task I: ETK ダッシュボード確認 | — | done (ch_etk.asset 作成: ETK_Menu をダッシュボードから起動可能に) |
| Task J: SaveSystem_README 更新 | — | done (既存セクション確認済み + SetActiveThreadType ロードフロー追記) |

**注意**: Task G は Batch 3 の Task K (ETK_ThreadType) で代替実装済み。

---

# Batch 3 (2026-03-16)

以下のタスクは基盤開発と並行して実行可能。

---

## Task K: ETK サブスレッド ThreadType テスト拡張

```
UnityChatNovelGame の EngineTestKit (Assets/Resources/Yarn/active/EngineTestKit.yarn) に
サブスレッド ThreadType のテスト項目を追加してください。

### 背景
サブスレッドUI に ThreadType システムが追加済み (e8e53cc):
- <<DeclareThreadTyped "threadId" "type" "displayName">> で型指定サブスレッド宣言
  - type: "A"(注釈)/"B"(追跡)/"C"(偵察)/"branch"(分岐)
- <<DeclareThread "threadId" "displayName">> は type=Annotation にフォールバック
- <<AddThreadMessage "threadId" "text">> / <<AddThreadChat "threadId" "charID" "text">>
- ドロップダウンに型アイコン [A]/[B]/[C]/[>] と型別色を表示
- スレッド切替時にチャット上部にヘッダーバー（型色帯+型名）を表示

### 作業内容
1. ETK_Menu に「サブスレッド/ThreadType テスト」選択肢を追加
   - <<declare $etk_done_threadtype = false>> を追加
   - 他のテスト項目と同じパターン

2. ETK_ThreadType ノードを新規作成:
   - DeclareThreadTyped で 3種類のスレッドを宣言 (A, B, C)
   - 各スレッドに AddThreadMessage / AddThreadChat で2-3件メッセージ追加
   - SystemMessage で「ドロップダウンで型アイコンと色を確認してください」の案内
   - SystemMessage で「スレッド切替後、上部のヘッダーバーの色帯を確認してください」
   - 60秒の StartWait (テスト時間確保)
   - 「メニューに戻る」選択肢で $etk_done_threadtype = true

3. 既存の SubthreadTest.yarn は独立テスト用として残す

### 検証
- ETK_Menu で「サブスレッド/ThreadType テスト」が表示される
- 3種のスレッドがドロップダウンに [A]/[B]/[C] アイコンで表示される
- スレッド切替で型色帯ヘッダーバーが表示される
- Mainに戻るとヘッダーバーが消える

### 参考ファイル
- Assets/Resources/Yarn/active/SubthreadTest.yarn (実装例)
- Assets/Resources/Yarn/active/EngineTestKit.yarn (既存構造)
```

---

## Task L: スレッド通知バナー実装

```
UnityChatNovelGame のメインチャット画面に、未読サブスレッドの通知バナーを
実装してください。

### 背景
サブスレッドにメッセージが追加されると UnreadCount がインクリメントされるが、
ユーザーがドロップダウンを開かないと未読に気づけない。
メインチャット画面上にインライン通知バナーを表示して、
未読サブスレッドへの誘導を改善する。

### 仕様
1. ScenarioManager.OnThreadMessageAdded イベントを購読
2. メインスレッド閲覧中に未読メッセージが追加されたら:
   - チャットエリア下部（入力欄の上）に通知バナーを表示
   - バナー内容: 「[A] {スレッド名} に新着メッセージ (+{件数})」
   - バナー背景色: スレッド型色 (alpha=0.2)
   - テキスト色: スレッド型色
   - タップでそのスレッドに切替 (ThreadSwitcherController.OnSelectThread呼出)
   - 5秒後に自動フェードアウト (DOTween DOFade)
3. 複数スレッドの通知は最新のもので上書き（スタックしない）
4. サブスレッド閲覧中は通知バナーを表示しない

### 実装場所
- ThreadSwitcherController.cs に通知バナー機能を追加
  - CreateUI() 内でバナー用 GameObject を Canvas 下部に生成
  - OnThreadMessageAdded で表示制御

### 関連ファイル
- Assets/Scripts/UI/ThreadSwitcherController.cs
- Assets/Scripts/Core/ScenarioManager.cs (OnThreadMessageAdded イベント)
- Assets/Scripts/Data/SubthreadData.cs (ThreadType, UnreadCount)

### 制約
- ChatController.cs への変更は最小限（できれば不要）
- DOTween の DOFade を使用（既存の依存に含まれる）

### 検証
- SubthreadTest を実行
- メインスレッド閲覧中にサブスレッドにメッセージが追加されると下部にバナー表示
- バナータップでスレッド切替
- 5秒後にバナーが消える
```

---

## Task M: WORKFLOW_STATE_SSOT 更新

```
UnityChatNovelGame の docs/WORKFLOW_STATE_SSOT.md を更新し、
ThreadType 導入 + 型別レンダリング(ヘッダーバー) の完了を反映してください。

### 背景
サブスレッドUI に以下が追加された (e8e53cc):
- ThreadType enum (Annotation/Tracking/Scout/Branch)
- DeclareThreadTyped コマンド (3引数、型指定)
- ドロップダウンに型アイコン [A]/[B]/[C]/[>] + 型別色
- スレッド切替時のヘッダーバー（型色帯）
- SubthreadTest.yarn を DeclareThreadTyped に更新

### 作業内容
1. 「サブスレッドUI」関連の完了ステータスを更新
2. ThreadType を Done 条件に追加
3. 次ステップ（通知バナー、サイドバー等）を future に記載
4. 技術的インサイトに DeclareThreadTyped の設計判断を追記

### 関連ファイル
- docs/WORKFLOW_STATE_SSOT.md (更新対象)
- docs/ENGINE_FEATURE_INVENTORY.md (参照: セクション10a)

### 制約
- コード変更不要（ドキュメントのみ）
```

---

### Batch 3 実行結果

| タスク | コミット | 状態 |
|--------|----------|------|
| Task K: ETK ThreadType テスト | `20b9f18` | done (ETK_ThreadType + ETK_ThreadParallel 追加) |
| Task L: スレッド通知バナー | `761625d` | done (DOTween フェードイン/アウト、クリック切替、型色/アイコン) |
| Task M: SSOT 更新 | `de44142` | done (ThreadType + ThreadParallel 完了反映) |

---

# Batch 4 (2026-03-17)

以下のタスクはドキュメント整合・仕様棚卸し系。コード変更なし。

---

## Task N: spec-index.json 完成度棚卸し

```
UnityChatNovelGame の docs/spec-index.json を実コードと照合し、
pct (完成度) と summary を最新化してください。

### 背景
複数セッションで機能追加が進み、spec-index の pct が実態と乖離している可能性がある。
特に以下が要確認:
- SP-008 (UI/UX): pct=80 だが、サブスレッドUI/レスポンシブ/バブル修正が完了済み
- SP-016 (サブスレッドUI): pct=95 だが、CompleteThread実装済み
- SP-006 (ハルシコイン): pct=95 だが、AddHalluciCoin実装済み
- EN-001 (エンジン機能リファレンス): Yarnコマンド数が21に増加

### 作業内容
1. spec-index.json の全エントリを読む
2. status が "partial" のエントリについて:
   - 対応する仕様ファイルを読み、実装状況を確認
   - pct を実態に合わせて更新
   - summary を最新の実装状況に更新
3. status が "done" のエントリについて:
   - summary が古い場合のみ更新
4. 変更した箇所を一覧で報告

### 制約
- コード変更は不要（ドキュメントのみ）
- pct の判定基準: 仕様書に記載された要件のうち、実装済みの割合
- 楽観的に盛らない
```

---

## Task O: SP-099 未決定事項の棚卸し

```
UnityChatNovelGame の docs/StorySpec/99_open_questions.md を読み、
未決定の4件の現状を確認してください。

### 背景
SP-099 (pct=90) に未決定事項が4件残っている。
これらが基盤設計に影響する可能性があるため、
現時点で「決定可能か / 保留継続か / 論点整理が必要か」を判定する。

### 作業内容
1. 99_open_questions.md を全文読む
2. 未決定4件を抽出し、それぞれについて:
   - 現在の実装状況との関連
   - 決定に必要な情報
   - 推奨アクション (決定案 / 保留理由 / 論点整理)
3. 結果を短いレポートとして報告（ファイル変更不要）

### 制約
- コード変更・ドキュメント変更は不要（調査のみ）
- HUMAN_AUTHORITY に触れる判断は推奨案の提示に留める
```

---

### Batch 4 実行結果

| タスク | コミット | 状態 |
|--------|----------|------|
| Task N: spec-index 棚卸し | `253b45a` | done (pct/summary/乖離修正3件: 16_subthread_ui, 14_interaction_mechanics, ENGINE_FEATURE_INVENTORY) |
| Task O: SP-099 棚卸し | — | done (調査のみ: 4件未決定、全てHUMAN_AUTHORITY。DeclareThreadLatentCond実装でG再訪トリガー到達) |
| Task P: Phase A checklist 更新 | 前セッション | done (73項目。D-8〜D-11追加済み: 潜在/完了/分岐/自動追跡) |

---

## Task P: Phase A クロージングチェックリスト更新

```
UnityChatNovelGame の docs/acceptance/phase-a-closing-checklist.md を更新し、
2026-03-17 の実装追加分を反映してください。

### 背景
チェックリストは 2026-03-16 作成。その後以下が追加された:
- SP-014 Phase 1: 分岐内トピック自動追跡 + 自動反映メッセージ
- SP-006: AddHalluciCoin コマンド
- SP-016: CompleteThread + DeclareThreadLatent/ManifestThread

### 作業内容
1. 既存のブロック D (サブスレッドUI) に以下を追加:
   - D-8: CompleteThread 確認 (ETK_Branch 内の潜在→顕在→完了フロー)
   - D-9: DeclareThreadLatent + ManifestThread 確認 (ETK_Branch 内)
2. ブロック D の末尾に D-10 を追加:
   - 分岐内 UnlockTopic → 自動反映メッセージ確認 (ETK_Branch 内 etk_branch_auto)
3. HalluciCoin 関連の確認項目がなければ追加
4. 項目番号の整合を確認

### 関連ファイル (読み取り対象)
- docs/acceptance/phase-a-closing-checklist.md (更新対象)
- Assets/Resources/Yarn/active/EngineTestKit.yarn (テストフロー参照)

### 制約
- コード変更は不要（ドキュメントのみ）
- 既存の確認項目の構造を維持
```
