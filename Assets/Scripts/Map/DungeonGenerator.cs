using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private int width, height;
    private int maxRoomSize, minRoomSize;
    private int maxRooms;
    private int maxEnemies;
    private int maxItems;

    private int currentFloor = 0;

    List<Room> rooms = new List<Room>();

    // Lijst van vijanden in volgorde van sterkte
    public List<string> enemyPrefabs = new List<string> { "Ghost", "Spider", "Skeleton", "Demon", "Dragon", "Lich", "Behemoth", "Archdemon" };

    public void SetSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public void SetRoomSize(int min, int max)
    {
        minRoomSize = min;
        maxRoomSize = max;
    }

    public void SetMaxRooms(int max)
    {
        maxRooms = max;
    }

    public void SetMaxEnemies(int max)
    {
        maxEnemies = max;
    }

    public void SetMaxItems(int max)
    {
        maxItems = max;
    }

    public void SetCurrentFloor(int floor)
    {
        currentFloor = floor;
    }

    public void Generate()
    {
        rooms.Clear();

        for (int roomNum = 0; roomNum < maxRooms; roomNum++)
        {
            int roomWidth = Random.Range(minRoomSize, maxRoomSize);
            int roomHeight = Random.Range(minRoomSize, maxRoomSize);

            int roomX = Random.Range(0, width - roomWidth - 1);
            int roomY = Random.Range(0, height - roomHeight - 1);

            var room = new Room(roomX, roomY, roomWidth, roomHeight);

            if (room.Overlaps(rooms))
            {
                continue;
            }

            for (int x = roomX; x < roomX + roomWidth; x++)
            {
                for (int y = roomY; y < roomY + roomHeight; y++)
                {
                    if (x == roomX
                        || x == roomX + roomWidth - 1
                        || y == roomY
                        || y == roomY + roomHeight - 1)
                    {
                        if (!TrySetWallTile(new Vector3Int(x, y)))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        SetFloorTile(new Vector3Int(x, y, 0));
                    }
                }
            }

            if (rooms.Count != 0)
            {
                TunnelBetween(rooms[rooms.Count - 1], room);
            }

            // Plaats vijanden
            PlaceEnemies(room, maxEnemies);
            PlaceItems(room, maxItems);
            rooms.Add(room);
        }

        if (currentFloor == 0)
        {
            GameManager.Get.CreateActor("Player", rooms[0].Center());
        }
        else
        {
            GameManager.Get.MoveActorToPosition(GameManager.Get.Player, rooms[0].Center());
        }

        // Plaats een ladder naar beneden in het midden van de laatste kamer
        GameManager.Get.CreateLadder(new Vector2(rooms[rooms.Count - 1].X + rooms[rooms.Count - 1].Width / 2,
                                                  rooms[rooms.Count - 1].Y + rooms[rooms.Count - 1].Height / 2),
                                      true);

        if (currentFloor > 0)
        {
            // Plaats een ladder naar boven in het midden van de eerste kamer
            GameManager.Get.CreateLadder(new Vector2(rooms[0].X + rooms[0].Width / 2,
                                                      rooms[0].Y + rooms[0].Height / 2),
                                          false);
        }
    }

    private bool TrySetWallTile(Vector3Int pos)
    {
        if (MapManager.Get.FloorMap.GetTile(pos))
        {
            return false;
        }
        else
        {
            MapManager.Get.ObstacleMap.SetTile(pos, MapManager.Get.WallTile);
            return true;
        }
    }

    private void SetFloorTile(Vector3Int pos)
    {
        if (MapManager.Get.ObstacleMap.GetTile(pos))
        {
            MapManager.Get.ObstacleMap.SetTile(pos, null);
        }
        MapManager.Get.FloorMap.SetTile(pos, MapManager.Get.FloorTile);
    }

    private void TunnelBetween(Room oldRoom, Room newRoom)
    {
        Vector2Int oldRoomCenter = oldRoom.Center();
        Vector2Int newRoomCenter = newRoom.Center();
        Vector2Int tunnelCorner;

        if (Random.value < 0.5f)
        {
            tunnelCorner = new Vector2Int(newRoomCenter.x, oldRoomCenter.y);
        }
        else
        {
            tunnelCorner = new Vector2Int(oldRoomCenter.x, newRoomCenter.y);
        }

        List<Vector2Int> tunnelCoords = new List<Vector2Int>();
        BresenhamLine.Compute(oldRoomCenter, tunnelCorner, tunnelCoords);
        BresenhamLine.Compute(tunnelCorner, newRoomCenter, tunnelCoords);

        for (int i = 0; i < tunnelCoords.Count; i++)
        {
            SetFloorTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y));

            for (int x = tunnelCoords[i].x - 1; x <= tunnelCoords[i].x + 1; x++)
            {
                for (int y = tunnelCoords[i].y - 1; y <= tunnelCoords[i].y + 1; y++)
                {
                    if (!TrySetWallTile(new Vector3Int(x, y, 0)))
                    {
                        continue;
                    }
                }
            }
        }
    }

    private void PlaceEnemies(Room room, int maxEnemies)
    {
        // Aantal vijanden dat we willen plaatsen
        int num = Random.Range(0, maxEnemies + 1);

        // Loop door het aantal vijanden dat we willen plaatsen
        for (int counter = 0; counter < num; counter++)
        {
            // Randen van de kamer zijn muren, dus voeg en trek 1 af
            int x = Random.Range(room.X + 1, room.X + room.Width - 1);
            int y = Random.Range(room.Y + 1, room.Y + room.Height - 1);

            // Creeer verschillende vijanden
            string enemyPrefabName = GetEnemyPrefabName();
            GameManager.Get.CreateActor(enemyPrefabName, new Vector2(x, y));
        }
    }

    // Kies een vijand prefabnaam op basis van de huidige verdieping
    private string GetEnemyPrefabName()
    {
        // Bereken de index van de vijand prefab in de lijst op basis van de huidige verdieping
        int index = Mathf.Clamp(currentFloor, 0, enemyPrefabs.Count - 1);
        return enemyPrefabs[index];
    }

    private void PlaceItems(Room room, int maxItems)
    {
        int numItems = Random.Range(0, maxItems + 1);

        for (int i = 0; i < numItems; i++)
        {
            int x = Random.Range(room.X + 1, room.X + room.Width - 1);
            int y = Random.Range(room.Y + 1, room.Y + room.Height - 1);

            // Willekeurig een itemnaam selecteren met behulp van een if-statement
            if (Random.value < 0.33f)
            {
                GameManager.Get.CreateItem("Healthpotion", new Vector2(x, y));
            }
            else if (Random.value < 0.66f)
            {
                GameManager.Get.CreateItem("Fireball", new Vector2(x, y));
            }
            else
            {
                GameManager.Get.CreateItem("ScrollOfConfusion", new Vector2(x, y));
            }
        }
    }
}

