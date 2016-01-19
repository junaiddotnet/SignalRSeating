using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SignalRSeating.Views.Home
{
    [HubName("planeHub")]
    public class PlaneHub:Hub
    {
        const string SessionId = "SessionId";
        static int userId;
        public static List<Models.PlaneSeatArrangment> allSeats = new List<Models.PlaneSeatArrangment>();
        //public static Dictionary<string,int> UserList = new Dictionary<string,int>();
        public static Dictionary<string, User> UserLists = new Dictionary<string, User>();
        public void selectSeat (int userId,int seatNumber)
        {
            Models.PlaneSeatArrangment planeSeat = new Models.PlaneSeatArrangment() {UserId=userId,SeatNumber=seatNumber };
           
            allSeats.Add(planeSeat);
            var retunData = Newtonsoft.Json.JsonConvert.SerializeObject(planeSeat);
            Clients.All.selectSeat(planeSeat.UserId,planeSeat.SeatNumber);
        }

        public void toggleSeat (int userId, int seatNumber)
        {
            Models.PlaneSeatArrangment mySeat = allSeats.Where(s=>s.SeatNumber == seatNumber).FirstOrDefault();
            var retunData = Newtonsoft.Json.JsonConvert.SerializeObject(mySeat);
            if (mySeat!=null && userId==mySeat.UserId)
            {
                allSeats.Remove(mySeat);
                Clients.All.toggleSeat(mySeat.SeatNumber);
            }
            else
            {
                Clients.Caller.alert(mySeat.UserId);
            }
           
        }
        public void createUser()
        {
             bool PageReload = false;
            User alreadyUser=null ;
            if (UserLists.Count()>0)
            {
                if (UserLists.ContainsKey(Context.QueryString[SessionId]))
                {
                    alreadyUser = UserLists [Context.QueryString[SessionId]];
                    UserLists[Context.QueryString[SessionId]] = new User() { userId = alreadyUser.userId, ConnectionId = Context.ConnectionId };
                    PageReload = true;
                }
            }
            if (PageReload==false)
            {
                userId++;
                UserLists.Add(Context.QueryString[SessionId], new User() {userId=userId,ConnectionId=Context.ConnectionId });
                //UserList.Add(Context.QueryString[SessionId], userId);
                Clients.Caller.createUser(userId);
            }
            else if (PageReload==true)
            {
                Clients.Caller.createUser(alreadyUser.userId);
            }
            
            
            
            
        }
        public void populateSeatData()
        {
            var occupiedSeats = Newtonsoft.Json.JsonConvert.SerializeObject(allSeats);
            Clients.Caller.populateSeatData(occupiedSeats);
        }

        public void sendMessage(int selectuser,int senderuser ,string message)
        {
            var SelectedUser = UserLists.Values.Where(a => a.userId == selectuser).FirstOrDefault();
            if (SelectedUser!=null)
            {
                Clients.Client(SelectedUser.ConnectionId).displayMessage(senderuser, message);
            }
            else
            {
                Clients.Others.displayMessage(senderuser, message);
            }
            
        }
        public void broadcastMessage(int senderuser, string message)
        {
            Clients.All.displayMessage(senderuser, message);

        }
        public override Task OnConnected()
        {
            
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
    }
}