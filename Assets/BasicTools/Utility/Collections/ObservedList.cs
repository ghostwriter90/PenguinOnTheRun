using System;
using System.Collections.Generic;

namespace BasicTools.Utility
{
    /// <summary>
    /// Megfigyelt lista.
    /// </summary>
    /// <typeparam name="T">A lista elemeinek típusa.</typeparam>
    [Serializable]
    public class ObservedList<T> : List<T>
    {
        /// <summary>
        /// Lista elemének megváltozásának eseménye.
        /// A megváltozott elem indexét adja paraméterben.
        /// </summary>
        public event Action<int> Changed;
        /// <summary>
        /// A lista megváltozásának eseménye.
        /// </summary>
        public event Action Updated;

        /// <summary>
        /// Lista megváltozásának eseményének kiváltása.
        /// </summary>
        public void InvokeUpdated()
        {
            if (Updated != null)
                Updated();
        }

        /// <summary>
        /// Egy elem hozzáadása a listához.
        /// </summary>
        /// <param name="item">Hozzáadott elem.</param>
        public new void Add(T item)
        {
            base.Add(item);
            InvokeUpdated();
        }

        /// <summary>
        /// Egy elem törlése a listából.
        /// </summary>
        /// <param name="item">A törölt elem.</param>
        public new void Remove(T item)
        {
            base.Remove(item);
            InvokeUpdated();
        }

        /// <summary>
        /// Egy gyüjtemény hozzáadása a listához.
        /// </summary>
        /// <param name="collection">A hozzáadandó gyüjtemény.</param>
        public new void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);
            InvokeUpdated();
        }

        /// <summary>
        /// Elemek törlése egy adott ponttól.
        /// </summary>
        /// <param name="index">Az idexedik elemtől kezdve.</param>
        /// <param name="count">Ennyi elemet töröljünk a listából.</param>
        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            InvokeUpdated();
        }

        /// <summary>
        /// Lista teljes törlése.
        /// </summary>
        public new void Clear()
        {
            base.Clear();
            InvokeUpdated();
        }

        /// <summary>
        /// Elem beszúrása a megadott pozicióba.
        /// </summary>
        /// <param name="index">A megadott pozicó a listában ahova beszúrjuk az új elemet.</param>
        /// <param name="item">Ezt az elemet szúrjuk be.</param>
        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            InvokeUpdated();
        }

        /// <summary>
        /// Egy megadott indextől kezdve egy gyüjtemény elemeinek beszúrása.
        /// </summary>
        /// <param name="index">Ettől az indextől kezdve szúrjuk be az elemeket.</param>
        /// <param name="collection">Ezen gyüjtemény elemeit szúrjuk be.</param>
        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            base.InsertRange(index, collection);
            InvokeUpdated();
        }

        /// <summary>
        /// Töröljük az összes elemet, ami teljesíti a feltételt.
        /// </summary>
        /// <param name="match">A feltétel.</param>
        public new void RemoveAll(Predicate<T> match)
        {
            base.RemoveAll(match);
            InvokeUpdated();
        }

        /// <summary>
        /// Indexedik elem lekérdezése és módosítása.
        /// </summary>
        /// <param name="index">Elem indexe.</param>
        /// <returns>A lista indexedik helyén álló elem.</returns>
        public new T this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                base[index] = value;
                if (Changed != null)
                    Changed(index);
            }
        }
    }
}