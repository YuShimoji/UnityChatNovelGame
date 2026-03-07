# UnityChatNovelGame

Unity (C#) チャット/ビジュアルノベルゲーム。MVPアーキテクチャパターン。

## PROJECT CONTEXT
プロジェクト名: ハルシネーション・シミュレーター / Hallucination Simulator
環境: Unity 6000.3.6f1 + YarnSpinner 3.1.3 + DOTween + Newtonsoft.Json
ブランチ戦略: trunk-based (main のみ)
現フェーズ: プロトタイプ（Ch1/Ch2=エンジン検証用モック、メカニクス方針決定済み）
直近の状態: 物語構造を3部x3章x3節=27ブロックに修正。StorySpec全体にAI生成ドラフト注記を追加。アルケミーD-3+D-1ハイブリッド/断片UIダッシュボード統合を決定。手動セットアップ+動作確認待ち(ダッシュボード+矛盾Phase2+バブル表示)。Ch1/Ch2はモック（本編非採用）と認識を修正。

## DECISION LOG
| 日付 | 決定事項 | 選択肢 | 決定理由 |
|------|----------|--------|----------|
| 2026-03-07 | ダッシュボードUIをオーバーレイパネルで実装 | オーバーレイ / 専用シーン / ハイブリッド | 既存DebugHubパターンと統一、シーン遷移不要 |
| 2026-03-07 | ダッシュボードMVPスコープ=チャンネル一覧+HC表示のみ | 最小 / フラグメント込み / フル仕様 | 手動テスト未実施のため小さく始める |
| 2026-03-07 | レガシーDoc一括削除(318ファイル) | 選別保持 / 一括削除 | 全てレガシー、参照ゼロ、gitで復元可能 |
| 2026-03-07 | 断片(Fragment)とトピック(Topic)は概念的に別物 | 同一 / 別物 | 断片=ゲーム内の物理的紙片、トピック=システム用語。データ型の統合/分離は今後検討 |
| 2026-03-07 | ゲームメカニクス(ボード/矛盾指摘/分岐スレッド)は仕様整理・隔離優先、実装は後回し | 即実装 / 仕様整理先行 | アイデアが散在・流動的。置き換え可能な設計が必要 |
| 2026-03-07 | メカニクス優先順位: A(矛盾指摘) > B(アルケミー) > C(分岐スレッド) | 別順序 / 全凍結 | Aは Phase 2 実装済みで即検証可能。B/Cは仕様未確定 |
| 2026-03-07 | DeductionBoard は凍結・隔離 (D-1) | 凍結 / 軽量リファクタ / 廃止再設計 | 仕様未確定のまま先行実装された。アルケミー仕様が固まるまで触らない |
| 2026-03-07 | 矛盾発見→断片入手の紐づけを外す (T-1) | 外す / 別データ型分離 / 現状維持 | 矛盾報酬=HalluciCoinのみが自然。断片は物理的紙片で別入手経路 |
| 2026-03-08 | 物語構造: 3部x3章x3節=27ブロック(1節=1日) | 9ブロック / 27ブロック | ユーザー元来の想定。ドキュメントは9ブロックで記載されていたがこれはAI生成時の取り落とし |
| 2026-03-08 | Ch1/Ch2 Yarnスクリプトはモック（本編非採用） | モック維持 / 本編化 | AI生成コンテンツでドリフトしている。エンジン検証用として維持、本編は別途設計 |
| 2026-03-08 | StorySpecのAI生成部分にDRAFT注記を追加 | マーキング / 削除 / 放置 | AI生成コンテンツが仕様として蓄積→次AIセッションが参照→ドリフト拡大のループ防止 |
| 2026-03-08 | アルケミー方式: D-3+D-1ハイブリッド | パッシブのみ / ミニマルのみ / ハイブリッド / ボードゲーム型 | パッシブ主体が「推理より検証」に整合。重大局面のみ能動合成でゲーム性確保 |
| 2026-03-08 | ~~断片UIの配置: ダッシュボード統合(i+iv)~~ **撤回** | - | 前提誤り。断片はアイテムの一種であり専用UIは不要。汎用インベントリ/プレイヤーリソース表示域を設計すべき |
| 2026-03-08 | プレイヤーリソース: 断片/冒険アイテム/HC/トピックの4種を統合表示域で管理 | 断片専用UI / 汎用インベントリ | 断片はアイテムの一種。全リソースを統一的に表示する |

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
