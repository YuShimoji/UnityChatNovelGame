# CORE_RULESET.md
Ruleset-Version: v19
Status: canonical
Audience: Claude Code, Codex, and any adapter that reads project-local AI rules.

## Purpose
This ruleset provides vendor-neutral principles for AI-assisted development. Daily execution, restart budget, and stop conditions are owned by `docs/REPO_LOCAL_RULES.md`; the files in `docs/ai/` define the detailed packet, gate, status, and phase semantics.
Adapters such as `.claude/CLAUDE.md` and `AGENTS.md` must stay thin and defer to those owner documents.

## Source-of-truth policy
- Daily execution/read/stop rules live in `docs/REPO_LOCAL_RULES.md`; detailed cross-client workflow semantics live in `docs/ai/*.md`.
- Normal restart order is owned by `docs/REPO_LOCAL_RULES.md`. `docs/ai/READ_ORDER.md` is a reference index, not a second mandatory reading sequence.
- Adapters, prompts, hooks, and helper agents are subordinate.
- Project-local canonical docs (`INVARIANTS`, `USER_REQUEST_LEDGER`, `OPERATOR_WORKFLOW`, `INTERACTION_NOTES`) are factual project memory, not optional decoration.
- If a rule conflicts with project-local canonical docs, first verify whether the docs reflect newer explicit user instruction.

## Core principles
### Artifact-first
Advance the active artifact or its verified delivery path. Docs, cleanup, tests, mocks, and surveys are supporting work unless they clearly unblock the artifact.

### Explain Once Canonicalization
If the user states a durable constraint, workflow pain, invariant, backlog item, or prohibited shortcut, write it into the appropriate canonical doc in the same block. Do not postpone that write to handoff.

### Question Dedup
Before asking, read the canonical rules and project-local canonical docs. Summarize what is already known, then ask only for missing deltas. Do not ask the user to re-explain known context.

### Frontier discipline
Do not re-open rejected, boundary-stopped, or quarantined frontiers as normal next steps. User interest in “looking again” is not automatic approval.

### Selection is not approval
If the user chooses a proposed item only for deeper review, that means “evaluate/specify this next”, not “approve implementation”. A direction-check question may make the bounded production integration and acceptance conditions explicit; in that case, selecting one option is approval for that stated integration. Do not leave the meaning of the choice implicit.
An explicit user request to implement a bounded slice is approval for that slice. Do not add a second permission round for reversible in-scope work.

### No pendulum compensation
Do not choose work because the previous sessions were “too much X” and therefore the next one should be “not-X”. Choose work based on the current bottleneck.

### Actor/owner discipline
Every major action has an actor and an owner artifact.
- actor = who performs the work now (`user`, `assistant`, `tool`, `shared`)
- owner = who owns the resulting artifact or judgment
Do not silently slide human-owned creative work into assistant execution.
Research, materially different options, and disposable comparison prototypes do not seize human ownership; they are encouraged when they reduce late rework.

### Read-only audit phases
REFRESH, REANCHOR, SCAN, AUDIT, and similar phases are read-only by default. They do not write repo state, commit, push, or mutate long-lived files unless the user explicitly asks for that mutation in the current block.

### Write failure hard stop
If a write fails, a readback mismatch occurs, or the result is uncertain, do not commit, push, or claim completion in that block. Repair or clearly stop.

### Verification cost principle
Manual verification and test execution are not proofs of progress. They are costs paid against a specific change. Apply in this order:
- If the change has **no visible/behavioral effect** (docs, internal Yarn nodes, refactors, internal string edits), do **not** request manual verification or test runs.
- If the change has localized behavioral effect, request verification **only for the affected surface**, not a full playthrough.
- Full manual playthroughs and full test suite runs belong to **SUBSEQUENT slices only**, not to every implementation slice.
- Unchecked-box checklists, "未実施" counters, and verification log directories must not become implicit todo lists that demand execution on every session.
- If multiple micro-edits accumulate in a block, propose one batched verification at the block's terminus — never per edit.

## Canonical doc roles
- `HANDOFF.md`: the sole live current position and next entry points
- `runtime-state.md`: shareable environment and validation facts, not a session diary
- `project-context.md`: durable product direction and roadmap, not live status
- `INVARIANTS.md`: non-negotiables, UX/algorithm invariants, role boundaries, prohibited shortcuts
- `USER_REQUEST_LEDGER.md`: durable requests, backlog deltas, unresolved user corrections
- `OPERATOR_WORKFLOW.md`: human/operator workflow, pain points, quality goals, manual vs assisted steps
- `INTERACTION_NOTES.md`: reporting style, ask hygiene, disliked patterns, manual verification conventions

## Evidence discipline
Use visual or artifact evidence whenever relevant. If evidence is stale or unknown, say so. Do not substitute documentation for actual observation when the question is about behavior.
