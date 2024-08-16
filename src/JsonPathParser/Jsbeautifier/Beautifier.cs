#region License

// MIT License
//
// Copyright (c) 2018 Denis Ivanov
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using System.Globalization;
using System.Text.RegularExpressions;

namespace XavierJefferson.JsonPathParser.Jsbeautifier;

public class Beautifier
{
    public Beautifier()
        : this(new BeautifierOptions())
    {
    }

    public Beautifier(BeautifierOptions opts)
    {
        Opts = opts;
        BlankState();
    }

    public BeautifierOptions Opts { get; set; }

    public BeautifierFlags Flags { get; set; }

    private List<BeautifierFlags> FlagStore { get; set; }

    private bool WantedNewline { get; set; }

    private bool JustAddedNewline { get; set; }

    private bool DoBlockJustClosed { get; set; }

    private string IndentString { get; set; }

    private string PreindentString { get; set; }

    private string LastWord { get; set; }

    private TokenTypeEnum LastType { get; set; }

    private string LastText { get; set; }

    private string LastLastText { get; set; }

    private string Input { get; set; }

    private List<string> Output { get; set; }

    private char[] Whitespace { get; set; }

    private string Wordchar { get; set; }

    private string Digits { get; set; }

    private string[] Punct { get; set; }

    private string[] LineStarters { get; set; }

    private int ParserPos { get; set; }

    private int NNewlines { get; set; }

    private void BlankState()
    {
        // internal flags
        Flags = new BeautifierFlags("BLOCK");
        FlagStore = new List<BeautifierFlags>();
        WantedNewline = false;
        JustAddedNewline = false;
        DoBlockJustClosed = false;

        if (Opts.IndentWithTabs)
            IndentString = "\t";
        else
            IndentString = new string(Opts.IndentChar, (int)Opts.IndentSize);

        PreindentString = "";
        LastWord = ""; // last TK_WORD seen
        LastType = TokenTypeEnum.TkStartExpr; // last token type
        LastText = ""; // last token text
        LastLastText = ""; // pre-last token text
        Input = null;
        Output = new List<string>(); // formatted javascript gets built here
        Whitespace = new[] { '\n', '\r', '\t', ' ' };
        Wordchar = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_$";
        Digits = "0123456789";
        Punct =
            "+ - * / % & ++ -- = += -= *= /= %= == === != !== > < >= <= >> << >>> >>>= >>= <<= && &= | || ! !! , : ? ^ ^= |= :: <?= <? ?> <%= <% %>"
                .Split(' ');

        // Words which always should start on a new line
        LineStarters = "continue,try,throw,return,var,if,switch,case,default,for,while,break,function".Split(',');
        SetMode("BLOCK");
        ParserPos = 0;
    }

    private void SetMode(string mode)
    {
        var prev = new BeautifierFlags("BLOCK");

        if (Flags != null)
        {
            FlagStore.Add(Flags);
            prev = Flags;
        }

        Flags = new BeautifierFlags(mode);

        if (FlagStore.Count == 1)
        {
            Flags.IndentationLevel = 0;
        }
        else
        {
            Flags.IndentationLevel = prev.IndentationLevel;

            if (prev.VarLine && prev.VarLineReindented) Flags.IndentationLevel = Flags.IndentationLevel + 1;
        }

        Flags.PreviousMode = prev.Mode;
    }

    public string Beautify(string s, BeautifierOptions? opts = null)
    {
        if (opts != null) Opts = opts;

        BlankState();

        while (s.Length != 0 && (s[0] == ' ' || s[0] == '\t'))
        {
            PreindentString += s[0];
            s = s.Remove(0, 1);
        }

        Input = s;
        ParserPos = 0;
        while (true)
        {
            var token = GetNextToken();
            // print (token_text, token_type, self.flags.mode)
            var tokenText = token.Item1;
            var tokenType = token.TokenType;

            if (tokenType == TokenTypeEnum.TkEof) break;

            var handlers = new Dictionary<TokenTypeEnum, Action<string>>
            {
                { TokenTypeEnum.TkStartExpr, HandleStartExpr },
                { TokenTypeEnum.TkEndExpr, HandleEndExpr },
                { TokenTypeEnum.TkStartBlock, HandleStartBlock },
                { TokenTypeEnum.TkEndBlock, HandleEndBlock },
                { TokenTypeEnum.TkWord, HandleWord },
                { TokenTypeEnum.TkSemicolon, HandleSemicolon },
                { TokenTypeEnum.TkString, HandleString },
                { TokenTypeEnum.TkEquals, HandleEquals },
                { TokenTypeEnum.TkOperator, HandleOperator },
                { TokenTypeEnum.TkComma, HandleComma },
                { TokenTypeEnum.TkBlockComment, HandleBlockComment },
                { TokenTypeEnum.TkInlineComment, HandleInlineComment },
                { TokenTypeEnum.TkComment, HandleComment },
                { TokenTypeEnum.TkDot, HandleDot },
                { TokenTypeEnum.TkUnknown, HandleUnknown }
            };

            handlers[tokenType](tokenText);

            if (tokenType != TokenTypeEnum.TkInlineComment)
            {
                LastLastText = LastText;
                LastType = tokenType;
                LastText = tokenText;
            }
        }

        var regex = new Regex(@"[\n ]+$");

        var sweetCode = PreindentString + regex.Replace(string.Concat(Output), "", 1);
        return sweetCode;
    }

