using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class Programm
    {
        private const int ScreenWidth = 100;
        private const int ScreenHeight = 50;

        private const int MapWidth = 32;
        private const int MapHeight = 32;

        private const double Fov = Math.PI / 3;
        private const double Depth = 16;

        private static double _playerX = 5;
        private static double _playerY = 5;
        private static double _playerA = 0;

        private static string _map = "";

        private static readonly char[] Screen = new char[ScreenWidth * ScreenHeight];

        static void Main(string[] args)
        {
            Console.SetWindowSize(ScreenWidth, ScreenHeight);
            Console.SetBufferSize(ScreenWidth, ScreenHeight);
            Console.CursorVisible = false;

            _map += "#######################################";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#.....................................#";
            _map += "#######################################";

            DateTime dateTimeFrom = DateTime.Now; 


            while (true)
            {
                DateTime dateTimeTo = DateTime.Now;
                double elapsedTime = (dateTimeFrom - dateTimeTo).TotalSeconds;
                dateTimeFrom = DateTime.Now;

                if (Console.KeyAvailable)
                {
                    ConsoleKey consoleKey = Console.ReadKey(true).Key;

                    switch (consoleKey)
                    {
                        case ConsoleKey.A:
                            _playerA -= elapsedTime * 10;
                            break;
                        case ConsoleKey.D:
                            _playerA += elapsedTime * 10;
                            break;
                        case ConsoleKey.W: 
                        {
                             _playerX -= Math.Sin(_playerA) * 10 * elapsedTime;
                             _playerY -= Math.Cos(_playerA) * 10 * elapsedTime;

                             if (_map[(int) _playerY * MapWidth + (int) _playerX] == '#')
                             {
                                    _playerX += Math.Sin(_playerA) * 10 * elapsedTime;
                                    _playerY += Math.Cos(_playerA) * 10 * elapsedTime;  
                             }

                             break; 
                        }
                        case ConsoleKey.S:
                        {
                             _playerX += Math.Sin(_playerA) * 10 * elapsedTime;
                             _playerY += Math.Cos(_playerA) * 10 * elapsedTime;

                             if (_map[(int)_playerY * MapWidth + (int)_playerX] == '#')
                             {
                                    _playerX -= Math.Sin(_playerA) * 10 * elapsedTime;
                                    _playerY -= Math.Cos(_playerA) * 10 * elapsedTime;
                             }

                             break;
                        }
                    }
                }

                for (int x = 0; x < ScreenWidth; x++)
                {
                    double rayAngle = _playerA + Fov / 2 - x * Fov / ScreenWidth;

                    double rayX = Math.Sin(rayAngle);
                    double rayY = Math.Cos(rayAngle);

                    double distanceToWall = 0;
                    bool hitWall = false;
                    bool isBounds = false;

                    while (!hitWall && distanceToWall < Depth)
                    {
                        distanceToWall += 0.1;

                        int testX = (int)(_playerX + rayX * distanceToWall);
                        int testY = (int)(_playerY + rayY * distanceToWall);

                        if (testX < 0 || testX >= Depth + _playerX || testY < 0 || testY >= Depth + _playerY)
                        {
                            hitWall = true;
                            distanceToWall = Depth;
                        }
                        else
                        {
                            char testCell = _map[testY * MapWidth + testX];

                            if (testCell == '#')
                            {
                                hitWall = true;

                                var boundsVecorList = new List<(double module, double cos)>();

                                for (int tx = 0; tx < 2; tx++)
                                {
                                    for (int ty = 0; ty < 2; ty++)
                                    {
                                        double vx = testX + tx - _playerX;
                                        double vy = testY + ty - _playerY;

                                        double vectorModule = Math.Sqrt(vx * vy + vy * vy);
                                        double cosAngle = rayX * vx / vectorModule + rayY * vy / vectorModule;

                                        boundsVecorList.Add((vectorModule, cosAngle));

                                    }
                                }

                                boundsVecorList = boundsVecorList.OrderBy(v => v.module).ToList();

                                double boundAngle = 0.03 / distanceToWall;

                                if (Math.Acos(boundsVecorList[0].cos) < boundAngle || Math.Acos(boundsVecorList[1].cos) < boundAngle)
                                {
                                    isBounds = true; 
                                }
                            }
                        }
                    }

                    int celling = (int)(ScreenHeight / 2d - ScreenHeight * Fov / distanceToWall);
                    int floor = ScreenHeight - celling;

                    char wallShade;

                    if (isBounds)
                    {
                        wallShade = '|'; 
                    }

                    if(distanceToWall <= Depth / 4d)
                    {
                        wallShade = '\u2588'; 
                    }
                    else if(distanceToWall <= Depth / 3d)
                    {
                        wallShade = '\u2593'; 
                    }
                    else if(distanceToWall <= Depth / 2d)
                    {
                        wallShade = '\u2592'; 
                    }
                    else if(distanceToWall <= Depth)
                    {
                        wallShade = '\u2591'; 
                    }
                    else
                    {
                        wallShade = ' '; 
                    }

                    for (int y = 0; y < ScreenHeight; y++)
                    {
                        if (y <= celling)
                        {
                            Screen[y * ScreenWidth + x] = ' ';
                        }
                        else if (y > celling && y <= floor)
                        {
                            Screen[y * ScreenWidth + x] = wallShade;
                        }
                        else
                        {
                            char floorShade;

                            double b = 1 - (y - ScreenHeight / 2d) / (ScreenHeight - 2d);

                            if (b < 0.25)
                            {
                                floorShade = '#';
                            }
                            else if (b < 0.5)
                            {
                                floorShade = 'x';
                            }
                            else if (b < 0.75)
                            {
                                floorShade = '-';
                            }
                            else if (b < 0.9)
                            {
                                floorShade = '.';
                            }
                            else
                            {
                                floorShade = ' '; 
                            }

                            Screen[y * ScreenWidth + x] = floorShade;
                        }
                    }
                }

                //stats
                char[] stats = $"X : {_playerX}, Y : {_playerY}, FPS: {(int) (1/elapsedTime)}".ToCharArray();
                stats.CopyTo(Screen, 0);

                Console.SetCursorPosition(0, 0);
                Console.Write(Screen);
            }
        }
        //char c = ' ';

        //while (true)
        //{
        //    if (Console.KeyAvailable)
        //    {
        //        c = Console.ReadKey().KeyChar;                 
        //    }

        //    Array.Fill(Screen, c);
        //    Console.SetCursorPosition(0, 0);
        //    Console.WriteLine(Screen);
        //}
    }
}