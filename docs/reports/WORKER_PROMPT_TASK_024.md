# Worker Prompt: Fix Performance Compilation Error (TASK_024)

## あなたの役割
あなたは「Build Engineer / Unity Specialist」です。
現在、プロジェクトはコンパイルエラーによりブロックされています。
最速でエラーを解消し、正常なビルド状態を回復させてください。

## 対象タスク
- **Task ID**: `TASK_024`
- **Definition**: `docs/tasks/TASK_024_FixPerformanceCompilation.md`
- **Focus Area**: `Assets/Scripts/Tests/`, `Assets/Scripts/Utils/`

## 状況 (Context)
- `PerformanceBaselineVerification.cs` というテストコードが追加されましたが、参照している `Assets.Scripts.Utils.PerformanceMonitor` が存在しない（または名前空間が違う）ため、`CS0234` エラーが発生しています。
- これにより、他の作業（TASK_022等）がブロックされています。

## 指示 (Instructions)
1. **調査**: `PerformanceMonitor` クラスがプロジェクト内に存在するか確認してください。
   - 存在する場合: Namespaceを確認し、`PerformanceBaselineVerification.cs` の `using` を修正してください。
   - 存在しない場合: `Assets/Scripts/Utils/PerformanceMonitor.cs` を新規作成してください（中身は空の MonoBehaviour スタブで構いません）。
2. **検証**: コンパイルエラーが消滅したことを確認してください。
3. **報告**: `docs/reports/REPORT_TASK_024_FixPerformanceCompilation.md` を作成し、修正内容を報告してください。

## 制約 (Constraints)
- **DoD**: コンパイルが通ること。
- 既存のコードを壊さないこと。
- 余計な機能実装はしないこと（Monitorの中身は TASK_022 で実装するため、ここでは枠だけで良い）。