    private void TrimOutput(bool eatNewlines = false)
    {
        while (Output.Count != 0 &&
               (Output[Output.Count - 1] == " " ||
                Output[Output.Count - 1] == IndentString ||
                Output[Output.Count - 1] == PreindentString ||
                (eatNewlines && (Output[Output.Count - 1] == "\n" || Output[Output.Count - 1] == "\r"))))
            Output.RemoveAt(Output.Count - 1);
    }

    private bool IsSpecialWord(string s)
    {
        return s == "case" || s == "return" || s == "do" || s == "if" || s == "throw" || s == "else";
    }

    private bool IsArray(string mode)
    {
        return mode == "[EXPRESSION]" || mode == "[INDENTED-EXPRESSION]";
    }

    private bool IsExpression(string mode)
    {
        return mode == "[EXPRESSION]" ||
               mode == "[INDENTED-EXPRESSION]" ||
               mode == "(EXPRESSION)" ||
               mode == "(FOR-EXPRESSION)" ||
               mode == "(COND-EXPRESSION)";
    }

    private void AllowWrapOrPreservedNewline(string tokenText, bool forceLinwrap = false)
    {
        if (Opts.WrapLineLength > 0 && !forceLinwrap)
        {
            var startLine = Output.Count - 1;

            while (startLine >= 0)
            {
                if (Output[startLine] == "\n") break;

                startLine--;
            }

            startLine++;

            if (startLine < Output.Count)
            {
                var slice = new string[Output.Count - startLine];
                Output.CopyTo(startLine, slice, 0, slice.Length);
                var currentLine = string.Concat(slice);

                if (currentLine.Length + tokenText.Length >= Opts.WrapLineLength) forceLinwrap = true;
            }
        }

        if (!JustAddedNewline && ((Opts.PreserveNewlines && WantedNewline) || forceLinwrap))
        {
            AppendNewline(true, false);
            AppendIndentString();
            WantedNewline = false;
        }
    }

    private void AppendNewline(bool ignoreRepeated = true, bool resetStatementFlags = true)
    {
        if (Opts.KeepArrayIndentation && IsArray(Flags.Mode)) return;

        if (resetStatementFlags)
        {
            Flags.IfLine = false;
            Flags.ChainExtraIndentation = 0;
        }

        TrimOutput();

        if (Output.Count == 0) return;

        if (Output[Output.Count - 1] != "\n" || !ignoreRepeated)
        {
            JustAddedNewline = true;
            Output.Add("\n");
        }

        if (!string.IsNullOrEmpty(PreindentString)) Output.Add(PreindentString);

        foreach (var i in Enumerable.Range(0, Flags.IndentationLevel + Flags.ChainExtraIndentation))
            AppendIndentString();

        if (Flags.VarLine && Flags.VarLineReindented) AppendIndentString();
    }

    private void AppendIndentString()
    {
        if (LastText != "") Output.Add(IndentString);
    }

    private void Append(string s)
    {
        if (s == " ")
        {
            // do not add just a single space after the // comment, ever
            if (LastType == TokenTypeEnum.TkComment)
            {
                AppendNewline();
                return;
            }

            // make sure only single space gets drawn
            if (Output.Count != 0 &&
                Output[Output.Count - 1] != " " &&
                Output[Output.Count - 1] != "\n" &&
                Output[Output.Count - 1] != IndentString)
                Output.Add(" ");
        }
        else
        {
            JustAddedNewline = false;
            Output.Add(s);
        }
    }

    private void Indent()
    {
        Flags.IndentationLevel = Flags.IndentationLevel + 1;
    }

    private void RemoveIndent()
    {
        if (Output.Count != 0 &&
            (Output[Output.Count - 1] == IndentString ||
             Output[Output.Count - 1] == PreindentString))
            Output.RemoveAt(Output.Count - 1);
    }

    private void RestoreMode()
    {
        DoBlockJustClosed = Flags.Mode == "DO_BLOCK";

        if (FlagStore.Count > 0)
        {
            var mode = Flags.Mode;
            Flags = FlagStore[FlagStore.Count - 1];
            FlagStore.RemoveAt(FlagStore.Count - 1);
            Flags.PreviousMode = mode;
        }
    }

