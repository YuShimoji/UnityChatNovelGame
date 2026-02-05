# Worker Prompt: TASK_023_VerificationGap

## 参照
- チケット: `docs/tasks/TASK_023_VerificationGap.md`
- SSOT: `docs/Windsurf_AI_Collab_Rules_latest.md`
- HANDOVER: `docs/HANDOVER.md`

## 境界
- **Focus Area**: `Assets/Scripts/Tests/`, `docs/evidence/`
- **Forbidden Area**: `Assets/Scripts/Core/` (Logic変更禁止)

## DoD
- [ ] Evidence for Chat UI exists (`docs/evidence/TASK_023_ChatUI.png`).
- [ ] Evidence for Synthesis exists (`docs/evidence/TASK_023_Synthesis.png`).
- [ ] Report confirms visual verification of both systems.
- [ ] No compilation errors or runtime exceptions during capture.

## 停止条件
- `VerificationCapture` が動作しない場合（ログ提出して停止）。
- コンパイルエラーが発生している場合。

## 納品先
- `docs/reports/REPORT_TASK_023_VerificationGap.md`
