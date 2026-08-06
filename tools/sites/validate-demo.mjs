import { readFile } from "node:fs/promises";
import { dirname, resolve } from "node:path";
import { fileURLToPath } from "node:url";

const scriptDirectory = dirname(fileURLToPath(import.meta.url));
const repositoryRoot = resolve(scriptDirectory, "..", "..");
const demoRoot = resolve(repositoryRoot, "sites", "foundphone-demo");
const requiredFiles = [
  "index.html",
  "styles.css",
  "app.js",
  "content/demo.json",
  "README.md",
  "SITES_IMPORT_BRIEF.md"
];
const failures = [];

function assert(condition, message) {
  if (!condition) {
    failures.push(message);
  }
}

const contents = new Map();
for (const relativePath of requiredFiles) {
  try {
    contents.set(relativePath, await readFile(resolve(demoRoot, relativePath), "utf8"));
  } catch (error) {
    failures.push(`${relativePath}: ${error.message}`);
  }
}

let demo;
try {
  demo = JSON.parse(contents.get("content/demo.json") ?? "");
} catch (error) {
  failures.push(`content/demo.json parse failed: ${error.message}`);
}

if (demo) {
  assert(demo.meta?.canonStatus?.includes("non-canon"), "content must be explicitly non-canon");
  assert(demo.meta?.contentLabel === "Prototype content / not final story", "visible prototype label is missing");
  assert(Number.isInteger(demo.flow?.totalSteps) && demo.flow.totalSteps > 0, "flow.totalSteps must be a positive integer");

  const nodes = demo.flow?.nodes ?? {};
  assert(Boolean(nodes[demo.flow?.start]), "flow.start must reference an existing node");

  const choiceEntries = Object.entries(nodes).filter(([, node]) => node.type === "choice");
  assert(choiceEntries.length >= 1, "at least one choice node is required");

  for (const [nodeId, node] of choiceEntries) {
    assert(Array.isArray(node.options) && node.options.length >= 2, `${nodeId}: choice needs at least two options`);
    const targets = new Set(node.options?.map((option) => option.next));
    assert(targets.size >= 2, `${nodeId}: choice options must branch to different nodes`);
  }

  const reachable = new Set();
  const pending = [demo.flow.start];
  while (pending.length > 0) {
    const nodeId = pending.pop();
    if (!nodeId || reachable.has(nodeId)) {
      continue;
    }

    reachable.add(nodeId);
    const node = nodes[nodeId];
    assert(Boolean(node), `reachable node is missing: ${nodeId}`);
    if (!node) {
      continue;
    }

    if (node.next) {
      pending.push(node.next);
    }
    for (const option of node.options ?? []) {
      pending.push(option.next);
    }
  }

  assert(Object.keys(nodes).every((nodeId) => reachable.has(nodeId)), "all demo nodes must be reachable");
  assert([...reachable].some((nodeId) => nodes[nodeId]?.type === "ending"), "an ending node must be reachable");
}

const html = contents.get("index.html") ?? "";
const css = contents.get("styles.css") ?? "";
const app = contents.get("app.js") ?? "";
const runtimeSurface = [html, css, app, contents.get("content/demo.json") ?? ""].join("\n");

assert(html.includes('lang="ja"'), "document language must be declared");
assert(html.includes('aria-live="polite"'), "an aria-live status region is required");
assert(html.includes("Prototype content / not final story"), "prototype label must exist before JavaScript loads");
assert(css.includes(":focus-visible"), "readable focus-visible styling is required");
assert(css.includes("prefers-reduced-motion"), "reduced-motion path is required");

for (const eventName of ["demo_started", "choice_selected", "demo_completed", "outbound_store_intent"]) {
  assert(app.includes(`"${eventName}"`), `semantic event name is missing: ${eventName}`);
}

const prohibitedPatterns = [
  [/type\s*=\s*["'](?:email|password)["']/i, "personal-data input field"],
  [/card(?:_|-)?number|checkout\s*\(|payment\s*\(/i, "payment or card handler"],
  [/google-analytics|googletagmanager|\bgtag\s*\(|mixpanel|segment\.com|amplitude|posthog/i, "external analytics endpoint"],
  [/api[_-]?key\s*[:=]|client[_-]?secret\s*[:=]|bearer\s+[a-z0-9._-]{12,}/i, "secret-like value"],
  [/https?:\/\//i, "external URL in runtime artifact"]
];

for (const [pattern, label] of prohibitedPatterns) {
  assert(!pattern.test(runtimeSurface), `prohibited pattern found: ${label}`);
}

assert(!/<form\b/i.test(html), "forms are outside this slice");
assert(app.includes('fetch("./content/demo.json"'), "content must load from content/demo.json");
assert(!/localStorage|sessionStorage|indexedDB|document\.cookie/i.test(app), "persistent browser storage is prohibited");

if (failures.length > 0) {
  console.error("FoundPhone demo validation: FAIL");
  for (const failure of failures) {
    console.error(`- ${failure}`);
  }
  process.exit(1);
}

console.log("FoundPhone demo validation: PASS");
console.log(`Required files: ${requiredFiles.length}`);
console.log(`Demo nodes: ${Object.keys(demo.flow.nodes).length}`);
console.log("Choice branching, non-canon label, accessibility hooks, and prohibited-pattern audit passed.");
