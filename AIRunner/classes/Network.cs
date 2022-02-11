using System;
using System.Collections.Generic;
using System.IO;

namespace NetworkCore
{
    class Network
    {
        public List<double> Errors { get; set; }
        public double Coefficient { get; set; }
        private Random Rand = new Random();
        private List<List<Neuron>> Neurons { get; set; }

        private int CountInputNeurons = 0;
        private int CountOutputNeurons = 0;
        private int CountHiddenNeurons = 0;
        private int CountHiddenLayers = 0;

        public Network(string FileName)
        {
            Load(FileName);
        }

        public Network(int CountInputNeurons, int CountHiddenNeurons, int CountHiddenLayers, int CountOutputNeurons, bool NeuronsBias = false)
        {
            Create(CountInputNeurons, CountHiddenNeurons, CountHiddenLayers, CountOutputNeurons, NeuronsBias);
        }

        private void Create(int CountInputNeurons, int CountHiddenNeurons, int CountHiddenLayers, int CountOutputNeurons, bool NeuronsBias = false)
        {
            Errors = new List<double>();
            Coefficient = 0.1;
            this.CountHiddenNeurons     = CountHiddenNeurons;
            this.CountHiddenLayers      = CountHiddenLayers;
            this.CountInputNeurons      = CountInputNeurons;
            this.CountOutputNeurons     = CountOutputNeurons;
            Neurons = new List<List<Neuron>>();

            int CountLayers = (2 + CountHiddenLayers);
            int BackCount = 0;
            for (int Layers = 0; Layers < CountLayers; Layers++)
            {
                List<Neuron> TempNeurons = new List<Neuron>();
                int Count;

                if (Layers <= 0) Count = CountInputNeurons;
                else if (Layers >= (CountLayers - 1)) Count = CountOutputNeurons;
                else Count = CountHiddenNeurons;

                for (int PosNeuron = 0; PosNeuron < Count; PosNeuron++)
                    TempNeurons.Add(new Neuron() { Weights = GenerateWeightsList(BackCount) });

                if (NeuronsBias) TempNeurons.Add(new Neuron() { Output = 1 });

                BackCount = TempNeurons.Count;

                Neurons.Add(TempNeurons);
            }
        }

        private List<double> GenerateWeightsList(int count)
        {
            List<double> Weights = new List<double>();
            for (int pos = 0; pos < count; pos++)
                Weights.Add(Math.Round((double)(Rand.Next(-10, 10) * 0.1), 3));
            return Weights;
        }

        public List<double> ForwardPropagation(List<double> Input)
        {
            if (Input.Count > CountInputNeurons)
                throw new Exception("Входных значений больше, чем нейронов");

            for (int Pos = 0; Pos < Input.Count; Pos++)
                Neurons[0][Pos].Output = Input[Pos];

            for(int Pos = 1; Pos < Neurons.Count; Pos++)
            {
                List<double> TempInput = new List<double>();
                foreach (Neuron neur in Neurons[Pos - 1])
                    TempInput.Add(neur.Output);

                foreach (Neuron neur in Neurons[Pos])
                    neur.RunNeuron(TempInput);
            }

            List<double> Output = new List<double>();
            foreach (Neuron neur in Neurons[Neurons.Count - 1])
                Output.Add(neur.Output);

            return Output;
        }

        public List<double> GetError(List<double> Expected)
        {
            if (Expected.Count > CountOutputNeurons)
                throw new Exception("Значений больше, чем выходных нейронов");

            List<double> Error = new List<double>();

            for(int Pos = 0; Pos < CountOutputNeurons; Pos++)
                Error.Add(Expected[Pos] - Neurons[Neurons.Count - 1][Pos].Output);

            Errors = new List<double>(Error);
            return Error;
        }

        public void BackPropagation(List<double> Input)
        {
            if (Input.Count > CountOutputNeurons)
                throw new Exception("Значений больше, чем выходных нейронов");

            for (int Pos = 0; Pos < CountOutputNeurons; Pos++)
                Neurons[Neurons.Count - 1][Pos].OutputError = Input[Pos];

            for (int Pos = (Neurons.Count - 1); Pos > 0; Pos--)
            {
                foreach (Neuron neur in Neurons[Pos - 1])
                    neur.OutputError = 0;

                foreach (Neuron neur in Neurons[Pos])
                    for(int i = 0; i < neur.Weights.Count; i++)
                        Neurons[Pos - 1][i].OutputError += neur.OutputError * neur.Weights[i];
                    
            }
        }

        public void UpdateWeights()
        {
            for (int Pos = 1; Pos < Neurons.Count; Pos++)
                foreach (Neuron neur in Neurons[Pos])
                    for (int i = 0; i < neur.Weights.Count; i++)
                        neur.Weights[i] += Coefficient * neur.OutputError * (/*производная*/ neur.Output * (1 - neur.Output)) * Neurons[Pos - 1][i].Output;
        }

        public List<double> StepTraining(List<double> Input, List<double> Expected)
        {
            List<double> Out = ForwardPropagation(Input);
            BackPropagation(GetError(Expected));
            UpdateWeights();
            return Out;
        }

        public void Save(string FileName)
        {
            List<string> Out = new List<string>();
            Out.Add(CountInputNeurons + ":" + CountHiddenNeurons + ":" + CountHiddenLayers + ":" + CountOutputNeurons);

            int pos = 0;
            foreach (List<Neuron> neurons in Neurons)
            {
                if (pos > 0)
                {
                    string Temp = "";
                    foreach (Neuron neuron in neurons)
                    {
                        List<double> TEMP = new List<double>();
                        foreach (double w in neuron.Weights)
                            TEMP.Add(Math.Round(w, 15));
                        Temp += String.Join(":", TEMP) + "|";
                    }

                    Out.Add(Temp);
                }

                pos++;
            }


            File.WriteAllLines(FileName, Out);
        }


        private void Load(string FileName)
        {
            string[] file = File.ReadAllLines(FileName);

            string[] header = file[0].Split(':');
            Create(Int32.Parse(header[0]), Int32.Parse(header[1]), Int32.Parse(header[2]), Int32.Parse(header[3]));

            for(int x = 1; x < file.Length; x++)
            {
                if (!String.IsNullOrEmpty(file[x]))
                {
                    string[] neurons = file[x].Split('|');
                    for (int y = 0; y < neurons.Length; y++)
                    {
                        if (!String.IsNullOrEmpty(neurons[y]))
                        {
                            string[] neuron = neurons[y].Split(':');
                            for (int w = 0; w < neuron.Length; w++)
                                if (!String.IsNullOrEmpty(neuron[w]))
                                    Neurons[x][y].Weights[w] = Double.Parse(neuron[w]);
                        }
                    }
                }
            }
        }
    }
}
