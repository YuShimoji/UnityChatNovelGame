# Worker Prompt: TASK_025_GCAllocReduction

## 概要
TASK_022 のベースライン計測で観測された GC Alloc（平均 22KB/frame）を、根拠（Profiler）に基づいて低減する。

## 現状
- ベースラインは取得済み（REPORT_TASK_022_PerformanceBaseline.md）。
- GC Alloc が平均 22KB/frame 発生しているため、発生源の特定と削減が次のテーマ。

## 次のアクション
1. Unity Profiler（Timeline / Memory）で GC Alloc の発生箇所を特定（上位1〜3）。
2. 影響が小さい変更で alloc を削減（根拠がある変更のみ）。
3. 同条件で再計測し、Before/After をレポート化。

## 参照
- チケット: docs/tasks/TASK_025_GCAllocReduction.md
- SSOT: docs/Windsurf_AI_Collab_Rules_latest.md
- HANDOVER: docs/HANDOVER.md
- MISSION_LOG: .cursor/MISSION_LOG.md

## 境界
- Focus Area:
  - Unity Profiler (Timeline / Memory)
  - DebugChatScene
  - Chat UI 更新ループ / 文字列生成
  - エフェクト再生（DOTween、UIアニメ）
  - `Assets/Scripts/Utils/PerformanceMonitor.cs`
- Forbidden Area:
  - 仕様変更（ユーザー体験を変えるUI/演出の変更）
  - 無関係な大規模リファクタリング

## DoD
- [ ] 主なGC Alloc発生箇所（上位1〜3）が特定され、根拠（Profilerのスクリーンショット等）が残っている
- [ ] 変更後の計測で、GC Allocがベースライン（平均22KB/frame）から低減している
- [ ] `docs/reports/REPORT_TASK_025_GCAllocReduction.md` が作成され、Before/Afterが明記されている

## 停止条件
- Unity Profilerが利用できない（環境制約）
- DebugChatSceneが起動できない/コンパイル不能
- alloc発生源が特定できず、仮定が3つ以上必要

## 納品先
- docs/inbox/REPORT_TASK_025_GCAllocReduction.md
