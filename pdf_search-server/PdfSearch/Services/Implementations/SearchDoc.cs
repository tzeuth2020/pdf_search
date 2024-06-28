namespace SearchServices;

public class SearchDoc {

    /** finds all indices in the text which begin a section which have an edit distance atmost limit away from the pattern, 
     optionally can specify start and end indices to search only part of the text  **/
    public static List<int> findMatches(string pattern, string text, int limit, int start = 0, int end = -1) {

        List<int> indices = new List<int>();

        if (pattern.Length == 0 || text.Length == 0) {
            return indices;
        }

        if (pattern.Length > 32) {
            throw new ArgumentException("Pattern is too long");
        }

        if (limit >= pattern.Length || limit < 0) {
            throw new ArgumentException("Limit is out of range");
        }

        Dictionary<int, int> charMap = new Dictionary<int, int>();

        char[] p = pattern.ToCharArray();
        char[] t = text.ToCharArray();

        foreach (char c in p) {
            if (!charMap.ContainsKey(c)) {
                charMap.Add(c, ~0);
            }
        }
        for (int i = 0; i < p.Length; i++) {
            charMap[p[i]] &= ~(1 << i);
        }

        if (start > text.Length || end > text.Length || start < -1 || end < -1) {
            throw new ArgumentException("Bounds outside of file Length");
        }

        long stop = end != -1 ? end : text.Length;

        
        int[] R = new int[limit + 1];
        for (int i = 0; i < R.Length; i ++) {
            R[i] = ~1;
        }
        for (int i = start; i < stop; i++) {
            int old_Rd1 = R[0];

            int textByte = t[i];
            if (!charMap.ContainsKey(textByte)) {
                charMap.Add(textByte, ~0);
            }

            R[0] |= charMap[textByte];
            R[0] <<= 1;

            for (int d = 1; d <= limit; d++) {
                int tmp = R[d];
                R[d] = (old_Rd1 & (R[d] | charMap[textByte])) << 1;
                old_Rd1 = tmp;
            }

            if (0 == (R[limit] & (1 << pattern.Length))) {
                indices.Add(i - p.Length + 1);
            }
        }

        return indices;
    }

    // same as function above but only looks for the first exact match of the patern in the text
    private static int firstMatch(string pattern, string text, int start = 0) {

        List<int> indices = new List<int>();

        Dictionary<int, int> charMap = new Dictionary<int, int>();

        char[] p = pattern.ToCharArray();
        char[] txt = text.ToCharArray();
        long t = text.Length;

        foreach (char c in p) {
            if (!charMap.ContainsKey(c)) {
                charMap.Add(c, ~0);
            }
        }
        for (int i = 0; i < p.Length; i++) {
            charMap[p[i]] &= ~(1 << i);
        }

        int R = ~1;

        for (int i = start; i < t; i++) {
            int old_Rd1 = R;

            int textByte = text[i];
            if (!charMap.ContainsKey(textByte)) {
                charMap.Add(textByte, ~0);
            }

            R |= charMap[textByte];
            R <<= 1;


            if (0 == (R & (1 << pattern.Length))) {
                return i - p.Length + 1 + start;
            }
        }

        return -1;
    }

    
    // find the start and end of a given question number in the file 
    public static List<int> getQuestion(string text, int number, int start = 0) {
        List<int> result = new List<int>();
        if (number < 0) {
            return result;
        }

        string firstQuestion = "\n" + "Q" + number.ToString() + ".";
        string nextQuestion = "\n" + "Q" + (number + 1).ToString() + ".";

        int firstIndex = firstMatch(firstQuestion, text); 

        if (firstIndex == -1) {
            return result;
        }

        result.Add(firstIndex);

        int secondIndex = firstMatch(nextQuestion, text);

        if (secondIndex == -1) {
            return result;
        }

        result.Add(secondIndex);
        return result;
    }
}