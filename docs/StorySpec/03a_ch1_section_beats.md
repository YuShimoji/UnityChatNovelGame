# 03a --- Chapter 1 Section Beats (3 days detail)

> **ステータス**: DRAFT v2 (user review required)
> **親ドキュメント**: `03_chapter_beats.md` 第1章
> **更新日**: 2026-03-12

## 設計方針

### 逆算の起点: プレイヤーが第1章を終えた時に持っているべきもの
1. **理解**: 端末は壊れている（or 最初からこういう仕様）。情報の信頼性は保証されない
2. **仲間**: 4人の人間 + 1体のAIと接点を持ち、各人の「読み方」の違いを知る
3. **手段**: 断片（不可索引物）という概念を知り、3つの実例を手元に持つ
4. **予感**: 矛盾は事件ではなくノイズ。だがノイズの中にパターンがあるかもしれない
5. **動機**: 端末の外に出て確かめたい（第2章への牽引力）

### 第1章で「まだ」持たせないもの
- 矛盾指摘メカニクス（Ch2で導入。Ch1では矛盾を「感じる」だけ）
- ハルシコイン（指摘メカニクスと連動。Ch1では存在を匂わせない）
- ~~サブスレッド（初期モックではメインログのみ）~~ → **SP-022 により任意サブスレッド（A/B/C）のパイロットを Ch1 に追加**（下表）。メイン Hub の必須トピック列は従来どおり
- 「陰謀」「制度」という確信（Ch1の認識は「ノイズ」止まり）

### インタラクションモデル: ハブ&スポーク

各 Day は以下の3フェーズで構成する:

```
Opening (線形・自動進行)
  ↓
Hub (トピック選択画面 --- 全消化で次へ)
  ├→ Topic A → Hub に戻る
  ├→ Topic B → Hub に戻る
  ├→ Topic C → Hub に戻る
  └→ (全消化) → Winding へ
  ↓
Winding + Pyramid補足 + EndDay (線形・自動進行)
```

**設計原則**:
- トピックは全て選択可能。順番だけがプレイヤーの自由
- 全トピック消化で Day 終了可能になる（mock段階では全必須）
- 各トピックの会話内容は将来トピック別ログ（サブスレッド）に保存する想定
- コスト機構は mock 段階では未実装（全トピック無料）。後付け設計に備えて構造だけ用意

### ペーシング原則
- FireWatch参照: 日常のルーティンと時間経過の感覚
- 情報は会話の中で自然に流れる。説明台詞を避け、キャラの反応から推測させる
- 各日の終わりにPyramidが「補足」する形で矛盾の種を蒔く（プレイヤーだけが気づける情報）

---

## Day 1: 端末の起動

### プレイヤー体験の目標
- **感覚**: 「見知らぬ場所で壊れた端末を拾った」感覚
- **獲得**: Pyramid（AI）とMarco（人間）の2人と接触。端末の不安定さを体感
- **断片**: 1つ目の断片を入手。「検索しても出てこない」奇妙さに触れる
- **認識更新**: 矛盾はある。でもそれは「通信環境のせい」かもしれない

### 登場キャラクター
| ID | 表示名 | 役割 |
|----|--------|------|
| `pyramid` | Pyramid | AIアシスタント。丁寧だが的外れ |
| `marco` | Marco Gross | 先住者。実地の知恵。ツッコミ役 |
| `player` | あなた | プレイヤー（選択肢のみ） |

### フェーズ構成

#### Opening: 接続 + Marco合流（線形）

**Yarn ノード**: `Ch1_Day1_Opening`

1. SystemMessage ブートシーケンス + Glitch演出
2. Pyramid が最初に話しかける（場所・目的の曖昧な説明）
3. SystemMessage「新しい参加者が接続しました」→ Marco 合流
4. Marco の第一声（苛立ち + 状況説明）
5. → Hub へ遷移

#### Hub: トピック選択

**Yarn ノード**: `Ch1_Day1_Hub`

