# UnityChatNovelGame

Unity (C#) チャット/ビジュアルノベルゲーム。MVPアーキテクチャパターン。

## PROJECT CONTEXT

プロジェクト名: FoundPhone (UnityChatNovelGame)
環境: Unity 6.3 LTS (6000.3.6f1) / C# / Yarn Spinner 3.1.3 / DOTween
ブランチ戦略: trunk-based (main のみ)
フェーズ: プロトタイプ → α移行中
現在の状況 (2026-03-18 session 6 NIGHTSHIFT):
  - session 5 マージ統合完了 (コンフリクト5件解決)
  - docs大規模同期: 既知問題3件を修正済みに更新(YarnVariables CRITICAL/AutoSave/m_CurrentChannel)
  - SP-017解放トリガー仕様スタブ作成 + spec-indexエントリ追加 (26エントリ)
  - ETK declare集約修正、TransferSelectionUI.meta追跡、SSOT項目数修正(56→65)
  - spec-index: 26エントリ (done 16 / partial 5 / draft 3 / todo 2)
  - 次の作業: Unity Editor手動検証(65項目/約80分) + SP-019/020のHUMAN_AUTHORITY承認
  - 残存問題: UnreadCount復元(LOW), TransferFlagsクリア(HIGH), EndBranch CancellationToken(HIGH)

## DECISION LOG

| 日付 | 決定事項 | 選択肢 | 決定理由 |
|------|----------|--------|----------|
| 2026-03-11 | インベントリをダッシュボード内タブに統合 | 独立画面 / ダッシュボード内タブ | 既存DashboardControllerの拡張で最小工数。チャット画面からのオーバーレイアクセスも追加 |
| 2026-03-11 | TopicDataプレフィックス分類規約を策定 | fragment_ / record_ / topic_ / その他 | InventoryTabController.GetCategory()に集約。T_*/debug_*はTopicにフォールバック |
| 2026-03-11 | バブルスクロール制御をタイプライター限定ピンニングに変更 | 常時ピンニング / タイプライター限定 | 常時ピンニングはユーザーのスクロール操作を完全に阻害していた |
| 2026-03-11 | 選択肢色をUIConfig.choiceButtonColorに統一 | playerThemeColor / choiceButtonColor | playerThemeColorは鮮やかすぎ、選択肢は独立した控えめな色が適切 |
| 2026-03-12 | NPC名前:本文を改行分離 | 同一行「名前: 本文」/ 改行分離 | 折り返し時に本文開始位置で揃えるため。名前はmessageFontSize*0.75のボールド |
| 2026-03-12 | バブル幅計算をラッパー配置後に最終確定 | 配置前に確定 / 配置後にFinalize | 配置前だとHLG+アイコンによる幅変動で高さ不整合が発生する構造的問題 |
| 2026-03-12 | SystemMessage表示先の分岐準備 | 即実装 / ルーティングコメントのみ | ステータスバーUI新設は今は不要。将来の分離に備えて分岐ポイントのみ |
| 2026-03-12 | Debug Overlay デフォルトOFF | true / false | 通常表示と開発表示の同居で確認対象がぶれるため。Inspector ON で個別有効化可能 |
| 2026-03-12 | EndDay破壊的変更: 「ch{N}完了」→「現在チャンネルのDay N進捗記録」 | 許容/新コマンド追加 | マルチDayチャプター対応に必須。旧挙動はCurrentChannel未設定時にフォールバック |
| 2026-03-12 | 開発目的の明文化: エンジン基盤+ツール優先、コンテンツ執筆は検証範囲に限定 | N/A | 作業がコンテンツ方向にドリフトする傾向への対策。CLAUDE.mdにガードレール追加 |
| 2026-03-13 | Yarnディレクトリ分離: active/archive方式 | ディレクトリ分離 / ファイル名変更 / Editorツール | ファイル名変更はUnity+Gitで危険。ディレクトリ分離が最小侵襲。将来Editorツール化を予定 |
| 2026-03-13 | UnityEngine.Object派生型に??/?.を使わない | ??使用 / !=null三項演算子 | Unity operator==オーバーロードを??が迂回し、Inspector未設定のfake nullを透過する。全バブルで角丸が効いていなかった原因 |
| 2026-03-13 | SystemMessageに角丸スプライトを適用しない | 角丸適用 / 角丸除外 | 9-sliceのcornerRadius=16がバー高さに対して大きすぎ表示が圧迫される。ステータスインジケーターにバブル装飾は不適切 |
| 2026-03-13 | #line:タグはYarnプロジェクト内で一意でなければならない | タグ再利用 / ETK専用タグ | Yarn Spinnerが重複#line:でプロジェクト全体のコンパイルに失敗する。ETK用に独自ペア(etk_region_identity)を作成 |
| 2026-03-16 | スレッド切替UIをドロップダウン→左サイドバーに置換 | ドロップダウン維持 / 左サイドバー / 右サイドバー / ボトムシート | SP-016仕様(アイコントレイ)準拠。モバイル9:16で左スワイプ操作が自然。ThreadTypeグループ分類で視認性向上。ドロップダウンは小画面で見切れるリスクがあった |
| 2026-03-17 | SP-016 Step3 Phase3a: 型色ティント+A型カード表示 | 全型カード / ティントのみ / フルStep3 / 仕様先行 | 最小工数で「スレッドごとに見た目が違う」を実現。A型は仕様通りSystemMessage風カード。B/C/分岐はティントで差別化。Wiki/成果物カードは後送り |
| 2026-03-17 | スマホレスポンシブ: CRITICAL+HIGH 3件一括 | CRITICAL+HIGH / CRITICALのみ / エンジン優先 / 両方並行 | バブル幅(canvas幅<800→0.85) + サイドバー幅(max 40%占有) + フォントサイズ(canvas幅<900→縮小)。CanvasScaler MatchWidthOrHeight=1.0推奨は別途 |
| 2026-03-17 | SP-014 Branch Step2+: 全自動切替/自由切替/Yarn指定型反映 | 全自動/エントリ自動+復帰手動/両方手動, 自由切替/ロック, Yarn指定/自動生成/両方 | BeginBranch→自動切替、EndBranch→自動復帰。分岐中もサイドバーでMainに戻れる(自由切替)。反映メッセージはSetBranchReflectionでYarn作者が指定 |
| 2026-03-17 | SP-014 Phase 4: 知識転送選択UIは能動選択型(B型)を採用 | 受動表示+活用選択 / 能動選択型 / 保留(現行維持) | EndBranch時にプレイヤーが「何を持ち帰るか」を選択。戦略性が最も高い。$has_topic変数はtrue維持(知っているが見せないだけ) |
| 2026-03-18 | SP-017(解放トリガー)は独立仕様ではなくSP-099内の未決定事項として管理 | 独立仕様 / SP-099内管理 | 仕様ファイル・spec-index未登録のまま宙に浮いていた。Phase A検証後に独立化の要否を判断 |
| 2026-03-18 | ランタイム既知問題4件を即時修正 (6d87d3e) | 即修正 / Phase A後 / 保留 | $halluci_coin同期(CRITICAL)は即修正が妥当。TransferFlags/SelectionUI/矛盾AutoSaveも修正コストが低く即対応 |
| 2026-03-18 | Ch2にC型偵察スレッドを組み込み | C型追加 / ETKモックのみ / 保留 | Mason/Oliver の偵察設定と自然に接合。成果物カード+CompleteThreadの実チャプター検証が必要 |
| 2026-03-18 | 主人公裏切りUIはd)複合段階式を採用 | a色差異のみ / c時系列 / d複合 / 保留 | Ch6→a(色)、Ch7→c(タイムスタンプ)、Ch8→b(ログ改竄)の段階的不信感蓄積。MVP: Ch6でaのみ先行検証 |
| 2026-03-18 | コンテンツ量: 断片3/ch + スレッド2-3/ch | 2/ch / 3/ch / 4+/ch | 9章x3=27断片、9章x2.5=22スレッド。Ch1-2実績と整合し管理可能な規模 |
| 2026-03-18 | サブスレッド解放は複合トリガー方式 | HCのみ / ストーリーのみ / 複合 / 保留 | HC閾値+ストーリー進行+断片収集の3条件OR。既存エンジン機能(DeclareThreadLatentCond+ChannelData)で新規実装不要 |

