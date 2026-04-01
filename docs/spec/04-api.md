# 04. API サーバー仕様

## 要件

- BveEx API とネットワーク API を接続する
- BveEx 側の入出力をすべてエンドポイントとして公開する

## 設計方針

- 原則 1:1 でエンドポイント化
- 想定プロトコルは HTTP（詳細は別途確定）

## エンドポイント例（参考）

- `GET /api/input/{name}`: 入力値取得
- `POST /api/output/{name}`: 出力値設定
- `GET /api/health`: 生存確認
