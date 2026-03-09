# 15. Feature Triage (2026-03-10)

## Purpose
- Do not treat all four items as net-new development.
- Separate implementation tasks, spec-definition tasks, and documentation tasks.

## Current Classification

### 1) Face Icon Display
- Status: Partially implemented
- Evidence:
  - `CharacterProfile` has `Icon` and `DisplayMode`.
  - `ChatController` handles NPC icon rendering and `NameOnly` / `IconOnly` / `IconAndName`.
- Gap:
  - Expected behavior matrix for Player/System/NPC is not fully documented.

### 2) Chat Room Thread Management
- Status: Partially implemented (UI and internal state are at different stages)
- UI subthread side:
  - Discord-like subthread concept is documented in StorySpec.
  - Runtime UI implementation is still mostly unimplemented.
- Internal branch-state side:
  - `BranchThreadState` exists.
  - `ScenarioManager` has snapshot/apply/begin/add-flag/end APIs.
  - `SaveData` and `SaveManager` are wired for save/load.
- Rule:
  - Keep "StorySpec subthread UI" and "C-branch state bridge" as separate tracks.

### 3) Text Animation System
- Status: Partially implemented
- Evidence:
  - Typewriter effect exists.
  - Typing indicator exists.
  - `StartWait` and fast-forward skip behavior exists.
- Gap:
  - Responsibility boundary between rendering-side and flow-control-side needs explicit spec.

### 4) Designer Authoring Environment / Guide / External Reference
- Status: Partially implemented
- Evidence:
  - `YarnEditingPipeline.md`, `ContentAuthoring` scene, `Play from Node`, and `ContentAuthoringBatchValidator` exist.
  - A single unified guide and external-share wiki pages are not yet prepared.
- Priority:
  - Prioritize clear authoring guidance and externalizable structure over engine feature expansion.

## Recommended Next Moves

### P0
1. Spec-definition: Separate SSOT for thread management (UI subthreads vs branch-state bridge).
2. Documentation: Build a single designer-facing authoring guide from existing assets.

### P1
1. Documentation: Prepare external-wiki-ready structure (chapters, summary pages, update rules), without fixing a publishing platform.
2. Spec-definition: Face icon behavior matrix (DisplayMode x speaker type).
3. Spec-definition: Text animation responsibility split (visual rendering vs flow timing).

### P2
1. Implementation: C-branch spike Step2+ (option entry, return hook, deterministic reflection message).

## Task Separation
- Implementation tasks:
  - C-branch step2+ slice implementation.
- Spec-definition tasks:
  - Thread model split.
  - Face icon behavior matrix.
  - Text animation contract.
- Documentation tasks:
  - Unified designer guide.
  - External reference wiki-ready edit package.
