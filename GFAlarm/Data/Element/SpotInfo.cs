using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class SPOTTYPE
{
    public static int COMMAND = 1;                // 1: 지휘부
    public static int NORMAL = 2;                 // 2: 일반거점
    public static int HELIPORT = 3;               // 3: 헬리포트
    public static int RADAR = 4;                  // 4: 레이더
    public static int NORMAL_WITH_SUPPLY = 5;     // 5: 일반거점 (보급품)
    public static int SUPPLY_LINE = 6;            // 6: 보급거점
    public static int HEAVY_HELIPORT = 7;         // 7: 대형헬리포트
}

/// <summary>
/// 거점 정보
/// </summary>
public class SpotInfo
{
    public int id = 0;
    public int mission_id = -1;                      // 전역 번호
    public int[] coord = new int[] { 0, 0 };         // 좌표 (x, y)
    public int belong = -1;                          // 점령 (1: 그리폰)
    public int type = -1;                            // 종류 (1: 지휘부, 2: 일반거점, 3: 헬리포트, 4:???, 5: 보급품, 6: 보급거점, 7: 대형헬리포트)
    public int[] map_route = new int[] { };          // 길 (이어진 거점)
    public int[] active_cycle = new int[] { };       // 헬리포트 열림 간격 (N턴 후, N턴 간 열림)

    public int team_id = -1;                         // 제대
    public int squad_id = -1;                        // 중장비 제대
    public int friend_team_id = -1;                  // 친구 제대
    public int enemy_team_id = -1;                   // 적 제대
    public int hostage_id = -1;                      // 인질 제대
    public int building_id = -1;                     // 건물

    /// <summary>
    /// 리셋
    /// </summary>
    public void Reset()
    {
        this.id = 0;
        this.coord = new int[] { 0, 0 };
        this.belong = -1;
        this.type = -1;
        this.team_id = -1;
        this.squad_id = -1;
        this.friend_team_id = -1;
        this.enemy_team_id = -1;
        this.hostage_id = -1;
        this.building_id = -1;
    }
}
