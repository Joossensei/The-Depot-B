public class Tours {  

        public static DateTime now = DateTime.Now;
        public static Tour[] tours = 
        {

            
            new (){
            dateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 20, 0)
            },
            new(){
                dateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 40, 0)
            },
            new(){
                dateTime = new DateTime(now.Year, now.Month, now.Day, now.AddHours(1).Hour, 0, 0)
            }
        };
}