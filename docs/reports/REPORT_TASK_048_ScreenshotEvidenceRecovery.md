# Report: Screenshot Evidence Recovery

Date: 2026-02-13
Owner: Worker
Status: COMPLETED

## Summary

`mcp2_manage_scene screenshot` では SceneView 相当の画像が混入する問題を再確認し、PlayMode テスト経由で `GameView` を撮る方式に切り替えました。  
`Title → Chat → Choice → End` の4画面を `docs/evidence/` に再生成し、テストでハッシュ一意性を検証しました。

## Evidence

- `docs/evidence/01_Title_Screen.png` - タイトル画面 (SHA256: `714E3A32...`)
- `docs/evidence/02_Chat_Screen.png` - チャット画面 (SHA256: `26F559E6...`)
- `docs/evidence/03_Choice_Screen.png` - 選択肢画面 (SHA256: `B8818FF4...`)
- `docs/evidence/04_End_Screen.png` - エンド画面 (SHA256: `358E8866...`)

### ハッシュ値検証結果

4枚すべて異なる画像であることを PlayMode テストで検証済み。

## Findings

- `mcp2_manage_scene screenshot` は本件の証跡用途では非推奨（SceneView/空シーン画像の混入リスク）。
- `ScreenCapture.CaptureScreenshot` を PlayMode 実行中に使うと `GameView` を安定取得できる。
- `VerificationCapture.cs` は `VerificationResults/` 出力で、状態遷移連動の証跡用途には不足がある。

## Technical Details

- 追加テスト: `Assets/Scripts/Tests/PlayMode/MVPScreenshotEvidencePlayModeTests.cs`
- 実行テスト: `ProjectFoundPhone.Tests.MVPScreenshotEvidencePlayModeTests.MVPFlow_CapturesDistinctGameViewEvidence`
- 判定ロジック:
  - `StartButton` クリックで Chat へ遷移
  - `ChoicePanel` 表示待ち
  - `ChoiceA` クリックで End へ遷移
  - 各状態で `docs/evidence/01-04_*.png` を保存し SHA256 一意性をアサート

## Next Actions

- 証跡は回収済み。Evidence Missing 状態は解消。
- 今後の再検証は本 PlayMode テストを再実行して更新する。
