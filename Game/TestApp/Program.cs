// See https://aka.ms/new-console-template for more information

using TextFragmentLib2.TextProcessing.Parsing;

/*var result01 = FragmentParserDebug.ParseUnary("!1");
var result02 = FragmentParserDebug.ParseUnary(" !1 ");
var result03 = FragmentParserDebug.ParseUnary(" ! 1 ");

var result04 = FragmentParserDebug.ParseCall("Test()");
var result05 = FragmentParserDebug.ParseCall("Test() ");
var result06 = FragmentParserDebug.ParseCall(" Test ( ) ");

var result07 = FragmentParserDebug.ParseCall("Test(1)");
var result08 = FragmentParserDebug.ParseCall(" Test(1) ");
var result09 = FragmentParserDebug.ParseCall(" Test ( 1 ) ");

var result10 = FragmentParserDebug.ParseCall("Test(1,2)");
var result11 = FragmentParserDebug.ParseCall(" Test(1,2) ");
var result12 = FragmentParserDebug.ParseCall(" Test ( 1 , 2 ) ");

var result13 = FragmentParserDebug.ParseBinary("1+2");
var result14 = FragmentParserDebug.ParseBinary(" 1+2 ");
var result15 = FragmentParserDebug.ParseBinary(" 1 + 2 ");

var result16 = FragmentParser.ParseExpression("Test2(2) + !121 and 2 or 1");*/

var result = FragmentParserDebug.ParseNodes("(TestAtt1=10+8+5+9,TestAtt2=Hallo(2, 2+2))");

Console.ReadKey();