| トピック | 変数 | 内容（スポーク） | 主な獲得物 |
|---------|------|----------------|-----------|
| この場所について | `$d1_asked_region` | Pyramidが「第4管理区域」と説明。Marcoが疑問を呈す。矛盾ペア3組が自然に出る | 世界の基本情報 + 矛盾の種 |
| 端末の状態 | `$d1_asked_terminal` | 端末の不調について。Glitch実演。Marcoの引用「仕様なんだ」| 端末不安定の体感 |
| Marco に聞く | `$d1_asked_marco` | 他メンバーの予告（Bernardo/Mason/Oliver）。配属の経緯 | 今後のキャラ予告 |
| 変なテキストについて | `$d1_asked_fragment` | Marcoが断片を共有。Pyramid検索→「該当なし」。`<<UnlockTopic "fragment_ch1_01">>` | 断片 #1 + 不可索引物の概念（Marco経由） |

**Yarn 実装パターン**:
```yarn
title: Ch1_Day1_Hub
---
<<set $speaker to "pyramid">>
<<if not $d1_asked_region and not $d1_asked_terminal and not $d1_asked_marco and not $d1_asked_fragment>>
何かお手伝いできることはありますか？
<<endif>>

-> この場所はどこなんだ？ <<if not $d1_asked_region>>
    <<set $d1_asked_region to true>>
    <<jump Ch1_Day1_Region>>
-> この端末、調子が悪いのか？ <<if not $d1_asked_terminal>>
    <<set $d1_asked_terminal to true>>
    <<jump Ch1_Day1_Terminal>>
-> Marco、他にも人がいるのか？ <<if not $d1_asked_marco>>
    <<set $d1_asked_marco to true>>
    <<jump Ch1_Day1_AskMarco>>
-> さっきの変なテキストは何だ？ <<if not $d1_asked_fragment>>
    <<set $d1_asked_fragment to true>>
    <<jump Ch1_Day1_Fragment>>
-> 今日はここまでにしよう <<if $d1_asked_region and $d1_asked_terminal and $d1_asked_marco and $d1_asked_fragment>>
    <<jump Ch1_Day1_Winding>>
===
```

各スポークは末尾で `<<jump Ch1_Day1_Hub>>` に戻る。

#### Winding: Pyramid補足 + EndDay（線形）

**Yarn ノード**: `Ch1_Day1_Winding` → `Ch1_Day1_End`

1. Pyramid セッション終了告知
2. Marco が無人売店の存在を教える（生活の甘美）
3. Marco 切断
4. **Pyramid 補足**（1対1）:
   - 「断片について再検索 → 1件確認 → 内容不一致」
   - 「2019年の再編の実施記録が確認できない」（自身の矛盾を自白）
5. `<<EndDay 1>>`

---

## Day 2: 断片の読み方

### プレイヤー体験の目標
- **感覚**: 「昨日のあれは何だったのか」を振り返る間もなく、分析できる人が現れる
- **獲得**: ベルナルド（文体分析の専門家）と接触。断片を「読む」方法を学ぶ
- **断片**: 2つ目の断片を入手。Day 1の断片との違いに気づく
- **認識更新**: 断片はランダムなノイズではない。書式・文体にパターンがある

### 登場キャラクター
| ID | 表示名 | 役割 |
|----|--------|------|
| `pyramid` | Pyramid | 前日の会話を一部「覚えていない」 |
| `marco` | Marco Gross | 紹介役。分析に感心しつつ「で、何がわかるんだ」 |
| `bernardo` | Bernardo Fonseca | 断片分析師。元新聞社の校正者/編集者 |
| `player` | あなた | プレイヤー |

### フェーズ構成

#### Opening: 再接続 + Bernardo合流（線形）

**Yarn ノード**: `Ch1_Day2_Opening`

1. 接続演出（Day1より短い --- 「慣れ」の表現）
2. Pyramid「おはようございます」→ 昨日の会話を誤って要約（矛盾ペア: ch1_session_memory）
3. Marco が Bernardo の到着を予告
4. SystemMessage → Bernardo 合流。最小限の自己紹介
5. → Hub へ遷移

#### Hub: トピック選択

**Yarn ノード**: `Ch1_Day2_Hub`

