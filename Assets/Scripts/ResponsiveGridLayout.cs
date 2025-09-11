using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class ResponsiveGridLayout : MonoBehaviour
{
    private GridLayoutGroup gridLayoutGroup;  // Reference to the GridLayoutGroup component
    private RectTransform rectTransform;      // Reference to this object's RectTransform

    private readonly Vector2 baseResolution = new Vector2(1920, 1080); // Base resolution for scaling
    private readonly Vector2 baseCellSpacing = new Vector2(20, 20);    // Default spacing between cells
    private RectOffset basePadding;                                    // Default padding

    private Vector2 lastScreenSize;    // To track changes in screen size
    private int lastChildCount = -1;   // To track changes in number of children

    public int columnNos = 2;          // Default number of columns

    void Awake()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();
        basePadding = new RectOffset(35, 35, 35, 35); // Initial padding values
    }

    void Start()
    {
        AdjustLayout(columnNos); // Initial layout setup
    }

    void Update()
    {
        // Recalculate layout if screen size or child count changes
        if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y || transform.childCount != lastChildCount)
        {
            AdjustLayout(columnNos);
        }
    }

    public void AdjustLayout(int columnCount)
    {
        // Force grid to use fixed number of columns
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = columnCount;
        int childCount = transform.childCount;

        // Calculate scale factor based on resolution
        float scaleFactor = Mathf.Min(Screen.width / baseResolution.x, Screen.height / baseResolution.y);

        // Adjust spacing and padding according to scale
        gridLayoutGroup.spacing = baseCellSpacing * scaleFactor;
        gridLayoutGroup.padding = new RectOffset(
            Mathf.RoundToInt(basePadding.left * scaleFactor),
            Mathf.RoundToInt(basePadding.right * scaleFactor),
            Mathf.RoundToInt(basePadding.top * scaleFactor),
            Mathf.RoundToInt(basePadding.bottom * scaleFactor)
        );

        int rows = 0;
        int cols = 0;

        // Determine rows and columns depending on constraint type
        if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            cols = gridLayoutGroup.constraintCount;
            rows = Mathf.CeilToInt((float)childCount / cols);
        }
        else if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedRowCount)
        {
            rows = gridLayoutGroup.constraintCount;
            cols = Mathf.CeilToInt((float)childCount / rows);
        }
        else
        {
            cols = Mathf.CeilToInt(Mathf.Sqrt(childCount));
            rows = Mathf.CeilToInt((float)childCount / cols);
        }

        const float targetAspectRatio = 110f / 150f; // Desired aspect ratio for each cell

        // Calculate available space inside grid (accounting for padding and spacing)
        float totalSpacingX = (cols > 1) ? (cols - 1) * gridLayoutGroup.spacing.x : 0;
        float totalSpacingY = (rows > 1) ? (rows - 1) * gridLayoutGroup.spacing.y : 0;

        float totalHorizontalPadding = gridLayoutGroup.padding.left + gridLayoutGroup.padding.right;
        float totalVerticalPadding = gridLayoutGroup.padding.top + gridLayoutGroup.padding.bottom;

        float availableWidth = rectTransform.rect.width - totalHorizontalPadding - totalSpacingX;
        float availableHeight = rectTransform.rect.height - totalVerticalPadding - totalSpacingY;

        // Initial cell size calculation
        float cellHeight = availableHeight / rows;
        float cellWidth = availableWidth / cols;

        // Adjust cell size to maintain target aspect ratio
        if (cellWidth / targetAspectRatio > cellHeight)
        {
            cellWidth = cellHeight * targetAspectRatio;
        }
        else
        {
            cellHeight = cellWidth / targetAspectRatio;
        }

        // Apply final cell size
        gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);

        // Save state for next update
        lastScreenSize = new Vector2(Screen.width, Screen.height);
        lastChildCount = childCount;
    }
}
