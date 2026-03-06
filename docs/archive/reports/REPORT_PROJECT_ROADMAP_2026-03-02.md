# プロジェクト・ロードマップ評価レポート

**作成日時**: 2026-03-02  
**作成者**: Cascade Orchestrator  
**対象リポジトリ**: YuShimoji/UnityChatNovelGame  
**評価基準**: 短期（1-2週間）・中期（1-2ヶ月）・長期（3-6ヶ月）

---

## エグゼクティブサマリー

### プロジェクトの現在地

**フェーズ**: Editor-Ready Pivot（2026-03-01実施）  
**ミッション**: デザイナーがUnity Editor上でYarn会話を編集し、1クリックで再生確認できる制作ループの確立  
**ブランチ**: `feature/task-049-build-gate-fix`（最新mainマージ済み）  
**直近の主要成果**:
- ContentAuthoringシーン生成とデバッグオーバーレイ実装完了
- ChatDialogueView/ChatController統合（MessageBubblePool含む）
- MVP検証パック（SG-1/MG-1）完了（100%）

### 技術的負債と課題

1. **TASK_056/057/058（旧Phase 7タスク）**: Editor-Ready pivot時に凍結、CI関連タスクとして再定義済み
2. **TASK_055**: Evidence Reuse Automation（IN_PROGRESS、証跡再利用ルール整備中）
3. **CI/CD基盤**: GitHub Actions workflow追加済み（Layer B: リモート初回実行待ち）
4. **Yarn Spinner依存**: `#if YARN_SPINNER`ガード適用済み、パッケージ除外対応完了

---

## 短期タスクマップ（1-2週間）

### 優先度: 高（速度重視・即座着手推奨）

| タスクID | タスク名 | 状態 | 推定工数 | 依存関係 | 分類 |
|---------|---------|------|---------|---------|------|
| **ContentAuthoring実コンテンツ制作** | 初の本格的なシナリオ実装 | PENDING | 2-3日 | なし | A（コア機能） |
| TASK_055 | Evidence Reuse Automation完了 | IN_PROGRESS | 1-2日 | なし | C（復旧性） |
| TASK_056_CI | CI Readiness Baseline（Layer B） | IN_PROGRESS | 1日 | リモートpush | B（速度向上） |

### 優先度: 中（短期内に対応推奨）

| タスクID | タスク名 | 状態 | 推定工数 | 依存関係 | 分類 |
|---------|---------|------|---------|---------|------|
| **ドキュメント整流** | WORKFLOW_STATE_SSOT/MISSION_LOG更新 | PENDING | 0.5日 | なし | B（速度向上） |
| **未追跡ファイル整理** | MessageBubblePool.cs.meta追加 | PENDING | 0.1日 | なし | C（復旧性） |

### 短期アクションプラン

1. **即座実行**: ContentAuthoringシーンでの実コンテンツ制作開始（ユーザーディレクション待ち）
2. **並行実行**: TASK_055完了（証跡再利用ルール文書化）
3. **検証待ち**: TASK_056_CI Layer B（GitHub Actions初回green run確認）

---

## 中期タスクマップ（1-2ヶ月）

### 優先度: 高（LG-1マイルストーン関連）

| タスクID | タスク名 | 状態 | 推定工数 | 依存関係 | 分類 |
|---------|---------|------|---------|---------|------|
| TASK_057_QA | CharacterDatabase EditMode Coverage | IN_PROGRESS | 3-5日 | TASK_056_CI | B（速度向上） |
| TASK_058_Remote | Remote Unity EditMode CI Path | IN_PROGRESS | 5-7日 | TASK_057_QA | B（速度向上） |
| **Addressables移行計画** | アセット管理基盤の刷新 | PENDING | 10-14日 | CI基盤安定化 | A（コア機能） |

### 優先度: 中（品質基盤強化）

| タスクID | タスク名 | 状態 | 推定工数 | 依存関係 | 分類 |
|---------|---------|------|---------|---------|------|
| **パフォーマンス最適化** | GC Alloc削減の継続改善 | PENDING | 3-5日 | なし | B（速度向上） |
| **セーブシステム拡張** | UI統合とデータ永続化 | PENDING | 5-7日 | なし | A（コア機能） |

### 中期マイルストーン

- **LG-1進捗目標**: 45% → 70%（2026-04月末）
- **重点領域**: CI/CD自動化、Addressables移行、QA基盤強化

---

## 長期タスクマップ（3-6ヶ月）

### 優先度: 高（Production Readiness）

| 領域 | タスク概要 | 推定工数 | 開始時期 | 分類 |
|------|----------|---------|---------|------|
| **Addressables完全移行** | 全アセットのAddressables化 | 20-30日 | 2026-04 | A（コア機能） |
| **CI/CD完全自動化** | Build/Test/Deploy pipeline | 15-20日 | 2026-04 | B（速度向上） |
| **QA自動化拡充** | PlayMode/Integration Test拡張 | 10-15日 | 2026-05 | C（復旧性） |

### 優先度: 中（機能拡張）

| 領域 | タスク概要 | 推定工数 | 開始時期 | 分類 |
|------|----------|---------|---------|------|
| **ナラティブ機能拡張** | 分岐・フラグ管理の高度化 | 10-15日 | 2026-05 | A（コア機能） |
| **演出システム強化** | エフェクト・アニメーション拡充 | 15-20日 | 2026-06 | A（コア機能） |

