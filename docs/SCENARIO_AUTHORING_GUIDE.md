# Scenario Authoring Guide

執筆者向けの最短ガイド。詳細は関連ドキュメントへ分離する。

## 1. 最短手順

1. `Assets/Resources/Yarn/active/` に `.yarn` を作成
2. `Tools > FoundPhone > Content Pipeline` で `Sync Authoring Assets`
3. `ContentAuthoring` で StartNode を確認して再生
4. 問題は `docs/UI_ISSUES.md` か `docs/StorySpec/22_subquest_exploration_content.md` へ記録

## 2. 必須チェック

- `title:` / `===` の欠落がない
- 選択肢に必要なフラグガードがある
- `#line:` タグは重複しない
- `EndDay` を適切に配置

## 3. 参照先

- コマンド一覧・編集フロー: `docs/YarnEditingPipeline.md`
- 実装状態: `docs/FEATURE_STATUS_AUDIT.md`
- 仕様進捗: `docs/spec-index.json`

## 4. 執筆パターン（最小）

- 基本は **Hub & Spoke**（ハブでトピック分岐し、消化後に戻る）
- 分岐スレッドは再入防止フラグを必ず付ける（`$did_branch_xxx`）
- `EndBranch "select"` は知識が複数ある場面でのみ使う
- Day 構造は `EndDay` で区切り、次 Day 導線を明示する

## 5. よくある詰まり（最小）

- コンパイル失敗: `active/` 配下配置と `title:` / `===` 欠落を確認
- 選択肢ループ: フラグガード不足を確認
- 分岐再入: `did_branch` 系フラグ不足を確認
- テンポ不一致: `StartWait` と Inspector の遅延値で調整

## 6. 役割境界

- ストーリー内容・トーンはユーザー（ライター）主導
- AI はツール・検証導線・同期手順を整備
