# Windsurf AI 協調開発ルール (v1.1)

> [!WARNING]
> **⚠�E�E重要E 本ファイルはレガシー�E�非推奨�E�です。使用しなぁE��ください、E*
> 
> **最新のルールおよび唯一のエントリポインチE*: [Windsurf_AI_Collab_Rules_latest.md](./Windsurf_AI_Collab_Rules_latest.md)
> 
> - 本ファイル�E�E1.1�E��E **参老E��E��としてのみ** 保持されてぁE��ぁE> - 新しいプロジェクトや既存�Eロジェクト�E更新時�E、忁E�� `latest.md` を参照してください
> - 以降�EアチE�EチE�Eト�E `latest.md` に対してのみ行われまぁE> - 本ファイルへの変更は行わなぁE��ください

## 0. 起動シーケンス�E��Eロジェクト抽象匁E

- 規篁E��ESOT�E��E常に本リポジトリの `docs/Windsurf_AI_Collab_Rules_latest.md` を参照してください�E�本ドキュメント�E旧版�E参老E��E���E�、E- AI は「各プロジェクト�E」リポジトリ直下にある `AI_CONTEXT.md` を起動時に読み込みます（中央リポジトリのファイルは参�Eしません�E�、E- `AI_CONTEXT.md` が存在しなぁE��合�E、付録 A のチE��プレートに従って最小構�Eを生成し、�Eロジェクト�Eへ追加することを推奨します、E- ルール本斁E�E常に本リポジトリの `docs/Windsurf_AI_Collab_Rules_latest.md` を参照し、各プロジェクト�Eのルールファイルは中央リポジトリへのリンクのみを保持します、E- Pre-flight�E��Eロジェクト�E�E�では `AI_CONTEXT.md` の存在と忁E��フィールド（最終更新/ミッション/ブランチE次の中断可能点等）を静的検証してください、E
## 1. 目皁E��適用篁E��

- 人間と AI エージェントが協調し、E��速かつ安�Eに価値を継続提供するため�E原則・手頁E�E監査要件を定義します、E- 適用対象はコード生成、テスト、デプロイ、ドキュメント生成、インフラ操作など全ての開発活動です、E
## 2. 用語定義

- AI: 本プロジェクトで自動化/支援を行うエージェント！Ector_id を付与）、E- Issue: すべての作業の起点となるトラチE��ング単位、E- Tier: 実行リスク刁E��。Tier 1�E�低！ETier 2�E�中�E�ETier 3�E�高）、E- Pre-flight Check: 実行前の自動検証群、E- 褁E��ミッション: 実裁E�E改喁E�Eリファクタ・リリース準備等を冁E��する、AI の長期作業単位。単一の Issue/ブランチEPR に雁E��E��る、E- AI_CONTEXT.md: リポジトリ直下で AI の状態を永続化/同期するための単一ソース。最終更新日時、現在ミッション、E��連 Issue/PR、E��捗、次の中断可能点、封E��提案等を記録、E- 中断可能点: 人間が介�E/停止しやすい工程上�E安�Eな区刁E���E�侁E PR 作�E直後、CI 成功直後、デプロイ前）、E
## 3. 原則�E�Erinciples�E�E
### 3.1 段階的権限付与！Eiered Delegation�E�E
- Tier 1�E�低！E ドキュメント更新、スタイル修正、軽微なチE��ト�ECI 設定、EI は自律実行可�E�要EPre-flight 合格�E�、E- Tier 2�E�中�E�E 機�E実裁E仕様追加等、EI ぁEPR を作�E。定義された品質ゲート（侁E チE��トカバレチE��≥90%、脆弱性 High/Critical=0�E�が満たされれば自動�Eージ可、E- Tier 3�E�高！E 本番チE�Eタ変更、DB マイグレーション、デプロイ/本番インフラ変更等、EI は提案�Eみ。シニア承認と二段階承認が忁E��、E
### 3.2 自律性の篁E��

