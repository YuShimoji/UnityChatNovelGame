# LATER: Ch2 執筆プレイブック（SUBSEQUENT 完了後）

[docs/project-context.md](../project-context.md) の **LATER RECOMMENDED SLICE** を実行する際の短い手順書。本文は仕様の正典ではなく **オペレーション用**。

## 前提

- Ch1 SUBSEQUENT 済み: ギャップに **P0/P1/P2** が付いている。
- 方針: **Ch2 をメイン＋サブで前進**をデフォルト。**P0 だけ**短い仕様／実装スライスを挟む。**P1/P2 は繰り上げない**。
- 執筆前に [ch2_later_consistency_note.md](ch2_later_consistency_note.md) で **ChannelData の Day 数と Yarn `EndDay` の整合**を確認（現状は要レビュー）。

## 手順

1. Yarn: [Assets/Resources/Yarn/active/Ch2_LocationConfusion.yarn](../../Assets/Resources/Yarn/active/Ch2_LocationConfusion.yarn)（または Ch2 本体）を編集。
2. Content Pipeline → **Sync Authoring Assets**。
3. ContentAuthoring で再生確認。StartNode は Ch2 入口に合わせる。
4. サブクエストは Ch1 と同パターン（[22_subquest_exploration_content.md](22_subquest_exploration_content.md) を Ch2 用にコピーして優先・本数を更新）。
5. **BL-002（ポートレート）**: Ch2 の視認性がボトルネックと判断した時だけ着手（中期 S23）。

## P0 だけ実装する場合の門番

- 進行不能、セーブ／スレッドのデータ破綻、誤表示で継続困難 → P0 候補。
- それ以外 → UI_ISSUES または P1/P2 に留め、LATER では実装に入らない。
