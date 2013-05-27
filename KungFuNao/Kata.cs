using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Kinect.Toolbox;

namespace KungFuNao
{ //: IEnumerable<KataTechnique>
    class Kata
    {
        private string name;
        private IList<KataTechnique> techniques;

        public Kata( string name )
        {
            this.name = name;
            this.techniques = new List<KataTechnique>();
        }

        public void Add(KataTechnique technique)
        {
            this.techniques.Add(technique);
        }

        public int Size()
        {
            return this.techniques.Count;
        }

        /*
        public IEnumerator<KataTechnique> GetEnumerator()
        {
            return this.techniques.GetEnumerator();
        }
         * */
    }
}
