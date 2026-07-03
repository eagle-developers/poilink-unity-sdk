# Changelog

このパッケージへの主要な変更点を記録します。

## [1.1.0] - 2026-07-03

### Changed
- `RewardReceiveCallback` に第4引数 `missionCode` を追加 — `(string grantId, string itemCode, int quantity, string missionCode)`。WebPortal のアイテム報酬受取通知で、対象ミッションのミッションコード（アプリ側マスタと対応するコード値）を受け取れるようになりました。旧シグネチャのコールバックはコンパイルエラーになるため、引数を1つ追加してください。

## [1.0.0] - 2026-05-20

初回公開リリース。

### Added
- `PoilinkSDK.Initialize()` / `InitializeAsync()` — SDK 初期化
- `PoilinkSDK.Authenticate()` / `Unauthenticate()` — ユーザ認証・セッション終了 (ユーザ切替時は `Unauthenticate()` → `Authenticate()` の 2-step が必須)
- `PoilinkSDK.SetRefreshToken()` / `GetRefreshToken()` — アカウント引き継ぎ用 RefreshToken の設定・取得
- `PoilinkSDK.ShowWebPortal()` / `CloseWebPortal()` / `PreloadWebPortal()` — WebPortal 表示制御 (FULLSCREEN / EMBEDDED 対応)
- `PoilinkSDK.ProgressMission()` / `ProgressMissionImmediate()` — ミッション進捗更新 (キュー版 / 即時版、`ProgressMissionMode.Increase` / `AtLeast` をモード引数で指定)
- `PoilinkSDK.GetMissionList()` — キャッシュからミッション一覧取得 (`MissionListFilter` によるフィルタ対応)
- iOS / Android ネイティブブリッジ
- EDM4U 依存定義 (`PoilinkSDKDependencies.xml`)
- Editor モック実装 (Mac / Windows / Linux エディタ向け)
- `PoilinkErrorCode` 列挙 (1001-1014)
