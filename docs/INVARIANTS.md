# Invariants

破ってはいけない条件・責務境界・UX不変量を保持する正本。

## Systemic Diagnosis

- 重要なのは個々のUI症状でなく、全体のデザイン一貫性 (session 16 フィードバック)
- 微修正→手動検証ループは構造的問題。1件ごとに回さない (session 17 方向転換)

## UX / Algorithmic Invariants

- タップスキップは2段階: 第1タップ=テキスト全文表示、第2タップ=次メッセージへ
- SystemMessage はタップスキップ対象外 (即時表示・遅延なし)
- スレッド切替時はフェードイン (CanvasGroup alpha 0→1)。フラッシュ禁止
- スクロール吸着はスムーズアニメーション (DOTween 0.2s)。直接代入禁止
- フォントサイズのハードコード禁止。ChatUIConfig / UIFontConfig 経由のみ
- 明文化なき表示挙動の実装禁止。DISPLAY_ALGORITHMS.md を先に更新する

## Responsibility Boundaries

- 値の調整 (フォント/色/タイミング/レイアウト) は Inspector 操作。コード変更ではない
- コンテンツ追加は Yarn 執筆 → Validator → SO生成 のパイプライン経由
- 制作フロー再開時の入口は `docs/HANDOFF.md`。会話ログ依存の handoff を正本にしない
- セッションの主成果物は「プレイアブルなコンテンツ」か「新機能」。UI修正だけのセッションは原則禁止
- **Yarn 執筆はユーザー (デザイナー/ライター) の仕事。AI はシステム・ツール・パイプラインを整備する**
- 機能検証は DebugQuickTest / EngineTestKit で行う。本編 Yarn を実験台にしない
- AI の判断は「前回の作業の反動」で振り子的に決めない。本当に必要なものを特定する

## Spec Status Semantics

- spec-index の `done` は「初期仕様の実装完了」を意味する。「改善の余地がない」「手を入れない」を意味しない
- done 済み仕様への改善・拡張は `docs/FEATURE_REGISTRY.md` に ENH-xxx として登録する
- テストが実装と乖離した場合、テストの期待値が旧仕様の名残でないか先に確認する。テストパスのために実装を変えない
- テストの EditMode / PlayMode 境界を守る。EditMode テストから PlayMode 専用 API (DontDestroyOnLoad 等) を呼ばない

## Prohibited Interpretations / Shortcuts

- Yarn の NextContentToken を遅延の LinkedTokenSource に混入させない (自動スキップバグの原因)
- session 14 nightshift パターンの繰り返し禁止: .cs だけ revert して .asset を放置しない
- UI バグ発見時に即修正しない。docs/UI_ISSUES.md に記録し一括処理
- done 済み仕様の改善を「仕様変更」として拒否しない。FEATURE_REGISTRY に受け容れる

## 運用ルール

- ユーザーが一度説明した非交渉条件は、同一ブロック内でここへ固定する
- `docs/DECISION_LOG.md` に決定理由を残し、ここ（INVARIANTS）には条件そのものを残す
