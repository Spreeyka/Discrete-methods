using System.Collections.Generic;
using System.Linq;

namespace MetodyDyskretne
{
    class NeighborhoodUtils
    {
        public static void SetNeumannNeighbours(int width, int height, int length, Element[,,] elements)
        {
            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 1; j < height - 1; j++)
                {
                    for (int k = 1; k < length - 1; k++)
                    {
                        elements[i, j, k].neighbours.Add(elements[i - 1, j, k]);
                        elements[i, j, k].neighbours.Add(elements[i, j + 1, k]);
                        elements[i, j, k].neighbours.Add(elements[i, j - 1, k]);
                        elements[i, j, k].neighbours.Add(elements[i + 1, j, k]);

                        elements[i, j, k].neighbours.Add(elements[i, j, k + 1]);
                        elements[i, j, k].neighbours.Add(elements[i, j, k - 1]);
                    }
                }
            }
        }

        public static void SetMooreNeighbours(int width, int height, int length, Element[,,] elements)
        {

            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 1; j < height - 1; j++)
                {
                    for (int k = 1; k < length - 1; k++)
                    {
                        //same
                        elements[i, j, k].neighbours.Add(elements[i - 1, j - 1, k]);
                        elements[i, j, k].neighbours.Add(elements[i - 1, j, k]);
                        elements[i, j, k].neighbours.Add(elements[i - 1, j + 1, k]);

                        elements[i, j, k].neighbours.Add(elements[i, j + 1, k]);
                        elements[i, j, k].neighbours.Add(elements[i + 1, j + 1, k]);

                        elements[i, j, k].neighbours.Add(elements[i + 1, j, k]);
                        elements[i, j, k].neighbours.Add(elements[i + 1, j - 1, k]);
                        elements[i, j, k].neighbours.Add(elements[i, j - 1, k]);

                        //warstwa niżej
                        elements[i, j, k].neighbours.Add(elements[i - 1, j - 1, k - 1]);
                        elements[i, j, k].neighbours.Add(elements[i - 1, j, k - 1]);
                        elements[i, j, k].neighbours.Add(elements[i - 1, j + 1, k - 1]);

                        elements[i, j, k].neighbours.Add(elements[i, j + 1, k - 1]);
                        elements[i, j, k].neighbours.Add(elements[i, j, k - 1]);
                        elements[i, j, k].neighbours.Add(elements[i + 1, j + 1, k - 1]);

                        elements[i, j, k].neighbours.Add(elements[i + 1, j, k - 1]);
                        elements[i, j, k].neighbours.Add(elements[i + 1, j - 1, k - 1]);
                        elements[i, j, k].neighbours.Add(elements[i, j - 1, k - 1]);

                        //warstwa wyżej
                        elements[i, j, k].neighbours.Add(elements[i - 1, j - 1, k + 1]);
                        elements[i, j, k].neighbours.Add(elements[i - 1, j, k + 1]);
                        elements[i, j, k].neighbours.Add(elements[i - 1, j + 1, k + 1]);

                        elements[i, j, k].neighbours.Add(elements[i, j + 1, k + 1]);
                        elements[i, j, k].neighbours.Add(elements[i, j, k + 1]);
                        elements[i, j, k].neighbours.Add(elements[i + 1, j + 1, k + 1]);

                        elements[i, j, k].neighbours.Add(elements[i + 1, j, k + 1]);
                        elements[i, j, k].neighbours.Add(elements[i + 1, j - 1, k + 1]);
                        elements[i, j, k].neighbours.Add(elements[i, j - 1, k + 1]);

                    }
                }
            }
        }
    }
}
