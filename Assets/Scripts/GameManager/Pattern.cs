using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pattern
{
    public int[,] _obstacles;
    public int _length, _width;
    
    public enum ObstaclesType
    {
        empty,
        elecCables,
        babredWires,
        hand,
        bones,
        total
    }

    public Pattern()
    {
        //same size for every pattern

        _obstacles = new int[5, 5];
    }

    public Pattern(int length, int width)
    {
        _length = length;
        _width = width;

        _obstacles = new int[length, width];
    }

    /*
     * Position in pattern go like so :
     * 
     *  y  _________________
     *  3 |              X
     *  2 |      X     X
     *  1 |    X   
     *  0 |_________________
     *     0 1 2 3 4 5 6 7  x
     *     
     *  
     *  so obstacle in example would be in position :
     *  (2, 1), (3, 2), (6, 2), (7, 3)
     *  
     *  let's say "x" are spikes. then, to add them in the obstacles, do:
     *  AddObstacle(2, 1, ObstaclesType.spike);
     *  [...]
     */

    public void AddObstacle(int x, int y, ObstaclesType obst)
    {
        _obstacles[x, y] = (int)obst;
    }


}
