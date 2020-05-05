using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlockSlider
{
    public class Board
    {
        public Vector2 CellSize;
        public Cell this[int column, int row]
        {
            get { return Cells[column, row]; }
            set { Cells[column, row] = value; }
        }

        private Cell[,] Cells;
        public int Column {get; private set;}
        public int Row {get; private set;}

        public Board()
        {
            Initialize(8,11);
        }
        public Board(int column, int row)
        {
            Initialize(column, row);
        }

        public void Initialize(int column, int row)
        {
            Column = column;
            Row =row;
            Cells = new Cell[Column, Row ];
        }
        
        public void Print()
        {
            for(var i = 0; i<Row; i++)
            {
                for(var j = 0; j < Column; j++)
                {
                    if(Cells[j,i].Block!=null)
                    {
                         Debug.Log($"Block {j} {i}");
                    }
                }

                
            }
        }

        public float GetMinPos{ get{ return  Cells[0,0].Position.x;}}
        public float GetMaxPos{ get{ return  Cells[Column-1,0].Position.x;}}
        

        public Vector2 GetSlideRange(Block block)
        {
            float max = 0f, min = 0f;
            
            bool isFind = false;

            int sCol = block.Index[0].Col;
            if(block.Type == BlockType.Two)
            {
                sCol = block.Index[1].Col;
            }
            if(block.Type == BlockType.Three)
            {
                sCol = block.Index[2].Col;
            }
            if(block.Type == BlockType.Four)
            {
                sCol = block.Index[3].Col;
            }

            // right
            for(var i = sCol+1; i<Column; i++)
            {
                if(Cells[i,(int)block.Index[0].Row].Block!=null)
                {
                    max = Cells[i-1,(int)block.Index[0].Row].Position.x;
                    isFind = true;

                  //  Debug.Log("find R "+i+" "+max + " | "+sCol);
                    break;
                }
            }

            if(!isFind)
                max = GetMaxPos;

            // if type change max
            max -= 50*block.TypeIndex;


            isFind = false;
            // left
            for(var i = block.Index[0].Col-1; i>=0; i--)
            {
                if(Cells[i,(int)block.Index[0].Row].Block!=null)
                {
                    min = Cells[i+1,(int)block.Index[0].Row].Position.x;
                    isFind = true;

                   // Debug.Log("find L "+i+" "+min + " | "+block.Index[0].Col);
                    break;
                }
            }
            if(!isFind)
                min = GetMinPos;

            min += 50 * block.TypeIndex;

          //  Debug.Log(min +" "+ max );
            return new Vector2(min, max);
        }

        public Cell GetNearEmptyCell(Cell cell)
        {
            Cell near = null;

            for (int i = cell.Index.Row-1; i >= 0; i--)
            {
                 if(Cells[cell.Index.Col,i].Block == null) 
                 {
                    near = Cells[cell.Index.Col,i];
                 }
                 else
                 {
                     break;                     
                 }
            }

            return near;
        }

        public void MoveBlockByPosition(Block block)
        {
            int index = (int)(( block.BlockView.position.x-(GetMinPos-CellSize.x*0.5f)-50*block.TypeIndex)/CellSize.x);
           // Debug.Log($"{  ( block.BlockView.position.x-(GetMinPos-CellSize.x/2f))} {( block.BlockView.position.x-(GetMinPos-CellSize.x/2f))/CellSize.x}  {index} ");

            ChangeBlockPosition(block, Cells[index,block.Index[0].Row]);
        }

        private void ChangeBlockPosition(Block block, Cell cell)
        {
            DeleteBlock(block);
            AddBlock(cell,block);
        }

// blocks
        public void AddBlock(Cell cell, Block block)
        {
            block.Index[0] = cell.Index;
            cell.Block = block;
            if(block.Type == BlockType.Two)
            {
                 block.Index[1] = (cell.Index.Col+1,cell.Index.Row);
                 Cells[cell.Index.Col+1,cell.Index.Row].Block = block;
            }
            else
            if(block.Type == BlockType.Three)
            {
                 block.Index[1] = (cell.Index.Col+1,cell.Index.Row);
                 Cells[cell.Index.Col+1,cell.Index.Row].Block = block;
                 block.Index[2] = (cell.Index.Col+2,cell.Index.Row);
                 Cells[cell.Index.Col+2,cell.Index.Row].Block = block;
            }
             else
            if(block.Type == BlockType.Four)
            {
                 block.Index[1] = (cell.Index.Col+1,cell.Index.Row);
                 Cells[cell.Index.Col+1,cell.Index.Row].Block = block;
                 block.Index[2] = (cell.Index.Col+2,cell.Index.Row);
                 Cells[cell.Index.Col+2,cell.Index.Row].Block = block;
                 block.Index[3] = (cell.Index.Col+3,cell.Index.Row);
                 Cells[cell.Index.Col+3,cell.Index.Row].Block = block;

            }
           
            // to event!!!!!
            if( block.BlockView)
               // block.BlockView.transform.position = cell.Position;
                 block.BlockView.transform.position = new Vector3(cell.Position.x + 50*block.TypeIndex, cell.Position.y, cell.Position.z);

        }

        public void DeleteBlock(/* Cell cell,*/ Block block)
        {
            //block.Index = cell.Index;
            Cells[block.Index[0].Col,block.Index[0].Row].Block = null;
            if(block.Type == BlockType.Two)
            {
                Cells[block.Index[1].Col,block.Index[1].Row].Block = null;
            }
            else
            if(block.Type == BlockType.Three)
            {
                Cells[block.Index[1].Col,block.Index[1].Row].Block = null;
                Cells[block.Index[2].Col,block.Index[2].Row].Block = null;
            }
            else
            if(block.Type == BlockType.Four)
            {
                Cells[block.Index[1].Col,block.Index[1].Row].Block = null;
                Cells[block.Index[2].Col,block.Index[2].Row].Block = null;
                Cells[block.Index[3].Col,block.Index[3].Row].Block = null;
            }


        }
        // public void DeleteBlock(Cell cell)
        // {
        //     //block.Index = cell.Index;
        //     cell.Block = null;
        // }

        public Action<Block> OnBlockDestroy;
        public void DestroyBlock(Block block)
        {
            if(OnBlockDestroy != null)
                OnBlockDestroy(block);

            DeleteBlock(block);
        }
//

// Moves
    public void MoveBlocksDown()
    {

         List<Block> blocks = new List<Block>();
        for(var i = 0; i<Row; i++)
        {
                for(var j = 0; j < Column; j++)
                {
                    if(Cells[j,i].Block!=null && !blocks.Contains(Cells[j,i].Block)) // add check to - CAN MOVED
                    {
                      //  Debug.Log($"Block {j} {i}");
                        blocks.Add(Cells[j,i].Block);
                        // var near = GetNearEmptyCell(Cells[j,i]);
                        // if(near != null)
                        // {
                        //     ChangeBlockPosition(Cells[j,i].Block, near);
                        // }
                    }
                }

                
        }

        foreach(var block in blocks)
        {
            if(block.Type == BlockType.Four)
            {
                var near1 = GetNearEmptyCell(Cells[block.Index[0].Col,block.Index[0].Row]);
                var near2 = GetNearEmptyCell(Cells[block.Index[1].Col,block.Index[1].Row]);
                var near3 = GetNearEmptyCell(Cells[block.Index[2].Col,block.Index[2].Row]);
                var near4 = GetNearEmptyCell(Cells[block.Index[3].Col,block.Index[3].Row]);


                Debug.Log($"1 {near1} 2 {near2} 3 {near3} 4 {near4}");

                if(near1 != null && near2 != null && near3 != null && near4 != null)
                {
                    int[] arrayOfIndex = {near1.Index.Row, near2.Index.Row, near3.Index.Row, near4.Index.Row};
                    var index = Mathf.Max(arrayOfIndex);

                    if(block.Index[0].Row !=index)
                    {                    
                        var near = Cells[block.Index[0].Col,index];
                        ChangeBlockPosition(Cells[block.Index[0].Col,block.Index[0].Row].Block, near);
                    }
                }


                continue;
            }
            else
            if(block.Type == BlockType.Three)
            {
                var near1 = GetNearEmptyCell(Cells[block.Index[0].Col,block.Index[0].Row]);
                var near2 = GetNearEmptyCell(Cells[block.Index[1].Col,block.Index[1].Row]);
                var near3 = GetNearEmptyCell(Cells[block.Index[2].Col,block.Index[2].Row]);

                if(near1 != null && near2 != null && near3 != null)
                {
                    int[] arrayOfIndex = {near1.Index.Row, near2.Index.Row, near3.Index.Row};
                    var index = Mathf.Max( arrayOfIndex);

                    if(block.Index[0].Row !=index)
                    {                    
                        var near = Cells[block.Index[0].Col,index];
                        ChangeBlockPosition(Cells[block.Index[0].Col,block.Index[0].Row].Block, near);
                    }
                }

                continue;
            }
            if(block.Type == BlockType.Two)
            {
                var near1 = GetNearEmptyCell(Cells[block.Index[0].Col,block.Index[0].Row]);
                var near2 = GetNearEmptyCell(Cells[block.Index[1].Col,block.Index[1].Row]);

                if(near1 != null && near2 != null)
                {
                    var near = near1;
                    if(near1.Index.Row < near2.Index.Row)
                        near = Cells[block.Index[0].Col, near2.Index.Row ];

                    ChangeBlockPosition(Cells[block.Index[0].Col,block.Index[0].Row].Block, near);
                }

                continue;
            }
            else
            {
                var near = GetNearEmptyCell(Cells[block.Index[0].Col,block.Index[0].Row]);
                if(near != null)
                {
                    ChangeBlockPosition(Cells[block.Index[0].Col,block.Index[0].Row].Block, near);
                }
            }

            
        }

    }
    public void MoveBlocksUp()
    {
        List<Block> blocks = new List<Block>();

        for(var i = Row-1; i >= 0; i--)
        {
                for(var j = 0; j < Column; j++)
                {
                    if(Cells[j,i].Block!=null && !blocks.Contains(Cells[j,i].Block)) // add check to - CAN MOVED
                    {
                    //    Debug.Log($"Block {j} {i}");
                        blocks.Add(Cells[j,i].Block);
                       // ChangeBlockPosition(Cells[j,i].Block, Cells[j,i+1]); // make command
                    }
                }

                
        }
        // revert
       // blocks.Reverse();
        
        foreach(var block in blocks)
        {
             if(block.Index[0].Row + 1 >= Row)
             {
                 Debug.Log(" LOOSE ");
                 return;
             }
             ChangeBlockPosition(block, Cells[block.Index[0].Col,block.Index[0].Row+1]);
             // Check to 
        }
    }
//
//
    // public void AddLine()
    // {
    //     MoveBlocksUp();
    // }

    public bool CheckLines()
    {
        for(var i = 0; i < Row-1; i++)
        {
            if(CheckRowLine(i))
            {
                List<Block> blocks = new List<Block>();
                for(var j = 0; j < Column; j++)
                {
                    if(Cells[j,i].Block!=null && !blocks.Contains(Cells[j,i].Block)) 
                        blocks.Add(Cells[j,i].Block);
                   // DeleteBlock(Cells[j,i]);
                }

                foreach(var block in blocks)
                {
                     DestroyBlock(block);
                }

                return true;
            }
        }

        return false;
    }

    public void DeleteRow(int rowIndex)
    {
        List<Block> blocks = new List<Block>();
        for(var j = 0; j < Column; j++)
        {
            if(Cells[j,rowIndex].Block!=null && !blocks.Contains(Cells[j,rowIndex].Block)) 
                blocks.Add(Cells[j,rowIndex].Block);
        // DeleteBlock(Cells[j,i]);
        }

        foreach(var block in blocks)
        {
            DestroyBlock(block);
        }   
    }

    public bool CheckRowLine(int row)
    {
        for(var j = 0; j < Column; j++)
        {
            if(Cells[j,row].Block == null)
            {
                return false;
            }
        }

        return true;
    }

    public void RunStates()
    {
        UIBoardView.runtime.RunStateAnim();
        // MoveBlocksDown();

        // CheckLines();

        // MoveBlocksUp();
        // UIBoardView.runtime.AddLine();
        // MoveBlocksDown();// if down ++ up

        // CheckLines();
    }



//ё

//
// Made Commands for change
//
//
//


    }


}