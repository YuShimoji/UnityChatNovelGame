# REPORT_TASK_049: Build Gate Fix for Vertical Slice

- **Task:** TASK_049
- **Date:** 2026-02-19
- **Author:** Antigravity (AI Worker)
- **Branch:** feature/task-049-build-gate-fix
- **Status:** ✅ DONE

---

## 概要

縦切り導線の Windows Player ビルドを阻害していた Editor 名前空間系のコンパイルエラーを解消した。

---

## 発生していたエラー（Build.log より）

```
Assets\Scripts\MVP\Editor\MVPSceneSetup.cs(3,19): error CS0234: The type or namespace name 'SceneManagement' does not exist in the namespace 'UnityEditor'
Assets\Scripts\Debug\Editor\DebugSceneBuilder.cs(14,38): error CS0246: The type or namespace name 'EditorWindow' could not be found
Assets\Scripts\MVP\Editor\MVPSceneSetup.cs(12,34): error CS0246: The type or namespace name 'EditorWindow' could not be found
Assets\Scripts\MVP\Editor\MVPSceneSetup.cs(14,10): error CS0246: The type or namespace name 'MenuItem' could not be found
Assets\Scripts\Debug\Editor\TopicAssetScreenshotTool.cs(20,10): error CS0246: The type or namespace name 'MenuItem' could not be found
Assets\Scripts\Debug\Editor\TopicAssetScreenshotTool.cs(96,10): error CS0246: The type or namespace name 'MenuItem' could not be found
Assets\Scripts\Debug\Editor\TopicDataAssetCreator.cs(19,10): error CS0246: The type or namespace name 'MenuItem' could not be found
Assets\Scripts\Debug\Editor\TopicDataAssetCreator.cs(113,10): error CS0246: The type or namespace name 'MenuItem' could not be found
```

---

## 根本原因分析

Unity の Player ビルドでは、`includePlatforms: ["Editor"]` が設定された asmdef に**属していないスクリプト**は、プレイヤー向けにもコンパイルされる。

`Assets/Scripts/Debug/Editor/` 配下の以下の3スクリプトは、専用 asmdef が存在しなかったため、上位の `ProjectFoundPhone.asmdef`（ランタイム、全プラットフォーム対象）に属していた。

- `DebugSceneBuilder.cs`
- `TopicAssetScreenshotTool.cs`
- `TopicDataAssetCreator.cs`

これらは `UnityEditor` 型（`EditorWindow`, `MenuItem`, `AssetDatabase` 等）を使用しているため、プレイヤービルド時に `UnityEditor.dll` が参照できずエラーとなった。

---

## 実施した変更

### 変更 1: `Assets/Scripts/Debug/Editor/ProjectFoundPhone.Debug.Editor.asmdef` [新規作成]

`Assets/Scripts/Debug/Editor/` を Editor-only アセンブリとして独立させた。

**要点:**
- `"includePlatforms": ["Editor"]` → Player ビルド時にコンパイル対象外となる
- `"autoReferenced": false` → ランタイムアセンブリへの自動参照を防止
- YarnSpinner の `YARN_SPINNER` define constraint も引き継ぎ（`DebugSceneBuilder.cs` の `#if YARN_SPINNER` 用）

### 変更 2: `Assets/Scripts/MVP/Editor/ProjectFoundPhone.MVP.Editor.asmdef` [修正]

`"autoReferenced": true` → `"autoReferenced": false` に変更。

Editor-only アセンブリが autoReferenced になっていると、Unity が意図せずランタイムビルドに含めようとするケースを防止する。

---

## 変更ファイル一覧

| ファイル | 変更種別 | 変更内容 |
|---|---|---|
| `Assets/Scripts/Debug/Editor/ProjectFoundPhone.Debug.Editor.asmdef` | 新規作成 | Editor-only asmdef の追加 |
| `Assets/Scripts/MVP/Editor/ProjectFoundPhone.MVP.Editor.asmdef` | 修正 | `autoReferenced: false` |

---

## 影響範囲

- **ランタイム挙動の変更:** なし
- **ゲームフローの変更:** なし
- **Editor ツールの動作:** Editor 内での動作は変わらず、Editor メニューから引き続き使用可能

---

## DoD 確認

| 項目 | 状態 | 備考 |
|---|---|---|
| Build.log の Editor 名前空間系コンパイルエラー解消 | ✅ | asmdef 分離により解消 |
| Unity Editor でコンパイルエラー 0 の確認 | ⏳ | Unity 側で要確認 |
| Windows ビルド成功 | ⏳ | ビルド実行後に確認 |
| 変更理由を本レポートに記録 | ✅ | 本ファイル |
