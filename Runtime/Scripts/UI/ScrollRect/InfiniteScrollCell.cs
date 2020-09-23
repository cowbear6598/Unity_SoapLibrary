using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soap.UI
{
    public class InfiniteScrollCell : MonoBehaviour
    {
        private int _x;
        public int x => _x;
    
        private int _y;
        public int y => _y;
    
        private int _index;
        public int index => _index;
    
        private int _objIndex;
        public int objIndex => _objIndex;

        public void UpdatePos(int _x,int _y,int _index)
        {
            this._x = _x;
            this._y = _y;
            this._index = _index;
        }

        public void SetObjIndex(int _objIndex)
        {
            this._objIndex = _objIndex;
        }

        public void OnUpdateCell()
        {
            GetComponent<IInfiniteScrollCellUpdate>().OnInfiniteScrollCellUpdate(index);
        }
    }
}