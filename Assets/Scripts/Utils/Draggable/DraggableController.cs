using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace neuroears.allen.utils
{
    public class DraggableController : MonoBehaviour
    {
        public enum Mode
        {
            Vertical_TtoB, Horizontal_LtoR
        }
        public Transform tempBox;
        public Draggable temp;
        public Mode mode = Mode.Vertical_TtoB;
        //
        private List<DraggableBox> boxes = new();
        private DraggableBox cur_box = null;
        private Draggable cur_target = null;
        private int startIdx = -1;

        private void Awake()
        {
            Debug.Log(mode);
        }
        public void BeginDrag(Draggable target)
        {
            SetDragableObject(target);
            ChangeDraggable(this.cur_target, temp);
            UpdateBoxes();
            boxes.ForEach(box => box.UpdateDraggables());
        }


        public void Drag(Draggable target)
        {
            if(boxes.Count > 0)
            {
                var box = boxes.Find(el => ContainBox(el, target));
                if (box != null)
                {
                    var tempIdx = temp.transform.GetSiblingIndex();
                    if(this.cur_box == box)
                    {
                        var idx = box.GetDragIndex(target, tempIdx);
                        if(idx != tempIdx)
                        {
                    
                            ChangeDraggable(temp, box.GetDraggableByIndex(idx));
                            box.UpdateDraggables();
                        }
                    }
                    else
                    {
                        this.cur_box = box;
                        temp.transform.SetParent(box.transform, false);
                    }
                }
                else
                {
                    //Debug.Log("out");
                }
            }
        }

        public void EndDrag(Draggable target)
        {
            //Debug.Log("EndDrag");
            ChangeDraggable(temp, this.cur_target);
            UpdateBoxes();
            boxes.ForEach(box => box.UpdateDraggables());
        }
        private void SetDragableObject(Draggable target)
        {
            this.cur_target = target;
            this.startIdx = target.transform.GetSiblingIndex();
            var box = target.transform.parent.GetComponent<DraggableBox>();
            if (box == null)
            {
                Debug.LogError("DraggableBox is null");
            }
            else
            {
                this.cur_box = box;
            }
        }
        private void ChangeDraggable(Draggable src, Draggable dst)
        {
            var srcIdx = src.transform.GetSiblingIndex();
            var dstIdx = src.transform.GetSiblingIndex();

            var srcParent = src.transform.parent;
            var dstParent = dst.transform.parent;

            src.transform.SetParent(dstParent);
            src.transform.SetSiblingIndex(dstIdx);

            dst.transform.SetParent(srcParent);
            dst.transform.SetSiblingIndex(srcIdx);
        }

        private bool ContainBox(DraggableBox box, Draggable target)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(box.GetComponent<RectTransform>(), target.transform.position); 
        }
        private void UpdateBoxes()
        {
            var childCountWithoutTemp = transform.childCount - 1;
            for (int i = 0; i < childCountWithoutTemp; i++)
            {
                var box = transform.GetChild(i).GetComponent<DraggableBox>();
                if (box == null) continue;
                box.SetMode(mode);
                if (i == boxes.Count) boxes.Add(null);
                if (boxes[i] != box) boxes[i] = box;
            }
            boxes.RemoveRange(childCountWithoutTemp, boxes.Count - childCountWithoutTemp);
        }
    }

}