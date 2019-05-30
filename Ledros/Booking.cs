using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ledros.Json;
using StringDict = System.Collections.Generic.Dictionary<string, string>;
using SODict = System.Collections.Generic.Dictionary<string, object>;
using MMC = MementoConnection.MMConnection;

namespace Ledros
{
    public class Booking
    {
        public static string Sow(string data)
        {
            JsonBookingSpark.RootObject bs = null;
            try
            {
                 bs = JsonConvert.DeserializeObject<JsonBookingSpark.RootObject>(data);
            }
            catch
            {
                return data.Contains("BubbleBooking") ? "DataError" : null;
            }
            return Harvest(bs);
        }
        
        static string Harvest(JsonBookingSpark.RootObject root)
        {
            switch (root.Channel)
            {
                case "Ctrip":
                    return GetCtripBooking(root);
                default:
                    return null;
            }
        }

        static string GetCtripBooking(JsonBookingSpark.RootObject root)
        {
            // 获取包含订单Token值的订单列表，默认获取只含指定订单的列表
            string bsString = NetRequest.Post(
                    HeartReader.GetPostPort(root.Channel, "OrderToken"),
                    root.Cookie,
                    HeartReader.GetPostParam(root.Channel, "OrderToken")
                        .Replace("ResNumber", root.ResNumber));
            // bc为Ctrip订单列表的Json数据的实体类
            var bc = JsonConvert.DeserializeObject
                <JsonCtripFilteredBookingCollection.RootObject>(bsString);
            string bcString = NetRequest.Post(
                HeartReader.GetPostPort(root.Channel, "Order"),
                root.Cookie,
                HeartReader.GetPostParam(root.Channel, "Order")
                    .Replace("ResKey", bc.Data[0].OrderKey)
                    .Replace("ResToken", bc.Data[0].Token));
            // booking为Ctrip订单详情的Json数据的实体类
            var booking = JsonConvert.DeserializeObject<JsonCtripBooking.RootObject>(bcString);
            long? uid = MMC.GetUIDByFullName(booking.Data.UserInfo["FullName"] as string);
            if (uid is null)// 不存在同样的用户姓名则创建
                uid = MMC.InsertUser(booking.Data.UserInfo);
            // 新用户创建不成功则不予保存订单
            if (uid < 0) return null;
            SODict resInfo = booking.Data.ResInfo;
            resInfo.Add("uid", uid);
            // 保存订单信息，如果失败则不予继续保存
            if (!MMC.InsertRes(resInfo)) return null;
            // 保存所有订房信息，如果存在失败，则删除此订单
            foreach (SODict roomInfo in booking.RoomInfos)
            {
                roomInfo.Add("uid", uid);
                if (!MMC.InsertResRoom(roomInfo))
                {
                    MMC.DeleteResByResNumber(booking.Data.ResInfo["ResNumber"] as string);
                    return null;
                }
            }
            return resInfo["ResNumber"] as string;
        }
    }
}
