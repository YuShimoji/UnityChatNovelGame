# 並行開発レーン用プロンプト集（プラン作成直前までの準備）

本ファイルは **コア＋複数レーン**で並行してエージェント／人間に渡すための **コピペ用プロンプト**を集約する。実際の PLAN MODE 文書化の直前に、担当レーンのブロックだけを切り出して使う。

---

## 0. プラン作成直前チェックリスト（全レーン共通・先に満たす）

以下が埋まるまで、大きな PLAN 本文は書かない（`docs/ai/DECISION_GATES.md` の Value Validation / Actor Gate と整合）。

1. **コア固定（1 文）**
  例: 「Ch1 を Day 単位でプレイアブルに前進し、Content Pipeline で同期した状態が再生できること」  
   ※ `docs/project-context.md` の `CURRENT DEVELOPMENT AXIS` と矛盾させない。
2. **主レーンは 1 つ**
  並行は **副レーン** に限定。主レーンが複数になる場合は、セッションを分割するかユーザー承認を取る。
3. **価値検証（各 1 文）**
  - 出力はどの工程（検証・執筆・ビルド・ドキュメント）に入るか  
  - 人の手作業・判断のどれが減るか  
  - まだ Unity／手動が本線なら、その前提を明示する
4. **アクターとオーナー**
  各タスクに `user` / `assistant` / `shared` と、所有アーティファクト（Yarn / 仕様 / C# / docs）を付ける。
5. **横断保留の確認**
  `project-context.md` の「横断保留」（UI 個別修正のバッチ化、B 型 Wiki 未承認実装など）に触れる場合は、**逸脱であること**をプロンプトに明記する。
6. **衝突回避**
  - 同一ファイルを複数レーンで編集しない（特に `ChatController.cs`、大きなシーン）。  
  - UI の見た目変更は **UI レーンに集約**（他レーンは `UI_ISSUES.md` へ記録のみ可）。

---

## 1. コア（全レーンに貼る短い前置き）

以下を各プロンプトの先頭に **そのまま付けてもよい**。

```text
【コア】
リポジトリ: FoundPhone (UnityChatNovelGame)。Unity 6000.3.6f1 前後（ProjectVersion.txt 準拠）。
正典の優先順位: docs/project-context.md の CURRENT DEVELOPMENT AXIS / CURRENT LANE / CURRENT SLICE。
ルート CLAUDE.md の DEVELOPMENT PURPOSE に従い、検証に必要な範囲外のフル執筆はしない。
docs/archive/ は明示依頼がない限り読まない。
```

---

## 2. レーン A — Content（メイン Yarn・チャプター前進）

**貼り付け用プロンプト:**

```text
【レーン】Content（メイン Yarn・Day／節の前進）

【目的】
project-context の CURRENT SLICE に沿い、指定チャプター（通常 Ch1）のメインスレッドを Day 単位でプレイアブルに前進させる。

【先に読む】
docs/HANDOFF.md → docs/project-context.md → docs/OPERATOR_WORKFLOW.md
執筆時: docs/SCENARIO_AUTHORING_GUIDE.md、該当章の StorySpec（例: 03a_ch1_section_beats.md）

【やること】
- メイン Yarn のノード追加・分岐整理（検証目的が一文で言える範囲）
- チャンネル／Day 開始ノードと ch*.asset の整合を壊さない
- 変更後は Content Pipeline の Sync Authoring Assets をオペレーターが実行できる状態にする（手順を短く記録）

【やらないこと】
- SP-022 の HUMAN_AUTHORITY（§3・§4）を勝手に確定しない（仮置き・提案は可、確定はユーザー）
- UI のコード修正（気づきは docs/UI_ISSUES.md）
- B 型 Wiki のエンジン新規実装

【アクター】
主: user（ストーリー・必須度）。assistant は構成・コマンド整合・ツール支援。

【完了条件】
- 「この Yarn で機能 X を検証できる」が一文で言える
- 関連 SO / Channel がパイプライン手順と矛盾しない（静的に確認できる範囲まで）
```

---

## 3. レーン B — Subquest（SP-022・サブスレッド）

**貼り付け用プロンプト:**

```text
【レーン】Subquest（SP-022・サブクエスト設計とパイロット検証）

【目的】
docs/StorySpec/22_subquest_exploration_content.md に沿い、Ch1（または指定章）のサブスレッドを既存 Yarn コマンドのみで検証可能な状態にする。ギャップは §6 と P0/P1/P2 表に残す。

【先に読む】
docs/StorySpec/22_subquest_exploration_content.md（§3・§4・§6）
docs/StorySpec/03a_ch1_section_beats.md（節↔ID）
docs/HANDOFF.md（手動ハンズオン）
詳細検証手順は docs/ai/TASK_PROMPT_ch1_sidequest_verification.md も可

【やること】
- Editor 通しまたは静的整合で、パイロット ID の到達・CompleteThread 後の破綻がないか確認
- ギャップを 2026-04-08-ch1-subquest-gap-template.md または §6 に P0/P1/P2 で追記
- 仕様の空白は「要 HUMAN_AUTHORITY」と明記

【やらないこと】
- B 型アプリ内 Wiki の実装
- C 型成果物カードのリッチ UI 実装（仕様承認まで）

【アクター】
判断・必須/任意: user。手順書・表の整理・PlayMode 補助: assistant。

【完了条件】
再現手順が 1 ドキュメントで追える。未解決は P 付きで正典に残っている。
```

