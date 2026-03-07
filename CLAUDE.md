# UnityChatNovelGame

Unity (C#) チャット/ビジュアルノベルゲーム。MVPアーキテクチャパターン。

## PROJECT CONTEXT
プロジェクト名: ハルシネーション・シミュレーター / Hallucination Simulator
環境: Unity 6000.3.6f1 + YarnSpinner 3.1.3 + DOTween + Newtonsoft.Json
ブランチ戦略: trunk-based (main のみ)
現フェーズ: プロトタイプ（Ch1/Ch2実装済み、ゲームメカニクス模索中）
直近の状態: ダッシュボードMVP実装・レガシーDoc整理完了。手動セットアップ+動作確認待ち。ゲームシステムのアイデア整理が必要。

## DECISION LOG
| 日付 | 決定事項 | 選択肢 | 決定理由 |
|------|----------|--------|----------|
| 2026-03-07 | ダッシュボードUIをオーバーレイパネルで実装 | オーバーレイ / 専用シーン / ハイブリッド | 既存DebugHubパターンと統一、シーン遷移不要 |
| 2026-03-07 | ダッシュボードMVPスコープ=チャンネル一覧+HC表示のみ | 最小 / フラグメント込み / フル仕様 | 手動テスト未実施のため小さく始める |
| 2026-03-07 | レガシーDoc一括削除(318ファイル) | 選別保持 / 一括削除 | 全てレガシー、参照ゼロ、gitで復元可能 |
| 2026-03-07 | 断片(Fragment)とトピック(Topic)は概念的に別物 | 同一 / 別物 | 断片=ゲーム内の物理的紙片、トピック=システム用語。データ型の統合/分離は今後検討 |
| 2026-03-07 | ゲームメカニクス(ボード/矛盾指摘/分岐スレッド)は仕様整理・隔離優先、実装は後回し | 即実装 / 仕様整理先行 | アイデアが散在・流動的。置き換え可能な設計が必要 |

## Key Paths

- Source: `Assets/Scripts/`
- Docs: `docs/`
- StorySpec: `docs/StorySpec/`
- State SSOT: `docs/WORKFLOW_STATE_SSOT.md`
- Feature Inventory: `docs/ENGINE_FEATURE_INVENTORY.md`

## Rules

- Respond in Japanese
- No emoji
- Do NOT read `docs/specs/_ARCHIVED*` unless explicitly asked
- Use Serena's symbolic tools (find_symbol, get_symbols_overview) instead of reading entire .cs files
- When exploring code, start with get_symbols_overview, then read only the specific symbols needed
- Keep responses concise — avoid repeating file contents back to the user
