# CommEx

AtsEX / BveEX Extension として、BVE と外部機器・外部アプリケーションを
Serial / UDP / TCP / MQTT / WebSocket で接続するプラグインを開発しています。

2026-09-17/18 の対話で初版の製品方針を確定しました。現在は仕様・検証基盤の整備段階です。
通信機能や下記の対応環境が実装・検証済みであることを示すものではありません。

このリポジトリは、仕様確定後の機能分解、実装、ビルド、単体・統合・BVE E2E
テスト、修正、証跡保存を Codex に反復実行させることを前提にしています。

## 固定方針

- 対象: Windows 11 / BVE 5.8.7554.391・6.0.7554.619
- ホスト: AtsEX 1.0.41005.1、BveEX 2.1.51225.1 の通常・レガシーモード。計6組合せを検証対象とする
- ランタイム: .NET Framework 4.8
- UI: WinForms
- 正式なビルド経路: Visual Studio 付属 MSBuild
- 完了判定: `scripts/verify.ps1` の終了コード
- BVE のコールバックスレッドでは、ネットワーク・Serial・ファイル I/O を待機しない
- 実行時依存 DLL を極力減らし、単一 DLL の配布を理想とする。依存 DLL と自作 DLL の結合は必須にしない
- 利用範囲は同一 PC・信頼できる LAN。MQTT は外部ブローカーに接続し、WebSocket はクライアント・サーバー両対応

## 初版の合意事項

- Communication.dll、BIDS ASCII v202、BIDS Binary、BveSerialOutput、NCI の既存互換と、JSON の Native v1 を提供する。
- 通信路と形式を分離する。対応する組合せは[互換性表](specs/000-product/compatibility.md)で管理する。
- Native はデータとして扱える公開 API 値を対象に、要求取得・選択購読・指定項目／全情報の周期送信を提供する。
- 全接続から操作を受け付け、同じ対象では後に受け付けた操作を反映する。切断時はハンドルを維持し、切断元のキー押下を解放する。
- Panel 書込みは許可番号へ1回だけ適用する。その他の書込み対象は個別の仕様確定が必要。
- 環境別 JSON 設定を共通形式で取込・書出しする。設定は「車両→シナリオ→環境既定」の順で一式を選択する。
- 各接続に「自動接続」「再接続」の独立したチェックボックスを設け、新規設定ではともに OFF。再試行は最大5回で停止する。
- シナリオ切替時は旧接続・未処理入力・キー押下を整理し、新設定の自動接続を開始する。
- 不正設定は拒否する。妥当な設定は接続先が利用できなくても保存し、失敗した接続を停止・設定に従って再試行する。
- 設定・ログの保存先は `%LocalAppData%\CommEx\<environment-id>`。ログは最大5本×10 MiB、通信全文記録は既定 OFF。

詳細と未解消事項は[仕様](specs/000-product/spec.md)、実装順は[計画](specs/000-product/plan.md)、
未実装作業は[タスク](specs/000-product/tasks.md)を参照してください。
[データ一覧](specs/000-product/data-catalog.md)は初期一覧であり、全ホスト API の照合は未完了です。

## 初期セットアップ

管理者権限を必要としない PowerShell から実行します。

```powershell
uv tool install specify-cli
specify init . --integration codex --script ps --force
specify extension add bug
```

開発支援スキルを利用する場合も、承認済み feature 仕様・constitution・`AGENTS.md` の
優先順位に従います。機能の完成判定には `scripts/verify.ps1` と必要な受入証跡を用います。

次に `config/repo.local.example.json` を `config/repo.local.json` へコピーし、
BVE、AtsEX / BveEX、MSBuild、および E2E 用シナリオの実パスを設定してください。
既存の設定例・スクリプトは単一環境向けです。6環境の検証設定・ランナーへの拡張は
未完了タスクです。BVE 5 等の不足パスは後から追記し、未準備環境は検証済みと扱いません。
外部の設定保存先・ログ・検証用資材・配置先へツールが書き込む前に、そのパスをローカル設定へ宣言してください。

```powershell
Copy-Item config/repo.local.example.json config/repo.local.json
```

## 人間が仕様を確定する手順

1. `.specify/memory/constitution.md` を確認する。
2. [製品仕様](specs/000-product/spec.md)の合意事項と B01–B10 の残課題を確認する。
3. Spec Kit の specify / clarify / plan / tasks / analyze を実行する。
4. 各 feature に機械判定可能な Acceptance Criteria を設定する。
5. 対象機能に影響する残課題を解消し、その機能の仕様を承認してから自律実装を開始する。

互換バイト列、Native の詳細スキーマ、全ホスト API の対応付けなどを推測で実装しません。
製品方針への合意と個別機能の実装承認・検証完了は区別します。

## 検証

```powershell
powershell -ExecutionPolicy Bypass -File scripts/verify.ps1
```

レベルは Build → Unit → Integration → BVE E2E の順です。前段が失敗した場合、
後段は実行しません。E2E を明示的に無効化した実行は開発途中の確認には使えますが、
feature の完成証明にはなりません。現時点の E2E スクリプトは未実装を明示する失敗用の
プレースホルダーです。

[受入試験表](specs/000-product/acceptance.md)では、物理 Serial ループバックと6環境の E2E を必須とします。
固定負荷で10分間測定し、平均 FPS 低下5%以内、受信受付から操作反映まで99%が100 ms以内、
終了処理5秒以内を要求します。接続数・送信量等の固定負荷条件は、検証前に別途確定します。

## 自律実行

エージェントは、意味のある最小単位の変更を検証したら、その都度コミットします。
関連する実装とテスト、整合のため同時更新が必要な文書はまとめ、別目的の作業は分けます。
機能全体の完了やセッション終了までコミットを溜め込みません。コミット自体を機能の完成とは扱わず、
完了には従来どおり全必須検証を要求します。詳細は [AGENTS.md](AGENTS.md) の Commit policy for agents を参照してください。

```powershell
powershell -ExecutionPolicy Bypass -File scripts/autonomous-loop.ps1
```

このスクリプトは `specs/*/tasks.md` の未完了 feature を Codex に1件ずつ渡すための
入口です。最大試行回数、commit の可否、停止条件は設定ファイルで管理します。
自律運転前に、必ず手動で `verify.ps1` が期待どおり失敗・成功を返すことを確認してください。

## 証跡

各 feature の結果は `reports/<feature-id>/` に保存します。生成ログやスクリーンショットは
原則 Git 管理外です。要約と `result.json` のみを必要に応じて commit します。
