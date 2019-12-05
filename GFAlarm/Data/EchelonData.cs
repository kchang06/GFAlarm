using GFAlarm.Data.Packet.Index;
using GFAlarm.View.DataTemplate;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFAlarm.Data
{
    public class EchelonData
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static Dictionary<int, ObservableCollection<EchelonTemplate>> echelons { get; set; } = new Dictionary<int, ObservableCollection<EchelonTemplate>>()
        {
            { 1, new ObservableCollection<EchelonTemplate>() },
            { 2, new ObservableCollection<EchelonTemplate>() },
            { 3, new ObservableCollection<EchelonTemplate>() },
            { 4, new ObservableCollection<EchelonTemplate>() },
            { 5, new ObservableCollection<EchelonTemplate>() },
            { 6, new ObservableCollection<EchelonTemplate>() },
            { 7, new ObservableCollection<EchelonTemplate>() },
            { 8, new ObservableCollection<EchelonTemplate>() },
            { 9, new ObservableCollection<EchelonTemplate>() },
            { 10, new ObservableCollection<EchelonTemplate>() },
        };

        /// <summary>
        /// 추가
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="data"></param>
        public static void Add(EchelonTemplate data)
        {
            if (data.teamId != 0 && data.location != 0)
            {
                int teamId = data.teamId;
                int location = data.location;
                Remove(teamId, location);

                echelons[teamId].Add(data);

                Sort(teamId);
            }
        }

        public static void Remove(EchelonTemplate data)
        {
            Remove(data.teamId, data.location);
        }
        public static void Remove(int teamId, int location)
        {
            for (int i = 0; i < echelons[teamId].Count(); i++)
            {
                if (location != 0 && echelons[teamId][i].location == location)
                {
                    echelons[teamId].RemoveAt(i);
                    break;
                }
            }
            Sort(teamId);
        }

        /// <summary>
        /// 초기화
        /// </summary>
        public static void Clear()
        {
            for (int i = 1; i <= 10; i++)
            {
                foreach (dynamic item in echelons[i].ToList())
                {
                    Remove(item);
                }
            }
        }

        /// <summary>
        /// 정렬
        /// </summary>
        /// <param name="teamId"></param>
        public static void Sort(int teamId)
        {
            var tempData = echelons[teamId].OrderBy(o => o.location).ToList();
            foreach (var temp in tempData)
            {
                int oldIdx = echelons[teamId].IndexOf(temp);
                int newIdx = tempData.IndexOf(temp);
                echelons[teamId].Move(oldIdx, newIdx);
            }
        }
    }
}
