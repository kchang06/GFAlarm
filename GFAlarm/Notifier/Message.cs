using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFAlarm.Notifier
{
    /// <summary>
    /// 메시지 종류
    /// </summary>
    public enum MessageType
    {
        start_operation,          // 군수지원 시작
        start_auto_mission,       // 자율작전 시작

        complete_operation,       // 군수지원 완료
        complete_auto_mission,    // 자율작전 완료
        complete_produce_doll,    // 인형제조 완료
        complete_produce_equip,   // 장비제조 완료
        complete_skill_train,     // 스킬훈련 완료
        complete_exp_train,       // 경험훈련 완료
        complete_restore_doll,    // 인형수복 완료
        complete_data_analysis,   // 정보분석 완료
        complete_explore,         // 탐색현황 완료
        complete_battle_report,   // 작전보고서 완료

        reach_max_global_exp,     // 자유경험치
        reach_max_bp_point,       // 모의작전
        reach_max_doll,           // 인형창고 상한
        reach_max_equip,          // 장비창고 상한

        mission_win,              // 전역승리
        mission_move_finish,      // 제대이동
        mission_get_doll,         // 인형획득
        mission_get_equip,        // 장비획득

        doll_need_expand,         // 편제확대 필요
        doll_wounded,             // 중상
        doll_max_level,           // 최대 레벨

        /// <summary>
        /// 일간임무
        /// </summary>
        acheive_daily_combat_sim,       // 모의작전 임무 달성
        acheive_daily_data_analysis,            // 정보분석 임무 달성
        acheive_daily_eat_doll,                 // 인형강화 임무 달성
        acheive_daily_eat_equip,                // 장비강화 임무 달성
        acheive_daily_fix_doll,                 // 인형수복 임무 달성
        acheive_daily_get_battery,              // 공유전지 획득 임무 달성
        acheive_daily_operation,                // 군수지원 임무 달성
        acheive_daily_produce_doll,             // 인형제조 임무 달성
        acheive_daily_produce_equip,            // 장비제조 임무 달성
        acheive_daily_reinforce,                // 지원제대 임무 달성
        acheive_daily_win,                      // 전역승리 임무 달성

        /// <summary>
        /// 주간임무
        /// </summary>
        acheive_weekly_s_battle,                // S랭크 전투 주간임무 달성

        acheive_weekly_fix_gun,                 // 인형수복 주간임무 달성 
        acheive_weekly_kill_mech,               // 철혈기계 처치 주간임무 달성
        acheive_weekly_kill_doll,               // 철혈인형 처치 주간임무 달성
        acheive_weekly_kill_boss,               // 철혈보스 처치 주간임무 달성
        acheive_weekly_produce_heavy_doll,      // 중형인형제조 주간임무 달성
        acheive_weekly_produce_heavy_equip,     // 중형장비제조 주간임무 달성
        acheive_weekly_kill_armor_mech,         // 철혈장갑기계 주간임무 달성
        acheive_weekly_kill_armor_doll,         // 철혈장갑인형 주간임무 달성

        acheive_weekly_operation,               // 군수지원 주간임무 달성
        acheive_weekly_combat_sim,              // 모의작전 주간임무 달성
        acheive_weekly_skill_train,             // 스킬훈련 주간임무 달성
        acheive_weekly_data_analysis,           // 정보분석 주간임무 달성

        acheive_weekly_produce_doll,            // 인형제조 주간임무 달성
        acheive_weekly_produce_equip,           // 장비제조 주간임무 달성

        acheive_weekly_eat_gun,                 // 인형강화 주간임무 달성
        acheive_weekly_eat_equip,               // 장비강화 주간임무 달성
        acheive_weekly_eat_fairy,               // 요정강화 주간임무 달성
        acheive_weekly_eat_chip,                // 칩셋강화 주간임무 달성

        acheive_weekly_adjust_fairy,            // 요정교정 주간임무 달성
        acheive_weekly_adjust_equip,            // 장비교정 주간임무 달성

        /// <summary>
        /// 정보임무
        /// </summary>
        acheive_research_kill,                  // 아무 적 처치 정보임무 달성
        acheive_research_kill_boss,             // 철혈보스 처치 정보임무 달성
        acheive_research_kill_armor_doll,       // 철혈장갑인형 처치 정보임무 달성
        acheive_research_kill_armor_mech,       // 철혈장갑기계 처치 정보임무 달성
        acheive_research_kill_doll,             // 철혈인형 처치 정보임무 달성
        acheive_research_kill_mech,             // 철혈기계 처치 정보임무 달성

        acheive_research_mission,               // 전역승리 정보임무 달성
        acheive_research_mission_46,            // 4-6 전역승리 정보임무 달성
        acheive_research_mission_56,            // 5-6 전역승리 정보임무 달성
        acheive_research_mission_66,            // 6-6 전역승리 정보임무 달성
        acheive_research_mission_76,            // 7-6 전역승리 정보임무 달성

        acheive_research_mission_normal,        // 일반 전역승리 정보임무 달성
        acheive_research_mission_emergency,     // 긴급 전역승리 정보임무 달성
        acheive_research_mission_night,         // 야간 전역승리 정보임무 달성

        connect,                  // 연결

        test,                     // 테스트
        other,                    // 기타
        random,                   // 랜덤
    }

    /// <summary>
    /// 메시지 보이기/숨기기
    /// </summary>
    public enum MessageVisible
    {
        Show,
        Hide
    }

    /// <summary>
    /// 메시지 보내기 방식
    /// </summary>
    public enum MessageSend
    {
        Mail,
        Toast,
        Voice,
        All
    }

    public class Message
    {
        public MessageType type = MessageType.other;             // 메시지 종류
        public MessageSend send = MessageSend.All;               // 메시지 보내기 방식
        public int gunId = 0;                                    // 인형 ID (음성)
        public int skinId = 0;                                   // 스킨 ID (음성)
        public string voice = "";                                // 음성 종류
        public int delay = 0;                                    // 딜레이
        public string subject = "";                              // 제목
        public string content = "";                              // 내용

        /* Mail
         * =========================================
         * [소녀전선] 인형제조 (content1)
         * =========================================
         */

        /* Windows Toast
         * =========================================
         * 인형제조 (subject)
         * N성 인형 ㅇㅇ (AR) 제조완료 (content)
         * 소녀전선 알리미
         * =========================================
         */
    }
}
