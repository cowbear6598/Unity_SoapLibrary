using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Soap.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class InfiniteScrollRect : MonoBehaviour
    {
        private enum ScrollType
        {
            Horizontal = 0,
            Vertical = 1
        }
        [SerializeField] private ScrollType scrollType = ScrollType.Horizontal;
        
        private bool IsInitialize = false;
        
        [SerializeField] private ScrollRect scrollRect = null;
        
        [SerializeField] private RectTransform cellObj = null;
    
        private int cellCount = 10;
        private float cellWidth;
        private float cellHeight;
    
        private List<Action<InfiniteScrollCell>> OnCellUpdateList = new List<Action<InfiniteScrollCell>>();
    
        private int row;
        private int col;
    
        private List<RectTransform> cellList = new List<RectTransform>();
        private List<InfiniteScrollCell> infiniteScrollCellList = new List<InfiniteScrollCell>();
    
        public void Cell_AddListener(Action<InfiniteScrollCell> _cell)
        {
            OnCellUpdateList.Add(_cell);
            Cell_Refresh();
        }
    
        public void Cell_RemoveListener(Action<InfiniteScrollCell> _cell)
        {
            OnCellUpdateList.Remove(_cell);
        }
    
        public void SetCellCount(int _count)
        {
            cellCount = Mathf.Max(0, _count);
    
            if (!IsInitialize)
                Initialize();

            switch (scrollType)
            {
                case ScrollType.Horizontal:
                    float newContentWidth = cellWidth * Mathf.CeilToInt((float) cellCount / row);
                    float newMaxX = newContentWidth - scrollRect.viewport.rect.width;
                    float minX = scrollRect.content.offsetMin.x;

                    newMaxX += minX;
                    newMaxX = Mathf.Max(minX, newMaxX);

                    scrollRect.content.offsetMax = new Vector2(newMaxX, 0);
                    
                    break;
                case ScrollType.Vertical:
                    float newContentHeight = cellHeight * Mathf.CeilToInt((float) cellCount / col);
                    float newMinY = -newContentHeight + scrollRect.viewport.rect.height;
                    float maxY = scrollRect.content.offsetMax.y;

                    newMinY += maxY;
                    newMinY = Mathf.Min(maxY, newMinY);

                    scrollRect.content.offsetMin = new Vector2(0, newMinY);
                    
                    break;
            }
            
            Cell_Create();
        }
    
        private void Initialize()
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
            cellWidth = cellTempRect.width;
            cellHeight = cellTempRect.height;
            
            switch (scrollType)
            {
                case ScrollType.Horizontal:
                    scrollRect.horizontal = true;
                    scrollRect.vertical = false;

                    col = Mathf.CeilToInt(scrollRect.viewport.rect.width / cellWidth);
                    row = Mathf.Max(1, Mathf.FloorToInt(scrollRect.viewport.rect.height / cellHeight));
                    break;
                case ScrollType.Vertical:
                    scrollRect.horizontal = false;
                    scrollRect.vertical = true;

                    col = Mathf.Max(1, Mathf.FloorToInt(scrollRect.viewport.rect.width / cellWidth));
                    row = Mathf.CeilToInt(scrollRect.viewport.rect.height / cellHeight);
                    break;
            }

            scrollRect.onValueChanged.AddListener(OnValueChange);
        }
    
        private void Cell_Refresh()
        {
            for (int i = 0; i < cellList.Count; i++)
            {
                Cell_Update(cellList[i],infiniteScrollCellList[i]);
            }
        }

        private void Cell_Create()
        {
            int row_Length = 0;
            int col_Length = 0;
            
            switch (scrollType)
            {
                case ScrollType.Horizontal:
                    row_Length = row;
                    col_Length = col + 1;
                    break;
                case ScrollType.Vertical:
                    row_Length = row + 1;
                    col_Length = col;
                    break;
            }
            
            for (int r = 0; r < row_Length; r++)
            {
                for (int c = 0; c < col_Length; c++)
                {
                    int index = 0;

                    switch (scrollType)
                    {
                        case ScrollType.Horizontal:
                            index = r * (col + 1) + c;
                            break;
                        case ScrollType.Vertical:
                            index = r * col + c;
                            break;
                    }

                    if (index < cellCount)
                    {
                        if (cellList.Count <= index)
                        {
                            RectTransform newCell = Instantiate(cellObj, scrollRect.content, true);
                            newCell.anchorMin = new Vector2(0, 1);
                            newCell.anchorMax = new Vector2(0, 1);

                            float x = cellWidth / 2 + c * cellWidth;
                            float y = -r * cellHeight - cellHeight / 2;
                            
                            newCell.localScale = Vector3.one;
                            newCell.anchoredPosition = new Vector2(x, y);
                            
                            InfiniteScrollCell infiniteScrollCell = newCell.gameObject.AddComponent<InfiniteScrollCell>();
                            infiniteScrollCell.SetObjIndex(index);
                            
                            cellList.Add(newCell);
                            infiniteScrollCellList.Add(infiniteScrollCell);
                        }
                    }
                }
            }
        }

        private int row_All => Mathf.CeilToInt((float) cellCount / col);
        private int col_All => Mathf.CeilToInt((float) cellCount / row);

        private void Cell_Update(RectTransform _cell, InfiniteScrollCell _infiniteScrollCell)
        {
            int x = Mathf.CeilToInt((_cell.anchoredPosition.x - cellWidth / 2) / cellWidth);
            int y = Mathf.Abs(Mathf.CeilToInt((_cell.anchoredPosition.y + cellHeight / 2) / cellHeight));

            int index = 0;
            bool IsOutDataRange = false;
            
            switch (scrollType)
            {
                case ScrollType.Horizontal:
                    index = y * col_All + x;
                    IsOutDataRange = index >= cellCount || x >= col_All;
                    
                    Debug.Log(index + "  " + cellCount + "  " + x + "  " + col_All);
                    break;
                case ScrollType.Vertical:
                    index = y * col + x;
                    IsOutDataRange = index >= cellCount || y >= row_All;

                    Debug.Log(index + "  " + cellCount + "  " + y + "  " + row_All);
                    break;
            }

            _infiniteScrollCell.UpdatePos(x, y, index);

            if (IsOutDataRange)
            {
                _cell.gameObject.SetActive(false);
            }
            else
            {
                if (!_cell.gameObject.activeInHierarchy)
                    _cell.gameObject.SetActive(true);

                for (int i = 0; i < OnCellUpdateList.Count; i++)
                {
                    OnCellUpdateList[i]?.Invoke(_infiniteScrollCell);
                }
            }
        }
        
        private void OnValueChange(Vector2 _pos)
        {
            for (int i = 0; i < cellList.Count; i++)
            {
                float dist = 0;
                
                switch (scrollType)
                {
                    case ScrollType.Horizontal:
                        dist = scrollRect.content.offsetMin.x + cellList[i].anchoredPosition.x;
                        float maxRight = col * cellWidth + cellWidth / 2;
                        float minLeft = -cellWidth / 2;
                       
                        if (dist > maxRight)
                        {
                            float newX = cellList[i].anchoredPosition.x - (col + 1) * cellWidth;
                            if (newX > 0)
                            {
                                cellList[i].anchoredPosition = new Vector2(newX, cellList[i].anchoredPosition.y);
                                Cell_Update(cellList[i], infiniteScrollCellList[i]);
                            }
                        }
                        else if (dist < minLeft)
                        {
                            float newX = cellList[i].anchoredPosition.x + (col + 1) * cellWidth;
                            
                            if (newX < scrollRect.content.rect.width)
                            {
                                cellList[i].anchoredPosition = new Vector2(newX, cellList[i].anchoredPosition.y);
                                Cell_Update(cellList[i], infiniteScrollCellList[i]);
                            }
                        }
                        break;
                    case ScrollType.Vertical: 
                        dist = scrollRect.content.offsetMax.y + cellList[i].anchoredPosition.y;
                        float maxTop = cellHeight / 2;
                        float minBottom = -((row + 1) * cellHeight) + cellHeight / 2;

                        if (dist > maxTop)
                        {
                            float newY = cellList[i].anchoredPosition.y - (row + 1) * cellHeight;

                            if (newY > -scrollRect.content.rect.height)
                            {
                                cellList[i].anchoredPosition = new Vector2(cellList[i].anchoredPosition.x, newY);
                                Cell_Update(cellList[i], infiniteScrollCellList[i]);
                            }
                        }
                        else if (dist < minBottom)
                        {
                            float newY = cellList[i].anchoredPosition.y + (row + 1) * cellHeight;

                            if (newY < 0)
                            {
                                cellList[i].anchoredPosition = new Vector2(cellList[i].anchoredPosition.x, newY);
                                Cell_Update(cellList[i], infiniteScrollCellList[i]);
                            }
                        }
                        break;
                }
            }
        }
    }
}