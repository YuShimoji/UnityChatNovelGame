# C-Branch Prototype Spike (2026-03-09)

## Goal
Validate a minimum "knowledge transfer / branch thread" experience without destabilizing current A/B mechanics.

## Prototype Scope (1 vertical slice)
- One temporary branch thread UI entry from chat.
- One branch node that returns to main chat with one transferred flag.
- One visible effect in main flow triggered by the transferred flag.

## Non-goals
- Full multi-branch authoring pipeline.
- Save/load schema migration for all branch states.
- Final UX polish.

## Implementation Sketch
1. [x] Add a small bridge model (`BranchThreadState`) to hold active branch id + transfer flags. (Implemented 2026-03-09)
2. Extend chat option handling with a single branch entry point.
3. Add return hook that writes one transfer flag into existing runtime state.
4. Reflect transfer in one deterministic chat/system message.

## Risks
- Option flow complexity in `RunOptionsAsync` behavior.
- Interaction with existing contradiction feedback timing.

## Exit Criteria
- Branch path can start and return once in play mode.
- Transfer flag effect is observable and logged.
- No regression in existing A/B flow smoke checks.


## Progress Update
- [x] Step 1 implemented on 2026-03-09:
  - Added `BranchThreadState`
  - Wired snapshot/apply in `ScenarioManager`
  - Wired save/load persistence in `SaveData` and `SaveManager`
