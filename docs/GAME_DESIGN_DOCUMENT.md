# Game Design Document: Project FoundPhone

**ステータス**: Living Document（正規仕様書）
**最終更新**: 2026-02-10
**統合元**: Core Specification, Core Specification2, Core System仕様書, ストーリーシステムブレスト, 拡張仕様書, ミニゲーム案, サブストーリー1, サブストーリー2（※演出案はブレスト扱いで specs に保管）

> このドキュメントはプロジェクトの**唯一の正規仕様書（SSOT）**です。
> 個々の機能の実装ステータスは `AI_CONTEXT.md`、タスク詳細は `docs/tasks/` を参照してください。

---

## 0. ゲームコンセプト（30秒エレベーターピッチ）

**「スマホの中の会話が、現実を少しずつ侵食していく」**

プレイヤーは架空のスマートフォンを操作し、メッセージアプリを通じてキャラクターと対話する。
物語は**手作りのシナリオ（自動生成なし）**で進行し、選択や情報の提示によって分岐する。
**チャットノベル × サスペンス/ホラー演出**の体験を軸にする。

このプロジェクトは**チャットノベルゲームエンジンとしてのコア機能**を重視する。
物語は「メタホラー」「FoundPhone」など**演出テーマを分離可能**にし、エンジンは共通基盤として成立させる。
必要に応じて、**サンプルストーリーの要求から標準機能を逆進的に追加**する方針を許容する。

### ジャンル
- Found Phone スタイルのチャットアドベンチャー（拾った設定に限定しない）
- 物語主導のチャットノベル
- ホラー/サスペンス演出（機能に紐づく範囲で使用）

### ターゲットプラットフォーム
- iOS / Android（主軸）
- PC（Steam / itch.io）

### プレイ時間目安
- メインストーリー: 3-5チャプター × 各30-60分

---

## 1. テクノロジースタック

| レイヤー | 技術 | 備考 |
|---------|------|------|
| エンジン | Unity 2022.3 LTS+ | |
| 言語 | C# | |
| シナリオエンジン | **Yarn Spinner** (v3.1.3) | SSOT。全会話は .yarn ファイルで管理 |
| UIアニメーション | DOTween Pro | |
| データ（静的） | ScriptableObject | TopicData, SynthesisRecipe, CharacterProfile 等 |
| データ（セーブ） | JSON (Newtonsoft.Json) | 3スロット + オートセーブ |
| テキスト描画 | TextMeshPro | Noto Sans JP + Material Icons |
| テスト | Unity Test Framework 2.0.1 | EditMode / PlayMode |

---

## 2. アーキテクチャ

### 2.1 設計原則

- **MVC パターン**準拠
- **SOLID 原則**を遵守
- **会話データのハードコーディング禁止** — 全てYarnファイルで管理
- **シナリオは手作り** — 物語/謎の自動生成は行わない
- **トピックは消費されない** — 使用後も保持され、別文脈で再利用可能

### 2.2 コアクラス構成

```
[Model層]
  CharacterProfile (SO)     … キャラの静的データ（ID, 名前, アイコン, テーマカラー）
  TopicData (SO)             … 手がかりデータ（ID, タイトル, 説明, 属性タグ）
  SynthesisRecipe (SO)       … 合成レシピ（入力A + 入力B → 出力）
  ChatScenarioData (SO)      … SOベースシナリオ（プロトタイプ/フォールバック用）
  SaveData                   … セーブデータ構造体

[View層]
  ChatController             … チャット画面全体（ScrollRect + VerticalLayoutGroup）
  MessageBubble              … 個々の吹き出しPrefab（左/右自動切替）
  TopicCard                  … トピックカード表示

[Controller層]
  ScenarioManager            … Yarn Spinnerラッパー。カスタムコマンド処理の司令塔
  SaveManager (Singleton)    … セーブ/ロード管理
  MetaEffectController       … グリッチ等のメタ演出制御
  TitleScreenManager         … タイトル画面制御
```

### 2.3 シナリオシステム方針

| 方式 | 用途 | 位置づけ |
|------|------|---------|
| **Yarn Spinner** | 本編シナリオ、分岐、フラグ管理 | **正式（SSOT）** |
| ScriptableObject (ChatScenarioData) | 簡易イベント、チュートリアル | プロトタイプ/フォールバック |