### 長期マイルストーン

- **LG-1完了目標**: 2026-06（100%達成）
- **Production Ready基準**: Addressables移行完了、CI/CD完全自動化、QA自動化率80%以上

---

## 技術スタック現状

### 実装済み・安定稼働

- ✅ Unity 2022.3 LTS
- ✅ Yarn Spinner（条件付き：`#if YARN_SPINNER`ガード）
- ✅ DOTween（アニメーション）
- ✅ TextMeshPro（UI）
- ✅ MessageBubblePool（オブジェクトプーリング）
- ✅ CharacterDatabase（キャラクター管理）

### 導入中・検証中

- 🔄 GitHub Actions（CI/CD基盤）
- 🔄 EditMode Tests（QA自動化）
- 🔄 Evidence Reuse Automation（証跡管理）

### 計画中・未着手

- 📋 Addressables（アセット管理）
- 📋 PlayMode Integration Tests（統合テスト）
- 📋 Performance Profiling Automation（性能監視）

---

## リスク評価と対策

### 高リスク（即座対応必要）

| リスク | 影響度 | 対策 | 担当 |
|--------|--------|------|------|
| ContentAuthoring実コンテンツ未着手 | 高 | ユーザーディレクション取得、サンプルシナリオ拡充 | Orchestrator |
| CI/CD初回実行未確認 | 中 | リモートpush後の動作確認、エラー時の即座修正 | Worker |

### 中リスク（監視継続）

| リスク | 影響度 | 対策 | 担当 |
|--------|--------|------|------|
| Addressables移行の複雑性 | 中 | 段階的移行計画、リスク分析の事前実施 | Orchestrator |
| QA自動化率の低さ | 中 | EditMode Tests拡充、PlayMode Tests段階導入 | Worker |

### 低リスク（長期監視）

| リスク | 影響度 | 対策 | 担当 |
|--------|--------|------|------|
| パフォーマンス劣化 | 低 | 定期的なProfiler計測、GC Alloc監視 | Worker |
| 技術的負債の蓄積 | 低 | リファクタリング計画、コードレビュー強化 | Orchestrator |

---

## 推奨アクション（優先順位順）

### 即座実行（Tier 1: 自律実行可）

1. **未追跡ファイル追加**: `MessageBubblePool.cs.meta`をgit add
2. **ドキュメント更新**: WORKFLOW_STATE_SSOT/MISSION_LOGの現状反映
3. **TASK_055完了**: 証跡再利用ルール文書化とレポート作成

### ユーザー確認必要（Tier 2: 承認後実行）

4. **ContentAuthoring実コンテンツ**: ユーザーからのシナリオディレクション取得
5. **リモートpush**: CI/CD初回実行確認のためのpush実施
6. **TASK_057/058再開判断**: Editor-Ready pivot後の優先度再評価

### 計画策定必要（Tier 3: 詳細設計後実行）

7. **Addressables移行計画**: 影響範囲分析、段階的移行ロードマップ作成
8. **QA自動化拡充**: PlayMode Tests導入計画、カバレッジ目標設定
9. **パフォーマンス最適化**: Profiler計測自動化、最適化優先順位決定

---

## 開発速度・品質指標

### 現状（2026-03-02時点）

- **完了タスク数**: 54タスク（TASK_001～TASK_058のうち完了分）
- **進行中タスク数**: 4タスク（TASK_055, TASK_056_CI, TASK_057_QA, TASK_058_Remote）
- **マイルストーン達成率**: SG-1/MG-1完了（100%）、LG-1進行中（45%）
- **コンパイルエラー**: 0件（静的確認済み）
- **CI/CD状態**: GitHub Actions追加済み、初回実行待ち

### 目標（2026-04月末）

- **LG-1進捗**: 45% → 70%
- **CI/CD自動化**: 初回green run確認、安定稼働
- **QA自動化率**: EditMode Tests 50%カバレッジ達成
- **ContentAuthoring**: 実コンテンツ1本完成、制作ループ確立

---

## 次のフェーズへの移行条件

### Editor-Ready Phase → Production Phase

1. ✅ ContentAuthoringシーンでの制作ループ確立
2. ⏳ CI/CD基盤の安定稼働（初回green run確認）
3. ⏳ 実コンテンツ1本の完成と動作確認
4. ⏳ TASK_055完了（証跡再利用ルール確立）

### Production Phase → Release Phase

1. ⏳ Addressables移行完了
2. ⏳ CI/CD完全自動化（Build/Test/Deploy）
3. ⏳ QA自動化率80%以上
4. ⏳ パフォーマンス基準達成（60fps安定、GC Alloc最小化）

---

## 付録: タスク分類基準（再掲）

- **A（コア機能・目的の達成）**: ゲーム本体機能、ナラティブシステム、必須UI
- **B（制作/開発速度の向上・互換設定）**: CI/CD、ツール、ワークフロー改善
- **C（失敗からの復旧しやすさ）**: テスト、証跡管理、エラーハンドリング
- **D（将来のための品質や汎化）**: 過度なテスト、過剰レポート、当面不要なリファクタリング → **凍結**

---

**レポート終了**