| トピック | 変数 | 内容（スポーク） | 主な獲得物 |
|---------|------|----------------|-----------|
| 昨日の断片を見せる | `$d2_asked_analysis` | Bernardo が fragment_ch1_01 を分析。文体＝行政文書、書式が半端、参照先が消えている。「不可索引物」を正式定義 | 断片の分析法 + 不可索引物の定義 |
| Bernardo について | `$d2_asked_bernardo` | 経歴（新聞社の校正者）。文体の違いを見抜く能力の由来 | Bernardo の人物理解 |
| Pyramid に昨日のことを聞く | `$d2_asked_pyramid` | Pyramid が昨日の補足内容を「覚えていない」。矛盾ペア: ch1_session_memory | Pyramid の記憶不正確を確認 |
| 新しいテキストが出た | `$d2_asked_fragment2` | 端末に直接表示された断片 #2（教育教材風）。Bernardo が即座に「行政文書ではない。教育教材」と指摘。`<<UnlockTopic "fragment_ch1_02">>` | 断片 #2 + 文体カテゴリの概念 |

**新断片のトリガー**: `$d2_asked_analysis` が true になった後、Hub に戻るタイミングで SystemMessage「端末に新しいテキストが表示されました」→ 「新しいテキストが出た」トピックが出現。

**Yarn 実装メモ**: 断片 #2 トピックの出現条件は `<<if $d2_asked_analysis and not $d2_asked_fragment2>>`。分析を先に聞かないと新断片に気づかない演出。

#### Winding: Pyramid補足 + EndDay（線形）

**Yarn ノード**: `Ch1_Day2_Winding` → `Ch1_Day2_End`

1. Pyramid セッション終了告知。「明日はもう1名接続予定」（Mason予告）
2. Bernardo「今日の断片を保存しておいてくれ。文体分類で出典追跡できるかもしれない」
3. Marco / Bernardo 切断
4. **Pyramid 補足**（1対1）:
   - 「教育課程の該当カリキュラムが確認できない」
   - 「『統合地理』という教科名はデータベースに未登録」
5. `<<EndDay 2>>`

---

## Day 3: 外の世界

### プレイヤー体験の目標
- **感覚**: 端末の中だけでは限界がある。外に出て確かめた人がいる
- **獲得**: メイスン（偵察者）と接触。端末情報と物理世界の齟齬を知る
- **断片**: 3つ目の断片を入手（物理世界由来）
- **認識更新**: 端末は壊れているのではなく、情報環境そのものが断片化している
- **牽引**: 第2章への動機

### 登場キャラクター
| ID | 表示名 | 役割 |
|----|--------|------|
| `pyramid` | Pyramid | 記憶劣化がさらに進行 |
| `marco` | Marco Gross | メイスンの報告に興味津々 |
| `bernardo` | Bernardo Fonseca | 3断片の比較分析 |
| `mason` | Mason | 偵察者。簡潔に事実だけ伝える |
| `player` | あなた | プレイヤー |

### フェーズ構成

#### Opening: 再接続 + Mason合流（線形）

**Yarn ノード**: `Ch1_Day3_Opening`

1. Barnaby より先に Marco + Bernardo + Pyramid が接続済み（世界が動いている感覚）
2. Marco「今日はメイスンが来る」
3. SystemMessage → Mason 合流。最小限の挨拶、すぐ本題
4. → Hub へ遷移

#### Hub: トピック選択

**Yarn ノード**: `Ch1_Day3_Hub`

| トピック | 変数 | 内容（スポーク） | 主な獲得物 |
|---------|------|----------------|-----------|
| メイスンの報告を聞く | `$d3_asked_mason_report` | 無人売店の二重看板、閉鎖建物の訪問。Pyramidが施設ステータスを出せない（矛盾ペア: ch1_facility_status） | 偵察の概念 + 端末 vs 物理の齟齬 |
| 掲示板の紙を見る | `$d3_asked_fragment3` | Mason が撮影した掲示板の紙 → 断片 #3（施設利用案内）。`<<UnlockTopic "fragment_ch1_03">>` | 断片 #3（物理由来） |
| 3つの断片を比較 | `$d3_asked_compare` | Bernardo が3断片を並べて分析。「行政/教育/施設案内。書き手が違う。でも全部、参照先が消えている」。Marco「わざと消してるのか」→ Bernardo「両方ありうる」 | パターンの明示 + 陰謀に短絡しない留保 |
| これからどうする | `$d3_asked_plan` | チームの現状整理。Marco「情報環境そのものがこうなってる」。Mason「明日もう少し遠くまで行く」。Bernardo「材料が足りない」 | 章の結論 + 第2章への動機 |

