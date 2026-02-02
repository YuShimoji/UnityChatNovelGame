# Worker Prompt: TASK_016_VerificationTools

## 目的
MCP環境下で手動検証（操作・スクショ撮影）が困難な問題を解決するため、PlayMode実行時に証拠を自動生成するユーティリティを実装してください。

## 参照
- チケット: `docs/tasks/TASK_016_VerificationTools.md`
- SSOT: `docs/Windsurf_AI_Collab_Rules_latest.md` (または `.shared-workflows/docs/Windsurf_AI_Collab_Rules_latest.md`)

## 行動指針
- Focus Area: `Assets/Scripts/Utils/`
- Forbidden Area: `Assets/Scripts/Core/` (ロジック実装を壊さないこと)
- 探索: Unityの `ScreenCapture.CaptureScreenshot` や `Application.logMessageReceived` の仕様を調査してください。

## 実装要件
1. **VerificationCapture.cs**:
   - シーン開始時 (`Start`) または指定タイミングでスクショ撮影
   - 保存先は `docs/evidence/` 配下にすること（パス解決に注意）
   - ファイル名はユニークに（タイムスタンプ推奨）
2. **検証用シーン**:
   - `Assets/Scenes/DebugVerification.unity` (新規作成または既存利用)
   - スクリプトをアタッチして PlayMode で動作確認

## 停止と報告
- **成功時**:
  - `docs/evidence/` に生成された画像のファイル名を確認
  - `docs/inbox/REPORT_TASK_016_VerificationTools.md` に結果を記述
- **失敗時**:
  - エラーログと原因分析を報告