- Assume Yes は Tier 1 のみ適用。Tier 2/3 は承認フローに従う、E- すべての AI 操作�E PR/変更ログを�E動生成し、理由・実行コマンドを記録する、E- 「CI 連携マ�Eジ、E CI が�E功しぁEPR は AI が�E動でマ�Eジする�E��E己 PR の承認�E省略�E�。Tier 2 は品質ゲート合格が条件。品質ゲートを満たしぁEPR は AI の判断で自律的にマ�Eジしてよい、E
### 3.3 可観測性と監査

- すべての AI 操作�E JSON 形式ログに記録�E�Eimestamp, actor, issue-id, action, diff/changes, preflight-result, environment, correlation-id�E�、E- ログ保持: 90 日ホット、E 年アーカイブ、E
### 3.4 運用標準（毎回の開発の進め方�E�E
- 返信は忁E��日本語で行う、E- タスク化して進め、区刁E��の良ぁE��ころでこまめにプッシュまで行う、E- 大頁E��・中頁E��・小頁E��を設定し、各タスクの目皁E��完亁E��件を�E確化、E- 進捗と今後�E計画を�E度明示し、それに従って作業を進める、E- 自律的にチE��ト（単佁E統吁E静的�E�を実行し、正常性を確認、E- 手動チE��トが忁E��な場合�E、完�Eな手頁E��ドキュメント化、E- Issue とドキュメントを頻繁に点検�E更新し最新に保つ、E- 長朁E中朁E短期�E「開発の区刁E��」を意識し、状態を点検�E更新、E
## 4. ワークフロー�E�Essue 駁E��開発�E�E
1) 着想: AI はコード�E极E抽象要汁E封E��提案に基づぁEIssue を起票可、Essue には Goal/ToDo/受�E基溁E影響篁E��/推定リスク�E�Eier�E�を明記、E2) 計画: タスク刁E��/サブタスク化。Patch スコープ定義、E3) 実裁E ブランチ命名�Eコミット規紁E��従い進める、E4) 検証: CI でユニッチE統吁E静的解极EセキュリチE��スキャン。Pre-flight 合格を�Eージ条件、E5) 統吁E ルール 3.1/3.2 に従ってマ�Eジ�E��E動�Eージ条件を満たす場合�E AI が実行）、E6) 斁E��匁E 変更めEdocs/ に反映。ドキュメントへ「最終更新日時」「更新老E��を明記、E
### 4.1 褁E��ミッション・ワークフロー

- 関連タスク�E�実裁E改喁Eリファクタ/リリース準備�E�を 1 つの「褁E��ミッション」として束�E、単一の Issue/ブランチEPR に雁E��E��E- Issue 起票は AI が�E律的に実施できる。忁E��な惁E���E�Eoal/Scope/DoD/リスク等）を満たす限り、事前承認なしで作�Eしてよい、E- 中間報告�E原則不要、EI は AI_CONTEXT.md を更新し続け、工程�E「中断可能点」を明示、E- 侁E Mission N を単一 PR で完�E ↁECI 成功 ↁEAI が�E動�Eージ、E
### 4.2 AI_CONTEXT.md 運用

