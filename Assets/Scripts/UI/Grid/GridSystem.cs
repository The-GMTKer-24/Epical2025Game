using System;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int cellWidth = 10;
    [SerializeField] private int cellHeight = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private bool renderGrid = false;

    private LineRenderer lineRenderer;
    
    private void Start()
    {
        
        InitializeGridRender(out var lr);
        lineRenderer = lr;
    }

    /// <summary>
    /// Instantiates a grid line renderer and returns it. The renderer will not update if the values change
    /// </summary>
    /// <param name="lr"></param>
    private void InitializeGridRender(out LineRenderer lr)
    {
        lr = gameObject.GetComponent<LineRenderer > ();
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
        for (var i = 0; i < gridHeight - 1; i++)
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
        lineRenderer.enabled = renderGrid;
        Debug.Log(WorldToGridSpace(Input.mousePosition));
    }

    /// <summary>
    /// Returns the grid coordinates of any 2d world space position. Returns -1,-1 if the space is outside of the grid.
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    private Vector2 WorldToGridSpace(Vector2 worldPosition)
    {
        // Bounds check
        if (worldPosition.x < transform.position.x || worldPosition.y < transform.position.y || worldPosition.x > transform.position.x + gridWidth || worldPosition.y > transform.position.y + gridHeight)
        {
            return new Vector2(-1, -1);
        }
        
        // If we get here then it's somewhere inside the grid
        // Recenter the grid to make math easier
        Vector2 centeredWorldPosition =
            new Vector2(worldPosition.x - transform.position.x, worldPosition.y - transform.position.y);

        int row = (int) Math.Floor(centeredWorldPosition.x / cellWidth);
        int column = (int) Math.Floor(centeredWorldPosition.y / cellHeight);

        return new Vector2(row, column);
    }

    private void DrawCell(Vector2 position)
    {
    }
}