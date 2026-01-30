# Worker Prompt for TASK_046: Fix MCP Syntax Error

## 依頼内容
`Assets/MCPForUnity/Editor/Dependencies/Models/DependencyStatus.cs` に混入した不正な文字によるコンパイルエラーを修正してください。

## コンテキスト
60行目の `ToString()` メソッド内に `✁` という文字が含まれており、C# の構文として不正になっています。
元々はチェックマーク（✓）や「OK」などを意図していたと思われます。

## 手順
1. `Assets/MCPForUnity/Editor/Dependencies/Models/DependencyStatus.cs` を開く。
2. 60行目を以下のように修正する（可読性を重視）。
   - 修正案: `var status = IsAvailable ? "[OK]" : "[MISSING]";`
3. コンパイルエラーが解消されたことを確認する。

## 提出物
- 修正された `DependencyStatus.cs`
- `docs/reports/REPORT_TASK_046_FixMCPDependencyStatus.md`
