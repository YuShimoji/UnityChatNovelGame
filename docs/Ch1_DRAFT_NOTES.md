# Chapter 1 Draft Notes — 「端末の貧弱さ」

**ファイル**: `Assets/Resources/Yarn/Ch1_Terminal.yarn`
**ステータス**: たたき台（Draft v0.1）
**作成日**: 2026-03-03

---

## ノード構成図

```text
Ch1_Opening
    ├→ Ch1_AskWhere ─→ Ch1_PyramidExplain
    └→ Ch1_AskWho ──→ Ch1_PyramidExplain
                            ↓
                    Ch1_MarcoArrival
                            ↓
                    Ch1_MarcoReacts_Confused
                            ↓
                    Ch1_AreaContradiction
                            ↓
                    Ch1_TerminalTrouble
                            ↓
                    Ch1_FragmentDiscovery
                            ↓
                    Ch1_DayWinding
                            ↓
                    Ch1_DayEnd
```

**ノード数**: 10
**推定メッセージ数**: 約50-60（選択肢による変動あり）
**推定プレイ時間**: 8-12分

---

## 設計意図

### 段階的登場

1. **冒頭**: Barnaby（プレイヤー）+ Pyramid のみ
2. **中盤**: Marco が合流（接続トラブルを演出に活用）
3. **言及のみ**: Bernardo, Mason, Oliver（Ch2以降で実際に登場）

### 埋め込まれた矛盾（Ch2の指摘メカニクスへの伏線）

| 箇所 | 矛盾の内容 | プレイヤーの気づき |
| --- | --- | --- |
| Ch1_AreaContradiction | Pyramidが「2019年の広域行政再編」と言及 | 情報の正確性への疑問 |
| Ch1_AreaContradiction | 「教育支援施設」vs Marcoの「学校」 | 名称のズレ |
| Ch1_DayEnd | Pyramidが自ら「2019年の再編記録が確認できない」と訂正 | AIの情報が信頼できない |
| Ch1_FragmentDiscovery | 断片テキストの参照先が欠損 | 「不可索引物」概念の導入 |

これらの矛盾はCh1では「ノイズ」として受け流される。Ch2で指摘メカニクスが導入されてから、プレイヤーは能動的に矛盾を指摘できるようになる。

### Pyramidの失調表現

- 曖昧に整合を作る（「正式名称に基づいた表記」）
- 矛盾を指摘されると別の曖昧な整合で上書き（「地域によっては」）
- セッション終了後に自ら矛盾を認めるが、原因を通信環境のせいにする
- 05_ai_models.md の設計思想を忠実に再現

### 断片の導入

- Marcoのスクリーンショット経由で「不可索引物」の概念を自然導入
- Bernardoの不在時に彼の用語を引用（キャラの存在を先行示唆）
- `<<UnlockTopic>>` でゲームシステムとの統合を実証

---

## 使用コマンド一覧

| コマンド | 使用回数 | 用途 |
| --- | --- | --- |
| `<<set $speaker to "...">>` | 多数 | キャラクター切替 |
| `<<SystemMessage>>` | 8 | システム通知・接続状態 |
| `<<StartWait>>` | 多数 | 会話ペーシング |
| `<<Glitch>>` | 3 | 端末不調の演出（レベル1のみ） |
| `<<UnlockTopic>>` | 1 | 断片の記録 |
| `<<declare>>` | 4 | チャプター変数宣言 |
| 選択肢 (`->`) | 8組 | プレイヤーの応答 |
| `<<jump>>` | 10 | ノード間遷移 |

---

## 動作確認手順

1. Unity で ContentAuthoring シーンを開く
2. Inspector で Start Node = `Ch1_Opening` を選択
3. 「Play from Node」をクリック
4. 全分岐パターンを通す（最低2周）

### 必要な事前準備

- [x] CharacterProfile 作成: `pyramid`（ThemeColor: 薄い緑系 r:0.55/g:0.82/b:0.6、IsPlayer: false）→ `CharacterProfile_NPC_Pyramid.asset`
- [x] CharacterProfile 作成: `marco`（ThemeColor: 暖色系 r:0.9/g:0.55/b:0.3、IsPlayer: false）→ `CharacterProfile_NPC_Marco.asset`
- [x] CharacterProfile 確認: `player`（既存の `CharacterProfile_Player.asset` を使用）
- [x] TopicData 作成: `fragment_ch1_01`（タイトル: 施設管理規約（部分））→ `fragment_ch1_01.asset`

---

## 英語版リファレンス（ローカライズ用）

以下は将来の `#line:` タグ付与時に参照する英語翻訳のドラフトです。

### 主要セリフの英訳例

**Pyramid**:

- "おはようございます。Pyramid アシスタントです。" → "Good morning. This is the Pyramid assistant."
- "本日のセッションを開始します。" → "Initiating today's session."
- "通信品質が不安定なため、メッセージの遅延や欠落が発生する場合があります。" → "Due to unstable connection quality, message delays or losses may occur."
- "第4管理区域は、2019年の広域行政再編により設定された区分です。" → "Administrative Zone 4 was established as part of the 2019 regional administrative restructuring."

**Marco**:

- "3回目の接続で、やっとだ。" → "Third attempt, and it finally went through."
- "毎回パスワードが変わるのは仕様なのか嫌がらせなのか。" → "Is the password changing every time a feature or harassment?"
- "「この端末は壊れているんじゃない、最初からこういう仕様なんだ」" → "'This terminal isn't broken—it was designed this way from the start.'"
- "索引に載らない情報。検索しても引っかからない文書の欠片。" → "Information that doesn't appear in any index. Fragments of documents that no search can find."

**選択肢**:

- "ここはどこだ？" → "Where am I?"
- "...誰かいるのか？" → "...Is anyone there?"
- "地域コミュニケーション支援？" → "Community communication support?"
- "他に何かやることがあるわけでもない。" → "It's not like I have anything else to do."

---

## 今後の課題

### Ch1 への追加検討

- [ ] Marcoの文学的な語り口をもっと強調する（引用・暗喩の使用）
- [ ] 無人売店の描写を偵察パート的に膨らませるか（小報酬）
- [ ] 端末不調の演出バリエーション（Glitchレベル2を入れるか）
- [ ] 日によって「調子の良し悪し」のメリハリを表現するにはCh内でどう見せるか

### Ch2 への接続

- Ch2 で Bernardo, Mason, Oliver が合流
- 指摘メカニクスの導入（テキスト選択UI）
- 地名の矛盾が具体化（2節のテーマ）
- Ch1 で言及された「教育支援施設/学校」の再登場
