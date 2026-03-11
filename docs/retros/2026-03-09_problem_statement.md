# Retrospective Problem Statement (2026-03-09)

## Theme
Stability debt is currently mixed with feature progress, which slows decision velocity.

## Observed Problems
1. Package-layer noise (duplicate assemblies, lock drift) appears in the same sessions as feature validation.
2. Doc truth is split across SSOT, inventory, and StorySpec notes with partial overlap.
3. Setup scripts encode environment assumptions (`-noUpm`, autostart overrides) not centrally documented.

## Why this matters now
- Upcoming C-branch experimentation will increase complexity in option flow and state propagation.
- If baseline stability is not isolated first, feature regressions and environment regressions become hard to separate.

## Proposed operating rule (next cycle)
- Run each cycle as two lanes:
  - Lane 1: environment/package stability checks
  - Lane 2: gameplay feature changes
- Merge only when Lane 1 stays green for the same baseline session.