    private Tkval GetNextToken()
    {
        NNewlines = 0;

        if (ParserPos >= Input.Length) return new Tkval("", TokenTypeEnum.TkEof);

        WantedNewline = false;
        var c = Input[ParserPos];
        ParserPos += 1;
        var keepWhitespace = Opts.KeepArrayIndentation && IsArray(Flags.Mode);

        if (keepWhitespace)
        {
            var whitespaceCount = 0;

            while (Whitespace.Contains(c))
            {
                if (c == '\n')
                {
                    TrimOutput();
                    Output.Add("\n");
                    JustAddedNewline = true;
                    whitespaceCount = 0;
                }
                else if (c == '\t')
                {
                    whitespaceCount += 4;
                }
                else if (c == '\r')
                {
                }
                else
                {
                    whitespaceCount += 1;
                }

                if (ParserPos >= Input.Length) return new Tkval("", TokenTypeEnum.TkEof);

                c = Input[ParserPos];
                ParserPos += 1;
            }

            if (JustAddedNewline)
                foreach (var i in Enumerable.Range(0, whitespaceCount))
                    Output.Add(" ");
        }
        else //  not keep_whitespace
        {
            while (Whitespace.Contains(c))
            {
                if (c == '\n')
                    if (Opts.MaxPreserveNewlines == 0 || Opts.MaxPreserveNewlines > NNewlines)
                        NNewlines += 1;

                if (ParserPos >= Input.Length) return new Tkval("", TokenTypeEnum.TkEof);

                c = Input[ParserPos];
                ParserPos += 1;
            }

            if (Opts.PreserveNewlines && NNewlines > 1)
                foreach (var i in Enumerable.Range(0, NNewlines))
                {
                    AppendNewline(i == 0);
                    JustAddedNewline = true;
                }

            WantedNewline = NNewlines > 0;
        }

        var cc = c.ToString();

        if (Wordchar.Contains(c))
        {
            if (ParserPos < Input.Length)
            {
                cc = c.ToString();

                while (Wordchar.Contains(Input[ParserPos]))
                {
                    cc += Input[ParserPos];
                    ParserPos += 1;
                    if (ParserPos == Input.Length)
                        break;
                }
            }

            // small and surprisingly unugly hack for 1E-10 representation
            if (ParserPos != Input.Length && "+-".Contains(Input[ParserPos]) && Regex.IsMatch(cc, "^[0-9]+[Ee]$"))
            {
                var sign = Input[ParserPos];
                ParserPos++;
                var t = GetNextToken();
                cc += sign + t.Item1;
                return new Tkval(cc, TokenTypeEnum.TkWord);
            }

            if (cc == "in") // in is an operator, need to hack
                return new Tkval(cc, TokenTypeEnum.TkOperator);

            if (WantedNewline
                && LastType != TokenTypeEnum.TkOperator
                && LastType != TokenTypeEnum.TkEquals
                && !Flags.IfLine
                && (Opts.PreserveNewlines || LastText != "var"))
                AppendNewline();

            return new Tkval(cc, TokenTypeEnum.TkWord);
        }

        if ("([".Contains(c)) return new Tkval(c.ToString(), TokenTypeEnum.TkStartExpr);

        if (")]".Contains(c)) return new Tkval(c.ToString(), TokenTypeEnum.TkEndExpr);

        if (c == '{') return new Tkval(c.ToString(), TokenTypeEnum.TkStartBlock);

        if (c == '}') return new Tkval(c.ToString(), TokenTypeEnum.TkEndBlock);

        if (c == ';') return new Tkval(c.ToString(), TokenTypeEnum.TkSemicolon);

        if (c == '/')
        {
            var comment = "";
            var inlineComment = true;

            if (Input[ParserPos] == '*') // peek /* .. */ comment
            {
                ParserPos += 1;
                if (ParserPos < Input.Length)
                    while (!(Input[ParserPos] == '*' && ParserPos + 1 < Input.Length && Input[ParserPos + 1] == '/') &&
                           ParserPos < Input.Length)
                    {
                        c = Input[ParserPos];
                        comment += c;

                        if ("\r\n".Contains(c)) inlineComment = false;

                        ParserPos += 1;

                        if (ParserPos >= Input.Length) break;
                    }

                ParserPos += 2;

                if (inlineComment && NNewlines == 0)
                    return new Tkval($"/*{comment}*/", TokenTypeEnum.TkInlineComment);

                return new Tkval($"/*{comment}*/", TokenTypeEnum.TkBlockComment);
            }

            if (Input[ParserPos] == '/') // peek // comment
            {
                comment = c.ToString();
                while (!"\r\n".Contains(Input[ParserPos]))
                {
                    comment += Input[ParserPos];
                    ParserPos += 1;

                    if (ParserPos >= Input.Length) break;
                }

                if (WantedNewline) AppendNewline();

                return new Tkval(comment, TokenTypeEnum.TkComment);
            }
        }

        if (c == '\'' || c == '"' ||
            (c == '/' &&
             ((LastType == TokenTypeEnum.TkWord && IsSpecialWord(LastText)) ||
              (LastType == TokenTypeEnum.TkEndExpr && (Flags.PreviousMode == "(FOR-EXPRESSION)" ||
                                                       Flags.PreviousMode == "(COND-EXPRESSION)")) ||
              new[]
              {
                  TokenTypeEnum.TkComment, TokenTypeEnum.TkStartExpr, TokenTypeEnum.TkStartBlock,
                  TokenTypeEnum.TkEndBlock, TokenTypeEnum.TkOperator, TokenTypeEnum.TkEquals, TokenTypeEnum.TkEof,
                  TokenTypeEnum.TkSemicolon, TokenTypeEnum.TkComma
              }.Contains(LastType))))
        {
            var sep = c;
            var betterSep = c == '\'' ? '"' : c;
            var esc = false;
            var esc1 = 0;
            var esc2 = 0;
            var resultingString = betterSep.ToString();

            if (ParserPos < Input.Length)
            {
                if (sep == '/')
                {
                    // handle regexp
                    var inCharClass = false;
                    while (esc || inCharClass || Input[ParserPos] != sep)
                    {
                        resultingString += Input[ParserPos];
                        if (!esc)
                        {
                            esc = Input[ParserPos] == '\\';
                            if (Input[ParserPos] == '[')
                                inCharClass = true;
                            else if (Input[ParserPos] == ']') inCharClass = false;
                        }
                        else
                        {
                            esc = false;
                        }

                        ParserPos += 1;
                        if (ParserPos >= Input.Length)
                            // ncomplete regex when end-of-file reached
                            // bail out with what has received so far
                            return new Tkval(resultingString, TokenTypeEnum.TkString);
                    }
                }
                else
                {
                    // handle string
                    //while (esc || Input[ParserPos] != sep)
                    //{

                    while (true)
                    {
                        var ipp = Input[ParserPos];
                        if (!(esc || ipp != sep)) break;
                        if (ipp == betterSep && sep != betterSep)
                        {
                            if (!esc) resultingString += "\\";

                            resultingString += $"u{((int)betterSep).ToString("x4")}";
                        }
                        else
                        {
                            if (ipp == '\'' && esc)
                                resultingString += $"u{((int)ipp).ToString("x4")}";
                            else
                                resultingString += ipp;
                        }

                        if (esc1 != 0 && esc1 >= esc2)
                        {
                            if (!int.TryParse(
                                    new string(resultingString.Skip(Math.Max(0, resultingString.Count() - esc2))
                                        .Take(esc2).ToArray()), NumberStyles.HexNumber, CultureInfo.CurrentCulture,
                                    out esc1))
                                esc1 = 0;
                            if (esc1 != 0 && esc1 >= 0x20 && esc1 <= 0x7e)
                            {
                                // FIXME
                                resultingString = new string(resultingString.Take(2 + esc2).ToArray());

                                if ((char)esc1 == sep || (char)esc1 == '\\') resultingString += '\\';

                                resultingString += (char)esc1;
                            }

                            esc1 = 0;
                        }

                        if (esc1 != 0)
                            ++esc1;
                        else if (!esc)
                            esc = Input[ParserPos] == '\\';
                        else
                            esc = false;
                        // TODO
                        //if (/*this.Opts.UnescapeStrings*/false)
                        /*{
                                if (this.Input[this.ParserPos] == 'x')
                                {
                                    ++esc1;
                                    esc2 = 2;
                                }
                                else if (this.Input[this.ParserPos] == 'u')
                                {
                                    ++esc1;
                                    esc2 = 4;
                                }
                            }*/
                        ParserPos += 1;
                        if (ParserPos >= Input.Length)
                            // incomplete string when end-of-file reached
                            // bail out with what has received so far
                            return new Tkval(resultingString, TokenTypeEnum.TkString);
                    }
                }
            }

            ParserPos += 1;
            resultingString += betterSep;
            if (sep == '/')
                // regexps may have modifiers /regexp/MOD, so fetch those too
                while (ParserPos < Input.Length && Wordchar.Contains(Input[ParserPos]))
                {
                    resultingString += Input[ParserPos];
                    ParserPos += 1;
                }

            return new Tkval(resultingString, TokenTypeEnum.TkString);
        }

        if (c == '#')
        {
            var resultString = "";
            // she-bang
            if (Output.Count == 0 && Input.Length > 1 && Input[ParserPos] == '!')
            {
                resultString = c.ToString();
                while (ParserPos < Input.Length && c != '\n')
                {
                    c = Input[ParserPos];
                    resultString += c;
                    ParserPos += 1;
                }

                Output.Add(resultString.Trim() + '\n');
                AppendNewline();
                return GetNextToken();
            }

            //  Spidermonkey-specific sharp variables for circular references
            // https://developer.mozilla.org/En/Sharp_variables_in_JavaScript
            // http://mxr.mozilla.org/mozilla-central/source/js/src/jsscan.cpp around line 1935
            var sharp = "#";

            if (ParserPos < Input.Length && Digits.Contains(Input[ParserPos]))
                while (true)
                {
                    c = Input[ParserPos];
                    sharp += c;
                    ParserPos += 1;

                    if (ParserPos >= Input.Length || c == '#' || c == '=') break;
                }

            if (c == '#' || ParserPos >= Input.Length)
            {
                // pass
            }
            else if (Input[ParserPos] == '[' && Input[ParserPos + 1] == ']')
            {
                sharp += "[]";
                ParserPos += 2;
            }
            else if (Input[ParserPos] == '{' && Input[ParserPos + 1] == '}')
            {
                sharp += "{}";
                ParserPos += 2;
            }

            return new Tkval(sharp, TokenTypeEnum.TkWord);
        }

        if (c == '<' && Input.Substring(ParserPos - 1, Math.Min(4, Input.Length - ParserPos + 1)) == "<!--")
        {
            ParserPos += 3;
            var ss = "<!--";

            while (ParserPos < Input.Length && Input[ParserPos] != '\n')
            {
                ss += Input[ParserPos];
                ParserPos += 1;
            }

            Flags.InHtmlComment = true;
            return new Tkval(ss, TokenTypeEnum.TkComment);
        }

        if (c == '-' && Flags.InHtmlComment && Input.Substring(ParserPos - 1, 3) == "-->")
        {
            Flags.InHtmlComment = false;
            ParserPos += 2;

            if (WantedNewline) AppendNewline();

            return new Tkval("-->", TokenTypeEnum.TkComment);
        }

        if (c == '.') return new Tkval(".", TokenTypeEnum.TkDot);

        if (Punct.Contains(c.ToString()))
        {
            var ss = c.ToString();
            while (ParserPos < Input.Length && Punct.Contains(ss + Input[ParserPos]))
            {
                ss += Input[ParserPos];
                ParserPos += 1;

                if (ParserPos >= Input.Length) break;
            }

            if (ss == "=") return new Tkval("=", TokenTypeEnum.TkEquals);

            if (ss == ",") return new Tkval(",", TokenTypeEnum.TkComma);

            return new Tkval(ss, TokenTypeEnum.TkOperator);
        }

        return new Tkval(c.ToString(), TokenTypeEnum.TkUnknown);
    }

