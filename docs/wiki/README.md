# FoundPhone Authoring Wiki

本Wikiはポータルのみを保持する。内容ページは `docs/` 正典へ統合済み。

## 主要入口

- 執筆ガイド: `../SCENARIO_AUTHORING_GUIDE.md`
- 編集パイプライン: `../YarnEditingPipeline.md`
- オペレーター手順: `../OPERATOR_WORKFLOW.md`
- セーブ仕様: `../SaveSystem_README.md`
- UI 実装仕様: `../UI_IMPLEMENTATION_SPEC.md`

## 起動方法

```bash
# プロジェクトルートで実行
cd docs/wiki
npx docsify serve .

# ブラウザで http://localhost:3000 を開く
```

または Python:

```bash
cd docs/wiki
python -m http.server 3000
```

## 技術スタック

- **エンジン**: Unity 6000.4.9f1
- **スクリプト言語**: Yarn Spinner 3.1.3
- **エディタ**: VS Code + Yarn Spinner Extension
- **UI**: DOTween + TextMeshPro
