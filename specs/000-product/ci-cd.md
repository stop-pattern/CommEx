# GitHub管理・CI/CD仕様

状態: 2026-09-18に提示された2つのワークフローの要件を記録（総数は同日の訂正により2つと確定）。
この文書は仕様であり、GitHub Actionsの実装・実行・配布が完了したことを示さない。

## 管理と成果物

- リポジトリのリモート管理にGitHubを使用し、GitHub Actionsでコードチェックとビルド・コンパイルを実行する。
- 成功した実行の生成物にはプラグインDLLを含め、権限を持つ利用者がGitHubからダウンロードできるようにする。
- ビルド対象は.NET Framework 4.8とし、リポジトリのビルドスクリプトを入口にする。
  CI用の環境設定に個人のローカル設定・パス・秘密情報を含めない。
- 添付対象・依存DLL・ホスト別構成・ライセンスは[FR-019/B09](spec.md)に従う。
  BVE/AtsEX/BveEXランタイムを成果物として再配布しない。

## ワークフロー

| ID | トリガー | 実行内容 | 成果物の扱い |
|---|---|---|---|
| CI-01 | ブランチへのpush | 対象コミットのコードチェック、ビルド・コンパイルを実行し、成功した生成物をアップロードする | Actions artifactとしてDLLをダウンロード可能にし、保管期間を1日に設定する |
| CI-02 | tagのpush | タグが指すコミットについてCI-01と同じチェック・ビルド・成果物アップロードを実行し、成功後にそのタグのGitHub Releaseをdraftで生成して生成物を添付する | Actions artifactは同じ1日保管。Releaseには同じビルドの生成物をassetとして添付する |

CI-02の「CI-01と同じ処理」は共通のチェック・ビルド手順を意味する。別のブランチpush実行の成功や
最新成果物を代用せず、タグが指すコミットを検証する。YAMLファイル数やジョブ分割は実装計画で決める。
コードチェックまたはビルドに失敗した場合は成功した配布成果物として扱わず、ドラフト作成へ進めない。
添付に失敗した場合もワークフローを成功扱いにしない。

短期保管はActions artifactに適用する。`actions/upload-artifact`の`retention-days`は最小1日のため、
「1日以下」は`retention-days: 1`で指定する。`0`（既定値の使用）や未指定にはしない。
Release assetは別の保存先であり、短期artifactへのリンクだけで添付を代替しない。
Release assetを1日で削除する要件はない。

## ドラフトと正式公開

CI-02の到達点は生成物を添付した`draft: true`のReleaseであり、自動公開は含まない。
ドラフトへのアクセスにはGitHub側の権限が必要であり、一般向けの公開ダウンロード開始を意味しない。
公開版の配布には引き続き[製品の完了条件](spec.md#product-completion)と
[AC-14](acceptance.md)を適用する。CIのチェック・コンパイル成功やドラフト作成だけで、
BVE E2E・実機Serial・対応ホストの検証を完了したことにしない。

## CI/CDの受入条件

| ID | 観測する条件 | 必要な証跡 |
|---|---|---|
| CI-AC-01 | ブランチpushで対象コミットのチェックとビルドが成功し、ダウンロードしたartifactにDLLが含まれる | push/run/commitの対応、各工程の結果、取得した成果物の一覧とハッシュ |
| CI-AC-02 | CI-01・CI-02のActions artifactが1日保管でアップロードされる | ワークフローの設定とartifactの作成・期限情報 |
| CI-AC-03 | tag pushでタグのコミットをチェック・ビルド後、そのタグのdraft releaseに生成物を添付できる | tag/run/commitの対応、`draft: true`、添付一覧とビルド成果物・取得assetのハッシュ一致 |
| CI-AC-04 | チェックまたはビルドの失敗時にドラフト作成へ進まず、アップロード・添付失敗時に成功を報告しない | 意図的な失敗を用いた実行結果と後続工程・Release状態 |
| CI-AC-05 | CI-02がReleaseを公開せず、製品検証の未実施段階を成功扱いにしない | 公開状態、CI結果と製品受入結果の区別 |

## 実装前に確定する事項

- 「コードチェック」の具体的なコマンド、失敗基準、テストの適用範囲を実装計画で明記する。
  [コーディング規約](../../docs/coding-standards.md)と既存の受入条件を弱めない。
- ランナー・ビルド構成・依存物の取得・添付ファイルの選定を明記する。ホスト別配布物の未確定事項はB09で追跡する。
- 同じタグで再実行した場合の既存draft/assetの扱いを確定する。既存Releaseや添付物の削除・上書きを暗黙に選ばない。

## 参照

- [GitHub公式 upload-artifact: retention-days](https://github.com/actions/upload-artifact#retention-period)
- [GitHub公式: Releaseの管理](https://docs.github.com/en/repositories/releasing-projects-on-github/managing-releases-in-a-repository)
- [GitHub公式: Releases API](https://docs.github.com/en/rest/releases/releases)
- [GitHub公式: Release assets API](https://docs.github.com/en/rest/releases/assets)
