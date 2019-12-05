namespace GFAlarm.Transaction
{
    public class GFPacket
    {
        public long req_id = 0;
        //public int type = 0; // 1: request, 2: response
        public string uri = "";
        public string outdatacode = "";
        public string body = "";
    }
}
