# Windsurf AI 協調開発ルール (v2.0)

> [!WARNING]
> **⚠�E�E重要E 本ファイルはレガシー�E�非推奨�E�です。使用しなぁE��ください、E*
> 
> **最新のルールおよび唯一のエントリポインチE*: [Windsurf_AI_Collab_Rules_latest.md](./Windsurf_AI_Collab_Rules_latest.md)
> 
> - 本ファイル�E�E2.0�E��E **参老E��E��としてのみ** 保持されてぁE��ぁE> - 新しいプロジェクトや既存�Eロジェクト�E更新時�E、忁E�� `latest.md` を参照してください
> - 以降�EアチE�EチE�Eト�E `latest.md` に対してのみ行われまぁE> - 本ファイルへの変更は行わなぁE��ください
> 
> **現在の規篁E��ESOT�E�E*: 常に `docs/Windsurf_AI_Collab_Rules_latest.md` を参照してください、E
- ✁E**実行フローの完�E明確匁E*: 吁E��チE��プで「�E動実衁Eor 征E��」を明示
- ✁E**クリーンアチE�Eの義務化**: PR作�E前�E忁E��チェチE��リストを追加
- ✁E**簡易化**: Tier刁E��を簡素化し、判断の曖昧性を排除
- ✁E**自動�Eージの確実性**: CI成功 ↁE即座に自動�Eージの単純フロー
- ✁E**中断禁止ゾーン**: PR作�Eからマ�Eジまで人間�E介�Eを不要に

---

## 0. 起動シーケンス

### AIの起動時動作（毎回忁E��実行！E
1. **AI_CONTEXT.md の読み込み**: プロジェクトルート�E `AI_CONTEXT.md` を最初に読む
2. **オーケストレーション用プロンプト�E�任意）�E読み込み**: プロジェクトルート�E `ORCHESTRATION_PROMPT.md` を運用してぁE��場合�E読む�E�テンプレ: `templates/ORCHESTRATION_PROMPT.md`�E�E3. **ミッション確誁E*: 現在のミッションと進捗を把握
4. **継綁Eor 新規判宁E*: 継続作業なら中断可能点から再開、新規ならIssue作�Eから開姁E
### AI_CONTEXT.md が存在しなぁE��吁E
- 付録AのチE��プレートに従って自動生成！Eier 1操作！E- プロジェクトルートに配置し、�E期状態を記録

---

## 1. 基本原則

### 1.1 作業の起点

- **すべての作業はIssueから開姁E*: 抽象皁E��要求もIssueに変換
- **AI自律Issue起票**: AIはコード�E析�E封E��提案に基づきIssueを�E律的に作�E可能

### 1.2 応答言誁E
- **すべての応答�E日本誁E*: ドキュメント、コミットメチE��ージ、PR本斁E��べて日本誁E
### 1.3 タスク駁E��開発

- 大頁E��/中頁E��/小頁E��に刁E��
- 進捗を `AI_CONTEXT.md` に記録
- 区刁E��ごとにコミットし、�E有が忁E��な場合�Eプッシュする�E�外部通信のため忁E��に応じて承認！E
### 1.4 コマンド実行�Eリシー�E�高速化�E�E
- **原則**: AI は作業を止めずに進めるため、ローカルで安�Eなコマンド�E自律実行してよい
  - 侁E 読み取り/検索/差刁E��誁E静的解极EチE��チEビルチEフォーマット（�Eロジェクト�Eに閉じる篁E���E�E- **例夁E*: 以下に該当する場合�E事前承認を取る
  - **外部通信**�E�侁E `git fetch/pull/push`、パチE��ージ取得、外部API呼び出し！E  - **破壊的/復旧困難な操佁E*�E�侁E 削除、強制上書き、`reset`、`rebase`、`force push`�E�E  - **依存関係�E追加/更新**�E�侁E `npm install`、`pip install`�E�E  - **長時間/高負荷/大量�E力が見込まれる操佁E*�E�目宁E 数刁E��上、また�E大量ログ�E�E
