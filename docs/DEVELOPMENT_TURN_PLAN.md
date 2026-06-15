# Development Turn Plan

日付ではなく、作業ターン単位で現在地と次の区切りを見るための計画です。ここでの Turn は「調査 → 実装または検証 → ローカル確認 → docs 同期」までをひとまとまりにした作業ブロックを指します。

## ターン単位の見方

| Turn | 主な bottleneck | 作業内容 | 出口条件 | 参照する正本 |
|---|---|---|---|---|
| Turn 0 | docs 入口と runtime pin の混線 | AI 入口薄型化、Codex 固定削除、MkDocs 閲覧面追加 | `mkdocs build` pass、repo-local runtime pin 残存なし | `docs/HANDOFF.md`, `docs/index.md` |
| Turn 1 | SP-023 の見た目証跡不足 | `SP023_NarrationMargin_Start` / `SP023_LocalExtensions_Start` / `SP023_DisplayShowcase_Start` を確認し、必要ならスクショ追加 | 3 画面の OK/NG と画像配置が残る | `docs/plans/display-batch-showcase.md`, `docs/VISUAL_PROGRESS_INDEX.md` |
| Turn 2 | SP-024 の没入表示が未検収 | `SP024_Immersion_Start` で timestamp / read / deleted / narration 表示を確認 | SP-024 S1/S2/S5 の見た目判断が残る | `docs/StorySpec/24_chat_immersion.md` |
| Turn 3 | M1 サブスレッド全型の実機確認 | A/B/C 型、Latent、Branch を DebugQuickTest または最小 Yarn で確認 | 不具合があれば修正 + PlayMode テスト追加 | `docs/project-context.md`, `docs/FEATURE_STATUS_AUDIT.md` |
| Turn 4 | M2 Save/Load と章遷移の信頼性 | Save → Load → 状態復元、EndDay / 章遷移を確認 | 復元・遷移の PlayMode カバーが増える | `docs/SaveSystem_README.md`, `docs/project-context.md` |
| Turn 5 | SUBSEQUENT ゲート判定 | 未実装 / 未確認を P0/P1/P2 に振り分ける | P0 が 0 なら Ch1 フルコンテンツ解放、残れば P0 修正ターンへ | `docs/FEATURE_STATUS_AUDIT.md`, `docs/FEATURE_REGISTRY.md` |
| Turn 6 | コンテンツ前進とエンジン改善の接続 | Ch1 前進と P1 エンジン改善を並行 | 新しいエンジン能力の検証を伴うコンテンツだけ進める | `docs/StorySpec/22_subquest_exploration_content.md` |

## 進め方の制約

- Turn 1 と Turn 2 は `docs/HANDOFF.md` の表示検収再開に対応する。
- Turn 3 以降は `docs/project-context.md` のエンジン能力マイルストーンへ戻る。
- Turn 5 を通過するまで、フルコンテンツ執筆を主成果にしない。
- UI 個別修正は、Turn の主目的を壊す場合は `docs/UI_ISSUES.md` に積み、別の UI バッチターンへ回す。

## ターン終了時に更新する場所

| 更新対象 | 何を書くか | 更新タイミング |
|---|---|---|
| `docs/runtime-state.md` | 実施した Turn、検証結果、次の推奨 Turn | 各 Turn の終端 |
| `docs/HANDOFF.md` | 別端末がすぐ再開できる Safe Next Steps | 表示検収や大きな検証が進んだ時 |
| `docs/VISUAL_PROGRESS_INDEX.md` | 追加スクリーンショットと鮮度メモ | 画像証跡を追加した時 |
| `docs/FEATURE_STATUS_AUDIT.md` | 実装済み / 未確認 / 未実装の状態 | SUBSEQUENT ゲートや再監査時 |
| `docs/FEATURE_REGISTRY.md` | ENH 候補の状態変更 | candidate 以上の判断が出た時 |

## Turn から外すもの

- 日付別ログの増殖。履歴は `docs/runtime-state.md` に必要最小限で残す。
- スクリーンショットだけを増やす作業。画像は判断や検収の出口条件とセットで置く。
- 仕様承認なしの新機能実装。candidate は `FEATURE_REGISTRY.md` に置き、approved まで実装しない。
