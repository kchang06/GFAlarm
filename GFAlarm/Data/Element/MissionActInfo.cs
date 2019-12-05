using GFAlarm.Util;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFAlarm.Data.Element
{
    #region Packet Example
    /*
	"mission_act_info": {
		"gundie_num": "1",
		"enemydie_num_killbymyside": "0",
		"gunid_gunlev": "{\"20002\":\"120\",\"20055\":\"120\"}",
		"end_enemy_instance_id": "41",
		"spots_change": "[]",
		"type5_score": "0",
		"defend_line_start_turn": "0",
		"pass_missionevent": "0",
		"mission_id": "109",
		"spot_belong_remain_reset_count": "-1",
		"spot": "{\"8990\":{\"spot_id\":\"8990\",\"belong\":\"2\",\"ally_instance_ids\":[1],\"if_random\":\"0\",\"seed\":1223,\"team_id\":\"0\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_ai\":0,\"enemy_ai_para\":\"\",\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"enemy_birth_turn\":\"999\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"8991\":{\"spot_id\":\"8991\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":0,\"enemy_ai_para\":0,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":9107,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"8992\":{\"spot_id\":\"8992\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":0,\"enemy_ai_para\":0,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":469,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"8993\":{\"spot_id\":\"8993\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":0,\"enemy_ai_para\":0,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":7841,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"8994\":{\"spot_id\":\"8994\",\"belong\":\"2\",\"ally_instance_ids\":[2],\"if_random\":\"0\",\"seed\":7524,\"team_id\":\"0\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_ai\":0,\"enemy_ai_para\":\"\",\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"enemy_birth_turn\":\"999\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"8995\":{\"spot_id\":\"8995\",\"enemy_team_id\":\"2261\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"6\",\"enemy_birth_turn\":\"1\",\"enemy_ai\":\"0\",\"enemy_ai_para\":0,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":5043,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"8996\":{\"spot_id\":\"8996\",\"enemy_team_id\":\"2261\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"7\",\"enemy_birth_turn\":\"1\",\"enemy_ai\":\"0\",\"enemy_ai_para\":0,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":6729,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"8997\":{\"spot_id\":\"8997\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":0,\"enemy_ai_para\":0,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":1914,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"8998\":{\"spot_id\":\"8998\",\"belong\":\"2\",\"ally_instance_ids\":[3],\"if_random\":\"0\",\"seed\":5954,\"team_id\":\"0\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_ai\":0,\"enemy_ai_para\":\"\",\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"enemy_birth_turn\":\"999\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"8999\":{\"spot_id\":\"8999\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":0,\"enemy_ai_para\":0,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":7221,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9000\":{\"spot_id\":\"9000\",\"enemy_team_id\":\"2266\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"11\",\"enemy_birth_turn\":\"1\",\"enemy_ai\":\"101\",\"enemy_ai_para\":1,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":7286,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9001\":{\"spot_id\":\"9001\",\"enemy_team_id\":\"2266\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"12\",\"enemy_birth_turn\":\"1\",\"enemy_ai\":\"101\",\"enemy_ai_para\":1,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":1162,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9002\":{\"spot_id\":\"9002\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":0,\"enemy_ai_para\":0,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":7064,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9003\":{\"spot_id\":\"9003\",\"enemy_team_id\":\"2262\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"14\",\"enemy_birth_turn\":\"1\",\"enemy_ai\":\"101\",\"enemy_ai_para\":1,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":4113,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9004\":{\"spot_id\":\"9004\",\"belong\":\"2\",\"ally_instance_ids\":[4],\"if_random\":\"0\",\"seed\":5459,\"team_id\":\"0\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_ai\":0,\"enemy_ai_para\":\"\",\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"enemy_birth_turn\":\"999\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9005\":{\"spot_id\":\"9005\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":0,\"enemy_ai_para\":0,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":7463,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9006\":{\"spot_id\":\"9006\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":0,\"enemy_ai_para\":0,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":4688,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9007\":{\"spot_id\":\"9007\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":0,\"enemy_ai_para\":0,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":4867,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9008\":{\"spot_id\":\"9008\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":0,\"enemy_ai_para\":0,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":7868,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9009\":{\"spot_id\":\"9009\",\"enemy_team_id\":\"2266\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"20\",\"enemy_birth_turn\":\"1\",\"enemy_ai\":\"101\",\"enemy_ai_para\":1,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":3817,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9010\":{\"spot_id\":\"9010\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":0,\"enemy_ai_para\":0,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":6545,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9011\":{\"spot_id\":\"9011\",\"enemy_team_id\":\"2265\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"22\",\"enemy_birth_turn\":\"1\",\"enemy_ai\":\"101\",\"enemy_ai_para\":1,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":751,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9012\":{\"spot_id\":\"9012\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":0,\"enemy_ai_para\":0,\"belong\":\"3\",\"if_random\":\"0\",\"seed\":2771,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9013\":{\"spot_id\":\"9013\",\"enemy_team_id\":\"2265\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"24\",\"enemy_birth_turn\":\"1\",\"enemy_ai\":\"101\",\"enemy_ai_para\":1,\"belong\":\"3\",\"if_random\":\"0\",\"seed\":5786,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9014\":{\"spot_id\":\"9014\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":0,\"enemy_ai_para\":0,\"belong\":\"3\",\"if_random\":\"0\",\"seed\":3770,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9015\":{\"spot_id\":\"9015\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":0,\"enemy_ai_para\":0,\"belong\":\"3\",\"if_random\":\"0\",\"seed\":9615,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9019\":{\"spot_id\":\"9019\",\"enemy_team_id\":\"2259\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"27\",\"enemy_birth_turn\":\"1\",\"enemy_ai\":\"0\",\"enemy_ai_para\":0,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":4637,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9020\":{\"spot_id\":\"9020\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":\"0\",\"enemy_ai_para\":\"\",\"belong\":\"2\",\"if_random\":\"0\",\"seed\":736,\"team_id\":\"1\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9021\":{\"spot_id\":\"9021\",\"enemy_team_id\":\"2259\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"29\",\"enemy_birth_turn\":\"1\",\"enemy_ai\":\"0\",\"enemy_ai_para\":0,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":837,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9022\":{\"spot_id\":\"9022\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":0,\"enemy_ai_para\":0,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":3744,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9023\":{\"spot_id\":\"9023\",\"enemy_team_id\":\"2264\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"31\",\"enemy_birth_turn\":\"1\",\"enemy_ai\":\"101\",\"enemy_ai_para\":1,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":2120,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9024\":{\"spot_id\":\"9024\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":0,\"enemy_ai_para\":0,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":8678,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9025\":{\"spot_id\":\"9025\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":0,\"enemy_ai_para\":0,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":9973,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9026\":{\"spot_id\":\"9026\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":0,\"enemy_ai_para\":0,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":9644,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9027\":{\"spot_id\":\"9027\",\"belong\":\"2\",\"ally_instance_ids\":[5],\"if_random\":\"0\",\"seed\":6701,\"team_id\":\"0\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_ai\":0,\"enemy_ai_para\":\"\",\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"enemy_birth_turn\":\"999\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9028\":{\"spot_id\":\"9028\",\"enemy_team_id\":\"2327\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"36\",\"enemy_birth_turn\":\"1\",\"enemy_ai\":\"0\",\"enemy_ai_para\":0,\"belong\":\"3\",\"if_random\":\"0\",\"seed\":1557,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9029\":{\"spot_id\":\"9029\",\"enemy_team_id\":\"2258\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"37\",\"enemy_birth_turn\":\"1\",\"enemy_ai\":\"0\",\"enemy_ai_para\":0,\"belong\":\"3\",\"if_random\":\"0\",\"seed\":8277,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9377\":{\"spot_id\":\"9377\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":0,\"enemy_ai_para\":0,\"belong\":\"2\",\"if_random\":\"0\",\"seed\":2655,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9389\":{\"spot_id\":\"9389\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":0,\"enemy_ai_para\":0,\"belong\":\"3\",\"if_random\":\"0\",\"seed\":8778,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9390\":{\"spot_id\":\"9390\",\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_birth_turn\":\"999\",\"enemy_ai\":0,\"enemy_ai_para\":0,\"belong\":\"3\",\"if_random\":\"0\",\"seed\":5563,\"team_id\":\"0\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"reinforce_count\":\"0\",\"supply_count\":\"0\"},\"9016\":{\"spot_id\":\"9016\",\"team_id\":\"0\",\"belong\":\"1\",\"if_random\":\"0\",\"reinforce_count\":\"1\",\"seed\":3816,\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_ai\":0,\"enemy_ai_para\":\"\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"enemy_birth_turn\":\"999\",\"supply_count\":\"0\"},\"9018\":{\"spot_id\":\"9018\",\"team_id\":\"0\",\"belong\":\"1\",\"if_random\":\"0\",\"reinforce_count\":\"1\",\"seed\":5841,\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_ai\":0,\"enemy_ai_para\":\"\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"enemy_birth_turn\":\"999\",\"supply_count\":\"1\"},\"9017\":{\"spot_id\":\"9017\",\"team_id\":\"3\",\"belong\":\"1\",\"if_random\":\"0\",\"reinforce_count\":\"1\",\"seed\":9676,\"enemy_team_id\":\"0\",\"boss_hp\":\"0\",\"enemy_hp_percent\":\"1\",\"enemy_instance_id\":\"0\",\"enemy_ai\":0,\"enemy_ai_para\":\"\",\"ally_instance_ids\":[],\"squad_instance_ids\":[],\"hostage_id\":\"0\",\"hostage_hp\":\"0\",\"hostage_max_hp\":\"0\",\"enemy_birth_turn\":\"999\",\"supply_count\":\"0\"}}",
                "9016": {
                    "spot_id": "9016",
                    "team_id": "0",
                    "belong": "1",
                    "if_random": "0",
                    "reinforce_count": "1",
                    "seed": 3816,
                    "enemy_team_id": "0",
                    "boss_hp": "0",
                    "enemy_hp_percent": "1",
                    "enemy_instance_id": "0",
                    "enemy_ai": 0,
                    "enemy_ai_para": "",
                    "ally_instance_ids": [],
                    "squad_instance_ids": [],
                    "hostage_id": "0",
                    "hostage_hp": "0",
                    "hostage_max_hp": "0",
                    "enemy_birth_turn": "999",
                    "supply_count": "0"
                },
                "9017": {
                    "spot_id": "9017",
                    "team_id": "3",
                    "belong": "1",
                    "if_random": "0",
                    "reinforce_count": "1",
                    "seed": 9676,
                    "enemy_team_id": "0",
                    "boss_hp": "0",
                    "enemy_hp_percent": "1",
                    "enemy_instance_id": "0",
                    "enemy_ai": 0,
                    "enemy_ai_para": "",
                    "ally_instance_ids": [],
                    "squad_instance_ids": [],
                    "hostage_id": "0",
                    "hostage_hp": "0",
                    "hostage_max_hp": "0",
                    "enemy_birth_turn": "999",
                    "supply_count": "0"
                },
                "9020": {
                    "spot_id": "9020",
                    "enemy_team_id": "0",
                    "boss_hp": "0",
                    "enemy_hp_percent": "1",
                    "enemy_instance_id": "0",
                    "enemy_birth_turn": "999",
                    "enemy_ai": "0",
                    "enemy_ai_para": "",
                    "belong": "2",
                    "if_random": "0",
                    "seed": 736,
                    "team_id": "1",
                    "ally_instance_ids": [],
                    "squad_instance_ids": [],
                    "hostage_id": "0",
                    "hostage_hp": "0",
                    "hostage_max_hp": "0",
                    "reinforce_count": "0",
                    "supply_count": "0"
                },
		"user_id": "343650",
		"enemydie_num_killbyfriend": "0",
		"mysquad_die_num": "0",
		"ally_instance_info": "{\"1\":{\"ally_instance_id\":\"1\",\"ally_team_id\":\"2216\",\"ally_enemy\":\"2264\",\"ally_type\":\"1\",\"ai\":\"1\",\"ai_para\":\"\",\"ally_boss_hp\":\"0\",\"ally_enemy_hp_percent\":\"1\",\"team_guns_life\":{\"728\":{\"id\":\"728\",\"life\":\"970\"}},\"move_turn\":\"0\",\"transform_condition2\":\"\",\"betray_condition2\":\"\"},\"2\":{\"ally_instance_id\":\"2\",\"ally_team_id\":\"2224\",\"ally_enemy\":\"2263\",\"ally_type\":\"2\",\"ai\":\"101\",\"ai_para\":1,\"ally_boss_hp\":\"0\",\"ally_enemy_hp_percent\":\"1\",\"team_guns_life\":[],\"move_turn\":\"0\",\"transform_condition2\":\"\",\"betray_condition2\":\"\"},\"3\":{\"ally_instance_id\":\"3\",\"ally_team_id\":\"2217\",\"ally_enemy\":\"2264\",\"ally_type\":\"2\",\"ai\":\"102\",\"ai_para\":\"8998,1,8998-8999\",\"ally_boss_hp\":\"0\",\"ally_enemy_hp_percent\":\"1\",\"team_guns_life\":[],\"move_turn\":\"0\",\"transform_condition2\":\"\",\"betray_condition2\":\"\"},\"4\":{\"ally_instance_id\":\"4\",\"ally_team_id\":\"2215\",\"ally_enemy\":\"2264\",\"ally_type\":\"2\",\"ai\":\"102\",\"ai_para\":\"9004,1,9004-8991\",\"ally_boss_hp\":\"0\",\"ally_enemy_hp_percent\":\"1\",\"team_guns_life\":[],\"move_turn\":\"0\",\"transform_condition2\":\"\",\"betray_condition2\":\"\"},\"5\":{\"ally_instance_id\":\"5\",\"ally_team_id\":\"2224\",\"ally_enemy\":\"2263\",\"ally_type\":\"2\",\"ai\":\"101\",\"ai_para\":1,\"ally_boss_hp\":\"0\",\"ally_enemy_hp_percent\":\"1\",\"team_guns_life\":[],\"move_turn\":\"0\",\"transform_condition2\":\"\",\"betray_condition2\":\"\"}}",
		"gun_average_level": "",
		"friend_team_fairy": "[]",
		"growenemy_with_enemydie": "0",
		"reinforce_ally_team": "[]",
		"enemydie_num": "1",
		"reinforce_squads": "[]",
		"ap": "5",
		"enemydie_this_turn": "[]",
		"lose_type": "1",
		"is_win": "0",
		"turn": "1",
		"join_teams": "1,3",
		"squad_info": "[]",
		"mission_control_trigger_spot_id": "[]",
		"enter_assisttype_squads": "[]",
		"get_guns": "",
		"fairy_skill_perform": "[]",
		"mission_control": "{\"1\":1,\"2\":2,\"3\":3}",
		"gunid_gunlv": "",
		"last_battle_finish_time": "1564650281",
		"last_battle_info": "",
		"team_effect": "",
		"fairy_skill_on_team": "[]",
		"battle_count": "1",
		"fairy_skill_on_enemy": "[]",
		"end_squad_instance_id": "1",
		"turn_belong": "1",
		"end_skill_id": "0",
		"building_info": "[]",
		"fairy_skill_on_spot": "[]",
		"save_hostage": "0",
		"enemydie_tigger_team_colour": "[\"2260\"]",
		"enemydie_num_killbyhostage": "0",
		"end_ally_instance_id": "6",
		"reinforce_teams": "1,2,3",
		"fairy_skill_on_squad": "[]",
		"boss_hp": "0",
		"boss_max_hp": "0"
	},
     */
    #endregion

    public class MissionActInfo
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        // 전역 ID
        private int _missionId = 0;
        public int missionId
        {
            get
            {
                return _missionId;
            }
            set
            {
                JObject data = GameData.Mission.GetData(value);
                if (data != null)
                {
                    this.missionLocation = Parser.Json.ParseString(data["location"]);
                }
                _missionId = value;
            }
        }
        // 전역 이름
        public string missionLocation = "";
        // 참여제대
        public List<int> joinTeams = new List<int>();
        /// <summary>
        /// 제대위치정보
        /// team_id, spot_id
        /// </summary>
        public Dictionary<int, int> teamSpots = new Dictionary<int, int>();
        // 현재 턴
        public int turn = 0;

        public MissionActInfo(dynamic json)
        {
            try
            {
                if (json != null && json is JObject)
                {
                    if (json.ContainsKey("mission_id"))
                        this.missionId = Parser.Json.ParseInt(json["mission_id"]);
                    // 참여제대
                    if (json.ContainsKey("join_teams"))
                    {
                        string joinTeamsString = Parser.Json.ParseString(json["join_teams"]);
                        string[] joinTeams = joinTeamsString.Split(',');
                        foreach (string joinTeam in joinTeams)
                        {
                            this.joinTeams.Add(Parser.String.ParseInt(joinTeam));
                        }
                    }
                    // 제대위치정보
                    if (json.ContainsKey("spot"))
                    {
                        Dictionary<string, string> spots = Parser.Json.ParseItems(Parser.Json.ParseString(json["spot"]));
                        foreach (KeyValuePair<string, string> item in spots)
                        {
                            try
                            {
                                JObject spot = Parser.Json.ParseJObject(item.Value);
                                int teamId = Parser.Json.ParseInt(spot["team_id"]);
                                int spotId = Parser.Json.ParseInt(spot["spot_id"]);
                                if (teamId > 0)
                                {
                                    // team_id, spot_id
                                    teamSpots.Add(teamId, spotId);
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }
                        }
                    }
                    // 현재 턴
                    if (json.ContainsKey("turn"))
                    {
                        this.turn = Parser.Json.ParseInt(json["turn"]);
                    }
                }
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