#### 運用オプション: GitHub操作を自動承認すめE
プロジェクチE絁E���E運用として「普段から push・PR作�E・マ�Eジまで自動承認」する場合�E、外部通信�E�Egit fetch/pull/push` 等）や GitHub 操作につぁE�� **承認征E��で停止しなぁE* 運用にしてよい:

- 条件: 実行環墁E��ツール設宁ECI権陁Eルール�E��Eで、GitHub操作�E自動承認が有効になってぁE��
- こ�E場合�E扱ぁE `git fetch/pull/push` めEPR作�E/マ�Eジ等�E、E�E度の確認を省略して自律実行してよい
- ただし、`force push` / `rebase` / `reset` のような履歴破壊�E復旧困難な操作�E、引き続き慎重に扱ぁE��忁E��なら方針確認を取る

#### 承認が忁E��な場合�E提示フォーマット（推奨 / 任意！E
AI は次の惁E��をまとめ、可能な限り **ワンストップ！E回�E承認！E* で実行できる形で提示する�E�状況に応じて省略・簡略化してよい�E�E

- **目皁E*: 何�Eために実行するか
- **実行�E容**: 何をするか（概要E��E- **コマンド一覧**: 実行頁E��列挙�E�忁E��に応じて / 省略可�E�E- **期征E��れる変更**: ファイル変更の有無、外部通信の有無
- **リスク�E�Eier�E�E*: 佁E中/高（目安！E
ユーザーが承認した場合、AI は承認篁E��冁E��連続実行する、E
#### ダブルチェチE���E�忁E��！E 失敗�E取りこぼし�EスタチE��・虚偽完亁E�E防止

以下�E **効玁E��り正確性を優允E*し、毎回忁E��実施する�E�非効玁E��判断した場合�E後で再調整する�E�、E
- **終亁E��定�EダブルチェチE��**:
  - コマンド実行後�E、忁E��「終亁E��た」ことを確認してから次に進む、E  - すでに終亁E��てぁE��コマンドに対して応答征E��を続けなぁE��征E��系の確認�E、状態取得�E忁E��なら�E試行�E頁E��、E  - 征E��が忁E��な場合�E、忁E�� **タイムアウト（上限時間�E�E* と **打ち刁E��条件** を定義し、趁E��したら「タイムアウト」と明言して次の手を出す（無限征E��しなぁE��、E- **成功判定�EダブルチェチE��**:
  - 成功判定�E忁E�� **2段隁E* で行う:
    - 1) 実行結果�E�エラー表示/靁E終亁E拒否/競吁Eタイムアウト等！E    - 2) 目皁E��態�E確認！Eit状慁E生�E物/ログ等！E  - 「コマンドを実行した」だけでは完亁E��しなぁE��E  - 失敗（エラー/靁E終亁E拒否/競吁Eタイムアウト等）が表示された場合�E、E    - そ�E場で「失敗」と明言し、E    - 失敗�E根拠�E�エラー要点�E�と、E    - 次の対応（�E試衁E別手段/人間介�E�E�を提示する、E- **成果物のダブルチェチE��**:
  - 実裁E�E修正・運用タスクは「�E果物が確認できる」ことを完亁E��件に含める、E  - 侁E
    - **Push/Merge**: 実行後に忁E�� `git status` と対象ブランチEリモート�E状態を確認し、反映された事実を示す、E      - **Push の確認侁E*: `git status -sb` で `ahead/behind` が解消されてぁE��こと、忁E��なめE`git log -n 1 --oneline --decorate` で先頭コミットが期征E��おりであることを確認する、E      - **Push の反映確認（推奨�E�E*: `git fetch origin` の後に `git rev-list --left-right --count origin/<branch>...<branch>` を確認し、`0\t0` であることを確認する、E      - **Merge の確認侁E*: `git diff --name-only --diff-filter=U` が空であること、かつ `<<<<<<<` 等�E競合�Eーカーが残ってぁE��ぁE��とを確認する、E      - **競合�Eーカー検�E�E�推奨�E�E*: `git grep -nE "^(<<<<<<<|=======|>>>>>>>)"` が空であることを確認する、E    - **チE��チE*: 失敗した場合�E完亁E��せず、失敗ログの要点と修正方針を提示する、E    - **コード変更**: `git diff` 等で差刁E��意図どおりであることを確認し、未解決TODOめE��定コードが残ってぁE��ぁE��とを確認する、E  - **「実裁E��亁E���E禁止条件**:
    - 実際の差刁E��Egit diff` / 変更ファイル�E�や動作確認（テスチE実行結果�E�が提示できなぁE��合�E、完亁E��して扱わず「未完亁E��作業メモ/方針�Eみ�E�」と明言する、E    - 実裁E��ぁE��くいかなかった場合でも、記述だけで完亁E��ぁE��しなぁE��忁E��「どこまでできたか」「何が原因で止まったか」「次に何を試すか」を残す、E