    private void HandleStartExpr(string tokenText)
    {
        if (tokenText == "[")
        {
            if (LastType == TokenTypeEnum.TkWord || LastText == ")")
            {
                if (LineStarters.Contains(LastText)) Append(" ");

                SetMode("(EXPRESSION)");
                Append(tokenText);
                return;
            }

            if (Flags.Mode == "[EXPRESSION]" || Flags.Mode == "[INDENTED-EXPRESSION]")
            {
                if (LastLastText == "]" && LastText == ",")
                {
                    // # ], [ goes to a new line
                    if (Flags.Mode == "[EXPRESSION]")
                    {
                        Flags.Mode = "[INDENTED-EXPRESSION]";
                        if (!Opts.KeepArrayIndentation) Indent();
                    }

                    SetMode("[EXPRESSION]");

                    if (!Opts.KeepArrayIndentation) AppendNewline();
                }
                else if (LastText == "[")
                {
                    if (Flags.Mode == "[EXPRESSION]")
                    {
                        Flags.Mode = "[INDENTED-EXPRESSION]";

                        if (!Opts.KeepArrayIndentation) Indent();
                    }

                    SetMode("[EXPRESSION]");

                    if (!Opts.KeepArrayIndentation) AppendNewline();
                }
                else
                {
                    SetMode("[EXPRESSION]");
                }
            }
            else
            {
                SetMode("[EXPRESSION]");
            }
        }
        else
        {
            if (LastText == "for")
                SetMode("(FOR-EXPRESSION)");
            else if (LastText == "if" || LastText == "while")
                SetMode("(COND-EXPRESSION)");
            else
                SetMode("(EXPRESSION)");
        }

        if (LastText == ";" || LastType == TokenTypeEnum.TkStartBlock)
        {
            AppendNewline();
        }
        else if (LastType == TokenTypeEnum.TkEndExpr || LastType == TokenTypeEnum.TkStartExpr ||
                 LastType == TokenTypeEnum.TkEndBlock || LastText == ".")
        {
            // do nothing on (( and )( and ][ and ]( and .(
            if (WantedNewline) AppendNewline();
        }
        else if (LastType != TokenTypeEnum.TkWord && LastType != TokenTypeEnum.TkOperator)
        {
            Append(" ");
        }
        else if (LastWord == "function" || LastWord == "typeof")
        {
            // function() vs function (), typeof() vs typeof ()
            if (Opts.JslintHappy) Append(" ");
        }
        else if (LineStarters.Contains(LastText) || LastText == "catch")
        {
            Append(" ");
        }

        if (LastType == TokenTypeEnum.TkEquals ||
            LastType == TokenTypeEnum.TkOperator)
            if (Flags.Mode != "OJBECT")
                AllowWrapOrPreservedNewline(tokenText);

        Append(tokenText);
    }

