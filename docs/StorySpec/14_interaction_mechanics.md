# 14. インタラクション・メカニクス

> チャットノベルの単調さ（早送り+選択肢だけ）を打破するためのゲームシステム群。
> 複数のアイデアが模索段階にあり、**置き換え可能・オンオフ可能**な設計が求められる。

> **注記**: メカニクスの優先順位・設計原則・決定事項はユーザー定義。
> 各メカニクスの詳細説明（例示・特徴・課題の具体的記述）はAI生成ドラフトであり、要レビュー。
> 「5. コード実態マッピング」は実装済み状態の記録として確定仕様扱い。

**ステータス**: 方針決定済み（2026-03-07）。アルケミー方針追加（2026-03-08）：D-3+D-1ハイブリッド（パッシブ主体、重大局面のみ能動合成）。
断片は汎用インベントリ/プレイヤーリソースの一種であり、専用UIは不要（2026-03-08修正）。

**決定事項**:
- 優先順位: A(矛盾指摘) > B(アルケミー) > C(分岐スレッド)
- DeductionBoard: 凍結・隔離（アルケミー仕様確定まで触らない）
- 矛盾発見 → 断片入手の紐づけ: 外す（報酬は HalluciCoin のみ、断片は別経路）

---

## 1. メカニクス候補一覧

### A. ハルシネーション指摘（矛盾タップ）

**概要**: チャットルーム内のAIのハルシネーション（誤字・矛盾・コンテキスト喪失）を即座に「指摘（タップ）」してハルシコインを稼ぐ。

**対象**: AI チャットボット（Pyramid 等）のみ。人間キャラはストーリー進行のみで指摘対象外。

**操作**: テキスト長押し → 選択 → 対応バブルをタップ → 成功/失敗判定

**特徴**:
- AI特攻型でメリハリが生まれる
- コメディ感が出る
- 懸念: 単調になる可能性

**既存仕様との関連**:
- `06_hallucicoin.md`: 獲得方法・難易度曲線・UX演出が定義済み
- `08_ui_ux.md` L41-42: テキスト長押し→指摘メニュー

**コード実態**:
- `ContradictionManager.cs`: 矛盾ペアのマッチング + クールダウン
- `ContradictionFeedbackController.cs`: 成功/失敗の視覚演出
- `MessageBubble.cs`: 長押し検出 + タップ
- `ContradictionDatabase.cs` + `ContradictionPair.cs`: データ定義
- Phase 2 実装済み（動作未確認）

**課題**:
- ~~現在の矛盾ペアはAI/人間の区別なく定義されている~~ → **解決方針**: 対象制限はYarnスクリプト側のタグ付与で制御する。AIキャラのメッセージにのみ `#line:` タグを付与し、人間キャラには付けないことで仕様を満たす。コード側にフィルタは不要
- ~~「指摘対象 = AI のみ」の制約がコードに反映されていない~~ → 同上
- ~~HalluciCoin 以外の報酬（断片入手等）が矛盾発見に紐づいているが、概念的にはズレがある~~ → **決定済み** (2026-03-07): 矛盾報酬=HalluciCoinのみ。ContradictionPair.UnlockTopic は `[Obsolete]` マーク済み

---

### B. アルケミー・ボード（バブル組み合わせ）

**概要**: チャットバブルを長押しまたはタップで「ボード」に送り、保持しておき、関連付けを探す（または生成を待つ）システム。

**パターン**: Alchemy 系パズルの換骨奪胎。

**例**:
- 「何か外から音が聞こえる」+ 「誰もいないのにこの国の流通はどうなっているんだろう」+ 「今日は天気が良さそうだ」
- → 「ちょっと外に出てみようと思う」（ストーリーが進行）

**逆転裁判との違い**: トピックを「保有して突きつける」のではなく、「組み合わせて新しい展開を生む」。生成的。

**既存仕様との関連**:
- `00_overview.md` L16: "推理よりも検証を積む" — 伝統的推理は明示的に排除
- 直接的な仕様記述は StorySpec にない（アイデア段階）

**コード実態**:
- `DeductionBoard.cs`: カード一覧 + ドラッグ&ドロップ合成（SynthesisRecipe）
- `TopicData.cs`: ScriptableObject（合成の素材兼結果）
- 合成レシピ: `SynthesisRecipe.cs` + `RecipeAssetCreator.cs`
- **仕様が固まる前に先行実装された**。隔離・置き換え可能にする必要あり

**課題**:
- 仕様が曖昧 + 複雑。本来は後回しにすべきだった
- 「何と何を組み合わせたら何が出るか」のコンテンツ設計が未着手
- バブル → ボードへの送り込み UI が未設計
- ボードの表示タイミング・場所が未定