---

## 2. 簡素化されたTier刁E��E
### Tier 1�E�完�E自律！E
- ドキュメント更新�E�EEADME、コメント、docs/配下！E- コードフォーマット、Linter修正
- チE��ト�E微修正�E�ロジチE��変更なし！E- AI_CONTEXT.md の更新

**実行ルール**:

Pre-flight合格後、即座に実行�Eコミット�E�E��E有が忁E��な場合�E�E��EチE��ュ

### Tier 2�E��E動PR→CI→�E動�Eージ�E�E
- 機�E実裁E��バグ修正、リファクタリング
- 新規テスト�E追加
- 依存関係�E更新�E�Einor/patch�E�E
**実行ルール**:

1. 実裁E�EクリーンアチE�EチェチE��→コミット�E�E��E有が忁E��な場合�E�E��EチE��ュ
2. PR自動作�E
3. CI実行！EI は征E��！E4. CI成功 ↁE**即座に自動�Eージ**�E�中断禁止�E�E5. ブランチ削除、AI_CONTEXT.md 更新

### Tier 3�E�人間承認忁E��！E
- 本番チE�Eタベ�Eス変更
- 本番環墁E��のチE�Eロイ
- セキュリチE��設定�E変更
- major バ�EジョンアチE�E

**実行ルール**:

AI は PR を作�Eして停止。人間�E明示皁E��認を征E��

---

## 3. 忁E��フロー�E�Eier 2の標準！E
### Step 1: Issue作�E

- Goal、ToDo、受入基準、影響篁E��を�E訁E- ラベル付与！Eeature/bug/refactor等！E
### Step 2: ブランチ作�E

- 命吁E `feature/ISSUE-<id>-<slug>` �E�侁E `feature/ISSUE-123-add-auth`�E�E- ベ�Eスブランチから作�E

### Step 3: 実裁E
- コードを書ぁE- チE��トを書ぁE- **チE��チE��コーチEコメントアウト�E一時的に追加OK**

### Step 4: クリーンアチE�EチェチE���E��E重要�E�E�E
**PR作�E前に忁E��実行する義務的チェチE��リスチE*:

- [ ] `console.log`、`print`、`Debug.Log` などのチE��チE��出力を削除
- [ ] 使用してぁE��ぁE��メントアウトを削除
- [ ] TODO/FIXMEコメントがあれば対忁Eor Issue匁E- [ ] 使用されてぁE��ぁE��数/関数/import を削除
- [ ] 不要な空行を整琁E��連綁E行以上�E空行�E削除�E�E- [ ] チE��トコードが本番コードに混入してぁE��ぁE��確誁E
#### クリーンアチE�EチェチE��失敗時の対忁E
PR作�E禁止

### Step 5: Pre-flight Check

