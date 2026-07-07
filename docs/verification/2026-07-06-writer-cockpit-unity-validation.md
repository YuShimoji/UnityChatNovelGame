# Writer Cockpit Unity Validation Attempt

Date: 2026-07-06
Last rechecked: 2026-07-07

## Scope

Validate the Writer Cockpit MVP added under `Tools > FoundPhone > Writer Cockpit` using the project-required Unity version.

## Environment

- Project version: `6000.4.9f1`
- Unity executable checked: `C:\Program Files\Unity\Hub\Editor\6000.4.9f1\Editor\Unity.exe`
- Other Unity editors were not used.

## Commands / Logs

```text
Unity.exe -batchmode -quit -projectPath <repo> -logFile Logs\writer-cockpit-unity-open-2026-07-06.log
Unity.exe -batchmode -quit -projectPath <repo> -logFile Logs\writer-cockpit-unity-open-2026-07-06-rerun.log
Unity.exe -batchmode -quit -projectPath <repo> -logFile Logs\writer-cockpit-yarn-validator-2026-07-06.log -executeMethod ProjectFoundPhone.Editor.ContentPipelineBatch.RunYarnValidatorBatch
Unity.exe -batchmode -quit -projectPath <repo> -logFile Logs\writer-cockpit-cache-utc-timestamp-restored-2026-07-06.log
Unity.exe -batchmode -quit -projectPath <repo> -logFile Logs\writer-cockpit-final-yarn-validator-2026-07-06.log -executeMethod ProjectFoundPhone.Editor.ContentPipelineBatch.RunYarnValidatorBatch
Unity.exe -batchmode -quit -projectPath <repo> -logFile Logs\writer-cockpit-unity-open-2026-07-07.log
Unity.exe -batchmode -quit -projectPath <repo> -logFile Logs\writer-cockpit-yarn-validator-2026-07-07.log -executeMethod ProjectFoundPhone.Editor.ContentPipelineBatch.RunYarnValidatorBatch
```

`Logs/` is ignored by `.gitignore`; the logs are local evidence only.

## Result

Unity 6000.4.9f1 now reaches package registration, script compilation, and batch quit when the local generated Package Manager cache and the `Packages/manifest.json` / `Packages/packages-lock.json` timestamps match the cache metadata.

Passing batch-open evidence:

```text
[Package Manager] Restoring resolved packages state from cache
[Package Manager] Registered 39 packages:
Batchmode quit successfully invoked - shutting down!
Application will terminate with return code 0
```

The previous blocker is still reproducible if Package Manager is forced onto a fresh resolve path. Removing `Packages/packages-lock.json`, removing only `Library/PackageManager/ProjectCache*` / `projectResolution.json`, or changing `Packages/manifest.json` invalidates the cache and returns:

```text
[Package Manager] The "path" argument must be of type string. Received undefined
Failed to resolve packages: The "path" argument must be of type string. Received undefined. No packages loaded.
Application will terminate with return code 1
```

This happens before registry access is visible in the logs. It is not explained by malformed package JSON: `Packages/manifest.json`, `Packages/packages-lock.json`, `Assets/MCPForUnity/package.json`, and package cache `package.json` files parse, and no invalid required package fields were found.

`Packages/manifest.json` asks for `com.unity.test-framework` `2.0.1`, while the checked-in lock, local package cache, and Unity 6000.4.9f1 built-in package metadata resolve `com.unity.test-framework` as `1.6.0`. A trial manifest correction to `1.6.0` did not fix the fresh-resolve `path undefined` failure, so no manifest/lock source change was kept.

Non-mutating Yarn validator batch reached the validator:

```text
YarnContentValidator (batch): errors=0, warnings=33, info=3.
Scanned 11 files, 74 nodes, 24 #line: tags, 42 declared variables
```

The validator process returned exit code 0. The warnings are the existing unknown command / unknown character / undeclared variable warnings surfaced by the validator; they are not Package Manager or compile failures.

## Recheck (2026-07-07)

After `git pull --ff-only` reported the local `main` branch was already up to date, the restored local Package Manager state was revalidated without changing package source files or generated caches.

Batch open still reaches the recovered path:

```text
[Package Manager] Restoring resolved packages state from cache
[Package Manager] Registered 39 packages:
DisplayProgressbar: Compiling Scripts
*** Tundra build success (0.19 seconds), 0 items updated, 326 evaluated
Batchmode quit successfully invoked - shutting down!
Application will terminate with return code 0
```

The non-mutating Yarn validator batch also still reaches the validator:

```text
YarnContentValidator (batch): errors=0, warnings=33, info=3.
Scanned 11 files, 74 nodes, 24 #line: tags, 42 declared variables
```

`Packages/manifest.json` and `Packages/packages-lock.json` still parse as JSON. `Assets/MCPForUnity/package.json` also parses and contains required package identity fields (`name`, `version`, `displayName`). No `file:`, `../`, absolute Windows path, or package `path` field was found in the inspected package manifests/settings. The known manifest/lock mismatch remains: manifest requests `com.unity.test-framework` `2.0.1`, while lock/cache resolve built-in `1.6.0`.

No interactive Unity Editor menu pass was performed in this recheck. `Tools > FoundPhone > Writer Cockpit` and `Tools > FoundPhone > Content Pipeline` remain static `MenuItem` sources only until a visible Editor pass confirms the actual menu and Cockpit UI.

## Static Checks Completed

- `ProjectSettings/ProjectVersion.txt` requires `6000.4.9f1`.
- `Packages/manifest.json` parses with PowerShell `ConvertFrom-Json`.
- `Packages/packages-lock.json` parses with PowerShell `ConvertFrom-Json`.
- Static grep found `MenuItem("Tools/FoundPhone/Writer Cockpit", false, 19)`.
- Static grep found existing `MenuItem("Tools/FoundPhone/Content Pipeline")`.
- Static grep found no `SaveGame`, `LoadGame`, `DeleteSave`, or `AutoSave` invocation in `WriterCockpitWindow.cs`.
- Save/autosave status code only checks `File.Exists` under `Application.persistentDataPath`.

## Status

- Writer Cockpit compile reachability: proven by Unity 6000.4.9f1 batch open after Package Manager cache/timestamp recovery.
- Writer Cockpit menu source reachability: statically proven by `MenuItem("Tools/FoundPhone/Writer Cockpit", false, 19)`.
- Existing Content Pipeline menu source reachability: statically proven by `MenuItem("Tools/FoundPhone/Content Pipeline")`.
- Interactive menu presence: not proven; no interactive Unity window was opened.
- Root blocker narrowed: fresh Package Manager resolution is still fragile and fails with `path undefined`; the local validation path currently depends on coherent generated `Library/PackageManager` cache metadata and matching source-file timestamps.

## Next Move

Use the current restored Package Manager cache state to open Unity interactively and verify `Tools > FoundPhone > Writer Cockpit` appears, then test Apply / Play from the Cockpit. Do not delete `Library/PackageManager` or regenerate `Packages/packages-lock.json` as a first step; that re-enters the `path undefined` failure.
