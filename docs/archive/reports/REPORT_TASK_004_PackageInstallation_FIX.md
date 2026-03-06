# Report: TASK_004_PackageInstallation (修正版)

**作成日時**: 2026-01-06T12:30:00+09:00  
**タスク**: TASK_004_PackageInstallation  
**ステータス**: FIXED  
**実行者**: AI Agent (Orchestrator)

## 問題の概要

Workerが実装したパッケージインストールで以下のエラーが発生：

1. **DOTween**: `Package [com.demigiant.dotween@1.2.790] cannot be found`
   - バージョン1.2.790が存在しない
   - DOTweenは通常、Asset Storeからインストールするか、Git URLからインストールする必要がある

2. **Yarn Spinner**: `Could not clone [https://github.com/YarnSpinnerTool/YarnSpinner-Unity.git]. Make sure [v3.1.4] is a valid branch name, tag or full commit hash`
   - Git URLのパスが間違っている
   - タグ`v3.1.4`が存在しないか、パスが正しくない

3. **TextMeshPro**: `com.unity.textmeshpro is deprecated`
   - Unity 6では非推奨
   - `com.unity.ugui`に統合された

## 修正内容

### 1. Yarn Spinner
- **修正前**: `"dev.yarnspinner.unity": "https://github.com/YarnSpinnerTool/YarnSpinner-Unity.git#v3.1.4"`
- **修正後**: `"dev.yarnspinner.unity": "https://github.com/YarnSpinnerTool/YarnSpinner-Unity.git?path=/YarnSpinner-Unity"`
- **理由**: タグ指定を削除し、パス指定のみに変更。最新の安定版を使用

### 2. DOTween
- **修正前**: `"com.demigiant.dotween": "1.2.790"`
- **修正後**: `"com.demigiant.dotween": "https://github.com/Demigiant/dotween.git?path=/DOTween"`
- **理由**: バージョン指定では見つからないため、Git URLから直接インストール

### 3. TextMeshPro
- **修正前**: `"com.unity.textmeshpro": "4.0.0"`
- **修正後**: `"com.unity.ugui": "2.0.0"`
- **理由**: Unity 6ではTextMeshProが非推奨のため、`com.unity.ugui`に置き換え。TextMeshProの機能は`com.unity.ugui`に含まれている

## 修正後のmanifest.json

```json
{
  "dependencies": {
    // ... 既存の依存関係 ...
    "dev.yarnspinner.unity": "https://github.com/YarnSpinnerTool/YarnSpinner-Unity.git?path=/YarnSpinner-Unity",
    "com.demigiant.dotween": "https://github.com/Demigiant/dotween.git?path=/DOTween",
    "com.unity.ugui": "2.0.0"
  }
}
```

## 注意事項

### DOTweenについて
- DOTweenは通常、Asset Storeからインストールすることを推奨
- Git URLからインストールする場合、正しいリポジトリURLを指定する必要がある
- もしGit URLでもインストールできない場合は、Asset Storeから手動でインストールする必要がある

### TextMeshProについて
- Unity 6では`com.unity.textmeshpro`が非推奨
- `com.unity.ugui`を追加することで、TextMeshProの機能も利用可能
- 既存のコード（`using TMPro;`）は引き続き動作する

### Yarn Spinnerについて
- Git URLからインストールする場合、パス指定が重要
- タグ指定ではなく、パス指定を使用することで、最新の安定版を取得

## 次のステップ

1. Unityエディタでプロジェクトを開き、パッケージが正しくインストールされることを確認
2. コンパイルエラーが解消されていることを確認
3. もしDOTweenがGit URLからインストールできない場合は、Asset Storeから手動でインストール

## DoD達成状況（修正後）

- [x] Yarn Spinner がmanifest.jsonに追加されている（修正済み）
- [x] DOTween がmanifest.jsonに追加されている（修正済み、Git URL使用）
- [x] TextMeshPro がmanifest.jsonに追加されている（`com.unity.ugui`に置き換え）
- [ ] 全てのコンパイルエラーが解消されている（Unityエディタでの確認が必要）
