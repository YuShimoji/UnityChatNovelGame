# DECISION_GATES.md
Ruleset-Version: v19
Status: canonical

## Active Artifact and Change Relation
Each block must know:
- active artifact
- artifact surface
- current bottleneck
- change relation: `direct`, `unblocker`, `cleanup`, or `evidence-only`

If change relation is repeatedly `cleanup` or `evidence-only`, do not use pendulum logic. Identify the actual bottleneck.

## Risk-tiered Execution Gate

Do not apply the same approval cost to every change.

- **Proceed**: a user-requested slice, reversible implementation, related bug fix, narrow test, and canonical status update. Complete these as one block without asking again.
- **Direction check**: a subjective choice with high rework cost, such as a layout system, visual language, animation style, localization policy, or new content direction. Before full implementation, show 2–3 materially different options, a recommendation, and the smallest comparable sample. State the bounded integration and acceptance conditions that the choice will approve; then ask once and batch the selected direction through verification.
- **Hard stop**: destructive changes, new dependencies, DB/save/auth/API contract changes, specification conflicts, or missing authority for an external side effect.

Ordinary warnings, minor unknowns, and implementation details are not hard stops. Prefer a reversible assumption and report it at close.

## Success Transition Gate
After a success definition is reached, do not invent a new frontier just to keep moving. Move only to:
- approved next frontier
- explicit user request
- a verified blocker that prevents the artifact path from continuing

## Frontier Re-entry Gate
The following do not become standard options without explicit re-approval:
- rejected
- hold due to unresolved prerequisites
- quarantined
- boundary-stopped or responsibility-external items

## Value Validation Gate
Use this gate for a new proposal whose value path is uncertain. Do not repeat it for an explicitly requested implementation, a bug with a reproduced impact, or a related fix inside an approved slice. For a genuinely new proposal, answer all of these in one sentence each:
1. What workflow step or integration point will the output feed into?
2. What manual step, judgment, copy, or transfer is actually removed?
3. If an external GUI/API is still the real integration point, does manual transfer remain?

If these cannot be answered, or the answer is effectively “manual copy still remains and little friction is removed”, stop and return the item as value-unverified.

## Bottleneck Proof Gate
A proposed next task must state the current bottleneck it resolves.
If the reason is only “we have done too much of X lately”, reject the proposal.

## Actor / Owner Gate
Every major next-step option and every subjective direction check must make clear:
- actor: `user`, `assistant`, `tool`, `shared`
- owner artifact: what artifact this actor actually owns
If the task is a human-owned creative/manual step, the assistant may support or scaffold it, but must not silently become the actor.
Proposal, research, and low-cost comparison do not transfer ownership. The assistant may initiate them while the user retains the final creative judgment.

## Workflow-Proof Gate
If the project depends on a human-authored production workflow, do not jump to quantity expansion (content writing, asset proliferation, mass production) before the workflow has been proven once end-to-end.
Examples of workflow proof:
- author → validate → generate → preview
- operator edits → tool runs → result observed

## Read-Only Refresh Gate
During REFRESH / REANCHOR / SCAN / AUDIT:
- no writes to long-lived repo files
- no commits / pushes
- no mutation justified only by “while we are here”
Creating local scratch notes is acceptable only if explicitly asked and clearly not treated as project progress.

## Write Failure Hard Stop
If any of the following occurs in the current block, stop before commit/push/handoff-complete:
- write failed
- readback mismatch
- permission denied
- tool output uncertain or truncated in a way that affects correctness

## Ask Hygiene Gate
Before asking:
- verify whether the answer already exists in canonical docs or recent verified context
- keep one intent per ask
- do not mix manual verification with next-direction choice
- do not use procedural yes/no traps as the main options
- collect related blocking decisions into one direction check; do not turn each implementation detail into a new prompt
- if the answer does not change a contract, a destructive action, or a high-rework subjective direction, choose a reversible default and continue
