using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BlockSlider
{
    public class Slider : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public Block Block; // todo change to subscribe to events
        private Vector2 Range;

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = new Vector3(
                Mathf.Clamp(eventData.position.x, Range.x, Range.y)
                ,transform.position.y ,transform.position.z );
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            GlobalState.Instance.Board.Print();
            Range = GlobalState.Instance.Board.GetSlideRange(Block);
        //   _Text.text = "You dragging!";
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // if POSITION changed
            GlobalState.Instance.Board.MoveBlockByPosition(Block);

            GlobalState.Instance.Board.RunStates();

            //Block.BlockView.transform.position = GlobalState.Instance.Board[Block.Index.Col,Block.Index.Row].Position;
        }
    }
}