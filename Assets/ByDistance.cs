using System;
using System.Collections.Generic;
public class ByDistance: IComparer<Room>
{
	public ByDistance()
	{

	}
    public int Compare(Room r1, Room r2)
    {
        if(r1.Distance == Room.INF && r2.Distance == Room.INF)
        {
            return 0;
        }
        else if(r1.Distance == Room.INF)
        {
            return 1;
        }
        else if (r2.Distance == Room.INF)
        {
            return -1;
        }
        else
        {
            // can't do something like return (int)(r1.distance - r2.distance) because that would cast off
            // fractional differences and so distances that varied by .5 would be marked equal
            if(r1.Distance > r2.Distance)
            {
                return 1;
            }
            else if(r1.Distance == r2.Distance)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}
