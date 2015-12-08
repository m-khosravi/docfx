﻿namespace Microsoft.DocAsCode.EntityModel
{
    using System.Text.RegularExpressions;
    using Microsoft.DocAsCode.MarkdownLite;

    public class DfmParagraphBlockRule : GfmParagraphBlockRule
    {
        public static readonly Regex _paragraph = new Regex(@"^((?:[^\n]+\n?(?! *(`{3,}|~{3,}) *(\S+)? *\n([\s\S]+?)\s*\2 *(?:\n+|$)|( *)((?:[*+-]|\d+\.)) [\s\S]+?(?:\n+(?=\5?(?:[-*_] *){3,}(?:\n+|$))|\[!((?i)code(-(?<lang>(\w|-)+))?)\[(?<name>(?:\[[^\]]*\]|[^\[\]]|\](?=[^\[]*\]))*)\]\(\s*<?(?<path>[\s\S]*?)>?(?:\s+(?<quote>['""])(?<title>[\s\S]*?)\k<quote>)?\s*\)\]\s*(\n|$)|<!--(\s*)((?i)BEGINSECTION)(\s*)(?<attributes>.*?)(\s*)-->(\s*)(?:\n+|$)|<!--(\s*)((?i)ENDSECTION)(\s*)-->(\s*)(?:\n+|$)|\n+(?= *\[([^\]]+)\]: *<?([^\s>]+)>?(?: +[""(]([^\n]+)["")])? *(?:\n+|$))|\n{2,}(?! )(?!\5(?:[*+-]|\d+\.) )\n*|\s*$)|( *[-*_]){3,} *(?:\n+|$)| *(#{1,6}) *([^\n]+?) *#* *(?:\n+|$)|([^\n]+)\n *(=|-){2,} *(?:\n+|$)|( *>[^\n]+(\n(?! *\[([^\]]+)\]: *<?([^\s>]+)>?(?: +[""(]([^\n]+)["")])? *(?:\n+|$))[^\n]+)*\n*)+|<(?!(?:a|em|strong|small|s|cite|q|dfn|abbr|data|time|code|var|samp|kbd|sub|sup|i|b|u|mark|ruby|rt|rp|bdi|bdo|span|br|wbr|ins|del|img)\b)\w+(?!:\/|[^\w\s@]*@)\b| *\[([^\]]+)\]: *<?([^\s>]+)>?(?: +[""(]([^\n]+)["")])? *(?:\n+|$)))+)\n*", RegexOptions.Compiled);

        public override Regex Paragraph
        {
            get
            {
                return _paragraph;
            }
        }
    }
}