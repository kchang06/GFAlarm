using GFAlarm.Data;
using GFAlarm.Util;
using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Threading;
using System.Windows.Media;

namespace GFAlarm.Notifier
{
    public static class Manager
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        // 트레이 알림 소리
        private static SoundPlayer soundPlayer = new SoundPlayer();

        // 타이머
        private static Timer timer = new Timer(Tick, null, 0, 1000);

        // 메시지 큐
        // (쌓인 알림은 순차적으로 전달된다.)
        public static Queue<Message> notifyQueue = new Queue<Message>();

        /// <summary>
        /// 타이머
        /// </summary>
        /// <param name="state"></param>
        private static void Tick(object state)
        {
            if (notifyQueue.Count() > 0)
            {
                Message msg = notifyQueue.Dequeue();
                if (msg.delay > 0)
                {
                    Thread.Sleep(msg.delay);
                }
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer = new Timer(Tick, null, 3000, 1000);
                Notify(msg);
            }
        }

        /// <summary>
        /// 통합 알림
        /// </summary>
        /// <param name="msg"></param>
        private static void Notify(Message msg)
        {
            switch(msg.send)
            {
                // 모든 알림
                case MessageSend.All:
                    if (Config.Setting.winToast)
                        ShowWinToast(msg);
                    if (Config.Setting.mailNotification)
                        SendMail(msg);
                    break;
                // 윈도우 토스트
                case MessageSend.Toast:
                    if (Config.Setting.winToast)
                        ShowWinToast(msg);
                    break;
                // 메일 알림
                case MessageSend.Mail:
                    if (Config.Setting.mailNotification)
                        SendMail(msg);
                    break;
                // 음성 알림
                case MessageSend.Voice:
                    if (Config.Setting.voiceNotification)
                        PlayVoice(msg);
                    break;
            }
        }

        /// <summary>
        /// 윈도우 토스트 보여주기
        /// </summary>
        /// <param name="msg"></param>
        private static void ShowWinToast(Message msg)
        {
            try
            {
                // 별도 알림 소리
                if (Config.Setting.voiceNotification)
                {
                    PlayVoice(msg);
                }

                if (Os.isWin10)
                {
                    // Windows 10
                    Toast.ShowToast(msg, Config.Setting.voiceNotification, Config.Setting.strongWinToast);
                }
                else 
                {
                    // Other Windows
                    MainWindow.view.ShowTrayTip(msg);
                }
            }
            catch (Exception ex)
            {
                MainWindow.view.ShowTrayTip(msg);
                log.Error(ex);
            }
            
        }

        /// <summary>
        /// 메일 보내기
        /// </summary>
        /// <param name="msg"></param>
        private static void SendMail(Message msg)
        {
            Mail.Send(msg);
        }

