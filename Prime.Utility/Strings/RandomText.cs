using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Utility
{
    public class Rnd
    {
        private Rnd() { }

        public static Rnd I { get { return Lazy.Value; } }
        private static readonly Lazy<Rnd> Lazy = new Lazy<Rnd>(() => new Rnd());

        /// <summary>
        /// Returns a random number between 0 and max
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public int Next(int max) { return Next(0, max); }

        /// <summary>
        /// Helper method, just rolls a virtual 100 sided dice. 'True' you won, 'False' you lost.
        /// </summary>
        /// <param name="percentageWin">The percentage of rolls you'd like to win.</param>
        /// <returns></returns>
        public bool DiceRoll(int percentageWin)
        {
            return Next(0, 99) < percentageWin;
        }

        /// <summary>
        /// Returns a random number between and including min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public int Next(int min, int max)
        {
            return min == max ? min : BaseRandom.Next(min, max);
        }

        public string Next(params string[] pars)
        {
            if (pars == null || pars.Length == 0)
                return null;

            return pars[Next(0, pars.Length - 1)];
        }

        /// <summary>
        /// http://stackoverflow.com/a/12388092/1318333
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public decimal Next(decimal from, decimal to)
        {
            var fromScale = 20;
            var toScale = 20;

            var scale = (byte)(fromScale + toScale);
            if (scale > 28)
                scale = 28;

            var r = new decimal(BaseRandom.Next(), BaseRandom.Next(), BaseRandom.Next(), false, scale);
            if (Math.Sign(from) == Math.Sign(to) || from == 0 || to == 0)
                return decimal.Remainder(r, to - from) + from;

            var getFromNegativeRange = (double)from + BaseRandom.NextDouble() * ((double)to - (double)from) < 0;
            return getFromNegativeRange ? decimal.Remainder(r, -from) + from : decimal.Remainder(r, to);
        }

        public T PickOne<T>(IEnumerable<T> objects)
        {
            return objects.OrderByRandom().FirstOrDefault();
        }

        public double Next(double min, double max)
        {
            return BaseRandom.NextDouble() * (max - min) + min;
        }

        public decimal Money(decimal min, decimal max)
        {
            return min == max ? min : (BaseRandom.Next((int)(min * 100), (int)(max * 100)) / (decimal)100);
        }

        private Random _baseRandom;
        public Random BaseRandom
        {
            get { return _baseRandom ?? (_baseRandom = new System.Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber))); }
        }

        /// <summary>
        /// Attempts to get a random value from the given Enum type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Enum<T>() where T : struct, IConvertible
        {
            return EnumExtensionMethods.RndEnum<T>();
        }
    }


    public class RandomText
    {
        /// <summary>
        /// Generates an 4 letter random text.
        /// </summary>
        public static string Generate(int length)
        {
            // Generate random text
            var s = "";
            var chars = "bcdghjklmnpqrstvwxyz0123456789".ToCharArray();
            for (var i = 0; i < length; i++)
                s += chars[Rnd.I.Next(chars.Length - 1)].ToString();
            return s;
        }

        //http://stackoverflow.com/a/1344255/1318333
        public static string GetUniqueKey(int maxSize)
        {
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_-=+{}[]?/><,.|\\~`".ToCharArray();
            var data = new byte[1];
            var crypto = RandomNumberGenerator.Create();
            crypto.GetBytes(data);
            data = new byte[maxSize];
            crypto.GetBytes(data);
            var result = new StringBuilder(maxSize);
            foreach (var b in data)
                result.Append(chars[b % (chars.Length)]);

            return result.ToString();
        }

        /// <summary>
        /// Create a pusedo-random number generator to be used for the instance of this object
        /// </summary>
        private Random _random;

        /// <summary>
        /// Create a list of possible end of sentance punctuation marks.  There are multiple
        /// periods so that more often than not a period ends the sentance.
        /// </summary>
        private char[] _endOfSentancePunctuation = new char[] {'.', '.', '.', '!', '.', '.', '?', '.', '.', '.'};

        /// <summary>
        /// Create a list of possible middle of the sentance punctuation marks.
        /// </summary>
        private char[] _middleOfSentancePunctuation = new char[] {','};

        /// <summary>
        /// Indicate whether or not to include random middle of the sentance punctuation
        /// marks in generated sentances
        /// </summary>
        public bool AddMiddleOfSentancePunctuationMarks = false;

        /// <summary>
        /// Indicates whether or not to add an end of sentance punctuation mark
        /// </summary>
        public bool AddEndOfSentancePunctuation = true;

        /// <summary>
        /// List of possible word lengths when user does not specify their own list
        /// </summary>
        private int[] _defaultWordLengths = new int[] {1, 2, 3, 3, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6, 6, 7, 7, 8};

        /// <summary>
        /// List of possible sentance lengths when user does not specify their own list
        /// </summary>
        private int[] _defaultSentanceLengths = new int[]
                                                    {3, 4, 5, 6, 6, 7, 7, 7, 8, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13};

        /// <summary>
        /// List of possible paragrpah lengths when user does not specify their own list
        /// </summary>
        private int[] _defaultParagraphLengths = new int[]
                                                     {
                                                         3, 4, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 10, 10, 11, 11, 11, 11,
                                                         12, 12, 13, 13, 14, 15
                                                     };

        public RandomText()
        {
            // Create a Random object for use during the existence of this object
            _random = new Random(DateTime.Now.Millisecond);
        }

        /// <summary>
        /// Generates a random string of a specfic length.
        /// </summary>        
        /// <returns>Returns a randomly generated string (lower case) of a specific length.</returns>
        public string String()
        {
            int wordLength = _defaultWordLengths.GetRandomElement<int>();
            return String(wordLength);
        }

        /// <summary>
        /// Generates a random string of a specfic length.
        /// </summary>
        /// <param name="length">The length of the random string to generate.</param>        
        /// <returns>Returns a randomly generated string (lower case) of a specific length.</returns>
        public string String(int length)
        {
            return String(length, false);
        }

        /// <summary>
        /// Generates a random string of a specfic length.
        /// </summary>
        /// <param name="length">The length of the random string to generate.</param>
        /// <param name="randomCharacterCase">If true, each character in the string will have
        /// an equal chance of being either upper case or lower case.  If false, the generated
        /// string will be all lower case.
        /// </param>
        /// <returns>Returns a randomly generated string of a specific length.</returns>
        public string String(int length, bool randomCharacterCase)
        {
            StringBuilder s = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                // Get a random integer in the range of ASCII lower case characters
                int c = _random.Next(97, 123);

                // If we are to randomize the case of the characters in the string
                // then if a random number is evenly divisible by two (a 1 in 2 chance)
                // then subtract 32 which converts the character to upper case
                if (randomCharacterCase && _random.Next()%2 == 0)
                    c -= 32;

                // Add the character to the string
                s.Append(Convert.ToChar(c));
            }

            return s.ToString();
        }

        /// <summary>
        /// Returns a random number within a specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>        
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be
        ///  greater than or equal to minValue.</param>               
        /// <returns>A 32-bit signed integer greater than or equal to minValue and less than maxValue;
        ///  that is, the range of return values includes minValue but not maxValue. If
        ///  minValue equals maxValue, minValue is returned.</returns>
        private int Number(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

        /// <summary>
        /// Generates a random sentance.
        /// </summary>        
        /// <returns>Returns a random sentance of random length and words from the default sentance and word lengths.</returns>
        public string Sentence()
        {
            int numberOfWords = _defaultSentanceLengths.GetRandomElement<int>();
            return Sentence(numberOfWords);
        }

        /// <summary>
        /// Generates a random sentance of a given number of words .
        /// </summary>
        /// <param name="numberOfWords">The number of words in the sentance</param>
        /// /// <returns>Returns a random sentance of the specified length.</returns>
        public string Sentence(int numberOfWords)
        {
            return Sentence(numberOfWords, _defaultWordLengths);
        }

        /// <summary>
        /// Generates a random sentance of a given number of words and possible word lengths.
        /// </summary>
        /// <param name="numberOfWords">The number of words in the sentance</param>
        /// <param name="possibleWordLengths">An array of integers representing the possible number of characters in each word</param>
        /// <returns>Returns a string containing a specified number of random words composed of random characters</returns>
        public string Sentence(int numberOfWords, int[] possibleWordLengths)
        {
            // Parameter validation
            if (possibleWordLengths == null)
                throw new ArgumentNullException("possibleWordLengths");
            else if (possibleWordLengths.Length < 1 || possibleWordLengths.Where(l => l < 0).Any())
                throw new ArgumentException(
                    "Parameter 'possibleWordLengths' must have one or more elements and cannot contain a negative number.");

            StringBuilder s = new StringBuilder();

            for (int i = 1; i <= numberOfWords; i++)
            {
                // Randomly choose a word length
                int wordLength = possibleWordLengths.GetRandomElement<int>();

                // Generate a random word of the indicated length
                s.Append(String(wordLength, false));

                if (AddMiddleOfSentancePunctuationMarks && i != numberOfWords && _random.Next()%15 == 0)
                    s.Append(_middleOfSentancePunctuation.GetRandomElement<char>());

                // Add a space if we aren't at the end of the sentance;
                s.Append(i == numberOfWords ? string.Empty : " ");
            }

            // Capitalize the first letter
            if (s.Length > 0)
                s[0] = char.ToUpper(s[0]);

            // Add a puncuation mark to the end of the sentance if desired
            if (AddEndOfSentancePunctuation)
                s.Append(_endOfSentancePunctuation.GetRandomElement<char>());

            return s.ToString();
        }

        /// <summary>
        /// Generates a random paragraph.
        /// </summary>
        public string Paragraph()
        {
            int numberOfSentances = _defaultParagraphLengths.GetRandomElement<int>();
            return Paragraph(numberOfSentances);
        }

        /// <summary>
        /// Generates a random paragraph of a given number of sentances.
        /// </summary>
        /// <param name="numberOfSentances">The number of sentances in the paragraph.</param>
        public string Paragraph(int numberOfSentances)
        {
            return Paragraph(numberOfSentances, _defaultSentanceLengths, _defaultWordLengths);
        }

        /// <summary>
        /// Generates a random paragraph of a given number of sentances.
        /// </summary>
        /// <param name="numberOfSentances">The number of sentances in the paragraph.</param>
        /// <param name="possibleSentanceLengths">An array of integers representing the possible number of words in each sentance.</param>
        public string Paragraph(int numberOfSentances, int[] possibleSentanceLengths)
        {
            return Paragraph(numberOfSentances, possibleSentanceLengths, _defaultWordLengths);
        }

        /// <summary>
        /// Generates a random paragraph of a given number of sentances.
        /// </summary>
        /// <param name="numberOfSentances">The number of sentances in the paragraph.</param>
        /// <param name="possibleSentanceLengths">An array of integers representing the possible number of words in each sentance.</param>
        /// <param name="possibleWordLengths">An array of integers representing the possible number of characters in each word</param>
        /// <returns>Returns a string containing a specified number of random sentances composed of random words and characters</returns>
        public string Paragraph(int numberOfSentances, int[] possibleSentanceLengths, int[] possibleWordLengths)
        {
            // Parameter validation
            if (possibleWordLengths == null)
                throw new ArgumentNullException("possibleWordLengths");
            else if (possibleWordLengths.Length < 1 || possibleWordLengths.Where(l => l < 0).Any())
                throw new ArgumentException(
                    "Parameter 'possibleWordLengths' must have one or more elements and cannot contain a negative number.");

            if (possibleSentanceLengths == null)
                throw new ArgumentNullException("possibleSentanceLengths");
            else if (possibleSentanceLengths.Length < 1 || possibleWordLengths.Where(l => l < 0).Any())
                throw new ArgumentException(
                    "Parameter 'possibleSentanceLengths' must have one or more elements and cannot contain a negative number.");

            StringBuilder s = new StringBuilder();

            for (int i = 1; i <= numberOfSentances; i++)
            {
                int numberOfWords = possibleSentanceLengths.GetRandomElement<int>();
                s.Append(Sentence(numberOfWords, possibleWordLengths));

                s.Append(i == numberOfSentances ? string.Empty : "  ");
            }

            return s.ToString();
        }
    }

    public static class Extensions
    {
        private static Random random = new Random();

        /// <summary>
        /// Gets a random element from a callection of elements that implments the IEnumerable interface
        /// </summary>
        /// <typeparam name="T">The type of each element in the collection</typeparam>
        /// <param name="list">A collection of elements of type T</param>
        /// <returns>Returns a random element from a collection</returns>        
        public static T GetRandomElement<T>(this IEnumerable<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            // Get the number of elements in the collection
            int count = list.Count();

            // If there are no elements in the collection, return the default value of T
            if (count == 0)
                return default(T);

            // Get a random index
            int index = random.Next(list.Count());

            // When the collection has 100 elements or less, get the random element
            // by traversing the collection one element at a time.
            if (count <= 100)
            {
                using (IEnumerator<T> enumerator = list.GetEnumerator())
                {
                    // Move down the collection one element at a time.
                    // When index is -1 we are at the random element location
                    while (index >= 0 && enumerator.MoveNext())
                        index--;

                    // Return the current element
                    return enumerator.Current;
                }
            }

            // Get an element using LINQ which casts the collection
            // to an IList and indexes into it.
            return list.ElementAt(index);
        }
    }
}