---

## 4. レーン C — Unlock（制作パイプライン・SO 同期）

**貼り付け用プロンプト:**

```text
【レーン】Unlock（Content Pipeline・YarnSOGenerator・Validator）

【目的】
オペレーターが「Yarn 編集 → 検証 → SO 同期 → 再生」を迷わず通せる。Editor ウィンドウ・スクリプトの欠落・エラーを解消する。

【先に読む】
docs/OPERATOR_WORKFLOW.md、docs/YarnEditingPipeline.md
docs/project-context.md（パイプライン表）

【やること】
- Content Pipeline / YarnSOGenerator / Validator の不具合修正（範囲は一文で定義）
- 推奨 StartNode や Channel 同期のドキュメント追随
- 再現手順を短く docs またはコメントに残す

【やらないこと】
- メインストーリーの大量執筆
- 本番シーンの見た目のみの調整（UI_ISSUES へ）

【アクター】
実装: assistant 主。実機での最終クリック確認: user。

【完了条件】
手順どおりに Sync が通り、既知のブロッカーが issue 化または修正済み。
```

---

## 5. レーン D — Engine / Quality（ランタイム・テスト・セーブ）

**貼り付け用プロンプト:**

```text
【レーン】Engine / Quality（C#・PlayMode・セーブ／スレッド安定性）

【目的】
進行不能・セーブ破綻・テスト赤を解消する。新機能は「コアに直結する最小限」に留める。

【先に読む】
docs/INVARIANTS.md、docs/FEATURE_STATUS_AUDIT.md（必要箇所）
該当する spec-index / ENGINE_FEATURE_INVENTORY の項目

【やること】
- バグ修正・回帰テスト（EditMode / PlayMode）
- batch XML 等の検証導線の保守
- 変更理由と影響範囲をコミットメッセージに書く

【やらないこと】
- テストを通すための仕様後退（session 21 方針: テスト側を現仕様に合わせる）
- カバレッジ目的の網羅テスト追加（テストは実バグの再現・回帰用に書く。内部呼び出しでありえない null/empty のガードテストは不要）
- IP-PC-002 級のスレッド管理リファクタ本実装（PLAN MODE 設計まで保留）

【アクター】
assistant 主。仕様が割れる場合は user に一文で確認。

【完了条件】
対象テストが緑、または意図したスキップ理由が文書化されている。
```

---

## 6. レーン E — UI バッチ（見た目・レイアウト一括）

**貼り付け用プロンプト:**

```text
【レーン】UI バッチ（レイアウト・配色・リスト項目の一括修正）

【目的】
docs/UI_ISSUES.md に蓄積した項目を、1 ブロックでまとめて修正する。モダン／ミニマル化もこのレーンに集約（重い表現は別 Task）。

【先に読む】
docs/UI_ISSUES.md、docs/DISPLAY_ALGORITHMS.md（該当箇所）

【やること】
- Open Issues の [FIXED] 化または優先度付け
- ダッシュボード・チャット・サイドバー等の一貫した余白・色（既存パターンに合わせる）
- パフォーマンスを落とすマテリアル・ポストエフェクトは採用しない（要別 Task と明記）

【やらないこと】
- コンテンツ執筆
- 仕様未承認の B 型・C 型リッチ表現の本実装

【アクター】
見た目の最終判断: user。実装: assistant。

【完了条件】
UI_ISSUES の対象項目が解消または次バッチに明確に繰越。必要なら Unity 目視のチェックリスト 1 行。
```

---

## 7. レーン F — Audit / Evidence（ドキュメント・CI・検証ログ）

**貼り付け用プロンプト:**

```text
【レーン】Audit / Evidence（検証記録・spec 整合・CI ログ）

【目的】
再現性と正典の単一性を上げる。コードを変えない読み取り中心でもよい。

【先に読む】
docs/HANDOFF.md、docs/runtime-state.md、docs/spec-index.json（必要行のみ）

【やること】
- docs/verification/ に PlayMode 実ラン・手動確認の要約を 1 ファイル追加
- project-context / HANDOFF / runtime-state の数値・日付の軽い同期（過剰な複製はしない）
- DECISION_LOG への追記が必要なら 1 行（表が肥大化しないよう要約）

【やらないこと】
- REFRESH モードでの長寿命ファイルの無承認大量編集（WORKFLOWS 参照）
- アーカイブ docs の読了（明示依頼時のみ）

【アクター】
assistant 主。リリース判断文は user。

【完了条件】
「次のセッションが同じ前提で再開できる」状態の記録が残っている。
```

---

## 8. 並行時のすみ分け（短いルール）


| 衝突しやすい領域                   | ルール                                   |
| -------------------------- | ------------------------------------- |
| 大きな .cs（例: ChatController） | 同時編集禁止。1 レーンに寄せるか順番を決める               |
| Yarn と SO                  | Content が Yarn、Unlock が同期ツール、マージ順を決める |
| UI                         | E レーン以外は UI_ISSUES のみ                 |
| docs 正典                    | Audit が日付・メトリクス、他レーンは必要最小の差分          |


---

## メタ

- 本ファイルの位置づけ: **PLAN MODE 直前の「レーン別起票テンプレ」**。正式な開発プラン本文は `project-context` のスライス語彙と整合させること。
- 読書順索引: `docs/ai/READ_ORDER.md` のタスク別に本ファイルを列挙済み（追加時はそこも更新）。

