using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soap.UI
{
    public class InfiniteScrollVerticle : InfiniteScrollRect
    {
        public override void SetCellCount(int _count)
        {
            base.SetCellCount(_count);
            
            float newContentHeight = cellHeight * Mathf.CeilToInt((float) cellCount / col) + -margin.y;
            float newMinY = -newContentHeight + scrollRect.viewport.rect.height;
            float maxY = scrollRect.content.offsetMax.y;

            newMinY += maxY;
            newMinY = Mathf.Min(maxY, newMinY);

            scrollRect.content.offsetMin = new Vector2(0, newMinY);
            
            Cell_Create();
        }

        protected override void Initialize()
        {
            base.Initialize();
            
            scrollRect.horizontal = false;
            scrollRect.vertical = true;

            col = Mathf.Max(1, Mathf.FloorToInt(scrollRect.viewport.rect.width / cellWidth));
            row = Mathf.CeilToInt(scrollRect.viewport.rect.height / cellHeight);
            
            scrollRect.onValueChanged.AddListener(OnValueChange);
        }

        protected override void Cell_Create()
        {
            int row_Length = 0;
            int col_Length = 0;
            
            row_Length = row + 1;
            col_Length = col;

            for (int r = 0; r < row_Length; r++)
            {
                for (int c = 0; c < col_Length; c++)
                {
                    int index = 0;
                    
                    index = index = r * col + c;
                    
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

            index = y * col + x;
            IsOutDataRange = index >= cellCount || y >= row_All;

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
            }
        }
    }
}

