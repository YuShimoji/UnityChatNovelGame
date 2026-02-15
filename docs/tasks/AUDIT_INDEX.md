# プロジェクト総点検インデックス

**点検日**: 2026-02-08
**対象**: Project FoundPhone 全コードベース・ドキュメント・アセット
**参照**: `docs/PROJECT_ROADMAP.md`, `AI_CONTEXT.md`

---

## 点検結果サマリー

| カテゴリ | ファイル | 課題数 | 最重要度 |
|---------|---------|--------|---------|
| コード品質・技術的負債 | `AUDIT_01_CODE_QUALITY.md` | 12 | 🔴 高 |
| 未実装機能・仕様ギャップ | `AUDIT_02_FEATURE_GAPS.md` | 14 | 🔴 高 |
| テスト・品質保証 | `AUDIT_03_TESTING.md` | 8 | 🟡 中 |
| アセット・データ | `AUDIT_04_ASSETS.md` | 6 | 🟡 中 |
| アーキテクチャ・設計 | `AUDIT_05_ARCHITECTURE.md` | 5 | 🟠 高〜中 |

**総課題数**: 45件（重複排除済み）

---

## 優先度凡例

- 🔴 **高**: ゲームプレイに直接影響 / 放置するとブロッカーになる
- 🟠 **高〜中**: 中期的にリスクが顕在化する
- 🟡 **中**: 品質向上・開発効率に寄与
- 🟢 **低**: あると良い / 長期的改善

---

## クイックアクション（今すぐ着手可能な Top 5）

1. **`Resources/Characters/` フォルダが存在しない** → CharacterDatabase が空で動作。SOアセット作成が必要（`AUDIT_04`）
2. **ChatDialogueView 未実装** → Yarn Spinner との正式連携が不完全（`AUDIT_02`）
3. **ImageBubblePrefab 未作成** → 画像メッセージが MessageBubble にフォールバック中（`AUDIT_04`）
4. **テストカバレッジ ≈15%** → ChatController / ScenarioManager / DeductionBoard のテストが皆無（`AUDIT_03`）
5. **DeductionBoard.AddTopic の Show() が TODO** → トピック追加時の UI フィードバックなし（`AUDIT_01`）

---

## 既存タスクとの対応表

| 既存タスク | ステータス | 関連 AUDIT |
|-----------|-----------|-----------|
| TASK_025 (GC Alloc) | IN_PROGRESS（計測待ち） | AUDIT_01 #CQ-03 |
| TASK_027 (Playthrough) | IN_PROGRESS（手動待ち） | AUDIT_03 #QA-08 |
| S2-2 (ChatDialogueView) | 未着手 | AUDIT_02 #FG-01 |

---

*各ファイルの詳細は同ディレクトリ内の `AUDIT_0X_*.md` を参照してください。*
