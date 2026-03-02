# 証跡再利用ガイド

**作成日**: 2026-03-02  
**対象**: MVP検証・QAゲート作業における証跡の再利用判定  
**関連タスク**: TASK_055

---

## 目的

連続するゲート作業（TASK_053等）において、既存証跡を再利用できる条件を明確化し、証跡回収コストを削減する。

---

## 証跡再利用の基本原則

### 再利用可能な条件（全て満たす必要あり）

1. **ファイル存在**: 証跡ファイルが `docs/evidence/` 配下に存在する
2. **テスト名一致**: 実行したテスト名が完全一致する
3. **実行日時記録**: 証跡に実行日時が明記されている
4. **結果一致**: 期待する結果（Success/Pass等）が記録されている
5. **コード変更なし**: 証跡取得後、対象コードに変更がない
6. **依存関係変更なし**: 証跡取得後、依存パッケージ・アセットに変更がない

### 再利用不可の条件（1つでも該当すれば不可）

1. **コード変更あり**: 証跡取得後、テスト対象コードが変更された
2. **依存関係変更あり**: パッケージ・アセット・シーン構成が変更された
3. **証跡不完全**: ログ・スクリーンショット・結果が欠損している
4. **実行環境変更**: Unity版・OS・ビルド設定が変更された
5. **証跡期限切れ**: 証跡取得から7日以上経過（鮮度基準）

---

## 証跡タイプ別の再利用判定

### 1. PlayMode Tests

**再利用可能な条件**:
- テストコード（`Assets/Scripts/Tests/PlayMode/`）に変更なし
- テスト対象コード（UI/Core/Data）に変更なし
- シーン構成（ContentAuthoring.unity等）に変更なし
- 証跡に実行ログ・スクリーンショット・結果が含まれる

**判定方法**:
```bash
# 最終証跡取得後のコード変更確認
git log --since="<証跡取得日時>" --oneline -- Assets/Scripts/Tests/PlayMode/
git log --since="<証跡取得日時>" --oneline -- Assets/Scripts/UI/
git log --since="<証跡取得日時>" --oneline -- Assets/Scenes/
```

**証跡例**:
- `docs/evidence/TASK_027/FULL_PLAYTHROUGH_RESULTS_20260301_034145.md`
- `docs/evidence/TASK_027/Log_20260301_034145.txt`
- `docs/evidence/TASK_027/Capture_*.png`

### 2. EditMode Tests

**再利用可能な条件**:
- テストコード（`Assets/Scripts/Tests/EditMode/`）に変更なし
- テスト対象コード（Data/Core）に変更なし
- ScriptableObject定義に変更なし
- 証跡に実行ログ・結果が含まれる

**判定方法**:
```bash
git log --since="<証跡取得日時>" --oneline -- Assets/Scripts/Tests/EditMode/
git log --since="<証跡取得日時>" --oneline -- Assets/Scripts/Data/
```

**証跡例**:
- `docs/evidence/EDITMODE_TEST_RESULTS_<日時>.md`

### 3. Build Tests

**再利用可能な条件**:
- ビルド設定（EditorBuildSettings.asset）に変更なし
- ビルド対象シーンに変更なし
- アセンブリ定義（.asmdef）に変更なし
- 証跡にビルドログ・成功確認が含まれる

**判定方法**:
```bash
git log --since="<証跡取得日時>" --oneline -- ProjectSettings/EditorBuildSettings.asset
git log --since="<証跡取得日時>" --oneline -- Assets/Scenes/
git log --since="<証跡取得日時>" --oneline -- "*.asmdef"
```

**証跡例**:
- `docs/evidence/TASK_049/Build2.log`
- `docs/evidence/TASK_049/BuildSuccess.png`

### 4. Performance Tests

**再利用可能な条件**:
- パフォーマンス計測対象コードに変更なし
- 計測シーン・条件に変更なし
- 証跡にProfilerデータ・GC Alloc値が含まれる

