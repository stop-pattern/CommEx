# C# コーディング規約

CommEx の手書き C# コードには、Microsoft の
[Common C# code conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
を基本として適用します。本書の必須ルールを優先し、承認済み仕様、constitution、
[AGENTS.md](../AGENTS.md) の制約に従います。テストと検証ツールの C# コードも対象です。

Microsoft の資料は最新言語のサンプルを含むため、記載された構文を無条件に採用しません。
対象は .NET Framework 4.8、設定 UI は WinForms、ビルドは設定された Visual Studio MSBuild です。
プロジェクト作成時にコンパイラーと言語バージョンを確認し、承認された構成で利用できる構文・API を使います。
file-scoped namespace、nullable reference types、records などの採用を本書から要求しません。
`LangVersion=latest` や `preview` を規約適用のために導入しません。
Microsoft も再現性のために `latest` を避けるよう
[説明しています](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/configure-language-version)。

## ソースコードのフォルダ構成

MicrosoftのC#/.NETの規約とプロジェクト分割例に従い、プロジェクト単位のディレクトリ、
名前空間に対応するサブフォルダ、型名に対応するファイル名で整理します。
C#の言語仕様が唯一のリポジトリ配置を定めているわけではないため、次の配置をCommExの標準とします。
根拠はMicrosoftの [名前空間の説明](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/namespaces) と
[プロジェクトとテストの整理例](https://learn.microsoft.com/en-us/dotnet/core/tutorials/testing-with-cli) です。

| 対象 | 配置・命名 |
|---|---|
| ソリューション | ルートの `CommEx.slnx`。VS Codeも同じルートとプロジェクトを参照する |
| 製品プロジェクト | `src/<ProjectName>/<ProjectName>.csproj`。現在は `src/CommEx/CommEx.csproj` |
| 手書きの型 | 原則1ファイル1主要型とし、`TypeName.cs` に置く。型名・名前空間・C#のサブフォルダ名はPascalCase |
| プロジェクト内部 | 実装する責任に応じて `Host`、`Core`、`Codecs`、`Communication`、`Configuration`、`UI`、`Diagnostics` へ整理する |
| 名前空間 | プロジェクトのルート名前空間と相対フォルダを対応させる。例: `Codecs/ExampleCodec.cs` は `CommEx.Codecs.ExampleCodec` |
| メタデータ | 現在の明示的なアセンブリ属性は `Properties/AssemblyInfo.cs` に置く |
| WinForms | 手書きの `ExampleForm.cs`、生成される `ExampleForm.Designer.cs`、`ExampleForm.resx` は同じフォルダに置く |
| テスト | `tests/Unit/<ProjectName>.Tests/`、`tests/Integration/<ProjectName>.IntegrationTests/`、`tests/BveE2E/` の責任別に配置する。未作成のプロジェクトは計画として扱う |
| 出力・補助 | `bin/`、`obj/` は生成物。実行スクリプトは `scripts/`、独立テスト相手は `tools/TestPeer/` に置く |

現在の `CommExMain.cs` はプロジェクト直下の `CommEx.CommExMain` として対応しています。
この承認済みのエントリー型を配置のためだけに改名しません。将来の責任別サブフォルダは
実装時に追加し、空のフォルダや空プロジェクトを先に量産しません。partial型、ネストされた補助型、
アセンブリ属性、生成ファイルは1主要型1ファイルの例外です。PowerShellから単独コンパイルする
`tests/BveE2E/ProcessFileMappings.cs` の `CommEx.Testing` はテスト補助専用の名前空間です。
責任別フォルダの分割はDLLの分割を意味しません。新規プロジェクトの境界と依存関係は承認済み計画に従います。

## 書式と命名

- インデントは必ず半角スペース4個とし、タブ文字を使いません。
- 波括弧 `{` と `}` はそれぞれ独立した行に置く Allman スタイルとします。
- 1行に1文・1宣言を基本とし、長い式は意味が読み取れる位置で改行します。
- 型、メソッド、プロパティ、イベントは `PascalCase`、インターフェイスは `I` 接頭辞付きの
  `PascalCase`、引数とローカル変数は `camelCase` とします。意味を表す名前を使います。
- `var` は右辺から型が明らかな場合に使います。名前だけに型情報を持たせません。
- `using` 指令は名前空間宣言の外に置きます。実装上の理由を説明する通常コメントには `//` を使います。

外部 API の override・interface 実装など、既存契約に名前を合わせる必要がある宣言はその契約に従います。
規約に合わせるためにプロトコル識別子や公開契約を変更しません。

## XML ドキュメントコメントは必須

アクセシビリティにかかわらず、手書きの型とメンバーに `///` XML ドキュメントコメントを書きます。
`public` だけでなく `internal`、`protected`、`private`、その組合せも対象です。
クラス、構造体、インターフェイス、列挙型と列挙値、デリゲート、メソッド、コンストラクター、
プロパティ、インデクサー、イベント、フィールド、定数、演算子などの宣言を含みます。
ローカル関数にも XML ドキュメントコメントを必須とします。採用するコンパイラーがローカル関数の構文で
XML ドキュメントコメントを受け付けない場合は、XML ドキュメントコメントを付けた private メソッドにします。
実装を説明する通常コメントは、XML ドキュメントコメントの代わりにはなりません。

| タグ | 記述する内容 |
|---|---|
| `<summary>` | 必須。何を表す型か、何を行うメンバーかを意味のある文章で説明する |
| `<param name="...">` | 各引数の役割、単位、許容範囲、必要な前提条件 |
| `<typeparam name="...">` | 各型引数の役割と制約の意味 |
| `<returns>` | 戻り値がある場合の意味。非同期の場合は完了と結果の意味も記述する |
| `<value>` | プロパティやインデクサーの値の意味、単位、適用条件 |
| `<exception cref="...">` | 呼び出し側に公開する例外と、その発生条件 |
| `<remarks>` | 必要に応じてスレッド、所有権、キャンセル、破棄、順序などの補足契約 |

該当するタグを使い、存在しない引数・戻り値・例外を記載しません。空のタグ、名前をそのまま言い換えた
コメント、実装と一致しない説明は要件を満たしません。変更時にはコメントも更新します。
本規約の全アクセシビリティへの必須化は、Microsoft の公開 API 向け推奨を拡張したプロジェクト固有ルールです。
[Microsoft の XML タグ資料](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/recommended-tags)
も非公開メンバーの XML コメントを扱っています。

例えば、次のように private メソッドでも契約を説明します。

```csharp
/// <summary>
/// 指定された範囲に値が含まれるかを判定します。
/// </summary>
/// <param name="value">判定する値。</param>
/// <param name="minimum">範囲の下限。この値を含みます。</param>
/// <param name="maximum">範囲の上限。この値を含みます。</param>
/// <returns>下限以上かつ上限以下の場合は true、それ以外は false。</returns>
private static bool IsWithinRange(int value, int minimum, int maximum)
{
    return value >= minimum && value <= maximum;
}
```

生成ツールが所有するファイルは、生成元または partial 型の手書き部分で説明を管理します。
生成コードを直接編集してコメントを失わせたり、手書きコードを生成扱いにして要件を回避したりしません。
名前空間宣言など XML ドキュメントコメントの適用対象でない構文に、不正な `///` を付けません。

## 確認方法と適用範囲

[.editorconfig](../.editorconfig) は C# のスペース4個、波括弧の配置、基本的な命名を共有します。
Microsoft の [C# 書式設定](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/csharp-formatting-options)
と [命名規則](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/naming-rules)
にある設定を使用します。実際に利用できる IDE 診断・ビルド診断は採用するツールチェーンによります。

このファイルだけで XML コメントの存在や内容をコンパイラーが強制するわけではありません。
XML 出力を有効にしたときの
[CS1591](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/cs1591)
は公開される型・メンバーのコメント欠落を検出しますが、private/internal を含む本規約全体の検証には不十分です。
各変更のレビューで手書き宣言と該当タグ、コメントの意味、非公開メンバーも確認します。
自動検証を追加する場合は別の検証可能な変更として実装し、必要な依存関係とツールの選定に従います。

`src/CommEx/CommEx.csproj` は C# 7.3 / .NET Framework 4.8 を対象とし、XMLドキュメントを出力します。
現時点では通信処理を持たない [最小BveEX Extension](../specs/001-host-bootstrap/spec.md) です。
コンストラクター・Tick・DisposeはI/Oや状態変更を追加せず、所有するリソースもありません。
ビルドとオフライン契約検査だけでは、非公開メンバーを含む規約検査や実ホストの受入検証を完了した扱いにしません。
実装時の検証と完了判定には [AGENTS.md](../AGENTS.md) に定めた手順を使用します。
