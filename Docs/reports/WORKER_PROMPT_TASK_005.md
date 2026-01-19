# Worker Prompt: TASK_005_PackageInstallationFix

## 参照
- チケット: Docs/tasks/TASK_005_PackageInstallationFix.md
- SSOT: Docs/Windsurf_AI_Collab_Rules_latest.md
- HANDOVER: Docs/HANDOVER.md
- AI_CONTEXT: AI_CONTEXT.md
- MISSION_LOG: .cursor/MISSION_LOG.md
- 前タスクレポート: Docs/inbox/REPORT_TASK_004_PackageInstallation_FIX.md
- プロジェクト仕様: 最初のプロンプト（プロジェクトルート）

## 境界

### Focus Area
- `Packages/manifest.json` の修正
- Git URLのパス指定の修正
- DOTweenの手動インポート状態の確認と対応
- Yarn Spinnerの正しいインストール方法の実装

### Forbidden Area
- 既存のDOTweenファイルの削除（手動インポート済みのため）
- Unityプロジェクト設定の変更（パッケージインストール以外）
- 実装済みコードの変更（パッケージインストールのみ）
- 新規スクリプトの作成

## Tier / Branch
- Tier: 2（機能実装）
- Branch: main

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

## 停止条件
- Forbidden Area に触れないと完遂できない
- 仕様の仮定が 3 つ以上必要
- 依存追加/更新、破壊的Git操作、GitHubAutoApprove不明での push が必要
- SSOT不足を `ensure-ssot.js` で解決できない
- 長時間待機が必要（定義したタイムアウト超過）
- Yarn SpinnerのGit URLが正しくない場合（リポジトリ構造を確認する必要がある）

停止時は以下を実施：
1. チケットのStatusをBLOCKEDに更新
2. 事実/根拠/次手（候補）をチケット本文に追記
3. docs/inbox/REPORT_TASK_005_PackageInstallationFix.md を作成し、停止理由を記録
4. チケットのReport欄にレポートパスを追記

## 納品先
- docs/inbox/REPORT_TASK_005_PackageInstallationFix.md

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

**推奨**: まずオプション1を試し、それでもエラーが出る場合はオプション3を試す。

#### 確認事項
- `Yarn.Unity` 名前空間が利用可能
- `DialogueRunner` クラスが利用可能
- コンパイルエラーが解消されている

### 2. DOTween 対応

#### 現状
- DOTweenは既に手動でインポート済み（`Assets/Plugins/Demigiant/DOTween/` が存在）
- API Updaterエラーが発生しているが、これはUnity 6への移行時の一時的な問題の可能性
- 手動インポート済みのため、Package Managerでの管理は不要

#### 対応方法
1. **manifest.jsonからDOTweenのGit URLを削除**
   - `"com.demigiant.dotween": "https://github.com/Demigiant/dotween.git?path=/DOTween"` の行を削除
   - 手動インポート済みのため、Package Managerでの管理は不要

2. **API Updaterエラーの確認**
   - エラーは警告レベルで、実際の動作には影響しない可能性
   - もし問題がある場合は、DOTweenを再インポートする（ただし、これはForbidden Areaに触れる可能性があるため、確認が必要）

#### 確認事項
- `DG.Tweening` 名前空間が利用可能
- `DOTween.To()` メソッドが利用可能
- コンパイルエラーが解消されている

### 3. TextMeshPro / UGUI

#### 現状
- `com.unity.ugui` がmanifest.jsonに追加済み
- 問題なし

## コーディング規約
- manifest.jsonのJSON形式を正しく維持する
- Git URLのパス指定は、リポジトリの実際の構造に合わせる

## 参考情報
- 前タスクレポート: `Docs/inbox/REPORT_TASK_004_PackageInstallation_FIX.md` を参照
- プロジェクト仕様: `最初のプロンプト`（プロジェクトルート）を参照
- Unityバージョン: Unity 6 (or 2022 LTS)
- 現在のmanifest.json: `Packages/manifest.json` を参照
- DOTweenの手動インポート状態: `Assets/Plugins/Demigiant/DOTween/` を確認

## 注意事項
1. **DOTween**: 既に手動でインポート済みのため、manifest.jsonから削除して手動管理に切り替える
2. **Yarn Spinner**: Git URLのパス指定が間違っているため、正しいパスを指定する必要がある
3. **API Updaterエラー**: DOTweenのAPI Updaterエラーは警告レベルで、実際の動作には影響しない可能性がある
4. **コンパイルエラー**: Yarn Spinnerがインストールされていないため、`Yarn` 名前空間が見つからないエラーが発生している