---

### C. 分岐スレッド（If ストーリー）

**概要**: 特定のコメントから分岐して別スレッドに入り、If ストーリーのような会話へ発展。そこで新事実を発見する。ブランチ間のメッセージ・知識をプレイヤーだけが受け渡しできる。

**特徴**:
- プレイヤーが「知識の運び屋」になる
- マルチバース的な展開が可能
- サブスレッド（A/B/C）とは異なる概念

**既存仕様との関連**:
- `01_gdd_gameplay.md` L14-18: トピック分岐 → サブスレッド展開の記述あり
- `08_ui_ux.md` L15-24: Discord 的マルチスレッド UI の仕様あり
- ただし「ブランチ間の知識受け渡し」は StorySpec に記述なし（新規アイデア）

**コード実態**:
- `BranchThreadState.cs`: 分岐状態モデル (ActiveBranchId/TransferFlags/TransferredFlags/HiddenFlags)
- `ScenarioManager.cs`: BeginBranch/EndBranch/SetBranchReflection Yarnコマンド
- `TransferSelectionUI.cs`: 知識転送選択UIオーバーレイ (EndBranch "select" モード)
- `SubthreadData.cs`: ManifestCondition/AutoBeginBranch (条件付き潜在+自動分岐開始)
- Phase 1-4 全実装済み

**課題**:
- ~~サブスレッド（A/B/C）との関係整理が必要~~ → **決定済み**: 統合型（`16_subthread_ui.md`）
- ~~分岐の発生条件・収束条件が未定義~~ → **決定済み**: 2段階トリガー（前提条件+顕在化条件、`16_subthread_ui.md` Section 2）
- ~~知識の受け渡しの具体的な UI/UX が未定義~~ → **実装済み**: EndBranch "select" + TransferSelectionUI (Phase 4)
- ~~Yarn での実装方式~~ → **実装済み**: 全22コマンド (`ENGINE_FEATURE_INVENTORY.md` Section 1)
- 未実装: ブランチ間クロスリファレンスUI（別分岐の知識を参照・提示するUI）

---

## 2. 断片（Fragment）とトピック（Topic）の区別

### 断片
- **ゲーム内概念**: その世界に出現する正体不明の紙片。得も知れぬことが書かれている
- **仮称**: キャラクターたちが「断片」と呼んでいるだけ
- **性質**: コレクション要素 + フラグ持ちアイテム
- **入手経路**: 偵察（録音/撮影/採取）、ストーリー進行中の発見
- **用途**: 閲覧・照合の素材。「使う・捨てる」は想定しない
- **公式用語**: 「不可索引物」(unindexable artifact)
- **参照**: `07_fragments.md`

### トピック
- **システム用語兼ゲーム内用語**: 便宜上の管理単位
- **性質**: 知識・情報のラベル（「○○について知っている」状態を表す）
- **現在の用途**: Yarn 変数 `$has_topic_{id}` として分岐条件に使用

### 現在の問題
- コード上では `TopicData` が断片とトピックの両方を表現している
- `ContradictionPair.UnlockTopic` により矛盾発見 → 断片入手になっているが、概念的にはズレている
  - 断片 = 物理的に見つかる紙片 ≠ 矛盾を指摘して得られるもの
  - 矛盾発見の報酬は HalluciCoin が自然
- `fragment_ch2_01` 等のIDで TopicData アセットが作られているが、本来は別の入手経路であるべき

### 決定済み方向性（2026-03-07）
- 矛盾発見 → 断片入手の紐づけを外す（ContradictionPair.UnlockTopic は無効化）
- 矛盾発見の報酬は HalluciCoin のみ
- 断片は Yarn の `<<UnlockTopic>>` や偵察（将来実装）で個別に入手
- データ型の分離（断片 vs トピック）は将来検討。現時点では TopicData を共用

---

## 3. UI配置の原則

- **別画面はなるべく増やさない**（切り替えの認知負荷）
- チャットルームに被さるスライドUIは別スレッドとの共通化がありメタ認知負荷
- 現在の画面構成:
  - ダッシュボード（チャンネル選択）
  - チャットルーム（メイン会話）
  - DebugHub（F12、開発用）
- 断片インベントリの配置: **決定済み** — ダッシュボード内タブ + フローティングUI（`08_ui_ux.md` 参照）

---

## 4. 設計原則

- **置き換え可能**: メカニクスA/B/Cは独立モジュールとして隔離し、オンオフ・差し替えが可能であること
- **段階的導入**: 全てを同時に実装しない。1つずつ検証してから次へ
- **チャットが主軸**: 全てのメカニクスは「チャットバブルとの相互作用」から始まる
- **コンテンツ依存**: メカニクスの面白さはコンテンツ（シナリオ・断片テキスト）に強く依存するため、コンテンツ設計と並行して検証する必要がある

