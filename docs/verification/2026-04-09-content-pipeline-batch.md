# Content Pipeline — batchmode 検証メモ（C レーン / Unlock）

**目的**: `YarnContentValidator` → `YarnSOGenerator.SyncAllAuthoringAssets` を GUI なしで再現し、CI やエージェントからも同じ経路を叩けるようにする。

## 実装

- `ProjectFoundPhone.Editor.ContentPipelineBatch`（`Assets/Scripts/Editor/ContentPipelineBatch.cs`）
  - `RunYarnValidatorBatch` — Console に出力、エラー件数 > 0 で終了コード 1
  - `RunSyncAuthoringAssetsBatch` — 同期のみ、成功で 0
  - `RunValidateThenSyncBatch` — 検証でエラーなら Sync せず 1

## 実行例（Windows / Unity 6000.3.6f1）

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.3.6f1\Editor\Unity.exe" `
  -batchmode -quit `
  -projectPath "C:\Users\PLANNER007\UnityChatNovelGame" `
  -logFile "$env:TEMP\unity-content-pipeline.log" `
  -executeMethod ProjectFoundPhone.Editor.ContentPipelineBatch.RunValidateThenSyncBatch
```

## 制約・結果（2026-04-09）

- **プロジェクトロック**: 同一リポを通常の Unity Editor が開いていると、`Multiple Unity instances cannot open the same project` で即終了する。batch を回す前に Editor を閉じるか、別ワークツリーで実行する。
- **EditMode 全件**: 上記ロックのため、実装セッションでは Unity Test Runner（EditMode 75）の再実行は未実施。Editor を閉じたうえで batch を通すか、開いたままなら Test Runner から手動で回帰すること。
- **スクリプトコンパイル**: 初回バッチ実行でコンパイルエラー（CS1628）を検出した場合は修正後に再実行すること。修正後のコンパイルはローカル batch で成功を確認済み（`-quit` 前にドメインリロード完了）。
- **ログ**: `ContentPipelineBatch:` / `YarnContentValidator (batch):` / `YarnSOGenerator:` を `-logFile` で確認する。

## 手動との対応

| 手動（Content Pipeline ウィンドウ） | batch |
|-------------------------------------|--------|
| Open Yarn Validator | `RunYarnValidatorBatch` |
| Sync Authoring Assets | `RunSyncAuthoringAssetsBatch` |
| 両方 | `RunValidateThenSyncBatch` |
