using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding
{
    public static class PathFinder
    {
        public static List<Hex> FindPath_BFS(Hex start, Hex end)
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

                foreach (var neighbor in current.neighbors)
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

        public static List<Hex> FindPath_GreedyBestFirstSearch(Hex start, Hex end)
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

                foreach (var neighbor in current.neighbors)
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
        public static List<Hex> FindPath_Dijkstra(Hex start, Hex end, List<Hex> map)
        {

            foreach (Hex hex in map)
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

                foreach (var neighbor in current.neighbors)
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

        public static List<Hex> FindPath_AStar(Hex start, Hex end, List<Hex> map)
        {
            foreach (Hex hex in map)
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

                foreach (var neighbor in current.neighbors)
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

        public static List<Hex> BFS_HexesMoveRange(Hex start, float MoveRange, List<Hex> map)
        {
            List<Hex> tilesInRange = new List<Hex>();

            foreach (Hex hex in map)
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

                foreach (var neighbor in current.neighbors)
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

