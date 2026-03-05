## 現在のミッション

- **タイトル**: Phase 2 — シナリオ整備 & 仕様確定
- **ブランチ**: main
- **進捗**: エンジンMVP 95%完了。シナリオ演出整備・仕様ドキュメント化を推進中。

## 実装ステータス

### 完了（実動作確認済み）

- [x] ChatController: メッセージバブル、タイプライター効果、スクロール吸着、キャラアイコン、連続メッセージグループ化
- [x] ChatDialogueView: DialoguePresenterBase実装、Yarn Spinner統合
- [x] TypingIndicator: ハイブリッド方式（自動0.3s + 手動演出制御）
- [x] カスタムコマンド: Message, SystemMessage, StartWait, SkipWait, Typing, UnlockTopic, Image
- [x] 選択肢表示: 200ms遅延、二重表示防止ガード
- [x] SaveManager: 3スロット手動セーブ/ロード（Newtonsoft.Json）
- [x] SaveLoadUI: スロット表示、セーブ/ロード切替
- [x] ScenarioManager: Yarn Spinnerラッパー、変数管理
- [x] DeductionBoard: トピック管理（HashSet）
- [x] MessageBubblePool: オブジェクトプーリング
- [x] CharacterProfile/CharacterDatabase: SO基盤、テーマカラー適用
- [x] TitleScreenManager: タイトル画面
- [x] MetaEffectController: グリッチスケルトン（Lv1-2）
- [x] EditModeテスト: 18ケース（TopicData, SynthesisRecipe, SaveData, CharacterProfile）
- [x] MVPTest.yarn: エンジン検証用テストシナリオ

### 設計完了（未実装）

- [ ] オートセーブ機能（設計書: docs/AUTOSAVE_DESIGN.md）
- [ ] 矛盾指摘UI（バブルタップ → フライアニメーション → ボード）
- [ ] Safe Area対応（ノッチ + ソフトウェアキーボード）

### 未着手

- [ ] グリッチ演出詳細（Lv2-3のShader実装）
- [ ] 連絡先リスト（Contact List）
- [ ] 探索スレッド / ミニゲーム
- [ ] 音楽 / SE
- [ ] Addressables移行
- [ ] PlayModeテスト拡充

## シナリオ状況

| ファイル | 構成上の位置 | 状態 |
| ---- | ---- | ---- |
| Ch1_Terminal.yarn | 第1部・第1章・セッション1 | Typing演出済み |
| Ch2_LocationConfusion.yarn | 第1部・第1章・セッション2 | Typing演出済み |
| MVPTest.yarn | テスト用 | 完了 |
| 第1部・第1章・セッション3 | （未作成） | 未着手 |

## 主要クラス

| クラス | 責務 |
| ---- | ---- |
| ChatController | チャット画面全体（バブル生成、スクロール、TypingIndicator、選択肢） |
| ChatDialogueView | Yarn Spinner統合（DialoguePresenterBase実装） |
| ScenarioManager | Yarn Spinnerラッパー、カスタムコマンド処理 |
| SaveManager | セーブ/ロード管理（Singleton） |
| DeductionBoard | トピック管理、矛盾指摘ロジック |
| ContradictionManager | 矛盾データ管理 |
| MetaEffectController | グリッチ等のメタ演出制御 |
| TitleScreenManager | タイトル画面制御 |
| CharacterDatabase | キャラクタープロファイル検索 |

## 仕様ドキュメント

| ドキュメント | 内容 |
| ---- | ---- |
| docs/GAME_DESIGN_DOCUMENT.md | 正規仕様書（SSOT） |
| docs/SPEC_DECISIONS.md | Q&Aセッションで確定した仕様記録 |
| docs/AUTOSAVE_DESIGN.md | オートセーブ機能設計書 |
| docs/UI_IMPLEMENTATION_SPEC.md | UI実装仕様書 |
| docs/MVP_TEST_GUIDE.md | MVPテストガイド |

## リスク/懸念

- **シナリオシステムの二重構造**: Yarn Spinner方式 + ScriptableObject方式 — Yarn Spinnerを正とし、ChatScenarioDataは段階的に縮小
- **ファイル命名規則**: 現在のCh1/Ch2は「セッション」に相当。3部3章3節に合わせた命名変更を検討中

## 履歴

- 2026-03-05: 仕様Q&Aセッション実施（ストーリー構成、グリッチ、矛盾指摘、HalluciCoin、オートセーブ等確定）
- 2026-03-05: Ch2 Typing演出追加、AUTOSAVE_DESIGN.md作成、SPEC_DECISIONS.md作成
- 2026-03-04: Ch1 Typing演出追加、UI_IMPLEMENTATION_SPEC.md作成
- 2026-03-04: TypingIndicator修正（wrapper参照保持、プール分離、作成順序修正）
- 2026-03-04: スクロール吸着修正、選択肢二重表示防止、200ms遅延追加
- 2026-03-03: MVPTest.yarn作成、MVP_TEST_GUIDE.md作成
