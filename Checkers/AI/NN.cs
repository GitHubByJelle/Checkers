using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.ModelBinding;


namespace NNC
{
    class Neuron
    {
        public double[] weights;
        public double val;

        public Neuron(double[] wghts)
        {
            this.weights = wghts;
        }
    }

    class Layer
    {
        public Neuron[] neurons;
        private Random rand = new Random();

        public Layer(int nPrevious, int nCurrent)
        {
            this.neurons = new Neuron[nCurrent];

            if (nPrevious > 0)
            {
                for (int n = 0; n < nCurrent; n++)
                {
                    this.neurons[n] = new Neuron(generateWeights(nPrevious));
                }
            }
            else
            {
                for (int n = 0; n < nCurrent; n++)
                {
                    this.neurons[n] = new Neuron(new double[] { 0 });
                }
            }
        }

        public double[] generateWeights(int number)
        {
            // Instantiate an array of double
            double[] a = new double[number];

            // Store random floating point 
            // numbers in the array
            for (int i = 0; i < number; i++)
                a[i] = 2*rand.NextDouble()-1;
                //a[i] = 1;

            return a;
        }
    }

    class NN
    {
        Layer[] layers;

        public NN(int input_size, int[] hidden_layers_size, int output_size)
        {
            // Create Neural Network
            layers = new Layer[2 + hidden_layers_size.Length];

            // First layer
            layers[0] = new Layer(0, input_size);

            // Hidden layers
            for (int i = 1; i < 1 + hidden_layers_size.Length; i++)
            {
                if (i == 1)
                    layers[i] = new Layer(input_size, hidden_layers_size[i - 1]);
                else
                    layers[i] = new Layer(hidden_layers_size[i - 2], hidden_layers_size[i - 1]);
            }

            // Output layer
            layers[1 + hidden_layers_size.Length] = new Layer(hidden_layers_size[hidden_layers_size.Length - 1], output_size);
        }

        public double[] forward(double[] input)
        {
            for (int i = 0; i < this.layers.Length; i++)
            {
                if (i == 0)
                {
                    // Add values to neurons (of input)
                    for (int n = 0; n < this.layers[i].neurons.Length; n++)
                    {
                        this.layers[i].neurons[n].val = input[n];
                    }
                }
                else if (i == this.layers.Length - 1)
                {
                    // Get all values previous layer
                    double[] prev = getValuesLayer(this.layers[i - 1]);

                    // Calculate value for every node
                    for (int n = 0; n < this.layers[i].neurons.Length; n++)
                    {
                        // Multiply and use activation function
                        this.layers[i].neurons[n].val = multiSumArrays(prev, this.layers[i].neurons[n].weights);
                    }
                }
                else
                {
                    // Get all values previous layer
                    double[] prev = getValuesLayer(this.layers[i-1]);

                    // Calculate value for every node
                    for (int n = 0; n < this.layers[i].neurons.Length; n++)
                    {
                        // Multiply and use activation function
                        this.layers[i].neurons[n].val = ReLU(multiSumArrays(prev, this.layers[i].neurons[n].weights));
                    }
                }
            }

            // Send array of last values
            return getValuesLayer(this.layers[this.layers.Length-1]); ;
        }

        private int calculateTotalWeights()
        {
            int prev = this.layers[0].neurons.Length;
            int count = 0;
            int current;
            for (int i = 1; i < this.layers.Length; i++)
            {
                current = this.layers[i].neurons.Length;
                count += prev * current;
                prev = current;
            }

            return count;
        }

        public double[] getWeights()
        {
            double[] weights = new double[calculateTotalWeights()];

            int count = 0;
            for (int i = 1; i < this.layers.Length; i++)
            {
                for (int j = 0; j < this.layers[i].neurons.Length; j++)
                {
                    for (int k = 0; k < this.layers[i].neurons[j].weights.Length; k++)
                    {
                        weights[count] = this.layers[i].neurons[j].weights[k];

                        count++;
                    }
                }
            }

            return weights;
        }

        public void setWeights(double[] weights)
        {
            int count = 0;
            for (int i = 1; i < this.layers.Length; i++)
            {
                for (int j = 0; j < this.layers[i].neurons.Length; j++)
                {
                    for (int k = 0; k < this.layers[i].neurons[j].weights.Length; k++)
                    {
                        this.layers[i].neurons[j].weights[k] = weights[count];

                        count++;
                    }
                }
            }
        }

        private double ReLU(double value)
        {
            return Math.Max(0, value);
        }

        private double[] getValuesLayer(Layer layer)
        {
            double[] result = new double[layer.neurons.Length];

            for (int i = 0; i < layer.neurons.Length; i++)
                result[i] = layer.neurons[i].val;

            return result;
        }

        private double multiSumArrays(double[] arr1, double[] arr2)
        {
            double result = 0;

            for (int i = 0; i < arr1.Length; i++)
                result += arr1[i] * arr2[i];

            return result;
        }
    }
}
