# CommEx
[AtsEX](https://github.com/automatic9045/AtsEX)AtsEXを通してBveと外部で通信するプラグイン


## プラグインの機能
- 未定


## 導入方法
### 1. AtsEXの導入
[公式のダウンロードページ](https://automatic9045.github.io/AtsEX/download/)を参照してください
### 2. 本プラグインの導入
1. [Releases](releases/)から最新版がダウンロードできます
1. AtsExの導入場所にある`Extensions`フォルダの中に本プラグインを配置します
    - デフォルト: `C:\Users\Public\Documents\AtsEx\1.0\Extensions`
    - プラグインはBveの起動と同時に読み込まれ、必要に応じて他のプラグインから利用されます
1. Bveから拡張機能が有効になっているか確認
    1. Bveを起動し右クリック
    1. **AtsEX バージョン情報・プラグイン一覧** を開く
    1. **CommEx** が有効になっていることを確認（デフォルトで有効）


## 使い方
未定


## ライセンス
- [MIT](LICENSE)
    - できること
        - 商用利用
        - 修正
        - 配布
        - 私的使用
    - ダメなこと
        - 著作者は一切責任を負わない
        - 本プラグインは無保証で提供される


## 動作環境
- Windows
    - Win10 22H2
    - Win11 23H2 or later
- [Bve](https://bvets.net/)
    - BVE Trainsim Version 5.8.7554.391 or later
    - BVE Trainsim Version 6.0.7554.619 or later
- [AtsEX](https://github.com/automatic9045/AtsEX)
    - [ver1.0-RC9 - v1.0.40627.1](https://github.com/automatic9045/AtsEX/releases/tag/v1.0.40627.1) or later


## 開発環境
- [AtsEX](https://github.com/automatic9045/AtsEX)
    - [ver1.0-RC9 - v1.0.40627.1](https://github.com/automatic9045/AtsEX/releases/tag/v1.0.40627.1)
- Win10 22H2
    - Visual Studio 2022
        - Microsoft Visual Studio Community 2022 (64 ビット) - Current Version 17.5.3
- [Bve](https://bvets.net/)
    - BVE Trainsim Version 5.8.7554.391
    - BVE Trainsim Version 6.0.7554.619


## 依存環境
- AtsEx.CoreExtensions (1.0.0-rc9)
- AtsEx.PluginHost (1.0.0-rc9)

(開発者向け)  
間接参照を含めたすべての依存情報については、各プロジェクトのフォルダにある `packages.lock.json` をご確認ください。
