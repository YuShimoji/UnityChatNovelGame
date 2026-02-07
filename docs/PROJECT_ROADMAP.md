# Project Roadmap: Unity Chat Novel Game (Project FoundPhone)

**作成日**: 2026-02-07
**分析対象**: コードベース全体、仕様書、AI_CONTEXT.md、HANDOVER.md、全37スクリプト

---

## 1. 現状分析サマリー

### 1.1 完了済み機能一覧

| カテゴリ | 機能 | 状態 | 備考 |
|---------|------|------|------|
| **Core** | ScenarioManager | ✅ | Yarn Spinner連携 + ScriptableObjectベース両対応 |
| **Core** | SaveManager | ✅ | 3スロット、JSON永続化 |
| **UI** | ChatController | ✅ | ScrollRect + DOTween自動スクロール |
| **UI** | DeductionBoard | ✅ | トピック管理 + 合成システム |
| **UI** | SaveLoadUI / SaveSlotUI | ✅ | セーブ/ロードUI |
| **UI** | TitleScreenManager | ✅ | ニューゲーム/ロード/終了 |
| **UI** | TopicCard | ✅ | トピックカード表示 |
| **Data** | TopicData | ✅ | ScriptableObject、7アセット作成済 |
| **Data** | SynthesisRecipe | ✅ | 4レシピ作成済 |
| **Data** | ChatScenarioData | ✅ | SO方式のシナリオ（2アセット） |
| **Data** | SaveData | ✅ | バージョン管理・バリデーション付き |
| **Effects** | MetaEffectController | ✅ | グリッチ + 汎用エフェクト |
| **Effects** | GlitchEffect | ✅ | レベル1-5対応 |
| **Scene** | DebugChatScene | ✅ | デバッグ用統合シーン |
| **Scene** | TitleScene | ✅ | タイトル画面 |
| **Scene** | VerificationScene | ✅ | 検証用 |
| **Yarn** | DebugScript.yarn | ✅ | 基本コマンドテスト用（32行） |
| **Test** | SaveSystemTests | ✅ | 6テストケース |
| **Editor** | 各種ツール | ✅ | 12種のエディタツール |

### 1.2 進行中タスク

- **TASK_025**: GC Alloc Reduction — コード実装済み、After計測待ち
- **TASK_027**: Full Playthrough Test — 手動テスト実行待ち

---

## 2. 課題・技術的負債の詳細分析

### 2.1 アーキテクチャ上の課題

#### A. シナリオシステムの二重構造（重要度: 高）

現在、2つのシナリオシステムが共存している:

1. **Yarn Spinner方式**: `DialogueRunner` + カスタムコマンド（`<<Message>>`, `<<Image>>` 等）
2. **ScriptableObject方式**: `ChatScenarioData` + `PlayScenarioRoutine()`

```
ScenarioManager.cs:
  - RegisterCustomCommands() → Yarn Spinner系
  - PlayScenario(ChatScenarioData) → SO系
```

**リスク**: どちらが本番用かが不明確。メンテナンスコストが二重に発生する。
**提案**: Yarn Spinner を SSOT として統一し、ChatScenarioData はプロトタイプ用/フォールバックとして位置づけを明確化する。

#### B. シングルトン過多（重要度: 中）

3つのシングルトンが存在: `DeductionBoard`, `SaveManager`, `MetaEffectController`

- `DeductionBoard` 内で `FindFirstObjectByType<ScenarioManager>()` を直接呼び出している（L271）
- 依存関係が暗黙的で、テスタビリティが低い

**提案**: ServiceLocator パターンまたは軽量DIの導入を検討。最低限、インターフェース抽出でモック可能にする。

#### C. Resources.Load の多用（重要度: 中）

`ScenarioManager`, `DeductionBoard`, `MetaEffectController`, `SaveManager` の4クラスで `Resources.Load` を使用。

**リスク**: ビルドサイズ肥大化、ロード時間増大、参照の追跡困難。
**提案**: 中期でAddressablesへの移行を計画。

### 2.2 実装ギャップ（仕様書 vs 実装）

