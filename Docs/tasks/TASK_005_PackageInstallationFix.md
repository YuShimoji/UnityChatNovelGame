# Task: パッケージインストールエラー修正（Git URLパス指定）
Status: DONE
Tier: 2
Branch: main
Owner: Worker
Created: 2026-01-06T13:00:00Z
Report: Docs/inbox/REPORT_TASK_005_PackageInstallationFix.md 

## Objective
Git URLからのパッケージインストールエラーを修正する。Yarn SpinnerとDOTweenの正しいインストール方法を実装し、コンパイルエラーを解消する。

実装対象：
1. **Yarn Spinner**: Git URLのパス指定を修正
2. **DOTween**: 手動インポート済みのため、manifest.jsonから削除またはGit URLを修正
3. **コンパイルエラー解消**: Yarn名前空間のエラーを解消

## Context
- 前タスク（TASK_004）でパッケージインストールを試みたが、Git URLのパス指定エラーが発生
- エラー内容:
  - `com.demigiant.dotween: pathspec 'DOTween' did not match any file(s) known to git`
  - `dev.yarnspinner.unity: pathspec 'YarnSpinner-Unity' did not match any file(s) known to git`
- DOTweenは既に手動でインポート済み（`Assets/Plugins/Demigiant/DOTween/` が存在）
- Yarn Spinnerはインストールできていない（コンパイルエラーが発生）
- 参照ドキュメント: `最初のプロンプト`（プロジェクトルート）、`Docs/inbox/REPORT_TASK_004_PackageInstallation_FIX.md`

## Focus Area
- `Packages/manifest.json` の修正
- Git URLのパス指定の修正
- DOTweenの手動インポート状態の確認と対応
- Yarn Spinnerの正しいインストール方法の実装

## Forbidden Area
- 既存のDOTweenファイルの削除（手動インポート済みのため）
- Unityプロジェクト設定の変更（パッケージインストール以外）
- 実装済みコードの変更（パッケージインストールのみ）
- 新規スクリプトの作成

## Constraints
- テスト: コンパイルエラーが解消されることを確認
- フォールバック: DOTweenは手動インポート済みのため、manifest.jsonから削除して手動管理
- パッケージバージョン: Unity 6 (or 2022 LTS) と互換性のあるバージョンを使用
- インストール方法: Git URLのパス指定を修正、または手動インポートを維持

## DoD
- [ ] Yarn Spinner が正しくインストールされている
  - [ ] Git URLのパス指定を修正
  - [ ] コンパイルエラー（Yarn名前空間）が解消されている
- [ ] DOTween の状態を確認・修正
  - [ ] 手動インポート済みのDOTweenが正しく動作することを確認
  - [ ] manifest.jsonからDOTweenのGit URLを削除（手動管理に切り替え）
  - [ ] コンパイルエラー（DG名前空間）が解消されている
- [ ] 全てのコンパイルエラーが解消されている
- [ ] docs/inbox/ にレポート（REPORT_TASK_005_PackageInstallationFix.md）が作成されている
- [ ] 本チケットの Report 欄にレポートパスが追記されている

## 実装詳細

### 1. Yarn Spinner インストール修正

#### 問題
- Git URLのパス指定が間違っている
- `pathspec 'YarnSpinner-Unity' did not match any file(s) known to git`

#### 修正方法
Yarn Spinnerの正しいGit URLを確認し、修正する。

**オプション1**: パス指定を削除してルートからインストール
```json
"dev.yarnspinner.unity": "https://github.com/YarnSpinnerTool/YarnSpinner-Unity.git"
```

**オプション2**: 正しいパスを指定（リポジトリ構造を確認）
```json
"dev.yarnspinner.unity": "https://github.com/YarnSpinnerTool/YarnSpinner-Unity.git?path=/YarnSpinner-Unity"
```

**オプション3**: 特定のブランチ/タグを指定
```json
"dev.yarnspinner.unity": "https://github.com/YarnSpinnerTool/YarnSpinner-Unity.git#main"
```

#### 確認事項
- `Yarn.Unity` 名前空間が利用可能
- `DialogueRunner` クラスが利用可能
- コンパイルエラーが解消されている

### 2. DOTween 対応

#### 現状
- DOTweenは既に手動でインポート済み（`Assets/Plugins/Demigiant/DOTween/` が存在）
- API Updaterエラーが発生しているが、これはUnity 6への移行時の一時的な問題の可能性

#### 対応方法
1. **manifest.jsonからDOTweenのGit URLを削除**
   - 手動インポート済みのため、Package Managerでの管理は不要
   - `com.demigiant.dotween` の行を削除

2. **API Updaterエラーの確認**
   - エラーは警告レベルで、実際の動作には影響しない可能性
   - もし問題がある場合は、DOTweenを再インポートする

#### 確認事項
- `DG.Tweening` 名前空間が利用可能
- `DOTween.To()` メソッドが利用可能
- コンパイルエラーが解消されている

### 3. TextMeshPro / UGUI

#### 現状
- `com.unity.ugui` がmanifest.jsonに追加済み
- 問題なし

## Notes
- Status は OPEN / IN_PROGRESS / BLOCKED / DONE を想定
- BLOCKED の場合は、事実/根拠/次手（候補）を本文に追記し、Report に docs/inbox/REPORT_...md を必ず設定
- DOTweenは手動インポート済みのため、manifest.jsonから削除して手動管理に切り替える
- Yarn SpinnerのGit URLは、リポジトリの実際の構造を確認して正しいパスを指定する必要がある
