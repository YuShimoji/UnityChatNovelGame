# Worker Prompt: TASK_032_FixDebugNamespace

## 目的
`ProjectFoundPhone.Debug` 名前空間と `UnityEngine.Debug` クラスの衝突（Name Collision）を解消し、コンパイルエラー（CS0234, CS1955）をゼロにする。

## 参照
- チケット: `docs/tasks/TASK_032_FixDebugNamespace.md`
- SSOT: `docs/Windsurf_AI_Collab_Rules_latest.md`
- HANDOVER: `docs/HANDOVER.md`
- ターゲットファイル: `Assets/Scripts/Debug/CustomLogger.cs`

## 境界
- **Focus Area**:
  - `Assets/Scripts/Debug/CustomLogger.cs` (Namespace Rename)
  - `Assets/Scripts/` (Compilation Fix)
- **Forbidden Area**:
  - `Assets/Plugins/` (変更禁止)
  - ロジックの変更（リファクタリングのみ）
  - 無関係な Warning の修正

## 戦略 (Recommended)
1. `Assets/Scripts/Debug/CustomLogger.cs` の namespace を `ProjectFoundPhone.Debug` から `ProjectFoundPhone.Logging` に変更する。
2. 必要に応じて、コンパイルエラーが出るファイルの `using` を修正する。
3. `Unity Editor` または `mcp_refresh_unity` を使用してコンパイル可否を確認する。

## DoD (Definition of Done)
- [ ] `CustomLogger.cs` の namespace が `ProjectFoundPhone.Logging` に変更されている。
- [ ] プロジェクト全体のコンパイルエラー（CS0234, CS1955）が解消されている。
- [ ] `docs/reports/REPORT_TASK_032_FixDebugNamespace.md` に結果をまとめる。

## 停止条件
- 名前空間を変更してもエラーが解消せず、大規模なコード修正（50ファイル以上）が必要になった場合。
- Unity Editor がクラッシュする場合。

## 納品先
- `docs/inbox/REPORT_TASK_032_FixDebugNamespace.md`