    private void HandleEndExpr(string tokenText)
    {
        if (tokenText == "]")
        {
            if (Opts.KeepArrayIndentation)
            {
                if (LastText == "}")
                {
                    RemoveIndent();
                    Append(tokenText);
                    RestoreMode();
                    return;
                }
            }
            else if (Flags.Mode == "[INDENTED-EXPRESSION]")
            {
                if (LastText == "]")
                {
                    RestoreMode();
                    AppendNewline();
                    Append(tokenText);
                    return;
                }
            }
        }

        RestoreMode();
        Append(tokenText);
    }

    private void HandleStartBlock(string tokenText)
    {
        if (LastWord == "do")
            SetMode("DO_BLOCK");
        else
            SetMode("BLOCK");

        if (Opts.BraceStyle == BraceStyle.Expand)
        {
            if (LastType != TokenTypeEnum.TkOperator)
            {
                if (LastType == TokenTypeEnum.TkEquals ||
                    (IsSpecialWord(LastText) && LastText != "else"))
                    Append(" ");
                else
                    AppendNewline();
            }

            Append(tokenText);
            Indent();
        }
        else
        {
            if (LastType != TokenTypeEnum.TkOperator && LastType != TokenTypeEnum.TkStartExpr)
            {
                if (LastType == TokenTypeEnum.TkStartBlock)
                    AppendNewline();
                else
                    Append(" ");
            }
            else
            {
                // if TK_OPERATOR or TK_START_EXPR
                if (IsArray(Flags.PreviousMode) && LastText == ",")
                {
                    if (LastLastText == "}")
                        Append(" ");
                    else
                        AppendNewline();
                }
            }

            Indent();
            Append(tokenText);
        }
    }

