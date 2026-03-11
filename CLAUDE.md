# UnityChatNovelGame

Unity (C#) チャット/ビジュアルノベルゲーム。MVPアーキテクチャパターン。

## PROJECT CONTEXT

プロジェクト名: FoundPhone (UnityChatNovelGame)
環境: Unity 6.3 LTS (6000.3.6f1) / C# / Yarn Spinner 3.1.3 / DOTween
ブランチ戦略: trunk-based (main のみ)
フェーズ: プロトタイプ → α移行中
現在の状況 (2026-03-12): チャットバブルUI大幅リファクタ実施（未テスト）。バブル幅計算を構造修正（FinalizeBubbleSize新設: ラッパー配置後に高さ再計算）。名前:本文を改行分離（リッチテキスト）。9-slice角丸+影を追加。SystemMessage分岐ルーティングの準備コメント追加。Debug Overlay デフォルトOFF化。全変更はUnity再生テスト未実施。

## DECISION LOG

| 日付 | 決定事項 | 選択肢 | 決定理由 |
|------|----------|--------|----------|
| 2026-03-11 | インベントリをダッシュボード内タブに統合 | 独立画面 / ダッシュボード内タブ | 既存DashboardControllerの拡張で最小工数。チャット画面からのオーバーレイアクセスも追加 |
| 2026-03-11 | TopicDataプレフィックス分類規約を策定 | fragment_ / record_ / topic_ / その他 | InventoryTabController.GetCategory()に集約。T_*/debug_*はTopicにフォールバック |
| 2026-03-11 | バブルスクロール制御をタイプライター限定ピンニングに変更 | 常時ピンニング / タイプライター限定 | 常時ピンニングはユーザーのスクロール操作を完全に阻害していた |
| 2026-03-11 | 選択肢色をUIConfig.choiceButtonColorに統一 | playerThemeColor / choiceButtonColor | playerThemeColorは鮮やかすぎ、選択肢は独立した控えめな色が適切 |
| 2026-03-12 | NPC名前:本文を改行分離 | 同一行「名前: 本文」/ 改行分離 | 折り返し時に本文開始位置で揃えるため。名前はmessageFontSize*0.75のボールド |
| 2026-03-12 | バブル幅計算をラッパー配置後に最終確定 | 配置前に確定 / 配置後にFinalize | 配置前だとHLG+アイコンによる幅変動で高さ不整合が発生する構造的問題 |
| 2026-03-12 | SystemMessage表示先の分岐準備 | 即実装 / ルーティングコメントのみ | ステータスバーUI新設は今は不要。将来の分離に備えて分岐ポイントのみ |
| 2026-03-12 | Debug Overlay デフォルトOFF | true / false | 通常表示と開発表示の同居で確認対象がぶれるため。Inspector ON で個別有効化可能 |

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
