# Sites Authoring Bridge v1 Verification

## Mission

- Launch set: `UCNG-20260726-01`
- Mission: `UCNG-SITES-AUTHORING-BRIDGE`
- Attempt: `1`
- Worktree: `C:\Users\thank\Storage\Game Projects\UnityChatNovelGame-sites-authoring-bridge-v1`
- Branch: `codex/sites-authoring-bridge-v1`
- Start revision: `2f3753495dbb4dbdce1bf3fac763a057ac1442d2`
- Remote baseline: `origin/main` at `73aef720933a630024b2e6ee460a02e74f70bf94`

The start revision was one local commit ahead of `origin/main`. The pre-existing
`origin/main..HEAD` change set was verified as documentation-only before this
mission began. Work was performed in an isolated worktree. The original `main`
checkout was not modified.

## Before and After

Before this slice, `sites/foundphone-demo` was a manually authored, local static
prototype backed by `content/demo.json`. The Unity Writer Cockpit could inspect
and validate Yarn nodes but could not generate content for the Sites preview.

After this slice:

- the Writer Cockpit can export the selected verification node as deterministic
  Sites Preview Package v1 JSON;
- the static renderer retains the original fixture mode and adds a fixed,
  query-selected generated-package mode;
- the chat surface itself advances sequential content by pointer activation,
  while the native Continue button remains available;
- choice activation remains explicit and isolated from surface advancement;
- the renderer remains local-only, storage-free, analytics-free, and
  authentication-free.

## Export Contract

- Schema: `foundphone.sites-preview-package`
- Schema version: `1`
- Content label: `Prototype content / not final story`
- Canon status: `non-canon Unity/Yarn verification preview`
- Output: `sites/foundphone-demo/content/generated-preview.json`
- Selected node: `SP023_NarrationMargin_Start`
- Source: `Assets/Resources/Yarn/active/SP023_NarrationMarginDemo.yarn:10`
- Exported display lines: `8`
- Package identity SHA-256:
  `50b235a22ce1550d3586ce01106457e8844a72fd752c330711713221b931bc77`
- Source content SHA-256:
  `7374c31a69ed6937dc0190ba527b5b78ed6df9d9814a2cab586f7e08dcdeaab2`
- Generated file SHA-256:
  `ec57d11d633903e6d1270122fc065890380579493f252962db3bda4464dc4e66`

The generated JSON is intentionally ignored by Git. Running the export twice
produced the same file SHA-256 and package identity.

Supported constructs in Package v1:

- plain text
- speaker assignment
- `SystemMessage`
- `Narration`

Non-blocking unsupported-command diagnostics for the selected node:

- `StartWait`: 7 occurrences
- `BubbleMargin`: 2 occurrences

Unknown or flow-changing commands, including `jump`, fail the export. Export is
limited to verification nodes prefixed with `DQT_`, `SP023_`, or `SP024_`;
canonical Chapter 1 content is rejected before source reading.

## Writer Cockpit Access

1. Open the existing Writer Cockpit window.
2. Select a supported non-canonical verification node with a known asset
   location.
3. Use `Export Sites Preview Package`.
4. Read the package status, output path, line count, diagnostic count, and
   identity displayed in the window.

The existing Refresh, Validate, Apply, and Play paths were left unchanged.

## Local Preview Access

From the repository root:

```powershell
python -m http.server 4320 --directory sites/foundphone-demo
```

- Original fixture: `http://127.0.0.1:4320/`
- Generated Package v1: `http://127.0.0.1:4320/?content=generated`

The query mode maps only to the fixed local generated-package path. It does not
accept an arbitrary URL or file path. Both modes continuously display the
prototype/non-final-story notice. The fixture mode remains sourced from
`content/demo.json` and keeps its two-choice branching and distinct endings.

## Interaction and Responsive Evidence

In-app browser QA verified:

- chat-surface click/tap advances one ready message;
- a 42 px pointer drag does not advance;
- text selection and interactive descendants do not trigger surface advance;
- choice activation produces only the selected reply before its continuation;
- Enter and Space activate the native Continue and choice buttons without a
  duplicate advance;
- restart returns the fixture to progress 1 with one displayed message;
- generated mode renders all eight lines in source order;
- the stable chat shell keeps controls visible while the message viewport
  scrolls internally.

Responsive checks:

- at a 390 px-class portrait viewport, document `clientWidth` and `scrollWidth`
  were both 375 px and the controls remained fully visible;
- at 320 x 700, document `clientWidth` and `scrollWidth` were both 320 px,
  restart remained visible, and both choice buttons stayed within the viewport;
- a wide desktop viewport showed wrapped long content with no page-level
  horizontal overflow;
- browser console errors and warnings: none.

The existing native buttons, focus-visible treatment, keyboard operation,
`aria-live`, progressbar semantics, and reduced-motion behavior remain present.

## Technical Verification

- `git diff --check`: passed
- JavaScript syntax checks for the renderer and validators: passed
- `node tools/sites/validate-demo.mjs`: passed
- `node tools/sites/validate-preview-package.mjs`: passed
- Unity batch compile/open: exit 0, no compiler errors
- Targeted EditMode suite `ProjectFoundPhone.Editor.Tests`:
  18 passed, 0 failed, 0 skipped
- Batch export through
  `ProjectFoundPhone.Editor.SitesPreviewBatch.ExportVerificationNode`: passed
- Repeated export byte identity: passed
- Local HTTP response and MIME checks for HTML, CSS, JavaScript, fixture JSON,
  and generated JSON: passed

The full 97-test suite and persistent-save tests were intentionally not run.

## Protected Boundaries

No tracked changes were made to:

- active Yarn content
- Chapter 1 canon
- runtime Unity UI
- `SaveManager` or persistent-save data
- `Packages/`
- `ProjectSettings/`
- `docs/HANDOFF.md`
- `docs/runtime-state.md`
- `docs/PROJECT_COCKPIT.md`
- `docs/SUPERVISOR_REPORT.md`

No hosted Sites project, version, deployment, access policy, or domain was read
or mutated during this slice. No push, merge, deployment, publication, package
upgrade, audit fix, database, persistent storage, external analytics, external
communication, authentication, form, payment path, API key, or secret was
introduced.

## Environment Intrusion

Unity created only normal ignored worktree-local `Library` and `Logs` state plus
expected tracked `.meta` files for new source directories and tests. Temporary
local HTTP servers were stopped. Browser QA tabs were finalized. Transient
screenshots and the generated preview JSON were not committed.

## Bounded Debt and Next Gates

- Package v1 is a linear authoring bridge. Choice, conditional, and jump export
  remain outside the accepted subset and must continue to fail closed.
- `StartWait` and `BubbleMargin` are reported as diagnostics but are not
  reproduced in the Sites renderer.
- Two-choice continuation remains proven by the manually authored fixture;
  generated-package choice authoring is a future schema/version slice.
- Hosted Sites behavior for this revision is unverified because hosted access
  and mutation were outside mission authority.
- Writer editability is proven only for the bounded non-canonical Package v1
  subset. Any canon-content, runtime-UI, save-system, hosted-preview, or public
  release step requires separate authorization and acceptance.
