# Ch2: チャンネル設定と Yarn の整合メモ（LATER / 監査）

**目的**: Phase D（LATER）前の**事実整理**。P0 でない限りコード変更はしない（横断保留と整合）。

## 現状（2026-04-10 リポジトリ）

| 項目 | [ch2.asset](../../Assets/Resources/Channels/ch2.asset) | [Ch2_LocationConfusion.yarn](../../Assets/Resources/Yarn/active/Ch2_LocationConfusion.yarn) |
|------|----------------|----------------------------|
| Day 数 | `m_TotalDays: **1**` | `<<EndDay **2**>>` が `Ch2_Closing` に存在 |

`m_DayStartNodeNames` は `Ch2_Opening` のみ。Yarn 内の `EndDay` 引数は「章内の日番号」表現と推測されるが、**ChannelData の Day 数と文言上の「2日目」が一致しているか**は、Save/EndDay 実装とあわせて LATER で一度確認するのがよい。

## 推奨次アクション（優先度は SUBSEQUENT 完了後）

1. `ScenarioManager` / `EndDay` のチャンネル完了条件をコードで確認し、**意図した挙動**を 1 段落で `docs/StorySpec/` か SP に残す（HUMAN_AUTHORITY）。
2. 不整合がバグなら **P0** として短いスライス。仕様どおりなら本メモに「解釈」を追記してクローズ。

## LATER での執筆

詳細手順は [LATER_CH2_PLAYBOOK.md](LATER_CH2_PLAYBOOK.md)。本メモは **執筆前の整合チェック**用。
