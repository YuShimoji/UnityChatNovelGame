# Task: Unity プロジェクト構造の整理

Status: OPEN
Tier: 1
Branch: main
Owner: Worker-1
Created: 2026-01-08T13:55:40Z
Report: 

## Objective
- Unity プロジェクトのディレクトリ構造を仕様書に基づいて整理し、開発環境の基盤を構築する
- MVCパターンに準拠したフォルダ構成を作成する
- 必須パッケージ（Yarn Spinner, DOTween Pro, TextMeshPro）の導入準備を行う
- コアシステム実装の前提条件を整える

## Context
- プロジェクトは Unity 2022.3 LTS 以降を使用
- チャットノベルゲーム（Lost Phone系）の開発を開始する
- 仕様書（`docs/Core Specification`, `docs/Core System仕様書`）に基づいた構造が必要
- 現在、Unity プロジェクトのディレクトリ構造が未整備の状態

## Focus Area
- `Assets/Scripts/` 配下のディレクトリ構造作成:
  - `Core/` (GameManager, SaveManager, TimeManager等)
  - `UI/` (ChatController, DeductionBoard, NotificationManager等)
  - `Data/` (ScriptableObjects定義)
  - `Logic/` (Minigames, ExplorationThreads等)
- `Assets/Resources/` 配下のディレクトリ構造作成:
  - `Yarn/` (シナリオファイル)
  - `Topics/` (Topic ScriptableObjects)
- `Assets/Prefabs/` 配下の構造作成（UI Prefabs用）
- `Assets/Sprites/` 配下の構造作成（UI素材用）
- Unity プロジェクトの基本設定（レイヤー、タグ、シーン構成等）

## Forbidden Area
- 既存のUnityプロジェクトファイル（存在する場合）の破壊的変更
- コアシステムの実装（本タスクは構造整理のみ）
- アセットの実制作（構造とプレースホルダーのみ）

## Constraints
- テスト: 主要パスのみ（ディレクトリ構造の確認、Unity Editorでの表示確認）
- フォールバック: 新規追加禁止（既存の構造がある場合は確認してから調整）
- Unity 2022.3 LTS 以降のバージョンに対応
- MVCパターンに準拠した構造を維持すること

## DoD
- [ ] `Assets/Scripts/` 配下に Core/, UI/, Data/, Logic/ ディレクトリが作成されている
- [ ] `Assets/Resources/` 配下に Yarn/, Topics/ ディレクトリが作成されている
- [ ] `Assets/Prefabs/`, `Assets/Sprites/` などの基本ディレクトリが作成されている
- [ ] 各ディレクトリに `.gitkeep` または適切なプレースホルダーファイルが配置されている
- [ ] Unity Editorでプロジェクト構造が正しく表示されることを確認
- [ ] 仕様書に記載された構造要件を満たしていることを確認
- [ ] `docs/inbox/` にレポート（REPORT_TASK_001_*.md）が作成されている
- [ ] 本チケットの Report 欄にレポートパスが追記されている

## Notes
- Status は OPEN / IN_PROGRESS / BLOCKED / DONE を想定
- BLOCKED の場合は、事実/根拠/次手（候補）を本文に追記し、Report に docs/inbox/REPORT_...md を必ず設定
- Unity プロジェクトが既に存在する場合は、既存構造を確認してから調整すること
- 必須パッケージの導入は別タスクとする可能性がある（本タスクでは構造のみ）
