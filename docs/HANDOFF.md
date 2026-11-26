# Handoff

最終更新: 2026-07-21

このブランチは旧版の文書整理を保存したもの。開発は `origin/main` の `845e0ac` 以降から再開し、そちらの `docs/HANDOFF.md` を読む。以下は文書整理時の記録。

## 現在地

- プロジェクト: FoundPhone / UnityChatNovelGame
- ブランチ: `main`
- 同期基準: `73aef720`。2026-07-21 に `git fetch --prune origin` と `git pull --ff-only origin main` を実行し、`51dc8bc` から 8 コミットを fast-forward で取得
- 文書整理: 同期前から存在した正本の整理を記録。コード・Yarn・シーン・素材・パッケージの変更は含まない
- 現在の開発スライス: 受入済み Writer Cockpit navigation と Owner-only Sites Version 1 を維持し、作者導線の人間環境レビューと hosted 本文レビューを閉じる

## 取り込んだ能力

| 領域 | 最新状態 | workflow / decision への効果 |
|---|---|---|
| Writer Cockpit | 74 Node 検索、source path / line、diagnostic drilldown、External Script Editor への fail-closed jump を追加 | Yarn のエラー箇所から修正対象へ戻る導線が一画面でつながる |
| Validator | runtime command handler と CharacterProfile を registry 化し、active Yarn は `errors=0 / warnings=0 / info=3` | 既知 command / character の偽陽性を除き、作者が実診断へ集中できる |
| Targeted tests | `ProjectFoundPhone.Editor.Tests` の Writer Cockpit navigation 14/14 pass がリモート正本に記録済み | source 索引、診断、missing file の退行を狭く再検証できる |
| Unity 起動 | `tools/run-unity.ps1` が必要 Unity 版を選び、欠落した `ALLUSERSPROFILE` を子プロセス内だけ補完 | package JSON や cache を手作業で壊さず batch / interactive を再開できる |
| Sites demo | tracked static fixture と validator を追加し、Owner-only Version 1 の deployment success を記録 | direct Unity Web の blocker と分離して軽量 UX を private review できる |

## 開発可能性

| 経路 | 現在の判断 | 残る確認 |
|---|---|---|
| Git | 文書整理時の `main` は `73aef720` | 現在の再開版は `origin/main` の `845e0ac` 以降 |
| Unity / C# | 最新 checkout で 39 packages、script compile、batch exit 0 を再確認 | interactive Editor 操作は人間環境確認が必要 |
| Yarn authoring | validator `errors=0 / warnings=0 / info=3`、11 files / 74 nodes を再確認 | Apply / Play / Last Action の本統合後 interactive 再受入 |
| Writer Cockpit tests | 専用 EditMode 14/14 pass、failed / skipped 0 | full suite は save data 安全化後 |
| Sites static package | validator pass。6 required files、7 nodes、分岐・非canon表示・a11y hook・禁止 pattern を確認 | hosted 本文は Owner sign-in 後 review 待ち |
| Docs | nav 生成と MkDocs strict build pass | Pages 自動公開は未導入 |
| Full regression | 未更新 | save data を隔離してから EditMode 73 / PlayMode 10 を実行 |

## 残る不確実性

- External Script Editor が設定された人間環境で、Node と diagnostic の実 file / line jump はまだ受け入れていない。
- Writer Cockpit の Apply / Play / Last Action は既存処理を共有し compile 済みだが、最新統合後の interactive loop は未確認。
- 現行 83 tests は、実ユーザーの `Application.persistentDataPath` を削除しうるテストから save data を保護するまで実行しない。
- Sites は Owner-only のまま。hosted 本文、network、両分岐、keyboard、320–430px は Owner sign-in 後の最終 review が必要。public/shared access と custom domain は未承認。
- direct Unity Web は Web Build Support、有効な public gameplay scene、TitleScene からの公開導線が揃うまで blocked。

## 次に入れる作業

| 入口 | 解く摩擦 | 完了すると可能になること | 前提 |
|---|---|---|---|
| Advance: Writer Cockpit full-loop 受入 | navigation と既存 Apply / Play が別々にしか証明されていない | Yarn 保存から選択ノード再生までを日常導線として閉じ、SP-023 / SP-024 表示検収へ戻れる | Unity External Script Editor を人間が選択 |
| Verify: full 83 tests の安全化 | 現行全体回帰が旧 8 PlayMode 基準 | save data を傷つけず EditMode 73 / PlayMode 10 を継続基準にできる | test data 隔離または実データ退避 |
| Audit: Sites hosted 本文 | private deployment と実表示の間に人間認証ゲートが残る | Version 1 の両分岐・network・keyboard・responsive を正式受入できる | Owner sign-in。access policy は変更しない |
| Explore: SP-023 / SP-024 表示比較 | 実装済み表示能力の画面証跡が不足 | 同条件の portrait / wide 比較から UI バッチ基準を凍結できる | Writer Cockpit full-loop 受入後 |

## 触らない範囲

- `Packages/manifest.json` / `packages-lock.json` の削除・手動再生成を復旧の第一手にしない
- ユーザー承認前の Unity module / Localization 依存追加、public/shared Sites access、custom domain
- Yarn 本編の代筆、未承認の B 型 Wiki / C 型リッチカード実装
- UI 問題の 1 件ずつの即時修正。`docs/UI_ISSUES.md` に集約してバッチ化する
