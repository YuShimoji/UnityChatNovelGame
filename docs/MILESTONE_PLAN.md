# Milestone Plan

## Metadata

- Last Updated: 2026-03-01T16:29:51+09:00
- Updated By: Codex Orchestrator

---

## Long Milestone

### LG-1: Production readiness

- Objective: move Addressables / CI / QA toward a reusable production baseline
- Target Window: 2026-06
- Status: started
- Progress: 45%
- Related:
  - `TASK_056_CIReadinessBaseline`
  - `TASK_057_QACharacterDatabaseEditModeCoverage`
  - `TASK_058_RemoteUnityEditModeCIPath`
  - later candidate: Addressables migration planning

---

## Mid Milestone

### MG-1: MVP verification and minimum release confidence

- Objective: close the main MVP verification loop and leave a stable state behind
- Target Window: 2026-03-05
- Status: completed
- Progress: 100%

---

## Short Milestone

### SG-1: MVP verification pack closure

- Objective: close the MVP checklist at the minimum acceptable level
- Target Window: 2026-03-02
- Status: completed
- Progress: 100%

---

## Current Map
```mermaid
gantt
    title Project Milestones
    dateFormat  YYYY-MM-DD
    section Long
    LG-1 :active, lg1, 2026-03-01, 2026-06-30
    section Mid
    MG-1 :done, mg1, 2026-02-12, 2026-03-01
    section Short
    SG-1 :done, sg1, 2026-02-12, 2026-03-01
```

---

## Keep / Problem / Try

### 2026-03-01

**Keep**

- The CI baseline created a reusable remote guard without requiring manual local testing.
- The QA slice stayed narrow and high-signal by focusing on `CharacterDatabase` EditMode behavior.
- The Unity EditMode CI path now exists as a concrete Layer B closing route.

**Problem**

- `TASK_056`, `TASK_057`, and `TASK_058` remain open on Layer B until the first remote runs are observed.
- User-side local modifications and one generated artifact still keep the worktree dirty.

**Try**

- Observe the first remote `repo-guards` and `unity-editmode-tests` runs.
- After remote execution is confirmed, choose the next LG-1 slice between Addressables planning and CI hardening.

---

## History

- 2026-03-01 16:29: `TASK_058_RemoteUnityEditModeCIPath` added to define the Unity EditMode CI path
- 2026-03-01 16:26: `TASK_057_QACharacterDatabaseEditModeCoverage` added to expand EditMode coverage
- 2026-03-01 16:21: `TASK_056_CIReadinessBaseline` added as the first LG-1 slice
