namespace SearchServices;

public class SearchDoc {

    /** finds all indices in the text which begin a section which have an edit distance atmost limit away from the pattern, 
     optionally can specify start and end indices to search only part of the text  **/
    public static List<int> findMatches(string pattern, Stream text, int limit, int start = -1, int end = -1) {

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
        int offset = start != -1 ? start : 0;
        if (start != -1) {
            text.Seek(start, SeekOrigin.Begin);
        }

        long stop = end != -1 ? end : text.Length;
        long lengthToCheck = stop - text.Position;
        
        int[] R = new int[limit + 1];
        for (int i = 0; i < R.Length; i ++) {
            R[i] = ~1;
        }
        for (int i = 0; i < lengthToCheck; i++) {
            int old_Rd1 = R[0];

            int textByte = text.ReadByte();
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
                indices.Add(offset + i - p.Length + 1);
            }
        }

        return indices;
    }

    // same as function above but only looks for the first exact match of the patern in the text
    private static int firstMatch(string pattern, Stream text) {
        long start = text.Position;
        List<int> indices = new List<int>();

        Dictionary<int, int> charMap = new Dictionary<int, int>();

        char[] p = pattern.ToCharArray();
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

        for (int i = 0; i < t; i++) {
            int old_Rd1 = R;

            int textByte = text.ReadByte();
            if (!charMap.ContainsKey(textByte)) {
                charMap.Add(textByte, ~0);
            }

            R |= charMap[textByte];
            R <<= 1;


            if (0 == (R & (1 << pattern.Length))) {
                return i - p.Length + 1 + (int) start;
            }
        }

        return -1;
    }

    
    // find the start and end of a given question number in the file 
    public static List<int> getQuestion(Stream file, int number) {
        List<int> result = new List<int>();
        long start = file.Position;
        if (number < 0) {
            return result;
        }

        string firstQuestion = "\n" + "Q" + number.ToString() + ".";
        string nextQuestion = "\n" + "Q" + (number + 1).ToString() + ".";

        int firstIndex = firstMatch(firstQuestion, file); 

        if (firstIndex == -1) {
            file.Seek(start, SeekOrigin.Begin);
            return result;
        }

        result.Add(firstIndex);


        int secondIndex = firstMatch(nextQuestion, file);
        file.Seek(start, SeekOrigin.Begin);

        if (secondIndex == -1) {
            return result;
        }

        result.Add(secondIndex);
        return result;


    }
}