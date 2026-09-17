# BveCommPlugin

BveEX Extension として動作し、BVE と外部機器・外部アプリケーションの間を
UDP/TCP/Serial で接続するプラグインです。

このリポジトリは、仕様確定後の機能分解、実装、ビルド、単体・統合・BVE E2E
テスト、修正、証跡保存を Codex に反復実行させることを前提にしています。

## 固定方針

- 対象: Windows 11 / BVE Trainsim / BveEX
- ランタイム: .NET Framework 4.8
- UI: WinForms
- 正式なビルド経路: Visual Studio 付属 MSBuild
- 完了判定: `scripts/verify.ps1` の終了コード
- BVE のコールバックスレッドでは、ネットワーク・Serial・ファイル I/O を待機しない
- 実行時依存 DLL は可能な限り減らす。ただし「単一 DLL」は絶対条件ではなく、保守性と安全性を優先する

## 初期セットアップ

管理者権限を必要としない PowerShell から実行します。

```powershell
uv tool install specify-cli
specify init . --integration codex --script ps --force
specify extension add bug
```

Superpowers は、Spec Kit 単体で運用確認した後に導入します。導入する場合も、
本リポジトリの `AGENTS.md` と `scripts/verify.ps1` を最上位の制約・完了判定とします。

次に `config/repo.local.example.json` を `config/repo.local.json` へコピーし、
BVE、BveEX、MSBuild、および E2E 用シナリオの実パスを設定してください。

```powershell
Copy-Item config/repo.local.example.json config/repo.local.json
```

## 人間が仕様を確定する手順

1. `.specify/memory/constitution.md` を確認する。
2. `specs/000-product/spec.md` の未確定事項を決める。
3. Spec Kit の specify / clarify / plan / tasks / analyze を実行する。
4. 各 feature に機械判定可能な Acceptance Criteria を設定する。
5. 仕様を承認してから自律実装を開始する。

## 検証

```powershell
powershell -ExecutionPolicy Bypass -File scripts/verify.ps1
```

レベルは Build → Unit → Integration → BVE E2E の順です。前段が失敗した場合、
後段は実行しません。E2E を明示的に無効化した実行は開発途中の確認には使えますが、
feature の完成証明にはなりません。

## 自律実行

```powershell
powershell -ExecutionPolicy Bypass -File scripts/autonomous-loop.ps1
```

このスクリプトは `specs/*/tasks.md` の未完了 feature を Codex に1件ずつ渡すための
入口です。最大試行回数、commit の可否、停止条件は設定ファイルで管理します。
自律運転前に、必ず手動で `verify.ps1` が期待どおり失敗・成功を返すことを確認してください。

## 証跡

各 feature の結果は `reports/<feature-id>/` に保存します。生成ログやスクリーンショットは
原則 Git 管理外です。要約と `result.json` のみを必要に応じて commit します。