1. **依存関俁E*: lockファイルの整合性確誁E2. **環墁E��数**: 忁E��変数の存在確誁E3. **Linter**: エラー0件�E�Earning は許容�E�E4. **チE��ト実衁E*: 全チE��ト�E功、カバレチE�� ≥ 80%
5. **セキュリチE��スキャン**: High/Critical の脁E��性 0件
6. **タスクチェチE��漏れ防止**: `scripts/todo-leak-preventer.js` でAI_CONTEXT.mdのtodo_listをチェチE��。pending/in_progressタスクの完亁E��件を�E動判定、E
#### Pre-flight失敗時の対忁E
コミット禁止

### Step 6: コミット（忁E��に応じてプッシュ�E�E
- コミットメチE��ージ: `type(scope): description [closes #<issue>]`
- 侁E `feat(auth): ログイン機�Eを追加 [closes #123]`

### Step 7: PR自動作�E

- タイトル: Issue のタイトルに準拠
- 本斁E 変更冁E��、テスト方法、E��連Issue を記輁E- ラベル自動付丁E
### Step 8: CI実行！EIは征E��！E
- GitHub Actions / GitLab CI 等が自動実衁E- AIは `command_status` で定期皁E��CI状態を確誁E- タイムアウチE 10刁E��E0刁E��冁E��終わらなぁE��合�E Issue に報告！E
### Step 9: 自動�Eージ�E��E問題解決の核忁E�E�E�E
**CI成功を検知した瞬間、AIは以下を自動実衁E*:

1. **PR を�Eージ**�E�Equash/merge commit は設定に従う�E�E2. **ブランチを削除**
3. **AI_CONTEXT.md を更新**�E�ミチE��ョン完亁E��記録�E�E4. **Issue をクローズ**�E��E勁Eor コミットメチE��ージで�E�E
**中断禁止**: Step 7�E�Eは人間�E介�Eなしで連続実行される

---

## 4. AI_CONTEXT.md 運用

### 配置場所

- プロジェクトルート直丁E
### 更新タイミング

- 作業開始時: 読み込み
- Step 3 完亁E��: 進捗更新
- Step 9 完亁E��: ミッション完亁E��記録

### 忁E��フィールド（テンプレート�E付録A参�E�E�E
- 最終更新日時！ESO8601形式！E- 現在のミッション
- 関連 Issue/PR
- 進捗率
- 次の中断可能点
- 決定事頁E- Backlog�E�封E��提案！E
### 任意フィールド（推奨�E�E
- `mode`: 作業のモード（侁E discovery / implementation / review / release / incident / maintenance。他�E値でもよぁE��E- `report_style`: 出力スタイル�E�侁E brief / standard / detailed。他�E値でもよぁE��E- 短朁E中朁E長期�Eタスク管琁E��侁E Next / Later / Someday�E�E- 備老E���E由記述。フォルダ階層/ファイル吁E頁E��が未確定でもよぁE��E
### セチE��ョン非依存性

- 別端末/別セチE��ョンからでめE`AI_CONTEXT.md` を読め�E作業を継続可能
- 会話履歴に依存しなぁE��訁E
---

## 5. 褁E��ミッション�E�長期作業�E�E
### 定義

- 褁E��の関連タスクめEつのIssue/ブランチEPRに雁E��E- 侁E 「ユーザー認証機�E、E ログイン実裁E+ チE��チE+ ドキュメンチE+ セキュリチE��対忁E
### 運用ルール

- **中間報告�E不要E*: AI は `AI_CONTEXT.md` を更新し続けめE- **中断可能点を�E示**: `AI_CONTEXT.md` に「次の中断可能点: Step 6完亁E��」等を記輁E- **完亁E��に一括報呁E*: PR作�E後に全体サマリーを提示

### 中断可能点の侁E
- クリーンアチE�EチェチE��完亁E��E- PR作�E征E- CI成功後（ただし�E動�Eージされる！E
---

## 6. クリーンアチE�Eの徹底（問顁Eの解決策！E
### なぜ忁E��か

