# CommEx

CommEx は、BVE と外部機器・アプリケーションをつなぐ AtsEX / BveEX Extension です。
Serial・UDP・TCP・MQTT・WebSocket を通して状態を取得し、仕様で許可された操作を受け付けます。

現在は製品仕様と検証基盤を整備している段階です。以下は初版の開発対象であり、
機能実装や全環境での動作確認が完了したことを意味しません。

## 目的と初版の範囲

- Communication.dll、BIDS ASCII v202、BIDS Binary、BveSerialOutput、NCIとの互換、およびJSONのNative v1。
- Nativeでの要求取得・選択購読・指定項目/全情報の周期送信。取得可能な公開API値を一覧化して管理する。
- 複数の同時接続、外部操作、許可番号への1回限りのPanel書込み。
- WinForms設定UI、環境別JSON、取込/書出し、車両→シナリオ→環境既定の設定切替。
- 接続ごとの自動接続/再接続、最大5回の再試行、状態表示と手動停止。
- 同一PC・信頼できるLANを対象とし、MQTTは外部ブローカー、WebSocketはクライアント/サーバー両対応。
- 設定・ログは %LocalAppData%/CommEx/ の環境別領域に保存する。ログは最大5本×10 MiB、全文通信ログは既定OFF。
- 依存DLLを極力減らし、単一DLLを理想とする。ただし依存DLLと自作DLLの結合は必須にしない。

正確な動作、既定値、対象外事項、未解消の判断は [製品仕様](specs/000-product/spec.md) を参照してください。
仕様はDraftであり、未確定の契約を推測して実装しません。

## 対応環境と開発要件

| 項目 | 基準 |
|---|---|
| OS | Windows 11 |
| BVE | 5.8.7554.391 / 6.0.7554.619 |
| ホスト | AtsEX 1.0.41005.1、BveEX 2.1.51225.1の通常/レガシーモード |
| 検証対象 | 上記BVE 2種類×ホスト3種類、計6組合せ |
| ランタイム / UI | .NET Framework 4.8 / WinForms |
| ビルド | Visual Studio付属MSBuild、.NET Framework 4.8用の開発環境 |
| コマンド環境 | Windows PowerShell |
| 外部検証 | 実Serialループバック、独立したTestPeer、LAN内MQTTブローカー、ホストUI自動化 |

ホストの準備方法と未検証状況は [互換性表](specs/000-product/compatibility.md) に記載しています。
不足するBVE 5等のパスはローカル設定へ追記します。未準備の環境を検証済みにはしません。
実行時依存やプロジェクト名の最終選定は、各featureの計画とライセンス確認で行います。

## リポジトリ構造

次の責任分担を [AGENTS.md](AGENTS.md) にも同じ内容で定義しています。
planned は未作成の実装領域です。空のプロジェクトや大量の雛形を作っただけで実装済みとは扱いません。

