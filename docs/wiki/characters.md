# キャラクター管理

## 登録済みキャラクター

| ID | 表示名 | 色 | 種別 | 表示モード |
|----|--------|-----|------|-----------|
| `player` | あなた | 紺 (0.2, 0.35, 0.55) | プレイヤー | 右寄せ、名前なし |
| `pyramid` | Pyramid | 緑 (0.55, 0.82, 0.6) | AI | IconOnly |
| `marco` | Marco Gross | オレンジ (0.9, 0.55, 0.3) | NPC | NameOnly |
| `bernardo` | Bernardo Fonseca | 紫 (0.55, 0.4, 0.75) | NPC | NameOnly |
| `mason` | Mason | 茶 (0.6, 0.5, 0.35) | NPC | NameOnly |
| `oliver` | Oliver | 青 (0.35, 0.65, 0.85) | NPC | NameOnly |
| `unknown` | 不明な連絡先 | グレー (0.85, 0.85, 0.85) | 不明 | NameOnly |

## Yarn での使い方

```yaml
<<set $speaker to "pyramid">>
こんにちは。   # pyramid の色・名前で表示

<<set $speaker to "player">>
やあ。          # プレイヤーの色で右寄せ表示
```

`$speaker` を変更するまで同じキャラクターが話し続けます。

## 新しいキャラクターを追加する

### 1. CharacterProfile を作成

Unity メニュー: `Create > Project FoundPhone > Character Profile`

### 2. 設定項目

| プロパティ | 説明 |
|-----------|------|
| CharacterID | Yarn で参照する一意のID (例: `detective_kim`) |
| DisplayName | UI に表示される名前 (例: `Kim 捜査官`) |
| Icon | アバター画像 (Sprite) |
| ThemeColor | バブル背景色 (白の9-Slice Spriteに乗算) |
| IsPlayer | `true` で右寄せ表示 |
| DisplayMode | NameOnly / IconOnly / IconAndName |

### 3. 配置

`Resources/Characters/` フォルダに保存。ファイル名は `Character_{ID}` を推奨。

### 4. 使用

```yaml
<<set $speaker to "detective_kim">>
捜査官のKimです。よろしく。
```

## 表示モード

| モード | 説明 |
|--------|------|
| NameOnly | バブル内にテキストで名前表示。デフォルト |
| IconOnly | バブル横にアイコン表示、名前省略 (Pyramid で使用) |
| IconAndName | アイコン + 名前の両方表示 |

## NPC バブルの名前表示

NPC のバブルでは名前行と本文行が改行で分離されます:

- 名前行: `messageFontSize * 0.75` のボールドフォント
- 本文行: `messageFontSize` の通常フォント
- Player バブルには名前を表示しない
