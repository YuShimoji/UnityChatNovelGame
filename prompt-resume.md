この repo を再開します。まず以下の順で読み、既知文脈の再質問をせずに現状復帰してください。

1. `docs/ai/CORE_RULESET.md`
2. `docs/ai/DECISION_GATES.md`
3. `docs/ai/STATUS_AND_HANDOFF.md`
4. `docs/ai/WORKFLOWS_AND_PHASES.md`
5. `docs/HANDOFF.md`
6. `docs/project-context.md`
7. `docs/runtime-state.md`
8. `docs/INVARIANTS.md`
9. `docs/USER_REQUEST_LEDGER.md`
10. `docs/OPERATOR_WORKFLOW.md`
11. `docs/INTERACTION_NOTES.md`

復帰後の前提:

- AI の役割は Yarn 執筆ではなく、制作ツール / パイプライン / 検証導線の整備
- 制作フローの入口は `docs/HANDOFF.md`
- 現在の主目的は、`Content Pipeline` と `YarnSOGenerator` を含む制作フローの Unity 実機検証
- UI 値調整は Inspector 作業。コード変更で吸わない
- `DebugQuickTest` を先に使い、本編 Yarn を実験台にしない

再開時の最初の行動:

- `git status --short --branch` で状態確認
- `docs/HANDOFF.md` の Safe Next Steps に沿って進める
