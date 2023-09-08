using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding
{
    public static class PathFinder
    {
        public static List<Hex> FindPath_BFS(Hex start, Hex end, Map map)
        {

            HashSet<Hex> visited = new HashSet<Hex>();
            visited.Add(start);

            Queue<Hex> frontier = new Queue<Hex>();
            frontier.Enqueue(start);

            start.path_data.prev_hex = null;

            while (frontier.Count > 0)
            {
                Hex current = frontier.Dequeue();

                if (current == end)
                {
                    break;
                }

                foreach (var neighbor in current.GetNeighbors(map))
                {
                    if (neighbor.IsWalkable())
                    {
                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            frontier.Enqueue(neighbor);

                            neighbor.path_data.prev_hex = current;
                        }
                    }
                }
            }

            List<Hex> path = BacktrackToPath(end);

            return path;
        }

        public static List<Hex> FindPath_GreedyBestFirstSearch(Hex start, Hex end, Map map)
        {

            Comparison<Hex> heuristicComparison = (lhs, rhs) =>
            {
                float lhsCost = GetEuclideanHeuristicCost(lhs, end);
                float rhsCost = GetEuclideanHeuristicCost(rhs, end);

                return lhsCost.CompareTo(rhsCost);
            };

            MinHeap<Hex> frontier = new MinHeap<Hex>(heuristicComparison);
            frontier.Add(start);

            HashSet<Hex> visited = new HashSet<Hex>();
            visited.Add(start);

            start.path_data.prev_hex = null;

            while (frontier.Count > 0)
            {
                Hex current = frontier.Remove();

                if (current == end)
                {
                    break;
                }

                foreach (var neighbor in current.GetNeighbors(map))
                {
                    if (neighbor.IsWalkable())
                    {
                        if (!visited.Contains(neighbor))
                        {
                            frontier.Add(neighbor);
                            visited.Add(neighbor);

                            neighbor.path_data.prev_hex = current;
                        }
                    }
                }
            }

            List<Hex> path = BacktrackToPath(end);

            return path;
        }
        public static List<Hex> FindPath_Dijkstra(Hex start, Hex end, Map map)
        {

            foreach (Hex hex in map.hexes)
            {
                hex.path_data.cost = int.MaxValue;
            }

            start.path_data.cost = 0;

            HashSet<Hex> visited = new HashSet<Hex>();
            visited.Add(start);

            MinHeap<Hex> frontier = new MinHeap<Hex>((lhs, rhs) => lhs.path_data.cost.CompareTo(rhs.path_data.cost));
            frontier.Add(start);

            start.path_data.prev_hex = null;

            while (frontier.Count > 0)
            {
                Hex current = frontier.Remove();

                if (current == end)
                {
                    break;
                }

                foreach (var neighbor in current.GetNeighbors(map))
                {
                    if (neighbor.IsWalkable())
                    {
                        int newNeighborCost = current.path_data.cost + neighbor.path_data.weight;
                        if (newNeighborCost < neighbor.path_data.cost)
                        {
                            neighbor.path_data.cost = newNeighborCost;
                            neighbor.path_data.prev_hex = current;
                        }

                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            frontier.Add(neighbor);
                        }
                    }

                }
            }

            List<Hex> path = BacktrackToPath(end);

            return path;
        }

        public static List<Hex> FindPath_AStar(Hex start, Hex end, Map map)
        {
            foreach (Hex hex in map.hexes)
            {
                hex.path_data.cost = int.MaxValue;
            }

            start.path_data.cost = 0;
            Comparison<Hex> heuristicComparison = (lhs, rhs) =>
            {
                float lhsCost = lhs.path_data.cost + GetEuclideanHeuristicCost(lhs, end);
                float rhsCost = rhs.path_data.cost + GetEuclideanHeuristicCost(rhs, end);

                return lhsCost.CompareTo(rhsCost);
            };

            MinHeap<Hex> frontier = new MinHeap<Hex>(heuristicComparison);
            frontier.Add(start);

            HashSet<Hex> visited = new HashSet<Hex>();
            visited.Add(start);

            start.path_data.prev_hex = null;

            while (frontier.Count > 0)
            {
                Hex current = frontier.Remove();

                if (current == end)
                {
                    break;
                }

                foreach (var neighbor in current.GetNeighbors(map))
                {
                    if (neighbor.IsWalkable())
                    {
                        int newNeighborCost = current.path_data.cost + neighbor.path_data.weight;
                        if (newNeighborCost < neighbor.path_data.cost)
                        {
                            neighbor.path_data.cost = newNeighborCost;
                            neighbor.path_data.prev_hex = current;
                        }

                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            frontier.Add(neighbor);
                        }
                    }
                }
            }

            List<Hex> path = BacktrackToPath(end);

            return path;
        }

        public static List<Hex> BFS_HexesMoveRange(Hex start, float MoveRange, Map map)
        {
            List<Hex> tilesInRange = new List<Hex>();

            foreach (Hex hex in map.hexes)
            {
                hex.path_data.cost = int.MaxValue;
            }

            start.path_data.cost = 0;

            HashSet<Hex> visited = new HashSet<Hex>();
            visited.Add(start);

            Queue<Hex> frontier = new Queue<Hex>();
            frontier.Enqueue(start);

            start.path_data.prev_hex = null;

            while (frontier.Count > 0)
            {
                Hex current = frontier.Dequeue();

                foreach (var neighbor in current.GetNeighbors(map))
                {
                    if (neighbor.IsWalkable())
                    {
                        int newNeighborCost = current.path_data.cost + neighbor.path_data.weight;

                        if (newNeighborCost < neighbor.path_data.cost)
                        {
                            neighbor.path_data.cost = newNeighborCost;
                            neighbor.path_data.prev_hex = current;
                        }


                        if (!visited.Contains(neighbor))
                        {
                            if (MoveRange - newNeighborCost >= 0)
                            {
                                tilesInRange.Add(neighbor);
                            }
                            else
                            {
                                return tilesInRange;
                            }
                            visited.Add(neighbor);
                            frontier.Enqueue(neighbor);
                        }
                    }
                }
            }
            return tilesInRange;
        }
        public static List<Hex> BFS_LongestEnemyUnitPath(Hex start, Unit cast_unit, Map map, int _max)
        {
            List<Hex> longestPath = new List<Hex>();

            foreach (Hex tile in map.hexes)
            {
                tile.path_data.cost = int.MaxValue;
            }

            start.path_data.cost = 0;

            HashSet<Hex> visited = new HashSet<Hex>();
            visited.Add(start);

            Queue<Hex> frontier = new Queue<Hex>();
            frontier.Enqueue(start);

            start.path_data.prev_hex = null;

            while (frontier.Count > 0)
            {
                Hex current = frontier.Dequeue();

                foreach (var neighbor in current.GetNeighbors(map))
                {
                    Unit unit = neighbor.GetUnit();
                    if (unit != null && unit.class_type != cast_unit.class_type)
                    {
                        int newNeighborCost = current.path_data.cost + neighbor.path_data.weight;
                        if (newNeighborCost < neighbor.path_data.cost)
                        {
                            neighbor.path_data.cost = newNeighborCost;
                            neighbor.path_data.prev_hex = current;
                        }

                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            frontier.Enqueue(neighbor);
                        }

                    }

                }
            }
            Dictionary<Hex, int> pathdicitonary = new Dictionary<Hex, int>();

            foreach (Hex h in visited)
            {
                List<Hex> p = BacktrackToPath(h);
                pathdicitonary.Add(h, p.Count);
            }

            int max = pathdicitonary.Values.Max();

            pathdicitonary = pathdicitonary.Where(kvp => max >= _max ? kvp.Value >= _max : kvp.Value == max)
                              .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            List<Hex> longest_path_hexes = new List<Hex>();
            foreach (var p in pathdicitonary)
                    longest_path_hexes.Add(p.Key);
            if (longest_path_hexes.Count > 0)
            {
                int rand = (int)MathF.Floor(NetworkManager.Instance.games[cast_unit.match_id].random_seeds_generator.GetRandomPercentSeed() * longest_path_hexes.Count);
                Hex choosen_one = longest_path_hexes[rand];
                longestPath = BacktrackToPath(choosen_one);
            }

            return longestPath;
        }
        private static float GetEuclideanHeuristicCost(Hex current, Hex end)
        {
            float heuristicCost = (current.game_object.transform.position - end.game_object.transform.position).magnitude;
            return heuristicCost;
        }

        private static List<Hex> BacktrackToPath(Hex end)
        {
            Hex current = end;
            List<Hex> path = new List<Hex>();

            while (current != null)
            {
                path.Add(current);
                current = current.path_data.prev_hex;
            }

            path.Reverse();

            return path;
        }
    }
}

