# 16 — サブスレッドUI仕様

> **ステータス**: done（2026-03-17 完了）
> **依存**: 08_ui_ux.md, 14_interaction_mechanics.md, 01_gdd_gameplay.md
> **前提決定**: 統合型（サブスレッド/分岐スレッドを同一概念として扱う）、2段階トリガー

---

## 1. 概念モデル

### 1.1 スレッドの統合

「サブスレッド」（A:注釈/B:調査/C:偵察強化）と「分岐スレッド」（if分岐型の物語分岐）を**同一の「スレッド」概念**で統合する。

チャンネル（=チャプター）内に「メインスレッド」と複数の「サブスレッド」が存在する構造。

```
チャンネル (ch1)
├── メインスレッド（会話ログ）
├── サブスレッド: [A] 用語注釈
├── サブスレッド: [B] 地名追跡
├── サブスレッド: [C] 偵察指示
└── サブスレッド: [分岐] Pyramid独白ルート
```

### 1.2 スレッドの種類

| 種別 | ID接頭辞 | 内容 | 生成パターン |
|------|----------|------|-------------|
| メイン | `main` | チャプターの主会話ログ | チャンネル開始時に自動生成 |
| A: 注釈 | `annot_` | 用語・背景・制度・AI挙動の短い解説 | 条件トリガー |
| B: 追跡 | `track_` | 地名/機関名/出来事のリンク追跡（Wiki型） | 条件トリガー |
| C: 偵察 | `scout_` | 外出・探索の質向上（録音/撮影/採取） | 条件トリガー |
| 分岐 | `branch_` | if分岐型の物語分岐 | 条件トリガー |

すべてのスレッドは同じUIコンポーネントで表示される。種別はアイコン・色で視覚的に区別。

---

## 2. 2段階トリガー

スレッドの出現は「前提条件」と「顕在化条件」の2段階で制御する。

### 2.1 前提条件（Precondition）

- Yarnスクリプトが `<<DeclareThread>>` コマンドでスレッドを**潜在登録**する
- プレイヤーには見えないが、システムが条件追跡を開始する
- デザイナーが「このスレッドはこの時点以降で出現可能」と宣言する手段

```yarn
// 現在の実装（即時可視化）:
<<DeclareThreadTyped "annot_pyramid_theory" "A" "Pyramidの理論体系">>
// → スレッド "annot_pyramid_theory" が即座に可視状態で登録される
// → 種別A（注釈）、表示名「Pyramidの理論体系」

// type省略時（Annotationフォールバック）:
<<DeclareThread "annot_pyramid_theory" "Pyramidの理論体系">>
```

### 2.2 顕在化条件（Manifestation）

- プレイヤーの行動が閾値を満たすと、潜在スレッドが**可視**になる
- 条件の種類:
  - 断片取得（特定の断片IDを入手）
  - 矛盾発見（特定の矛盾ペアを成功指摘）
  - HalluciCoin蓄積（閾値到達）
  - トピック解放（特定のトピックID取得）
  - 即時（前提条件と同時にフラグ注入 → ストーリー上で即出現させたい場合）

```yarn
// 条件付き潜在登録（Step 4 実装済み）:
<<DeclareThreadLatentCond "annot_pyramid_theory" "A" "Pyramidの理論体系" "$has_topic_pyramid_intro">>
// → Yarn変数 $has_topic_pyramid_intro が true になったときに自動顕在化

// 手動顕在化:
<<DeclareThreadLatent "branch_pyramid_solo" "branch" "Pyramidの独白">>
<<ManifestThread "branch_pyramid_solo">>
// → ManifestThread で明示的に顕在化

// 即時可視化（従来通り）:
<<DeclareThreadTyped "branch_pyramid_solo" "branch" "Pyramidの独白">>
// → 宣言と同時に可視化
```

### 2.3 状態遷移

```
[未登録] → DeclareThread → [潜在] → 条件達成 → [可視/未読]
                                                    ↓
                                              プレイヤーがタップ
                                                    ↓
                                              [可視/既読]
                                                    ↓
                                              スレッド完了
                                                    ↓
                                              [完了]
```

---

## 3. UI設計

### 3.1 基本方針

- **フルスクリーンフォーカス**: 1スレッドが画面全体を占有。没入重視
- **スワイプ切替**: 左端からスワイプでサイドバー（アイコントレイ）がスライドイン
- 常時表示される要素は最小限（停滞感回避）

