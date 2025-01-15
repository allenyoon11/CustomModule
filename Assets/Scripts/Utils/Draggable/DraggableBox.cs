using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace neuroears.allen.utils
{
    public class DraggableBox : MonoBehaviour
    {
        private List<Draggable> draggables = new();
        private DraggableController.Mode mode;
        public void UpdateDraggables()
        {
            var childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var draggable = transform.GetChild(i).GetComponent<Draggable>();
                if (draggable == null) continue;

                if (i == draggables.Count) draggables.Add(null);
                if (draggables[i] != draggable)
                {
                    draggables[i] = draggable;
                }

            }
            draggables.RemoveRange(childCount, draggables.Count - childCount);
        }
        public int GetDragIndex(Draggable target, int skipIndex = -1)
        {
            int idx = 0;
            for (int i = 0; i < draggables.Count; i++)
            {
                switch (mode)
                {
                    case DraggableController.Mode.Horizontal_LtoR:
                        if (draggables[i].transform.position.x > target.transform.position.x)
                        {
                            break;
                        }
                        else if (i != skipIndex)
                        {

                            idx++;
                        }
                        break;
                    case DraggableController.Mode.Vertical_TtoB:
                        if (draggables[i].transform.position.y < target.transform.position.y)
                        {
                            break;
                        }
                        else if (i != skipIndex)
                        {
                            idx++;
                        }
                        break;
                }
            }
            return idx;
        }
        public Draggable GetDraggableByIndex(int idx) => draggables[idx];

        public void SetMode(DraggableController.Mode mode) => this.mode = mode;
    }

}