    private void HandleEndBlock(string tokenText)
    {
        RestoreMode();
        if (Opts.BraceStyle == BraceStyle.Expand)
        {
            if (LastText != "{") AppendNewline();
        }
        else
        {
            if (LastType == TokenTypeEnum.TkStartBlock)
            {
                if (JustAddedNewline)
                    RemoveIndent();
                else
                    TrimOutput();
            }
            else
            {
                if (IsArray(Flags.Mode) && Opts.KeepArrayIndentation)
                {
                    Opts.KeepArrayIndentation = false;
                    AppendNewline();
                    Opts.KeepArrayIndentation = true;
                }
                else
                {
                    AppendNewline();
                }
            }
        }

        Append(tokenText);
    }

    private void HandleWord(string tokenText)
    {
        var mightEnclose = false;
        if (DoBlockJustClosed)
        {
            Append(" ");
            Append(tokenText);
            Append(" ");
            DoBlockJustClosed = false;
            return;
        }

        if (tokenText == "function")
        {
            if (Flags.VarLine && LastText != "=") Flags.VarLineReindented = !Opts.KeepFunctionIndentation;

            if ((JustAddedNewline || LastText == ";") && LastText != "{")
            {
                // make sure there is a nice clean space of at least one blank line
                // before a new function definition
                var haveNewlines = NNewlines;
                if (!JustAddedNewline) haveNewlines = 0;

                if (!Opts.PreserveNewlines) haveNewlines = 1;

                for (var i = 0; i < 2 - haveNewlines; ++i) AppendNewline(false);
            }

            if (LastText == "get" || LastText == "set" || LastText == "new" ||
                LastType == TokenTypeEnum.TkWord) Append(" ");

            if (LastType == TokenTypeEnum.TkWord)
            {
                if (LastText == "get" || LastText == "set" || LastText == "new" || LastText == "return")
                    Append(" ");
                else
                    AppendNewline();
            }
            else if (LastType == TokenTypeEnum.TkOperator || LastText == "=")
            {
                // foo = function
                Append(" ");
            }
            else if (IsExpression(Flags.Mode))
            {
                // (function
            }
            else
            {
                AppendNewline();
            }

            Append("function");
            LastWord = "function";
            return;
        }

        if (tokenText == "case" || (tokenText == "default" && Flags.InCaseStatement))
        {
            AppendNewline();
            if (Flags.CaseBody)
            {
                RemoveIndent();
                Flags.CaseBody = false;
                Flags.IndentationLevel -= 1;
            }

            Append(tokenText);
            Flags.InCase = true;
            Flags.InCaseStatement = true;
            return;
        }

        var prefix = "NONE";

        if (LastType == TokenTypeEnum.TkEndBlock)
        {
            if (tokenText != "else" && tokenText != "catch" && tokenText != "finally")
            {
                prefix = "NEWLINE";
            }
            else
            {
                if (Opts.BraceStyle == BraceStyle.Expand || Opts.BraceStyle == BraceStyle.EndExpand)
                {
                    prefix = "NEWLINE";
                }
                else
                {
                    prefix = "SPACE";
                    Append(" ");
                }
            }
        }
        else if (LastType == TokenTypeEnum.TkSemicolon && (Flags.Mode == "BLOCK" || Flags.Mode == "DO_BLOCK"))
        {
            prefix = "NEWLINE";
        }
        else if (LastType == TokenTypeEnum.TkSemicolon && IsExpression(Flags.Mode))
        {
            prefix = "SPACE";
        }
        else if (LastType == TokenTypeEnum.TkString)
        {
            prefix = "NEWLINE";
        }
        else if (LastType == TokenTypeEnum.TkWord)
        {
            if (LastText == "else")
                // eat newlines between ...else *** some_op...
                // won't preserve extra newlines in this place (if any), but don't care that much
                TrimOutput(true);
            prefix = "SPACE";
        }
        else if (LastType == TokenTypeEnum.TkStartBlock)
        {
            prefix = "NEWLINE";
        }
        else if (LastType == TokenTypeEnum.TkEndExpr)
        {
            Append(" ");
            prefix = "NEWLINE";
        }

        if (Flags.IfLine && LastType == TokenTypeEnum.TkEndExpr) Flags.IfLine = false;

        if (LastType == TokenTypeEnum.TkComma ||
            LastType == TokenTypeEnum.TkStartExpr ||
            LastType == TokenTypeEnum.TkEquals ||
            LastType == TokenTypeEnum.TkOperator)
            if (Flags.Mode != "OBJECT")
                AllowWrapOrPreservedNewline(tokenText);

        if (LineStarters.Contains(tokenText))
        {
            if (LastText == "else")
                prefix = "SPACE";
            else
                prefix = "NEWLINE";
        }

        if (tokenText == "else" || tokenText == "catch" || tokenText == "finally")
        {
            if (LastType != TokenTypeEnum.TkEndBlock || Opts.BraceStyle == BraceStyle.Expand ||
                Opts.BraceStyle == BraceStyle.EndExpand)
            {
                AppendNewline();
            }
            else
            {
                TrimOutput(true);
                Append(" ");
            }
        }
        else if (prefix == "NEWLINE")
        {
            if (IsSpecialWord(LastText))
            {
                // no newline between return nnn
                Append(" ");
            }
            else if (LastType != TokenTypeEnum.TkEndExpr)
            {
                if ((LastType != TokenTypeEnum.TkStartExpr || tokenText != "var") && LastText != ":")
                {
                    // no need to force newline on VAR -
                    // for (var x = 0...
                    if (tokenText == "if" && LastWord == "else" && LastText != "{")
                    {
                        Append(" ");
                    }
                    else
                    {
                        Flags.VarLine = false;
                        Flags.VarLineReindented = false;
                        mightEnclose = true;
                        AppendNewline();
                    }
                }
            }
            else if (LineStarters.Contains(tokenText) && LastText != ")")
            {
                Flags.VarLine = false;
                Flags.VarLineReindented = false;
                AppendNewline();
            }
            //mightEnclose = true;
        }
        else if (IsArray(Flags.Mode) && LastText == "," && LastLastText == "}")
        {
            AppendNewline(); //}, in lists get a newline
        }
        else if (prefix == "SPACE")
        {
            Append(" ");
        }

        if (mightEnclose && !tokenText.StartsWith("\"")) Append("\"");

        Append(tokenText);
        if (mightEnclose && !tokenText.StartsWith("\"")) Append("\"");
        LastWord = tokenText;

        if (tokenText == "var")
        {
            Flags.VarLine = true;
            Flags.VarLineReindented = false;
            Flags.VarLineTainted = false;
        }

        if (tokenText == "if") Flags.IfLine = true;

        if (tokenText == "else") Flags.IfLine = false;
    }

