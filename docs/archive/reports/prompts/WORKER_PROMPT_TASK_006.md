# Worker Prompt: TASK_006_CompileErrorFix

## 参照
- チケット: docs/tasks/TASK_006_CompileErrorFix.md
- SSOT: docs/Windsurf_AI_Collab_Rules_latest.md
- HANDOVER: docs/HANDOVER.md
- AI_CONTEXT: AI_CONTEXT.md
- MISSION_LOG: .cursor/MISSION_LOG.md
- 前タスクレポート: docs/inbox/REPORT_TASK_005_PackageInstallationFix.md
- プロジェクト仕様: 最初のプロンプト（プロジェクトルート）

## 境界

### Focus Area
- `Assets/Scripts/Core/ScenarioManager.cs` の修正
- Yarn SpinnerのVariableStorage APIの正しい使用方法の実装
- Unity 6の非推奨APIの置き換え

### Forbidden Area
- 既存ファイルの削除・破壊的変更
- Unityプロジェクト設定の変更
- パッケージの追加・削除
- 他のスクリプトファイルの変更

## Tier / Branch
- Tier: 2（機能実装）
- Branch: main

## DoD
- [ ] TryGetValueエラーが解消されている
  - [ ] 型引数を明示的に指定
  - [ ] または、正しいAPIシグネチャを使用
- [ ] SetValueエラーが解消されている
  - [ ] 引数の型を正しく指定
  - [ ] または、正しいAPIシグネチャを使用
- [ ] FindObjectOfType警告が解消されている
  - [ ] FindFirstObjectByTypeまたはFindAnyObjectByTypeに置き換え
- [ ] m_IsInputLocked警告が解消されている
  - [ ] 変数を使用するか、削除するか、#pragma warningで抑制
- [ ] 全てのコンパイルエラーが解消されている
- [ ] docs/inbox/ にレポート（REPORT_TASK_006_CompileErrorFix.md）が作成されている
- [ ] 本チケットの Report 欄にレポートパスが追記されている

## 停止条件
- Forbidden Area に触れないと完遂できない
- 仕様の仮定が 3 つ以上必要
- 依存追加/更新、破壊的Git操作、GitHubAutoApprove不明での push が必要
- SSOT不足を `ensure-ssot.js` で解決できない
- 長時間待機が必要（定義したタイムアウト超過）
- Yarn SpinnerのVariableStorage APIが確認できない場合（ドキュメントが不足している場合）

停止時は以下を実施：
1. チケットのStatusをBLOCKEDに更新
2. 事実/根拠/次手（候補）をチケット本文に追記
3. docs/inbox/REPORT_TASK_006_CompileErrorFix.md を作成し、停止理由を記録
4. チケットのReport欄にレポートパスを追記

## 納品先
- docs/inbox/REPORT_TASK_006_CompileErrorFix.md

## 実装詳細

### 1. TryGetValueエラー修正（287行目）

#### 問題
```csharp
if (m_DialogueRunner.VariableStorage.TryGetValue(variableName, out var value))
```
- エラー: `CS0411: The type arguments for method 'VariableStorageBehaviour.TryGetValue<T>(string, out T?)' cannot be inferred from the usage.`

#### 修正方法
型引数を明示的に指定する必要があります。

**推奨**: 型引数を明示的に指定
```csharp
// object型で取得してからキャスト
if (m_DialogueRunner.VariableStorage.TryGetValue<object>(variableName, out var value))
{
    // value is T typedValue のパターンでキャスト
    if (value is T typedValue)
    {
        return typedValue;
    }
}
```

**代替**: 正しいAPIシグネチャを確認してから実装
- Yarn SpinnerのVariableStorage APIは、通常 `TryGetValue<T>(string name, out T value)` の形式
- 型引数 `<T>` を明示的に指定する必要がある

#### 確認事項
- コンパイルエラーが解消されている
- 型安全性が保たれている

### 2. SetValueエラー修正（322行目）

#### 問題
```csharp
m_DialogueRunner.VariableStorage.SetValue(variableName, value);
```
- エラー: `CS1503: Argument 2: cannot convert from 'T' to 'string'`

#### 修正方法
Yarn SpinnerのVariableStorage APIを確認し、正しいシグネチャを使用します。

**推奨**: object型にキャストしてから設定
```csharp
// SetValueは通常、object型を受け取る
m_DialogueRunner.VariableStorage.SetValue(variableName, (object)value);
```

**代替**: 型に応じた適切なメソッドを使用
- Yarn SpinnerのVariableStorage APIは、通常 `SetValue(string name, object value)` の形式
- ジェネリック型 `T` を `object` にキャストする必要がある

#### 確認事項
- コンパイルエラーが解消されている
- 型安全性が保たれている

### 3. FindObjectOfType警告解消（60行目）

#### 問題
```csharp
m_ChatController = FindObjectOfType<ChatController>();
```
- 警告: `CS0618: 'Object.FindObjectOfType<T>()' is obsolete`

#### 修正方法
Unity 6の新しいAPIに置き換えます。

**推奨**: FindFirstObjectByTypeを使用
```csharp
m_ChatController = FindFirstObjectByType<ChatController>();
```

**代替**: FindAnyObjectByTypeを使用（パフォーマンス重視の場合）
```csharp
m_ChatController = FindAnyObjectByType<ChatController>();
```

#### 確認事項
- 警告が解消されている
- 動作が正しく保たれている

### 4. m_IsInputLocked警告解消（21行目）

#### 問題
```csharp
private bool m_IsInputLocked = false;
```
- 警告: `CS0414: The field 'ScenarioManager.m_IsInputLocked' is assigned but its value is never used`

#### 修正方法
変数を使用するか、削除するか、#pragma warningで抑制します。

**推奨**: #pragma warningで抑制（将来の実装用として保持）
```csharp
#pragma warning disable CS0414
private bool m_IsInputLocked = false;
#pragma warning restore CS0414
```

**代替1**: 変数を使用する（StartWaitCommandなどで使用）
```csharp
// StartWaitCommand()で使用
private void StartWaitCommand(string[] args)
{
    m_IsInputLocked = true;
    // ... 待機処理 ...
    m_IsInputLocked = false;
}
```

**代替2**: 変数を削除（使用予定がない場合）
```csharp
// 変数を削除
```

#### 確認事項
- 警告が解消されている
- コードの意図が明確になっている

## コーディング規約
- Unity C# コーディング規約に準拠
- 変数名: `m_VariableName` (private field)
- クラス/メソッド: PascalCase
- `#region`を使用してコードを整理

## 参考情報
- 前タスクレポート: `docs/inbox/REPORT_TASK_005_PackageInstallationFix.md` を参照
- プロジェクト仕様: `最初のプロンプト`（プロジェクトルート）を参照
- Unityバージョン: Unity 6 (or 2022 LTS)
- Yarn Spinner API: 最新のドキュメントを参照（バージョン依存の可能性あり）
- エラー発生箇所: `Assets/Scripts/Core/ScenarioManager.cs` (287行目, 322行目, 60行目, 21行目)

## 注意事項
1. **Yarn Spinner API**: VariableStorage APIはバージョンによって異なる可能性があります。実装時は実際のAPIを確認してから実装してください。
2. **型安全性**: TryGetValueとSetValueの修正時は、型安全性を保つように注意してください。
3. **Unity 6 API**: FindObjectOfTypeは非推奨のため、FindFirstObjectByTypeまたはFindAnyObjectByTypeに置き換える必要があります。
4. **未使用変数**: m_IsInputLockedは将来の実装用として保持する場合は、#pragma warningで抑制することを推奨します。
