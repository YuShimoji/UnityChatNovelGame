# Task Prompt: Ch1 サブクエスト（SP-022）検証・ギャップ整理

別スレッド／別エージェントにそのまま貼り付けて使う用の依頼文。

---

## コンテキスト

- リポジトリ: FoundPhone (UnityChatNovelGame)、Unity 6000.4.9f1、主レーンは `docs/project-context.md` の CURRENT。
- 「サブクエスト」は仕様 [docs/StorySpec/22_subquest_exploration_content.md](../StorySpec/22_subquest_exploration_content.md)（SP-022）に定義。エンジンは既存 Yarn コマンド（`DeclareThread*` / `CompleteThread` / `ManifestThread` 等）で表現済み。
- **リポジトリ上は Ch1 にパイロットが既に存在**する（未着手は主に **Ch2 以降のサブ**と **Editor 通し検証の未記録**）。機械集計は SP-022 §4.1 参照。

## 目的（この Task で達成すること）

1. **Unity Editor** で `HANDOFF.md` の手動ハンズオン（または `docs/verification/templates/SUBSEQUENT_playthrough_and_tests.md`）に沿い、Ch1 Day1〜3 とサブスレッド（`scout_*` / `annot_*` / `ch1_cond_analysis` 等）を通す。
2. 結果を次のいずれかに記録する:
   - 問題なし → `docs/verification/` に短い実施日付＋OKメモ、または既存 `2026-04-10-subsequent-completion-report.md` の「手動通し結果」表を更新。
   - 問題あり → `docs/UI_ISSUES.md`（見た目・操作）／`docs/StorySpec/22_subquest_exploration_content.md` §6 と `docs/verification/templates/2026-04-08-ch1-subquest-gap-template.md` の **P0/P1/P2** 表（進行・仕様ギャップ）。
3. SP-022 §3・§4 の **HUMAN_AUTHORITY**（優先種別・本数レンジ）を変えたい場合は、根拠を 1 段落で § に追記案を用意する（実装は不要）。

## 読む順（最小）

1. `docs/HANDOFF.md`
2. `docs/StorySpec/22_subquest_exploration_content.md` §4.1・§6
3. `docs/StorySpec/03a_ch1_section_beats.md`（節↔ID）
4. `docs/verification/2026-04-10-ch1-day1-3-preflight.md` 節 A（任意・静的）

## やらないこと

- B 型 Wiki のエンジン新規実装
- 大規模 UI リファクタ・新規アセット大量投入
- Yarn のフル執筆（検証に必要な最小修正のみ可）

## 完了の定義

- Ch1 通しの **再現手順**が 1 ドキュメント上で追える。
- ギャップがあれば **P0/P1/P2 付き**でどこか正典に残っている。
- `git status` がクリーン、またはユーザー合意の範囲のコミットのみ。
