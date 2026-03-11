# UnityChatNovelGame

Unity (C#) チャット/ビジュアルノベルゲーム。MVPアーキテクチャパターン。

## PROJECT CONTEXT

プロジェクト名: FoundPhone (UnityChatNovelGame)
環境: Unity 6.3 LTS (6000.3.6f1) / C# / Yarn Spinner 3.1.3 / DOTween
ブランチ戦略: trunk-based (main のみ)
フェーズ: プロトタイプ → α移行中
現在の状況: バブル表示8問題修正済み。インベントリUI(3サブタブ+オーバーレイ)実装済み。仕様Doc同期完了。未使用パッケージ削除済み。次はSSOT整備→新規開発(Records実装/サブスレッド/C-branch)。

## DECISION LOG

| 日付 | 決定事項 | 選択肢 | 決定理由 |
|------|----------|--------|----------|
| 2026-03-11 | インベントリをダッシュボード内タブに統合 | 独立画面 / ダッシュボード内タブ | 既存DashboardControllerの拡張で最小工数。チャット画面からのオーバーレイアクセスも追加 |
| 2026-03-11 | TopicDataプレフィックス分類規約を策定 | fragment_ / record_ / topic_ / その他 | InventoryTabController.GetCategory()に集約。T_*/debug_*はTopicにフォールバック |
| 2026-03-11 | バブルスクロール制御をタイプライター限定ピンニングに変更 | 常時ピンニング / タイプライター限定 | 常時ピンニングはユーザーのスクロール操作を完全に阻害していた |
| 2026-03-11 | 選択肢色をUIConfig.choiceButtonColorに統一 | playerThemeColor / choiceButtonColor | playerThemeColorは鮮やかすぎ、選択肢は独立した控えめな色が適切 |

## Key Paths

- Source: `Assets/Scripts/`
- Docs: `docs/`
- Specs: `docs/StorySpec/`, `docs/ENGINE_FEATURE_INVENTORY.md`
- Spec Index: `docs/spec-index.json`
- Topics: `Assets/Resources/Topics/`
- Channels: `Assets/Resources/Channels/`
- Characters: `Assets/Resources/Characters/`

## Rules

- Respond in Japanese
- No emoji
- Do NOT read `docs/archive/` unless explicitly asked
- When exploring code, use Grep/Glob to locate symbols instead of reading entire .cs files
- Keep responses concise — avoid repeating file contents back to the user
