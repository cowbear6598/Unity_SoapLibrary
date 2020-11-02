using System.Collections;
using System.Collections.Generic;
using Soap.UI;
using UnityEngine;

namespace Soap.UI
{
    public class InfiniteScrollHorizontal : InfiniteScrollRect
    {
        public override void SetCellCount(int _count)
        {
            base.SetCellCount(_count);

            float newContentWidth = cellWidth * Mathf.CeilToInt((float) cellCount / row) + margin.x;
            float newMaxX = newContentWidth - scrollRect.viewport.rect.width;
            float minX = scrollRect.content.offsetMin.x;

            newMaxX += minX;
            newMaxX = Mathf.Max(minX, newMaxX);

            scrollRect.content.offsetMax = new Vector2(newMaxX, 0);
            
            Cell_Create();
        }

        protected override void Initialize()
        {
            base.Initialize();
            
            scrollRect.horizontal = true;
            scrollRect.vertical = false;
            
            col = Mathf.CeilToInt(scrollRect.viewport.rect.width / cellWidth);
            row = Mathf.Max(1, Mathf.FloorToInt(scrollRect.viewport.rect.height / cellHeight));
            
            scrollRect.onValueChanged.AddListener(OnValueChange);
        }

        protected override void Cell_Create()
        {
            int row_Length = 0;
            int col_Length = 0;
            
            row_Length = row;
            col_Length = col + 1;
            
            for (int r = 0; r < row_Length; r++)
            {
                for (int c = 0; c < col_Length; c++)
                {
                    int index = 0;
                    
                    index = index = r * (col + 1) + c;
                    
                    if (index < cellCount)
                    {
                        if (cellList.Count <= index)
                        {
                            RectTransform newCell = Instantiate(cellObj, scrollRect.content, true);
                            newCell.anchorMin = new Vector2(0, 1);
                            newCell.anchorMax = new Vector2(0, 1);

                            float x = cellWidth / 2 + c * cellWidth + margin.x;
                            float y = -r * cellHeight - cellHeight / 2 + margin.y;
                            
                            newCell.localScale = Vector3.one;
                            newCell.anchoredPosition = new Vector2(x, y);
                            
                            InfiniteScrollCell infiniteScrollCell = newCell.gameObject.GetComponent<InfiniteScrollCell>();

                            if (infiniteScrollCell == null)
                                infiniteScrollCell = newCell.gameObject.AddComponent<InfiniteScrollCell>();
                            
                            infiniteScrollCell.SetObjIndex(index);
                            
                            cellList.Add(newCell);
                            infiniteScrollCellList.Add(infiniteScrollCell);
                        }
                    }
                }
            }
        }

        protected override void Cell_Update(RectTransform _cell, InfiniteScrollCell _infiniteScrollCell)
        {
            int x = Mathf.CeilToInt((_cell.anchoredPosition.x - margin.x - cellWidth / 2) / cellWidth);
            int y = Mathf.Abs(Mathf.CeilToInt((_cell.anchoredPosition.y - margin.y + cellHeight / 2) / cellHeight));

            int index = 0;
            bool IsOutDataRange = false;

            index = y * col_All + x;
            IsOutDataRange = index >= cellCount || x >= col_All;

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
        
        protected override void OnValueChange(Vector2 _pos)
        {
            for (int i = 0; i < cellList.Count; i++)
            {
                float dist = 0;
                
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
            }
        }
    }
}