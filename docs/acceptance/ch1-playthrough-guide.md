# Ch1 通しプレイ体験ガイド

対象: UnityChatNovelGame (FoundPhone)
目的: Ch1 Day1 の起動→完了の導線が破綻なく動作することを確認する
前提: ContentAuthoring シーンを使用

---

## 準備

- [ ] DashboardManager の m_ShowOnStart = true
- [ ] DebugHubController の m_ShowOnStart = false
- [ ] ScenarioManager の m_AutoStartYarn = false
- [ ] ContradictionManager の m_Database がアサイン済み

---

## 導線フロー

### 1. ダッシュボード表示
- [ ] シーン再生 → ダッシュボードが表示される
- [ ] Ch1 カードが [AVAILABLE] 表示
- [ ] HC: 0 が右上に表示される

### 2. Ch1 Day1 開始
- [ ] Ch1 カードをクリック → チャット画面に遷移
- [ ] SystemMessage「端末を起動しています...」が表示
- [ ] Glitch 1 のエフェクトが走る
- [ ] Pyramid の最初のメッセージ (タイピングインジケーター → メッセージ)

### 3. Hub&Spoke: 4トピック消化
以下4つの選択肢を順に消化:

**3a. 「この場所はどこなんだ？」 → Ch1_Day1_Region**
- [ ] Pyramid の回答に #line: タグ付きメッセージあり (ch1_region_identity_src)
- [ ] Marco の反応に #line: タグ付きメッセージあり (ch1_region_identity_tgt)
- [ ] Hub に戻る

**3b. 「この端末、調子が悪いのか？」 → Ch1_Day1_Terminal**
- [ ] Hub に戻る

**3c. 「Marco、他にも人がいるのか？」 → Ch1_Day1_AskMarco**
- [ ] Hub に戻る

**3d. 「さっきの変なテキストは何だ？」 → Ch1_Day1_Fragment**
- [ ] UnlockTopic「fragment_ch1_01」発火
- [ ] SystemMessage「断片「施設管理規約（部分）」を記録しました」
- [ ] ManifestThread「ch1_note_facility」が発火 → サイドバーに「施設管理規約メモ」が出現
- [ ] 通知メッセージ「[A] 新しいスレッド「施設管理規約メモ」が利用可能です」
- [ ] ハンバーガーボタンが青色パルス
- [ ] Hub に戻る

### 4. オプショナル分岐 (region + fragment 消化後に出現)
**「Pyramid、さっきの情報には矛盾がないか？」 → Ch1_Day1_BranchPyramid**
- [ ] 「聞かせてくれ」を選択 → BeginBranch で分岐スレッドに自動切替
- [ ] ヘッダーバーに紫 [>] Pyramidの補足分析 が表示
- [ ] 分岐内で UnlockTopic「topic_suspicious_message」
- [ ] EndBranch → メインに自動復帰 + 反映メッセージ表示
- [ ] Hub に戻る

### 5. Day 終了
**「今日はここまでにしよう」(4トピック全消化後)**
- [ ] Ch1_Day1_Winding に遷移
- [ ] Marco の切断メッセージ表示
- [ ] AddHalluciCoin +2 (silent、画面上通知なし)
- [ ] Ch1_Day1_End → EndDay 1 実行
- [ ] SystemMessage「--- 1日目 終了 ---」

### 6. ダッシュボード復帰
- [ ] ESC/Back でダッシュボードに戻る
- [ ] HC: 2 が表示 (パルスアニメーション)
- [ ] Ch1 カードが [IN PROGRESS] (Day1 完了、Day2 残)

### 7. サイドバー確認
- [ ] ハンバーガーボタンをタップ → サイドバー開く
- [ ] 「施設管理規約メモ」(A型) が表示されている
- [ ] 「施設管理規約メモ」を選択 → A型カード表示 (中央配置、情報カード風)
- [ ] AddThreadMessage で追加した2件のメッセージが見える
- [ ] Main に戻る

---

## 検証ポイント

| カテゴリ | 確認対象 | 判定基準 |
|---------|---------|---------|
| 基本導線 | ダッシュボード→Ch1→Hub→完了 | 破綻なく遷移 |
| 新コマンド | DeclareThreadLatent/ManifestThread | 潜在→断片発見で顕在化 |
| 分岐 | BeginBranch/EndBranch | 自動切替・復帰・反映メッセージ |
| HC | AddHalluciCoin | ダッシュボード復帰時にパルス |
| 矛盾タグ | #line: タグ付きバブル | バブルが長押し可能 (Phase A-C で検証) |
| サブスレッド | A型カード表示 | 中央配置・情報カード風 |

---

## Console エラー確認
- [ ] Exception / Error が出ていないこと
- [ ] DialogueException が出ていないこと
