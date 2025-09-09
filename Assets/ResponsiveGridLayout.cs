using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class ResponsiveGridLayout : MonoBehaviour
{
    private GridLayoutGroup gridLayoutGroup;
    private RectTransform rectTransform;

    private readonly Vector2 baseResolution = new Vector2(1920, 1080);
    private readonly Vector2 baseCellSpacing = new Vector2(20, 20);
    private RectOffset basePadding;

    private Vector2 lastScreenSize;
    private int lastChildCount = -1;

    public int columnNos = 2;

    void Awake()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();
        basePadding = new RectOffset(35, 35, 35, 35);
    }

    void Start()
    {
        AdjustLayout(columnNos);
    }

    void Update()
    {
        if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y || transform.childCount != lastChildCount)
        {
            AdjustLayout(columnNos);
        }
    }

    public void AdjustLayout(int columnCount)
    {
        
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = columnCount;
        int childCount = transform.childCount;
        

        float scaleFactor = Mathf.Min(Screen.width / baseResolution.x, Screen.height / baseResolution.y);

        gridLayoutGroup.spacing = baseCellSpacing * scaleFactor;
        gridLayoutGroup.padding = new RectOffset(
            Mathf.RoundToInt(basePadding.left * scaleFactor),
            Mathf.RoundToInt(basePadding.right * scaleFactor),
            Mathf.RoundToInt(basePadding.top * scaleFactor),
            Mathf.RoundToInt(basePadding.bottom * scaleFactor)
        );

        int rows = 0;
        int cols = 0;

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

        const float targetAspectRatio = 110f / 150f;

        float totalSpacingX = (cols > 1) ? (cols - 1) * gridLayoutGroup.spacing.x : 0;
        float totalSpacingY = (rows > 1) ? (rows - 1) * gridLayoutGroup.spacing.y : 0;

        float totalHorizontalPadding = gridLayoutGroup.padding.left + gridLayoutGroup.padding.right;
        float totalVerticalPadding = gridLayoutGroup.padding.top + gridLayoutGroup.padding.bottom;

        float availableWidth = rectTransform.rect.width - totalHorizontalPadding - totalSpacingX;
        float availableHeight = rectTransform.rect.height - totalVerticalPadding - totalSpacingY;

        float cellHeight = availableHeight / rows;
        float cellWidth = availableWidth / cols;

        if (cellWidth / targetAspectRatio > cellHeight)
        {
            cellWidth = cellHeight * targetAspectRatio;
        }
        else
        {
            cellHeight = cellWidth / targetAspectRatio;
        }

        gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);

        lastScreenSize = new Vector2(Screen.width, Screen.height);
        lastChildCount = childCount;
    }
}

