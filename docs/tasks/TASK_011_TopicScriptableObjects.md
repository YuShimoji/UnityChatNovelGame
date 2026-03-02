# Task: Topic ScriptableObjects Creation

Status: DONE
Tier: 2
Branch: feat/topic-scriptableobjects
Owner: Worker
Created: 2026-01-17T02:00:00+09:00
Updated: 2026-03-01T06:10:00+09:00
Report: docs/reports/REPORT_TASK_011_TopicScriptableObjects.md

## Objective

`TopicData` ScriptableObject アセットを作成し、`UnlockTopicCommand` と `DeductionBoard` で利用できる状態にする。

## Current Status

- `Assets/Resources/Topics/` に topic assets が存在する
- `TopicDataAssetCreator` によりアセット生成と `Resources.Load` テストが実装済み
- `TASK_027` full playthrough で topic unlock と DeductionBoard 追加が latest evidence として確認済み

## DoD (Definition of Done)

- [x] `Assets/Resources/Topics/` ディレクトリが存在する
- [x] 初期シナリオで使用する topic asset が複数存在する
- [x] `Resources.Load<TopicData>` で読み込める
- [x] `UnlockTopicCommand` 経由で topic unlock が確認されている
- [x] `DeductionBoard` 上で topic 表示が確認されている
- [x] automation-first evidence に基づいて report が更新されている

## Evidence

- `Assets/Resources/Topics/`
- `docs/reports/REPORT_TASK_011_TopicScriptableObjects.md`
- `docs/reports/REPORT_TASK_027_FullPlaythroughTest.md`
