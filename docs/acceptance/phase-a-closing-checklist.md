# Phase A クロージング 手動確認チェックリスト

対象: UnityChatNovelGame (FoundPhone)
作成日: 2026-03-16
確認環境: Unity Editor (ContentAuthoring シーン)

---

## 事前セットアップ

シーン再生前に Inspector で以下を確認する。

- [ ] `DashboardManager` が Hierarchy に存在し `m_ShowOnStart = true`
- [ ] `DebugHubController` の `m_ShowOnStart = false`
- [ ] `ScenarioManager` の `m_AutoStartYarn = false`
- [ ] `ContradictionFeedback` オブジェクトが Canvas 直下に存在する

### ContradictionFeedbackController セットアップ（未作成の場合）

1. Canvas 右クリック → Create Empty → 名前「ContradictionFeedback」
2. Add Component → `ContradictionFeedbackController`
3. Inspector の `Chat Controller` フィールドに ChatController をアサイン
4. シーンを保存

---

## A: ダッシュボード基本導線（スモーク）

前回確認済み (2026-03-11)。サブスレッド追加後の回帰として簡易確認。

- [ ] A-1. シーン再生 → ダッシュボードがデフォルト表示
- [ ] A-2. Ch1 カードが [AVAILABLE]
- [ ] A-3. Ch2 カードが [LOCKED]
- [ ] A-4. Ch1 クリック → チャット開始
- [ ] A-5. Back / ESC → シナリオ停止 → ダッシュボード復帰
- [ ] A-6. F12 → DebugHub 独立表示（競合なし）

---

## B: Ch1 / Ch2 再生確認

### B-1. Ch1 後半プレイヤーセリフ

- [ ] B-1a. プレイヤーセリフがチャット画面に表示される（欠落なし）
- [ ] B-1b. プレイヤーバブルが右寄せ・選択肢バブルと同色
- [ ] B-1c. 選択後にテキストがメッセージ表示され、二重表示なし

### B-2. Ch2 タイピングインジケーター

- [ ] B-2a. 3点インジケーターがチャット最下部に表示（上部固定でない）
- [ ] B-2b. 次メッセージ出現でインジケーター消滅

### B-3. Ch2 選択肢タイミング

- [ ] B-3a. 選択肢表示前に 400ms 程度の遅延（アニメ重複なし）
- [ ] B-3b. 選択後、未選択肢がフェードアウト

---

## C: 矛盾 Phase 2 手動テスト

### C-1. 成功ケース

DebugHub → `ETK_Contradiction`（または Ch1 の `#line:` タグ付きメッセージ）

1. [ ] C-1a. 矛盾タグ付きバブルを 0.5秒長押し → 青ハイライト
2. [ ] C-1b. ヒントバナー出現「2つ目のメッセージをタップしてください」
3. [ ] C-1c. 対応バブルをタップ → 両バブルが緑フラッシュ
4. [ ] C-1d. 両バブル間に接続線表示（その後フェードアウト）
5. [ ] C-1e. 通知パネル（中央）が下からスライドイン + HalluciCoin 報酬表示
6. [ ] C-1f. 通知パネルが約3秒後に自動退場

### C-2. 失敗ケース（不一致）

1. [ ] C-2a. 矛盾タグ付きバブルを長押し → 青ハイライト
2. [ ] C-2b. ペアでないバブルをタップ → 赤フラッシュ + シェイク
3. [ ] C-2c. エラーバナー「矛盾が見つかりませんでした」
4. [ ] C-2d. エラーバナーが約2秒後に自動消滅

### C-3. 既発見ケース

- [ ] C-3a. 成功済みペアを再度操作 → 黄色フラッシュ + 「既に発見済みです」

---

## D: サブスレッド UI（5a-5e + 5d）

DebugHub → `ETK_ThreadType`

### D-1. スレッド宣言と基本表示

- [ ] D-1a. DeclareThreadTyped 実行後、左上にハンバーガーボタン（≡）出現
- [ ] D-1b. 最初のスレッド宣言前はハンバーガーボタン非表示

### D-2. 通知バナー

- [ ] D-2a. Main 表示中にサブスレッドへメッセージ追加 → 上部に通知バナー
- [ ] D-2b. バナーに型アイコン（[A]/[B]/[C]）とスレッド名が含まれる
- [ ] D-2c. バナーが約3.5秒後にフェードアウト
- [ ] D-2d. バナークリック → 該当スレッドに切替 + バナー消滅

### D-3. サイドバー開閉

