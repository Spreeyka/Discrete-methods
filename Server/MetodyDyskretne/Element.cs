using System.Collections.Generic;
using System.Linq;

namespace MetodyDyskretne
{
    class Element
    {
        public int id;
        public int idChanged;
        public List<Element> neighbours;
        public bool IsColored { get; set; } = false;
        public bool IsChecked { get; set; } = false;
        public bool IsShuffled { get; set; } = false;

        public int Energy { get; set; } = 0;

        public Element(int id)
        {
            this.id = id;
        }

        public void SetEnergy()
        {
            foreach (Element el in neighbours.Where(n => n.id != id))
            {
                ++Energy;
            }
        }
    }
}
