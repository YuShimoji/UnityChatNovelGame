# Worker Prompt: TASK_004_PackageInstallation

## 参照
- チケット: docs/tasks/TASK_004_PackageInstallation.md
- SSOT: docs/Windsurf_AI_Collab_Rules_latest.md
- HANDOVER: docs/HANDOVER.md
- AI_CONTEXT: AI_CONTEXT.md
- MISSION_LOG: .cursor/MISSION_LOG.md
- 前タスクレポート: docs/inbox/REPORT_TASK_002_LogicImplementation.md
- プロジェクト仕様: 最初のプロンプト（プロジェクトルート）

## 境界

### Focus Area
- `Packages/manifest.json` の更新
- Unity Package Managerを使用したパッケージインストール
- パッケージバージョンの確認と互換性チェック
- コンパイルエラーの解消確認

### Forbidden Area
- 既存ファイルの削除・破壊的変更
- Unityプロジェクト設定の変更（パッケージインストール以外）
- 実装済みコードの変更（パッケージインストールのみ）
- 新規スクリプトの作成

## Tier / Branch
- Tier: 2（機能実装）
- Branch: main

## DoD
- [ ] Yarn Spinner がインストールされている
  - [ ] `com.yarnspinner.yarnspinner` パッケージがmanifest.jsonに追加されている
  - [ ] コンパイルエラー（Yarn名前空間）が解消されている
- [ ] DOTween Pro がインストールされている
  - [ ] `com.demigiant.dottweenpro` または `com.demigiant.dottween` パッケージがmanifest.jsonに追加されている
  - [ ] コンパイルエラー（DG名前空間）が解消されている
- [ ] TextMeshPro がインストールされている
  - [ ] `com.unity.textmeshpro` パッケージがmanifest.jsonに追加されている
  - [ ] コンパイルエラー（TMPro名前空間）が解消されている
- [ ] UnityEngine.UI が利用可能であることを確認
  - [ ] Unity UIモジュールが有効になっていることを確認
- [ ] 全てのコンパイルエラーが解消されている
- [ ] docs/inbox/ にレポート（REPORT_TASK_004_PackageInstallation.md）が作成されている
- [ ] 本チケットの Report 欄にレポートパスが追記されている

## 停止条件
- Forbidden Area に触れないと完遂できない
- 仕様の仮定が 3 つ以上必要
- 依存追加/更新、破壊的Git操作、GitHubAutoApprove不明での push が必要
- SSOT不足を `ensure-ssot.js` で解決できない
- 長時間待機が必要（定義したタイムアウト超過）
- DOTween Proが有料パッケージで購入が必要な場合（Free版を使用する場合は継続可能）

停止時は以下を実施：
1. チケットのStatusをBLOCKEDに更新
2. 事実/根拠/次手（候補）をチケット本文に追記
3. docs/inbox/REPORT_TASK_004_PackageInstallation.md を作成し、停止理由を記録
4. チケットのReport欄にレポートパスを追記

## 納品先
- docs/inbox/REPORT_TASK_004_PackageInstallation.md

## 実装詳細

### 1. Yarn Spinner インストール

#### パッケージ情報
- **パッケージ名**: `com.yarnspinner.yarnspinner`
- **インストール方法**: 
  - Unity Package Manager: `Window` → `Package Manager` → `+` → `Add package from git URL` → `https://github.com/YarnSpinner/YarnSpinner-Unity.git?path=/YarnSpinner-Unity`
  - または manifest.jsonに直接追加: `"com.yarnspinner.yarnspinner": "https://github.com/YarnSpinner/YarnSpinner-Unity.git?path=/YarnSpinner-Unity"`

#### manifest.jsonへの追加例
```json
{
  "dependencies": {
    "com.yarnspinner.yarnspinner": "https://github.com/YarnSpinner/YarnSpinner-Unity.git?path=/YarnSpinner-Unity",
    ...
  }
}
```

#### 確認事項
- `Yarn.Unity` 名前空間が利用可能
- `DialogueRunner` クラスが利用可能
- コンパイルエラーが解消されている

### 2. DOTween Pro インストール

#### パッケージ情報
- **パッケージ名**: `com.demigiant.dottweenpro` (Pro版) または `com.demigiant.dottween` (Free版)
- **インストール方法**: 
  - Asset Storeからインストール（推奨、Pro版の場合）
  - または manifest.jsonに直接追加（Free版の場合）
- **注意**: DOTween Proは有料パッケージのため、Asset Storeから購入・インストールが必要な場合があります。Free版を使用する場合は、manifest.jsonに適切なパッケージIDを追加してください。

#### manifest.jsonへの追加例（Free版の場合）
```json
{
  "dependencies": {
    "com.demigiant.dottween": "1.2.632",
    ...
  }
}
```

#### 確認事項
- `DG.Tweening` 名前空間が利用可能
- `DOTween.To()` メソッドが利用可能
- コンパイルエラーが解消されている

### 3. TextMeshPro インストール

#### パッケージ情報
- **パッケージ名**: `com.unity.textmeshpro`
- **インストール方法**: 
  - Unity Package Manager: `Window` → `Package Manager` → `Unity Registry` → `TextMeshPro` を検索してインストール
  - または manifest.jsonに直接追加: `"com.unity.textmeshpro": "3.0.6"` (Unity 2022 LTSの場合)

#### manifest.jsonへの追加例
```json
{
  "dependencies": {
    "com.unity.textmeshpro": "3.0.6",
    ...
  }
}
```

#### 確認事項
- `TMPro` 名前空間が利用可能
- `TextMeshProUGUI` クラスが利用可能
- コンパイルエラーが解消されている

### 4. UnityEngine.UI 確認

#### 確認事項
- Unity UIモジュールが有効になっていることを確認
- `UnityEngine.UI` 名前空間が利用可能
- `ScrollRect`, `VerticalLayoutGroup` クラスが利用可能
- manifest.jsonに `"com.unity.modules.ui": "1.0.0"` が含まれていることを確認（既に含まれている可能性あり）

## コーディング規約
- manifest.jsonのJSON形式を正しく維持する
- パッケージバージョンはUnity 6 (or 2022 LTS) と互換性のあるバージョンを使用

## 参考情報
- 前タスクレポート: `docs/inbox/REPORT_TASK_002_LogicImplementation.md` を参照
- プロジェクト仕様: `最初のプロンプト`（プロジェクトルート）を参照
- Unityバージョン: Unity 6 (or 2022 LTS)
- 現在のmanifest.json: `Packages/manifest.json` を参照

## 注意事項
1. **DOTween Pro**: DOTween Proが有料パッケージの場合、Asset Storeから購入・インストールが必要です。Free版（DOTween）を使用する場合は、manifest.jsonに適切なパッケージIDを追加してください。
2. **Unityエディタ**: Unityエディタが起動していない場合でも、manifest.jsonを直接編集することでパッケージを追加できます。ただし、Unityエディタで開いた際にパッケージが正しくインストールされることを確認してください。
3. **パッケージバージョン**: パッケージバージョンはUnity 6 (or 2022 LTS) と互換性のあるバージョンを使用してください。最新バージョンを使用する場合は、互換性を確認してください。
4. **コンパイルエラー**: パッケージインストール後、全てのコンパイルエラーが解消されていることを確認してください。