> Yarn Spinner を正とし、ChatScenarioData は段階的に縮小する。

---

## 3. ゲームシステム

### 3.1 チャットシステム（コアループ）

プレイヤーはメッセージアプリを通じてキャラクターと対話する。

#### メッセージ表示
- **左バブル（NPC）**: 左寄せ、グレー背景（or キャラテーマカラー）、アイコンあり
- **右バブル（プレイヤー）**: 右寄せ、アクセントカラー、アイコンなし
- **システムメッセージ**: 中央揃え、グレーテキスト（「グループに参加しました」等）
- 吹き出しは**画面幅の70%を最大幅**とする
- テキスト量に応じて縦方向に自動伸縮（ContentSizeFitter）
- 背景は9-Slice Sprite（`Bubble_Base.png`）をキャラカラーで着色

#### アニメーション（DOTween）
- **出現**: Scale 0→1 (OutBack, 0.3s)＋Fade＋横スライド（左から/右から）
- **タイピングインジケーター**: 3ドットのSin波ループ → メッセージ受信で消滅
- **自動スクロール**: 新メッセージ追加時に最下部へ。ただしユーザーが過去ログ閲覧中は強制しない

#### Yarnカスタムコマンド

| コマンド | 書式 | 機能 |
|---------|------|------|
| `<<Message>>` | `<<Message "CharID" "Text">>` | 指定キャラからのメッセージ表示 |
| `<<Image>>` | `<<Image "CharID" "ImageName">>` | 画像メッセージ送信 |
| `<<Wait>>` / `<<StartWait>>` | `<<Wait seconds>>` | 指定秒数待機（入力中演出付き） |
| `<<Typing>>` | `<<Typing seconds>>` | 「入力中...」表示 |
| `<<ChangeStatus>>` | `<<ChangeStatus "CharID" "Status">>` | オンライン/オフライン/取り込み中 |
| `<<Glitch>>` | `<<Glitch "Level" duration>>` | グリッチエフェクト発動（Level 1-5） |
| `<<UnlockTopic>>` | `<<UnlockTopic "TopicID">>` | トピック獲得 |
| `<<PresentTopic>>` | `<<PresentTopic>>` | プレイヤーにトピック選択を促す |
| `<<SystemMessage>>` | `<<SystemMessage "Text">>` | システム通知表示 |
| `<<AddContact>>` | `<<AddContact "CharID">>` | 連絡先追加 |

---

### 3.2 トピックシステム（情報の多層化）

手がかりは「使い捨ての鍵」ではなく「再利用可能な化学物質」。

#### データ構造
```csharp
public class TopicData : ScriptableObject {
    public string id;            // "T_Photo_1004"
    public string title;         // "10月4日の写真"
    public Sprite icon;
    [TextArea] public string description;
    public bool isPhysicalEvidence;    // 物証か
    public bool isEmotionalLeverage;   // 感情的揺さぶりに使えるか
    public string[] relatedCharacterIDs;
}
```

#### 設計原則
- **1トピックに複数用途**: 事実（Fact）＋文脈（Context）の二面性
- **消費されない**: 合成に使っても消滅しない
- **HashSet\<string\>** で管理（アイテム管理ではなくロック解除管理）

---

### 3.3 ウェイティングシステム（時間管理）

「待たせる」のではなく「プレイヤーが動くと時間が進む」仕組み。

#### 基本挙動
- チャット相手がオフライン/調査中 → **最長15秒**のカウントダウン
- **放置**: 15秒で自動進行
- **行動**: ミニゲームクリア・別アプリ閲覧で即完了

#### 状態遷移
```
Idle（通常会話中）→ Waiting（カウントダウン中）→ Ready（通知表示 → タップで再開）
```

#### UI演出
- プログレスバー「データ解析中... 45%」でポジティブ待機感
- 完了時は通知バナー「新着メッセージ：[キャラ名]」＋振動

---

### 3.4 探索スレッドシステム（サブコンテンツ）

メインストーリーとは独立した「クエスト」。TRPG的なルールを適用する。

