using UnityEngine;
using System.Collections;

public class Tree  {

    public int wood;
    public Vector2 Pos;

    public Tree(int h, int w)
    {
        Pos = new Vector2(h, w);
        wood = 10;
    }
    public bool CUT ()
    {
        if(wood > 0 )
        {
            wood--;
            return true;
        }
        return false;
    }
    public void Replace()
    {
        if (wood >= 10)
            wood = 9;
        wood++;
    }
    public bool Full()
    {
        return wood == 10;
    }
}