| 仕様書記載の機能 | 実装状態 | 優先度 |
|-----------------|---------|--------|
| `CharacterProfile` ScriptableObject | ❌ 未実装 | **高** |
| `ChatDialogueView` (DialogueViewBase継承) | ❌ 未実装 | **高** |
| 画像バブル（`send_image`） | ⚠️ スタブのみ（テキスト代替） | **高** |
| `system_message` コマンド | ❌ 未実装 | 中 |
| `ChangeStatus` コマンド | ❌ 未実装 | 中 |
| `add_contact` コマンド | ❌ 未実装 | 中 |
| 連絡先リスト機能 | ❌ 未実装 | 中 |
| キーボード/SafeArea対応 | ❌ 未実装 | 中 |
| オブジェクトプーリング（MessageBubble） | ❌ 未実装 | 中 |
| 動的タイムスタンプ | ❌ 未実装 | 低 |
| ライトボックス画像表示 | ❌ 未実装 | 低 |
| オートセーブ | ❌ 未実装 | 低 |
| データ暗号化 | ❌ 未実装 | 低 |
| クラウドセーブ | ❌ 未実装 | 低 |

### 2.3 コード品質の課題

| 問題 | 影響箇所 | 詳細 |
|------|---------|------|
| **SaveData の Dictionary シリアライズ** | `SaveData.cs:40` | `Dictionary<string, object>` は `JsonUtility` で正しくシリアライズできない。Newtonsoft.Json への切替またはカスタムシリアライザが必要 |
| **ImageCommand がスタブ** | `ScenarioManager.cs:165` | `[Image: {imageID}]` とテキスト表示するだけで、画像表示未実装 |
| **毎フレームのスクロールチェック** | `ChatController.cs:61` | `Update()` 内で `CheckUserScrollInput()` を毎フレーム実行。イベント駆動に変更可能 |
| **StartWait の進行制御不完全** | `ScenarioManager.cs:191-193` | DialogueRunner の進行一時停止が TODO コメントのまま |
| **TopicData.OnValidate が空** | `TopicData.cs:43-45` | バリデーションが TODO のまま |
| **SynthesisRecipe.OnValidate が空** | `SynthesisRecipe.cs:37-40` | 循環参照チェック等が TODO のまま |

### 2.4 テストカバレッジ

| モジュール | テスト有無 | テスト数 |
|-----------|----------|---------|
| SaveSystem | ✅ | 6 |
| ChatController | ❌ | 0 |
| ScenarioManager | ❌ | 0 |
| DeductionBoard | ⚠️ 検証スクリプトのみ | 0 (正式テストなし) |
| TopicData | ❌ | 0 |
| SynthesisRecipe | ❌ | 0 |
| MetaEffectController | ❌ | 0 |
| TitleScreenManager | ❌ | 0 |

**テストカバレッジ推定**: 約 10-15%（SaveSystem のみ）

### 2.5 コンテンツ状況

- **Yarnスクリプト**: 1ファイル（DebugScript.yarn、32行）— ゲーム本編のコンテンツは皆無
- **トピック**: 7アセット（デバッグ/プロトタイプ用）
- **レシピ**: 4アセット
- **シナリオ**: 2アセット（テスト用）
- **画像素材**: 未確認（Resources/Images にデバッグ画像があるかどうか不明）

---

## 3. 伸びしろ（Opportunity Areas）

### 3.1 ゲーム体験の差別化ポイント

1. **合成システム（DeductionBoard）**: トピック合成は「Lost Phone系」ゲームの中では独自性がある。ドラッグ&ドロップUIが実装されれば強力な差別化要素になる
2. **メタ演出（Glitch）**: 第四の壁を破る演出は没入感を高める。段階的なグリッチレベル対応は既に基盤がある
3. **二重シナリオシステム**: 現状は課題だが、Yarn Spinner（複雑な分岐）とSO方式（簡易イベント）の使い分けとして活かせる可能性がある

### 3.2 技術的伸びしろ

1. **Addressables + アセットバンドル**: コンテンツの動的ロード基盤を整えれば、DLCやエピソード配信が可能に
2. **UI Toolkit移行**: 将来的にuGUIからUI Toolkitへ移行すれば、CSS的なスタイリングとデータバインディングが使える
3. **自動テスト基盤**: Unity Test Framework 2.0.1 が導入済み。EditMode/PlayMode テストの拡充余地が大きい
4. **MCP for Unity**: 既にプロジェクトに導入済み。AI支援開発のパイプラインとして活用可能

---

## 4. ロードマップ

### 4.1 短期プラン（1-2週間）— 品質基盤の確立

> **目標**: 既存実装の品質を担保し、次フェーズの開発基盤を固める

#### Sprint S1: ブロッカー解消 & テスト基盤（3-5日）

