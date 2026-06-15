# REPO_LOCAL_RULES.md — repo-local operating rules

UnityChatNovelGame の通常再開で読む短い front-door。ここには毎回効く行動ルールだけを置く。事故履歴、詳細な手順、報告テンプレート、個別スライスの作業ログは置かない。

## Restart Read Budget

通常再開で読むのは次の 4 点まで。

1. `AGENTS.md`
2. `docs/REPO_LOCAL_RULES.md`
3. `docs/HANDOFF.md`
4. `docs/runtime-state.md`

追加で読むのは、今の作業を進める根拠が不足している場合だけ。読む範囲は該当節・該当 ID・該当 artifact に限定し、全文読了を progress にしない。

## Core Rules

- Repo-local authority comes first. Global Codex files and prompt helpers are fallback only.
- `AGENTS.md`, `CLAUDE.md`, and `.claude/CLAUDE.md` are entry pointers. Do not add procedures, status, roadmaps, report formats, option menus, or history there.
- Stay inside this repo unless the user explicitly names a cross-project scope. If cross-project scope is explicit, touch only that scope.
- Use `rg` / glob-style search before opening code, and do not read entire `.cs` files just to locate symbols.
- Do not read `docs/archive/` unless explicitly asked.
- Keep task-specific scars out of this file. Put lane-specific constraints in the relevant spec, registry, handoff artifact, or `runtime-state`.

## Development Boundary

- The project is a Unity chat / visual novel game, but AI work should primarily improve engine, tool, pipeline, and verification capacity.
- Yarn story writing, character voice, and creative judgment belong to the user. Assistant work may scaffold, validate, or improve the workflow, but should not silently become the writer.
- Content changes are valid when they are the smallest practical probe for an engine/tool capability. Content volume alone is not progress.
- Values such as font, color, timing, and layout tuning are Inspector work unless a reusable system capability is missing.
- UI issues should be batched through `docs/UI_ISSUES.md` instead of one-by-one micro-fix loops.

## Codex / Client Runtime Config

- Do not track repo-local Codex model, approval policy, sandbox mode, or client runtime overrides.
- `.codex/config.toml`, `.codex/*.toml`, `.codex/hooks.json`, `.codex/hooks/`, and `.claude/settings.local.json` are local machine/client state, not project authority.
- Project rules live in visible docs. Runtime selection belongs to the user's Codex / Claude client configuration.

## Git And Tests

- Start by checking `git status --short --branch` when resuming substantial work.
- Pull remote changes with fast-forward intent before editing when the user asks for sync.
- For docs-only or viewer-only changes, prefer `git diff --check` and the narrow doc build over Unity test runs.
- For C# / scene / asset behavior changes, run the narrow relevant Unity/EditMode/PlayMode validation when the local Unity version can support it.
- Do not request manual visual verification for docs-only or runtime-config cleanup.

## Reporting Rule

Reports should make the work usable without forcing the user to open files. State what changed, why it matters, what remains uncertain, and what the next concrete move is.

When listing residual work or options, give each item enough context to choose: purpose, effect, prerequisite, current state, and next move. Avoid fixed English closeout labels unless the user explicitly asks for them.
