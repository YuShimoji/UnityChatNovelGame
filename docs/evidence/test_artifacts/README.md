# Test Artifacts Directory

**作成日**: 2026-02-07

このディレクトリは、プロジェクトのテスト・検証中に生成されるスクリーンショットやログを格納するための専用フォルダです。

---

## ディレクトリ構造

```
test_artifacts/
├── README.md          # このファイル
├── screenshots/       # スクリーンショット格納用
├── recordings/        # 動画/GIF格納用
├── logs/              # テストログ格納用
└── reports/           # テストレポート格納用
```

---

## 命名規則

### スクリーンショット

```
{YYYY-MM-DD}_{機能名}_{状態}.{png|jpg}
```

例:

- `2026-02-07_ChatUI_Initial.png`
- `2026-02-07_DeductionBoard_AfterSynthesis.png`
- `2026-02-07_TitleScreen_MainMenu.png`

### 動画/GIF

```
{YYYY-MM-DD}_{機能名}_{テスト内容}.{mp4|gif|webp}
```

例:

- `2026-02-07_ChatFlow_MessageAnimation.gif`
- `2026-02-07_Synthesis_TopicMerge.mp4`

### ログファイル

```
{YYYY-MM-DD}_{テストタイプ}_{結果}.{txt|log}
```

例:

- `2026-02-07_UnitTest_Pass.txt`
- `2026-02-07_PlaythroughTest_Issues.log`

---

## 使用方法

1. **スクリーンショットの保存**
   - Unity Editorの Game ビューから `Screenshot Utility` を使用
   - または `Window > Analysis > Screen Capture` を使用
   - 保存先をこのディレクトリのサブフォルダに指定

2. **テストレポートの作成**
   - テスト実行後、結果を `reports/` に Markdown 形式で保存
   - 関連するスクリーンショットへのリンクを含める

3. **Git管理**
   - このディレクトリ配下のファイルはGit管理対象
   - 大きなファイル（10MB超）は `.gitignore` に追加を検討

---

## 現在の状態（2026-02-07）

| カテゴリ | 件数 | 備考 |
|---------|------|------|
| スクリーンショット | 0 | 本セッションで追加予定 |
| 動画/GIF | 0 | - |
| ログ | 0 | - |
| レポート | 0 | - |

---

## 関連ドキュメント

- [AI_CONTEXT.md](file:///c:/Users/thank/Storage/Game%20Projects/UnityChatNovelGame/AI_CONTEXT.md) - 現在のミッション・運用ルール
- [PROJECT_ROADMAP.md](file:///c:/Users/thank/Storage/Game%20Projects/UnityChatNovelGame/docs/PROJECT_ROADMAP.md) - 短期/中期/長期プラン
- [HANDOVER.md](file:///c:/Users/thank/Storage/Game%20Projects/UnityChatNovelGame/docs/HANDOVER.md) - 引き継ぎ情報