## IDEA POOL

| ID | アイデア | 状態 | 関連領域 | 再訪トリガー |
|----|----------|------|----------|--------------|
| IP-001 | アルケミーボード (DeductionBoard) の再活性化 | hold | system/SP-014 | 矛盾メカニクスが3章以上で安定運用された時 |
| IP-002 | B型Wikiリンク遷移の本格実装 | hold | ui/SP-016 | B型追跡スレッドのコンテンツが3章分揃った時 |
| IP-003 | C型成果物カードのリッチ表示 (アイコン+インタラクション) | hold | ui/SP-016 | C型偵察スレッドのUnitiy手動検証完了後 |
| IP-004 | ProgressTracker Phase 2: チャプター間接続の可視化 | hold | ui/SP-018 | Ch3設計でチャプター間接続パターンが明確になった時 |
| IP-005 | オーサリングコマンド拡張: <<DeclareAndDiscover>> (宣言+発見一括) | hold | tooling | Ch3制作でDiscoverFragmentの使い勝手を評価した後 |
| IP-006 | 主人公裏切りUI Phase 1: テキスト色差異 (a) の先行実装 | hold | system/SP-099 | Ch5-6のシナリオ設計に着手した時 |

## DEVELOPMENT PURPOSE

本プロジェクトの目的は「ストーリーを最終話まで書くこと」ではなく、「最終的にストーリーを載せられるエンジン基盤とツールを開発すること」である。

