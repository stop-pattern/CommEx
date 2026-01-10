# CommEx
[BveEX](https://github.com/automatic9045/BveEX)を通してBveと外部で通信するプラグイン


## プラグインの機能
- [ ] シリアル通信
    - [ ] バイナリ
    - [ ] BIDS互換
    - [ ] SerialOutput互換
- [ ] Ethernet
    - [ ] UDP


## 導入方法
### 1. BveEXの導入
[公式のダウンロードページ](https://bveex.okaoka-depot.com/download)を参照してください
### 2. 本プラグインの導入
1. [Releases](releases/)から最新版がダウンロードできます
1. BveEXの導入場所にある`Extensions`フォルダの中に本プラグインを配置します
    - デフォルト: `C:\Users\Public\Documents\BveEx\2.0\Extensions`
    - プラグインはBveの起動と同時に自動的に読み込まれます
1. Bveから拡張機能が有効になっているか確認
    1. Bveを起動し右クリック
    1. **BveEX バージョン情報・プラグイン一覧** を開く
    1. **CommEx** が有効になっていることを確認（デフォルトで有効）
        - 有効になっていない場合：プラグイン名を右クリックして有効化


## 使い方
Bve起動後に右クリックメニューから設定してください。

## ライセンス
- [MIT](LICENSE)
    - できること
        - 商用利用
        - 修正
        - 配布
        - 私的使用
    - ダメなこと
        - コントリビューターに何らかの責任を取らせる
        - コントリビューターに何らかの保証を求める

> [!CAUTION]
> 【重要】本プラグインは、**[BveEXのラインセンス](https://github.com/automatic9045/BveEX/blob/main/LICENSE.md)により商用利用できません**。

本プラグインの動作環境は、BveExの導入が前提となっています。
そのため、BveExが使用できない環境では使用することができません。
文化祭やイベント（NTほげほげ、Maker Faireなど）で使用する際は、BveEXのライセンスに配慮し利用してください。

なお、CommEx単体については、MITライセンスに従っている限り利用時の許可取りや報告は不要です。
（不要ではありますが、利用報告・採用例を見せていただけると嬉しいです。）


## 動作環境
- Windows
    - Win11 24H2 or later
- [Bve](https://bvets.net/)
    - BVE Trainsim Version 5.8.7554.391 or later
    - BVE Trainsim Version 6.0.7554.619 or later
- [BveEX](https://github.com/automatic9045/BveEX)
    - [ver2.1 - v2.1.51225.1](https://github.com/automatic9045/BveEX/releases/tag/v2.1.51225.1) or later


## 開発環境
- [BveEX](https://github.com/automatic9045/BveEX)
    - [ver2.1 - v2.1.51225.1](https://github.com/automatic9045/BveEX/releases/tag/v2.1.51225.1)
- Win11 24H2
    - Visual Studio 2026
        - Microsoft Visual Studio Community 2026 (64 ビット)
        - Version 18.11.1
- [Bve](https://bvets.net/)
    - BVE Trainsim Version 5.8.7554.391
    - BVE Trainsim Version 6.0.7554.619


## 依存環境
- BveEx.CoreExtensions (2.0.8)
- BveEX.PluginHost (>= 2.0.8)

### 開発者向け
間接参照を含めたすべての依存情報については、各プロジェクトのフォルダにある `packages.lock.json` をご確認ください。