- [ ] D-3a. ハンバーガーボタンタップ → サイドバーが左からスライドイン（0.25秒）
- [ ] D-3b. 半透明オーバーレイが背景に表示
- [ ] D-3c. オーバーレイタップ → サイドバーが閉じる
- [ ] D-3d. サイドバー先頭に「Main」エントリが常に表示

### D-4. ThreadType グループ分類

- [ ] D-4a. Annotation/Tracking/Scout のグループヘッダーが型ごとに区切って表示
- [ ] D-4b. 各エントリに型アイコンが型別色で表示
  - Annotation: 青 (#4A90D9)
  - Tracking: 緑 (#4CAF50)
  - Scout: オレンジ (#FF9800)

### D-5. スレッド切替とヘッダーバー

- [ ] D-5a. サイドバーでスレッド選択 → チャット内容が切替
- [ ] D-5b. 切替時にチャット上部にヘッダーバー（型色帯 + 型名 + スレッド名）表示
- [ ] D-5c. Main に戻るとヘッダーバー非表示
- [ ] D-5d. サイドバーでアクティブスレッドが強調表示

### D-6. 未読バッジ

- [ ] D-6a. 未読ありスレッドのサイドバーエントリ右端にバッジ（赤丸+数値）
- [ ] D-6b. ハンバーガーボタン右上に未読合計バッジ
- [ ] D-6c. スレッドを開くと未読バッジクリア
- [ ] D-6d. 全スレッド未読ゼロでハンバーガーバッジ消滅

### D-7. 複数スレッド並走（ETK_ThreadParallel）

ETK_ThreadType から「並走テストへ進む」を選択。

- [ ] D-7a. ノード遷移後も前ノードで宣言した3スレッドが維持
- [ ] D-7b. 各スレッドに追加したメッセージが正しい末尾に追加
- [ ] D-7c. etk_track に pyramid → bernardo 順でメッセージ追加
- [ ] D-7d. etk_note に marco のメッセージ追加
- [ ] D-7e. メッセージ追加時に通知バナーが適切に表示（Main 表示中）

### D-8. 潜在スレッド (DeclareThreadLatent + ManifestThread)

ETK_Branch テスト内で確認。

- [ ] D-8a. DeclareThreadLatent 後、サイドバーに「隠された覚書」が表示されていないこと
- [ ] D-8b. 潜在中に AddThreadMessage でメッセージ蓄積されていること (顕在化後に確認)
- [ ] D-8c. ManifestThread 後、サイドバーに「隠された覚書」が出現すること
- [ ] D-8d. 出現通知 (型色付きシステムメッセージ + ハンバーガーパルス) が表示されること

### D-9. CompleteThread

ETK_Branch テスト内で確認。

- [ ] D-9a. CompleteThread 後、サイドバーで対象スレッドがグレーアウト表示
- [ ] D-9b. スレッド名の前にチェックマーク (✓) が付いていること
- [ ] D-9c. 完了スレッドの未読バッジが消えていること

### D-10. 分岐スレッド (BeginBranch / EndBranch)

ETK_Branch テスト内で確認。

- [ ] D-10a. BeginBranch で自動的にブランチスレッドに切替されること
- [ ] D-10b. ヘッダーバーに紫色の [>] + スレッド名が表示されること
- [ ] D-10c. 分岐中にサイドバーで Main に自由に切替・復帰できること
- [ ] D-10d. EndBranch 後にメインに自動復帰し、反映メッセージが表示されること
- [ ] D-10e. SetBranchReflection で指定したテキストが反映メッセージに使われること

### D-11. 分岐内トピック自動追跡 → 自動反映メッセージ

ETK_Branch 内 etk_branch_auto で確認。

- [ ] D-11a. 分岐内で UnlockTopic 実行後、EndBranch 時に自動反映メッセージが Main に表示されること
- [ ] D-11b. 自動反映メッセージにトピック名が含まれること
- [ ] D-11c. SetBranchReflection 未指定時でも自動反映メッセージが生成されること

---

## G: HalluciCoin 確認

### G-1. Silent Increment

ETK_Commands → AddHalluciCoin テストで確認。

- [ ] G-1a. AddHalluciCoin 実行後、ダッシュボードに戻ると HC 値が増加している
- [ ] G-1b. HC 表示がパルスアニメーション (scale + 色ハイライト) すること
- [ ] G-1c. チャット画面では HC 増加の通知が表示されないこと (silent)

---

## E: Save / Load スレッド履歴保持

ブロック D の途中（スレッドに複数メッセージがある状態）で実施。
**重要: サブスレッド表示中にセーブすること（過去バグ 3d0a0f6 の再確認）**

1. [ ] E-1. セーブスロットにセーブ実行
2. [ ] E-2. シーン停止 → 再生 → 同スロットからロード
3. [ ] E-3. Main スレッドの履歴が保持されている
4. [ ] E-4. サブスレッドの履歴が保持されている
5. [ ] E-5. ハンバーガーボタン再表示、サイドバーで全スレッド確認可能
6. [ ] E-6. ロード後にスレッド切替が正常動作

---

## F: Console エラー確認

各ブロック通じて随時確認。

- [ ] F-1. Console に Exception / Error 出力がない
- [ ] F-2. DialogueException が出ていない
- [ ] F-3. ContradictionFeedbackController の Warning がない
- [ ] F-4. ThreadSwitcherController の Warning がない

---

## 最短実行フロー (推奨手順)

Unity起動後、以下の順で実施すると操作の重複を最小化できる。
F (Console確認) は全ステップで常時監視する。

### Step 0: 事前セットアップ (5分)
- Inspector 確認 (上記「事前セットアップ」4項目)
- ContradictionFeedbackController が未作成の場合は作成

### Step 1: ダッシュボードスモーク → A全項目 (3分)
- シーン再生 → A-1〜A-6 を確認
- A-5 でダッシュボードに戻った状態を維持

### Step 2: ETK_AutoVerify → D-1〜D-9, E, F 一括 (15分)
- F12 → DebugHub → ETK_Menu → 「自動検証 (Phase A Block D/E)」
- ETK_AutoVerify が D-1〜D-9 + E + F の [CHECK] メッセージを順に表示
- 各 [CHECK] で目視確認しながら進行
- E のセーブ/ロードは手動操作 (サブスレッド表示中にセーブすること)

### Step 3: ETK_Branch → D-10, D-11 (10分)
- ETK_Menu → 「分岐スレッド テスト」
- ETK_Branch 内で D-8〜D-11 を網羅 (潜在→顕在→分岐→自動反映→完了)
- Step 2 との重複あり (D-8/D-9) — 2回目の確認として扱う

### Step 4: ETK_ThreadType → D-7 並走テスト (5分)
- ETK_Menu → 「サブスレッド/ThreadType テスト」→「並走テストへ進む」
- D-7a〜D-7e を確認

### Step 5: ETK_Commands → G (HalluciCoin) (5分)
- ETK_Menu → 「コマンド テスト」
- G-1c: チャット画面でHC通知が出ないこと確認
- Back → ダッシュボード復帰 → G-1a, G-1b 確認

### Step 6: ETK_Contradiction → C (矛盾) (10分)
- ETK_Menu → 「矛盾指摘 テスト」
- C-1: 成功ケース (長押し→タップ→緑フラッシュ→接続線→報酬パネル)
- C-2: 失敗ケース (ペアでないバブルタップ→赤フラッシュ→シェイク)
- C-3: 既発見ケース (再操作→黄色フラッシュ)

### Step 7: Ch1/Ch2 回帰 → B (10分)
- ダッシュボード → Ch1 選択 → B-1a〜B-1c
- Back → ダッシュボード → Ch2 選択 → B-2a〜B-3b

### 合計見積: 約60分 (セットアップ含む)

---

## 確認優先順 (参考: ブロック単位)

| 優先 | ブロック | 理由 |
|------|----------|------|
| 1 | A (スモーク) | 基本導線の破綻確認。全ブロックの前提 |
| 2 | D-8/9/10/11 (潜在/完了/分岐/自動追跡) | 2026-03-17 新機能。ETK_Branch から起動可能 |
| 3 | C (矛盾 Phase2) | 最長未確認。セットアップ必要で完全未テスト |
| 4 | D-1〜7 + E (サブスレッド + Save/Load) | 5a-5e の受け入れ。ETK から単独起動可能 |
| 5 | G (HalluciCoin) | ETK_Commands から起動、ダッシュボード復帰で確認 |
| 6 | B (Ch1/Ch2 再生) | 回帰確認。Ch1 に新コマンド組込あり |
| 7 | F (Console) | 各ブロック通じて都度確認 |

---

## 注意点（過去バグの再発箇所）

1. **ContradictionFeedbackController 未アサイン**: m_ChatController が null だとハイライトが出ない
2. **サブスレッド表示中セーブ**: 修正前は Main 履歴消失。E-1 ではサブスレッド表示中にセーブすること
3. **ノード遷移後スレッド消失**: m_DeclaredThreads は遷移で消えないが LoadGame で ClearDeclaredThreads が走る
4. **矛盾アニメ中の連続操作**: m_IsPlayingResultAnimation フラグが false に戻る前の長押しでハイライト競合の可能性
