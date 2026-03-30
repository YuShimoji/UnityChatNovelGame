# UI Issues (バッチ修正用)

コンテンツ制作中に発見した UI 問題を記録する。即修正せず蓄積し、3-5件溜まったら1ブロックでまとめて修正する。

## 運用ルール

- 発見時: このファイルに追記 (日付 + 症状 + 再現手順)
- 修正時: 1ブロックでまとめて対応。修正後に該当行を `[FIXED]` に変更
- 1件ごとに手動確認ループを回さない

## Open Issues

- 2026-03-30: DebugChatScene.unity の CanvasScaler が 1920x1080 のまま。コード (MetaEffectController, DebugSceneBuilder) は 1080x1920 に修正済み。DebugSceneBuilder で再生成するか、Inspector で手動修正が必要。再現: DebugChatScene を開き CanvasScaler の referenceResolution を確認

## Fixed Issues

(なし)
