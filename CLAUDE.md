# UnityChatNovelGame

Unity (C#) チャット/ビジュアルノベルゲーム。MVPアーキテクチャパターン。

## PROJECT CONTEXT

**正典:** いまのレーン・次スライス・数値の詳細は [`docs/project-context.md`](docs/project-context.md) と [`docs/HANDOFF.md`](docs/HANDOFF.md)。アシスタントの読書順のみ [`docs/ai/READ_ORDER.md`](docs/ai/READ_ORDER.md) に一本化している。表形式の決定履歴は必要時に [`docs/DECISION_LOG.md`](docs/DECISION_LOG.md)（毎回読む必要はない）。

- プロジェクト名: FoundPhone (UnityChatNovelGame)
- 環境: Unity 6.3 LTS (6000.3.6f1) / C# / Yarn Spinner 3.1.3 / DOTween
- ブランチ戦略: trunk-based (main のみ) / フェーズ: プロトタイプ → α移行中
- 技術サマリ（2026-04-10 時点・検証は正典へ）: YarnSOGenerator + Content Pipeline + HasNode 事前チェック + batch XML 出力。EditMode 75/75、PlayMode 8 件（実機・CI は好機に）。spec-index 39 エントリ。FEATURE_REGISTRY で ENH 系を管理

## IDEA POOL

| ID | アイデア | 状態 | 関連領域 | 再訪トリガー |
|----|----------|------|----------|--------------|
| IP-001 | アルケミーボード (DeductionBoard) の再活性化 | hold | system/SP-014 | 矛盾メカニクスが3章以上で安定運用された時 |
| IP-002 | B型Wikiリンク遷移の本格実装 | hold | ui/SP-016 | B型追跡スレッドのコンテンツが3章分揃った時 |
| IP-003 | C型成果物カードのリッチ表示 (アイコン+インタラクション) | hold | ui/SP-016 | C型偵察スレッドのUnity手動検証完了後 |
| IP-004 | ProgressTracker Phase 2: チャプター間接続の可視化 | hold | ui/SP-018 | Ch3設計でチャプター間接続パターンが明確になった時 |
| IP-005 | オーサリングコマンド拡張: <<DeclareAndDiscover>> (宣言+発見一括) | hold | tooling | Ch3制作でDiscoverFragmentの使い勝手を評価した後 |
| IP-006 | 主人公裏切りUI Phase 1: テキスト色差異 (a) の先行実装 | hold | system/SP-099 | Ch5-6のシナリオ設計に着手した時 |
| IP-PC-001 | メッセージごとのポートレート画像挿入 | active | ui/新機能 | Unity実機確認完了後。HUMAN_AUTHORITY: インラインアバター拡大 or 独立画像バブル or カットイン？ |
| IP-PC-002 | スレッド管理のシンプル化リファクタリング | active | system/リファクタ | PLAN MODE で設計後。BeginBranch/EndBranch/SwitchToThread の責務整理 |
| IP-PC-003 | StartWait 中のタップスキップ対応 | backlog | system/演出 | 現在は RunLineAsync 内の遅延のみ。StartWait のスキップも要検討 |

## DEVELOPMENT PURPOSE

本プロジェクトの目的は「ストーリーを最終話まで書くこと」ではなく、「最終的にストーリーを載せられるエンジン基盤とツールを開発すること」である。

**運用優先と本節の関係:** 「いま何を優先するか」の実行計画は常に `docs/project-context.md` の `CURRENT DEVELOPMENT AXIS` / `CURRENT LANE` / `CURRENT SLICE` に従う。本節以下のガードレールは、それに上乗せされる**常時制約**（無目的なフル執筆の抑制など）である。

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
- Docs: `docs/`（アシスタント読書順: `docs/ai/READ_ORDER.md`）
- Specs: `docs/StorySpec/`, `docs/ENGINE_FEATURE_INVENTORY.md`
- Spec Index: `docs/spec-index.json`
- 決定ログ（表）: `docs/DECISION_LOG.md`
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
