# FoundPhone Authoring Wiki

FoundPhone のシナリオ執筆・コンテンツ制作のための統合リファレンスです。

## このWikiでできること

- 新しいチャプター/ストーリーの追加手順を学ぶ
- 全 Yarn コマンドのリファレンスを参照する
- キャラクター、スレッド、分岐の設計パターンを知る
- エディタツールの使い方を理解する

## すぐに始める

> **[Quick Start: ストーリーを追加する](quick-start.md)** -- 5分で最初のシーンを動かす

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

- **エンジン**: Unity 6.3 LTS (6000.3.6f1)
- **スクリプト言語**: Yarn Spinner 3.1.3
- **エディタ**: VS Code + Yarn Spinner Extension
- **UI**: DOTween + TextMeshPro
