using System.Collections.Generic;

namespace Retseptiraamat
{
    public class RetseptiKategooria : List<Retsept>
    {
        public string Nimetus { get; set; }

        public RetseptiKategooria(string nimetus, List<Retsept> retseptid) : base(retseptid)
        {
            Nimetus = nimetus;
        }
    }
}
