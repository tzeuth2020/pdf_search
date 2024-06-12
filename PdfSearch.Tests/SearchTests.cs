using SearchServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace pdfSearch.Tests;

public class SearchTests {
    [Fact]
    public void SimpleString()
    {
        string pattern = "aa";
        string text = "bbaabb";
        int limit = 0;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int>([2]);
        SearchDoc Sd = new SearchDoc();
        Assert.Equal(result, SearchDoc.findMatches(pattern, ms, limit));
    }

    [Fact]
    public void MultipleMatches()
    {
        string pattern = "aa";
        string text = "bbaabbaa";
        int limit = 0;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int>([2, 6]);
        Assert.Equal(result, SearchDoc.findMatches(pattern, ms, limit));
    }

    [Fact]
    public void ConsecuitveMatches()
    {
        string pattern = "aa";
        string text = "bbaaabb";
        int limit = 0;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int>([2, 3]);
        Assert.Equal(result, SearchDoc.findMatches(pattern, ms, limit));
    }

    [Fact]
    public void BasicWithLimit()
    {
        string pattern = "aa";
        string text = "bbaabb";
        int limit = 1;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int>([1, 2, 3]);
        Assert.Equal(result, SearchDoc.findMatches(pattern, ms, limit));
    }

    [Fact]
    public void MultiplePatternsWithLimit()
    {
        string pattern = "aa";
        string text = "bbaabbaa";
        int limit = 1;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int>([1, 2, 3, 5, 6]);
        Assert.Equal(result, SearchDoc.findMatches(pattern, ms, limit));
    }

    [Fact]
    public void BasicWithWords()
    {
        string pattern = "cat";
        string text = "the fat cat ate a large rat";
        int limit = 0;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int>([8]);
        Assert.Equal(result, SearchDoc.findMatches(pattern, ms, limit));
    }

    [Fact]
    public void BasicWithWordsAndLowLimit()
    {
        string pattern = "cat";
        string text = "the fat cat ate a large rat";
        int limit = 1;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int>([4, 8, 11, 24]);
        Assert.Equal(result, SearchDoc.findMatches(pattern, ms, limit));
    }

    [Fact]
    public void BasicWithWordsAndHighLimit()
    {
        string pattern = "cat";
        string text = "the fat cat ate a large rat";
        int limit = 2;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int>([4, 8, 11, 15, 18, 24]);
        Assert.Equal(result, SearchDoc.findMatches(pattern, ms, limit));
    }

    [Fact]
    public void LongPatternExactMatch()
    {
        string pattern = "cat with rat";
        string text = "here asdfa ckjcv cat with rat asl;kjd";
        int limit = 0;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int>([17]);
        Assert.Equal(result, SearchDoc.findMatches(pattern, ms, limit));
    }


    [Fact]
    public void LongPatternBounds()
    {
        string pattern = "cat with rat";
        string text = "here asdfa ckjcv cat with rat asl;kjd";
        int limit = 0;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int>();
        Assert.Equal(result, SearchDoc.findMatches(pattern, ms, limit, -1, 18));
    }

        [Fact]
    public void LongPatternLowerBounds()
    {
        string pattern = "cat with rat";
        string text = "here asdfa ckjcv cat with rat asl;kjd";
        int limit = 0;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int>();
        Assert.Equal(result, SearchDoc.findMatches(pattern, ms, limit, 18));
    }



    [Fact]
    public void NoExactMatch()
    {
        string pattern = "cat with rat";
        string text = "here asdfa ckjcv cat wivh eat asl;kjd";
        int limit = 0;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int>();
        Assert.Equal(result, SearchDoc.findMatches(pattern, ms, limit));
    }

    [Fact]
    public void NoExactMatchLastLetter()
    {
        string pattern = "cat with rat";
        string text = "here asdfa ckjcv cat with rag asl;kjd";
        int limit = 0;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int>();
        Assert.Equal(result, SearchDoc.findMatches(pattern, ms, limit));
    }

    [Fact]
    public void EmptyPattern()
    {
        string pattern = "";
        string text = "here asdfa ckjcv cat with rag asl;kjd";
        int limit = 0;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int>();
        Assert.Equal(result, SearchDoc.findMatches(pattern, ms, limit));
    }

