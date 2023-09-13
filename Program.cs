using RacingApp2.Classes;

namespace RacingApp2
{
    internal class Program
    {
        static double simTime = 0.1;
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to the 2023 Stockholm F1 Grand Prix!");
            Console.WriteLine("Press any key to start the race");
            Console.ReadLine();

            await RacingGame();

            Console.WriteLine("The Grand prix has been concluded! Press any key to close:");
            Console.ReadLine();
        }

        public static async Task RacingGame()
        {
            Car redBull = new Car("Red Bull Racing");
            Car mercedes = new Car("Mercedes AMG");
            Car macLaren = new Car("MacLaren Racing");

            //Race is initiated.
            var redbullTask = RaceApp(redBull);
            var mercedesTask = RaceApp(mercedes);
            var macLarenTask = RaceApp(macLaren);
            var raceStatus = CarStatus(new List<Car> { redBull, mercedes, macLaren });
            var raceTasks = new List<Task> { redbullTask, mercedesTask, macLarenTask, raceStatus };

            //Tracking if one of the car has finished to determine winner.
            bool winner = false;

            while (raceTasks.Count > 0)
            {
                Task raceWinner = await Task.WhenAny(raceTasks);

                if (raceWinner == redbullTask)
                {
                    var rbTime = convertSecToMin(redBull.RaceTime);

                    if (winner == false)
                    {
                        winner = true;
                        Console.WriteLine($"{redBull.Name} has won it with a final time of {rbTime.min}:{rbTime.sec}!");
                    }

                    else Console.WriteLine($"{redBull.Name} has finished with a final time of {rbTime.min}:{rbTime.sec}!");
                }

                else if (raceWinner == mercedesTask)
                {
                    var mercedesTime = convertSecToMin(mercedes.RaceTime);

                    if (winner == false)
                    {
                        winner = true;
                        Console.WriteLine($"{mercedes.Name} has won it with a final time of {mercedesTime.min}:{mercedesTime.sec}!");
                    }

                    else Console.WriteLine($"{mercedes.Name} has finished with a final time of {mercedesTime.min}:{mercedesTime.sec}!");
                }

                else if (raceWinner == macLarenTask)
                {
                    var mclTime = convertSecToMin(macLaren.RaceTime);

                    if (winner == false)
                    {
                        winner = true;
                        Console.WriteLine($"{macLaren.Name} has won it with a final time of {mclTime.min}:{mclTime.sec}!");
                    }

                    else Console.WriteLine($"{macLaren.Name} has finished with a final time of {mclTime.min}:{mclTime.sec}!");
                }

                else if (raceWinner == raceStatus)
                {
                    Console.WriteLine($"All cars finished!");
                }

                await raceWinner;
                raceTasks.Remove(raceWinner);
            }

        }

        public async static Task<Car> RaceApp(Car car)
        {
            int incidentCount = 0;
            //Marks the beginning of race for each car.
            Console.WriteLine($"{car.Name} takes off!");

            while (true)
            {
                //Results in the sim time of race being 10x faster.
                await Task.Delay(100);

                //Counter for generating events every 30 seconds (3 seconds of sim time)
                incidentCount++;

                //Counter for each cars race time
                car.RaceTime++;

                if (car.DistanceDriven >= car.RaceDistance)
                {
                    return car;
                }

                car.DistanceDriven += (car.Speed / 3.6);

                if (incidentCount == 30)
                {
                    incidentCount = 0;

                    int incidentResult = IncidentHandler(car);

                    switch (incidentResult)
                    {
                        case 1:
                            await timeDelay(simTime * 30);
                            car.RaceTime += 30;
                            break;
                        case 2:
                            await timeDelay(simTime * 20);
                            car.RaceTime += 20;
                            break;
                        case 3:
                            await timeDelay(simTime * 10);
                            car.RaceTime += 10;
                            break;
                        case 4:
                            car.Speed -= 1;
                            break;
                        default:
                            break;
                    }


                }
            }

        }


