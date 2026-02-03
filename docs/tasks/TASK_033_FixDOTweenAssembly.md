# Task Ticket: Fix DOTween Assembly Definitions

## Status
- [ ] OPEN
- [ ] IN_PROGRESS
- [ ] DONE

## Context
The project uses `DOTween` and `DOTweenPro` from `Assets/Plugins/Demigiant`. However, these folders lack `.asmdef` files. Since the main game code (`Assets/Scripts`) is now wrapped in `ProjectFoundPhone.asmdef`, it cannot access the DOTween source code (Modules, Pro extensions) which defaults to `Assembly-CSharp-firstpass`.
This causes `CS1061` and `CS1929` errors in `GlitchEffect.cs` because `RectTransform.DOShakeAnchorPos` and `Image.DOFade` are not visible.

## Objectives
- Create a new Assembly Definition file for DOTween.
- Update `ProjectFoundPhone.asmdef` to reference it.
- Resolve invalid `DOTween` / `DOTweenPro` references.

## Focus Area
- `Assets/Plugins/Demigiant/` (Create .asmdef)
- `Assets/Scripts/ProjectFoundPhone.asmdef` (Update refs)

## Forbidden Area
- Moving DOTween files (keep folder structure).

## Constraints
- The new ASMDEF must reference `UnityEngine.UI` and `Unity.TextMeshPro` to support DOTween's modules for those systems.

## Definition of Done (DoD)
- [ ] `Assets/Plugins/Demigiant/Demigiant.DOTween.asmdef` created.
- [ ] `ProjectFoundPhone.asmdef` updated to reference `Demigiant.DOTween`.
- [ ] `GlitchEffect.cs` compiles without errors.
- [ ] Unity Compilation succeeds.
