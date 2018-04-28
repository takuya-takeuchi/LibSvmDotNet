using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LibSvmDotNet;

namespace Classification10
{

    internal class Program
    {

        private static void Main()
        {
            // Create random data
            var r = new Random();
            const int trainCount = 500;
            const int testCount = 100;

            var temp = Path.GetTempPath();
            var trainDic = new StringBuilder();
            var testDic = new StringBuilder();
            var testDicAns = new List<int>();

            for (var l = 0; l < 10; l++)
            {
                for (var i = 0; i < trainCount; i++)
                {
                    // Create decimal value 0 or greater but less than 10
                    var v = r.NextDouble() + l;
                    trainDic.AppendLine($"{l} 1:{v}");
                }
                for (var i = 0; i < testCount; i++)
                {
                    // Create decimal value 0 or greater but less than 10
                    var v = r.NextDouble() + l;
                    testDic.AppendLine($"{l} 1:{v}");
                    testDicAns.Add(l);
                }
            }

            var tempTrainPath = Path.Combine(temp, "train");
            var tempTestPath = Path.Combine(temp, "test");

            using (var fs = new FileStream(tempTrainPath, FileMode.Create, FileAccess.Write, FileShare.Write))
            using (var sw = new StreamWriter(fs, Encoding.ASCII))
                sw.Write(trainDic.ToString());

            using (var fs = new FileStream(tempTestPath, FileMode.Create, FileAccess.Write, FileShare.Write))
            using (var sw = new StreamWriter(fs, Encoding.ASCII))
                sw.Write(testDic.ToString());

            // Load training data and test data set
            using (var train = Problem.FromFile(tempTrainPath))
            using (var test = Problem.FromFile(tempTestPath))
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
                        if (ret1 == testDicAns[i])
                            correct++;

                        total++;
                    }

                    Console.WriteLine($"Accuracy: {correct / (double)total * 100}%");
                }
            }
        }

    }

}