        private static string GetVoiceFile(int gunId, int skinId = 0, MessageType msgType = MessageType.other)
        {
            string filename = "";

            string path = string.Format("{0}", gunId);

            // 아동절 스킨 여부
            List<int> childSkinIds = GameData.Doll.GetDollSkin(false, true);
            if (childSkinIds.Contains(skinId))
            {
                path += "_pedo";
            }

            string voice = "";
            switch (msgType)
            {
                // 일간임무
                case MessageType.acheive_daily_combat_sim:              // 모의작전 임무 달성
                    voice = "acheive_combat_sim";
                    break;
                case MessageType.acheive_daily_data_analysis:           // 정보분석 임무 달성
                    voice = "acheive_data_analysis";
                    break;
                case MessageType.acheive_daily_eat_doll:                // 인형강화 임무 달성
                    voice = "acheive_eat_doll";
                    break;
                case MessageType.acheive_daily_eat_equip:               // 장비강화 임무 달성
                    voice = "acheive_eat_equip";
                    break;
                case MessageType.acheive_daily_fix_doll:                // 인형수복 임무 달성
                    voice = "acheive_fix_doll";
                    break;
                case MessageType.acheive_daily_get_battery:             // 공유전지 임무 달성
                    voice = "acheive_get_battery";
                    break;
                case MessageType.acheive_daily_operation:               // 군수지원 임무 달성
                    voice = "acheive_operation";
                    break;
                case MessageType.acheive_daily_produce_doll:            // 인형제조 임무 달성
                    voice = "acheive_produce_doll";
                    break;
                case MessageType.acheive_daily_produce_equip:           // 장지제조 임무 달성
                    voice = "acheive_produce_equip";
                    break;
                case MessageType.acheive_daily_reinforce:               // 지원제대 임무 달성
                    voice = "acheive_reinforce";
                    break;
                case MessageType.acheive_daily_win:                     // 전역승리 임무 달성
                    voice = "acheive_win";
                    break;

                /// 주간임무
                case MessageType.acheive_weekly_s_battle:                // S랭크 전투 주간임무 달성
                    voice = "acheive_weekly_s_battle";
                    break;
                case MessageType.acheive_weekly_fix_gun:                 // 인형수복 주간임무 달성
                    voice = "acheive_weekly_fix_gun";
                    break;
                case MessageType.acheive_weekly_kill_mech:               // 철혈기계 처치 주간임무 달성
                    voice = "acheive_weekly_kill_mech";
                    break;
                case MessageType.acheive_weekly_kill_doll:               // 철혈인형 처치 주간임무 달성
                    voice = "acheive_weekly_kill_doll";
                    break;
                case MessageType.acheive_weekly_kill_boss:               // 철혈보스 처치 주간임무 달성
                    voice = "acheive_weekly_kill_boss";
                    break;
                case MessageType.acheive_weekly_produce_heavy_doll:      // 중형인형제조 주간임무 달성
                    voice = "acheive_weekly_produce_heavy_doll";
                    break;
                case MessageType.acheive_weekly_produce_heavy_equip:     // 중형장비제조 주간임무 달성
                    voice = "acheive_weekly_produce_heavy_equip";
                    break;
                case MessageType.acheive_weekly_kill_armor_mech:         // 철혈장갑기계 주간임무 달성
                    voice = "acheive_weekly_kill_armor_mech";
                    break;
                case MessageType.acheive_weekly_kill_armor_doll:         // 철혈장갑인형 주간임무 달성
                    voice = "acheive_weekly_kill_armor_doll";
                    break;
                case MessageType.acheive_weekly_operation:               // 군수지원 주간임무 달성
                    voice = "acheive_weekly_operation";
                    break;
                case MessageType.acheive_weekly_combat_sim:              // 모의작전 주간임무 달성
                    voice = "acheive_weekly_combat_sim";
                    break;
                case MessageType.acheive_weekly_skill_train:             // 스킬훈련 주간임무 달성
                    voice = "acheive_weekly_skill_train";
                    break;
                case MessageType.acheive_weekly_data_analysis:           // 정보분석 주간임무 달성
                    voice = "acheive_weekly_data_analysis";
                    break;
                case MessageType.acheive_weekly_produce_doll:            // 인형제조 주간임무 달성
                    voice = "acheive_weekly_produce_doll";
                    break;
                case MessageType.acheive_weekly_produce_equip:           // 장비제조 주간임무 달성
                    voice = "acheive_weekly_produce_equip";
                    break;
                case MessageType.acheive_weekly_eat_gun:                 // 인형강화 주간임무 달성
                    voice = "acheive_weekly_eat_gun";
                    break;
                case MessageType.acheive_weekly_eat_equip:               // 장비강화 주간임무 달성
                    voice = "acheive_weekly_eat_equip";
                    break;
                case MessageType.acheive_weekly_eat_fairy:               // 요정강화 주간임무 달성
                    voice = "acheive_weekly_eat_fairy";
                    break;
                case MessageType.acheive_weekly_eat_chip:                // 칩셋강화 주간임무 달성
                    voice = "acheive_weekly_eat_chip";
                    break;
                case MessageType.acheive_weekly_adjust_fairy:            // 요정교정 주간임무 달성
                    voice = "acheive_weekly_adjust_fairy";
                    break;
                case MessageType.acheive_weekly_adjust_equip:            // 장비교정 주간임무 달성
                    voice = "acheive_weekly_adjust_equip";
                    break;

                /// 정보임무
                case MessageType.acheive_research_kill:                  // 아무 적 처치 정보임무 달성
                    voice = "acheive_research_kill";
                    break;
                case MessageType.acheive_research_kill_boss:             // 철혈보스 처치 정보임무 달성
                    voice = "acheive_research_kill_boss";
                    break;
                case MessageType.acheive_research_kill_armor_doll:       // 철혈장갑인형 처치 정보임무 달성
                    voice = "acheive_research_kill_armor_doll";
                    break;
                case MessageType.acheive_research_kill_armor_mech:       // 철혈장갑기계 처치 정보임무 달성
                    voice = "acheive_research_kill_armor_mech";
                    break;
                case MessageType.acheive_research_kill_doll:             // 철혈인형 처치 정보임무 달성
                    voice = "acheive_research_kill_doll";
                    break;
                case MessageType.acheive_research_kill_mech:             // 철혈기계 처치 정보임무 달성
                    voice = "acheive_research_kill_mech";
                    break;
                case MessageType.acheive_research_mission:               // 전역승리 정보임무 달성
                    voice = "acheive_research_mission";
                    break;
                case MessageType.acheive_research_mission_46:            // 4-6 전역승리 정보임무 달성
                    voice = "acheive_research_mission_46";
                    break;
                case MessageType.acheive_research_mission_56:            // 5-6 전역승리 정보임무 달성
                    voice = "acheive_research_mission_56";
                    break;
                case MessageType.acheive_research_mission_66:            // 6-6 전역승리 정보임무 달성
                    voice = "acheive_research_mission_66";
                    break;
                case MessageType.acheive_research_mission_76:            // 7-6 전역승리 정보임무 달성
                    voice = "acheive_research_mission_76";
                    break;
                case MessageType.acheive_research_mission_normal:        // 일반 전역승리 정보임무 달성
                    voice = "acheive_research_mission_normal";
                    break;
                case MessageType.acheive_research_mission_emergency:     // 긴급 전역승리 정보임무 달성
                    voice = "acheive_research_mission_emergency";
                    break;
                case MessageType.acheive_research_mission_night:         // 야간 전역승리 정보임무 달성
                    voice = "acheive_research_mission_night";
                    break;

                /// 알림
                case MessageType.start_auto_mission:
                    voice = "start_auto_mission";
                    break;
                case MessageType.start_operation:
                    voice = "start_operation";
                    break;
                case MessageType.complete_auto_mission:
                    voice = "complete_auto_mission";
                    break;
                case MessageType.complete_battle_report:
                    voice = "complete_battle_report";
                    break;
                case MessageType.complete_data_analysis:
                    voice = "complete_data_analysis";
                    break;
                case MessageType.complete_explore:
                    voice = "complete_explore";
                    break;
                case MessageType.complete_operation:
                    voice = "complete_operation";
                    break;
                case MessageType.complete_produce_doll:
                    voice = "complete_produce_doll";
                    break;
                case MessageType.complete_produce_equip:
                    voice = "complete_produce_equip";
                    break;
                case MessageType.complete_restore_doll:
                    voice = "complete_restore_doll";
                    break;
                case MessageType.complete_skill_train:
                    voice = "complete_skill_train";
                    break;
                case MessageType.complete_exp_train:
                    voice = "complete_exp_train";
                    break;
                case MessageType.doll_max_level:
                    voice = "doll_max_level";
                    break;
                case MessageType.doll_need_expand:
                    voice = "doll_need_expand";
                    break;
                case MessageType.doll_wounded:
                    voice = "doll_wounded";
                    break;
                case MessageType.mission_get_doll:
                    voice = "mission_get_doll";
                    break;
                case MessageType.mission_get_equip:
                    voice = "mission_get_equip";
                    break;
                case MessageType.mission_move_finish:
                    voice = "mission_move_finish";
                    break;
                case MessageType.mission_win:
                    voice = "mission_win";
                    break;
                case MessageType.reach_max_bp_point:
                    voice = "reach_max_bp_point";
                    break;
                case MessageType.reach_max_doll:
                    voice = "reach_max_doll";
                    break;
                case MessageType.reach_max_equip:
                    voice = "reach_max_equip";
                    break;
                case MessageType.reach_max_global_exp:
                    voice = "reach_max_global_exp";
                    break;

                case MessageType.connect:
                    voice = "connect";
                    break;
                case MessageType.test:
                    voice = "complete_operation";
                    break;
                case MessageType.random:
                    string tempPath = "";
                    if (gunId == 0)
                    {
                        tempPath = string.Format("{0}\\Resource\\Sound\\{1}",
                            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Config.Setting.language);
                    }
                    else
                    {
                        tempPath = string.Format("{0}\\Resource\\Sound\\Voice\\{1}",
                            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
                    }
                    string[] file = Directory.GetFiles(tempPath, "*.wav", SearchOption.TopDirectoryOnly);
                    if (file.Length > 0)
                    {
                        int randIdx = new Random().Next(0, file.Length);
                        voice = file[randIdx];
                        return voice;
                    }
                    else
                    {
                        voice = "notify";
                    }
                    break;
                default:
                    voice = "notify";
                    break;
            }

            if (gunId == 0)
            {
                filename = string.Format("{0}\\Resource\\Sound\\{2}\\{1}.wav", 
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), voice, Config.Setting.language);
            }
            else
            {
                filename = string.Format("{0}\\Resource\\Sound\\Voice\\{1}\\{2}.wav", 
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path, voice);
            }

            return filename;
        }

