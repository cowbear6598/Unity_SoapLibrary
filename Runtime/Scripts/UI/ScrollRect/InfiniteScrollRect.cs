using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Soap.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public abstract class InfiniteScrollRect : MonoBehaviour
    {
        [SerializeField] protected Margin margin;
        
        // private enum ScrollType
        // {
        //     Horizontal = 0,
        //     Vertical = 1
        // }
        // [SerializeField] private ScrollType scrollType = ScrollType.Horizontal;
        
        private bool IsInitialize = false;
        
        [SerializeField] protected ScrollRect scrollRect = null;
        
        [SerializeField] protected RectTransform cellObj = null;
    
        protected int cellCount = 10;
        protected float cellWidth;
        protected float cellHeight;
    
        protected List<Action<InfiniteScrollCell>> OnCellUpdateList = new List<Action<InfiniteScrollCell>>();
    
        protected int row;
        protected int col;
    
        protected List<RectTransform> cellList = new List<RectTransform>();
        protected List<InfiniteScrollCell> infiniteScrollCellList = new List<InfiniteScrollCell>();
    
        public void Cell_AddListener(Action<InfiniteScrollCell> _cell)
        {
            OnCellUpdateList.Add(_cell);
            Cell_Refresh();
        }
    
        public void Cell_RemoveListener(Action<InfiniteScrollCell> _cell)
        {
            OnCellUpdateList.Remove(_cell);
        }
    
        public virtual void SetCellCount(int _count)
        {
            cellCount = Mathf.Max(0, _count);
    
            if (!IsInitialize)
                Initialize();
        }
    
        protected virtual void Initialize()
        {
            if (cellObj == null)
            {
                Debug.LogWarning("沒有偵測到單元物件");
                return;
            }
    
            IsInitialize = true;
            
            scrollRect.viewport.localScale = Vector3.one;
            scrollRect.viewport.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
            scrollRect.viewport.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
            scrollRect.viewport.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
            scrollRect.viewport.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
            scrollRect.viewport.anchorMin = Vector2.zero;
            scrollRect.viewport.anchorMax = Vector2.one;
            
            scrollRect.content.localScale = Vector3.one;
            scrollRect.content.offsetMax = Vector2.zero;
            scrollRect.content.offsetMin = Vector2.zero;
            scrollRect.content.anchorMin = Vector2.zero;
            scrollRect.content.anchorMax = Vector2.one;
            
            Rect cellTempRect = cellObj.rect;
            cellWidth = cellTempRect.width + margin.spaceX;
            cellHeight = cellTempRect.height + margin.spaceY;
        }
    
        private void Cell_Refresh()
        {
            for (int i = 0; i < cellList.Count; i++)
            {
                Cell_Update(cellList[i],infiniteScrollCellList[i]);
            }
        }

        protected virtual void Cell_Create()
        {
            
        }

        protected int row_All => Mathf.CeilToInt((float) cellCount / col);
        protected int col_All => Mathf.CeilToInt((float) cellCount / row);

        protected virtual void Cell_Update(RectTransform _cell, InfiniteScrollCell _infiniteScrollCell)
        {
            
        }

        protected virtual void OnValueChange(Vector2 _pos)
        {
        }
    }

    [Serializable]
    public class Margin
    {
        public int x;
        public int y;
        public int spaceX;
        public int spaceY;
    }
}