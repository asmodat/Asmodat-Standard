using System.Collections.Generic;

namespace AsmodatStandard.Extensions
{
    public static partial class StringEx
    {
        /// <summary>
        /// * matches zero or more characters.
        /// ? matches exactly one character.
        /// to escape wildcard use \? or \* within 'wildcardString'
        /// </summary>
        /// <param name="text"></param>
        /// <param name="wildcardString"></param>
        /// <returns>true if matches, false if not</returns>
        public static bool IsWildcardMatch(this string text, string wildcardString)
        {
            if(wildcardString.Contains("\\?") && text.Contains("?"))
            {
                text = text.Replace("?", "!");
                wildcardString = wildcardString.Replace("\\?", "!");
            }

            if (wildcardString.Contains("\\*") && text.Contains("*"))
            {
                text = text.Replace("*", "@");
                wildcardString = wildcardString.Replace("\\*", "@");
            }

            var isLike = true;
            byte matchCase = 0;
            char[] filter, reversedFilter, reversedWord, word;
            int currentPatternStartIndex = 0, 
                lastCheckedHeadIndex = 0, 
                lastCheckedTailIndex = 0, 
                reversedWordIndex = 0;
            var reversedPatterns = new List<char[]>();

            if (text == null || wildcardString == null)
                return false;

            word = text.ToCharArray();
            filter = wildcardString.ToCharArray();

            //Set which case will be used (0 = no wildcards, 1 = only ?, 2 = only *, 3 = both ? and *
            for (int i = 0; i < filter.Length; i++)
                if (filter[i] == '?')
                {
                    ++matchCase;
                    break;
                }

            for (int i = 0; i < filter.Length; i++)
                if (filter[i] == '*')
                {
                    matchCase += 2;
                    break;
                }

            if ((matchCase == 0 || matchCase == 1) && word.Length != filter.Length)
                return false;

            switch (matchCase)
            {
                case 0:
                    isLike = (text == wildcardString);
                    break;
                case 1:
                    for (int i = 0; i < text.Length; i++)
                    {
                        if ((word[i] != filter[i]) && filter[i] != '?')
                            isLike = false;
                    }
                    break;
                case 2:
                    //Search for matches until first *
                    for (int i = 0; i < filter.Length; i++)
                        if (filter[i] != '*')
                        {
                            if (filter[i] != word[i])
                                return false;
                        }
                        else
                        {
                            lastCheckedHeadIndex = i;
                            break;
                        }
                    
                    //Search Tail for matches until first *
                    for (int i = 0; i < filter.Length; i++)
                    {
                        if (filter[filter.Length - 1 - i] != '*')
                        {
                            if (filter[filter.Length - 1 - i] != word[word.Length - 1 - i])
                                return false;
                        }
                        else
                        {
                            lastCheckedTailIndex = i;
                            break;
                        }
                    }

                    //Create a reverse word and filter for searching in reverse. The reversed word and filter do not include already checked chars
                    reversedWord = new char[word.Length - lastCheckedHeadIndex - lastCheckedTailIndex];
                    reversedFilter = new char[filter.Length - lastCheckedHeadIndex - lastCheckedTailIndex];

                    for (int i = 0; i < reversedWord.Length; i++)
                        reversedWord[i] = word[word.Length - (i + 1) - lastCheckedTailIndex];
                    
                    for (int i = 0; i < reversedFilter.Length; i++)
                        reversedFilter[i] = filter[filter.Length - (i + 1) - lastCheckedTailIndex];

                    //Cut up the filter into seperate patterns, exclude * as they are not longer needed
                    for (int i = 0; i < reversedFilter.Length; i++)
                        if (reversedFilter[i] == '*')
                        {
                            if (i - currentPatternStartIndex > 0)
                            {
                                var pattern = new char[i - currentPatternStartIndex];
                                for (int j = 0; j < pattern.Length; j++)
                                    pattern[j] = reversedFilter[currentPatternStartIndex + j];

                                reversedPatterns.Add(pattern);
                            }
                            currentPatternStartIndex = i + 1;
                        }

                    //Search for the patterns
                    for (int i = 0; i < reversedPatterns.Count; i++)
                    {
                        for (int j = 0; j < reversedPatterns[i].Length; j++)
                        {
                            if (reversedWordIndex > reversedWord.Length - 1)
                                return false;


                            if (reversedPatterns[i][j] != reversedWord[reversedWordIndex + j])
                            {
                                reversedWordIndex += 1;
                                j = -1;
                            }
                            else if (j == reversedPatterns[i].Length - 1)
                                reversedWordIndex = reversedWordIndex + reversedPatterns[i].Length;
                        }
                    }
                    break;

                case 3:
                    //Same as Case 2 except ? is considered a match
                    //Search Head for matches util first *
                    for (int i = 0; i < filter.Length; i++)
                    {
                        if (filter[i] != '*')
                        {
                            if (filter[i] != word[i] && filter[i] != '?')
                                return false;
                        }
                        else
                        {
                            lastCheckedHeadIndex = i;
                            break;
                        }
                    }
                    //Search Tail for matches until first *
                    for (int i = 0; i < filter.Length; i++)
                    {
                        if (filter[filter.Length - 1 - i] != '*')
                        {
                            if (filter[filter.Length - 1 - i] != word[word.Length - 1 - i] && filter[filter.Length - 1 - i] != '?')
                                return false;
                        }
                        else
                        {
                            lastCheckedTailIndex = i;
                            break;
                        }
                    }
                    // Reverse and trim word and filter
                    reversedWord = new char[word.Length - lastCheckedHeadIndex - lastCheckedTailIndex];
                    reversedFilter = new char[filter.Length - lastCheckedHeadIndex - lastCheckedTailIndex];

                    for (int i = 0; i < reversedWord.Length; i++)
                        reversedWord[i] = word[word.Length - (i + 1) - lastCheckedTailIndex];
                    
                    for (int i = 0; i < reversedFilter.Length; i++)
                        reversedFilter[i] = filter[filter.Length - (i + 1) - lastCheckedTailIndex];

                    for (int i = 0; i < reversedFilter.Length; i++)
                    {
                        if (reversedFilter[i] == '*')
                        {
                            if (i - currentPatternStartIndex > 0)
                            {
                                char[] pattern = new char[i - currentPatternStartIndex];
                                for (int j = 0; j < pattern.Length; j++)
                                {
                                    pattern[j] = reversedFilter[currentPatternStartIndex + j];
                                }
                                reversedPatterns.Add(pattern);
                            }

                            currentPatternStartIndex = i + 1;
                        }
                    }
                    //Search for the patterns
                    for (int i = 0; i < reversedPatterns.Count; i++)
                    {
                        for (int j = 0; j < reversedPatterns[i].Length; j++)
                        {
                            if (reversedWordIndex > reversedWord.Length - 1)
                                return false;

                            if (reversedPatterns[i][j] != '?' && reversedPatterns[i][j] != reversedWord[reversedWordIndex + j])
                            {
                                reversedWordIndex += 1;
                                j = -1;
                            }
                            else if (j == reversedPatterns[i].Length - 1)
                                reversedWordIndex = reversedWordIndex + reversedPatterns[i].Length;
                        }
                    }
                    break;
            }
            return isLike;
        }
    }
}
