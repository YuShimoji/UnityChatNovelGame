# Worker Prompt: TASK_001_UnityProjectStructure

## 参照
- チケット: `Docs/tasks/TASK_001_UnityProjectStructure.md`
- SSOT: `Docs/Windsurf_AI_Collab_Rules_latest.md`
- HANDOVER: `Docs/HANDOVER.md`
- AI_CONTEXT: `AI_CONTEXT.md`
- MISSION_LOG: `.cursor/MISSION_LOG.md`

## 境界
- **Focus Area**: 
  - `Assets/Scripts/` 配下のディレクトリ構造作成（Core/, UI/, Data/, Logic/）
  - `Assets/Resources/` 配下のディレクトリ構造作成（Yarn/, Topics/）
  - `Assets/Prefabs/`, `Assets/Sprites/` などの基本ディレクトリ作成
  - Unity プロジェクトの基本設定（レイヤー、タグ、シーン構成等）
  - 各ディレクトリに `.gitkeep` または適切なプレースホルダーファイルの配置

- **Forbidden Area**: 
  - 既存のUnityプロジェクトファイル（存在する場合）の破壊的変更
  - コアシステムの実装（本タスクは構造整理のみ）
  - アセットの実制作（構造とプレースホルダーのみ）

## 目的
Unity プロジェクトのディレクトリ構造を仕様書に基づいて整理し、開発環境の基盤を構築する。MVCパターンに準拠したフォルダ構成を作成し、コアシステム実装の前提条件を整える。

## コンテキスト
- プロジェクトは Unity 2022.3 LTS 以降を使用
- チャットノベルゲーム（Lost Phone系）の開発を開始する
- 仕様書（`Docs/Core Specification`, `Docs/Core System仕様書`）に基づいた構造が必要
- 現在、Unity プロジェクトのディレクトリ構造が未整備の状態

## DoD
- [ ] `Assets/Scripts/` 配下に Core/, UI/, Data/, Logic/ ディレクトリが作成されている
- [ ] `Assets/Resources/` 配下に Yarn/, Topics/ ディレクトリが作成されている
- [ ] `Assets/Prefabs/`, `Assets/Sprites/` などの基本ディレクトリが作成されている
- [ ] 各ディレクトリに `.gitkeep` または適切なプレースホルダーファイルが配置されている
- [ ] Unity Editorでプロジェクト構造が正しく表示されることを確認
- [ ] 仕様書に記載された構造要件を満たしていることを確認
- [ ] `docs/inbox/` にレポート（REPORT_TASK_001_*.md）が作成されている
- [ ] 本チケットの Report 欄にレポートパスが追記されている

## 停止条件
- Forbidden Area に触れないと完遂できない
- 仕様の仮定が 3 つ以上必要
- 既存のUnityプロジェクトファイルの破壊的変更が必要
- Unity プロジェクトが存在しない場合、新規作成が必要（これは許可される）

## 納品先
- `docs/inbox/REPORT_TASK_001_*.md`（ISO8601形式のタイムスタンプを含む）

## 制約
- テスト: 主要パスのみ（ディレクトリ構造の確認、Unity Editorでの表示確認）
- フォールバック: 新規追加禁止（既存の構造がある場合は確認してから調整）
- Unity 2022.3 LTS 以降のバージョンに対応
- MVCパターンに準拠した構造を維持すること

## 補足情報
- 仕様書の参照パス:
  - `Docs/Core Specification` - プロジェクト全体仕様書
  - `Docs/Core System仕様書` - コアシステム仕様書
  - `最初のプロンプト` - 初期設計ドキュメント
- 必須パッケージの導入は別タスクとする可能性がある（本タスクでは構造のみ）
- Unity プロジェクトが既に存在する場合は、既存構造を確認してから調整すること
