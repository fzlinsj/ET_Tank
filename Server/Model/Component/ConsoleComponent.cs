using System;
using System.Threading;
using System.Threading.Tasks;

namespace ETModel
{
    [ObjectSystem]
    public class ConsoleComponentAwakeSystem : StartSystem<ConsoleComponent>
    {
        public override void Start(ConsoleComponent self)
        {
            self.Start().NoAwait();
        }
    }

    public static class ConsoleMode
    {
        public const string None = "";
        public const string Repl = "repl";
    }
    
    public class ConsoleComponent: Entity
    {
        public CancellationTokenSource CancellationTokenSource;
        public string Mode = "";

        public async ETVoid Start()
        {
            this.CancellationTokenSource = new CancellationTokenSource();
            
            while (true)
            {
                try
                {
                    string line = await Task.Factory.StartNew(() =>
                    {
                        Console.Write($"{this.Mode}> ");
                        return Console.In.ReadLine();
                    }, this.CancellationTokenSource.Token);
                    
                    line = line.Trim();

                    if (this.Mode != "")
                    {
                        bool isExited = true;
                        switch (this.Mode)
                        {
                            case ConsoleMode.Repl:
                            {
                                ReplComponent replComponent = this.GetComponent<ReplComponent>();
                                if (replComponent == null)
                                {
                                    Console.WriteLine($"no command: {line}!");
                                    break;
                                }
                            
                                try
                                {
                                    isExited = await replComponent.Run(line, this.CancellationTokenSource.Token);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }

                                break;
                            }
                        }

                        if (isExited)
                        {
                            this.Mode = "";
                        }

                        continue;
                    }

                    bool hasExp = false;

                    switch (line)
                    {
                        case "reload": 
                            try
                            {
                                Game.EventSystem.Add(DLLType.Hotfix, DllHelper.GetHotfixAssembly());
                            }
                            catch (Exception e)
                            {
                                hasExp = true;
                                Console.WriteLine(e);
                            }
                            break;
                        case "repl":
                            try
                            {
                                this.Mode = ConsoleMode.Repl;
                                this.AddComponent<ReplComponent>();
                            }
                            catch (Exception e)
                            {
                                hasExp = true;
                                Console.WriteLine(e);
                            }
                            break;
                        case "tankCount":
                            try
                            {
                                Log.Info($"坦克数量 = {Game.Scene.GetComponent<TankComponent>().Count}");
                                if (Game.Scene.GetComponent<TankComponent>().Count >= 1)
                                {
                                    long tankId =  Game.Scene.GetComponent<TankComponent>().GetAll()[0].Id;
                                    Log.Info($"第一辆坦克id = {tankId}");
                                }
                            }
                            catch (Exception e)
                            {
                                hasExp = true;
                                Console.WriteLine(e);
                            }

                            break;
                        case "ClearTank":
                            try
                            {
                                Game.Scene.GetComponent<TankComponent>().ClearTanks();
                            }
                            catch (Exception e)
                            {
                                hasExp = true;
                                Console.WriteLine(e);
                            }
                            break;
                        case "tank info":
                            try
                            {
                                Battle battle =  Game.Scene.GetComponent<BattleComponent>().GetAll()[0];

                                Tank[] tanks =  battle.GetAll();

                                foreach (Tank tank in tanks)
                                {
                                    Console.WriteLine($"{tank.Died}");
                                }
                            }
                            catch (Exception e)
                            {
                                hasExp = true;
                                Console.WriteLine(e);
                            }
                            break;
                        case "room num":
                            try
                            {
                                Console.WriteLine(Game.Scene.GetComponent<RoomComponent>().GetAll.Length);
                            }
                            catch (Exception e)
                            {
                                hasExp = true;
                                Console.WriteLine(e);
                                throw;
                            }
                            break;
                        case "battle num":
                            try
                            {
                                Console.WriteLine(Game.Scene.GetComponent<BattleComponent>().Count);
                            }
                            catch (Exception e)
                            {
                                hasExp = true;
                                Console.WriteLine(e);
                                throw;
                            }
                            break;
                        case "player num":
                            try
                            {
                                Console.WriteLine(Game.Scene.GetComponent<PlayerComponent>().Count);
                            }
                            catch (Exception e)
                            {
                                hasExp = true;
                                Console.WriteLine(e);
                                throw;
                            }
                            break;
                        default:
                            Console.WriteLine($"no such command: {line}");
                            break;
                    }

                    // if (!hasExp)
                    // {
                    //     Console.WriteLine("Finish");
                    // }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}