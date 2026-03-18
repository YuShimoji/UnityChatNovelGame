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
// 部分実現済み（DeclareThreadLatentCond、Step 4 相当）:
// Yarn変数条件による自動顕在化 + AutoBeginBranch を実装済み
<<DeclareThreadLatentCond "annot_pyramid_theory" "branch" "Pyramidの理論体系" "$pyramid_intro_seen">>
// → $pyramid_intro_seen が true になったときに自動顕在化 + 分岐開始
// 未実装: トピック/断片/HC閾値による複合トリガー条件

// 現在の実装では全て即時可視化:
<<DeclareThreadTyped "branch_pyramid_solo" "branch" "Pyramidの独白">>
// → 宣言と同時に可視化（conditionパラメータは未実装）
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

### 未実装（Step 4 で対応予定）

| コマンド | 構文 | 説明 |
|----------|------|------|
| DeclareThread (条件付き) | `<<DeclareThread "threadID" "type" "displayName" "condition">>` | 条件トリガー付きスレッド宣言（潜在登録） |
| ManifestThread | `<<ManifestThread "threadID">>` | スレッドを即座に顕在化（条件無視） |
| CompleteThread | `<<CompleteThread "threadID">>` | スレッドを完了状態にする |

- `condition` パラメータ（将来実装）:
  - `"topic:topicID"` — トピック解放時
  - `"fragment:fragmentID"` — 断片取得時
  - `"contradiction:pairID"` — 矛盾指摘成功時
  - `"hc:amount"` — HalluciCoin閾値到達時
  - `"immediate"` — 即時顕在化

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

## 7. 検証・モック不足 (2026-03-18 監査)

### 7.1 ETK カバレッジ状況

| ETKノード | カバー範囲 | 未カバー |
|-----------|-----------|---------|
| ETK_ThreadType | A/B/C型宣言、メッセージ追加、基本切替 | 通知バナーのクリック切替 (目視のみ) |
| ETK_ThreadParallel | 3スレッド並走、ノード遷移後維持 | 10スレッド以上の負荷テスト |
| ETK_Branch | 潜在→顕在→分岐→自動反映→完了 | 分岐中セーブ→ロード→再開のフルパス |
| ETK_CondBranch | 条件トリガー→自動顕在化→AutoBeginBranch | ロード後の条件再評価 |
| ETK_AutoVerify | D-1〜D-9, E, F の目視ガイド | D-7 並走は ETK_ThreadType で別途カバー |
| ETK_BranchTransferSelect | 選択UI + 後方互換 | 選択UI中のStopScenario割り込み |

### 7.2 未作成のモック・テストシナリオ

| モック | 目的 | 優先度 |
|--------|------|--------|
| **B型追跡スレッドの実用モック** | Tracking型スレッドでキャラの行動を追跡する体験。Wikiリンク遷移 (`[link:threadId:label]`) の実動作確認 | HIGH |
| **C型偵察スレッドの実用モック** | Scout型スレッドで成果物カード (`[artifact:type:desc]`) の表示確認。録音/撮影/採取のUI検証 | HIGH |
| **サブスレッドライフサイクル一気通貫モック** | 宣言→メッセージ蓄積→潜在→条件トリガー→顕在化→分岐開始→分岐内対話→知識転送選択→完了→Save/Load→復元 の全工程 | HIGH |
| **10スレッド並走ストレステスト** | サイドバーのスクロール・パフォーマンス・未読バッジ累積の確認 | MEDIUM |
| **分岐中セーブ→ロード→再開テスト** | TransferFlags クリア問題 (EN-003 既知問題参照) の検証 | HIGH |
| **メインスレッド空状態からの復帰テスト** | 全スレッド完了後にメインに戻った際の表示 | LOW |

### 7.3 ランタイム制限事項 (実装済み機能の制約)

| 制限 | 詳細 | 影響 |
|------|------|------|
| **サブスレッド内でYarnノード実行不可** | AddThreadMessage/AddThreadChat のみ。Yarnダイアログ (選択肢・分岐) はメインスレッドでしか動かない | B型/C型スレッドの対話体験が制限される |
| **サブスレッド内で矛盾指摘不可** | ContradictionManager はメインスレッドの ChatHistory のみを検索対象とする | サブスレッド内の矛盾ペアは検出できない |
| **DeclareThreadLatentCond の条件はYarn変数のみ** | トピック取得/断片収集数/HC閾値などの複合条件はYarn変数を介して間接的に表現する必要がある | オーサリングの手間が増える |
| **Yarn外からのVariableStorage直接操作で条件評価が走らない** | `ScenarioManager.SetVariable<T>` 経由でないと `EvaluateAllLatentConditions` が発火しない | 将来の拡張でサイレント不整合のリスク |
| **ロード後の AutoBeginBranch 再発火** | IsLatent=true のまま保存されたスレッドは、ロード後の変数復元で条件成立時に自動顕在化+分岐開始が再発火する。意図通りの場合と意図外の場合がある | テストで挙動を確認する必要あり |

---

## 8. 未決定事項

- [ ] スレッドの最大同時数制限の有無
- [ ] スレッド内で矛盾指摘を許可するか（メインスレッドのみ？）
- [ ] スレッド間のメッセージ参照（「メインスレッドのこの発言について」等）
- [ ] キャラ状態表示の具体的なデータソースとUI
- [ ] 探索先アイコン一覧の具体的な仕様
- [ ] スレッド完了後の再閲覧可否（現実装: 完了後もサイドバーに表示、履歴閲覧可能）
- [ ] B型Wikiリンク遷移の遷移先コンテンツ定義
- [ ] C型成果物カードの種別一覧とアイコン仕様
