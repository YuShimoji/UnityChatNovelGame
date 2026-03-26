# Runtime State

**Updated**: 2026-03-26

## Current Position

- project: FoundPhone (UnityChatNovelGame)
- branch: main
- lane: Excise + Advance (リファクタリング + ツール追加)
- slice: 安定版整備 (デッドコード根絶 + リファクタリング + オーサリングツール)
- active_artifact: FoundPhone エンジン基盤 (Unity 6.3 + Yarn Spinner 3.1.3)
- artifact_surface: Unity Editor ContentAuthoring シーン再生
- last_change_relation: direct (BubbleSpriteFactory抽出 + YarnContentValidator新規)

## Counters

- blocks_since_user_visible_change: 0 (YarnContentValidator = 新ツール)
- blocks_since_visual_audit: 1+
- blocks_since_unlock: 0 (YarnContentValidator = Unlock)
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
