#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MBS
{
    public class LoopFinder
    {
        public static void FindLoops(DrawingWall item, MBSBuilder builder)
        {
            var leftPath = FindLoop(false, item, item);
            var rightPath = FindLoop(true, item, item);

            if (rightPath.isProper && leftPath.isProper)
            {
                float leftArea = Area.CalcArea(leftPath.lists.points.ToArray());
                float rightArea = Area.CalcArea(rightPath.lists.points.ToArray());

                if (leftArea < rightArea)
                {
                    builder._areaManager.AddNewArea(leftPath.lists.items);
                }
                else if (leftArea > rightArea)
                {
                    builder._areaManager.AddNewArea(rightPath.lists.items);
                }
                else
                {
                    builder._areaManager.AddNewArea(leftPath.lists.items);
                }
            }
        }


        public static (bool isProper, (List<DrawingWall> items, List<Vector3> points) lists) FindLoop(
            bool isInRight,
            DrawingWall firstItem,
            DrawingWall currentItem,
            bool isInFront = true,
            Vector3 cameFrom = default,
            List<DrawingWall> visitedItems = null,
            List<Vector3> visitedPoints = null)
        {
            if (firstItem == null) return (false, (null, null));
            if (currentItem == null) return (false, (null, null));
            if (cameFrom == default) cameFrom = firstItem.RearEndPoint;
            if (visitedItems == null) visitedItems = new List<DrawingWall>();
            if (visitedPoints == null) visitedPoints = new List<Vector3>();

            visitedPoints.Add(cameFrom);
            cameFrom = (isInFront) ? currentItem.FrontEndPoint : currentItem.RearEndPoint;

            if (visitedPoints.Contains(cameFrom))
            {
                if (visitedItems.Contains(currentItem)) return (false, (null, null));


                if (cameFrom == firstItem.RearEndPoint)
                {
                    visitedItems.Add(currentItem);
                    return (true, (visitedItems, visitedPoints));
                }
                else return (false, (null, null));
            }
            else visitedItems.Add(currentItem);


            List<(DrawingWall item, bool isInRight)> itemConnections = CheckNextItem(currentItem, isInFront, isInRight);

            if (itemConnections == null) return (false, (null, null));

            for (int i = 0; i < itemConnections.Count; i++)
            {
                var retval = FindLoop(isInRight,
                                    firstItem,
                                    itemConnections[i].item,
                                    itemConnections[i].isInRight,
                                    cameFrom,
                                    new List<DrawingWall>(visitedItems),
                                    new List<Vector3>(visitedPoints));

                if (retval.lists.items != null) return retval;
            }

            return (false, (null, null));
        }

        private static List<(DrawingWall, bool)> CheckNextItem(DrawingWall currentItem, bool inFront, bool inRight)
        {
            List<DrawingWall> curItemConnections = (inFront) ? currentItem.FrontConnections : currentItem.RearConnections;
            if (curItemConnections.Count == 0) return null;

            if (curItemConnections.Count == 1)
            {

                bool nextDirection = inFront;
                DrawingWall nextItem = curItemConnections[0];
                List<DrawingWall> nextItemConnections = (inFront) ? nextItem.FrontConnections : nextItem.RearConnections;
                if (nextItemConnections.Contains(currentItem)) nextDirection = (inFront) ? false : true;
                else nextDirection = (inFront) ? true : false;

                List<(DrawingWall, bool)> retval = new List<(DrawingWall, bool)>();
                retval.Add((nextItem, nextDirection));
                return retval;

            }
            else
            {

                List<KeyValuePair<float, (DrawingWall, bool)>> properSidePairs = new List<KeyValuePair<float, (DrawingWall, bool)>>();
                List<KeyValuePair<float, (DrawingWall, bool)>> opposSidePairs = new List<KeyValuePair<float, (DrawingWall, bool)>>();

                float rightAngle = (inRight) ? float.MaxValue : float.MinValue;
                float leftAngle = (inRight) ? float.MaxValue : float.MinValue;
                bool[] localInFront = new bool[curItemConnections.Count];

                for (int i = 0; i < curItemConnections.Count; i++)
                {
                    DrawingWall nextItem = curItemConnections[i];
                    bool nextDirection = inFront;

                    List<DrawingWall> nextDirConnections = (inFront) ? nextItem.FrontConnections : nextItem.RearConnections;
                    List<DrawingWall> nextOppConnections = (inFront) ? nextItem.RearConnections : nextItem.FrontConnections;

                    if (nextDirConnections.Contains(currentItem))
                    {
                        nextDirection = (inFront) ? false : true;
                    }
                    else
                    {
                        nextDirection = (inFront) ? true : false;
                    }

                    Vector3 mainDirPoint = (inFront) ? currentItem.FrontEndPoint : currentItem.RearEndPoint;
                    Vector3 a = mainDirPoint - currentItem.transform.position;
                    Vector3 b = mainDirPoint - nextItem.transform.position;
                    float angle = (float)System.Math.Round(Vector3.SignedAngle(b, a, Vector3.up), 3);

                    if (inRight)
                    {
                        if (angle > 0)
                        {
                            properSidePairs.Add(new KeyValuePair<float, (DrawingWall, bool)>(angle, (nextItem, nextDirection)));
                        }
                        else if (angle < 0)
                        {
                            opposSidePairs.Add(new KeyValuePair<float, (DrawingWall, bool)>(angle, (nextItem, nextDirection)));
                        }
                    }
                    else
                    {
                        if (angle < 0)
                        {
                            properSidePairs.Add(new KeyValuePair<float, (DrawingWall, bool)>(angle, (nextItem, nextDirection)));
                        }
                        else if (angle > 0)
                        {
                            opposSidePairs.Add(new KeyValuePair<float, (DrawingWall, bool)>(angle, (nextItem, nextDirection)));
                        }
                    }
                }

                properSidePairs = (inRight) ? properSidePairs.OrderBy(i => i.Key).ToList() : properSidePairs.OrderByDescending(i => i.Key).ToList();
                opposSidePairs = (inRight) ? opposSidePairs.OrderBy(i => i.Key).ToList() : opposSidePairs.OrderByDescending(i => i.Key).ToList();

                properSidePairs.AddRange(opposSidePairs);
                return properSidePairs.Select(i => i.Value).ToList();
            }
        }
    }
}
#endif