#### コンセプト：リモート・ナビゲーション
- プレイヤーは現地に行けない。現地のキャラにチャットで指示を出す
- 『Lifeline』的な緊張感のあるコマンド選択

#### スレッドリスト画面
- タイトル、サムネイル、難易度（★1-5）、目安時間
- ステータス: Locked → New → Cleared / Failed

#### アンロック条件
- トピック所持
- シナリオ進行度
- キャラクター信頼度

#### 探索中のリソース管理
- リソース管理は**簡易・低負荷**にする（詳細は後で決定）
- **有効/無効を切り替え可能**な設計とし、MVPでは無効化する
- バッテリー管理は当面除外し、必要なら後から復活可能とする
- 候補リソースは**未定（ブレストで検討）** ← TBD

#### 報酬と接続
- クリア: 高レアリティのトピック（証拠）獲得 → メインストーリー進行に必要
- 失敗: 小規模トピック獲得 + リトライ可能（クールタイムあり）

#### データ構造
```csharp
[CreateAssetMenu(menuName = "Game/ExplorationThread")]
public class ExplorationThreadData : ScriptableObject {
    public string id;
    public string title;
    [TextArea] public string description;
    public Sprite thumbnail;
    public int difficultyStars;       // 1-5
    public string estimatedTime;
    public TopicData requiredTopic;   // アンロック条件
    public string startNodeName;      // Yarn開始ノード
    public CharacterProfile explorer; // 探索者キャラ
    public TopicData[] rewardTopics;  // クリア報酬
}
```

---

### 3.5 ミニゲーム（待ち時間ブースター）

待ち時間を「短縮する」ための軽量ゲーム。内容は**候補**であり決定稿ではない。

#### 例: 波形シンクロ（Signal Sync）
- ターゲット波形にスライダーで自分の波形を合わせる
- LineRendererでSin波描画、`frequency`/`amplitude`をスライダー操作
- ノイズ → クリア音への変化で気持ちよさを演出
- プレイ時間: 3-8秒
- 用途: 通信傍受、盗聴、データ解析シーン

#### その他候補
| 案 | テーマ | プレイ時間 | 実装難度 |
|----|--------|-----------|---------|
| 回転デクリプト | ファイルロック解除 | 5-10秒 | ★★☆ |
| ヘックス・ハント | データ解析 | 10秒 | ★☆☆ |
| ノード接続パズル | 回路修理 | 5-10秒 | ★☆☆ |

---

## 4. 演出・メタ要素

### 4.1 ビジュアルグリッチ（5段階）

シナリオフラグに応じてブレンド。

| レベル | エフェクト | トリガー |
|--------|-----------|---------|
| 1 | 微かな色収差（Chromatic Aberration） | 不穏な話題 |
| 2 | 走査線ノイズ（Scanline Jitter） | 敵対存在からのメッセージ |
| 3 | 画面揺れ + ノイズ強化 | 重要な発見 |
| 4 | ピクセルソーティング | 危険な画像を開いた瞬間 |
| 5 | デジタル崩壊（Datamosh） | ゲームオーバー直前 |

### 4.2 入力ハイジャック

- **ゴーストタイピング**: プレイヤーの入力を無視し、「逃げろ」等が勝手に入力される
- **UIの逃走**: 「通話を切る」ボタンが逃げる
- **偽システムダイアログ**: 「ストレージが破損しています」（「はい」しか選べない）

### 4.3 サウンド・ハプティクス

- **バイノーラルサウンド**: 心霊現象はパンを振って「耳の後ろ」から
- **幻の振動**: 通知がないのに一瞬だけ振動 → 過敏化
- **心拍振動**: 緊迫シーンで心臓の鼓動リズムの微弱振動

### 4.4 安全弁（アクセシビリティ）

- 激しい点滅/揺れは設定でOFF可能
- iOS/Androidサンドボックス規約を遵守（カメラ起動等はフェイクのみ）
- 「日常パート（正常）」と「異常パート」のメリハリをフラグ管理

---

## 5. データ管理

### 5.1 キャラクター定義

```csharp
public class CharacterProfile : ScriptableObject {
    public string characterID;    // Yarnスクリプト内の名前と一致
    public string displayName;
    public Sprite icon;
    public Color themeColor;      // バブルのベースカラー
    public bool isPlayer;
}
```

