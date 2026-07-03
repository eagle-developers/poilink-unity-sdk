# Poilink SDK for Unity

Unity 向け Poilink SDK (iOS / Android 両対応)。ユーザ認証、WebPortal 表示、ミッション進捗管理、アカウント引き継ぎ機能を提供します。

ドキュメント: [https://docs.poilink.com/](https://docs.poilink.com/) (Unity SDK は `/unity/` 配下)

---

## 動作要件

| 項目 | バージョン |
|---|---|
| Unity Editor | 2020.3 以上 (推奨: 2022.3 LTS) |
| Scripting Backend | IL2CPP / Mono 両対応 |
| .NET Standard | 2.1 (C# 8.0 / async / await) |
| iOS Deployment Target | 15.0 以上 |
| iOS Xcode | 15.0 以上 |
| Android Min SDK | API 26 (Android 8.0) 以上 |
| Android Target SDK | 34 以上 (Google Play 公開要件に準拠) |
| Android Compile SDK | 34 以上推奨 |

### 動作対象プラットフォーム

| プラットフォーム | サポート | 備考 |
|---|---|---|
| iOS Player | ✅ | 実機 / シミュレータ両対応 |
| Android Player | ✅ | arm64-v8a / armeabi-v7a / x86_64 |
| Unity Editor (Mac / Windows / Linux) | ⚠️ Mock | モック応答を返します。本番動作の最終確認は必ず実機で |
| WebGL / Standalone | ❌ 非対応 | ネイティブライブラリ非対応 |

---

## インストール

Unity Editor の `Window > Package Manager` → `+` → `Add package from git URL` で以下を入力します。

```
https://github.com/eagle-developers/poilink-unity-sdk.git#1.0.0
```

または `Packages/manifest.json` を直接編集します。

```json
{
  "dependencies": {
    "com.poilink.sdk": "https://github.com/eagle-developers/poilink-unity-sdk.git#1.0.0"
  }
}
```

詳細なセットアップ手順 (PoilinkSettings 作成、Android / iOS の追加設定、ネットワーク要件等) は [ドキュメントサイト](https://docs.poilink.com/) を参照してください。

---

## ライセンス

[LICENSE.md](LICENSE.md) を参照してください。