### 開発サイクル

基盤開発 → 確認モック → デザイン変更発見 → ツール開発 → 基盤拡張

1. **基盤開発**: エンジン機能の実装（Day Resume、サブスレッド、セーブ/ロード拡張等）
2. **確認モック**: 基盤の動作確認に必要な最小限のYarnコンテンツを作成
3. **デザイン変更発見**: モックを動かして画面効果から逆算し、システム/アセットの境界を判断
4. **ツール開発**: デザイナーが効率的にコンテンツを載せるためのオーサリング支援
5. **基盤拡張**: 発見された要件をエンジンに反映し、サイクルを回す

### スコープ境界（ガードレール）

- コンテンツ執筆（Yarn台詞・ストーリー展開）は基盤検証に必要な範囲に限定する
- 「このYarnを書けば機能Xが検証できる」という目的が明示できない場合、コンテンツ作業に入らない
- コンテンツ量の目安: 1機能につき1-2ノードの検証モック。フルチャプター執筆は基盤完成後
- ストーリー設計（ビート表・キャラ設定）はSPEC FIRSTとして仕様化するが、実装フェーズでは基盤を優先する
- 作業がコンテンツ方向に流れていると感じたら、「この作業は基盤/ツールのどちらに貢献するか？」を自問し、ユーザーに確認する

### システム vs アセット の判断基準

画面効果の実現方法を検討する際:

- **システム（C#エンジン）に乗せるもの**: 複数箇所で再利用される / ランタイムで動的に変化する / セーブ/ロードに影響する
- **アセット（Yarn/ScriptableObject）で解決するもの**: コンテンツ固有 / デザイナーが直接編集する / 実行時に静的
- 判断に迷う場合はユーザーに選択肢を提示する（HUMAN_AUTHORITY）

## Key Paths

- Source: `Assets/Scripts/`
- Docs: `docs/`
- Specs: `docs/StorySpec/`, `docs/ENGINE_FEATURE_INVENTORY.md`
- Spec Index: `docs/spec-index.json`
- Yarn (active): `Assets/Resources/Yarn/active/`
- Yarn (archive): `Assets/Resources/Yarn/archive/`
- YarnProject: `Assets/Resources/Yarn/Project.yarnproject`
- Topics: `Assets/Resources/Topics/`
- Channels: `Assets/Resources/Channels/`
- Characters: `Assets/Resources/Characters/`

## Rules

- Respond in Japanese
- No emoji
- Do NOT read `docs/archive/` unless explicitly asked
- When exploring code, use Grep/Glob to locate symbols instead of reading entire .cs files
- Keep responses concise — avoid repeating file contents back to the user
