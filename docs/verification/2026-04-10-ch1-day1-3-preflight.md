# Ch1 Day1〜3 — プレフライト（リポジトリ整合）+ Editor 通し手順

**目的**: 推奨プラン Phase A（CURRENT）。CI/エージェントで Unity を回せない環境向けに、**静的整合**を記録し、**Editor 通し**はオペレーターが実施する。

## A. 自動確認済み（2026-04-10 時点のリポジトリ）

| チェック | 結果 |
|----------|------|
| [ch1.asset](../../Assets/Resources/Channels/ch1.asset) `m_TotalDays` | **3** |
| Day 開始ノード | `Ch1_Day1_Opening`, `Ch1_Day2_Opening`, `Ch1_Day3_Opening` が列挙と一致 |
| [Ch1_Day1.yarn](../../Assets/Resources/Yarn/active/Ch1_Day1.yarn) `title:` | Day1〜3 の Hub / スポーク / Winding / End が存在 |
| `<<EndDay 1>>` / `<<EndDay 2>>` / `<<EndDay 3>>` | 各 Day の終端に存在 |

**未実施（要 Unity）**: Content Pipeline **Sync Authoring Assets**、実機再生、Console エラーなし、UI 目視。

## B. オペレーター手順（Editor）

1. `Tools > FoundPhone > Content Pipeline` → **Sync Authoring Assets**（エラー解消まで）
2. `ContentAuthoring.unity` を開く。ScenarioManager の **Start Node** = `Ch1_Day1_Opening`
3. Play で Day1 → `EndDay 1` → ダッシュボードまたは再入で Day2 → Day3 まで通し
4. 気づき → `docs/UI_ISSUES.md`。進行不能・仕様不足 → `docs/StorySpec/22_subquest_exploration_content.md` §6

## C. 実施後の追記欄

- 実施日:
- Unity 版:
- 結果: OK / NG（要約）:

**2026-04-10（エージェント）**: 節 A の静的整合を再確認済み。Editor 通しは [2026-04-10-subsequent-completion-report.md](2026-04-10-subsequent-completion-report.md) に手順と「未実施」理由を記録。オペレーターが Play で通したら、本欄に **実測日・結果** を追記する。