### 3.2 アイコントレイ（サイドバー）

左端スワイプで表示されるサイドバー。

```
┌──────────┐
│ [Main] ● │  ← メインスレッド（常に最上部）
│──────────│
│ [A] 📝   │  ← 注釈スレッド（新着バッジ付き）
│ [B] 🔍   │  ← 追跡スレッド
│ [C] 🗺   │  ← 偵察スレッド
│ [分] ⑂   │  ← 分岐スレッド
│──────────│
│ キャラ状態 │  ← 各キャラの現在状態（将来拡張）
│ 探索先    │  ← 探索先アイコン一覧（将来拡張）
└──────────┘
```

- 各スレッドアイコンに**新着バッジ**（未読メッセージ数）
- タップでフルスクリーン切替
- スレッドは種別でグループ化、種別内は出現順
- サイドバー外をタップまたは右スワイプで閉じる

### 3.3 ヘッダー

フォーカス中のスレッド情報を表示。

```
┌─────────────────────────────────┐
│ ≡  [A] Pyramidの理論体系    📦 │
│     Ch1 / 未読3件              │
└─────────────────────────────────┘
  ↑              ↑             ↑
  サイドバー     スレッド名     インベントリ
  トグル                       ボタン
```

- `≡` タップでもサイドバーを開閉可能（スワイプの代替）
- スレッド種別アイコン + 表示名
- チャプター名 + 未読件数（サブテキスト）

### 3.4 スレッド内コンテンツ

- メインスレッドと同じチャットバブルUIを使用（08_ui_ux.md Section 6準拠）
- スレッド種別に応じた差異:
  - **A（注釈）**: SystemMessage風の短文カード。対話なし、情報表示のみ
  - **B（追跡）**: Wiki的なリンクカード + チャット。リンクタップで別スレッドへ遷移可能
  - **C（偵察）**: チャット + 成果物カード（録音/撮影/採取）
  - **分岐**: メインスレッドと同じチャットUI。選択肢あり
- 各スレッドのメッセージ履歴は独立して保持

### 3.5 スレッド出現通知

- スレッドが顕在化したとき、**メインスレッド内にシステムメッセージ**を表示
  - 例: `「新しいスレッドが利用可能です: Pyramidの理論体系」`
- サイドバーのアイコンに**パルスアニメーション**（初回のみ）
- 画面上部にトースト通知（控えめ）

---

## 4. データモデル

### 4.1 ThreadData（新規ScriptableObject or ランタイム生成）

```
ThreadID: string          // "annot_pyramid_theory"
ThreadType: enum          // Main / Annotation / Tracking / Scout / Branch
DisplayName: string       // "Pyramidの理論体系"
ChannelID: string         // 所属チャンネル
State: enum               // Latent / Visible / Read / Completed
PreconditionSource: string // 顕在化条件（"topic:pyramid_intro" / "immediate"）
YarnStartNode: string     // スレッド開始ノード（ある場合）
```

### 4.2 BranchThreadState との関係

- 既存の `BranchThreadState`（ScenarioManager内）は**分岐スレッドの内部状態ブリッジ**として維持
- `ThreadData` はUIレイヤーの管理データ
- 分岐スレッドの場合: `ThreadData.ThreadType == Branch` && `BranchThreadState.ActiveBranchId == ThreadData.ThreadID`

### 4.3 SaveData拡張

```csharp
// SaveData に追加
public List<ThreadData> Threads;  // 全スレッドの状態
// BranchThread は既存のまま維持（内部状態ブリッジ）
```

---

## 5. Yarnコマンド

### 実装済み

| コマンド | 構文 | 説明 |
|----------|------|------|
| DeclareThread | `<<DeclareThread "threadID" "displayName">>` | サブスレッドを宣言（type=Annotation）。即時可視化 |
| DeclareThreadTyped | `<<DeclareThreadTyped "threadID" "type" "displayName">>` | 型指定でサブスレッドを宣言。type: "A"/"B"/"C"/"branch"。即時可視化 |
| AddThreadMessage | `<<AddThreadMessage "threadID" "text">>` | サブスレッドにシステムメッセージを追加 |
| AddThreadChat | `<<AddThreadChat "threadID" "charID" "text">>` | サブスレッドにキャラクター付きメッセージを追加 |

