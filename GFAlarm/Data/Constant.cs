using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFAlarm.Data
{
    public class Constant
    {
        public const long HOUR = 60 * 60 * 1000;
        public const long MINUTE = 60 * 1000;

        public const double ZOOM = 0.15;
    }

    public class PATH
    {
        // 메인화면 갱신
        public const string HOME = "Index/home";

        // 인형 장비 입기/벗기
        public const string GUN_EQUIP = "Equip/gunEquip";
        // {"if_outfit":0,"gun_with_user_id":345595000,"equip_with_user_id":88239725,"equip_slot":1}
        // {"if_outfit":1,"gun_with_user_id":345595000,"equip_with_user_id":88239725,"equip_slot":1}
    }
}
