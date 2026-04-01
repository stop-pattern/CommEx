# CommEx ドキュメント

このディレクトリは、CommEx の通信機能拡張に関する仕様をまとめるためのドキュメント置き場です。

## 目次

- [システム仕様（ドラフト）](./specification.md)
- [Windows 環境構築とビルド手順](./windows-build.md)

## 読み方

- **まず全体像を知りたい方**: `specification.md`（分割仕様への入口）→ `spec/01-overview.md`
- **実装担当の方**: `spec/02-packet.md` `spec/03-udp.md` `spec/04-api.md` `spec/05-ntp.md` `spec/06-serial.md` `spec/07-settings.md` `spec/08-threading.md`
- **将来の自動化・AI支援を想定する方**: `spec/09-implementation-guide.md`
- **ビルド担当の方**: `windows-build.md` の「1. 前提」「2. 環境構築」「3. ビルド」「4. 成果物」

---

> 本仕様は初期ドラフトです。`TODO` と記載された項目は、今後詳細化します。
