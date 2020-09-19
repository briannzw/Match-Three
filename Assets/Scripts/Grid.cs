using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int gridSizeX, gridSizeY;
    public Vector2 startPos, offset;

    public GameObject tilePrefab;

    public GameObject[,] tiles;
    public GameObject[] candies;

    public int specialChance = 10;

    private void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        tiles = new GameObject[gridSizeX, gridSizeY];

        offset = tilePrefab.GetComponent<SpriteRenderer>().bounds.size;
        startPos = transform.position + (Vector3.left * (offset.x * gridSizeX / 2) + (Vector3.down * (offset.y * gridSizeY / 3)));

        for (int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                Vector2 pos = new Vector3(startPos.x + (x * offset.x), startPos.y + (y * offset.y));

                GameObject backgroundTile = Instantiate(tilePrefab, pos, tilePrefab.transform.rotation);

                backgroundTile.transform.parent = transform;
                backgroundTile.name = "(" + x + "," + y + ")";

                int index = Random.Range(0, candies.Length);

                if(index == candies.Length - 1)
                {
                    if(!(Random.Range(1, 100) <= specialChance))
                    {
                        index = Random.Range(0, candies.Length - 1);
                    }
                }

                int MAX_ITERATION = 0;
                while(MatchesAt(x, y, candies[index]) && MAX_ITERATION < 100)
                {
                    index = Random.Range(0, candies.Length);

                    if (index == candies.Length - 1)
                    {
                        if (!(Random.Range(1, 100) <= specialChance))
                        {
                            index = Random.Range(0, candies.Length - 1);
                        }
                    }

                    MAX_ITERATION++;
                }

                GameObject candy = ObjectPooler.instance.SpawnFromPool(index.ToString(), pos, Quaternion.identity);

                candy.name = "(" + x + "," + y + ")";
                tiles[x, y] = candy;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece) //Random sampai tidak ada yang match
    {
        //Cek jika ada tile yg sama di bwh dan samping
        if (column > 1 && row > 1)
        {
            if((tiles[column - 1, row].tag == piece.tag || tiles[column - 1, row].tag == "Special") && (tiles[column - 2, row].tag == piece.tag || tiles[column - 2, row].tag == "Special"))
            {
                return true;
            }
            if((tiles[column, row - 1].tag == piece.tag || tiles[column, row - 1].tag == "Special") && (tiles[column, row - 2].tag == piece.tag || tiles[column, row - 2].tag == "Special"))
            {
                return true;
            }
        }
        else if(column <= 1 || row <= 1)
        {
            //Cek jika ada tile yg sama di atas dan samping
            if(row > 1)
            {
                if ((tiles[column, row - 1].tag == piece.tag || tiles[column, row - 1].tag == "Special") && (tiles[column, row - 2].tag == piece.tag || tiles[column, row - 2].tag == "Special"))
                {
                    return true;
                }
            }
            if(column > 1)
            {
                if((tiles[column - 1, row].tag == piece.tag || tiles[column - 1, row].tag == "Special") && (tiles[column - 2, row].tag == piece.tag || tiles[column - 2, row].tag == "Special"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if(tiles[column, row].GetComponent<Tile>().isMatched)
        {
            GameManager.instance.GetScore(10);
            tiles[column, row].gameObject.SetActive(false);
            tiles[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        StopAllCoroutines();
        GameManager.instance.GetCombo();

        for (int i = 0; i < gridSizeX; i++)
        {
            for(int j = 0; j < gridSizeY; j++)
            {
                if(tiles[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        
        StartCoroutine(DecreaseRow());
    }

    public void DestroyTags(string _tag)
    {
        foreach(var obj in GameObject.FindGameObjectsWithTag(_tag))
        {
            obj.GetComponent<Tile>().isMatched = true;
        }

        StartCoroutine(DestroyAfter(1));
    }

    private IEnumerator DestroyAfter(int time)
    {
        yield return new WaitForSeconds(time);

        DestroyMatches();
    }

    private void RefillBoard()
    {
        for(int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                if(tiles[x, y] == null)
                {
                    Vector2 tempPosition = new Vector2(startPos.x + (x * offset.x), startPos.y + (y * offset.y));
                    int candyToUse = Random.Range(0, candies.Length);

                    if (candyToUse == candies.Length - 1)
                    {
                        if (!(Random.Range(1, 100) <= specialChance))
                        {
                            candyToUse = Random.Range(0, candies.Length - 1);
                        }
                    }

                    GameObject tileToRefill = ObjectPooler.instance.SpawnFromPool(candyToUse.ToString(), tempPosition, Quaternion.identity); //buat objek baru


                    tileToRefill.GetComponent<Tile>().Initialize();
                    tiles[x, y] = tileToRefill;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for(int i = 0; i < gridSizeX; i++)
        {
            for(int j = 0; j < gridSizeY; j++)
            {
                if(tiles[i, j] != null)
                {
                    if(tiles[i, j].GetComponent<Tile>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private IEnumerator DecreaseRow()
    {
        int nullCount = 0;
        for(int i = 0; i < gridSizeX; i++)
        {
            for(int j = 0; j < gridSizeY; j++)
            {
                if(tiles[i, j] == null)
                {
                    nullCount++;
                }
                else if(nullCount > 0)
                {
                    tiles[i, j].GetComponent<Tile>().row -= nullCount; //Cek ke atas dan jatuhkan
                    tiles[i, j] = null;
                }
            }
            nullCount = 0;
        }

        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoard());
    }

    private IEnumerator FillBoard()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches(); //jika yang baru ada yang sama
        }
    }
}
