# 次回推奨開発プラン作成の準備メモ

最終更新: 2026-04-08
対象ブランチ: `main`

## 1) 現状スナップショット（プラン起票前の事実）

- Ch1 は `Ch1_Day1.yarn` 内で Day1〜Day3 まで接続済み。
- `ch1` チャンネルは 3 日構成（`m_TotalDays: 3`）で登録済み。
- SP-022 は Ch1 実測用の集計表（§4.1）と ID 対応（03a）まで反映済み。
- SUBSEQUENT 用に手動ハンズオンと PlayMode 8 件の記録テンプレを配置済み。
- GitHub Actions の Unity テスト運用手順（シークレット要件含む）を整備済み。
- Ch2 は LATER 入口として整合メモを追加済み（Day 数と `EndDay` 解釈は要レビュー）。

## 2) 次回プラン作成のための不足情報（人間実測が必要）

次回プランの精度を上げるには、以下を先に埋める。

1. Unity Editor 通し結果（Ch1 Day1→2→3）
   - `Content Pipeline > Sync Authoring Assets` の成否
   - 進行不能の有無（再現手順つき）
2. UI 目視の新規気づき
   - `docs/UI_ISSUES.md` への追記候補
3. SP-022 §3/§4 の最終判断
   - Ch1 の本数圧縮を行うか
   - Day3 C スレッド 2 本の扱い（統合/遅延/維持）
4. PlayMode 8 件の実行結果
   - pass/fail とログ保存先

## 3) 次回プラン作成時の入力ソース（読む順）

1. `docs/HANDOFF.md`
2. `docs/project-context.md`
3. `docs/runtime-state.md`
4. `docs/StorySpec/22_subquest_exploration_content.md`
5. `docs/verification/templates/SUBSEQUENT_playthrough_and_tests.md`
6. `docs/StorySpec/LATER_CH2_PLAYBOOK.md`
7. `docs/StorySpec/ch2_later_consistency_note.md`

## 4) 次の数回で見込まれる変化（予測）

### セッション 1（実測回）

- Ch1 通し確認ログが追加される。
- P0/P1/P2 の優先度表が具体化される。
- 影響範囲: 主に `docs/verification/*`, `docs/UI_ISSUES.md`, SP-022 §6。

### セッション 2（仕様確定回）

- SP-022 §3/§4 の数値・優先度が「仮」から「運用値」に更新される。
- 03a の節↔ID 対応が最終化される。
- 影響範囲: `docs/StorySpec/22_subquest_exploration_content.md`, `docs/StorySpec/03a_ch1_section_beats.md`。

### セッション 3（LATER 着手回）

- Ch2 メイン＋サブの最小進行が追加される（P0 のみ例外実装）。
- BL-002 は視認性ボトルネックが確認された場合のみ判断対象に昇格。
- 影響範囲: `Assets/Resources/Yarn/active/Ch2_LocationConfusion.yarn` と関連 StorySpec。

## 5) プラン起票時の意思決定ポイント（HUMAN_AUTHORITY）

- B 型 Wiki を仕様先行のまま維持するか（実装開始しないか）。
- Ch1 の「任意サブ」の定義を Day3 の進行条件とどう整合させるか。
- Ch2 着手前に `EndDay` 粒度の解釈を固定するか（仕様メモで済ませるか）。
