using System;
using System.Collections.Generic;
using LibSvmDotNet;

namespace Classification2
{

    internal class Program
    {

        private static void Main()
        {
            // Create random data
            var r = new Random();
            const int trainCount = 500;
            const int testCount = 100;

            var trainNodes = new List<Node[]>();
            var trainLabels = new List<double>();
            var testNodes = new List<Node[]>();
            var testLabels = new List<double>();

            for (var l = 0; l < 2; l++)
            {
                for (var i = 0; i < trainCount; i++)
                {
                    // Create decimal value 0 or greater but less than 2
                    var v = r.NextDouble() + l;
                    trainNodes.Add(new[] { new Node { Index = 0, Value = v } });
                    trainLabels.Add(l);
                }
                for (var i = 0; i < testCount; i++)
                {
                    // Create decimal value 0 or greater but less than 2
                    var v = r.NextDouble() + l;
                    testNodes.Add(new[] { new Node { Index = 0, Value = v } });
                    testLabels.Add(l);
                }
            }

            // Load training data and test data set
            using (var train = Problem.FromSequence(trainNodes, trainLabels))
            using (var test = Problem.FromSequence(testNodes, testLabels))
            {
                // Configure parameter
                var param = new Parameter
                {
                    SvmType = SvmType.CSVC,
                    KernelType = KernelType.RBF,
                    Gamma = 0.05d,
                    C = 5d,
                    CacheSize = 100,
                    Degree = 3,
                    Coef0 = 0,
                    Nu = 0.5,
                    Epsilon = 1e-3,
                    P = 0.1,
                    Shrinking = true,
                    Probability = false,
                    WeightLabel = new int[0],
                    Weight = new double[0]
                };

                var message = LibSvm.CheckParameter(train, param);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    Console.WriteLine($"Error: {message} for train problem");
                    return;
                }

                message = LibSvm.CheckParameter(test, param);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    Console.WriteLine($"Error: {message} for test problem");
                    return;
                }

                // Train training data
                using (var model = LibSvm.Train(train, param))
                {
                    var correct = 0;
                    var total = 0;
                    var x = test.X;
                    for (var i = 0; i < test.Length; i++)
                    {
                        // Get vector from test data
                        var array = x[i];

                        // Get classification result (returns label)
                        var ret1 = (int)LibSvm.Predict(model, array);
                        if (ret1 == (int)testLabels[i])
                            correct++;

                        total++;
                    }

                    Console.WriteLine($"Accuracy: {correct / (double)total * 100}%");
                }
            }
        }

    }

}