---

## 5. コード実態マッピング

| コンポーネント | メカニクス | ステータス | 備考 |
|---------------|----------|----------|------|
| `ContradictionManager` | A (指摘) | Phase 2 実装済み | 動作未確認 |
| `ContradictionFeedbackController` | A (指摘) | Phase 2 実装済み | 演出統括 |
| `ContradictionPair` / `ContradictionDatabase` | A (指摘) | データ定義済み | 7ペア (Ch1x4, Ch2x3) |
| `MessageBubble` (長押し/タップ) | A (指摘) | 実装済み | 選択→マッチング |
| `DeductionBoard` | B (ボード) | 先行実装 | 仕様未確定のまま実装された |
| `TopicData` | B (ボード) + 断片 | 実装済み | 断片/トピック未分離 |
| `SynthesisRecipe` | B (ボード) | 実装済み | レシピ定義 |
| サブスレッドUI | C (分岐) に関連 | Step 1-4 全実装済み | サイドバー+ThreadType+種別差異レンダリング+通知+条件付き潜在+Save/Load (`16_subthread_ui.md`) |
| 分岐スレッド（知識受け渡し） | C (分岐) | Phase 1-4 全実装済み | BeginBranch/EndBranch/TransferFlags自動追跡/自動反映/知識転送選択UI/条件付き分岐トリガー |

---

## 6. 未解決の設計質問

1. ~~**メカニクスの優先順位**~~: **決定済み** — A > B > C
2. **断片の入手経路**: 偵察が未実装の現在、Yarn コマンド以外の入手手段はどうするか
3. ~~**DeductionBoard の処遇**~~: **決定済み** — 凍結・隔離
4. ~~**断片インベントリの配置**~~: **決定済み** — ダッシュボード内タブ + フローティングUI。断片は汎用インベントリの一種（`08_ui_ux.md` 参照）
5. ~~**分岐スレッドとサブスレッドの関係**~~: **決定済み** — 統合型。同一の「スレッド」概念でUI統合（`16_subthread_ui.md` 参照）
6. ~~**矛盾発見 → 断片入手の紐づけ**~~: **決定済み** — 外す（HalluciCoinのみ）

---

## 2026-03-09 C-Branch Spike Step1 (Implemented)

### Scope
- Implemented only Step1 from the C-branch spike plan.
- Added a bridge state model to hold branch-thread runtime state and transfer flags.

### New Data Model
- `BranchThreadState` (`Assets/Scripts/Data/BranchThreadState.cs`)
  - `ActiveBranchId` : currently active branch thread id
  - `IsActive` : branch thread running state
  - `WasCompleted` : whether branch flow completed before returning
  - `TransferFlags` : list of transfer flag ids for main-thread reflection
  - `Clone()` and `Clear()` are provided for safe snapshot/reset operations

### Runtime Integration
- `ScenarioManager` now owns `m_BranchThreadState`.
- Added minimal APIs:
  - `GetBranchThreadStateSnapshot()`
  - `ApplyBranchThreadState(BranchThreadState state)`
  - `BeginBranchThread(string branchId)`
  - `AddBranchTransferFlag(string flagId)`
  - `EndBranchThread(bool completed)`

### Save/Load Integration
- `SaveData` now has `BranchThread` field.
- `SaveManager` saves `ScenarioManager.GetBranchThreadStateSnapshot()`.
- `SaveManager` restores via `ScenarioManager.ApplyBranchThreadState(...)`.

### Compatibility Notes
- This step does not yet add UI entry points or automatic return effects.
- Existing A/B contradiction mechanics are unaffected by this change scope.

---

## 2026-03-17 Branch Step2+ Phase 1: 分岐内トピック自動追跡 + 自動反映メッセージ (Implemented)

### Scope
- 分岐スレッド内で UnlockTopic が呼ばれた場合、TransferFlags に自動登録
- EndBranch 時に SetBranchReflection 未設定の場合、TransferFlags から反映メッセージを自動生成

### 反映メッセージ優先順位
1. **Yarn指定** (SetBranchReflection): 作者が明示指定したテキストをそのまま使用
2. **自動生成** (TransferFlags): 分岐内で解錠されたトピック名から「分岐で新情報を確認: {トピック名1}, {トピック名2}」を生成
3. **なし**: TransferFlags も空の場合、反映メッセージなし

### 実装詳細
- `UnlockTopicCommand`: `m_BranchThreadState.IsActive` 時に `AddBranchTransferFlag(topicID)` を自動呼出
- `ResolveReflectionMessage()`: 新規メソッド。上記優先順位で反映メッセージを決定
- `EndBranchCommand`: `ResolveReflectionMessage()` を使用するよう変更
- TopicData.Title を使用してユーザー可読な名前で反映メッセージを構成

