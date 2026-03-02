# WORKFLOW STATE SSOT

**Updated**: 2026-03-02  
**Phase**: Editor-Ready + Production Readiness (LG-1)  
**Branch**: feature/task-049-build-gate-fix (latest main merged)

## Mission

初の本格的なコンテンツ制作スライスの開始（ContentAuthoring シーンでのノードピッカーとオーバーレイを利用した実コンテンツの構築）

## Done 条件

- [ ] ユーザーからのディレクション（どのノード/コンテンツを作成するか）の受け取り
- [ ] ContentAuthoring シーンにて指定された会話の実装と動作確認
- [ ] デバッグオーバーレイおよびノードピッカーを用いた表示・遷移の検証

## 現在の状態

### 完了済み（2026-03-02）
- ✅ リモート最新状態（origin/main）のマージ完了
- ✅ 競合解決（ChatDialogueView/ChatController統合、MessageBubblePool保持）
- ✅ プロジェクト評価レポート作成（`docs/reports/REPORT_PROJECT_ROADMAP_2026-03-02.md`）
- ✅ 短期・中期・長期タスクマップ策定

### 進行中
- 🔄 TASK_055: Evidence Reuse Automation（証跡再利用ルール整備）
- 🔄 TASK_056_CI: CI Readiness Baseline（Layer B: リモート初回実行待ち）
- 🔄 TASK_057_QA: CharacterDatabase EditMode Coverage
- 🔄 TASK_058_Remote: Remote Unity EditMode CI Path

### 次のアクション
1. **即座実行**: ContentAuthoringシーンでの実コンテンツ制作（ユーザーディレクション待ち）
2. **並行実行**: TASK_055完了（証跡再利用ルール文書化）
3. **検証待ち**: CI/CD初回green run確認

## 選別規則

当面は以下の作業分類に従い、D（将来のための品質や汎化）は凍結とします。

- A. コア機能・目的の達成
- B. 制作/開発速度の向上・互換設定
- C. 失敗からの復旧しやすさ
- D. テスト拡充、過度なレポート、当面に直結しないリファクタリング → **凍結**

## 禁止事項

- Editor-Ready 状態（1クリックでの再生確認やデバッグ表示）を損なう変更を行わないこと。
- MVP の最小導線を破壊しないこと。
- Console Error / Exception を発生させないこと。
- 過度なテスト要求、過剰なレポート生成、今の目的に直結しない汎化リファクタリングを行わないこと。
