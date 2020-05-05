using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlockSlider
{
    public class UIBoardView : MonoBehaviour
    {
        public static UIBoardView runtime;
        [SerializeField] private RectTransform UICell;
        [SerializeField] private Vector2 CellSize;
        [SerializeField] private Vector2 ColumnRow;

        private int MaxLine = 8;// TODO

        [SerializeField] private Transform[] BlockItems;

        private Board Board;

        public void Start()
        {
            runtime = this;

            // /InitializeViewTest(ColumnRow,CellSize);
            Board = new Board();
            Board.CellSize = CellSize;
            InitializeBoard(Board,CellSize);// to scriptoble object
            Board.OnBlockDestroy += OnBoardDestory;

            // CreateBlock(Board[5,4]);
            // CreateBlock(Board[5,2]);
            // CreateBlock(Board[5,1]);

            GlobalState.Instance.Board = Board;
            //StartCoroutine(TestView(boardTest));
            AddLine();
        }

        private void OnBoardDestory(Block block)
        {
            //block.BlockView.gameObject.SetActive(false);
            Destroy(block.BlockView.gameObject,0.2f);
        }

        public void InitializeViewTest(Vector2 columnRow,Vector2 cellSize)
        {
            var width = columnRow.x*cellSize.x;
            var height = columnRow.y*cellSize.y;

            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

            var startX = transform.position.x - width/2f + cellSize.x/2;
            var startY = transform.position.y - height/2f + cellSize.y/2;

            var posY = startY;
            for(var i = 0 ; i< columnRow.y; i++)
            {
                var posX = startX;

                for(var j = 0 ; j<columnRow.x; j++)
                {
                    var cell = Instantiate(UICell);
                    cell.transform.parent = this.transform;
                    cell.position = new Vector3(posX,posY,0);
                    posX+=cellSize.x;
                }

                posY += cellSize.y;
            }

        }

        // to board CONSTRUCTOR!!!!!!!!!!!!!!!!!
        public void InitializeBoard(Board board,Vector2 cellSize)
        {
            var width = board.Column * cellSize.x;
            var height = board.Row * cellSize.y;

            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

            var startX = transform.position.x - width/2f + cellSize.x/2;
            var startY = transform.position.y - height/2f + cellSize.y/2;

            var posY = startY;
            for(var i = 0 ; i< board.Row; i++)
            {
                var posX = startX;

                for(var j = 0 ; j< board.Column; j++)
                {
                    board[j,i] = new Cell();
                    board[j,i].Index = (Col:j, Row:i);
                    board[j,i].Position = new Vector3(posX,posY,0);
                    //test
                    board[j,i].BlockTest = Instantiate(UICell);
                    board[j,i].BlockTest.transform.parent = this.transform;
                    board[j,i].BlockTest.position = new Vector3(posX,posY,0);
                    //
                    posX+=cellSize.x;
                }
                posY += cellSize.y;
            }
        }
        public void CreateBlock(Cell cell, BlockType blockType = BlockType.One)
        {
             // test
            var block = new Block(blockType);
            // view CREATE

            var index = ((int)blockType)-1;
            block.BlockView = Instantiate(BlockItems[index]);
            block.BlockView.transform.parent = this.transform;
            block.BlockView.transform.position = new Vector3(cell.Position.x + 50*index, cell.Position.y, cell.Position.z);


            block.BlockView.GetComponent<Slider>().Block = block;

            Board.AddBlock(cell,block);

        }

//
        public void AddLine()
        {
            SetRandomLine();
return;
// test random
            Board.DeleteRow( indexInst);
            SetRandomLine();

            indexInst++;
            if(indexInst == 11)
            {
                indexInst = 0;

            }
            return;
//
          //   CreateBlock(Board[1,1]);
            // //CreateBlock(Board[2,0]);
            // //CreateBlock(Board[4,0]);
          //   CreateBlock(Board[1,3], BlockType.Two);
          //   CreateBlock(Board[1,5], BlockType.Four);
            // CreateBlock(Board[7,0]);
        }

        private void SetRandomLineTemplate()
        {

        }

        private void SetRandomLine()
        {

            //1 Set Random
            //2 Get Arrays
            //3 Fill arraays
            //4 add blocks
         
         //1
          //float[] mas = new float[] {0.35f,0.30f,0.25f,0.10f};
            float[] emptyProb = new float[] {0.40f,0.30f,0.20f,0.10f};

            int emptyCounts = ProbabilityController.GetRandom(emptyProb)+1;
            Debug.Log($"emptyCounts {emptyCounts}");

            var emptyIndexes =  getUniqueRandomArray(0, MaxLine, emptyCounts).OrderBy(i=>i);// board width 88888
    
            foreach(var index in emptyIndexes)
            {
                Debug.Log($"empty {index}");
            }

        //   int freeCells = 8;
        //   Queue<BlockType> line = new Queue<BlockType>();

        //     int freeCells = 8;
        //     int rangeTypes = 3;
        //     while(freeCells > 0)
        //     {
        //     }

            Queue<BlockType> line = new Queue<BlockType>();

            int current = 0;

            foreach(var index in emptyIndexes)
            {
                var length = index - current;

                if(length>0)
                {
                    var group = FillBlocks(length);
                    foreach (var item in group)
                    {
                        line.Enqueue(item);
                    }
                }
                current = index + 1;
                line.Enqueue(BlockType.Empty);
            }

            // for last 
            if(current != MaxLine)
            {
                var length = MaxLine - current;

                if(length>0)
                {
                    var group = FillBlocks(length);
                    foreach (var item in group)
                    {
                        line.Enqueue(item);
                    }
                }
               
                line.Enqueue(BlockType.Empty);
            }

            // print
            // foreach (var p in line)
            // {
            //     Debug.Log($"line {p}");
            // }

            current = 0;
            //while(line.Count != 0)
            for (var i=0; i<line.Count; i++)
            {
                var  blockType = line.Dequeue();
                Debug.Log($"{current} {line.Dequeue()}");
                if( blockType == BlockType.Empty)
                {
                    current+=1;
                    continue;
                }

                CreateBlock(Board[current, indexInst], blockType);

                 current +=(int) blockType;

            }
          
        }

        private List<BlockType> FillBlocks(int length)
        {
            int freeCells = length;
            List<BlockType> result = new List<BlockType>();
            int rangeTypes = 4;// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            while(freeCells > 0)
            {
                var max = freeCells;
                if(freeCells > rangeTypes)
                {
                    max = rangeTypes;
                }

                float[] mas = new float[] {0.35f,0.30f,0.25f,0.10f};
                var blockIndex = 1;
                if(max >= 4)
                {
                    blockIndex = (ProbabilityController.GetRandom(mas)-4) * -1;
                }
                else
                if(max >= 3)
                {
                    mas = new float[] {0.55f,0.35f,0.10f};
                    blockIndex = (ProbabilityController.GetRandom(mas)-3) * -1;
            
                }
                else
                if(max >= 2)
                {
                    mas = new float[] {0.55f,0.45f};
                    blockIndex = (ProbabilityController.GetRandom(mas)-2) * -1;
                }

    
                Debug.Log($"! {blockIndex}  {max+1}");
                var blockType = (BlockType) (blockIndex); 

                result.Add(blockType);
            

                freeCells -=blockIndex; 

            }

            return result;
        }

        public static int[] getUniqueRandomArray(int min, int max, int count) {
            int[] result = new int[count];
            var numbersInOrder = new List<int>();
            for (var x = min; x < max; x++) {
                numbersInOrder.Add(x);
            }
            for (var x = 0; x < count; x++) {
                var randomIndex = UnityEngine.Random.Range(0, numbersInOrder.Count);
                result[x] = numbersInOrder[randomIndex];
                numbersInOrder.RemoveAt(randomIndex);
            }

            return result;
        }

        //  public static List<int> getUniqueRandomList(int min, int max, int count)
        //     {
        //         var result = new List<int> (getUniqueRandomArray(min, max, count));
        //         result.OrderBy();
        //         return result;
        //     }
        


        private int indexInst = 0;
        // private void SetRandomLine()
        // {
        //     int freeCells = 8;
        //     int rangeTypes = 3;
        //     int empty = 1;
        //     while(freeCells > 0)
        //     {
        //         var max = freeCells;
        //         if(freeCells > rangeTypes)
        //         {
        //             max = rangeTypes;
        //         }

        //         float[] mas = new float[] {0.35f,0.30f,0.25f,0.10f};
        //         var blockIndex = 1;
        //         if(max >= 4)
        //         {
        //             blockIndex = (ProbabilityController.GetRandom(mas)-4) * -1;
        //         }
        //         else
        //         if(max >= 3)
        //         {
        //             mas = new float[] {0.55f,0.35f,0.10f};
        //             blockIndex = (ProbabilityController.GetRandom(mas)-3) * -1;
              
        //         }
        //         else
        //         if(max >= 2)
        //         {
        //             mas = new float[] {0.55f,0.45f};
        //             blockIndex = (ProbabilityController.GetRandom(mas)-2) * -1;
        //         }
               
        //        // var blockIndex = UnityEngine.Random.Range(1,max+1);

               
        //         mas = new float[] {0.55f,0.45f};
        //         if(ProbabilityController.GetRandom(mas) == 0 || empty>0)
        //         {// free
        //             //freeCells -=blockIndex;
        //             // empty
        //             empty--;
        //         }
        //         else
        //         {
        //             Debug.Log($"! {blockIndex} 1 {max+1}");
        //             var blockType = (BlockType) (blockIndex-1); 

        //             CreateBlock(Board[8-freeCells, indexInst], blockType);
        //         }

        //         freeCells -=blockIndex; 

        //     }


        // }

        public void TestMove()
        {
            Board.MoveBlocksDown();
        }
        public void TestMoveUp()
        {
            Board.MoveBlocksUp();

        }
//
        IEnumerator TestView(Board board) 
        {
            for(var i = 0 ; i< board.Row; i++)
            {
                for(var j = 0 ; j< board.Column; j++)
                { 
                    board[j,i].BlockTest.localScale = new Vector3(0.9f,0.9f,0.9f);
                    yield return new WaitForSeconds(.1f);
                }
            }
        }

        public void RunStateAnim()
        {
            lineAdded =1;
            StartCoroutine(RunState());
        }

        private int lineAdded;
        IEnumerator RunState() 
        {
            yield return new WaitForSeconds(.1f);
            Board.MoveBlocksDown();
            yield return new WaitForSeconds(.1f);

            while(Board.CheckLines())
            {
                yield return new WaitForSeconds(.5f);
                Board.MoveBlocksDown();
            }

            if(lineAdded > 0)
            {
                yield return new WaitForSeconds(.1f);
                Board.MoveBlocksUp();
                yield return new WaitForSeconds(.6f);
                
                AddLine();
                lineAdded--;
                StartCoroutine(RunState());
            }
            

            // yield return new WaitForSeconds(.1f);
            // Board.MoveBlocksDown();// if down ++ up

            // yield return new WaitForSeconds(.1f);
            // Board.CheckLines();

            // Board.MoveBlocksDown();
        }

    }


    // public class RectView
    // {
        
        
    // }


}

public class ProbabilityController
{
    // public static int ChoiceRandom(BonusItem[] items)
    // {
    //     Array.Sort(items, (x, y) => -x.CompareTo(y));
    //     var array = new float[items.Length];

    //     for (int index = 0; index < items.Length; index++)
    //         array[index] = items[index].Percent;

    //     var choice = GetRandom(array);

    //     return choice;
    // }

    public static int GetRandom(float[] probability)
    {
        float total = 0;

        for (int index = 0; index < probability.Length; index++)
            total += probability[index]; 

        if (total > 1)
            throw new Exception("Overall probability is greater than 1");

        var randomPoint = UnityEngine.Random.value * total;

        for (int i = 0; i < probability.Length; i++)
        {
            if (randomPoint <= probability[i])
                return i;

            randomPoint -= probability[i];

        }

        return probability.Length - 1;
    }
}