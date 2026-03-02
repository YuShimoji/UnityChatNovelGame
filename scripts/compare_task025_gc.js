const fs = require("fs");
const path = require("path");

function read(filePath) {
  return fs.readFileSync(filePath, "utf8");
}

function extractBaselineAverage(text) {
  const match = text.match(/\*\*GC Alloc\*\*: Avg (\d+(?:\.\d+)?) KB\/frame/i);
  if (!match) {
    throw new Error("Failed to extract baseline GC Alloc average.");
  }

  return Number(match[1]);
}

function extractAfterValues(text) {
  const values = [];
  const rowPattern = /^\|\s*\d+(?:\.\d+)?\s*\|\s*[\d.]+\s*\|\s*[\d.]+\s*\|\s*[\d.]+\s*\|\s*(\d+(?:\.\d+)?)\s*\|$/gm;
  let match;

  while ((match = rowPattern.exec(text)) !== null) {
    values.push(Number(match[1]));
  }

  if (values.length === 0) {
    throw new Error("Failed to extract after-measurement GC Alloc values.");
  }

  return values;
}

function average(values) {
  return values.reduce((sum, value) => sum + value, 0) / values.length;
}

function formatNumber(value) {
  return Number.isInteger(value) ? String(value) : value.toFixed(2);
}

function buildMarkdown(baselineAverage, afterValues) {
  const afterAverage = average(afterValues);
  const afterMin = Math.min(...afterValues);
  const afterMax = Math.max(...afterValues);
  const delta = afterAverage - baselineAverage;
  const verdict = delta < 0 ? "IMPROVED" : "NO_MEASURABLE_REDUCTION";

  return [
    "# TASK_025 GC Alloc Delta Summary",
    "",
    `- Baseline Avg GC Alloc: ${formatNumber(baselineAverage)} KB/frame`,
    `- After Avg GC Alloc: ${formatNumber(afterAverage)} KB/frame`,
    `- After Range: ${formatNumber(afterMin)}-${formatNumber(afterMax)} KB/frame`,
    `- Delta: ${delta >= 0 ? "+" : ""}${formatNumber(delta)} KB/frame`,
    `- Verdict: ${verdict}`,
    "",
    "## Interpretation",
    verdict === "IMPROVED"
      ? "- The after measurement shows a measurable reduction versus the baseline."
      : "- The after measurement remains within the same 22-23 KB/frame band as the baseline.",
  ].join("\n");
}

function main() {
  const projectRoot = process.cwd();
  const baselinePath = path.join(projectRoot, "docs", "reports", "REPORT_TASK_022_PerformanceBaseline.md");
  const task025Path = path.join(projectRoot, "docs", "reports", "REPORT_TASK_025_GCAllocReduction.md");
  const outFlagIndex = process.argv.indexOf("--out");

  const baselineText = read(baselinePath);
  const task025Text = read(task025Path);

  const baselineAverage = extractBaselineAverage(baselineText);
  const afterValues = extractAfterValues(task025Text);
  const markdown = buildMarkdown(baselineAverage, afterValues);

  if (outFlagIndex >= 0 && process.argv[outFlagIndex + 1]) {
    const outputPath = path.resolve(projectRoot, process.argv[outFlagIndex + 1]);
    fs.mkdirSync(path.dirname(outputPath), { recursive: true });
    fs.writeFileSync(outputPath, markdown + "\n", "utf8");
    console.log(outputPath);
    return;
  }

  console.log(markdown);
}

main();
