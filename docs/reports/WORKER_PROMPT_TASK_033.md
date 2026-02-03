# Worker Prompt: TASK_033_FixDOTweenAssembly

## 目的
DOTween (`Assets/Plugins/Demigiant`) 用の Assembly Definition を作成し、メインアセンブリからの参照を確立して `GlitchEffect.cs` 等のコンパイルエラーを解消する。

## 参照
- チケット: `docs/tasks/TASK_033_FixDOTweenAssembly.md`
- SSOT: `docs/Windsurf_AI_Collab_Rules_latest.md`
- HANDOVER: `docs/HANDOVER.md`

## 境界
- **Focus Area**:
  - `Assets/Plugins/Demigiant/` (ASMDEF作成)
  - `Assets/Scripts/ProjectFoundPhone.asmdef` (参照更新)
- **Forbidden Area**:
  - `DOTween` フォルダ構造の変更
  - 他のプラグインへの影響（`Demigiant` フォルダ以下のみ対象）

## 手順
1. **ASMDEF作成**: `Assets/Plugins/Demigiant/Demigiant.DOTween.asmdef` を作成する。
   - `name`: "Demigiant.DOTween"
   - `references`: `["Unity.TextMeshPro", "UnityEngine.UI"]` (UIとTMPサポートのため必須)
   - `includePlatforms`: [] (All)
   - `autoReferenced`: true

2. **参照更新**: `Assets/Scripts/ProjectFoundPhone.asmdef` を編集する。
   - `references` リストから誤った `"DOTween"` や `"DOTweenPro"` を削除する。
   - 新しい `"Demigiant.DOTween"` を追加する。

3. **検証**: コンパイルエラー（特に `GlitchEffect.cs` の `DOShakeAnchorPos` など）が消えるか確認する。

## DoD (Definition of Done)
- [ ] `Demigiant.DOTween.asmdef` が存在し、適切な参照設定になっている。
- [ ] `ProjectFoundPhone.asmdef` が上記を正しく参照している。
- [ ] コンパイルエラー `CS1061`, `CS1929` が解消されている。

## 停止条件
- DOTween のバージョンが古く、ASMDEF化によって内部エラーが多発する場合（その場合は、DOTweenフォルダをASMDEF化せず、ProjectFoundPhone.asmdef を削除する等のロールバック戦略が必要になるが、まずはASMDEF化を試みる）。

## 納品先
- `docs/inbox/REPORT_TASK_033_FixDOTweenAssembly.md`