        /// <summary>
        /// 음성 재생
        /// </summary>
        /// <param name="msg"></param>
        private static void PlayVoice(Message msg)
        {
            int gunId = msg.gunId;
            int skinId = msg.skinId;
            MessageType msgType = msg.type;
            if (Config.Setting.startVoice == false)
            {
                switch (msgType)
                {
                    case MessageType.start_auto_mission:
                    case MessageType.start_operation:
                        return;
                }
            }

            // 부관 음성만
            if (Config.Setting.adjutantVoiceOnly || gunId == 0)
            {
                gunId = UserData.adjutantDoll;
                skinId = UserData.adjutantDollSkin;
            }

            string filename = GetVoiceFile(gunId, skinId, msgType);
            if (!File.Exists(filename))
            {
                /// 개조 음성이 없는 경우
                if (gunId > 20000)
                {
                    gunId = gunId % 20000;
                }
                filename = GetVoiceFile(gunId, skinId, msgType);
                /// 인형 음성이 없는 경우
                if (!File.Exists(filename))
                {
                    filename = GetVoiceFile(0, 0, msgType);
                    /// TTS 음성도 없는 경우
                    if (!File.Exists(filename))
                    {
                        filename = string.Format("{0}\\Resource\\Sound\\notify.wav", 
                            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                        // 출발 알림 제외
                        switch (msgType)
                        {
                            case MessageType.start_auto_mission:
                            case MessageType.start_operation:
                                return;
                        }
                    }
                }
            }

            MainWindow.view.VolumeMediaPlayer(Config.Setting.voiceVolume);

            if (File.Exists(filename))
            {
                if (Config.Setting.useSoundPlayerApi)
                {
                    log.Debug("SoundPlayer 파일 재생 {0}", filename);
                    soundPlayer.SoundLocation = filename;
                    soundPlayer.Play();
                }
                else
                {
                    log.Debug("MediaPlayer 파일 재생 {0}", filename);
                    MainWindow.view.OpenMediaPlayer(filename);
                    MainWindow.view.PlayMediaPlayer();
                }
            }
        }
    }
}
