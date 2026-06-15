# AGENTS.md — UnityChatNovelGame entry pointer

This file is intentionally thin. Do not grow it into an operations manual,
status snapshot, roadmap, report template, or handoff log.

## Read Order

1. `docs/REPO_LOCAL_RULES.md`
2. `docs/ai/READ_ORDER.md`
3. `docs/HANDOFF.md`
4. `docs/runtime-state.md`

Read additional docs only when the current task needs them.

## Hard Rules

- Respond in Japanese.
- Do not use emoji.
- Do not read `docs/archive/` unless explicitly asked.
- When exploring code, use Grep/Glob to locate symbols instead of reading entire `.cs` files.
- Keep responses concise; avoid repeating file contents back to the user.
- Do not add repo-local Codex model, approval, sandbox, or client-runtime pins.

## Authority

- User / developer instructions override this file.
- Project-local canonical docs override global Codex fallback rules.
- `docs/REPO_LOCAL_RULES.md` owns daily operating rules and restart budget.
- `docs/INVARIANTS.md` owns non-negotiable product and workflow boundaries.
- `docs/HANDOFF.md` and `docs/runtime-state.md` own current state.
