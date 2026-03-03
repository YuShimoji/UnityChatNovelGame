# Contradiction Pointing Mechanic - Design Draft

**Status**: Draft v0.1 (2026-03-04)
**導入チャプター**: Chapter 2
**前提**: Ch1 で矛盾の伏線が埋め込み済み

---

## 1. コアインタラクション

### フロー概要

```
プレイヤーがバブルを長押し (500ms)
    → バブルがハイライト状態に入る
    → 画面下部に「矛盾を指摘」ボタンが出現
    → 指摘対象の2つ目のバブルをタップ
    → 2つのバブルの矛盾判定
        → 成功: HalluciCoin + 断片アンロック + Pyramid の反応
        → 不一致: 「関連性が見つかりません」フィードバック
```

### 操作体系

| 操作 | 効果 |
|------|------|
| バブル長押し (500ms) | 指摘モード開始、1つ目を選択 |
| 2つ目のバブルをタップ | ペアリング完了、矛盾判定実行 |
| 画面外タップ / 戻るボタン | 指摘モードキャンセル |
| スクロール | 指摘モード中もスクロール可能（過去ログ参照） |

---

## 2. データモデル

### ContradictionPair (ScriptableObject)

```
ContradictionID: string          // "ch1_admin_reform"
SourceLineTag: string            // Yarn の #line: タグ or メッセージテキストのハッシュ
TargetLineTag: string            // 対になるメッセージ
Chapter: int                     // 検出可能になるチャプター
RewardCoin: int                  // 獲得 HalluciCoin
UnlockTopicID: string            // 発見時にアンロックされる TopicData
PyramidReactionNode: string      // 発見時に再生される Yarn ノード
Difficulty: int                  // 1-3 (視覚的ヒントの量に影響)
```

### Ch1 に埋め込まれた矛盾ペア（4件）

| ID | Source | Target | 概要 |
|----|--------|--------|------|
| `ch1_admin_reform` | 「2019年の広域行政再編」(Pyramid) | 「そんな再編の話は聞いてない」(Marco) | 年代の矛盾 |
| `ch1_facility_name` | 「教育支援施設」(Pyramid) | 「あれは学校だったはずだ」(Marco) | 名称の矛盾 |
| `ch1_search_result` | 「該当する文書は見つかりません」(Pyramid) | 「行政文書が1件確認されました」(Pyramid) | 検索結果の自己矛盾 |
| `ch1_region_identity` | 「第4管理区域」(Pyramid) | 「ここがどこかもはっきりしない」(Marco) | 地域認識の矛盾 |

---

## 3. UI 設計

### 指摘モードの視覚表現

- **選択中バブル**: 枠線が脈動する青いグロー (DOTween)
- **指摘対象候補**: 矛盾ペアに該当するバブルに微かなパルス表示（難易度1のみ）
- **成功時**: 2つのバブルを結ぶ線 + 「矛盾を発見」テキスト + HalluciCoin カウンタ増加
- **失敗時**: 選択バブルが赤く点滅 → 「関連性が見つかりません」トースト

### HalluciCoin 表示

- 画面右上に常時表示（チャット画面のヘッダ内）
- 獲得時: 数値が跳ねるアニメーション + SE

---

## 4. 実装方針

### Phase 1: 基盤（バブルの長押し検出 + データ構造）

- `ChatMessage` に `LineTag` フィールドを追加（矛盾判定用の識別子）
- `ContradictionPair` ScriptableObject の定義
- `ContradictionDatabase` (ScriptableObject) でペア一覧を管理
- バブルに `IPointerDownHandler` / `IPointerUpHandler` を追加

### Phase 2: 指摘フロー

- `ContradictionManager` (MonoBehaviour): 指摘モードの状態管理
- ChatController に指摘モード連動 API を追加
- 矛盾判定ロジック（2つの LineTag がペアに該当するか照合）

### Phase 3: フィードバック・報酬

- HalluciCoin カウンタ UI
- 成功/失敗アニメーション
- Pyramid 反応ノードの再生（YarnSpinner 連動）

---

## 5. 意思決定結果

| 項目 | 決定 | 理由 |
|------|------|------|
| **A. 指摘の粒度** | バブル単位 | 実装が単純。Ch1-3 で十分 |
| **B. 視覚的ヒント** | 段階的開示 | Ch2-3 は難易度1のペアにヒント表示、Ch4以降はヒントなし |
| **C. 誤指摘ペナルティ** | クールダウン (10秒) | 試行錯誤を許容しつつ連打防止 |

---

## 6. 実装済みコンポーネント

| ファイル | 役割 |
|----------|------|
| `Assets/Scripts/Data/ContradictionPair.cs` | ScriptableObject: 矛盾ペア定義 |
| `Assets/Scripts/Data/ContradictionDatabase.cs` | ScriptableObject: 全ペア管理 + 検索 |
| `Assets/Scripts/Core/ContradictionManager.cs` | シングルトン: 指摘モード状態管理、判定、クールダウン、HalluciCoin |
| `Assets/Scripts/UI/MessageBubble.cs` | バブルコンポーネント: 長押し検出 + タップ選択 + ハイライト |
| `Assets/Scripts/Data/SaveData.cs` | `DiscoveredContradictionIDs`, `HalluciCoin` フィールド追加 |
| `Assets/Scripts/Core/SaveManager.cs` | ContradictionManager のセーブ/ロード統合 |

### Yarn コマンド

```
<<Message "charID" "text">>              // 通常メッセージ（タグなし）
<<MessageTagged "charID" "text" "tag">>  // 矛盾判定対応メッセージ
```

### 未実装（Phase 2-3）

- HalluciCoin カウンタ UI
- 成功/失敗アニメーション（バブル間の線、トースト通知）
- Pyramid 反応ノードの再生（YarnSpinner 連動）
- Ch1 の Yarn スクリプトに `<<MessageTagged>>` タグを埋め込み