```text
CommEx/
|-- AGENTS.md                 # mandatory agent instructions
|-- README.md                 # human entry point
|-- .editorconfig             # shared C# formatting and naming preferences
|-- docs/                     # development and coding guides
|-- specs/                    # product and approved feature contracts
|-- .specify/                 # constitution, Spec Kit templates/scripts
|-- .agents/                  # repository-local agent skills
|-- config/                   # tracked examples; ignored machine-local configuration
|-- scripts/                  # build, test, deploy and orchestration entry points
|-- src/<project>/            # planned: production C# projects and logical modules
|-- tests/
|   |-- Unit/                 # planned: deterministic tests
|   |-- Integration/          # planned: real transport/resource tests
|   `-- BveE2E/               # planned: host automation and assertions
|-- tools/TestPeer/           # planned: independent external test peer
|-- reports/
|   |-- _templates/           # sanitized goal/progress/journal templates
|   `-- <work-id>/            # resumable records and verification reports
`-- artifacts/               # ignored build/debug/raw evidence
```

src/ 内はホストアダプター、Core、Codec/通信、設定、UI、診断等の責任で分けます。
フォルダ分割とDLL分割は同義ではありません。テスト補助実行プログラムは tools/TestPeer/、
製品の契約は specs/、共通の開発手順は docs/、再開記録・検証結果は reports/ に置きます。
新しい最上位フォルダを追加する際は、人間向けとagent向けの構造説明を同時更新します。
.gitignore と LICENSE はルートで管理し、生成物・秘密情報をソース領域へ混在させません。

## セットアップ

1. 必要なVisual Studio/MSBuild、.NET Framework 4.8の開発環境、対象ホストを準備します。
2. 既存のローカル設定を上書きせず、未作成の場合だけ設定例をコピーします。
3. 実パス、使用環境、試行上限等を config/repo.local.json に設定します。
4. 承認済み仕様・タスクと実際の検証環境を照合して作業を開始します。

```powershell
if (-not (Test-Path -LiteralPath config/repo.local.json)) {
    Copy-Item -LiteralPath config/repo.local.example.json -Destination config/repo.local.json
}
```

ローカル設定はGit管理外です。実パス、ユーザー名、認証情報を追跡対象の設定例へ転記しません。
リポジトリ外への配置・書込み先は、使用前にこのローカル設定で宣言してください。

既存スクリプト/設定例は単一環境向けで、solution/実装もまだ整っていません。
設定例と実際のプロジェクト名・出力先は基盤実装時に整合させます。
Spec Kitの設定は .specify/ と .agents/ にあります。既存リポジトリに対して初期化コマンドを
無条件で再実行せず、通常は既存のスキル/テンプレートを使用します。

## ビルド・配置・検証

```powershell
powershell -ExecutionPolicy Bypass -File scripts/build.ps1
powershell -ExecutionPolicy Bypass -File scripts/test-unit.ps1
powershell -ExecutionPolicy Bypass -File scripts/test-integration.ps1
powershell -ExecutionPolicy Bypass -File scripts/verify.ps1
```

verify.ps1 は Build → Unit → Integration → BVE E2E の順に実行します。
前段の失敗、必要な環境の不足、必須段階のスキップを成功と扱いません。
test-bve.ps1 は現在、未実装を明示して失敗するプレースホルダーです。

deploy-bve.ps1 は設定先へDLLを1ファイルコピーする補助です。実行前に成果物のコミット/ハッシュ、
対象環境、プロセス/ポートの所有、停止/アンロード、バックアップと復旧手順を確認します。
安全な配置制御や6環境切替が既に自動化されているとは扱いません。

[受入試験表](specs/000-product/acceptance.md) では物理Serialループバックと6環境のE2Eを必須とします。
固定負荷で10分運転し、平均FPS低下5%以内、操作受付から反映まで99%が100 ms以内、
終了処理5秒以内を要求します。接続数・送信量等の負荷条件は検証前に確定します。

## 自立した開発と再開

goal → 計画 → 実装 → ビルド → 解析/修正 → 再ビルド → 配置 → ログ取得/デバッグ → 修正を、
承認範囲と試行上限の中で繰り返します。詳細は [開発・再開ガイド](docs/development-workflow.md) に従います。

作業ごとに `reports/<work-id>/goal.md`、progress.md、journal.md を用意し、必要な細かな進捗を随時保存します。
新セッションではこれらと仕様/タスク、Git状態、配置状態を照合して再開します。
試行回数はセッションやsubagentを替えても引き継ぎます。形式は [記録ガイド](reports/README.md) と
[テンプレート](reports/_templates/progress.md) を参照してください。

read-onlyの調査・解析・レビューは適宜subagentへ委譲し、実装・テスト・文書も独立した範囲で
積極的に並行化します。書込み対象と共有リソースの担当を明確にし、coordinatorが結果を検証・統合します。

scripts/autonomous-loop.ps1 は **長期無人運用には未対応** です。現在は
specs/000-product/tasks.md を固定参照し、全変更の一括ステージ、完了時コミット、
呼出しごとに初期化される試行数など、新方針と合わない処理があります。
永続状態、承認/依存関係チェック、範囲を限定したコミット等の基盤タスクが完了するまで起動しません。
ガイドに従ってagentが作業することと、このスクリプトが完全な自律制御を提供することは別です。

## コーディング・文書・コミット規約

詳細は [C#コーディング規約](docs/coding-standards.md) と [.editorconfig](.editorconfig) に定義します。
エディター設定だけではXMLコメントを強制できないため、非公開メンバーを含めレビューでも確認します。

- C#はMicrosoftの標準的な規約を基準に、承認されたコンパイラ/.NET Framework 4.8の範囲で記述します。
- インデントは4スペース、タブ禁止。手書きの型・関数/メソッド・コンストラクター・プロパティ等には、
  非公開メンバーも含めXMLドキュメンテーションコメントを必ず記載します。
- 情報の追加/削除時は関連する本文・表・例・参照を再構成し、新しい内容が初めから存在したように整えます。
  矛盾する旧記述を追記で覆い隠しません。過去の証跡は履歴として区別して保持します。
- 意味のある最小単位が検証できたらその都度コミットします。1プロンプト1コミットではありません。
  関連する実装とテストや文書群はまとめ、独立した変更は分けます。
- コミットは機能の完成を意味しません。全必須検証の成功を確認するまでタスクを完了扱いにしません。

## 個人情報・機密情報と証跡

個人情報や機密情報はコミットしません。ユーザー名入りフルパス、個人の連絡先、秘密鍵、
トークン、認証付き接続文字列、ログ/画面に映った情報も対象です。
設定例・文書・コミットメッセージ・Gitの作者情報を含め確認し、公開可能な情報か匿名表現を使います。

生ログ/画面は除外済みの artifacts/ または `reports/<work-id>/logs`/ 等に保持します。
コミットするのは消毒済みの必要な要約と相対参照です。Gitignoreだけで安全と判断せず、
毎回ステージ対象を確認します。第三者バイナリやBVE配布物はコミットしません。

## 仕様と関連資料

- [製品仕様・未解消事項](specs/000-product/spec.md)
- [実装計画](specs/000-product/plan.md) / [タスク](specs/000-product/tasks.md)
- [互換性表](specs/000-product/compatibility.md) / [初期データ一覧](specs/000-product/data-catalog.md)
- [受入試験](specs/000-product/acceptance.md) / [検証記録の形式](reports/README.md)
- [エージェント指示](AGENTS.md) / [.specify/memory/constitution.md](.specify/memory/constitution.md)

仕様の優先順位はAGENTS.mdに従います。製品ライセンスは [LICENSE](LICENSE)、
依存物の再配布条件は各依存物のライセンスに従います。
