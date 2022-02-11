using System;
using System.Collections.Generic;

namespace NetworkCore
{
    class Neuron
    {
        public List<double> Weights { get; set; }
        public double Output { get; set; }
        public double Input { get; set; }
        public double OutputError { get; set; }

        public Neuron()
        {
            Weights = new List<double>();
            Output = 0;
            OutputError = 0;
            Input = 0;
        }

        public void RunNeuron(List<double> Input)
        {
            if (Weights.Count <= 0) return;

            Output = 0;
            this.Input = 0;
            for (int Pos = 0; Pos < Weights.Count; Pos++)
                Output += (Input[Pos] * Weights[Pos]);

            this.Input = Output;
            Output = FuncActivation(Output);
        }

        private double FuncActivation(double Input)
        {
            return 1 / (1 + Double.Parse(Math.Exp(-Input).ToString().Split('E')[0]));
        }
    }
}
