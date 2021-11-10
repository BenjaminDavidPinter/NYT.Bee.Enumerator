using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WindowsInput;

namespace NYT.Bee.Enumerator
{
    class Program
    {
        public static int SystemWideDelay = 100;
        public static int EnterKeyWait = 100;
        public static List<Tuple<WindowsInput.Native.VirtualKeyCode, char>> todaysLetters = new List<Tuple<WindowsInput.Native.VirtualKeyCode, char>>()
        {
            Tuple.Create(WindowsInput.Native.VirtualKeyCode.VK_A, 'a'),
            Tuple.Create(WindowsInput.Native.VirtualKeyCode.VK_G, 'g'),
            Tuple.Create(WindowsInput.Native.VirtualKeyCode.VK_H, 'h'),
            Tuple.Create(WindowsInput.Native.VirtualKeyCode.VK_I, 'i'),
            Tuple.Create(WindowsInput.Native.VirtualKeyCode.VK_K, 'k'),
            Tuple.Create(WindowsInput.Native.VirtualKeyCode.VK_W, 'w'),
            Tuple.Create(WindowsInput.Native.VirtualKeyCode.VK_S, 's'),
        };

        public static Tuple<WindowsInput.Native.VirtualKeyCode, char> MustHaveLetter = Tuple.Create(WindowsInput.Native.VirtualKeyCode.VK_I, 'i');

        public static List<string> dictionary = File.ReadAllLines("Dictionary.txt").ToList();

        static void Main(string[] args)
        {
            System.Threading.Thread.Sleep(5000);
            dictionary = dictionary.Distinct().ToList();
            var thisDict = new List<string>(dictionary);

            for (int i = 0; i < dictionary.Count; i++)
            {
                dictionary[i] = dictionary[i].ToLower();
                if(!dictionary[i].All(x => todaysLetters.Select(y => y.Item2).Contains(x))
                    || !dictionary[i].Contains(MustHaveLetter.Item2)
                    || dictionary[i].Length < 4)
                {
                    thisDict.Remove(dictionary[i]);
                }
            }
            thisDict.OrderBy(x => x.Length);
           
            InputSimulator sim = new InputSimulator();
            foreach (var word in thisDict)
            {
                foreach (var charac in word)
                {
                    sim.Keyboard.KeyPress(todaysLetters.First(x => x.Item2 == charac).Item1);
                    System.Threading.Thread.Sleep(SystemWideDelay);
                }
                sim.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.RETURN);
                System.Threading.Thread.Sleep(EnterKeyWait);
            }
        }

        public static Collection<String> GetWordsOfLength(int length)
        {
            Collection<String> eligibleWords = new Collection<string>();
            foreach (var permus in ShuffleListExtension.CombinationsWithRepetition(todaysLetters
                .Select(x => x.Item2.ToString()), length)
                .Where(x => x.Length == length)
                .Where(x => x.Contains(MustHaveLetter.Item2) && dictionary.Contains(x)))
            {
                eligibleWords.Add(permus);
            }
            return eligibleWords;
        }
    }
}
