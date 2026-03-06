# AUDIT_04: アセット・データ

**点検日**: 2026-02-08

---

## 現状のアセット棚卸し

| カテゴリ | パス | 件数 | 備考 |
|---------|------|------|------|
| Topics | `Resources/Topics/` | 7 | デバッグ/プロトタイプ用 |
| Recipes | `Resources/Recipes/` | 4 | テスト用 |
| Images | `Resources/Images/` | 1 | `debug_image_01.png` のみ |
| ChatScenarios | `Resources/ChatScenarios/` | 2 | テスト用 SO |
| Yarn Scripts | `Resources/Yarn/` | 1 | `DebugScript.yarn` (32行) |
| Characters | `Resources/Characters/` | **0 (フォルダ自体が不在)** | ⚠️ |
| Prefabs (UI) | `Prefabs/UI/` | 4 | MessageBubble, TypingIndicator, DeductionBoard, TopicCard |
| Scenes | `Scenes/` | 3 | DebugChat, Title, Verification |

---

## AS-01: Resources/Characters/ フォルダが存在しない 🔴

- **内容**: `CharacterDatabase` は `Resources.LoadAll<CharacterProfile>("Characters")` でプロファイルをロードするが、`Resources/Characters/` ディレクトリ自体が存在しない
- **影響**: `CharacterDatabase.ProfileCount` が常に 0。全キャラクターがフォールバック色（青/グレー）で表示される
- **対応**: フォルダを作成し、最低限 `player` と NPC 1体分の CharacterProfile SO アセットを配置

---

## AS-02: ImageBubblePrefab が未作成 🟠

- **内容**: `ChatController.m_ImageBubblePrefab` に割り当てる専用 Prefab が `Prefabs/UI/` に存在しない
- **影響**: `AddImageMessage()` が `MessageBubblePrefab` にフォールバックし、画像表示が不安定（`ImageContent` 子オブジェクトが見つからない場合テキスト表示になる）
- **対応**: `ImageBubble.prefab` を作成（背景 Image + 子 `ImageContent` Image）

---

## AS-03: ChoiceButton Prefab の所在が不明 🟡

- **内容**: `ChatController.m_ChoiceButtonPrefab` に割り当てる Prefab が `Prefabs/UI/` に見当たらない
- **影響**: 選択肢表示機能が動作しない可能性（Inspector で手動割り当てされていれば問題ないが、Prefab として管理されていない）
- **対応**: `ChoiceButton.prefab` を `Prefabs/UI/` に作成・管理

---

## AS-04: SystemMessage 専用 Prefab がない 🟢

- **内容**: システムメッセージが `MessageBubblePrefab` を流用してスタイルを上書き（`AUDIT_01 CQ-08` と同根）
- **対応**: `SystemMessageBubble.prefab` を作成し、中央揃え・半透明グレー背景をデフォルトに

---

## AS-05: Yarn スクリプトがデバッグ用 1 ファイルのみ 🟡

- **内容**: `DebugScript.yarn` (32行) のみ。ゲーム本編のコンテンツが皆無
- **影響**: ゲームとしてのプレイ体験を評価できない
- **対応**: 中期 M3-1 でテンプレート作成後、本編コンテンツ制作に着手

---

## AS-06: Topic/Recipe アセットの命名規則が不統一 🟢

- **内容**:
  - Topics: `T_FoundPhone.asset`, `debug_topic_01.asset`, `topic_found_phone.asset` — 3種の命名規則が混在
  - Recipes: `Recipe_Test_DebugSignalPhone.asset`, `SynthesisRecipe_01.asset` — 2種の命名規則が混在
- **リスク**: アセット管理の混乱、検索性の低下
- **提案**: 統一命名規則を策定（例: `Topic_{PascalCaseID}.asset`, `Recipe_{PascalCaseID}.asset`）し、既存アセットをリネーム
