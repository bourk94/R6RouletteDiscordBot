using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace R6_Roulette_Bot
{
    public class BdDefi : IEnumerable<Defi>
    {
        public List<Defi> listeDefi;

        public BdDefi()
        {
            listeDefi = new List<Defi>();
        }

        public List<Defi> getListeDefie()
        {
            return listeDefi;
        }

        public void setListeDefie(List<Defi> _listeDefi)
        {
            listeDefi = _listeDefi;
        }

        public void Add(Defi _defi)
        {
            Defi unDefi = _defi;
            listeDefi.Add(unDefi);
        }

        public void supprimer(Defi _defi)
        {
            Defi unDefi = _defi;
            listeDefi.Remove(unDefi);
        }

        public void supprimer(int _position)
        {
            listeDefi.RemoveAt(_position);
        }

        public void modifier(int _position, Defi _defi)
        {
            Defi unDefi = _defi;
            listeDefi[_position] = unDefi;
        }

        public int indexOf(Defi _defi)
        {
            Defi unDefi = _defi;
            return listeDefi.IndexOf(unDefi);
        }
        public Defi lire(Defi _defi)
        {
            Defi unDefi = _defi;
            return listeDefi[indexOf(unDefi)];
        }

        public Defi lire(int _position)
        {
            return listeDefi[_position];
        }

        public int size()
        {
            return listeDefi.Count;
        }

        public override string ToString()
        {
            return listeDefi.ToString();
        }

        public void AddRange(params Defi[] _defi)
        {
            listeDefi.AddRange(_defi);
        }

        public IEnumerator<Defi> GetEnumerator()
        {
            return listeDefi.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