**出現条件**: 「3つの断片を比較」は `$d3_asked_fragment3` が true のとき出現。「これからどうする」は `$d3_asked_compare` が true のとき出現。

#### Winding: Pyramid補足 + EndDay（線形）

**Yarn ノード**: `Ch1_Day3_Winding` → `Ch1_Day3_End`

1. Pyramid セッション終了告知
2. Marco → Bernardo → Mason の順に切断
3. **Pyramid 補足**（1対1）:
   - 「断片3件を端末内に保存しました」
   - 「一致する公的記録が見つかりませんでした」
   - （一拍）「...公的記録が存在しない、というわけではありません。照合範囲が限定されている可能性があります」
4. `<<EndDay 3>>`

---

## Yarn ノード一覧（実装用）

### Day 1（11ノード・SP-022 パイロット含む）
| ノード | 種別 | 遷移先 |
|--------|------|--------|
| `Ch1_Day1_Opening` | Opening (線形) | → Hub |
| `Ch1_Day1_Hub` | Hub (選択) | → 各スポーク / Winding |
| `Ch1_Day1_Region` | スポーク | → Hub |
| `Ch1_Day1_Terminal` | スポーク | → Hub |
| `Ch1_Day1_AskMarco` | スポーク | → Hub |
| `Ch1_Day1_ScoutNetwork` | スポーク（任意・C） | → Hub |
| `Ch1_Day1_AnnotGlossary` | スポーク（任意・A） | → Hub |
| `Ch1_Day1_Fragment` | スポーク | → Hub |
| `Ch1_Day1_BranchPyramid` | 分岐（任意） | → Hub |
| `Ch1_Day1_Winding` | Winding (線形) | → End |
| `Ch1_Day1_End` | Pyramid補足 (線形) | → EndDay |

### Day 2（6ノード・`Ch1_Day1.yarn` 内モック Hub + SP-022 パイロット）
| ノード | 種別 | 遷移先 |
|--------|------|--------|
| `Ch1_Day2_Opening` | Opening (線形) | → Hub |
| `Ch1_Day2_Hub` | Hub (選択) | → 各スポーク / Winding |
| `Ch1_Day2_Update` | スポーク | → Hub |
| `Ch1_Day2_Status` | スポーク | → Hub |
| `Ch1_Day2_ScoutPing` | スポーク（任意・C） | → Hub |
| `Ch1_Day2_Winding` | Winding (線形) | → EndDay |

### Day 2（設計ビート・上文 Day 2 節。本番尺は未だ別ファイル未作成）

| ノード | 種別 | 遷移先 |
|--------|------|--------|
| `Ch1_Day2_Opening` | Opening (線形) | → Hub |
| `Ch1_Day2_Hub` | Hub (選択) | → 各スポーク / Winding |
| `Ch1_Day2_Analysis` | スポーク | → Hub |
| `Ch1_Day2_AskBernardo` | スポーク | → Hub |
| `Ch1_Day2_AskPyramid` | スポーク | → Hub |
| `Ch1_Day2_Fragment2` | スポーク | → Hub |
| `Ch1_Day2_Winding` | Winding (線形) | → End |
| `Ch1_Day2_End` | Pyramid補足 (線形) | → EndDay |

### Day 3（10ノード）
| ノード | 種別 | 遷移先 |
|--------|------|--------|
| `Ch1_Day3_Opening` | Opening (線形) | → Hub |
| `Ch1_Day3_Hub` | Hub (選択) | → 各スポーク / Winding |
| `Ch1_Day3_MasonReport` | スポーク | → Hub |
| `Ch1_Day3_Fragment3` | スポーク | → Hub |
| `Ch1_Day3_Compare` | スポーク (条件付き出現) | → Hub |
| `Ch1_Day3_Plan` | スポーク (条件付き出現) | → Hub |
| `Ch1_Day3_Winding` | Winding (線形) | → End |
| `Ch1_Day3_End` | Pyramid補足 (線形) | → EndDay |

