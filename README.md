# CommEx
[AtsEX](https://github.com/automatic9045/AtsEX)AtsEXを通してBveと外部で通信するプラグイン


## プラグインの機能
- [ ] シリアル通信
    - [ ] バイナリ
    - [ ] BIDS互換
    - [ ] SerialOutput互換
- [ ] ネットワーク
    - [ ] UDP


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

## このテンプレートの機能
- 取っ掛かりやすいように3種類のプラグインのファイル
    - マッププラグイン
    - 車両プラグイン
    - 拡張機能
- Actionsでのdll自動生成
- 頑張って書いた[wiki](../../wiki/)


## 動作環境
- [AtsEX](https://github.com/automatic9045/AtsEX)
    - [ver1.0-RC8 - v1.0.40401.1](https://github.com/automatic9045/AtsEX/releases/tag/v1.0.40401.1) or later
- Win10 22H2, Win11 22H2 or later
    - Visual Studio 2022
        - Microsoft Visual Studio Community 2022 (64 ビット) - Current Version 17.5.3
- [Bve](https://bvets.net/)
    - BVE Trainsim Version 6.0.7554.619


## 依存環境
- [AtsEx.CoreExtensions](https://www.nuget.org/packages/AtsEx.CoreExtensions/) (1.0.0-rc7)
- [AtsEx.PluginHost](https://www.nuget.org/packages/AtsEx.PluginHost/) (1.0.0-rc8)

間接参照を含めたすべての依存情報については、各プロジェクトのフォルダにある `packages.lock.json` をご確認ください。


## 使い方
1. Use this template から新しくリポジトリを作成する
1. githubリポジトリの詳細を設定する
2. LICENSEの著作権表記を変更する
1. 自分の作りたい機能に合わせて設定する
1. コードを書く
1. リリースする

### 0. 下準備
#### 0.1. テンプレートから新しくリポジトリを作成して設定する
1. `Use this template`ボタンから新しいリポジトリの作成画面に入る
    - `Create a new repository`で新しくリポジトリを作成する
    - リポジトリの名前はお好みで
    - Description にプラグインの概要とかを書いておくといい
1. リポジトリの設定画面でDescriptionやTopicsを設定する
1. LICENSEの著作権表記を変更する
1. README.md を消した後 README_TEMPLATE.md を README.md にリネームしてtodoを埋める

#### 0.2. ローカルにクローンする
1. `< > Code`からURLをコピーする
1. Visual Studio を開いて リポジトリのクローン からコピーしたURLでローカルにクローンする

できないときとかは下のコマンドでできる
```bash
git clone https://github.com/USERNAME/REPONAME.git
```

#### 0.3. Visual Studio でビルドできる状態にする
1. AtsExCsTemplate.csproj を開いてすべて保存から適当な場所にslnを生成する
1. NuGetからAtsEx関連のライブラリを入れる（ビルドすれば勝手に入る）
1. 開発するプラグインの種類に応じて要らないファイルを削除する
    - MapPlugin/
        - マッププラグイン用のプロジェクト
    - VehiclePlugin/
        - 車両プラグイン用のプロジェクト
    - Extension/
        - 拡張機能用のプロジェクト

#### 0.4. プラグイン情報の設定
**Properties/AssemblyInfo.cs**
BveからAtsExのバージョン情報を見たときに表示される内容を設定できます
AtsExのバージョン情報画面から見えるのはファイル名と下の3項目です

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

### 1. コードを書く
頑張ってゴリゴリ書きましょう

## 動作環境
- Windows
    - Win10 22H2
    - Win11 23H2 or later
- [Bve](https://bvets.net/)
    - BVE Trainsim Version 5.8.7554.391 or later
    - BVE Trainsim Version 6.0.7554.619 or later
- [AtsEX](https://github.com/automatic9045/AtsEX)
    - [ver1.0-RC9 - v1.0.40627.1](https://github.com/automatic9045/AtsEX/releases/tag/v1.0.40627.1) or later

### 3. 公開する
公開ができる状態になったらmainにpushしてtag打ってreleaseを作りましょう
<!-- tagを打つとciが走って自動でreleaseが作られビルド生成物が添付されます -->

## 開発環境
- [AtsEX](https://github.com/automatic9045/AtsEX)
    - [ver1.0-RC9 - v1.0.40627.1](https://github.com/automatic9045/AtsEX/releases/tag/v1.0.40627.1)
- Win10 22H2
    - Visual Studio 2022
        - Microsoft Visual Studio Community 2022 (64 ビット) - Current Version 17.5.3
- [Bve](https://bvets.net/)
    - BVE Trainsim Version 5.8.7554.391
    - BVE Trainsim Version 6.0.7554.619

## デバッグについて
※この項目に書いてあることは環境によって差異があるかもしれないので適宜自分の環境に合わせて読み替えること
### 1. 生成物がAtsExから読めるようにする
そのままの状態でビルドしてもデバッグできないのでBveからAtsEx経由でビルドしたプラグインが読み込めるようにする必要があります  
そのためには大きく次のA,Bで2通りのやり方があります  
おすすめはBのシンボリックリンク経由です  
シンボリックリンク経由だとpdbなどのごみがBve側のディレクトリに散らばったりしなくて嬉しいです  
#### 1.A. 生成物の出力パスをいじる
1. メニューバー > プロジェクト > (プロジェクト名)のプロパティ を選択しプロジェクトのプロパティ画面を開く
1. サイドバー > ビルド を選択しビルドの設定画面を開く
1. 出力セクションの出力パスをプラグインの出力先に設定する
1. 試しにビルドしてみて出力されるか確認する
#### 1.B.シンボリックリンクを張る
1. 生成物がない場合はビルドしてダミーのdllを生成する
1. 出力ディレクトリ(binの下)にある生成物へのシンボリックリンクをプラグインの配置場所に配置する
    - winでシンボリックリンクを簡単に張るには[Link Shell Extension](https://www.gigafree.net/system/explorer/hardlinkshellextension.html)がおすすめ
1. 試しにビルドしてみて更新されるか確認する
### 2. Visual Studioでデバッグする
#### 2.1. デバッグの設定をする
1. メニューバー > デバッグ > (プロジェクト名)のデバッグプロパティ を選択しプロジェクトのデバッグプロパティ画面を開く
1. 開始動作を"外部プログラムの開始"を選択しBveのパスを設定する
1. 必要があればコマンドライン引数にシナリオファイルのパスを設定する
    - ここでシナリオファイルのパスを設定したらそのシナリオが直接読み込まれる
    - 何も指定しなければ普通にシナリオ選択画面が立ち上がる
#### 2.2.実際にデバッグする
1. 適当にブレークとかを張る
1. デバッグを始める
    - F5キー
    - メニューバーのデバッグとテストの下あたりの開始ボタン（緑の三角形）
1. 張ったブレークで止まるか見てみる

## 依存環境
- AtsEx.CoreExtensions (1.0.0-rc9)
- AtsEx.PluginHost (1.0.0-rc9)

(開発者向け)  
間接参照を含めたすべての依存情報については、各プロジェクトのフォルダにある `packages.lock.json` をご確認ください。