- 位置: リポジトリ直下に `AI_CONTEXT.md`、E- 更新タイミング: 作業開始時の読込、作業の区刁E���E�コミット前/PR 作�E征ECI 完亁E��など�E�に更新、E- 記載�E容: 最終更新日時！ESO8601�E�、現在のミッション、E��連 Issue/PR、E��捗、次の中断可能点、決定事頁E��リスク、封E��提案！Eacklog�E�、E- 端末閁E会話セチE��ョン非依存で、どこからでも�E開可能にする、E
### 4.3 対話皁E��琁E��中断可能点と提案！E
- チャチE��報告時は「次の安�Eな中断可能点」を明示�E�侁E 「PR 作�E後」）、E- 封E��検討事頁E�E AI_CONTEXT.md の Backlog に蓁E��し、ミチE��ョン完亁E��告時に提案、E
### 4.4 GitHub 自律操佁E
- AI は Issue/ブランチEPR 作�E、ラベル付与、PR の自動�Eージ�E�ルール準拠�E�を実施、E- 自動�Eージは「CI 連携マ�Eジ」原剁E��従う。Tier 2 は品質ゲート合格時�Eみ自動。�Eージ後�E AI が�E律的に作業ブランチを削除する、E- 自己 PR は Approve 不可のため、承認を省略し、CI 成功後に直接マ�Eジする�E�EI 連携マ�Eジ�E�、E
### 4.5 命吁Eコミット規紁E
- ブランチE `feature/ISSUE-<id>-<slug>`�E�許容: `feature/#<id>-<slug>`�E�、E- コミッチE `type(scope): short description [closes #<issue>]`、E- type: feat/fix/chore/docs/refactor/test/build/ci/release など、E
### 4.6 Pre-flight Check�E�忁E��！E
1. 依存関係�E整合！Eock の整合）、E2. 忁E��環墁E��数の存在/妥当性、E3. Linter エラー 0、E4. チE��ト実行と閾値�E��E佁E≥80%、E��要モジュール ≥90% 推奨�E�、E5. セキュリチE��スキャン�E�Eigh/Critical=0、Medium は影響評価と承認）、E6. 影響篁E��レポ�Eト（本番影響: 変更ファイル一覧/DB 操佁E外部 API 影響�E�、E7. プロジェクト固有�E Smoke/静的検証�E�侁E `scripts/dev-check.js`�E�、E
### 4.7 セキュリチE��/秘寁E��報

- API Key/賁E��惁E��はハ�Eドコード禁止。忁E��な場合�E安�Eなストアを使用、E
### 4.8 バックアウチEロールバック

- 影響が大きい変更はリリースノ�EチEロールバック手頁E�� PR に明記、E
### 4.9 バ�Eジョニング/リリース

- セマンチE��チE��に準拠し、CHANGELOG/VERSION を更新。リリース PR は CI 成功後に自動�Eージ可�E�Eier に応じて�E�、E
### 4.10 ドキュメント生戁E
- Doxygen/PlantUML 等により設訁E変更点めEdocs/ に反映、E
## 5. 自動化・冪等性・リトライ

- 冪等運用。失敗時は最大 3 回まで自動リトライ、E 回失敗で中断し、Issue にエラーレポ�Eト付与、E
## 6. 監査/ログ

- 監査用 JSON ログ�E�E.3 参�E�E�を生�E。PR/Issue に紐づけ可能な correlation-id を使用、E
---

## 0. マニュアルの使ぁE��

- **対象**: コード生成、テスト、CI/CD、ドキュメント、インフラ運用など、E��発ライフサイクル全般で AI を活用するチ�Eム、E- **忁E��ファイル**:
  - `AI_CONTEXT.md`: 作業状態とモード、レポ�Eトスタイルを同期するため�E現場用ファイル、E  - `REPORT_CONFIG.yml`: レポ�EチE��ング/創造性の既定値、禁止表現、トリガー、�Eロンプト上書きを定義、E  - `docs/PROMPT_TEMPLATES.md`: スタイル別・リスク別に最適化されたプロンプトチE��プレート集、E  - `docs/CENTRAL_REPO_REF.md`: 中央リポジトリ�E�ESOT�E��E参�E/導�E手頁E��E  - `AI_FEEDBACK_LOG.md`: 応答評価を記録し、E��応学習に活用するログ、E- **運用サイクル**: 「起勁EↁEモード選抁EↁEワークフロー実施 ↁEレポ�Eト生戁EↁEフィードバチE��記録 ↁE改喁E��とぁE��ループで継続的に自治度を高める、E
---

