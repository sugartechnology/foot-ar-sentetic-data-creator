using System;
using System.Collections;
using System.Collections.Generic;


[Serializable]
public class Point{

    public Point(float x, float y){
        this.x = x;
        this.y = y;
    }
    public float x;
    public float y;
}


[Serializable]
public class ShoeData{

    public  ShoeData(){
        points = new List<Point>();
    }
    public List<Point> points; 
}

[Serializable]
public class SceneData{

    public  SceneData(){
        shoes = new List<ShoeData>();
    }
    public List<ShoeData> shoes; 
}