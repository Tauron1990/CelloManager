// See https://aka.ms/new-console-template for more information

using Pidgin;
using TextFragmentLib;

var test3 = TextFragmentParser.ParseExpression("Test2(2) + !121 and 2 or 1");
var test2 = TextFragmentParser.ParseExpression("1");
var test = TextFragmentParser.ParseExpression("1 + 2");
var result = TextFragmentParser.ParseExpression("!1 + 2 : 6 * Test2(8 : 1, 3) + !7 and 10 or 11");

Console.ReadKey();