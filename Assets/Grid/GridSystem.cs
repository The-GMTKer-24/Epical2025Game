using UnityEngine;

public class GridSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] int cellWidth = 10;
    [SerializeField] int cellHeight = 10;
    [SerializeField] int gridHeight = 10;
    [SerializeField] int gridWidth = 10;
    


    void Start()
    {  
        LineRenderer lr = gameObject.AddComponent<LineRenderer>();
        lr.endWidth = 0.2f;
        lr.startWidth = 0.2f;
        lr.positionCount = 5 + 3*(gridHeight-1) + 3*(gridWidth-1) + 1; 
        
        
        int gridSystemHeight = cellHeight * gridHeight;
        int gridSystemWidth = cellWidth * gridWidth;
        
        
        lr.SetPosition(0, new Vector3(0,0, 0));
        lr.SetPosition(1, new Vector3(0, gridSystemHeight, 0));
        lr.SetPosition(2, new Vector3(gridSystemWidth, gridSystemHeight, 0));
        lr.SetPosition(3, new Vector3(gridSystemWidth, 0, 0));
        lr.SetPosition(4, new Vector3(0, 0, 0));
        
        // Render the cross lines
        int index = 5;
        int yposition = 0;
        int xposition = 0;
        for (int i = 0; i < gridWidth - 1; i++)
        {
            yposition += cellHeight;
            
            lr.SetPosition(index, new Vector3(xposition, yposition,0));
            index++;
            
            xposition += gridSystemWidth;
            lr.SetPosition(index, new Vector3(xposition, yposition,0));
            index++;
            
            
            xposition -= gridSystemWidth;
            lr.SetPosition(index, new Vector3(xposition, yposition,0));
            index++;

        }
        yposition += cellHeight;
            
        lr.SetPosition(index, new Vector3(xposition, yposition,0));
        index++;
        
        for (int i = 0; i < gridWidth - 1; i++)
        {
            xposition += cellWidth;
            
            lr.SetPosition(index, new Vector3(xposition, yposition,0));
            index++;
            
            yposition -= gridSystemHeight;
            lr.SetPosition(index, new Vector3(xposition, yposition,0));
            index++;
            
            
            yposition += gridSystemHeight;
            lr.SetPosition(index, new Vector3(xposition, yposition,0));
            index++;

        }
    }

    void DrawCell(Vector2 position)
    {
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
