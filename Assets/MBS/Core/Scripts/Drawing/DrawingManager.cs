#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MBS
{
    public class DrawingManager
    {
        public MBSBuilder builder;
        public List<DrawingWall> walls;
        private List<Vector3> allEndPoints;
        private List<Vector3> allEndPoints_ForSnaps;

        public int floorNumber;
        public int wallNumber;
        public int wall_name_iterator;

        public SceneData sntd
        {
            get => builder._sceneData;
            set => builder._sceneData = value;
        }
        public AssetsData astd
        {
            get => builder._assetsData;
            set => builder._assetsData = value;
        }
        public List<DrawingWall> Items
        {
            get => walls;
            set => walls = value;
        }

        public DrawingManager(MBSBuilder builder)
        {
            this.builder = builder;
            this.walls = new List<DrawingWall>();
        }

        public void Initialize()
        {
            FindChildrenWalls();
            UpdateEndPointsList();
        }

        public void FindChildrenWalls()
        {
            if (walls == null)
                walls = new List<DrawingWall>();
            else walls.Clear();

            List<DrawingWall> list = builder.transform.GetComponentsInChildren<DrawingWall>(true).ToList();
            foreach (DrawingWall dw in list)
            {
                if (dw.Builder != this.builder)
                    dw.Builder = this.builder;
            }
            foreach (DrawingWall dw in list)
            {
                AddDrawingWall(dw, false, true);
            }
        }


        public void UpdateEndPointsList()
        {
            if (this.walls != null)
            {
                walls = walls.Distinct().ToList();
                walls = walls.Where(i => i != null).ToList();

                allEndPoints = new List<Vector3>();
                if (walls.Count > 0)
                {
                    for (int i = 0; i < walls.Count; i++)
                    {
                        if (walls[i].FrontEndPoint != null)
                            allEndPoints.Add(walls[i].FrontEndPoint);
                        if (walls[i].RearEndPoint != null)
                            allEndPoints.Add(walls[i].RearEndPoint);
                    }
                    allEndPoints = allEndPoints.Distinct().ToList();
                }

                allEndPoints_ForSnaps = new List<Vector3>();
                allEndPoints_ForSnaps.Add(Vector3.zero);
                if (walls.Count > 0)
                {
                    for (int i = 0; i < walls.Count; i++)
                    {
                        if (walls[i].FrontEndPoint != null)
                        {
                            Vector3 v = walls[i].FrontEndPoint;
                            v.y = 0;
                            allEndPoints_ForSnaps.Add(v);
                        }
                        if (walls[i].RearEndPoint != null)
                        {
                            Vector3 v = walls[i].RearEndPoint;
                            v.y = 0;
                            allEndPoints_ForSnaps.Add(v);
                        }
                    }
                    allEndPoints_ForSnaps = allEndPoints_ForSnaps.Distinct().ToList();
                }
            }
        }

        public void AddEndPointsToList(DrawingWall wall)
        {
            if (wall != null && wall.FrontEndPoint != null && wall.RearEndPoint != null)
            {
                if (allEndPoints == null) allEndPoints = new List<Vector3>();
                if (!allEndPoints.Contains(wall.FrontEndPoint)) allEndPoints.Add(wall.FrontEndPoint);
                if (!allEndPoints.Contains(wall.RearEndPoint)) allEndPoints.Add(wall.RearEndPoint);

                if (allEndPoints_ForSnaps == null) allEndPoints_ForSnaps = new List<Vector3>();
                Vector3 front = wall.FrontEndPoint;
                front.y = 0;
                Vector3 rear = wall.RearEndPoint;
                rear.y = 0;
                if (!allEndPoints_ForSnaps.Contains(front)) allEndPoints_ForSnaps.Add(front);
                if (!allEndPoints_ForSnaps.Contains(rear)) allEndPoints_ForSnaps.Add(rear);
            }
        }

        public bool TryFindEndPointToSnap(Vector3 localPos_NG, float radius, out Vector3 outPosition)
        {
            outPosition = default;

            float minDistance = float.MaxValue;
            int minDistIndex = -1;

            localPos_NG.y = 0;
            Vector3 endPoint = Vector3.zero;

            if (allEndPoints_ForSnaps == null || allEndPoints_ForSnaps.Count == 0)
                UpdateEndPointsList();

            for (int i = 0; i < allEndPoints_ForSnaps.Count; i++)
            {
                float distance = Vector3.Distance(localPos_NG, allEndPoints_ForSnaps[i]);
                if (distance <= radius)
                {
                    minDistance = distance;
                    minDistIndex = i;
                }
            }

            if (minDistIndex == -1)
                return false;

            outPosition = allEndPoints_ForSnaps[minDistIndex];
            return true;
        }

        public bool TryFindEndPointToSnap(Vector3 localPos_NG, Vector3 localPos_G, float radius, out Vector3 outPosition)
        {
            outPosition = default;

            float minDistance = float.MaxValue;
            int minDistIndex = -1;

            localPos_NG.y = 0;
            Vector3 endPoint = Vector3.zero;

            if (allEndPoints_ForSnaps == null || allEndPoints_ForSnaps.Count == 0)
                UpdateEndPointsList();

            for (int i = 0; i < allEndPoints_ForSnaps.Count; i++)
            {
                float distance = Vector3.Distance(localPos_NG, allEndPoints_ForSnaps[i]);
                if (distance <= radius)
                {
                    minDistance = distance;
                    minDistIndex = i;
                }
            }

            if (minDistIndex == -1)
            {
                for (int i = 0; i < allEndPoints_ForSnaps.Count; i++)
                {
                    float distance = Vector3.Distance(localPos_G, allEndPoints_ForSnaps[i]);
                    if (distance <= radius)
                    {
                        minDistance = distance;
                        minDistIndex = i;
                    }
                }
            }

            if (minDistIndex == -1)
                return false;

            outPosition = allEndPoints_ForSnaps[minDistIndex];
            return true;
        }


        public List<DrawingWall> isTheneItemsAtPosition(DrawingWall wall, Vector3 position1, Vector3 position2)
        {
            List<DrawingWall> retval = new List<DrawingWall>();
            if (walls != null && walls.Count > 0)
            {
                for (int i = 0; i < walls.Count; i++)
                {
                    if (walls[i] == wall) continue;

                    if (walls[i].FrontEndPoint == position1 || walls[i].RearEndPoint == position1 ||
                        walls[i].FrontEndPoint == position2 || walls[i].RearEndPoint == position2)
                        retval.Add(walls[i]);
                }
            }
            return retval;
        }
        public bool IsThereWallOnLine(Vector3 start, Vector3 end)
        {
            int pointsToCheck = 3;

            start = builder.WPos(start);
            end = builder.WPos(end);

            Vector3 line = end - start;

            float sideOffset = line.magnitude / 10;
            float shiftStep = (line.magnitude + (sideOffset * 2)) / pointsToCheck;

            int rayDistance = 1000;
            Vector3 distanceUp = Vector3.up * rayDistance;
            Vector3 distanceDown = Vector3.down * rayDistance;


            RaycastHit[] hits;
            bool[] hitChecks = new bool[pointsToCheck];

            GameObject wall = null;

            for (int i = 0; i < pointsToCheck; i++)
            {
                float shift = sideOffset + (shiftStep * i);
                Vector3 pointAlongLine = Vector3.Lerp(start, end, shift);
                Vector3 from = pointAlongLine + distanceUp;

                hits = Physics.RaycastAll(from, Vector3.down, rayDistance + 100);

                wall = null;
                if (hits != null && hits.Length > 0)
                {
                    for (int j = 0; j < hits.Length; j++)
                    {

                        if (hits[j].transform.tag != DefaultConfig.TAG_WALL) continue;

                        if (hits[j].transform.position.y == start.y)
                        {
                            wall = hits[j].transform.gameObject;
                        }
                    }
                }

                if (wall == null)
                    hitChecks[i] = false;
                else
                    hitChecks[i] = true;
            }

            bool isAll = hitChecks.All(p => p == true);
            return isAll;
        }


        public void RemoveDrawingItem(DrawingWall item, bool doUpdateMeshes)
        {
            if (item != null && walls != null && walls.Contains(item))
                walls.Remove(item);

            List<DrawingWall> frontConnections = item.FrontConnections;
            List<DrawingWall> rearConnections = item.RearConnections;

            if (frontConnections != null && frontConnections.Count > 0)
            {
                for (int i = 0; i < frontConnections.Count; i++)
                {
                    if (frontConnections[i] != null)
                        frontConnections[i].RemoveConnection(item, false);
                }
                frontConnections[0].RecalculateBothSides(doUpdateMeshes);
            }
            if (rearConnections != null && rearConnections.Count > 0)
            {
                for (int i = 0; i < rearConnections.Count; i++)
                {
                    if (rearConnections[i])
                        rearConnections[i].RemoveConnection(item, false);
                }
                rearConnections[0].RecalculateBothSides(doUpdateMeshes);
            }

            UpdateEndPointsList();
        }

        public void AddDrawingWall(DrawingWall item, bool doInstantiateMesh, bool doCreateArea = true)
        {
            if (walls == null)
                walls = new List<DrawingWall>();
            else
                walls.RemoveAll(i => i == null);

            List<DrawingWall> findedObjectsInPos = isTheneItemsAtPosition(item, item.FrontEndPoint, item.RearEndPoint);

            if (findedObjectsInPos.Count != 0)
            {
                for (int i = 0; i < findedObjectsInPos.Count; i++)
                {
                    if (item.RearEndPoint == findedObjectsInPos[i].RearEndPoint)
                    {
                        item.AddRearConnection(findedObjectsInPos[i]);
                        findedObjectsInPos[i].AddRearConnection(item);
                    }

                    if (item.FrontEndPoint == findedObjectsInPos[i].FrontEndPoint)
                    {
                        item.AddFrontConnection(findedObjectsInPos[i]);
                        findedObjectsInPos[i].AddFrontConnection(item);
                    }

                    if (item.FrontEndPoint == findedObjectsInPos[i].RearEndPoint)
                    {
                        item.AddFrontConnection(findedObjectsInPos[i]);
                        findedObjectsInPos[i].AddRearConnection(item);
                    }

                    if (item.RearEndPoint == findedObjectsInPos[i].FrontEndPoint)
                    {
                        item.AddRearConnection(findedObjectsInPos[i]);
                        findedObjectsInPos[i].AddFrontConnection(item);
                    }
                }
                item.RecalculateBothSides(doInstantiateMesh);
            }

            walls.Add(item);
            AddEndPointsToList(item);

            if (item.FrontConnections.Count == 0 || item.RearConnections.Count == 0) return;
            if (doCreateArea == false) return;

            LoopFinder.FindLoops(item, builder);
        }
















        public static (bool isThereDiagonal, DrawingCorner corner) IsDiagonalAtPointRaycast(Vector3 localPos, Vector3 worldPos, MBSBuilder builder, out Vector3[] cornersPositions)
        {
            AssetsData astd = builder._assetsData;

            Vector3 topRight, botRight, topLeft, botLeft;
            topRight = localPos + (new Vector3(builder._assetsData.Current_Asset_Size.x, 0, builder._assetsData.Current_Asset_Size.z) / 2);
            botRight = localPos + (new Vector3(builder._assetsData.Current_Asset_Size.x, 0, -builder._assetsData.Current_Asset_Size.z) / 2);
            botLeft = localPos + (new Vector3(-builder._assetsData.Current_Asset_Size.x, 0, -builder._assetsData.Current_Asset_Size.z) / 2);
            topLeft = localPos + (new Vector3(-builder._assetsData.Current_Asset_Size.x, 0, builder._assetsData.Current_Asset_Size.z) / 2);

            bool d1 = builder._drawingManager.IsThereWallOnLine(topRight, botLeft);
            bool d2 = builder._drawingManager.IsThereWallOnLine(topLeft, botRight);

            topRight = localPos + (new Vector3(builder._assetsData.Current_Asset_Size.x, 0, builder._assetsData.Current_Asset_Size.z) / 4);
            botRight = localPos + (new Vector3(builder._assetsData.Current_Asset_Size.x, 0, -builder._assetsData.Current_Asset_Size.z) / 4);
            botLeft = localPos + (new Vector3(-builder._assetsData.Current_Asset_Size.x, 0, -builder._assetsData.Current_Asset_Size.z) / 4);
            topLeft = localPos + (new Vector3(-builder._assetsData.Current_Asset_Size.x, 0, builder._assetsData.Current_Asset_Size.z) / 4);
            cornersPositions = new[] { topRight, botRight, botLeft, topLeft };

            if (d1 && !d2)
            {
                bool topLeftInArea = builder._areaManager.GetAreaAtPoint(topLeft) == builder._sceneData.drawingInArea;
                bool botRightInArea = builder._areaManager.GetAreaAtPoint(botRight) == builder._sceneData.drawingInArea;

                if (topLeftInArea)
                {
                    return (true, DrawingCorner.TopLeft);
                }
                else if (botRightInArea)
                {
                    return (true, DrawingCorner.BotRight);
                }
            }
            else if (!d1 && d2)
            {
                bool topRightInArea = builder._areaManager.GetAreaAtPoint(topRight) == builder._sceneData.drawingInArea;
                bool botLeftInArea = builder._areaManager.GetAreaAtPoint(botLeft) == builder._sceneData.drawingInArea;

                if (topRightInArea)
                {
                    return (true, DrawingCorner.TopRight);
                }
                else if (botLeftInArea)
                {
                    return (true, DrawingCorner.BotLeft);
                }
            }
            else
            {
            }
            return (false, DrawingCorner.None);
        }

        public static (bool isThereDiagonal, DrawingCorner corner) IsDiagonalAtPointInnerArea(Vector3 localPos, Vector3 assetSize, AreaManager areaManager, Area drawingInArea)
        {
            Vector3 topRight, botRight, topLeft, botLeft;

            topRight = localPos + (new Vector3(assetSize.x, 0, assetSize.z) / 3);
            botRight = localPos + (new Vector3(assetSize.x, 0, -assetSize.z) / 3);
            botLeft = localPos + (new Vector3(-assetSize.x, 0, -assetSize.z) / 3);
            topLeft = localPos + (new Vector3(-assetSize.x, 0, assetSize.z) / 3);

            int neededNumber = 0;

            Area a_topRight = areaManager.GetAreaAtPoint(topRight);
            Area a_botRight = areaManager.GetAreaAtPoint(botRight);
            Area a_botLeft = areaManager.GetAreaAtPoint(botLeft);
            Area a_topLeft = areaManager.GetAreaAtPoint(topLeft);

            if (a_topRight == drawingInArea) neededNumber++;
            if (a_botRight == drawingInArea) neededNumber++;
            if (a_botLeft == drawingInArea) neededNumber++;
            if (a_topLeft == drawingInArea) neededNumber++;

            if (neededNumber == 2 || neededNumber == 4)
                return (false, DrawingCorner.All);

            else if (a_topRight == drawingInArea && a_botLeft != drawingInArea)
            {
                return (true, DrawingCorner.TopRight);
            }
            else if (a_botRight == drawingInArea && a_topLeft != drawingInArea)
            {
                return (true, DrawingCorner.BotRight);
            }
            else if (a_botLeft == drawingInArea && a_topRight != drawingInArea)
            {
                return (true, DrawingCorner.BotLeft);
            }
            else if (a_topLeft == drawingInArea && a_botRight != drawingInArea)
            {
                return (true, DrawingCorner.TopLeft);
            }

            return (false, DrawingCorner.None);
        }

        public void BeginDrawingFloor(SceneData sd)
        {
            sd.drawingGroupRoot = new GameObject("DrawingFloor " + (floorNumber + 1));
            sd.drawingGroupRoot.transform.SetParent(builder.transform, false);
            sd.drawingGroupRoot.transform.localPosition = Vector3.zero;

            sd.currentLineIndex = 0;
            sd.drawingFloorLines = new List<GameObject>();
            sd.drawingFloorLinesItems = new List<List<(GameObject g1, GameObject g2)>>();
            sd.drawingFloorTileChecker = new List<List<bool>>();
            sd.tileLocalPositions = new List<List<Vector3>>();
            sd.drawingInArea = builder._areaManager.GetAreaAtPoint(sd.mCurrentPosNG);
        }

        public void BeginDrawingFloorLine(int lineNumber, SceneData sd)
        {
            GameObject newLine = new GameObject("FloorLine " + lineNumber);
            newLine.transform.SetParent(sd.drawingGroupRoot.transform, false);
            newLine.transform.localPosition = sd.mStartPosG;

            sd.drawingFloorLines.Add(newLine);
            sd.tileLocalPositions.Add(new List<Vector3>());
            sd.drawingFloorLinesItems.Add(new List<(GameObject g1, GameObject g2)>());
        }

        public (GameObject g1, GameObject g2) CreateFloorItemInPosition(int lineIndex, int itemIndex, Vector3 localPos)
        {
            GameObject g1 = null;
            GameObject g2 = null;
            GameObject squarePrefab = builder._assetsData.Current_Asset_FirstPrefab(FloorTileType.Square);
            GameObject cornerPrefab = builder._assetsData.Current_Asset_FirstPrefab(FloorTileType.Corner);

            Vector3 worldPos = builder.WPos(localPos);

            bool pointerInsideDrawingArea = builder._areaManager.GetAreaAtPoint(localPos) == builder._sceneData.drawingInArea;

            if (!pointerInsideDrawingArea)
            {
                var diagonalCheck = DrawingManager.IsDiagonalAtPointInnerArea(localPos, astd.Current_Asset_Size, builder._areaManager, builder._sceneData.drawingInArea);

                if (diagonalCheck.isThereDiagonal)
                {
                    if (cornerPrefab != null)
                    {
                        g1 = GameObject.Instantiate(cornerPrefab);
                        FloorMeshModifier.ModifyFloorCorner(g1, diagonalCheck.corner, builder._assetsData.Current_Asset_FirstPrefab_Type(FloorTileType.Corner));
                    }
                    else g1 = GameObject.Instantiate(squarePrefab);
                }
                else if (diagonalCheck.corner == DrawingCorner.All)
                {
                    g1 = GameObject.Instantiate(squarePrefab);
                }
                else
                {
                    return (null, null);
                }
            }
            else
            {
                var diagonalCheck = DrawingManager.IsDiagonalAtPointRaycast(localPos, worldPos, builder, out Vector3[] cornerPositions);

                if (diagonalCheck.isThereDiagonal)
                {
                    if (cornerPrefab != null)
                    {
                        g1 = GameObject.Instantiate(cornerPrefab);
                        FloorMeshModifier.ModifyFloorCorner(g1, diagonalCheck.corner, builder._assetsData.Current_Asset_FirstPrefab_Type(FloorTileType.Corner));

                        (bool oppositeInArea, DrawingCorner corner) secondCheck = default;

                        if (diagonalCheck.corner == DrawingCorner.TopRight)
                        {
                            secondCheck = (builder._areaManager.GetAreaAtPoint(cornerPositions[2]) == builder._sceneData.drawingInArea, DrawingCorner.BotLeft);
                        }
                        else if (diagonalCheck.corner == DrawingCorner.BotRight)
                        {
                            secondCheck = (builder._areaManager.GetAreaAtPoint(cornerPositions[3]) == builder._sceneData.drawingInArea, DrawingCorner.TopLeft);
                        }
                        else if (diagonalCheck.corner == DrawingCorner.BotLeft)
                        {
                            secondCheck = (builder._areaManager.GetAreaAtPoint(cornerPositions[0]) == builder._sceneData.drawingInArea, DrawingCorner.TopRight);
                        }
                        else if (diagonalCheck.corner == DrawingCorner.TopLeft)
                        {
                            secondCheck = (builder._areaManager.GetAreaAtPoint(cornerPositions[1]) == builder._sceneData.drawingInArea, DrawingCorner.BotRight);
                        }

                        if (secondCheck.oppositeInArea)
                        {
                            g2 = GameObject.Instantiate(cornerPrefab);
                            FloorMeshModifier.ModifyFloorCorner(g2, secondCheck.corner, builder._assetsData.Current_Asset_FirstPrefab_Type(FloorTileType.Corner));
                        }
                    }
                    else g1 = GameObject.Instantiate(squarePrefab);
                }
                else
                {
                    var secondCheck = DrawingManager.IsDiagonalAtPointInnerArea(localPos, builder._assetsData.Current_Asset_Size, builder._areaManager, builder._sceneData.drawingInArea);
                    if (secondCheck.isThereDiagonal && cornerPrefab != null)
                    {
                        g1 = GameObject.Instantiate(cornerPrefab);
                        FloorMeshModifier.ModifyFloorCorner(g1, secondCheck.corner, builder._assetsData.Current_Asset_FirstPrefab_Type(FloorTileType.Corner));
                    }
                    else
                        g1 = GameObject.Instantiate(squarePrefab);
                }
            }

            g1.name = "FloorTile " + itemIndex;
            g1.transform.SetParent(sntd.drawingFloorLines[lineIndex].transform, false);
            g1.transform.localPosition = sntd.drawingFloorLines[lineIndex].transform.InverseTransformPoint(worldPos);
            if (g2 != null)
            {
                g2.name = "FloorTile " + itemIndex + "_1";
                g2.transform.SetParent(sntd.drawingFloorLines[lineIndex].transform, false);
                g2.transform.localPosition = sntd.drawingFloorLines[lineIndex].transform.InverseTransformPoint(worldPos);
            }
            return (g1, g2);
        }

        public void AutoFillFloor(SceneData sd, AssetsData ad)
        {
            int lineNameIterator = 0;
            int itemNameIterator = 0;
            int rX = 0;
            int rZ = 0;

            int longSide = Mathf.CeilToInt(Mathf.Max(sd.drawingInArea.areaSize.x, sd.drawingInArea.areaSize.y));
            sd.drawingGroupRoot.transform.right = builder.transform.right;

            for (int x = -longSide; x < longSide; x++)
            {
                Vector3 localLinePos = (Vector3.right * x * ad.GridSize) + sd.mStartPosG;

                BeginDrawingFloorLine(lineNameIterator, sd);

                itemNameIterator = 0;
                rZ = 0;

                for (int z = -longSide; z < longSide; z++)
                {
                    Vector3 localTilePos = Vector3.forward * ad.GridSize * z + localLinePos;
                    sd.drawingFloorLinesItems[rX].Add(CreateFloorItemInPosition(rX, itemNameIterator, localTilePos));

                    if (sd.drawingFloorLinesItems[rX][rZ].g1 != null)
                        itemNameIterator++;

                    rZ++;
                }

                if (sd.drawingFloorLines[rX].transform.childCount == 0)
                {
                    GameObject.DestroyImmediate(sd.drawingFloorLines[rX]);
                    sd.drawingFloorLines[rX] = null;
                }
                else lineNameIterator++;

                sd.currentLineIndex++;
                rX++;
            }
        }

        public void DestroyFloorItemFromLinesAt(int lineIndex, int itemIndex)
        {
            GameObject.DestroyImmediate(sntd.drawingFloorLinesItems[lineIndex][itemIndex].g1);
            GameObject.DestroyImmediate(sntd.drawingFloorLinesItems[lineIndex][itemIndex].g2);
            sntd.drawingFloorLinesItems[lineIndex].RemoveAt(itemIndex);
        }

        public void RemoveDestroyFloorLineAt(int index)
        {
            GameObject.DestroyImmediate(sntd.drawingFloorLines[index]);
            sntd.drawingFloorLines.RemoveAt(index);
            sntd.drawingFloorLinesItems.RemoveAt(index);
        }

        public void EndDrawingFloorLine()
        {
            sntd.drawingFloorLines = null;
        }

        public void EndDrawingFloor(SceneData sd, string undoText = "Drawing Floor")
        {
            if (sd.drawingFloorLines.Count > 0)
            {
                Undo.IncrementCurrentGroup();
                Undo.SetCurrentGroupName(undoText);
                sd.drawingGroupRoot.RecordCreatedUndo("Floor group");

                for (int i = 0; i < sd.drawingFloorLines.Count; i++)
                {
                    if (sd.drawingFloorLines[i] != null)
                        sd.drawingFloorLines[i].RecordCreatedUndo("floor tile line");
                }
            }

            sd.drawingGroupRoot = null;
            sd.drawingFloorLines = null;
            sd.drawingFloorLinesItems = null;
            sd.drawingFloorTileChecker = null;
            sd.tileLocalPositions = null;
            sd.drawingInArea = null;
        }

        public void BeginDrawingWall(SceneData sd, AssetsData ad)
        {
            sd.drawingGroupRoot = new GameObject("WallGroup");
            sd.drawingGroupRoot.transform.SetParent(builder.transform, true);
            sd.drawingGroupRoot.transform.localPosition = sntd.mStartPosG;

            sd.drawingWallItems = new List<GameObject>();

        }

        public void EndDrawingWall(SceneData sd, AssetsData ad)
        {
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("MBS Drawing Wall");
            sd.drawingGroupRoot.RecordCreatedUndo();
            builder.RecordObjectUndo();

            int count = sd.drawingWallItems.Count;
            bool even = (sd.drawingWallItems.Count % 2) == 0;
            bool instantiateMesh = false;

            for (int i = 0; i < count; i++)
            {
                sd.drawingWallItems[i].AddComponent<MeshCollider>();
                sd.drawingWallItems[i].tag = DefaultConfig.TAG_WALL;
                sd.drawingWallItems[i].name = "DrawingWall " + wall_name_iterator;

                DrawingWall dw = sntd.drawingWallItems[i].AddComponent<DrawingWall>();
                dw.InitializeAfterDrawing(builder, ad.Current_Asset, ad.Current_Asset_Size, ad.Current_Asset_FirstPrefab_Type(), sd.drawingAt45Deg);

                if (i == 0 || i == count - 1 || (i % 2) != 0)
                    instantiateMesh = true;

                builder._drawingManager.AddDrawingWall(dw, instantiateMesh, true);

                this.wall_name_iterator++;
            }
        }

    }
}
#endif