| # | タスク | 優先度 | 工数目安 | 依存 |
|---|--------|--------|---------|------|
| S1-1 | TASK_025 GC Alloc After計測の実施 | 高 | 0.5日 | Unity Editor |
| S1-2 | TASK_027 Full Playthrough 手動テスト実行 | 高 | 1日 | Unity Editor |
| S1-3 | **SaveData シリアライズ修正** — `Dictionary<string, object>` を Newtonsoft.Json ベースに切替、またはシリアライズ可能な構造体へ変換 | 高 | 0.5日 | — |
| S1-4 | **CharacterProfile ScriptableObject 作成** — 仕様書に記載された `characterID`, `displayName`, `icon`, `themeColor`, `isPlayer` を持つSO定義 | 高 | 0.5日 | — |
| S1-5 | **ChatController / ScenarioManager のEditModeテスト追加** — 最低各3ケース | 高 | 1日 | — |
| S1-6 | **DeductionBoard / SynthesisRecipe のEditModeテスト追加** — 合成ロジックの検証 | 中 | 0.5日 | — |

#### Sprint S2: 仕様ギャップ解消（3-5日）

| # | タスク | 優先度 | 工数目安 | 依存 |
|---|--------|--------|---------|------|
| S2-1 | **ImageCommand の実装完了** — 画像バブルPrefab作成、Resources/Images からの実際のSprite表示 | 高 | 1日 | S1-4 |
| S2-2 | **ChatDialogueView の実装** — `DialogueViewBase` を継承し、Yarn SpinnerのDialogue Runnerと正式連携 | 高 | 1.5日 | — |
| S2-3 | **system_message コマンド実装** — 中央揃えグレーテキストのシステム通知 | 中 | 0.5日 | S2-2 |
| S2-4 | **StartWait の進行制御修正** — DialogueRunner の一時停止を正式実装 | 中 | 0.5日 | S2-2 |
| S2-5 | **TopicData / SynthesisRecipe の OnValidate 実装** — バリデーションロジック追加 | 低 | 0.5日 | — |

### 4.2 中期プラン（2-6週間）— 機能拡充 & コンテンツ制作準備

> **目標**: ゲームとしての体験を形作り、コンテンツ制作が開始できる状態にする

#### Phase M1: UI/UX ポリッシュ（1-2週間）

| # | タスク | 優先度 | 詳細 |
|---|--------|--------|------|
| M1-1 | **MessageBubble のオブジェクトプーリング** | 高 | 長いチャットログでのパフォーマンス確保。`Queue<GameObject>` ベースのプール実装 |
| M1-2 | **CharacterProfile ベースのバブルカラーリング** | 高 | キャラごとのテーマカラー、アイコン表示、左右配置の自動化 |
| M1-3 | **メッセージアニメーション強化** — Scale + Slide + Fade の三重アニメーション | 中 | DOTween Sequence で仕様書通りの「ポンッ」演出 |
| M1-4 | **SaveLoadUI ビジュアルデザイン** | 中 | スロット選択画面のデザイン改善 |
| M1-5 | **Options パネル実装** — 音量、テキスト速度、言語（将来） | 中 | TitleScreenManager.OpenOptions() の実装 |
| M1-6 | **Safe Area / キーボード対応** | 中 | モバイル対応の基盤 |

#### Phase M2: ゲームシステム拡張（1-2週間）

| # | タスク | 優先度 | 詳細 |
|---|--------|--------|------|
| M2-1 | **連絡先リスト（Contact List）実装** — `add_contact` コマンド対応 | 高 | Lost Phone系の中核機能。複数キャラとのチャット切替 |
| M2-2 | **ChangeStatus コマンド実装** — オンライン/オフライン/取り込み中 | 中 | キャラクターの状態管理 |
| M2-3 | **オートセーブ機能** — `OnApplicationPause` + 重要ポイント自動保存 | 中 | モバイルでは必須 |
| M2-4 | **動的タイムスタンプ** — ゲーム内時間管理システム | 低 | 初期は現在時刻で代替可 |
| M2-5 | **画像ライトボックス** — サムネイルタップで拡大表示 | 低 | 演出強化 |

#### Phase M3: コンテンツ制作パイプライン（1週間）

| # | タスク | 優先度 | 詳細 |
|---|--------|--------|------|
| M3-1 | **Yarn スクリプトテンプレート作成** — Chapter構造、コマンド使用例、ベストプラクティス | 高 | コンテンツ制作の生産性向上 |
| M3-2 | **CharacterProfile の本番データ作成** — メインキャラ3-5人分 | 高 | ストーリー制作の前提 |
| M3-3 | **Topic/Recipe のマスターデータ設計** — ゲーム全体のトピックツリー設計 | 高 | 合成パズルのゲームデザイン |
| M3-4 | **エディタツール強化** — Yarnスクリプトプレビューア、トピックグラフビューア | 中 | コンテンツ制作効率化 |