**現リポ実装（`Ch1_Day1.yarn`）**: Day1 11 ノード + Day2 モック 6 ノード + Day3 本番尺（設計表の 10 ノードに相当するブロックが同一ファイル内に存在）。**設計上の Ch1 全 Day**: 27 ノード目安

---

## SP-022 サブクエスト対応（Ch1 パイロット）

[22_subquest_exploration_content.md](22_subquest_exploration_content.md) の節↔ID 対応。**既存 Yarn コマンドのみ**。

| サブクエスト ID | 型 | Day | 挿入位置 | 必須 | Yarn ノード |
|-----------------|-----|-----|----------|------|-------------|
| ch1_note_facility | A | 1 | 断片共有後（Manifest） | 任意（自動） | `Ch1_Day1_Fragment` |
| ch1_cond_analysis | B | 1→2 | 断片取得済みで Day2 から | 任意（自動） | `Ch1_Day2_Opening`（LatentCond） |
| scout_ch1_network | C | 1 | Hub（任意選択） | 任意 | `Ch1_Day1_ScoutNetwork` |
| annot_ch1_glossary | A | 1 | Hub（断片閲覧後・任意） | 任意 | `Ch1_Day1_AnnotGlossary` |
| scout_ch1_day2_ping | C | 2 | Day2 Hub（任意選択） | 任意 | `Ch1_Day2_ScoutPing` |
| scout_ch1_d3_route | C | 3 | Mason 報告（Manifest） | 任意（Hub 必須トピック内） | `Ch1_Day3_MasonReport` |
| scout_ch1_d3_board | C | 3 | 断片 #3 取得時（Manifest） | 任意（Hub 必須トピック内） | `Ch1_Day3_Fragment3` |
| annot_ch1_d3_compare | A | 3 | 3 断片比較（Manifest） | 任意（Hub 必須トピック内） | `Ch1_Day3_Compare` |
| ch1_branch_analysis | 分岐 | 1 | Winding（Pyramid 補足分析） | メイン進行に沿う | `Ch1_Day1_BranchPyramid`（`BeginBranch`） |

### SUBSEQUENT 移行判定メモ（2026-04-09）

- **LATER 接続 OK 条件**:
  - Day1〜Day3 の必須Hubを通過し、`EndDay 3` まで進行不能がない
  - Day3 の `scout_ch1_d3_route` / `scout_ch1_d3_board` / `annot_ch1_d3_compare` の Manifest 後に破綻がない
  - Save/Load でサブスレッド状態が再現可能
- **P0（先に止める）**:
  - 上記 3 条件のいずれかが破綻
- **P1（Ch2 着手と並行で仕様調整）**:
  - Day3 の C2 本がテンポを崩す場合（統合 / 遅延 / 維持の判断）
  - `ch1_cond_analysis` の表現が B 型期待との差で誤読を招く場合
- **P2（後続スライス）**:
  - Wiki 遷移・成果物カードなど新規UI実装を伴う拡張

**2026-04-10（ドキュメント）**: [docs/verification/2026-04-10-subsequent-completion-report.md](../verification/2026-04-10-subsequent-completion-report.md) で再現手順と静的整合を固定。**Editor 実測前**は上記 OK 条件の最終判定を保留。実測で P0 なし → Ch2（LATER）。P0 あり → P0 のみ短いスライス → 再検証。

---

## 断片一覧（Ch1）

| ID | 名称 | 文体カテゴリ | Day | Hub トピック | 欠損パターン |
|----|------|-------------|-----|-------------|-------------|
| fragment_ch1_01 | 施設管理規約（部分） | 行政文書 | 1 | 変なテキストについて | 参照先（付録C）が伏字。発行主体が空白 |
| fragment_ch1_02 | 教育課程概要（部分） | 教育教材 | 2 | 新しいテキストが出た | 正解欄が欠損。教科名が見慣れない |
| fragment_ch1_03 | 施設利用案内（部分） | 施設案内 | 3 | 掲示板の紙を見る | 適用地名が途中で変更。改訂履歴欄が空白 |

---

## 矛盾ペア（Ch1）

