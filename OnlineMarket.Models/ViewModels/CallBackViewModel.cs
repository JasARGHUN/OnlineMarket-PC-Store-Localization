using System.Collections.Generic;

namespace OnlineMarket.Models.ViewModels
{
    public class CallBackViewModel
    {
        public int Id { get; set; }

        public CallBack CallBack { get; set; }
        public IEnumerable<CallBack> CallBacks { get; set; }
    }
}