### 4.3 長期プラン（2-6ヶ月）— プロダクション & リリース準備

> **目標**: リリース可能な品質のゲームを完成させる

#### Phase L1: コンテンツプロダクション（1-3ヶ月）

| # | タスク | 詳細 |
|---|--------|------|
| L1-1 | **メインストーリー制作** — 3-5チャプター、各30-60分のプレイ時間 |
| L1-2 | **キャラクターアート** — アイコン、背景画像、演出用画像の制作 |
| L1-3 | **SE/BGM** — チャット通知音、BGM、グリッチSE |
| L1-4 | **ローカライズ基盤** — 日本語/英語の多言語対応 |

#### Phase L2: 技術的成熟（並行実施）

| # | タスク | 詳細 |
|---|--------|------|
| L2-1 | **Addressables 移行** — Resources.Load からの脱却、アセットバンドル化 |
| L2-2 | **セーブデータ暗号化** — AES暗号化 + 改ざん検知 |
| L2-3 | **クラウドセーブ** — Unity Cloud Save または Firebase 連携 |
| L2-4 | **パフォーマンス最適化** — プロファイリング、メモリ最適化、ロード時間短縮 |
| L2-5 | **CI/CD パイプライン** — 自動ビルド、テスト実行、デプロイ |

#### Phase L3: リリース準備

| # | タスク | 詳細 |
|---|--------|------|
| L3-1 | **QA テスト** — 全プラットフォーム（iOS/Android/PC）の動作検証 |
| L3-2 | **ストア準備** — App Store / Google Play 申請素材 |
| L3-3 | **Analytics 導入** — プレイヤー行動トラッキング |
| L3-4 | **クラッシュレポート** — Firebase Crashlytics 等の導入 |

---

## 5. リスク & 対策マトリクス

| リスク | 影響度 | 発生確率 | 対策 |
|--------|--------|---------|------|
| SaveData の Dictionary シリアライズ不具合 | **高** | **高** | S1-3 で即座に修正 |
| Yarn Spinner バージョン固定なし（GitHub直参照） | 中 | 中 | manifest.json で特定コミット/タグを指定 |
| シナリオ二重構造による保守コスト増大 | 中 | 高 | 短期で方針を明確化し、ドキュメントに記録 |
| テスト不足による回帰バグ | **高** | **高** | S1-5, S1-6 でテスト基盤を確立 |
| コンテンツ制作ボトルネック | **高** | 中 | M3 でパイプラインを整備 |
| モバイル特有の問題（SafeArea、キーボード等） | 中 | 高 | M1-6 で早期対応 |

---

## 6. 推奨する次の一手（Top 3）

1. **SaveData のシリアライズ修正**（S1-3）: 現状のまま放置するとセーブ/ロードが本番環境で壊れる。`Dictionary<string, object>` は `JsonUtility` で扱えないため、最優先で修正が必要。

2. **テストカバレッジ拡充**（S1-5, S1-6）: コアロジック（合成判定、メッセージ追加、セーブ/ロード統合）のテストがないと、今後の変更で回帰バグが発生するリスクが高い。

3. **CharacterProfile SO の導入**（S1-4）: 仕様書の中核データ構造が未実装。これがないとキャラごとのバブルカラーやアイコン表示が実現できず、中期のUI/UXポリッシュ全体がブロックされる。

---

## 7. メトリクス（現在値 → 目標値）

| メトリクス | 現在値 | 短期目標 | 中期目標 | 長期目標 |
|-----------|--------|---------|---------|---------|
| スクリプト数 | 37 | 42+ | 55+ | 70+ |
| テストケース数 | 6 | 20+ | 40+ | 80+ |
| テストカバレッジ | ~15% | 40%+ | 60%+ | 80%+ |
| Yarn スクリプト行数 | 32 | 100+ | 1000+ | 5000+ |
| トピック数 | 7 | 10+ | 30+ | 50+ |
| レシピ数 | 4 | 6+ | 20+ | 30+ |
| キャラクター数 | 0 (SO) | 3+ | 5+ | 8+ |
| シーン数（本番） | 2 | 3 | 5+ | 8+ |

---

*このロードマップは AI_CONTEXT.md, HANDOVER.md, 全仕様書, 全37スクリプトの分析に基づいて作成されました。プロジェクトの進捗に応じて定期的に見直すことを推奨します。*