    private void HandleSemicolon(string tokenText)
    {
        Append(tokenText);
        Flags.VarLine = false;
        Flags.VarLineReindented = false;
        if (Flags.Mode == "OBJECT")
            // OBJECT mode is weird and doesn't get reset too well.
            Flags.Mode = "BLOCK";
    }

    private void HandleString(string tokenText)
    {
        if (LastType == TokenTypeEnum.TkEndExpr &&
            (Flags.PreviousMode == "(COND-EXPRESSION)" || Flags.PreviousMode == "(FOR-EXPRESSION)"))
        {
            Append(" ");
        }
        else if (LastType == TokenTypeEnum.TkWord)
        {
            Append(" ");
        }
        else if (LastType == TokenTypeEnum.TkComma ||
                 LastType == TokenTypeEnum.TkStartExpr ||
                 LastType == TokenTypeEnum.TkEquals ||
                 LastType == TokenTypeEnum.TkOperator)
        {
            if (Flags.Mode != "OBJECT") AllowWrapOrPreservedNewline(tokenText);
        }
        else
        {
            AppendNewline();
        }

        Append(tokenText);
    }

    private void HandleEquals(string tokenText)
    {
        if (Flags.VarLine)
            // just got an '=' in a var-line, different line breaking rules will apply
            Flags.VarLineTainted = true;

        Append(" ");
        Append(tokenText);
        Append(" ");
    }

    private void HandleComma(string tokenText)
    {
        if (LastType == TokenTypeEnum.TkComment) AppendNewline();

        if (Flags.VarLine)
        {
            if (IsExpression(Flags.Mode) || LastType == TokenTypeEnum.TkEndBlock)
                // do not break on comma, for ( var a = 1, b = 2
                Flags.VarLineTainted = false;
            if (Flags.VarLineTainted)
            {
                Append(tokenText);
                Flags.VarLineReindented = true;
                Flags.VarLineTainted = false;
                AppendNewline();
                return;
            }

            Flags.VarLineTainted = false;

            Append(tokenText);
            Append(" ");
            return;
        }

        if (LastType == TokenTypeEnum.TkEndBlock && Flags.Mode != "(EXPRESSION)")
        {
            Append(tokenText);
            if (Flags.Mode == "OBJECT" && LastText == "}")
                AppendNewline();
            else
                Append(" ");
        }
        else
        {
            if (Flags.Mode == "OBJECT")
            {
                Append(tokenText);
                AppendNewline();
            }
            else
            {
                // EXPR or DO_BLOCK
                Append(tokenText);
                Append(" ");
            }
        }
    }

