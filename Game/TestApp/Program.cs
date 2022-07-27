// See https://aka.ms/new-console-template for more information

// var test3 = TextFragmentParser.ParseExpression("Test2(2) + !121 and 2 or 1");
// var test2 = TextFragmentParser.ParseExpression("1");
// var test = TextFragmentParser.ParseExpression("1 + 2");
// var result = TextFragmentParser.ParseExpression("!1 + 2 : 6 * Test2(8 : 1, 3) + !7 and 10 or 11");

//var attributeValue = TextFragmentParser.AttributeParameters.Parse("(true and false, 1 + 2, Test(1,2))");

using TextFragmentLib2.TextProcessing.Parsing;

var test3 = FragmentParser.ParseExpression("Test2(2) + !121 and 2 or 1");

Console.ReadKey();