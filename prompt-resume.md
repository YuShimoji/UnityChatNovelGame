この repo を再開します。まず `AGENTS.md` → `docs/REPO_LOCAL_RULES.md` → `docs/HANDOFF.md` を読み、active artifact に直接必要な正本だけを追加してください。`docs/ai/READ_ORDER.md` は owner が不明なときの索引です。既知文脈を再質問せずに現状復帰してください。

復帰後の前提:

- **主目的:** `docs/HANDOFF.md` のライブ現在地と次入口に従う。古い Ch1 固定や前スライスの勢いだけで進めない
- AI の役割は Yarn 執筆ではなく、ツール・パイプライン・検証導線の整備
- 制作フローの入口は `docs/HANDOFF.md`
- Content Pipeline / YarnSOGenerator の実機確認は、上記主目的を進めるための**実走**として HANDOFF・project-context に従う（単独の最優先タスクではない）
- UI 値調整は Inspector 作業。コード変更で吸わない（方針の例外は HANDOFF / project-context）
- `DebugQuickTest` を先に使い、本編 Yarn を実験台にしない

再開時の最初の行動:

- `git status --short --branch` で状態確認
- `docs/HANDOFF.md` の active artifact、bottleneck、次入口に沿って進める
