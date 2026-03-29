# 21. Branch Thread (分岐スレッド) 仕様

**最終更新**: 2026-03-29

---

## 概要

Branch Thread は、メインの会話フローから一時的に分岐し、別の視点や分析を行った後にメインに復帰する仕組み。

---

## ライフサイクル

```
[メイン会話]
  ↓ <<BeginBranch "branchId" "displayName">>
[分岐スレッド開始]
  - 画面がクリアされ、分岐スレッドの履歴が表示される (初回は空)
  - 以降のメッセージは分岐スレッドに蓄積される
  - UnlockTopic で TransferFlags に知識を登録
  ↓ <<EndBranch true|false ["select"]>>
[メイン復帰]
  - TransferSelectionUI (selectモード時) でプレイヤーが持ち帰る知識を選択
  - メインスレッドの履歴が復元される (即時表示、アニメーションなし)
  - 反映メッセージがシステムメッセージとして投入される
  - 分岐状態がクリアされる
  ↓
[メイン会話が続行]
```

---

## コマンドリファレンス

| コマンド | 構文 | 説明 |
|----------|------|------|
| BeginBranch | `<<BeginBranch "id" "name">>` | 分岐開始。自動でスレッド切替 |
| EndBranch | `<<EndBranch true\|false>>` | 分岐終了。メインに自動復帰 |
| EndBranch (select) | `<<EndBranch true "select">>` | 知識転送選択UI付き終了 |
| SetBranchReflection | `<<SetBranchReflection "text">>` | 反映メッセージを手動設定 |

---

## 使い方: 基本パターン

```yarn
// 分岐前にフラグで再入を防止する
<<set $did_branch_xxx to true>>
<<BeginBranch "branch_id" "表示名">>

<<set $speaker to "npc">>
<<StartWait 0.8>>
分岐内のメッセージ。

<<UnlockTopic "topic_id">>

<<EndBranch true>>
```

### 知識転送選択UI付き

```yarn
<<BeginBranch "branch_id" "分析">>

<<UnlockTopic "topic_a">>
<<UnlockTopic "topic_b">>

<<EndBranch true "select">>
```

プレイヤーが「持ち帰る」知識を選択。選ばれなかった知識は `$has_topic_xxx = true` のままだが反映メッセージに含まれない。

---

## 設計ルール

### 1. 再入防止フラグは必須

分岐に入る選択肢には必ずフラグガードを付けること。

```yarn
// 良い例: フラグで1回限り
-> 分析を聞く <<if not $did_branch_analysis>>
    <<set $did_branch_analysis to true>>
    <<BeginBranch ...>>

// 悪い例: フラグなし → 何度でも入れてしまう
-> 分析を聞く
    <<BeginBranch ...>>
```

### 2. 分岐内にメタ発言を入れない

「これは別スレッドです」のようなメタ発言は不要。分岐への切り替えは画面遷移で視覚的に示される。

### 3. TransferSelectionUI は2件以上の知識がある場合に使う

知識が1件のみの場合は `<<EndBranch true>>` (selectなし) で十分。

### 4. 分岐 ID はチャプター・シーン単位でユニークにする

```
ch1_branch_analysis
ch2_branch_location_check
ch3_branch_document_review
```

---

## 再入時の挙動

コードレベルでの安全策（session 13 実装済み）:

1. **BeginBranch で既存分岐を再検出** → 古い ChatHistory をクリアして空状態で開始
2. **DeclareThread の重複スキップ** → 既に宣言済みの場合は警告ログのみ
3. **EndBranchThread でフラグクリア** → TransferFlags / TransferredFlags / HiddenFlags を全クリア

Yarn側で再入防止フラグを設定することが推奨。コード側の安全策はフォールバック。

---

## 既知の制限事項

1. **分岐のネスト不可**: BeginBranch の中で別の BeginBranch は呼べない
2. **セーブ復元**: 分岐内でセーブ → ロードした場合、ノード先頭から再開されるため分岐状態は失われる。Yarn変数（$did_branch_xxx）は保持されるため、再入防止フラグで安全性を確保
3. **サイドバー表示**: 分岐スレッドはサイドバーに Branch 型（紫色）として表示される。完了後もエントリは残る

---

## スレッド管理の全体像

```
ScenarioManager
  ├─ m_DeclaredThreads: Dict<string, SubthreadData>  ← 全スレッド (A/B/C/Branch)
  ├─ m_BranchThreadState: BranchThreadState           ← 現在アクティブな分岐
  └─ ChatController
       ├─ m_ThreadHistories: Dict<string, List<SavedChatMessage>>  ← スレッド別履歴
       ├─ m_ActiveThreadId: string?                    ← 現在表示中 (null=メイン)
       └─ SwitchToThread() → RestoreChatHistory()     ← 切替+履歴復元
```

### スレッド種別

| 型 | 用途 | Yarn コマンド | サイドバー色 |
|----|------|--------------|-------------|
| A (Annotation) | 覚書・メモ | DeclareThread / DeclareThreadTyped "A" | 青 |
| B (Tracking) | 追跡ログ | DeclareThreadTyped "B" | 緑 |
| C (Scout) | 偵察報告 | DeclareThreadTyped "C" | オレンジ |
| Branch | 分岐分析 | BeginBranch | 紫 |

---

## リファクタリング候補 (HUMAN_AUTHORITY)

以下は将来の改善候補。実装は承認後。

1. **SwitchToThread のアニメーション**: 現在は即時切替。フェードトランジションの追加
2. **分岐中の視覚的インジケーター**: 分岐中であることを画面上に常時表示
3. **TransferSelectionUI の UX**: 1件時の自動確定オプション
4. **スレッド管理の簡素化**: BranchThreadState と SubthreadData の責務整理
