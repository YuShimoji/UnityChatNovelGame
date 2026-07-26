import { createHash } from "node:crypto";
import { readFile } from "node:fs/promises";
import { dirname, resolve } from "node:path";
import { fileURLToPath } from "node:url";

const PACKAGE_SCHEMA = "foundphone.sites-preview-package";
const PACKAGE_VERSION = 1;
const scriptDirectory = dirname(fileURLToPath(import.meta.url));
const repositoryRoot = resolve(scriptDirectory, "..", "..");
const packagePath = resolve(
  repositoryRoot,
  "sites",
  "foundphone-demo",
  "content",
  "generated-preview.json"
);
const failures = [];

function assert(condition, message) {
  if (!condition) {
    failures.push(message);
  }
}

function sha256(value) {
  return createHash("sha256").update(value, "utf8").digest("hex");
}

function appendIdentityField(parts, value) {
  const normalized = String(value ?? "");
  parts.push(`${normalized.length}:${normalized}\n`);
}

function computePackageIdentity(previewPackage) {
  const parts = [];
  appendIdentityField(parts, previewPackage.schema);
  appendIdentityField(parts, previewPackage.version);
  appendIdentityField(parts, previewPackage.contentLabel);
  appendIdentityField(parts, previewPackage.canonStatus);
  appendIdentityField(parts, previewPackage.nodeName);
  appendIdentityField(parts, previewPackage.source?.assetPath);
  appendIdentityField(parts, previewPackage.source?.titleLine ?? 0);
  appendIdentityField(parts, previewPackage.source?.contentSha256);

  const supportedConstructs = previewPackage.supportedConstructs ?? [];
  appendIdentityField(parts, supportedConstructs.length);
  for (const construct of supportedConstructs) {
    appendIdentityField(parts, construct);
  }

  const displayLines = previewPackage.displayLines ?? [];
  appendIdentityField(parts, displayLines.length);
  for (const line of displayLines) {
    appendIdentityField(parts, line.ordinal);
    appendIdentityField(parts, line.sourceLine);
    appendIdentityField(parts, line.kind);
    appendIdentityField(parts, line.speakerId);
    appendIdentityField(parts, line.speakerLabel);
    appendIdentityField(parts, line.text);
  }

  const diagnostics = previewPackage.diagnostics ?? [];
  appendIdentityField(parts, diagnostics.length);
  for (const diagnostic of diagnostics) {
    appendIdentityField(parts, diagnostic.sourceLine);
    appendIdentityField(parts, diagnostic.severity);
    appendIdentityField(parts, diagnostic.code);
    appendIdentityField(parts, diagnostic.command);
    appendIdentityField(parts, diagnostic.message);
  }

  return sha256(parts.join(""));
}

function extractNodeSource(sourceText, nodeName, titleLine) {
  const lines = sourceText.replace(/\r\n?/g, "\n").split("\n");
  const titleIndex = titleLine - 1;
  assert(lines[titleIndex]?.trim() === `title: ${nodeName}`, "source title line does not match package provenance");
  const endIndex = lines.findIndex((line, index) => index > titleIndex && line.trim() === "===");
  assert(endIndex > titleIndex, "source node end marker was not found");
  return endIndex > titleIndex
    ? lines.slice(titleIndex, endIndex + 1).join("\n")
    : "";
}

let previewPackage;
try {
  previewPackage = JSON.parse(await readFile(packagePath, "utf8"));
} catch (error) {
  failures.push(`generated package could not be read: ${error.message}`);
}

if (previewPackage) {
  assert(previewPackage.schema === PACKAGE_SCHEMA, "package schema id is invalid");
  assert(previewPackage.version === PACKAGE_VERSION, "package version is invalid");
  assert(previewPackage.contentLabel === "Prototype content / not final story", "prototype label is missing");
  assert(previewPackage.canonStatus?.includes("non-canon"), "package must be explicitly non-canon");
  assert(/^((DQT)|(SP023)|(SP024))_/.test(previewPackage.nodeName), "package node is not a verification node");
  assert(/^Assets\/Resources\/Yarn\/active\/.+\.yarn$/.test(previewPackage.source?.assetPath ?? ""),
    "source asset path is outside active Yarn");
  assert(Number.isInteger(previewPackage.source?.titleLine) && previewPackage.source.titleLine > 0,
    "source title line must be 1-based");
  assert(/^[a-f0-9]{64}$/.test(previewPackage.source?.contentSha256 ?? ""),
    "source content hash is invalid");
  assert(/^[a-f0-9]{64}$/.test(previewPackage.packageIdentitySha256 ?? ""),
    "package identity is invalid");
  assert(Array.isArray(previewPackage.displayLines) && previewPackage.displayLines.length > 1,
    "package needs multiple display lines");
  assert(previewPackage.displayLines?.every((line, index) =>
    line.ordinal === index + 1 &&
    Number.isInteger(line.sourceLine) &&
    line.sourceLine > previewPackage.source.titleLine &&
    typeof line.text === "string" &&
    line.text.length > 0
  ), "display line order/source metadata is invalid");
  assert(Array.isArray(previewPackage.diagnostics), "diagnostics must be an array");
  assert(previewPackage.diagnostics?.every((diagnostic) =>
    ["warning", "error"].includes(diagnostic.severity) &&
    typeof diagnostic.command === "string" &&
    typeof diagnostic.message === "string" &&
    diagnostic.message.length > 0
  ), "unsupported construct diagnostic is invalid");
  assert(!previewPackage.diagnostics?.some((diagnostic) => diagnostic.severity === "error"),
    "generated package contains a blocking diagnostic");

  try {
    const sourcePath = resolve(repositoryRoot, previewPackage.source.assetPath);
    const sourceText = await readFile(sourcePath, "utf8");
    const nodeSource = extractNodeSource(
      sourceText,
      previewPackage.nodeName,
      previewPackage.source.titleLine
    );
    assert(sha256(nodeSource) === previewPackage.source.contentSha256,
      "source content hash does not match the selected Yarn node");
  } catch (error) {
    failures.push(`source provenance could not be verified: ${error.message}`);
  }

  assert(
    computePackageIdentity(previewPackage) === previewPackage.packageIdentitySha256,
    "package identity does not match deterministic Package v1 content"
  );
}

if (failures.length > 0) {
  console.error("Sites Preview Package v1 validation: FAIL");
  failures.forEach((failure) => console.error(`- ${failure}`));
  process.exit(1);
}

console.log("Sites Preview Package v1 validation: PASS");
console.log(`Node: ${previewPackage.nodeName}`);
console.log(`Source: ${previewPackage.source.assetPath}:${previewPackage.source.titleLine}`);
console.log(`Display lines: ${previewPackage.displayLines.length}`);
console.log(`Unsupported diagnostics: ${previewPackage.diagnostics.length}`);
console.log(`Identity: ${previewPackage.packageIdentitySha256}`);
