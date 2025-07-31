using UnityEngine;

public class GridSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int cellWidth = 10;
    [SerializeField] private int cellHeight = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private int gridWidth = 10;


    private void Start()
    {
        var lr = gameObject.AddComponent<LineRenderer>();
        lr.endWidth = 0.2f;
        lr.startWidth = 0.2f;
        lr.positionCount = 5 + 3 * (gridHeight - 1) + 3 * (gridWidth - 1) + 1;


        var gridSystemHeight = cellHeight * gridHeight;
        var gridSystemWidth = cellWidth * gridWidth;


        lr.SetPosition(0, new Vector3(0, 0, 0));
        lr.SetPosition(1, new Vector3(0, gridSystemHeight, 0));
        lr.SetPosition(2, new Vector3(gridSystemWidth, gridSystemHeight, 0));
        lr.SetPosition(3, new Vector3(gridSystemWidth, 0, 0));
        lr.SetPosition(4, new Vector3(0, 0, 0));

        // Render the cross lines
        var index = 5;
        var yposition = 0;
        var xposition = 0;
        for (var i = 0; i < gridWidth - 1; i++)
        {
            yposition += cellHeight;

            lr.SetPosition(index, new Vector3(xposition, yposition, 0));
            index++;

            xposition += gridSystemWidth;
            lr.SetPosition(index, new Vector3(xposition, yposition, 0));
            index++;


            xposition -= gridSystemWidth;
            lr.SetPosition(index, new Vector3(xposition, yposition, 0));
            index++;
        }

        yposition += cellHeight;

        lr.SetPosition(index, new Vector3(xposition, yposition, 0));
        index++;

        for (var i = 0; i < gridWidth - 1; i++)
        {
            xposition += cellWidth;

            lr.SetPosition(index, new Vector3(xposition, yposition, 0));
            index++;

            yposition -= gridSystemHeight;
            lr.SetPosition(index, new Vector3(xposition, yposition, 0));
            index++;


            yposition += gridSystemHeight;
            lr.SetPosition(index, new Vector3(xposition, yposition, 0));
            index++;
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void DrawCell(Vector2 position)
    {
    }
}