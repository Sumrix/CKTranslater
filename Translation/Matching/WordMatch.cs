using System;
using System.Collections.Generic;
using Translation.Storages;

namespace Translation.Matching
{
    /// <summary>
    ///     Класс устанавливает соответствие между графемами слов
    /// </summary>
    public class WordMatch
    {
        private const float MinimumSimilarityToMatch = 0.68f;
        private readonly CalculationCell[,] calcCells;
        private IReadOnlyCollection<LettersMatch> matches;
        private float similarity = -1;

        private WordMatch(CalculationCell[,] steps)
        {
            this.calcCells = steps;
        }

        public IReadOnlyCollection<LettersMatch> LetterMatches
        {
            get
            {
                if (this.matches == null)
                {
                    this.matches = this.GetMatches();
                }

                return this.matches;
            }
        }

        public float Similarity
        {
            get
            {
                if (this.similarity == -1)
                {
                    this.GetMatches();
                }

                return this.similarity;
            }
        }

        public bool Success => this.Similarity > WordMatch.MinimumSimilarityToMatch;

        private static float SimilarityF(int? eng, int? rus)
        {
            return eng == null && rus == null ? 0 :
                eng == null ? DB.EngToRusSimilarities.EmptyEngToRus(rus.Value) :
                rus == null ? DB.EngToRusSimilarities.EmptyRusToEng(eng.Value) :
                DB.EngToRusSimilarities[eng.Value, rus.Value];
        }

        public static WordMatch Create(string word0, string word1, Language language0, Language language1)
        {
            CalculationCell[,] steps =
                WordMatch.FillCalculationArray(word0, word1, language0, language1, WordMatch.SimilarityF);

            return new WordMatch(steps);
        }

        //private float GetSimilarity()
        //{
        //    int index0 = this.word0.Length - 1;
        //    int index1 = this.word1.Length - 1;

        //    CalculationCell step = this.calcCells[index0, index1];

        //    return FVector.Max(
        //        step.Route0.Null,
        //        step.Route0.Value,
        //        step.Route1.Null,
        //        step.Route1.Value
        //    ).PathAverage;
        //}

        private IReadOnlyCollection<LettersMatch> GetMatches()
        {
            Stack<LettersMatch> matches = new Stack<LettersMatch>();

            int index0 = this.calcCells.GetLength(0) - 1;
            int index1 = this.calcCells.GetLength(1) - 1;

            CalculationCell lastCell = this.calcCells[index0, index1];
            CalculationCell lastValueCell = this.calcCells[index0 - 1, index1 - 1];
            FVector vector = FVector.Max(
                lastCell?.Route0?.Null,
                lastCell?.Route1?.Null,
                //lastValueCell?.Route0?.Null,
                lastValueCell?.Route0?.Value,
                //lastValueCell?.Route1?.Null,
                lastValueCell?.Route1?.Value
            );

            this.similarity = vector.PathAverage;

            for (; vector != null; vector = vector.Previous)
            {
                matches.Push(new LettersMatch(vector.Letters0 ?? "", vector.Letters1 ?? "", vector.Average));
            }

            return matches;
        }

        /// <summary>
        ///     Произвести вычисления по массивам применяя функцию
        /// </summary>
        /// <param name="numbers0">Массив чисел 0</param>
        /// <param name="numbers1">Массив чисел 1</param>
        /// <param name="F01">Функция между числами массива 0 и массива 1</param>
        /// <returns></returns>
        private static CalculationCell[,] FillCalculationArray(string word0,
            string word1,
            Language language0,
            Language language1,
            Func<int?, int?, float> F01)
        {
            // Массив вычислений
            int[] numbers0 = language0.ToIdentifiers(word0);
            int[] numbers1 = language1.ToIdentifiers(word1);

            CalculationCell[,] calcs01 = new CalculationCell[numbers0.Length + 1, numbers1.Length + 1];

            for (int index0 = 0; index0 <= numbers0.Length; index0++)
            for (int index1 = 0; index1 <= numbers1.Length; index1++)
            {
                // Определяем текущие буквы
                char? letter0 = index0 < word0.Length ? word0[index0] : (char?) null;
                char? letter1 = index1 < word1.Length ? word1[index1] : (char?) null;

                // Определяем текущие числа
                int? number0 = index0 < word0.Length ? numbers0[index0] : (int?) null;
                int? number1 = index1 < word1.Length ? numbers1[index1] : (int?) null;

                // Определяем значения функций для текущих чисел
                float f0N = F01(number0, null);
                float fN1 = F01(null, number1);
                float f01 = F01(number0, number1);

                // Для первой ячейки уникальная обработка
                if (index0 == 0 && index1 == 0)
                {
                    FVector valueStart = FVector.Start(f01, letter0, letter1);
                    calcs01[index0, index1] = new CalculationCell
                    {
                        Route0 =
                        {
                            Null = FVector.Start(f0N, letter0, null),
                            Value = valueStart
                        },
                        Route1 =
                        {
                            Null = FVector.Start(fN1, null, letter1),
                            Value = valueStart
                        }
                    };

                    continue;
                }

                // Получение предыдущих вычислений
                CalculationCell last0Cell = index0 > 0 ? calcs01[index0 - 1, index1] : null;
                CalculationCell last1Cell = index1 > 0 ? calcs01[index0, index1 - 1] : null;
                CalculationCell last01Cell = index0 > 0 && index1 > 0 ? calcs01[index0 - 1, index1 - 1] : null;

                // Подготовка
                FVector last01FVector = FVector.Max(
                    last01Cell?.Route0?.Value,
                    last01Cell?.Route1?.Value
                );
                FVector last01ValueVector = last01FVector?.New(f01, letter0, letter1);
                MaxFRoute last1Route = last1Cell?.Route1;
                MaxFRoute last0Route = last0Cell?.Route0;

                // Производим новые вычислений для текущих индексов
                calcs01[index0, index1] = new CalculationCell
                {
                    Route0 =
                    {
                        Null = FVector.Max(
                            last0Route?.Null?.Continue(f0N, letter0, null),
                            last01FVector?.New(f0N, letter0, null)
                        ),
                        Value = FVector.Max(
                            last0Route?.Null?.New(f01, letter0, letter1),
                            last0Route?.Value?.Continue(f01, letter0, null),
                            last01ValueVector
                        )
                    },
                    Route1 =
                    {
                        Null = FVector.Max(
                            last1Route?.Null?.Continue(fN1, null, letter1),
                            last01FVector?.New(fN1, null, letter1)
                        ),
                        Value = FVector.Max(
                            last1Route?.Null?.New(f01, letter0, letter1),
                            last1Route?.Value?.Continue(f01, null, letter1),
                            last01ValueVector
                        )
                    }
                };
            }

            return calcs01;
        }
    }
}