### テスト
- ETK_Branch ノードに自動反映テスト追加 (etk_branch_auto)
- ETK用 TopicData アセット追加: etk_topic_alpha ("Alpha Signal"), etk_topic_beta ("Beta Pattern")

### 未実装 (Phase 3以降)
- 分岐収束の自動判定
- ブランチ間知識受け渡しUI

---

## 2026-03-17 Branch Step2+ Phase 2: 条件付き分岐トリガー (Implemented)

### Scope
- DeclareThreadLatentCond コマンド: 条件付き潜在スレッド登録
- Yarn変数変更時にリアクティブに条件評価 (EvaluateAllLatentConditions)
- 条件trueで自動ManifestThread + Branch型は自動BeginBranch
- 条件式: 単一変数 ($var) + 比較式 ($var >= N)

### 新コマンド

| コマンド | 構文 | 説明 |
|---------|------|------|
| DeclareThreadLatentCond | `<<DeclareThreadLatentCond "id" "type" "name" "$condition">>` | 条件付き潜在スレッド。条件trueで自動顕在化。Branch型は自動BeginBranch |

### データモデル拡張
- SubthreadData.ManifestCondition: Yarn変数条件式 (null=手動ManifestThread)
- SubthreadData.AutoBeginBranch: 顕在化時に自動BeginBranch (Branch型でtrue)

### 条件評価
- SetVariable<T> 呼び出し時に EvaluateAllLatentConditions() を自動実行
- EvaluateConditionExpression: 単一bool変数 or 比較演算子 (>=, <=, >, <, ==, !=)
- 即時評価: DeclareThreadLatentCond 登録時に条件が既に満たされている場合も対応

### テスト

- ETK_CondBranch ノード追加: $etk_cond_trigger をtrue→自動分岐開始を検証

---

## 2026-03-17 Branch Step2+ Phase 3: 安全弁 + サイドバーバッジ (Implemented)

### Scope

- 安全弁: StopScenario時に分岐状態を自動クリア
- サイドバーバッジ: 完了分岐スレッドに取得トピック数バッジを表示

### 安全弁

- StopScenario() 内で m_BranchThreadState.IsActive なら EndBranchThread(false) を自動呼出
- メインスレッドへの復帰 + ヘッダーバークリアも実行
- ダッシュボード復帰 (ESC) / DebugHub操作時に分岐状態が残り続けるバグを防止

### サイドバーバッジ

- SubthreadData.AcquiredTopicCount: EndBranch時にTransferFlags.Countを記録
- ThreadSwitcherController.OnThreadCompleted: トピック数>0なら緑色バッジ表示
- トピック数=0の完了分岐はバッジなし (グレーアウト+チェックマークのみ)

### 未実装

- ブランチ間知識受け渡しUI: クロスリファレンス (将来: 別分岐の知識を参照・提示するUI)

---

## 2026-03-17 Branch Phase 4: 知識転送選択UI (Implemented)

### Scope
- EndBranch に "select" モードを追加
- 分岐終了時に「どの知識をメインに持ち帰るか」をプレイヤーが選択するUI

### コマンド拡張

| コマンド | 構文 | 説明 |
|---------|------|------|
| EndBranch | `<<EndBranch true "select">>` | 知識転送選択UIを表示。選択後にメイン復帰+反映メッセージ |
| EndBranch | `<<EndBranch true>>` | 従来通り全転送 (後方互換) |

### データモデル拡張
- BranchThreadState.TransferredFlags: プレイヤーが「持ち帰る」と選択したトピックID
- BranchThreadState.HiddenFlags: プレイヤーが「持ち帰らない」と選択したトピックID
- BranchThreadState.SelectionApplied: 選択UIを通過したかどうか

### UI: TransferSelectionUI
- Canvas直下にオーバーレイ動的生成
- スクリム背景 + 中央ダークカード
- トグル付きトピックリスト (デフォルト全件ON)
- 「N/M 選択中」カウンター + 「決定」ボタン
- DOTween フェードイン/アウト

### 設計判断
- $has_topic_X のYarn変数は選択に関わらずtrueのまま (プレイヤーは知っているが見せないだけ)
- ResolveReflectionMessage は SelectionApplied 時に TransferredFlags のみ使用
- AcquiredTopicCount (サイドバーバッジ) も TransferredFlags のカウントを使用

### テスト
- ETK_BranchTransferSelect: 3トピック解錠 → EndBranch "select" → 選択UI → 反映確認
- 後方互換: EndBranch true (selectなし) → 従来通り全転送