        public async static Task CarStatus(List<Car> cars)
        {

            Console.WriteLine("Press any key to see car/race info");
            while (true)
            {
                bool gotkey = true;
                DateTime start = DateTime.Now;

                while ((DateTime.Now - start).TotalSeconds < 1)
                {
                    if (Console.KeyAvailable)
                    {
                        gotkey = true;
                        break;
                    }
                }
                if (gotkey)
                {
                    Console.ReadKey(true);
                    foreach (Car car in cars)
                    {
                        double dist = Math.Round(car.DistanceDriven, 2);
                        string time = TimeSpan.FromSeconds(car.RaceTime).ToString("c");
                        Console.WriteLine($"\nStatus for: {car.Name}\n" +
                            $"race time: {time}\n" +
                            $"Speed: {car.Speed} km/h ({Math.Round(car.Speed / 3.6m, 2)} m/s)\n" +
                            $"Distance driven: {dist} meters\n" +
                            $"Distance left: {Math.Round(car.RaceDistance - car.DistanceDriven, 2)} meters\n" +
                            $"Last Incident: {car.IncidentReport}\n");

                    }
                    Console.WriteLine();
                    gotkey = false;
                }

                await timeDelay(simTime);

                var remainingTime = cars.Select(car => car.RaceTime).Sum();
                if (remainingTime >= 0)
                {
                    return;
                }
            }
        }

        //Generates a number, if in given range which corresponds to 1/50 (fuel), 2/50(tire), 5/50(bird on windshield)
        //or 10/50 (engine failure). Event is triggered.
        static int IncidentHandler(Car car)
        {
            int result = 0;

            Random random = new Random();
            int incidentGenerator = random.Next(1, 51);

            switch (incidentGenerator)
            {
                case 1:
                    var time = convertSecToMin(car.RaceTime);
                    Incidents emptyFuel = new Incidents("Out of Fuel", $"{car.Name} has ran out of fuel at {time.min}:{time.sec} min, need to make a 10 sec pitstop!", 10, 0);
                    Console.WriteLine($"{car.Name} {emptyFuel.Description}");
                    car.IncidentReport = emptyFuel.Name;
                    result = 1;
                    break;

                case int n when (n >= 2 && n <= 3):
                    var time1 = convertSecToMin(car.RaceTime);
                    Incidents Puncture = new Incidents("Punctured Tire", $"{car.Name} has a punctured tire at {time1.min}:{time1.sec} min, need to make a 20 sec pitstop!", 20, 0);
                    Console.WriteLine($"{car.Name} {Puncture.Description}");
                    car.IncidentReport = Puncture.Name;
                    result = 2;
                    break;

                case int n when (n >= 4 && n <= 8):
                    var time2 = convertSecToMin(car.RaceTime);
                    Incidents Bird = new Incidents("Bird on windshield", $"How unlucky! {car.Name} has a bird on the windshield at {time2.min}:{time2.sec} min, need to make a 10 sec pitstop!", 10, 0);
                    Console.WriteLine($"{car.Name} {Bird.Description}");
                    car.IncidentReport = Bird.Name;
                    result = 3;
                    break;

                case int n when (n >= 9 && n <= 18):
                    var time3 = convertSecToMin(car.RaceTime);
                    Incidents Engine = new Incidents("Engine failure", $"Seems to be engine issues for {car.Name} at {time3.min}:{time3.sec} min. The car speed is down by 1km/h!", 0, 1);
                    Console.WriteLine($"{car.Name} {Engine.Description}");
                    car.IncidentReport = Engine.Name;
                    result = 4;
                    break;
                default:
                    break;
            }

            return result;
        }

        //Stops and delays the time following incident.
        static async Task timeDelay(double tick)
        {
            await Task.Delay(TimeSpan.FromSeconds(tick));
        }

        //Converts time so that full min and sec are shown.
        public static (double min, double sec) convertSecToMin(double seconds)
        {

            var timeSpan = TimeSpan.FromSeconds(seconds);
            double minutes = timeSpan.Minutes;
            double secs = timeSpan.Seconds;

            return (minutes, secs);
        }
    }
}