## 1. 起動手頁E��運用モーチE
### 1.1 クイチE��スターチE
1. `AI_CONTEXT.md` を読み込み、`mode` と `report_style` を確誁E設定する、E2. `scripts/detect-project-type.js` を実行し、�Eロジェクトタイプ！Estandard`/`web`/`unity` など�E�を判定、E3. `scripts/report-style-hint.js <style>` を実行し、`REPORT_HINT.md` を生成、E4. 忁E��に応じて `scripts/creativity-booster.js` と `scripts/adapt-response.js` を実行し、提示されたヒントを対話冒頭で共有、E5. ワークフローに着手し、区刁E��ごとに `AI_CONTEXT.md` とログ類を更新、E
### 1.2 運用モーチE
- **Standard Mode**: Issue 駁E��で着実に進める。Tier 2/3 作業の既定モード、E- **Fast Mode**: Web/Unity 等でスピ�Eド重視。ダイレクトコミットとチE��ト閾値緩和を許容するが、`AI_CONTEXT.md` で合意が忁E��、E- **Custom Mode**: `AI_CONTEXT.md` に `mode: custom` を記載し、`REPORT_CONFIG.yml` の `prompt_overrides` と `creativity_triggers` を絁E��合わせてプロジェクト特化モードを構築、E
---

## 2. 権限モチE��とリスク管琁E
### 2.1 Tiered Delegation

- **Tier 1�E�低リスク�E�E*: ドキュメント更新、軽微なリファクタ、スタイル修正。Pre-flight 合格時�E AI が�E律実行し、�E動�Eージも許容、E- **Tier 2�E�中リスク�E�E*: 機�E実裁E��仕様追加、主要リファクタ、EI は PR を作�Eし、品質ゲート（テスチE≥80%、脆弱性 High/Critical=0�E�を満たした場合に自動�Eージ可、E- **Tier 3�E�高リスク�E�E*: 本番チE�Eタ操作、インフラ変更、リリース判断、EI は提案まで。人間による二段階承認とバックアウト�Eランが忁E��、E
### 2.2 自律性と安�E裁E��

- Assume Yes は Tier 1 に限定。Tier 2/3 では `AI_CONTEXT.md` に承認老E�E条件を記録する、E- すべてのオペレーションは PR/ログで追跡し、緊急時�E「例外条頁E��を記録して逸脱琁E��と是正策を残す、E- `creativity_triggers` を適用する場合でも、セキュリチE��めE��紁E��反が懸念される提案�E自動的に抑制する�E�スクリプト側でフィルタリング�E�、E
---

## 3. エンドツーエンチEワークフロー

1. **Ideation**: AI はコード�E极E抽象要汁E改喁E��案に基づぁEIssue を起票。`Goal`、`DoD`、`Tier`、影響篁E��を忁E��記載、E2. **Planning**: タスク刁E��、サブタスク化、Patch スコープ決定。`AI_CONTEXT.md` の Backlog を同期、E3. **Implementation**: ガイドラインに従いブランチ作�E、コミット、ドラフトコミット活用、E4. **Verification**: Pre-flight�E�依存関係、Linter、テスト、セキュリチE��、影響篁E���E�を実行。高速モード時はチE��ト閾値めESmoke を調整、E5. **Integration**: Tier/モードに応じて自動�Eージまた�E承認フローへ、EI 成功をトリガーに `scripts/adapt-response.js` を�E実行すると、次ラウンド�Eヒントを自動生成、E6. **Documentation**: `docs/` を更新し、`AI_CONTEXT.md` に中断可能点を記録。忁E��に応じて `AI_FEEDBACK_LOG.md` に評価を追記、E
### 3.1 Mission 管琁E
- 長期タスクは「ミチE��ョン」と定義し、E0 刁E��上�E工程�EサブミチE��ョンへ自動�E割、E- `AI_CONTEXT.md` の `Backlog` と `決定事頁E を更新し、別端末/別会話でもスムーズに再開できるようにする、E- **Worker同期強匁E*: WorkerAIの作業完亁E��Orchestratorが確認してから次のスチE��プへ進む。`AI_CONTEXT.md` にWorker完亁E��チE�Eタスを記録し、未完亁E��は中断し、リトライまた�E手動介�Eを俁E��、E*非同期モードオプション**: `async_mode: true/false` を追加。true時�Ecritical Workerのみ同期、E��criticalは並行実行。タイムアウト（デフォルチE0刁E��趁E��時�Eerror扱ぁE��E
### 3.2 対話皁E��琁E
- チャチE��報告�E「次の安�Eな中断ポイント」を明示�E�侁E 「CI 成功後に区刁E��」）、E- `REPORT_HINT.md` と `CREATIVITY_HINT.md` を参照し、スタイルとトリガーに従った進捗�E有を実践、E- 失敗リトライ 3 回で人間介�Eを俁E���E�EAI_CONTEXT.md` と Issue にログ�E�、E
---

## 4. レポ�EチE��ングとプロンプト統制

### 4.1 レポ�Eトスタイル

- `REPORT_CONFIG.yml` の `style_presets` に基づき、`standard`/`concise`/`narrative`/`creative` 等を選択、E- `report_style` 未持E��時は `default_style`�E�既宁E `standard`�E�を使用、E- 吁E��タイルは「目皁E�E現状・次のアクション」を忁E��情報とし、表現方法�Eみ変化させる、E
### 4.2 創造性トリガー

- `creativity_triggers` でスタイルごとの追加アクションを定義、E- トリガーは `scripts/creativity-booster.js` によりランダム/重み付きで選択され、`CREATIVITY_HINT.md` に出力される、E- ハイリスク作業では、安�E性チェチE��後にトリガーを適用するか、人間が承認するかを判断、E
### 4.3 プロンプトチE��プレーチE
- `docs/PROMPT_TEMPLATES.md` にスタイル別・Tier別チE��プレートを掲載、E- `REPORT_CONFIG.yml` の `prompt_overrides` により、�Eロジェクト固有�Eプロンプトを上書き可能、E- `scripts/report-style-hint.js` はチE��プレ惁E��を反映し、ヒント�Eに推奨見�Eしと禁止表現を提示、E
### 4.4 フィードバチE��ルーチE
- 応答後�E `AI_FEEDBACK_LOG.md` に創造性スコア、�E瞭性スコア、改喁E��、次回試すトリガーを記録、E- `scripts/adapt-response.js` がログを解析し、`ADAPTATION_HINT.md` に次回�E推奨スタイル/トリガーを提案、E
---

## 5. 自動化とスクリプト

- `scripts/detect-project-type.js`: プロジェクトタイプ判定、E- `scripts/report-style-hint.js`: `REPORT_HINT.md` 生�E�E�スタイル/禁止表現/推奨見�Eし）、E- `scripts/creativity-booster.js`: `creativity_triggers` を基に `CREATIVITY_HINT.md` を生成、E- `scripts/adapt-response.js`: `AI_FEEDBACK_LOG.md` を解析し、`ADAPTATION_HINT.md` を生成、E- 追加の Pre-flight チE�Eル�E�Eint/チE��チEセキュリチE���E��Eプロジェクト固有スクリプトに委譲し、本リポジトリからはガイドラインのみ提示、E
---

## 6. 監査・ログ・セキュリチE��

- すべての操作�E JSON 監査ログに記録�E�Eimestamp, actor, issue-id, command, preflight-result�E�、E- ログ保持: 詳細ログ 90 日、要紁E��グ 長期保管。`AI_FEEDBACK_LOG.md` は長期保存推奨、E- API Key/賁E��惁E��はハ�Eドコード禁止。Pre-flight に秘匿惁E��スキャンを含める、E- 緊急対応時は「例外条頁E��をログし、E��脱琁E��と是正策を明記、E
---

## 7. プロジェクトタイプ別ガイチE
- **Standard**: Issue 駁E��、Tier ガード強化、CI 完亁E���E自動�Eージ、E- **Web/Unity (Fast Mode)**:
  - ダイレクトコミット許可。テスト閾値 50% まで緩和可、E  - `creativity_triggers` を積極活用し、UI/UX 改喁E��や演�E提案を俁E��、E  - 高速モード時めE`AI_CONTEXT.md` の更新を省略しなぁE��E- **Custom**: `PROJECT_TYPE` ファイルを用意し、特化ルールとスクリプトを絁E��合わせる。侁E チE�Eタサイエンス案件向けに `analysis` モードを追加、E
---

## 8. 継続的改喁E��レーム

- **創造性メトリクス**: 新規提案数、多様性スコア、満足度�E�E-10�E�、�E利用アイチE��数、E- **レビュー頻度**: 月次で `AI_FEEDBACK_LOG.md` と Issue を振り返り、`REPORT_CONFIG.yml` と `PROMPT_TEMPLATES.md` を更新、E- **Issue 管琁E*: 改喁E��イチE��は GitHub Issue として起票し、Kanban で進捗を可視化。`creativity_backlog` ラベルを推奨、E- **モチE��差異対筁E*: 異なめEAI モチE��で同一チE��プレートをチE��トし、差刁E�� `AI_FEEDBACK_LOG.md` に記録。`scripts/adapt-response.js` が推奨スタイルを調整、E
---

## 付録 A: `AI_CONTEXT.md` チE��プレーチE
```markdown
# AI Context
- 最終更新: <ISO8601>
- 現在のミッション: <title> (#<issue>)
- ブランチE <branch>
- 関連: Issue <url>, PR <url>
- 進捁E <percentage>% / スチE�Eタス: <phase>
- 次の中断可能点: <when>
- モーチE <standard|fast|custom>
- レポ�Eトスタイル: <standard|concise|narrative|creative|custom>
- 使用トリガー: <trigger ids>
- **async_mode**: <true|false> (true: 非同期でcritical Workerのみ同期、false: 全同期)
- **Worker完亁E��チE�Eタス**: <worker1: completed|pending|error, priority: critical|non-critical, timeout: <minutes>>, <worker2: ...> (未完亁Eritical Workerがある場合、次スチE��プを中断)

## 決定事頁E- <decision/why>

## リスク/懸念
- <risk/mitigation>

## Backlog�E�封E��提案！E- <idea/impact/rough-scope>
```

---

## 付録 B: Issue/PR チE��プレート要紁E
- **Issue**: Goal / Scope / DoD / Tier / 影響篁E�� / 関連リンク / 想定スタイル、E- **Pull Request**: 概要E/ 変更点 / チE��チE/ リスク / 関連 Issue / 中断可能点 / 適用トリガー、E
---

## 付録 C: スクリプト クイチE��リファレンス

- `node scripts/detect-project-type.js`
- `node scripts/report-style-hint.js <style?>`
- `node scripts/creativity-booster.js <style?>`
- `node scripts/adapt-response.js`
- 実行結果の `.md` ファイルを対話冒頭で要紁E�E有することを推奨、E
---

## 付録 D: 創造性評価チェチE��リスチE
- **アイチE��の幁E*: 代替案が褁E��提示されてぁE��か、E- **表現の工夫**: 比喩・スト�EリーチE��ング・視覚的リストなど多様な表現が使われてぁE��か、E- **リスク認譁E*: 大胁E��提案にも安�E策やバックアウト案が添えられてぁE��か、E- **協調性**: 人間と AI の役割刁E��めE��の介�Eポイントが明確か、E- **評価と学翁E*: フィードバチE��ぁE`AI_FEEDBACK_LOG.md` に記録され、次回への示唁E��抽出されてぁE��か、E
---

## 付録 E: 参老E��ンク

- `docs/PROMPT_TEMPLATES.md`
- `REPORT_CONFIG.yml`
- `AI_FEEDBACK_LOG.md`
- プロジェクト固有�Eガイドライン�E�侁E `docs/project-type/web.md` 等！E