Ch1では矛盾指摘メカニクスは発火しない。line tags はYarnスクリプトに付与し、Ch2以降に備える。

| ペアID | ソース（Pyramid） | ターゲット（人間側） | Day | Hub トピック |
|--------|------------------|---------------------|-----|-------------|
| ch1_region_identity | 「第4管理区域」 | Marco「聞いたことない」 | 1 | この場所について |
| ch1_admin_reform | 「2019年改編」 | Marco「2021年に来た」 | 1 | この場所について |
| ch1_facility_name | 「教育支援施設」 | Marco「学校だったはず」 | 1 | この場所について |
| ch1_search_result | 「該当なし」 | Pyramid自身が「1件確認」 | 1 | (Winding) Pyramid補足 |
| ch1_session_memory | Day1の会話を誤要約 | プレイヤーが前日と比較 | 2 | (Opening) 自動 |
| ch1_curriculum_ref | 「該当カリキュラムなし」 | 断片に「統合地理」が存在 | 2 | (Winding) Pyramid補足 |
| ch1_facility_status | 施設ステータス不明 | Masonの物理確認と齟齬 | 3 | メイスンの報告 |
| ch1_record_scope | 「公的記録なし」→「照合範囲が限定」 | Pyramid自身の前言修正 | 3 | (Winding) Pyramid補足 |

---

## エンジン実装メモ

### 必要なコマンド/ハンドラ
| コマンド | 現状 | 備考 |
|----------|------|------|
| `<<EndDay N>>` | 実装済み | SystemMessage表示 + `ch{N}` をSaveData完了登録。Day単位 vs Chapter単位の粒度は要検討 |
| `<<UnlockTopic "id">>` | 実装済み | fragment_ch1_01 ~ 03 で使用 |
| `<<SystemMessage "text">>` | 実装済み | 接続/切断/通信状況/断片獲得で多用 |
| `<<Glitch N>>` | 実装済み | Day 1 で多め、Day 3 で減少 |
| `<<StartWait N>>` | 実装済み | 会話テンポ制御 |
| `<<Typing bool>>` | 実装済み | Pyramid の長考演出 |

### Yarnスクリプト構成
- `Assets/Resources/Yarn/active/Ch1_Day1.yarn` --- **Day 1〜3** 全ノード + SP-022 パイロット（単一ファイル）
- 設計ビートの本番尺に分割する場合のみ `Ch1_Day2.yarn` / `Ch1_Day3.yarn` へ切り出しを検討

### ダッシュボード連携（要決定）
- EndDay が `ch{N}` を完了登録する現在の実装では、Day1で `ch1` が完了になる
- Ch1を3 Day で構成する場合の選択肢:
  - A) `ch1_day1` / `ch1_day2` / `ch1_day3` の3チャンネルに分割
  - B) EndDay のチャンネルID生成ロジックを変更
  - C) Ch1は単一チャンネルのまま、Day遷移はYarn内部で処理（EndDayを使わない）

### 将来のコスト機構フック
- Hub の選択肢にコスト条件を追加するだけで拡張可能:
  ```yarn
  -> トピックA <<if not $asked_a and $action_points >= 1>>
  ```
- mock 段階ではコスト条件なし（全トピック無料）

### 将来のトピック別ログ（サブスレッド）
- 各スポークの会話内容をトピック別スレッドに保存する仕組みは `16_subthread_ui.md` と接続
- mock 段階ではメインチャットに全て表示（スレッド分離なし）

---

## 第2章への接続

第1章終了時点でプレイヤーが持つ「引き」:
1. **断片のパターン**: 3つの断片に共通する欠損。「もっと集めれば何かわかる」
2. **Pyramidへの不信**: 矛盾を重ねるが、他に情報源がない
3. **メイスンの偵察**: 「明日はもう少し遠くまで」--- 外の世界にもっと手がかりがある
4. **未登場のオリバー**: Marco/Bernardo が名前だけ言及。「もう一人いるはず」

第2章（地名・所属・時刻の滑り）では:
- オリバーが合流し、地名の二重表記問題が中心化
- 矛盾指摘メカニクスが導入（ハルシコインの獲得開始）
- 断片の文体カテゴリが拡大（二重地名表記の断片）
