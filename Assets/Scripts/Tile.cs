using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Tile : MonoBehaviour
{
    private Vector3 firstPosition;
    private Vector3 finalPosition;
    private float swipeAngle;

    public float xPosition;
    public float yPosition;
    public int column;
    public int row;
    private Vector3 tempPosition;
    private Grid grid;
    private GameObject otherTile;

    public bool isMatched = false;

    private int previousColumn;
    private int previousRow;

    public static bool canMove = true;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        isMatched = false;
        grid = FindObjectOfType<Grid>();
        xPosition = transform.position.x;
        yPosition = transform.position.y;
        column = Mathf.RoundToInt((xPosition - grid.startPos.x) / grid.offset.x);
        row = Mathf.RoundToInt((yPosition - grid.startPos.y) / grid.offset.y);

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = Color.white;
    }

    private void Update()
    {
        if (isMatched)
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.color = Color.grey;
        }

        CheckMatches();

        xPosition = (column * grid.offset.x) + grid.startPos.x;
        yPosition = (row * grid.offset.y) + grid.startPos.y;
        SwipeTile();
    }

    private void OnMouseDown()
    {
        firstPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        finalPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }

    private void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(finalPosition.y - firstPosition.y, finalPosition.x - firstPosition.x) * Mathf.Rad2Deg;
        if (canMove)
        {
            canMove = false;
            GameManager.instance.AddMove();
            MoveTile();
        }
    }

    private void MoveTile()
    {
        previousColumn = column;
        previousRow = row;

        if(swipeAngle > -45 && swipeAngle <= 45)
        {
            //Right swipe
            SwipeRightMove();
        }
        else if(swipeAngle > 45 && swipeAngle <= 135)
        {
            //Up swipe
            SwipeUpMove();
        }
        else if(swipeAngle > 135 || swipeAngle <= -135)
        {
            //Left swipe
            SwipeLeftMove();
        }
        else if(swipeAngle < -45 && swipeAngle >= -135)
        {
            //Down swipe
            SwipeDownMove();
        }

        StartCoroutine(CheckMove());
    }

    private void SwipeRightMove()
    {
        if(column + 1 < grid.gridSizeX)
        {
            otherTile = grid.tiles[column + 1, row];
            otherTile.GetComponent<Tile>().column--;
            column++;
        }
    }

    private void SwipeUpMove()
    {
        if(row + 1 < grid.gridSizeY)
        {
            otherTile = grid.tiles[column, row + 1];
            otherTile.GetComponent<Tile>().row--;
            row++;
        }
    }

    private void SwipeLeftMove()
    {
        if(column - 1 >= 0)
        {
            otherTile = grid.tiles[column - 1, row];
            otherTile.GetComponent<Tile>().column++;
            column--;
        }
    }

    private void SwipeDownMove()
    {
        if(row - 1 >= 0)
        {
            otherTile = grid.tiles[column, row - 1];
            otherTile.GetComponent<Tile>().row++;
            row--;
        }
    }

    private void SwipeTile()
    {
        if(Mathf.Abs(xPosition - transform.position.x) > .1)
        {
            tempPosition = new Vector2(xPosition, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
            tempPosition = new Vector2(xPosition, transform.position.y);
            transform.position = tempPosition;
            grid.tiles[column, row] = this.gameObject;
        }

        if(Mathf.Abs(yPosition - transform.position.y) > .1)
        {
            tempPosition = new Vector2(transform.position.x, yPosition);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, yPosition);
            transform.position = tempPosition;
            grid.tiles[column, row] = this.gameObject;
        }
    }

    private void CheckMatches()
    {
        if(column > 0 && column < grid.gridSizeX - 1)
        {
            GameObject leftTile = grid.tiles[column - 1, row];
            GameObject rightTile = grid.tiles[column + 1, row];
            if(leftTile != null && rightTile != null && (leftTile != gameObject && rightTile != gameObject))
            {
                if(leftTile.CompareTag(gameObject.tag) && rightTile.CompareTag(gameObject.tag))
                {
                    isMatched = true;
                    rightTile.GetComponent<Tile>().isMatched = true;
                    leftTile.GetComponent<Tile>().isMatched = true;
                }
            }
        }

        if (row > 0 && row < grid.gridSizeY - 1)
        {
            GameObject upTile = grid.tiles[column, row + 1];
            GameObject downTile = grid.tiles[column, row - 1];
            if (upTile != null && downTile != null && (upTile != gameObject && downTile != gameObject))
            {
                if (upTile.CompareTag(gameObject.tag) && downTile.CompareTag(gameObject.tag))
                {
                    isMatched = true;
                    upTile.GetComponent<Tile>().isMatched = true;
                    downTile.GetComponent<Tile>().isMatched = true;
                }
            }
        }
    }

    IEnumerator CheckMove()
    {
        yield return new WaitForSeconds(.5f);

        if (otherTile != null)
        {
            if (!isMatched && !otherTile.GetComponent<Tile>().isMatched)
            {
                otherTile.GetComponent<Tile>().row = row;
                otherTile.GetComponent<Tile>().column = column;
                row = previousRow;
                column = previousColumn;
            }
            else
            {
                grid.DestroyMatches();
            }
        }

        otherTile = null;
    }
}