**判定方法**:
```bash
git log --since="<証跡取得日時>" --oneline -- Assets/Scripts/UI/
git log --since="<証跡取得日時>" --oneline -- Assets/Scripts/Core/
```

**証跡例**:
- `docs/evidence/PERFORMANCE_MEASUREMENT_20260301_051439.md`
- `docs/reports/REPORT_TASK_022_PerformanceBaseline_RAW_*.md`

---

## 証跡再利用ワークフロー

### Step 1: 証跡存在確認

```bash
# 対象タスクの証跡ディレクトリ確認
ls docs/evidence/TASK_<番号>/

# 必須ファイルの存在確認
# - README.md（証跡概要）
# - *_RESULTS_*.md（結果レポート）
# - Log_*.txt（実行ログ）
# - Capture_*.png（スクリーンショット、該当する場合）
```

### Step 2: コード変更確認

```bash
# 証跡取得日時を確認
cat docs/evidence/TASK_<番号>/README.md | grep "取得日時"

# 証跡取得後のコード変更を確認
git log --since="<証跡取得日時>" --oneline -- <対象パス>
```

### Step 3: 再利用判定

| 条件 | 確認方法 | 判定 |
|------|---------|------|
| ファイル存在 | `ls docs/evidence/TASK_*/` | ✅/❌ |
| コード変更なし | `git log --since="<日時>"` | ✅/❌ |
| 証跡完全性 | ログ・結果・スクリーンショット確認 | ✅/❌ |
| 鮮度基準 | 証跡取得から7日以内 | ✅/❌ |

**全て✅の場合**: 証跡再利用可能  
**1つでも❌の場合**: 証跡再取得必要

### Step 4: 再利用時の記録

証跡を再利用する場合、以下を記録する:

```markdown
## 証跡再利用記録

- **再利用元**: docs/evidence/TASK_047/FULL_PLAYTHROUGH_RESULTS_20260301_034145.md
- **再利用先タスク**: TASK_053
- **再利用日時**: 2026-03-02 14:00
- **再利用判定理由**: コード変更なし、証跡完全、鮮度基準内
- **確認者**: Cascade Orchestrator
```

---

## 証跡マニフェスト（自動生成）

証跡の再利用判定を効率化するため、マニフェストを生成する。

### マニフェスト形式

```json
{
  "task_id": "TASK_047",
  "evidence_dir": "docs/evidence/TASK_047/",
  "created_at": "2026-03-01T03:41:45+09:00",
  "test_type": "PlayMode",
  "test_name": "FullPlaythroughTest",
  "result": "Success",
  "files": [
    "FULL_PLAYTHROUGH_RESULTS_20260301_034145.md",
    "Log_20260301_034145.txt",
    "Capture_01_start.png",
    "Capture_02_topic.png",
    "Capture_03_synthesis_or_end.png"
  ],
  "code_snapshot": {
    "commit_hash": "8ff2c11",
    "modified_files": [
      "Assets/Scripts/UI/ChatController.cs",
      "Assets/Scripts/Core/ScenarioManager.cs"
    ]
  },
  "reusable_until": "2026-03-08T03:41:45+09:00"
}
```

### マニフェスト生成スクリプト（例）

