using System;
using System.Collections.Generic;
using System.Linq;

namespace lab04_Hrybniak
{
    public class Solution
    {

        /// <summary>
        /// Number of activities
        /// </summary>
        private static int na;

        private static void Main(string[] args)
        {
            do
            {
                Console.Write("This program example demonstrates the Critical Path Method's algorithm.\n");

                // Array to store the activities that'll be evaluated.
                Activity[] list = null;

                list = GetActivities(list);
                list = WalkListAhead(list);
                list = WalkListAback(list);

                var critical = CriticalPath(list);
                var transit = FindTransit(list, critical);
                Console.Write(" Do you wanna a new critical path solution? y\\n: ");
            }
            while (Console.ReadKey().KeyChar == 'y' || Console.ReadKey().KeyChar == 'Y');
        }

        private static Activity[] FindTransit(Activity[] allActivities, Activity[] criticalPathActivities)
        {
            var transit = new List<Activity>();
            foreach(var activity in allActivities)
            {
                for (int i = 0; i < activity.Successors.Length - 1; i++)
                {
                    var copy = new List<Activity>();
                    copy = allActivities.ToList();
                    var newActivity = MergeActivities(activity.Successors[i], activity.Successors[i + 1]);
                    var firstIndex = copy.FindIndex(p => p.Id == activity.Successors[i].Id);
                    var secondIndex = copy.FindIndex(p => p.Id == activity.Successors[i].Id);
                    copy.RemoveAt(firstIndex);
                    copy.RemoveAt(secondIndex);
                    copy.Insert(firstIndex, newActivity);
                    var copy1 = copy.ToArray();
                    copy1 = WalkListAhead(copy1);
                    copy1 = WalkListAback(copy1);

                    var critical = CriticalPath(copy1);
                    if (ArraysOfActivitiesHaveSameElements(critical, criticalPathActivities))
                    {
                        if (!transit.Any(a => a.Id == activity.Successors[i].Id))
                        {
                            transit.Add(activity.Successors[i]);
                            Console.WriteLine(activity.Successors[i].Id);
                        }
                        if (!transit.Any(a => a.Id == activity.Successors[i + 1].Id))
                        {
                            transit.Add(activity.Successors[i + 1]);
                            Console.WriteLine(activity.Successors[i + 1].Id);
                        }
                    }
                }
            }
            return transit.ToArray();
        }

        private static bool ArraysOfActivitiesHaveSameElements(Activity[] activities1, Activity[] activities2)
        {
            if (activities1.Length == activities2.Length)
            {
                for(int i = 0; i < activities1.Length; i++)
                {
                    if (activities1[i].Id != activities2[i].Id)
                    {
                        return false;
                    }
                }
                return true;
            }
            else return false;
        }

        private static Activity MergeActivities(Activity a, Activity b)
        {
            var activity = new Activity();
            activity.Id = a.Id;
            activity.Predecessors = a.Predecessors.Union(b.Predecessors).ToArray();
            activity.Successors = a.Successors.Union(b.Successors).ToArray();
            activity.Duration = a.Duration + b.Duration;
            return activity;
        }
        /// <summary>
        /// Gets the activities that'll be evaluated by the critical path method.
        /// </summary>
        /// <param name="list">Array to store the activities that'll be evaluated.</param>
        /// <returns>list</returns>
        private static Activity[] GetActivities(Activity[] list)
        {
            do
            {
                Console.Write("\n       Number of activities: ");
                if ((na = int.Parse(Console.ReadLine())) < 2)
                {
                    Console.Beep();
                    Console.Write("\n Invalid entry. The number must be >= 2.\n");
                }
            }
            while (na < 2);

            list = new Activity[na];

            for (int i = 0; i < na; i++)
            {
                Activity activity = new Activity();

                Console.Write("\n                Activity {0}\n", i + 1);

                Console.Write("\n                     ID: ", i + 1);
                activity.Id = Console.ReadLine();

                Console.Write("            Description: ", i + 1);
                activity.Description = Console.ReadLine();

                Console.Write("               Duration: ", i + 1);
                activity.Duration = int.Parse(Console.ReadLine());

                Console.Write(" Number of predecessors: ", i + 1);
                int np = int.Parse(Console.ReadLine());

                if (np != 0)
                {
                    activity.Predecessors = new Activity[np];

                    string id;

                    for (int j = 0; j < np; j++)
                    {
                        Console.Write("    #{0} predecessor's ID: ", j + 1);
                        id = Console.ReadLine();

                        Activity aux = new Activity();

                        if ((aux = aux.CheckActivity(list, id, i)) != null)
                        {
                            activity.Predecessors[j] = aux;

                            list[aux.GetIndex(list, aux, i)] = aux.SetSuccessors(aux, activity);
                        }
                        else
                        {
                            Console.Beep();
                            Console.Write("\n No match found! Try again.\n\n");
                            j--;
                        }
                    }
                }
                list[i] = activity;
            }

            return list;
        }

        /// <summary>
        /// Performs the walk ahead inside the array of activities calculating for each
        /// activity its earliest start time and earliest end time.
        /// </summary>
        /// <param name="list">Array storing the activities already entered.</param>
        /// <returns>list</returns>
        private static Activity[] WalkListAhead(Activity[] list)
        {
            list[0].Eet = list[0].Est + list[0].Duration;

            for (int i = 1; i < list.Length; i++)
            {
                foreach (Activity activity in list[i].Predecessors)
                {
                    if (list[i].Est < activity.Eet)
                        list[i].Est = activity.Eet;
                }

                list[i].Eet = list[i].Est + list[i].Duration;
            }

            return list;
        }

        /// <summary>
        /// Performs the walk aback inside the array of activities calculating for each
        /// activity its latest start time and latest end time.
        /// </summary>
        /// <param name="list">Array storing the activities already entered.</param>
        /// <returns>list</returns>
        private static Activity[] WalkListAback(Activity[] list)
        {
            list[list.Length - 1].Let = list[list.Length - 1].Eet;
            list[list.Length - 1].Lst = list[list.Length - 1].Let - list[list.Length - 1].Duration;

            for (int i = list.Length - 2; i >= 0; i--)
            {
                foreach (Activity activity in list[i].Successors)
                {
                    if (list[i].Let == 0)
                        list[i].Let = activity.Lst;
                    else
                      if (list[i].Let > activity.Lst)
                        list[i].Let = activity.Lst;
                }

                list[i].Lst = list[i].Let - list[i].Duration;
            }

            return list;
        }

        /// <summary>
        /// Calculates the critical path by verifyng if each activity's earliest end time
        /// minus the latest end time and earliest start time minus the latest start
        /// time are equal zero. If so, then prints out the activity id that match the
        /// criteria. Plus, prints out the project's total duration. 
        /// </summary>
        /// <param name="list">Array containg the activities already entered.</param>
        private static Activity[] CriticalPath(Activity[] list)
        {
            var criticalPath = new List<Activity>();
            Console.Write("\n          Critical Path: ");

            foreach (Activity activity in list)
            {
                if ((activity.Eet - activity.Let == 0) && (activity.Est - activity.Lst == 0))
                {
                    criticalPath.Add(activity);
                    Console.Write("{0} ", activity.Id);
                }
            }

            Console.Write("\n\n         Total duration: {0}\n\n", list[list.Length - 1].Eet);
            return criticalPath.ToArray();
        }
    }
}
