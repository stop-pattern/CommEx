using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommEx.Serial.Bids
{
    internal enum Errors
    {
        /// <summary>
        /// 原因不明エラー
        /// </summary>
        Unknown,
        /// <summary>
        /// コンバータとBIDSpp.dllとの間の接続が確立されていない
        /// </summary>
        NotConnected,
        /// <summary>
        /// 要求情報コードの数値部が不正
        /// </summary>
        ErrorInCodeNumber,
        /// <summary>
        /// 要求情報コードの記号部が不正
        /// </summary>
        ErrorInCodeSymbol,
        /// <summary>
        /// 識別子が不正
        /// </summary>
        ErrorInIdentifier,
        /// <summary>
        /// 数値変換がオーバーフローした
        /// </summary>
        Overflow,
        /// <summary>
        /// 要求情報コードの数値部に数値以外が混入している
        /// </summary>
        BadFormatInCode,
        /// <summary>
        /// 要求情報コードの数値部もしくは記号部が不正
        /// </summary>
        BadFormatCode,
        /// <summary>
        /// BVEのウィンドウハンドルを取得できない(キーイベント送信時)
        /// </summary>
        CantGetWindowHandle,
        /// <summary>
        /// (情報なし)
        /// </summary>
        NoInfo1,
        /// <summary>
        /// (情報なし)
        /// </summary>
        NoInfo2,
        /// <summary>
        /// (情報なし)
        /// </summary>
        NoInfo3,
        /// <summary>
        /// 配列の範囲外アクセス
        /// </summary>
        OutOfRange,
        /// <summary>
        /// シナリオが開始されていない
        /// </summary>
        NotStarted,
    }

    internal class BidsData
    {
        #region Field

        private static readonly Regex regex = new Regex(@"^(TR|EX)([RSPBKAIVE])([FNRUDPRCEHPSD]?)([+-]?\d+)$", RegexOptions.Multiline | RegexOptions.Compiled);

        private bool isError;
        private string header;
        private char identifier;
        private char info;
        private int code;
        private Errors error;
        private Type type;
        private object value;

        #endregion

        #region Properties

        /// <summary>
        /// エラーか否か
        /// </summary>
        public bool IsError
        {
            get;
            private set;
        }

        // TRxy000
        // 0012333
        // 
        // 0: ヘッダ
        // 1: 識別子
        // 2: 情報
        // 3: コード

        /// <summary>
        /// ヘッダ
        /// </summary>
        public string Header
        {
            get { return header; }
        }

        /// <summary>
        /// 識別子
        /// </summary>
        public char Identifier
        {
            get { return identifier; }
        }

        /// <summary>
        /// 情報
        /// </summary>
        public char Info
        {
            get { return info; }
        }

        /// <summary>
        /// コード
        /// </summary>
        public int Code
        {
            get { return code; }
        }

        /// <summary>
        /// エラー内容
        /// </summary>
        public Errors Error
        {
            get
            {
                return error;
            }
            }

        /// <summary>
        /// 返却値
        /// </summary>
        public object Value
        {
            get
            {
                return value;
        }
        }

        /// <summary>
        /// レスポンス
        /// </summary>
        public string Response
        {
            get
            {
                if (IsError)
                {
                    return header + "EX" + (int)error;
                }
                else
                {
                    string ret= "";
                    switch (type)
                    {
                        case Type te when te == typeof(Enum):
                        case Type tb when tb == typeof(bool):
                            ret = Convert.ToInt32(value).ToString();
                            break;
                        default:
                            ret = value.ToString();
                            break;
                    }
                    if (info == '\0')
                    {
                        return header + identifier + info + code + "X" + ret;
                    }
                    else
                    {
                        return header + identifier + code + "X" + ret;
                    }
                }
            }
        }

        #endregion

        #region Constructor

        private BidsData()
        {
            isError = true;
            header = "";
            identifier = '\0';
            info = '\0';
            code = 0;
            error = Errors.Unknown;
            value = 0;
        }

        public BidsData(string str) : this()
        {
            var matches = regex.Matches(str);

            if (matches.Count < 1)
            {
                return;
            }

            foreach (Match match in matches)
            {
                header = match.Groups[0].Value;
                identifier = match.Groups[1].Value[0];
                info = match.Groups[2].Value[0];

                if (int.TryParse(match.Groups[3].Value, out int result))
                {
                    code = result;
                }
                else if (int.TryParse(match.Groups[3].Value.Substring(1), out int res))
                {
                    code = res;

                    if (match.Groups[3].Value[0] == '+')
                    {
                        code *= 1;
                    }
                    else if (match.Groups[3].Value[0] == '-')
                    {
                        code *= -1;
                    }
                }
                else
                {
                    isError = true;
                    code = 0;
                    return;
                }
            }
            isError = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// パースを試みる
        /// </summary>
        /// <param name="str">入力文字列</param>
        /// <param name="data">パース後の構造体</param>
        /// <returns>パースできたか否か</returns>
        public static bool TryParse(string str, out BidsData data)
        {
            var matches = regex.Matches(str);
            data = new BidsData(str);
            return matches.Count > 0;
        }

        /// <summary>
        /// エラー情報を設定
        /// </summary>
        /// <param name="err">エラー情報</param>
        /// <returns>自身のインスタンス</returns>
        public BidsData SetError(Errors err)
        {
            IsError = true;
            error = err;
#if DEBUG
            Debug.WriteLine(err.ToString());
#endif
            return this;
        }

        public BidsData SetValue(bool val)
        {
            value = Convert.ToInt32(val);
            type = typeof(bool);
            return this;
        }

        /// <summary>
        /// 返却値を設定
        /// </summary>
        /// <param name="val">返却値</param>
        /// <returns>自身のインスタンス</returns>
        public BidsData SetValue(int val)
        {
            value = val;
            type = typeof(int);
            return this;
        }

        /// <summary>
        /// 返却値を設定
        /// </summary>
        /// <param name="val">返却値</param>
        /// <returns>自身のインスタンス</returns>
        public BidsData SetValue(float val)
        {
            value = val;
            type = typeof(float);
            return this;
        }

        /// <summary>
        /// 返却値を設定
        /// </summary>
        /// <param name="val">返却値</param>
        /// <returns>自身のインスタンス</returns>
        public BidsData SetValue(double val)
        {
            value = val;
            type = typeof(double);
            return this;
        }

        /// <summary>
        /// 返却値を設定
        /// </summary>
        /// <param name="val">返却値</param>
        /// <returns>自身のインスタンス</returns>
        public BidsData SetValue(Enum val)
        {
            value = Convert.ToInt32(val);
            type = typeof(int);
            return this;
        }

        #endregion
    }
}