```javascript
// .shared-workflows/scripts/generate-evidence-manifest.js
const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

function generateManifest(taskId, evidenceDir) {
  const files = fs.readdirSync(evidenceDir);
  const readmePath = path.join(evidenceDir, 'README.md');
  const readme = fs.readFileSync(readmePath, 'utf8');
  
  // 証跡取得日時を抽出
  const dateMatch = readme.match(/取得日時[:\s]+(\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\+\d{2}:\d{2})/);
  const createdAt = dateMatch ? dateMatch[1] : new Date().toISOString();
  
  // 最新コミットハッシュ取得
  const commitHash = execSync('git rev-parse HEAD').toString().trim();
  
  // マニフェスト生成
  const manifest = {
    task_id: taskId,
    evidence_dir: evidenceDir,
    created_at: createdAt,
    test_type: detectTestType(files),
    result: detectResult(readme),
    files: files.filter(f => f !== 'manifest.json'),
    code_snapshot: {
      commit_hash: commitHash
    },
    reusable_until: calculateExpiryDate(createdAt, 7)
  };
  
  fs.writeFileSync(
    path.join(evidenceDir, 'manifest.json'),
    JSON.stringify(manifest, null, 2)
  );
}

function detectTestType(files) {
  if (files.some(f => f.includes('PLAYTHROUGH'))) return 'PlayMode';
  if (files.some(f => f.includes('EDITMODE'))) return 'EditMode';
  if (files.some(f => f.includes('Build'))) return 'Build';
  if (files.some(f => f.includes('PERFORMANCE'))) return 'Performance';
  return 'Unknown';
}

function detectResult(readme) {
  if (readme.includes('Success') || readme.includes('Pass')) return 'Success';
  if (readme.includes('Failed') || readme.includes('Fail')) return 'Failed';
  return 'Unknown';
}

function calculateExpiryDate(createdAt, days) {
  const date = new Date(createdAt);
  date.setDate(date.getDate() + days);
  return date.toISOString();
}

module.exports = { generateManifest };
```

---

## TASK_053への適用

### TASK_053で必要な証跡

1. **MVP PlayMode Test**: Title→Chat→Choice→End フロー
2. **Vertical Slice PlayMode Test**: 縦切りシナリオの通し確認
3. **Build Test**: Windows/WebGL ビルド成功確認

### 再利用可能な既存証跡

| 証跡 | 取得日時 | 再利用可否 | 理由 |
|------|---------|-----------|------|
| TASK_047/FULL_PLAYTHROUGH_RESULTS_20260301_034145.md | 2026-03-01 03:41 | ✅ 可能 | コード変更なし、鮮度基準内 |
| TASK_049/Build2.log | 2026-02-19 | ❌ 不可 | 証跡取得後にコード変更あり |
| PERFORMANCE_MEASUREMENT_20260301_051439.md | 2026-03-01 05:14 | ✅ 可能 | コード変更なし、鮮度基準内 |

### 追加取得が必要な証跡

- ❌ **MVP PlayMode Test**: 既存証跡なし（新規取得必要）
- ❌ **Build Test（最新）**: コード変更後の再ビルド必要

---

## 運用ルール

### 証跡保持期間

- **ホットストレージ**: 90日間（`docs/evidence/`）
- **アーカイブストレージ**: 3年間（`.archive/evidence/`）

### 証跡命名規則

```
<テストタイプ>_<テスト名>_<YYYYMMDD>_<HHMMSS>.md
例: FULL_PLAYTHROUGH_RESULTS_20260301_034145.md
```

### 証跡必須項目

1. **実行日時**: YYYY-MM-DDTHH:MM:SS+09:00形式
2. **実行環境**: Unity版、OS、ビルド設定
3. **テスト結果**: Success/Failed/Pass/Fail
4. **実行ログ**: コンソール出力、エラーメッセージ
5. **スクリーンショット**: 該当する場合（PlayMode Tests等）

---

## トラブルシューティング

### Q: 証跡は存在するが、コード変更があった場合は？

**A**: 証跡再取得が必要です。変更内容が軽微でも、テスト結果に影響する可能性があるため、再取得を推奨します。

### Q: 証跡取得から7日以上経過している場合は？

**A**: 鮮度基準外のため、証跡再取得が必要です。ただし、コード変更がなく、緊急性が高い場合は、例外的に再利用を検討できます（要記録）。

### Q: 証跡が不完全（ログ欠損等）な場合は？

**A**: 証跡再取得が必要です。不完全な証跡は再利用できません。

### Q: マニフェストがない証跡は？

**A**: 手動で再利用判定を行うか、マニフェストを生成してから判定します。

---

**ガイド終了**
