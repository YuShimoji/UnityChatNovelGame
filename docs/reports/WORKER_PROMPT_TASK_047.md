# Worker Prompt for TASK_047: Fix ReImport Syntax & Encoding Errors

## 依頼内容
ReImport 後に発生した大量のコンパイルエラー（文字化け、構文破壊）を修正してください。

## コンテキスト
`ScenarioManager.cs` 等のファイルで、エンコーディング不整合による日本語コメントの文字化け（mojibake）が発生しています。
これにより、コンパイラがコメントをコードとして誤認識したり、中括弧 `}` の対応関係が崩れて `else cannot start a statement` 等のエラーを引き起こしています。

## 手順
1. **Encoding Normalization**:
   - 以下のファイルのエンコーディングを `UTF-8 w/ BOM` (または Unity が推奨する形式) に統一してください。
     - `Assets/Scripts/Core/ScenarioManager.cs`
     - `Assets/Scripts/Data/TopicData.cs`
     - `Assets/Scripts/Effects/GlitchEffect.cs`
     - `Assets/Scripts/UI/ChatController.cs`
     - `Assets/Scripts/Effects/MetaEffectController.cs`
   - 文字化けしているコメントは、可能であれば元の意味に復元するか、判読不能な場合は `// (garbled comment fix)` として無害化してください。

2. **Syntax Repair**:
   - `ScenarioManager.cs` および他ファイルで、欠落している中括弧 `}` や丸括弧 `)` を補完してください。
     - 特に `else` の前にあるべき `}` が消えているケースが多いです。
   - `TopicData.cs` (CS0106) の修正: 構造が破壊されている場合は、フィールドやメソッドの定義ブロックを正しく修正してください。

3. **Deprecation Warnings (Optional)**:
   - 余力があれば、`MCPForUnity` 以下の `FindObjectsOfType` 警告を修正してください。

## 提出物
- 修正されたスクリプト一式
- `docs/reports/REPORT_TASK_047_FixReimportErrors.md`