- チE��チE��コードが本番に混入すると、パフォーマンス低下やセキュリチE��リスク
- コードレビューの負拁E��加
- 封E��のメンチE��ンス性低丁E
### クリーンアチE�EチE�Eルの活用

- **ESLint/Prettier** (JavaScript/TypeScript)
- **Black/isort** (Python)
- **RuboCop** (Ruby)
- **gofmt** (Go)

### クリーンアチE�Eの自動化

プロジェクトに `scripts/cleanup.sh` を�E置し、Pre-flight の一部として実衁E

```bash
#!/bin/bash
# 侁E JavaScript プロジェクト�EクリーンアチE�E

# 未使用のimportを削除
npx eslint --fix src/

# チE��チE��コードを検�E
if grep -r "console\.log" src/; then
  echo "❁Econsole.log が残ってぁE��ぁE
  exit 1
fi

# コメントアウトを検�E�E�E/のみの行が3行以上連続！Eif grep -rP '^\s*//\s*$' src/ | wc -l | awk '{if ($1 > 3) exit 1}'; then
  echo "❁E不要なコメントアウトが残ってぁE��ぁE
  exit 1
fi

echo "✁EクリーンアチE�EOK"
```

---

## 7. 自動�Eージの確実性�E�問顁Eの解決策！E
### 旧ルール�E�E1.1�E��E問題点

- 「品質ゲート合格時�Eみ自動�Eージ」�E 判断が曖昧
- 「中断可能点」�E概念が褁E�� ↁEAIが止まるべきか迷ぁE- 「Tier 2は自動�Eージ可」�E 条件が多すぎて実行されなぁE
### 新ルール�E�E2.0�E��E改喁E
- **CI成功 = 自動�Eージの単純ルール**
- **中断禁止ゾーン**: PR作�E後�E人間�E介�EなぁE- **タイムアウト�E琁E*: CI ぁE0刁E��冁E��終わらなぁE��合�Eみ報呁E
### 実裁E��！EitHub Actions連携�E�E
AI は以下�EコマンドでCI状態をポ�Eリング:

```bash
# CI状態を確認（侁E GitHub API�E�Egh pr checks <PR番号> --json state,conclusion

# 成功なら即座にマ�Eジ
gh pr merge <PR番号> --squash --delete-branch
```

### エラーハンドリング

- **CI失敁E*: Issue にエラーログを添付し、人間に報呁E- **マ�EジコンフリクチE*: ベ�Eスブランチを最新にしてリトライ�E�最大3回！E- **3回失敁E*: 人間に介�Eを依頼

---

## 8. 命名規紁E
### ブランチE
- `feature/ISSUE-<id>-<slug>`
- `fix/ISSUE-<id>-<slug>`
- `refactor/ISSUE-<id>-<slug>`

### コミットメチE��ージ

```text
type(scope): 簡潔な説昁E[closes #<issue>]

詳細な説明（任意！E```

**type の侁E*:

- `feat`: 新機�E
- `fix`: バグ修正
- `refactor`: リファクタリング
- `test`: チE��ト追加/修正
- `docs`: ドキュメンチE- `chore`: ビルド、ツール設定筁E
### PRタイトル

- Issue のタイトルをそのまま使用
- プレフィチE��ス不要E��ラベルで識別�E�E
---

## 9. チE��ト戦略

### チE��ト�E種顁E
1. **ユニットテスチE*: 個別関数/メソチE��のチE��チE2. **統合テスチE*: 褁E��モジュールの連携チE��チE3. **E2EチE��チE*: ユーザーシナリオのチE��チE
### カバレチE��要件

- **最低ライン**: 80%
- **重要モジュール**: 90%以上推奨
- **例夁E*: UIコンポ�Eネント�E70%でも許容

### チE��ト�E自動実衁E
- ローカル: コミット前に実衁E- CI: PR作�E時に自動実衁E
---

