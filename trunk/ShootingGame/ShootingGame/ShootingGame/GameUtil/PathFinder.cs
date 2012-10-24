using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ShootingGame.GameUtil
{
    class PathFinder
    {
        #region Sub-classes

        public class Tile
        {
            public bool Walkable = true;
        }
        public class Node
        {
            public Node(Node parentNode, int newX, int newY, int newWalkedCost, int goalX, int goalY)
            {
                ParentNode = parentNode;
                x = newX;
                y = newY;
                walkedCost = newWalkedCost;
                int xDist = Math.Abs(goalX - x), yDist = Math.Abs(goalY - y);
                if (xDist > yDist)
                    estimateCostLeft = 14 * yDist + 10 * (xDist - yDist);
                else
                    estimateCostLeft = 14 * xDist + 10 * (yDist - xDist);
                totalCost = walkedCost + estimateCostLeft;
            }

            public Node ParentNode;

            public int X { get { return x; } }
            public int Y { get { return y; } }
            public int WalkedCost { get { return walkedCost; } set { walkedCost = value; totalCost = walkedCost + estimateCostLeft; } }
            public int EstimateCostLeft { get { return estimateCostLeft; } }
            public int TotalCost { get { return totalCost; } }

            private int x, y, walkedCost, estimateCostLeft, totalCost;
        }

        public void SetUpMap(int[,] map)
        {
            Map = new Tile[map.GetLength(0), map.GetLength(1)];
            for (int x = 0; x < Map.GetLength(0); x++)
            {
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    Map[x, y] = new PathFinder.Tile();
                    Map[x, y].Walkable = (map[x, y] == 0); ;
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the map used for path finding.
        /// </summary>
        public Tile[,] Map;

        /// <summary>
        /// Gets the shortest possible path found (null if no path was found).
        /// If giveUpCost is 0 any path may be explored, otherwize, if path is
        /// estimated to be longer than giveUpCost the search will end.
        /// </summary>
        public LinkedList<Vector2> FindPath(Vector3 start, Vector3 end)
        {
            int giveUpCost = 0;
            int startX = (int)start.X;
            int startY = (int)start.Z;
            int goalX = (int)end.X;
            int goalY = (int)end.Z;

            if (Map == null)
                return null;

            Node[] open = new Node[Map.Length];
            open[0] = new Node(null, startX, startY, 0, goalX, goalY);
            int openSize = 1;
            LinkedList<Node> closed = new LinkedList<Node>();

            while (openSize != 0)
            {
                // Current node (least TotalCost) is sorted to be the first
                Node currentNode = open[0];

                // Switch current node to closed list
                BinaryHeapRemoveFirst(open, ref openSize);
                closed.AddLast(currentNode);

                // Goal Reached?
                if (currentNode.X == goalX && currentNode.Y == goalY)
                {
                    LinkedList<Vector2> path = new LinkedList<Vector2>();
                    for (Node n = currentNode; n != null; n = n.ParentNode)
                        path.AddFirst(new Vector2(n.X, n.Y));
                    return path;
                }

                // Give up path?
                if (giveUpCost != 0 && currentNode.TotalCost > giveUpCost)
                    break;

                // Add neighbour tiles to open list...
                bool leftNotBlocked = AddToOpen(currentNode, currentNode.X - 1, currentNode.Y, currentNode.WalkedCost + 10, goalX, goalY, open, ref openSize, closed); // Left
                bool rightNotBlocked = AddToOpen(currentNode, currentNode.X + 1, currentNode.Y, currentNode.WalkedCost + 10, goalX, goalY, open, ref openSize, closed); // Right
                bool upNotBlocked = AddToOpen(currentNode, currentNode.X, currentNode.Y - 1, currentNode.WalkedCost + 10, goalX, goalY, open, ref openSize, closed); // Up
                bool downNotBlocked = AddToOpen(currentNode, currentNode.X, currentNode.Y + 1, currentNode.WalkedCost + 10, goalX, goalY, open, ref openSize, closed); // Down
                // Diagonals...
                if (leftNotBlocked && upNotBlocked)
                    AddToOpen(currentNode, currentNode.X - 1, currentNode.Y - 1, currentNode.WalkedCost + 14, goalX, goalY, open, ref openSize, closed);    // Upper Left
                if (rightNotBlocked && upNotBlocked)
                    AddToOpen(currentNode, currentNode.X + 1, currentNode.Y - 1, currentNode.WalkedCost + 14, goalX, goalY, open, ref openSize, closed);    // Upper Right
                if (leftNotBlocked && downNotBlocked)
                    AddToOpen(currentNode, currentNode.X - 1, currentNode.Y + 1, currentNode.WalkedCost + 14, goalX, goalY, open, ref openSize, closed);    // Bottom Left
                if (rightNotBlocked && downNotBlocked)
                    AddToOpen(currentNode, currentNode.X + 1, currentNode.Y + 1, currentNode.WalkedCost + 14, goalX, goalY, open, ref openSize, closed);    // Bottom Right
            }



            return null;
        }

        #region Private functions used by 'this.FindPath'

        /// <summary>
        /// Adds the tile to the open list if walkable and not on the closed list.
        /// If it's already on the open list the shortest path to the tile is used.
        /// Also, the function returns weather or not this square is walkable.
        /// </summary>
        private bool AddToOpen(Node parentNode, int x, int y, int walkedCost, int goalX, int goalY, Node[] open, ref int openSize, LinkedList<Node> closed)
        {
            // Cancel if tile is not walkable
            if (x < 0 || x >= Map.GetLength(0) || y < 0 || y >= Map.GetLength(1) || Map[x, y] == null || !Map[x, y].Walkable)
                return false;
            // Cancel if tile is on the closed list
            bool onClosed = false;
            for (LinkedListNode<Node> closedNode = closed.First; closedNode != null; closedNode = closedNode.Next)
            {
                if (closedNode.Value.X == x && closedNode.Value.Y == y)
                {
                    onClosed = true;
                    break;
                }
            }
            if (onClosed)
                return true;

            // Check if tile is already on the open list
            bool onOpen = false;
            for (int i = 0; i < openSize; i++)
            {
                if (open[i].X == x && open[i].Y == y)
                {
                    onOpen = true;
                    // Replace path if better
                    if (walkedCost < open[i].WalkedCost)
                    {
                        open[i].ParentNode = parentNode;
                        open[i].WalkedCost = walkedCost;

                        // Resort heap (according to a lower TotalCost - not a general resort method)
                        int pos = i;
                        while (pos != 0)
                        {
                            int parentPos = (pos + 1) / 2 - 1;
                            if (open[parentPos].TotalCost >= open[pos].TotalCost)
                            {
                                Node swap = open[parentPos];
                                open[parentPos] = open[pos];
                                open[pos] = swap;
                                pos = parentPos;
                            }
                            else
                                break;
                        }
                    }
                    break;
                }
            }
            // If not on open list: add it
            if (!onOpen)
                BinaryHeapAdd(open, ref openSize, new Node(parentNode, x, y, walkedCost, goalX, goalY));
            return true;
        }

        /// <summary>
        /// Binary Heap adding function of nodes.
        /// </summary>
        private void BinaryHeapAdd(Node[] list, ref int size, Node item)
        {
            list[size] = item;
            int pos = size;
            size++;
            while (pos != 0)
            {
                int parentPos = (pos + 1) / 2 - 1;
                if (list[parentPos].TotalCost >= list[pos].TotalCost)
                {
                    Node swap = list[parentPos];
                    list[parentPos] = list[pos];
                    list[pos] = swap;
                    pos = parentPos;
                }
                else
                    break;
            }
        }

        /// <summary>
        /// Binary Heap removing function of nodes.
        /// </summary>
        private void BinaryHeapRemoveFirst(Node[] list, ref int size)
        {
            if (size == 0)
                return;
            size--;
            list[0] = list[size];
            list[size] = null;
            int pos = 0;
            while (true)
            {
                int childPos = (pos + 1) * 2 - 1;
                if (childPos >= size)
                    break;
                if (list[pos].TotalCost > list[childPos].TotalCost || ((childPos + 1 >= size) || list[pos].TotalCost > list[childPos + 1].TotalCost))
                {
                    int minChildPos = ((childPos + 1 >= size) || list[childPos].TotalCost <= list[childPos + 1].TotalCost) ? childPos : childPos + 1;
                    Node swap = list[minChildPos];
                    list[minChildPos] = list[pos];
                    list[pos] = swap;
                    pos = minChildPos;
                }
                else
                    break;
            }
        }

        #endregion
    }
}