    [Fact]
    public void EmptyText()
    {
        string pattern = "cat with rat";
        string text = "";
        int limit = 0;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int>();
        Assert.Equal(result, SearchDoc.findMatches(pattern, ms, limit));
    }

    [Fact]
    public void InvalidPattern()
    {
        string pattern = "cat with rat asdfjhasdlkfjhasldkjfhlaskdjfhasdfasdfasdf";
        string text = "here asdfa ckjcv cat with rag asl;kjd";
        int limit = 0;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int>();
        Assert.Throws<ArgumentException>(() => SearchDoc.findMatches(pattern, ms, limit));
    }

    [Fact]
    public void NegativeLimit()
    {
        string pattern = "cat";
        string text = "here asdfa ckjcv cat with rag asl;kjd";
        int limit = -1;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int>();
        Assert.Throws<ArgumentException>(() => SearchDoc.findMatches(pattern, ms, limit));
    }


    [Fact]
    public void LimitTooHigh()
    {
        string pattern = "cat";
        string text = "here asdfa ckjcv cat with rag asl;kjd";
        int limit = 3;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int>();
        Assert.Throws<ArgumentException>(() => SearchDoc.findMatches(pattern, ms, limit));
    } 


    [Fact]
    public void InvalidLowerBound()
    {
        string pattern = "cat";
        string text = "here asdfa ckjcv cat with rag asl;kjd";
        int limit = 1;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int>();
        Assert.Throws<ArgumentException>(() => SearchDoc.findMatches(pattern, ms, limit, 55));
    } 

    [Fact]
    public void InvalidUpperBound()
    {
        string pattern = "cat";
        string text = "here asdfa ckjcv cat with rag asl;kjd";
        int limit = 1;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int>();
        Assert.Throws<ArgumentException>(() => SearchDoc.findMatches(pattern, ms, limit, 2, 77));
    } 

    [Fact]
    public void NegativeBound()
    {
        string pattern = "cat";
        string text = "here asdfa ckjcv cat with rag asl;kjd";
        int limit = 1;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int>();
        Assert.Throws<ArgumentException>(() => SearchDoc.findMatches(pattern, ms, limit, -3));
    } 




    [Fact]
    public void TestMultipleCloseMatchesLargetText() {
        string pattern = "needle";
        string text = new string(' ', 200)+ "needle" + new string('a', 55) + "neadle" + new string('b', 55) + "neddle" + new string(' ', 200);
        int limit = 1;
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        List<int> result = new List<int> { 200, 261, 322 };
        Assert.Equal(result, SearchDoc.findMatches(pattern, ms, limit));
    }

    [Fact]
    public void TestOverlappingLargetText() {
        string pattern = "aaa";
        string text = new string('a', 1000);
        int limit = 1;
        var expected = new List<int>();
        for (int i = 0; i <= 997; i++)
        {
            expected.Add(i);
        }
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        Assert.Equal(expected, SearchDoc.findMatches(pattern, ms, limit));
    }

    [Fact]
    public void TestHighLimitLargeText() {
        string pattern = "needle";
        string text = new string('a', 1000) + "neadle" + new string('b', 1000);
        int limit = 5;
        var expected = new List<int> {996,  999, 1000, 1003, 1004 };
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        Assert.Equal(expected, SearchDoc.findMatches(pattern, ms, limit));
    }

    [Fact]
    public void TestWithFileNoLimit() {
        string pattern = "expectation";
        int limit = 0;
        var expected= new List<int> {1678, 2848};
        Directory.SetCurrentDirectory("/");
        var testFile = File.Open("/Users/thomas/Documents/SummerProject/PdfSearch/examples/txt_files/testGroup/test3.txt", FileMode.Open);
        Assert.Equal(expected, SearchDoc.findMatches(pattern, testFile, limit));
        testFile.Close();
    }