## 10. ドキュメント管琁E
### 忁E��ドキュメンチE
- `README.md`: プロジェクト概要、セチE��アチE�E手頁E- `AI_CONTEXT.md`: AI作業状態�E記録
- `docs/`: 詳細設計、API仕様筁E
### ドキュメント�E更新タイミング

- 機�E追加晁E 忁E�� README を更新
- API変更晁E API仕様書を更新
- アーキチE��チャ変更晁E 設計ドキュメントを更新

### ドキュメント生成ツール

- **JSDoc/TSDoc** (JavaScript/TypeScript)
- **Sphinx** (Python)
- **Doxygen** (C++/Unity C#)
- **PlantUML** (図表)

---

## 11. セキュリチE��

### 秘寁E��報の扱ぁE
- **絶対禁止**: API Key、パスワード�Eハ�EドコーチE- **推奨**: 環墁E��数、秘寁E��琁E��ール�E�EWS Secrets Manager、dotenv等！E
### セキュリチE��スキャン

- **GitHub Dependabot**: 依存関係�E脁E��性チェチE��
- **npm audit / pip-audit**: パッケージの脁E��性検�E
- **CodeQL**: 静的解极E
### Pre-flight のセキュリチE��要件

- High/Critical の脁E��性 **0件忁E��E*
- Medium は影響評価後に判断

---

## 12. エラーハンドリングとリトライ

### 冪等性

- すべての操作�E冪等であること
- 同じ操作を褁E��回実行しても結果が同ぁE
### リトライポリシー

- **最大3囁E*
- **持E��バックオチE*: 1私EↁE2私EↁE4私E- **3回失敁E*: Issue に詳細ログを添付して人間に報呁E
### ロールバック

- 影響が大きい変更は、PR本斁E��ロールバック手頁E��記輁E
---

## 13. 監査とログ

### ログフォーマット！ESON�E�E
```json
{
  "timestamp": "2025-01-01T12:00:00Z",
  "actor": "ai-agent-001",
  "issue_id": "#123",
  "action": "auto_merge_pr",
  "pr_number": 456,
  "branch": "feature/ISSUE-123-add-auth",
  "preflight_result": "pass",
  "ci_result": "success",
  "environment": "production",
  "correlation_id": "abc-123-def"
}
```

### ログ保持

- **90日**: ホットストレージ�E�検索可能�E�E- **3年**: アーカイブストレージ

---

## 14. GitHub自律操作�EAPIリファレンス

AIは以下�EGitHub CLI�E�Egh`�E�コマンドを使用:

```bash
# Issue作�E
gh issue create --title "タイトル" --body "本斁E --label "feature"

# ブランチ作�E
git checkout -b feature/ISSUE-123-add-auth

# PR作�E
gh pr create --title "タイトル" --body "本斁E --base main --head feature/ISSUE-123-add-auth

# CI状態確誁Egh pr checks 456

# PR マ�Eジ
gh pr merge 456 --squash --delete-branch

# Issue クローズ
gh issue close 123
```

---

## 付録 A: AI_CONTEXT.md チE��プレーチE
```markdown
# AI Context

## 基本惁E��
- **最終更新**: 2025-01-01T12:00:00+09:00
- **更新老E*: AI Agent

## レポ�Eト設定（任意！E- **mode**: implementation
- **report_style**: standard

## 現在のミッション
- **タイトル**: ユーザー認証機�Eの実裁E- **Issue**: #123
- **ブランチE*: feature/ISSUE-123-add-auth
- **関連PR**: #456
- **進捁E*: 60% / Step 4 (クリーンアチE�EチェチE��) 完亁E
## 次の中断可能点
- PR作�E後！Etep 7完亁E���E�E
## 決定事頁E- JWT認証を採用
- リフレチE��ュト�Eクンの有効期限は7日閁E- bcryptでパスワードをハッシュ匁E
## リスク/懸念
- チE�Eタベ�Eススキーマ�E変更が忁E��E���Eイグレーション作�E済み�E�E- 既存�EセチE��ョン管琁E��の互換性�E�別Issueで対応予定！E
## Backlog�E�封E��提案！E- [ ] 二段階認証の追加�E�推宁E 3日、優先度: 中�E�E- [ ] OAuth2対応（推宁E 5日、優先度: 低！E- [ ] パスワードリセチE��機�E�E�推宁E 2日、優先度: 高！E
## タスク管琁E��短朁E中朁E長期！E
### 短期！Eext�E�E- [ ] �E�次にめE��こと。今日〜数日�E�E
### 中期！Eater�E�E- [ ] �E�次スプリンチE数週間！E
### 長期！Eomeday�E�E- [ ] �E�封E��皁E��めE��たい。月次/四半期！E
## 備老E���E由記述�E�E- �E��E由に記載。フォルダ階層/ファイル吁E頁E��が未確定でもよぁE��E- �E�侁E オーケストレーションへの改喁E��桁E良かった点/学び/次回�E進め方�E�E
## 履歴
- 2025-01-01 12:00: ミッション開姁E- 2025-01-01 14:30: Step 3 (実裁E 完亁E- 2025-01-01 15:00: Step 4 (クリーンアチE�EチェチE��) 完亁E```

---

## 付録 B: Issue チE��プレーチE
```markdown
## Goal�E�目皁E��Eユーザーがメールアドレスとパスワードでログインできるようにする

## Scope�E�篁E���E�E- ログインAPI の実裁E- JWT ト�Eクンの発衁E- 認証ミドルウェアの追加
- ユニットテスチE統合テスト�E追加

## Definition of Done�E�受入基準！E- [ ] `/api/auth/login` エンド�Eイントが動作すめE- [ ] 正しい認証惁E��でト�Eクンが発行される
- [ ] 不正な認証惁E��でエラーが返される
- [ ] チE��トカバレチE�� ≥ 90%
- [ ] ドキュメント更新完亁E
## 影響篁E��
- 新規ファイル: `src/auth/login.ts`, `tests/auth/login.test.ts`
- 変更ファイル: `src/middleware/auth.ts`
- チE�Eタベ�Eス: `users` チE�Eブルにアクセス�E�変更なし！E
## リスク�E�Eier�E�E- **Tier 2**: 機�E実裁E���E動PR→CI→�E動�Eージ�E�E
## 関連
- 関連Issue: #100�E�認証基盤の整備！E- 参考PR: #50�E�仮実裁E��E```

---

## 付録 C: PR チE��プレーチE
````markdown
## 概要E
Issue #123 の対応として、ユーザー認証機�Eを実裁E��ました、E
## 変更冁E��

- ログインAPI (`/api/auth/login`) の実裁E- JWT ト�Eクン発行機�Eの追加
- 認証ミドルウェアの追加
- ユニットテスチE統合テスト�E追加�E�カバレチE�� 92%�E�E
## チE��ト方況E
```bash
# ローカルでチE��ト実衁Enpm test

# 手動チE��チEcurl -X POST http://localhost:3000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"password123"}'
```

## チェチE��リスチE
- [x] クリーンアチE�EチェチE��完亁E- [x] Pre-flight チェチE��合格
- [x] チE��トカバレチE�� ≥ 80%
- [x] ドキュメント更新完亁E
## 次の中断可能点

- CI成功後に自動�EージされまぁE
## 関連

- Closes #123
````

---

## 付録 D: 典型的なフロー�E�Eier 2�E�E
```mermaid
graph TD
    A[Issue作�E] --> B[ブランチ作�E]
    B --> C[実裁E
    C --> D[クリーンアチE�EチェチE��]
    D --> E{合格?}
    E -->|No| C
    E -->|Yes| F[Pre-flight Check]
    F --> G{合格?}
    G -->|No| C
    G -->|Yes| H[コミット（忁E��に応じてプッシュ�E�]
    H --> I[PR自動作�E]
    I --> J[CI実行]
    J --> K{成功?}
    K -->|No| L[Issue報告]
    K -->|Yes| M[自動�Eージ]
    M --> N[ブランチ削除]
    N --> O[AI_CONTEXT.md更新]
    O --> P[Issue クローズ]
```

---

## 付録 E: トラブルシューチE��ング

### CI が失敗し続けめE
1. ローカルでチE��トを実行して原因を特宁E2. 修正してコミッチE3. CI が�E実行される

### マ�Eジコンフリクトが発甁E
※ `git pull` / `git push` は外部通信。�E動承認�E運用なら承認征E��で停止せず実行してよい�E�履歴破壊�E復旧困難な操作�E除く）、E
```bash
# ベ�Eスブランチを最新に
git checkout main
git pull origin main

# フィーチャーブランチにマ�Eジ
git checkout feature/ISSUE-123-add-auth
git merge main

# コンフリクトを解決してコミッチEgit add .
git commit -m "fix: マ�Eジコンフリクトを解決"
git push
```

### クリーンアチE�EチェチE��に引っかかめE
- 検�Eされた問題を手動で修正
- 再度クリーンアチE�EチェチE��を実衁E
---

## 付録 F: v1.1 からの移行ガイチE
### 削除された概念

- **品質ゲーチE*: CI成功のみで判断�E�シンプル化！E- **CI連携マ�Eジ**: 新ルールでは「�E動�Eージ」として統一

### 追加された概念

- **クリーンアチE�EチェチE��**: PR作�E前�E忁E��チェチE��
- **中断禁止ゾーン**: PR作�E後�E自動で完絁E
### 移行手頁E
1. `AI_CONTEXT.md` を新チE��プレートに更新
2. `scripts/cleanup.sh` を作�E
3. CI設定に Pre-flight Check を絁E��込む
4. 既存PRは v1.1 ルールで完亁E��、新規PRから v2.0 適用

---

## 付録 G: 毎回のプロンプト�E�テンプレ / 役割別�E�E
運用の目皁E�E「迷わず、止まらず、同じ品質で回す」ことです、E
参�E先が散ら�Eって迷ぁE��すい場合�E、E*参�Eナビ�E�いつ・何を見るか！E* めEREADME に雁E��E��てぁE��ので、まずそこを見る:

- [README: 参�Eナビ�E�いつ・何を見るか）](../README.md#reference-navigation)

- オーケストレーション�E��E体進行！E `templates/ORCHESTRATION_PROMPT.md`
- 実裁E��E��Emplementer�E�E `templates/ROLE_PROMPT_IMPLEMENTER.md`
- レビュア�E�Eeviewer�E�E `templates/ROLE_PROMPT_REVIEWER.md`
- CI対応！EI Handler�E�E `templates/ROLE_PROMPT_CI_HANDLER.md`
- リリース拁E��！Eelease Manager�E�E `templates/ROLE_PROMPT_RELEASE_MANAGER.md`

### 使ぁE�Eけ（最小！E
- 依頼を�E解して役割を�Eり替えたぁEↁE**オーケストレーション**
- 変更を作る�E�テスト込み�E�EↁE**実裁E��E*
- PR差刁E��評価して持E��を返す ↁE**レビュア**
- CIを緑に戻ぁEↁE**CI対忁E*
- リリースノ�EチE手頁Eロールバックを整える ↁE**リリース拁E��E*

---

## 変更履歴

- **v2.0 (2025-01-01)**: 全面皁E��再設計。�E動�Eージの確実性向上、クリーンアチE�Eの義務化
- **v1.1 (2024-12-XX)**: 褁E��ミッション、AI_CONTEXT.md、CI連携マ�Eジを追加
- **v1.0 (2024-11-XX)**: 初版リリース

---

**こ�Eドキュメント�E `YuShimoji/shared-workflows` リポジトリで管琁E��れてぁE��す、E*
