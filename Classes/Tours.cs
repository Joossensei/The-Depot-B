using System;
using System.IO;
using Newtonsoft.Json;

public class Tours
{   

    public static DateTime now = DateTime.Now;
    public Tour[] tours;

    public Tours()
    {
    }

    private void LoadTours()
    {

        //hierin kan nog wat uitgevoerd

        tours = new Tour[] {
            new() {
                dateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 20, 0)
            },
            new() {
                dateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 40, 0)
            },
            new() {
                dateTime = new DateTime(now.Year, now.Month, now.Day, now.AddHours(1).Hour, 0, 0)
            }
        };
    }

    public Tour[] GetTours()
    {
        if (tours == null)
        {
            LoadTours();
        }

        return tours;
    }
}
