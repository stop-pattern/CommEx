# CommEx 通信拡張 仕様書（ドラフト）

- 最終更新: 2026-04-01
- ステータス: Draft v0.2

`docs/specification.md` は総合インデックスです。  
詳細仕様は機能ごとに分割し、以下へ整理しました。

## 目次（分割版）

1. [概要と全体アーキテクチャ](./spec/01-overview.md)
2. [パケット仕様](./spec/02-packet.md)
3. [UDP 仕様](./spec/03-udp.md)
4. [API サーバー仕様](./spec/04-api.md)
5. [NTP サーバー仕様](./spec/05-ntp.md)
6. [シリアル（COM）仕様](./spec/06-serial.md)
7. [設定画面要件](./spec/07-settings.md)
8. [スレッド設計](./spec/08-threading.md)
9. [実装ガイド（人間優先 + AI可読性補助）](./spec/09-implementation-guide.md)
10. [未確定事項・受け入れチェック](./spec/10-open-issues-and-checklist.md)
11. [Windows 環境構築・ビルド](./windows-build.md)

## 運用ルール

- 仕様追加・修正は、原則として該当する分割ファイルに対して行う。
- 機能横断の変更（例: パケット仕様変更）は、関連する全ファイルの整合性を同時更新する。
- 将来ツール連携（Codex / Copilot）を考慮し、用語を統一する（例: `req`, `res`, `n`）。