### 5.2 セーブシステム

- **3スロット** + オートセーブ（OnApplicationPause + 重要ポイント）
- Newtonsoft.Json によるシリアライズ

```json
{
  "YarnVariables": { ... },
  "ChatHistory": [
    { "CharID": "A", "Type": "Text", "Content": "Hello", "Timestamp": "10:00" }
  ],
  "CurrentNode": "Chapter1_Start",
  "UnlockedTopicIDs": ["T_Photo_1004", "T_Contradiction_A"],
  "SystemSettings": { "Volume": 0.8, "TextSpeed": 1.0 }
}
```

---

## 6. UI/UX要件

### 6.1 レスポンシブ対応
- 縦横比が異なるモバイル端末に対応
- SafeArea対応（ノッチ）
- ソフトウェアキーボード出現時のScrollRect調整

### 6.2 チュートリアル / オンボーディング
- ゲーム開始時はタイトル画面なしで「ロック画面」から始めるオプション

### 6.3 アセット要件

| アセット | 仕様 | 用途 |
|---------|------|------|
| `Bubble_Base.png` | 9-Slice (Border 20px) | 全メッセージ背景 |
| `Avatar_Mask.png` | 円形マスク | アイコン切り抜き |
| Noto Sans JP | 本文フォント | UIテキスト全般 |
| Material Icons | アイコンフォント | ボタン・装飾 |

---

## 7. ゲームループ総括

```
┌─────────────────────────────────────────────┐
│                メインループ                   │
│                                              │
│  ① チャット（進行）                           │
│     キャラと対話 → 物語が進む                 │
│          ↓                                   │
│  ② 情報の提示/選択                            │
│     トピック提示・選択 → 分岐                 │
│          ↓                                   │
│  ③ 必要に応じて補助システム                   │
│     探索スレッド / ミニゲーム                  │
│          ↓                                   │
│  ④ チャットに戻る                             │
└─────────────────────────────────────────────┘
```

---

## 8. デバッグ機能

| 機能 | 用途 |
|------|------|
| 時間操作（SkipWait） | 待ち時間を強制0 |
| トピック全開放 | 全トピック所持状態 |
| Yarnノードジャンプ | 任意シーンから開始 |
| SaveSystemDebugger | Editorウィンドウからセーブ操作 |
| GC Alloc計測 | パフォーマンスプロファイリング |

---

## 付録A: ドキュメント構成ガイド

```
docs/
├── GAME_DESIGN_DOCUMENT.md    ← このファイル（正規仕様書・SSOT）
├── PROJECT_ROADMAP.md         ← 実装ロードマップ・スケジュール
├── HANDOVER.md                ← 引き継ぎ用ステータスシート
├── MCP_SETUP.md               ← MCP接続ガイド
├── SaveSystem_README.md       ← セーブシステム実装ガイド
├── specs/                     ← 旧仕様メモ（アーカイブ）
│   ├── _ARCHIVED_Core_Specification.md
│   ├── _ARCHIVED_Core_Specification2.md
│   ├── _ARCHIVED_Core_System仕様書.md
│   ├── _ARCHIVED_ストーリーシステムブレスト.md
│   ├── _ARCHIVED_演出案.md
│   ├── _ARCHIVED_拡張仕様書.md
│   ├── _ARCHIVED_ミニゲーム案.md
│   ├── _ARCHIVED_サブストーリー1.md
│   └── _ARCHIVED_サブストーリー2.md
├── tasks/                     ← 個別タスク定義
├── reports/                   ← タスク完了レポート
└── evidence/                  ← スクリーンショット等
```

---

## 付録B: 用語集

| 用語 | 定義 |
|------|------|
| トピック (Topic) | プレイヤーが収集する手がかり。消費されず再利用可能 |
| 合成 (Synthesis) | 2つのトピックを組み合わせて新しいトピックを解放する行為 |
| 探索スレッド (Exploration Thread) | サブクエスト。NPCに遠隔指示を出すTRPG的パート |
| グリッチ (Glitch) | 画面のデジタル的破損演出（レベル1-5） |
| メタ演出 | 第四の壁を破る演出（ゴーストタイピング等） |
| SSOT | Single Source of Truth（唯一の正規情報源） |

