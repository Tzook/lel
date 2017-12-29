using UnityEngine;
using System.Collections;

public class LocalUserInfo
{
    public static User Me
    {
        get
        {
            if ( me == null )
            {
                me = new User();
            }

            return me;
        }

        protected set
        {
            me = value;
        }
    }

    protected static User me;

    public static void DisposeCurrentUser()
    {
        me = null;
    }
}
