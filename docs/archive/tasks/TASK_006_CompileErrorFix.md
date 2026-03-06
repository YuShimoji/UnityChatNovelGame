# Task: コンパイルエラー修正（ScenarioManager.cs）
Status: DONE
Tier: 2
Branch: main
Owner: Worker
Created: 2026-01-06T14:30:00Z
Report: docs/reports/REPORT_TASK_006_CompileErrorFix.md 

## Objective
ScenarioManager.csのコンパイルエラーを修正する。Yarn SpinnerのVariableStorage APIの正しい使用方法を実装し、非推奨APIの警告を解消する。

実装対象：
1. **TryGetValueエラー修正**: 型引数の推論エラーを修正
2. **SetValueエラー修正**: 引数の型変換エラーを修正
3. **非推奨API警告解消**: FindObjectOfTypeをFindFirstObjectByTypeに置き換え
4. **未使用変数警告解消**: m_IsInputLockedの使用または削除

## Context
- TASK_005完了後、ScenarioManager.csでコンパイルエラーが発生
- エラー内容:
  - `CS0411`: TryGetValueの型引数が推論できない（287行目）
  - `CS1503`: SetValueの引数2をstringに変換できない（322行目）
  - `CS0618`: FindObjectOfTypeが非推奨（60行目）
  - `CS0414`: m_IsInputLockedが使用されていない（21行目）
- 参照ドキュメント: `最初のプロンプト`（プロジェクトルート）、`docs/inbox/REPORT_TASK_005_PackageInstallationFix.md`

## Focus Area
- `Assets/Scripts/Core/ScenarioManager.cs` の修正
- Yarn SpinnerのVariableStorage APIの正しい使用方法の実装
- Unity 6の非推奨APIの置き換え

## Forbidden Area
- 既存ファイルの削除・破壊的変更
- Unityプロジェクト設定の変更
- パッケージの追加・削除
- 他のスクリプトファイルの変更

## Constraints
- テスト: コンパイルエラーが解消されることを確認
- フォールバック: Yarn SpinnerのVariableStorage APIの正しいシグネチャを確認してから実装
- Unityバージョン: Unity 6 (or 2022 LTS) に対応
- コーディング規約: Unity C# コーディング規約に準拠

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

## 実装詳細

### 1. TryGetValueエラー修正（287行目）

#### 問題
```csharp
if (m_DialogueRunner.VariableStorage.TryGetValue(variableName, out var value))
```
- エラー: `CS0411: The type arguments for method 'VariableStorageBehaviour.TryGetValue<T>(string, out T?)' cannot be inferred from the usage.`

#### 修正方法
型引数を明示的に指定する必要があります。Yarn SpinnerのVariableStorage APIを確認し、正しい使用方法を実装します。

**オプション1**: 型引数を明示的に指定
```csharp
if (m_DialogueRunner.VariableStorage.TryGetValue<T>(variableName, out var value))
```

**オプション2**: 正しいAPIシグネチャを使用（Yarn Spinnerのバージョンによって異なる可能性）
```csharp
// Yarn SpinnerのVariableStorage APIを確認してから実装
// 例: TryGetValue<T>(string name, out T value) の場合
```

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

**オプション1**: SetValueの正しいシグネチャを使用
```csharp
// Yarn SpinnerのVariableStorage APIを確認してから実装
// 例: SetValue(string name, object value) の場合
m_DialogueRunner.VariableStorage.SetValue(variableName, (object)value);
```

**オプション2**: 型に応じた適切なメソッドを使用
```csharp
// 例: SetValue<T>(string name, T value) の場合
m_DialogueRunner.VariableStorage.SetValue<T>(variableName, value);
```

#### 確認事項
- コンパイルエラーが解消されている
- 型安全性が保たれている

### 3. FindObjectOfType警告解消（60行目）

#### 問題
```csharp
m_DialogueRunner = FindObjectOfType<DialogueRunner>();
```
- 警告: `CS0618: 'Object.FindObjectOfType<T>()' is obsolete: 'Object.FindObjectOfType has been deprecated. Use Object.FindFirstObjectByType instead or if finding any instance is acceptable the faster Object.FindAnyObjectByType'`

#### 修正方法
Unity 6の新しいAPIに置き換えます。

**推奨**: FindFirstObjectByTypeを使用（最初に見つかったインスタンスを取得）
```csharp
m_DialogueRunner = FindFirstObjectByType<DialogueRunner>();
```

**代替**: FindAnyObjectByTypeを使用（パフォーマンス重視の場合）
```csharp
m_DialogueRunner = FindAnyObjectByType<DialogueRunner>();
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

**オプション1**: 変数を使用する（将来の実装用として保持）
```csharp
// StartWaitCommandなどで使用する
// または、#pragma warning disable CS0414 で抑制
```

**オプション2**: 変数を削除（使用予定がない場合）
```csharp
// 変数を削除
```

**オプション3**: #pragma warningで抑制（将来の実装用として保持する場合）
```csharp
#pragma warning disable CS0414
private bool m_IsInputLocked = false;
#pragma warning restore CS0414
```

#### 確認事項
- 警告が解消されている
- コードの意図が明確になっている

## Notes
- Status は OPEN / IN_PROGRESS / BLOCKED / DONE を想定
- BLOCKED の場合は、事実/根拠/次手（候補）を本文に追記し、Report に docs/inbox/REPORT_...md を必ず設定
- Yarn SpinnerのVariableStorage APIはバージョンによって異なる可能性があるため、実際のAPIを確認してから実装する必要がある
- Unity 6の非推奨APIは、新しいAPIに置き換える必要がある
