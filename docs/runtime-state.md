# Runtime State

**Updated**: 2026-03-27

## Current Position

- project: FoundPhone (UnityChatNovelGame)
- branch: main
- lane: Unlock (SO自動生成ツール実装)
- slice: Pipelineツール実装 (SO自動生成)
- active_artifact: FoundPhone エンジン基盤 (Unity 6.3 + Yarn Spinner 3.1.3)
- artifact_surface: Unity Editor > Tools > FoundPhone > Yarn SO Generator
- last_change_relation: direct (YarnSOGenerator.cs 新規作成)

## Counters

- blocks_since_user_visible_change: 0 (YarnSOGenerator = 新ツール)
- blocks_since_visual_audit: 3+
- blocks_since_unlock: 0 (YarnSOGenerator = Unlock)
- consecutive_excise_blocks: 0

## Quantitative Metrics

- impl_files: 67 (テスト除く .cs、ChatTextParser+BubbleSpriteFactory+YarnContentValidator追加、MVPScreenshotTest削除)
- test_files: 6 (EditMode 4 + PlayMode 2)
- playmode_test_files: 2 (MVPScreenshotEvidencePlayModeTests削除)
- mock_files: 0
- yarn_active: 9
- yarn_archive: 1
- spec_entries: 34 (done 22 / partial 9 / draft 1 / todo 2)
- todo_fixme_hack: 1 (ChatController.cs:1296)
- obsolete_marks: 2 (ContradictionPair.UnlockTopic x2)
- chatcontroller_lines: 2549 (session 10: 2753、-204行 / -7.4%)

## Visual Evidence

- visual_evidence_status: unknown
- last_visual_audit_path: (none)
- blocks_since_visual_audit: 1+

## Session Log

### 2026-03-27 session 12
- Block 1 (Advance): Pipeline設計確定 — 出力形態/プラットフォーム/自動化/サウンド/マネタイズの5決定
  - ゲームアプリ (モバイル優先 iOS/Android)
  - SO自動生成 + E2E自動検証
  - サウンド: コンテンツ後回し、マネタイズ: F2P+広告
  - project-context.md / CLAUDE.md / runtime-state.md 更新
- Block 2 (Unlock): YarnSOGenerator.cs 新規作成 (Editor ツール)
  - Yarnファイル走査 → 不足 TopicData / CharacterProfile 自動検出・生成
  - Tools > FoundPhone > Yarn SO Generator
  - YarnContentValidator のパース技術を転用
  - Unity コンパイル確認待ち

### 2026-03-26 session 11 nightshift
- Block 1 (Excise): MVPScreenshotEvidencePlayModeTests.cs 削除 + SetThreadHistories() 削除 + FEATURE_STATUS_AUDIT.md 数値修正
- Block 2 (Advance): ChatTextParser.cs 抽出 (ParseThreadMarkup/StripNamePrefix/CloseUnclosedRichTextTags)
- Block 3 (Excise): MessageTagged コマンド完全除去 (ScenarioManager + docs 8ファイル一括更新)
- Block 4 (Unlock): YarnContentValidator Editor ツール新規作成 (静的バリデーション)
- Block 5 (Advance): BubbleSpriteFactory.cs 抽出 (GetOrCreateRoundedSprite/CreateCircleSprite)
- ChatController.cs: 2753→2549行 (-204行、抽出2クラス: ChatTextParser + BubbleSpriteFactory)

### 2026-03-26 session 10 nightshift
- 全コードベース監査実施 (task-scout + Explore agent)
- docs/FEATURE_STATUS_AUDIT.md 作成: 実装26機能/未確認10/未実装15/懸念6
- レガシー19件削除: ~920行 + シーン2 + docs25ファイル
  - FragmentListUI chain (3), DeductionBoard verification chain (3), Dev/ (2)
  - MVP/ (4+scene), VerificationCapture+VerificationScene
  - docs/evidence/TASK_047,049, docs/archive/inbox,reports,tasks
- VerificationMenu.cs 修正: 削除シーン参照除去
- EditorBuildSettings: MVPScene除去
- spec-index.json: EN-010追加