    [Fact]
    public void TestWithFileWithLimit() {
        string pattern = "expectation";
        int limit = 2;
        var expected = new List<int> {1678, 2848};
        Directory.SetCurrentDirectory("/");
        var testFile = File.Open("/Users/thomas/Documents/SummerProject/PdfSearch/examples/txt_files/testGroup/test3.txt", FileMode.Open);
        Assert.Equal(expected, SearchDoc.findMatches(pattern, testFile, limit));
        testFile.Close();
    }

    [Fact]
    public void TestWithFileBounds() {
        string pattern = "expectation";
        int limit = 2;
        var expected = new List<int> {1678};
        Directory.SetCurrentDirectory("/");
        var testFile = File.Open("/Users/thomas/Documents/SummerProject/PdfSearch/examples/txt_files/testGroup/test3.txt", FileMode.Open);
        Assert.Equal(expected, SearchDoc.findMatches(pattern, testFile, limit, 500, 2000));
        testFile.Close();
    }



    [Fact]
    public void BasicGetQuestion() {
        string text = '\n' + "Q1. ";
        var expected = new List<int> {0};
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        Assert.Equal(expected, SearchDoc.getQuestion(ms, 1));
    }

    [Fact]
    public void GetQuestionBothIndices() {
        string text = "Hello\n" + "Q1. This is Q1. \nQ2. This is next";
        var expected = new List<int> {5, 22};
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        Assert.Equal(expected, SearchDoc.getQuestion(ms, 1));
    }

    [Fact]
    public void MultiIndicesLastIndex() {
        string text = "Hello\n" + "Q1. This is Q1. \nQ2. This is next";
        var expected = new List<int> {22};
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        Assert.Equal(expected, SearchDoc.getQuestion(ms, 2));
    }


    [Fact]
    public void MultiIndicesHigherQeustion() {
        string text = "Hello\n" + "Q1. This is Q1. \nQ2. This is next \nQ3. \nQ4. \nQ5. ";
        var expected = new List<int> {45, 50};
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        Assert.Equal(expected, SearchDoc.getQuestion(ms, 4));
    }

    [Fact]
    public void QuestionNotFound() {
        string text = "Hello\n" + "Q1. This is Q1. \nQ2. This is next \nQ3. \nQ4. \nQ5. ";
        var expected = new List<int> ();
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        Assert.Equal(expected, SearchDoc.getQuestion(ms, 7));
    }

    [Fact]
    public void RealFileFirstQuestion() {
        var expected = new List<int> {83, 1699};
        Directory.SetCurrentDirectory("/");
        var testFile = File.Open("/Users/thomas/Documents/SummerProject/PdfSearch/examples/txt_files/testGroup/test3.txt", FileMode.Open);
        Assert.Equal(expected, SearchDoc.getQuestion(testFile, 1));
        testFile.Close();
    }

    [Fact]
    public void RealFileNoQuestion() {
        var expected = new List<int> ();
        Directory.SetCurrentDirectory("/");
        var testFile = File.Open("/Users/thomas/Documents/SummerProject/PdfSearch/examples/txt_files/testGroup/test3.txt", FileMode.Open);
        Assert.Equal(expected, SearchDoc.getQuestion(testFile, 0));
        testFile.Close();
    }

    [Fact]
    public void RealFileLastQuestion() {
        var expected = new List<int> {1699};
        Directory.SetCurrentDirectory("/");
        var testFile = File.Open("/Users/thomas/Documents/SummerProject/PdfSearch/examples/txt_files/testGroup/test3.txt", FileMode.Open);
        Assert.Equal(expected, SearchDoc.getQuestion(testFile, 2));
        testFile.Close();
    }


    [Fact]
    public void TestWithQuestionsBounds() {
        string pattern = "expectation";
        int limit = 2;
        var expected = new List<int> {1678};
        Directory.SetCurrentDirectory("/");
        var testFile = File.Open("/Users/thomas/Documents/SummerProject/PdfSearch/examples/txt_files/testGroup/test3.txt", FileMode.Open);
        List<int> bounds = SearchDoc.getQuestion(testFile, 1);
        Assert.Equal(expected, SearchDoc.findMatches(pattern, testFile, limit, bounds[0], bounds[1]));
        testFile.Close();
    }
}

    