# Windows 環境構築とビルド手順

- 最終更新: 2026-04-01
- 対象: Windows 環境

---

## 1. 前提

本リポジトリは .NET プロジェクト群（`*.csproj`）を含むソリューション（`BveExCsTemplate.sln`）で構成されています。  
ビルドは **sln を丸ごと投げて実行**する想定です。

対象ソリューション:

- `BveExCsTemplate.sln`

---

## 2. 環境構築（Windows + .NET）

## 2.1 必要ソフトウェア

- .NET SDK（Windows 版）
- （任意）Visual Studio 2022 以上、または VS Code + C# 拡張

> CLI だけでビルドする場合は .NET SDK があれば十分です。

## 2.2 インストール確認

PowerShell またはコマンドプロンプトで以下を実行します。

```powershell
dotnet --info
```

`dotnet` コマンドが認識され、SDK 情報が表示されれば準備完了です。

---

## 3. ビルド手順

## 3.1 リポジトリルートへ移動

```powershell
cd <このリポジトリのパス>
```

## 3.2 ソリューションをビルド

```powershell
dotnet build .\BveExCsTemplate.sln -c Release
```

- `-c Release` は配布向け最適化ビルドです。
- デバッグ用途では `-c Debug` でも可。

## 3.3 （必要に応じて）復元のみ先行

```powershell
dotnet restore .\BveExCsTemplate.sln
```

通常は `dotnet build` 内で自動実行されますが、ネットワークやキャッシュ事情で分けたい場合に使用します。

---

## 4. 成果物（DLL）の扱い

## 4.1 出力先の基本

ビルド成果物は各プロジェクト配下の `bin/<Configuration>/` に生成されます。

例:

- `Extension/bin/Release/`
- `MapPlugin/bin/Release/`
- `VehiclePlugin/bin/Release/`

## 4.2 配布対象のルール

配布対象は **このプロジェクトから生成された DLL のみ** とします。

- 含める: 本リポジトリのプロジェクトがビルドで生成した DLL
- 含めない: 参照先 NuGet パッケージ由来 DLL、COM 由来ファイル、ランタイム同梱物

## 4.3 実務上の判定方法（推奨）

以下の考え方で選別します。

1. 各プロジェクト名と同名（または対応する）DLL を優先して採用
2. `obj/` ではなく `bin/<Configuration>/` 側を採用
3. 外部依存 DLL（NuGet / COM / システム DLL）は除外

---

## 5. 参考コマンド（成果物確認）

PowerShell 例（Release ビルド後、プロジェクト生成 DLL を確認）:

```powershell
Get-ChildItem -Path .\Extension\bin\Release, .\MapPlugin\bin\Release, .\VehiclePlugin\bin\Release -Filter *.dll -Recurse |
  Select-Object FullName
```

> 最終的な配布物には、上記のうち「本プロジェクト生成 DLL」のみを含めてください。

---

## 6. トラブルシュート（簡易）

- `dotnet` が見つからない: .NET SDK のインストールと PATH を確認
- 復元エラー: ネットワーク接続、NuGet ソース設定、プロキシ設定を確認
- ビルド失敗: まず `dotnet restore` を単独実行して依存解決可否を切り分け

