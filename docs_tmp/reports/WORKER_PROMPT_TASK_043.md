# Worker Prompt for TASK_043: Performance Benchmark

## 依頼内容
以下の手順に従い、アプリケーションのパフォーマンスベースライン（ロード時間、FPS、メモリ）を計測してください。

## コンテキスト
- プロジェクト: UnityChatNovelGame
- 目的: 最適化前の現状（Baseline）を数値化する。
- ターゲット: `DebugChatScene`

## 手順
1. `Assets/Scripts/Debug/PerformanceBenchmark.cs` を作成する。
   - `Using UnityEngine.Profiling;` を使用。
   - `Awake` で `Stopwatch` を開始。
   - `Start` でロード時間を計測(`Stopwatch.ElapsedMilliseconds`)。
   - `Update` で FPS を計測 (Time.deltaTime の平均)。
   - `CapturedMetrics` クラス (struct) にデータを保持。
2. Unity Editor で `DebugChatScene` を開き、空の GameObject に `PerformanceBenchmark` をアタッチする。
3. Play モードを実行し、約10秒間待機（または自動終了）させて計測する。
4. 計測結果を Markdown 形式で `docs/reports/PERFORMANCE_BASELINE_{YYYYMMDD}.md` に出力する。
   - 項目: Date, BootTime(ms), AvgFPS, MemoryUsed(MB).

## 禁止事項
- 既存の `ChatController` や `ScenarioManager` を変更しない。
- 計測用オブジェクトを Commit に含めない（Scriptのみ Commit する、Hierarchy変更は Revert する、または専用シーンを作る）。
  - 推奨: 計測用シーン `Assets/Scenes/DebugPerformance.unity` を新規作成してもよい。

## 提出物
- `Assets/Scripts/Debug/PerformanceBenchmark.cs`
- `docs/reports/PERFORMANCE_BASELINE_*.md`
- `docs/reports/REPORT_TASK_043_performance_baseline.md` (Worker Report)
