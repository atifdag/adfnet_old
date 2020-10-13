using System;

namespace Adfnet.Core.Constants
{
    public static class RoleConstants
    {
        public static Tuple<string, string, int> Developer => new Tuple<string, string, int>("Developer", "Geliştirici",1);
        public static Tuple<string, string, int> Manager => new Tuple<string, string, int>("Manager", "Yönetici", 10);
        public static Tuple<string, string, int> Editor => new Tuple<string, string, int>("Editör", "Kullanıcı", 20);
        public static Tuple<string, string, int> Subscriber => new Tuple<string, string, int>("Subscriber", "Abone Kullanıcı", 1000);
        public static Tuple<string, string, int> Default => new Tuple<string, string, int>("Default", "Varsayılan Kullanıcı", 10000);

    }
}
