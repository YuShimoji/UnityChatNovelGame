# SP-020: オンボーディング体験

Status: PARTIAL (Phase 1 実装済み: f8a536c)
作成日: 2026-03-18
カテゴリ: system/ui

## 目的

初回プレイヤーが矛盾メカニクス・断片収集・HalluciCoinの意味を自然に学べる導線を設計する。
「説明画面」ではなく「体験の中で学ぶ」方式を基本とし、
システムとアセット（Yarn）の責務分離を明確にする。

## 体験逆算: プレイヤーが学ぶべきこと

### 学習順序 (Ch1 Day1-Day3 で段階的に)

| 順序 | 学習対象 | 学び方 | タイミング |
|------|----------|--------|-----------|
| 1 | チャットの基本操作 | 最初のメッセージ送受信 | Ch1 Day1 冒頭 |
| 2 | 選択肢 | 初回選択肢の出現 | Ch1 Day1 中盤 |
| 3 | 断片の発見 | 初回 DiscoverFragment 時の SystemMessage | Ch1 Day1 後半 |
| 4 | サブスレッド | 断片発見で顕在化した A型スレッドの通知 | Ch1 Day1 後半 |
| 5 | 矛盾メカニクス | 初回の「矛盾に気づく」体験 | Ch1 Day2 |
| 6 | HalluciCoin | 矛盾発見時の報酬取得 | Ch1 Day2 |
| 7 | ダッシュボード | Day終了後のダッシュボード表示 | Ch1 Day1 終了時 |
| 8 | HCゲート | Ch2 解放条件の認識 | Ch1 完了後 |

### 現状の問題

- 順序1-2は既に自然に起きる (チャット/選択肢はUI自明)
- 順序3-4: DiscoverFragment の SystemMessage は出るが「これは何か」の説明がない
- 順序5-6: 矛盾メカニクスの操作方法が不明 (長押し→ペア選択)
- 順序7: ダッシュボードの存在が分からない (戻るボタンを押す必要がある)
- 順序8: HCゲートの意味が分からない

## 設計方針: システム vs アセット

### システムで解決するもの (エンジン基盤)

**初回検出フラグ**:
- `$onboarding_seen_fragment`: false → 初回断片発見時に true
- `$onboarding_seen_contradiction`: false → 初回矛盾発見時に true
- `$onboarding_seen_dashboard`: false → 初回ダッシュボード表示時に true

**初回時の追加SystemMessage**:
- 初回断片発見時: 「断片はサブスレッドに記録され、いつでも見返せます」
- 初回矛盾発見時: 「矛盾を見つけると HalluciCoin を獲得します。コインは新しいチャンネルの解放に使えます」
- 初回ダッシュボード表示時: 「ここから各チャンネルにアクセスできます」

**責務**: 全チャプター共通の「初回体験検出 + ヒント表示」メカニズム

### アセット（Yarn）で解決するもの

**Ch1 Day1 のストーリー内チュートリアル**:
- Pyramid (AI) がプレイヤーに「この文書を見てくれ」と促す → 断片発見の文脈
- Bernardo が「ここがおかしい」と示唆する → 矛盾への注意喚起
- ストーリーの自然な流れの中で操作を誘導

**責務**: Ch1固有のストーリー内ガイダンス

## Phase 1: 初回検出 + SystemMessage (MVP)

### 実装

```
OnboardingFlags (ScenarioManager 内 Dictionary<string, bool>)
  - "fragment": 初回断片発見検出
  - "contradiction": 初回矛盾発見検出
  - "dashboard": 初回ダッシュボード表示検出
```

**DiscoverFragmentCommand 拡張**:
```
if (!onboardingFlags["fragment"])
    SystemMessage("断片はサブスレッドに記録されます。左のサイドバーからいつでも確認できます")
    onboardingFlags["fragment"] = true
```

**ContradictionManager.OnContradictionFound 拡張**:
```
if (!onboardingFlags["contradiction"])
    SystemMessage("矛盾を見つけました。HalluciCoin を獲得 — 新しいチャンネルの解放に使えます")
    onboardingFlags["contradiction"] = true
```

**DashboardController.Show 拡張**:
```
if (!onboardingFlags["dashboard"])
    // NudgeSystem のヒントを強調表示
    onboardingFlags["dashboard"] = true
```

### Save/Load

OnboardingFlags は SaveData に含めず、$onboarding_seen_* Yarn 変数として保持する。
既存の YarnVariables 保存機構で自動永続化される。

### 受け入れ条件

1. 初回断片発見時に追加 SystemMessage が表示される
2. 2回目以降は表示されない
3. 初回矛盾発見時に追加 SystemMessage が表示される
4. Save/Load 後もフラグが保持される
5. 既存の Ch1/Ch2 Yarn を変更せずに動作する

## Phase 2: 矛盾操作チュートリアル (将来)

- 矛盾指摘可能なメッセージに初回のみハイライト表示
- 「このメッセージを長押しして矛盾を指摘できます」のツールチップ
- HUMAN_AUTHORITY: UI/UX 設計判断が必要

## Phase 3: インタラクティブガイド (将来)

- 操作手順のステップバイステップオーバーレイ
- スキップ可能
- HUMAN_AUTHORITY: UI/UX + 体験設計

## やらないこと

- 独立したチュートリアル画面/ステージ
- ゲームを止めるモーダルダイアログ
- 操作説明の翻訳対応 (現時点は日本語のみ)

## エンジン基盤としての位置づけ

Phase 1 は「初回検出 + 文脈的ヒント」の汎用メカニズムであり、
矛盾/断片に限らず将来の新機能でも同じパターンで拡張可能。
Yarn 変数ベースなので Save/Load 互換も既存機構で担保される。
