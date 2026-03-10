# UnityChatNovelGame

Unity (C#) チャット/ビジュアルノベルゲーム。MVPアーキテクチャパターン。

## PROJECT CONTEXT
プロジェクト名: ハルシネーション・シミュレーター / Hallucination Simulator
環境: Unity 6000.3.6f1 + YarnSpinner 3.1.3 + DOTween + Newtonsoft.Json
ブランチ戦略: trunk-based (main のみ)
現フェーズ: プロトタイプ（Ch1/Ch2=エンジン検証用モック、メカニクス方針決定済み）
直近の状態: Phase A エンジン検証を実施。m_AutoStartYarn=0に変更しダッシュボード→Ch1/Ch2再生確認済み。矛盾長押しraycastTarget修正済み。選択肢表示位置・選択後プレイヤーメッセージ未表示の2件はデバッグログ追加済みで再テスト待ち。サブスレッドUI仕様(16_subthread_ui.md)策定済み。ROADMAP_TO_PRODUCTION.md作成済み。

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
| 2026-03-11 | サブスレッドUI: 統合型スレッドモデル | 分離型 / 統合型 | StorySpec上の構造とランタイム分岐を同一UIで扱う |
| 2026-03-11 | サブスレッドUI: フルスクリーンフォーカス+スワイプサイドバー | Discord型 / フルスクリーン / チャンネル切替 | 停滞感回避+世界観整合 |
| 2026-03-11 | サブスレッドトリガー: 2段階(前提条件+顕在化) | 即時 / 条件 / 2段階 | 前兆→顕在化の自然さ+Yarnフラグ柔軟性 |

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

## PROJECT CONTEXT (LATEST 2026-03-11)
- Phase A エンジン検証: Ch1/Ch2再生OK、ダッシュボード表示OK
- バグ修正: 矛盾長押し raycastTarget=false → lineTag付きバブルでtrue設定
- 未解決バグ(デバッグログ追加済み・再テスト待ち):
  - 選択後プレイヤーメッセージ未表示: [AddMessage]/[RunOptionsAsync]/[FadeAndHideChoices] ログ
  - 選択肢中央表示: [CreateRuntimeChoiceContainer] Screen.width計算値ログ
- 新規ドキュメント: 16_subthread_ui.md (統合型スレッド仕様)、ROADMAP_TO_PRODUCTION.md
- ContradictionFeedbackController は手動シーン設定が未実施
- Priority for next operator:
  1) Unityで再テスト → デバッグログ確認 → 選択肢/メッセージ表示バグ修正
  2) ContradictionFeedbackController シーン設定
  3) デバッグログ除去 → コミット

## DECISION LOG (ADDENDUM 2026-03-10)
| 2026-03-10 | 4項目の優先度を「監視型」ではなく「既存資産の整理」に固定 | 仕様整理 / ドキュメント整理 / 新規実装 | 既存コード・既存ドキュメントの再発掘を優先し、過去パネルの判断を統合 |
| 2026-03-10 | スレッド管理を UIサブスレッド と BranchThreadState 別トラックで管理 | UI設計 / C-branch実装 | StorySpec上の構造とランタイム状態管理は性質が異なる |
| 2026-03-10 | 上記項目を StorySpec に追記記録 (`15_feature_triage_2026-03-10.md`) | 仕様記録 | 過去パネルの情報欠落防止 |

## DECISION LOG (ADDENDUM 2026-03-11)
| 2026-03-11 | サブスレッドUI: 統合型スレッドモデル(サブスレッド+分岐スレッド=同一概念) | 分離型 / 統合型 | StorySpec上の構造とランタイム分岐は同一UIで扱う方が自然 |
| 2026-03-11 | サブスレッドUI: フルスクリーンフォーカス+スワイプサイドバー | Discord型常時表示 / フルスクリーン+サイドバー / チャンネル切替型 | 停滞感回避+世界観との整合(1スレッドにフォーカスが自然) |
| 2026-03-11 | サブスレッドトリガー: 2段階(DeclareThread前提条件+ManifestThread顕在化) | 即時出現 / 条件トリガー / 2段階 | ストーリー上の自然さ(前兆→顕在化)とYarnフラグ制御の柔軟性 |
| 2026-03-11 | ContradictionPair.UnlockTopic に [Obsolete] 追加 | 削除 / Obsolete / 放置 | DeductionBoard凍結中で参照は残すが新規利用を警告 |
