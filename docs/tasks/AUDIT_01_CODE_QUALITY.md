# AUDIT_01: コード品質・技術的負債

**点検日**: 2026-02-08
**対象ファイル数**: 37スクリプト

---

## CQ-01: ChatController.Update() の毎フレームスクロール監視 🟡

- **場所**: `Assets/Scripts/UI/ChatController.cs:64-68`
- **内容**: `CheckUserScrollInput()` を毎フレーム `Update()` で呼び出している
- **リスク**: 不要な毎フレーム処理。メッセージ追加時のイベント駆動で十分
- **提案**: `ScrollRect.onValueChanged` イベントに切り替え、`Update()` を削除

---

## CQ-02: CreateMessageBubble と AddImageMessage のコード重複 🟡

- **場所**: `Assets/Scripts/UI/ChatController.cs:131-203, 241-354`
- **内容**: プレイヤー判定・テーマカラー取得・Anchor設定・背景色適用のロジックが `CreateMessageBubble()` と `AddImageMessage()` で完全に重複
- **リスク**: 修正漏れ、保守コスト増大
- **提案**: 共通の `ConfigureBubble(GameObject bubble, string charID)` メソッドに抽出

---

## CQ-03: GC Alloc 削減の After 計測が未完了 🟡

- **場所**: TASK_025
- **内容**: コード修正は完了しているが、Unity Editor での After 計測が実施されていない
- **リスク**: 改善効果が不明。回帰の検知不可
- **提案**: Unity Profiler で計測し、`docs/evidence/` に結果を保存

---

## CQ-04: TopicData.IsValid() の TODO コメント残存 🟢

- **場所**: `Assets/Scripts/Data/TopicData.cs:67`
- **内容**: `IsValid()` に TODO コメントが残っているが、実装自体は機能している（ID・Title の空チェック済み）
- **提案**: TODO コメントを削除するか、Description の空チェックを追加して完了とする

---

## CQ-05: SynthesisRecipe.IsValid() の TODO コメント残存 🟢

- **場所**: `Assets/Scripts/Data/SynthesisRecipe.cs:100-101`
- **内容**: 2つの TODO コメントが残存。null チェックは実装済みだが、各トピックの `IsValid()` 呼び出しが未実装
- **提案**: `m_IngredientA.IsValid() && m_IngredientB.IsValid() && m_Result.IsValid()` を追加

---

## CQ-06: DeductionBoard.AddTopic の Show() が TODO 🟡

- **場所**: `Assets/Scripts/UI/DeductionBoard.cs:122-126`
- **内容**: `m_ShowOnTopicAdded` フラグは存在するが、実際の表示処理が TODO のまま
- **リスク**: トピック追加時にユーザーへの視覚フィードバックがない
- **提案**: `gameObject.SetActive(true)` またはアニメーション付き表示メソッドを実装

---

## CQ-07: m_IsInputLocked が未使用（CS0414 警告抑制） 🟡

- **場所**: `Assets/Scripts/Core/ScenarioManager.cs:27-29`
- **内容**: `#pragma warning disable CS0414` で警告を抑制しているが、フィールド自体が外部から参照されていない
- **リスク**: 入力ロック機能が実質的に機能していない（UIの入力制御に反映されていない）
- **提案**: ChatController 側で `IsInputLocked` プロパティを参照し、入力欄の有効/無効を制御する

---

## CQ-08: AddSystemMessage が MessageBubblePrefab を流用 🟡

- **場所**: `Assets/Scripts/UI/ChatController.cs:381`
- **内容**: システムメッセージ用の専用 Prefab がなく、通常の MessageBubble を流用してスタイルを上書き
- **リスク**: レイアウト崩れ、スタイル変更時の副作用
- **提案**: `SystemMessagePrefab` を別途作成し、専用のスタイルを持たせる

---

## CQ-09: DebugSceneGenerator.cs が空ファイル 🟢

- **場所**: `Assets/Editor/DebugSceneGenerator.cs` (0 bytes)
- **内容**: 空のスクリプトファイルが残存
- **提案**: 不要なら削除。今後使う予定があるなら最低限のスケルトンを記述

---

## CQ-10: Yarn Spinner バージョン未固定 🟠

- **場所**: `Packages/manifest.json:12`
- **内容**: `"dev.yarnspinner.unity": "https://github.com/YarnSpinnerTool/YarnSpinner-Unity.git"` — ブランチ/タグ指定なし
- **リスク**: `Library/` 再構築時に破壊的変更を取り込む可能性
- **提案**: `#v2.4.2` 等のタグを URL 末尾に追加して固定

---

## CQ-11: ChatController.AutoScroll が Invoke ベース 🟢

- **場所**: `Assets/Scripts/UI/ChatController.cs:524`
- **内容**: `Invoke(nameof(PerformAutoScroll), 0.1f)` で遅延実行。Coroutine や DOTween の遅延と混在
- **リスク**: タイミング制御が不正確になる可能性
- **提案**: 統一的に Coroutine または DOTween.Sequence に置き換え

---

## CQ-12: ScenarioManager の二重シナリオシステム 🟠

- **場所**: `Assets/Scripts/Core/ScenarioManager.cs:296-398`
- **内容**: Yarn Spinner 方式と ScriptableObject（`ChatScenarioData`）方式が共存。どちらが本番用か不明確
- **リスク**: メンテナンスコスト二重化、バグ混入リスク
- **提案**: Yarn Spinner を SSOT とし、SO 方式はプロトタイプ/フォールバックとして明文化。将来的に SO 方式を deprecated にする
