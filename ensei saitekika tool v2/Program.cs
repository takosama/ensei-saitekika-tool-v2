using System;
using Google.OrTools.LinearSolver;
using System.Linq;
namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var solver = Solver.CreateSolver("IntegerProgramming", "CBC_MIXED_INTEGER_PROGRAMMING"))
            {
                if (solver == null)
                {
                    Console.WriteLine("ソルバーを初期化できませんでした。");
                    return;
                }
                var objective = solver.Objective();
                objective.SetMinimization();
                int[] sigen = new int[] { 80, 20, 250, 280 };
                int[] Time = new int[] { 40, 90, 300, 500 };

                var mokuhyou = 10000;


                //カウント
                Variable TotalTime = solver.MakeIntVar(0, double.PositiveInfinity, "");
                Variable[] Counts = new Variable[4];
                for (int i = 0; i < Counts.Length; i++)
                    Counts[i] = solver.MakeIntVar(0, double.PositiveInfinity, "");


                //目的関数
                objective.SetCoefficient(TotalTime, 1);

                //制約式
                //資源合計を目標以上に
                var e1 = solver.MakeConstraint(mokuhyou, double.PositiveInfinity);
                foreach (var tmp in Counts.Zip(sigen, (a, b) => (count: a, sigen: b)))
                    e1.SetCoefficient(tmp.count, tmp.sigen);
                e1.SetCoefficient(TotalTime, 1.0/3.0);

                //同時出撃防止遠征制約
                Constraint[] e2 = new Constraint[Counts.Length];
                for (int i = 0; i < Counts.Length; i++)
                {
                    e2[i] = solver.MakeConstraint(0, double.PositiveInfinity);
                    e2[i].SetCoefficient(TotalTime, 1);
                    e2[i].SetCoefficient(Counts[i], -(double)Time[i]);

                }



                var e3 = solver.MakeConstraint(double.NegativeInfinity, 0);
                foreach (var tmp in Counts.Zip(Time, (a, b) => (count: a, time: b)))
                    e3.SetCoefficient(tmp.count, tmp.time);
                e3.SetCoefficient(TotalTime, -3);


                var reslt = solver.Solve();
                if (reslt != Solver.OPTIMAL)
                { 
                    Console.WriteLine("ソルバーで解けませんでした。");
                    return;
                }

                foreach (var Count in Counts)
                {
                    Console.WriteLine(Count.SolutionValue());
                }

                int k = 0;

                double sum = 0;
                foreach (var Count in Counts)
                {
                    sum += Count.SolutionValue() * sigen[k];
                    k++;
                }
                Console.WriteLine((int)(sum + TotalTime.SolutionValue() * (1.0 / 3.0)));
                Console.WriteLine(TotalTime.SolutionValue());
                k = 0;
                sum = 0;
                foreach (var Count in Counts)
                {
                    sum += Count.SolutionValue() * Time[k];
                    k++;
                }

                Console.WriteLine(sum);

            }
            return;
        }
    }
}