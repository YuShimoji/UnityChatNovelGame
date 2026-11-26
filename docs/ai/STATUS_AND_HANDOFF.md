# STATUS_AND_HANDOFF.md
Ruleset-Version: v19
Status: canonical

## Feature status semantics

Priority and lifecycle status are separate.

| Status | Meaning | Allowed work |
|---|---|---|
| `proposed` | Value, direction, or scope is still being evaluated | Research, comparison, and a disposable low-cost prototype. No production integration |
| `approved` | Scope and acceptance conditions are defined, and the user has approved product implementation | Implementation may start and continue through related fixes, verification, and closeout |
| `in-progress` | Approved implementation is active | Complete the approved slice; do not expand it silently |
| `done` | The approved acceptance conditions and relevant verification are complete | Follow-up ideas return as new `proposed` items |
| `hold` | Worth retaining, but a prerequisite, timing, or value path blocks it | No normal implementation |
| `rejected` | Outside the current product or workflow direction | Do not re-enter without explicit re-evaluation |
| `quarantined` | Provenance or authorization is uncertain | Review individually before any other transition |

Selecting a `proposed` item only for comparison is not product implementation approval. If a direction-check question explicitly states the bounded production integration and frozen acceptance conditions, selecting one option is approval for that stated integration. An explicit user request to implement a bounded item is likewise approval for that requested slice; do not ask for the same permission again.

## FEATURE_REGISTRY discipline

Each major candidate should make the decision possible rather than merely accumulate an idea:

- hypothesis and user/workflow effect
- integration point and bottleneck addressed
- smallest comparable prototype or evidence
- actor and owner artifact
- acceptance condition that will be frozen after approval
- priority and lifecycle status

`approved` requires a clear boundary, a stated value path, no unresolved responsibility violation, and explicit user authorization for product implementation. `proposed` permits investigation and low-cost comparison without treating the result as shipped product work.

## One live handoff

`docs/HANDOFF.md` is the sole live current-position page. Update it by replacing stale state, not by appending another historical snapshot. Git history owns history.

A closeout keeps only what another session needs to act:

- update date and current commit or working-tree relation
- active artifact, current bottleneck, and achieved state
- verification evidence and its freshness
- remaining uncertainty or fragile local condition
- 2–4 next entry points that solve different bottlenecks
- explicit do-not-touch boundaries when relevant

`docs/runtime-state.md` owns shareable environment and validation facts. `docs/project-context.md` owns durable direction and roadmap. Neither duplicates the live handoff.

## Trust and uncertainty

When evidence is stale, incomplete, or a local workaround is fragile, state that in natural language beside the affected claim. Do not bury it in a fixed report template and do not turn every uncertainty into a blocker.

## No progress laundering

Do not claim progress merely because a document, framework-compliant report, test run, or audit exists. State what became usable, decidable, or less costly in the actual development path.
