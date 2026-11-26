# CLAUDE.md — project entry pointer

This file is intentionally thin. Do not grow it into an operations manual,
status snapshot, roadmap, report template, or handoff log.

## Read Order

1. `docs/REPO_LOCAL_RULES.md`
2. `docs/HANDOFF.md`
3. The canonical document directly related to the active artifact

Use `docs/ai/READ_ORDER.md` as a reference index only when the owner document is unclear. Read additional docs only when the current task needs them. If rules conflict,
prefer the narrower project-local canonical document over this pointer.

## Authority

- User / developer instructions override this file.
- `docs/REPO_LOCAL_RULES.md` owns daily operating rules and restart budget.
- `docs/INVARIANTS.md` owns non-negotiable product and workflow boundaries.
- `docs/HANDOFF.md` owns the live current position; `docs/runtime-state.md` owns environment and validation facts.
- `docs/project-context.md` owns longer-running direction and decision context.

## Anti-Growth Rule

Do not add project history, temporary plans, model/runtime settings, or
client-specific permissions here. Put those in the narrow owner document, or
leave runtime settings in the user's Codex / Claude client configuration.
