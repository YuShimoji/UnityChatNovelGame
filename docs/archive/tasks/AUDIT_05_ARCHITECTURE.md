# AUDIT_05: アーキテクチャ・設計

**点検日**: 2026-02-08

---

## AR-01: シングルトン過多（3箇所） 🟠

- **該当クラス**:
  - `DeductionBoard` — `Assets/Scripts/UI/DeductionBoard.cs:14-34`
  - `SaveManager` — `Assets/Scripts/Core/SaveManager.cs`
  - `MetaEffectController` — `Assets/Scripts/Effects/MetaEffectController.cs`
- **追加**: `CharacterDatabase` も実質シングルトン（`Assets/Scripts/Data/CharacterDatabase.cs:13-28`）
- **問題点**:
  - 暗黙的な依存関係（`DeductionBoard` 内で `FindFirstObjectByType<ScenarioManager>()` を直接呼び出し: L271）
  - テスタビリティが低い（モック注入が困難）
  - シーン遷移時のライフサイクル管理が複雑化
- **提案**:
  - 短期: インターフェース抽出（`IDeductionBoard`, `ISaveManager` 等）でモック可能にする
  - 中期: ServiceLocator パターンまたは軽量 DI コンテナ（VContainer 等）の導入を検討

---

## AR-02: Resources.Load の多用（4クラス） 🟠

- **該当箇所**:
  - `ScenarioManager` — `Resources.Load<TopicData>`, `Resources.Load<Sprite>`, `Resources.Load<ChatScenarioData>`
  - `DeductionBoard` — `Resources.LoadAll<SynthesisRecipe>`
  - `CharacterDatabase` — `Resources.LoadAll<CharacterProfile>`
  - `MetaEffectController` — エフェクトアセットのロード
- **リスク**:
  - ビルドサイズ肥大化（Resources フォルダ内は全てビルドに含まれる）
  - ロード時間増大（起動時に全アセットがインデックス化される）
  - 参照の追跡が困難（文字列ベースのパス指定）
- **提案**:
  - 短期: 現状維持（アセット数が少ないため実害は小さい）
  - 中期: Addressables への段階的移行を計画（L2-1）
  - 移行順序: CharacterDatabase → DeductionBoard → ScenarioManager

---

## AR-03: シナリオシステムの二重構造 🟠

- **詳細**: `AUDIT_01 CQ-12` と同根
- **現状の構造**:
  ```
  ScenarioManager
  ├── Yarn Spinner 方式: RegisterCustomCommands() → DialogueRunner
  └── SO 方式: PlayScenario(ChatScenarioData) → PlayScenarioRoutine()
  ```
- **設計上の問題**:
  - 2つのシナリオ実行パスが独立しており、状態管理が分散
  - SO 方式は `m_IsInputLocked` を直接操作するが、Yarn 方式は Coroutine で制御
  - 選択肢処理のフローが異なる（Yarn: DialogueViewBase / SO: ChatController.ShowChoices）
- **提案**:
  - **方針決定が最優先**: Yarn Spinner を SSOT とするか、両方を活かすかを明文化
  - Yarn Spinner を SSOT とする場合: SO 方式を `[Obsolete]` マークし、段階的に移行
  - 両方活かす場合: 共通インターフェース `IScenarioPlayer` を定義し、状態管理を統一

---

## AR-04: ChatController の責務過多 🟡

- **対象**: `Assets/Scripts/UI/ChatController.cs` (628行)
- **現在の責務**:
  1. メッセージバブルの生成・配置
  2. 画像バブルの生成・配置
  3. システムメッセージの生成
  4. スクロール制御（自動スクロール + ユーザー操作検知）
  5. タイピングインジケーター制御
  6. 選択肢 UI の表示・非表示
  7. ユーザー入力（InputField + SendButton）
  8. メッセージ履歴のクリア
- **提案**: 以下のように責務を分離
  - `MessageFactory` — バブル生成・スタイル適用
  - `ScrollController` — スクロール制御
  - `ChatInputHandler` — ユーザー入力処理
  - `ChatController` — 上記を統合するファサード

---

## AR-05: asmdef 構成の整理余地 🟢

- **現状の asmdef**:
  - `ProjectFoundPhone` (メイン)
  - `ProjectFoundPhone.Tests`
  - `ProjectFoundPhone.Editor`
  - `ProjectFoundPhone.Dev`
  - `ProjectFoundPhone.Utils`
- **問題点**:
  - `Dev` と `Debug` の境界が曖昧（`Debug/CustomLogger.cs` は `Dev` asmdef に含まれていない）
  - `Utils/PerformanceMonitor.cs` と `Utils/VerificationCapture.cs` は開発ツールだが `Utils` asmdef に配置
- **提案**:
  - `Debug/` 配下を `Dev` asmdef に統合するか、`Debug` 用の asmdef を新設
  - `Utils` の中身を精査し、ランタイム必須のユーティリティと開発ツールを分離
