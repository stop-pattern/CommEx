# AGENTS.md

このリポジトリでは、MVVM 構成での実装を基本方針とします。

## アーキテクチャ方針
- `CommEx/Models`: 純粋なドメイン状態・設定値
- `CommEx/ViewModels`: UI/プラグイン制御向けの状態管理・ユースケース呼び出し
- `CommEx/Views`: Bve 側の表示連携や将来の UI アダプタ
- `CommEx/Services`: I/O や通信など副作用を伴う処理
- `CommEx/Infrastructure`: ロギングや時刻取得など横断的関心事

## 実装ルール
- ViewModel から直接静的 API を呼ばず、原則インターフェース経由で依存注入する。
- 例示コードを追加する際は、最小限でも `Model`, `ViewModel`, `Service` を対応させる。
- 設計変更時は `docs/` か README のどちらかに意図を追記する。
