#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MBS
{
    public class AreaManager
    {
        private const string INPUT_LIST_ITEMS_EMPTY = "MBS. Fatal Error. Area creation: input list of items is null or empty.";


        public MBSBuilder builder;
        private List<Area> areas;
        public int areaNameIterator;


        public AreaManager(MBSBuilder builder)
        {
            this.builder = builder;
            this.areas = new List<Area>();
        }

        public void AddNewArea(List<DrawingWall> items)
        {
            if (items == null || items.Count == 0)
            {
                Debug.LogError(INPUT_LIST_ITEMS_EMPTY);
                return;
            }

            if (IsThereEqualArea(items)) return;

            Area area = new Area(items, this);
            areas.Add(area);
            areaNameIterator++;
        }

        public void RemoveArea(Area area)
        {
            if (areas == null || areas.Count == 0)
            {
                return;
            }

            areas = areas.Where((source, index) => source != area).ToList();
        }

        public Area GetAreaAtPoint(Vector3 pointInBuilderSpace)
        {
            if (areas == null || areas.Count == 0)
            {
                return null;
            }

            Area area = null;
            float minArea = float.MaxValue;

            for (int i = 0; i < areas.Count; i++)
            {
                if (areas[i].IsPointInsideArea(pointInBuilderSpace))
                {
                    if (areas[i].area < minArea)
                    {
                        area = areas[i];
                        minArea = area.area;
                    }
                }
            }
            return area;
        }

        public List<Area> GetAreasAtPoint(Vector3 pointInBuilderSpace)
        {
            List<Area> _areas = new List<Area>(); ;
            if (this.areas == null || this.areas.Count == 0)
            {
                return _areas;
            }

            for (int i = 0; i < areas.Count; i++)
            {
                if (areas[i].IsPointInsideArea(pointInBuilderSpace))
                {
                    _areas.Add(areas[i]);
                }
            }
            return _areas;
        }

        public bool IsThereEqualArea(List<DrawingWall> items)
        {
            if (areas == null) return false;
            if (areas.Count == 0) return false;
            if (items == null) return false;
            if (items.Count == 0) return false;

            for (int i = 0; i < areas.Count; i++)
            {
                if (areas[i].Items.Count == 0) continue;

                int intersect = areas[i].Items.Intersect(items).ToList().Count;
                if (intersect == items.Count && intersect == areas[i].Items.Count)
                    return true;
            }

            return false;
        }

        public static bool IsTwoAreasEqual(Area area1, Area area2)
        {
            int intersectItems = area1.Items.Intersect(area2.Items).ToList().Count;
            if (intersectItems == area1.Items.Count && intersectItems == area2.Items.Count)
                return true;

            int intersectPoints = area1.GetExtremePoints().Intersect(area2.GetExtremePoints()).ToList().Count;
            if (intersectPoints == area1.GetExtremePoints().Length && intersectPoints == area2.GetExtremePoints().Length)
                return true;

            return false;
        }
    }
}
#endif
