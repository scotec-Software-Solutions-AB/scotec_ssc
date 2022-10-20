#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#endregion


namespace Scotec.XMLDatabase.ReaderWriter.Xml
{
    internal sealed class XmlDataSet : IDataSet
    {
        private readonly List<IDataObject> _dataObjects;


        #region Constructor

        public XmlDataSet()
        {
            _dataObjects = new List<IDataObject>();
        }


        public XmlDataSet(List<IDataObject> dataObjects)
        {
            _dataObjects = dataObjects;
        }

        #endregion Constructor


        #region IEnumerable Implementation

        public IEnumerator<IDataObject> GetEnumerator()
        {
            return _dataObjects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dataObjects.GetEnumerator();
        }

        #endregion


        #region IDataSet Interface

        int IDataSet.Count
        {
            get { return _dataObjects.Count; }
        }


        IDataObject IDataSet.this[int index]
        {
            get
            {
                if (index >= _dataObjects.Count)
                    throw new Exception("Invalid index.");

                return _dataObjects[index];
            }
        }


        int IDataSet.Add(IDataObject dataObject)
        {
            if (_dataObjects.Contains(dataObject) == false)
                _dataObjects.Add(dataObject);
            return _dataObjects.Count;
        }


        int IDataSet.RemoveAt(int index)
        {
            if (index >= _dataObjects.Count)
                throw new Exception("Invalid index.");

            _dataObjects.RemoveAt(index);

            return _dataObjects.Count;
        }


        IDataSet IDataSet.Union(IDataSet dataSet)
        {
            var list1 = DataList;
            var list2 = ((XmlDataSet)dataSet).DataList;
            var list3 = list1.ToList();

            foreach (var o in list2.Where(o => !list3.Contains(o)))
            {
                list3.Add(o);
            }

            return new XmlDataSet(list3);
        }


        IDataSet IDataSet.Intersect(IDataSet dataSet)
        {
            var list1 = DataList;
            var list2 = ((XmlDataSet)dataSet).DataList;
            var list3 = list1.Where(list2.Contains).ToList();

            foreach (var o in list2.Where(o => list1.Contains(o) && !list3.Contains(o)))
            {
                list3.Add(o);
            }

            return new XmlDataSet(list3);
        }


        IDataSet IDataSet.Difference(IDataSet dataSet)
        {
            var list1 = DataList;
            var list2 = ((XmlDataSet)dataSet).DataList;
            var list3 = list1.Where(o => !list2.Contains(o)).ToList();

            return new XmlDataSet(list3);
        }


        IDataSet IDataSet.SymmetricDifference(IDataSet dataSet)
        {
            var list1 = DataList;
            var list2 = ((XmlDataSet)dataSet).DataList;
            var list3 = list1.Where(o => !list2.Contains(o)).ToList();
            list3.AddRange(list2.Where(o => !list1.Contains(o)));

            return new XmlDataSet(list3);
        }


        bool IDataSet.Contains(IDataObject dataObject)
        {
            return _dataObjects.Contains(dataObject);
        }

        #endregion IDataSet Interface


        #region XmlDataSet Implementation

        /// <summary>
        ///   Used to set the list in operations such as Union(), Intersect(),...
        /// </summary>
        private List<IDataObject> DataList
        {
            get { return _dataObjects; }
        }

        #endregion XmlDataSet Implementation
    }
}
