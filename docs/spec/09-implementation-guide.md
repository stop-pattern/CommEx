# 09. 実装ガイド（人間優先 + AI可読性補助）

## ドキュメント記述ルール

- 人が読みやすい自然言語を優先
- 厳密仕様は表・箇条書きで明確化
- 例示データは小さく具体的に示す

## AI 補助（Codex / Copilot）向け工夫

- 用語一貫性を保つ（例: `req`, `res`, `n`）
- パケット例を構文忠実に記載
- `TODO` を明示し未確定要素を抽出しやすくする

## 設定データ構造例（参考）

```yaml
transport:
  udp:
    enabled: true
    interfaces:
      - nic: "Ethernet0"
        local_ip: "192.168.1.10"
        destination_ip: "192.168.1.100"
  api:
    enabled: true
    port: 8080
  serial:
    enabled: true
    ports:
      - com: "COM3"
        protocol: "bids"
        baudrate: 115200
        databits: 8
        stopbits: 1
        parity: "none"
        dtr: true
        rts: false
        autostart: true
```