> **注**: Yarn Spinner は同名コマンドの引数違いオーバーロードを解決できないため、`DeclareThread`(2引数) と `DeclareThreadTyped`(3引数) に分離している。

### 実装済み（Step 4 完了）

| コマンド | 構文 | 説明 |
|----------|------|------|
| DeclareThreadLatent | `<<DeclareThreadLatent "threadID" "type" "displayName">>` | 潜在登録（UIに出さない）。ManifestThread で顕在化 |
| DeclareThreadLatentCond | `<<DeclareThreadLatentCond "threadID" "type" "displayName" "$condition">>` | 条件付き潜在登録。Yarn変数変更時に条件を自動評価し、trueで自動顕在化。Branch型は自動BeginBranch |
| ManifestThread | `<<ManifestThread "threadID">>` | 潜在スレッドを即座に顕在化（通知メッセージ+サイドバー追加） |
| CompleteThread | `<<CompleteThread "threadID">>` | スレッドを完了状態にする（サイドバーでグレーアウト+チェックマーク） |

- `$condition` パラメータ（DeclareThreadLatentCond）:
  - 単一bool変数: `"$var_name"` — 変数がtrueで顕在化
  - 比較式: `"$var_name >= N"` — 比較演算子(>=, <=, >, <, ==, !=)対応
  - Yarn変数の `SetVariable<T>` 呼び出し時にリアクティブ評価

---

## 6. 実装ロードマップ

### Step 1: データモデル + 最小UI（中期）
- ThreadData クラス定義
- DashboardController にスレッド一覧表示
- SaveData 拡張
- DeclareThread / ManifestThread Yarnコマンド実装

### Step 2: フルスクリーンフォーカス + サイドバー（中期） ✅ 実装済み (2026-03-16)
- ~~アイコントレイ（スワイプサイドバー）~~ → 左スライドインサイドバー実装
  - ハンバーガーボタン(≡) + 未読合計バッジ (左上)
  - 半透明オーバーレイ (タップで閉じ)
  - ThreadType グループヘッダーで種別ごとに分類
  - DOTween スライドアニメ (0.25s)
  - ScrollRect 内蔵 (スレッド多数対応)
  - Main エントリは常に先頭、太字表示
- ~~スレッド切替~~ → OnSelectThread でサイドバー閉じ + 履歴スワップ
- ~~ヘッダー表示~~ → ThreadHeaderBar (型色15%帯 + 型アイコン + 表示名)
- ~~スレッド内チャットUI~~ → 既存 ChatController.SwitchToThread 再利用

### Step 3: 種別ごとの差異 + 通知（中〜長期） — Phase 3a 実装済み (2026-03-17)
- [x] A型(注釈): 情報カード表示（中央配置、キャラアイコンなし、型色カード背景、型色テキスト）
- [x] B/C/分岐: 通常バブル + 型色ティント（キャラ色に12%混合）
- [x] SystemMessage: サブスレッド内で型色ティント適用
- [x] 出現通知: 型アイコン+型色付きシステムメッセージ + ハンバーガーボタン型色パルス (2026-03-17)
- [x] Wiki的リンク遷移: `[link:threadId:label]` マークアップ → TMP link + クリックでスレッド遷移 (2026-03-17)
- [x] 偵察成果物カード: `[artifact:type:desc]` マークアップ → アイコン+イタリック表示 (2026-03-17)

### Step 4: 条件トリガーエンジン — 全実装完了 (2026-03-17)
- [x] DeclareThreadLatent: 潜在登録 (UIに出さない)
- [x] ManifestThread: 顕在化 (通知+サイドバー追加を発火)
- [x] 潜在中もAddThreadMessageでメッセージ追加可能
- [x] 自動トリガー: DeclareThreadLatentCond — Yarn変数条件でリアクティブ自動顕在化 + Branch型AutoBeginBranch (2026-03-17)
- [x] CompleteThread: スレッド完了状態管理 + サイドバーグレーアウト+チェックマーク (2026-03-17)

---

## 7. 未決定事項

- [ ] スレッドの最大同時数制限の有無
- [ ] スレッド内で矛盾指摘は可能か（メインスレッドのみ？）
- [ ] スレッド間のメッセージ参照（「メインスレッドのこの発言について」等）
- [ ] キャラ状態表示の具体的なデータソースとUI
- [ ] 探索先アイコン一覧の具体的な仕様
- [ ] スレッド完了後の再閲覧可否
