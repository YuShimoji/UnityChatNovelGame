# Task: Unityパッケージインストール
Status: DONE
Tier: 2
Branch: main
Owner: Worker
Created: 2026-01-06T10:15:00Z
Report: docs/reports/REPORT_TASK_004_PackageInstallation.md 

## Objective
Unityプロジェクトに必要なパッケージ（Yarn Spinner, DOTween, TextMeshPro）をインストールする。コンパイルエラーを解消し、実装済みコードが正常に動作する環境を整える。

実装対象：
1. **Yarn Spinner**: シナリオ管理とカスタムコマンド処理
2. **DOTween Pro**: UIアニメーション（スクロールアニメーション等）
3. **TextMeshPro**: テキスト表示（メッセージバブル、タイピングインジケーター）

## Context
- 前タスク（TASK_001, TASK_002）でコード実装が完了
- コンパイルエラーが発生している（必要なパッケージが未インストール）
- エラー内容:
  - `Yarn` 名前空間が見つからない
  - `DG` (DOTween) 名前空間が見つからない
  - `TMPro` (TextMeshPro) 名前空間が見つからない
  - `UnityEngine.UI` が見つからない（Unity UIモジュール）
- 参照ドキュメント: `最初のプロンプト`（プロジェクトルート）

## Focus Area
- `Packages/manifest.json` の更新
- Unity Package Managerを使用したパッケージインストール
- パッケージバージョンの確認と互換性チェック
- コンパイルエラーの解消確認

## Forbidden Area
- 既存ファイルの削除・破壊的変更
- Unityプロジェクト設定の変更（パッケージインストール以外）
- 実装済みコードの変更（パッケージインストールのみ）
- 新規スクリプトの作成

## Constraints
- テスト: コンパイルエラーが解消されることを確認
- フォールバック: パッケージインストールのみ（コード変更なし）
- パッケージバージョン: Unity 6 (or 2022 LTS) と互換性のあるバージョンを使用
- インストール方法: Unity Package Managerまたはmanifest.jsonの直接編集

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

## 実装詳細

### 1. Yarn Spinner インストール

#### パッケージ情報
- **パッケージ名**: `com.yarnspinner.yarnspinner`
- **インストール方法**: 
  - Unity Package Manager: `Window` → `Package Manager` → `+` → `Add package from git URL` → `https://github.com/YarnSpinner/YarnSpinner-Unity.git?path=/YarnSpinner-Unity`
  - または manifest.jsonに直接追加: `"com.yarnspinner.yarnspinner": "https://github.com/YarnSpinner/YarnSpinner-Unity.git?path=/YarnSpinner-Unity"`

#### 確認事項
- `Yarn.Unity` 名前空間が利用可能
- `DialogueRunner` クラスが利用可能
- コンパイルエラーが解消されている

### 2. DOTween Pro インストール

#### パッケージ情報
- **パッケージ名**: `com.demigiant.dottweenpro` (Pro版) または `com.demigiant.dottween` (Free版)
- **インストール方法**: 
  - Asset Storeからインストール（推奨）
  - または manifest.jsonに直接追加（Asset Storeパッケージの場合は別途対応が必要）
- **注意**: DOTween Proは有料パッケージのため、Asset Storeから購入・インストールが必要な場合があります

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

#### 確認事項
- `TMPro` 名前空間が利用可能
- `TextMeshProUGUI` クラスが利用可能
- コンパイルエラーが解消されている

### 4. UnityEngine.UI 確認

#### 確認事項
- Unity UIモジュールが有効になっていることを確認
- `UnityEngine.UI` 名前空間が利用可能
- `ScrollRect`, `VerticalLayoutGroup` クラスが利用可能

## Notes
- Status は OPEN / IN_PROGRESS / BLOCKED / DONE を想定
- BLOCKED の場合は、事実/根拠/次手（候補）を本文に追記し、Report に docs/inbox/REPORT_...md を必ず設定
- DOTween Proが有料パッケージの場合、Asset Storeから購入・インストールが必要です。Free版（DOTween）を使用する場合は、manifest.jsonに適切なパッケージIDを追加してください。
- Unityエディタが起動していない場合は、manifest.jsonを直接編集してパッケージを追加することも可能です。ただし、Unityエディタで開いた際にパッケージが正しくインストールされることを確認してください。
