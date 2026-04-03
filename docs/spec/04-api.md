# 04. API サーバー仕様

## 概要
本APIは、BveEXを利用したBVEシミュレータに対し、外部アプリケーションから状態取得および操作を行うためのREST APIです。

- ベースURL: `/api/v1`
- データ形式: JSON
- 通信方式: HTTP (REST)

## 設計方針
- GET：状態取得
- PUT / PATCH：状態変更
- POST：瞬間的な操作（コマンド）

## 1. サーバー情報

### GET /api/v1
{
  "name": "BveExRestBridge",
  "apiVersion": "1.0"
}

### GET /api/v1/capabilities
{
  "vehicle": true,
  "time": true,
  "stations": true,
  "sections": true
}

## 2. 自列車
### GET /api/v1/vehicle
現在の車両状態を取得

### GET /api/v1/vehicle/location
現在の距離程を取得

### PATCH /api/v1/vehicle/location
指定距離程にジャンプ

### GET /api/v1/vehicle/state
現在の車両状態を取得

### GET /api/v1/vehicle/handles
ハンドル位置を取得

### PATCH /api/v1/vehicle/handles/driver
ハンドル位置を変更

### GET /api/v1/vehicle/doors
ドア状態を取得

### PATCH /api/v1/vehicle/doors
ドア状態を変更

## 3. パネル・サウンド
### GET /api/v1/vehicle/panel/{index}
panel[index]を取得

### PUT /api/v1/vehicle/panel/{index}
panel[index]を設定

### GET /api/v1/vehicle/sound/{index}
sound[index]を取得

### PUT /api/v1/vehicle/sound/{index}
sound[index]を設定

## 4. 時刻
### GET /api/v1/time
時刻を取得

### PUT /api/v1/time
時刻を設定

## 5. 閉塞
### GET /api/v1/sections
閉塞情報を取得

### GET /api/v1/sections/current
現在閉塞を取得

### PATCH /api/v1/sections/{index}/signal
sections[index]の信号を設定

## 6. 駅
### GET /api/v1/stations
駅リストを取得

### GET /api/v1/stations/{index}
駅を取得

### PATCH /api/v1/stations/{index}
駅を設定

## 7. 入力
### POST /api/v1/commands/keys/{key}/press
キー押下イベントを発生

### POST /api/v1/commands/keys/{key}/release
キー解放イベントを発生

## 8. イベント
### GET /api/v1/events?since={cursor}

## 9. Raw API
### GET /api/v1/raw/{path}
生の値を取得

### PATCH /api/v1/raw/{path}
生の値を設定

## 10. エラー

{
  "error": {
    "code": "INVALID_REQUEST",
    "message": "Invalid parameter"
  }
}
