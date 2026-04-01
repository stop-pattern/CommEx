# 02. パケット仕様（通信方式別）

## 基本方針

- 各通信方式でパケット形式は**共通化しない**。
- UDP とシリアルは別仕様として管理する。
- API / NTP も、それぞれのプロトコル流儀で定義する。

---

## 2.1 UDP

- 現時点では **後日検討**。
- 本ドラフトでは、UDP のペイロード構造を固定しない。

### 暫定運用

- 送信対象データの列挙のみ先行し、実フレーム定義は保留。
- 互換性確保のため、UDP 仕様確定時にバージョン番号を付与する。

---

## 2.2 シリアル（COM）

シリアルは、次の 4 プロトコルから選択可能とする（将来拡張可）。

1. bids互換（ヘッダ `EX/TR`）
2. communication.dll互換
3. BveSerialOutput互換
4. バイナリ独自形式

### 2.2.1 bids互換

- `Tralsys/BIDSid_SerCon` の `CommandList` を準拠元にする。
- 受信/送信の主形式は `TR + 識別子 + 要求/返答 + 改行`。
- 互換性維持のため、`R/S/P/B/K/I/V/E` 識別子を受理対象とする。

### 2.2.2 communication.dll互換

- フレーム構造・詳細仕様は **後日検討**。
- 現段階ではプロトコル選択肢としてのみ提供。

### 2.2.3 BveSerialOutput互換

- `GraphTechKEN/SerialOutputEx` 実装を準拠元にする。
- 固定ヘッダ必須ではなく、設定（XML）で定義した各項目を連結した ASCII 電文を主対象とする。
- 改行は実装互換として `\r` または `\r\n` 系の終端を許容する。

### 2.2.4 バイナリ独自形式

- ASCII 文字列前提を置かず、バイト列として送受信する。
- フレーム構造・詳細仕様は別途検討する。

---

## 2.3 参照元

- BIDSid_SerCon: https://github.com/Tralsys/BIDSid_SerCon
- SerialOutputEx: https://github.com/GraphTechKEN/SerialOutputEx
