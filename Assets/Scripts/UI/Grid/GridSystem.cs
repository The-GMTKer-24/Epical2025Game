using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int cellWidth = 10;
    [SerializeField] private int cellHeight = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private bool renderGrid = false;
    [SerializeField] private Camera camera;

    private LineRenderer lineRenderer;
    private int gridSystemWidth;
    private int gridSystemHeight;
    
    private void Start()
    {
        gridSystemHeight = cellHeight * gridHeight;
        gridSystemWidth = cellWidth * gridWidth;
        
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
        
        var worldPos= camera.ScreenToWorldPoint(new Vector2(Mouse.current.position.x.value, Mouse.current.position.y.value));
        
        Debug.Log(WorldToGridSpace(worldPos));
    }

    /// <summary>
    /// Returns the grid coordinates of any 2d world space position. Returns -1,-1 if the space is outside of the grid.
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    private Vector2 WorldToGridSpace(Vector2 worldPosition)
    {
        // Bounds check
        if (worldPosition.x < transform.position.x || worldPosition.y < transform.position.y || worldPosition.x > transform.position.x + gridSystemWidth || worldPosition.y > transform.position.y + gridSystemHeight)
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