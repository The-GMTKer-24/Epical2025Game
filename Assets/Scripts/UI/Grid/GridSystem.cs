using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Factory_Elements;
using Scriptable_Objects;
using Unity.Mathematics;

public class GridSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int cellWidth = 10;
    [SerializeField] private int cellHeight = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private bool renderGrid;
    [SerializeField] private Camera camera;
    [SerializeField] private FactoryElementType belt;
    [SerializeField] private FactoryElementType pulverizer;
    [SerializeField] private FactoryElementType itemSource;
    [SerializeField] private FactoryElementType trophey;
    private PlayerControls playerControls;

    private LineRenderer lineRenderer;
    private int gridSystemWidth;
    private int gridSystemHeight;
    private FactoryElementType selectedElement;
    
    private void Start()
    {
        gridSystemHeight = cellHeight * gridHeight;
        gridSystemWidth = cellWidth * gridWidth;
        selectedElement = belt;

        InitializeGridRender(out var lr);
        lineRenderer = lr;
        playerControls.Player.PlaceMachine.performed += placeMachine;
        playerControls.Player.SelectBelt.performed += SelectBelt;
        playerControls.Player.SelectItemSource.performed += SelectItemSource;
        playerControls.Player.SelectPulverizer.performed += SelectPulverizer;
        playerControls.Player.SelectItemTrophey.performed += SelectTrophey;
    }

    private void SelectBelt(InputAction.CallbackContext ctx)
    {
        selectedElement = belt;
    }
    
    private void SelectItemSource(InputAction.CallbackContext ctx)
    {
        selectedElement = itemSource;
    }
    
    private void SelectPulverizer(InputAction.CallbackContext ctx)
    {
        selectedElement = pulverizer;
    }
    
    private void SelectTrophey(InputAction.CallbackContext ctx)
    {
        selectedElement = trophey;
    }
    
    private void OnEnable()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    /// <summary>
    /// Instantiates a grid line renderer and returns it. The renderer will not update if the values change
    /// </summary>
    /// <param name="lr"></param>
    private void InitializeGridRender(out LineRenderer lr)
    {
        lr = gameObject.GetComponent<LineRenderer>();
        lr.positionCount = 5 + 3 * (gridHeight - 1) + 3 * (gridWidth - 1) + 1;

        // This just draws a rectangle with the bottom left at the position of the transform
        lr.SetPosition(0, new Vector3(transform.position.x, transform.position.y, 0));
        lr.SetPosition(1, new Vector3(transform.position.x, gridSystemHeight + transform.position.y, 0));
        lr.SetPosition(2,
            new Vector3(gridSystemWidth + transform.position.x, gridSystemHeight + transform.position.y, 0));
        lr.SetPosition(3, new Vector3(gridSystemWidth + transform.position.x, transform.position.y, 0));
        lr.SetPosition(4, new Vector3(transform.position.x, transform.position.y, 0));

        // Render the cross lines
        var index = 5;
        var yposition = transform.position.y;
        var xposition = transform.position.x;
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
    }

    private void placeMachine(InputAction.CallbackContext ctx)
    {
        var mouseWorldPoint =
            camera.ScreenToWorldPoint(new Vector2(Mouse.current.position.x.value, Mouse.current.position.y.value));

        var gridSpace = WorldToGridSpace(mouseWorldPoint);

        var factory = Factory.Instance;

        if (Mathf.Approximately(gridSpace.x, -1f))
        {
            return;
        }

        GameObject placedElement = factory.TryPlace(selectedElement, new int2((int)gridSpace.x, (int)gridSpace.y), out bool placed);
        if (placed)
        {
            placedElement.transform.position = GridToWorldSpace(new int2((int)gridSpace.x, (int)gridSpace.y));
        }
        
    }

    /// <summary>
    ///     Returns the grid coordinates of any 2d world space position. Returns -1,-1 if the space is outside the grid.
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    private Vector2 WorldToGridSpace(Vector2 worldPosition)
    {
        Debug.Log(worldPosition);
        Debug.Log(transform.position);
        // Bounds check
        if (worldPosition.x < transform.position.x || worldPosition.y < transform.position.y ||
            worldPosition.x > transform.position.x + gridSystemWidth ||
            worldPosition.y > transform.position.y + gridSystemHeight) return new Vector2(-1, -1);

        // If we get here then it's somewhere inside the grid
        // Recenter the grid to make math easier
        var centeredWorldPosition =
            new Vector2(worldPosition.x - transform.position.x, worldPosition.y - transform.position.y);

        var row = (int)Math.Floor(centeredWorldPosition.x / cellWidth);
        var column = (int)Math.Floor(centeredWorldPosition.y / cellHeight);

        return new Vector2(row, column);
    }

    /// <summary>
    ///     Returns the world space position of a grid position. Throws an IndexOutOfRangeException if the bound is not within
    ///     the grid
    /// </summary>
    /// <param name="gridPosition">The vector2 of the grid position</param>
    /// <returns>World space coordinates of the center of the grid cell</returns>
    private Vector2 GridToWorldSpace(int2 gridPosition)
    {
        if (gridPosition.x < 0 || gridPosition.x > gridWidth - 1 || gridPosition.y < 0 ||
            gridPosition.y > gridHeight - 1)
            throw new IndexOutOfRangeException("The provided coordinates are not valid in the grid");

        return new Vector2(gridPosition.x * cellWidth + transform.position.x,
            gridPosition.y * cellHeight + transform.position.y);
    }


    private void DrawCell(Vector2 position)
    {
    }
}