    private void HandleOperator(string tokenText)
    {
        var spaceBefore = true;
        var spaceAfter = true;

        if (IsSpecialWord(LastText))
        {
            // return had a special handling in TK_WORD
            Append(" ");
            Append(tokenText);
            return;
        }

        // hack for actionscript's import .*;
        if (tokenText == "*" && LastType == TokenTypeEnum.TkDot && !LastLastText.All(char.IsDigit))
        {
            Append(tokenText);
            return;
        }

        if (tokenText == ":" && Flags.InCase)
        {
            Flags.CaseBody = true;
            Indent();
            Append(tokenText);
            AppendNewline();
            Flags.InCase = true;
            return;
        }

        if (tokenText == "::")
        {
            // no spaces around the exotic namespacing syntax operator
            Append(tokenText);
            return;
        }

        if (tokenText == "++" || tokenText == "--" || tokenText == "!" || ((tokenText == "+" || tokenText == "-") &&
                                                                           (LastType == TokenTypeEnum.TkStartBlock ||
                                                                            LastType == TokenTypeEnum.TkStartExpr ||
                                                                            LastType == TokenTypeEnum.TkEquals ||
                                                                            LastType == TokenTypeEnum.TkOperator ||
                                                                            LineStarters.Contains(LastText) ||
                                                                            LastText == ",")))
        {
            spaceBefore = false;
            spaceAfter = false;

            if (LastText == ";" && IsExpression(Flags.Mode))
                // for (;; ++i)
                // ^^
                spaceBefore = true;

            if (LastType == TokenTypeEnum.TkWord && LineStarters.Contains(LastText)) spaceBefore = true;

            if (Flags.Mode == "BLOCK" && (LastText == ";" || LastText == "{"))
                // { foo: --i }
                // foo(): --bar
                AppendNewline();
        }
        else if (tokenText == ":")
        {
            if (Flags.TernaryDepth == 0)
            {
                if (Flags.Mode == "BLOCK") Flags.Mode = "OBJECT";
                spaceBefore = false;
            }
            else
            {
                Flags.TernaryDepth -= 1;
            }
        }
        else if (tokenText == "?")
        {
            Flags.TernaryDepth += 1;
        }

        if (spaceBefore) Append(" ");

        Append(tokenText);

        if (spaceAfter) Append(" ");
    }

    private void HandleBlockComment(string tokenText)
    {
        var lines = tokenText.Replace("\r", "").Split('\n');
        // all lines start with an asterisk? that's a proper box comment

        if (lines.Skip(1).Where(x => x.Trim().Length == 0 || x.TrimStart()[0] != '*').All(string.IsNullOrEmpty))
        {
            AppendNewline();
            Append(lines[0]);
            foreach (var line in lines.Skip(1))
            {
                AppendNewline();
                Append(" " + line.Trim());
            }
        }
        else
        {
            // simple block comment: leave intact
            if (lines.Length > 1)
                // multiline comment starts on a new line
                AppendNewline();
            else
                // single line /* ... */ comment stays on the same line
                Append(" ");
            foreach (var line in lines)
            {
                Append(line);
                Append("\n");
            }
        }

        AppendNewline();
    }

    private void HandleInlineComment(string tokenText)
    {
        Append(" ");
        Append(tokenText);
        Append(" ");
    }

    private void HandleComment(string tokenText)
    {
        if (LastText == "," && !WantedNewline) TrimOutput(true);

        if (LastType != TokenTypeEnum.TkComment)
        {
            if (WantedNewline)
                AppendNewline();
            else
                Append(" ");
        }

        Append(tokenText);
        AppendNewline();
    }

    private void HandleDot(string tokenText)
    {
        if (IsSpecialWord(LastText))
            Append(" ");
        else
            AllowWrapOrPreservedNewline(tokenText, LastText == ")" && Opts.BreakChainedMethods);

        Append(tokenText);
    }

    private void HandleUnknown(string tokenText)
    {
        Append(tokenText);
    }

    private class Tkval
    {
        public Tkval(string item1, TokenTypeEnum item2)
        {
            Item1 = item1;
            TokenType = item2;
        }

        public string Item1 { get; }
        public TokenTypeEnum TokenType { get; }
    }

    private enum TokenTypeEnum
    {
        None,
        TkEof,
        TkStartExpr,
        TkEndExpr,
        TkStartBlock,
        TkEndBlock,
        TkWord,
        TkSemicolon,
        TkString,
        TkEquals,
        TkOperator,
        TkComma,
        TkBlockComment,
        TkInlineComment,
        TkComment,
        TkDot,
        TkUnknown
    }
}