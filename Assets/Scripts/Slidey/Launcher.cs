using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockSlider
{
    public class Launcher : MonoBehaviour
    {

    }

    [System.Serializable]
    public class Cell
    {
        public (int Col, int Row) Index;
        public Vector3 Position;
        public CellType Type;
        public Block Block;
        public Transform BlockTest;

        public override string ToString()
        {
            return $"|{Index.Col} {Index.Row}|";
        }

    }

    public enum CellType
    {
        Empty,
        Full,
        Lock
    }

    [System.Serializable]
    public class Block
    {
        public int TypeIndex{ get{ return ((int)Type)-1;}}
        public BlockType Type;
        //public Cell[] Cell;
       // public Vector2 Index; // todo change to indexes
        public (int Col, int Row)[] Index; // to multi

        public Transform BlockView;

        public Block(BlockType blockType)
        {
            Type = blockType;
            Index = new (int Col, int Row)[((int)blockType)];
        }

    }

// (string Message, int SomeNumber) t = ("Hello", 4);
// //or using implicit typing 
// var t = (Message:"Hello", SomeNumber:4);

// Console.WriteLine("{0} {1}", t.Message, t.SomeNumber);
//Tuple<string, int> t = new Tuple<string, int>("Hello", 4);

    public enum BlockType
    {
        Empty = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4
    }


}